using System;
using System.Collections.Generic;
using System.Text.Json;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Giao diện đánh dấu cho các DTO phân cấp được đặt vào Lớp đệm (Staging Payload).
    /// </summary>
    public interface IDraftPayload
    {
        string GetDraftType();
        string GetDisplayTitle();
    }

    /// <summary>
    /// DTO Phân cấp: Sao kê ngân hàng (Bank Statement).
    /// Dùng khi import file sao kê XML/Excel/PDF từ Vietcombank, BIDV, Techcombank...
    /// </summary>
    public class BankStatementDraft : IDraftPayload
    {
        public string GetDraftType() => "BankStatement";
        public string GetDisplayTitle() => $"Sao kê {BankName} - TK {AccountNumber} ({Transactions.Count} GD)";

        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Currency { get; set; } = "VND";
        public DateTime FromDate { get; set; } = DateTime.Today;
        public DateTime ToDate { get; set; } = DateTime.Today;
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public List<BankStatementLine> Transactions { get; set; } = new();
    }

    public class BankStatementLine
    {
        public int LineNo { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public string RefNo { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }   // Số tiền ghi nợ (rút ra/chi)
        public decimal CreditAmount { get; set; }  // Số tiền ghi có (nạp vào/thu)
        public decimal BalanceAfter { get; set; }  // Số dư sau GD
        public string Description { get; set; } = string.Empty;

        // Dữ liệu đối tác đối chiếu
        public string RawCounterpartyName { get; set; } = string.Empty;
        public string RawCounterpartyAccount { get; set; } = string.Empty;
        public string? ResolvedPartnerId { get; set; }
        public string? ResolvedAccountId { get; set; } // Ví dụ: 1121, 331, 131, 642...
        public MatchInfo Match { get; set; } = new();
    }

    /// <summary>
    /// DTO Phân cấp: Kiểm kê / Xuất nhập kho biến động từ Excel.
    /// Dùng khi kế toán đưa file kiểm kê nhiều cột hoặc phiếu nhập xuất vật tư từ chi nhánh/kho bãi.
    /// </summary>
    public class InventoryDraft : IDraftPayload
    {
        public string GetDraftType() => "InventoryMovement";
        public string GetDisplayTitle() => $"Phiếu kho {WarehouseId} - {Items.Count} mặt hàng";

        public string WarehouseId { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; } = DateTime.Today;
        public string MovementType { get; set; } = "NK"; // NK: Nhập kho, XK: Xuất kho, KK: Kiểm kê
        public string Reason { get; set; } = string.Empty;
        public List<InventoryDraftItem> Items { get; set; } = new();
    }

    public class InventoryDraftItem
    {
        public int LineNo { get; set; }
        public string RawItemCode { get; set; } = string.Empty;
        public string RawItemName { get; set; } = string.Empty;
        public string RawUnit { get; set; } = string.Empty;
        public string? BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal BookQuantity { get; set; }
        public decimal ActualQuantity { get; set; }
        public decimal DiffQuantity => ActualQuantity - BookQuantity;
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }

        public string? ResolvedMaterialId { get; set; }
        public string? ResolvedWarehouseId { get; set; }
        public string? ResolvedAccountId { get; set; } // 152, 156, 155, 632...
        public MatchInfo Match { get; set; } = new();
    }

    /// <summary>
    /// DTO Phân cấp: Bảng lương / Trích BHXH từ file Excel nhân sự.
    /// Dùng khi hạch toán bảng lương tháng, trích nộp bảo hiểm (3383, 3384, 3386), thuế TNCN (3335).
    /// </summary>
    public class PayrollDraft : IDraftPayload
    {
        public string GetDraftType() => "Payroll";
        public string GetDisplayTitle() => $"Bảng lương T{PeriodMonth}/{PeriodYear} ({Employees.Count} NV)";

        public int PeriodMonth { get; set; } = DateTime.Today.Month;
        public int PeriodYear { get; set; } = DateTime.Today.Year;
        public string Department { get; set; } = string.Empty;
        public decimal TotalGrossSalary { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalNetPay { get; set; }
        public List<PayrollEmployeeLine> Employees { get; set; } = new();
    }

    public class PayrollEmployeeLine
    {
        public int LineNo { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public decimal WorkingDays { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal AllowancesTotal { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal SocialInsuranceCompany { get; set; }
        public decimal SocialInsuranceEmployee { get; set; }
        public decimal PersonalIncomeTax { get; set; }
        public decimal NetPay { get; set; }

        public string? ResolvedEmployeeId { get; set; }
        public string CostAccountId { get; set; } = "6421"; // TK Chi phí lương (6411, 6421, 622, 6271)
        public string PayableAccountId { get; set; } = "3341"; // TK Phải trả NLĐ
    }

    /// <summary>
    /// Bộ đăng ký và nhận diện DTO phân cấp tự động dựa theo DraftType.
    /// </summary>
    public static class DraftPayloadRegistry
    {
        private static readonly Dictionary<string, Type> TypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["PurchaseInvoice"] = typeof(VoucherDraft),
            ["SalesInvoice"] = typeof(VoucherDraft),
            ["GeneralJournal"] = typeof(VoucherDraft),
            ["CashReceipt"] = typeof(VoucherDraft),
            ["CashPayment"] = typeof(VoucherDraft),
            ["BankReceipt"] = typeof(VoucherDraft),
            ["BankPayment"] = typeof(VoucherDraft),
            ["BankStatement"] = typeof(BankStatementDraft),
            ["InventoryMovement"] = typeof(InventoryDraft),
            ["InventoryCount"] = typeof(InventoryDraft),
            ["Payroll"] = typeof(PayrollDraft)
        };

        public static void Register(string draftType, Type payloadType)
        {
            TypeMap[draftType] = payloadType;
        }

        public static Type? ResolveType(string draftType)
        {
            TypeMap.TryGetValue(draftType, out var type);
            return type;
        }

        public static object? Deserialize(string draftType, string payloadJson, JsonSerializerOptions? options = null)
        {
            var targetType = ResolveType(draftType);
            if (targetType == null) return null;

            return JsonSerializer.Deserialize(payloadJson, targetType, options ?? StagingDocument.DefaultJsonOptions);
        }
    }
}
