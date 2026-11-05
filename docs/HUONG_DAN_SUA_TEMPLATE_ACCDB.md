# HƯỚNG DẪN ĐIỀU CHỈNH SCHEMA CƠ SỞ DỮ LIỆU (TEMPLATE.ACCDB)
*Tài liệu chuẩn hóa Schema, Dọn dẹp bảng thừa và Bổ sung hạ tầng Custom Fields & Dimensions cho NewaccNet*

---

## MỤC LỤC
1. [Danh sách Bảng thừa cần XÓA BỎ](#1-danh-sách-bảng-thừa-cần-xóa-bỏ)
2. [Cảnh báo: Các bảng TUYỆT ĐỐI KHÔNG XÓA](#2-cảnh-báo-các-bảng-tuyệt-đối-không-xóa)
3. [Bổ sung Cột dự phòng (Slots) vào 4 Bảng Cốt lõi](#3-bổ-sung-cột-dự-phòng-slots-vào-4-bảng-cốt-lõi)
4. [Tạo mới 4 Bảng Định nghĩa (Custom Fields & Dimensions)](#4-tạo-mới-4-bảng-định-nghĩa)
5. [Tạo mới Bảng Lưu dữ liệu Tính toán Tạm (CalculationSession / Temp JSON)](#5-tạo-mới-bảng-lưu-tính-toán-tạm-json)
6. [Quy trình Đồng bộ lại LLBLGen Pro v5.12](#6-quy-trình-đồng-bộ-lại-llblgen-pro-v512)

---

## 1. DANH SÁCH BẢNG THỪA CẦN XÓA BỎ
Trong file `template.accdb`, xóa bỏ các bảng di sản sau (đây là các bảng tạm từ thời cũ, logic mới tính toán trực tiếp trong RAM/DTO qua `IAccountingCalculator` và XtraReport):

### Nhóm 1: Bảng tạm in sổ sách (7 bảng)
- [ ] `GeneralLedger` (Sổ cái tạm)
- [ ] `DebtLedger` (Sổ công nợ tạm)
- [ ] `InventoryLedger` (Sổ kho tạm)
- [ ] `CostLedger` (Sổ chi phí tạm)
- [ ] `AssetLedger` (Sổ tài sản tạm)
- [ ] `CurrencyLedger` (Sổ ngoại tệ tạm)
- [ ] `InvestmentLedger` (Sổ đầu tư tạm)

### Nhóm 2: Bảng tính toán & xuất nhập cũ (8 bảng)
- [ ] `CashFlowBalance`
- [ ] `CashFlowYearBalance`
- [ ] `FinancialMonth`
- [ ] `Depreciation`
- [ ] `AssetTrackingDepreciation`
- [ ] `Interest`
- [ ] `InterestDetail`
- [ ] `CustomerExportLine`
- [ ] `InventoryExportVoucher`
- [ ] `AssetExportVoucher`

---

## 2. CẢNH BÁO: CÁC BẢNG TUYỆT ĐỐI KHÔNG XÓA
Các bảng sau đây phục vụ module hóa đơn điện tử Tổng cục Thuế (`NewaccNet.TaxInvoice`), **PHẢI GIỮ LẠI NGUYÊN VẸN**:
- ✅ `TaxInvoice`
- ✅ `TaxInvoiceDetail`
- ✅ `TaxInvoiceTaxRate`
- ✅ `TaxAccountConfig`

---

## 3. BỔ SUNG CỘT DỰ PHÒNG (SLOTS) VÀO 4 BẢNG CỐT LÕI
> **Quy tắc bắt buộc trong Access:** 
> - `Required`: **No** (cho phép NULL).
> - Đối với Short Text: `Allow Zero Length`: **Yes**.

### 3.1. Bảng `Partner` (Khách hàng / Nhà cung cấp)
Thêm các cột:
- `Text1`, `Text2`, `Text3`, `Text4`, `Text5`: **Short Text** (Field Size: `255`)
- `Num1`, `Num2`, `Num3`: **Number** (Field Size: `Decimal`, Scale: `4`, Precision: `18`) hoặc kiểu **Currency**
- `Date1`, `Date2`: **Date/Time**
- `Flag1`, `Flag2`: **Yes/No**
- `ExtData`: **Long Text** (Memo)

### 3.2. Bảng `InventoryItem` (Vật tư / Hàng hóa)
Thêm các cột:
- `Text1`, `Text2`, `Text3`, `Text4`, `Text5`: **Short Text** (Field Size: `255`)
- `Num1`, `Num2`, `Num3`: **Number** (Field Size: `Decimal`, Scale: `4`, Precision: `18`) hoặc kiểu **Currency**
- `Date1`, `Date2`: **Date/Time**
- `Flag1`, `Flag2`: **Yes/No**
- `ExtData`: **Long Text** (Memo)

### 3.3. Bảng `JournalVoucher` (Chứng từ - Header)
Thêm các cột:
- `Text1`, `Text2`, `Text3`, `Text4`, `Text5`: **Short Text** (Field Size: `255`)
- `Num1`, `Num2`, `Num3`: **Number** (Field Size: `Decimal`, Scale: `4`, Precision: `18`) hoặc kiểu **Currency**
- `Date1`, `Date2`: **Date/Time**
- `Flag1`, `Flag2`: **Yes/No**
- `ExtData`: **Long Text** (Memo)
- `RowGuid`: **Short Text** (Field Size: `36`) - Dùng cho UUID định danh chứng từ
- `VoucherSourceCode`: **Short Text** (Field Size: `10`) - Phân loại nguồn chứng từ (NM, XB, PT, PC, BC, BN...)

### 3.4. Bảng `JournalEntry` (Dòng định khoản - Detail)
Thêm các cột:
- `Text1`, `Text2`, `Text3`: **Short Text** (Field Size: `255`)
- `Num1`, `Num2`: **Number** (Field Size: `Decimal`, Scale: `4`, Precision: `18`) hoặc kiểu **Currency**
- `Date1`: **Date/Time**
- `ExtData`: **Long Text** (Memo)
- `Dim1`, `Dim2`, `Dim3`, `Dim4`, `Dim5`: **Short Text** (Field Size: `50`) - 5 chiều thống kê phân tích

---

## 4. TẠO MỚI 4 BẢNG ĐỊNH NGHĨA

### 4.1. Bảng `CustomFieldDefinition` (Định nghĩa trường mở rộng)
| Tên trường (Field Name) | Kiểu dữ liệu trong Access | Thiết lập thuộc tính |
| :--- | :--- | :--- |
| `Id` | AutoNumber | **Primary Key** |
| `EntityName` | Short Text (50) | Required = Yes |
| `FieldKey` | Short Text (50) | Required = Yes (vd: `contract_no`) |
| `Label` | Short Text (100) | Required = Yes (vd: `Số hợp đồng`) |
| `DataType` | Short Text (20) | Required = Yes (`Text`, `Number`, `Date`, `Bool`, `List`) |
| `StorageKind` | Short Text (10) | Required = Yes (`Slot` hoặc `Json`) |
| `SlotName` | Short Text (10) | Required = No (vd: `Text1`, `Num1`) |
| `IsRequired` | Yes/No | Default = No |
| `IsFilterable` | Yes/No | Default = No |
| `MaxLength` | Number (Long Integer) | Required = No |
| `ValidationPattern` | Short Text (200) | Required = No |
| `DefaultValue` | Short Text (255) | Required = No |
| `HelpText` | Short Text (255) | Required = No |
| `SortOrder` | Number (Integer) | Default = 0 |
| `IsActive` | Yes/No | Default = Yes |

### 4.2. Bảng `CustomFieldOption` (Tùy chọn danh sách cho kiểu List)
| Tên trường (Field Name) | Kiểu dữ liệu trong Access | Thiết lập thuộc tính |
| :--- | :--- | :--- |
| `FieldId` | Number (Long Integer) | Foreign Key trỏ về `CustomFieldDefinition.Id` |
| `Code` | Short Text (50) | Required = Yes |
| `Label` | Short Text (100) | Required = Yes |
| `SortOrder` | Number (Integer) | Default = 0 |
| `IsActive` | Yes/No | Default = Yes |
*(Tạo Primary Key kết hợp trên 2 cột: `FieldId` + `Code`)*

### 4.3. Bảng `Dimension` (Danh mục 5 chiều phân tích)
| Tên trường (Field Name) | Kiểu dữ liệu trong Access | Thiết lập thuộc tính |
| :--- | :--- | :--- |
| `Code` | Short Text (50) | **Primary Key** (vd: `DUAN`, `HOPDONG`, `PHONGBAN`) |
| `Name` | Short Text (100) | Required = Yes (vd: `Dự án`, `Hợp đồng kinh tế`) |
| `SlotIndex` | Number (Integer) | Required = Yes (Giá trị từ 1 đến 5, trỏ vào `Dim1..Dim5`) |
| `IsActive` | Yes/No | Default = Yes |

### 4.4. Bảng `DimensionValue` (Chi tiết các đối tượng theo chiều)
| Tên trường (Field Name) | Kiểu dữ liệu trong Access | Thiết lập thuộc tính |
| :--- | :--- | :--- |
| `DimCode` | Short Text (50) | Foreign Key trỏ về `Dimension.Code` |
| `Code` | Short Text (50) | Required = Yes (vd: `DA01`, `HD_2026_001`) |
| `Name` | Short Text (150) | Required = Yes |
| `IsActive` | Yes/No | Default = Yes |
*(Tạo Primary Key kết hợp trên 2 cột: `DimCode` + `Code`)*

---

## 5. TẠO MỚI BẢNG LƯU TÍNH TOÁN TẠM (JSON SNAPSHOT)

Để giải quyết bài toán: **Tính toán phức tạp sinh dữ liệu temp mà chưa định hình trước cấu trúc**, thay vì tạo hàng chục bảng temp khác nhau, hãy tạo **1 bảng duy nhất** chuyên lưu snapshot kết quả dạng JSON:

### Bảng `CalculationSession`
| Tên trường (Field Name) | Kiểu dữ liệu trong Access | Thiết lập thuộc tính |
| :--- | :--- | :--- |
| `SessionId` | Short Text (36) | **Primary Key** (Guid định danh phiên tính) |
| `CalculationType` | Short Text (50) | Required = Yes (vd: `CostAllocation`, `Payroll`, `Depreciation`) |
| `FiscalYear` | Number (Integer) | Năm tài chính |
| `Period` | Number (Integer) | Kỳ / Tháng (1..12) |
| `PayloadJson` | Long Text (Memo) | **Lưu toàn bộ chuỗi JSON dữ liệu tính toán** |
| `RecordCount` | Number (Long Integer) | Số dòng kết quả |
| `CreatedBy` | Short Text (50) | Người thực hiện |
| `CreatedAt` | Date/Time | Thời điểm tính toán |

---

## 6. QUY TRÌNH ĐỒNG BỘ LẠI LLBLGEN PRO v5.12

Sau khi hoàn tất chỉnh sửa trên file `template.accdb`:
1. Mở phần mềm **LLBLGen Pro Designer**.
2. Mở file project: `D:\Databases\DataAccess.llblgenproj`.
3. **Xóa Entities thừa:** Trong cửa sổ *Project Explorer*, xóa các Entity thuộc nhóm Ledger đã xóa trong DB.
4. **Đồng bộ CSDL:** Chọn **Project → Sync Relational Model Data** (Refresh catalog từ `template.accdb`).
5. **Ánh xạ Entity & Cột mới:** 
   - Kiểm tra các Entity `Partner`, `InventoryItem`, `JournalVoucher`, `JournalEntry` đã nhận đủ các trường `Text1..Text5`, `ExtData`...
   - Reverse-Engineer đưa các bảng mới vào Entities: `CustomFieldDefinition`, `CustomFieldOption`, `Dimension`, `DimensionValue`, `CalculationSession`.
6. **Generate Code:** Chọn **Project → Generate Source Code**, chọn thư mục đích đè lên `D:\NewaccNet\dataaccess`.
7. **Biên dịch:** Mở Solution `NewaccNet.slnx` và bấm Rebuild để hoàn tất.
