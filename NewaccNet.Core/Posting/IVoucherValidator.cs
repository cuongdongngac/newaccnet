using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Contracts;
using NewaccNet.Core.Data;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Core.Posting
{
    public interface IVoucherValidator
    {
        /// <summary>
        /// Kiểm tra tính hợp lệ của JournalVoucherEntity trên bộ nhớ trước khi lưu vào CSDL.
        /// Không bật MessageBox, trả về danh sách vấn đề (DraftIssue).
        /// </summary>
        List<DraftIssue> ValidateEntity(JournalVoucherEntity voucher, int newVouchersCountInBatch = 1);

        /// <summary>
        /// Kiểm tra tính hợp lệ của toàn bộ lô chứng từ trước khi mở Transaction lưu.
        /// </summary>
        List<DraftIssue> ValidateBatch(IReadOnlyList<JournalVoucherEntity> vouchers);

        /// <summary>
        /// Kiểm tra tính hợp lệ của bản nháp (VoucherDraft) trên Lớp đệm (Staging).
        /// </summary>
        List<DraftIssue> ValidateDraft(VoucherDraft draft);
    }

    public class VoucherValidator : IVoucherValidator
    {
        public List<DraftIssue> ValidateEntity(JournalVoucherEntity voucher, int newVouchersCountInBatch = 1)
        {
            var issues = new List<DraftIssue>();

            if (voucher == null)
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Error,
                    Code = "NULL_VOUCHER",
                    Field = nameof(voucher),
                    Message = "Chứng từ không tồn tại hoặc bị rỗng."
                });
                return issues;
            }

            // 1. Kiểm tra ngày chứng từ và ngày khóa sổ / hạn dùng
            if (voucher.VoucherDate.HasValue)
            {
                if (!DbConfigService.CanUpdateVoucher(voucher.VoucherDate.Value, out string warningMsg))
                {
                    issues.Add(new DraftIssue
                    {
                        Severity = IssueSeverity.Error,
                        Code = "PERIOD_LOCKED_OR_EXPIRED",
                        Field = nameof(voucher.VoucherDate),
                        Message = warningMsg
                    });
                }
            }
            else
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Error,
                    Code = "MISSING_VOUCHER_DATE",
                    Field = nameof(voucher.VoucherDate),
                    Message = "Ngày chứng từ không được để trống."
                });
            }

            // 2. Kiểm tra cân đối Nợ = Có
            double balance = 0;
            if (voucher.JournalEntries != null && voucher.JournalEntries.Count > 0)
            {
                foreach (var je in voucher.JournalEntries)
                {
                    balance += (je.Amount ?? 0) * (je.Dbcr ?? 0);
                }

                if (Math.Round(balance, 0) != 0)
                {
                    issues.Add(new DraftIssue
                    {
                        Severity = IssueSeverity.Error,
                        Code = "UNBALANCED_ENTRIES",
                        Field = nameof(voucher.JournalEntries),
                        Message = $"Chứng từ số '{voucher.VoucherNo}' chưa cân đối Nợ - Có (Chênh lệch: {balance:N0} đ)."
                    });
                }
            }
            else
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Error,
                    Code = "EMPTY_JOURNAL_ENTRIES",
                    Field = nameof(voucher.JournalEntries),
                    Message = $"Chứng từ số '{voucher.VoucherNo}' chưa có dòng hạch toán nào."
                });
            }

            // 3. Kiểm tra hạn mức số lượng chứng từ bản quyền (chỉ áp dụng cho chứng từ mới)
            if (voucher.Id == 0 && newVouchersCountInBatch > 0)
            {
                if (!DbConfigService.CheckRecordLimit(out int count, out int maxLimit, out string warningMsg, newVouchersCountInBatch))
                {
                    issues.Add(new DraftIssue
                    {
                        Severity = IssueSeverity.Error,
                        Code = "RECORD_LIMIT_EXCEEDED",
                        Field = "License",
                        Message = warningMsg
                    });
                }
            }

            return issues;
        }

        public List<DraftIssue> ValidateBatch(IReadOnlyList<JournalVoucherEntity> vouchers)
        {
            var issues = new List<DraftIssue>();
            if (vouchers == null || vouchers.Count == 0)
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Warning,
                    Code = "EMPTY_BATCH",
                    Message = "Danh sách chứng từ cần lưu trống."
                });
                return issues;
            }

            int newVouchersCount = vouchers.Count(v => v.Id == 0);

            for (int i = 0; i < vouchers.Count; i++)
            {
                var v = vouchers[i];
                var singleIssues = ValidateEntity(v, newVouchersCount);
                foreach (var issue in singleIssues)
                {
                    issue.Message = $"[Phiếu {i + 1}/{vouchers.Count}] {issue.Message}";
                    issues.Add(issue);
                }
            }

            return issues;
        }

        public List<DraftIssue> ValidateDraft(VoucherDraft draft)
        {
            var issues = new List<DraftIssue>();

            if (draft == null)
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Error,
                    Code = "NULL_DRAFT",
                    Message = "Bản nháp không tồn tại."
                });
                return issues;
            }

            if (string.IsNullOrWhiteSpace(draft.Counterparty.Resolved.PartnerId))
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Warning,
                    Code = "UNRESOLVED_PARTNER",
                    Field = "Counterparty.Resolved.PartnerId",
                    Message = $"Chưa xác định được mã đối tượng cho bên bán: '{draft.Counterparty.Raw.Name}' (MST: {draft.Counterparty.Raw.TaxCode})."
                });
            }

            if (draft.Lines == null || draft.Lines.Count == 0)
            {
                issues.Add(new DraftIssue
                {
                    Severity = IssueSeverity.Error,
                    Code = "NO_LINES",
                    Field = nameof(draft.Lines),
                    Message = "Hóa đơn/Chứng từ nháp không có dòng hàng nào."
                });
            }
            else
            {
                for (int i = 0; i < draft.Lines.Count; i++)
                {
                    var line = draft.Lines[i];
                    if (string.IsNullOrWhiteSpace(line.Resolved.MaterialId))
                    {
                        issues.Add(new DraftIssue
                        {
                            Severity = IssueSeverity.Warning,
                            Code = "UNRESOLVED_MATERIAL",
                            Field = $"Lines[{i}].Resolved.MaterialId",
                            Message = $"Dòng {i + 1} ({line.Raw.ItemName}) chưa được ánh xạ mã vật tư/dịch vụ."
                        });
                    }

                    if (string.IsNullOrWhiteSpace(line.Resolved.DebitAccountId))
                    {
                        issues.Add(new DraftIssue
                        {
                            Severity = IssueSeverity.Warning,
                            Code = "UNRESOLVED_DEBIT_ACCOUNT",
                            Field = $"Lines[{i}].Resolved.DebitAccountId",
                            Message = $"Dòng {i + 1} ({line.Raw.ItemName}) chưa có tài khoản Nợ."
                        });
                    }
                }
            }

            return issues;
        }
    }
}
