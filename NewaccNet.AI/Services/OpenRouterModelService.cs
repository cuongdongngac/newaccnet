using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Configuration;
using NewaccNet.AI.Contracts;

namespace NewaccNet.AI.Services
{
    public static class OpenRouterModelService
    {
        private static List<OpenRouterModelDto>? _cachedModels;
        private static DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
        private static readonly SemaphoreSlim _lock = new(1, 1);

        /// <summary>
        /// Tải danh sách model từ OpenRouter Gateway (hoặc dùng Cache 1 giờ).
        /// Có fallback danh sách model thông dụng nếu mất kết nối internet.
        /// </summary>
        public static async Task<List<OpenRouterModelDto>> FetchModelsAsync(
            string? apiKey = null,
            string? baseUrl = null,
            bool forceRefresh = false,
            CancellationToken cancellationToken = default)
        {
            if (!forceRefresh && _cachedModels != null && (DateTime.Now - _cacheTime) < CacheDuration)
            {
                return _cachedModels;
            }

            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (!forceRefresh && _cachedModels != null && (DateTime.Now - _cacheTime) < CacheDuration)
                {
                    return _cachedModels;
                }

                string endpoint = string.IsNullOrWhiteSpace(baseUrl) ? OpenRouterOptions.DefaultBaseUrl : baseUrl;
                if (!endpoint.EndsWith("/")) endpoint += "/";
                endpoint += "models";

                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
                httpClient.DefaultRequestHeaders.Add("User-Agent", "NewaccNet-Accounting-WPF");
                httpClient.DefaultRequestHeaders.Add("X-Title", "NewaccNet Accounting");
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                }

                var response = await httpClient.GetAsync(endpoint, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

                    var list = new List<OpenRouterModelDto>();
                    if (doc.RootElement.TryGetProperty("data", out var dataElem) && dataElem.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in dataElem.EnumerateArray())
                        {
                            var dto = ParseModel(item);
                            if (dto != null) list.Add(dto);
                        }
                    }

                    if (list.Count > 0)
                    {
                        _cachedModels = list;
                        _cacheTime = DateTime.Now;
                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[OpenRouterModelService] Lỗi tải models: {ex.Message}");
            }
            finally
            {
                _lock.Release();
            }

            // Fallback danh sách mô hình khuyên dùng nếu không kết nối được mạng
            return GetFallbackModels();
        }

        private static OpenRouterModelDto? ParseModel(JsonElement el)
        {
            if (!el.TryGetProperty("id", out var idProp)) return null;
            string id = idProp.GetString() ?? "";
            if (string.IsNullOrWhiteSpace(id)) return null;

            string name = el.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? id : id;
            string desc = el.TryGetProperty("description", out var descProp) ? descProp.GetString() ?? "" : "";
            int? ctx = el.TryGetProperty("context_length", out var ctxProp) && ctxProp.TryGetInt32(out int cVal) ? cVal : null;
            long? created = el.TryGetProperty("created", out var crProp) && crProp.TryGetInt64(out long crVal) ? crVal : null;

            decimal? priceIn = null;
            decimal? priceOut = null;
            if (el.TryGetProperty("pricing", out var pricingProp) && pricingProp.ValueKind == JsonValueKind.Object)
            {
                if (pricingProp.TryGetProperty("prompt", out var promptProp))
                    priceIn = ParsePricePerMillion(promptProp.GetString());
                if (pricingProp.TryGetProperty("completion", out var compProp))
                    priceOut = ParsePricePerMillion(compProp.GetString());
            }

            bool supportsVision = false;
            if (el.TryGetProperty("architecture", out var archProp) && archProp.ValueKind == JsonValueKind.Object)
            {
                if (archProp.TryGetProperty("input_modalities", out var modsProp) && modsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var m in modsProp.EnumerateArray())
                    {
                        if (string.Equals(m.GetString(), "image", StringComparison.OrdinalIgnoreCase))
                        {
                            supportsVision = true;
                            break;
                        }
                    }
                }
            }

            bool supportsJson = false;
            if (el.TryGetProperty("supported_parameters", out var paramsProp) && paramsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in paramsProp.EnumerateArray())
                {
                    string pStr = p.GetString() ?? "";
                    if (pStr.Equals("response_format", StringComparison.OrdinalIgnoreCase) ||
                        pStr.Equals("structured_outputs", StringComparison.OrdinalIgnoreCase) ||
                        pStr.Equals("tools", StringComparison.OrdinalIgnoreCase))
                    {
                        supportsJson = true;
                        break;
                    }
                }
            }

            string provider = id.Contains('/') ? id.Split('/')[0] : "";
            bool isFree = id.EndsWith(":free", StringComparison.OrdinalIgnoreCase) || 
                          ((priceIn == null || priceIn == 0) && (priceOut == null || priceOut == 0));

            return new OpenRouterModelDto
            {
                Id = id,
                Name = name,
                Provider = provider,
                ContextLength = ctx,
                PriceIn = priceIn,
                PriceOut = priceOut,
                IsFree = isFree,
                SupportsVision = supportsVision,
                SupportsJson = supportsJson,
                Description = desc,
                Created = created
            };
        }

        private static decimal? ParsePricePerMillion(string? strVal)
        {
            if (string.IsNullOrWhiteSpace(strVal)) return null;
            if (decimal.TryParse(strVal, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal val))
            {
                if (val < 0) return null;
                return Math.Round(val * 1_000_000m, 4);
            }
            return null;
        }

        /// <summary>
        /// Danh sách dự phòng mô hình AI thông dụng và tối ưu nhất cho Kế toán & OCR.
        /// </summary>
        public static List<OpenRouterModelDto> GetFallbackModels()
        {
            return new List<OpenRouterModelDto>
            {
                new OpenRouterModelDto
                {
                    Id = "google/gemini-2.5-flash",
                    Name = "Google: Gemini 2.5 Flash",
                    Provider = "google",
                    ContextLength = 1048576,
                    PriceIn = 0.075m,
                    PriceOut = 0.30m,
                    IsFree = false,
                    SupportsVision = true,
                    SupportsJson = true,
                    Description = "Mô hình siêu nhanh, chi phí cực rẻ, ngữ cảnh 1 triệu token. Tối ưu xuất sắc cho nhập liệu kế toán và OCR hóa đơn."
                },
                new OpenRouterModelDto
                {
                    Id = "google/gemini-2.5-pro",
                    Name = "Google: Gemini 2.5 Pro",
                    Provider = "google",
                    ContextLength = 2097152,
                    PriceIn = 1.25m,
                    PriceOut = 5.00m,
                    IsFree = false,
                    SupportsVision = true,
                    SupportsJson = true,
                    Description = "Mô hình tư duy phân tích đỉnh cao của Google. Thích hợp cho CFO Brain, phân tích báo cáo tài chính phức tạp."
                },
                new OpenRouterModelDto
                {
                    Id = "anthropic/claude-3.5-sonnet",
                    Name = "Anthropic: Claude 3.5 Sonnet",
                    Provider = "anthropic",
                    ContextLength = 200000,
                    PriceIn = 3.00m,
                    PriceOut = 15.00m,
                    IsFree = false,
                    SupportsVision = true,
                    SupportsJson = true,
                    Description = "Chuẩn mực cao nhất về độ chính xác lập trình và suy luận logic. Bóc tách hóa đơn và bảng biểu tài chính hoàn hảo."
                },
                new OpenRouterModelDto
                {
                    Id = "deepseek/deepseek-chat",
                    Name = "DeepSeek: DeepSeek V3",
                    Provider = "deepseek",
                    ContextLength = 64000,
                    PriceIn = 0.14m,
                    PriceOut = 0.28m,
                    IsFree = false,
                    SupportsVision = false,
                    SupportsJson = true,
                    Description = "Mô hình mã nguồn mở thế hệ mới với hiệu năng tương đương GPT-4o nhưng chi phí chỉ bằng 1/10."
                },
                new OpenRouterModelDto
                {
                    Id = "deepseek/deepseek-r1",
                    Name = "DeepSeek: DeepSeek R1 (Suy luận)",
                    Provider = "deepseek",
                    ContextLength = 64000,
                    PriceIn = 0.55m,
                    PriceOut = 2.19m,
                    IsFree = false,
                    SupportsVision = false,
                    SupportsJson = true,
                    Description = "Mô hình suy luận chuyên sâu (Reasoning). Tuyệt vời cho việc phân tích các bài toán thuế, chi phí và dòng tiền."
                },
                new OpenRouterModelDto
                {
                    Id = "openai/gpt-4o-mini",
                    Name = "OpenAI: GPT-4o Mini",
                    Provider = "openai",
                    ContextLength = 128000,
                    PriceIn = 0.15m,
                    PriceOut = 0.60m,
                    IsFree = false,
                    SupportsVision = true,
                    SupportsJson = true,
                    Description = "Mô hình nhỏ gọn, thông minh và giá rẻ của OpenAI. Hỗ trợ đa phương tiện hình ảnh và JSON có cấu trúc."
                },
                new OpenRouterModelDto
                {
                    Id = "openai/gpt-4o",
                    Name = "OpenAI: GPT-4o",
                    Provider = "openai",
                    ContextLength = 128000,
                    PriceIn = 2.50m,
                    PriceOut = 10.00m,
                    IsFree = false,
                    SupportsVision = true,
                    SupportsJson = true,
                    Description = "Mô hình hàng đầu toàn diện của OpenAI cho cả văn bản, hình ảnh, tài liệu và phân tích logic."
                },
                new OpenRouterModelDto
                {
                    Id = "google/gemini-2.0-flash-exp:free",
                    Name = "Google: Gemini 2.0 Flash (Free)",
                    Provider = "google",
                    ContextLength = 1048576,
                    PriceIn = 0m,
                    PriceOut = 0m,
                    IsFree = true,
                    SupportsVision = true,
                    SupportsJson = true,
                    Description = "Bản dùng thử MIỄN PHÍ 100% từ Google qua OpenRouter. Thích hợp cho trải nghiệm ban đầu không tốn phí."
                }
            };
        }
    }
}
