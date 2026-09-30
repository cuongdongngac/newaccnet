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
