# NGHIỆP VỤ CHỐT SỔ CUỐI KỲ & CHUYỂN SỐ DƯ SANG NĂM MỚI (YEAR-END CLOSING & BALANCE ROLLOVER)

## 1. Khái niệm & Mục tiêu
Cuối năm tài chính (thường là ngày 31/12), kế toán thực hiện hành động **chốt sổ niên độ cũ** và **chuyển số dư sang niên độ mới**:
1. Tạo cơ sở dữ liệu mới cho năm tiếp theo.
2. Tổng hợp dữ liệu cuối kỳ (Aggregate Balances) từ cơ sở dữ liệu năm cũ.
3. Chuyển số dư sang cơ sở dữ liệu mới dưới dạng **Số dư đầu kỳ** (Opening Balances) và **Phiếu nhập kho nội bộ**.

---

## 2. Tạo Cơ Sở Dữ Liệu Mới (New Year Database)
- **Đối với MS Access (.mdb / .accdb)**:
  - Nhân bản (clone/copy) từ một tệp cơ sở dữ liệu rỗng chuẩn (`template.mdb` / rỗng sẵn có).
  - Đổi tên tệp CSDL mới theo niên độ làm việc (ví dụ: `Data2027.mdb`).
- **Đối với SQL Server / PostgreSQL**:
  - Tạo Database rỗng mới (dùng DDL script schema hoặc sao chép từ database mẫu).
  - Khởi tạo kết nối tới Database mới.

---

## 3. Quy Tắc Chuyển Số Dư (Balance Transfer Logic)
Bản chất của việc chuyển số dư hoàn toàn tương tự như nghiệp vụ **Nhập số dư đầu kỳ (`OpeningBalanceWindow`)** cho từng tài khoản kèm chi tiết:

### A. Đối với Tài khoản Kế toán thông thường
- Lấy số dư cuối kỳ (Ending Balance) tại thời điểm 31/12 của năm cũ.
- Quy tắc ghi nhận vào `JournalEntry` (với `JournalVoucherId == null` hoặc `0`):
  - **Nếu số dư Dương (> 0)**: Ghi nhận bên **Nợ** (`Dbcr = 1`, `Amount = Math.Abs(balance)`).
  - **Nếu số dư Âm (< 0)**: Ghi nhận bên **Có** (`Dbcr = -1`, `Amount = Math.Abs(balance)`).
- Kèm theo thông tin chi tiết (nếu tài khoản có theo dõi chi tiết):
  - Chi tiết công nợ (`DebtDetail`): Mã đối tượng công nợ, số dư nợ/có chi tiết.
  - Chi tiết chi phí (`CostDetail`): Khoản mục chi phí.
  - Chi tiết tài sản (`AssetDetail`): Danh mục tài sản, nguyên giá, hao mòn lũy kế.

### B. Đối với Hàng hóa & Vật tư (Tài khoản kho 152, 153, 155, 156...)
- Không chuyển thành dòng số dư thô, mà sinh một **Phiếu Nhập Kho Nội Bộ (NNB)** ngày 01/01 của năm mới:
  - Loại chứng từ: `Taxflag = false` (Nhập nội bộ).
  - Từng dòng chi tiết `InventoryVoucherLine`:
    - Kho hàng (`WarehouseId`).
    - Mã vật tư (`MaterialId`).
    - Số lượng tồn cuối kỳ (`Quantity`).
    - Đơn giá tồn cuối kỳ (`Price`).
    - Thành tiền tồn (`Amount = Quantity * Price`).
  - Định khoản tự động vào tài khoản kho tương ứng.

---

## 4. Phân Tách Giai Đoạn Nghiệp Vụ (Decoupled Workflow)
Quá trình chốt năm được tách bạch rõ thành 2 giai đoạn độc lập:
1. **Giai đoạn 1 - Xử lý cuối kỳ trong năm cũ (Module Kết chuyển & Bút toán cuối năm)**:
   - Kết chuyển doanh thu, chi phí, xác định kết quả kinh doanh (TK 911).
   - Kết chuyển Lợi nhuận sau thuế chưa phân phối (TK 4212 $\rightarrow$ 4211).
   - Các thao tác này sẽ do module riêng xử lý và sinh ra các chứng từ kế toán thông thường trong năm cũ.
   - Khi giai đoạn này hoàn tất, số dư cuối kỳ trên Bảng cân đối tài khoản của năm cũ đã hoàn toàn chuẩn xác.

2. **Giai đoạn 2 - Chốt sổ & Chuyển số dư sang Database mới**:
   - Khi số liệu năm cũ đã khóa chuẩn, việc chuyển số dư sang DB mới chỉ là hành động tổng hợp (Aggregate) số dư và ghi nhận thành Số dư đầu kỳ (`OpeningBalance`) cùng Phiếu nhập kho nội bộ (`NNB`) cho năm mới.

---

## 5. Định Hướng Khảo Sát Thêm Cho Các Bảng Chi Tiết (To-Do & Research)
Trước khi lập trình module chuyển số dư chi tiết, cần rà soát kỹ cấu trúc dữ liệu mở rộng:
- **Công nợ (`DebtDetail`)**: Nhóm theo đối tượng (`PartnerId`), loại công nợ (phải thu/phải trả/tạm ứng), hạn thanh toán, hợp đồng (nếu có).
- **Chi phí (`CostDetail`)**: Nhóm theo đối tượng tập hợp chi phí, yếu tố chi phí, vụ việc/công trình.
- **Tài sản (`AssetDetail`)**: Nhóm theo mã tài sản, nguồn hình thành tài sản (nguồn vốn kinh doanh, quỹ phát triển...), bộ phận sử dụng.
- **Vật tư (`InventoryVoucherLine`)**: Nhóm theo kho và mã vật tư.

