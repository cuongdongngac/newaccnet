using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.TaxInvoice
{
    /// <summary>
    /// Giao diện giải mã Captcha (hỗ trợ mô hình toán học cục bộ hoặc AI Vision Fallback).
    /// </summary>
    public interface ICaptchaSolver
    {
        /// <summary>
        /// Giải mã nội dung Captcha (SVG hoặc Base64 Image) thành chuỗi ký tự.
        /// Trả về null nếu không giải được để hệ thống chuyển tiếp giải pháp khác.
        /// </summary>
        Task<string?> SolveAsync(CaptchaChallenge challenge, CancellationToken cancellationToken = default);
    }
}
