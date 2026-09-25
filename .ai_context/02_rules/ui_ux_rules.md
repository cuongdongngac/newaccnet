# QUY TẮC UI/UX VÀ CƠ CHẾ KẾ THỪA GIAO DIỆN

**QUY TẮC TẠO CONTROL KẾ THỪA:**
Khi lập trình viên ra lệnh ngắn dạng "Tạo màn hình Danh mục Khách hàng kế thừa từ BaseUC/BaseWindow", AI có nghĩa vụ:
1. Tự động tạo file vào thư mục nghiệp vụ tương ứng (không ném vào thư mục Base).
2. Ép lớp mới kế thừa từ `NewaccNet.Wpf.Views.Base.BaseUC` hoặc `BaseWindow`.
3. Tự động override hoặc chuẩn bị sẵn cấu trúc gọi các hàm `LoadData()`, `Save()`, `Print()` theo đúng mẫu quy định.
4. Tích hợp `MainToolBar` vào giao diện XAML mới nếu đó là màn hình nhập liệu hoặc danh mục.