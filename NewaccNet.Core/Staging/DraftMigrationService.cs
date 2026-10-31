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
        /// Sử dụng StagingDocument để giữ nguyên vẹn mọi cấu trúc phân cấp trong PayloadJson.
        /// </summary>
        public async Task<MigrationResult> MigrateJsonToDbAsync(
            bool overwrite = false,
            IProgress<string>? progress = null,
            CancellationToken ct = default)
        {
            var result = new MigrationResult();
            progress?.Report("Đang quét các tệp JSON trong thư mục Staging...");

            var jsonDocs = await _jsonStore.QueryDocumentsAsync(filter: null, ct).ConfigureAwait(false);
            result.TotalFound = jsonDocs.Count;
            progress?.Report($"Tìm thấy {result.TotalFound} chứng từ nháp từ tệp JSON.");

            foreach (var doc in jsonDocs)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (!overwrite)
                    {
                        var existing = await _dbStore.GetDocumentAsync(doc.DraftId, ct).ConfigureAwait(false);
                        if (existing != null)
                        {
                            result.SkippedCount++;
                            continue;
                        }
                    }

                    await _dbStore.SaveDocumentAsync(doc, ct).ConfigureAwait(false);
                    result.SuccessCount++;
                    progress?.Report($"[JSON ➔ DB] Đã nạp nháp: {doc.DraftId} ({doc.DraftType})");
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Messages.Add($"Lỗi nạp nháp {doc.DraftId}: {ex.Message}");
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

            var dbDocs = await _dbStore.QueryDocumentsAsync(filter: null, ct).ConfigureAwait(false);
            result.TotalFound = dbDocs.Count;
            progress?.Report($"Tìm thấy {result.TotalFound} chứng từ nháp trong CSDL.");

            foreach (var doc in dbDocs)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    if (!overwrite)
                    {
                        var existing = await _jsonStore.GetDocumentAsync(doc.DraftId, ct).ConfigureAwait(false);
                        if (existing != null)
                        {
                            result.SkippedCount++;
                            continue;
                        }
                    }

                    await _jsonStore.SaveDocumentAsync(doc, ct).ConfigureAwait(false);
                    result.SuccessCount++;
                    progress?.Report($"[DB ➔ JSON] Đã xuất nháp: {doc.DraftId} ({doc.DraftType})");
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Messages.Add($"Lỗi xuất nháp {doc.DraftId}: {ex.Message}");
                }
            }

            progress?.Report($"Hoàn tất xuất ra tệp JSON: {result.SuccessCount}/{result.TotalFound} thành công, {result.SkippedCount} bỏ qua, {result.ErrorCount} lỗi.");
            return result;
        }
    }
}
