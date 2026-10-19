# Triết lý Tìm kiếm thông minh trên Dropdown (`SearchLookUpBehavior`)

## 1. Tổng quan & Vị trí mã nguồn
Toàn bộ các Dropdown (`dxg:LookUpEdit` / `dxe:ComboBoxEdit`) trong dự án sử dụng cơ chế **Behavior gắn kèm (`SearchLookUpBehavior`)** tại thư mục:
- `d:\NewaccNet\NewaccNet.Wpf\AppSystem\Helpers\combobox\SearchLookUpBehavior.cs` (Namespace: `NewaccNet.Wpf.Controls.Search`, XML namespace: `xmlns:dxs="clr-namespace:NewaccNet.Wpf.Controls.Search"`)
- `d:\NewaccNet\NewaccNet.Wpf\AppSystem\Helpers\combobox\VnSearchFunctions.cs`

---

## 2. Nguyên lý hoạt động (Vừa tìm theo Mã/Key, vừa tìm theo Tên/Display Name)

Khi gắn `<dxs:SearchLookUpBehavior/>` vào một `LookUpEdit`:
1. **Tự động nhận diện `SearchProfile` theo kiểu Entity của `ItemsSource`**:
   - Tự động ánh xạ cặp trường **`CodeField` (Mã/Key)** và **`NameField` (Tên hiển thị/Display Name)** cùng tiêu đề 2 cột (`CodeHeader` | `NameHeader`) dựa vào kiểu dữ liệu của `ItemsSource` (hoặc cho phép override trực tiếp qua thuộc tính `CodeField`, `NameField` trên XAML).
2. **Tự động bắt sự kiện `SubstituteDisplayFilter` của DevExpress**:
   - Khi người dùng gõ chuỗi tìm kiếm `text` vào ô Dropdown, `SearchLookUpBehavior` chặn bộ lọc mặc định và thay bằng biểu thức `CriteriaOperator` kết hợp cả 2 trường (khi `EffectiveMode = SearchMode.All` - mặc định):
     - **Tìm theo Mã (`CodeField`)**: `VnSearchFunctions.StartsWith(CodeField, text)` — khớp bắt đầu (StartsWith), không phân biệt hoa thường.
     - **Tìm theo Tên (`NameField`)**: `VnSearchFunctions.ContainsAll(NameField, text)` — khớp tất cả các từ khóa trong tên, **hỗ trợ gõ tiếng Việt không dấu hoặc có dấu** (`SearchMatcher.ContainsAllWords`).
     - **Kết hợp (`OR`)**: `GroupOperator(GroupOperatorType.Or, byCode, byName)` — chỉ cần khớp **Mã** HOẶC khớp **Tên** là dòng dữ liệu sẽ hiển thị.
3. **Tự động dựng 2 cột chuẩn (`Mã | Tên`)**:
   - Khi mở Popup (`OnPopupOpened`), nếu lưới bên trong chưa được cấu hình cột riêng, `BuildColumns` tự động dựng 2 cột chuẩn: `CodeField` (Width = 110) và `NameField` (`AutoWidth = true`), ẩn `GroupPanel` và `AutoFilterRow` để giao diện gọn nhẹ, đồng nhất.

---

## 3. Quy ước sử dụng chuẩn trong XAML

```xml
<dxg:LookUpEdit ItemsSource="{Binding Partners}"
                ValueMember="Id"
                EditValue="{Binding PartnerId, Mode=TwoWay}">
    <dxmvvm:Interaction.Behaviors>
        <dxs:SearchLookUpBehavior />
    </dxmvvm:Interaction.Behaviors>
</dxg:LookUpEdit>
```

- **Lưu ý quan trọng**:
  - `SearchLookUpBehavior` đã tự quản lý `DisplayMember`, `IncrementalFiltering = true`, `ImmediatePopup = true`, `AutoComplete = false`, `AutoPopulateColumns = false`.
  - Nếu cần chỉ định rõ trường Mã/Tên cho DTO đặc thù chưa đăng ký trong `SearchProfiles`, có thể truyền trực tiếp:
    ```xml
    <dxs:SearchLookUpBehavior CodeField="Id" NameField="CustomerName" CodeHeader="Mã ĐT" NameHeader="Tên đối tượng" />
    ```
