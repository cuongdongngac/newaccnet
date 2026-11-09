# Quy tắc Phát triển & Phiên bản Công nghệ Chuẩn

> **⚠️ ƯU TIÊN CAO NHẤT cho mọi việc liên quan AI và Plugin:** đọc `docs/AI_PLATFORM_AND_PLUGIN_SPEC_v5.md` trước khi viết code. Khi mâu thuẫn với các tài liệu khác về AI/Plugin, làm theo tài liệu đó. Chỉ làm đúng task được giao (A1, A2, B1… theo Mục 12), chỉ sửa file trong danh sách "Được sửa" của task, dừng sau mỗi task.


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

## 3. Kiến Trúc Plugin & Trí Tuệ Nhân Tạo (AI)
- **Plugin - Clean Core:** Không `ALTER TABLE` thêm/xóa cột trên các bảng Core. Dữ liệu mở rộng dùng bảng riêng 1-1 (`Plg_...`) hoặc JSON attributes.
- **Plugin - Self-cleaning:** Khi gỡ bỏ (Uninstall), plugin tự chịu trách nhiệm dọn dẹp các bảng của nó (`IPluginLifecycle`). Plugin Manager điều phối, cảnh báo người dùng và bảo vệ tuyệt đối các bảng Core.
- **AI - Core Platform:** Core quản lý tập trung 1 cổng OpenRouter (API Key, Base URL, định tuyến Model, đếm token) và là chốt chặn kiểm toán (kiểm tra cân Nợ/Có của mọi bút toán AI đề xuất).
- **AI - Plugin Skills:** Plugin không tạo API Key riêng; Plugin gọi nhờ hạ tầng AI của Core, cung cấp Prompt nghiệp vụ chuyên sâu và đăng ký các AI Tools mở rộng.

Tham khảo chi tiết tại `.ai_context/02_rules/csharp_rules.md`, `.ai_context/02_rules/report_rules.md` và `.ai_context/02_rules/plugin_and_ai_architecture_rules.md`.
