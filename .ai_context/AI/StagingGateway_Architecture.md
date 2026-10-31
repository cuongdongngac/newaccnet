# Kiến trúc Lớp đệm (Staging) & Cổng ghi sổ duy nhất (Posting Gateway)

_Phiên bản: 1.1 — 29/10/2026 (bổ sung Office Studio làm màn hình nháp, IAiTool, Skill). Tài liệu định hướng cho AI-coding. Đọc file này trước khi viết bất kỳ code nào liên quan tới: nhập hóa đơn từ cổng thuế, import Excel, OCR, trợ lý AI hạch toán._

---

## 0. Bối cảnh & vấn đề cần giải quyết

- AI hiện tại (`NewaccNet.AI/AccountingAgentService`) chỉ "gọi một lần": prompt → JSON → đẩy thẳng vào `VoucherEntryWindow`. Không tra cứu được đối tượng, vật tư, lịch sử; thiếu bảng chi tiết (`DebtDetail`, `InventoryVoucherLine`, `VatEntry`...).
- Logic kiểm tra & lưu chứng từ nằm trong code-behind `VoucherEntryWindow.xaml.cs` (`BtnSave_Click`, `SaveSingleVoucher`) → không nguồn nào khác dùng lại được.
- Dữ liệu từ ngoài vào (hóa đơn cổng thuế, Excel, OCR, AI) cần **rà soát, đối chiếu, mapping mã** trước khi thành chứng từ thật.

## 1. Nguyên tắc bất biến (KHÔNG được vi phạm)

1. **AI không bao giờ ghi sổ.** AI chỉ được đọc dữ liệu và **đề xuất** vào bản nháp. Người dùng luôn là người bấm nút ghi sổ.
2. **Một cổng duy nhất vào DB thật.** Mọi chứng từ (nhập tay, import, AI, hóa đơn cổng thuế) đều đi qua `IPostingGateway`. Không service nào khác được gọi `adapter.SaveEntity` lên `JournalVoucherEntity` và các bảng chi tiết.
3. **Lớp đệm là định dạng chuẩn.** Mọi nguồn dữ liệu đều được chuyển về một định dạng trung gian `VoucherDraft` (JSON). Cổng chỉ nhận `VoucherDraft`, không cần biết dữ liệu đến từ đâu.
4. **Chuyện nào ra chuyện đấy.** Các khâu Thu thập → Chuẩn hóa → Đối chiếu/Mapping → Rà soát → Ghi sổ là các service tách biệt. Mỗi service làm đúng một việc, có thể chạy độc lập.
5. **Thủ công và AI dùng chung đường ống.** AI chỉ là một "người gợi ý" cắm vào khâu Mapping. Tắt AI thì toàn bộ quy trình vẫn chạy được bằng tay (import → tự mapping → ghi sổ).
6. **Tất định trước, AI sau, người cuối cùng.** Khâu mapping luôn chạy quy tắc tất định (MST, mã, bí danh đã học, quy tắc hạch toán) trước. AI chỉ xử lý phần còn lại. Người dùng xác nhận tất cả.
7. Giữ nguyên các quy tắc dự án: DB-agnostic (không stored procedure/view), LLBLGen, không sửa `dataaccess/`, MVVM, DevExpress WPF.

## 2. Luồng tổng thể

```
 NGUỒN                   THU THẬP             LỚP ĐỆM (Staging)                       CỔNG           DB THẬT
 ─────                   ────────             ─────────────────                       ────           ───────
 Cổng thuế (XML/JSON) ─┐
 Excel / CSV          ─┼─► IDraftSource ─► VoucherDraft (JSON) ─► Matching ─► Review ─► IPostingGateway ─► JournalVoucher
 OCR ảnh/PDF          ─┤   (adapter)        Status=Imported      (rule→AI)   (người)   (validate+build   JournalEntry
 Trợ lý AI (chat)     ─┤                                          Status=      Status=   +save 1 trans)    *Details
 Sao kê NH (sau này)  ─┘                                          Matched      Approved  Status=Posted     VatEntry...
                                                                                                    │
 Nhập tay trên VoucherEntryWindow ──────────────────────────────────────────────────────────────────┘
 (cũng đi qua IVoucherValidator + IVoucherPostingService của cổng)
```

**Màn hình kiểm duyệt duy nhất là Office Studio** (`OfficeStudioWindow`, OpenXML + DevExpress Spreadsheet). Mọi nguồn — kể cả hóa đơn tải từ cổng thuế — đều được hiển thị lên đó để người dùng xem "mặt mũi" chứng từ, sửa mã, rồi mới duyệt. Office Studio chỉ là chỗ **xem – sửa – duyệt**; dữ liệu thật là `VoucherDraft` JSON nằm bên trong file (xem mục 4.9).

Dữ liệu thô vẫn giữ ở nơi riêng của nó (ví dụ `TaxInvoiceEntity` / `TaxInvoiceDetailEntity` đã có). Lớp đệm chỉ tham chiếu tới nguồn qua `SourceRef`, không sao chép logic tải hóa đơn.

## 3. Định dạng chuẩn `VoucherDraft` (JSON)

### 3.1 Vòng đời trạng thái (`DraftStatus`)

| Trạng thái | Ý nghĩa | Ai chuyển |
|---|---|---|
| `Imported` | Vừa chuyển từ nguồn về, chưa mapping | `IDraftSource` |
| `Matched` | Đã chạy mapping, có thể còn dòng chưa xác định | `DraftMatchingService` |
| `NeedsReview` | Còn lỗi/thiếu mã/nghi trùng | `DraftMatchingService` / `DraftValidationService` |
| `Approved` | Người dùng đã duyệt, sẵn sàng ghi sổ | Người dùng (UI) |
| `Posted` | Đã ghi sổ, có `PostedVoucherId` | `IPostingGateway` |
| `Rejected` | Bỏ qua (không hạch toán) | Người dùng |
| `Failed` | Ghi sổ lỗi, xem `Issues` | `IPostingGateway` |

Chỉ `IPostingGateway` được chuyển sang `Posted`. AI không được chuyển trạng thái quá `Matched`.

### 3.2 Cấu trúc (ví dụ hóa đơn mua hàng)

```json
{
  "schemaVersion": 1,
  "draftId": "b3f0c1e2-...",
  "draftType": "PurchaseInvoice",
  "status": "Matched",
  "source": { "kind": "TaxInvoice", "ref": "TaxInvoice:1523", "importedAt": "2026-10-29T10:00:00+07:00", "importedBy": "admin" },

  "header": {
    "voucherDate": "2026-10-15",
    "voucherNo": null,
    "voucherSourceCode": "NM",
    "contents": "Mua hàng theo HĐ 0001234 - Cty ABC",
    "personName": "Công ty TNHH ABC"
  },

  "counterparty": {
    "raw":      { "taxCode": "0101234567", "name": "CÔNG TY TNHH ABC", "address": "..." },
    "resolved": { "partnerId": "ABC01", "isNew": false },
    "match":    { "method": "TaxCodeExact", "confidence": 1.0, "reason": "Trùng MST với đối tượng ABC01" }
  },

  "invoice": { "template": "1", "series": "C26TAA", "number": "0001234", "date": "2026-10-15",
               "amountBeforeTax": 10000000, "vatAmount": 1000000, "totalAmount": 11000000 },

  "lines": [
    {
      "lineNo": 1,
      "raw":      { "itemCode": "SP01", "itemName": "Giấy A4 Double A", "unit": "Ram", "quantity": 100, "unitPrice": 100000, "amount": 10000000, "vatRate": 10, "vatAmount": 1000000 },
      "resolved": { "materialId": "VPP-A4", "warehouseId": "KHO1", "debitAccountId": "1561", "vatAccountId": "1331", "costObjectId": null },
      "match": [
        { "field": "materialId",     "method": "AliasMemory",  "confidence": 0.98, "reason": "Đã map 'Giấy A4 Double A' của NCC ABC01 → VPP-A4 ngày 02/09" },
        { "field": "debitAccountId", "method": "PostingRule",  "confidence": 0.95, "reason": "Quy tắc: nhóm VPP → 1561" },
        { "field": "warehouseId",    "method": "History",      "confidence": 0.80, "reason": "3 lần gần nhất nhập KHO1" }
      ]
    }
  ],

  "settlement": { "creditAccountId": "331", "dueDate": null },

  "proposedMasterData": [
    { "kind": "Partner", "tempKey": "NEW:0109999999", "fields": { "taxno": "0109999999", "customerName": "..." }, "approved": false }
  ],

  "issues": [
    { "severity": "Warning", "code": "DUPLICATE_SUSPECT", "field": "invoice", "message": "Đã có VatEntry cùng số HĐ 0001234 + MST 0101234567 (CT PN0012)" }
  ],

  "audit": [
    { "at": "2026-10-29T10:01:00+07:00", "by": "system", "action": "Matched" },
    { "at": "2026-10-29T10:01:05+07:00", "by": "ai:model-name", "action": "SuggestField", "detail": "line 2 materialId" }
  ],

  "postedVoucherId": null
}
```

Quy ước:
- Mỗi trường mapping có ba phần: `raw` (dữ liệu gốc, KHÔNG sửa), `resolved` (mã trong hệ thống), `match` (nguồn gốc + độ tin cậy + lý do). UI hiển thị cả ba để người dùng kiểm tra.
- `match.method` ∈ `TaxCodeExact | CodeExact | AliasMemory | PostingRule | History | Fuzzy | Ai | User`.
- Tiền tệ trong draft dùng `decimal`; chuyển kiểu khi build entity.
- Draft **không chứa định khoản Nợ/Có**. Định khoản do `IDraftVoucherBuilder` sinh ra theo `draftType`. Ngoại lệ: `draftType = "GeneralJournal"` (bút toán tổng hợp) cho phép khai báo dòng Nợ/Có trực tiếp.
- Bổ sung `schemaVersion` để nâng cấp định dạng sau này.

### 3.3 Các `DraftType` dự kiến

| DraftType | Nguồn chính | Builder sinh ra |
|---|---|---|
| `PurchaseInvoice` | Hóa đơn đầu vào cổng thuế, OCR | NM: Có 331 / Nợ 15x(+InventoryVoucherLine) / Nợ 1331(+VatEntry) — hoặc Nợ 6xx + CostDetail nếu là dịch vụ |
| `SalesInvoice` | Hóa đơn đầu ra cổng thuế | XB: Nợ 131 / Có 511 / Có 3331 (+ giá vốn 632/156 nếu có kho) |
| `CashReceipt` / `CashPayment` | AI chat, Excel | PT / PC |
| `BankReceipt` / `BankPayment` | Sao kê ngân hàng (giai đoạn sau) | BC / BN |
| `GeneralJournal` | Excel, AI chat | Dòng Nợ/Có khai báo trực tiếp |

## 4. Danh sách Service (tên tiếng Anh, vị trí, trách nhiệm)

### 4.1 `NewaccNet.Contracts` — Mô hình dùng chung (không phụ thuộc LLBLGen)

| Kiểu | Mô tả |
|---|---|
| `VoucherDraft`, `DraftHeader`, `DraftCounterparty`, `DraftLine`, `DraftSettlement` | POCO khớp JSON mục 3.2 |
| `RawValue<T>` / `ResolvedValue` / `MatchInfo` | Bộ ba raw–resolved–match |
| `DraftStatus`, `DraftType`, `MatchMethod`, `IssueSeverity` | Enum |
| `DraftIssue` | `Severity`, `Code`, `Field`, `Message` |
| `ProposedMasterData` | Đề xuất tạo mới đối tượng/vật tư, cần người duyệt |
| `PostingResult` | `Success`, `VoucherId`, `VoucherNo`, `Issues` |
| `DraftSerializer` | Serialize/deserialize JSON (System.Text.Json), kiểm tra `schemaVersion` |

### 4.2 `NewaccNet.Core/Posting` — CỔNG GHI SỔ (bước tiên quyết, làm đầu tiên)

| Service | Trách nhiệm |
|---|---|
| `IVoucherValidator` / `VoucherValidator` | Kiểm tra **entity** trước khi lưu: cân đối Nợ=Có, tài khoản tồn tại & là TK chi tiết, TK cần chi tiết thì phải có chi tiết (theo CategoryId), khóa sổ/hạn dùng (`DbConfigService.CanUpdateVoucher`), hạn mức bản quyền (`CheckRecordLimit`). Trả `List<DraftIssue>`, không hiện MessageBox. |
| `IVoucherNumberService` / `VoucherNumberService` | Sinh số chứng từ theo nguồn CT (chuyển từ `VoucherEntryWindow.GenerateNextVoucherNo`). |
| `IVoucherPostingService` / `VoucherPostingService` | Lưu lô `JournalVoucherEntity` trong 1 transaction (chuyển nguyên `SaveSingleVoucher` + phần diff/xóa chi tiết từ code-behind sang). |
| `IDraftVoucherBuilder` (+ các bản cài đặt `PurchaseInvoiceVoucherBuilder`, `SalesInvoiceVoucherBuilder`, `CashVoucherBuilder`, `GeneralJournalVoucherBuilder`) | Chuyển `VoucherDraft` đã resolve → `JournalVoucherEntity` đầy đủ chi tiết (DebtDetail, InventoryVoucher/Line, MaterialTaxLine, VatEntry, CostDetail). Theo mẫu `CostAllocationVoucherBuilder`. Logic sinh chùm NM/XB hiện nằm ở `MaterialDetailWindow` phải chuyển về đây để UI và builder dùng chung. |
| `IDraftVoucherBuilderFactory` | Chọn builder theo `DraftType`. |
| **`IPostingGateway` / `PostingGateway`** | **Cổng duy nhất.** Xem quy trình mục 5. |
| `IMasterDataGateway` / `MasterDataGateway` | Cổng tạo mới đối tượng/vật tư từ `ProposedMasterData` đã được duyệt. Không cho tạo trùng MST/mã. |

Sau khi có các service này, `VoucherEntryWindow.BtnSave_Click` chỉ còn: gom UI → gọi `IVoucherValidator` → hiện lỗi → gọi `IVoucherPostingService`.

### 4.3 `NewaccNet.Core/Staging` — Lưu trữ lớp đệm

| Service | Trách nhiệm |
|---|---|
| `IDraftStore` | `Save`, `Get`, `Query(status, type, source, from, to)`, `Delete`, `ExistsBySourceRef`. |
| `JsonFileDraftStore` | Bản cài đặt mặc định: mỗi draft 1 file `.json` trong thư mục `Staging/{yyyy-MM}/`. Phù hợp Access một người dùng. |
| `DbDraftStore` (giai đoạn sau) | 1 bảng `StagingDraft(Id, DraftType, Status, SourceKind, SourceRef, PayloadJson, CreatedAt, UpdatedAt, PostedVoucherId)`; cột `PayloadJson` lưu nguyên JSON. Cần cho SQL Server/PostgreSQL nhiều người dùng. |
| `DraftLifecycleService` | Chuyển trạng thái hợp lệ (máy trạng thái mục 3.1), ghi `audit`. Chặn chuyển trạng thái sai (ví dụ AI → Approved). |
| `DraftExportService` / `DraftImportService` | Xuất/nhập draft ra file JSON/Excel để người dùng sửa ngoài hoặc chuyển máy. |

### 4.4 `NewaccNet.Core/Ingestion` — Thu thập (mỗi nguồn 1 adapter)

| Service | Trách nhiệm |
|---|---|
| `IDraftSource` | `Task<IReadOnlyList<VoucherDraft>> CreateDraftsAsync(DraftSourceRequest req)` |
| `TaxInvoiceDraftSource` | Đọc `TaxInvoiceEntity` + `TaxInvoiceDetailEntity` (đã tải bằng `NewaccNet.TaxInvoice`) → draft `PurchaseInvoice`/`SalesInvoice`. Bỏ qua hóa đơn đã có `VoucherId` hoặc đã có draft cùng `SourceRef`. |
| `ExcelDraftSource` | Import theo mẫu Excel chuẩn (có file mẫu trong `Template/`). |
| `OcrDraftSource` | Gọi `OcrVisionService` → draft. Chỉ dùng cho chứng từ giấy/ảnh; hóa đơn điện tử ưu tiên `TaxInvoiceDraftSource`. |
| `AiChatDraftSource` | Câu lệnh tự nhiên → draft (thay cho đường cũ `ParseNaturalLanguageJournalAsync` → `VoucherEntryWindow`). |
| `BankStatementDraftSource` | Giai đoạn sau. |

Adapter **chỉ chuyển dạng**, không mapping mã, không gọi AI để đoán tài khoản.

### 4.5 `NewaccNet.Core/Matching` — Đối chiếu & Mapping TẤT ĐỊNH (không AI)

| Service | Trách nhiệm |
|---|---|
| `DraftMatchingService` | Điều phối: chạy lần lượt các matcher bên dưới cho từng draft, rồi gọi `DraftValidationService`. Cho phép cắm thêm `IFieldSuggester` (AI) ở cuối. |
| `PartnerMatcher` | MST trùng khớp tuyệt đối `PartnerEntity.Taxno` → confidence 1.0. Không có MST → so tên chuẩn hóa (bỏ dấu, bỏ "CÔNG TY TNHH"...). Không tìm thấy → tạo `ProposedMasterData` (không tự tạo đối tượng). |
| `MaterialMatcher` | Thứ tự: `AliasMemory` (tên hàng của NCC này đã từng map) → mã hàng trùng `MaterialId` → tên trùng tuyệt đối → fuzzy (chỉ gợi ý, confidence < 0.7). |
| `PostingRuleEngine` | Áp bảng quy tắc hạch toán do kế toán trưởng khai báo (mục 6.2). |
| `PostingHistoryService` | Đọc `FlattenedDetailService` (Inventory/Cost/Vat/Debt flat rows): lần trước NCC/vật tư này hạch toán TK nào, kho nào, đối tượng CP nào. Chỉ đọc. |
| `MappingMemoryStore` | Lưu bí danh đã được người dùng xác nhận: `(PartnerId, RawItemName/RawItemCode) → MaterialId, AccountId, WarehouseId`. Ghi **sau khi** `PostingGateway` ghi sổ thành công (học từ dữ liệu đã duyệt, không học từ đề xuất). Lưu JSON hoặc bảng riêng, giống `IDraftStore`. |
| `DuplicateDetector` | Nghi trùng: `TaxInvoiceEntity.VoucherId` đã có; `VatEntry` trùng (`Vatbillno` + `Taxno` + `Vatbilldate`); draft khác cùng `SourceRef`. |
| `DraftValidationService` | Kiểm tra **draft** (chưa build entity): thiếu mã, mã không tồn tại, tổng dòng ≠ tổng hóa đơn, thuế suất lệch, nghi trùng → ghi `issues`, đặt `NeedsReview` nếu có lỗi. |

### 4.6 `NewaccNet.Core/Reconciliation` — Đối chiếu sổ sách với cổng thuế (độc lập với nhập liệu)

| Service | Trách nhiệm |
|---|---|
| `TaxInvoiceReconciliationService` | So danh sách hóa đơn cổng thuế với `VatEntry` trong sổ theo kỳ: có trên cổng chưa có trong sổ, có trong sổ không có trên cổng, lệch tiền/thuế, hóa đơn bị hủy/điều chỉnh. Cập nhật `TaxInvoiceEntity.ProcessStatus` / `ReconcileNote`. Không tạo chứng từ. |

### 4.7 `NewaccNet.AI` — AI chỉ là người gợi ý

| Service | Trách nhiệm |
|---|---|
| `IFieldSuggester` (Contracts) / `AiFieldSuggester` | Nhận các trường **chưa resolve** sau bước tất định, trả gợi ý kèm `confidence` + `reason`, gắn `method = Ai`. Không sửa trường đã có `method` khác (trừ khi người dùng yêu cầu). |
| `AiToolRegistry` | Đăng ký các tool chỉ-đọc cho LLM (function calling qua OpenRouter `tools`): `search_partners`, `search_materials`, `get_account_info`, `get_posting_history`, `get_mapping_memory`, `get_posting_rules`. Mỗi tool bọc một service Core ở mục 4.5. |
| `AiDraftEditTools` | Tool ghi duy nhất AI được dùng: `suggest_field(draftId, lineNo, field, value, confidence, reason)` → ghi vào draft qua `IDraftStore`. **Không có tool nào gọi `IPostingGateway` hoặc `IMasterDataGateway`.** |
| `AiReviewAssistant` | Giải thích cho người dùng vì sao gợi ý như vậy, tóm tắt các vấn đề trong draft. |
| `OpenRouterGateway` | Bổ sung vòng lặp tool-calling (hoặc chuyển sang `Microsoft.Extensions.AI` function invocation). |
| `AccountingAgentService` (cũ) | Giữ lại để tương thích; luồng mới đi qua `AiChatDraftSource`. Bỏ các giá trị viết cứng `"156"`, `"KHO_HH"`. |

### 4.8 `NewaccNet.Wpf` — Giao diện

| Màn hình | Chức năng |
|---|---|
| `DraftInboxWindow` | Danh sách draft (quét `IDraftStore`) theo trạng thái/nguồn/kỳ. Nút: Lấy từ cổng thuế, Import Excel/JSON, Chạy mapping, Gợi ý AI (bật/tắt), Ghi sổ hàng loạt (chỉ draft `Approved`, không còn issue mức Error). Double-click một draft → mở **Office Studio** ở chế độ nháp. |
| `OfficeStudioWindow` (chế độ Draft) | Màn hình kiểm duyệt duy nhất. Xem mục 4.9. Nút thêm: **Lưu nháp** (về `IDraftStore`, chưa đụng DB), **Duyệt**, **Bỏ qua**, **Xem định khoản** (build entity trên RAM, hiện chùm Nợ/Có để soát, không lưu), **Ghi sổ** (gọi `IPostingGateway`). |
| `VoucherEntryWindow` | Tùy chọn "Mở trên phiếu" khi cần chỉnh định khoản sâu (`LoadFromAiVoucher`). Lưu vẫn qua cổng. |
| `PostingRuleListView` | Danh mục quy tắc hạch toán (Inline Edit theo chuẩn danh mục). |
| `MappingMemoryListView` | Xem/xóa bí danh đã học. |

### 4.9 Office Studio làm màn hình nháp (Draft Review Surface)

Office Studio có **hai chế độ, không trộn lẫn**:

| Chế độ | Đầu ra | Cột | Vào DB |
|---|---|---|---|
| **Document** (như hiện nay) | Đơn thuốc, chấm công, biên bản, phân tích... (`DynamicSpreadsheetDto` / `DynamicWordDocumentDto`) | Tự do, AI quyết định | **Không bao giờ** |
| **Draft** (mới) | Nháp chứng từ (`VoucherDraft`) | **Cố định theo mẫu** của từng `DraftType` | Có, chỉ qua `IPostingGateway` |

Service (đặt ở `NewaccNet.AI/Services` cạnh `OpenXmlReportService`, hoặc `NewaccNet.Core/Staging`):

| Service | Trách nhiệm |
|---|---|
| `DraftWorkbookRenderer` | `VoucherDraft` → `MemoryStream` xlsx theo mẫu `Template/Draft_{DraftType}.xlsx`. Zero-disk như quy tắc OpenXML hiện có. |
| `DraftWorkbookReader` | Đọc ngược workbook người dùng đã sửa → cập nhật `resolved` (+ `match.method = User`, ghi `audit`). Đọc theo **tên cột cố định / named range**, không đoán. |
| `DraftWorkbookPackage` | Đọc/ghi JSON nháp vào **`CustomXmlPart`** trong file xlsx: một file vừa có trang tính cho người, vừa có JSON chuẩn cho máy. |
| `DraftLookupListProvider` | Cung cấp danh sách mã (vật tư, TK chi tiết, kho, đối tượng) để renderer gắn `DataValidation` dạng list → người dùng chọn mã bằng dropdown trong ô. |

Quy ước trình bày trên mẫu:
- Sheet `Header`: thông tin hóa đơn gốc (người bán, MST, số/ký hiệu/ngày HĐ, tổng tiền) — **khóa**, nền xám.
- Sheet `Lines`: mỗi dòng hàng = cột gốc (khóa, xám) + cột mã (`MaterialId`, `WarehouseId`, `DebitAccountId`, `CostObjectId`) mở để sửa + cột `Nguồn gợi ý` + `Độ tin cậy` + `Lý do`.
- Tô màu cột mã theo `match.method`: xanh = TaxCode/Alias/Rule, vàng = History/Fuzzy, tím = AI, đỏ = còn trống.
- Sheet `Issues`: danh sách `DraftIssue` (thiếu mã, nghi trùng, lệch tổng).
- Sheet `Preview` (tùy chọn): chùm định khoản Nợ/Có do builder sinh thử — chỉ để xem.
- Khi mở lại file: đọc `CustomXmlPart` trước; nếu ô trên sheet khác JSON → coi là người dùng sửa.

Luồng hóa đơn cổng thuế:
```
TaxInvoiceDownloader → TaxInvoiceEntity (dữ liệu thô)
   → TaxInvoiceDraftSource → VoucherDraft (Imported)
   → DraftMatchingService (+ AiFieldSuggester nếu bật) → Matched/NeedsReview
   → DraftWorkbookRenderer → Office Studio (người xem mặt mũi, sửa mã)
   → Lưu nháp (để lúc khác)  |  Duyệt → Ghi sổ → IPostingGateway
```
OCR hóa đơn giấy đi y hệt, chỉ khác bước đầu là `OcrDraftSource`.

### 4.10 "Công tắc" cho AI (`IAiTool`)

```csharp
public enum AiToolAccess { Read, DraftWrite }   // KHÔNG tồn tại mức Post

public interface IAiTool
{
    string Name { get; }                 // snake_case, ví dụ "search_partners"
    string Description { get; }          // tiếng Việt: khi nào dùng, trả về gì
    string ParametersJsonSchema { get; } // JSON Schema cho tham số
    AiToolAccess Access { get; }
    Task<string> ExecuteAsync(JsonElement args, CancellationToken ct); // trả JSON gọn
}
```

- `AiToolRegistry`: đăng ký/tra cứu tool; plugin đăng ký thêm qua `IPluginContext`. Chỉ phát cho model các tool mà skill đang chạy cho phép.
- `AiToolLoopRunner` (trong `OpenRouterGateway` hoặc tách riêng): gửi `tools` → nhận `tool_calls` → chạy `ExecuteAsync` → gửi kết quả lại → lặp tới khi model trả lời xong. Giới hạn số vòng (mặc định 8) và ghi log từng lần gọi vào `audit` của draft.
- Mỗi tool **bọc một service Core** đã có (mục 4.5), không chứa logic nghiệp vụ riêng.

Bộ tool khởi điểm:

| Tool | Access | Bọc service |
|---|---|---|
| `search_partners` | Read | `PartnerMatcher` |
| `search_materials` | Read | `MaterialMatcher` |
| `get_account_info` | Read | ChartOfAccount (CategoryId, có phải TK chi tiết) |
| `get_posting_history` | Read | `PostingHistoryService` |
| `get_mapping_memory` | Read | `MappingMemoryStore` |
| `get_posting_rules` | Read | `PostingRuleEngine` |
| `suggest_field` | DraftWrite | `IDraftStore` + `DraftLifecycleService` |

### 4.11 Skill nghiệp vụ

- Thư mục `NewaccNet.AI/Skills/*.md`, copy theo bản cài. Mỗi skill = frontmatter + hướng dẫn.
- `SkillLoader`: đọc file, parse frontmatter. `SkillSelector`: chọn skill **bằng code** theo `DraftType` (không để AI tự chọn với luồng chứng từ).
- `SkillPromptComposer`: ghép prompt = lời dặn chung + nội dung skill + JSON nháp (**chỉ các trường còn trống**) ; danh sách tool lọc theo frontmatter `tools`.
- Khách hàng **không sửa file skill**; tùy biến bằng bảng `PostingRule` (skill đọc qua `get_posting_rules`).
- Nguyên tắc: việc gì viết được thành code (khớp MST, bí danh, quy tắc) thì để matcher làm; skill/AI chỉ xử lý phần cần phán đoán.

Mẫu skill:

```markdown
---
name: purchase-invoice-mapping
draftType: PurchaseInvoice
tools: [search_partners, search_materials, get_account_info, get_posting_history, get_mapping_memory, get_posting_rules, suggest_field]
maxToolRounds: 8
---
# Gợi ý mã cho hóa đơn mua vào
Chỉ xử lý các trường resolved = null.

## Thứ tự
1. Đối tượng: search_partners theo MST. Không thấy → để trống, KHÔNG đoán.
2. Dòng hàng: get_mapping_memory (NCC + tên hàng) → search_materials → get_posting_history.
3. Tài khoản: get_posting_rules → lịch sử → mặc định: hàng hóa 156, NVL 152, CCDC 153,
   dịch vụ dùng ngay (điện, nước, cước, sửa chữa nhỏ) 642/641 không qua kho.
4. Mỗi gợi ý: suggest_field kèm confidence và reason nêu căn cứ.

## Cấm
- Không tạo mã mới, không sửa dữ liệu gốc.
- confidence < 0.6 → không gợi ý.
```

## 5. Quy trình bên trong `PostingGateway.PostAsync(draftId)`

```
1. Load draft từ IDraftStore. Status phải = Approved. Khóa draft (tránh ghi 2 lần).
2. DraftValidationService.Validate(draft)        → còn Error  ⇒ dừng, Status=NeedsReview.
3. DuplicateDetector.Check(draft)                → nghi trùng ⇒ dừng, trừ khi người dùng đã xác nhận "vẫn ghi".
4. MasterData: mọi ProposedMasterData chưa approved ⇒ dừng.
5. builder = IDraftVoucherBuilderFactory.Get(draft.DraftType)
   voucher = builder.Build(draft)                 (JournalVoucherEntity + JournalEntry + *Details, trên RAM)
6. voucher.VoucherNo = IVoucherNumberService.Next(...) nếu trống.
7. IVoucherValidator.Validate(voucher)           → lỗi ⇒ dừng, Status=Failed, ghi issues.
8. Transaction:
     IVoucherPostingService.Save(voucher)
     Ghi ngược nguồn: TaxInvoiceEntity.VoucherId = voucher.Id, ProcessStatus = "Posted"
   Commit.
9. Draft: Status=Posted, PostedVoucherId, audit.
10. MappingMemoryStore.Learn(draft)              (chỉ học các trường method=User/Ai đã được duyệt)
11. Trả PostingResult.
```

`PostManyAsync(draftIds)`: mỗi draft một transaction riêng; lỗi một cái không làm hỏng cái khác; trả báo cáo tổng.

## 6. Bảng cấu hình nghiệp vụ (người dùng tự sửa được)

### 6.1 Mặc định theo loại draft (`DraftTypeDefaults`)
TK công nợ mặc định (331/131), TK thuế (1331/3331), nguồn chứng từ, kho mặc định.

### 6.2 Quy tắc hạch toán (`PostingRule`)

| Cột | Ví dụ |
|---|---|
| `Priority` | 10 |
| `DraftType` | PurchaseInvoice |
| `PartnerId` (tùy chọn) | PETRO01 |
| `ItemNameContains` / `CategoryId` (tùy chọn) | "xăng", "dầu" |
| `→ DebitAccountId` | 6418 |
| `→ CostObjectId`, `WarehouseId` (tùy chọn) | |
| `RequireApprovalAboveAmount` | 20.000.000 |

Đây chính là "skill" mức khách hàng: kế toán trưởng khai báo, AI và matcher cùng đọc.

## 7. Lộ trình triển khai

| Giai đoạn | Nội dung | Kết quả kiểm tra được |
|---|---|---|
| **P0 – Tách cổng** | `VoucherValidator`, `VoucherNumberService`, `VoucherPostingService` ra Core; `VoucherEntryWindow` gọi lại. Chuyển logic sinh chùm NM/XB từ `MaterialDetailWindow` về builder. | Nhập tay chạy y như cũ; có unit test cho validator. |
| **P1 – Lớp đệm thủ công** | Contracts `VoucherDraft`, `JsonFileDraftStore`, `DraftLifecycleService`, `TaxInvoiceDraftSource`, `PartnerMatcher`, `DuplicateDetector`, `DraftValidationService`, `PurchaseInvoiceVoucherBuilder`, `PostingGateway`, `DraftInboxWindow`, `DraftWorkbookRenderer`/`Reader`/`Package` + chế độ Draft của Office Studio. | Lấy hóa đơn đầu vào → mapping MST → người dùng chọn vật tư/TK bằng tay → ghi sổ. **Chưa có AI.** |
| **P2 – Học & quy tắc** | `MaterialMatcher`, `MappingMemoryStore`, `PostingHistoryService`, `PostingRuleEngine`, `ExcelDraftSource`, `DraftImportService`. | Tháng thứ hai, phần lớn dòng hàng của NCC quen tự map, không cần AI. |
| **P3 – AI gợi ý** | `AiToolRegistry`, `AiToolLoopRunner`, `AiFieldSuggester`, `SkillLoader`/`SkillSelector`/`SkillPromptComposer`, skill `purchase-invoice-mapping`. | Đo tỷ lệ gợi ý AI được chấp nhận không sửa; chỉ áp cho trường còn trống. |
| **P4 – Mở rộng** | `SalesInvoiceVoucherBuilder`, `TaxInvoiceReconciliationService`, `AiChatDraftSource`, `OcrDraftSource`, `DbDraftStore`, sao kê ngân hàng. | |

## 8. Quy tắc cho AI-coding khi làm các phần trên

- KHÔNG gọi `adapter.SaveEntity` cho chứng từ ngoài `VoucherPostingService`. KHÔNG gọi cho Partner/InventoryItem ngoài `MasterDataGateway` (riêng các form danh mục hiện có giữ nguyên).
- KHÔNG để service AI tham chiếu `IPostingGateway`, `IMasterDataGateway`, `IVoucherPostingService`. Nếu cần, đó là thiết kế sai.
- KHÔNG sửa `raw` trong draft. Mọi thay đổi ghi vào `resolved` + `match` + `audit`.
- KHÔNG đặt giá trị mặc định viết cứng (mã kho, mã TK) trong code; lấy từ `DraftTypeDefaults` / `PostingRule`.
- Service Core không hiện MessageBox; trả `DraftIssue` để UI hiển thị.
- Matcher/validator phải có unit test với dữ liệu mẫu trong `TestData/`.
- Office Studio chế độ Document (cột tự do) KHÔNG được có nút ghi sổ / import vào DB. Chỉ chế độ Draft mới có.
- Mọi tool AI phải có mô tả tiếng Việt rõ ràng và chỉ trả dữ liệu cần thiết (không trả toàn bộ hệ thống tài khoản).

## 9. Các điểm cần chủ dự án quyết định

1. Lớp đệm lưu file JSON (`JsonFileDraftStore`) hay bảng `StagingDraft` ngay từ đầu? (Nhiều người dùng trên SQL Server → nên dùng bảng.)
2. Draft ghi sổ trực tiếp từ Office Studio, hay bắt buộc mở qua `VoucherEntryWindow` để xem định khoản rồi mới lưu?
3. Hóa đơn dịch vụ (không qua kho): quy tắc chọn giữa Nợ 15x + kho và Nợ 6xx + CostDetail.
4. Ngưỡng tiền cần duyệt cấp hai (giám đốc) có cần ngay từ P1 không.
