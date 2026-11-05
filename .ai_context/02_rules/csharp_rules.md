# Quy tắc C# & Công nghệ (Bắt buộc)
- **Bản chất dự án:** "Accounting-first" (Ưu tiên tuyệt đối tính chính xác dữ liệu, nghiệp vụ tài chính, định khoản, sổ cái trước UI).
- **Công nghệ cốt lõi:** WPF thuần túy trên .NET mới nhất (.NET 10).
- **CẢNH BÁO NGHIÊM NGẶT:** TUYỆT ĐỐI KHÔNG dùng thư viện, sự kiện, control, namespace của WinForms (vd: `System.Windows.Forms`). AI phải kiểm tra kỹ, không nhầm lẫn WinForms và WPF.
- **UI/UX & Components:** Dùng TOÀN BỘ controls của **DevExpress v25** (phiên bản chuẩn `25.2.x`) cho WPF (GridControl, LayoutControl, DXWindow, SimpleButton, XtraReport...). Nghiêm cấm dùng controls mặc định WPF trừ khi có chỉ định riêng. Không tự ý nâng cấp lên version khác khi chưa có yêu cầu.
- **ORM:** Dùng độc quyền **LLBLGen Pro v5.12** (`5.12.0`). Tuân thủ đúng cấu trúc Entity, TypedList, và Adapter/SelfServicing.
- **In ấn:** Báo cáo/sổ sách/BCTC = `XtraReport` + Band (xem `.ai_context/02_rules/report_rules.md`). `GridControl` + `ReportManager.PrintGridControl` **chỉ** cho in danh mục.
- **Kiến trúc:** MVVM nghiêm ngặt. Không viết logic nghiệp vụ/dữ liệu vào Code-Behind (`.xaml.cs`). Toàn bộ sự kiện từ DevExpress phải chuyển sang ViewModel qua EventToCommand hoặc Binding.

- **Kiến trúc DB-Agnostic (CORE DIRECTIVE):** Ứng dụng đa nền tảng, lớp giao tiếp không biết DB là gì. BẮT BUỘC sử dụng ORM và Lập trình Hướng đối tượng (OOP) cho toàn bộ logic/tính toán. TUYỆT ĐỐI KHÔNG dùng bất kỳ thủ tục (Stored Procedure) hay hàm cơ sở dữ liệu nào để xử lý nghiệp vụ.
- **Nguyên tắc Đơn nhiệm & Hiệu năng dữ liệu (Single Responsibility & Performance):** Nguồn dữ liệu danh mục cho LookUp/Combobox/Bộ lọc phải độc lập hoàn toàn, dùng đối tượng/DTO riêng (POCO), chỉ query đúng bảng danh mục, TUYỆT ĐỐI KHÔNG JOIN lồng ghép dữ liệu giao dịch hay các bảng không cần thiết làm ảnh hưởng hiệu năng và gây phức tạp hóa mô hình dữ liệu.
