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

### 10. Báo Cáo Nghĩa Vụ Thuế (ObligationTax) & Tối Ưu Hóa Tương Lai
- **Thiết kế hiện tại**: Sử dụng kiến trúc Data-Driven & Snapshot JSON. Hệ thống gom dữ liệu theo ColIndex và Method để tính toán BeginTax, DebitTax, CreditTax và chốt EndTax. Dữ liệu các kỳ được chuyển tiếp thông qua Snapshot JSON giúp cắt đứt sự phụ thuộc chéo giữa các Database / Năm tài chính.
- **Dữ liệu JSON xuất ra**: Chỉ lưu Code (Mã chỉ tiêu) và EndTax (Số dư cuối kỳ). Cấu trúc này tối giản, đủ để merge lại thành BeginTax hoặc AccTax (Lũy kế) ở kỳ báo cáo tiếp theo thông qua cơ chế Deserialize linh hoạt.
- **Ghi chú tối ưu hóa (Technical Debt)**: 
  - Hiện tại hàm FetchJournalEntryPairs kéo toàn bộ JournalEntry trong khoảng thời gian romDate -> 	oDate về RAM rồi dùng LINQ in-memory filtering.
  - Tương lai (khi có hàng triệu bút toán), nếu hiệu năng bị ảnh hưởng: Nâng cấp thuật toán bằng cách đọc trước danh sách AccountId từ AccountTaxObligations, sau đó đẩy xuống database thành mệnh đề IN hoặc LIKE trong SQL query thông qua PredicateExpression của LLBLGen để giảm thiểu lượng dữ liệu tải về RAM. 
  - Tạm thời đã bọc Async / Await trên UI (bật con trỏ xoay) để tránh treo ứng dụng khi tính toán lâu.

### 10. Kiến Trúc Tổng Hợp: Trục Trung Gian Làm Phẳng (Flattened Ledger Service)
- **Vấn đề cũ**: Các báo cáo như Sổ Cái (Ledger), Cân đối phát sinh (Trial Balance), và Báo cáo thuế đều có các hàm Calculate độc lập để tính dữ liệu từ JournalEntry. Việc này dẫn đến sự không nhất quán khi tính Số dư đầu kỳ.
- **Giải pháp (Domain-Driven Design)**: Tạo ra lớp logic đáy FlattenedLedgerService chịu trách nhiệm Làm phẳng mọi bút toán. Giao dịch đơn (nhập số dư) tự động Nhồi VoucherDate. Tiện lợi cho mọi báo cáo, sau này có thể Materialize thành DB Cache.

### 11. Phân Hệ Nhập Liệu Vật Tư (Inventory Voucher Architecture & Tasks)
- **Bản chất nghiệp vụ**:
  - Tách bạch 2 tầng: Tầng 1 (giá vốn/giá gốc - `InventoryVoucherLine`), Tầng 2 (giá bán/thuế - `MaterialTaxLine` quan hệ 1-1).
  - Chân trang form Vật tư đặt các ô chọn tài khoản độc lập (không phải lưới). Khi bấm Chấp nhận quay về form chính `JournalVoucher`, form chính mới là Lưới, thực hiện gán định khoản ban đầu (TK 156) thành con của định khoản mới (TK 632) và đảo chiều Nợ/Có.
- **Các lỗi cần giải quyết**:
  1. Vật tư chưa load lên Grid khi mở chứng từ cũ (do thiếu PrefetchPath 3 tầng và cờ `isNew` bị gán cứng).
  2. Cột `MaterialTaxLine` (Giá bán, Thuế) trên Grid chỉ Đọc, chưa Edit được (do cơ chế Binding thuộc tính lồng trên EntityCollection LLBLGen).
  3. Đồng bộ trạng thái Ẩn/Hiện và Change-Tracking khi Lưu chứng từ.
- **Tài liệu & Kế hoạch chi tiết**: Đã lưu tại [.ai_context/InventoryVoucher_Tasks_Plan.md](file:///d:/NewaccNet/.ai_context/InventoryVoucher_Tasks_Plan.md).

### 12. Kế Hoạch Nâng Cấp Form Chứng Từ & Hóa Đơn (Voucher & Invoice Enhancements)
- **Các yêu cầu trọng tâm**:
  1. Đưa thông tin in ấn bổ trợ vào vùng Accordion (Collapsible Group), mặc định thu gọn, bỏ qua focus bàn phím, chỉ mở khi click chuột.
  2. Co gọn khoảng trắng vùng Header cho liền mạch và trực quan.
  3. Xây dựng Collection of Voucher trong RAM để thanh Navigation di chuyển linh hoạt giữa các phần tử.
  4. Nút New trong form tạo phần tử mới, thêm vào Collection và Set Current ngay lập tức; tích hợp Navigation control DevExpress.
  5. Thêm nút Tạo mới nhanh (+) trên các LookUpEdit/ComboBox danh mục (Công nợ, Chi phí, Kho...) gọi Form Editor tương ứng và tự động add vào collection của dropdown.
- **Tài liệu chi tiết & Checklist**: Đã ghi nhận tại [.ai_context/VOUCHER_ENHANCEMENT_PLAN.md](file:///d:/NewaccNet/.ai_context/VOUCHER_ENHANCEMENT_PLAN.md).
- **Trạng thái thực hiện**: **ĐÃ HOÀN THÀNH 100%**.
  - **Accordion in ấn & Header co gọn**: Header 1 dòng gọn gàng, nhóm thông tin in ấn và tài khoản ngân hàng đặt trong Accordion `dxlc:LayoutGroup` thu gọn, `KeyboardNavigation.IsTabStop="False"` bỏ qua tab phím.
  - **Lô chứng từ & Navigation**: Quản lý `_voucherBatch` và chỉ số `_currentVoucherIndex` kèm các nút điều hướng First, Prev, Next, Last và nhãn `[X / N]`. Nút New commit chứng từ cũ, thêm chứng từ mới và set current ngay. Nút Save kiểm tra cân đối toàn bộ lô và lưu trong 1 Transaction duy nhất. `VoucherListView` giữ nguyên trạng thái tĩnh khi thoát form.
  - **Nút Tạo mới nhanh (+)**: Đã tích hợp nút `+` vào toàn bộ các LookUpEdit danh mục:
    - Công nợ (`DebtDetailWindow`): Đối tượng (`Partner`), Nội dung (`DebtType`), Lý do (`DebtReason`).
    - Chi phí (`CostDetailWindow`): Đối tượng chi phí (`CostObject`), Yếu tố chi phí (`CostElement`), Đối tượng thuế (`Partner`).
    - Vật tư & Kho (`MaterialDetailWindow`): Kho hàng (`Warehouse`), Vật tư (`Material`), Đối tượng thuế (`Partner`).
    - Tạo mới `WarehouseEditorWindow` phục vụ tạo mới kho trực tiếp.
    - Cấu hình `EditorButtonShowMode="ShowForFocusedRow"` và `CellTemplate` với `PART_Editor`, lưu tự động vào DB và cập nhật ngay vào collection nguồn cũng như ô/dòng hiện hành.
  - **Khắc phục lỗi tràn số chứng từ (VoucherNo Overflow Fix):**
    - Cột `VoucherNo` trong database tối đa 10 ký tự (`VarWChar(10)`). Đã thay thế định dạng cũ 11 ký tự bằng hàm `GenerateNextVoucherNo()` tự động tăng số chứng từ trong lô (ví dụ `PK001` -> `PK002`) hoặc mặc định theo ngày `CTyyMMdd{seq:D2}` (đúng 10 ký tự).
    - Cấu hình thuộc tính `MaxLength` trên XAML và cơ chế cắt an toàn trên C# cho toàn bộ các trường header & accordion (`VoucherNo`, `Contents`, `Invoicesnumber`, `Personname`, `Paymentacc`, v.v.).
  - **Build Status**: Dự án biên dịch thành công 0 lỗi (`Build Succeeded`).

### 13. Phân Hệ Khai Báo Số Dư Đầu Kỳ & Tích Hợp Search LookUp
- **Tích hợp Search LookUp (`NewaccNet.Wpf.Controls.Search`)**:
  - Chuẩn hóa namespace và đăng ký `SearchProfiles` cho 8 danh mục cốt lõi (`ChartOfAccountEntity`, `PartnerEntity`, `WarehouseEntity`, `CostObjectEntity`, `CostElementEntity`, `InventoryItemEntity`, `DebtTypeEntity`, `DebtReasonEntity`).
  - Hỗ trợ tìm kiếm tiếng Việt không dấu (`VnStartsWith`, `VnContainsAll`), tự động cấu hình 2 cột `[Mã | Tên]`, tự động quản lý `DisplayMember`.
  - Tích hợp `SearchLookUpBehavior` vào `DebtDetailWindow`, `CostDetailWindow`, `MaterialDetailWindow`.
- **Form Nhập số dư (`OpeningBalanceWindow`) - Chuẩn theo mẫu nguyên bản**:
  - Đặt nút **"Nhập số dư"** ngay cạnh nút **"Chứng từ"** trên Ribbon tab "Nghiệp vụ" (`MainWindow.xaml`).
  - Giao diện siêu gọn, đúng nguyên bản mẫu của chương trình (Title: "Nhập số dư", kích thước nhỏ gọn ~480x360).
  - Không có thanh Toolbar cồng kềnh, không có Footer kiểm tra cân đối hay cảnh báo Nợ/Có.
  - Lưới nhập liệu gồm đúng 3 cột đồng bộ chuẩn với Voucher:
    1. **Nợ/Có** (`Dbcr`: Dropdown chọn "Nợ" / "Có").
    2. **Tài khoản** (`AccountId`: Dùng chung `AccountLookUpEdit` như ở Voucher, có sẵn nút `...` ngay trên combobox để người dùng bấm vào sửa chi tiết công nợ/chi phí mà không phải gõ lại tài khoản).
    3. **Số tiền** (`Amount`: Ô nhập số tiền thông thường định dạng `N0`, không nút bấm thừa).
  - Tự động lưu (`SaveDataSilently`) khi chuyển dòng (`RowUpdated`) hoặc khi đóng cửa sổ (`Closing`), xóa dòng bằng phím `Delete`, lưu bằng `Ctrl+S`, đóng bằng `Esc`. Không bao giờ chặn hay hỏi về cân đối.
- **Trạng thái**: **ĐÃ HOÀN THÀNH**.

### 14. Chuẩn Hóa UI, Tối Ưu Hiển Thị & Nền Tảng Xóa Theo Lô (Unit of Work)
- **Cột Nút Xóa Dòng Dùng Chung (`AppDetailTableView`)**:
  - Kế thừa `TableView`, tự động nhúng cột xóa vector thùng rác tinh gọn ở cuối dòng (`colRowDeleteAction`, Width 36px).
  - Tự động ẩn trên `NewItemRow` và `AutoFilterRow`, hiệu ứng hover tròn đỏ tinh tế.
  - Xử lý mượt mà cả với Grid thường lẫn Grid dùng Bands (`GridControlBand`).
  - Phát lệnh xóa trực tiếp lên binding context (`DeleteRow`), bắn sự kiện `RowDeleting` (cho phép can thiệp/hủy) và `RowDeleted` (tính toán lại số liệu).
- **Cơ Chế Xóa In-Memory (Unit of Work Pattern)**:
  - Mọi thao tác xóa dòng chi tiết hoặc xóa chứng từ trong lô chỉ tác động lên Collection trong RAM (không xóa ngay trong DB).
  - Chỉ commit xóa và lưu xuống DB trong cùng 1 Transaction duy nhất khi bấm nút "Lưu (Save)".
- **Tối Ưu Form Nhập Số Dư (`OpeningBalanceWindow`)**:
  - Mở rộng kích thước form lên `660x500` (MinWidth 540, MinHeight 380).
  - Cột `Dbcr` (65px, FixedWidth), `AccountId` (110px, FixedWidth).
  - Cột `Amount` co giãn tự do (`Width="*" MinWidth="220"`), căn phải chuẩn kế toán. Với `AutoWidth="True"`, cột `Amount` chiếm toàn bộ phần còn lại (> 410px), khắc phục triệt để lỗi khuất số tiền.
  - Chuẩn hóa thuộc tính `x:Name="PART_Editor"` giải quyết triệt để lỗi biên dịch `XLS0513`/`XDG0008`.
- **Tối Ưu Form Chi Tiết Vật Tư (`MaterialDetailWindow`)**:
  - Mở rộng cửa sổ lên `1150x680` (MinWidth 1000, MinHeight 550).
  - Header Thông tin hóa đơn (`pnlTaxInfo`) bố trí 2 cột ngang gọn gàng, giảm chiều cao từ 250px xuống ~100px.
  - Độ rộng các cột trên lưới chuẩn hóa: `MaterialId` co giãn (`Width="*" MinWidth="180"`), các cột số liệu tối ưu vừa vặn. Toàn bộ các cột (bao gồm `% Thuế` và `Tiền thuế`) luôn nằm trọn trong vùng nhìn thấy khi XB/NM.
  - Tiêu đề Band đổi động: "Giá bán & Thuế" khi XB, "Thuế GTGT" khi NM.
  - Footer co giãn thông minh qua DataTrigger: Tự động co cột Doanh thu về 0 khi NM để hiển thị 3 cột sạch sẽ (Tiền hàng $\rightarrow$ Thuế $\rightarrow$ Tổng TT); hiển thị 4 cột khi XB.
- **Tối Ưu Header & Layout Voucher Entry (`VoucherEntryWindow`)**:
  - Header 2 hàng: Hàng 1 chứa thông tin chứng từ, ComboBox Loại phiếu in (5 loại: Thông thường, Thu, Chi, UNC, Giấy nộp tiền), Nút in, Checkbox Đã ghi sổ. Hàng 2 chứa trường "Nội dung diễn giải" kéo dài 100% toàn bộ chiều ngang.
  - Nút In trên Toolbar chính và Header tự động Enable khi chọn loại phiếu đặc thù; Accordion thông tin in ấn & ngân hàng tự động mở rộng.
  - Khắc phục khoảng trắng thừa: Thay `dxlc:LayoutControl` bằng `Grid` WPF chuẩn 2 hàng, kéo tiêu đề định khoản chính/chi tiết áp sát ngay trên mỗi Grid (khoảng cách chỉ 4px), lưới giãn nở lấp đầy màn hình.
- **Phác Thảo Kế Hoạch Chốt Sổ & Chuyển Số Dư Cuối Năm (`TransferBalance.md`)**:
  - Ghi nhận đầy đủ mô hình clone CSDL (Access/MSSQL), quy tắc chuyển số dư Nợ/Có, sinh phiếu Nhập kho nội bộ (NNB) cho vật tư ngày 01/01 năm mới, tách biệt module kết chuyển lợi nhuận cuối kỳ và định hướng rà soát cấu trúc chi tiết đa chiều.
- **Trạng thái**: **ĐÃ HOÀN THÀNH VÀ TEST SƠ BỘ OK**.





### 15. Phân Hệ Chuyển Đổi Số Dư Sang Năm Mới (Balance Transfer - Flat & Multi-DB)
- **Tài liệu thiết kế chi tiết**: [.ai_context/BalanceTransfer_Design_Doc.md](file:///d:/NewaccNet/.ai_context/BalanceTransfer_Design_Doc.md).
- **Giao diện Chuyển đổi số dư (`TransferBalanceWindow`)**:
  - Tích hợp gọi từ Ribbon menu hệ thống.
  - Hỗ trợ chọn CSDL đích linh hoạt (MS Access, SQL Server, PostgreSQL) và kiểm tra kết nối động.
  - Loại bỏ các CheckBox lựa chọn phân hệ đơn lẻ: Quy trình chuyển đổi là **hàng loạt toàn bộ (Batch/All-in-one)** nhằm đảm bảo tính cân đối kế toán tuyệt đối.
  - Tích hợp thanh tiến trình `ProgressBar`, nhãn bước thực thi thời gian thực (`lblCurrentStep`) và khung ghi log chi tiết.
- **Kiến trúc Dịch vụ & Làm phẳng (FLAT Architecture)**:
  - Khởi tạo `IBalanceTransferService` và `BalanceTransferService` hoạt động hoàn toàn ở tầng ORM (LLBLGen Pro), độc lập với CSDL đích.
  - Bắt đầu từ cấp bảng chi tiết (`DebtDetail`, `InventoryVoucherLine`, `CostDetail`, `AssetDetail`, `InvestmentDetail`, `CurrencyDetail`) truy ngược lên `JournalEntry` / `InventoryVoucher`, dùng LINQ tính tổng Net Amount/Quantity và gom vào các DTO phẳng.
- **Đồng bộ Danh mục & Quy tắc Quản trị người dùng**:
  - Bước tiền đề tự động đồng bộ toàn bộ Master Data (`ChartOfAccount`, `Partner`, `Warehouse`, `Category`, `InventoryItem`, `Asset`, `CostObject`, `CostElement`, `Stock`, `Currency`) sang CSDL đích trước để triệt tiêu lỗi ràng buộc khóa ngoại (Foreign Key).
  - Đối với `SystemUser`: Tài khoản `admin` trong CSDL đích được **UPDATE** mật khẩu và thông tin thực tế từ nguồn (thay vì mật khẩu mặc định `newacc` của template), các user khác thực hiện **INSERT**.
- **Xử lý đặc thù theo từng phân hệ**:
  - Hàng tồn kho (`Category = 'B'`): Tự động tạo `JournalEntry` (`Dbcr = 1`) kèm chứng từ Nhập kho nội bộ NNB (`InventoryVoucher` với `Taxflag = false`) và các dòng `InventoryVoucherLine`.
  - Ngoại tệ (`Category = 'C'`): Tự động tính ngược tỷ giá ghi sổ bình quân đầu kỳ: $\text{ExchangeRate} = \frac{\text{Tổng VND}}{\text{Tổng ngoại tệ}}$.
  - Tách bạch từng bước thực thi trong `ExecuteTransferAsync` kèm try-catch để ghi log chi tiết và phát hiện ngay vị trí phát sinh lỗi.
- **Trạng thái**: **ĐÃ HOÀN THÀNH TOÀN DIỆN, BUILD SUCCEEDED (0 LỖI)**.

---

### 16. Phân Hệ Phân Bổ Chi Phí (Cost Allocation / Expense Distribution)
- **Tài liệu tham khảo**: `.ai_context/04_legacy_notes/legacy_queries -report-vba/expenseDistribution/`
- **Mô hình DTOs & Service (`ICostAllocationService`, `CostAllocationService`)**:
  - Kiến trúc thuần ORM/LINQ với LLBLGen Pro, triệt tiêu câu lệnh SQL tĩnh.
  - Lọc giao dịch gốc (`Transflag == false`) trong kỳ `[FromDate, ToDate]`.
  - Tập hợp Tử số chung (`TotalPoolAmount`) theo Tài khoản nguồn (`SourceAccountId`, khớp tiền tố).
  - Tập hợp Mẫu số tiêu thức (`TotalBaseAmount`) theo Danh sách tài khoản tiêu thức cấp lá (`AllocationBaseAccountIds`).
  - Tính tỷ lệ gia quyền bình quân: $\text{Ratio} = \frac{\text{BaseAmount}}{\text{TotalBaseAmount}}$; $\text{Allocated} = \text{Ratio} \times \text{TotalPoolAmount}$.
  - Xử lý làm tròn nguyên đồng, tự động bù lệch vào đối tượng có giá trị lớn nhất.
  - Sinh chứng từ phân bổ tự động (`JournalVoucher` với `Transflag = true`).
- **Giao diện Phân bổ Chi phí (`CostAllocationWindow`)**:
  - Chuẩn hóa toàn bộ ComboBox/LookUp sang `LookUpEdit` tích hợp `SearchLookUpBehavior` 2 cột `[Số TK | Tên TK]` và `[Mã YTCP | Tên yếu tố CP]`, tìm kiếm không phân biệt dấu tiếng Việt.
  - Tự động lưu và phục hồi tất cả tham số (`FromDate`, `ToDate`, `SourceAccount`, `DestAccount`, `DestCostElement`, danh sách tiêu thức) vào `UserSettings.json` qua `UserPreferencesHelper`.
  - Bộ chọn tài khoản tiêu thức chỉ hiển thị các tài khoản cấp dưới cùng (lá). Tự động bung tài khoản tổng hợp thành tài khoản lá.
  - Hỗ trợ xóa nhiều tài khoản tiêu thức (phím Delete, Backspace, Double-click).
  - Tối ưu lưới dữ liệu: Cột Đối tượng chi phí chiếm toàn bộ độ rộng còn lại (`Width="*" MinWidth="260"`), cột số liệu cố định (`FixedWidth="True"`), bổ sung thanh trượt `GridSplitter`.
  - Bẫy bảo vệ nghiêm ngặt: Tự động khóa nút Phân bổ và cảnh báo khi Dòng tổng = 0.
- **Trạng thái**: **ĐÃ HOÀN THÀNH VÀ BUILD THÀNH CÔNG (0 LỖI)**.

---

### 17. Định Hướng Phân Hệ Khấu Hao Tài Sản Cố Định & Kết Chuyển Chi Phí (Kế hoạch ngày mai)
- **Triết lý thiết kế tinh gọn**:
  - Đối với phần mềm kế toán vừa và nhỏ, module Tài sản tập trung vào các nghiệp vụ cốt lõi: Danh mục thông tin chính, Tính khấu hao định kỳ, In bảng kê và Tự động sinh chứng từ kế toán.
  - Bỏ các tính năng rườm rà (như nút Undo riêng biệt), thay bằng việc mở trực tiếp chứng từ vừa sinh ra để xem/xóa tự nhiên trong hệ thống chứng từ.
- **Thuật toán Khấu hao theo ngày thực tế (`AssetDepreciationHelper.cs`)**:
  - Đã đóng gói thành công hàm tính toán tổng quát trong `NewaccNet.Wpf.AppSystem.Helpers.AssetDepreciationHelper`.
  - Tự động kẹp biên xử lý lệch pha mốc thời gian: tài sản mua giữa kỳ (tính từ ngày mua), hết hạn giữa kỳ (dừng đúng ngày hết hạn), hoặc nằm ngoài kỳ (khấu hao = 0).
  - Công thức chuẩn: $\text{Tiền KH} = \text{Amount} \times \frac{\text{DepRate} \times \text{Days}}{365}$.
- **Hai nhiệm vụ trọng tâm chuẩn bị triển khai**:
  1. **Khấu hao Tài sản Cố định (`AssetDepreciationWindow`)**:
     - Lấy bản ghi theo dõi mới nhất (`AssetTracking`) bằng LINQ (thay thế bảng tạm `AssetTrackingDep`).
     - Lưới hiển thị danh sách tài sản trích khấu hao (Checkbox chọn, Tên TS, Đơn vị, Còn lại, Tiền KH kỳ này).
     - Cho phép In bảng kê phân bổ khấu hao.
     - Sinh 1 chứng từ duy nhất: Ghi Có TK 214 (Hao mòn) / Ghi Nợ các TK Chi phí (642, 627, 154...) gắn với Đối tượng CP và Yếu tố CP.
  2. **Kết Chuyển Chi Phí (`ExpenseTransferWindow`)**:
     - Căn cứ theo mẫu màn hình `ExpenseTrasfer.png` (`.ai_context/04_legacy_notes/legacy_queries -report-vba/expenseDistribution/`).
     - Chọn Đến ngày, Tài khoản nguồn (2211, 621, 622, 627, 154...), Tài khoản đích (154, 911...), Checkbox Kết chuyển doanh thu.
     - Lưới chi tiết: Đối tượng chi phí, Yếu tố chi phí, Checkbox KC, Số dư chi phí, Chi phí để lại.

