using System;
using System.Collections.Generic;
using System.Text;

namespace Hddt
{
    /// <summary>Hóa đơn mua vào hay bán ra.</summary>
    public enum InvoiceDirection
    {
        /// <summary>Mua vào (api .../invoices/purchase)</summary>
        Purchase,
        /// <summary>Bán ra (api .../invoices/sold)</summary>
        Sold,
    }

    /// <summary>Nguồn dữ liệu trên cổng.</summary>
    public enum InvoiceSource
    {
        /// <summary>Hóa đơn điện tử thông thường (api/query)</summary>
        Query,
        /// <summary>Hóa đơn có mã khởi tạo từ máy tính tiền (api/sco-query)</summary>
        Sco,
    }

    /// <summary>Captcha nhận từ cổng.</summary>
    public sealed class CaptchaChallenge
    {
        public CaptchaChallenge(string key, string svg, string guess)
        {
            Key = key; Svg = svg; Guess = guess;
        }

        /// <summary>Gửi lại khi đăng nhập (trường ckey).</summary>
        public string Key { get; }
        /// <summary>Ảnh SVG gốc, để hiển thị cho người dùng nếu đoán sai.</summary>
        public string Svg { get; }
        /// <summary>Kết quả đọc tự động, có thể rỗng.</summary>
        public string Guess { get; }
    }

    /// <summary>Token đăng nhập (JWT) và thời điểm hết hạn.</summary>
    public sealed class HddtToken
    {
        public HddtToken(string value, DateTimeOffset obtainedAt)
        {
            Value = value;
            ObtainedAt = obtainedAt;
            ExpiresAt = ReadJwtExpiry(value);
        }

        public string Value { get; }
        public DateTimeOffset ObtainedAt { get; }
        /// <summary>Đọc từ trường exp của JWT; null nếu không đọc được.</summary>
        public DateTimeOffset? ExpiresAt { get; }

        /// <summary>Còn hạn ít nhất <paramref name="margin"/> nữa không (mặc định 5 phút).</summary>
        public bool IsValid(TimeSpan? margin = null)
        {
            if (ExpiresAt == null) return true; // không biết hạn: cứ dùng, lỗi 401 sẽ báo
            return DateTimeOffset.UtcNow + (margin ?? TimeSpan.FromMinutes(5)) < ExpiresAt.Value;
        }

        private static DateTimeOffset? ReadJwtExpiry(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length < 2) return null;
                var b64 = parts[1].Replace('-', '+').Replace('_', '/');
                b64 = b64.PadRight(b64.Length + (4 - b64.Length % 4) % 4, '=');
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("exp", out var exp) && exp.TryGetInt64(out var sec))
                    return DateTimeOffset.FromUnixTimeSeconds(sec);
            }
            catch { /* không phải JWT chuẩn */ }
            return null;
        }
    }

    /// <summary>
    /// Bộ 4 khóa định danh một hóa đơn trên cổng (nbmst, khmshdon, khhdon, shdon) + nguồn.
    /// </summary>
    public sealed class InvoiceKey : IEquatable<InvoiceKey>
    {
        public InvoiceKey(string nbmst, string khmshdon, string khhdon, string shdon, InvoiceSource source)
        {
            Nbmst = nbmst; Khmshdon = khmshdon; Khhdon = khhdon; Shdon = shdon; Source = source;
        }

        /// <summary>MST người bán (luôn là chuỗi, giữ số 0 đầu).</summary>
        public string Nbmst { get; }
        /// <summary>Ký hiệu mẫu số (1 = GTGT, 2 = bán hàng...).</summary>
        public string Khmshdon { get; }
        /// <summary>Ký hiệu hóa đơn, ví dụ K26TKA.</summary>
        public string Khhdon { get; }
        /// <summary>Số hóa đơn.</summary>
        public string Shdon { get; }
        public InvoiceSource Source { get; }

        /// <summary>Tên file an toàn: {nbmst}_{khmshdon}_{khhdon}_{shdon}</summary>
        public string FileStem => Safe(Nbmst) + "_" + Safe(Khmshdon) + "_" + Safe(Khhdon) + "_" + Safe(Shdon);

        internal string QueryString =>
            "nbmst=" + Uri.EscapeDataString(Nbmst) +
            "&khhdon=" + Uri.EscapeDataString(Khhdon) +
            "&shdon=" + Uri.EscapeDataString(Shdon) +
            "&khmshdon=" + Uri.EscapeDataString(Khmshdon);

        private static string Safe(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var c in s) sb.Append(char.IsLetterOrDigit(c) || c == '-' ? c : '-');
            return sb.ToString();
        }

        public bool Equals(InvoiceKey? o) => o != null && Nbmst == o.Nbmst && Khmshdon == o.Khmshdon && Khhdon == o.Khhdon && Shdon == o.Shdon;
        public override bool Equals(object? obj) => Equals(obj as InvoiceKey);
        public override int GetHashCode() => (Nbmst + "|" + Khmshdon + "|" + Khhdon + "|" + Shdon).GetHashCode();
        public override string ToString() => FileStem + " (" + Source + ")";
    }

    /// <summary>Một trang kết quả danh sách hóa đơn.</summary>
    public sealed class InvoiceListPage
    {
        public InvoiceListPage(string rawJson, IReadOnlyList<InvoiceListItem> items, string? nextState)
        {
            RawJson = rawJson; Items = items; NextState = nextState;
        }

        /// <summary>Nguyên văn phản hồi của cổng.</summary>
        public string RawJson { get; }
        public IReadOnlyList<InvoiceListItem> Items { get; }
        /// <summary>Giá trị state để lấy trang tiếp; null là hết trang.</summary>
        public string? NextState { get; }
    }

    /// <summary>Khóa + vài trường cơ bản của một dòng trong danh sách.</summary>
    public sealed class InvoiceListItem
    {
        public InvoiceListItem(InvoiceKey key, string? tdlap)
        {
            Key = key; Tdlap = tdlap;
        }

        public InvoiceKey Key { get; }
        /// <summary>Ngày lập (chuỗi ISO gốc của cổng).</summary>
        public string? Tdlap { get; }
    }

    /// <summary>Lỗi khi gọi cổng.</summary>
    public class HddtException : Exception
    {
        public HddtException(string message, int? statusCode = null, string? responseBody = null, Exception? inner = null)
            : base(message, inner)
        {
            StatusCode = statusCode; ResponseBody = responseBody;
        }

        public int? StatusCode { get; }
        public string? ResponseBody { get; }
    }

    /// <summary>Đăng nhập thất bại (sai MST/mật khẩu/captcha).</summary>
    public sealed class HddtLoginException : HddtException
    {
        public HddtLoginException(string message, bool isCaptchaError, int? statusCode = null, string? body = null)
            : base(message, statusCode, body)
        {
            IsCaptchaError = isCaptchaError;
        }

        /// <summary>True nếu lỗi do captcha: lấy captcha mới rồi thử lại.</summary>
        public bool IsCaptchaError { get; }
    }
}
