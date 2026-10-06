# Kiến trúc tính giá vốn Hàng tồn kho (Inventory Valuation)

Tài liệu này mô tả chi tiết kiến trúc, thuật toán và cách tổ chức các luồng tính giá xuất kho / tồn kho trong hệ thống ERP. Đích đến cuối cùng của mọi phương pháp tính đều là cập nhật bảng trạng thái `MaterialPrice` (lưu trữ Tồn lượng và Đơn giá hiện tại của từng Vật tư tại từng Kho).

Hệ thống hỗ trợ 2 phương pháp tính giá vốn chính:
1. Bình quân gia quyền liên hoàn (Moving Weighted Average).
2. Nhập trước Xuất trước (FIFO).

Hệ thống được thiết kế theo 2 Giai đoạn / Thực thể xử lý độc lập để đảm bảo hiệu năng và tính linh hoạt cho nghiệp vụ kế toán.

## 1. Giai đoạn 1: Chốt sổ / Kiểm kê toàn cục (Batch Recalculation)
- **Mục đích:** Khởi tạo bảng giá từ con số 0 khi mới cài phần mềm, hoặc tính toán lại đồng loạt vào cuối kỳ để sửa sai/chốt sổ khi có thay đổi dữ liệu trong quá khứ.
- **Thực thể xử lý:** Class `InventoryValuationService`.
- **Cách hoạt động:**
  - Cung cấp 2 chế độ:
    - **Rebuild from scratch (Kiểm kê từ đầu):** Xóa trắng và quét toàn bộ lịch sử chứng từ từ trước tới nay.
    - **Forward calculation (Tính tiếp từ thời điểm):** Lấy dữ liệu bảng `MaterialPrice` hiện hành làm Tồn đầu kỳ (nhằm bảo toàn các khoản điều chỉnh giá thủ công hoặc chốt sổ trước đó), và chỉ tính tiếp các chứng từ phát sinh sau mốc thời gian chỉ định.
  - Dữ liệu được sắp xếp nghiêm ngặt theo thời gian: `VoucherDate` -> `Nhập trước, Xuất sau` -> `VoucherId` -> `LineId`.
  - Chạy thuật toán vòng lặp mô phỏng lại quá trình nhập/xuất để chốt ra Số lượng và Giá tồn cuối cùng.
  - Ghi đè kết quả tính toán vào `MaterialPrice`.

## 2. Giai đoạn 2: Tính liên hoàn (Event-driven / Continuous Valuation)
- **Mục đích:** Tính tức thời ngay khi nhân viên lập 1 phiếu Nhập/Xuất kho mới trong ngày. Đảm bảo kho luôn có giá trị Tức thời mà không làm đơ hệ thống, không phải chạy lại toàn cục.
- **Thực thể xử lý:** Class `MaterialPriceStore` (In-memory document store).
- **Cách hoạt động theo từng phương pháp:**
  - **Bình quân gia quyền (`ApplyMovingAverageReceipt`, `ApplyMovingAverageIssue`):** 
    - Tính toán thuần túy trên RAM (Scalar math).
    - Tính theo công thức: Giá mới = Tổng Giá Trị (Tồn cũ + Nhập mới) / Tổng Số Lượng.
    - Chạy đồng bộ (Synchronous) cùng chung Transaction lưu phiếu, không cần query lại DB.
  - **Nhập trước Xuất trước - FIFO (`ApplyFifoBackground`):**
    - Chạy ngầm (Background/Async) ngay sau khi phiếu được lưu thành công.
    - Thuật toán sẽ Query lại lịch sử chứng từ của **riêng 1 mã Vật tư và 1 Kho đó**.
    - Sử dụng `InventoryCostCalculator.CalculateFifo` để bóc tách các lớp (layers) nhập chưa xuất hết, tính ra giá trị Tồn/Giá FIFO mới nhất.
    - Cập nhật ngầm vào bảng `MaterialPrice`. Tốc độ cực nhanh vì chỉ giới hạn ở phạm vi 1 mặt hàng.

## 3. Quản lý điều chỉnh tay (Manual Adjustments)
- Bảng Grid hiển thị `MaterialPrice` hỗ trợ **Inline-Editing** (Sửa trực tiếp trên ô).
- Kế toán trưởng có quyền can thiệp, gõ sửa trực tiếp Đơn giá hoặc Tồn lượng để xử lý các nghiệp vụ đánh giá lại tài sản, kiểm kê thực tế hoặc rớt giá đột biến.
- Các thao tác gõ tay này được hệ thống **Auto-Save** ngầm ngay khi nhảy dòng. Dữ liệu tay này lập tức trở thành mốc Tồn/Giá tham chiếu chuẩn xác cho "Giai đoạn 1 (Forward calculation)" hoặc "Giai đoạn 2" chạy tiếp.
