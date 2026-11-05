# Quy tắc Phát triển & Phiên bản Công nghệ Chuẩn

## 1. Công nghệ & Thư viện Cố định
- **DevExpress:** Chuẩn **DevExpress v25** (phiên bản đang dùng: `25.2.3`). Áp dụng cho toàn bộ WPF UI components, Data Grid, LayoutControl và XtraReport.
- **ORM:** Chuẩn **LLBLGen Pro v5.12** (phiên bản package: `5.12.0`). Tuân thủ chặt chẽ cấu trúc Entity, TypedList, Unit of Work / Adapter của LLBLGen. Không tự ý thay đổi hay nâng cấp version ORM.
- **Nền tảng:** C# / WPF thuần túy trên .NET mới nhất (.NET 10).

## 2. Kỷ luật Kiến trúc
- **Accounting-first:** Ưu tiên hàng đầu là sự chuẩn xác của số liệu kế toán, định khoản nợ/có, sổ cái.
- **Cấm WinForms:** TUYỆT ĐỐI KHÔNG dùng thư viện, control, hay namespace của Windows Forms (`System.Windows.Forms`).
- **MVVM:** Tách biệt triệt để Logic và View. Không viết business logic trong Code-Behind (`.xaml.cs`).
- **DB-Agnostic:** Không dùng Stored Procedure, xử lý nghiệp vụ hoàn toàn qua ORM và OOP.
- **In ấn & Báo cáo:** 
  - Báo cáo sổ sách/BCTC: Dùng `XtraReport` với Band layout thuần WPF.
  - Danh mục: Mới sử dụng `GridControl` + `ReportManager.PrintGridControl`.

Tham khảo chi tiết tại `.ai_context/02_rules/csharp_rules.md` và `.ai_context/02_rules/report_rules.md`.
