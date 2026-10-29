using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.TaxInvoice
{
    public sealed class IndexEntry
    {
        public string SellerTaxCode { get; set; } = string.Empty;
        public string InvoiceTemplate { get; set; } = string.Empty;
        public string InvoiceSeries { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? CreatedDateText { get; set; }
        public string FileStem { get; set; } = string.Empty;
        public string DetailStatus { get; set; } = "not-requested";
        public string XmlStatus { get; set; } = "not-requested";
        public string? ErrorMessage { get; set; }
    }

    public sealed class MonthLog
    {
        public string TaxCode { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset FinishedAt { get; set; }
        public int InvoiceCount { get; set; }
        public Dictionary<string, int> PagesBySource { get; set; } = new Dictionary<string, int>();
        public List<string> Errors { get; set; } = new List<string>();
        public bool Completed { get; set; }
    }

    public sealed class DownloadResult
    {
        public List<string> MonthFolders { get; } = new List<string>();
        public int InvoiceCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }

    public sealed class TaxInvoiceDownloader
    {
        private readonly TaxInvoiceClient _client;
        private readonly TaxInvoiceFileStore _store;

        public TaxInvoiceDownloader(TaxInvoiceClient client, TaxInvoiceFileStore store)
        {
            _client = client;
            _store = store;
        }

        public async Task<DownloadResult> DownloadAsync(DownloadRequest request, IProgress<DownloadProgress>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.TaxCode))
            {
                throw new ArgumentException("Tax code cannot be empty.");
            }
            if (_client.Token == null)
            {
                throw new InvalidOperationException("Client is not authenticated. Token is null.");
            }

            var result = new DownloadResult();
            var monthRanges = DateRangeHelper.SplitByMonth(request.FromDate, request.ToDate);

            foreach (var direction in request.Directions)
            {
                foreach (var month in monthRanges)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await DownloadMonthAsync(request, direction, month, result, progress, cancellationToken).ConfigureAwait(false);
                }
            }

            progress?.Report(new DownloadProgress("done", $"Completed: {result.InvoiceCount} invoices, {result.ErrorCount} errors.", result.InvoiceCount, result.InvoiceCount));
            return result;
        }

        private async Task DownloadMonthAsync(
            DownloadRequest request,
            InvoiceDirection direction,
            DateRange monthRange,
            DownloadResult result,
            IProgress<DownloadProgress>? progress,
            CancellationToken cancellationToken)
        {
            var folder = _store.GetMonthFolder(request.TaxCode, direction, monthRange);
            var directionLabel = direction == InvoiceDirection.Purchase ? "Purchase" : "Sold";
            var monthLog = new MonthLog
            {
                TaxCode = request.TaxCode,
                Direction = direction == InvoiceDirection.Purchase ? "purchase" : "sold",
                FromDate = monthRange.FromDate.ToString("yyyy-MM-dd"),
                ToDate = monthRange.ToDate.ToString("yyyy-MM-dd"),
                StartedAt = DateTimeOffset.Now,
            };
            var entries = new List<IndexEntry>();
            result.MonthFolders.Add(folder);

            void RecordError(string errorMsg, IndexEntry? entry = null)
            {
                monthLog.Errors.Add(errorMsg);
                result.Errors.Add($"{monthRange.MonthLabel} {directionLabel}: {errorMsg}");
                result.ErrorCount++;
                if (entry != null) entry.ErrorMessage = errorMsg;
            }

            void ClearError(IndexEntry entry)
            {
                monthLog.Errors.RemoveAll(e => e.Contains(entry.FileStem));
                var match = result.Errors.Find(e => e.Contains(entry.FileStem));
                if (match != null)
                {
                    result.Errors.Remove(match);
                    result.ErrorCount = Math.Max(0, result.ErrorCount - 1);
                }
                entry.ErrorMessage = null;
            }

            try
            {
                // 1. Fetch Invoices List
                var sources = request.IncludeSco ? new[] { InvoiceSource.Query, InvoiceSource.Sco } : new[] { InvoiceSource.Query };
                var uniqueKeys = new HashSet<InvoiceKey>();

                foreach (var source in sources)
                {
                    _store.ClearListPages(folder, source);
                    string? stateToken = null;
                    var pageNumber = 0;

                    try
                    {
                        do
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var page = await _client.GetInvoicePageAsync(
                                direction,
                                source,
                                monthRange.FromDate,
                                monthRange.ToDate,
                                request.InvoiceStatusFilter,
                                request.ProcessStatusFilter,
                                stateToken,
                                cancellationToken).ConfigureAwait(false);

                            pageNumber++;
                            _store.SaveListPage(folder, source, pageNumber, page.RawJson);

                            foreach (var item in page.Items)
                            {
                                if (uniqueKeys.Add(item.Key))
                                {
                                    entries.Add(new IndexEntry
                                    {
                                        SellerTaxCode = item.Key.SellerTaxCode,
                                        InvoiceTemplate = item.Key.InvoiceTemplate,
                                        InvoiceSeries = item.Key.InvoiceSeries,
                                        InvoiceNo = item.Key.InvoiceNo,
                                        Source = TaxInvoiceFileStore.GetSourceTag(source),
                                        CreatedDateText = item.CreatedDateText,
                                        FileStem = item.Key.FileStem,
                                    });
                                }
                            }

                            progress?.Report(new DownloadProgress("list", $"{directionLabel} {monthRange} ({source}) page {pageNumber}: {page.Items.Count} invoices"));
                            stateToken = page.NextState;
                        } while (stateToken != null);
                    }
                    catch (TaxInvoiceException ex) when (ex.StatusCode != 401)
                    {
                        RecordError($"List query error ({source}) page {pageNumber + 1}: {ex.Message}");
                    }

                    monthLog.PagesBySource[TaxInvoiceFileStore.GetSourceTag(source)] = pageNumber;
                }

                monthLog.InvoiceCount = entries.Count;
                result.InvoiceCount += entries.Count;

                // 2. Fetch Detail Line Items
                if (request.DownloadDetail)
                {
                    var index = 0;
                    var failedDetailEntries = new List<IndexEntry>();
                    foreach (var entry in entries)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        index++;
                        var key = ToInvoiceKey(entry);

                        if (request.SkipExistingDetail && File.Exists(_store.GetDetailPath(folder, key)))
                        {
                            entry.DetailStatus = "skipped";
                            continue;
                        }

                        try
                        {
                            var detailJson = await _client.GetDetailJsonAsync(key, cancellationToken).ConfigureAwait(false);
                            _store.SaveDetail(folder, key, detailJson);
                            entry.DetailStatus = "ok";
                        }
                        catch (TaxInvoiceException ex) when (ex.StatusCode != 401)
                        {
                            entry.DetailStatus = "error";
                            RecordError($"Detail error for {key.FileStem}: {ex.Message}", entry);
                            failedDetailEntries.Add(entry);
                        }

                        progress?.Report(new DownloadProgress("detail", $"{directionLabel} {monthRange.MonthLabel} {key.InvoiceSeries}-{key.InvoiceNo}", index, entries.Count));
                    }

                    // Recovery Pass: Tự động phục hồi các chi tiết bị gián đoạn do nghẽn mạng / HTTP 429
                    if (failedDetailEntries.Count > 0)
                    {
                        progress?.Report(new DownloadProgress("recovery", $"Đang phục hồi chi tiết (Recovery Pass) cho {failedDetailEntries.Count} hóa đơn bị lỗi mạng..."));
                        await Task.Delay(1500, cancellationToken).ConfigureAwait(false);

                        foreach (var entry in failedDetailEntries)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var key = ToInvoiceKey(entry);
                            try
                            {
                                var detailJson = await _client.GetDetailJsonAsync(key, cancellationToken).ConfigureAwait(false);
                                _store.SaveDetail(folder, key, detailJson);
                                entry.DetailStatus = "ok";
                                ClearError(entry);
                                progress?.Report(new DownloadProgress("recovery", $"[Phục hồi chi tiết thành công] {directionLabel} {key.InvoiceSeries}-{key.InvoiceNo}"));
                            }
                            catch
                            {
                                // Giữ nguyên trạng thái nếu lần 2 vẫn không kết nối được
                            }
                        }
                    }
                }

                // 3. Fetch XML Zip Archive
                if (request.DownloadXml)
                {
                    var index = 0;
                    var failedXmlEntries = new List<IndexEntry>();
                    foreach (var entry in entries)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        index++;
                        var key = ToInvoiceKey(entry);

                        if (File.Exists(_store.GetXmlZipPath(folder, key)))
                        {
                            entry.XmlStatus = "exists";
                            continue;
                        }

                        try
                        {
                            var zipBytes = await _client.ExportXmlZipAsync(key, cancellationToken).ConfigureAwait(false);
                            if (zipBytes == null)
                            {
                                entry.XmlStatus = "none";
                            }
                            else
                            {
                                _store.SaveXmlZip(folder, key, zipBytes, request.ExtractXml);
                                entry.XmlStatus = "ok";
                            }
                        }
                        catch (Exception ex) when ((ex is TaxInvoiceException h && h.StatusCode != 401) || ex is InvalidDataException || ex is IOException)
                        {
                            entry.XmlStatus = "error";
                            RecordError($"XML archive error for {key.FileStem}: {ex.Message}", entry);
                            failedXmlEntries.Add(entry);
                        }

                        progress?.Report(new DownloadProgress("xml", $"{directionLabel} {monthRange.MonthLabel} {key.InvoiceSeries}-{key.InvoiceNo}", index, entries.Count));
                    }

                    // Recovery Pass: Tự động phục hồi tải XML
                    if (failedXmlEntries.Count > 0)
                    {
                        progress?.Report(new DownloadProgress("recovery", $"Đang phục hồi tải XML (Recovery Pass) cho {failedXmlEntries.Count} hóa đơn..."));
                        await Task.Delay(1500, cancellationToken).ConfigureAwait(false);

                        foreach (var entry in failedXmlEntries)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var key = ToInvoiceKey(entry);
                            try
                            {
                                var zipBytes = await _client.ExportXmlZipAsync(key, cancellationToken).ConfigureAwait(false);
                                if (zipBytes != null)
                                {
                                    _store.SaveXmlZip(folder, key, zipBytes, request.ExtractXml);
                                    entry.XmlStatus = "ok";
                                    ClearError(entry);
                                    progress?.Report(new DownloadProgress("recovery", $"[Phục hồi XML thành công] {directionLabel} {key.InvoiceSeries}-{key.InvoiceNo}"));
                                }
                            }
                            catch
                            {
                                // Giữ nguyên trạng thái
                            }
                        }
                    }
                }

                monthLog.Completed = true;
            }
            finally
            {
                monthLog.FinishedAt = DateTimeOffset.Now;
                _store.SaveJson(Path.Combine(folder, "index.json"), entries);
                _store.SaveJson(Path.Combine(folder, "_log.json"), monthLog);
            }
        }

        private static InvoiceKey ToInvoiceKey(IndexEntry entry) =>
            new InvoiceKey(entry.SellerTaxCode, entry.InvoiceTemplate, entry.InvoiceSeries, entry.InvoiceNo, entry.Source == "sco" ? InvoiceSource.Sco : InvoiceSource.Query);
    }
}
