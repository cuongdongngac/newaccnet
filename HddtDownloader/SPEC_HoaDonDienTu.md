# Đặc tả: Module Hóa đơn điện tử (tải từ cổng Thuế) cho phần mềm kế toán

> Tài liệu này dành cho AI lập trình (AI-coding) làm tiếp. Nội dung gồm: bối cảnh, phân tích tool Excel/VBA gốc,
> thư viện C# đã có, thiết kế dữ liệu, các quyết định đã chốt với chủ dự án và việc cần làm theo từng giai đoạn.
> Đọc hết mục 1–3 trước khi viết code.

---

## 1. Bối cảnh và nguyên tắc

- Chủ dự án đang viết **phần mềm kế toán bằng C#**, theo hướng **accounting first**: **sổ là nguồn sự thật**.
  Mọi báo cáo thuế và báo cáo tài chính đi ra từ sổ.
- Cấu trúc sổ của phần mềm là dạng phẳng: **Định khoản → Chi tiết**, trong đó phần chi tiết chứa **thông tin hóa đơn GTGT (VAT)**.
- Dữ liệu hóa đơn tải từ cổng Thuế (hoadondientu.gdt.gov.vn) **không tự sinh bút toán**. Nó chỉ dùng để:
  1. **Tra cứu và điền sẵn** thông tin VAT khi kế toán nhập chứng từ (tiện ích, không bắt buộc).
  2. **Đối chiếu** sổ với cổng: hóa đơn sót, hóa đơn bị hủy hoặc thay thế, lệch tiền (đây là công dụng chính).
  3. **Báo cáo về hóa đơn** (danh sách, tổng hợp theo nhà cung cấp hoặc khách hàng): lấy thẳng từ dữ liệu tải về.
  4. **Import danh mục đối tượng** (đối tượng công nợ, đối tượng VAT) theo MST.
- **KHÔNG** tự động hạch toán, **KHÔNG** đoán tài khoản đối ứng. Kế toán tự định khoản.
- **KHÔNG** lấy dữ liệu cổng để lập tờ khai thuế. Tờ khai lấy từ sổ, vì danh sách trên cổng khác với danh sách được kê khai:
  có hóa đơn không được khấu trừ, có hóa đơn khác kỳ, và thuế hàng nhập khẩu nằm trên tờ khai hải quan, không có trên cổng.

---

## 2. Tool gốc: `TaiHoaDonDienTu_v6.2_ThuyHien.xlsm` (VBA, tác giả "ongke0711")

### 2.1 Chức năng
Tool Excel tự đăng nhập cổng Thuế, tải hóa đơn **mua vào/bán ra** theo khoảng ngày rồi ghi ra các sheet:

| Sheet | Nội dung |
|---|---|
| MENU | 3 nút: Đăng nhập / Tải hóa đơn / Trích xuất XML. Cột A–E lưu user, **mật khẩu dạng chữ thường**, tên đơn vị, token, thời điểm |
| TongHopHD_Mua / TongHopHD_Ban | Mỗi hóa đơn 1 dòng (~56 cột) |
| ChiTietHD_Mua / ChiTietHD_Ban | Mỗi dòng hàng hóa 1 dòng |
| ChiTietHD_*_XML | Dữ liệu trích từ file XML có sẵn trên máy |
| LinkTraCuu | Link tra cứu PDF của các nhà cung cấp hóa đơn + bảng mã `tthai`, `ttxly` |
| Thamkhao (ẩn) | Tham khảo |

### 2.2 Cấu trúc VBA (project "TaiHDDT", không khóa mật khẩu)
| Thành phần | Vai trò |
|---|---|
| `modCommands` | `DangNhap` → `frmDangNhap.Show`; `TaiHoaDon` → `frmTaiHoaDon.Show`; `moUFTrichXuat` → `frmTrichXuatXML.Show` (gán vào nút trên MENU) |
| `frmDangNhap` | Lấy captcha, tự đọc, POST đăng nhập, lưu token |
| `frmTaiHoaDon` | Chọn mua/bán, ngày, bộ lọc; tải danh sách, chi tiết, XML |
| `frmTrichXuatXML` | Đọc XML/zip có sẵn → sheet `_XML` |
| `frmCapNhatMK` | Đổi mật khẩu đã lưu |
| `modDetectCaptcha` | Đọc captcha SVG (xem 3.2) |
| `modHTTPRequest` | Hàm GET/POST (MSXML2.ServerXMLHTTP) |
| `modGhiExcel` | Ánh xạ JSON → cột Excel (nguồn tên trường ở mục 4) |
| `JsonConverter` | Thư viện VBA-JSON |
| `modParseIso` | Chuyển ngày ISO |
| `Module1` | Tải PDF từ trang tra cứu của nhà cung cấp hóa đơn |
| `modMsgboxTV` | MsgBox tiếng Việt Unicode |

---

## 3. API cổng Thuế (đã trích từ VBA)

Base URL: `https://hoadondientu.gdt.gov.vn/api`.
Đây **không phải kênh tích hợp chính thức**: tool gọi API nội bộ của web, nên cổng có thể đổi bất cứ lúc nào.

### 3.1 Luồng
1. **Captcha:** `GET /captcha` → `{"key": "...", "content": "<svg ...>"}`
2. **Đăng nhập:** `POST /security-taxpayer/authenticate`
   body `{"username": MST, "password": "...", "cvalue": captcha, "ckey": key}`
   → thành công: `{"token": "<JWT>"}`; thất bại: `{"message": "..."}` (có chữ "captcha" nếu sai captcha).
   Các request sau gửi header `Authorization: Bearer <token>`.
3. **Danh sách:** `GET /query/invoices/{purchase|sold}?sort=tdlap:desc&size=50&search=<điều kiện>`
   - `search` = `tdlap=ge=dd/MM/yyyyT00:00:00;tdlap=le=dd/MM/yyyyT23:59:59`, có thể nối thêm `;tthai==X`, `;ttxly==Y`.
     Gửi **nguyên văn**, không URL-encode (`;` `=` `/` `:`), giống bản VBA.
   - **Tải theo từng tháng** (VBA chia khoảng ngày theo tháng).
   - **Phân trang:** phản hồi có `datas[]` và `state`. Nếu `state` khác null thì gọi lại với `&state=<state>`.
   - **Phải gọi thêm** `/sco-query/invoices/{purchase|sold}` với cùng tham số (hóa đơn có mã khởi tạo từ máy tính tiền). Thiếu đường này là sót hóa đơn.
4. **Chi tiết:** `GET /{query|sco-query}/invoices/detail?nbmst=&khhdon=&shdon=&khmshdon=` (theo nguồn của hóa đơn).
5. **XML gốc:** `GET /{query|sco-query}/invoices/export-xml?` cùng 4 tham số trên → file zip. **HTTP 500 nghĩa là không có hồ sơ gốc** (không phải lỗi).
6. **Lỗi:** 429 = gọi quá nhanh (phải nghỉ giữa các request); 401 = token hết hạn.

### 3.2 Thuật toán đọc captcha SVG
- Mỗi ký tự là một `<path d="...">`. Bỏ toạ độ, chỉ giữ dãy chữ lệnh `M/Q/Z` → được **chữ ký**, ví dụ `MQQQQQZMQQ...Z`.
- Tra chữ ký trong bảng 30 ký tự (A–Z trừ I, L, O, U; 2–9). Path không có trong bảng là nhiễu, bỏ qua.
- Sắp các ký tự theo **toạ độ x của lệnh M đầu tiên**.
- Bảng chữ ký đầy đủ nằm trong `src/Hddt/SvgCaptchaSolver.cs`, chép nguyên từ VBA.

### 3.3 Bảng mã
`tthai` (trạng thái hóa đơn): 1 Mới · 2 Thay thế · 3 Điều chỉnh · 4 Bị thay thế · 5 Đã bị điều chỉnh · 6 Bị hủy

`ttxly` (kết quả kiểm tra): 0 CQT đã nhận · 1 Đang kiểm tra điều kiện cấp mã · 2 CQT từ chối · 3 Đủ điều kiện cấp mã ·
4 Không đủ điều kiện cấp mã · 5 Đã cấp mã · 6 CQT đã nhận không mã · 7 Đã kiểm tra định kỳ HĐ không mã · 8 Có mã khởi tạo từ máy tính tiền

---

## 4. Các trường JSON (từ `modGhiExcel`)

### 4.1 Hóa đơn (mỗi phần tử `datas[]`; bản chi tiết có thêm `hdhhdvu`)
| Trường | Ý nghĩa |
|---|---|
| `nbmst`, `khmshdon`, `khhdon`, `shdon` | **Khóa duy nhất**: MST người bán, mẫu số, ký hiệu, số hóa đơn |
| `tlhdon` | Tên loại hóa đơn |
| `tdlap` | Ngày lập (ISO) |
| `dvtte`, `tgia` | Đơn vị tiền tệ, tỷ giá |
| `nbten`, `nbdchi` | Tên, địa chỉ người bán |
| `nmmst`, `nmten`, `nmdchi` | MST, tên, địa chỉ người mua |
| `nky` | Ngày ký số người bán |
| `mhdon`, `ncma` | Mã của CQT, ngày cấp mã |
| `thttltsuat[]` | **Tổng hợp theo thuế suất**: `tsuat`, `thtien` (chưa thuế), `tthue`, `gttsuat`. Một hóa đơn có thể có nhiều thuế suất |
| `tgtcthue`, `tgtthue`, `tgtttbso`, `tgtttbchu` | Tổng chưa thuế, tổng thuế, tổng thanh toán (số, chữ) |
| `tgtkcthue`, `ttcktmai`, `tgtkhac`, `thttlphi[]` | Không chịu thuế, chiết khấu thương mại, giảm trừ khác, phí |
| `gchu` | Ghi chú |
| `tthai`, `ttxly` | Trạng thái, kết quả kiểm tra (mục 3.3) |
| `msttcgp` | MST tổ chức cung cấp giải pháp hóa đơn |

### 4.2 Dòng hàng hóa (`hdhhdvu[]` trong phản hồi chi tiết)
`stt`, `tchat` (tính chất), `mhhdvu` (mã hàng), `ten`, `dvtinh`, `sluong`, `dgia`, `tlckhau`, `stckhau`, `ltsuat`, `tsuat`, `thtien`, `tthue`, `thtcthue`

> Dữ liệu thực có tiền âm (hóa đơn điều chỉnh giảm). Thuế suất có thể là số thập phân (0.1) hoặc chuỗi (KCT, KKKNT). Phải xử lý cả hai dạng.

---

## 5. Thư viện C# đã có: `HddtDownloader` (giai đoạn 1, đã xong)

Solution gồm `src/Hddt` (thư viện, đích **netstandard2.0 + net8.0**), `samples/Hddt.ConsoleDemo`, `tests/Hddt.Tests` (xUnit, có cổng giả lập).

| Lớp | Việc |
|---|---|
| `HddtClient` | `GetCaptchaAsync`, `LoginAsync`, `LoginAutoAsync(askUser)` (tự đọc captcha, sai thì thử lại, cần thì hỏi người dùng), `GetInvoicePageAsync(direction, source, from, to, tthai, ttxly, state)`, `GetDetailJsonAsync(key)`, `ExportXmlZipAsync(key)` (null nếu 500). Nghỉ 700ms giữa các request, thử lại khi 429/5xx/lỗi mạng |
| `SvgCaptchaSolver.Solve(svg)` | Đọc captcha |
| `HddtToken` | Token và hạn dùng đọc từ JWT, `IsValid()` |
| `DateRanges.SplitByMonth` | Chia khoảng ngày theo tháng |
| `HddtDownloader.DownloadAsync(DownloadRequest, IProgress)` | Tháng → danh sách (query + sco, đủ trang) → chi tiết → XML. Lỗi được ghi log rồi chạy tiếp; gặp 401 thì dừng |
| `InvoiceFileStore` | Lưu file (dưới đây) |

Thư mục lưu:
```
{root}/{MST}/{mua|ban}/{yyyy-MM}/
  list_query_001.json, list_sco_001.json   nguyên văn danh sách
  {nbmst}_{khmshdon}_{khhdon}_{shdon}.json  chi tiết
  {nbmst}_{khmshdon}_{khhdon}_{shdon}.zip   XML gốc
  index.json   [{nbmst,khmshdon,khhdon,shdon,source,tdlap,fileStem,detail,xml,error}]
  _log.json    {mst,direction,from,to,startedAt,finishedAt,invoiceCount,pagesBySource,errors,completed}
```

**Tình trạng kiểm thử:** build net8.0 sạch, 7 test đạt trên cổng giả lập.
**Chưa** chạy với cổng thật, **chưa** build target netstandard2.0 (cần NuGet System.Text.Json). Việc đầu tiên khi tiếp nhận là chạy `dotnet build`, `dotnet test`, rồi chạy ConsoleDemo với một tháng thật để xác nhận.

---

## 6. Quyết định đã chốt với chủ dự án

1. **Lưu trữ hai lớp:** file JSON/XML trên đĩa là bản lưu nguyên văn, có thể chạy lại bước nhập từ file. **Cơ sở dữ liệu** là nơi phần mềm làm việc.
2. **Nguồn tra cứu khi nhập VAT:** tìm trong cơ sở dữ liệu trước. Không thấy thì gọi cổng tải danh sách của tháng đó, lưu vào cơ sở dữ liệu rồi trả kết quả. Chỉ hỏi đăng nhập khi token hết hạn.
3. **Mã đối tượng = MST** (đã chuẩn hóa). Chi nhánh `0101234567-001` (14 ký tự) là đối tượng riêng. Cá nhân, hộ kinh doanh có MST 12 số (CCCD). Không có MST thì dùng "Khách lẻ" hoặc mã tự sinh `KL0001`.
   **Trường mã đối tượng phải nới từ 10 lên ≥ 20 ký tự**, ở mọi bảng có tham chiếu và trên form.
4. Đối tượng công nợ và đối tượng VAT dùng **chung một danh mục**, khóa là mã đối tượng.

---

## 7. Việc cần làm tiếp

### Giai đoạn 2: Nhập vào cơ sở dữ liệu (`HoaDonThueImporter`)
Hỏi chủ dự án dùng **SQL Server hay SQLite** trước khi viết.

```sql
HoaDonThue
  Id               PK
  ChieuHD          'mua' | 'ban'
  MstNguoiBan      varchar(20)    -- chuỗi
  MauSo            varchar(5)
  KyHieu           varchar(10)
  SoHD             bigint
  NgayLap          date
  MstNguoiMua      varchar(20) NULL
  MaDoiTuong       varchar(20) NULL  -- = MST đối tác (mua: người bán; bán: người mua)
  TenDoiTac        nvarchar(400)
  TongChuaThue, TongThue, TongThanhToan  decimal(19,4)
  TrangThai        int  -- tthai
  KetQuaKiemTra    int  -- ttxly
  Nguon            'query' | 'sco'
  RawJson          nvarchar(max) / TEXT   -- nguyên văn chi tiết
  NgayTai          datetime
  ChungTuId        NULL = chưa dùng; có giá trị = đã gắn vào chứng từ
  UNIQUE (MstNguoiBan, MauSo, KyHieu, SoHD)

HoaDonThue_ThueSuat           -- 1 dòng / phần tử thttltsuat
  HoaDonThueId FK, ThueSuat (varchar: '10%','8%','0%','KCT','KKKNT'...), TienChuaThue, TienThue
```
- **Upsert** theo khóa duy nhất: cập nhật trạng thái, số tiền, RawJson, **giữ nguyên `ChungTuId`**.
- Nguồn dữ liệu: `index.json` + file chi tiết (nếu chưa có chi tiết thì lấy dòng tương ứng trong `list_*.json`).
- Chuẩn hóa: MST bỏ khoảng trắng (giữ số 0 đầu và đuôi `-xxx`); ký hiệu viết hoa, bỏ khoảng trắng; số hóa đơn kiểu số; ngày ISO đổi về `date`.

### Giai đoạn 3: Tra cứu khi nhập chi tiết VAT
- Kế toán gõ số hóa đơn (có thể kèm MST). **Số hóa đơn một mình không duy nhất**: nếu ra nhiều kết quả thì hiện danh sách (người bán, ký hiệu, ngày, tổng tiền) để chọn.
- Chọn xong thì điền: mẫu số, ký hiệu, số, ngày, MST, tên, địa chỉ, và **mỗi thuế suất thành một dòng VAT** (thuế suất, tiền chưa thuế, tiền thuế).
- Ghi `ChungTuId` để đánh dấu đã dùng. **Chặn** dùng một hóa đơn cho hai chứng từ (hoặc cảnh báo).
- Cảnh báo khi `tthai` ∈ {4, 6} (bị thay thế, bị hủy) hoặc ∈ {3, 5} (liên quan điều chỉnh), và khi tổng tiền định khoản lệch với hóa đơn.
- Không thấy trong cơ sở dữ liệu thì tra cổng theo mục 6.2.
- Kế toán nhập tay (không qua tìm kiếm) cũng phải qua **cùng một hàm chuẩn hóa** ở giai đoạn 2, nếu không sẽ không đối chiếu được.

### Giai đoạn 4: Import danh mục đối tượng
- Lấy các MST khác nhau từ `HoaDonThue` (mua: `nbmst/nbten/nbdchi` là nhà cung cấp; bán: `nmmst/nmten/nmdchi` là khách hàng; có cả hai thì gán cả hai), kèm tên và địa chỉ của hóa đơn mới nhất.
- Đối tượng đã có thì **không ghi đè**. Tên hoặc địa chỉ khác thì liệt kê để kế toán duyệt.
- Màn hình xem trước rồi bấm import.

### Giai đoạn 5: Báo cáo
- **Từ dữ liệu cổng:** bảng kê hóa đơn mua/bán theo kỳ; tổng hợp theo đối tượng; danh sách hóa đơn bị hủy, thay thế, điều chỉnh.
- **Đối chiếu sổ với cổng** (khớp theo 4 khóa đã chuẩn hóa):
  1. Có trên cổng, chưa có trong sổ (`ChungTuId IS NULL`).
  2. Có trong sổ, không có trên cổng, hoặc cổng báo `tthai` = 4/6.
  3. Khớp khóa nhưng lệch tiền chưa thuế hoặc tiền thuế.
  4. Tổng hợp theo từng đối tượng: số hóa đơn trên cổng so với số hóa đơn trong sổ.

### Giai đoạn 6: Giao diện (WinForms hoặc nền tảng của phần mềm)
- Form đăng nhập: MST, mật khẩu, khung captcha (**SVG**: dùng WebView2 `NavigateToString` hoặc NuGet `Svg` để vẽ ra Bitmap; chỉ cần khi tự đọc thất bại), ô captcha.
- Form tải: từ/đến ngày, mua/bán, tải XML, thư mục lưu, tiến độ, log lỗi.
- Gọi bằng `async/await`; dùng `Progress<T>` tạo trên luồng UI. Gặp `HddtException.StatusCode == 401` thì mở lại form đăng nhập.
- Lưu token (mã hóa, ví dụ DPAPI) để dùng lại. **Không lưu mật khẩu dạng chữ thường** (lỗi của bản VBA).

---

## 8. Tiêu chí nghiệm thu chung
- Tải lại cùng kỳ **không sinh bản ghi trùng** và không làm mất đánh dấu `ChungTuId`.
- MST có số 0 đầu, MST chi nhánh 14 ký tự và MST 12 số đều được lưu và đối chiếu đúng.
- Hóa đơn nhiều thuế suất sinh đủ số dòng VAT, tổng các dòng bằng `tgtthue`.
- Hóa đơn tiền âm (điều chỉnh giảm) được giữ nguyên dấu.
- Có unit test cho: chuẩn hóa khóa, upsert, tra cứu nhiều kết quả, đối chiếu 4 loại chênh lệch.
- Không có code nào sinh bút toán tự động từ dữ liệu cổng.

## 9. Rủi ro và điểm chưa xác minh
- API nội bộ cổng Thuế có thể đổi; bảng captcha có thể lỗi thời → luôn có đường nhập captcha tay.
- Cú pháp lọc thêm kiểu `;shdon==...;nbmst==...` **chưa thử** trên cổng thật; thử trước khi dựa vào.
- Giới hạn tốc độ thực của cổng chưa đo; mặc định nghỉ 700ms, thử lại khi 429.
- Nếu đóng gói phần mềm để bán: cân nhắc kênh chính thức (T-VAN, nhà cung cấp hóa đơn) hoặc cho phép import file XML/Excel do người dùng tự tải từ cổng.
