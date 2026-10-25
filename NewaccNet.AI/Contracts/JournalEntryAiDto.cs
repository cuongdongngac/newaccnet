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
        public string Contents { get => Description; set => Description = value; }
        public string? PartnerCode { get; set; }
        public string? PartnerName { get; set; }
        public string SuggestedVoucherSourceCode { get; set; } = "PKT";
        public decimal TotalAmount { get; set; }

        public List<JournalEntryLineAiDto> Entries { get; set; } = new();
        public List<JournalEntryLineAiDto> Lines { get => Entries; set => Entries = value; }

        public string AiExplanation { get; set; } = string.Empty;
    }

    public class JournalEntryLineAiDto
    {
        public string DbcrText { get; set; } = "Nợ"; // "Nợ" hoặc "Có"
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? PartnerCode { get; set; }
        public string? WarehouseCode { get; set; }

        public string DebitAccount
        {
            get => DbcrText == "Nợ" ? AccountId : string.Empty;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    DbcrText = "Nợ";
                    AccountId = value;
                }
            }
        }

        public string CreditAccount
        {
            get => DbcrText == "Có" ? AccountId : string.Empty;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    DbcrText = "Có";
                    AccountId = value;
                }
            }
        }

        public string Note
        {
            get => Description;
            set => Description = value;
        }

        public string? MaterialCode { get; set; }
        public string? MaterialName { get; set; }
        public string? Unit { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? VatRate { get; set; }
        public decimal VatAmount { get; set; }
    }
}
