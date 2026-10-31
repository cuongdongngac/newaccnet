# NEWACC — Kiến trúc AI Agent, Lớp đệm (Staging) & Cổng ghi sổ (Posting Gateway)

_Phiên bản: 2.0 — 30/10/2026._
_Gộp và thay thế: `IntergrateAI.md` (AI Support & Reporting Integration) và `StagingGateway_Architecture.md` v1.1._
_Bổ sung mới: tách **Agent** thành khối riêng theo chuẩn .NET (Microsoft Agent Framework + Microsoft.Extensions.AI + MCP), ranh giới phụ thuộc được khóa bằng test, bộ đánh giá (Evals)._

> **Tài liệu định hướng cho AI-coding.** Đọc file này trước khi viết bất kỳ code nào liên quan tới: AI, trợ lý chat, nhập hóa đơn từ cổng thuế, import Excel, OCR, Office Studio, báo cáo OpenXML.
> Nếu một yêu cầu mâu thuẫn với **Mục 1 (Nguyên tắc bất biến)** hoặc **Mục 3 (Quy tắc phụ thuộc)** → dừng lại và hỏi chủ dự án, không tự xử lý.

---

## 0. Bối cảnh

### 0.1 Hệ thống
- **Dự án:** NEWACC — phần mềm kế toán desktop C# **.NET 9 WPF**.
- **Dữ liệu:** Accounting-First, bảng giao dịch phẳng (flat transaction tables với `ParentId`).
- **ORM:** LLBLGen Pro (multi-database: Access, SQL Server, PostgreSQL). DB-agnostic: không stored procedure, không view.
- **Giao diện:** DevExpress WPF (`ThemedWindow`, theme Office2019Colorful), MVVM.
- **Mô hình AI:** qua **OpenRouter** (một API, nhiều model).

### 0.2 Vấn đề của cách làm cũ ("mixed")
- AI (`NewaccNet.AI/AccountingAgentService`) chỉ "gọi một lần": prompt → JSON → đẩy thẳng vào `VoucherEntryWindow`. Không tra cứu được đối tượng, vật tư, lịch sử; thiếu bảng chi tiết (`DebtDetail`, `InventoryVoucherLine`, `VatEntry`...).
- Logic kiểm tra & lưu chứng từ nằm trong code-behind `VoucherEntryWindow.xaml.cs` (`BtnSave_Click`, `SaveSingleVoucher`) → nguồn khác không dùng lại được.
- Code AI, code nghiệp vụ và code giao diện trộn lẫn: không thể giao phần AI cho người/công cụ khác phát triển độc lập, không đo được chất lượng AI.
- Dữ liệu từ ngoài vào (hóa đơn cổng thuế, Excel, OCR, AI) cần **rà soát, đối chiếu, mapping mã** trước khi thành chứng từ thật.

### 0.3 Hướng mới (tóm tắt một câu)
**Core kế toán quyết định — Agent chỉ gợi ý.** Agent là một khối .NET riêng, chỉ làm việc với Core qua **danh sách tool** và **hợp đồng dữ liệu** (`NewaccNet.Contracts`), không bao giờ chạm vào DB hay cổng ghi sổ.

---

## 1. Nguyên tắc bất biến (KHÔNG được vi phạm)

1. **AI không bao giờ ghi sổ.** AI chỉ được đọc dữ liệu và **đề xuất** vào bản nháp. Người dùng luôn là người bấm nút ghi sổ.
2. **Một cổng duy nhất vào DB thật.** Mọi chứng từ (nhập tay, import, AI, hóa đơn cổng thuế) đều đi qua `IPostingGateway`. Không service nào khác được gọi `adapter.SaveEntity` lên `JournalVoucherEntity` và các bảng chi tiết.
3. **Lớp đệm là định dạng chuẩn.** Mọi nguồn dữ liệu đều chuyển về định dạng trung gian `VoucherDraft` (JSON). Cổng chỉ nhận `VoucherDraft`, không cần biết dữ liệu đến từ đâu.
4. **Chuyện nào ra chuyện đấy.** Thu thập → Chuẩn hóa → Đối chiếu/Mapping → Rà soát → Ghi sổ là các service tách biệt, mỗi service làm đúng một việc, chạy độc lập được.
5. **Thủ công và AI dùng chung đường ống.** AI chỉ là một "người gợi ý" cắm vào khâu Mapping. Tắt AI thì toàn bộ quy trình vẫn chạy bằng tay.
6. **Tất định trước, AI sau, người cuối cùng.** Mapping luôn chạy quy tắc tất định (MST, mã, bí danh đã học, quy tắc hạch toán) trước. AI chỉ xử lý phần còn lại. Người dùng xác nhận tất cả.
7. **Agent là khối tách rời.** Agent chỉ thấy Core qua tool + Contracts. Ranh giới được khóa bằng test kiến trúc (Mục 3.3), không chỉ bằng quy ước.
8. **Mô hình AI không tự tính tiền.** Mọi con số (số dư, tổng, thuế) phải lấy từ tool/Core. Mô hình chỉ diễn giải và gợi ý mã.
9. **Không có model/mã viết cứng.** Tên model, mã TK, mã kho... lấy từ cấu hình (`AgentProfiles`, `DraftTypeDefaults`, `PostingRule`).
10. Giữ nguyên quy tắc dự án: DB-agnostic, LLBLGen, không sửa `dataaccess/`, MVVM, DevExpress WPF.

---

## 2. Nền tảng công nghệ (chuẩn .NET)

| Thành phần | Package / công nghệ | Dùng ở đâu |
|---|---|---|
| Trừu tượng LLM | `Microsoft.Extensions.AI` (`IChatClient`, `AIFunction`) | Agent |
| Kết nối OpenRouter | `Microsoft.Extensions.AI.OpenAI` (OpenAI-compatible, `Endpoint = https://openrouter.ai/api/v1`) | Agent |
| Khung agent | **Microsoft Agent Framework 1.x** (`Microsoft.Agents.AI`) — agent, middleware, skill, workflow | Agent |
| Giao thức tool | **MCP C# SDK** (`ModelContextProtocol`) — server ở phía NEWACC, client ở phía Agent | Tools, McpServer, Agent |
| Excel/Word | `DocumentFormat.OpenXml` | Office |
| Hiển thị | `DevExpress.Wpf.Spreadsheet`, `DevExpress.Wpf.RichEdit` (đã có) | Wpf |
| Quan sát | OpenTelemetry (MAF và M.E.AI có sẵn hook) | Agent |
| Test kiến trúc | `NetArchTest.Rules` (hoặc tương đương) | ArchitectureTests |

Ghi chú cho AI-coding:
- Dùng phiên bản **ổn định** mới nhất của MAF và MCP C# SDK tại thời điểm code; **đọc tài liệu/sample của đúng phiên bản** trước khi viết, không đoán tên API từ trí nhớ.
- Không dùng Semantic Kernel/AutoGen cho code mới (MAF là bản kế nhiệm).
- API key OpenRouter: dev dùng User Secrets; bản cài đặt lưu mã hóa (DPAPI theo người dùng Windows). Không bao giờ nằm trong mã nguồn hay file cấu hình dạng rõ.

---

## 3. Cấu trúc solution & quy tắc phụ thuộc

### 3.1 Sơ đồ khối

```
┌──────────────────────────── NEWACC (đội kế toán) ────────────────────────────┐
│                                                                              │
│  NewaccNet.Wpf  (composition root, UI, Office Studio, DraftInbox)            │
│       │                                                                      │
│       ├── NewaccNet.Office   (OpenXML render/đọc, MemoryStream — tất định)   │
│       ├── NewaccNet.Core     (Posting, Staging, Ingestion, Matching, Recon)  │
│       └── NewaccNet.Tools    (bọc service Core thành tool — KHÔNG có tool    │
│                               ghi sổ, KHÔNG có tool tạo danh mục)            │
│                                                                              │
│  NewaccNet.McpServer.exe  (phát NewaccNet.Tools qua MCP/stdio — dev, test,   │
│                            eval, client ngoài)                               │
└───────────────────────────────────┬──────────────────────────────────────────┘
                                    │  HỢP ĐỒNG
                                    │  • NewaccNet.Contracts (POCO, interface)
                                    │  • Danh sách tool (tên + JSON Schema)
┌───────────────────────────────────┴───────────────────── (khối AI) ──────────┐
│  NewaccNet.Agent         (MAF: agent, skill, middleware, profile model)      │
│  NewaccNet.Agent.Skills  (thư mục *.md — skill nghiệp vụ)                    │
│  NewaccNet.Agent.Evals   (bộ dữ liệu chuẩn + chấm điểm)                      │
└──────────────────────────────────────────────────────────────────────────────┘
  NewaccNet.ArchitectureTests  — khóa các quy tắc ở 3.2
```

### 3.2 Bảng phụ thuộc (project reference)

| Project | Được tham chiếu | CẤM tham chiếu |
|---|---|---|
| `NewaccNet.Contracts` | BCL, `System.Text.Json` | Mọi project khác, LLBLGen, DevExpress, M.E.AI |
| `NewaccNet.Core` | Contracts, LLBLGen (`dataaccess/`) | Agent, Tools, Office, Wpf, DevExpress, M.E.AI |
| `NewaccNet.Tools` | Contracts, Core, `Microsoft.Extensions.AI.Abstractions`, `ModelContextProtocol` | Agent, Wpf |
| `NewaccNet.Office` | Contracts, `DocumentFormat.OpenXml` | Core.Posting, Agent, LLBLGen |
| `NewaccNet.McpServer` | Contracts, Core, Tools | Agent, Wpf |
| `NewaccNet.Agent` | **Chỉ** Contracts, M.E.AI, MAF, MCP client | **Core, Tools, Office, Wpf, LLBLGen, `dataaccess/`** |
| `NewaccNet.Agent.Evals` | Agent, Contracts, MCP client | Core.Posting |
| `NewaccNet.Wpf` | Tất cả (chỉ để đăng ký DI) | — |

Trong `NewaccNet.Wpf`, **chỉ composition root** (`App.xaml.cs`/`Bootstrapper`) được nhắc tới kiểu cụ thể của Agent. ViewModel chỉ dùng interface trong Contracts (`IAgentService`, `IFieldSuggester`).

### 3.3 Test kiến trúc (bắt buộc, chạy trong CI)

`NewaccNet.ArchitectureTests` phải fail khi:
1. `NewaccNet.Agent` tham chiếu `NewaccNet.Core`, `NewaccNet.Tools`, LLBLGen hoặc namespace `*.Posting`, `*.MasterData`.
2. Bất kỳ kiểu nào ngoài `VoucherPostingService` gọi `SaveEntity`/`SaveEntityCollection` trên `JournalVoucherEntity` và bảng chi tiết.
3. `NewaccNet.Tools` chứa tool tham chiếu `IPostingGateway`, `IMasterDataGateway`, `IVoucherPostingService`.
4. `NewaccNet.Core` tham chiếu `System.Windows`/DevExpress (service Core không hiện MessageBox).
5. Một tool có `Access` khác `Read` hoặc `DraftWrite` (không tồn tại mức `Post`).

---

## 4. Định dạng chuẩn `VoucherDraft` (JSON)

### 4.1 Vòng đời trạng thái (`DraftStatus`)

| Trạng thái | Ý nghĩa | Ai chuyển |
|---|---|---|
| `Imported` | Vừa chuyển từ nguồn về, chưa mapping | `IDraftSource` |
| `Matched` | Đã chạy mapping, có thể còn dòng chưa xác định | `DraftMatchingService` |
| `NeedsReview` | Còn lỗi/thiếu mã/nghi trùng | `DraftMatchingService` / `DraftValidationService` |
| `Approved` | Người dùng đã duyệt, sẵn sàng ghi sổ | Người dùng (UI) |
| `Posted` | Đã ghi sổ, có `PostedVoucherId` | `IPostingGateway` |
| `Rejected` | Bỏ qua (không hạch toán) | Người dùng |
| `Failed` | Ghi sổ lỗi, xem `Issues` | `IPostingGateway` |

Chỉ `IPostingGateway` được chuyển sang `Posted`. AI không được chuyển trạng thái quá `Matched`. Máy trạng thái nằm trong **Core** (`DraftLifecycleService`), không nằm trong workflow của Agent.

### 4.2 Cấu trúc (ví dụ hóa đơn mua hàng)

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
    { "at": "2026-10-29T10:01:05+07:00", "by": "ai:mapping/<model-id>", "action": "SuggestField", "detail": "line 2 materialId", "runId": "..." }
  ],

  "postedVoucherId": null
}
```

Quy ước:
- Mỗi trường mapping có ba phần: `raw` (dữ liệu gốc, KHÔNG sửa), `resolved` (mã trong hệ thống), `match` (nguồn gốc + độ tin cậy + lý do). UI hiển thị cả ba.
- `match.method` ∈ `TaxCodeExact | CodeExact | AliasMemory | PostingRule | History | Fuzzy | Ai | User`.
- Tiền tệ dùng `decimal`; chuyển kiểu khi build entity.
- Draft **không chứa định khoản Nợ/Có**. Định khoản do `IDraftVoucherBuilder` sinh theo `draftType`. Ngoại lệ: `GeneralJournal` cho phép khai báo dòng Nợ/Có trực tiếp.
- `audit` của AI ghi `profile/model-id` và `runId` để truy ngược log Agent.
- Có `schemaVersion` để nâng cấp định dạng.

### 4.3 Các `DraftType`

| DraftType | Nguồn chính | Builder sinh ra |
|---|---|---|
| `PurchaseInvoice` | Hóa đơn đầu vào cổng thuế, OCR | NM: Có 331 / Nợ 15x(+InventoryVoucherLine) / Nợ 1331(+VatEntry) — hoặc Nợ 6xx + CostDetail nếu là dịch vụ |
| `SalesInvoice` | Hóa đơn đầu ra cổng thuế | XB: Nợ 131 / Có 511 / Có 3331 (+ giá vốn 632/156 nếu có kho) |
| `CashReceipt` / `CashPayment` | AI chat, Excel | PT / PC |
| `BankReceipt` / `BankPayment` | Sao kê ngân hàng (giai đoạn sau) | BC / BN |
| `GeneralJournal` | Excel, AI chat | Dòng Nợ/Có khai báo trực tiếp |

---

## 5. Luồng tổng thể

```
 NGUỒN                   THU THẬP             LỚP ĐỆM (Staging)                       CỔNG           DB THẬT
 ─────                   ────────             ─────────────────                       ────           ───────
 Cổng thuế (XML/JSON) ─┐
 Excel / CSV          ─┼─► IDraftSource ─► VoucherDraft (JSON) ─► Matching ─► Review ─► IPostingGateway ─► JournalVoucher
 OCR ảnh/PDF          ─┤   (adapter)        Status=Imported      (rule→AI)   (người)   (validate+build   JournalEntry
 Trợ lý AI (chat)     ─┤                                          Status=      Status=   +save 1 trans)    *Details
 Sao kê NH (sau này)  ─┘                                          Matched      Approved  Status=Posted     VatEntry...
                                                  ▲                                                 │
                                   IFieldSuggester│ (Agent, chỉ gợi ý)                              │
 Nhập tay trên VoucherEntryWindow ──────────────────────────────────────────────────────────────────┘
 (cũng đi qua IVoucherValidator + IVoucherPostingService của cổng)
```

**Màn hình kiểm duyệt duy nhất là Office Studio** (`OfficeStudioWindow`, OpenXML + DevExpress Spreadsheet). Mọi nguồn — kể cả hóa đơn cổng thuế — đều được hiển thị lên đó để người dùng xem "mặt mũi" chứng từ, sửa mã, rồi mới duyệt. Dữ liệu thật là `VoucherDraft` JSON nằm bên trong file (Mục 7.2).

Dữ liệu thô vẫn giữ ở nơi riêng (ví dụ `TaxInvoiceEntity` / `TaxInvoiceDetailEntity`). Lớp đệm chỉ tham chiếu qua `SourceRef`.

---

## 6. Core kế toán (đội kế toán sở hữu)

### 6.1 `NewaccNet.Contracts` — Mô hình & interface dùng chung

| Kiểu | Mô tả |
|---|---|
| `VoucherDraft`, `DraftHeader`, `DraftCounterparty`, `DraftLine`, `DraftSettlement` | POCO khớp JSON 4.2 |
| `RawValue<T>` / `ResolvedValue` / `MatchInfo` | Bộ ba raw–resolved–match |
| `DraftStatus`, `DraftType`, `MatchMethod`, `IssueSeverity` | Enum |
| `DraftIssue` | `Severity`, `Code`, `Field`, `Message` |
| `ProposedMasterData` | Đề xuất tạo mới đối tượng/vật tư, cần người duyệt |
| `PostingResult` | `Success`, `VoucherId`, `VoucherNo`, `Issues` |
| `DraftSerializer` | System.Text.Json, kiểm tra `schemaVersion` |
| `DynamicSpreadsheetDto`, `DynamicWordDocumentDto`, `ReportQueryDto` | Đầu ra Office Studio chế độ Document / truy vấn báo cáo |
| **`IAgentService`** | Cổng gọi Agent từ phía ứng dụng (Mục 8.2) |
| **`IFieldSuggester`** | Điểm cắm AI vào khâu Matching |
| **`IAgentToolProvider`** | Cung cấp danh sách tool cho Agent (in-process hoặc MCP) |
| `AiToolAccess` (`Read`, `DraftWrite`) | **Không tồn tại mức `Post`** |

### 6.2 `NewaccNet.Core/Posting` — CỔNG GHI SỔ (làm đầu tiên)

| Service | Trách nhiệm |
|---|---|
| `IVoucherValidator` / `VoucherValidator` | Kiểm tra **entity** trước khi lưu: cân đối Nợ=Có, TK tồn tại & là TK chi tiết, TK cần chi tiết thì phải có chi tiết (theo CategoryId), khóa sổ/hạn dùng (`DbConfigService.CanUpdateVoucher`), hạn mức bản quyền (`CheckRecordLimit`). Trả `List<DraftIssue>`, không hiện MessageBox. |
| `IVoucherNumberService` / `VoucherNumberService` | Sinh số chứng từ theo nguồn CT (chuyển từ `VoucherEntryWindow.GenerateNextVoucherNo`). |
| `IVoucherPostingService` / `VoucherPostingService` | Lưu lô `JournalVoucherEntity` trong 1 transaction (chuyển `SaveSingleVoucher` + phần diff/xóa chi tiết từ code-behind sang). |
| `IDraftVoucherBuilder` (+ `PurchaseInvoiceVoucherBuilder`, `SalesInvoiceVoucherBuilder`, `CashVoucherBuilder`, `GeneralJournalVoucherBuilder`) | `VoucherDraft` đã resolve → `JournalVoucherEntity` đầy đủ chi tiết (DebtDetail, InventoryVoucher/Line, MaterialTaxLine, VatEntry, CostDetail). Theo mẫu `CostAllocationVoucherBuilder`. Logic sinh chùm NM/XB đang ở `MaterialDetailWindow` phải chuyển về đây. |
| `IDraftVoucherBuilderFactory` | Chọn builder theo `DraftType`. |
| **`IPostingGateway` / `PostingGateway`** | **Cổng duy nhất.** Quy trình Mục 6.7. |
| `IMasterDataGateway` / `MasterDataGateway` | Tạo mới đối tượng/vật tư từ `ProposedMasterData` đã duyệt. Không cho tạo trùng MST/mã. |

Sau khi có các service này, `VoucherEntryWindow.BtnSave_Click` chỉ còn: gom UI → `IVoucherValidator` → hiện lỗi → `IVoucherPostingService`.

### 6.3 `NewaccNet.Core/Staging` — Lưu trữ lớp đệm

| Service | Trách nhiệm |
|---|---|
| `IDraftStore` | `Save`, `Get`, `Query(status, type, source, from, to)`, `Delete`, `ExistsBySourceRef`. |
| `JsonFileDraftStore` | Mặc định: mỗi draft 1 file `.json` trong `Staging/{yyyy-MM}/`. Phù hợp Access một người dùng. |
| `DbDraftStore` (sau) | Bảng `StagingDraft(Id, DraftType, Status, SourceKind, SourceRef, PayloadJson, CreatedAt, UpdatedAt, PostedVoucherId)`. Cần cho SQL Server/PostgreSQL nhiều người dùng. |
| `DraftLifecycleService` | Chuyển trạng thái hợp lệ (4.1), ghi `audit`. Chặn chuyển sai (ví dụ AI → Approved). |
| `DraftExportService` / `DraftImportService` | Xuất/nhập draft JSON/Excel để sửa ngoài hoặc chuyển máy. |

### 6.4 `NewaccNet.Core/Ingestion` — Thu thập (mỗi nguồn 1 adapter)

| Service | Trách nhiệm |
|---|---|
| `IDraftSource` | `Task<IReadOnlyList<VoucherDraft>> CreateDraftsAsync(DraftSourceRequest req)` |
| `TaxInvoiceDraftSource` | `TaxInvoiceEntity` + `TaxInvoiceDetailEntity` (đã tải bằng `NewaccNet.TaxInvoice`) → `PurchaseInvoice`/`SalesInvoice`. Bỏ qua hóa đơn đã có `VoucherId` hoặc đã có draft cùng `SourceRef`. |
| `ExcelDraftSource` | Import theo mẫu Excel chuẩn (`Template/`). |
| `OcrDraftSource` | Gọi `IAgentService.ExtractDocumentAsync` (Agent, vision) → dữ liệu thô → draft. Chỉ cho chứng từ giấy/ảnh; hóa đơn điện tử ưu tiên `TaxInvoiceDraftSource`. |
| `AiChatDraftSource` | Câu lệnh tự nhiên → `IAgentService.ChatToDraftAsync` → draft `Imported`. Thay cho đường cũ `ParseNaturalLanguageJournalAsync` → `VoucherEntryWindow`. |
| `BankStatementDraftSource` | Giai đoạn sau. |

Adapter **chỉ chuyển dạng**, không mapping mã. Kết quả từ Agent ở bước này cũng chỉ là `raw` — mapping vẫn chạy ở 6.5.

### 6.5 `NewaccNet.Core/Matching` — Đối chiếu & Mapping TẤT ĐỊNH (không AI)

| Service | Trách nhiệm |
|---|---|
| `DraftMatchingService` | Điều phối các matcher bên dưới, rồi `DraftValidationService`. Cuối cùng (nếu bật AI) gọi `IFieldSuggester` cho các trường còn trống. |
| `PartnerMatcher` | MST trùng tuyệt đối `PartnerEntity.Taxno` → confidence 1.0. Không có MST → so tên chuẩn hóa (bỏ dấu, bỏ "CÔNG TY TNHH"...). Không thấy → `ProposedMasterData`. |
| `MaterialMatcher` | `AliasMemory` → mã trùng `MaterialId` → tên trùng tuyệt đối → fuzzy (chỉ gợi ý, confidence < 0.7). |
| `PostingRuleEngine` | Áp bảng quy tắc hạch toán (Mục 10.2). |
| `PostingHistoryService` | Đọc `FlattenedDetailService` (Inventory/Cost/Vat/Debt flat rows): lần trước NCC/vật tư này hạch toán TK/kho/đối tượng CP nào. Chỉ đọc. |
| `MappingMemoryStore` | Bí danh đã xác nhận: `(PartnerId, RawItemName/RawItemCode) → MaterialId, AccountId, WarehouseId`. Ghi **sau khi** ghi sổ thành công (học từ dữ liệu đã duyệt, không học từ đề xuất). |
| `DuplicateDetector` | Nghi trùng: `TaxInvoiceEntity.VoucherId` đã có; `VatEntry` trùng (`Vatbillno` + `Taxno` + `Vatbilldate`); draft khác cùng `SourceRef`. |
| `DraftValidationService` | Kiểm tra **draft**: thiếu mã, mã không tồn tại, tổng dòng ≠ tổng hóa đơn, thuế suất lệch, nghi trùng → `issues`, đặt `NeedsReview`. |

### 6.6 `NewaccNet.Core/Reconciliation`

| Service | Trách nhiệm |
|---|---|
| `TaxInvoiceReconciliationService` | So hóa đơn cổng thuế với `VatEntry` theo kỳ: có trên cổng chưa có trong sổ, ngược lại, lệch tiền/thuế, hóa đơn hủy/điều chỉnh. Cập nhật `TaxInvoiceEntity.ProcessStatus` / `ReconcileNote`. Không tạo chứng từ. |

### 6.7 Quy trình `PostingGateway.PostAsync(draftId)`

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

---

## 7. Office & giao diện

### 7.1 `NewaccNet.Office` — Sinh/đọc OpenXML trên RAM (tất định, không AI)

Chuyển từ `NewaccNet.AI` (cũ) sang project riêng vì đây **không phải AI**.

| Service | Trách nhiệm |
|---|---|
| `ReportGeneratorService` (trước là `OpenXmlReportService`) | DTO báo cáo / `DynamicSpreadsheetDto` / `DynamicWordDocumentDto` → `MemoryStream` xlsx/docx. |
| `DraftWorkbookRenderer` | `VoucherDraft` → `MemoryStream` xlsx theo mẫu `Template/Draft_{DraftType}.xlsx`. |
| `DraftWorkbookReader` | Đọc ngược workbook đã sửa → cập nhật `resolved` (+ `match.method = User`, `audit`). Đọc theo **tên cột cố định / named range**, không đoán. |
| `DraftWorkbookPackage` | Đọc/ghi JSON nháp vào **`CustomXmlPart`** trong xlsx: một file vừa có trang tính cho người, vừa có JSON chuẩn cho máy. |
| `DraftLookupListProvider` | Danh sách mã (vật tư, TK chi tiết, kho, đối tượng) → `DataValidation` dạng list (dropdown trong ô). Nhận dữ liệu qua interface, không gọi LLBLGen trực tiếp. |

Ràng buộc Zero-Disk I/O:
- **TUYỆT ĐỐI KHÔNG** dùng `FileStream` khi tạo Workbook/Document. Toàn bộ trên `MemoryStream`.
- Sau khi tạo xong: `stream.Seek(0, SeekOrigin.Begin)`.
- Chỉ ghi đĩa khi người dùng bấm **Save As** (`SaveFileDialog`).

### 7.2 Office Studio — hai chế độ, không trộn lẫn

| Chế độ | Đầu ra | Cột | Vào DB |
|---|---|---|---|
| **Document** | Đơn thuốc, chấm công, biên bản, phân tích... (`DynamicSpreadsheetDto` / `DynamicWordDocumentDto` do Agent sinh) | Tự do, AI quyết định | **Không bao giờ** |
| **Draft** | Nháp chứng từ (`VoucherDraft`) | **Cố định theo mẫu** của từng `DraftType` | Có, chỉ qua `IPostingGateway` |

Hiển thị:
- `ThemedWindow` chứa `SpreadsheetControl` (Excel) hoặc `RichEditControl` (Word).
- ViewModel nhận `MemoryStream` → `spreadsheetControl.LoadDocument(stream, DocumentFormat.Xlsx)`.
- Nút **Save As** bọc `SaveFileDialog`.

Mẫu trình bày chế độ Draft:
- Sheet `Header`: thông tin hóa đơn gốc — **khóa**, nền xám.
- Sheet `Lines`: cột gốc (khóa, xám) + cột mã (`MaterialId`, `WarehouseId`, `DebitAccountId`, `CostObjectId`) mở để sửa + `Nguồn gợi ý` + `Độ tin cậy` + `Lý do`.
- Màu cột mã theo `match.method`: xanh = TaxCode/Alias/Rule, vàng = History/Fuzzy, tím = AI, đỏ = còn trống.
- Sheet `Issues`: danh sách `DraftIssue`.
- Sheet `Preview` (tùy chọn): chùm định khoản Nợ/Có do builder sinh thử — chỉ để xem.
- Khi mở lại: đọc `CustomXmlPart` trước; ô trên sheet khác JSON → coi là người dùng sửa.

Nút chế độ Draft: **Lưu nháp** (`IDraftStore`), **Duyệt**, **Bỏ qua**, **Xem định khoản** (build trên RAM, không lưu), **Ghi sổ** (`IPostingGateway`). Chế độ Document KHÔNG có nút ghi sổ/import vào DB.

### 7.3 `NewaccNet.Wpf` — Màn hình

| Màn hình | Chức năng |
|---|---|
| `DraftInboxWindow` | Danh sách draft theo trạng thái/nguồn/kỳ. Nút: Lấy từ cổng thuế, Import Excel/JSON, Chạy mapping, Gợi ý AI (bật/tắt), Ghi sổ hàng loạt (chỉ `Approved`, không còn Error). Double-click → Office Studio chế độ Draft. |
| `OfficeStudioWindow` | Mục 7.2. |
| `VoucherEntryWindow` | Tùy chọn "Mở trên phiếu" khi cần chỉnh định khoản sâu (`LoadFromAiVoucher`). Lưu vẫn qua cổng. |
| `AiChatPanel` | Ô chat trợ lý: gọi `IAgentService.ChatAsync`. Kết quả dạng draft → vào `DraftInbox`; dạng tài liệu → Office Studio chế độ Document. |
| `PostingRuleListView` | Danh mục quy tắc hạch toán (Inline Edit theo chuẩn danh mục). |
| `MappingMemoryListView` | Xem/xóa bí danh đã học. |
| `AgentSettingsView` | Bật/tắt AI, chọn profile model, nhập API key (lưu DPAPI), bật che dữ liệu nhạy cảm. |

---

## 8. Ranh giới Core ↔ Agent

### 8.1 `NewaccNet.Tools` — Tool là bề mặt duy nhất Agent nhìn thấy

- Mỗi tool **bọc đúng một service Core** đã có (Mục 6.5/6.3), không chứa logic nghiệp vụ riêng.
- **Một nguồn định nghĩa duy nhất** cho mỗi tool (tên snake_case, mô tả tiếng Việt, JSON Schema tham số, `AiToolAccess`). Từ đó sinh ra:
  - danh sách `AIFunction` (Microsoft.Extensions.AI) cho chế độ **in-process**;
  - các tool đăng ký vào **MCP server** (`NewaccNet.McpServer`).
  Hai đường phải trả **cùng một kết quả** — có test so sánh.
- `ToolCatalog`: tra cứu tool theo tên, lọc theo danh sách mà skill cho phép.
- Tool trả JSON **gọn, chỉ dữ liệu cần thiết** (không trả toàn bộ hệ thống tài khoản, không trả cả bảng). Có giới hạn số dòng.
- Tool `DraftWrite` chỉ ghi vào `resolved` + `match(method=Ai)` + `audit` qua `IDraftStore` + `DraftLifecycleService`. Không đổi trạng thái quá `Matched`, không sửa trường đã có `method` khác.

Bộ tool khởi điểm:

| Tool | Access | Bọc service |
|---|---|---|
| `search_partners` | Read | `PartnerMatcher` |
| `search_materials` | Read | `MaterialMatcher` |
| `get_account_info` | Read | ChartOfAccount (CategoryId, có phải TK chi tiết) |
| `get_posting_history` | Read | `PostingHistoryService` |
| `get_mapping_memory` | Read | `MappingMemoryStore` |
| `get_posting_rules` | Read | `PostingRuleEngine` |
| `get_draft` | Read | `IDraftStore` (chỉ trả trường cần thiết, ưu tiên trường còn trống) |
| `suggest_field` | DraftWrite | `IDraftStore` + `DraftLifecycleService` |

**Không bao giờ có tool** gọi `IPostingGateway`, `IMasterDataGateway`, `IVoucherPostingService`, hay chạy SQL/LLBLGen tự do.

### 8.2 `IAgentService` (Contracts) — Ứng dụng gọi Agent qua đây

```csharp
public interface IAgentService
{
    // Gợi ý mã cho các trường còn trống của draft (ghi qua tool suggest_field)
    Task<AgentRunResult> SuggestDraftFieldsAsync(Guid draftId, CancellationToken ct);

    // Giải thích gợi ý / tóm tắt vấn đề của draft cho người dùng
    Task<string> ExplainDraftAsync(Guid draftId, CancellationToken ct);

    // Câu lệnh tự nhiên → dữ liệu thô cho AiChatDraftSource (chưa mapping)
    Task<ChatDraftResult> ChatToDraftAsync(string userText, CancellationToken ct);

    // Office Studio chế độ Document
    Task<DocumentResult> GenerateDocumentAsync(string userText, CancellationToken ct);

    // OCR/vision: ảnh/PDF → dữ liệu thô hóa đơn
    Task<ExtractedDocument> ExtractDocumentAsync(Stream content, string mimeType, CancellationToken ct);

    // Hỏi đáp trợ lý (chỉ đọc)
    IAsyncEnumerable<AgentChatUpdate> ChatAsync(AgentChatRequest req, CancellationToken ct);
}
```

- `AgentRunResult`: `RunId`, `Profile`, `ModelId`, `ToolCalls` (tên + thời gian + kết quả tóm tắt), `TokensIn/Out`, `Cost` (nếu router trả), `Issues`.
- `IFieldSuggester` (dùng trong `DraftMatchingService`) được cài đặt bằng một adapter mỏng gọi `IAgentService.SuggestDraftFieldsAsync`.
- Tắt AI → đăng ký `NullAgentService` / `NullFieldSuggester`: mọi luồng vẫn chạy.

### 8.3 Hai chế độ kết nối tool

| Chế độ | Dùng khi | Cách nối |
|---|---|---|
| **In-process** (mặc định khi cài cho khách) | Chạy trong NEWACC.exe | Wpf (composition root) đăng ký `InProcessToolProvider` (trong `NewaccNet.Tools`) làm `IAgentToolProvider`. Agent nhận `AIFunction` qua interface, **không biết** Core tồn tại. |
| **MCP** | Phát triển, test, eval, client ngoài | `NewaccNet.McpServer.exe` (stdio) nối DB thử nghiệm; Agent dùng `McpToolProvider` (MCP client). Kiểm tra được bằng MCP Inspector hoặc client MCP bất kỳ. |

Agent phải chạy được ở cả hai chế độ mà không sửa code — chỉ đổi đăng ký DI/cấu hình.

---

## 9. `NewaccNet.Agent` — khối AI

### 9.1 Thành phần

| Thành phần | Trách nhiệm |
|---|---|
| `ChatClientFactory` | Tạo `IChatClient` theo profile: OpenAI-compatible client trỏ `https://openrouter.ai/api/v1` (hoặc endpoint nội bộ tương thích OpenAI cho model chạy cục bộ). Bọc pipeline M.E.AI: function invocation, logging, OpenTelemetry. |
| `AgentProfiles` (cấu hình) | Mỗi profile: `ModelId`, `FallbackModelIds`, `Temperature`, `MaxToolRounds`, `MaxOutputTokens`, `TimeoutSeconds`, `AllowSendPii`. Ví dụ profile: `mapping`, `chat`, `document`, `vision`. **Không viết cứng tên model trong code.** |
| `AgentFactory` | Tạo agent MAF cho từng tác vụ: instructions chung + skill + danh sách tool đã lọc. |
| `SkillLoader` / `SkillSelector` / `SkillPromptComposer` | Mục 9.3. |
| Middleware (MAF) | `ToolAllowListMiddleware` (chặn tool ngoài danh sách skill), `RoundLimitMiddleware`, `AuditMiddleware` (ghi `RunId`, từng tool call), `PiiMaskingMiddleware` (tùy chọn: che tên người, số tài khoản NH, SĐT trước khi gửi model; MST giữ nguyên vì cần để đối chiếu), `CostGuardMiddleware` (giới hạn token/ngày). |
| `AgentService` | Cài đặt `IAgentService`. |
| `McpToolProvider` | MCP client → `AIFunction` cho chế độ MCP. |

Quy tắc:
- Workflow nhiều bước của MAF chỉ dùng **bên trong** một tác vụ của Agent (ví dụ: tra đối tượng → tra lịch sử → gợi ý). Trạng thái chứng từ và việc duyệt **luôn ở Core**, không đưa vào workflow của Agent.
- Mỗi lần chạy có giới hạn vòng tool (mặc định 8) và timeout. Hết giới hạn → dừng, trả `Issues`, không thử "đoán" tiếp.
- Đầu ra có cấu trúc (draft thô, document DTO) phải là JSON khớp schema trong Contracts; parse lỗi → trả lỗi rõ ràng, không cố sửa bằng heuristic.
- Model không hỗ trợ tool calling tốt → không được dùng cho profile `mapping` (ghi rõ trong cấu hình và kiểm bằng Evals).

### 9.2 Che dữ liệu & quyền riêng tư
- Mặc định chỉ gửi model **dữ liệu của draft đang xử lý** và kết quả tool, không gửi cả sổ sách.
- Cấu hình cho phép khách chọn: model đám mây qua OpenRouter, hoặc endpoint cục bộ (tương thích OpenAI) cho dữ liệu nhạy cảm.
- Log Agent lưu cục bộ, có thời hạn xóa; không log API key; log nội dung prompt chỉ khi bật chế độ chẩn đoán.

### 9.3 Skill nghiệp vụ
- Thư mục `NewaccNet.Agent.Skills/*.md`, copy theo bản cài. Định dạng theo chuẩn Agent Skills mà MAF hỗ trợ (frontmatter + nội dung); các khóa riêng của NEWACC (`draftType`, `tools`, `maxToolRounds`, `profile`) đặt trong frontmatter.
- `SkillSelector` chọn skill **bằng code** theo `DraftType`/tác vụ (không để AI tự chọn với luồng chứng từ). Trợ lý chat tự do được phép để MAF tự nạp skill theo mô tả.
- `SkillPromptComposer`: lời dặn chung + skill + JSON nháp (**chỉ các trường còn trống**); danh sách tool lọc theo frontmatter `tools`.
- Khách hàng **không sửa file skill**; tùy biến bằng bảng `PostingRule` (skill đọc qua `get_posting_rules`).
- Nguyên tắc: việc gì viết được thành code (khớp MST, bí danh, quy tắc) thì để matcher làm; skill/AI chỉ xử lý phần cần phán đoán.

Mẫu skill:

```markdown
---
name: purchase-invoice-mapping
description: Gợi ý mã vật tư, kho, tài khoản cho các dòng còn trống của hóa đơn mua vào.
draftType: PurchaseInvoice
profile: mapping
tools: [get_draft, search_partners, search_materials, get_account_info, get_posting_history, get_mapping_memory, get_posting_rules, suggest_field]
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
- Không tự tính tiền; mọi số liệu lấy từ draft/tool.
- confidence < 0.6 → không gợi ý.
```

### 9.4 `NewaccNet.Agent.Evals` — Đo chất lượng AI (bắt buộc trước khi bật AI cho khách)

- **Bộ dữ liệu** `TestData/Evals/{task}/*.json`: draft đầu vào (đã chạy matcher tất định) + đáp án đúng do kế toán xác nhận + trường "nên để trống". Khởi điểm ≥ 50 hóa đơn mua vào thật (đã che thông tin nhạy cảm), tăng dần lên vài trăm.
- **Chạy** qua MCP server nối DB mẫu (`TestData/EvalDb`), cho từng profile/model.
- **Chỉ số:**
  - Độ chính xác theo trường (`materialId`, `debitAccountId`, `warehouseId`...).
  - Tỷ lệ **bỏ trống đúng** (không biết thì không đoán) và tỷ lệ gợi ý sai có confidence ≥ 0.8 (lỗi nguy hiểm nhất).
  - Số vòng tool, thời gian, chi phí trung bình mỗi draft.
- **Ngưỡng chặn:** đổi model / skill / prompt / tool mà chỉ số giảm so với baseline đã lưu → CI fail.
- Thực tế vận hành: ghi nhận tỷ lệ gợi ý AI được người dùng chấp nhận không sửa (từ `audit` khi ghi sổ) để bổ sung ca khó vào bộ Evals.

---

## 10. Bảng cấu hình nghiệp vụ (người dùng tự sửa được)

### 10.1 Mặc định theo loại draft (`DraftTypeDefaults`)
TK công nợ mặc định (331/131), TK thuế (1331/3331), nguồn chứng từ, kho mặc định.

### 10.2 Quy tắc hạch toán (`PostingRule`)

| Cột | Ví dụ |
|---|---|
| `Priority` | 10 |
| `DraftType` | PurchaseInvoice |
| `PartnerId` (tùy chọn) | PETRO01 |
| `ItemNameContains` / `CategoryId` (tùy chọn) | "xăng", "dầu" |
| `→ DebitAccountId` | 6418 |
| `→ CostObjectId`, `WarehouseId` (tùy chọn) | |
| `RequireApprovalAboveAmount` | 20.000.000 |

Đây là "skill" mức khách hàng: kế toán trưởng khai báo, matcher và AI cùng đọc.

### 10.3 Cấu hình Agent (`agentsettings.json` + kho bí mật)
```json
{
  "Agent": {
    "Enabled": true,
    "Provider": "OpenRouter",
    "Endpoint": "https://openrouter.ai/api/v1",
    "ApiKeySecretName": "NEWACC_OPENROUTER_KEY",
    "Profiles": {
      "mapping":  { "ModelId": "<chọn qua Evals>", "FallbackModelIds": [], "Temperature": 0.0, "MaxToolRounds": 8, "TimeoutSeconds": 60, "AllowSendPii": false },
      "chat":     { "ModelId": "<model rẻ hơn>",    "Temperature": 0.3, "MaxToolRounds": 6 },
      "document": { "ModelId": "<...>",             "Temperature": 0.2 },
      "vision":   { "ModelId": "<model có vision>", "Temperature": 0.0 }
    },
    "DailyTokenLimit": 2000000,
    "DiagnosticLogging": false
  }
}
```

---

## 11. Chuyển đổi từ mã cũ ("mixed") sang cấu trúc mới

| Mã hiện tại | Đích mới | Ghi chú |
|---|---|---|
| `NewaccNet.AI/AccountingAgentService` | `NewaccNet.Agent/AgentService` | Giữ lớp cũ làm adapter tạm cho đến khi `AiChatDraftSource` chạy ổn, rồi xóa. Bỏ giá trị cứng `"156"`, `"KHO_HH"`. |
| `ParseNaturalLanguageJournalAsync` → `VoucherEntryWindow` | `IAgentService.ChatToDraftAsync` → `AiChatDraftSource` → DraftInbox | Không đẩy thẳng vào phiếu nữa. |
| `OpenRouterGateway`, `AiToolLoopRunner` | `ChatClientFactory` + function invocation của M.E.AI/MAF | Không tự viết vòng lặp tool. |
| `AiToolRegistry`, `IAiTool` | `NewaccNet.Tools` (`ToolCatalog`) | Giữ tên tool, mô tả, `AiToolAccess`. |
| `AiFieldSuggester`, `AiReviewAssistant` | `IFieldSuggester` adapter + `IAgentService.ExplainDraftAsync` | |
| `OcrVisionService` | `IAgentService.ExtractDocumentAsync` (profile `vision`) | |
| `OpenXmlReportService`, `ReportGeneratorService` | `NewaccNet.Office` | Không phải AI. |
| `SkillLoader/Selector/Composer`, `Skills/*.md` | `NewaccNet.Agent` + `NewaccNet.Agent.Skills` | |
| Logic lưu trong `VoucherEntryWindow.xaml.cs` | `NewaccNet.Core/Posting` | Bước P0. |
| Logic sinh chùm NM/XB trong `MaterialDetailWindow` | `IDraftVoucherBuilder` | Bước P0. |

---

## 12. Lộ trình triển khai

| Giai đoạn | Nội dung | Kết quả kiểm tra được |
|---|---|---|
| **P0 – Tách cổng** | `VoucherValidator`, `VoucherNumberService`, `VoucherPostingService` ra Core; `VoucherEntryWindow` gọi lại. Chuyển logic NM/XB về builder. Tạo `NewaccNet.ArchitectureTests` với quy tắc 3.3 (mục 2, 4). | Nhập tay chạy y như cũ; unit test validator; test kiến trúc xanh. |
| **P1 – Lớp đệm thủ công** | Contracts `VoucherDraft`, `JsonFileDraftStore`, `DraftLifecycleService`, `TaxInvoiceDraftSource`, `PartnerMatcher`, `DuplicateDetector`, `DraftValidationService`, `PurchaseInvoiceVoucherBuilder`, `PostingGateway`, `DraftInboxWindow`. Tách `NewaccNet.Office`; `DraftWorkbookRenderer`/`Reader`/`Package` + chế độ Draft của Office Studio. | Lấy hóa đơn đầu vào → mapping MST → người dùng chọn vật tư/TK bằng tay → ghi sổ. **Chưa có AI.** |
| **P2 – Học & quy tắc** | `MaterialMatcher`, `MappingMemoryStore`, `PostingHistoryService`, `PostingRuleEngine`, `ExcelDraftSource`, `DraftImportService`. | Tháng thứ hai, phần lớn dòng hàng của NCC quen tự map, không cần AI. |
| **P2.5 – Ranh giới Agent** | `NewaccNet.Tools` (`ToolCatalog`, `InProcessToolProvider`), `NewaccNet.McpServer`, `IAgentService`/`IFieldSuggester`/`IAgentToolProvider` trong Contracts, `NullAgentService`. Bổ sung quy tắc kiến trúc 3.3 (mục 1, 3, 5). | Gọi được mọi tool qua MCP Inspector trên DB mẫu; kết quả in-process = MCP. |
| **P3 – Agent gợi ý** | `NewaccNet.Agent` trên MAF: `ChatClientFactory`, profiles, middleware, skill `purchase-invoice-mapping`, `AgentService.SuggestDraftFieldsAsync`. `NewaccNet.Agent.Evals` + bộ 50 hóa đơn + baseline. Gỡ `AccountingAgentService` cũ khỏi luồng chứng từ. | Báo cáo Evals theo model; chọn model profile `mapping` theo số liệu. Bật AI chỉ cho trường còn trống. |
| **P4 – Mở rộng** | `SalesInvoiceVoucherBuilder`, `TaxInvoiceReconciliationService`, `AiChatDraftSource`, `OcrDraftSource` (profile `vision`), Office Studio Document qua `GenerateDocumentAsync`, `AiChatPanel`, `DbDraftStore`, sao kê ngân hàng. | Mỗi tính năng AI mới có bộ Evals riêng trước khi phát hành. |

---

## 13. Quy tắc cho AI-coding

**Ghi sổ & dữ liệu**
- KHÔNG gọi `adapter.SaveEntity` cho chứng từ ngoài `VoucherPostingService`. KHÔNG gọi cho Partner/InventoryItem ngoài `MasterDataGateway` (các form danh mục hiện có giữ nguyên).
- KHÔNG sửa `raw` trong draft. Mọi thay đổi ghi vào `resolved` + `match` + `audit`.
- KHÔNG đặt giá trị mặc định viết cứng (mã kho, mã TK) trong code; lấy từ `DraftTypeDefaults` / `PostingRule`.
- Service Core không hiện MessageBox; trả `DraftIssue` để UI hiển thị.
- Matcher/validator/builder phải có unit test với dữ liệu mẫu trong `TestData/`.

**Ranh giới Agent**
- `NewaccNet.Agent` KHÔNG tham chiếu Core, Tools, Office, Wpf, LLBLGen. Cần dữ liệu → thêm tool ở `NewaccNet.Tools` (việc của phía Core), không "đi tắt".
- KHÔNG tạo tool gọi `IPostingGateway`, `IMasterDataGateway`, `IVoucherPostingService` hoặc truy vấn tự do. Nếu thấy cần, đó là thiết kế sai.
- Mỗi tool: mô tả tiếng Việt rõ ràng (khi nào dùng, trả về gì), JSON Schema chặt, trả dữ liệu tối thiểu, có giới hạn số dòng.
- KHÔNG tự viết vòng lặp tool-calling hay client HTTP tới OpenRouter; dùng M.E.AI + MAF.
- KHÔNG viết cứng tên model; dùng `AgentProfiles`.
- Đổi skill/prompt/model/tool → chạy `NewaccNet.Agent.Evals` và đính kèm kết quả so với baseline.

**Office**
- OpenXML chỉ trên `MemoryStream`; `Seek(0)` sau khi tạo; ghi đĩa chỉ qua Save As.
- Office Studio chế độ Document KHÔNG có nút ghi sổ / import vào DB. Chỉ chế độ Draft mới có.

**Chung**
- Đọc tài liệu đúng phiên bản package (MAF, MCP C# SDK, M.E.AI) trước khi dùng API; không đoán.
- Không đưa API key, chuỗi kết nối, dữ liệu khách thật vào repo hay log.

---

## 14. Các điểm cần chủ dự án quyết định

1. Lớp đệm lưu file JSON (`JsonFileDraftStore`) hay bảng `StagingDraft` ngay từ đầu? (Nhiều người dùng trên SQL Server → nên dùng bảng.)
2. Draft ghi sổ trực tiếp từ Office Studio, hay bắt buộc mở qua `VoucherEntryWindow` để xem định khoản rồi mới lưu?
3. Hóa đơn dịch vụ (không qua kho): quy tắc chọn giữa Nợ 15x + kho và Nợ 6xx + CostDetail.
4. Ngưỡng tiền cần duyệt cấp hai (giám đốc) có cần ngay từ P1 không.
5. Bản cài cho khách: Agent chạy **in-process** (khuyến nghị, đơn giản) hay tách tiến trình riêng?
6. Có cho phép khách nối client MCP bên ngoài (ví dụ trợ lý AI trên máy họ) vào `NewaccNet.McpServer` không? Nếu có: chỉ tool `Read`, cần xác thực.
7. Chính sách dữ liệu mặc định: cho phép gửi dữ liệu draft lên model đám mây, hay mặc định che/dùng model cục bộ?
8. Ai xác nhận đáp án cho bộ Evals (kế toán trưởng?), và ngưỡng chỉ số tối thiểu để bật AI cho khách.
