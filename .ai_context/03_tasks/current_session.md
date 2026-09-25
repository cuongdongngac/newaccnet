# Cơ chế quản lý Token
Quy trình giảm Token: Cuối mỗi phiên làm việc, Lập trình viên sẽ ra lệnh cho AI tự tổng hợp tiến độ và ghi đè vào file này. Đầu phiên làm việc tiếp theo, AI có nghĩa vụ đọc file này trước tiên để lấy bối cảnh, tuyệt đối không quét lại toàn bộ mã nguồn nếu không được yêu cầu, nhằm tiết kiệm chi phí.

---

# Tiến độ dự án (Cập nhật mới nhất: 25/09/2026 16:11)

## 1. Các công việc đã hoàn thành:
- **Khởi tạo kiến trúc dự án:** Tạo khung dự án `NewaccNet.Wpf` sử dụng DevExpress v25.
- **Thiết lập Base Views:** Xây dựng hệ thống form/control cơ sở (`BaseWindow`, `BaseUC`, `BaseEntryUC`, `BaseReportUC`) và `MainToolBar` hỗ trợ chuẩn MVVM.
  - **LƯU Ý:** `BaseUC` đã được chuyển thành class C# thuần (`BaseUC.cs`, không có `.xaml`) vì WPF không cho phép kế thừa XAML giữa các UserControl. `BaseWindow` kế thừa từ `ThemedWindow`.
- **Cấu hình AI Context:** Lập bộ luật chặt chẽ phục vụ chuyển đổi hệ thống (`csharp_rules.md`, `ui_ux_rules.md`, `migration_rules.md`, `system_design.md`).
- **Tích hợp Tầng Data (ORM):** Đã nạp thành công các dự án do LLBLGen sinh ra trong thư mục `dataaccess/` (Generic, SQL Server, MS Access, PostgreSQL) làm tham chiếu cho dự án startup `NewaccNet.Wpf`. Đã sẵn sàng truy vấn dữ liệu từ C#.

## 2. QUY TẮC NGHIÊM NGẶT ĐỐI VỚI THƯ MỤC `dataaccess/`:
- **Vai trò:** Thư mục `dataaccess/` chứa toàn bộ Entities, TypedLists, và DataAccessAdapter sinh ra bởi LLBLGen Pro ORM. Đây là tầng trung gian duy nhất để dự án giao tiếp với cơ sở dữ liệu.
- **CẢNH BÁO TỚI AI:** Toàn bộ code trong `dataaccess/` là AUTO-GENERATED. AI **TUYỆT ĐỐI KHÔNG ĐƯỢC TỰ Ý SỬA CHỮA, THÊM BỚT HAY XÓA BỎ** bất kỳ dòng code nào trong thư mục này. Khi nghiệp vụ thay đổi, code sẽ được sinh lại bằng tool LLBLGen. AI chỉ có nhiệm vụ **ĐỌC** và **GỌI** các class trong thư mục này để thực hiện truy vấn ở tầng `Services/` hoặc `ViewModels/`.

## 3. Module Hệ thống (System Module):
- **Cấu trúc thư mục:** Đã tạo thư mục `NewaccNet.Wpf/System` chuyên chứa các màn hình và logic cấu hình hệ thống, tách biệt hoàn toàn khỏi nghiệp vụ kế toán.
- **Namespace:** Các file trong thư mục `System` dùng namespace `NewaccNet.Wpf.AppSystem` (tránh xung đột với `System` của .NET).
- **Danh sách file trong `System/`:**
  - `AppConfig.cs` – Enum `DatabaseType` + class `AppConfig` chứa các thuộc tính cấu hình cho 3 loại DB.
  - `ConfigManager.cs` – Đọc/ghi file `appsettings.json`, build ConnectionString.
  - `AppDataAccessAdapter.cs` – Factory tạo `IDataAccessAdapter` (LLBLGen) đúng loại DB đang cấu hình.
  - `DbConfigWindow.xaml/.xaml.cs` – Form cấu hình kết nối CSDL (Access/SQL Server/PostgreSQL), có live preview ConnectionString, tự động restart app sau khi lưu.
  - `LoginWindow.xaml/.xaml.cs` – Form đăng nhập, truy vấn bảng `SystemUser` qua LinqMetaData.

## 4. Luồng khởi động ứng dụng (App Startup Flow):
- **`App.xaml`:** Đã xóa bỏ `StartupUri="MainWindow.xaml"`.
- **`App.xaml.cs` (OnStartup):**
  1. Thiết lập Theme toàn cục: `ApplicationThemeHelper.ApplicationThemeName = "Office2019Colorful"`.
  2. Gọi `ConfigManager.LoadConfig()`.
  3. Nếu **CHƯA có** `appsettings.json` → Mở `DbConfigWindow` (gợi ý mặc định Access + template.accdb) → Sau khi Lưu, app tự restart.
  4. Nếu **ĐÃ có** `appsettings.json` → Mở `LoginWindow` → Đăng nhập thành công → Mở `MainWindow`.
  5. Nếu người dùng đóng form Login → `Application.Current.Shutdown()`.

## 5. Chuẩn hóa Theme & Giao diện (UI Standardization):
- **Theme toàn cục:** `Office2019Colorful` – thiết lập 1 lần duy nhất tại `App.xaml.cs`.
- **Window base:** Tất cả Window/Form đều dùng `dx:ThemedWindow` (XAML) và kế thừa `ThemedWindow` (C#). Đã rà soát và chuẩn hóa:
  - `MainWindow.xaml/.xaml.cs` – ✅ `dx:ThemedWindow` / `ThemedWindow`
  - `DbConfigWindow.xaml/.xaml.cs` – ✅ `dx:ThemedWindow` / `ThemedWindow`
  - `LoginWindow.xaml/.xaml.cs` – ✅ `dx:ThemedWindow` / `ThemedWindow`
  - `BaseWindow.xaml/.xaml.cs` – ✅ `dx:ThemedWindow` / `ThemedWindow` (đã sửa từ `DXWindow`)
- **Không hardcode theme:** Đã quét toàn bộ XAML, không có `ThemeManager.ThemeName` cục bộ nào.
- **Quy chuẩn đã được ghi vào** `02_rules/ui_ux_rules.md` để AI tự động áp dụng cho mọi phiên làm việc sau.

## 6. Cấu hình dự án (.csproj) – Các thay đổi quan trọng:
- `ImplicitUsings` = `disable` (tránh xung đột WinForms vs WPF).
- Đã thêm `PackageReference`: `DevExpress.Wpf.Core`, `DevExpress.Wpf.LayoutControl`, `System.Data.SqlClient`, `Npgsql`.
- Đã thêm `ProjectReference`: PostgreSQL DatabaseSpecific (`DataAccess.PostgreSqlDBSpecific.csproj`).

## 7. Trạng thái Build hiện tại:
- **Build succeeded** – 0 Error(s), 10 Warning(s) (chủ yếu là cảnh báo obsolete SqlClientFactory và nullable).
