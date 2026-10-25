using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.AI.Configuration;

namespace NewaccNet.AI.Gateway
{
    public class OpenRouterResponse
    {
        public string Content { get; set; } = string.Empty;
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens => PromptTokens + CompletionTokens;
        public string ModelUsed { get; set; } = string.Empty;
    }

    public class OpenRouterGateway : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;
        private readonly OpenRouterOptions _options;

        public OpenRouterGateway(OpenRouterOptions options, HttpClient? httpClient = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _ownsHttpClient = httpClient == null;
            _httpClient = httpClient ?? new HttpClient();

            if (_httpClient.BaseAddress == null)
            {
                var baseUri = string.IsNullOrWhiteSpace(_options.BaseUrl) ? OpenRouterOptions.DefaultBaseUrl : _options.BaseUrl;
                if (!baseUri.EndsWith("/")) baseUri += "/";
                _httpClient.BaseAddress = new Uri(baseUri);
            }
        }

        public void Dispose()
        {
            if (_ownsHttpClient)
            {
                _httpClient.Dispose();
            }
        }

        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey)) return false;

            try
            {
                var response = await SendChatRequestAsync(
                    systemPrompt: "You are a test ping helper.",
                    userPrompt: "Ping. Reply with PONG.",
                    model: _options.Model,
                    temperature: 0.1,
                    cancellationToken: cancellationToken);

                return !string.IsNullOrWhiteSpace(response.Content);
            }
            catch
            {
                return false;
            }
        }

        public async Task<OpenRouterResponse> SendChatAsync(string userPrompt, CancellationToken cancellationToken = default)
        {
            return await SendChatRequestAsync("Bạn là trợ lý AI chuyên nghiệp về kế toán, tài chính và phân tích dữ liệu văn phòng.", userPrompt, cancellationToken: cancellationToken);
        }

        public async Task<OpenRouterResponse> SendChatRequestAsync(
            string systemPrompt,
            string userPrompt,
            string? model = null,
            double temperature = 0.2,
            bool jsonMode = false,
            CancellationToken cancellationToken = default)
        {
            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            };

            return await PostCompletionAsync(messages, model, temperature, jsonMode, cancellationToken).ConfigureAwait(false);
        }

        public async Task<OpenRouterResponse> SendVisionRequestAsync(
            string systemPrompt,
            string userTextPrompt,
            IEnumerable<string> fileDataUris,
            string? model = null,
            double temperature = 0.1,
            bool jsonMode = false,
            CancellationToken cancellationToken = default)
        {
            var contentParts = new List<object>
            {
                new { type = "text", text = userTextPrompt }
            };

            foreach (var uri in fileDataUris)
            {
                if (string.IsNullOrWhiteSpace(uri)) continue;
                contentParts.Add(new
                {
                    type = "image_url",
                    image_url = new { url = uri }
                });
            }

            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = contentParts }
            };

            var targetModel = model ?? _options.VisionModel;
            return await PostCompletionAsync(messages, targetModel, temperature, jsonMode, cancellationToken).ConfigureAwait(false);
        }

        public async Task<T?> SendStructuredRequestAsync<T>(
            string systemPrompt,
            string userPrompt,
            string? model = null,
            double temperature = 0.1,
            CancellationToken cancellationToken = default) where T : class
        {
            var response = await SendChatRequestAsync(systemPrompt, userPrompt, model, temperature, jsonMode: true, cancellationToken).ConfigureAwait(false);
            return DeserializeJson<T>(response.Content);
        }

        public async Task<T?> SendStructuredVisionRequestAsync<T>(
            string systemPrompt,
            string userTextPrompt,
            IEnumerable<string> fileDataUris,
            string? model = null,
            double temperature = 0.1,
            CancellationToken cancellationToken = default) where T : class
        {
            var response = await SendVisionRequestAsync(systemPrompt, userTextPrompt, fileDataUris, model, temperature, jsonMode: true, cancellationToken).ConfigureAwait(false);
            return DeserializeJson<T>(response.Content);
        }

        private async Task<OpenRouterResponse> PostCompletionAsync(
            List<object> messages,
            string? model,
            double temperature,
            bool jsonMode,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("Chưa cấu hình OpenRouter API Key. Vui lòng thiết lập API Key trước khi sử dụng AI.");
            }

            var targetModel = !string.IsNullOrWhiteSpace(model) ? model : _options.Model;

            object requestBody;
            if (jsonMode)
            {
                requestBody = new
                {
                    model = targetModel,
                    temperature = temperature,
                    response_format = new { type = "json_object" },
                    messages = messages
                };
            }
            else
            {
                requestBody = new
                {
                    model = targetModel,
                    temperature = temperature,
                    messages = messages
                };
            }

            var jsonContent = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            if (!string.IsNullOrWhiteSpace(_options.HttpReferer))
            {
                request.Headers.Add("HTTP-Referer", _options.HttpReferer);
            }
            if (!string.IsNullOrWhiteSpace(_options.AppTitle))
            {
                request.Headers.Add("X-Title", _options.AppTitle);
            }

            using var httpResponse = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var responseString = await httpResponse.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"OpenRouter API Lỗi (Mã {httpResponse.StatusCode}):\n{responseString}");
            }

            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;

            string content = string.Empty;
            if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var textElem))
                {
                    content = textElem.GetString() ?? string.Empty;
                }
            }

            int promptTokens = 0;
            int completionTokens = 0;
            if (root.TryGetProperty("usage", out var usage))
            {
                if (usage.TryGetProperty("prompt_tokens", out var pt)) promptTokens = pt.GetInt32();
                if (usage.TryGetProperty("completion_tokens", out var ct)) completionTokens = ct.GetInt32();
            }

            // Strip code fences if returned by model
            content = StripMarkdownFences(content);

            return new OpenRouterResponse
            {
                Content = content,
                PromptTokens = promptTokens,
                CompletionTokens = completionTokens,
                ModelUsed = targetModel
            };
        }

        public static string StripMarkdownFences(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            var trimmed = text.Trim();

            // Match ```json ... ``` or ```markdown ... ```
            var match = Regex.Match(trimmed, @"^```(?:json|markdown|xml|html)?\s*([\s\S]*?)\s*```$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return trimmed;
        }

        private static T? DeserializeJson<T>(string rawJson) where T : class
        {
            if (string.IsNullOrWhiteSpace(rawJson)) return null;
            var clean = StripMarkdownFences(rawJson);

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<T>(clean, options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Deserialize Error: {ex.Message}\nRaw: {clean}");
                return null;
            }
        }
    }
}
