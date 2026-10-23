# Hddt – module tải hóa đơn điện tử từ cổng Thuế (C#)

Viết lại từ phần tải hóa đơn của file `TaiHoaDonDienTu_v6.2` (VBA). Chỉ làm một việc:
**đăng nhập → tải danh sách, chi tiết, XML gốc → lưu nguyên văn ra JSON**.
Xử lý, đối chiếu, hạch toán để phần mềm kế toán làm sau.

## Cấu trúc

```
src/Hddt/                 thư viện (netstandard2.0 + net8.0: dùng được cho .NET Framework 4.6.2+ và .NET 6/8)
  HddtClient.cs           gọi API cổng: captcha, đăng nhập, danh sách, chi tiết, export-xml
  SvgCaptchaSolver.cs     đọc captcha SVG (chuyển từ modDetectCaptcha)
  HddtDownloader.cs       điều phối: chia tháng → danh sách (query + sco, đủ trang) → chi tiết → XML
  InvoiceFileStore.cs     lưu file theo thư mục
  DateRanges.cs, Models.cs
samples/Hddt.ConsoleDemo  ví dụ chạy thử từ dòng lệnh
tests/Hddt.Tests          xUnit, có cổng giả lập (không gọi mạng thật)
```

## Dùng trong phần mềm

```csharp
using var client = new HddtClient();            // mỗi MST một client
await client.LoginAutoAsync(mst, matKhau,
    askUser: cap => HienCaptchaChoNguoiDung(cap.Svg));   // chỉ gọi khi tự đọc captcha không được

var store = new InvoiceFileStore(@"D:\KeToan\HoaDonThue");
var result = await new HddtDownloader(client, store).DownloadAsync(new DownloadRequest
{
    Mst = mst,
    From = new DateTime(2026, 4, 1),
    To = new DateTime(2026, 6, 30),
    DownloadDetail = true,
    DownloadXml = true,
}, new Progress<DownloadProgress>(p => lblTrangThai.Text = p.ToString()));
```

Lưu `client.Token.Value` lại để lần sau gán `client.Token = new HddtToken(value, obtainedAt)`.
Dùng `IsValid()` để biết token còn hạn hay không, khỏi phải đăng nhập lại.

## Dữ liệu lưu ra

```
{root}/{MST}/{mua|ban}/{yyyy-MM}/
    list_query_001.json ...   nguyên văn danh sách (hóa đơn thường)
    list_sco_001.json ...     nguyên văn danh sách (máy tính tiền)
    {nbmst}_{khmshdon}_{khhdon}_{shdon}.json   chi tiết, gồm hàng hóa (hdhhdvu)
    {nbmst}_{khmshdon}_{khhdon}_{shdon}.zip    XML gốc
    index.json    danh sách hóa đơn của tháng + tình trạng chi tiết/XML
    _log.json     thời điểm tải, số lượng, lỗi, đã chạy xong hay chưa
```

- Tên file theo bộ 4 khóa của hóa đơn, nên **tải lại sẽ ghi đè**, không bị trùng, và trạng thái mới nhất luôn được cập nhật.
- MST và số hóa đơn luôn được giữ dạng chuỗi, không bị mất số 0 đầu.
- Module xử lý chỉ cần đọc `index.json`, rồi mở file chi tiết theo `fileStem`.

## Cách xử lý lỗi

| Tình huống | Cách xử lý |
|---|---|
| 429 (gọi quá nhanh) | chờ (theo Retry-After hoặc 5s, 10s, 15s) rồi thử lại. Giữa các request luôn nghỉ 0,7s (`MinRequestInterval`) |
| 502/503/504, lỗi mạng | thử lại tối đa 3 lần |
| export-xml trả 500 | coi là hóa đơn không có hồ sơ gốc: `xml = "none"`, giống bản VBA |
| lỗi ở một tháng hoặc một hóa đơn | ghi vào `_log.json` và `index.json`, chạy tiếp phần còn lại |
| 401 (token hết hạn) | dừng và ném `HddtException` (StatusCode 401). Index/log của tháng đang tải vẫn được ghi |

## Build

```
dotnet build
dotnet test
dotnet run --project samples/Hddt.ConsoleDemo -- 0101234567 01/05/2026 31/05/2026 data --xml
```

## Lưu ý

- Module gọi API nội bộ của trang web hoadondientu.gdt.gov.vn (giống bản VBA). Đây không phải kênh kết nối chính thức, nên cổng có thể đổi API hoặc captcha bất cứ lúc nào.
- Bảng nhận dạng captcha lấy nguyên từ bản VBA. Nếu cổng đổi font captcha, `Guess` sẽ rỗng hoặc sai. Khi đó `LoginAutoAsync` sẽ gọi `askUser`, nên luôn truyền hàm này vào.
- Không lưu mật khẩu Thuế dạng chữ thường. Nếu phải lưu, hãy mã hóa (ví dụ DPAPI trên Windows: `ProtectedData.Protect`).
