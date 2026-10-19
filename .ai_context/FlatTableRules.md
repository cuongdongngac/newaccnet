# Quy tắc dựng bảng phẳng (Flat Table Rules)

Mục tiêu: một cách hiểu chung cho mọi nơi cần tính toán từ dữ liệu chi tiết (báo cáo chi tiết, bảng cân đối tài khoản, chuyển số dư, kết chuyển chi phí, phân bổ chi phí, khấu hao TSCĐ...).

## 0. Nguyên tắc

- Đa CSDL: **không dùng stored procedure, view hay SQL riêng theo hệ**. Tất cả tính toán nằm ở tầng ứng dụng (LLBLGen + LINQ trên bộ nhớ).
- Chấp nhận chậm, đổi lại dễ kiểm soát và dễ bảo trì. Không viết mẹo thông minh. Mỗi bước làm một việc.
- Mọi tính toán đều đi qua bảng phẳng. Không gom nhóm trực tiếp trên dữ liệu thô.

## 1. Ba tầng

### Tầng 1: bảng phẳng cấp tài khoản
Code hiện có: [`NewaccNet.Reports/FlattenedLedgerService.cs`](file:///d:/NewaccNet/NewaccNet.Reports/FlattenedLedgerService.cs) (`FlattenedLedgerEntry`).

| Cột | Ý nghĩa |
|---|---|
| `EntryId` | Id của `JournalEntry` gốc, dùng làm khóa nối xuống chi tiết |
| `AccountId`, `CounterAccountId` | Tài khoản và tài khoản đối ứng |
| `Dbcr`, `Amount` | Chiều (+1 Nợ, -1 Có) và số tiền |
| `VoucherDate` | Ngày lấy từ chứng từ |

Quy ước:
- Bút toán có `ParentId` được tách thành hai dòng (phía con và phía cha). Mỗi dòng giữ `EntryId` của chính nó.
- `JournalVoucherId = null` là số dư đầu kỳ. Gán ngày `beginDate - 1`, số chứng từ `SDDK`.
- Bút toán lẻ không có đối ứng vẫn được giữ, `CounterAccountId` để trống.

---

### Tầng 2: bảng phẳng cấp chi tiết (`NewaccNet.Reports.FlattenedDetails`)
Code chuẩn dùng chung toàn hệ thống:
- Models: [`NewaccNet.Reports/FlattenedDetails/FlatDetailRows.cs`](file:///d:/NewaccNet/NewaccNet.Reports/FlattenedDetails/FlatDetailRows.cs)
- Service: [`NewaccNet.Reports/FlattenedDetails/FlattenedDetailService.cs`](file:///d:/NewaccNet/NewaccNet.Reports/FlattenedDetails/FlattenedDetailService.cs)

Dòng chi tiết (`DebtDetail`, `CostDetail`, `AssetDetail`, `InventoryVoucherLine`, `CurrencyDetail`, `InvestmentDetail`, `VatEntry`) là dòng gốc. Nó đã có sẵn khóa chi tiết và `Amount`. Phần còn thiếu là **tài khoản (`AccountId`, `CounterAccountId`)**, **chiều định khoản (`Dbcr`)** và **chứng từ (`VoucherDate`, `VoucherNo`, `Contents`)**, lấy từ `JournalEntry` và `JournalVoucher` của nó.

#### Lớp cơ sở chung: `FlatDetailRowBase`
Mọi dòng phẳng chi tiết đều kế thừa `FlatDetailRowBase` gồm:
- `DetailId`, `JournalEntryId`, `JournalVoucherId`, `VoucherNo`, `VoucherDate`, `Contents`
- `AccountId`, `CounterAccountId`, `Dbcr` (`+1` Nợ / `-1` Có), `Amount`
- Thuộc tính tính sẵn: `DebitAmount`, `CreditAmount`, `SignedAmount` (`Dbcr * Amount`), `IsOpening`
- Hàm kiểm tra mốc thời gian:
  - `IsOpeningAt(DateTime beginDate)`: thuộc số dư đầu kỳ tại mốc `beginDate` (`IsOpening || VoucherDate < beginDate`)
  - `IsInPeriod(DateTime beginDate, DateTime endDate)`: phát sinh trong kỳ `[beginDate .. endDate]`
  - `IsUpToDate(DateTime endDate)`: lũy kế tính đến ngày `endDate` (`IsOpening || VoucherDate <= endDate`)

#### Danh sách 7 Class Flat Chi Tiết Chuẩn (`FlatDetailRows.cs`):

| STT | Class phẳng (Tầng 2) | Bảng gốc (Entity) | Khóa lọc / Gom nhóm chính | Hàm nạp trong `FlattenedDetailService` |
| :---: | :--- | :--- | :--- | :--- |
| **1** | `DebtFlatRow` | `DebtDetailEntity` | `PartnerId` (Đối tượng), `DebtTypeId` (Nội dung), `DebtReasonId`, `LiabilityDuesId`, `LiabilityDate`, `CurrencyId` | `LoadDebtFlatRows(...)` / `FlattenDebtEntries(...)` |
| **2** | `CostFlatRow` | `CostDetailEntity` | `CostObjectId` (Đối tượng CP), `CostElementId` (Yếu tố CP), `TransFlag`, `SerialNo` | `LoadCostFlatRows(...)` |
| **3** | `AssetFlatRow` | `AssetDetailEntity` | `AssetId` (Mã TS), `SourceCode` (Nguồn vốn), `ReasonId` (Lý do), `ReasonFlag`, `Quantity`, `Price` | `LoadAssetFlatRows(...)` |
| **4** | `InventoryFlatRow` | `InventoryVoucherLineEntity` + `InventoryVoucherEntity` | `WarehouseId` (Kho), `MaterialId` (Vật tư), `CostObjectId`, `InOutNumber`, `Quantity`, `Price` | `LoadInventoryFlatRows(...)` |
| **5** | `CurrencyFlatRow` | `CurrencyDetailEntity` | `CurrencyId` (Mã ngoại tệ), `Quantity` (Nguyên tệ), `ExchangeRate` (Tỷ giá) | `LoadCurrencyFlatRows(...)` |
| **6** | `InvestmentFlatRow` | `InvestmentDetailEntity` | `StockId` (Mã CK), `CustomerId` (Đối tượng), `StockStoreId`, `Quantity`, `InPrice`, `OutPrice`, `OriginAmount` | `LoadInvestmentFlatRows(...)` |
| **7** | `VatFlatRow` | `VatEntryEntity` | `VatType` (Đầu vào/ra), `CatId`, `TaxRateId`, `Rate`, `BeforeTaxAmount`, `VatBillNo`, `VatBillDate`, `VatObjectId`, `TaxNo` | `LoadVatFlatRows(...)` |

#### Quy tắc đối chiếu với số dư tài khoản (`includeUnmappedEntries`)
Nếu cần đối chiếu với số dư tài khoản (ví dụ: Kiểm tra công nợ hoặc Bảng cân đối tài khoản): `JournalEntry` của tài khoản chi tiết mà **không có** dòng chi tiết nào thì sinh một dòng khóa chi tiết để trống (`IsUnmappedEntry = true`), số tiền lấy từ bút toán. Chênh lệch lộ ra thay vì bị mất.

---

### Tầng 3: tính toán
Chỉ dùng LINQ trên bảng phẳng tầng 2 (`List<TFlatRow>`):
1. Lọc theo ngày (`IsOpeningAt(beginDate)`, `IsInPeriod(beginDate, endDate)`, hoặc `IsUpToDate(endDate)`).
2. Lọc theo danh sách đối tượng / nội dung được chọn trên bộ lọc báo cáo (ví dụ: `selectedPartnerIds.Contains(r.PartnerId)`).
3. `GroupBy` theo khóa chi tiết và tính số dư / phát sinh (`Sum(r => r.SignedAmount)`, `Sum(r => r.DebitAmount)`, `Sum(r => r.CreditAmount)`).
