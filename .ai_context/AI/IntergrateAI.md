# AI Support & Reporting Integration Specification

## 1. Ngữ cảnh hệ thống (Context)
- **Dự án:** NEWACC (Phần mềm kế toán C# .NET 9 WPF).
- **Kiến trúc dữ liệu:** Accounting-First, lưu trữ phẳng (flat transaction tables với ParentId).
- **ORM:** LLBLGen Pro (Multi-database).
- **Giao diện:** DevExpress WPF (ThemedWindow, giao diện Office2019Colorful).
- **Mục tiêu:** Tích hợp AI thông qua API tập trung của OpenRouter để xử lý ngôn ngữ tự nhiên (NLP) thành DTO, sau đó dùng OpenXML sinh tệp Excel/Word trên RAM và hiển thị trực tiếp lên DevExpress Spreadsheet/RichEdit Control.

## 2. Yêu cầu cài đặt thư viện (Dependencies)
AI Agent cần kiểm tra và cài đặt các NuGet packages sau vào dự án WPF:
1. `Microsoft.Extensions.AI` và `Microsoft.Extensions.AI.OpenAI` (Xử lý giao tiếp LLM).
2. `DocumentFormat.OpenXml` (Tạo và thao tác tệp Excel/Word).
3. (Đã có sẵn) `DevExpress.Wpf.Spreadsheet` và `DevExpress.Wpf.RichEdit` (Hiển thị luồng bộ nhớ).

## 3. Cấu hình OpenRouter Gateway
- Khởi tạo `HttpClient` trỏ `BaseAddress` tới `https://openrouter.ai/api/v1`.
- Sử dụng thư viện AI chuẩn của Microsoft, tiêm API Key của OpenRouter thông qua cấu hình môi trường hoặc User Secrets.
- Định nghĩa class `AiAgentService` để bọc các hàm gọi LLM. Model mặc định: `google/gemini-2.5-pro` hoặc `anthropic/claude-3.5-sonnet`.

## 4. Bóc tách dữ liệu và Mapping DTO (Function Calling)
- **Mục tiêu:** Biến câu lệnh tự do của người dùng thành đối tượng C#.
- **Yêu cầu AI Agent:** Viết prompt template yêu cầu LLM trả về JSON tuân thủ cấu trúc của `JournalEntryRequestDto` (nhập liệu) hoặc `ReportQueryDto` (truy vấn báo cáo).
- Dữ liệu trả về không ghi thẳng vào DB mà chỉ đưa vào ViewModel để chờ người dùng xác nhận qua UI, sau đó mới gọi LLBLGen Pro UnitOfWork để lưu.

## 5. Sinh tệp OpenXML trên MemoryStream (Zero-Disk I/O)
- Xây dựng class `ReportGeneratorService`.
- **Logic:** Chuyển đổi dữ liệu từ các DTO báo cáo thành cấu trúc bảng biểu Excel/Word bằng `DocumentFormat.OpenXml`.
- **Ràng buộc quan trọng:** TUYỆT ĐỐI KHÔNG dùng `FileStream`. Toàn bộ quá trình tạo Workbook/Document phải thực hiện trên một đối tượng `MemoryStream`.
- Đảm bảo stream được `Seek(0, SeekOrigin.Begin)` sau khi hoàn tất việc tạo cấu trúc OpenXML.

## 6. Hiển thị lên DevExpress UI
- Trong file XAML, tạo một `ThemedWindow` chứa `SpreadsheetControl` (cho Excel) hoặc `RichEditControl` (cho Word).
- Ở Code-behind hoặc ViewModel, nhận `MemoryStream` từ `ReportGeneratorService`.
- Gọi phương thức `spreadsheetControl.LoadDocument(memoryStream, DocumentFormat.Xlsx)` để render báo cáo.
- Cung cấp một nút "Save As" bọc lệnh `SaveFileDialog` để người dùng lưu stream xuống đĩa vật lý nếu cần thiết.