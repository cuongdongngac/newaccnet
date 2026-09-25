# Quy tắc C# & Công nghệ (Bắt buộc)
- **Bản chất dự án:** "Accounting-first" (Ưu tiên tuyệt đối tính chính xác dữ liệu, nghiệp vụ tài chính, định khoản, sổ cái trước UI).
- **Công nghệ cốt lõi:** WPF thuần túy trên .NET mới nhất.
- **CẢNH BÁO NGHIÊM NGẶT:** TUYỆT ĐỐI KHÔNG dùng thư viện, sự kiện, control, namespace của WinForms (vd: `System.Windows.Forms`). AI phải kiểm tra kỹ, không nhầm lẫn WinForms và WPF.
- **UI/UX:** Dùng TOÀN BỘ controls của DevExpress v25 cho WPF (GridControl, LayoutControl, DXWindow, SimpleButton...). Nghiêm cấm dùng controls mặc định WPF trừ khi có chỉ định riêng.
- **ORM:** Dùng độc quyền LLBLGen Pro. Tuân thủ đúng cấu trúc Entity, TypedList, và Adapter/SelfServicing.
- **Kiến trúc:** MVVM nghiêm ngặt. Không viết logic nghiệp vụ/dữ liệu vào Code-Behind (`.xaml.cs`). Toàn bộ sự kiện từ DevExpress phải chuyển sang ViewModel qua EventToCommand hoặc Binding.
