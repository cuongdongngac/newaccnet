# Quy tắc dựng bảng phẳng (Flat Table Rules)

Mục tiêu: một cách hiểu chung cho mọi nơi cần tính toán từ dữ liệu chi tiết (báo cáo, chuyển số dư, kết chuyển chi phí...).

## 0. Nguyên tắc

- Đa CSDL: **không dùng stored procedure, view hay SQL riêng theo hệ**. Tất cả tính toán nằm ở tầng ứng dụng (LLBLGen + LINQ trên bộ nhớ).
- Chấp nhận chậm, đổi lại dễ kiểm soát và dễ bảo trì. Không viết mẹo thông minh. Mỗi bước làm một việc.
- Mọi tính toán đều đi qua bảng phẳng. Không gom nhóm trực tiếp trên dữ liệu thô.

## 1. Ba tầng

### Tầng 1: bảng phẳng cấp tài khoản
Code hiện có: `NewaccNet.Reports/FlattenedLedgerService.cs` (`FlattenedLedgerEntry`).

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

### Tầng 2: bảng phẳng cấp chi tiết
Dòng chi tiết (`CostDetail`, `DebtDetail`, `AssetDetail`...) là dòng gốc. Nó đã có sẵn khóa chi tiết và `Amount`. Phần còn thiếu là **tài khoản**, **Dbcr** và **ngày**, lấy từ `JournalEntry` và chứng từ của nó.

**Chỉ cần đi qua `JournalEntry` (và `JournalVoucher` để lấy ngày), không cần qua bảng phẳng ledger (tầng 1).** Tầng 1 chỉ cần khi cần tài khoản đối ứng (sổ cái, nhật ký, báo cáo theo cặp Nợ/Có). Kết chuyển chi phí không cần đối ứng nên bỏ qua tầng 1.

Cách dựng:
1. Mỗi dòng chi tiết nối về `JournalEntry` theo `JournalEntryId`, lấy `AccountId` và `Dbcr`.
2. `JournalEntry` nối tới `JournalVoucher` lấy `VoucherDate`. Nếu `JournalVoucherId = null` thì là số dư đầu kỳ (`IsOpening = true`).
3. Số tiền chi tiết: ưu tiên `Amount` của chi tiết. Nếu null thì dùng số tiền của bút toán.
4. Nếu cần đối chiếu với số dư tài khoản: `JournalEntry` của tài khoản chi tiết mà **không có** dòng chi tiết nào thì sinh một dòng khóa chi tiết để trống, số tiền lấy từ bút toán. Chênh lệch lộ ra thay vì bị mất.

Ví dụ chi phí: `CostFlatRow = (JournalEntryId, AccountId, CostObjectId, CostElementId, Dbcr, Amount, VoucherDate, IsOpening)`.


### Tầng 3: tính toán
Chỉ dùng LINQ trên bảng phẳng tầng 2:
1. Lọc theo ngày (`VoucherDate <= endDate`), phân biệt đầu kỳ bằng `IsOpening`.
2. `GroupBy` theo khóa chi tiết.
3. Số dư = `Σ(Dbcr × Amount)`.

## 2. Việc cần làm / ghi chú

- Chuẩn hóa chuỗi khóa (`Trim`, null thành rỗng) trước khi `GroupBy`.
- Hiện `DebtAccountCalculator` tự làm bước nối tầng 2. Nên có một hàm chung cho mọi loại chi tiết để tránh lặp. Chưa sửa code cũ, chỉ áp dụng cho kết chuyển chi phí trước.
- Mỗi dòng chi tiết thuộc đúng một `JournalEntry` qua `JournalEntryId`, nên không phải xử lý cặp cha/con (`ParentId`) ở tầng 2.
- Loại tài khoản (`CategoryId`): `E` = chi phí, `I` = tiền mặt, không theo dõi chi tiết. Chỉ `E` mới có `CostDetail`.

## 3. Áp dụng: kết chuyển chi phí

Bảng phẳng chi phí của tài khoản nguồn → lọc `VoucherDate <= Đến ngày` → gom `(CostObjectId, CostElementId)` → `Rest` → giữ `Rest <> 0` → danh sách `CostTransferRow` trong bộ nhớ (cột chọn, chi phí để lại, số chuyển). Không có bảng tạm trong DB.
