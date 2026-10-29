using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Configuration;
using NewaccNet.AI.Gateway;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public class AiMappingSuggestion
    {
        public string InvoiceItemName { get; set; } = string.Empty;
        public string? MatchedInternalCode { get; set; }
        public string? MatchedInternalName { get; set; }
        public int Confidence { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Dịch vụ gợi ý ghép nối danh mục thông minh sử dụng AI (Tier 3: AI Semantic Mapping).
    /// </summary>
    public static class AiItemMappingService
    {
        public static async Task<List<AiMappingSuggestion>> MapBatchAsync(
            List<string> unmatchedInvoiceItems,
            List<(string Code, string Name)> existingDbItems,
            CancellationToken cancellationToken = default)
        {
            var results = new List<AiMappingSuggestion>();
            if (unmatchedInvoiceItems == null || unmatchedInvoiceItems.Count == 0 || existingDbItems == null || existingDbItems.Count == 0)
            {
                return results;
            }

            var config = ConfigManager.Current;
            var apiKey = config.OpenRouterApiKey?.Trim();
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Chưa cấu hình OpenRouter API Key. Vui lòng vào Cài đặt AI để thiết lập khóa API.");
            }

            var options = new OpenRouterOptions
            {
                ApiKey = apiKey,
                BaseUrl = string.IsNullOrWhiteSpace(config.OpenRouterBaseUrl) ? "https://openrouter.ai/api/v1" : config.OpenRouterBaseUrl,
                Model = string.IsNullOrWhiteSpace(config.OpenRouterChatModel) ? "google/gemini-2.5-flash" : config.OpenRouterChatModel
            };

            using var gateway = new OpenRouterGateway(options);

            // Giới hạn danh mục DB đưa vào ngữ cảnh (Top 150 mặt hàng hoặc rút gọn)
            var dbContext = string.Join("\n", existingDbItems.Take(200).Select(x => $"- [{x.Code}] {x.Name}"));
            var invoiceContext = string.Join("\n", unmatchedInvoiceItems.Take(50).Select((name, idx) => $"{idx + 1}. {name}"));

            var systemPrompt = @"Bạn là trợ lý AI chuyên gia kế toán kho và vật tư hàng hóa.
Nhiệm vụ của bạn là phân tích danh sách các mặt hàng ghi trên hóa đơn thuế (thường viết tắt, khác quy cách, thêm bớt từ như bao, thùng, kích cỡ) và đối chiếu với danh mục vật tư hiện có trong CSDL nội bộ.

YÊU CẦU:
1. Nhận diện các mặt hàng cùng bản chất (Ví dụ: 'Sắt phi 10 Hòa Phát' tương đương 'Thép cuộn D10 HP', 'XM Hoàng Thạch PCB40' tương đương 'Xi măng Hoàng Thạch').
2. Nếu tìm thấy mặt hàng nội bộ tương đương, gán 'MatchedInternalCode' bằng mã nội bộ đó và chấm điểm 'Confidence' (từ 50 đến 99) kèm lý do ngắn gọn.
3. Nếu thực sự là mặt hàng mới hoàn toàn chưa có trong CSDL, đặt 'MatchedInternalCode' là null và 'Confidence' là 0.
4. Trả về đúng định dạng JSON Array chứa các object:
[
  {
    ""InvoiceItemName"": ""Tên mặt hàng trên hóa đơn"",
    ""MatchedInternalCode"": ""Mã nội bộ gợi ý (hoặc null)"",
    ""Confidence"": 90,
    ""Reason"": ""Giải thích ngắn gọn lý do ghép nối""
  }
]";

            var userPrompt = $"[DANH MỤC VẬT TƯ HIỆN CÓ TRONG CSDL NỘI BỘ]:\n{dbContext}\n\n[DANH SÁCH MẶT HÀNG TRÊN HÓA ĐƠN THUẾ CẦN GHÉP NỐI]:\n{invoiceContext}";

            try
            {
                var response = await gateway.SendChatRequestAsync(systemPrompt, userPrompt, cancellationToken: cancellationToken);
                var content = response.Content?.Trim() ?? "";

                // Bóc tách JSON từ markdown block ```json ... ```
                if (content.StartsWith("```"))
                {
                    var lines = content.Split('\n').ToList();
                    if (lines.Count > 2)
                    {
                        lines.RemoveAt(0);
                        if (lines[^1].Trim().StartsWith("```")) lines.RemoveAt(lines.Count - 1);
                        content = string.Join("\n", lines).Trim();
                    }
                }

                var parsed = JsonSerializer.Deserialize<List<AiMappingSuggestion>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed != null)
                {
                    var dbDict = existingDbItems.ToDictionary(x => x.Code, x => x.Name, StringComparer.OrdinalIgnoreCase);
                    foreach (var item in parsed)
                    {
                        if (!string.IsNullOrEmpty(item.MatchedInternalCode) && dbDict.TryGetValue(item.MatchedInternalCode, out var internalName))
                        {
                            item.MatchedInternalName = internalName;
                        }
                        results.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi AI Mapping: {ex.Message}");
                throw;
            }

            return results;
        }
    }
}
