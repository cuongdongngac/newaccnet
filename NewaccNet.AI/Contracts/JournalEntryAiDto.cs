using System;
using System.Collections.Generic;

namespace NewaccNet.AI.Contracts
{
    public class JournalEntryAiDto
    {
        public string VoucherType { get; set; } = "PKT"; // PT (Thu), PC (Chi), BN (Báo nợ), BC (Báo có), PN (Nhập), PX (Xuất), PKT (Kế toán chung)
        public string? VoucherDate { get; set; } // yyyy-MM-dd
        public string PersonName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TaxNo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? PartnerCode { get; set; }
        public string? PartnerName { get; set; }
        public decimal TotalAmount { get; set; }
        public List<JournalEntryLineAiDto> Lines { get; set; } = new();
        public string AiExplanation { get; set; } = string.Empty;
    }

    public class JournalEntryLineAiDto
    {
        public string DebitAccount { get; set; } = string.Empty;
        public string CreditAccount { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? WarehouseCode { get; set; }
        public string? Unit { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? VatRate { get; set; }
        public decimal VatAmount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
