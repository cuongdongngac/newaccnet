# TÀI LIỆU THIẾT KẾ & KIẾN TRÚC: MODULE CHUYỂN ĐỔI SỐ DƯ (BALANCE TRANSFER)
*Dự án: NewaccNet (WPF - DevExpress - Multi Database)*

---

## 1. Bối cảnh & Mục tiêu

### 1.1. Hiện trạng & Sự khác biệt so với hệ thống cũ (VB6/Access)
- **Hệ thống cũ**:
  - Chỉ hỗ trợ Access đơn tệp (`.mdb`).
  - Sử dụng các câu lệnh DAO/SQL lồng ghép phức tạp (`legacy_queries -report-vba/TransferBalance/vbacode.txt`) phụ thuộc chặt vào cú pháp riêng của Access Jet Engine.
  - Phân tán, khó kiểm soát lỗi khi chạy hàng loạt.
- **Hệ thống mới (NewaccNet)**:
  - Hỗ trợ đa hệ quản trị CSDL (**Multi-Database**): Microsoft Access (`.accdb`), Microsoft SQL Server, PostgreSQL (`Npgsql`).
  - Hoàn toàn độc lập CSDL nhờ tầng ORM (**LLBLGen Pro**).
  - Tách bạch rõ ràng: CSDL nguồn là kết nối hiện hành (`Source Connection`), CSDL đích là kết nối do người dùng cấu hình (`Destination Connection`).
  - Áp dụng ý tưởng **FLAT (Làm phẳng số dư từ cấp chi tiết)**, loại bỏ hoàn toàn các câu lệnh SQL lồng nhau.

### 1.2. Mục tiêu thiết kế
1. Xây dựng cửa sổ **`TransferBalanceWindow`**:
   - Cấu hình CSDL đích trực quan, kế thừa trải nghiệm tương tự `DbConfigWindow`.
   - Hỗ trợ chọn CSDL đích (Access, SQL Server, PostgreSQL) kèm chức năng **Kiểm tra kết nối** (Test Connection).
   - Loại bỏ các CheckBox phân hệ đơn lẻ: Quy trình chuyển đổi là **hàng loạt (Batch/All-in-one)** để đảm bảo tính cân đối kế toán tổng thể (Trial Balance).
   - Có thanh tiến trình `ProgressBar`, nhãn bước thực thi thời gian thực (`lblCurrentStep`) và khung nhật ký (Log) chi tiết.
2. Thiết kế tầng nghiệp vụ theo kiến trúc dịch vụ (**Service Architecture**) và ý tưởng **FLAT**:
   - Vét cạn toàn bộ các phân hệ tài khoản theo danh mục chuẩn cứng (`AccountTypeMapping`).
   - Mỗi phân hệ chạy độc lập tuần tự (không gộp chung 1 packet) để ghi log rõ ràng và khoanh vùng ngay khi có lỗi.

---

## 2. Danh mục vét cạn các phân hệ số dư (Exhaustive Balance Categories)

Dựa trên cấu trúc chuẩn danh mục tài khoản trong hệ thống (`AccountTypeMapping`) và các bảng chi tiết trong `NewaccNet`:

| STT | Phân hệ / Module | Phân loại Category | Bảng đích (Destination Table) | Bản chất nghiệp vụ kết chuyển |
|---|---|---|---|---|
| 0 | **Thư viện / Danh mục (Master Data)** | Tiền đề | `SystemUser`, `ChartOfAccount`, `Partner`, `Warehouse`, `Category`, `InventoryItem`, `Asset`, `CostObject`, `CostElement`, `Stock`, `Currency` | Sao chép/đồng bộ danh mục trước để chống vi phạm ràng buộc khóa ngoại (FK).<br>- Riêng `admin`: **UPDATE** mật khẩu/thông tin thực tế.<br>- User khác: **INSERT** nếu chưa có. |
| 1 | **Số dư tài khoản thường (No Details)** | Category `I` (Tiền mặt, Tiền gửi, Vốn...) | `JournalEntry` (với `JournalVoucherId = null`) | Tổng hợp số dư lũy kế cuối kỳ đến 31/12:<br>- Net >= 0: Ghi Nợ (`Dbcr = 1`, `Amount = Net`)<br>- Net < 0: Ghi Có (`Dbcr = -1`, `Amount = -Net`) |
| 2 | **Số dư Công nợ (Debt Details)** | Category `A` (TK 131, 331, 141, 138, 338...) | `JournalEntry` + `DebtDetail` | Làm phẳng từ `DebtDetail` truy ngược lên `JournalEntry`. Group theo `AccountId, PartnerId, DebtReasonId, DebtTypeId`. |
| 3 | **Số dư Hàng tồn kho & Vật tư (Material Inventory)** | Category `B` (TK 152, 153, 155, 156, 157...) | `JournalEntry` + `InventoryVoucher` + `InventoryVoucherLine` | **Không ghi dòng số dư thô**, mà sinh một **Phiếu Nhập Kho Nội Bộ (NNB)** ngày 01/01 năm mới:<br>- `Dbcr = 1` (Tồn kho luôn là Nợ)<br>- `Taxflag = false`<br>- Chi tiết: Kho (`WarehouseId`), Mã vật tư (`MaterialId`), Số lượng tồn, Đơn giá tồn, Thành tiền tồn (`Quantity * Price`). |
| 4 | **Số dư Chi phí SXKD dở dang (Cost Details)** | Category `E` (TK 154, 621, 622, 627...) | `JournalEntry` + `CostDetail` | Làm phẳng từ `CostDetail` truy ngược lên `JournalEntry`. Group theo `AccountId, CostObjectId, CostElementId`. |
| 5 | **Số dư Tài sản cố định (Asset Details)** | Category `D` (TK 211, 214...) | `JournalEntry` + `AssetDetail` | Làm phẳng từ `AssetDetail` truy ngược lên `JournalEntry`. Group theo `AccountId, AssetId, Srcode, Reasonid`. Ghi Nợ (`Dbcr = 1`). |
| 6 | **Số dư Cổ phiếu & Đầu tư tài chính (Stock/Investment)** | Category `S` (TK 121, 221, 228...) | `JournalEntry` + `InvestmentDetail` | Làm phẳng từ `InvestmentDetail` truy ngược lên `JournalEntry`. Group theo `AccountId, Stockid, CustomerId`. Ghi Nợ (`Dbcr = 1`). |
| 7 | **Số dư Ngoại tệ (Currency Details)** | Category `C` (Các TK có gốc ngoại tệ) | `JournalEntry` + `CurrencyDetail` | Làm phẳng từ `CurrencyDetail` truy ngược lên `JournalEntry`. Group theo `AccountId, CurrencyId`. **Tính ngược lại tỷ giá bình quân**: $\text{ExchangeRate} = \frac{\text{Tổng VND}}{\text{Tổng ngoại tệ}}$. |

---

## 3. Kiến trúc kỹ thuật & Tổ chức Code (Technical Architecture)

```
NewaccNet.Wpf/
├── AppSystem/
│   ├── Common/
│   │   └── AccountTypeMapping.cs              # Định nghĩa cứng danh mục AccountCategory
│   ├── BalanceTransfer/
│   │   ├── TransferBalanceDtos.cs             # Các DTO làm phẳng số dư (Flat DTOs)
│   │   ├── TransferBalanceContext.cs          # Model chứa tham số kết nối nguồn, đích, niên độ
│   │   ├── IBalanceTransferService.cs         # Interface định nghĩa các hàm vét cạn
│   │   ├── BalanceTransferService.cs          # Service triển khai nghiệp vụ chuyển đổi (ORM LLBLGen)
│   │   ├── TransferBalanceWindow.xaml         # UI Form chuyển đổi số dư
│   │   └── TransferBalanceWindow.xaml.cs      # Code-behind xử lý tương tác UI
```

### 3.1. Ý tưởng làm phẳng (FLAT Architecture)
- Bắt đầu từ bảng chi tiết (Details) của từng phân hệ:
  - `DebtDetail` $\rightarrow$ `JournalEntry`
  - `InventoryVoucherLine` $\rightarrow$ `InventoryVoucher` $\rightarrow$ `JournalEntry`
  - `CostDetail` $\rightarrow$ `JournalEntry`
  - `AssetDetail` $\rightarrow$ `JournalEntry`
  - `InvestmentDetail` $\rightarrow$ `JournalEntry`
  - `CurrencyDetail` $\rightarrow$ `JournalEntry`
- Tải tập dữ liệu kèm quan hệ bằng `PrefetchPath2` của LLBLGen.
- Áp dụng LINQ để group theo các thuộc tính chi tiết, tính lũy kế Net Amount: $\sum (\text{Amount} \times \text{Dbcr})$ hoặc Net Quantity: $\sum (\text{Quantity} \times \text{Dbcr})$.
- Gói kết quả vào các DTO phẳng (`NormalBalanceDto`, `DebtBalanceDto`, `MaterialBalanceDto`, `CostBalanceDto`, `AssetBalanceDto`, `StockBalanceDto`, `CurrencyBalanceDto`).
- Từ các DTO này, sinh các Entity đích tương ứng và lưu vào CSDL đích qua `destAdapter.SaveEntityCollection(...)`.

### 3.2. Quy tắc xử lý danh mục & tài khoản người dùng
- **Đồng bộ Master Data trước**: Bổ sung các bản ghi chưa có ở đích cho `ChartOfAccount`, `Partner`, `Warehouse`, `Category`, `InventoryItem`, `Asset`, `CostObject`, `CostElement`, `Stock`, `Currency`.
- **Tài khoản `SystemUser`**:
  - `admin`: Trong template đích có tài khoản `admin` với mật khẩu mặc định `newacc`. Khi chuyển đổi, tìm thấy `admin` ở đích thì thực hiện **UPDATE** mật khẩu và thông tin thực tế từ nguồn sang để giữ đúng thông tin đăng nhập của người dùng.
  - Các tài khoản khác: Nếu chưa tồn tại ở đích thì thực hiện **INSERT**.

### 3.3. Cơ chế Multi-Adapter (Source vs. Destination)
- **Source Adapter**: `AppDataAccessAdapter.Create()` (kết nối hiện hành).
- **Destination Adapter**: `AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType)`.

---

## 4. Trạng thái kiểm thử & Biên dịch (Build Status)
- Dự án `NewaccNet.Wpf` đã biên dịch thành công hoàn toàn (**0 Errors**).
- Giao diện và dịch vụ đã liên kết chặt chẽ qua cơ chế `IProgress<TransferProgressReport>`.
