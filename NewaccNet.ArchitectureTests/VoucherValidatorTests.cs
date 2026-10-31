using System;
using System.Collections.Generic;
using DataAccess.EntityClasses;
using NewaccNet.Contracts;
using NewaccNet.Core.Posting;
using Xunit;

namespace NewaccNet.ArchitectureTests
{
    public class VoucherValidatorTests
    {
        private readonly VoucherValidator _validator = new();

        [Fact]
        public void ValidateEntity_WhenUnbalanced_ShouldReturnError()
        {
            var voucher = new JournalVoucherEntity
            {
                VoucherDate = DateTime.Today,
                VoucherNo = "TEST01"
            };

            // Nợ 1.000.000 nhưng chỉ Có 900.000 -> lệch 100.000
            voucher.JournalEntries.Add(new JournalEntryEntity { AccountId = "1111", Dbcr = 1, Amount = 1000000 });
            voucher.JournalEntries.Add(new JournalEntryEntity { AccountId = "3311", Dbcr = -1, Amount = 900000 });

            var issues = _validator.ValidateEntity(voucher, 1);

            Assert.Contains(issues, i => i.Code == "UNBALANCED_ENTRIES" && i.Severity == IssueSeverity.Error);
        }

        [Fact]
        public void ValidateEntity_WhenBalanced_ShouldPassBalanceCheck()
        {
            var voucher = new JournalVoucherEntity
            {
                VoucherDate = DateTime.Today,
                VoucherNo = "TEST02"
            };

            // Nợ 1.000.000 và Có 1.000.000 -> cân đối
            voucher.JournalEntries.Add(new JournalEntryEntity { AccountId = "1111", Dbcr = 1, Amount = 1000000 });
            voucher.JournalEntries.Add(new JournalEntryEntity { AccountId = "3311", Dbcr = -1, Amount = 1000000 });

            var issues = _validator.ValidateEntity(voucher, 0);

            Assert.DoesNotContain(issues, i => i.Code == "UNBALANCED_ENTRIES");
        }

        [Fact]
        public void ValidateDraft_WhenLinesEmpty_ShouldReturnNoLinesError()
        {
            var draft = new VoucherDraft
            {
                DraftType = DraftType.PurchaseInvoice,
                Lines = new List<DraftLine>()
            };

            var issues = _validator.ValidateDraft(draft);

            Assert.Contains(issues, i => i.Code == "NO_LINES" && i.Severity == IssueSeverity.Error);
        }

        [Fact]
        public void PurchaseInvoiceVoucherBuilder_ShouldGenerateBalancedEntityGraph()
        {
            var draft = new VoucherDraft
            {
                DraftType = DraftType.PurchaseInvoice,
                Header = new DraftHeader
                {
                    VoucherDate = DateTime.Today,
                    VoucherNo = "NM0001",
                    Contents = "Mua vật tư A4"
                },
                Counterparty = new DraftCounterparty
                {
                    Raw = new CounterpartyRaw { Name = "Công ty ABC", TaxCode = "0101234567" },
                    Resolved = new CounterpartyResolved { PartnerId = "ABC01" }
                },
                Invoice = new DraftInvoiceInfo
                {
                    Number = "0001234",
                    Series = "C26TAA",
                    Date = DateTime.Today
                },
                Settlement = new DraftSettlement
                {
                    CreditAccountId = "3311"
                },
                Lines = new List<DraftLine>
                {
                    new DraftLine
                    {
                        LineNo = 1,
                        Raw = new DraftLineRaw
                        {
                            ItemName = "Giấy A4",
                            Quantity = 10,
                            UnitPrice = 100000,
                            Amount = 1000000,
                            VatRate = 10,
                            VatAmount = 100000
                        },
                        Resolved = new DraftLineResolved
                        {
                            MaterialId = "VPP01",
                            WarehouseId = "KHO1",
                            DebitAccountId = "1561",
                            VatAccountId = "13311"
                        }
                    }
                }
            };

            var builder = new PurchaseInvoiceVoucherBuilder();
            var voucher = builder.Build(draft);

            Assert.NotNull(voucher);
            Assert.Equal("NM0001", voucher.VoucherNo);
            
            // Kiểm tra cân đối Nợ = Có:
            // Có 331: 1.100.000 (-1)
            // Nợ 1561: 1.000.000 (+1)
            // Nợ 13311: 100.000 (+1)
            // Tổng: 0
            double balance = 0;
            foreach (var je in voucher.JournalEntries)
            {
                balance += (je.Amount ?? 0) * (je.Dbcr ?? 0);
            }

            Assert.Equal(0, Math.Round(balance, 2));
            Assert.Equal(3, voucher.JournalEntries.Count);
        }
    }
}
