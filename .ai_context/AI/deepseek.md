
**Nguyên lý:** trục trên sửa 1 lần = trục dưới sửa 100 lần. Thứ tự ưu tiên mặc định: **Trục 1 → Trục 2 → Trục 3**, nhưng không tuyệt đối — có hạng mục Trục 3 có giá trị chứng minh cao đến mức chen vào giữa Trục 2 (ví dụ: một vertical slice staging → ghi sổ, không AI, làm rất sớm).

---

## Phần 3 — Chi tiết hóa Trục 1: Nền dữ liệu

### 3.1 Danh sách hạng mục

| Mã | Hạng mục | Nguồn v3.1 | Trạng thái |
|---|---|---|---|
| F1 | `SchemaVersion` + `SchemaMigrator` + `Migrations/` | S17 | Đã có đặc tả |
| F2 | `float` → `decimal` | S1 | Đã có đặc tả |
| F3 | Index (12+ trên bảng chính) | S2 | Đã có đặc tả |
| F4 | `JournalEntry` NOT NULL + `EntryKind` | S3 | **Chưa chốt** (câu 12) |
| F5 | 1-1 `InventoryVoucher` ↔ `JournalEntry` | S4 | Đã có đặc tả |
| F6 | `VoucherSourceCode` + UNIQUE `(SourceCode, Year, VoucherNo)` | S5 | **Cần sửa UNIQUE có năm** |
| F7 | Audit fields + `RowVersion` | S6 | Đã có đặc tả |
| F8 | `InventoryVoucher.Graph` + `InventoryGraphRebuilder` | S6 + 7.9.4 | Đã có đặc tả |
| F9 | `JournalVoucher.RowGuid` | S14 | Đã có đặc tả |
| F10 | Password hash | S13 | **Chưa xác nhận hiện trạng** |
| F11 | **Mô hình kỳ kế toán** | **CHƯA CÓ** | **Thiếu — xem 3.3** |
| F12 | **Chiến lược backup/restore** | **CHƯA CÓ** | **Thiếu — xem 3.3** |
| F13 | **Mô hình bút toán đảo/điều chỉnh** | **CHƯA CÓ** | **Thiếu — xem 3.3** |
| F14 | Trường mở rộng (Custom Fields) | S15 | Đã có đặc tả |
| F15 | Mã thống kê (Dimensions) | S16 | Đã có đặc tả |
| F16 | `ExchangeRate` có cột `Year` | Câu 13 | **Lỗi thiết kế — cần sửa** |

### 3.2 Tiêu chí "đúng" cho Trục 1

Một hạng mục Trục 1 được coi là xong khi:
1. Có migration đủ 3 file SQL (Access/SQL Server/PostgreSQL), chạy lại an toàn (idempotent).
2. Có script chuyển dữ liệu cho DB khách đang chạy.
3. Test trên bản sao dữ liệu thật của ít nhất 1 khách mỗi loại DB.
4. Cân đối phát sinh và sổ chi tiết trùng tuyệt đối trước/sau.
5. Code LLBLGen sinh lại, commit cùng migration.

### 3.3 Ba lỗ hổng cần bổ sung ngay **[GỘP VÀO GỐC]**

#### F11 — Mô hình kỳ kế toán

Tài liệu gốc nói nhiều về `CanUpdateVoucher`, khóa sổ, nhưng không mô tả **kỳ kế toán lưu ở đâu**.
