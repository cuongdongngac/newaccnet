using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Posting
{
    public interface IDraftVoucherBuilder
    {
        DraftType SupportedDraftType { get; }

        /// <summary>
        /// Chuyển đổi một VoucherDraft (JSON) đã được resolve danh mục thành
        /// đồ thị thực thể kế toán JournalVoucherEntity đầy đủ trên RAM.
        /// </summary>
        JournalVoucherEntity Build(VoucherDraft draft);
    }

    public interface IDraftVoucherBuilderFactory
    {
        IDraftVoucherBuilder GetBuilder(DraftType draftType);
    }

    public class DraftVoucherBuilderFactory : IDraftVoucherBuilderFactory
    {
        private readonly Dictionary<DraftType, IDraftVoucherBuilder> _builders = new();

        public DraftVoucherBuilderFactory(IEnumerable<IDraftVoucherBuilder>? builders = null)
        {
            if (builders != null)
            {
                foreach (var b in builders)
                {
                    _builders[b.SupportedDraftType] = b;
                }
            }

            // Đăng ký builder mặc định nếu chưa có
            if (!_builders.ContainsKey(DraftType.PurchaseInvoice))
            {
                _builders[DraftType.PurchaseInvoice] = new PurchaseInvoiceVoucherBuilder();
            }
        }

        public IDraftVoucherBuilder GetBuilder(DraftType draftType)
        {
            if (_builders.TryGetValue(draftType, out var builder))
            {
                return builder;
            }

            throw freshNotSupportedException(draftType);
        }

        private static NotSupportedException freshNotSupportedException(DraftType draftType)
        {
            return new NotSupportedException($"Chưa có DraftVoucherBuilder hỗ trợ loại chứng từ '{draftType}'.");
        }
    }

    /// <summary>
    /// Builder sinh chứng từ Nhập kho mua hàng (NM):
    /// - Bút toán Nợ 15x / Có 331 (kèm InventoryVoucher và DebtDetail)
    /// - Bút toán Nợ 1331 / Có 331 (kèm VatEntry và DebtDetail)
    /// </summary>
    public class PurchaseInvoiceVoucherBuilder : IDraftVoucherBuilder
    {
        public DraftType SupportedDraftType => DraftType.PurchaseInvoice;

        public JournalVoucherEntity Build(VoucherDraft draft)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            var voucher = new JournalVoucherEntity
            {
                VoucherDate = draft.Header.VoucherDate,
                VoucherNo = draft.Header.VoucherNo ?? string.Empty,
                Invoicesnumber = "01", // Số lượng hóa đơn kèm theo chứng từ (max length = 2)
                Contents = !string.IsNullOrWhiteSpace(draft.Header.Contents) 
                    ? draft.Header.Contents 
                    : $"Mua hàng theo HĐ số {draft.Invoice.Number} - {draft.Counterparty.Raw.Name}",
                Personname = !string.IsNullOrWhiteSpace(draft.Header.PersonName)
                    ? draft.Header.PersonName
                    : draft.Counterparty.Raw.Name,
                Personaddress = draft.Counterparty.Raw.Address
            };

            string partnerId = draft.Counterparty.Resolved.PartnerId;
            string creditAcc = !string.IsNullOrWhiteSpace(draft.Settlement.CreditAccountId) ? draft.Settlement.CreditAccountId : "3311";

            decimal totalGoodsAmount = draft.Lines.Sum(l => l.Raw.Amount);
            decimal totalVatAmount = draft.Lines.Sum(l => l.Raw.VatAmount);
            if (totalVatAmount == 0 && draft.Invoice.VatAmount > 0)
            {
                totalVatAmount = draft.Invoice.VatAmount;
            }

            decimal grandTotal = totalGoodsAmount + totalVatAmount;

            // 1. Dòng Có 331 (Tổng tiền phải trả người bán)
            var creditJe = new JournalEntryEntity
            {
                AccountId = creditAcc,
                Dbcr = -1, // Có
                Amount = (double)grandTotal
            };

            if (!string.IsNullOrWhiteSpace(partnerId))
            {
                var debt = new DebtDetailEntity
                {
                    PartnerId = partnerId,
                    Amount = (double)grandTotal,
                    Liabilitydate = draft.Settlement.DueDate ?? draft.Header.VoucherDate
                };
                creditJe.DebtDetails.Add(debt);
            }
            voucher.JournalEntries.Add(creditJe);

            // 2. Gom các dòng nhập kho theo từng Kho và từng TK Nợ (1561, 152...)
            var groupedLines = draft.Lines
                .GroupBy(l => new { 
                    AccountId = !string.IsNullOrWhiteSpace(l.Resolved.DebitAccountId) ? l.Resolved.DebitAccountId : "1561", 
                    WarehouseId = !string.IsNullOrWhiteSpace(l.Resolved.WarehouseId) ? l.Resolved.WarehouseId : "KHO1" 
                });

            foreach (var group in groupedLines)
            {
                decimal groupAmount = group.Sum(l => l.Raw.Amount);
                var debitJe = new JournalEntryEntity
                {
                    AccountId = group.Key.AccountId,
                    Dbcr = 1, // Nợ
                    Amount = (double)groupAmount
                };

                // Tạo phiếu kho đi kèm (InventoryVoucherEntity)
                var iv = new InventoryVoucherEntity
                {
                    WarehouseId = group.Key.WarehouseId,
                    Taxflag = true,
                    Taxno = draft.Counterparty.Raw.TaxCode,
                    Vatobjectid = partnerId,
                    Vatobjectname = draft.Counterparty.Raw.Name,
                    Vataddress = draft.Counterparty.Raw.Address,
                    Vatbillno = draft.Invoice.Number,
                    Serialno = draft.Invoice.Series,
                    Vatbilldate = draft.Invoice.Date,
                    Vatmaterialdes = voucher.Contents
                };

                foreach (var line in group)
                {
                    var ivl = new InventoryVoucherLineEntity
                    {
                        MaterialId = line.Resolved.MaterialId,
                        Quantity = (double)line.Raw.Quantity,
                        Price = (double)line.Raw.UnitPrice
                    };
                    iv.InventoryVoucherLines.Add(ivl);
                }

                debitJe.InventoryVouchers.Add(iv);
                voucher.JournalEntries.Add(debitJe);
            }

            // 3. Dòng Nợ 1331 (Thuế GTGT đầu vào được khấu trừ) nếu có thuế
            if (totalVatAmount > 0)
            {
                string vatAcc = draft.Lines.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l.Resolved.VatAccountId))?.Resolved.VatAccountId ?? "13311";
                var vatJe = new JournalEntryEntity
                {
                    AccountId = vatAcc,
                    Dbcr = 1, // Nợ
                    Amount = (double)totalVatAmount
                };

                var vatEntry = new VatEntryEntity
                {
                    Taxno = draft.Counterparty.Raw.TaxCode,
                    Vatobjectid = partnerId,
                    Vatobjectname = draft.Counterparty.Raw.Name,
                    Vatbillno = draft.Invoice.Number,
                    Serialno = draft.Invoice.Series,
                    Vatbilldate = draft.Invoice.Date,
                    Beforetaxamount = (double)totalGoodsAmount,
                    Amount = (double)totalVatAmount,
                    Vatdescription = voucher.Contents
                };
                vatJe.VatEntries.Add(vatEntry);

                voucher.JournalEntries.Add(vatJe);
            }

            return voucher;
        }
    }
}
