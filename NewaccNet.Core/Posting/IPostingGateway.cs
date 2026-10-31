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
