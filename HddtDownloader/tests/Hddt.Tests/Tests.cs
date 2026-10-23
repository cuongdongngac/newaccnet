using System.Net;
using System.Text;
using System.Text.Json;
using Hddt;
using Xunit;

public class CaptchaTests
{
    // Chữ ký thật lấy từ bảng của bản VBA
    const string A = "MQQQQQZMQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQZMQQZ";
    const string K = "MQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQZ";
    const string Seven = "MQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQQQZ";
    const string Nine = "MQQQQQQQQZMQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQQQQQQQQQQZMQQQQQQQQQQQZ";

    static string Path(string sig, double x)
    {
        var sb = new StringBuilder();
        var first = true;
        foreach (var c in sig)
        {
            if (c == 'M') { sb.Append(first ? $"M{x:0.##} 12.5 " : "M3 4 "); first = false; }
            else if (c == 'Q') sb.Append("Q1.2 3.4 5.6 7.8 ");
            else sb.Append('Z');
        }
        return $"<path fill=\"#333\" d=\"{sb}\"/>";
    }

    [Fact]
    public void Solves_in_x_order_and_ignores_noise()
    {
        var svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"150\" height=\"50\">"
                + "<path d=\"M5 30 C 40 10 80 40 140 20\" stroke=\"#777\" fill=\"none\"/>" // đường nhiễu
                + Path(Nine, 98.5) + Path(A, 12) + Path(Seven, 60) + Path(K, 35.25)
                + "</svg>";
        Assert.Equal("AK79", SvgCaptchaSolver.Solve(svg));
    }

    [Fact]
    public void Empty_when_nothing_recognised() => Assert.Equal("", SvgCaptchaSolver.Solve("<svg><path d=\"M1 1Z\"/></svg>"));
}

public class DateRangeTests
{
    [Fact]
    public void Splits_by_calendar_month_with_partial_edges()
    {
        var r = DateRanges.SplitByMonth(new DateTime(2024, 1, 15), new DateTime(2024, 3, 10));
        Assert.Equal(3, r.Count);
        Assert.Equal((new DateTime(2024, 1, 15), new DateTime(2024, 1, 31)), (r[0].From, r[0].To));
        Assert.Equal((new DateTime(2024, 2, 1), new DateTime(2024, 2, 29)), (r[1].From, r[1].To));
        Assert.Equal((new DateTime(2024, 3, 1), new DateTime(2024, 3, 10)), (r[2].From, r[2].To));
    }

    [Fact]
    public void Single_day_and_year_boundary()
    {
        Assert.Single(DateRanges.SplitByMonth(new DateTime(2026, 5, 31), new DateTime(2026, 5, 31)));
        var r = DateRanges.SplitByMonth(new DateTime(2025, 12, 1), new DateTime(2026, 1, 31));
        Assert.Equal(new[] { "2025-12", "2026-01" }, r.Select(x => x.MonthLabel));
    }
}

public class TokenTests
{
    [Fact]
    public void Reads_jwt_expiry()
    {
        var exp = DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds();
        string B64(string s) => Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var jwt = B64("{\"alg\":\"HS512\"}") + "." + B64("{\"sub\":\"0101234567\",\"exp\":" + exp + "}") + ".sig";
        var t = new HddtToken(jwt, DateTimeOffset.UtcNow);
        Assert.Equal(exp, t.ExpiresAt!.Value.ToUnixTimeSeconds());
        Assert.True(t.IsValid());
    }
}

/// <summary>Giả lập cổng Thuế để chạy toàn bộ luồng tải mà không gọi mạng thật.</summary>
public class FakePortal : HttpMessageHandler
{
    public List<string> Calls { get; } = new();
    int _rateLimitOnce = 1;

    static HttpResponseMessage Json(string body, HttpStatusCode code = HttpStatusCode.OK) =>
        new(code) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    static string Item(string nbmst, int khms, string khhdon, int shdon) =>
        $"{{\"nbmst\":\"{nbmst}\",\"khmshdon\":{khms},\"khhdon\":\"{khhdon}\",\"shdon\":{shdon},\"tdlap\":\"2026-05-10T00:00:00Z\",\"nbten\":\"CÔNG TY Á ĐÔNG\"}}";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken ct)
    {
        var url = r.RequestUri!.ToString();
        Calls.Add(r.Method + " " + url);
        var path = r.RequestUri.AbsolutePath;

        if (path.EndsWith("/captcha"))
            return Task.FromResult(Json("{\"key\":\"k1\",\"content\":\"<svg></svg>\"}"));
        if (path.EndsWith("/security-taxpayer/authenticate"))
        {
            var body = r.Content!.ReadAsStringAsync(ct).Result;
            return Task.FromResult(body.Contains("\"cvalue\":\"OK\"")
                ? Json("{\"token\":\"abc.def.ghi\"}")
                : Json("{\"message\":\"Mã captcha không đúng\"}", HttpStatusCode.BadRequest));
        }

        Assert.Equal("Bearer abc.def.ghi", r.Headers.Authorization?.ToString());

        if (path.EndsWith("/query/invoices/purchase"))
        {
            if (_rateLimitOnce-- > 0) return Task.FromResult(new HttpResponseMessage((HttpStatusCode)429)); // thử lại được
            return Task.FromResult(url.Contains("state=P2")
                ? Json("{\"datas\":[" + Item("0101234567", 1, "K26TAA", 3) + "],\"state\":null}")
                : Json("{\"datas\":[" + Item("0101234567", 1, "K26TAA", 1) + "," + Item("0101234567", 1, "K26TAA", 2) + "],\"state\":\"P2\"}"));
        }
        if (path.EndsWith("/sco-query/invoices/purchase"))
            return Task.FromResult(Json("{\"datas\":[" + Item("0309999999", 2, "K26MAB", 77) + "],\"state\":null}"));
        if (path.EndsWith("/invoices/detail"))
            return Task.FromResult(Json("{\"shdon\":1,\"hdhhdvu\":[{\"ten\":\"Hàng A\"}]}"));
        if (path.EndsWith("/invoices/export-xml"))
        {
            if (url.Contains("&shdon=2&")) return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            var ms = new MemoryStream();
            using (var z = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
            using (var w = new StreamWriter(z.CreateEntry("invoice.xml").Open())) w.Write("<HDon/>");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(ms.ToArray()) });
        }
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}

public class DownloaderTests
{
    static HddtClient NewClient(FakePortal p) =>
        new(new HddtClientOptions { MinRequestInterval = TimeSpan.Zero, BaseUrl = "https://hoadondientu.gdt.gov.vn/api" }, p);

    [Fact]
    public async Task Login_retries_on_captcha_error_then_asks_user()
    {
        var portal = new FakePortal();
        using var c = NewClient(portal);
        var asked = 0;
        var token = await c.LoginAutoAsync("0101234567", "pw", cap => { asked++; return Task.FromResult("OK"); });
        Assert.Equal("abc.def.ghi", token.Value);
        Assert.Equal(1, asked); // captcha "<svg></svg>" không đọc được -> hỏi ngay
    }

    [Fact]
    public async Task Full_month_download_writes_expected_files()
    {
        var portal = new FakePortal();
        using var c = NewClient(portal);
        c.Token = new HddtToken("abc.def.ghi", DateTimeOffset.UtcNow);
        var root = Path.Combine(Path.GetTempPath(), "hddt-test-" + Guid.NewGuid().ToString("N"));
        var store = new InvoiceFileStore(root);

        var result = await new HddtDownloader(c, store).DownloadAsync(new DownloadRequest
        {
            Mst = "2802274721",
            From = new DateTime(2026, 5, 1),
            To = new DateTime(2026, 5, 31),
            Directions = new[] { InvoiceDirection.Purchase },
            DownloadXml = true,
            ExtractXml = true,
        });

        var dir = Path.Combine(root, "2802274721", "mua", "2026-05");
        Assert.Equal(4, result.InvoiceCount);           // 2 + 1 trang query, 1 sco
        Assert.Equal(0, result.ErrorCount);              // 429 được thử lại; XML 500 = "none", không phải lỗi
        Assert.True(File.Exists(Path.Combine(dir, "list_query_001.json")));
        Assert.True(File.Exists(Path.Combine(dir, "list_query_002.json")));
        Assert.True(File.Exists(Path.Combine(dir, "list_sco_001.json")));
        Assert.True(File.Exists(Path.Combine(dir, "0101234567_1_K26TAA_1.json")));   // MST giữ số 0 đầu
        Assert.True(File.Exists(Path.Combine(dir, "0309999999_2_K26MAB_77.json")));
        Assert.True(File.Exists(Path.Combine(dir, "0101234567_1_K26TAA_1", "invoice.xml")));
        Assert.False(File.Exists(Path.Combine(dir, "0101234567_1_K26TAA_2.zip")));

        var index = JsonDocument.Parse(File.ReadAllText(Path.Combine(dir, "index.json"))).RootElement;
        Assert.Equal(4, index.GetArrayLength());
        Assert.Equal("none", index.EnumerateArray().First(e => e.GetProperty("shdon").GetString() == "2").GetProperty("xml").GetString());
        Assert.Equal("sco", index[3].GetProperty("source").GetString());
        Assert.Contains("Á ĐÔNG", File.ReadAllText(Path.Combine(dir, "list_query_001.json")));

        var listCall = portal.Calls.First(u => u.Contains("/query/invoices/purchase"));
        Assert.Contains("search=tdlap=ge=01/05/2026T00:00:00;tdlap=le=31/05/2026T23:59:59", listCall);
        Assert.Contains(portal.Calls, u => u.Contains("/sco-query/invoices/detail?nbmst=0309999999&khhdon=K26MAB&shdon=77&khmshdon=2"));

        // Chạy lại: trang danh sách cũ được thay, XML đã có thì bỏ qua
        portal.Calls.Clear();
        await new HddtDownloader(c, store).DownloadAsync(new DownloadRequest
        {
            Mst = "2802274721", From = new DateTime(2026, 5, 1), To = new DateTime(2026, 5, 31),
            Directions = new[] { InvoiceDirection.Purchase }, DownloadXml = true,
        });
        Assert.Equal(1, portal.Calls.Count(u => u.Contains("export-xml"))); // chỉ hóa đơn số 2 (lần trước "none")
        Directory.Delete(root, true);
    }
}
