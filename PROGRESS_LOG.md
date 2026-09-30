# NHẬT KÝ TIẾN ĐỘ DỰ ÁN NEWACCNET
> Cập nhật lần cuối: 2026-09-27

---

## PHẦN I: HẠ TẦNG HỆ THỐNG (Hoàn thành)

### 1.1. Kết nối Cơ sở dữ liệu đa nền tảng
- **Adapter Pattern:** `AppDataAccessAdapter` hỗ trợ 3 DBMS: MS Access (OleDb), SQL Server (SqlClient), PostgreSQL (Npgsql).
- **Form cấu hình:** `DbConfigWindow.xaml` cho phép chọn và test kết nối trực quan.
- **Lưu cấu hình:** File JSON (`dbconfig.json`) lưu ở thư mục ứng dụng.

### 1.2. Đăng nhập & Phân quyền (RoleMask)
- **LoginWindow:** Form đăng nhập chuẩn với mã hóa mật khẩu.
- **RoleMask (Bitmask):** Hệ thống phân quyền theo từng bit:
  - Bit 0: Quản trị hệ thống
  - Bit 1: Ngoại tệ
  - Bit 2: Vật tư / Kho
  - Bit 3: Kế toán tổng hợp
  - Bit 4: Công nợ
  - Bit 5: Chứng khoán
  - Bit 6: Chi phí / Giá thành
  - Bit 7: Tài sản cố định
- **AuthHelper (Attached Property):** `auth:AuthHelper.RequiredMask="8"` gán trực tiếp vào XAML để tự động ẩn/hiện nút Ribbon theo quyền user.
- **UserEditorWindow:** Form quản lý người dùng và gán quyền.

### 1.3. ORM & Data Access
- **LLBLGen Pro:** Sử dụng LLBLGen Pro ORM với code-gen entities.
- **Unit of Work (UOW):** Cơ chế lưu đệ quy (recursive save) qua `adapter.SaveEntity(entity, refetchAfterSave: true, recurse: true)`.
- **Prefetch Paths:** Sử dụng PrefetchPath2 để nạp quan hệ cha-con trong 1 lần truy vấn.

---

## PHẦN II: DANH MỤC (Hoàn thành)

### 2.1. Kiến trúc BaseDictionaryWindow
- **Lớp cơ sở trừu tượng:** `BaseDictionaryWindow<TEntity>` cung cấp sẵn CRUD, Grid binding, Search, Print cho tất cả form danh mục.
- **Kế thừa:** Mỗi form danh mục chỉ cần kế thừa và override vài property.

### 2.2. Danh sách các Form Danh mục đã implement

| STT | Danh mục | Entity | Form | Event Handler |
|-----|----------|--------|------|---------------|
| 1 | Hệ thống Tài khoản | ChartOfAccountEntity | AccountWindow | BtnCategoryAccount_ItemClick |
| 2 | Nguồn Chứng từ | *(chưa entity)* | *(chưa form)* | — |
| 3 | Phòng ban | *(chưa entity)* | *(chưa form)* | — |
| 4 | Đối tượng Công nợ | PartnerEntity | PartnerWindow | BtnCategoryPartner_ItemClick |
| 5 | Nội dung Công nợ | DebtTypeEntity | DebtTypeWindow | BtnCategoryDebtType_ItemClick |
| 6 | Lý do Công nợ | DebtReasonEntity | DebtReasonWindow | BtnCategoryDebtReason_ItemClick |
| 7 | Kho hàng | WarehouseEntity | WarehouseWindow | BtnCategoryWarehouse_ItemClick |
| 8 | Nhóm Vật tư | CategoryEntity | CategoryWindow | BtnCategoryCategory_ItemClick |
| 9 | Danh mục Vật tư | InventoryItemEntity | InventoryItemWindow | BtnCategoryInventoryItem_ItemClick |
| 10 | Thuế suất | TaxRateEntity | TaxRateWindow | BtnCategoryTaxRate_ItemClick |
| 11 | Loại tiền | CurrencyEntity | CurrencyWindow | BtnCategoryCurrency_ItemClick |
| 12 | Tỷ giá | ExchangeRateEntity | ExchangeRateWindow | BtnCategoryExchangeRate_ItemClick |
| 13 | Đối tượng Chi phí | CostObjectEntity | CostObjectWindow | BtnCategoryCostObject_ItemClick |
| 14 | Yếu tố Chi phí | CostElementEntity | CostElementWindow | BtnCategoryCostElement_ItemClick |
| 15 | Nguồn Tài sản | SourceEntity | SourceWindow | BtnCategorySource_ItemClick |
| 16 | Lý do Tăng giảm | ReasonEntity | ReasonWindow | BtnCategoryReason_ItemClick |
| 17 | Loại hình Cổ phiếu | StockTypeEntity | StockTypeWindow | BtnCategoryStockType_ItemClick |
| 18 | Danh mục Cổ phiếu | StockEntity | StockWindow | BtnCategoryStock_ItemClick |

---

## PHẦN III: RIBBON & ICON MAPPING (Hoàn thành 2026-09-27)

### 3.1. Cấu trúc Ribbon

```
Tab 1: Nghiệp vụ (IsSelected=True — Tab mặc định)
  └─ Group: NHẬP LIỆU CHỨNG TỪ
       └─ NHẬP CHỨNG TỪ → BtnVoucherTest_ItemClick

Tab 2: Hệ thống
  └─ Group: Quản trị
       ├─ Người dùng → BtnUserManagement_ItemClick
       ├─ Cấu hình DB → BtnConfig_ItemClick
       └─ Khai báo

Tab 3: Danh mục
  ├─ Group: Kế toán (Tài khoản, Nguồn CT, Phòng ban)
  ├─ Group: Công nợ (Đối tượng, Nội dung CN, Lý do CN)
  ├─ Group: Kho & Vật tư (Kho hàng, Nhóm VT, DM Vật tư, Thuế suất, Giá VT)
  ├─ Group: Ngoại tệ (Loại tiền, Tỷ giá)
  ├─ Group: Chi phí (Đối tượng CP, Yếu tố CP)
  ├─ Group: TSCĐ (Tài sản, Nguồn TS, Lý do tăng giảm)
  └─ Group: Chứng khoán (Loại hình CP, Danh mục CP)

Tab 4: Thiết kế (Bảng CĐKT, KQ HĐKD, LCTT)

Tab 5: Báo cáo
  ├─ Group: Tài chính (CĐKT, KQKD, LCTT)
  ├─ Group: Sổ sách (Sổ cái, Cân đối PS, Nhật ký)
  ├─ Group: Công nợ (Tổng hợp CN, Chi tiết CN)
  └─ Group: Kho (Tồn kho, Nhập xuất)
```

### 3.2. Bảng Icon Mapping (DevExpress SVG)

> **Prefix chung:** `pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/`
> **Viết tắt:** `bo/` = `business%20objects/bo_`, `xaf/` = `xaf/`

| Nút | Icon SVG | Ý nghĩa |
|-----|----------|---------|
| **NHẬP CHỨNG TỪ** | `bo/audit.svg` | Sổ bút toán / phiếu kế toán |
| **Người dùng** | `xaf/filtereditor_user.svg` | Biểu tượng người dùng |
| **Cấu hình DB** | `SvgImages/Setup/Properties.svg` (DXImage) | Cài đặt hệ thống |
| **Khai báo** | `bo/organization.svg` | Tổ chức / doanh nghiệp |
| **Tài khoản** | `bo/validation.svg` | Cây kiểm tra / hệ thống TK |
| **Nguồn CT** | `bo/transition.svg` | Quy trình / luồng chứng từ |
| **Phòng ban** | `bo/department.svg` | Cơ cấu phòng ban |
| **Đối tượng (CN)** | `bo/contact.svg` | Đối tác / liên hệ |
| **Nội dung CN** | `bo/customer.svg` | Phân loại khách hàng |
| **Lý do CN** | `bo/note.svg` | Ghi chú / lý do |
| **Kho hàng** | `bo/localization.svg` | Địa điểm kho |
| **Nhóm Vật tư** | `bo/category.svg` | Phân nhóm |
| **DM Vật tư** | `bo/product.svg` | Sản phẩm / hàng hóa |
| **Thuế suất** | `bo/price.svg` | Giá / thuế |
| **Giá Vật tư** | `bo/sale.svg` | Bảng giá mua bán |
| **Loại tiền** | `bo/country.svg` | Tiền tệ quốc gia |
| **Tỷ giá** | `bo/attention.svg` | Biến động tỷ giá |
| **Đối tượng CP** | `bo/position.svg` | Vị trí tập hợp CP |
| **Yếu tố CP** | `bo/appearance.svg` | Phân tích yếu tố |
| **Tài sản** | `bo/state.svg` | Trạng thái tài sản |
| **Nguồn Tài sản** | `bo/audit.svg` | Nguồn hình thành |
| **Lý do tăng giảm** | `bo/transition.svg` | Biến động tăng giảm |
| **Loại hình CP** | `bo/category.svg` | Phân loại chứng khoán |
| **Danh mục CP** | `bo/security.svg` | Chứng khoán |
| **Bảng CĐKT** | `xaf/action_report_showdesigner.svg` | Báo cáo thiết kế |
| **KQ HĐKD** | `bo/pivotchart.svg` | Biểu đồ kết quả |
| **LCTT** | `bo/sale.svg` | Lưu chuyển tiền tệ |
| **CĐKT (in)** | `xaf/action_report_showdesigner.svg` | In bảng CĐKT |
| **KQKD (in)** | `bo/pivotchart.svg` | In kết quả HĐKD |
| **LCTT (in)** | `bo/sale.svg` | In lưu chuyển tiền |
| **Sổ cái** | `bo/list.svg` | Danh sách sổ cái |
| **Cân đối PS** | `bo/validation.svg` | Kiểm tra cân đối |
| **Nhật ký** | `bo/calendar.svg` | Nhật ký theo ngày |
| **Tổng hợp CN** | `bo/customer.svg` | Báo cáo công nợ TH |
| **Chi tiết CN** | `bo/appearance.svg` | Báo cáo công nợ CT |
| **Tồn kho** | `bo/localization.svg` | Vị trí tồn kho |
| **Nhập xuất** | `bo/transition.svg` | Luồng nhập xuất |
| **Window Icon** | `xaf/action_report_showdesigner.svg` | Icon ứng dụng |

---

## PHẦN IV: FORM NHẬP LIỆU CHỨNG TỪ (Hoàn thành phần cơ bản — 2026-09-26)

### 4.1. Kiến trúc đã hoàn thành

#### Files chính:
- `VoucherEntryWindow.xaml` + `.xaml.cs` — Form nhập liệu chứng từ chính
- `VoucherListView.xaml` + `.xaml.cs` — Danh sách chứng từ (entry point)
- `AccountLookUpEdit.cs` — Custom control cho dropdown tài khoản

#### Cơ chế Master-Detail (Dual Grid):
```
┌──────────────────────┬──────────────────────┐
│   LƯỚI TRÁI (Master) │  LƯỚI PHẢI (Detail)  │
│   ParentId = NULL     │  ParentId != NULL    │
│                       │  (con của dòng đang  │
│   _masterEntries      │   chọn bên trái)     │
│   (ObservableCollection)│ selectedMaster      │
│                       │   .SubEntries        │
└──────────────────────┴──────────────────────┘
```

#### Luồng dữ liệu:
1. **Tạo mới:** User gõ dòng Master bên Trái → Tab → Tự động nhảy sang Phải, tạo dòng Detail.
2. **Auto Dbcr:** Dòng Detail tự đảo chiều Nợ/Có so với dòng Master.
3. **Auto Amount:** Dòng Detail đầu tiên = Amount của Master. Dòng thứ 2 trở đi = Amount Master − Σ Amount các dòng con trước đó.
4. **Save:** `adapter.SaveEntity(_currentVoucher, refetchAfterSave: true, recurse: true)` → UOW tự cấp ID cho Master, gán ParentId cho Detail rồi lưu cả cụm.
5. **Load:** `RefreshVoucherUI()` quét danh sách phẳng, tự phân loại Master/Detail, link SubEntries.

### 4.2. Các tính năng đã implement

| # | Tính năng | Trạng thái |
|---|-----------|------------|
| 1 | Dual Grid (Trái = Master, Phải = Detail) | ✅ Hoàn thành |
| 2 | Object Graph Binding (SubEntries / ParentEntry) | ✅ Hoàn thành |
| 3 | Deep Graph Save (UOW recursive) | ✅ Hoàn thành |
| 4 | Tab từ Lưới Trái nhảy sang Lưới Phải | ✅ Hoàn thành |
| 5 | Auto đảo chiều Nợ/Có (Dbcr) | ✅ Hoàn thành |
| 6 | Auto bù trừ Số tiền (Amount) | ✅ Hoàn thành |
| 7 | Real-time Balance Tracking (footer) | ✅ Hoàn thành |
| 8 | Hard-block Save khi lệch cân đối | ✅ Hoàn thành |
| 9 | AccountLookUpEdit (chỉ hiện tài khoản lá) | ✅ Hoàn thành |
| 10 | VoucherListView (danh sách chứng từ) | ✅ Hoàn thành |
| 11 | Chỉ reload danh sách khi thực sự Save | ✅ Hoàn thành |
| 12 | RefreshVoucherUI() — hàm vẽ lại toàn bộ | ✅ Hoàn thành |
| 13 | Hiển thị Id, ParentId trên Grid | ✅ Hoàn thành |
| 14 | Hard-block New/Navigate khi lệch | 🔜 Chưa implement |
| 15 | Dirty tracking + hỏi trước khi thoát | 🔜 Chưa implement |
| 16 | Tắt nút Close mặc định | 🔜 Chưa implement |

### 4.3. Hàm RefreshVoucherUI() — "Máy vẽ lại"
```csharp
public void RefreshVoucherUI()
{
    // 1. Ép UI chốt dữ liệu đang gõ dở
    // 2. Xóa _masterEntries, reset gridRight
    // 3. Quét JournalEntries phẳng:
    //    - ParentId == null && ParentEntry == null → là Master → add vào _masterEntries
    //    - Tìm con ruột → link vào entry.SubEntries
    // 4. Phục hồi dòng đang chọn
    // 5. CalculateBalance()
}
```
> **Ý nghĩa chiến lược:** Khi Form Phụ (Voucher Generator) thao tác sinh/đảo chiều dữ liệu ngầm trong RAM, chỉ cần gọi `RefreshVoucherUI()` 1 lần → toàn bộ 2 lưới tự vẽ lại đúng cấu trúc Master-Detail mà không cần truy vấn DB.

---

## PHẦN V: BƯỚC TIẾP THEO (TODO)

### 5.1. Hoàn thiện Form Nhập liệu
- [ ] Hard-block New/Navigate khi chứng từ lệch cân đối
- [ ] Dirty tracking + hỏi trước khi thoát (ShowCloseButton=False)
- [ ] Nâng cấp thanh điều hướng (First, Prev, Next, Last) cho nhập liệu lô

### 5.2. Form Phụ — Voucher Generator (Ưu tiên cao)
- [ ] **Thiết kế Form Bán hàng:** Nhập Vật tư, Số lượng, Đơn giá → Tự sinh định khoản:
  - Nợ 632 (Giá vốn) / Có 156 (Hàng tồn kho)
  - Nợ 131 (Phải thu) / Có 511 (Doanh thu) + Có 3331 (Thuế GTGT)
- [ ] **Cơ chế đảo chiều:** Form phụ sinh dữ liệu → `RefreshVoucherUI()` → Master-Detail tự sắp xếp lại
- [ ] **Giá vật tư Engine:** Tính giá vốn bình quân tự động khi xuất kho

### 5.3. Báo cáo & Sổ sách
- [ ] Sổ cái tài khoản
- [ ] Bảng cân đối phát sinh
- [ ] Báo cáo tài chính (CĐKT, KQKD, LCTT)

---

## PHẦN VI: GHI CHÚ KỸ THUẬT QUAN TRỌNG

### 6.1. LLBLGen — Bài học kinh nghiệm
- **ChartOfAccountEntity:** Primary key là `AccountId` (string), KHÔNG phải `Id` (int).
- **SubEntries prefetch:** KHÔNG dùng `.SubPath.Add(PrefetchPathSubEntries)` khi cần binding UI vì nó tạo bản sao Object. Thay vào đó, dùng vòng lặp thủ công để link `entry.SubEntries.Add(child)`.
- **Recursive Save:** `adapter.SaveEntity(voucher, true, true)` sẽ tự động: Insert Master → Lấy Id → Gán ParentId cho Detail → Insert Detail. Tất cả trong 1 Transaction.

### 6.2. DevExpress — Bài học kinh nghiệm
- **SVG Icon paths cực kỳ nhạy cảm.** Luôn dùng prefix `svgimages/business%20objects/bo_xxx.svg` hoặc `svgimages/xaf/action_xxx.svg`. Tuyệt đối không tự chế path.
- **GridViewBase.FocusedColumn** đã bị obsolete → Dùng `DataControlBase.CurrentColumn` thay thế.
- **Binding ParentId trong Grid:** Dùng `Binding="{Binding ParentId}"` thay vì chỉ `FieldName="ParentId"` để đảm bảo UI update real-time.
- **NewItemRowPosition="Bottom":** Đặt dòng thêm mới ở cuối grid.

### 6.3. VoucherListView — Tối ưu hiệu năng
- **Chỉ query bảng JournalVoucher** (không prefetch JournalEntries).
- **Chỉ reload khi thực sự Save:** Cờ `HasSaved` trên `VoucherEntryWindow` kiểm soát việc này.

## PHẦN VII: CHI TIẾT CÔNG NỢ & NGOẠI TỆ (2026-09-27)

### 7.1. Kiến trúc Bộ định tuyến (Router) và Thư viện Dữ liệu Tham chiếu (Reference Data)
- **AccountLookUpEdit Router:** Khởi tạo bộ định tuyến độc lập trên Dropdown Tài khoản. Dựa vào CategoryId (A, B...), tự động bật các Form chi tiết tương ứng (Công nợ, Vật tư, v.v.).
- **Tư duy "Chi tiết làm gốc" (Source of Truth):**
  - Không bắt lỗi kiểm tra cân đối ngay tại Form Phụ.
  - Sau khi người dùng chốt số liệu ở Form Phụ, Sum(Amount) của chi tiết sẽ tự động **Ghi đè** ngược lên dòng Master tương ứng.
  - Khi đóng Form Phụ, tự động kích hoạt oucherWindow.CalculateBalance() để cập nhật cảnh báo Cân đối của toàn bộ Voucher một cách real-time.

### 7.2. Các Form Chi tiết đã hoàn thành
- **DebtDetailWindow (Chi tiết Công nợ):**
  - Giao diện lưới DataGrid.
  - Gom các nút tùy chọn "Cam kết" (CK) và "Ngoại tệ" ($) vào một cột "Mở rộng" gọn gàng ở cuối dòng, dùng SVG Icons chuẩn của DevExpress.
- **CurrencyLiabilityWindow (Chi tiết Ngoại tệ 1-1):**
  - Thiết kế Form nhập liệu phẳng, nền trắng sạch sẽ, không dùng thanh cuộn DataNavigator (do quan hệ 1-1).
  - Tự động đồng bộ LineId và tự tính quy đổi cập nhật ngược lại số tiền của dòng chi tiết.
- **LiabilityDueWindow (Thư viện Thời hạn & Lãi suất):**
  - Không dùng khóa ngoại cứng (No Foreign Key constraints) đối với các bảng giao dịch để trở thành một "Thư viện" dùng chung (quản trị).
  - Truy vấn 2 khóa độc lập: PartnerId (Đối tượng) và DebtTypeId (Nội dung) qua RelationPredicateBucket.
  - Có cơ chế _isDirty (Dirty Tracking) hỏi lưu trước khi thoát.

### 7.3. Fix lỗi kỹ thuật cốt lõi
- **Fix "Dòng tàng hình" (Disappearing Row) trên Grid Master:** Bọc lệnh ẩn NewItemRowPosition = None vào Dispatcher.BeginInvoke để tránh xung đột với tiến trình Commit Virtualization của DevExpress.
- **Fix LLBLGen PrefetchPath:** Bổ sung DebtDetails và CurrencyLiabilityLine vào Graph PrefetchPath của LoadVoucher để tải lại đầy đủ Cây dữ liệu khi xem lại chứng từ cũ.

---

## PHẦN VIII: BƯỚC TIẾP THEO (TODO - SẮP TỚI)

### 8.1. UI/UX cho MainWindow
- Thiết lập Background Image (Ảnh nền) cho Main Form, cho phép load động từ thư mục Images của phần mềm. Sử dụng Stretch="UniformToFill".
- Bố trí các nút thao tác nhanh (Quick Access / Dashboard) nổi đè lên trên ảnh nền.

### 8.2. Hệ thống Form Chi tiết (Detail Forms) - Giai đoạn 2
- **Chi tiết Chi phí:** (Form phụ tương tự Công nợ) để bóc tách phí.
- **Chi tiết Vật tư / Bán hàng (Category 'B'):** Module phức tạp nhất đòi hỏi Voucher Generator (Tự động hạch toán Giá vốn + Kho + Thuế + Doanh thu từ Form nhập liệu).

---

## PHẦN IX: SỔ NHẬT KÝ & BÁO CÁO (HOÀN THÀNH 29/09/2026)

### 9.1. Sổ Nhật Ký (Diary Report) — Module Báo cáo đầu tiên hoạt động

#### Files chính đã tạo:
- [DiaryCalculator.cs](file:///d:/NewaccNet/NewaccNet.Wpf/System/Reports/Calculators/DiaryCalculator.cs) — Class tính toán + DTO
- [DiaryReportWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/System/Reports/DiaryReportWindow.xaml) + `.xaml.cs` — UI xem Nhật ký
- **Update:** [MainWindow.xaml](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml#L331) (gắn ItemClick) + [MainWindow.xaml.cs](file:///d:/NewaccNet/NewaccNet.Wpf/MainWindow.xaml.cs#L215) (handler)

#### Mapping Legacy Access (tham chiếu basequery.txt):
| Access (Bảng cũ) | LLBLGen (Bảng mới) | Trường tương ứng |
|------------------|---------------------|-------------------|
| `Bills` | `JournalVoucherEntity` | `[Bill ID]`→Id, `[Bill No]`→VoucherNo, `[Bill Date]`→VoucherDate, `Contents`→Contents, `BookFlag`→Bookflag |
| `[Transaction List]` | `JournalEntryEntity` | `[Account ID]`→AccountId, `Amount`→Amount, `DBCR`→Dbcr (1=Nợ, -1=Có) |
| `[Account List]` | `ChartOfAccountEntity` | `[Account ID]`→AccountId, `[Account Name]`→AccountName |

#### Truy vấn DiaryQueryBase (LINQ to LLBLGen qua LinqMetaData):
```csharp
from jv in JournalVoucher
join je in JournalEntry    on jv.Id equals je.JournalVoucherId
join coa in ChartOfAccount on je.AccountId equals coa.AccountId
where  jv.VoucherDate Between FromDate And ToDate
where  If(onlyBooked) jv.Bookflag == true
orderby jv.VoucherDate, jv.VoucherNo, jv.Id, je.Dbcr descending
select new DiaryReportModel { VoucherId, VoucherDate, ... , Debit, Credit }
```

#### Tính Nợ / Có (tương đương 2 hàm Access VBA):
```csharp
raw  = je.Dbcr * je.Amount           // Dbcr=1 * 1tr → Nợ 1tr; Dbcr=-1 * 1tr → Có 1tr
Debit  = raw > 0 ? raw : 0           // DebitValue()  Access
Credit = raw <= 0 ? -raw : 0         // CreditValue() Access
```

#### UI Nhật ký (DiaryReportWindow.xaml):
- **Row 0 / ToolbarControl:**
  - 2 `DateEdit` (Từ ngày / Đến ngày — mặc định: đầu tháng → hôm nay)
  - `BarCheckItem` "Chỉ CT đã ghi sổ (Bookflag=1)" → filter linh hoạt
  - 5 Nút: **Lọc (F5)** → Làm mới bộ lọc → **In (Ctrl+P)** → Bù cột → **Đóng (Esc)**
- **Row 1 / Tổng quan:** 4 thẻ nhanh = Số CT / Số bút toán / Tổng Nợ / Tổng Có
- **Row 2 / GridControl:** 10 cột → Ngày CT (Fixed Left), Số CT (Fixed Left), Nội dung, Mã TK, Tên TK, Ghi sổ (Checkbox), **Nợ/Có (Fixed Right)**.
  - Có **GroupPanel** (kéo thả Số CT để group).
  - `TotalSummary` + `GroupSummary` (Sum Nợ, Sum Có).
  - `ShowSearchPanelMode=Always` (tìm nhanh), `ShowAutoFilterRow=False` (theo quy tắc UI).
- **Row 3 / Footer Check:** Hiển thị màu XANH "✅ CÂN ĐỐI" nếu `|SumNợ - SumCó| < 0.005`; nếu không sẽ hiện màu ĐỎ "❌ LỆCH" và hiển thị độ lệch.

#### Shortcut keys đã bind:
- **F5:** Lọc lại dữ liệu Nhật ký theo kỳ mới
- **Ctrl+P:** In lưới qua `ReportManager.PrintGridControl`
- **Esc:** Đóng cửa sổ Nhật ký

### 9.2. Kế hoạch sổ sách tiếp theo (sắp xếp độ khó tăng dần):
| Độ khó | Báo cáo / Sổ sách | Nguồn dữ liệu | Formula chính |
|--------|-------------------|---------------|---------------|
| 🟢 Dễ (1) | **Bảng Cân đối Phát sinh (Trial Balance)** | `JournalEntry` (group by AccountId) | Opening + ΣDebit − ΣCredit = Closing |
| 🟢 Dễ (1) | **Sổ Cái (General Ledger) theo 1 TK** | `JournalEntry` WHERE AccountId = ? | Opening + Roll-forward từng dòng theo ngày |
| 🟡 TB (2) | **Báo cáo Công nợ** | `DebtLedger` + `JournalEntry` (lọc Category=C) | Group by PartnerId + Detail theo ngày |
| 🟡 TB (2) | **Báo cáo Kho Nhập Xuất Tồn** | `InventoryLedger` / `InventoryVoucherLine` | Opening + Nhập − Xuất = Tồn Cuối |
| 🔴 KHÓ (3) | **Báo cáo BCTC (B01, B02, B03)** | `GeneralLedger` + `ReportFormula` (thiết kế từ Designer) | `BalanceSheetCalculator` pattern đã có sẵn → fill dữ liệu |

### 9.3. Check cân đối Nhật ký — "Nguyên tắc vàng"
> Luôn kiểm tra `Sum(Debit) == Sum(Credit)` ở Footer **mọi lúc**. Nếu lệch (đỏ) → dữ liệu vào sai (có thể do test data nhập thiếu 1 bên, hoặc DB cũ migrate thiếu dòng). Không bao giờ xuất báo cáo / in Nhật ký nếu Footer còn ĐỎ.
