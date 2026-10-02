# Quy tắc In ấn & Báo cáo (Bắt buộc)

## Phân tách hai loại in

1. **Báo cáo / sổ sách / BCTC** (Nhật ký, Sổ cái, Cân đối PS, B01–B03, công nợ kỳ, …):
   - Làm **thuần WPF + DevExpress Reporting**.
   - Layout **bắt buộc dùng Band**: `ReportHeaderBand`, `PageHeaderBand`, `GroupHeaderBand`, `DetailBand`, `GroupFooterBand`, `ReportFooterBand`, `PageFooterBand` trên `DevExpress.XtraReports.UI.XtraReport`.
   - Filter kỳ/tham số = cửa sổ WPF (`ThemedWindow` / `BaseWindow`). Tính dữ liệu = Calculator. Dựng in = `*ReportFactory.CreateReport(...)`. Preview = `DevExpress.Xpf.Printing.PrintHelper.ShowPrintPreview`.
   - **Cấm** `GridControl` (kể cả grid ẩn trong memory), **cấm** `ReportManager.PrintGridControl`, **cấm** in từ `PrintableControlLink` của lưới cho loại này.

2. **Danh mục** (Tài khoản, Đối tác, Vật tư, TSĐB, …):
   - Màn hình xem/sửa = `GridControl`.
   - In danh mục = `ReportManager.PrintGridControl(gridControl, tiêu đề)` — **chỉ dùng cho danh mục**.

## Mẫu đúng / sai

```csharp
// ĐÚNG — báo cáo
var rows = new DiaryCalculator().Calculate(from, to, onlyBooked);
var report = DiaryReportFactory.CreateReport(rows, from, to);
PrintHelper.ShowPrintPreview(owner, report);

// SAI — báo cáo
var grid = DiaryReportGridFactory.Build(rows); // GridControl ẩn
ReportManager.PrintGridControl(grid, "SỔ NHẬT KÝ"); // infinite height / layout không phải band
```

Tham chiếu: `NewaccNet.Wpf/System/Reports/DiaryReportFactory.cs`, `ChartOfAccountReportFactory.cs`.
Đóng vai trò là một chuyên gia kiến trúc phần mềm C# WPF. Chúng ta đang xây dựng module Báo cáo (Reporting) theo kiến trúc tách biệt hoàn toàn giữa Design-Time (Lúc thiết kế kéo thả) và Run-Time (Lúc chạy thực tế).

Tôi yêu cầu sự tường minh tuyệt đối. Bạn không được gộp code, không được viết tắt. Với mỗi một báo cáo tôi yêu cầu, bạn phải viết mã nguồn chia làm 4 phần rõ rệt (có thể hơi thừa nhưng bắt buộc để dễ kiểm soát).

Dưới đây là quy trình 4 bước bạn phải tuân thủ khi tôi cung cấp cấu trúc của một báo cáo mới:

Bước 1: Định nghĩa Data Schema (DTO)

Viết một class DTO thuần túy chỉ chứa các Auto-Properties.

Đặt tên class theo mẫu: [TênBáoCáo]ReportDTO.

Ghi chú rõ kiểu dữ liệu và ý nghĩa của từng trường để bộ Report Designer có thể tự động đọc bằng Reflection.

Bước 2: Viết Service Sinh dữ liệu giả (Mock Data Generator)

Viết một class [TênBáoCáo]MockDataService.

Bên trong có hàm GenerateMockList() trả về một List<[TênBáoCáo]ReportDTO> chứa khoảng 5-10 dòng dữ liệu giả nhưng sát với thực tế nhất (có số dư, có số âm dương, ngày tháng chuẩn).

Viết thêm hàm ExportMockToJson(string filePath) để serialize cái List này ra file .json. File này sẽ được giao cho cán bộ Design để họ nạp vào làm Design-Time Data xem trước định dạng (Preview Format).

Bước 3: Viết hàm Export Schema (Tùy chọn cho Standalone Designer)

Viết một đoạn code ngắn dùng DataSet hoặc XmlSerializer để xuất cấu trúc class DTO ở Bước 1 ra file .xsd (XML Schema). File này dùng để cán bộ Design nạp vào phần mềm thiết kế lấy khung các trường (Field List) kéo thả.

Bước 4: Viết khung hàm Run-Time (Nạp dữ liệu thực và View)

Viết một hàm mẫu mô phỏng lúc phần mềm đang chạy: Load file template báo cáo (.mrt, .repx, v.v.), gán DataSource của báo cáo đó bằng một List<DTO> thực tế, và gọi lệnh ShowPreview().

THÔNG TIN BÁO CÁO CẦN LÀM LẦN NÀY:

Tên báo cáo: Sổ chi tiết tài khoản (Ledger)

Các trường cần có: RowType (int), VoucherDate (DateTime), VoucherNo (string), Contents (string), AccountId (string), CounterAccountId (string), Amount (decimal), Dbcr (int), Balance (decimal).

Hãy sinh code C# thực thi đúng 4 bước trên.

KẾT THÚC COPY.