// Ví dụ dùng thư viện Hddt: đăng nhập, tải hóa đơn mua/bán theo khoảng ngày, lưu JSON.
// Chạy: dotnet run --project samples/Hddt.ConsoleDemo -- <MST> <từ dd/MM/yyyy> <đến dd/MM/yyyy> [thư mục lưu] [--xml]
using System.Globalization;
using System.Text.Json;
using Hddt;

if (args.Length < 3)
{
    Console.WriteLine("Cách dùng: Hddt.ConsoleDemo <MST> <từ dd/MM/yyyy> <đến dd/MM/yyyy> [thư mục lưu] [--xml]");
    return 1;
}

Console.OutputEncoding = System.Text.Encoding.UTF8;
var mst = args[0];
var from = DateTime.ParseExact(args[1], "dd/MM/yyyy", CultureInfo.InvariantCulture);
var to = DateTime.ParseExact(args[2], "dd/MM/yyyy", CultureInfo.InvariantCulture);
var root = args.Length > 3 && !args[3].StartsWith("--") ? args[3] : "data";
var withXml = args.Contains("--xml");

using var client = new HddtClient();
var store = new InvoiceFileStore(root);

// Dùng lại token đã lưu nếu còn hạn (ví dụ đơn giản; phần mềm thật nên mã hóa file này).
var tokenFile = Path.Combine(store.RootFolder, mst, "token.json");
if (File.Exists(tokenFile))
{
    var saved = JsonSerializer.Deserialize<SavedToken>(File.ReadAllText(tokenFile));
    if (saved != null)
    {
        var t = new HddtToken(saved.Value, saved.ObtainedAt);
        if (t.IsValid()) { client.Token = t; Console.WriteLine($"Dùng token đã lưu, hết hạn {t.ExpiresAt:dd/MM/yyyy HH:mm}."); }
    }
}

if (client.Token == null)
{
    Console.Write("Mật khẩu: ");
    var password = ReadPassword();
    var token = await client.LoginAutoAsync(mst, password, askUser: cap =>
    {
        // Không tự đọc được: lưu SVG ra file để người dùng mở xem rồi nhập tay.
        var svgPath = Path.GetFullPath("captcha.svg");
        File.WriteAllText(svgPath, cap.Svg);
        Console.Write($"Mở {svgPath} và nhập captcha (máy đoán: '{cap.Guess}'): ");
        return Task.FromResult(Console.ReadLine() ?? "");
    });
    Directory.CreateDirectory(Path.GetDirectoryName(tokenFile)!);
    File.WriteAllText(tokenFile, JsonSerializer.Serialize(new SavedToken(token.Value, token.ObtainedAt)));
    Console.WriteLine($"Đăng nhập thành công, token hết hạn {token.ExpiresAt:dd/MM/yyyy HH:mm}.");
}

var downloader = new HddtDownloader(client, store);
var progress = new Progress<DownloadProgress>(p => Console.WriteLine(p));
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

try
{
    var result = await downloader.DownloadAsync(new DownloadRequest
    {
        Mst = mst,
        From = from,
        To = to,
        DownloadDetail = true,
        DownloadXml = withXml,
    }, progress, cts.Token);

    Console.WriteLine($"Đã lưu vào {store.RootFolder}");
    foreach (var e in result.Errors) Console.WriteLine("  Lỗi: " + e);
    return result.ErrorCount == 0 ? 0 : 2;
}
catch (HddtException ex) when (ex.StatusCode == 401)
{
    if (File.Exists(tokenFile)) File.Delete(tokenFile);
    Console.WriteLine("Token hết hạn. Chạy lại để đăng nhập; phần đã tải vẫn được giữ.");
    return 3;
}

static string ReadPassword()
{
    if (Console.IsInputRedirected) return Console.ReadLine() ?? "";
    var s = new System.Text.StringBuilder();
    while (true)
    {
        var k = Console.ReadKey(true);
        if (k.Key == ConsoleKey.Enter) break;
        if (k.Key == ConsoleKey.Backspace) { if (s.Length > 0) s.Length--; continue; }
        s.Append(k.KeyChar);
    }
    Console.WriteLine();
    return s.ToString();
}

record SavedToken(string Value, DateTimeOffset ObtainedAt);
