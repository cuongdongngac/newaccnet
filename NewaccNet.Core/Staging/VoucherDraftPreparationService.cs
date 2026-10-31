using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataAccess.EntityClasses;
using NewaccNet.Contracts;
using NewaccNet.Core.Matching;
using NewaccNet.Core.Posting;
using NewaccNet.TaxInvoice;

namespace NewaccNet.Core.Staging
{
    /// <summary>
    /// Dịch vụ chuẩn bị dữ liệu (Preparation / Staging Service).
    /// Biến hóa đơn điện tử thô thành các bản ghi VoucherDraft hoàn chỉnh,
    /// sẵn sàng cho việc xem xét, sửa đổi và ghi sổ qua WarehousePostingGateway.
    /// </summary>
    public class VoucherDraftPreparationService
    {
        private readonly List<PartnerEntity> _cachedPartners;
        private readonly List<InventoryItemEntity> _cachedMaterials;
        private readonly List<WarehouseEntity> _cachedWarehouses;

        public VoucherDraftPreparationService()
        {
            _cachedPartners = WarehousePostingGateway.LoadPartners();
            _cachedMaterials = WarehousePostingGateway.LoadMaterials();
            _cachedWarehouses = WarehousePostingGateway.LoadWarehouses();
        }

        public VoucherDraftPreparationService(
            List<PartnerEntity> partners,
            List<InventoryItemEntity> materials,
            List<WarehouseEntity> warehouses)
        {
            _cachedPartners = partners;
            _cachedMaterials = materials;
            _cachedWarehouses = warehouses;
        }

        /// <summary>
        /// Chuẩn bị 1 VoucherDraft từ hóa đơn điện tử TaxInvoiceDto.
        /// </summary>
        public VoucherDraft PrepareDraftFromInvoice(TaxInvoiceDto invoice, string? defaultWarehouseId = null)
        {
            var isPurchase = invoice.Direction == "purchase";
            var draft = new VoucherDraft
            {
                DraftType = isPurchase ? DraftType.PurchaseInvoice : DraftType.SalesInvoice,
                Status = DraftStatus.Imported,
                Source = new DraftSourceInfo
                {
                    Kind = "TaxInvoice",
                    Ref = $"{invoice.InvoiceSeries}_{invoice.InvoiceNo}",
                    ImportedAt = DateTime.Now
                },
                Header = new DraftHeader
                {
                    VoucherDate = invoice.InvoiceDate,
                    VoucherSourceCode = isPurchase ? "NM" : "XB",
                    Contents = $"Nhập mua theo HĐ {invoice.InvoiceSeries}-{invoice.InvoiceNo} của {invoice.SellerName ?? invoice.SellerTaxCode}",
                    PersonName = invoice.SellerName ?? string.Empty
                },
                Invoice = new DraftInvoiceInfo
                {
                    Template = invoice.InvoiceTemplate,
                    Series = invoice.InvoiceSeries,
                    Number = invoice.InvoiceNo.ToString(),
                    Date = invoice.InvoiceDate,
                    AmountBeforeTax = invoice.TotalAmountWithoutTax,
                    VatAmount = invoice.TotalTaxAmount,
                    TotalAmount = invoice.TotalAmount
                },
                Settlement = new DraftSettlement
                {
                    CreditAccountId = isPurchase ? "3311" : "1311"
                }
            };

            // Kho mặc định
            var targetWarehouse = defaultWarehouseId;
            if (string.IsNullOrWhiteSpace(targetWarehouse))
            {
                targetWarehouse = _cachedWarehouses.FirstOrDefault()?.WarehouseId ?? "156";
            }

            // 1. So khớp Đối tác
            var partnerTax = isPurchase ? invoice.SellerTaxCode : invoice.BuyerTaxCode;
            var partnerName = isPurchase ? invoice.SellerName : invoice.BuyerName;
            var partnerAddress = isPurchase ? invoice.SellerAddress : invoice.BuyerAddress;

            var partnerMatch = PartnerMatcher.Match(partnerTax, partnerName, partnerAddress, _cachedPartners);
            draft.Counterparty.Raw = new CounterpartyRaw
            {
                TaxCode = partnerTax ?? string.Empty,
                Name = partnerName ?? string.Empty,
                Address = partnerAddress ?? string.Empty
            };
            draft.Counterparty.Resolved = new CounterpartyResolved
            {
                PartnerId = partnerMatch.PartnerId,
                IsNew = partnerMatch.IsNew
            };
            draft.Counterparty.Match = new MatchInfo
            {
                Field = "PartnerId",
                Method = partnerMatch.Method,
                Confidence = partnerMatch.Confidence,
                Reason = partnerMatch.Reason
            };

            if (partnerMatch.Proposed != null)
            {
                draft.ProposedMasterData.Add(partnerMatch.Proposed);
            }

            // 2. So khớp từng dòng hàng
            int lineNo = 1;
            bool hasUnresolvedOrNew = partnerMatch.IsNew;

            if (invoice.Details != null && invoice.Details.Count > 0)
            {
                foreach (var d in invoice.Details)
                {
                    var matMatch = MaterialMatcher.Match(partnerTax, d.ItemName ?? string.Empty, d.UnitName ?? "", targetWarehouse, _cachedMaterials);
                    if (matMatch.IsNew) hasUnresolvedOrNew = true;

                    var draftLine = new DraftLine
                    {
                        LineNo = lineNo++,
                        Raw = new DraftLineRaw
                        {
                            ItemCode = d.ItemCode ?? string.Empty,
                            ItemName = d.ItemName,
                            Unit = d.UnitName ?? string.Empty,
                            Quantity = (decimal)d.Quantity,
                            UnitPrice = d.UnitPrice,
                            Amount = d.Amount,
                            VatRate = ParseVatRate(d.TaxRate),
                            VatAmount = d.TaxAmount
                        },
                        Resolved = new DraftLineResolved
                        {
                            MaterialId = matMatch.MaterialId,
                            WarehouseId = matMatch.WarehouseId,
                            DebitAccountId = matMatch.DebitAccountId,
                            VatAccountId = matMatch.VatAccountId
                        }
                    };

                    draftLine.Matches.Add(new MatchInfo
                    {
                        Field = "MaterialId",
                        Method = matMatch.Method,
                        Confidence = matMatch.Confidence,
                        Reason = matMatch.Reason
                    });

                    if (matMatch.Proposed != null)
                    {
                        draft.ProposedMasterData.Add(matMatch.Proposed);
                    }

                    draft.Lines.Add(draftLine);
                }
            }
            else
            {
                // Hóa đơn không có dòng chi tiết (chỉ có tổng tiền)
                var matMatch = MaterialMatcher.Match(partnerTax, "Hàng hóa dịch vụ tổng hợp", "", targetWarehouse, _cachedMaterials);
                if (matMatch.IsNew) hasUnresolvedOrNew = true;

                var draftLine = new DraftLine
                {
                    LineNo = 1,
                    Raw = new DraftLineRaw
                    {
                        ItemName = "Hàng hóa dịch vụ theo hóa đơn",
                        Quantity = 1,
                        UnitPrice = invoice.TotalAmountWithoutTax,
                        Amount = invoice.TotalAmountWithoutTax,
                        VatRate = invoice.TotalAmountWithoutTax > 0 ? (double)(invoice.TotalTaxAmount / invoice.TotalAmountWithoutTax * 100) : 10,
                        VatAmount = invoice.TotalTaxAmount
                    },
                    Resolved = new DraftLineResolved
                    {
                        MaterialId = matMatch.MaterialId,
                        WarehouseId = matMatch.WarehouseId,
                        DebitAccountId = matMatch.DebitAccountId,
                        VatAccountId = matMatch.VatAccountId
                    }
                };

                draftLine.Matches.Add(new MatchInfo
                {
                    Field = "MaterialId",
                    Method = matMatch.Method,
                    Confidence = matMatch.Confidence,
                    Reason = matMatch.Reason
                });

                if (matMatch.Proposed != null)
                {
                    draft.ProposedMasterData.Add(matMatch.Proposed);
                }

                draft.Lines.Add(draftLine);
            }

            // Đặt trạng thái
            if (hasUnresolvedOrNew)
            {
                draft.Status = DraftStatus.NeedsReview;
                draft.Issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Warning,
                    Code = "HAS_NEW_MASTERDATA",
                    Field = "MasterData",
                    Message = "Có đối tác hoặc vật tư mới cần kiểm tra và duyệt tạo danh mục"
                });
            }
            else
            {
                draft.Status = DraftStatus.Matched;
            }

            draft.Audit.Add(new DraftAuditEntry
            {
                At = DateTime.Now,
                By = "system",
                Action = "Prepared",
                Detail = $"Đã chuẩn bị draft từ HĐ {invoice.InvoiceSeries}-{invoice.InvoiceNo}"
            });

            return draft;
        }

        private static double ParseVatRate(string? taxRateStr)
        {
            if (string.IsNullOrWhiteSpace(taxRateStr)) return 10;
            var clean = taxRateStr.Replace("%", "").Trim();
            if (double.TryParse(clean, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var rate))
            {
                if (rate <= 1.0 && rate > 0) return rate * 100;
                return rate;
            }
            return 10;
        }

        /// <summary>
        /// Chuyển đổi từ VoucherDraft sang tham số ghi sổ của WarehousePostingGateway.
        /// </summary>
        public static WarehouseVoucherSaveParams ConvertDraftToSaveParams(VoucherDraft draft)
        {
            var p = new WarehouseVoucherSaveParams
            {
                VoucherId = draft.PostedVoucherId ?? 0,
                VoucherType = draft.Header.VoucherSourceCode,
                VoucherNo = draft.Header.VoucherNo ?? string.Empty,
                VoucherDate = draft.Header.VoucherDate,
                Description = draft.Header.Contents,
                Carrier = draft.Header.PersonName,

                PartnerId = draft.Counterparty.Resolved.PartnerId,
                PartnerName = draft.Counterparty.Raw.Name,
                TaxCode = draft.Counterparty.Raw.TaxCode,
                Address = draft.Counterparty.Raw.Address,

                VatBillNo = draft.Invoice.Number,
                SerialNo = draft.Invoice.Series,
                VatBillDate = draft.Invoice.Date,

                TotalAccountId = draft.Settlement.CreditAccountId
            };

            // Kho chính
            p.WarehouseId = draft.Lines.FirstOrDefault()?.Resolved.WarehouseId ?? "156";

            foreach (var line in draft.Lines)
            {
                var taxRateId = (int)Math.Round(line.Raw.VatRate);
                var lineItem = new WarehouseVoucherLineItem
                {
                    MaterialId = line.Resolved.MaterialId,
                    MaterialName = line.Raw.ItemName,
                    Unit = line.Raw.Unit,
                    Quantity = (double)line.Raw.Quantity,
                    Price = (double)line.Raw.UnitPrice,
                    Amount = (double)line.Raw.Amount,
                    SellPrice = (double)line.Raw.UnitPrice,
                    SellAmount = (double)line.Raw.Amount,
                    Taxrateid = taxRateId,
                    TaxRate = line.Raw.VatRate,
                    TaxAmount = (double)line.Raw.VatAmount,
                    Note = string.Empty
                };

                p.Lines.Add(lineItem);
            }

            return p;
        }

        /// <summary>
        /// Ghi sổ VoucherDraft trực tiếp vào CSDL thật thông qua WarehousePostingGateway.
        /// </summary>
        public bool PostDraft(VoucherDraft draft, out string errorMessage, out int savedVoucherId)
        {
            errorMessage = string.Empty;
            savedVoucherId = 0;

            if (draft == null)
            {
                errorMessage = "Dữ liệu nháp rỗng.";
                return false;
            }

            var p = ConvertDraftToSaveParams(draft);
            var success = WarehousePostingGateway.SaveVoucher(p, out errorMessage, out savedVoucherId);
            if (success)
            {
                draft.Status = DraftStatus.Posted;
                draft.PostedVoucherId = savedVoucherId;
                draft.Audit.Add(new DraftAuditEntry
                {
                    At = DateTime.Now,
                    By = "user",
                    Action = "Posted",
                    Detail = $"Đã ghi sổ thành công vào CSDL với số CT ID={savedVoucherId}"
                });

                // Tự động học vào sổ tay ghi nhớ
                MappingMemoryStore.Instance.LearnFromDraft(draft);
            }
            else
            {
                draft.Status = DraftStatus.Failed;
                draft.Issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Error,
                    Code = "POST_FAILED",
                    Field = "Posting",
                    Message = errorMessage
                });
            }

            return success;
        }
    }
}
