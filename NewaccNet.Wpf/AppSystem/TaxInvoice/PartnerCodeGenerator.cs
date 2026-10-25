using System;
using System.Collections.Generic;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public static class PartnerCodeGenerator
    {
        private static readonly string[] BusinessPrefixes = new[]
        {
            "cong ty co phan tap doan",
            "cong ty co phan dau tu",
            "cong ty co phan thuong mai",
            "cong ty co phan san xuat",
            "cong ty co phan",
            "cong ty tnhh mtv",
            "cong ty tnhh mot thanh vien",
            "cong ty tnhh tm dv",
            "cong ty tnhh thuong mai dich vu",
            "cong ty tnhh",
            "cong ty lien doanh",
            "cong ty hop danh",
            "cong ty",
            "cty cp",
            "cty tnhh",
            "cty",
            "doanh nghiep tu nhan",
            "dntn",
            "tong cong ty",
            "tap doan",
            "chi nhanh cong ty",
            "chi nhanh"
        };

        /// <summary>
        /// Sinh mã đối tác theo viết tắt tên công ty, tự động bỏ qua các tiền tố như "Công ty Cổ phần", "TNHH", "Chi nhánh"...
        /// Ví dụ: "Công ty Cổ phần Sữa Việt Nam" -> "SVN"
        /// "Công ty TNHH Thép Hòa Phát" -> "THP"
        /// </summary>
        public static string GenerateAcronymPartner(string name, int maxLen = 20)
        {
            if (string.IsNullOrWhiteSpace(name)) return "DT";

            var clean = MaterialCodeGenerator.RemoveDiacritics(name).Trim().ToLowerInvariant();

            foreach (var p in BusinessPrefixes)
            {
                if (clean.StartsWith(p + " "))
                {
                    clean = clean.Substring(p.Length).Trim();
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(clean))
            {
                clean = MaterialCodeGenerator.RemoveDiacritics(name).Trim();
            }

            return MaterialCodeGenerator.GenerateAcronym(clean, maxLen);
        }

        /// <summary>
        /// Sinh mã theo vai trò: Khách hàng -> KH0001, Nhà cung cấp -> NCC0001
        /// </summary>
        public static string GenerateRolePrefixedCode(string role, int nccCounter, int khCounter, int pad = 4, int maxLen = 20)
        {
            if (role == "Khách hàng")
            {
                return MaterialCodeGenerator.GeneratePrefixNumber("KH", khCounter, pad, maxLen);
            }
            return MaterialCodeGenerator.GeneratePrefixNumber("NCC", nccCounter, pad, maxLen);
        }
    }
}
