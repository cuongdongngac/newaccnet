using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Sổ chi tiết Tài sản cố định (Thẻ TSCĐ).
    /// Hỗ trợ Drill-down 2 cấp:
    ///   - Cấp 1: Từ Báo cáo Tổng hợp mở sang Sổ chi tiết theo AssetId.
    ///   - Cấp 2: Từ dòng chứng từ (JournalVoucherId) click mở VoucherEntryWindow.
    /// </summary>
    [HighlightedClass]
    public class AssetDetailReportDTO
    {
        public AssetDetailReportDTO()
        {
        }

        // THÔNG TIN TÀI SẢN
        public string AssetId { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        // THÔNG TIN CHỨNG TỪ & ĐỊNH KHOẢN
        public int? JournalVoucherId { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string Contents { get; set; } = string.Empty;
        public string CounterAccountId { get; set; } = string.Empty;

        // LÝ DO TĂNG / GIẢM
        public string ReasonId { get; set; } = string.Empty;
        public string ReasonName { get; set; } = string.Empty;

        /// <summary>
        /// Đơn giá / Nguyên giá
        /// </summary>
        public decimal Price { get; set; }

        // PHÁT SINH TRONG KỲ (TĂNG / GIẢM)
        public decimal DebitQty { get; set; }
        public decimal DebitAmount { get; set; }

        public decimal CreditQty { get; set; }
        public decimal CreditAmount { get; set; }

        // SỐ DƯ LŨY KẾ
        public decimal ClosingQty { get; set; }
        public decimal ClosingAmount { get; set; }

        /// <summary>
        /// Đánh dấu dòng này là dòng Số dư đầu kỳ.
        /// </summary>
        public bool IsOpeningRow { get; set; }
    }
}
