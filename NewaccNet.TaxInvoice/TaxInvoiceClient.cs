using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.TaxInvoice
{
    public sealed class TaxInvoiceClientOptions
    {
        public string BaseUrl { get; set; } = "https://hoadondientu.gdt.gov.vn/api";
        public TimeSpan MinRequestInterval { get; set; } = TimeSpan.FromMilliseconds(700);
        public int MaxRetries { get; set; } = 3;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
        public int PageSize { get; set; } = 50;
        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36";
    }

    public sealed class TaxInvoiceClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;
        private readonly TaxInvoiceClientOptions _options;
        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
        private DateTime _lastRequestUtc = DateTime.MinValue;

        public TaxInvoiceClient(TaxInvoiceClientOptions? options = null, HttpMessageHandler? handler = null)
        {
            _options = options ?? new TaxInvoiceClientOptions();
            handler ??= new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _httpClient = new HttpClient(handler, disposeHandler: true) { Timeout = _options.Timeout };
            _ownsHttpClient = true;
        }

        public TaxInvoiceToken? Token { get; set; }

        public async Task<CaptchaChallenge> GetCaptchaAsync(CancellationToken cancellationToken = default)
        {
            var responseBody = await SendStringAsync(HttpMethod.Get, _options.BaseUrl + "/captcha", requiresAuth: false, cancellationToken).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(responseBody);
            var key = GetString(doc.RootElement, "key") ?? throw new TaxInvoiceException("Captcha response is missing the 'key' field.", null, responseBody);
            var svgContent = GetString(doc.RootElement, "content") ?? string.Empty;
            return new CaptchaChallenge(key, svgContent, SvgCaptchaSolver.Solve(svgContent));
        }

        public async Task<TaxInvoiceToken> LoginAsync(string username, string password, string captchaKey, string captchaValue, CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["cvalue"] = captchaValue,
                ["ckey"] = captchaKey,
            });

            await ThrottleAsync(cancellationToken).ConfigureAwait(false);
            using var request = CreateRequest(HttpMethod.Post, _options.BaseUrl + "/security-taxpayer/authenticate", requiresAuth: false);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException) || !cancellationToken.IsCancellationRequested)
            {
                throw new TaxInvoiceException("Network error during login: " + ex.Message, null, null, ex);
            }

            using (response)
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                string? message = null;
                string? token = null;

                try
                {
                    using var doc = JsonDocument.Parse(responseBody);
                    message = GetString(doc.RootElement, "message");
                    token = GetString(doc.RootElement, "token");
                }
                catch (JsonException) { }

                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(token))
                {
                    Token = new TaxInvoiceToken(token!, DateTimeOffset.UtcNow);
                    return Token;
                }

                var isCaptchaError = (message ?? responseBody).IndexOf("captcha", StringComparison.OrdinalIgnoreCase) >= 0;
                throw new TaxInvoiceLoginException(
                    message ?? ("Login failed (HTTP " + (int)response.StatusCode + ")."),
                    isCaptchaError, (int)response.StatusCode, responseBody);
            }
        }

        public async Task<TaxInvoiceToken> LoginAutoAsync(string username, string password,
            Func<CaptchaChallenge, Task<string>>? promptUserFunc = null, int maxAttempts = 3, CancellationToken cancellationToken = default)
        {
            TaxInvoiceLoginException? lastException = null;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                var challenge = await GetCaptchaAsync(cancellationToken).ConfigureAwait(false);
                var captchaValue = challenge.RecognizedValue;

                var shouldPrompt = promptUserFunc != null && (string.IsNullOrEmpty(captchaValue) || (attempt > 0 && attempt == maxAttempts - 1));
                if (shouldPrompt)
                {
                    captchaValue = await promptUserFunc!(challenge).ConfigureAwait(false);
                }

                if (string.IsNullOrEmpty(captchaValue))
                {
                    lastException = new TaxInvoiceLoginException("Unable to read captcha image.", true);
                    continue;
                }

                try
                {
                    return await LoginAsync(username, password, challenge.Key, captchaValue, cancellationToken).ConfigureAwait(false);
                }
                catch (TaxInvoiceLoginException ex) when (ex.IsCaptchaError)
                {
                    lastException = ex;
                }
            }

            throw lastException ?? new TaxInvoiceLoginException("Login failed after maximum attempts.", false);
        }

        public async Task<InvoiceListPage> GetInvoicePageAsync(
            InvoiceDirection direction,
            InvoiceSource source,
            DateTime fromDate,
            DateTime toDate,
            string? statusFilter = null,
            string? processStatusFilter = null,
            string? nextStateToken = null,
            CancellationToken cancellationToken = default)
        {
            var searchCriteria = "tdlap=ge=" + fromDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "T00:00:00"
                               + ";tdlap=le=" + toDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "T23:59:59";

            if (!string.IsNullOrEmpty(statusFilter)) searchCriteria += ";tthai==" + statusFilter;
            if (!string.IsNullOrEmpty(processStatusFilter)) searchCriteria += ";ttxly==" + processStatusFilter;

            var directionSegment = direction == InvoiceDirection.Purchase ? "purchase" : "sold";
            var url = _options.BaseUrl + "/" + GetSourceSegment(source) + "/invoices/" + directionSegment
                    + "?sort=tdlap:desc&size=" + _options.PageSize.ToString(CultureInfo.InvariantCulture)
                    + (nextStateToken != null ? "&state=" + Uri.EscapeDataString(nextStateToken) : "")
                    + "&search=" + searchCriteria;

            var responseBody = await SendStringAsync(HttpMethod.Get, url, requiresAuth: true, cancellationToken).ConfigureAwait(false);

            var items = new List<InvoiceListItem>();
            string? nextState = null;

            using (var doc = JsonDocument.Parse(responseBody))
            {
                var root = doc.RootElement;
                if (!root.TryGetProperty("datas", out var datas) || datas.ValueKind != JsonValueKind.Array)
                {
                    throw new TaxInvoiceException("Response is missing 'datas': " + (GetString(root, "message") ?? ""), null, responseBody);
                }

                foreach (var item in datas.EnumerateArray())
                {
                    var key = new InvoiceKey(
                        GetString(item, "nbmst") ?? "",
                        GetString(item, "khmshdon") ?? "",
                        GetString(item, "khhdon") ?? "",
                        GetString(item, "shdon") ?? "",
                        source);

                    items.Add(new InvoiceListItem(key, GetString(item, "tdlap")));
                }

                var stateProp = GetString(root, "state");
                if (!string.IsNullOrEmpty(stateProp) && items.Count > 0)
                {
                    nextState = stateProp;
                }
            }

            return new InvoiceListPage(responseBody, items, nextState);
        }

        public Task<string> GetDetailJsonAsync(InvoiceKey key, CancellationToken cancellationToken = default)
        {
            var url = _options.BaseUrl + "/" + GetSourceSegment(key.Source) + "/invoices/detail?" + key.QueryString;
            return SendStringAsync(HttpMethod.Get, url, requiresAuth: true, cancellationToken);
        }

        public async Task<byte[]?> ExportXmlZipAsync(InvoiceKey key, CancellationToken cancellationToken = default)
        {
            var url = _options.BaseUrl + "/" + GetSourceSegment(key.Source) + "/invoices/export-xml?" + key.QueryString;
            var (statusCode, bytes) = await SendBytesAsync(url, cancellationToken).ConfigureAwait(false);
            return statusCode == 500 ? null : bytes;
        }

        private static string GetSourceSegment(InvoiceSource source) => source == InvoiceSource.Sco ? "sco-query" : "query";

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, bool requiresAuth)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.TryAddWithoutValidation("User-Agent", _options.UserAgent);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            if (requiresAuth)
            {
                if (Token == null) throw new TaxInvoiceException("Authentication required: Token is null.");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.Value);
            }
            return request;
        }

        private async Task<string> SendStringAsync(HttpMethod method, string url, bool requiresAuth, CancellationToken cancellationToken)
        {
            using var response = await SendWithRetryAsync(() => CreateRequest(method, url, requiresAuth), suppress500Error: false, cancellationToken).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new TaxInvoiceException(FormatErrorMessage((int)response.StatusCode) + " " + url, (int)response.StatusCode, responseBody);
            }
            return responseBody;
        }

        private async Task<(int statusCode, byte[]? bytes)> SendBytesAsync(string url, CancellationToken cancellationToken)
        {
            using var response = await SendWithRetryAsync(() => CreateRequest(HttpMethod.Get, url, requiresAuth: true), suppress500Error: true, cancellationToken).ConfigureAwait(false);
            var statusCode = (int)response.StatusCode;
            if (statusCode == 500) return (500, null);
            if (!response.IsSuccessStatusCode)
            {
                throw new TaxInvoiceException(FormatErrorMessage(statusCode) + " " + url, statusCode, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            return (statusCode, await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false));
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(Func<HttpRequestMessage> requestFactory, bool suppress500Error, CancellationToken cancellationToken)
        {
            for (var attempt = 0; ; attempt++)
            {
                await ThrottleAsync(cancellationToken).ConfigureAwait(false);
                HttpResponseMessage? response = null;
                Exception? networkError = null;

                using (var req = requestFactory())
                {
                    try
                    {
                        response = await _httpClient.SendAsync(req, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                    {
                        networkError = ex;
                    }
                }

                var status = response != null ? (int)response.StatusCode : 0;
                var canRetry = response == null || status == 429 || status == 502 || status == 503 || status == 504 || (status == 500 && !suppress500Error);

                if (!canRetry) return response!;
                if (attempt >= _options.MaxRetries)
                {
                    if (response != null) return response;
                    throw new TaxInvoiceException("Connection failure: " + networkError!.Message, null, null, networkError);
                }

                var delay = status == 429
                    ? (response!.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5 * (attempt + 1)))
                    : TimeSpan.FromSeconds(2 * (attempt + 1));

                response?.Dispose();
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task ThrottleAsync(CancellationToken cancellationToken)
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var waitInterval = _lastRequestUtc + _options.MinRequestInterval - DateTime.UtcNow;
                if (waitInterval > TimeSpan.Zero)
                {
                    await Task.Delay(waitInterval, cancellationToken).ConfigureAwait(false);
                }
                _lastRequestUtc = DateTime.UtcNow;
            }
            finally
            {
                _gate.Release();
            }
        }

        private static string FormatErrorMessage(int statusCode) => statusCode switch
        {
            401 => "HTTP 401 - Token expired or invalid. Please log in again.",
            403 => "HTTP 403 - Forbidden access.",
            429 => "HTTP 429 - Too many requests. Rate limit exceeded.",
            500 => "HTTP 500 - Server error from General Department of Taxation.",
            _ => $"HTTP {statusCode} error."
        };

        internal static string? GetString(JsonElement element, string propertyName)
        {
            if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(propertyName, out var prop)) return null;
            return prop.ValueKind switch
            {
                JsonValueKind.String => prop.GetString(),
                JsonValueKind.Number => prop.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null,
            };
        }

        public void Dispose()
        {
            if (_ownsHttpClient) _httpClient.Dispose();
            _gate.Dispose();
        }
    }
}
