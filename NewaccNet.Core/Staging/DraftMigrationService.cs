using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Staging
{
    public class MigrationResult
    {
        public int TotalFound { get; set; }
        public int SuccessCount { get; set; }
        public int SkippedCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Messages { get; set; } = new();
    }

    /// <summary>
    /// Công cụ chuyển đổi và đồng bộ chứng từ nháp hai chiều giữa Tệp JSON và Cơ sở dữ liệu (Staging Table).
    /// </summary>
    public class DraftMigrationService
    {
        private readonly JsonFileDraftStore _jsonStore;
        private readonly DbDraftStore _dbStore;

        public DraftMigrationService(string? jsonFolder = null)
        {
            _jsonStore = new JsonFileDraftStore(jsonFolder);
            _dbStore = new DbDraftStore();
        }

        /// <summary>
        /// Nạp toàn bộ chứng từ nháp từ các tệp JSON và lưu vào bảng StagingDraft trong CSDL.
        /// </summary>
        public async Task<MigrationResult> MigrateJsonToDbAsync(
            bool overwrite = false,
            IProgress<string>? progress = null,
            CancellationToken ct = default)
        {
            var result = new MigrationResult();
            progress?.Report("Đang quét các tệp JSON trong thư mục Staging...");

            var jsonDrafts = await _jsonStore.QueryAsync(filter: null, ct).ConfigureAwait(false);
            result.TotalFound = jsonDrafts.Count;
            progress?.Report($"Tìm thấy {result.TotalFound} chứng từ nháp từ tệp JSON.");

            foreach (var draft in jsonDrafts)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (!overwrite)
                    {
                        var existing = await _dbStore.GetAsync(draft.DraftId, ct).ConfigureAwait(false);
                        if (existing != null)
                        {
                            result.SkippedCount++;
                            continue;
                        }
                    }

                    await _dbStore.SaveAsync(draft, ct).ConfigureAwait(false);
                    result.SuccessCount++;
                    progress?.Report($"[JSON ➔ DB] Đã nạp nháp: {draft.DraftId} (HĐ: {draft.Invoice.Number})");
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Messages.Add($"Lỗi nạp nháp {draft.DraftId}: {ex.Message}");
                }
            }

            progress?.Report($"Hoàn tất nạp vào CSDL: {result.SuccessCount}/{result.TotalFound} thành công, {result.SkippedCount} bỏ qua, {result.ErrorCount} lỗi.");
            return result;
        }

        /// <summary>
        /// Xuất toàn bộ chứng từ nháp từ bảng StagingDraft trong CSDL ra các tệp JSON theo tháng.
        /// </summary>
        public async Task<MigrationResult> MigrateDbToJsonAsync(
            bool overwrite = false,
            IProgress<string>? progress = null,
            CancellationToken ct = default)
        {
            var result = new MigrationResult();
            progress?.Report("Đang đọc các chứng từ nháp từ bảng CSDL StagingDraft...");

            var dbDrafts = await _dbStore.QueryAsync(filter: null, ct).ConfigureAwait(false);
            result.TotalFound = dbDrafts.Count;
            progress?.Report($"Tìm thấy {result.TotalFound} chứng từ nháp trong CSDL.");

            foreach (var draft in dbDrafts)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (!overwrite)
                    {
                        var existing = await _jsonStore.GetAsync(draft.DraftId, ct).ConfigureAwait(false);
                        if (existing != null)
                        {
                            result.SkippedCount++;
                            continue;
                        }
                    }

                    await _jsonStore.SaveAsync(draft, ct).ConfigureAwait(false);
                    result.SuccessCount++;
                    progress?.Report($"[DB ➔ JSON] Đã xuất nháp: {draft.DraftId} (HĐ: {draft.Invoice.Number})");
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Messages.Add($"Lỗi xuất nháp {draft.DraftId}: {ex.Message}");
                }
            }

            progress?.Report($"Hoàn tất xuất ra JSON: {result.SuccessCount}/{result.TotalFound} thành công, {result.SkippedCount} bỏ qua, {result.ErrorCount} lỗi.");
            return result;
        }
    }
}
