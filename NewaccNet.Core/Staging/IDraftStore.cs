using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Staging
{
    public class DraftQueryFilter
    {
        public DraftStatus? Status { get; set; }
        public DraftType? DraftType { get; set; }
        public string? SourceKind { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchText { get; set; }
    }

    /// <summary>
    /// Giao diện kho lưu trữ Lớp đệm (Staging / Phòng chờ nháp).
    /// Hỗ trợ cả 2 cơ chế (tùy theo cấu hình người dùng chọn):
    /// 1. JsonFileDraftStore: Lưu file .json theo tháng cục bộ.
    /// 2. DbDraftStore: Lưu vào bảng StagingDraft trong CSDL (SQL Server/Postgres/Access).
    /// </summary>
    public interface IDraftStore
    {
        Task<VoucherDraft?> GetAsync(string draftId, CancellationToken ct = default);
        Task SaveAsync(VoucherDraft draft, CancellationToken ct = default);
        Task<IReadOnlyList<VoucherDraft>> QueryAsync(DraftQueryFilter? filter = null, CancellationToken ct = default);
        Task DeleteAsync(string draftId, CancellationToken ct = default);
        Task<bool> ExistsBySourceRefAsync(string sourceKind, string sourceRef, CancellationToken ct = default);
    }
}
