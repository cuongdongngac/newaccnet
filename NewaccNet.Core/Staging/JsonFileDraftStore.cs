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

        public Task<VoucherDraft?> GetAsync(string draftId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(draftId)) return Task.FromResult<VoucherDraft?>(null);

            string fileName = $"Draft_{draftId}.json";
            var files = Directory.GetFiles(_baseFolder, fileName, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                try
                {
                    string json = File.ReadAllText(files[0]);
                    var draft = JsonSerializer.Deserialize<VoucherDraft>(json, JsonOptions);
                    return Task.FromResult(draft);
                }
                catch { }
            }

            return Task.FromResult<VoucherDraft?>(null);
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
