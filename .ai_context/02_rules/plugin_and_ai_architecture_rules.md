# Quy Tắc Kiến Trúc: Plugin & Trí Tuệ Nhân Tạo (AI)

Tài liệu này quy định triết lý thiết kế và ranh giới kiến trúc bất biến giữa **Hệ thống Cốt lõi (Core)** và **Tiện ích Mở rộng (Plugin)** trong NewaccNet ERP.

---

## 1. Triết Lý Plugin: Clean Core & Tự Dọn Dẹp (Self-Cleaning)

### 1.1. Bất biến cấu trúc Core (Clean Core)
* **Tuyệt đối cấm:** Plugin can thiệp trực tiếp làm thay đổi cấu trúc bảng Core bằng các lệnh `ALTER TABLE ADD/DROP COLUMN` trên các bảng kế toán như `JournalVoucher`, `Account`, `Partner`, `Item`...
* **Giải pháp lưu trữ mở rộng:**
  * **Mô hình bảng mở rộng 1-1 (Khuyên dùng):** Plugin tự tạo bảng riêng với tiền tố `Plg_<TênPlugin>_<TênBảng>` (VD: `Plg_Erp_ItemExt`), liên kết với Core qua khóa chính/ngoại ID.
  * **Mô hình JSON Attributes:** Lưu các thuộc tính phụ trợ dạng JSON vào trường dữ liệu mở rộng.

### 1.2. Trách nhiệm khi Gỡ bỏ (Uninstall Lifecycle)
* **Nguyên tắc "Tự sinh ra thì tự dọn dẹp":**
  * Plugin tự implement giao diện `IPluginLifecycle` và tự chịu trách nhiệm thực thi các lệnh `DROP TABLE` cho các bảng do chính nó sở hữu.
  * Không để Plugin Manager (Core) phải "nhớ" cấu trúc bảng của từng plugin.
* **Vai trò của Plugin Manager:**
  * Là người điều phối (Orchestrator) và chốt chặn an toàn (Guard).
  * Hỏi người dùng lựa chọn: *Tạm gỡ (giữ nguyên dữ liệu)* hay *Gỡ sạch hoàn toàn (Purge data)*.
  * Hỗ trợ tự động sao lưu dữ liệu plugin ra file `.json`/`.bak` trước khi xóa.
  * Tuyệt đối bảo vệ: Chặn đứng mọi hành vi xóa bảng Core nếu có.

---

## 2. Chiến Lược AI: Nền Tảng (Core) vs Kỹ Năng (Plugin)

### 2.1. Vai trò của CORE: Hạ tầng & Trọng tài Kiểm toán (AI Platform & Guardrails)
* **Quản lý Gateway tập trung:**
  * Quản lý kết nối OpenRouter duy nhất (API Key, Base URL, lựa chọn model Chat, Vision, Analysis).
  * Kiểm soát số lượng token tiêu thụ, chi phí và ghi nhật ký kiểm toán (Audit Trail).
* **Cung cấp Runtime Engine dùng chung:**
  * `IAiGateway`: Cung cấp hàm gửi Chat, Vision (đọc ảnh/PDF), Structured JSON Output.
* **Chốt chặn Kiểm toán Kế toán (Accounting Guardrail - Bắt buộc):**
  * AI có thể bị ảo giác (hallucination). Mọi kết quả định khoản AI sinh ra (kể cả từ Core hay Plugin) đều bắt buộc phải đi qua bộ thẩm định kế toán của Core (`VoucherPostingValidator`) để kiểm tra cân Nợ/Có, khớp số tiền, tài khoản hợp lệ trước khi được ghi vào Sổ Cái.

### 2.2. Vai trò của PLUGIN: Tri thức Nghiệp vụ & Kỹ năng (Domain Skills & Tools)
* **Không làm việc trùng lặp:** Plugin không tự tạo HttpClient, không tự bắt người dùng nhập API Key hay kết nối trực tiếp đến OpenRouter.
* **Cung cấp Prompt chuyên ngành:** Định nghĩa kịch bản, Prompt và Class DTO đặc thù theo từng lĩnh vực (Kho ERP, Sao kê ngân hàng, Xây dựng...).
* **Cung cấp AI Tools (Function Calling):** Đăng ký các hàm tra cứu/xử lý nghiệp vụ chuyên biệt vào Core để Trợ lý AI có thể gọi khi cần (VD: `CheckStock`, `GetDebtInfo`).
