# Kiến Trúc Báo Cáo Lưu Chuyển Tiền Tệ (Cash Flow)

Tài liệu mô tả luồng xử lý và kiến trúc tính toán Báo cáo Lưu chuyển tiền tệ (LCTT - B03-DN) được phát triển trong ứng dụng NewaccNet.

## 1. Bài toán và Khó khăn
*   **Logic Nợ/Có đối ứng phức tạp:** Báo cáo LCTT cần bóc tách chính xác các cặp tài khoản Nợ - Có đối ứng (ví dụ: Thu tiền từ khách hàng Nợ 111 / Có 131).
*   **Giới hạn Database bị chia cắt theo năm:** ERP kế toán Việt Nam thường tách mỗi năm tài chính thành 1 Database riêng. Do đó, việc lấy cột **"Số kỳ trước"** (so sánh với năm trước) bằng query trực tiếp là bất khả thi hoặc tốn kém về hiệu năng (phải kết nối liên Server/DB).
*   **Tham số động:** Cột tiêu đề của "Kỳ này" và "Kỳ trước" có thể thay đổi linh hoạt (VD: Năm nay, Năm trước, Quý 1/2026, Quý 1/2025).

## 2. Thiết kế Lõi (Core Service)
Nằm tại: `NewaccNet.Reports\CashFlow\CashFlowService.cs`

### A. Data Transfer Object (DTO)
`CashFlowReportDTO` được dùng làm cầu nối truyền tải kết quả đã tính toán ra Grid hoặc XtraReport.
Các trường quan trọng:
*   `CategoryId`: ID Nhóm LCTT (Dùng để nhóm và sum các dòng "Lưu chuyển tiền thuần...").
*   `Code`: Mã chỉ tiêu LCTT (Dùng để mapping data JSON).
*   `Amount`: Số liệu truy vấn trực tiếp cho kỳ hiện tại.
*   `PrevAmount`: Số liệu nhồi từ Snapshot JSON của kỳ trước.

### B. Giải pháp Double-Entry (Bóc tách cặp đối ứng)
Do cấu trúc CSDL lưu sổ kép theo dạng Cha - Con (`ParentEntry` và `SubEntries`), thuật toán xử lý như sau:
1.  **Quét Dữ liệu:** Tải toàn bộ `JournalEntry` thuộc kỳ báo cáo mà có `ParentId != null` (Tức là dòng Con - đại diện trọn vẹn cho 1 cặp Nợ/Có).
2.  **Làm phẳng (Flatten):** Chuyển mỗi dòng Con thành 2 `AccountPair` (Góc nhìn từ Con -> Cha và từ Cha -> Con) để dễ dàng rà soát với cấu hình LCTT.
3.  **Khớp cấu hình:** Chạy vòng lặp qua `AccountCashFlowItem`, dùng `StartsWith` để kiểm tra `AccountId` và `CounterAccountId`.
4.  **Nhân dấu:** Sử dụng cấu hình `item.Dbcr` (True = 1, False = -1) để biến đổi Amount thành số dương hoặc âm trước khi cộng dồn.

## 3. Giải pháp Báo cáo so sánh xuyên năm (Snapshot JSON)
Để giải quyết bài toán Database bị cắt năm, hệ thống áp dụng chiến lược **Decoupled Snapshot**:
1.  **Chức năng Xuất (Export):** Ở Form Filter (`CashFlowFilterWindow`), kế toán bấm nút **"Xuất JSON kỳ này"**. Form sẽ gọi hàm Calculate để lấy số liệu kỳ hiện tại, rút trích 2 trường `Code` và `Amount`, sau đó tuần tự hóa ra một file `.json` nhẹ.
2.  **Chức năng Nạp (Merge):** Khi muốn in Báo cáo LCTT có cột so sánh, kế toán sử dụng Form Filter để `Browse` chỉ định file `.json` đã lưu. Service sẽ đọc file này, map theo `Code` và tự động đắp số liệu vào thuộc tính `PrevAmount`.
*Lợi ích: Hiệu năng tuyệt đối do không query DB cũ; linh hoạt so sánh mọi mốc thời gian không theo quy luật (Quý vs Năm, Tháng vs Quý...)*

## 4. UI Layer
Nằm tại: `NewaccNet.Wpf\AppSystem\Reports\Fincance\CashFlowFilterWindow.xaml`
*   Giao diện cho phép chọn `Từ ngày / Đến ngày` và đường dẫn file JSON `Kỳ so sánh`.
*   Tích hợp sẵn nút "Xuất JSON".
*   Gọi trực tiếp `DevExpress.Xpf.Printing.DocumentPreviewWindow` để nạp XtraReport và truyền `DataSource`.
*   Truyền tham số tên cột (`prmCurrentTitle`, `prmPrevTitle`) vào Parameter của Report (Các Parameter này có thể lợi dụng MultiLine trên Report để xuống dòng tự động thay vì xử lý mã code phức tạp ở TextBox).
