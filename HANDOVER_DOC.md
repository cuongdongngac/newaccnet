# TÀI LIỆU BÀN GIAO & ĐIỂM DỪNG DỰ ÁN (NewaccNet)

_Ngày cập nhật: 29/09/2026 (Lần 4 - Hoàn thiện Sổ Nhật ký Báo cáo)_

## 1. Các quy tắc chung (Bắt buộc tuân thủ)

- **LƯU Ý NGHIÊM TRỌNG VỀ SVG ICON CỦA DEVEXPRESS**: Tuyệt đối không tự đoán tên file SVG (ví dụ: o*currency.svg, ctions/add.svg) để đưa vào thuộc tính LargeGlyph. Nếu file không tồn tại trong thư viện DevExpress.Images.v25.2.dll, toàn bộ ứng dụng sẽ bị crash ngay khi khởi động (XamlParseException: Cannot locate resource). Giải pháp: Nếu không chắc chắn, hãy dùng các icon đã biết là an toàn như usiness%20objects/bo_document.svg hoặc xaf/action*... cho đến khi có thể chọn chính xác bằng Image Picker của Visual Studio.
- **Mô hình lưới dữ liệu (GridControl)**: Kế thừa từ `BaseDictionaryWindow`. Cần có thanh tìm kiếm (`ShowSearchPanelMode="Always"`), **BỎ dòng lọc trực tiếp** (`ShowAutoFilterRow="False"` ⚠ quy tắc cập nhật 28/09/2026), **giữ chế độ lọc ở Header** (mặc định của DevExpress Column Header Filter), và Menu chuột phải `RowCellMenuCustomizations` (Thêm, Sửa, Xóa, In).
- **Save danh mục không đi qua hàm kế thừa:** `BaseDictionaryWindow.SaveRecord()` là stub rỗng, không form nào override. Persist = `adapter.SaveEntity` trong code-behind:
  - **Form Popup Editor** (AllowEditing=False): Save sau `ShowDialog() == true` của Editor (gọi `adapter.SaveEntity`).
  - **Form Inline Edit** (AllowEditing=True): Bắt `TableView_RowUpdated` (khi user nhảy dòng) để auto save, và **PHẢI có nút "Lưu" trên ToolBar** (gọi `gridControl.View.CommitEditing()` để flush các thay đổi đang ở ô nhập liệu hiện tại). _Cập nhật 28/09/2026: 3 Form trước đây thiếu nút Lưu (Category, TaxRate, Currency) đã được bổ sung._
  - Chi tiết reference: `.ai_context/01_architecture/directory_save.md`.
- **In ấn Grid**: Sử dụng `ReportManager.PrintGridControl` (`PrintAutoWidth = true` và `Landscape = true`). Lọc trên Grid sẽ phản ánh đúng ra Report.
- **Xử lý form đặc thù (Nhỏ, ít biến động)**: Bắt buộc dùng **Inline Editing** (chỉnh sửa trực tiếp trên Grid, `AllowEditing="True"`).
  - Vị trí dòng thêm mới bắt buộc phải nằm ở dưới cùng: `NewItemRowPosition="Bottom"`.
  - Các cột liên kết khóa ngoại (Foreign Key) trên Grid Inline phải dùng Dropdown (`ComboBoxEditSettings`).
  - **Toolbar bắt buộc có 4 nút (đúng thứ tự)**: Lưu → Xóa → In → Làm mới.
  - _Áp dụng cho_: Kho hàng, **Bộ phận / Phòng ban (Department)**, Nhóm vật tư, Thuế suất, Tiền tệ, Tỷ giá, Nguồn tài sản, Lý do tăng giảm, Loại hình CP, Danh mục CP.

## 2. Các công việc vừa hoàn thành (Mới nhất: 29/09/2026)

- **MỚI NHẤT: Sổ Nhật Ký (Diary Report) - Module Báo cáo đầu tiên HOÀN THÀNH (29/09/2026):**
  - **Mapping Access → LLBLGen (basequery.txt):** Bills → JournalVoucher, Transaction List → JournalEntry, Account List → ChartOfAccount. Dbcr (1=Nợ, -1=Có) \* Amount → tính Debit/Credit.
  - **[DiaryCalculator.cs](file:///d:/NewaccNet/NewaccNet.Wpf/System/Reports/Calculators/DiaryCalculator.cs):** Dùng **LinqMetaData** (LINQ to LLBLGen, không cần Typelist manual) JOIN 3 bảng, filter theo FromDate/ToDate + Bookflag (optional), sort theo VoucherDate → VoucherNo → VoucherId → Dbcr DESC (hiện dòng Nợ trước, dòng Có sau trong cùng 1 CT). Tính Debit/Credit client-side.
  - **[DiaryReportWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/System/Reports/DiaryReportWindow.xaml) + .xaml.cs:**
    - **Toolbar:** 2 DateEdit (Từ/Đến ngày - mặc định Đầu tháng → Hôm nay), Check "Chỉ CT đã ghi sổ", Nút Lọc (F5), Làm mới, In (Ctrl+P), Bù cột, Đóng (Esc).
    - **Thanh tổng quan:** 4 thẻ = Tổng số CT / Số bút toán / Tổng Nợ / Tổng Có.
    - **GridControl:** 10 cột (Ngày CT + Số CT Fixed Left, Nội dung, Mã TK, Tên TK, Ghi sổ, Nợ/Có Fixed Right). GroupPanel kéo thả group theo Số CT. TotalSummary (Fixed Footer) + GroupSummary Sum Nợ/Có. SearchPanel Always, AutoFilterRow=False (quy tắc UI).
    - **Footer Check:** Màu xanh "✅ CÂN ĐỐI" nếu |SumNợ - SumCó| < 0.005, ngược lại đỏ "❌ LỆCH".
  - **Tích hợp MainWindow — Flow hoàn chỉnh (KHÔNG hiện Grid xem trước — theo yêu cầu user):**
    - [MainWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml#L331): Thêm `ItemClick="BtnReportDiary_ItemClick"` vào nút "Nhật ký" (Tab Báo cáo → Sổ sách, RequiredMask=8).
    - [MainWindow.xaml.cs BtnReportDiary_ItemClick](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml.cs#L215): Handler 3 bước:
      1. **Show [DiaryFilterWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/System/Reports/DiaryFilterWindow.xaml)** Modal — Dialog lấy tham số (Từ ngày / Đến ngày / Check "Chỉ CT đã ghi sổ Bookflag=1", mặc định đầu-tháng → hôm nay). Enter=OK, Esc=Cancel. Validate From<=To.
      2. Nếu người dùng OK → gọi `DiaryCalculator.Calculate(...)` → nếu 0 dòng → MessageBox "Không tìm thấy bút toán..." → dừng (không show empty report).
      3. Có dữ liệu → gọi **[DiaryReportGridFactory.Build(rows, summary)](file:///d:/NewaccNet/NewaccNet.Wpf/System/Reports/DiaryReportGridFactory.cs)** dựng **GridControl ẩn trong memory** (không hiển thị UI), đủ 8 cột + 3 TotalSummary (Count, Sum Debit, Sum Credit). Sau đó gọi `ReportManager.PrintGridControl(memGrid, TITLE)` → **Mở DevExpress Print Preview TRỰC TIẾP (Landscape)**. Tiêu đề = "SỔ NHẬT KÝ KẾ TOÁN (Từ ... Đến ...) | Số CT | Tổng Nợ | Tổng Có".
    - Print Preview window có đầy đủ Zoom, In, Xuất PDF/XLS/DOCX, Search, Page Setup.
  - **Đã XÓA DiaryReportWindow cũ (nguyên nhân crash):** Cụm XAML đặt `<dxe:DateEdit>` thẳng vào `<dxb:ToolBarControl>` gây XamlParseException `'DateEdit' is not of type 'IBarItem'` — Pattern DevExpress ToolBarControl yêu cầu dùng `BarEditItem` bọc bên trong (hoặc dùng StackPanel thông thường). Đã loại bỏ hoàn toàn thay vì fix vì user yêu cầu KHÔNG hiện Grid xem trước nữa.
  - **Build kiểm tra:** `dotnet build NewaccNet.Wpf.csproj` → ExitCode=0, 0 Error(s), 128 Warning(s) (nullable/obsolete cũ của hệ thống, không do code mới). ✅

- **MỚI: Tạo Danh mục Tài sản Cố định (AssetEntity) - Popup Editor Pattern (ListView ReadOnly + Editor Window):**
  - **AssetListView.xaml** (File: [AssetListView.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/AssetListView.xaml)):
    - Grid ReadOnly (`AllowEditing="False"`), `ShowAutoFilterRow=False`, ShowSearchPanelMode=Always, ShowTotalSummary=True
    - Toolbar 5 nút chuẩn: Thêm mới → Sửa → Xóa → In → Làm mới. Right-click Menu (RowCellMenuCustomizations) tương ứng.
    - 9 cột chính (theo mapping AssetEntity): AssetId (Mã TS), AssetHandle (Bút tệp/HS), AssetName (Tên TS), Unit (ĐVT), Qty (SL N2), Price (Nguyên giá N0), **Thành tiền (UnboundColumn = Qty \* Price tóm tắt trên Grid)**, Timeusing (TGSD theo tháng), Date (Ngày nhập), Country (Nước SX).
    - TotalSummary: Tổng thành tiền (Sum cột Unbound TotalAmount).
    - `RowDoubleClick` → gọi EditSelected.
  - **AssetListView.xaml.cs** (File: [AssetListView.xaml.cs](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/AssetListView.xaml.cs)):
    - Kế thừa `BaseDictionaryWindow`. `LoadData()` chuẩn LLBLGen: `EntityCollection<AssetEntity>`, `FetchEntityCollection(items, null)` với `ShowLoading("Đang tải danh mục Tài sản cố định...")`.
    - `MenuAdd_ItemClick` → OpenEditor(new AssetEntity, true)
    - `MenuEdit_ItemClick / RowDoubleClick` → `EditSelected()`: trước khi mở Editor gọi `FetchEntity(selectedItem.AssetId)` để lấy entity "detached sạch" (không dùng selectedItem trực tiếp, vì khi user cancel sẽ không bị dirty) → OpenEditor(existing, false).
    - `MenuDelete_ItemClick` → `DeleteSelected()`: Confirm Yes/No `MessageBox.Show($"Bạn có chắc muốn xóa tài sản '{selectedItem.AssetId} - {selectedItem.AssetName}'?" )` → `adapter.DeleteEntity(selectedItem)` + Remove khỏi DataSource.
    - `MenuRefresh_ItemClick` → gọi `LoadData()` (reload lại).
    - `MenuPrint_ItemClick` → `PrintRecord()`: `ReportManager.PrintGridControl(gridControl, "DANH MỤC TÀI SẢN CỐ ĐỊNH")`.
    - `OpenEditor(entity, isNew)`: `ShowDialog() == true` → `adapter.SaveEntity(entity, true, false)` → **Gọi lại `LoadData()`** để reload toàn bộ (như user yêu cầu "Khi save vào thì nó reload lại dữ liệu thôi").
  - **AssetEditorWindow.xaml** (File: [AssetEditorWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/AssetEditorWindow.xaml)):
    - Kế thừa `BaseWindow` (popup dialog Modal), Width=780, SizeToContent=Height, NoResize.
    - UI Layout: `<Border>` bao bọc `<dxlc:LayoutControl>` với `GroupBox Header="Thông tin chung tài sản"`.
    - 4 hàng thông tin (9 trường theo AssetEntity):
      1. Mã tài sản (\*) + Bút tệp/HS
      2. Tên tài sản (\*) (ở hàng riêng, rộng 2 cột)
      3. Đơn vị tính + Ngày nhập/Bắt đầu SD (DateEdit)
      4. Số lượng ban đầu (N2) + Thời gian SD (Tháng, N0)
      5. Nguyên giá (N0) + Nước SX/Nguồn gốc
    - Nút dưới phải: Lưu (Ctrl+S) / Hủy (Icon action_save / action_delete theo pattern InventoryItemEditorWindow).
  - **AssetEditorWindow.xaml.cs** (File: [AssetEditorWindow.xaml.cs](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/AssetEditorWindow.xaml.cs)):
    - Constructor nhận `(AssetEntity entity, bool isNew)`. Title động: "Thêm Mới Tài Sản Cố Định" / "Sửa Tài Sản: {AssetId}".
    - BindData: Nếu `!_isNew` → `txtAssetId.IsReadOnly = true` (không cho đổi mã khi sửa).
    - BtnSave_Click: Validate `AssetId trống` / `AssetName trống` → chặn save cảnh báo.
    - Mapping dữ liệu ngược về \_entity: Cắt Trim() các chuỗi, Parse double Qty/Price/Timeusing (null nếu empty). `(DateTime?)dtDate.EditValue`.
    - Trả về `DialogResult = true` + Close (Lưu thực tế xử lý ở ListView OpenEditor để thống nhất pattern InventoryItem).
  - **Gắn lên MainWindow Ribbon (Đã hoạt động):**
    - [MainWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml#L232) Nút "Tài sản" (nhóm TSCĐ, RequiredMask=128) → thêm `ItemClick="BtnCategoryAsset_ItemClick"`.
    - [MainWindow.xaml.cs](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml.cs#L173) Handler: `new AssetListView()` + `LoadData()` + `Show()`.
  - _Ghi chú thiết kế:_ Hiện tại Editor chỉ có Tab Thông tin chung (9 trường Master). **Luân chuyển Tài sản (AssetTracking 1-N, 14 trường) chưa được thêm vào Tab Tracking** theo hình ảnh Access cũ → để làm ở Backlog sau (để user test Master trước). Khi mở Editor → asset lookup của AssetDetailWindow (System/Voucher/Details/) giờ đã có dữ liệu để chọn.
  - **Build kiểm tra:** `dotnet build NewaccNet.Wpf.csproj` → ExitCode=0, 0 Error(s) ✅.

- **MỚI: Tạo Danh mục Bộ phận / Phòng ban (DepartmentEntity) - Inline Edit Pattern:**
  - [DepartmentListView.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/DepartmentListView.xaml): 2 cột `Deptid` (Width=180, FixedWidth), `DeptName` (Width=\*). 4 nút Lưu → Xóa → In → Làm mới. `ShowAutoFilterRow=False`, `NewItemRowPosition=Bottom`, Right-click Menu (Xóa, In).
  - [DepartmentListView.xaml.cs](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/DepartmentListView.xaml.cs): Pattern y hệt SourceListView: `LoadData()` Fetch DepartmentEntity collection; `TableView_RowUpdated` auto-save từng dòng; `MenuSave_ItemClick` CommitEditing + duyệt toàn bộ dirty/new SaveEntity; `DeleteSelected` confirm trước khi xóa; `PrintRecord` in "DANH SÁCH BỘ PHẬN / PHÒNG BAN". Validate Deptid trống → cảnh báo chặn Save.
  - **Gắn lên MainWindow Ribbon (Đã hoạt động):**
    - [MainWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml#L95) Nút "Phòng ban" (nhóm Kế toán, RequiredMask=8 (KTTH)) → thêm `ItemClick="BtnCategoryDepartment_ItemClick"`.
    - [MainWindow.xaml.cs](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml.cs#L166) Handler: `new DepartmentListView()` + `LoadData()` + `Show()`.
  - _Ghi chú:_ Department (Deptid, DeptName) là dữ liệu tham chiếu cho `AssetTracking.Deptid` (Luân chuyển tài sản theo bộ phận) → sẵn sàng dùng khi làm Asset Editor.

- **Rà soát toàn bộ 16 Form Danh mục (Ribbon Tab "Danh mục"):**
  - **Bỏ Row Filter (ShowAutoFilterRow) trên 9 Form, giữ Header Filter (implicit của DevExpress):**
    - PartnerListView.xaml (Đối tượng CN)
    - DebtTypeListView.xaml (Nội dung CN)
    - DebtReasonListView.xaml (Lý do CN)
    - CategoryListView.xaml (Nhóm vật tư)
    - InventoryItemListView.xaml (Danh mục vật tư)
    - TaxRateListView.xaml (Thuế suất)
    - CurrencyListView.xaml (Loại tiền)
    - CostObjectListView.xaml (Đối tượng CP)
    - CostElementListView.xaml (Yếu tố CP)
  - _7 Form còn lại đã đặt ShowAutoFilterRow="False" từ trước (ChartOfAccount dùng TreeList, Warehouse, ExchangeRate, Source, Reason, StockType, Stock)._
  - **Bổ sung nút "Lưu" tường minh trên Toolbar + MenuSave_ItemClick handler cho 3 Form Inline Edit thiếu (hôm trước chỉ có RowUpdated auto-save):**
    - [CategoryListView](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/CategoryListView.xaml) (Nhóm vật tư) → Thêm nút Lưu đầu Toolbar + `CommitEditing()` + MessageBox thông báo.
    - [TaxRateListView](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/TaxRateListView.xaml) (Thuế suất) → Tương tự.
    - [CurrencyListView](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory/CurrencyListView.xaml) (Tiền tệ) → Tương tự.
  - _Kết quả:_ Tất cả 9 Form Inline Edit (AllowEditing=True) đều có đủ 4 nút Lưu / Xóa / In / Làm mới trên Toolbar.
- **Công nợ (trước đó):** Đối tượng (`PartnerEntity`), Nội dung công nợ (`DebtTypeEntity`), Lý do công nợ (`DebtReasonEntity`).
- **Chi phí (trước đó):** Đối tượng CP (`CostObjectEntity`), Yếu tố CP (`CostElementEntity`).
- **Tài sản (trước đó):** Nguồn tài sản (`SourceEntity`), Lý do tăng giảm (`ReasonEntity`). Đã áp dụng quy tắc Edit Inline.

## 3. Kế hoạch công việc tiếp theo (Làm "cuốn chiếu" tuần tự)

**A. Phân hệ Chứng khoán / Cổ phiếu (Hoàn thành):**

1. **Loại hình cổ phiếu** (StockTypeEntity): Lưới Edit Inline (NewItemRowPosition="Bottom").
2. **Danh mục cổ phiếu** (StockEntity): Lưới Edit Inline (NewItemRowPosition="Bottom"), dùng Dropdown cho Stocktypeid.

**A. Phân hệ Ngoại tệ (Hoàn thành):**

1. **Tiền tệ** (CurrencyEntity): Lưới Edit Inline (NewItemRowPosition="Bottom").
2. **Tỷ giá** (ExchangeRateEntity): Lưới Edit Inline (NewItemRowPosition="Bottom"), dùng Dropdown cho CurrencyId.

**A. Phân hệ Vật tư (Hoàn thành):**

1. **Kho hàng** (Warehouse): Form inline edit (NewItemRowPosition="Bottom").
2. Sắp xếp Ribbon theo thứ tự: **Nhóm vật tư -> Danh mục vật tư -> Thuế suất**.
3. **Thuế suất** (`Taxrate`): Form inline edit (`NewItemRowPosition="Bottom"`).
4. **Nhóm vật tư** (`Category`): Form inline edit (`NewItemRowPosition="Bottom"`). Cột `TaxrateID` hiển thị dưới dạng Dropdown (ComboBox) chọn Thuế suất.
5. **Danh mục vật tư** (`InventoryItem`): Form chuẩn có Popup Editor (do nhiều trường thông tin).
6. Áp dụng quy tắc `NewItemRowPosition="Bottom"` ngược lại cho Nguồn TS và Lý do TS.

**A. Phân hệ Kế toán tổng hợp (Mới - 28/09/2026):**

1. ✅ **Bộ phận / Phòng ban (DepartmentEntity)**: Đã hoàn thành Form Inline Edit. 2 trường: Deptid (mã, PK) + DeptName (tên). Gắn lên Ribbon "Phòng ban" nhóm Kế toán (RequiredMask=8 - KTTH). _Ghi chú: Dữ liệu này dùng làm Lookup cho AssetTracking.Deptid (Luân chuyển TSCĐ theo bộ phận)._
2. ❌ **Nguồn Chứng từ (VoucherSource? / SourceEntity?)**: Trên Ribbon "Nguồn CT" (nhóm Kế toán, RequiredMask=8) VẪN CHƯA có event / form. _Lưu ý có thể trùng Entity "Source" của nhóm TSCĐ - cần xem schema cũ / hỏi user để quyết định tách bảng hay dùng chung._

**A. Phân hệ Tài sản cố định (TSCĐ - PRIORITY #1 TIẾP THEO):**

1. ✅ **Nguồn tài sản** (SourceEntity): Đã hoàn thành Inline Edit.
2. ✅ **Lý do tăng giảm** (ReasonEntity): Đã hoàn thành Inline Edit.
3. ❌ **Danh mục TÀI SẢN (AssetEntity + AssetTracking Detail)**: Đang chờ làm - Thiết kế đã thỏa thuận (Section 4 bên dưới).
   - _Tại sao quan trọng nhất?_ Form [AssetDetailWindow (trong Voucher/Details)](file:///d:/NewaccNet/NewaccNet.Wpf/System/Voucher/Details/AssetDetailWindow.xaml#L34-L48) đang có `lookupAsset.ItemsSource` tải AssetEntity - KHÔNG CÓ DỮ LIỆU NÀO nếu chưa nhập danh mục TSCĐ!

**B. Các phân hệ chờ xử lý:**

- ❌ Kho & Vật tư: Giá vật tư (MaterialPrice) - Ribbon chưa có event.
- ❌ Nghiệp vụ: Tất cả các Form Chứng từ (JournalVoucher).
- ❌ Thiết kế & Báo cáo tài chính: B01-DN (CĐKT), B02-DN (KQKD), B03-DN (LCTT), Sổ cái, Nhật ký, Cân đối PS.

## 4. Backlog (TODO Agent kế nhiệm)

### 4.1 🔴 PRIORITY #1: Tạo Danh mục TÀI SẢN (Asset) + Editor Tab Luân chuyển

_Thiết kế đã thỏa thuận user 28/09/2026:_

- **[AssetListView](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory) (Danh sách xem)**:
  - Pattern giống `PartnerListView`: **AllowEditing=False** (không sửa trực tiếp trên Grid).
  - **Toolbar 3 nút**: Xóa → In → Làm mới (không cần nút Lưu vì Save ở Editor popup).
  - **Cột Grid**: AssetId (Mã TS), AssetHandle (Bút tệp), AssetName (Tên), Date (Ngày nhập), Unit (ĐVT), Qty, Price (Nguyên giá), Timeusing (Thời gian TH tháng), Country (Nước SX).
  - **TotalSummary**: Tổng nguyên giá = Sum(Qty \* Price) (Unbound).
  - **DoubleClick / ContextMenu**: "Thêm tài sản", "Sửa tài sản" → Mở **AssetEditorWindow** dialog.
- **[AssetEditorWindow](file:///d:/NewaccNet/NewaccNet.Wpf/System/Directory) (Popup Master-Detail)**:
  - Base: `ThemedWindow` + `SizeToContent` + `Owner` (không cho taskbar). Title: "Thêm / Sửa Tài sản cố định".
  - **Layout Tab 1 - Thông tin tài sản (Master - 9 trường AssetEntity)**: LayoutControl 2 cột nhập liệu.
  - **Layout Tab 2 - 💼 Lịch sử Luân chuyển (Detail - AssetTrackings)**: `GridControl Inline Edit` giống SourceListView 100%.
    - `NewItemRowPosition="Bottom"`, `ShowAutoFilterRow="False"`, 4 nút Lưu / Xóa / In / Làm mới trên Toolbar nội bộ của Tab.
    - **Lookup bắt buộc (ComboBoxEditSettings / LookUpEditSettings)**:
      - `Deptid` → DepartmentEntity (Bộ phận vừa tạo xong ✅)
      - `ExobjectId` → CostObjectEntity (Đối tượng CP có sẵn ✅)
      - `AsaccountId` → ChartOfAccount (Tài khoản nhập TS ✅)
      - `ExaccountId` → ChartOfAccount (Tài khoản xuất TS ✅)
    - **Cột Grid 14 field**: Begindate / Enddate / Deptid (Lookup BP) / ExobjectId (Lookup ĐTCP) / AsaccountId (Lookup TK nhập) / ExaccountId (Lookup TK xuất) / Qty / Depprice (GT KH kỳ) / Deprate (%) / Remainvalue (GT còn lại) / Timeusing (TG đã dùng) / Country.
  - **Cơ chế Save Recursive (LLBLGen built-in, KHÔNG cần code thủ công từng dòng)**:
    1. Load Asset bằng `adapter.FetchEntity(asset, PrefetchPathAssetTrackings)` (Master + Detail cùng lúc).
    2. User chỉnh sửa Master / Thêm-Sửa-Xóa dòng Tracking trực tiếp trên Grid Tab 2.
    3. Bấm nút "Lưu & Đóng" → `using (var adapter = ...) { adapter.SaveEntity(selectedAsset, true); }` → LLBLGen tự động Save Master + tất cả các dòng Detail INSERT/UPDATE/DELETE trong **một transaction duy nhất**.
  - Hai nút cuối: `Lưu & Đóng` (DialogResult=true) + `Hủy bỏ` (Rollback Fields, DialogResult=false).
- **Gắn lên Ribbon**:
  - [MainWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml#L232) Nút "Tài sản" (nhóm TSCĐ) → Thêm `ItemClick="BtnCategoryAsset_ItemClick" auth:AuthHelper.RequiredMask="128"`.
  - `MainWindow.xaml.cs`: Handler `BtnCategoryAsset_ItemClick` → `new AssetListView()` + `LoadData()` + `Show()`.

### 4.2 Icon chuyên biệt Ribbon (Treo, cần quy trình thử-sai)

- Thay icon chuyên biệt cho Ribbon (hiện đang dùng icon tạm `bo_...`).
