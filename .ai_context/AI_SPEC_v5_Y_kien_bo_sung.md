# NEWACC — ĐẶC TẢ NỀN TẢNG AI (CORE) & KIẾN TRÚC PLUGIN — v5.0
> **Loại tài liệu:** Bản chỉ định thi công cho AI-coding (Antigravity, Cursor, Claude Code…).
> **Ngày:** 09/11/2026 · **Chủ dự án:** pvcuong1965
> **Hiệu lực:** Thay thế hoàn toàn v4. Tài liệu này **ưu tiên cao nhất** cho mọi việc liên quan tới AI và Plugin. Khi mâu thuẫn với `.ai_context/AI/NEWACC_Architecture_v3.md`, `.ai_context/02_rules/plugin_and_ai_architecture_rules.md` hoặc `docs/AI_AGENT_ROADMAP.md` → **làm theo tài liệu này**. Các phần khác của v3 (Staging, Posting, Calculation, Schema) giữ nguyên hiệu lực.
> **Trạng thái:** T1 (Hợp đồng trong `NewaccNet.Contracts/AI`, `NewaccNet.Contracts/Plugins`, `VoucherDraft.Suggestions`) **đã xong và giữ nguyên** trong v5.

---

## 0A. VÌ SAO CÓ v5 — Ý KIẾN BỔ SUNG SO VỚI v4

**Bối cảnh:** đội ngũ ít người, không theo dõi sát được từng dòng code. v4 đúng về *đích đến* nhưng *đường đi* còn nhiều bước phải sửa chồng lên mã đang chạy (T4 chuyển 6 màn hình, T6 sửa schema + LLBLGen, T7 sửa `IPluginContext`, T8 sửa plugin hiện có). Mỗi bước như vậy là một lần có thể làm hỏng cái đang chạy, và AI-coding hay "tiện tay" sửa thêm.

**Nguyên tắc v5 — "Thêm cạnh, không sửa giữa":**
1. **Cái đang chạy ổn thì để yên.** Mã AI cũ được *đóng băng* (chỉ sửa lỗi), không bắt buộc chuyển đổi. Chỉ chuyển một màn hình khi cần thêm tính năng cho nó hoặc khi nó hỏng.
2. **Ranh giới bằng project, không bằng lời dặn.** Nền tảng AI mới là **một project mới** `NewaccNet.AI.Platform` chỉ tham chiếu `Contracts` → trình biên dịch tự chặn vi phạm, không cần ai canh.
3. **Chia thành các luồng độc lập** (A: AI mới, B: Plugin host, C: mã cũ, D: Evals, E: CLI/MCP). Luồng này xong/dở không chặn luồng kia. Mỗi task nhỏ, có danh sách file được phép sửa.
4. **Mọi thứ mới đều có công tắc** trong cấu hình, mặc định **tắt**. Bật lên thấy lỗi thì tắt đi, phần mềm về như cũ.
5. **Interface mới là tùy chọn.** Plugin cũ không phải sửa gì; chỉ plugin nào cần bảng riêng/AI mới implement interface mới. Host dò bằng `is IPluginLifecycle`.
6. **Tránh đụng schema Core / LLBLGen.** Bảng hệ thống mới (nhật ký AI, sổ đăng ký plugin) **tự tạo bằng DDL** theo đúng mẫu `DbDraftStore` đang làm với bảng `StagingDraft` — không thêm vào `template.accdb`, không đồng bộ LLBLGen.
7. **Làm một lát cắt dọc chạy được sớm** (một tính năng AI đầu-cuối trên nền tảng mới) thay vì xây đủ mọi tầng rồi mới thấy kết quả.

**Bảng thay đổi chính v4 → v5:**

| Chủ đề | v4 | v5 |
|---|---|---|
| Nền tảng AI | Dọn `NewaccNet.AI` thành nền tảng, xóa mã cũ ở T4 | Project mới `NewaccNet.AI.Platform`; `NewaccNet.AI` cũ **đóng băng**, xóa khi không còn ai dùng (không hạn chót) |
| Màn hình AI cũ (6 màn hình) | Bắt buộc chuyển hết ở T4 | Giữ nguyên; chỉ chuyển khi cần (Luồng C) |
| Tool | Sửa Core tạo `IPartnerLookup`… | `NewaccNet.Tools` tự bọc các hàm `WarehousePostingGateway.Load*` sẵn có — **không sửa Core** |
| `IPluginContext` | Thêm 5 member (T7) | **Không sửa.** Plugin lấy dịch vụ qua `GetService<T>()` đã có trong `PluginHost` |
| Bảng `AiCallLog`, `SysPlugin`, `SysPluginTable` | Thêm vào `template.accdb` + LLBLGen (T6) | Nhật ký AI ghi **file** trước; bảng `Sys_*` **tự tạo bằng DDL** như `StagingDraft`, đọc/ghi bằng ADO tham số hóa |
| Plugin hiện có (ErpWarehouse, CashBank) | Bắt buộc chuẩn hóa (T8) | Không bắt buộc; chỉ khi plugin cần bảng riêng/AI |
| Thứ tự | Tuyến tính T1→T9 | 5 luồng song song, phụ thuộc tối thiểu (Mục 12) |
| CLI/MCP | Sau T9 | Luồng E, chỉ cần Tools (A2) là bắt đầu được |

---

## 0. HƯỚNG DẪN BẮT BUỘC CHO AI-CODING

1. **Đọc hết tài liệu này trước khi viết dòng code nào** liên quan AI/Plugin.
2. **Chỉ làm đúng task được giao** trong Mục 12, **chỉ sửa các file/thư mục được liệt kê** trong task đó. Không tự mở rộng phạm vi, không "tiện tay" sửa, đổi tên, dọn dẹp, refactor mã khác — kể cả khi thấy mã đó chưa đẹp. Thấy vấn đề → ghi vào báo cáo.
3. **Không phá cái đang chạy.** Sau mỗi task: build sạch, mọi chức năng cũ chạy như trước. Mã mới phải nằm sau công tắc cấu hình, mặc định tắt.
4. **Không tự quyết các điểm ở Mục 14.** Gặp điểm chưa rõ → dừng, ghi câu hỏi vào cuối báo cáo, không đoán.
5. Từ khóa: **PHẢI** = bắt buộc; **CẤM** = không bao giờ; **NÊN** = mặc định, chỉ lệch khi có lý do ghi rõ.
6. Mỗi task kết thúc bằng: build sạch + toàn bộ `NewaccNet.ArchitectureTests` pass + `git diff --stat` + checklist Mục 13. **Dừng lại sau mỗi task**, không tự chuyển task kế.
7. Không sửa `dataaccess/` (code sinh bởi LLBLGen). Không đổi version DevExpress 25.2.3 / LLBLGen 5.12 / .NET 10.
