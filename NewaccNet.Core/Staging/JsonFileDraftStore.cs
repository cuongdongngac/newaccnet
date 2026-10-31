using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Staging
{
    /// <summary>
    /// Lưu trữ các chứng từ nháp (VoucherDraft) dạng tệp JSON trong thư mục đệm (Staging).
    /// Phù hợp cho CSDL cục bộ (MS Access) hoặc khi người dùng cấu hình lưu file.
    /// </summary>
    public class JsonFileDraftStore : IDraftStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly string _baseFolder;

        public JsonFileDraftStore(string? baseFolder = null)
        {
            if (string.IsNullOrWhiteSpace(baseFolder))
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _baseFolder = Path.Combine(appData, "NewaccNet", "Staging");
            }
            else
            {
                _baseFolder = baseFolder;
            }

            Directory.CreateDirectory(_baseFolder);
        }

        private string GetMonthFolder(DateTime date)
        {
            string folder = Path.Combine(_baseFolder, date.ToString("yyyy-MM"));
            Directory.CreateDirectory(folder);
            return folder;
        }

        public Task SaveAsync(VoucherDraft draft, CancellationToken ct = default)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            string folder = GetMonthFolder(draft.Header.VoucherDate);
            string fileName = $"Draft_{draft.DraftId}.json";
            string filePath = Path.Combine(folder, fileName);

            string json = JsonSerializer.Serialize(draft, JsonOptions);
            File.WriteAllText(filePath, json);

            return Task.CompletedTask;
        }

        public async Task<VoucherDraft?> GetAsync(string draftId, CancellationToken ct = default)
        {
            var doc = await GetDocumentAsync(draftId, ct);
            if (doc == null) return null;

            if (!string.IsNullOrWhiteSpace(doc.PayloadJson))
            {
                try
                {
                    var parsed = JsonSerializer.Deserialize<VoucherDraft>(doc.PayloadJson, JsonOptions);
                    if (parsed != null && parsed.Header != null) return parsed;
                }
                catch { }
            }

            return null;
        }

        public Task SaveDocumentAsync(StagingDocument doc, CancellationToken ct = default)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            string folder = GetMonthFolder(doc.CreatedAt);
            string fileName = $"Draft_{doc.DraftId}.json";
            string filePath = Path.Combine(folder, fileName);

            string json = JsonSerializer.Serialize(doc, JsonOptions);
            File.WriteAllText(filePath, json);

            return Task.CompletedTask;
        }

        public Task<StagingDocument?> GetDocumentAsync(string draftId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(draftId)) return Task.FromResult<StagingDocument?>(null);

            string fileName = $"Draft_{draftId}.json";
            var files = Directory.GetFiles(_baseFolder, fileName, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                try
                {
                    string json = File.ReadAllText(files[0]);
                    using var jdoc = JsonDocument.Parse(json);
                    var root = jdoc.RootElement;

                    if (root.TryGetProperty("PayloadJson", out _) || root.TryGetProperty("payloadJson", out _))
                    {
                        var doc = JsonSerializer.Deserialize<StagingDocument>(json, JsonOptions);
                        return Task.FromResult(doc);
                    }
                    else
                    {
                        // File JSON nháp trực tiếp VoucherDraft
                        var vDraft = JsonSerializer.Deserialize<VoucherDraft>(json, JsonOptions);
                        if (vDraft != null)
                        {
                            var doc = new StagingDocument
                            {
                                DraftId = vDraft.DraftId,
                                DraftType = vDraft.DraftType.ToString(),
                                Status = vDraft.Status,
                                SourceKind = vDraft.Source.Kind,
                                SourceRef = vDraft.Source.Ref,
                                Title = vDraft.GetDisplayTitle(),
                                PayloadJson = json,
                                CreatedAt = vDraft.Header.VoucherDate,
                                UpdatedAt = DateTime.Now,
                                PostedVoucherId = vDraft.PostedVoucherId
                            };
                            return Task.FromResult<StagingDocument?>(doc);
                        }
                    }
                }
                catch { }
            }

            return Task.FromResult<StagingDocument?>(null);
        }

        public Task<IReadOnlyList<StagingDocument>> QueryDocumentsAsync(DraftQueryFilter? filter = null, CancellationToken ct = default)
        {
            var result = new List<StagingDocument>();
            if (!Directory.Exists(_baseFolder)) return Task.FromResult<IReadOnlyList<StagingDocument>>(result);

            var files = Directory.GetFiles(_baseFolder, "Draft_*.json", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                try
                {
                    string json = File.ReadAllText(f);
                    using var jdoc = JsonDocument.Parse(json);
                    var root = jdoc.RootElement;

                    StagingDocument? doc = null;
                    if (root.TryGetProperty("PayloadJson", out _) || root.TryGetProperty("payloadJson", out _))
                    {
                        doc = JsonSerializer.Deserialize<StagingDocument>(json, JsonOptions);
                    }
                    else
                    {
                        var vDraft = JsonSerializer.Deserialize<VoucherDraft>(json, JsonOptions);
                        if (vDraft != null)
                        {
                            doc = new StagingDocument
                            {
                                DraftId = vDraft.DraftId,
                                DraftType = vDraft.DraftType.ToString(),
                                Status = vDraft.Status,
                                SourceKind = vDraft.Source.Kind,
                                SourceRef = vDraft.Source.Ref,
                                Title = vDraft.GetDisplayTitle(),
                                PayloadJson = json,
                                CreatedAt = vDraft.Header.VoucherDate,
                                UpdatedAt = DateTime.Now,
                                PostedVoucherId = vDraft.PostedVoucherId
                            };
                        }
                    }

                    if (doc == null) continue;

                    if (filter != null)
                    {
                        if (filter.Status.HasValue && doc.Status != filter.Status.Value) continue;
                        if (!string.IsNullOrWhiteSpace(filter.DraftTypeString) && !doc.DraftType.Equals(filter.DraftTypeString, StringComparison.OrdinalIgnoreCase)) continue;
                        if (filter.DraftType.HasValue && !doc.DraftType.Equals(filter.DraftType.Value.ToString(), StringComparison.OrdinalIgnoreCase)) continue;
                        if (!string.IsNullOrWhiteSpace(filter.SourceKind) && !string.Equals(doc.SourceKind, filter.SourceKind, StringComparison.OrdinalIgnoreCase)) continue;
                        if (filter.FromDate.HasValue && doc.CreatedAt < filter.FromDate.Value.Date) continue;
                        if (filter.ToDate.HasValue && doc.CreatedAt > filter.ToDate.Value.Date) continue;
                    }

                    result.Add(doc);
                }
                catch { }
            }

            var ordered = result
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return Task.FromResult<IReadOnlyList<StagingDocument>>(ordered);
        }

        public Task<IReadOnlyList<VoucherDraft>> QueryAsync(DraftQueryFilter? filter = null, CancellationToken ct = default)
        {
            var result = new List<VoucherDraft>();
            if (!Directory.Exists(_baseFolder)) return Task.FromResult<IReadOnlyList<VoucherDraft>>(result);

            var files = Directory.GetFiles(_baseFolder, "Draft_*.json", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                try
                {
                    string json = File.ReadAllText(f);
                    var draft = JsonSerializer.Deserialize<VoucherDraft>(json, JsonOptions);
                    if (draft == null) continue;

                    if (filter != null)
                    {
                        if (filter.Status.HasValue && draft.Status != filter.Status.Value) continue;
                        if (filter.DraftType.HasValue && draft.DraftType != filter.DraftType.Value) continue;
                        if (!string.IsNullOrWhiteSpace(filter.SourceKind) && !draft.Source.Kind.Equals(filter.SourceKind, StringComparison.OrdinalIgnoreCase)) continue;
                        if (filter.FromDate.HasValue && draft.Header.VoucherDate < filter.FromDate.Value.Date) continue;
                        if (filter.ToDate.HasValue && draft.Header.VoucherDate > filter.ToDate.Value.Date) continue;
                        if (!string.IsNullOrWhiteSpace(filter.SearchText))
                        {
                            string s = filter.SearchText.ToLowerInvariant();
                            bool match = (draft.Header.VoucherNo?.ToLowerInvariant().Contains(s) ?? false) ||
                                         (draft.Counterparty.Raw.Name?.ToLowerInvariant().Contains(s) ?? false) ||
                                         (draft.Counterparty.Raw.TaxCode?.ToLowerInvariant().Contains(s) ?? false) ||
                                         (draft.Invoice.Number?.ToLowerInvariant().Contains(s) ?? false);
                            if (!match) continue;
                        }
                    }

                    result.Add(draft);
                }
                catch { }
            }

            var ordered = result
                .OrderByDescending(x => x.Header.VoucherDate)
                .ThenByDescending(x => x.Invoice.Number)
                .ToList();

            return Task.FromResult<IReadOnlyList<VoucherDraft>>(ordered);
        }

        public Task DeleteAsync(string draftId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(draftId)) return Task.CompletedTask;

            string fileName = $"Draft_{draftId}.json";
            var files = Directory.GetFiles(_baseFolder, fileName, SearchOption.AllDirectories);
            foreach (var f in files)
            {
                try { File.Delete(f); } catch { }
            }

            return Task.CompletedTask;
        }

        public async Task<bool> ExistsBySourceRefAsync(string sourceKind, string sourceRef, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sourceRef)) return false;

            var all = await QueryAsync(new DraftQueryFilter { SourceKind = sourceKind }, ct);
            return all.Any(d => d.Source.Ref.Equals(sourceRef, StringComparison.OrdinalIgnoreCase));
        }

        // ── Static Helper Compatibility Methods ──────────────────────────
        public static string GetDefaultPreparedFolder(string baseFolder)
        {
            if (string.IsNullOrWhiteSpace(baseFolder))
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appData, "NewaccNet", "Staging", "Prepared");
            }
            return Path.Combine(baseFolder, "Prepared");
        }

        public static void SaveDraft(string folder, VoucherDraft draft)
        {
            var store = new JsonFileDraftStore(folder);
            store.SaveAsync(draft).GetAwaiter().GetResult();
        }

        public static List<VoucherDraft> LoadDrafts(string folder)
        {
            var store = new JsonFileDraftStore(folder);
            var items = store.QueryAsync().GetAwaiter().GetResult();
            return items.ToList();
        }

        public static void DeleteDraft(string folder, string draftId)
        {
            var store = new JsonFileDraftStore(folder);
            store.DeleteAsync(draftId).GetAwaiter().GetResult();
        }
    }
}
