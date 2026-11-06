# Hướng Dẫn Kiến Trúc & Vận Hành: Công Cụ Chuyển Đổi & Nhân Bản CSDL (DbMigrator Plugin)

## 1. Giới thiệu & Triết lý Kiến trúc
Trong hệ sinh thái **NewaccNet**, việc nâng cấp cơ sở dữ liệu từ môi trường cá nhân (MS Access / SQLite) lên môi trường CSDL quan hệ máy chủ (MS SQL Server / PostgreSQL) được thiết kế theo nguyên tắc:

* **Clean Core:** Không sửa đổi, can thiệp hay phụ thuộc vào mã nguồn cũ của Core và công cụ `migrate.exe`. Toàn bộ chức năng được đóng gói độc lập trong Plugin **`NewaccNet.Plugin.DbMigrator`**.
* **Đồng cấu dữ liệu (Isomorphic Replication):** CSDL nguồn và CSDL đích đều sử dụng chung một chuẩn mô hình Entity (`DatabaseGeneric` của LLBLGen Pro v5.12). Do đó, bài toán không phải là chuyển đổi định dạng (mapping cột) mà là **nhân bản dữ liệu nguyên vẹn (Replication) giữa hai Adapter**.
* **Tách bạch 2 khâu độc lập:**
  1. **Khâu 1 - Dựng CSDL & Bảng (DDL Provisioning):** Đọc từ tệp DDL SQL chuẩn được xuất từ LLBLGen Pro, tự động làm sạch và dựng toàn bộ cấu trúc bảng, khóa chính, khóa ngoại.
  2. **Khâu 2 - Di chuyển dữ liệu (Data Replication):** Đọc dữ liệu từ CSDL nguồn đang kết nối và bơm sang CSDL đích bằng `SqlBulkCopy`.

---

## 2. Quy Tắc Bất Biến: Schema `Newacc`
Toàn bộ Entity của hệ thống NewaccNet được cấu hình ánh xạ cố định vào Schema **`Newacc`** (trong cả `DataAccess.SqlServerDBSpecific` và `DataAccess.PostgreSqlDBSpecific`):

```csharp
this.AddElementMapping("ChartOfAccountEntity", @"DataAccess", @"Newacc", "ChartOfAccount", ...);
this.AddElementMapping("JournalEntryEntity", @"DataAccess", @"Newacc", "JournalEntry", ...);
```

> [!IMPORTANT]
> **Quy tắc bắt buộc:**
> - Khi khởi tạo hoặc nhập (import) thủ công CSDL bằng SSMS hay pgAdmin, **tuyệt đối không được thay đổi schema** sang `dbo` hoặc `public`.
> - Toàn bộ bảng phải nằm trong schema `[Newacc]` để ORM LLBLGen nhận diện và truy vấn chính xác.

---

## 3. Metadata Thứ Tự Chuyển Đổi (`mapping/replication_order.csv`)
Tệp [replication_order.csv](file:///d:/NewaccNet/mapping/replication_order.csv) là tài liệu chuẩn do Core quản lý, quy định thứ tự chuyển đổi của toàn bộ 82 bảng trong hệ thống để đảm bảo quan hệ phụ thuộc (Foreign Key Dependency):

* **Level 0 (step 1 -> 31):** Danh mục gốc, độc lập hoàn toàn (`ChartOfAccount`, `Warehouse`, `Currency`, `Partner`, `TaxRate`...).
* **Level 1 (step 32 -> 45):** Các bảng phụ thuộc Level 0 (`InventoryItem`, `CounterAccounts`...).
* **Level 2 (step 46 -> 57):** Các bảng chứng từ và phân tích (`JournalEntry`, `InventoryVoucher`, `VatEntry`, `DebtDetail`...).
* **Level 3 -> 5 (step 58 -> 82):** Chi tiết dòng chứng từ sâu hơn (`InventoryVoucherLine`, `ExpenseTaxLine`...).

### Cờ bảo toàn khóa tự tăng `has_autonumber_pk`:
Đối với các bảng có `has_autonumber_pk = true` (ví dụ `JournalVoucher`, `JournalEntry`, `InventoryVoucher`...):
- Khi nạp dữ liệu: Bật cờ `SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls` để giữ nguyên toàn bộ ID cũ.
- Sau khi nạp: Chạy lệnh `DBCC CHECKIDENT ('[Newacc].[TableName]', RESEED)` để SQL Server cập nhật lại giá trị seed lớn nhất tiếp theo.

---

## 4. Cơ Chế Xử Lý & Làm Sạch Tệp DDL SQL
Tệp DDL xuất từ LLBLGen Pro (nằm trong `Template\SQL Server (SqlClient)\`) chứa một số lệnh mặc định cần được tiền xử lý trước khi thực thi:

1. **Xóa comment đa dòng `/* ... */`:** Loại bỏ các comment chứa từ khóa `GO` kẹt bên trong để tránh lỗi tách lệnh `Missing end comment mark '*/'`.
2. **Xóa triệt để các lệnh `USE ...`:** Ngăn chặn việc script tự ý chuyển database về `master` hoặc `DataAccess`. Toàn bộ lệnh được thực thi trực tiếp trên connection của CSDL đích.
3. **Xóa triệt để các lệnh `CREATE DATABASE ...`:** Vì CSDL đã được kiểm tra và tạo mới từ trước theo đúng tên người dùng nhập.
4. **Tách batch theo `GO`:** Tách các khối lệnh và gửi lần lượt qua `SqlCommand`.

---

## 5. Cơ Chế Ánh Xạ Cột Thông Minh (Case-Insensitive Mapping)
Để xử lý sự sai khác về cách viết hoa/thường giữa Access và SQL Server (ví dụ: `DBCR` trong Access vs `Dbcr` trong SQL Server):
- Code tự động truy vấn danh sách cột thực tế của bảng đích trên SQL Server thông qua `INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Newacc'`.
- So khớp tên cột nguồn và đích bằng `StringComparer.OrdinalIgnoreCase`.
- Bỏ qua các cột thừa nếu có giữa các phiên bản dữ liệu.

---

## 6. Hướng Dẫn Vận Hành Trên Giao Diện

### Vị trí mở chức năng:
Vào Tab **`Hệ thống`** trên thanh Ribbon $\rightarrow$ Nhóm **`[Cơ sở dữ liệu]`** $\rightarrow$ Bấm nút **`[Chuyển đổi CSDL]`**.

### Các bước thực hiện:
1. **Kiểm tra CSDL Nguồn:** Mục 1 tự động hiển thị thông tin CSDL đang kết nối hiện tại.
2. **Cấu hình CSDL Đích (MS SQL Server):**
   - Nhập máy chủ (`TargetServer`, ví dụ `.\SQLEXPRESS` hoặc `localhost`).
   - Chọn phương thức xác thực (*Windows Auth* hoặc *SQL Auth*).
   - Nhập tên CSDL muốn tạo (ví dụ `NewaccNet_2026`).
   - Bấm nút **[Kiểm tra kết nối]** để xác nhận thông máy chủ.
3. **Tùy chọn thực thi:**
   - **Tùy chọn A - Tạo Database:** Bấm nút **`[1. Tạo Database từ SQL]`** để tạo CSDL trắng có đầy đủ 74 bảng trong schema `Newacc`.
   - **Tùy chọn B - Chỉ sao chép dữ liệu:** Bấm nút **`[2. Chỉ sao chép dữ liệu]`** (sau khi CSDL đã được tạo).
   - **Tùy chọn C - Trọn gói A-Z:** Bấm nút **`[🚀 Tạo CSDL & Chuyển dữ liệu trọn gói (1 + 2)]`** để hệ thống tự động hoàn tất cả hai khâu.
4. **Theo dõi tiến trình:** Xem nhật ký thực hiện chi tiết trong khung Log và thanh tiến trình thời gian thực.
