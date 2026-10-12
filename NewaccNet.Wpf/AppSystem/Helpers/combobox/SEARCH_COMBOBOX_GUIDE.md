# SearchComboBox – Hướng dẫn cho AI coding agent

Tài liệu này mô tả thiết kế hiện tại của `SearchComboBox` (WPF), các quy tắc không được phá vỡ và danh sách việc cần làm tiếp. Đọc hết trước khi sửa code.

## 1. Mục tiêu

Tạo một ComboBox WPF có ô tìm kiếm giống react-select:
- Gõ vào ô thì dropdown tự mở và lọc ngay.
- Có thể đổi cách tìm lúc runtime: theo **mã**, theo **tên**, hoặc **cả hai** (`SearchMode`).
- Mỗi collection có thể đặt tên trường mã/tên khác nhau (`MaKH`/`TenKH`, `Sku`/`ProductName`...).
- Tìm kiếm không phân biệt hoa thường và **bỏ dấu tiếng Việt** (`duc` khớp `Đức`).

## 2. Cấu trúc file

| File | Phụ thuộc WPF | Vai trò |
|---|---|---|
| `SearchCore.cs` | **Không** | `SearchMode`, `ISearchItem`, `[SearchCode]`/`[SearchName]`, `SearchFieldResolver`, `TextNormalizer`, `SearchQuery`. Toàn bộ logic tìm/so khớp nằm ở đây. |
| `SearchComboBox.cs` | Có | Control `SearchComboBox` (kế thừa `ComboBox`) và base có kiểu `SearchComboBox<T>`. Chỉ lo UI: bắt TextChanged, áp Filter, giữ selection, xử lý phím. |
| `SearchExamples.cs` | Có | Ví dụ 3 cách khai báo mã/tên (`Customer`, `Product`, `EmployeeComboBox`). |
| `MainWindow.xaml(.cs)` | Có | Demo cũ dùng `CodePath`/`NamePath`, vẫn chạy được. |

Namespace: `MyApp.Controls`, `MyApp.Controls.Search`, `MyApp.Models`. Đổi theo dự án thật.

**Nguyên tắc:** logic tìm kiếm đặt trong `SearchCore.cs` để unit test được mà không cần WPF. XAML chỉ dùng để đặt control và bind dữ liệu.

## 3. Thứ tự lấy mã/tên của một item

Thứ tự ưu tiên từ cao đến thấp:

1. **Override** `GetSearchCode` / `GetSearchName` trong control con. Thường dùng qua `SearchComboBox<T>` với `GetCode(T)` / `GetName(T)`.
2. Item implement **`ISearchItem`** (`SearchCode`, `SearchName`).
3. Property có **`[SearchCode]` / `[SearchName]`**. Kết quả tra cứu được cache theo `Type`.
4. **`CodePath` / `NamePath`** (dependency property, path dạng `A.B.C`).
5. Không có gì ở trên thì dùng `ToString()`.

### Khi nào dùng cách nào

| Tình huống | Cách |
|---|---|
| Sở hữu model, muốn rõ ràng | `ISearchItem` (implement tường minh) |
| Sở hữu model, muốn ngắn gọn | Attribute |
| Không sửa được model (DTO sinh tự động, thư viện ngoài) hoặc mã/tên phải ghép từ nhiều trường | `SearchComboBox<T>` + lớp con không generic |
| Chỉ dùng một lần, làm nhanh | `CodePath`/`NamePath` trong XAML |

### Thêm loại dữ liệu mới bằng control con

```csharp
public class SupplierComboBox : SearchComboBox<Supplier>
{
    public SupplierComboBox() => TextSearch.SetTextPath(this, nameof(Supplier.Name));
    protected override string GetCode(Supplier s) => s.TaxCode;
    protected override string GetName(Supplier s) => s.Name;
}
```

Lớp dùng trong XAML **phải không generic**, vì XAML không khởi tạo được `SearchComboBox<T>` trực tiếp.

## 4. Luật so khớp (`SearchQuery`)

- Chuỗi gõ vào được chuẩn hóa: bỏ dấu, chuyển `đ` thành `d`, chữ thường, trim.
- **Code**: mã đã chuẩn hóa phải `StartsWith` chuỗi gõ vào.
- **Name**: tách chuỗi gõ vào theo khoảng trắng, **mọi từ** phải có trong tên (`Contains`). Ví dụ `ng an` khớp `Nguyễn Văn An`.
- **All**: khớp Code hoặc Name.
- Chuỗi rỗng thì khớp tất cả.
- `Matches` nhận `Func<string>` để chỉ tính mã/tên khi mode cần.

## 5. Bất biến KHÔNG được phá vỡ

1. **View riêng cho mỗi control.** `OnItemsSourceChanged` bọc `IList` vào `new ListCollectionView(list)`. Không dùng `CollectionViewSource.GetDefaultView` hay `Items.Filter` trên default view, vì như vậy hai combobox bind cùng một collection sẽ lọc lẫn nhau.
2. **Dùng `SetCurrentValue`, không dùng `ItemsSource = ...`**, để không phá Binding từ ViewModel.
3. **Cờ `_suppress`.** Mọi chỗ code tự gán `Text`/`SelectedItem`/`Filter` phải bật cờ này để tránh `Editor_TextChanged` chạy đệ quy.
4. **Bỏ qua TextChanged do chọn item.** Nếu `Text == GetDisplayText(SelectedItem)` thì không lọc.
5. **`GetDisplayText` không được override.** Nó phải trùng với text ComboBox tự hiển thị (`TextSearch.TextPath`, sau đó `DisplayMemberPath`). Muốn đổi chữ hiển thị thì set `TextSearch.TextPath`, có thể set trong constructor của control con.
6. **Khôi phục selection.** `_savedSelection` lưu item trước khi lọc. Khi đóng dropdown hoặc mất focus: ô trống thì bỏ chọn; ngược lại giữ `SelectedItem`, hoặc dùng `_savedSelection` nếu `SelectedItem` là null.
7. `SearchCore.cs` **không được** `using System.Windows*`.

## 6. Hạn chế đã biết

- Khi item đang chọn bị lọc mất, `SelectedItem` bind về ViewModel **tạm thời thành `null`**, rồi được khôi phục khi đóng dropdown. ViewModel không nên chạy logic nặng khi giá trị là `null`.
- Mỗi lần gõ, mọi item đều bị chuẩn hóa lại (`TextNormalizer.Normalize`), nên có thể chậm khi danh sách có trên 10k dòng.
- Đã có hỗ trợ `ObservableCollection` (thông qua `ListCollectionView`). `IEnumerable` không phải `IList` thì **không** được bọc view, nên lọc sẽ không hoạt động.

## 7. Việc cần làm tiếp (theo thứ tự ưu tiên)

Mỗi task có tiêu chí hoàn thành (AC). Làm từng task, build và test xong mới sang task tiếp.

### T1. Unit test cho `SearchCore.cs`
- Tạo project test (xUnit hoặc NUnit), không tham chiếu WPF.
- Test `TextNormalizer`: `"Nguyễn Đức"` thành `"nguyen duc"`, chuỗi null/rỗng, chữ hoa.
- Test `SearchQuery`: Code dùng StartsWith (`kh0` khớp `KH001`, không khớp `AKH001`); Name nhiều từ (`ng an` khớp `Nguyễn Văn An`, `an ng` cũng khớp); All; chuỗi rỗng.
- Test `SearchFieldResolver`: thứ tự ưu tiên ISearchItem > attribute > path > ToString; path lồng `A.B`; property null.
- **AC:** toàn bộ test pass; coverage của `SearchCore.cs` ≥ 90%.

### T2. Cache chuỗi đã chuẩn hóa
- Dùng `ConditionalWeakTable<object, CachedKeys>` trong control (hoặc một cache truyền vào `SearchQuery`) để lưu code/name đã normalize theo từng item.
- Xóa cache khi `ItemsSource` đổi, khi `CodePath`/`NamePath` đổi, và khi item raise `PropertyChanged` cho trường liên quan (nếu item implement `INotifyPropertyChanged`).
- Thêm vào `SearchQuery` các overload nhận chuỗi **đã** chuẩn hóa để không normalize lần hai.
- **AC:** gõ trong danh sách 50k item không giật (mỗi lần filter dưới 50ms trên máy dev); test T1 vẫn pass.

### T3. Debounce
- DP `SearchDelay` (TimeSpan, mặc định 0; gợi ý 150–250ms khi danh sách lớn). Dùng `DispatcherTimer`.
- Việc khôi phục text/caret vẫn chạy ngay, chỉ việc `ApplyFilter` bị trễ.
- **AC:** gõ liên tục chỉ gây một lần `Refresh` sau khi dừng gõ.

### T4. Ảo hóa dropdown
- Trong style mặc định đặt `ItemsPanel` là `VirtualizingStackPanel`, `VirtualizingPanel.IsVirtualizing=True`, `VirtualizationMode=Recycling`.
- **AC:** mở dropdown với 50k item mất dưới 200ms.

### T5. Placeholder theo `SearchMode`
- DP `PlaceholderCode`, `PlaceholderName`, `PlaceholderAll` (mặc định "Tìm theo mã…", "Tìm theo tên…", "Tìm mã hoặc tên…").
- Hiển thị khi ô trống và chưa có selection. Cần custom template hoặc adorner trên `PART_EditableTextBox`.
- **AC:** đổi `SearchMode` thì placeholder đổi ngay.

### T6. Phím tắt đổi mode
- Ví dụ `Ctrl+M` xoay vòng Code → Name → All. Dùng `SetCurrentValue(SearchModeProperty, ...)` để không phá binding.
- **AC:** đổi bằng phím thì ViewModel nhận được giá trị mới khi binding là TwoWay. Cân nhắc đặt `BindsTwoWayByDefault` cho `SearchModeProperty`.

### T7. Tô đậm phần khớp (tùy chọn)
- Attached property hoặc converter cho `TextBlock` trong `ItemTemplate`, nhận text và `SearchQuery` rồi tạo các `Run` in đậm.
- Lưu ý vị trí khớp phải map ngược từ chuỗi đã bỏ dấu về chuỗi gốc. Độ dài có thể khác sau khi FormD/FormC, nên cần làm mapping theo từng ký tự.
- **AC:** gõ `duc` thì "Phạm Minh **Đức**" được tô đậm đúng.

### T8. Style mặc định / Generic.xaml (nếu tách thành thư viện)
- Thêm `DefaultStyleKeyProperty.OverrideMetadata` và `Themes/Generic.xaml` chỉ khi cần template riêng (T5, T4). Nếu chưa cần thì giữ nguyên style ComboBox mặc định.

## 8. Kiểm tra thủ công sau mỗi thay đổi

1. Gõ `kh0` ở mode Code: chỉ còn các mã bắt đầu bằng KH0.
2. Đổi sang Name khi ô vẫn còn chữ: danh sách lọc lại ngay.
3. ↑/↓ rồi Enter: chọn đúng item, text đúng tên, dropdown đóng, danh sách không còn bị lọc.
4. Enter khi chưa highlight: chọn item đầu tiên.
5. Gõ linh tinh rồi Tab đi: khôi phục item đã chọn trước đó.
6. Xóa trắng rồi Tab đi: `SelectedItem` thành null.
7. Hai combobox bind **cùng** một collection: lọc ở cái này không ảnh hưởng cái kia.
8. ViewModel thay cả collection (gán `ItemsSource` mới): control vẫn lọc được, binding không bị mất.
9. Thêm/xóa item trong `ObservableCollection` khi đang lọc: không lỗi.

## 9. Môi trường

- Code viết theo cú pháp C# 7.3+, chạy được trên .NET Framework 4.7.2+ và .NET 6+ (`net6.0-windows` hoặc mới hơn với `<UseWPF>true</UseWPF>`).
- Không dùng thư viện ngoài.
