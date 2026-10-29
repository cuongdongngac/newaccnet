# GDT Invoice Manager v3.1.2

Ứng dụng desktop quản lý/tải hóa đơn từ Cổng Hóa đơn điện tử GDT.

## Dashboard
- Tên phần mềm, tên công ty, MST hiện tại.
- Nút sửa/thay đổi thông tin doanh nghiệp.
- Chọn Q1 / Q2 / Q3 / Q4 / Cả năm theo năm.
- Đăng nhập GDT với CAPTCHA, mật khẩu chỉ ở RAM.
- Tải HDDT + hóa đơn máy tính tiền.
- Query + export cross-check + detail + manifest.
- Xuất Excel và mở thư mục/báo cáo từ dashboard.

## Excel
Tên file: `MST_Q1_2026.xlsx`, `MST_Q2_2026.xlsx`, `MST_Q3_2026.xlsx`, `MST_Q4_2026.xlsx`, hoặc `MST_NAM_2026.xlsx`.

Tất cả số tiền được lưu/hiển thị dạng số nguyên; không dùng `0.00`/`0,00`.

## Bảo mật
- Không lưu password/token/CAPTCHA vào file.
- MST chỉ được mask trong diagnostic log.
- Invoice data được lưu trong thư mục `invoices` để phục vụ báo cáo và retry.

## Chạy
Double-click `RUN_SCANNER.bat` hoặc `python main.py`.
CLI cũ có thể chạy bằng `python main.py --cli` khi đã bật fixed_scan_range trong config.


## v3.1.2 — Production hardening
- Query/export parameters are URL-encoded instead of concatenated raw.
- Manifest is persisted progressively during the scan and ends with `status=COMPLETED`.
- Detail/XML artifacts are written atomically and carry SHA-256 fingerprints.
- Excel keeps exactly 3 sheets and adds an in-sheet reconciliation block.
- Config validation requires HTTPS and fresh request-id generation.
- No unconfirmed PDF/original-file endpoint is fabricated; XML is saved only when actually returned by GDT.


## 3.1.2 scan safety
- Bounded invoice request timeout/retries.
- Per-month/category scan deadline (default 90s).
- No nested aggressive recovery retry loop.
- GUI reports current category/month/detail progress.
- Timeout marks the window incomplete instead of hanging indefinitely.

## v3.1.2 hotfix
- Discovery, Export and Detail use separate timeout budgets. A slow Export can no longer exhaust the Detail deadline and mark every invoice as failed.
- Detail phase is bounded independently and continues for already discovered invoices.
- Export timeout is bounded independently.
