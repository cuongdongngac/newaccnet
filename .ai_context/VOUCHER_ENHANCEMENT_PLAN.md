# KẾ HOẠCH NÂNG CẤP CHỨNG TỪ & HÓA ĐƠN (VOUCHER & INVOICE ENHANCEMENT PLAN)
> Ngày tạo: 11/10/2026  
> Trạng thái: Hoàn thành (Completed)

---

## I. TỔNG HỢP YÊU CẦU NGƯỜI DÙNG

1. **Thông tin in ấn chứng từ/hóa đơn thu gọn dạng Accordion:**
   - Các thông tin bổ trợ phục vụ in ấn (Người nộp/nhận, địa chỉ, số chứng từ gốc kèm theo, tài khoản & ngân hàng thanh toán/nhận) ít khi nhập trừ phi muốn in chứng từ.
   - Thiết kế vùng thể hiện các thông tin này dưới dạng **Accordion (Collapsible Group)**, mặc định thu gọn.
   - **Không focus qua bàn phím** (bỏ qua phím Tab để người dùng nhập nhanh Số CT -> Ngày CT -> Nội dung -> Lưới định khoản mà không bị dừng lại ở các trường in ấn). Khi cần nhập thì click chuột mới mở ra.
2. **Co gọn vùng trống Header:**
   - Vùng trống giữa nội dung chứng từ/hóa đơn còn trống nhiều, cần co lại hợp lý, nhường vị trí gọn gàng cho Accordion.
3. **Cơ chế Collection of Voucher khi New / Mở chứng từ:**
   - Khi bấm New hoặc mở form chứng từ: Thiết lập một Collection of Voucher (`ObservableCollection<JournalVoucherEntity>`).
   - Thanh điều hướng (Navigation) di chuyển giữa các phần tử trong Collection này.
4. **Hành vi nút New và Control Navigation DevExpress:**
   - Khi click "New" trong form chứng từ: Thêm 1 phần tử voucher mới vào Collection và gán làm phần tử hiện hành (`Set Current`) ngay lập tức.
   - Thanh điều hướng First, Prev, Next, Last kèm nhãn chỉ số bản ghi `[X / N]` đồng bộ.
   - Lưu toàn bộ lô chứng từ (`SaveVoucherBatch`) trong một Transaction duy nhất.
5. **Nút Thêm mới nhanh (+) trên các ComboBox / LookUpEdit danh mục:**
   - Các LookUp/ComboBox chọn: Đối tượng công nợ (`Partner`), Nội dung (`DebtType`), Lý do công nợ (`DebtReason`), Đối tượng chi phí (`CostObject`), Yếu tố chi phí (`CostElement`), Kho hàng (`Warehouse`), Khách hàng thuế (`VatObject`), Vật tư (`Material`).
   - Tích hợp nút `+` gọi form Edit tương ứng (`PartnerEditorWindow`, `CostObjectEditorWindow`, `WarehouseEditorWindow`...) để tạo mới ngay tại chỗ khi chưa có đối tượng.
   - Sau khi tạo mới thành công: Tự động lưu vào DB, nạp bổ sung vào Collection nguồn của LookUp và gán chọn luôn cho dòng/ô hiện tại.

---

## II. CHECKLIST TRIỂN KHAI VÀ ĐÁNH DẤU TIẾN ĐỘ

### Mục 1 & 2: Vùng Accordion thông tin in ấn & Co gọn Header
- [x] **1.1 (UI XAML):** Thiết kế `dxlc:LayoutGroup View="GroupBox" IsCollapsible="True" IsCollapsed="True"` trên `VoucherEntryWindow.xaml`.
- [x] **1.2 (Keyboard Navigation):** Cấu hình `KeyboardNavigation.IsTabStop="False"` và `KeyboardNavigation.TabNavigation="None"` để phím Tab bỏ qua Accordion khi đang đóng hoặc di chuyển Tab từ Nội dung sang Lưới.
- [x] **1.3 (Binding Data):** Bổ sung binding cho các trường: `Personname`, `Personaddress`, `Invoicesnumber`, `Paymentacc`, `Paymentbank`, `Recievedcorp`, `Recievedacc`, `Recievedbank`.
- [x] **1.4 (Co gọn Layout):** Bố trí lại dòng Số chứng từ, Ngày chứng từ, Nội dung để loại bỏ khoảng trắng thừa, giao diện gọn gàng, liền mạch.

### Mục 3 & 4: Quản lý Collection of Voucher & Điều hướng DevExpress Navigation (Nhập theo lô)
- [x] **3.1 (Data Model - Batch Collection):** Quản lý `ObservableCollection<JournalVoucherEntity> _voucherBatch` trong `VoucherEntryWindow`.
- [x] **3.2 (Navigation Buttons & DevExpress Control):** Điều hướng First, Prev, Next, Last kèm nhãn bản ghi `lblNavPosition` `[X / N]` đồng bộ với `_voucherBatch`.
- [x] **3.3 (Hành vi nút New):** Khi click "New", commit chứng từ hiện tại vào batch, tạo mới `JournalVoucherEntity`, `_voucherBatch.Add(newVoucher)`, và `SetCurrentVoucher` ngay lập tức.
- [x] **3.4 (Lưu theo lô - Batch Save):** Khi bấm "Lưu (Save)", thực hiện lưu toàn bộ Collection `_voucherBatch` trong một CSDL Transaction duy nhất (`SaveVoucherBatch`), kiểm tra cân đối nợ có tất cả chứng từ trong lô trước khi commit.
- [x] **3.5 (VoucherListView tĩnh):** Bỏ tự động `DoSearch()` khi đóng form VoucherEntryWindow để tránh reload thừa thãi khi người dùng không có nhu cầu tìm kiếm lại.

### Mục 5: Nút Tạo mới nhanh (+) trên ComboBox/LookUpEdit
- [x] **5.1 (Công nợ - DebtDetailWindow):** Tích hợp nút `+` vào LookUp chọn `PartnerId`, `DebtTypeId`, `DebtReasonId`. Bấm vào mở `PartnerEditorWindow` / `DebtTypeEditorWindow` / `DebtReasonEditorWindow`. Sau khi tạo, thêm vào danh sách và chọn luôn cho ô hiện tại.
- [x] **5.2 (Chi phí - CostDetailWindow):** Tích hợp nút `+` vào ô chọn `CostObjectId`, `CostElementId` và `lookupVatObject`. Bấm vào mở `CostObjectEditorWindow` / `CostElementEditorWindow` / `PartnerEditorWindow`.
- [x] **5.3 (Vật tư / Thuế - MaterialDetailWindow):** Tích hợp nút `+` vào ô chọn `Warehouse` (mở `WarehouseEditorWindow`), `MaterialId` (mở `InventoryItemEditorWindow`), và `VatObject` (mở `PartnerEditorWindow`).
- [x] **5.4 (Kiểm thử & Đồng bộ ItemsSource):** Cấu hình `EditorButtonShowMode="ShowForFocusedRow"` và `CellTemplate` với `PART_Editor`, đảm bảo việc thêm mới vào collection không làm mất filter hoặc reset lại các dòng khác trên Grid.

---

## III. NHẬT KÝ THỰC HIỆN (EXECUTION LOG)

1. **VoucherListView tĩnh:** Đã gỡ bỏ gọi `DoSearch()` tự động khi `entryForm.ShowDialog()` đóng lại trong `BtnAdd_Click` và `EditSelected()` tại `VoucherListView.xaml.cs`.
2. **VoucherEntryWindow Accordion & Header:**
   - Dòng tiêu đề gộp gọn: ID, Số CT, Ngày CT, Diễn giải, Ghi sổ.
   - Nhóm thông tin in ấn và ngân hàng được đặt trong `dxlc:LayoutGroup` với `IsCollapsible="True"`, `IsCollapsed="True"`, `KeyboardNavigation.IsTabStop="False"` và `KeyboardNavigation.TabNavigation="None"`.
   - Các trường ngân hàng/in ấn được gắn thuộc tính `KeyboardNavigation.IsTabStop="False"`.
3. **Cơ chế Batch Voucher & Điều hướng:**
   - Quản lý `_voucherBatch` và chỉ số `_currentVoucherIndex`.
   - Các nút Toolbar: `btnFirst`, `btnPrev`, `lblNavPosition` (nhãn X / N), `btnNext`, `btnLast` hỗ trợ duyệt nhanh giữa các chứng từ trong lô.
   - Khi bấm New: Chứng từ đang nhập được commit vào batch, một chứng từ mới được thêm vào cuối lô và chuyển thành chứng từ hiện hành ngay lập tức.
   - Lưu lô (`SaveVoucherBatch`): Quét kiểm tra cân đối nợ có từng chứng từ. Nếu có chứng từ chưa cân đối, tự động chuyển đến chứng từ đó và báo lỗi. Nếu tất cả hợp lệ, lưu toàn bộ trong một Transaction duy nhất.
4. **Tạo mới nhanh (+) trên LookUpEdit:**
   - Tạo mới `WarehouseEditorWindow` (`WarehouseEditorWindow.xaml` + `.cs`).
   - Cập nhật `DebtDetailWindow`: Thay thế `EditSettings` bằng `CellTemplate` chứa `<dxg:LookUpEdit Name="PART_Editor">` và `<dxe:ButtonInfo GlyphKind="Plus">` cho `PartnerId`, `DebtTypeId`, `DebtReasonId`.
   - Cập nhật `CostDetailWindow`: Thay thế `EditSettings` bằng `CellTemplate` chứa LookUpEdit và nút `+` cho `CostObjectId`, `CostElementId`, và thêm nút `+` vào standalone `lookupVatObject`.
   - Cập nhật `MaterialDetailWindow`: Thêm nút `+` vào standalone `lueWarehouse` và `lueVatObject`, chuyển đổi cột `MaterialId` sang `CellTemplate` có nút `+`.
   - Kết quả: Khi click nút `+`, mở dialog tương ứng, lưu vào DB, nạp vào collection nguồn và cập nhật ngay vào dòng đang chọn.
5. **Khắc phục lỗi tràn độ dài trường VoucherNo (Database Overflow Fix):**
   - **Nguyên nhân:** Cột `VoucherNo` trong database và LLBLGen entity có độ dài tối đa là 10 ký tự (`VarWChar(10)` / `NVarChar(10)`). Chuỗi sinh tự động cũ `"N" + DateTime.Now.ToString("yyMMddHHmm")` có độ dài 11 ký tự gây `ArgumentOutOfRangeException`.
   - **Xử lý:**
     - Bổ sung hàm `GenerateNextVoucherNo()`: Tự động kế thừa và tăng dần số chứng từ theo định dạng số của chứng từ trước trong lô (ví dụ `PK001` -> `PK002`, `CT10` -> `CT11`), hoặc theo cấu trúc ngày `CTyyMMdd{seq:D2}` (chuẩn đúng 10 ký tự).
     - Bổ sung `MaxLength="10"` cho `txtVoucherNo` và cấu hình `MaxLength` tương ứng cho toàn bộ các trường khác trong header và accordion (`Contents`: 100, `Invoicesnumber`: 2, `Personname`: 50, `Personaddress`: 255, `Paymentacc`/`Recievedacc`: 50, `Paymentbank`/`Recievedbank`/`Recievedcorp`: 150).
     - Rà soát hàm `UnbindHeader()`, tự động cắt chuỗi an toàn trước khi gán vào entity để triệt tiêu hoàn toàn khả năng phát sinh exception tràn bộ đệm.
6. **Kiểm tra biên dịch:** Dự án `NewaccNet.Wpf` biên dịch thành công 100% với 0 lỗi (Build Succeeded).
