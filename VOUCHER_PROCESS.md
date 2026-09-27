# QUY TRÌNH & KIẾN TRÚC MÀN HÌNH NHẬP CHỨNG TỪ (VOUCHER ENTRY)

## 1. TỔNG QUAN KIẾN TRÚC (Accounting First)
Màn hình **Chứng từ Kế toán** là "trái tim" và điểm chạm duy nhất của toàn bộ hệ thống. Mọi nghiệp vụ (Thu, Chi, Nhập, Xuất, Phân bổ...) đều được quy về một nguyên lý hạch toán kép (Kế toán đi trước, nghiệp vụ chi tiết đi sau).

Dữ liệu di chuyển theo chiều dọc: 
JournalVoucher (Thông tin chung) -> JournalEntry (Hạch toán phẳng Nợ/Có) -> XXX_Details (Chi tiết theo tính chất tài khoản).

## 2. BẢN CHẤT DỮ LIỆU (DATABASE SCHEMA - FLAT)
Dưới Database, hệ thống lưu trữ ở dạng **PHẲNG (Flat)**:
- **JournalVoucherEntity**: Lưu header chứng từ (Số CT, Ngày CT, Nội dung...).
- **JournalEntryEntity**: Lưu các dòng hạch toán chi tiết. Mỗi dòng đại diện cho 1 chân Nợ hoặc 1 chân Có. Nó chỉ lưu AccountId (Mã TK), Amount (Số tiền) và cờ Dbcr (Nợ hay Có).
- **Các bảng Details (DebtDetails, CostDetails, InventoryVouchers...)**: Đây là các bảng vệ tinh bám vào JournalEntry. Tùy theo AccountId thuộc nhóm nào (Công nợ, Kho, Chi phí), hệ thống sẽ lưu thông tin chi tiết vào bảng vệ tinh tương ứng.

## 3. BẢN CHẤT GIAO DIỆN (UI - NON-FLAT / ĐỐI ỨNG)
Để đáp ứng thói quen của kế toán Việt Nam, giao diện UI phải bẻ cong dữ liệu phẳng thành mô hình **ĐỐI ỨNG**:
- **Cấu trúc 2 Lưới (Dual-Grid):** Giao diện chia làm 2 phần (Grid Nợ và Grid Có) hoặc phân tách Đối ứng chính/phụ rõ ràng.
- **Data Binding bóc tách:** Khi load dữ liệu từ DB, hệ thống tự động lọc các dòng JournalEntry có cờ Nợ đưa sang trái, cờ Có đưa sang phải. Tổng tiền 2 bên bắt buộc phải cân bằng. Khi Save, hệ thống gom 2 lưới này lại thành một tập hợp phẳng để lưu xuống DB.

## 4. CƠ CHẾ RẼ NHÁNH CHI TIẾT (DYNAMIC DETAIL FORM)
Đây là điểm thông minh nhất của hệ thống:
1. Người dùng nhập 1 dòng hạch toán (VD: Chọn TK 1412).
2. Hệ thống ngầm định truy xuất ChartOfAccount để biết 1412 có CategoryId = 'A' (Công nợ).
3. Biểu tượng "Thư mục" (Folder Icon) ở cuối dòng sáng lên.
4. Khi người dùng click (hoặc tự động bật), hệ thống mở Form **Chi tiết công nợ**.
5. Form này bind vào bảng DebtDetails (Lưu Đối tượng, Hợp đồng, Ngày đáo hạn...).
6. Dữ liệu chi tiết này được ôm chặt vào dòng JournalEntry đó trong bộ nhớ cho tới khi bấm Save toàn bộ chứng từ.

## 5. LỘ TRÌNH THỰC THI (IMPLEMENTATION PLAN)
- **Bước 1 (Base UI):** Tạo UserControl/Window VoucherEntryWindow chuẩn với Layout 2 lưới GridControl Nợ/Có, thanh Toolbar điều hướng (First, Prev, Next, Last) và tổng cộng tiền.
- **Bước 2 (Data Engine):** Viết logic bóc tách và hợp nhất (Split & Merge) dữ liệu giữa Entity phẳng và ObservableCollection trên UI.
- **Bước 3 (Router rẽ nhánh):** Xây dựng hàm OpenDetailForm(JournalEntry row). Dựa vào AccountTypeMapping, dùng Switch-Case để mở đúng Form (DebtDetail, CostDetail, InventoryDetail...).
- **Bước 4 (Entry Point):** Tạo một nút to, nổi bật nhất trên Ribbon tên là **"NHẬP CHỨNG TỪ"** (hoặc Phiếu kế toán tổng hợp).

## 6. CHIẾN LƯỢC GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (UX/UI)

### 6.1. Khung Giao diện Chính (Main Voucher Form)
- **Header (JournalVoucher):** Chỉ hiển thị các thông tin cốt lõi nhất để tránh rườm rà:
  - Số chứng từ (VoucherNo)
  - Ngày chứng từ (VoucherDate)
  - Nội dung (Contents)
  - Trạng thái: Đã ghi sổ (Bookflag / Confirmed)
  - *(Các trường phục vụ in ấn sẽ bị ẩn đi hoặc đẩy vào một tab/mục mở rộng).*
- **Vấn đề Nhập liệu Grid vs Form:**
  - Nhập liệu trực tiếp trên Grid có nhược điểm là khó navigate (di chuyển phím).
  - Đa phần chứng từ chỉ có 2 vế (1 Nợ - 1 Có), phức tạp nhất là Hóa đơn bán hàng (~5 vế).
  - **Yêu cầu cốt lõi:** Giao diện bắt buộc phải thể hiện được tính **ĐỐI ỨNG**.
  - **Hướng giải quyết UX:** Vẫn sử dụng Grid để thể hiện danh sách đối ứng, nhưng sẽ tối ưu hóa phím Enter/Tab, hoặc kết hợp một vùng nhập liệu nhanh bên ngoài Grid.

### 6.2. Custom Control: Account Selector (Dropdown Tài khoản)
- **Tính chất:** Là một control dùng chung (Reusable UserControl / Custom Edit), xuất hiện ở cả trong Grid (không viền - no border) và trên các Form (có viền).
- **Cấu tạo:** 
  - Một Dropdown chọn danh mục tài khoản (LookUpEdit / ComboBoxEdit).
  - Kèm theo một nút bấm (Folder Icon) bên cạnh.
- **Hành vi (Behavior):** 
  - Khi người dùng chọn xong tài khoản từ Dropdown, HOẶC bấm vào nút Folder -> Kích hoạt chung một sự kiện xử lý.
  - Sự kiện này sẽ mở Form chi tiết (Detail form) tương ứng với loại tài khoản.
- **Giai đoạn 1 (Mockup):** 
  - Implement để khi chọn, hệ thống chỉ cần hiển thị thông báo (MessageBox/Alert) nhận diện đúng "Loại tài khoản" (CategoryId) là đạt yêu cầu.

### 6.3. Kiến trúc Form Chi Tiết đóng vai trò "Voucher Generator"
- **Thực trạng:** Khi nhập Hóa đơn bán hàng, kế toán không nhập Nợ 131, Có 511, Có 3331 một cách thủ công trên lưới.
- **Giải pháp thiết kế:** 
  - Kế toán nhập 1 tài khoản (VD: Doanh thu hoặc Giá vốn) trên form chính.
  - Form phụ bật lên. Tại form phụ, Control AccountLookupControl (được thiết kế độc lập) tái sử dụng để kế toán chọn các tài khoản đối ứng (TK Thuế, TK Kho, TK Doanh thu).
  - Khi bấm **Save ở form phụ**, hệ thống tự động sinh ra một chùm định khoản (Nợ/Có) và đẩy ngược lại vào 2 Lưới ở màn hình chính. 
  - **Lợi ích:** Kế toán không cần thao tác nhảy qua nhảy lại giữa 2 lưới, dữ liệu luôn tự cân đối.

### 6.4. Kết quả Hiện thực hóa (Skeleton)
- Đã thiết lập thành công VoucherEntryWindow.xaml.
- **UI:** Bao gồm Toolbar chuẩn (Navigation, Save, Delete), Header Form và 2 Grid (Trái - Phải).
- **Logic Lưu trữ (UOW):** Sử dụng subEntry.ParentEntry = mainEntry ngay trên RAM. UnitOfWork của LLBLGen tự động tính toán đồ thị, insert dòng cha lấy Id, truyền vào ParentId của dòng con và insert tiếp một cách mượt mà trong 1 Transaction.
- **Tình trạng:** Khung sườn đã hoàn thành, lỗi icon SVG đã được fix. Nút trên Ribbon được đổi tên thành "Nhập chứng từ".

### 6.5. Quản lý Trạng thái & Trải nghiệm thao tác (UX)
- **Hiển thị khóa chính (Tracking IDs):** Bổ sung các cột ẩn/hiển thị Id, ParentId trên lưới và Header để theo dõi trực quan quá trình sinh khóa tự động của UOW trước và sau khi lưu.
- **Reload sau khi Lưu:** Sau khi Commit xuống DB, phải load lại chứng từ (Fetch) để cập nhật chính xác các khóa ngoại và trạng thái từ DB lên UI.
- **Cảnh báo dữ liệu chưa lưu (Dirty Tracking):** 
  - Gắn cờ theo dõi sự thay đổi. 
  - Bắt sự kiện thoát (Closing). Nếu chưa Save, hiển thị Prompt hỏi người dùng. Không được tự động lưu.
- **Vô hiệu hóa nút Close mặc định:** Tắt nút [X] của Window (ShowCloseButton="False") để bắt buộc người dùng thao tác qua bộ nút chuẩn trên Toolbar.

## 7. Nguyên tắc Cân đối Ghi Sổ Kép (Double-Entry Constraint)
Trong kế toán, một chứng từ chỉ hợp lệ khi và chỉ khi **Tổng Nợ = Tổng Có** (tương đương Sum(Amount * Dbcr) == 0).
- **Real-time Tracking:** Tính toán tức thời ngay khi rời ô nhập liệu (sự kiện CellValueChanged).
- **Luật kiểm soát (Strict Validation):**
  - **Save:** Không cho phép Lưu (Hard-block) nếu Độ lệch khác  .
  - **New:** Không cho phép tạo mới nếu chứng từ hiện tại đang nhập dở và bị lệch.
  - **Navigate:** Không cho phép chuyển sang chứng từ khác (trong batch/lô) nếu đang lệch.
