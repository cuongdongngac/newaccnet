using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hddt
{
    /// <summary>
    /// Đọc captcha SVG của cổng hoadondientu.gdt.gov.vn.
    /// Chuyển từ hàm detectSVGCaptcha (modDetectCaptcha) trong bản VBA:
    /// mỗi ký tự là một thẻ &lt;path&gt;; dãy lệnh M/Q/Z của path là "chữ ký" của ký tự,
    /// sắp xếp các ký tự theo toạ độ x của lệnh M đầu tiên.
    /// </summary>
    public static class SvgCaptchaSolver
    {
        // Chữ ký -> ký tự. Captcha của cổng không dùng I, L, O, U, 0, 1.
        private static readonly Dictionary<string, char> Glyphs = new Dictionary<string, char>
        {
            ["MQQQQQZMQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQZMQQZ"] = 'A',
            ["MQQQQQQQQQZMQQQQQQZMQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQZMQQQQQQQQZMQQQQQQQQZ"] = 'B',
            ["MQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'C',
            ["MQQQQQQQQZMQQQQQQQQQQZMQQQQQQQQQQQQQQQZMQQQQQQQZ"] = 'D',
            ["MQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'E',
            ["MQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQZ"] = 'F',
            ["MQQQQQQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'G',
            ["MQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'H',
            ["MQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQZ"] = 'J',
            ["MQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQZ"] = 'K',
            ["MQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'M',
            ["MQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQZ"] = 'N',
            ["MQQQQQQZMQQQQQQQQQQZMQQQQQQQQQQQQQQQZMQQQQQQQQZ"] = 'P',
            ["MQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQZ"] = 'Q',
            ["MQQQQQQZMQQQQQQQQQQQQZMQQQQQQQQQQQQQQQZMQQQQQQQQZ"] = 'R',
            ["MQQQQQQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'S',
            ["MQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQZ"] = 'T',
            ["MQQQQQQQQQQZMQQQQQQQQQQQQQQQQZ"] = 'V',
            ["MQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = 'W',
            ["MQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQZ"] = 'X',
            ["MQQQQQQQQQZMQQQQQQQQQQQQQZ"] = 'Y',
            ["MQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQZ"] = 'Z',
            ["MQQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = '2',
            ["MQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = '3',
            ["MQQQQZMQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQZMQQQQQZ"] = '4',
            ["MQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQZ"] = '5',
            ["MQQQQQQQQQZMQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQZ"] = '6',
            ["MQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQZ"] = '7',
            ["MQQQQQQQQZMQQQQQQQZMQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQZMQQQQQQQZ"] = '8',
            ["MQQQQQQQQZMQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQZ"] = '9',
        };

        private static readonly Regex PathD = new Regex("\\sd=\"([^\"]*)\"", RegexOptions.Compiled);
        private static readonly Regex Command = new Regex("([MQZ])([^MQZ]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex LeadingNumber = new Regex(@"^\s*(-?\d+(?:\.\d+)?)", RegexOptions.Compiled);

        /// <summary>Trả về chuỗi captcha đoán được, hoặc chuỗi rỗng nếu không nhận ra ký tự nào.</summary>
        public static string Solve(string svg)
        {
            if (string.IsNullOrEmpty(svg)) return string.Empty;

            var found = new List<KeyValuePair<double, char>>();
            foreach (Match p in PathD.Matches(svg))
            {
                var d = p.Groups[1].Value;
                var signature = Command.Replace(d, "$1");
                if (!Glyphs.TryGetValue(signature, out var ch)) continue; // path nhiễu

                var first = Command.Match(d);
                var x = 0d;
                var n = LeadingNumber.Match(first.Groups[2].Value);
                if (n.Success) x = double.Parse(n.Groups[1].Value, CultureInfo.InvariantCulture);
                found.Add(new KeyValuePair<double, char>(x, ch));
            }

            return new string(found.OrderBy(f => f.Key).Select(f => f.Value).ToArray());
        }
    }
}
