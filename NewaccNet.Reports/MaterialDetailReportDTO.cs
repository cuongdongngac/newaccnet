using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Báo cáo Sổ chi tiết Vật tư, dụng cụ, sản phẩm, hàng hóa (Mẫu S10-DN).
    /// Hỗ trợ Drill-down 2 cấp:
    ///   - Cấp 1: Từ Báo cáo Tổng hợp (Bảng Nhập - Xuất - Tồn) mở sang Sổ chi tiết theo (WarehouseId, MaterialId).
    ///   - Cấp 2: Từ dòng chi tiết chứng từ (JournalVoucherId) click mở màn hình chứng từ gốc (VoucherEntryWindow).
    /// </summary>
    [HighlightedClass]
    public class MaterialDetailReportDTO
    {
        public MaterialDetailReportDTO()
        {
        }

        // THÔNG TIN ĐỐI TƯỢNG VẬT TƯ & KHO
        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;

        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        // THÔNG TIN CHỨNG TỪ & ĐỊNH KHOẢN
        public int? JournalVoucherId { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string InOutNumber { get; set; } = string.Empty;
        public string Contents { get; set; } = string.Empty;
        public string CounterAccountId { get; set; } = string.Empty;

        /// <summary>
        /// Đơn giá vốn nhập / xuất (không phải giá bán).
        /// </summary>
        public decimal Price { get; set; }

        // PHÁT SINH TRONG KỲ
        public decimal DebitQty { get; set; }
        public decimal DebitAmount { get; set; }

        public decimal CreditQty { get; set; }
        public decimal CreditAmount { get; set; }

        // SỐ DƯ TỒN LŨY KẾ
        public decimal ClosingQty { get; set; }
        public decimal ClosingAmount { get; set; }

        /// <summary>
        /// Đánh dấu dòng này là dòng Số dư đầu kỳ.
        /// </summary>
        public bool IsOpeningRow { get; set; }
    }
}
