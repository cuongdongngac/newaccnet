# QUY TẮC UI/UX VÀ CƠ CHẾ KẾ THỪA GIAO DIỆN

**QUY TẮC TẠO CONTROL KẾ THỪA:**
Khi lập trình viên ra lệnh ngắn dạng "Tạo màn hình Danh mục Khách hàng kế thừa từ BaseUC/BaseWindow", AI có nghĩa vụ:
1. Tự động tạo file vào thư mục nghiệp vụ tương ứng (không ném vào thư mục Base).
2. Ép lớp mới kế thừa từ `NewaccNet.Wpf.Views.Base.BaseUC` hoặc `BaseWindow`.
3. Tự động override hoặc chuẩn bị sẵn cấu trúc gọi các hàm `LoadData()`, `Save()`, `Print()` theo đúng mẫu quy định.
4. Tích hợp `MainToolBar` vào giao diện XAML mới nếu đó là màn hình nhập liệu hoặc danh mục.
**QUY CHUẨN GIAO DIỆN (UI/UX) THEME & GRID:**
1. GLOBAL THEME: 
- Theme mặc định và bắt buộc cho toàn bộ ứng dụng là "Office2019Colorful".
- Bắt buộc thiết lập toàn cục tại App.xaml.cs (OnStartup) bằng lệnh: ApplicationThemeHelper.ApplicationThemeName = "Office2019Colorful";.

2. WINDOW BASE: 
- Tuyệt đối KHÔNG dùng thẻ <Window> mặc định của WPF. 
- Tất cả các form và cửa sổ bắt buộc phải kế thừa từ <dx:ThemedWindow> (XAML) và ThemedWindow (code-behind) để đồng bộ tiêu đề và viền.

3. KHÔNG HARDCODE THEME: 
- Không được phép gán thuộc tính dx:ThemeManager.ThemeName cục bộ trên bất kỳ control hoặc GridControl đơn lẻ nào. Mọi giao diện phải tự động kế thừa theme toàn cục.

4. TIÊU CHUẨN GRID KẾ TOÁN: 
- GridControl trên các màn hình kế toán phải cấu hình để có độ tương phản tốt.
- Không lạm dụng padding quá lớn, tối ưu hóa mật độ hiển thị dòng dữ liệu (Data Density) cho màn hình kế toán chuyên dụng.
