QUY TẮC CHUYỂN ĐỔI ACCESS SANG .NET (WPF + LLBLGen):
1. Khi nhận lệnh chuyển đổi một chức năng, AI bắt buộc phải đọc file CSV trong `database_mapping/` để tra cứu tên Bảng và tên Trường mới tương ứng. Không được dùng tên thực thể cũ của Access.
2. Đối với thuật toán tính toán: AI phải vào thư mục `legacy_queries/` hoặc `legacy_vba/` đọc logic tham chiếu gốc, giữ nguyên thuật toán gốc (Pascal standard logic) nhưng phải viết lại bằng C# theo mô hình MVVM.
3. Tầng dữ liệu: Chuyển đổi toàn bộ câu lệnh SQL Access hoặc xử lý Recordset của VBA thành các hàm truy vấn hướng đối tượng bằng LLBLGen Pro ORM (sử dụng Adapter/Linq to LLBLGen).
4. Tầng hiển thị: Tuyệt đối không bê giao diện Form của Access sang. Phải thiết kế mới bằng DevExpress v25 WPF Controls theo luật trong `ui_ux_rules.md`.