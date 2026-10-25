using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Contracts;
using NewaccNet.AI.Gateway;
using NewaccNet.AI.Prompts;

namespace NewaccNet.AI.Services
{
    public class OcrVisionService
    {
        private readonly OpenRouterGateway _gateway;

        public OcrVisionService(OpenRouterGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        /// <summary>
        /// Chuyển đổi tệp ảnh hoặc PDF trên máy thành chuỗi Data URI (Base64) để gửi qua API OpenRouter.
        /// </summary>
        public static string ConvertFileToDataUri(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Không tìm thấy tệp tài liệu: " + filePath);
            }

            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            var mimeType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".pdf" => "application/pdf",
                _ => "image/jpeg"
            };

            var bytes = File.ReadAllBytes(filePath);
            var base64 = Convert.ToBase64String(bytes);
            return $"data:{mimeType};base64,{base64}";
        }

        /// <summary>
        /// Bóc tách Hóa đơn / Chứng từ giấy thành đối tượng OcrInvoiceResultDto có cấu trúc hoàn chỉnh.
        /// </summary>
        public async Task<OcrInvoiceResultDto?> ExtractInvoiceFromImagesAsync(
            IEnumerable<string> fileDataUris,
            string? userNote = null,
            CancellationToken cancellationToken = default)
        {
            var systemPrompt = AiPromptTemplates.InvoiceOcrSystemPrompt;
            var userTextPrompt = "Hãy bóc tách toàn bộ thông tin trên hóa đơn/chứng từ này thành JSON OcrInvoiceResultDto.";
            if (!string.IsNullOrWhiteSpace(userNote))
            {
                userTextPrompt += $"\n[GHI CHÚ THÊM TỪ NGƯỜI DÙNG]:\n{userNote.Trim()}";
            }

            return await _gateway.SendStructuredVisionRequestAsync<OcrInvoiceResultDto>(
                systemPrompt,
                userTextPrompt,
                fileDataUris,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// OCR tài liệu / sách báo cũ / hợp đồng ra văn bản Markdown hoặc HTML sạch.
        /// </summary>
        public async Task<string> RecognizeDocumentTextAsync(
            IEnumerable<string> fileDataUris,
            string outputFormat = "markdown",
            string? additionalPrompt = null,
            CancellationToken cancellationToken = default)
        {
            var systemPrompt = outputFormat.Equals("html", StringComparison.OrdinalIgnoreCase)
                ? "Bạn là hệ thống OCR tái tạo tài liệu dưới dạng mã HTML sạch có căn lề, bảng biểu đẹp mắt. Chỉ trả về mã HTML sạch."
                : AiPromptTemplates.DocumentOcrSystemPrompt;

            if (!string.IsNullOrWhiteSpace(additionalPrompt))
            {
                systemPrompt += $"\n\n[YÊU CẦU ĐẶC BIỆT TỪ NGƯỜI DÙNG]:\n{additionalPrompt.Trim()}";
            }

            var response = await _gateway.SendVisionRequestAsync(
                systemPrompt,
                "Hãy tiến hành OCR tài liệu/ảnh sau đây theo đúng yêu cầu.",
                fileDataUris,
                cancellationToken: cancellationToken);

            return response.Content;
        }

        public async Task<OcrInvoiceResultDto?> ExtractInvoiceFromImageOrPdfAsync(
            string filePath,
            string? userNote = null,
            CancellationToken cancellationToken = default)
        {
            var uri = ConvertFileToDataUri(filePath);
            return await ExtractInvoiceFromImagesAsync(new[] { uri }, userNote, cancellationToken);
        }

        public async Task<string> ExtractDocumentTextAsync(
            string filePath,
            string outputFormat = "markdown",
            string? additionalPrompt = null,
            CancellationToken cancellationToken = default)
        {
            var uri = ConvertFileToDataUri(filePath);
            return await RecognizeDocumentTextAsync(new[] { uri }, outputFormat, additionalPrompt, cancellationToken);
        }
    }
}
