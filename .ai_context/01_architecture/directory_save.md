# Cơ chế Lưu (Save) các Form Danh mục

*Cập nhật: 28/09/2026*

## Kết luận ngắn

**Lưu không chạy qua hàm kế thừa.** `BaseDictionaryWindow.SaveRecord()` chỉ là stub rỗng; **không form nào override**. Persist thật sự nằm ở code-behind từng form: `adapter.SaveEntity(...)`.

Kế thừa chỉ cung cấp khung cửa sổ + vài helper (`ShowLoading` / `HideLoading`) và các virtual để form con **tự ghi đè** (`LoadData`, `PrintRecord`). CRUD save/delete được copy theo mẫu, không tập trung ở base.

## Chuỗi kế thừa

```
ThemedWindow
  └─ BaseWindow
       └─ BaseDictionaryWindow          // SaveRecord() { }  — không dùng
            └─ XxxListView              // LoadData override; save viết tay
```

File: `NewaccNet.Wpf/System/Directory/BaseDictionaryWindow.cs`

| Virtual trên base | Form con dùng thế nào |
|---|---|
| `SaveRecord()` | **Không override.** Nút Lưu không gọi hàm này. |
| `DeleteRecord()` | **Không override.** Xóa viết `DeleteSelected()` riêng. |
| `LoadData()` | **Có override** — fetch collection, gán `gridControl.ItemsSource`. |
| `PrintRecord()` | **Có override** — `ReportManager.PrintGridControl(...)`. |
| `NewRecord()` | Không dùng (dòng mới = `NewItemRowPosition="Bottom"` hoặc popup editor). |

API persist: `AppDataAccessAdapter.Create()` rồi `adapter.SaveEntity(entity, refetchAfterSave: true, recurse: false)` hoặc `adapter.DeleteEntity(entity)`.

## Hai mô hình Save

### A. Sửa trực tiếp trên lưới (inline)

Áp dụng: Loại hình CP, Danh mục CP, Kho, Nhóm VT, Thuế suất, Loại tiền, Tỷ giá, Nguồn TS, Lý do tăng giảm, …

1. XAML: `AllowEditing="True"`, `NewItemRowPosition="Bottom"`, `RowUpdated="TableView_RowUpdated"`.
2. User rời dòng (Enter / Tab / click dòng khác) → DevExpress ném `RowUpdated`.
3. Handler kiểm tra trường bắt buộc → `SaveEntity` **một dòng**.
4. Lỗi → MessageBox + `LoadData()` rollback lưới.

Nút toolbar **Lưu** (`MenuSave_ItemClick`) — **chỉ có một số form** (gồm 2 form chứng khoán):

```csharp
gridControl.View.CommitEditing();
MessageBox.Show("Lưu thành công!", ...);
```

Nút này **không** gọi `SaveRecord()` và **không** gọi `SaveEntity`. Nó chỉ chốt ô đang gõ; persist vẫn nhờ `RowUpdated` sau khi commit. Nếu editor chưa rời dòng thì có thể hiện “thành công” dù chưa ghi DB.

Ví dụ: `StockTypeListView.xaml.cs`, `StockListView.xaml.cs` — `TableView_RowUpdated`.

### B. Popup Editor (form nhiều trường)

Áp dụng: Tài khoản, Đối tượng CN, Nội dung/Lý do CN, Vật tư, Đối tượng/Yếu tố CP, Người dùng.

1. Lưới chủ yếu xem; Thêm/Sửa/double-click mở `*EditorWindow`.
2. Editor bind entity, bấm OK → `DialogResult = true` (**editor không SaveEntity**).
3. ListView nhận `ShowDialog() == true` → `SaveEntity` trên entity đó, rồi add vào collection nếu bản ghi mới.

Ví dụ: `PartnerListView.OpenEditor` → `PartnerEditorWindow`.

## Form chứng khoán (đối chiếu)

| Form | Mô hình | Save thật | Nút Lưu toolbar |
|---|---|---|---|
| `StockTypeListView` | A — inline | `TableView_RowUpdated` → `SaveEntity` | `CommitEditing` + MessageBox |
| `StockListView` | A — inline | `TableView_RowUpdated` → `SaveEntity` (chặn nếu `Stockid` trống) | giống trên |

Cả hai **đã implement save**, không thiếu handler; chỉ không đi qua `SaveRecord()` của lớp cha.

## Quy tắc khi sửa / thêm danh mục

1. Không giả định `SaveRecord()` trên base sẽ chạy — phải gắn event hoặc gọi `SaveEntity` tường minh.
2. Form inline: bắt buộc `RowUpdated` (hoặc tương đương) mới ghi DB.
3. Form popup: persist ở ListView sau dialog, không nhét `SaveEntity` vào editor trừ khi đổi chuẩn có chủ đích.
4. Không sửa thư mục `dataaccess/` (LLBLGen gen).
