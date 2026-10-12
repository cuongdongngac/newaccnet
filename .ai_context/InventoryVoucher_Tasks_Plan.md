# KẾ HOẠCH & HIỆN TRẠNG XỬ LÝ PHÂN HỆ VẬT TƯ (INVENTORY VOUCHER)
> Ngày ghi nhận: 2026-10-10  
> Trạng thái: Chuẩn bị thực hiện

---

## 1. TỔNG QUAN KIẾN TRÚC & NGUYÊN TẮC NGHIỆP VỤ

### 1.1. Bản chất dữ liệu Vật tư (Vật tư vs Các đối tượng khác)
- **Vật tư** gắn liền với hoạt động kho và thương mại (mua/bán):
  - **Tầng 1 (Giá gốc / Giá vốn):** Dùng chung bảng `MaterialDetail` (`InventoryVoucherLine`). Áp dụng cho cả Nhập nội bộ (`NNB`), Xuất nội bộ (`XNB`), Nhập mua (`NM`), Xuất bán (`XB`).
  - **Tầng 2 (Giá bán & Thuế):** Tách riêng bảng quan hệ 1-1 `MaterialTaxLine` (`LineId` nối 1-1 với `InventoryVoucherLine.LineId`) để tránh dư thừa dữ liệu cho các chứng từ không có thuế hoặc xuất nhập nội bộ.

### 1.2. Cơ chế Định khoản chân Form và Đảo chiều Cha - Con
- **Tại form `InventoryVoucher` (`MaterialDetailWindow`):**
  - Dưới chân trang **KHÔNG PHẢI LÀ LƯỚI** (vì các định khoản chưa liên hệ cấu trúc với nhau).
  - Nơi đây chỉ gồm các **ô chọn tài khoản đối ứng độc lập** (TK Giá vốn, TK Doanh thu, TK Thuế, TK Thanh toán) tùy theo loại chứng từ (ẩn/hiện theo `VoucherType`).
- **Khi Save quay về Form chính (`JournalVoucher` / `VoucherEntryWindow`):**
  - **LÚC NÀY MỚI LÀ LƯỚI:** Form chính tiến hành ráp nối cây đối ứng:
    - Định khoản mới được chọn ở chân trang trở thành **CHA (Master)**.
    - Định khoản ban đầu trên form `JournalVoucher` (ví dụ `156`) được **gán làm CON (SubEntry) của định khoản mới** và **đổi chiều từ Trái sang Phải** (ví dụ Nợ `632` làm Cha, Có `156` làm Con).
    - Đồng thời sinh nhánh Doanh thu - Thuế nếu là Xuất bán (Nợ `131` làm Cha, Có `511` và Có `3331` làm Con).

---

## 2. CÁC LỖI HIỆN TẠI & NGUYÊN NHÂN KỸ THUẬT (ROOT CAUSE)

### Lỗi 1: Vật tư chưa load lên Grid khi mở một chứng từ đã tồn tại
1. **Thiếu PrefetchPath 3 tầng trong `LoadExistingVoucher` (`VoucherEntryWindow.xaml.cs`):**
   - Hiện tại ORM LLBLGen chưa nạp nhánh vật tư:
     `JournalEntry` $\rightarrow$ `InventoryVouchers` $\rightarrow$ `InventoryVoucherLines` $\rightarrow$ `MaterialTaxLine`.
   - Kết quả: Khi mở chứng từ cũ, `entry.InventoryVouchers` luôn rỗng (`Count == 0`).
2. **Cờ `isNew` bị gán cứng trong `AccountLookUpEdit.cs`:**
   - Dòng code: `var materialForm = new Details.MaterialDetailWindow(entry, true);` luôn truyền `true`. Form luôn coi là phiếu mới tinh và tạo Entity trắng đè lên.

### Lỗi 2: Cột `MaterialTaxLine` (Giá bán, Thuế) trên Grid chỉ ĐỌC, KHÔNG EDIT ĐƯỢC
1. **DevExpress Data Binding với thuộc tính lồng (Nested Property):**
   - Grid cấu hình cột `FieldName="MaterialTaxLine.SellPrice"` và `FieldName="MaterialTaxLine.Taxrateid"`.
   - Với `EntityCollection<InventoryVoucherLineEntity>`, LLBLGen `ITypedList` không hỗ trợ multi-hop PropertyDescriptor để sửa trực tiếp trên ô lồng khi chưa có binding thích hợp.
   - Khi dòng mới được thêm, `row.MaterialTaxLine` bị null do sự kiện `InitNewRow` dùng sai `GetRowByListIndex(e.RowHandle)` (với `RowHandle` âm), dẫn đến DevExpress khóa chết việc gõ vào object null.
2. **Giải pháp chuẩn:**
   - Tạo Partial Class cho `InventoryVoucherLineEntity` với các property phẳng tiện ích: `SellPrice`, `TaxRateId`, `Rate` (bên trong tự động get/set qua `MaterialTaxLine`, tự khởi tạo `MaterialTaxLine` nếu đang null).
   - Bind trực tiếp các cột Grid vào các property phẳng này.

### Lỗi 3: Khi mở form đã tồn tại, Grid vật tư chưa thể hiện đúng trạng thái
1. Dữ liệu chưa nạp lên dẫn đến các cột Unbound (`Thành tiền`, `Tiền bán`, `Tiền thuế`) bằng 0.
2. Chưa kích hoạt đồng bộ hàm `UpdateUIVisibility()` theo `ExobjectId` và `Taxflag` của dữ liệu đã lưu.
3. Khi bấm Save trên form chính, chưa có logic đồng bộ đối soát dọn dẹp các dòng cũ trong DB cho `InventoryVouchers` (giống như đã làm với `DebtDetails`, `CostDetails`).

---

## 3. CHECKLIST CÔNG VIỆC CẦN TRIỂN KHAI & TIẾN ĐỘ THỰC HIỆN

- [x] **Task 1 (Data Layer - Load & Save):**
  - Đã bổ sung PrefetchPath 3 tầng vào `LoadExistingVoucher` và `BtnSave_Click` trong `VoucherEntryWindow.xaml.cs`.
  - Đã sửa `AccountLookUpEdit.cs`: kiểm tra `entry.InventoryVouchers.Count > 0` để truyền `isNew = false`.
  - Đã bổ sung logic cascade/change-tracking dọn dẹp `InventoryVoucher` và `InventoryVoucherLine` khi Save.

- [x] **Task 2 (Grid Material - Cho phép Edit & Khởi tạo `MaterialTaxLine`):**
  - Đã tạo partial class `InventoryVoucherLineEntity.Custom.cs` (các thuộc tính wrapper phẳng: `SellPrice`, `Taxrateid`, `Rate`, kèm `CleanUpTaxLine()`).
  - Đã cập nhật Binding trong `MaterialDetailWindow.xaml` sang các thuộc tính phẳng.
  - Sửa `viewDetails_InitNewRow` và logic khởi tạo instance `MaterialTaxLine`.

- [x] **Task 3 (Chân Form InventoryVoucher):**
  - Bỏ popup trung gian `MaterialAccountSelectionWindow`, đưa trực tiếp các LookUpEdit tài khoản độc lập tại chân trang của `MaterialDetailWindow`.
  - Đã hiển thị đủ 4 cặp Tiền - Tài khoản theo yêu cầu để người dùng quan sát trực quan trước khi ẩn/hiện.
  - Tự động gợi ý tài khoản mặc định (XB: 632, 511, 3331, 131; NM: 1331, 331).

- [x] **Task 4 (Đảo chiều & Ghép nối Cha - Con trên Form chính):**
  - Khi form `MaterialDetailWindow` đóng với kết quả OK, form chính `VoucherEntryWindow` nhận các tài khoản độc lập.
  - Tự động tạo định khoản Cha mới, đổi chiều định khoản ban đầu (TK 156) sang Có làm con của Cha mới (TK 632) đối với XB.
  - Sinh tiếp định khoản Doanh thu / Thuế (đối với XB: 5 bản ghi; NM: 3 bản ghi; NNB/XNB: 2 bản ghi).
  - Refresh và hiển thị mượt mà trên 2 Lưới của `VoucherEntryWindow`.

- [x] **Task 5 (Sửa lỗi UI phát sinh khi Test):**
  - **Lỗi Dropdown Vật tư trắng:** Đã bổ sung `LookUpEditSettings.PopupContentTemplate` với `PART_GridControl` (gồm 3 cột: Mã VT, Tên vật tư, ĐVT) và cấu hình `PopupWidth="460"`, `AutoComplete="True"`, `ImmediatePopup="True"`.
  - **Lỗi Dropdown Thuế trắng:** Đã bổ sung `PopupContentTemplate` với 2 cột (Thuế %, Mô tả) và đưa trường `Taxratedes` vào nạp nguồn `lueTaxRate.ItemsSource`.
  - **Lỗi luôn báo "Vui lòng chọn Kho":** Đã bổ sung 2-way binding `EditValue="{Binding WarehouseId, Mode=TwoWay}"`, bổ sung sự kiện `lueWarehouse_EditValueChanged`, tự động gợi ý kho mặc định đầu tiên khi mở form mới, và thêm fallback lấy trực tiếp từ `lueWarehouse.EditValue` khi bấm Chấp nhận.

- [x] **Task 6 (Ràng buộc chọn Nợ/Có trước khi chọn TK & Tối ưu hiển thị Vật tư):**
  - **Ràng buộc chọn Nợ/Có:** Bắt sự kiện `TableView_ShowingEditor` trên cả `gridLeft` và `gridRight` của `VoucherEntryWindow`. Nếu dòng hiện tại chưa chọn Nợ/Có (`Dbcr` null hoặc 0), hệ thống hủy kích hoạt ô Tài khoản (`e.Cancel = true`), thông báo nhắc nhở và tự động chuyển con trỏ về ô Nợ/Có. Đồng thời bổ sung chặn bảo vệ trong `AccountLookUpEdit.cs`.
  - **Tối ưu hiển thị cột Vật tư:** Bỏ cột `MaterialName` riêng biệt trên Grid `MaterialDetailWindow`. Cột `MaterialId` được đổi Header thành "Vật tư", cấu hình `DisplayMember="MaterialName" ValueMember="MaterialId"` để hiển thị trực tiếp Tên vật tư trong ô, và đảo vị trí cột `Tên vật tư` lên đầu tiên trong popup dropdown để tìm kiếm theo tên làm chủ đạo giống như cột thuế.
  - **Biên dịch:** Đã build thành công `dotnet build NewaccNet.slnx` với **0 Error(s)**.
