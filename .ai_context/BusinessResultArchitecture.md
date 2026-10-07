# Kiến Trúc Báo Cáo Kết Quả Hoạt Động Kinh Doanh (KQKD)

Tài liệu mô tả luồng xử lý và kiến trúc tính toán Báo cáo Kết quả hoạt động kinh doanh (B02-DN) trong ứng dụng NewaccNet.

## 1. Bài toán
Báo cáo KQKD cũng gặp các thách thức tương tự như báo cáo Lưu chuyển tiền tệ (LCTT):
*   **Database bị chia cắt theo năm:** ERP thường tách riêng các năm tài chính thành từng Database độc lập. Việc truy xuất cột "Năm trước" hoặc "Kỳ trước" đòi hỏi kỹ thuật Cross-DB, ảnh hưởng tốc độ.
*   **Cấu hình Nợ/Có đối ứng:** Cần truy vết số phát sinh dựa trên cặp tài khoản (Ví dụ: Doanh thu = Nợ các loại tiền/phải thu đối ứng với Có 511).
*   **Dấu cộng / trừ (Tăng / Giảm):** Một số chỉ tiêu làm tăng lợi nhuận (Cộng), một số chỉ tiêu làm giảm lợi nhuận (Trừ).

## 2. Thiết kế Lõi (Core Service)
Nằm tại: `NewaccNet.Reports\BusinessResult\BusinessResultService.cs`

### A. Data Transfer Object (DTO)
`BusinessResultReportDTO` chứa các trường để hiển thị ra XtraReport:
*   `Code`: Mã chỉ tiêu KQKD (Ví dụ: 01, 02, 10...).
*   `ItemName`: Tên chỉ tiêu (Doanh thu, Giá vốn...).
*   `Amount`: Số phát sinh của kỳ hiện tại.
*   `OtherAmount`: Số phát sinh của kỳ trước / lũy kế từ đầu năm (Được nạp thông qua cơ chế Snapshot JSON).

### B. Giải pháp Double-Entry (Bóc tách cặp đối ứng)
Thuật toán phân rã sổ kép ở KQKD đã được tinh chỉnh đơn giản và tối ưu hơn so với LCTT nhờ tính chất của tài khoản kế toán:
1.  **Quét Dữ liệu:** Lấy tất cả `JournalEntry` thuộc kỳ có `ParentId != null` (dòng chi tiết của bút toán).
2.  **Làm phẳng (Flatten):** Chuyển sổ kép thành `AccountPair` (Tài khoản Nợ, Tài khoản Có). Dựa vào thuộc tính `Dbcr` của bút toán (`Dbcr == 1` là Nợ, khác 1 là Có), hệ thống xếp đúng định dạng: `DebitAccount = ..., CreditAccount = ...`.
3.  **Khớp cấu hình:** Cấu hình KQKD (`AccountBusinessResultEntity`) yêu cầu khớp chính xác tài khoản Nợ (vào cột `AccountId`) và tài khoản Có (vào cột `CounterAccountId`). Hàm `.StartsWith()` được sử dụng để hỗ trợ việc lọc theo "Tài khoản mẹ" (Ví dụ `CounterAccountId="331"` sẽ tự động bao gồm `3311`, `3312`).
4.  **Nhân dấu (IsAdd):** Trong cấu hình, nếu cờ `IsAdd == True` (nghĩa là khoản mục có tính chất Giảm Trừ, ví dụ Giá vốn, Chi phí), giá trị được nhân với `-1`. Nếu `IsAdd == False`, giá trị giữ nguyên (`+1`).

## 3. Giải pháp Báo cáo so sánh (Snapshot JSON)
KQKD kế thừa hoàn toàn cơ chế "Decoupled Snapshot" của LCTT:
1.  **Xuất (Export):** Kế toán chọn một khoảng thời gian (VD: Quý 1/2025), bấm nút **"Xuất JSON kỳ này"** để lưu kết quả `{ Code, Amount }` thành file độc lập trên ổ cứng.
2.  **Nạp (Merge):** Khi xem báo cáo Quý 1/2026, kế toán chỉ định file JSON vừa xuất. Hệ thống đọc file, dò theo `Code` và tự động đắp số liệu vào trường `OtherAmount` để so sánh với `Amount` của Quý 1/2026.
*Lợi ích:* Giải quyết vĩnh viễn bài toán cắt năm dữ liệu và tự do so sánh chéo (tháng vs tháng, quý vs quý, năm vs năm, hoặc số phát sinh tháng vs số lũy kế).

## 4. Giao diện (UI)
Nằm tại: `NewaccNet.Wpf\AppSystem\Reports\Fincance\BusinessResultFilterWindow.xaml`
*   Hiển thị Form tùy chọn "Từ ngày / Đến ngày" và ô "File dữ liệu kỳ trước".
*   Hỗ trợ người dùng nhập tiêu đề cột tùy biến (Có thể gõ ký tự `\n` để hiển thị chữ nhiều dòng trên Report XtraReport, Miễn là label trên báo cáo có bật cờ `Multiline=True`).
