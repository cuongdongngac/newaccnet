using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Configuration;
using NewaccNet.AI.Gateway;
using NewaccNet.TaxInvoice;

namespace NewaccNet.AI.Services
{
    /// <summary>
    /// Bộ giải mã Captcha bằng AI Vision (Gemini 2.5 Flash / OpenRouter).
    /// Đóng vai trò là lớp dự phòng (Fallback) khi thuật toán toán học cục bộ thất bại
    /// hoặc khi gặp Captcha dạng ảnh bitmap / font bị biến dạng.
    /// </summary>
    public class AiCaptchaSolver : ICaptchaSolver
    {
        private readonly OpenRouterOptions? _options;
        private readonly OpenRouterGateway? _gateway;

        public AiCaptchaSolver(OpenRouterOptions? options = null)
        {
            _options = options;
        }

        public AiCaptchaSolver(OpenRouterGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<string?> SolveAsync(CaptchaChallenge challenge, CancellationToken cancellationToken = default)
        {
            if (challenge == null || string.IsNullOrWhiteSpace(challenge.SvgContent))
            {
                return null;
            }

            // Lấy options hoặc gateway
            var options = _options;
            if (options == null && _gateway == null)
            {
                var config = NewaccNet.Core.Data.ConfigManager.Current;
                if (!string.IsNullOrWhiteSpace(config?.OpenRouterApiKey))
                {
                    options = OpenRouterOptions.FromAppConfig(config);
                }
            }

            if (_gateway == null && (options == null || !options.IsConfigured))
            {
                // Chưa thiết lập API Key OpenRouter -> bỏ qua an toàn, không ném ngoại lệ
                return null;
            }

            try
            {
                using var gateway = _gateway ?? new OpenRouterGateway(options!);

                // Chuẩn bị Data URI từ SVG hoặc Base64 Image
                string dataUri;
                string content = challenge.SvgContent.Trim();
                if (content.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                {
                    dataUri = content;
                }
                else
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
                    dataUri = $"data:image/svg+xml;base64,{base64}";
                }

                var systemPrompt = "You are a specialized OCR engine for high-accuracy Captcha recognition. " +
                                   "Your task is to extract the 4 to 6 alphanumeric characters shown in the distorted Captcha image/vector. " +
                                   "Return ONLY the plain characters with NO spaces, NO punctuation, NO quotes, and NO commentary.";

                var userPrompt = "Identify and return only the characters shown in this captcha image:";

                var targetModel = !string.IsNullOrWhiteSpace(options?.VisionModel)
                    ? options.VisionModel
                    : OpenRouterOptions.DefaultVisionModel;

                var response = await gateway.SendVisionRequestAsync(
                    systemPrompt: systemPrompt,
                    userTextPrompt: userPrompt,
                    fileDataUris: new[] { dataUri },
                    model: targetModel,
                    temperature: 0.0,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (response != null && !string.IsNullOrWhiteSpace(response.Content))
                {
                    // Lọc chỉ giữ lại chữ cái và chữ số
                    var cleaned = Regex.Replace(response.Content.Trim(), @"[^a-zA-Z0-9]", "");
                    if (cleaned.Length >= 3 && cleaned.Length <= 8)
                    {
                        return cleaned;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AiCaptchaSolver] Error solving captcha: {ex.Message}");
            }

            return null;
        }
    }
}
