# TÀI LIỆU BÀN GIAO & ĐIỂM DỪNG DỰ ÁN (NewaccNet)
*Ngày cập nhật: 26/09/2026*

## 1. Các quy tắc chung (Bắt buộc tuân thủ)
- **LƯU Ý NGHIÊM TRỌNG VỀ SVG ICON CỦA DEVEXPRESS**: Tuyệt đối không tự đoán tên file SVG (ví dụ: o_currency.svg, ctions/add.svg) để đưa vào thuộc tính LargeGlyph. Nếu file không tồn tại trong thư viện DevExpress.Images.v25.2.dll, toàn bộ ứng dụng sẽ bị crash ngay khi khởi động (XamlParseException: Cannot locate resource). Giải pháp: Nếu không chắc chắn, hãy dùng các icon đã biết là an toàn như usiness%20objects/bo_document.svg hoặc xaf/action_... cho đến khi có thể chọn chính xác bằng Image Picker của Visual Studio.
- **Mô hình lưới dữ liệu (GridControl)**: Kế thừa từ `BaseDictionaryWindow`. Cần có thanh tìm kiếm, dòng lọc (`ShowAutoFilterRow="True"`), và Menu chuột phải `RowCellMenuCustomizations` (Thêm, Sửa, Xóa, In).
- **In ấn Grid**: Sử dụng `ReportManager.PrintGridControl` (`PrintAutoWidth = true` và `Landscape = true`). Lọc trên Grid sẽ phản ánh đúng ra Report.
- **Xử lý form đặc thù (Nhỏ, ít biến động)**: Bắt buộc dùng **Inline Editing** (chỉnh sửa trực tiếp trên Grid, `AllowEditing="True"`).
  - Vị trí dòng thêm mới bắt buộc phải nằm ở dưới cùng: `NewItemRowPosition="Bottom"`.
  - Các cột liên kết khóa ngoại (Foreign Key) trên Grid Inline phải dùng Dropdown (`ComboBoxEditSettings`).
  - *Áp dụng cho*: Nguồn tài sản, Lý do tăng giảm, Thuế suất, Nhóm vật tư.

## 2. Các công việc vừa hoàn thành
- **Công nợ**: Đối tượng (`PartnerEntity`), Nội dung công nợ (`DebtTypeEntity`), Lý do công nợ (`DebtReasonEntity`).
- **Chi phí**: Đối tượng CP (`CostObjectEntity`), Yếu tố CP (`CostElementEntity`).
- **Tài sản**: Nguồn tài sản (`SourceEntity`), Lý do tăng giảm (`ReasonEntity`). Đã áp dụng quy tắc Edit Inline.

## 3. Kế hoạch công việc tiếp theo (Làm "cuốn chiếu" tuần tự)

**A. Phân hệ Chứng khoán / Cổ phiếu (Hoàn thành):**
1. **Loại hình cổ phiếu** (StockTypeEntity): Lưới Edit Inline (NewItemRowPosition="Bottom").
2. **Danh mục cổ phiếu** (StockEntity): Lưới Edit Inline (NewItemRowPosition="Bottom"), dùng Dropdown cho Stocktypeid.

**A. Phân hệ Ngoại tệ (Hoàn thành):**
1. **Tiền tệ** (CurrencyEntity): Lưới Edit Inline (NewItemRowPosition="Bottom").
2. **Tỷ giá** (ExchangeRateEntity): Lưới Edit Inline (NewItemRowPosition="Bottom"), dùng Dropdown cho CurrencyId.

**A. Phân hệ Vật tư (Hoàn thành):**
0. **Kho hàng** (Warehouse): Form inline edit (NewItemRowPosition="Bottom").
1. Sắp xếp Ribbon theo thứ tự: **Nhóm vật tư -> Danh mục vật tư -> Thuế suất**.
2. **Thuế suất** (`Taxrate`): Form inline edit (`NewItemRowPosition="Bottom"`).
3. **Nhóm vật tư** (`Category`): Form inline edit (`NewItemRowPosition="Bottom"`). Cột `TaxrateID` hiển thị dưới dạng Dropdown (ComboBox) chọn Thuế suất.
4. **Danh mục vật tư** (`InventoryItem`): Form chuẩn có Popup Editor (do nhiều trường thông tin).
5. Áp dụng quy tắc `NewItemRowPosition="Bottom"` ngược lại cho Nguồn TS và Lý do TS.

**B. Các phân hệ chờ xử lý:**




## 4. Backlog
- Thay icon chuyên biệt cho Ribbon (hiện đang dùng icon tạm `bo_...`).

---

### CÔNG VIỆC CẦN LÀM (TODO DÀNH CHO AGENT KẾ NHIỆM)

**Nhiệm vụ: Cập nhật Icon (SVG) cho thanh Ribbon**
- **Vấn đề (Treo ứng dụng):** Hiện tại, nếu điền bừa một tên file SVG không tồn tại vào thuộc tính LargeGlyph (ví dụ: usiness%20objects/bo_currency.svg), ứng dụng WPF sẽ bị crash cứng ngay lúc khởi động với lỗi XamlParseException: Cannot locate resource.
- **Mục tiêu:** Tìm và thay thế các icon tạm thời (đang dùng chung o_document.svg) bằng các icon thể hiện ĐÚNG bản chất nghiệp vụ của từng nút (VD: nút Tiền tệ phải ra hình tiền, nút Kho hàng phải ra hình nhà kho).
- **Đang tắc ở đâu:** Không nắm được danh sách (tên file chính xác) các file ảnh SVG được tích hợp sẵn bên trong thư viện DevExpress.Images.v25.2.dll. Do đó, việc đoán mò tên file thường xuyên dẫn đến lỗi.
- **Cách làm (Quy trình thử - sai):**
  1. TUYỆT ĐỐI KHÔNG SỬA HÀNG LOẠT.
  2. Bắt buộc phải tìm trong kho ảnh có sẵn của thư viện DevExpress (pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/...).
  3. Cùng với User **thử sai từng nút một**. 
  4. Cập nhật 1 tên icon -> Build & Chạy thử (F5) -> Nếu Crash thì phải sửa lại tên khác hoặc lùi về ảnh cũ ngay lập tức -> Chạy lên thấy hình đẹp mới chuyển sang nút tiếp theo.