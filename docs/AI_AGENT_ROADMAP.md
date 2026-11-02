# KẾ HOẠCH HÀNH ĐỘNG & LỘ TRÌNH TRIỂN KHAI AI — NEWACC NET (V3.1)
> **Căn cứ:** Bản đặc tả kiến trúc chính thức `NEWACC_Architecture_v3.md` (01/11/2026) và kết quả thẩm định kiến trúc (`solution.pdf`).
> **Tôn chỉ bất biến:** *Core kế toán giữ quyền quyết định — AI chỉ tăng cường và gợi ý. Mọi kênh (WPF, CLI, MCP) đều là Adapter bình đẳng quay quanh Tool Catalog.*
> **Nguyên tắc triển khai:** *Không ôm đồm, nghiệm thu theo từng năng lực kiểm chứng được (Milestone-based), không chạy theo số lượng module.*

---

## 1. TỔNG KẾT HẠNG MỤC ĐÃ HOÀN THÀNH (BASELINE AUDIT — 02/11/2026)

Hôm nay hệ thống đã hoàn tất và kiểm thử thành công toàn bộ tầng kết nối kế toán & phân hệ vật tư/ERP:

- [x] **Tách bạch Thuế VAT & Chi tiết Công nợ (Debt Detail):**
  - Thông tin khai thuế VAT (`VatObjectID`, `VatObjectName`, số hóa đơn, ngày HĐ...) được lưu độc lập trên Header/VAT Entry.
  - Phân hệ Kho kết nối trực tiếp với thực thể công nợ `PartnerId` và `DebtDetailEntity`.
- [x] **Cải tiến `AccountLookUpEdit` (Chế độ `ButtonOnlyMode`):**
  - Thêm thuộc tính `ButtonOnlyMode="True"`: ẩn dropdown tài khoản thô của kế toán đối với người dùng ERP vật tư; chỉ hiển thị nút mở form chi tiết công nợ (`DebtDetailWindow`) khi tài khoản là loại công nợ (Category 'A').
- [x] **Quản lý thực thể chuẩn ORM (Single Source of Truth):**
  - Khởi tạo đồng bộ cha-con `JournalVoucher` và `InventoryVoucher` trước khi bind UI để loại bỏ hoàn toàn thực thể mồ côi.
  - Tự động lấy `VatBillDate` làm `VoucherDate` ngầm định, đánh dấu `Bookflag = false` cho chứng từ kho chờ kế toán ghi sổ.
  - Lưu trữ `PaymentType` trực tiếp trong `InventoryVoucher.EntryGraph` JSON để nạp lại tức thì, không cần thuật toán suy đoán ngược.
- [x] **Công cụ tra cứu chứng từ kho (F3 / Toolbar) & Cơ chế khóa ghi sổ:**
  - Tạo hộp thoại tra cứu `WarehouseVoucherLookupDialog` (F3 trên toolbar).
  - Mặc định chỉ lọc chứng từ chưa ghi sổ (`Bookflag = false`).
  - Tự động khóa Read-Only và cảnh báo trực quan nếu chứng từ đã được kế toán ghi sổ (`Bookflag = true`), bảo vệ số liệu kế toán không bị sửa đổi ngoài tầm kiểm soát.
- [x] **Cấu hình động phương thức thanh toán (`PaymentMethods` Schema Mapping):**
  - Lưu và đọc cấu hình phương thức thanh toán trong `DbConfigService` / `VoucherPostingConfig`.
  - Tự động đồng bộ tài khoản công nợ được cấu hình với danh sách phương thức thanh toán trong `VoucherPostingConfigWindow`.
- [x] **Build & Parity Verification:**
  - Biên dịch thành công 100% không lỗi trên `NewaccNet.Core`, `NewaccNet.Wpf`, `NewaccNet.Plugin.ErpWarehouse`.

---

## 2. NĂM CỘT MỐC TRIỂN KHAI THỰC CHẤT (5 MILESTONES)

Theo kết quả đánh giá kiến trúc, lộ trình không đo bằng số lượng chức năng mà đo bằng **năng lực kiểm chứng được**:

```
[MỐC 1: Core độc lập & ERP Kho]  ──► (HOÀN THÀNH - Đang test nghiệm thu)
               │
               ▼
[MỐC 2: Staging hoạt động thủ công] (XML HĐĐT -> Nháp -> Mapping tay -> Sinh hạch toán -> Ghi sổ)
               │
               ▼
[MỐC 3: Tự động hóa tất định] (Mapping bằng MST, Bí danh đã học, Posting Rules, Lịch sử)
               │
               ▼
[MỐC 4: Tích hợp AI & Bộ Evals] (AI gợi ý ca phức tạp, kiểm chứng bằng bộ 50 hóa đơn thực tế)
               │
               ▼
[MỐC 5: Tool Catalog & BYOA] (Headless CLI, MCP Server cho Claude/OpenClaw/Agent ngoài)
```

---

### MỐC 1: CORE ĐỘC LẬP & PHÂN HỆ KHO/VẬT TƯ (ĐÃ HOÀN THÀNH)
*Mục tiêu: Tách rời toàn bộ logic tính toán, kiểm tra và ghi sổ khỏi UI WPF. Phân hệ kho và kế toán hoạt động độc lập, chính xác.*
- Trạng thái: **Đã hoàn thành mã nguồn**, người dùng đang nghiệm thu chi tiết nghiệp vụ.

---

### MỐC 2: STAGING HOẠT ĐỘNG THỦ CÔNG (ƯU TIÊN TIẾP THEO)
*Mục tiêu: Một luồng hóa đơn điện tử đi xuyên suốt từ XML đến Sổ cái mà KHÔNG CẦN AI. Không mất dữ liệu, không sai định khoản.*
1. **Nạp hóa đơn XML vào Staging:**
   - Đọc tệp XML (chuẩn Thông tư 78), lưu vào `VoucherDraft` (Stage 1: Raw).
   - Lưu trữ bản nháp độc lập qua `JsonFileDraftStore` (hoặc `StagingDraft` table khi cần).
2. **Giao diện rà soát & Mapping thủ công:**
   - Mở bản nháp trong `OfficeStudioWindow` (chế độ Draft), người dùng chọn mã đối tượng, mã vật tư, tài khoản.
   - Chuyển sang Stage 2 (Resolved) và sinh cấu trúc hạch toán Stage 3 (Projection).
3. **Ghi sổ qua một cổng duy nhất (`IPostingGateway`):**
   - Đảm bảo tính nguyên tử (Transaction) và **Idempotent**: khi ghi sổ thành công, lưu liên kết `VoucherId` vào Draft. Nếu thao tác lặp lại, Gateway nhận diện và từ chối ghi sổ trùng.
   - Nghiệm thu: Nhập 1 file XML thật, mapping thủ công, ghi sổ thành công, lên đầy đủ Báo cáo thuế và Sổ cái.

---

### MỐC 3: TỰ ĐỘNG HÓA TẤT ĐỊNH (DETERMINISTIC AUTOMATION)
*Mục tiêu: Hệ thống tự động xử lý phần lớn hóa đơn quen thuộc bằng quy tắc toán học và lịch sử, chưa cần tiêu tốn token AI.*
1. **Bộ học bí danh (`MappingMemoryStore`):**
   - Tự động ghi nhớ: `PartnerId + Tên hàng trên hóa đơn -> MaterialId + AccountId`.
   - Lần sau gặp lại tự động điền với độ tin cậy 1.0 (Exact Match).
2. **Quy tắc hạch toán (`PostingRuleEngine`):**
   - Cho phép định nghĩa quy tắc (ví dụ: Nhà cung cấp Điện lực / Tiền điện -> Nợ 6427, Có 331).
3. **Tra cứu lịch sử (`PostingHistoryService`):**
   - Tìm kiếm các bút toán tương tự đã ghi sổ trong quá khứ để gợi ý cho người dùng.
4. **Nghiệm thu:** Đo lường tỷ lệ tự động điền đạt > 70% đối với các đối tác quen thuộc mà không cần gọi API đám mây.

---

### MỐC 4: TÍCH HỢP AI NỘI BỘ & BỘ ĐÁNH GIÁ (EVALS-DRIVEN AI)
*Mục tiêu: Đưa AI vào giải quyết các trường hợp mới/phức tạp, có cơ chế đo lường độ chính xác định lượng.*
1. **Agent nội bộ với Microsoft Agent Framework (MAF):**
   - Kết nối OpenRouter API với khả năng chuyển đổi model linh hoạt (Claude 3.5 Sonnet, Gemini 2.5 Flash, GPT-4o...).
   - Bổ sung `PiiMaskingMiddleware` để bảo vệ dữ liệu nhạy cảm (ẩn thông tin tài khoản ngân hàng, thông tin cá nhân trước khi gửi lên model).
2. **Bộ đánh giá Evals (`NewaccNet.Agent.Evals`):**
   - Chuẩn bị bộ 50 hóa đơn thực tế có đáp án chuẩn do kế toán xác nhận.
   - Đo lường định lượng: Độ chính xác theo từng trường, tỷ lệ bỏ trống đúng (biết không đoán mò), số vòng gọi tool và chi phí token.
   - Chỉ kích hoạt model khi vượt qua ngưỡng benchmark an toàn.
3. **Khả năng cách ly:** Tắt AI hoặc mất mạng, hệ thống vẫn vận hành bình thường nhờ Mốc 2 và Mốc 3.

---

### MỐC 5: HỆ SINH THÁI AGENT & TOOL CATALOG (BYOA - BRING YOUR OWN AGENT)
*Mục tiêu: Mở rộng NEWACC thành một nền tảng kế toán chuẩn mực, bất kỳ Agent nào cũng có thể khai thác.*
1. **Tool Catalog chuẩn mực (`NewaccNet.Tools`):**
   - Đóng gói các hàm nghiệp vụ: `search_partners`, `search_materials`, `get_debt_aging`, `get_account_balance`, `get_trial_balance`, `suggest_field`...
   - Ràng buộc phân quyền: Chỉ 2 mức `Read` và `DraftWrite` (Tuyệt đối không cấp quyền `Post` cho Agent).
2. **Kênh CLI (`newacc.exe`):**
   - Thực thi độc lập không cần giao diện, trả output JSON chuẩn.
3. **Kênh MCP Server (Model Context Protocol):**
   - Cung cấp MCP qua Stdio cho Claude Desktop hoặc HTTP Loopback (`127.0.0.1:{port}/mcp`) có xác thực Bearer Token.

---

## 3. NGUYÊN TẮC THIẾT KẾ & BẢO VỆ RỦI RO (RÚT RA TỪ ARCHITECTURE REVIEW)

| Rủi ro kiến trúc | Giải pháp chuẩn mực đã chốt |
|---|---|
| **Nguy cơ 2 đường ghi sổ** | Thống nhất duy nhất: Mọi nguồn đều đi qua `IPostingGateway`. `VoucherPostingService` chỉ là implementation nội bộ bên dưới Gateway. |
| **Ghi sổ trùng do crash ứng dụng** | Áp dụng tính **Idempotent** cho thao tác Posting: Lưu `VoucherId` vào Draft ngay trong transaction ghi sổ. |
| **Ôm đồm chuẩn hóa DB (P0.5)** | Không làm gián đoạn tiến độ bằng việc đại tu DB quy mô lớn. Chia nhỏ migration, ưu tiên luồng dữ liệu cho Staging và Posting. |
| **Toàn vẹn `InventoryVoucher.EntryGraph`** | Kiểm tra nghiêm ngặt: loại bỏ Id mồ côi, kiểm tra tính đầy đủ của chùm giao dịch trước khi serialize/deserialize. |
| **Dữ liệu AI đám mây** | Không gửi toàn bộ sổ sách lên mạng. Chỉ gửi chuỗi text mô tả hàng hóa cần gợi ý, kèm bộ lọc che PII. |

---

## 4. TÀI LIỆU THAM KHẢO
- Đặc tả kiến trúc chính: `.ai_context/AI/NEWACC_Architecture_v3.md`
- Nhật ký đánh giá kiến trúc với ChatGPT: `.ai_context/AI/solution.pdf`
- Lớp đệm Staging & Gateway: `NewaccNet.Core/Posting/` và `NewaccNet.Core/Staging/`
