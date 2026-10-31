using System;
using System.Collections.Generic;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Các giai đoạn trong Vòng đời (Lifecycle Stages) của dữ liệu Staging.
    /// Dữ liệu là bất biến ở phần gốc (Raw) và chuyển hóa qua các vòng đời.
    /// Có thể lưu lại ở trạng thái chờ (Persistent Checkpoint) để lần sau mở lên làm tiếp.
    /// </summary>
    public enum DraftLifecycleStage
    {
        /// <summary>
        /// Vòng đời 1: Dữ liệu thô vừa nạp (Format 1 tầng hoặc 2 tầng).
        /// Hóa đơn thuế: Tầng trên = MST/Người mua-bán; Tầng dưới = Chi tiết dòng hàng.
        /// Chưa so khớp danh mục.
        /// </summary>
        Stage1_Raw = 1,

        /// <summary>
        /// Vòng đời 2: Chuẩn hóa & So khớp danh mục (Master Data Resolution).
        /// Khớp MST ➔ Khách hàng/NCC nội bộ.
        /// Khớp Tên hàng ➔ Vật tư, Nhóm hàng, Kho nội bộ (với sự trợ giúp của AI).
        /// </summary>
        Stage2_Standardized = 2,

        /// <summary>
        /// Vòng đời 3: Chuẩn hóa mức DTO hạch toán (Cấu trúc 3 tầng: Voucher ➔ InventoryVoucher ➔ Detail).
        /// Nếu giao dịch công nợ thì gắn MST đối tác vào chi tiết 131/331.
        /// Cho phép load lên hiệu chỉnh tiếp nếu cần. Sẵn sàng bấm một nút là ghi sổ.
        /// </summary>
        Stage3_VoucherReady = 3,

        /// <summary>
        /// Vòng đời 4: Đã ghi sổ chính thức vào CSDL kế toán thật qua IPostingGateway.
        /// </summary>
        Stage4_Posted = 4
    }

    /// <summary>
    /// Định dạng Staging 1 tầng: Đồng bộ danh mục độc lập (Master Data Sync).
    /// Dùng khi chỉ cần so sánh danh sách Đối tác (Khách hàng/NCC) hoặc Hàng hóa/Vật tư từ nguồn ngoài với DB nội bộ,
    /// nhờ AI so khớp và gợi ý mã/tên/nhóm mới.
    /// </summary>
    public class MasterDataDraft : IDraftPayload
    {
        public string GetDraftType() => "MasterData";
        public string GetDisplayTitle() => $"Danh mục {Kind} - {Items.Count} bản ghi ({SourceRef})";

        public string Kind { get; set; } = "Partner"; // Partner, Material, Warehouse, Account
        public string SourceRef { get; set; } = string.Empty;
        public DateTime ImportedAt { get; set; } = DateTime.Now;
        public List<MasterDataDraftItem> Items { get; set; } = new();
    }

    public class MasterDataDraftItem
    {
        public int LineNo { get; set; }
        public string RawCode { get; set; } = string.Empty;
        public string RawName { get; set; } = string.Empty;
        public string RawTaxCode { get; set; } = string.Empty;
        public string RawAddress { get; set; } = string.Empty;
        public string RawUnit { get; set; } = string.Empty;

        // Kết quả đối chiếu với DB
        public bool ExistsInDb { get; set; }
        public string? MatchedDbId { get; set; }
        public string? MatchedDbName { get; set; }

        // Đề xuất chuẩn hóa (từ thuật toán hoặc AI gợi ý)
        public string? ProposedCode { get; set; }
        public string? ProposedName { get; set; }
        public string? ProposedGroup { get; set; }
        public string? ProposedDefaultAccount { get; set; }
        public MatchInfo Match { get; set; } = new();
        public bool IsApproved { get; set; }
    }

    /// <summary>
    /// Cấu trúc DTO 3 tầng hoàn chỉnh cho Chứng từ Kế toán (Vòng đời 3).
    /// Sinh ra sau khi đã hoàn tất chuẩn hóa danh mục ở Vòng đời 2.
    /// </summary>
    public class AccountingVoucherThreeTierDraft : IDraftPayload
    {
        public string GetDraftType() => "AccountingVoucher3Tier";
        public string GetDisplayTitle() => $"{TopVoucher.VoucherSourceCode} {TopVoucher.VoucherNo} - {TopVoucher.PartnerName} ({TopVoucher.TotalAmount:N0} đ)";

        public DraftLifecycleStage Stage { get; set; } = DraftLifecycleStage.Stage3_VoucherReady;

        // ══════════════════════════════════════════════════════════════════
        // TẦNG TOP (TẦNG 1): CHỨNG TỪ SỔ CÁI (JOURNAL VOUCHER)
        // ══════════════════════════════════════════════════════════════════
        public TopVoucherTier TopVoucher { get; set; } = new();

        // ══════════════════════════════════════════════════════════════════
        // TẦNG GIỮA (TẦNG 2): PHIẾU KHO (INVENTORY VOUCHER) - NẾU CÓ QUA KHO
        // ══════════════════════════════════════════════════════════════════
        public MiddleInventoryTier? InventoryVoucher { get; set; }

        // ══════════════════════════════════════════════════════════════════
        // TẦNG DƯỚI (TẦNG 3): CHI TIẾT ĐỊNH KHOẢN & VẬT TƯ (DETAILS)
        // ══════════════════════════════════════════════════════════════════
        public List<DetailLineTier> Details { get; set; } = new();
    }

    /// <summary>
    /// Tầng Top: Thông tin chứng từ tổng hợp (Journal Voucher)
    /// </summary>
    public class TopVoucherTier
    {
        public DateTime VoucherDate { get; set; } = DateTime.Today;
        public string VoucherSourceCode { get; set; } = "NM"; // NM: Mua hàng, XB: Bán hàng, PK: Phiếu kế toán...
        public string? VoucherNo { get; set; }
        public string Contents { get; set; } = string.Empty;

        // Thông tin đối tác giao dịch
        public string PartnerId { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Hóa đơn thuế kèm theo (nếu có)
        public string? InvoiceTemplate { get; set; }
        public string? InvoiceSeries { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }

        // Tổng tiền
        public decimal TotalAmountWithoutTax { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Thanh toán / Công nợ
        public bool IsDebtTransaction { get; set; } = true; // Giao dịch công nợ (131/331) hay Tiền mặt (111)/Ngân hàng (112)
        public string SettlementAccountId { get; set; } = "3311";
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// Tầng Giữa: Phiếu kho (Inventory Voucher) - Quản lý nhập/xuất vật chất
    /// </summary>
    public class MiddleInventoryTier
    {
        public string InventoryVoucherNo { get; set; } = string.Empty;
        public DateTime InventoryDate { get; set; } = DateTime.Today;
        public string MovementType { get; set; } = "NK"; // NK: Nhập kho, XK: Xuất kho
        public string DefaultWarehouseId { get; set; } = string.Empty;
        public string DefaultWarehouseName { get; set; } = string.Empty;
        public string ReceiverOrDeliverer { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }

    /// <summary>
    /// Tầng Dưới: Chi tiết từng dòng định khoản, hàng hóa, số lượng, tiền thuế
    /// </summary>
    public class DetailLineTier
    {
        public int LineNo { get; set; }

        // Dữ liệu hàng hóa/vật tư
        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }

        // Định khoản hạch toán
        public string DebitAccountId { get; set; } = "1561";
        public string CreditAccountId { get; set; } = "3311";

        // Thuế GTGT
        public double VatRate { get; set; } = 10;
        public decimal VatAmount { get; set; }
        public string VatDebitAccountId { get; set; } = "13311";

        // Bóc tách công nợ chi tiết theo đối tượng (Nếu giao dịch công nợ thì gắn MST / PartnerId vào)
        public string? DebtPartnerId { get; set; }
        public string? DebtPartnerTaxCode { get; set; }

        // Ghi chú & Đối chiếu
        public string LineNote { get; set; } = string.Empty;
        public MatchInfo Match { get; set; } = new();
    }
}
