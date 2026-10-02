# KIẾN TRÚC VÀ NGUYÊN TẮC THIẾT KẾ BÁO CÁO (REPORTING ARCHITECTURE)

## 1. Công nghệ cốt lõi
- **DevExpress WPF XtraReports**: Bắt buộc sử dụng hệ sinh thái DevExpress cho tất cả các báo cáo.
- **Tệp thiết kế Report**: Sử dụng định dạng `Class Report` của DevExpress (bao gồm `.cs` và `.Designer.cs`) để lưu trữ bản thiết kế. Các tệp này phải được đặt tại dự án giao diện chính (VD: `NewaccNet.Wpf\AppSystem\Reports\...`).

## 2. Nguyên tắc phân tách DTO và UI (Mô hình làm việc)
Triết lý thiết kế báo cáo tuân thủ tuyệt đối việc tách biệt giữa người viết code sinh dữ liệu và người thiết kế giao diện (drag & drop):

- **Project Tính toán (`NewaccNet.Reports`)**:
  - Chỉ chứa Logic tính toán (Calculators), truy xuất cơ sở dữ liệu.
  - Định nghĩa các đối tượng dữ liệu trung gian **DTO** (Data Transfer Objects) đại diện cho từng dòng dữ liệu trên báo cáo (VD: `DiaryReportDTO`).

- **Project Giao diện (`NewaccNet.Wpf`)**:
  - Chứa tệp thiết kế giao diện báo cáo (File `.cs` / `.Designer.cs`).
  - **Kim chỉ nam ("Làm neo")**: Bên trong file `.Designer.cs` của report, BẮT BUỘC phải nhúng sẵn một đối tượng `DevExpress.DataAccess.ObjectBinding.ObjectDataSource` và cấu hình thuộc tính `DataSource` của nó trỏ đích danh tới kiểu dữ liệu DTO ở trên (VD: `typeof(NewaccNet.Reports.DiaryReportDTO)`).
  - Điều này giúp cho đội ngũ thiết kế thuần túy có thể mở Visual Studio Designer, nhìn thấy sẵn danh sách các trường (Field List), từ đó thoải mái kéo thả và bind data vào các controls một cách trực quan nhất mà không cần viết code.

## 3. Quy tắc viết Code (Code Behind)
- Ở lớp `Factory` hoặc Code behind để mở báo cáo, nhiệm vụ của lập trình viên bị **giới hạn ở mức tối giản nhất**:
  1. Gọi Calculator sinh ra một danh sách `List<DTO>`.
  2. Khởi tạo đối tượng Report.
  3. Gán (Pass) danh sách vừa sinh vào `report.DataSource`.
  4. TUYỆT ĐỐI KHÔNG can thiệp, không sinh ra các UI Controls (XRLabel, XRTable...) bằng C# code trừ khi có logic đặc biệt (như Drill-down động). Mọi thứ liên quan đến hiển thị, cộng dồn (Running Sum), màu sắc phải được giải quyết bằng Designer hoặc Report Scripts.

## 4. Tránh xung đột Namespace
- Tuyệt đối KHÔNG đặt tên thư mục hoặc namespace là `System` để tránh lỗi `CS0234` xung đột với không gian tên lõi của .NET. Luôn sử dụng các tên mang tính nghiệp vụ như `AppSystem`, `Core`, `Infrastructure` (VD: `NewaccNet.Wpf.AppSystem`).
