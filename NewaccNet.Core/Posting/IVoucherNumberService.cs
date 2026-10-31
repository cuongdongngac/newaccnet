using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Core.Data;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Core.Posting
{
    public interface IVoucherNumberService
    {
        /// <summary>
        /// Sinh số hiệu chứng từ tiếp theo dựa trên tiền tố nguồn (ví dụ: NM, XB, PT, PC, PK...) và ngày chứng từ.
        /// </summary>
        string GenerateNextVoucherNo(string prefix, DateTime? voucherDate = null);

        /// <summary>
        /// Sinh số chứng từ tiếp theo dựa trên danh sách các số chứng từ đã có trong lô nhập hiện tại.
        /// (Ví dụ: PK001 -> PK002, CT26101101 -> CT26101102).
        /// </summary>
        string GenerateNextInBatch(IEnumerable<string?> existingNumbers, string defaultPrefix = "CT", int currentCount = 0);
    }

    public class VoucherNumberService : IVoucherNumberService
    {
        public string GenerateNextVoucherNo(string prefix, DateTime? voucherDate = null)
        {
            DateTime date = voucherDate ?? DateTime.Today;
            string datePart = date.ToString("yyMMdd");
            string fullPrefix = $"{prefix}{datePart}";

            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var col = new EntityCollection<JournalVoucherEntity>();
                var bucket = new RelationPredicateBucket(new FieldLikePredicate(JournalVoucherFields.VoucherNo, null, fullPrefix + "%"));
                adapter.FetchEntityCollection(col, bucket);

                int maxSeq = 0;
                foreach (var v in col)
                {
                    if (string.IsNullOrWhiteSpace(v.VoucherNo)) continue;
                    string no = v.VoucherNo.Trim();
                    if (no.StartsWith(fullPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        string suffix = no.Substring(fullPrefix.Length);
                        if (int.TryParse(suffix, out int s))
                        {
                            if (s > maxSeq) maxSeq = s;
                        }
                    }
                }

                int nextSeq = maxSeq + 1;
                string result = $"{fullPrefix}{nextSeq:D2}";
                return result.Length > 10 ? result.Substring(0, 10) : result;
            }
            catch
            {
                // Fallback nếu có lỗi kết nối DB
                return $"{fullPrefix}01";
            }
        }

        public string GenerateNextInBatch(IEnumerable<string?> existingNumbers, string defaultPrefix = "CT", int currentCount = 0)
        {
            // 1. Nếu trong lô đã có chứng từ trước đó có số hiệu, tìm số tiếp theo
            var lastVoucherNo = existingNumbers?
                .Where(no => !string.IsNullOrWhiteSpace(no))
                .LastOrDefault();

            if (!string.IsNullOrWhiteSpace(lastVoucherNo))
            {
                var match = Regex.Match(lastVoucherNo.Trim(), @"^(.*?)(\d+)$");
                if (match.Success)
                {
                    string prefix = match.Groups[1].Value;
                    string digits = match.Groups[2].Value;
                    if (long.TryParse(digits, out long num))
                    {
                        long nextNum = num + 1;
                        string nextDigits = nextNum.ToString().PadLeft(digits.Length, '0');
                        string candidate = prefix + nextDigits;
                        if (candidate.Length <= 10)
                        {
                            return candidate;
                        }
                    }
                }
            }

            // 2. Mặc định theo ngày và số thứ tự (CTyyMMdd + seq:D2, đúng 10 ký tự)
            int seq = (currentCount + 1) % 100;
            if (seq == 0) seq = 1;
            string defaultNo = $"{defaultPrefix}{DateTime.Now:yyMMdd}{seq:D2}";
            return defaultNo.Length > 10 ? defaultNo.Substring(0, 10) : defaultNo;
        }
    }
}
