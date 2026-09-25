# Cấu trúc Giải pháp NewaccNet
- **Tên giải pháp:** NewaccNet (Mô hình Kế toán Doanh nghiệp)
- **Dự án Client chính:** NewaccNet.Wpf (WPF + DevExpress v25)
- **Cấu trúc thư mục mã nguồn (WPF):**
  - `Views/`: Chứa các cửa sổ, màn hình nhập liệu, báo cáo dạng `.xaml` sử dụng DevExpress v25 WPF Controls.
  - `ViewModels/`: Chứa các lớp logic điều khiển, quản lý trạng thái màn hình kế toán, xử lý lệnh (ICommand).
  - `Models/`: Chứa các model phục vụ tầng hiển thị.
  - `Services/`: Chứa các lớp xử lý tính toán nghiệp vụ kế toán, định khoản, tích hợp tầng LLBLGen Pro ORM.
