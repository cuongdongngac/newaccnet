using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hddt
{
    /// <summary>Yêu cầu tải.</summary>
    public sealed class DownloadRequest
    {
        /// <summary>MST của đơn vị (dùng làm tên thư mục).</summary>
        public string Mst { get; set; } = "";
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IList<InvoiceDirection> Directions { get; set; } = new List<InvoiceDirection> { InvoiceDirection.Purchase, InvoiceDirection.Sold };
        /// <summary>Gọi thêm sco-query (hóa đơn từ máy tính tiền). Nên để true, nếu không sẽ sót hóa đơn.</summary>
        public bool IncludeSco { get; set; } = true;
        /// <summary>Tải chi tiết từng hóa đơn (danh sách hàng hóa).</summary>
        public bool DownloadDetail { get; set; } = true;
        /// <summary>Bỏ qua chi tiết đã có file (tải nhanh hơn khi chạy lại).</summary>
        public bool SkipExistingDetail { get; set; } = false;
        /// <summary>Tải XML gốc (zip). XML gốc không đổi nên luôn bỏ qua file đã có.</summary>
        public bool DownloadXml { get; set; } = false;
        /// <summary>Giải nén XML cạnh file zip.</summary>
        public bool ExtractXml { get; set; } = false;
        /// <summary>Lọc trạng thái hóa đơn (tthai), null = tất cả.</summary>
        public string? Tthai { get; set; }
        /// <summary>Lọc kết quả kiểm tra (ttxly), null = tất cả.</summary>
        public string? Ttxly { get; set; }
    }

    /// <summary>Thông báo tiến độ.</summary>
    public sealed class DownloadProgress
    {
        public DownloadProgress(string stage, string message, int done = 0, int total = 0)
        {
            Stage = stage; Message = message; Done = done; Total = total;
        }

        /// <summary>list | detail | xml | done</summary>
        public string Stage { get; }
        public string Message { get; }
        public int Done { get; }
        public int Total { get; }
        public override string ToString() => Total > 0 ? $"[{Stage} {Done}/{Total}] {Message}" : $"[{Stage}] {Message}";
    }

    /// <summary>Một dòng trong index.json của tháng.</summary>
    public sealed class IndexEntry
    {
        public string Nbmst { get; set; } = "";
        public string Khmshdon { get; set; } = "";
        public string Khhdon { get; set; } = "";
        public string Shdon { get; set; } = "";
        /// <summary>query | sco</summary>
        public string Source { get; set; } = "";
        public string? Tdlap { get; set; }
        public string FileStem { get; set; } = "";
        /// <summary>ok | skipped | error | not-requested</summary>
        public string Detail { get; set; } = "not-requested";
        /// <summary>ok | exists | none (cổng không có hồ sơ gốc) | error | not-requested</summary>
        public string Xml { get; set; } = "not-requested";
        public string? Error { get; set; }
    }

    /// <summary>Nội dung _log.json của tháng.</summary>
    public sealed class MonthLog
    {
        public string Mst { get; set; } = "";
        public string Direction { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public DateTimeOffset StartedAt { get; set; }
        public DateTimeOffset FinishedAt { get; set; }
        public int InvoiceCount { get; set; }
        public Dictionary<string, int> PagesBySource { get; set; } = new Dictionary<string, int>();
        public List<string> Errors { get; set; } = new List<string>();
        /// <summary>False nếu dừng giữa chừng (lỗi 401, huỷ...).</summary>
        public bool Completed { get; set; }
    }

    /// <summary>Kết quả tổng của một lần tải.</summary>
    public sealed class DownloadResult
    {
        public List<string> MonthFolders { get; } = new List<string>();
        public int InvoiceCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }

    /// <summary>
    /// Điều phối tải: chia theo tháng → danh sách (query + sco, đủ các trang) → chi tiết → XML,
    /// lưu qua <see cref="InvoiceFileStore"/>. Lỗi ở một tháng/hóa đơn được ghi log và chạy tiếp;
    /// riêng lỗi 401 (hết token) thì dừng để đăng nhập lại.
    /// </summary>
    public sealed class HddtDownloader
    {
        private readonly HddtClient _client;
        private readonly InvoiceFileStore _store;

        public HddtDownloader(HddtClient client, InvoiceFileStore store)
        {
            _client = client; _store = store;
        }

        public async Task<DownloadResult> DownloadAsync(DownloadRequest req, IProgress<DownloadProgress>? progress = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(req.Mst)) throw new ArgumentException("Thiếu MST.");
            if (_client.Token == null) throw new InvalidOperationException("Chưa đăng nhập.");

            var result = new DownloadResult();
            var months = DateRanges.SplitByMonth(req.From, req.To);

            foreach (var direction in req.Directions)
                foreach (var month in months)
                {
                    ct.ThrowIfCancellationRequested();
                    await DownloadMonthAsync(req, direction, month, result, progress, ct).ConfigureAwait(false);
                }

            progress?.Report(new DownloadProgress("done", $"Xong: {result.InvoiceCount} hóa đơn, {result.ErrorCount} lỗi."));
            return result;
        }

        private async Task DownloadMonthAsync(DownloadRequest req, InvoiceDirection direction, DateRange month,
            DownloadResult result, IProgress<DownloadProgress>? progress, CancellationToken ct)
        {
            var folder = _store.MonthFolder(req.Mst, direction, month);
            var dirLabel = direction == InvoiceDirection.Purchase ? "mua" : "bán";
            var log = new MonthLog
            {
                Mst = req.Mst,
                Direction = direction == InvoiceDirection.Purchase ? "mua" : "ban",
                From = month.From.ToString("yyyy-MM-dd"),
                To = month.To.ToString("yyyy-MM-dd"),
                StartedAt = DateTimeOffset.Now,
            };
            var entries = new List<IndexEntry>();
            result.MonthFolders.Add(folder);

            void Error(string msg, IndexEntry? e = null)
            {
                log.Errors.Add(msg);
                result.Errors.Add(month.MonthLabel + " " + dirLabel + ": " + msg);
                result.ErrorCount++;
                if (e != null) e.Error = msg;
            }

            try
            {
                // 1. Danh sách
                var sources = req.IncludeSco ? new[] { InvoiceSource.Query, InvoiceSource.Sco } : new[] { InvoiceSource.Query };
                var seen = new HashSet<InvoiceKey>();
                foreach (var source in sources)
                {
                    _store.ClearListPages(folder, source);
                    string? state = null;
                    var page = 0;
                    try
                    {
                        do
                        {
                            ct.ThrowIfCancellationRequested();
                            var p = await _client.GetInvoicePageAsync(direction, source, month.From, month.To, req.Tthai, req.Ttxly, state, ct).ConfigureAwait(false);
                            page++;
                            _store.SaveListPage(folder, source, page, p.RawJson);
                            foreach (var it in p.Items)
                                if (seen.Add(it.Key))
                                    entries.Add(new IndexEntry
                                    {
                                        Nbmst = it.Key.Nbmst, Khmshdon = it.Key.Khmshdon, Khhdon = it.Key.Khhdon, Shdon = it.Key.Shdon,
                                        Source = InvoiceFileStore.SourceTag(source), Tdlap = it.Tdlap, FileStem = it.Key.FileStem,
                                    });
                            progress?.Report(new DownloadProgress("list", $"Hóa đơn {dirLabel} {month} ({source}) trang {page}: {p.Items.Count} hóa đơn"));
                            state = p.NextState;
                        } while (state != null);
                    }
                    catch (HddtException ex) when (ex.StatusCode != 401)
                    {
                        Error($"Danh sách ({source}) trang {page + 1}: {ex.Message}");
                    }
                    log.PagesBySource[InvoiceFileStore.SourceTag(source)] = page;
                }
                log.InvoiceCount = entries.Count;
                result.InvoiceCount += entries.Count;

                // 2. Chi tiết
                if (req.DownloadDetail)
                {
                    var i = 0;
                    foreach (var e in entries)
                    {
                        ct.ThrowIfCancellationRequested();
                        i++;
                        var key = ToKey(e);
                        if (req.SkipExistingDetail && File.Exists(_store.DetailPath(folder, key))) { e.Detail = "skipped"; continue; }
                        try
                        {
                            var json = await _client.GetDetailJsonAsync(key, ct).ConfigureAwait(false);
                            _store.SaveDetail(folder, key, json);
                            e.Detail = "ok";
                        }
                        catch (HddtException ex) when (ex.StatusCode != 401)
                        {
                            e.Detail = "error";
                            Error($"Chi tiết {key.FileStem}: {ex.Message}", e);
                        }
                        progress?.Report(new DownloadProgress("detail", $"{dirLabel} {month.MonthLabel} {key.Khhdon}-{key.Shdon}", i, entries.Count));
                    }
                }

                // 3. XML gốc
                if (req.DownloadXml)
                {
                    var i = 0;
                    foreach (var e in entries)
                    {
                        ct.ThrowIfCancellationRequested();
                        i++;
                        var key = ToKey(e);
                        if (File.Exists(_store.XmlZipPath(folder, key))) { e.Xml = "exists"; continue; }
                        try
                        {
                            var zip = await _client.ExportXmlZipAsync(key, ct).ConfigureAwait(false);
                            if (zip == null) e.Xml = "none";
                            else { _store.SaveXmlZip(folder, key, zip, req.ExtractXml); e.Xml = "ok"; }
                        }
                        catch (Exception ex) when ((ex is HddtException h && h.StatusCode != 401) || ex is InvalidDataException || ex is IOException)
                        {
                            e.Xml = "error";
                            Error($"XML {key.FileStem}: {ex.Message}", e);
                        }
                        progress?.Report(new DownloadProgress("xml", $"{dirLabel} {month.MonthLabel} {key.Khhdon}-{key.Shdon}", i, entries.Count));
                    }
                }

                log.Completed = true;
            }
            finally
            {
                // Luôn ghi index/log, kể cả khi dừng vì 401 hoặc bị huỷ, để biết cần tải bù phần nào.
                log.FinishedAt = DateTimeOffset.Now;
                _store.SaveJson(Path.Combine(folder, "index.json"), entries);
                _store.SaveJson(Path.Combine(folder, "_log.json"), log);
            }
        }

        private static InvoiceKey ToKey(IndexEntry e) =>
            new InvoiceKey(e.Nbmst, e.Khmshdon, e.Khhdon, e.Shdon, e.Source == "sco" ? InvoiceSource.Sco : InvoiceSource.Query);
    }
}
