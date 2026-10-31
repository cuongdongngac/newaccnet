using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAccess.EntityClasses;
using NewaccNet.Contracts;

namespace NewaccNet.Core.Posting
{
    /// <summary>
    /// Cổng ghi sổ duy nhất (Single Source of Truth) vào CSDL thật.
    /// Mọi chứng từ (Nhập tay trên form, Import Excel, Hóa đơn điện tử, AI gợi ý)
    /// đều phải đi qua Cổng này.
    /// </summary>
    public interface IPostingGateway
    {
        /// <summary>
        /// Ghi sổ một VoucherDraft đã được người dùng duyệt từ Lớp đệm (Staging) vào CSDL.
        /// </summary>
        Task<PostingResult> PostDraftAsync(VoucherDraft draft, CancellationToken ct = default);

        /// <summary>
        /// Ghi sổ một chứng từ 3 tầng hoàn chỉnh (AccountingVoucherThreeTierDraft - Vòng đời 3) vào CSDL.
        /// </summary>
        Task<PostingResult> PostThreeTierDraftAsync(AccountingVoucherThreeTierDraft threeTierDraft, CancellationToken ct = default);

        /// <summary>
        /// Ghi sổ một chứng từ JournalVoucherEntity (ví dụ từ form nhập tay VoucherEntryWindow).
        /// </summary>
        Task<PostingResult> PostVoucherAsync(JournalVoucherEntity voucher, CancellationToken ct = default);

        /// <summary>
        /// Ghi sổ cả một lô JournalVoucherEntity trong 1 Transaction duy nhất.
        /// </summary>
        Task<PostingResult> PostBatchAsync(IReadOnlyList<JournalVoucherEntity> vouchers, IReadOnlyList<JournalVoucherEntity>? deletedVouchers = null, CancellationToken ct = default);
    }

    public class PostingGateway : IPostingGateway
    {
        private readonly IVoucherPostingService _postingService;
        private readonly IVoucherValidator _validator;
        private readonly IVoucherNumberService _numberService;
        private readonly IDraftVoucherBuilderFactory _builderFactory;

        public PostingGateway(
            IVoucherPostingService? postingService = null,
            IVoucherValidator? validator = null,
            IVoucherNumberService? numberService = null,
            IDraftVoucherBuilderFactory? builderFactory = null)
        {
            _validator = validator ?? new VoucherValidator();
            _postingService = postingService ?? new VoucherPostingService(_validator);
            _numberService = numberService ?? new VoucherNumberService();
            _builderFactory = builderFactory ?? new DraftVoucherBuilderFactory();
        }

        public async Task<PostingResult> PostDraftAsync(VoucherDraft draft, CancellationToken ct = default)
        {
            if (draft == null)
            {
                return new PostingResult { Success = false, ErrorMessage = "Bản nháp rỗng." };
            }

            // 1. Kiểm tra trạng thái: Phải được người dùng duyệt (Approved)
            if (draft.Status != DraftStatus.Approved && draft.Status != DraftStatus.Matched)
            {
                return new PostingResult
                {
                    Success = false,
                    ErrorMessage = $"Bản nháp đang ở trạng thái '{draft.Status}', chưa thể ghi sổ. Hãy chuyển sang 'Approved' trước."
                };
            }

            // 2. Kiểm tra tính hợp lệ của Draft (thiếu mã, thiếu dòng hàng...)
            var draftIssues = _validator.ValidateDraft(draft);
            var blockingIssues = draftIssues.Where(i => i.Severity == IssueSeverity.Error).ToList();
            if (blockingIssues.Count > 0)
            {
                draft.Status = DraftStatus.NeedsReview;
                draft.Issues.AddRange(draftIssues);
                return new PostingResult
                {
                    Success = false,
                    Issues = draftIssues,
                    ErrorMessage = string.Join("\n", blockingIssues.Select(b => b.Message))
                };
            }

            // 3. Sử dụng Builder tương ứng để sinh đồ thị thực thể kế toán trên RAM
            JournalVoucherEntity voucher;
            try
            {
                var builder = _builderFactory.GetBuilder(draft.DraftType);
                voucher = builder.Build(draft);
            }
            catch (Exception ex)
            {
                draft.Status = DraftStatus.Failed;
                return new PostingResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi khi khởi tạo chứng từ từ bản nháp: " + ex.Message
                };
            }

            // 4. Tự động sinh số chứng từ tiếp theo nếu chưa có
            if (string.IsNullOrWhiteSpace(voucher.VoucherNo))
            {
                voucher.VoucherNo = _numberService.GenerateNextVoucherNo(draft.Header.VoucherSourceCode, voucher.VoucherDate);
                draft.Header.VoucherNo = voucher.VoucherNo;
            }

            // 5. Ghi sổ vào DB qua IVoucherPostingService
            var postResult = await _postingService.PostVoucherAsync(voucher, ct);

            if (postResult.Success)
            {
                draft.Status = DraftStatus.Posted;
                draft.PostedVoucherId = postResult.VoucherId;
                draft.Audit.Add(new DraftAuditEntry
                {
                    Action = "Posted",
                    By = "PostingGateway",
                    Detail = $"Đã ghi sổ thành công vào chứng từ số {postResult.VoucherNo} (ID: {postResult.VoucherId})"
                });
            }
            else
            {
                draft.Status = DraftStatus.Failed;
                draft.Issues.AddRange(postResult.Issues);
            }

            return postResult;
        }

        public async Task<PostingResult> PostThreeTierDraftAsync(AccountingVoucherThreeTierDraft threeTierDraft, CancellationToken ct = default)
        {
            if (threeTierDraft == null)
            {
                return new PostingResult { Success = false, ErrorMessage = "Chứng từ 3 tầng rỗng." };
            }

            var top = threeTierDraft.TopVoucher;
            var voucher = new JournalVoucherEntity
            {
                VoucherDate = top.VoucherDate,
                VoucherNo = top.VoucherNo ?? string.Empty,
                Invoicesnumber = "01",
                Contents = !string.IsNullOrWhiteSpace(top.Contents) ? top.Contents : $"Giao dịch với {top.PartnerName}",
                Personname = top.PartnerName,
                Personaddress = top.Address
            };

            // Sinh số chứng từ nếu thiếu
            if (string.IsNullOrWhiteSpace(voucher.VoucherNo))
            {
                voucher.VoucherNo = _numberService.GenerateNextVoucherNo(top.VoucherSourceCode, voucher.VoucherDate);
                top.VoucherNo = voucher.VoucherNo;
            }

            // 1. Dòng Có (Thanh toán / Công nợ)
            string creditAcc = !string.IsNullOrWhiteSpace(top.SettlementAccountId) ? top.SettlementAccountId : "3311";
            var creditJe = new JournalEntryEntity
            {
                AccountId = creditAcc,
                Dbcr = -1,
                Amount = (double)top.TotalAmount
            };

            // Nếu là giao dịch công nợ thì gắn đối tác vào chi tiết 131/331
            if (top.IsDebtTransaction && !string.IsNullOrWhiteSpace(top.PartnerId))
            {
                creditJe.DebtDetails.Add(new DebtDetailEntity
                {
                    PartnerId = top.PartnerId,
                    Amount = (double)top.TotalAmount,
                    Liabilitydate = top.DueDate ?? top.VoucherDate
                });
            }
            voucher.JournalEntries.Add(creditJe);

            // 2. Các dòng Nợ (Vật tư / Kho / Chi phí)
            var groups = threeTierDraft.Details.GroupBy(d => new
            {
                AccountId = !string.IsNullOrWhiteSpace(d.DebitAccountId) ? d.DebitAccountId : "1561",
                WarehouseId = !string.IsNullOrWhiteSpace(d.WarehouseId) ? d.WarehouseId : (threeTierDraft.InventoryVoucher?.DefaultWarehouseId ?? "KHO1")
            });

            foreach (var g in groups)
            {
                decimal gAmount = g.Sum(x => x.Amount);
                var debitJe = new JournalEntryEntity
                {
                    AccountId = g.Key.AccountId,
                    Dbcr = 1,
                    Amount = (double)gAmount
                };

                // Nếu có phân hệ kho
                if (threeTierDraft.InventoryVoucher != null || g.Any(x => !string.IsNullOrWhiteSpace(x.MaterialId)))
                {
                    var iv = new InventoryVoucherEntity
                    {
                        WarehouseId = g.Key.WarehouseId,
                        Taxflag = true,
                        Taxno = top.TaxCode,
                        Vatobjectid = top.PartnerId,
                        Vatobjectname = top.PartnerName,
                        Vataddress = top.Address,
                        Vatbillno = top.InvoiceNumber ?? string.Empty,
                        Serialno = top.InvoiceSeries ?? string.Empty,
                        Vatbilldate = top.InvoiceDate ?? top.VoucherDate,
                        Vatmaterialdes = voucher.Contents
                    };

                    foreach (var item in g)
                    {
                        iv.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                        {
                            MaterialId = item.MaterialId,
                            Quantity = (double)item.Quantity,
                            Price = (double)item.UnitPrice
                        });
                    }

                    debitJe.InventoryVouchers.Add(iv);
                }

                voucher.JournalEntries.Add(debitJe);
            }

            // 3. Dòng Nợ Thuế GTGT (nếu có)
            if (top.TotalTaxAmount > 0)
            {
                string vatAcc = threeTierDraft.Details.FirstOrDefault(d => !string.IsNullOrWhiteSpace(d.VatDebitAccountId))?.VatDebitAccountId ?? "13311";
                var vatJe = new JournalEntryEntity
                {
                    AccountId = vatAcc,
                    Dbcr = 1,
                    Amount = (double)top.TotalTaxAmount
                };

                vatJe.VatEntries.Add(new VatEntryEntity
                {
                    Taxno = top.TaxCode,
                    Vatobjectid = top.PartnerId,
                    Vatobjectname = top.PartnerName,
                    Vatbillno = top.InvoiceNumber ?? string.Empty,
                    Serialno = top.InvoiceSeries ?? string.Empty,
                    Vatbilldate = top.InvoiceDate ?? top.VoucherDate,
                    Beforetaxamount = (double)top.TotalAmountWithoutTax,
                    Amount = (double)top.TotalTaxAmount,
                    Vatdescription = voucher.Contents
                });

                voucher.JournalEntries.Add(vatJe);
            }

            // Ghi sổ qua service
            var result = await _postingService.PostVoucherAsync(voucher, ct);
            if (result.Success)
            {
                threeTierDraft.Stage = DraftLifecycleStage.Stage4_Posted;
            }

            return result;
        }

        public Task<PostingResult> PostVoucherAsync(JournalVoucherEntity voucher, CancellationToken ct = default)
        {
            return _postingService.PostVoucherAsync(voucher, ct);
        }

        public Task<PostingResult> PostBatchAsync(IReadOnlyList<JournalVoucherEntity> vouchers, IReadOnlyList<JournalVoucherEntity>? deletedVouchers = null, CancellationToken ct = default)
        {
            return _postingService.PostBatchAsync(vouchers, deletedVouchers, ct);
        }
    }
}
