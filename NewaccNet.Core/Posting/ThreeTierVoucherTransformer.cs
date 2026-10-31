using System;
using System.Linq;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Posting
{
    /// <summary>
    /// Chuyển đổi dữ liệu từ Vòng đời 2 (Chuẩn hóa danh mục) sang Vòng đời 3 (DTO Chứng từ 3 tầng hoàn chỉnh).
    /// Tầng Top: Chứng từ sổ cái (Journal Voucher) kèm MST/Tên đối tác công nợ 131/331.
    /// Tầng Giữa: Phiếu kho (Inventory Voucher) - Quản lý nhập/xuất kho vật lý.
    /// Tầng Dưới: Chi tiết định khoản (Nợ TK Kho/Chi phí, Có TK Công nợ/Tiền, Thuế GTGT).
    /// </summary>
    public static class ThreeTierVoucherTransformer
    {
        public static AccountingVoucherThreeTierDraft Transform(VoucherDraft draft)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            var result = new AccountingVoucherThreeTierDraft
            {
                Stage = DraftLifecycleStage.Stage3_VoucherReady
            };

            // 1. TẦNG TOP (Chứng từ sổ cái)
            var partnerId = draft.Counterparty.Resolved.PartnerId;
            var taxCode = draft.Counterparty.Raw.TaxCode;
            var partnerName = draft.Counterparty.Raw.Name;

            bool isDebt = string.Equals(draft.Settlement.CreditAccountId, "331", StringComparison.OrdinalIgnoreCase) ||
                          draft.Settlement.CreditAccountId.StartsWith("331", StringComparison.OrdinalIgnoreCase) ||
                          draft.Settlement.CreditAccountId.StartsWith("131", StringComparison.OrdinalIgnoreCase);

            decimal totalAmountWithoutTax = draft.Lines.Sum(l => l.Raw.Amount);
            decimal totalTaxAmount = draft.Lines.Sum(l => l.Raw.VatAmount);
            decimal totalAmount = totalAmountWithoutTax + totalTaxAmount;

            result.TopVoucher = new TopVoucherTier
            {
                VoucherDate = draft.Header.VoucherDate,
                VoucherSourceCode = draft.Header.VoucherSourceCode,
                VoucherNo = draft.Header.VoucherNo,
                Contents = draft.Header.Contents,
                PartnerId = partnerId,
                PartnerName = partnerName,
                TaxCode = taxCode,
                Address = draft.Counterparty.Raw.Address,
                InvoiceTemplate = draft.Invoice.Template,
                InvoiceSeries = draft.Invoice.Series,
                InvoiceNumber = draft.Invoice.Number,
                InvoiceDate = draft.Invoice.Date,
                TotalAmountWithoutTax = totalAmountWithoutTax,
                TotalTaxAmount = totalTaxAmount,
                TotalAmount = totalAmount,
                IsDebtTransaction = isDebt,
                SettlementAccountId = draft.Settlement.CreditAccountId,
                DueDate = draft.Settlement.DueDate
            };

            // 2. TẦNG GIỮA (Phiếu kho - nếu có dòng hàng nhập/xuất kho)
            bool hasInventoryMovement = draft.Lines.Any(l => !string.IsNullOrWhiteSpace(l.Resolved.WarehouseId) ||
                                                            !string.IsNullOrWhiteSpace(l.Resolved.MaterialId));
            if (hasInventoryMovement)
            {
                string firstWarehouse = draft.Lines.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l.Resolved.WarehouseId))?.Resolved.WarehouseId ?? "KHO_TONG";
                result.InventoryVoucher = new MiddleInventoryTier
                {
                    InventoryVoucherNo = draft.Header.VoucherNo ?? string.Empty,
                    InventoryDate = draft.Header.VoucherDate,
                    MovementType = draft.DraftType == DraftType.PurchaseInvoice ? "NK" : "XK",
                    DefaultWarehouseId = firstWarehouse,
                    ReceiverOrDeliverer = draft.Header.PersonName,
                    Note = draft.Header.Contents
                };
            }

            // 3. TẦNG DƯỚI (Chi tiết định khoản & dòng vật tư)
            int lineNo = 1;
            foreach (var line in draft.Lines)
            {
                var detail = new DetailLineTier
                {
                    LineNo = lineNo++,
                    MaterialId = line.Resolved.MaterialId,
                    MaterialName = string.IsNullOrWhiteSpace(line.Resolved.MaterialId) ? line.Raw.ItemName : line.Resolved.MaterialId,
                    Unit = line.Raw.Unit,
                    WarehouseId = line.Resolved.WarehouseId,
                    Quantity = line.Raw.Quantity,
                    UnitPrice = line.Raw.UnitPrice,
                    Amount = line.Raw.Amount,
                    DebitAccountId = line.Resolved.DebitAccountId,
                    CreditAccountId = draft.Settlement.CreditAccountId,
                    VatRate = line.Raw.VatRate,
                    VatAmount = line.Raw.VatAmount,
                    VatDebitAccountId = line.Resolved.VatAccountId,
                    LineNote = line.Raw.ItemName,
                    Match = line.Matches.FirstOrDefault() ?? new MatchInfo()
                };

                // Nếu là giao dịch công nợ, lấy thông tin MST/PartnerId gắn vào chi tiết để theo dõi sổ công nợ chi tiết (131/331)
                if (isDebt)
                {
                    detail.DebtPartnerId = partnerId;
                    detail.DebtPartnerTaxCode = taxCode;
                }

                result.Details.Add(detail);
            }

            return result;
        }
    }
}
