# NEWACC — CẨM NANG TRIỂN KHAI PHÂN HỆ ĐỒNG BỘ HÓA ĐƠN & MAPPING AI
*Tài liệu định hướng kiến trúc kỹ thuật dành cho Kỹ sư hệ thống (Developer)*

---

## 1. TRIẾT LÝ THIẾT KẾ CỐT LÕI (BẤT BIẾN)
1. **Core quyết định - AI gợi ý:** AI không bao giờ có quyền ghi sổ (Post) hay tự tạo Danh mục. AI chỉ điền vào cột "Gợi ý" (`resolved` với `match.method = Ai`) trên Lớp đệm.
2. **Neo theo Hóa đơn Thuế:** Hóa đơn từ Tổng cục Thuế là gốc pháp lý tối cao và bất biến. Mọi số liệu tính toán, hạch toán phải khớp theo hóa đơn Thuế để doanh nghiệp không vi phạm.
3. **Danh mục đi trước - Số liệu theo sau:** Tuyệt đối không khớp số liệu thực (Transaction) khi Bản đồ Danh mục (Master Data Mapping) chưa thông. Danh mục lỗi = Chặn giao dịch.
4. **Human-in-the-loop:** Phần mềm không tự động 100% một cách sáo rỗng. Phần mềm làm chân sai vặt (tìm kiếm, bóc tách, gợi ý), Con người làm trọng tài bấm nút duyệt chốt.

---

## 2. KIẾN TRÚC DỮ LIỆU BỘ BA (RAW - RESOLVED - MATCH)
Mọi dòng hàng (Line-item) từ hóa đơn XML đưa vào Lớp đệm (`VoucherDraft`) phải được cấu trúc thành cấu trúc 3 phần để không làm mất dữ liệu gốc và phục vụ UI/UX đối chiếu:

- **Raw (Dữ liệu gốc):** Giữ nguyên 100% văn bản từ file XML của Thuế (Tên hàng, đơn vị tính, số lượng, số tiền của bên bán). KHÔNG ĐƯỢC SỬA.
- **Resolved (Dữ liệu hệ thống):** Mã đối tượng (`PartnerId`), Mã vật tư (`MaterialId`), Tài khoản (`AccountId`), Mã kho (`WarehouseId`) định danh trong DB của NEWACC.
- **Match (Lịch sử & Thuật toán):** Lưu vết phương thức khớp nối để hiển thị lên UI trực quan cho kế toán.
  - `match.method` ∈ `[TaxCodeExact, CodeExact, AliasMemory, PostingRule, History, Fuzzy, Ai, User]`
  - `confidence`: Độ tự tin từ 0.0 đến 1.0.
  - `reason`: Lý do tường minh bằng tiếng Việt (Ví dụ: *"Đã map theo bí danh ngày 02/09"* hoặc *"AI gợi ý dựa trên lịch sử NCC"*).

---

## 3. QUY TRÌNH XỬ LÝ 2 TẦNG TUẦN TỰ

### TẦNG 1: ĐỒNG BỘ DANH MỤC (MASTER DATA MAPPING)
Mục tiêu là thông suốt bảng ánh xạ trước khi đụng vào số liệu. Quy trình chạy qua 3 bộ lọc:
1. **Bộ lọc Tất định (Deterministic Matchers):**
   - Khớp `TaxCodeExact` (Trùng MST người bán trên cổng thuế) -> Khớp luôn `PartnerId`, confidence = 1.0.
   - Khớp `AliasMemory` (Tra cứu bảng `Item_Mapping_Rules` - những gì người dùng đã từng chọn và nhấn "Lưu luật" trước đây). Nếu trùng khớp `(PartnerId + RawItemName)` -> Điền ngay `MaterialId`, `AccountId`, `WarehouseId`.
   - Khớp `PostingRuleEngine` (Áp các quy tắc do Kế toán trưởng tự cấu hình trong phần mềm, ví dụ: Hóa đơn chứa từ "Xăng/Dầu" thì tự động điền TK Nợ 6418).
2. **Trợ lý AI gợi ý (Nếu các bước trên không tìm thấy):**
   - AI nhận các trường còn trống (`resolved = null`).
   - AI gọi tool `search_materials` để quét danh mục hệ thống. Sử dụng thuật toán so khớp chuỗi (`Levenshtein Distance`) hoặc Tìm kiếm ngữ nghĩa (`Semantic Embedding`) để tìm ra Top 3 mã vật tư có nghĩa gần nhất (Ví dụ: "Thép phi 12" gần với "Sắt XD 12mm").
   - AI điền gợi ý vào cột `resolved` kèm nhãn `method = Ai`. Nếu độ tự tin `confidence < 0.6`, AI bỏ trống, không đoán bừa.
3. **Xử lý ca khó (Mã mới hoàn toàn):**
   - Nếu mặt hàng mới tinh, AI sẽ bật flag `NeedsReview` và phân tích tên hàng để gợi ý cấu trúc tạo mã mới cho kế toán (Gợi ý thuộc nhóm vật tư nào, tài khoản kho mặc định nào phù hợp). Kế toán xác nhận duyệt thì Core mới Insert vào DB danh mục thật thông qua `IMasterDataGateway`.

### TẦNG 2: ĐỒNG BỘ SỐ LIỆU & XỬ LÝ VÊNH (TRANSACTION MATCHING)
Khi danh mục đã thông, hệ thống tiến hành kiểm tra chênh lệch số liệu giữa XML và dữ liệu quản trị nội bộ:
- **Vênh số thập phân / Làm tròn:** Thuật toán của Core (`DraftValidationService`) tự động tính toán khoản chênh lệch tổng tiền thuế (Delta). Nếu Delta nhỏ hơn ngưỡng cấu hình (ví dụ < 5.000đ), Core tự động tạo sẵn một bút toán điều chỉnh vào TK Chi phí khác (811) hoặc Điều chỉnh giá vốn thông qua `IDraftVoucherBuilder`. AI không tham gia tính toán tiền.
- **Vênh số lượng / Quy cách:** Nếu lệch lớn về số lượng (Bên bán xuất "Cuộn", bên mua nhập kho "Kg"), hệ thống bôi đỏ và hiển thị màn hình UI đối chiếu 2 cột độc lập để con người tự chọn hệ số quy đổi vật lý hoặc tách dòng chi phí.

---

## 4. THIẾT KẾ R ANH GIỚI HỆ THỐNG VÀ KIỂM TRA KIẾN TRÚC (NETARCHTEST)
Để bảo vệ hệ thống tuyệt đối khi đưa AI/Agent vào dự án C# .NET 9 WPF, bạn phải tuân thủ nghiêm ngặt ranh giới phụ thuộc và khóa bằng các bài kiểm tra tự động (`ArchitectureTests`):

1. **Ranh giới của Agent (`NewaccNet.Agent`):**
   - **CẤM** tham chiếu trực tiếp tới `NewaccNet.Core`, `NewaccNet.Tools`, LLBLGen Pro ORM (`dataaccess/`), hoặc các namespace chứa logic ghi sổ (`*.Posting`, `*.MasterData`).
   - Agent chỉ được nhìn thấy thế giới bên ngoài thông qua Hợp đồng dữ liệu (`NewaccNet.Contracts`) và các **Danh mục Tool chỉ-đọc/chỉ-nháp** được cung cấp qua giao thức MCP hoặc In-process.
2. **Ranh giới Cổng Ghi sổ (`IPostingGateway`):**
   - Đây là **cổng duy nhất** được phép thực hiện các lệnh chỉnh sửa DB thật đối với chứng từ (`adapter.SaveEntity` trên `JournalVoucherEntity` và các bảng chi tiết).
   - Mọi nguồn dữ liệu (Nhập tay, Excel, Hóa đơn cổng thuế, AI) bắt buộc phải convert về định dạng chuẩn `VoucherDraft` (JSON) trước khi nạp vào Cổng này.
3. **Ranh giới Office Studio (Giao diện Spreadsheet điều khiển RAM):**
   - Toàn bộ quá trình sinh file nháp Excel/Word để đối chiếu phải thực hiện qua `DocumentFormat.OpenXml` chạy hoàn toàn trên `MemoryStream` (Zero-Disk I/O). Sau khi sinh cấu trúc xong bắt buộc gọi `stream.Seek(0, SeekOrigin.Begin)` để nạp thẳng lên DevExpress Spreadsheet Control.
   - **Chế độ Document (Tự do):** Không bao giờ có nút ghi sổ vào DB.
   - **Chế độ Draft (Chứng từ cố định):** Chứa file JSON ẩn trong `CustomXmlPart`. Người dùng sửa ô nào trên màn hình Excel, hệ thống sẽ đọc ngược lại để cập nhật vào JSON nháp, nhấn Duyệt -> Đẩy qua Cổng để ghi sổ DB.

---

## 5. LỘ TRÌNH TRI N KHAI THỰC CHIẾN (CHIA ĐỂ TRỊ)
Đừng cố làm AI ngay từ ngày đầu tiên. Hãy làm hệ thống chạy mượt mà bằng tay trước khi cắm AI làm trợ lý.

- **Giai đoạn P0 (Tách Cổng):** Bốc toàn bộ logic lưu trữ, kiểm tra logic kế toán (cân đối Nợ/Có, kiểm tra tài khoản chi tiết) từ code-behind của các màn hình (`VoucherEntryWindow.xaml.cs`) ra các Service độc lập thuộc `NewaccNet.Core/Posting`. Viết Unit Test cho Validator.
- **Giai đoạn P1 (Lớp đệm thủ công):** Triển khai cấu trúc JSON `VoucherDraft`, bộ lưu trữ file nháp `JsonFileDraftStore`, viết các bộ Matcher tất định (khớp MST, khớp bí danh cũ). Xây dựng màn hình Office Studio chế độ Draft. Lúc này khách hàng lấy hóa đơn về, hệ thống tự map MST đối tượng, còn dòng hàng họ tự chọn bằng tay -> Nhấn Lưu/Ghi sổ chạy tốt.
- **Giai đoạn P2 (Hệ thống tự học):** Hoàn thiện bộ nhớ thông minh `MappingMemoryStore` và `PostingRuleEngine`. Phần mềm tự động lưu lại mọi quyết định map danh mục của người dùng sau mỗi lần ghi sổ thành công để tự động áp dụng cho lần sau.
- **Giai đoạn P3 (Tích hợp Trợ lý AI Gợi ý):** Lúc này mới đưa Microsoft Agent Framework và OpenRouter vào. Thiết kế Skill bằng file `.md` chỉ định rõ AI chỉ được can thiệp vào các ô dữ liệu còn trống sau giai đoạn P2. Viết bộ đánh giá `NewaccNet.Agent.Evals` với 50 hóa đơn mẫu để đo độ chính xác của AI trước khi đóng gói sản phẩm.
