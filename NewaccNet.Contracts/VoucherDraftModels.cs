using System;
using System.Collections.Generic;

namespace NewaccNet.Contracts
{
    public enum DraftStatus
    {
        Imported = 0,    // Vừa chuyển từ nguồn về, chưa mapping
        Matched = 1,     // Đã chạy mapping (tất định/AI), có thể còn dòng chưa xác định
        NeedsReview = 2, // Còn thiếu mã, nghi trùng hoặc có cảnh báo cần người xem
        Approved = 3,    // Người dùng đã kiểm tra và duyệt, sẵn sàng ghi sổ
        Posted = 4,      // Đã ghi sổ thành công vào CSDL (có PostedVoucherId)
        Rejected = 5,    // Bỏ qua (không hạch toán)
        Failed = 6       // Ghi sổ lỗi
    }

    public enum DraftType
    {
        PurchaseInvoice = 0, // Hóa đơn mua vào (NM)
        SalesInvoice = 1,    // Hóa đơn bán ra (XB)
        CashReceipt = 2,     // Thu tiền mặt (PT)
        CashPayment = 3,     // Chi tiền mặt (PC)
        BankReceipt = 4,     // Thu tiền gửi ngân hàng (BC)
        BankPayment = 5,     // Chi tiền gửi ngân hàng (BN)
        GeneralJournal = 6   // Bút toán tổng hợp khác (PK)
    }

    public enum MatchMethod
    {
        None = 0,
        TaxCodeExact = 1, // Trùng mã số thuế 100%
        CodeExact = 2,    // Trùng mã danh mục 100%
        AliasMemory = 3,  // Học từ sổ tay ghi nhớ lịch sử (NCC + tên hàng)
        PostingRule = 4,  // Áp theo quy tắc hạch toán định sẵn
        History = 5,      // Tra cứu lịch sử chứng từ gần nhất
        Fuzzy = 6,        // So khớp mờ độ tương đồng
        Ai = 7,           // AI suy luận đề xuất
        User = 8          // Người dùng tự tay chọn/sửa
    }

    public enum IssueSeverity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public class MatchInfo
    {
        public string Field { get; set; } = string.Empty;
        public MatchMethod Method { get; set; } = MatchMethod.None;
        public double Confidence { get; set; } = 1.0; // 0.0 - 1.0
        public string Reason { get; set; } = string.Empty;
    }

    public class DraftSourceInfo
    {
        public string Kind { get; set; } = "TaxInvoice"; // TaxInvoice, Excel, OCR, AiChat
        public string Ref { get; set; } = string.Empty;
        public DateTime ImportedAt { get; set; } = DateTime.Now;
        public string ImportedBy { get; set; } = "system";
    }

    public class DraftHeader
    {
        public DateTime VoucherDate { get; set; } = DateTime.Today;
        public string? VoucherNo { get; set; }
        public string VoucherSourceCode { get; set; } = "NM"; // NM, XB, PT, PC, PK...
        public string Contents { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
    }

    public class CounterpartyRaw
    {
        public string TaxCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
    }

    public class CounterpartyResolved
    {
        public string PartnerId { get; set; } = string.Empty;
        public bool IsNew { get; set; }
    }

    public class DraftCounterparty
    {
        public CounterpartyRaw Raw { get; set; } = new();
        public CounterpartyResolved Resolved { get; set; } = new();
        public MatchInfo Match { get; set; } = new();
    }

    public class DraftInvoiceInfo
    {
        public string Template { get; set; } = string.Empty;
        public string Series { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Today;
        public decimal AmountBeforeTax { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class DraftLineRaw
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public double VatRate { get; set; }
        public decimal VatAmount { get; set; }
    }

    public class DraftLineResolved
    {
        public string MaterialId { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string DebitAccountId { get; set; } = string.Empty;
        public string VatAccountId { get; set; } = "13311";
        public string? CostObjectId { get; set; }
    }

    public class DraftLine
    {
        public int LineNo { get; set; }
        public DraftLineRaw Raw { get; set; } = new();
        public DraftLineResolved Resolved { get; set; } = new();
        public List<MatchInfo> Matches { get; set; } = new();
    }

    public class DraftSettlement
    {
        public string CreditAccountId { get; set; } = "3311";
        public DateTime? DueDate { get; set; }
    }

    public class ProposedMasterData
    {
        public string Kind { get; set; } = "Partner"; // Partner hoặc Material
        public string TempKey { get; set; } = string.Empty;
        public Dictionary<string, string> Fields { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public bool Approved { get; set; }
    }

    public class DraftIssue
    {
        public IssueSeverity Severity { get; set; } = IssueSeverity.Warning;
        public string Code { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class DraftAuditEntry
    {
        public DateTime At { get; set; } = DateTime.Now;
        public string By { get; set; } = "system";
        public string Action { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    /// <summary>
    /// Định dạng trung gian (Lớp đệm - Staging) cho mọi nguồn hóa đơn, chứng từ.
    /// Cho phép lưu trữ, hiển thị, chỉnh sửa, rà soát và đối chiếu trước khi ghi sổ.
    /// </summary>
    public class VoucherDraft
    {
        public int SchemaVersion { get; set; } = 1;
        public string DraftId { get; set; } = Guid.NewGuid().ToString("N");
        public DraftType DraftType { get; set; } = DraftType.PurchaseInvoice;
        public DraftStatus Status { get; set; } = DraftStatus.Imported;

        public DraftSourceInfo Source { get; set; } = new();
        public DraftHeader Header { get; set; } = new();
        public DraftCounterparty Counterparty { get; set; } = new();
        public DraftInvoiceInfo Invoice { get; set; } = new();
        public List<DraftLine> Lines { get; set; } = new();
        public DraftSettlement Settlement { get; set; } = new();

        public List<ProposedMasterData> ProposedMasterData { get; set; } = new();
        public List<DraftIssue> Issues { get; set; } = new();
        public List<DraftAuditEntry> Audit { get; set; } = new();

        public int? PostedVoucherId { get; set; }
    }

    public class PostingResult
    {
        public bool Success { get; set; }
        public int? VoucherId { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public List<DraftIssue> Issues { get; set; } = new();
    }
}
