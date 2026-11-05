# KIẾN TRÚC TÍNH TOÁN & SCHEMA MỞ RỘNG (CUSTOM FIELDS & DIMENSIONS)
*Tài liệu Đặc tả Kỹ thuật Hệ thống NewaccNet - Phiên bản Kiến trúc .NET 10*

---

## MỤC LỤC
1. [Khả năng Hợp nhất (Merge) Tài liệu Tổng](#1-khả-năng-hợp-nhất-tài-liệu-tổng)
2. [Chiến lược Kiến trúc Dịch vụ Tính toán (Calculation Architecture)](#2-chiến-lược-kiến-trúc-dịch-vụ-tính-toán)
   - 2.1. Triết lý hiện tại (ORM-first & DB-Agnostic)
   - 2.2. Chiến lược bẻ lái sang Stored Procedure cho Doanh nghiệp lớn
   - 2.3. Nguyên tắc "Giữ nguyên vỏ hàm, chỉ tráo lõi tính toán"
   - 2.4. Cơ chế Dispatcher & Safe Fallback theo DbConfig
3. [Đặc tả Schema Mở rộng: Custom Fields & Dimensions](#3-đặc-tả-schema-mở-rộng-custom-fields--dimensions)
   - 3.1. Mô hình lưu trữ lai (Hybrid Storage: Slots + ExtData JSON)
   - 3.2. Cột dự phòng (Slots) trên 4 Bảng Cốt lõi
   - 3.3. Danh mục 4 Bảng Định nghĩa Động
4. [Bảng Lưu Dữ liệu Tính toán Tạm thời (CalculationSession)](#4-bảng-lưu-dữ-liệu-tính-toán-tạm-thời-calculationsession)
   - 4.1. Bối cảnh loại bỏ các bảng tạm di sản
   - 4.2. Cấu trúc bảng CalculationSession
5. [Tóm tắt Cấu trúc Thư mục Tài liệu Kỹ thuật](#5-tóm-tắt-cấu-trúc-thư-mục-tài-liệu-kỹ-thuật)

---

## 1. KHẢ NĂNG HỢP NHẤT TÀI LIỆU TỔNG
Toàn bộ các tài liệu kỹ thuật trong thư mục `docs/` được định dạng chuẩn **GitHub Flavored Markdown (GFM)** với hệ thống tiêu đề (H1 đến H4) nhất quán:
* `docs/HUONG_DAN_SUA_TEMPLATE_ACCDB.md`: Chi tiết sửa đổi file Database Template.
* `docs/MATERIAL_TRANSACTION_ARCHITECTURE.md`: Kiến trúc xử lý luồng kho và giá vốn.
* `docs/AI_AGENT_ROADMAP.md`: Lộ trình tích hợp AI kế toán.
* `docs/KIEN_TRUC_TINH_TOAN_VA_SCHEMA_MO_RONG.md`: Tài liệu này.

> [!TIP]
> **Quy trình Merge:** Khi cần xuất **Tài liệu Tổng thể (Master Technical Specification)**, chỉ cần sử dụng công cụ gom Markdown (như `Pandoc` hoặc script nối file theo thứ tự mục lục) để xuất ra một file duy nhất dạng PDF, Word (.docx) hoặc HTML mà không bị xung đột định dạng.

---

## 2. CHIẾN LƯỢC KIẾN TRÚC DỊCH VỤ TÍNH TOÁN

### 2.1. Triết lý hiện tại (ORM-first & DB-Agnostic)
* **Quy chuẩn:** NewaccNet hoạt động độc lập với hệ quản trị CSDL (hỗ trợ SQL Server, PostgreSQL, MS Access).
* **Xử lý:** Mọi phép tính nghiệp vụ (khấu hao, phân bổ, tính giá vốn, kết chuyển số dư) được viết bằng C# OOP thuần trên RAM thông qua LLBLGen ORM.
* **Tỷ trọng:** Phục vụ hoàn hảo 90% - 95% khách hàng vừa và nhỏ, dữ liệu vài chục vạn dòng chứng từ mà không cần cấu hình phức tạp trên DB Server.

### 2.2. Chiến lược bẻ lái sang Stored Procedure cho Doanh nghiệp lớn
Khi gặp khách hàng doanh nghiệp quy mô lớn (dữ liệu hàng triệu bản ghi):
* **Không làm "All-in-One":** Không cố nhồi nhét mọi logic Procedure vào bản cài chuẩn để tránh làm cồng kềnh hệ thống phổ thông. Khi đó sẽ phân nhánh giải pháp chuyên biệt (Enterprise Edition).
* **Tận dụng lại 90% Codebase:** Cấu trúc phân tầng hiện tại (`NewaccNet.Contracts`, `NewaccNet.Core`, `DataAccess`, `NewaccNet.Wpf`) cho phép tái sử dụng toàn bộ UI, ViewModel và Model.

### 2.3. Nguyên tắc "Giữ nguyên vỏ hàm, chỉ tráo lõi tính toán"
Các khâu nghiệp vụ kiểm tra xung quanh giữ nguyên 100%:
1. Kiểm tra ngày khóa sổ kế toán (`LockDate`).
2. Kiểm tra quyền hạn người dùng (`Permission`).
3. Validate tham số đầu vào (`ValuationParams / CostAllocationParams`).
4. Ghi nhật ký hệ thống (`AuditLog`).

**Chỉ tráo đổi duy nhất bước thực thi lõi:**
```csharp
public async Task<CalculationResult> CalculateAsync(CalculationParams args)
{
    // Vỏ hàm: Validate & kiểm tra khóa sổ dùng chung 100%
    ValidatePeriodAndLockDate(args);

    // Lõi: Bẻ lái dựa theo DbConfig và khả năng của CSDL
    if (_configService.CurrentConfig.UseStoredProcedure && _adapter.SupportsStoredProcedure)
    {
        await ExecuteViaProcedureAsync(args); // Lõi SP (Database Server xử lý)
    }
    else
    {
        await ExecuteViaOrmAsync(args);       // Lõi ORM thuần (RAM/LINQ)
    }

    AuditLogHelper.Log("Hoàn tất tính toán");
    return new CalculationResult { Success = true };
}
```

### 2.4. Cơ chế Dispatcher & Safe Fallback theo DbConfig
* **Cấu hình:** Bổ sung trường `CalculationEngine` trong `DbConfigDto` (0 = ORM, 1 = Stored Procedure).
* **Safe Fallback:** Nếu CSDL là **MS Access** (không có SP) hoặc Database khách hàng chưa cài đặt kịp Script Procedure, hệ thống tự động rơi về (Fallback) chạy ORM thuần mà không phát sinh lỗi Runtime.

---

## 3. ĐẶC TẢ SCHEMA MỞ RỘNG: CUSTOM FIELDS & DIMENSIONS

Nhằm đáp ứng yêu cầu linh hoạt của các doanh nghiệp mà không cần thay đổi cấu trúc bảng mỗi khi có yêu cầu thêm trường, NewaccNet sử dụng mô hình lưu trữ lai **(Hybrid Storage Architecture)**.

### 3.1. Mô hình lưu trữ lai (Slots + ExtData JSON)
* **Dạng Slot (`Text1..5`, `Num1..3`, `Date1..2`, `Flag1..2`):** Dành cho các trường cần **lọc nhanh, tìm kiếm, đánh chỉ mục (Index) hoặc lên báo cáo**.
* **Dạng `ExtData` (Long Text / JSON):** Dành cho dữ liệu thuộc tính động không giới hạn, lưu cấu trúc tự do không cần index.

### 3.2. Cột dự phòng (Slots) trên 4 Bảng Cốt lõi

| Bảng Cốt lõi | Kiểu thực thể | Các cột Slots được bổ sung | Chi tiết |
| :--- | :--- | :--- | :--- |
| **`Partner`** | Khách hàng / NCC | `Text1..5`, `Num1..3`, `Date1..2`, `Flag1..2`, `ExtData` | Lưu thêm: Hạn mức nợ riêng, Mã vùng, Nhóm bán hàng, Ngày ký HĐ... |
| **`InventoryItem`** | Vật tư / Hàng hóa | `Text1..5`, `Num1..3`, `Date1..2`, `Flag1..2`, `ExtData` | Lưu thêm: Quy cách đặc thù, Tiêu chuẩn kỹ thuật, Xuất xứ... |
| **`JournalVoucher`** | Chứng từ (Header) | `Text1..5`, `Num1..3`, `Date1..2`, `Flag1..2`, `ExtData`, `RowGuid`, `VoucherSourceCode` | Lưu thêm: Số hợp đồng, Người phê duyệt, Ký hiệu nguồn chứng từ... |
| **`JournalEntry`** | Dòng hạch toán (Detail) | `Text1..3`, `Num1..2`, `Date1`, `ExtData`, **`Dim1..Dim5`** | `Dim1..5` phục vụ **5 chiều phân tích tài chính đa năng** |

### 3.3. Danh mục 4 Bảng Định nghĩa Động

#### 1. Bảng `CustomFieldDefinition` (Định nghĩa trường mở rộng)
Quản lý việc ánh xạ một trường logic (ví dụ: *Số hợp đồng kinh tế*) vào Slot vật lý nào:
* `Id` (AutoNumber / PK)
* `EntityName` (Short Text 50): Tên thực thể (`Partner`, `InventoryItem`, `JournalVoucher`, `JournalEntry`).
* `FieldKey` (Short Text 50): Khóa định danh code (vd: `contract_no`).
* `Label` (Short Text 100): Nhãn hiển thị trên giao diện (vd: `Số hợp đồng`).
* `DataType` (Short Text 20): `Text`, `Number`, `Date`, `Bool`, `List`.
* `StorageKind` (Short Text 10): `Slot` (dùng cột dự phòng) hoặc `Json` (lưu trong `ExtData`).
* `SlotName` (Short Text 10): Tên cột vật lý tương ứng (`Text1`, `Num1`...).
* `IsRequired`, `IsFilterable`, `MaxLength`, `DefaultValue`, `SortOrder`, `IsActive`.

#### 2. Bảng `CustomFieldOption` (Tùy chọn danh sách cho kiểu List)
Phục vụ khi trường mở rộng có kiểu là Dropdown / ComboBox:
* `FieldId` (FK trỏ tới `CustomFieldDefinition.Id`)
* `Code` (Short Text 50): Mã giá trị.
* `Label` (Short Text 100): Tên hiển thị.
* `SortOrder`, `IsActive`.

#### 3. Bảng `Dimension` (Danh mục 5 chiều phân tích đa năng)
Quản lý các trục phân tích chi phí/doanh thu (gán vào `Dim1..Dim5` trên `JournalEntry`):
* `Code` (PK, Short Text 50): Mã chiều phân tích (vd: `DUAN`, `HOPDONG`, `PHONGBAN`, `KENHBANHANG`).
* `Name` (Short Text 100): Tên chiều phân tích.
* `SlotIndex` (Integer): Vị trí slot từ 1 đến 5 (trỏ tương ứng vào cột `Dim1` đến `Dim5`).
* `IsActive` (Yes/No).

#### 4. Bảng `DimensionValue` (Chi tiết các đối tượng của từng chiều)
Chứa danh sách giá trị thực tế của từng chiều phân tích:
* `DimCode` (FK trỏ tới `Dimension.Code`, ví dụ: `DUAN`).
* `Code` (Short Text 50, ví dụ: `DA_CAUTU_LIEM`).
* `Name` (Short Text 150, ví dụ: `Dự án Cầu Từ Liêm`).
* `IsActive` (Yes/No).

---

## 4. BẢNG LƯU DỮ LIỆU TÍNH TOÁN TẠM THỜI (CalculationSession)

### 4.1. Bối cảnh loại bỏ các bảng tạm di sản
Trong kiến trúc Access cũ, phần mềm có hàng chục bảng tạm (`GeneralLedger`, `DebtLedger`, `InventoryLedger`, `CostLedger`, `Depreciation`...) làm phình to file `.accdb` và gây xung đột khi chạy đa người dùng (Multi-user concurrency).

Toàn bộ các bảng trên **đã được xóa bỏ**. Dữ liệu báo cáo hiện được tính toán động (In-Memory DTO) và truyền thẳng vào DevExpress `XtraReport`.

### 4.2. Cấu trúc bảng `CalculationSession`
Đối với các nghiệp vụ tính toán nhiều bước (như phân bổ chi phí, tính lương, khấu hao) cần lưu lại kết quả tạm trước khi sinh chứng từ ghi sổ:

| Tên trường | Kiểu dữ liệu | Ý nghĩa |
| :--- | :--- | :--- |
| `SessionId` | Short Text (36) - **PK** | Định danh Guid của phiên tính toán |
| `CalculationType` | Short Text (50) | Loại tính toán (`CostAllocation`, `Payroll`, `Depreciation`) |
| `FiscalYear` | Number (Integer) | Năm tài chính |
| `Period` | Number (Integer) | Kỳ / Tháng tính toán (1..12) |
| `PayloadJson` | Long Text (Memo) | **Toàn bộ dữ liệu kết quả dạng JSON** |
| `RecordCount` | Number (Long Integer) | Số dòng kết quả sinh ra |
| `CreatedBy` | Short Text (50) | Tên người dùng thực hiện |
| `CreatedAt` | Date/Time | Thời điểm tính toán |

---

## 5. TÓM TẮT CẤU TRÚC THƯ MỤC TÀI LIỆU KỸ THUẬT

```
D:\NewaccNet\docs\
├── KIEN_TRUC_TINH_TOAN_VA_SCHEMA_MO_RONG.md   <-- (Tài liệu này)
├── HUONG_DAN_SUA_TEMPLATE_ACCDB.md            <-- Chi tiết sửa đổi file Database Template
├── MATERIAL_TRANSACTION_ARCHITECTURE.md       <-- Kiến trúc xử lý luồng kho và giá vốn
└── AI_AGENT_ROADMAP.md                        <-- Lộ trình tích hợp AI kế toán
```

Tất cả các tài liệu trên đã sẵn sàng để xuất hoặc gộp thành cuốn **Sổ tay Thiết kế Kiến trúc Toàn diện (Master Architecture Guide)** của NewaccNet bất kỳ lúc nào.
