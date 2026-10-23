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

namespace Hddt
{
    /// <summary>Tuỳ chọn cho <see cref="HddtClient"/>.</summary>
    public sealed class HddtClientOptions
    {
        public string BaseUrl { get; set; } = "https://hoadondientu.gdt.gov.vn/api";
        /// <summary>Khoảng nghỉ tối thiểu giữa hai request (cổng chặn 429 nếu gọi dồn dập).</summary>
        public TimeSpan MinRequestInterval { get; set; } = TimeSpan.FromMilliseconds(700);
        /// <summary>Số lần thử lại khi gặp 429 / 5xx / lỗi mạng.</summary>
        public int MaxRetries { get; set; } = 3;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
        /// <summary>Số hóa đơn mỗi trang (bản VBA dùng 50).</summary>
        public int PageSize { get; set; } = 50;
        public string UserAgent { get; set; } =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36";
    }

    /// <summary>
    /// Gọi API của cổng hoadondientu.gdt.gov.vn: captcha, đăng nhập, danh sách, chi tiết, XML.
    /// Một instance dùng cho một tài khoản (MST). Thread-safe ở mức gọi tuần tự có điều tiết.
    /// </summary>
    public sealed class HddtClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly bool _ownsHttp;
        private readonly HddtClientOptions _opt;
        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
        private DateTime _lastRequestUtc = DateTime.MinValue;

        public HddtClient(HddtClientOptions? options = null, HttpMessageHandler? handler = null)
        {
            _opt = options ?? new HddtClientOptions();
            handler ??= new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            _http = new HttpClient(handler, disposeHandler: true) { Timeout = _opt.Timeout };
            _ownsHttp = true;
        }

        /// <summary>Token hiện dùng. Có thể gán token đã lưu từ lần trước để khỏi đăng nhập lại.</summary>
        public HddtToken? Token { get; set; }

        // ------------------------------------------------------------------ Đăng nhập

        /// <summary>Lấy captcha mới (GET /captcha) và đoán sẵn nội dung.</summary>
        public async Task<CaptchaChallenge> GetCaptchaAsync(CancellationToken ct = default)
        {
            var body = await SendStringAsync(HttpMethod.Get, _opt.BaseUrl + "/captcha", auth: false, ct).ConfigureAwait(false);
            using var doc = JsonDocument.Parse(body);
            var key = GetString(doc.RootElement, "key") ?? throw new HddtException("Phản hồi captcha không có 'key'.", null, body);
            var svg = GetString(doc.RootElement, "content") ?? string.Empty;
            return new CaptchaChallenge(key, svg, SvgCaptchaSolver.Solve(svg));
        }

        /// <summary>Đăng nhập bằng captcha đã có (POST /security-taxpayer/authenticate).</summary>
        public async Task<HddtToken> LoginAsync(string username, string password, string captchaKey, string captchaValue, CancellationToken ct = default)
        {
            var payload = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["username"] = username,
                ["password"] = password,
                ["cvalue"] = captchaValue,
                ["ckey"] = captchaKey,
            });

            await ThrottleAsync(ct).ConfigureAwait(false);
            using var req = NewRequest(HttpMethod.Post, _opt.BaseUrl + "/security-taxpayer/authenticate", auth: false);
            req.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            try { res = await _http.SendAsync(req, ct).ConfigureAwait(false); }
            catch (Exception ex) when (!(ex is OperationCanceledException) || !ct.IsCancellationRequested)
            {
                throw new HddtException("Lỗi mạng khi đăng nhập: " + ex.Message, null, null, ex);
            }

            using (res)
            {
                var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                string? message = null, token = null;
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    message = GetString(doc.RootElement, "message");
                    token = GetString(doc.RootElement, "token");
                }
                catch (JsonException) { /* body không phải JSON */ }

                if (res.IsSuccessStatusCode && !string.IsNullOrEmpty(token))
                {
                    Token = new HddtToken(token!, DateTimeOffset.UtcNow);
                    return Token;
                }

                var isCaptcha = (message ?? body).IndexOf("captcha", StringComparison.OrdinalIgnoreCase) >= 0;
                throw new HddtLoginException(
                    message ?? ("Đăng nhập không thành công (HTTP " + (int)res.StatusCode + ")."),
                    isCaptcha, (int)res.StatusCode, body);
            }
        }

        /// <summary>
        /// Đăng nhập với captcha đọc tự động; nếu sai captcha thì lấy captcha mới và thử lại.
        /// Nếu tự đọc không được, dùng <paramref name="askUser"/> (nhận SVG, trả chuỗi người dùng nhập).
        /// </summary>
        public async Task<HddtToken> LoginAutoAsync(string username, string password,
            Func<CaptchaChallenge, Task<string>>? askUser = null, int maxAttempts = 3, CancellationToken ct = default)
        {
            HddtLoginException? last = null;
            for (var i = 0; i < maxAttempts; i++)
            {
                var cap = await GetCaptchaAsync(ct).ConfigureAwait(false);
                var value = cap.Guess;
                // Hỏi người dùng khi không đọc được, hoặc ở lần thử cuối sau khi đọc tự động đã sai.
                var ask = askUser != null && (string.IsNullOrEmpty(value) || (i > 0 && i == maxAttempts - 1));
                if (ask) value = await askUser!(cap).ConfigureAwait(false);
                if (string.IsNullOrEmpty(value)) { last = new HddtLoginException("Không đọc được captcha.", true); continue; }

                try { return await LoginAsync(username, password, cap.Key, value, ct).ConfigureAwait(false); }
                catch (HddtLoginException ex) when (ex.IsCaptchaError) { last = ex; }
            }
            throw last ?? new HddtLoginException("Đăng nhập không thành công.", false);
        }

        // ------------------------------------------------------------------ Dữ liệu hóa đơn

        /// <summary>
        /// Lấy một trang danh sách hóa đơn trong khoảng ngày lập [from, to].
        /// Khoảng ngày nên nằm trong một tháng (xem <see cref="DateRanges.SplitByMonth"/>).
        /// </summary>
        /// <param name="direction">Mua vào hay bán ra.</param>
        /// <param name="source">query (thường) hay sco (máy tính tiền).</param>
        /// <param name="from">Từ ngày lập.</param>
        /// <param name="to">Đến ngày lập.</param>
        /// <param name="tthai">Lọc trạng thái hóa đơn (tthai), null = tất cả.</param>
        /// <param name="ttxly">Lọc kết quả kiểm tra (ttxly), null = tất cả.</param>
        /// <param name="state">Giá trị NextState của trang trước; null cho trang đầu.</param>
        /// <param name="ct">Huỷ.</param>
        public async Task<InvoiceListPage> GetInvoicePageAsync(InvoiceDirection direction, InvoiceSource source,
            DateTime from, DateTime to, string? tthai = null, string? ttxly = null, string? state = null, CancellationToken ct = default)
        {
            var search = "tdlap=ge=" + from.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "T00:00:00"
                       + ";tdlap=le=" + to.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + "T23:59:59";
            if (!string.IsNullOrEmpty(tthai)) search += ";tthai==" + tthai;
            if (!string.IsNullOrEmpty(ttxly)) search += ";ttxly==" + ttxly;

            var url = _opt.BaseUrl + "/" + SourcePath(source) + "/invoices/" + (direction == InvoiceDirection.Purchase ? "purchase" : "sold")
                    + "?sort=tdlap:desc&size=" + _opt.PageSize.ToString(CultureInfo.InvariantCulture)
                    + (state != null ? "&state=" + Uri.EscapeDataString(state) : "")
                    + "&search=" + search; // giữ nguyên như bản VBA: cổng đọc ; = / : trực tiếp

            var body = await SendStringAsync(HttpMethod.Get, url, auth: true, ct).ConfigureAwait(false);

            var items = new List<InvoiceListItem>();
            string? next = null;
            using (var doc = JsonDocument.Parse(body))
            {
                var root = doc.RootElement;
                if (!root.TryGetProperty("datas", out var datas) || datas.ValueKind != JsonValueKind.Array)
                    throw new HddtException("Phản hồi danh sách không có 'datas': " + (GetString(root, "message") ?? ""), null, body);

                foreach (var it in datas.EnumerateArray())
                {
                    var key = new InvoiceKey(
                        GetString(it, "nbmst") ?? "", GetString(it, "khmshdon") ?? "",
                        GetString(it, "khhdon") ?? "", GetString(it, "shdon") ?? "", source);
                    items.Add(new InvoiceListItem(key, GetString(it, "tdlap")));
                }

                var st = GetString(root, "state");
                if (!string.IsNullOrEmpty(st) && items.Count > 0) next = st;
            }
            return new InvoiceListPage(body, items, next);
        }

        /// <summary>Chi tiết một hóa đơn (JSON gốc của cổng, gồm danh sách hàng hóa hdhhdvu).</summary>
        public Task<string> GetDetailJsonAsync(InvoiceKey key, CancellationToken ct = default) =>
            SendStringAsync(HttpMethod.Get, _opt.BaseUrl + "/" + SourcePath(key.Source) + "/invoices/detail?" + key.QueryString, auth: true, ct);

        /// <summary>
        /// Tải file zip chứa XML gốc. Trả về null nếu cổng báo không có hồ sơ gốc (HTTP 500, như bản VBA).
        /// </summary>
        public async Task<byte[]?> ExportXmlZipAsync(InvoiceKey key, CancellationToken ct = default)
        {
            var url = _opt.BaseUrl + "/" + SourcePath(key.Source) + "/invoices/export-xml?" + key.QueryString;
            var (status, bytes) = await SendBytesAsync(url, ct).ConfigureAwait(false);
            return status == 500 ? null : bytes;
        }

        // ------------------------------------------------------------------ HTTP

        private static string SourcePath(InvoiceSource s) => s == InvoiceSource.Sco ? "sco-query" : "query";

        private HttpRequestMessage NewRequest(HttpMethod method, string url, bool auth)
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.TryAddWithoutValidation("User-Agent", _opt.UserAgent);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            if (auth)
            {
                if (Token == null) throw new HddtException("Chưa đăng nhập (Token = null).");
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.Value);
            }
            return req;
        }

        private async Task<string> SendStringAsync(HttpMethod method, string url, bool auth, CancellationToken ct)
        {
            using var res = await SendWithRetryAsync(() => NewRequest(method, url, auth), final500: false, ct).ConfigureAwait(false);

            var body = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!res.IsSuccessStatusCode)
                throw new HddtException(Describe((int)res.StatusCode) + " " + url, (int)res.StatusCode, body);
            return body;
        }

        private async Task<(int status, byte[]? bytes)> SendBytesAsync(string url, CancellationToken ct)
        {
            using var res = await SendWithRetryAsync(() => NewRequest(HttpMethod.Get, url, auth: true), final500: true, ct).ConfigureAwait(false);
            var status = (int)res.StatusCode;
            if (status == 500) return (500, null);
            if (!res.IsSuccessStatusCode)
                throw new HddtException(Describe(status) + " " + url, status, await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            return (status, await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false));
        }

        private async Task<HttpResponseMessage> SendWithRetryAsync(Func<HttpRequestMessage> makeRequest, bool final500, CancellationToken ct)
        {
            for (var attempt = 0; ; attempt++)
            {
                await ThrottleAsync(ct).ConfigureAwait(false);
                HttpResponseMessage? res = null;
                Exception? netError = null;
                using (var req = makeRequest())
                {
                    try { res = await _http.SendAsync(req, ct).ConfigureAwait(false); }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
                    catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) { netError = ex; }
                }

                var status = res != null ? (int)res.StatusCode : 0;
                var retryable = res == null || status == 429 || status == 502 || status == 503 || status == 504 || (status == 500 && !final500);

                if (!retryable) return res!;
                if (attempt >= _opt.MaxRetries)
                {
                    if (res != null) return res;
                    throw new HddtException("Lỗi kết nối tới cổng: " + netError!.Message, null, null, netError);
                }

                var wait = status == 429
                    ? (res!.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(5 * (attempt + 1)))
                    : TimeSpan.FromSeconds(2 * (attempt + 1));
                res?.Dispose();
                await Task.Delay(wait, ct).ConfigureAwait(false);
            }
        }

        private async Task ThrottleAsync(CancellationToken ct)
        {
            await _gate.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                var wait = _lastRequestUtc + _opt.MinRequestInterval - DateTime.UtcNow;
                if (wait > TimeSpan.Zero) await Task.Delay(wait, ct).ConfigureAwait(false);
                _lastRequestUtc = DateTime.UtcNow;
            }
            finally { _gate.Release(); }
        }

        private static string Describe(int status) => status switch
        {
            401 => "HTTP 401 - Token hết hạn hoặc không hợp lệ, cần đăng nhập lại.",
            403 => "HTTP 403 - Không có quyền truy cập.",
            429 => "HTTP 429 - Gửi quá nhiều yêu cầu tới máy chủ.",
            500 => "HTTP 500 - Lỗi máy chủ cổng hóa đơn.",
            _ => "HTTP " + status + ".",
        };

        /// <summary>Đọc thuộc tính dạng chuỗi; số cũng trả về chuỗi (giữ nguyên MST, số hóa đơn).</summary>
        internal static string? GetString(JsonElement e, string name)
        {
            if (e.ValueKind != JsonValueKind.Object || !e.TryGetProperty(name, out var v)) return null;
            return v.ValueKind switch
            {
                JsonValueKind.String => v.GetString(),
                JsonValueKind.Number => v.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => null,
            };
        }

        public void Dispose()
        {
            if (_ownsHttp) _http.Dispose();
            _gate.Dispose();
        }
    }
}
