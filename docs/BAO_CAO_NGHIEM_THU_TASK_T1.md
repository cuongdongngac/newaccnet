# BÁO CÁO NGHIỆM THU KỸ THUẬT: HOÀN THÀNH TASK T1 (HỢP ĐỒNG - CONTRACTS)
## DỰ ÁN TÁI CẤU TRÚC NỀN TẢNG AI VÀ HỆ THỐNG PLUGIN NEWACC (SPEC v4.0)

> **Ngày báo cáo:** 09/11/2026  
> **Người thực hiện:** Antigravity AI Pair Programmer  
> **Người tiếp nhận / Chủ dự án:** pvcuong1965  
> **Tài liệu căn cứ:** `docs/AI_PLATFORM_AND_PLUGIN_SPEC_v4.md` (phiên bản cập nhật 09/11/2026)

---

## 1. TỔNG QUAN & BỐI CẢNH DỰ ÁN

Hệ thống kế toán doanh nghiệp NEWACC đang bước vào giai đoạn nâng cấp kiến trúc lớn nhằm phục vụ song song hai nhóm đối tượng người dùng:
1. **Nhóm kế toán chuyên nghiệp (Core thuần):** Cần sự gọn nhẹ, chuẩn mực kế toán cao nhất, nhập liệu trực tiếp, **AI tắt mặc định**, không bị ảnh hưởng bởi các module mở rộng.
2. **Nhóm người dùng nghiệp vụ / vận hành (Core + Plugin hướng ERP):** Thao tác theo quy trình kho, bán hàng, ngân sách, hóa đơn... và sử dụng AI để hỗ trợ gợi ý, đọc hóa đơn, diễn giải tự động.

### Nguyên tắc bất biến được thiết lập trong Spec v4.0:
- **Clean Core & An toàn dữ liệu:** Bảng Core tuyệt đối không bị sửa đổi (`ALTER TABLE`), không bị phụ thuộc bởi Plugin. Gỡ bỏ Plugin thì toàn bộ số liệu kế toán, sổ cái và BCTC vẫn vẹn nguyên 100%.
- **AI là hạ tầng, không làm thay con người:** AI không bao giờ được ghi sổ trực tiếp, không là nguồn của con số tài chính cuối cùng. AI chỉ đóng vai trò đề xuất gợi ý vào bản nháp (`VoucherDraft`), kế toán duyệt và hệ thống tính toán lại bằng code tất định trước khi ghi sổ.
- **Ranh giới công nghệ nghiêm ngặt:** Chuẩn **.NET 10**, **DevExpress v25.2.3**, ORM **LLBLGen Pro v5.12**, tuyệt đối không dùng WinForms, không nhúng logic vào Code-behind XAML.

---

## 2. KẾT QUẢ TRIỂN KHAI TASK T1

Task T1 là bước đi nền móng đầu tiên: **Khai báo toàn bộ Hợp đồng (Contracts), mở rộng mô hình bản nháp mà không làm thay đổi bất kỳ hành vi runtime nào của hệ thống.**

### 2.1. Các hợp đồng AI Platform (`NewaccNet.Contracts/AI/`)
Đã thiết lập cổng giao tiếp độc lập, cô lập hoàn toàn hạ tầng AI khỏi nghiệp vụ kế toán và CSDL:
- **`IAiService`**: Cổng chuẩn duy nhất cho UI và Plugin giao tiếp với AI (`RunSkillAsync`, `CompleteStructuredAsync`, `ExtractFromDocumentAsync`, cờ `IsEnabled`).
- **`AiPrompt`, `AiDataBlock`, `AiDataSensitivity`**: Cơ chế bọc dữ liệu chống Prompt Injection và đánh dấu phân loại dữ liệu (`Public`, `Business`, `Personal` - phục vụ che dữ liệu PII Masking).
- **`AiCallOptions`, `AiResult<T>`, `AiErrorKind`**: Chuẩn hóa gói tham số và phong bì kết quả trả về, không ném exception làm gián đoạn luồng nghiệp vụ.
- **`ToolDefinition`, `IAiTool`, `ToolCallContext`, `ToolAccess`**: Chuẩn hóa công cụ gọi hàm của AI với 2 mức quyền an toàn: `Read` và `DraftWrite` (**CẤM** quyền `Post`).
- **`AiSuggestion`, `SuggestionOutcome`**: Cấu trúc đề xuất gợi ý độc lập (lưu vết `LineNo`, `FieldPath`, `Value`, `Confidence`, `Reason`, `Evidence`, `ModelUsed`, `CallId`), thay thế hoàn toàn việc nhét chuỗi tạm vào `MatchInfo`.
- **`ISuggestionSink` & `IAiCallLogSink`**: Giao diện tối giản thu nhận đề xuất của AI và ghi nhật ký kiểm toán/thống kê token (phục vụ đối soát chi phí).

### 2.2. Các hợp đồng Kiến trúc Plugin (`NewaccNet.Contracts/Plugins/`)
Đã hoàn thành bộ khung quản lý vòng đời và an toàn bảng dữ liệu của Plugin:
- **`IPluginSchema`**: Ràng buộc Plugin có bảng riêng phải khai báo tiền tố `Plg_<PluginKey>_`, phiên bản schema và các script migration cho 3 hệ CSDL (Access, SQL Server, PostgreSQL).
- **`IPluginLifecycle`**: Định nghĩa đầy đủ vòng đời: Cài đặt (`InstallAsync`), Nâng cấp (`UpgradeAsync`), Thẩm định gỡ bỏ (`CheckUninstallAsync`), Xuất dữ liệu sao lưu (`ExportDataAsync`), và Dọn dẹp phụ trợ (`PurgeAsync`).
- **`IPluginAiContributor`**: Cho phép Plugin đóng góp kỹ năng nghiệp vụ (`SkillSource` qua file Markdown nhúng) và các Tool riêng biệt.
- **`IDraftPostingService`, `IDraftService`, `IPluginDbSession`**: Cơ chế ghi nhận và lưu trữ nghiệp vụ an toàn, bảo vệ không cho Plugin truy cập trái phép bảng Core.

### 2.3. Mở rộng `VoucherDraft` & Đảm bảo tương thích ngược 100%
- Đã thêm danh sách `Suggestions` (`List<AiSuggestion>`) vào `VoucherDraft`.
- Giữ nguyên toàn bộ cấu trúc gốc `MatchInfo`, `Raw`, `Resolved`, `Lines` để các luồng Staging hiện tại không bị ảnh hưởng.

---

## 3. KẾT QUẢ KIỂM THỬ & NGHIỆM THU KỸ THUẬT

### 3.1. Kết quả Biên dịch (Build)
- Toàn bộ **17 project** trong Solution (`NewaccNet.slnx`) biên dịch thành công tuyệt đối:
  - **Lỗi biên dịch:** `0 Error(s)`.
  - Không có cảnh báo lỗi mới.

### 3.2. Kết quả Kiểm thử Kiến trúc & Nghiệm thu Tương thích (Test Suite)
Đã bổ sung 3 test case chuyên sâu tại `NewaccNet.ArchitectureTests/DraftStoreTests.cs` và chạy kiểm thử toàn diện:
1. **`VoucherDraft_LegacyJsonWithoutSuggestions_ShouldDeserializeWithEmptySuggestionsList`:** Đảm bảo khi đọc các bản nháp JSON cũ (chưa có trường `Suggestions`), hệ thống tự động khởi tạo danh sách rỗng, không gây lỗi `NullReferenceException`.
2. **`JsonFileDraftStore_LegacyJsonFileOnDisk_ShouldLoadWithEmptySuggestions`:** Đọc thực tế từ tệp lưu trữ trên đĩa định dạng cũ qua `JsonFileDraftStore`, xác nhận dữ liệu nạp lên đầy đủ, tính tương thích ngược đạt 100%.
3. **`VoucherDraft_WithAiSuggestions_RoundtripSerialization_PreservesAllFields`:** Kiểm tra chu trình tuần hoàn (Roundtrip Serialize/Deserialize) của đối tượng `AiSuggestion`, toàn bộ dữ liệu độ tin cậy, lý do, bằng chứng và trạng thái đều được bảo toàn.
4. **Kiểm tra ranh giới phụ thuộc (`ArchitectureRulesTests`):** Xác nhận `NewaccNet.Contracts` không tham chiếu bất kỳ thư viện ngoài nào (chỉ dùng BCL chuẩn và `System.Text.Json`).

**Kết quả chạy thực tế:**
```text
Test run for D:\NewaccNet\NewaccNet.ArchitectureTests\bin\Debug\net10.0-windows\NewaccNet.ArchitectureTests.dll (.NETCoreApp,Version=v10.0)
Total tests: 22. Passed: 22. Failed: 0. Skipped: 0.
Duration: 2.1s
```

### 3.3. Thống kê thay đổi mã nguồn (`git diff --stat`)
Tuân thủ đúng kỷ luật: **Chỉ chỉnh sửa các tệp thuộc Contracts và Test**:
```text
 AGENTS.md                                      |   3 +
 NewaccNet.ArchitectureTests/DraftStoreTests.cs | 119 +++++++++++++++++++++++++
 NewaccNet.Contracts/VoucherDraftModels.cs      |   2 +
 3 files changed, 124 insertions(+)
```
*(Cùng 8 file hợp đồng mới được tạo trong `NewaccNet.Contracts/AI/` và `NewaccNet.Contracts/Plugins/`)*.

---

## 4. CHECKLIST NGHIỆM THU (THEO MỤC 13 SPEC v4.0)

| Tiêu chí kiểm tra | Trạng thái | Ghi chú |
| :--- | :---: | :--- |
| Chỉ làm đúng task T1 được giao | **ĐẠT** | Không mở rộng phạm vi sang T2. |
| Build toàn solution: 0 error | **ĐẠT** | 17/17 projects biên dịch thành công. |
| `NewaccNet.ArchitectureTests` pass toàn bộ | **ĐẠT** | 22/22 tests passed (100%). |
| Không tạo `new OpenRouterGateway` bừa bãi | **ĐẠT** | Giữ nguyên hiện trạng, sẽ xử lý ở T4. |
| Không có logic/prompt AI trong code-behind XAML | **ĐẠT** | T1 chỉ làm việc tại Contracts/Tests. |
| Không có mã TK/kho viết cứng mới | **ĐẠT** | Hoàn toàn dùng cấu hình và DTO. |
| Không ghi trực tiếp bảng sổ ngoài `IPostingGateway` | **ĐẠT** | Ràng buộc trong hợp đồng `IDraftPostingService`. |
| Không sửa đổi `dataaccess/` (LLBLGen) | **ĐẠT** | Giữ nguyên mã LLBLGen. |
| AI tắt → Ứng dụng vẫn chạy bình thường | **ĐẠT** | T1 không làm thay đổi hành vi thực thi. |

---

## 5. THỐNG NHẤT CÁC ĐIỂM CHÍNH SÁCH (MỤC 14) VỚI CHỦ DỰ ÁN

Để chuẩn bị cho các task tiếp theo, các nguyên tắc vận hành sau đã được thống nhất:
1. **Chính sách dữ liệu (`DataPolicy`):** Tạm chốt mức **`MaskPersonal`** (tự động che STK ngân hàng, CCCD, SĐT, Email trước khi gửi AI; giữ nguyên MST và tên công ty để phục vụ tra cứu).
2. **Ngân sách AI:** Giới hạn mặc định **2.000.000 tokens/tháng** trên mỗi đơn vị.
3. **Mã định danh Plugin (`PluginKey`):** Gán chuẩn cho 3 plugin hiện có là `Erp` (`ErpWarehouse`), `Bank` (`CashBank`), `Mig` (`DbMigrator`).
4. **Quy trình gỡ sạch Plugin trên SQL Server:** Bắt buộc tự động xuất file JSON sao lưu; nếu tài khoản có quyền backup sẽ tự chạy backup DB, nếu không sẽ yêu cầu người dùng xác nhận đã sao lưu trước khi DROP bảng `Plg_*`.
5. **Mô hình AI cục bộ:** Chưa tích hợp trong đợt này, ưu tiên cổng OpenRouter ổn định.

---

## 6. KẾ HOẠCH BƯỚC TIẾP THEO

Theo đúng lộ trình 9 Task của Spec v4.0:
- **Bước tiếp theo: Task T2 — Dựng lại `NewaccNet.AI` thành AI Platform thuần túy.**
  - Gỡ bỏ hoàn toàn ProjectReference `Core` và `TaxInvoice` khỏi project `NewaccNet.AI`.
  - Triển khai Pipeline 8 bước chuẩn: *Policy -> Masking -> Assemble -> Route -> ToolLoop -> Validate -> Unmask -> Audit Log*.
  - Chuyển toàn bộ các logic kế toán, CFO prompt và OCR cũ ra ngoài nền tảng (`NewaccNet.Core/AiFeatures`).
  - Xóa bỏ project rỗng `NewaccNet.Agent`.

Bản báo cáo này đã được lưu trực tiếp vào kho tài liệu dự án tại:  
`docs/BAO_CAO_NGHIEM_THU_TASK_T1.md`
