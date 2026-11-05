Sau khi chạy Bước 1, 4 file sau sẽ xuất hiện ở đây:
- mapping_tables.csv
- mapping_columns.csv
- mapping_relationships.csv
- migration_order.csv     <-- QUAN TRỌNG NHẤT: quyết định thứ tự migrate
                              (bảng cha trước, bảng con sau, theo từng cấp).
                              Xem kỹ các dòng có cảnh báo vòng lặp quan hệ.

Mở bằng Excel để review/sửa trước khi chạy Bước 2. Bước 2 và Bước 3 sẽ chạy
đúng theo thứ tự trong migration_order.csv, không tự tính lại.
