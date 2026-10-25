using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public enum MaterialCodeRule
    {
        Acronym,          // 1. Viết tắt chữ cái đầu (VD: Xi măng PCB40 -> XMHTPCB40)
        PrefixNumber,     // 2. Tiền tố + Số tự tăng (VD: VT0001, VT0002...)
        InvoiceItemCode,  // 3. Mã hàng trên HĐĐT (nếu rỗng thì sinh theo viết tắt)
        CondensedName     // 4. Tên viết liền không dấu (VD: XIMANGPCB40)
    }

    public static class MaterialCodeGenerator
    {
        /// <summary>
        /// Chuyển đổi chuỗi tiếng Việt có dấu thành không dấu.
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            var result = sb.ToString().Normalize(NormalizationForm.FormC);
            result = result.Replace('đ', 'd').Replace('Đ', 'D');
            return result;
        }

        /// <summary>
        /// Quy tắc 1: Viết tắt chữ cái đầu từng từ, giữ nguyên số/mã hiệu, giới hạn tối đa maxLen (20 ký tự).
        /// VD: "Xi măng Hoàng Thạch PCB40" -> "XMHTPCB40"
        /// "Cát vàng hạt lớn phi 10" -> "CVHLP10"
        /// </summary>
        public static string GenerateAcronym(string name, int maxLen = 20)
        {
            if (string.IsNullOrWhiteSpace(name)) return "VT";

            var clean = RemoveDiacritics(name).Trim();
            var parts = Regex.Split(clean, @"[\s\-_,./;:()\[\]{}+*]+");
            var sb = new StringBuilder();

            foreach (var p in parts)
            {
                if (string.IsNullOrWhiteSpace(p)) continue;

                // Nếu từ hoàn toàn là số
                if (Regex.IsMatch(p, @"^\d+$"))
                {
                    sb.Append(p);
                }
                // Nếu từ viết hoa toàn bộ và ngắn (ví dụ: "PCB", "DN", "ISO", "PVC")
                else if (p.Length <= 4 && p.ToUpperInvariant() == p && Regex.IsMatch(p, @"^[A-Z0-9]+$"))
                {
                    sb.Append(p);
                }
                else
                {
                    // Lấy ký tự đầu tiên
                    char first = char.ToUpperInvariant(p[0]);
                    if (char.IsLetterOrDigit(first))
                    {
                        sb.Append(first);
                    }
                    // Nếu trong từ có số ở đuôi (ví dụ "PCB40" hay "M10")
                    var matchDigits = Regex.Match(p, @"\d+");
                    if (matchDigits.Success && matchDigits.Index > 0)
                    {
                        sb.Append(matchDigits.Value);
                    }
                }

                if (sb.Length >= maxLen) break;
            }

            var code = sb.ToString();
            if (string.IsNullOrWhiteSpace(code)) code = "VT";
            if (code.Length > maxLen) code = code.Substring(0, maxLen);
            return code.ToUpperInvariant();
        }

        /// <summary>
        /// Quy tắc 2: Tiền tố + Số thứ tự tự tăng (VD: VT0001, VT0002...)
        /// </summary>
        public static string GeneratePrefixNumber(string prefix, int index, int pad = 4, int maxLen = 20)
        {
            var p = string.IsNullOrWhiteSpace(prefix) ? "VT" : prefix.Trim().ToUpperInvariant();
            var num = index.ToString().PadLeft(pad, '0');
            var full = p + num;
            if (full.Length > maxLen) full = full.Substring(0, maxLen);
            return full;
        }

        /// <summary>
        /// Quy tắc 4: Tên viết liền không dấu (VD: "Xi Măng" -> "XIMANG")
        /// </summary>
        public static string GenerateCondensed(string name, int maxLen = 20)
        {
            if (string.IsNullOrWhiteSpace(name)) return "VT";
            var clean = RemoveDiacritics(name);
            var alphanum = Regex.Replace(clean, @"[^a-zA-Z0-9]", "").ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(alphanum)) alphanum = "VT";
            if (alphanum.Length > maxLen) alphanum = alphanum.Substring(0, maxLen);
            return alphanum;
        }

        /// <summary>
        /// Làm sạch mã vật tư (chỉ giữ ký tự an toàn, không quá maxLen ký tự).
        /// </summary>
        public static string SanitizeCode(string rawCode, int maxLen = 20)
        {
            if (string.IsNullOrWhiteSpace(rawCode)) return "VT";
            var clean = RemoveDiacritics(rawCode);
            var safe = Regex.Replace(clean, @"[^a-zA-Z0-9_\-]", "").ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(safe)) safe = "VT";
            if (safe.Length > maxLen) safe = safe.Substring(0, maxLen);
            return safe;
        }

        /// <summary>
        /// Đảm bảo mã không bị trùng lặp với danh sách đã sử dụng. Nếu trùng, tự động gắn hậu tố -1, -2...
        /// </summary>
        public static string MakeUniqueCode(string baseCode, HashSet<string> existingCodes, int maxLen = 20)
        {
            if (!existingCodes.Contains(baseCode))
            {
                existingCodes.Add(baseCode);
                return baseCode;
            }

            int counter = 1;
            while (true)
            {
                var suffix = "-" + counter.ToString();
                var allowedBaseLen = maxLen - suffix.Length;
                var trimmedBase = baseCode.Length > allowedBaseLen ? baseCode.Substring(0, allowedBaseLen) : baseCode;
                var candidate = trimmedBase + suffix;

                if (!existingCodes.Contains(candidate))
                {
                    existingCodes.Add(candidate);
                    return candidate;
                }
                counter++;
            }
        }
    }
}
