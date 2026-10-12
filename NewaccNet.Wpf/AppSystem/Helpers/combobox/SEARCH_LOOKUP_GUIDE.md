# Search LookUp (DevExpress WPF v25) – Hướng dẫn cho AI coding agent

Tài liệu này mô tả thiết kế hiện tại, các quy tắc không được phá vỡ và danh sách việc cần làm tiếp. Đọc hết trước khi sửa code.

> Bản WPF thuần trước đây (`SearchComboBox.cs`, `MainWindow.xaml`) đã **bỏ**. Không khôi phục.

## 1. Mục tiêu

Ô chọn danh mục dùng `dxg:LookUpEdit` (dropdown 2 cột **Mã | Tên**) của DevExpress WPF v25:
- Gõ vào ô thì dropdown tự mở và lọc ngay (`IncrementalFiltering` + `ImmediatePopup`).
- Đổi cách tìm lúc runtime: theo **mã**, theo **tên**, hoặc **cả hai** (`SearchMode`).
- Mỗi loại danh mục tự cấu hình:
  - trường mã/tên (mỗi collection đặt tên khác nhau: `Sku`/`ProductName`, `SoTK`/`TenTK`...);
  - **hiển thị** mã hay tên trong ô sau khi chọn (`SearchDisplay`). Sản phẩm hiện tên, tài khoản kế toán hiện số TK;
  - mode tìm mặc định;
  - tiêu đề 2 cột.
- Từng chỗ dùng có thể ghi đè cấu hình trên XAML.
- So khớp không phân biệt hoa thường và **bỏ dấu tiếng Việt** (`duc` khớp `Đức`).

## 2. Cấu trúc file

| File | Phụ thuộc | Vai trò |
|---|---|---|
| `SearchCore.cs` | **Không** (chỉ BCL) | `SearchMode`, `SearchDisplay`, `ISearchItem`, `[SearchCode]`/`[SearchName]`/`[SearchProfile]`, `SearchProfile`, `SearchProfiles` (registry + resolve), `TextNormalizer`, `SearchMatcher`. |
| `VnSearchFunctions.cs` | `DevExpress.Data` | Đăng ký custom function `VnStartsWith`, `VnContainsAll` cho `CriteriaOperator`, gọi sang `SearchMatcher`. |
| `SearchLookUpBehavior.cs` | DevExpress WPF | `Behavior<LookUpEditBase>`: resolve cấu hình theo kiểu item, set `DisplayMember`, dựng 2 cột, thay filter trong `SubstituteDisplayFilter`. |
| `SearchExamples.cs` | – | Model mẫu (`Product`, `Account`, `Warehouse`, `CustomerDto`), `SearchSetup.Configure()`, XAML mẫu. |

Namespace: `MyApp.Controls.Search` (core), `MyApp.Controls.Dx` (DevExpress), `MyApp.Models`. Đổi theo dự án thật.

**Không** đặt namespace có đoạn `DevExpress` (VD `MyApp.Controls.DevExpress`): `using DevExpress.Data...` bên trong sẽ bị resolve nhầm thành namespace của mình và không biên dịch được.

## 3. Cách cấu hình

### 3.1 Thứ tự ưu tiên (cao đến thấp)

1. Thuộc tính đặt trên **`SearchLookUpBehavior`** trong XAML (`CodeField`, `NameField`, `Display`, `SearchMode`, `CodeHeader`, `NameHeader`). Để trống (null) nghĩa là dùng cấu hình của model.
2. **`SearchProfiles.Register<T>(...)`** bằng code (dành cho DTO không sửa được). Áp dụng cho cả lớp con của `T`.
3. **Attribute trên model**: `[SearchCode]`, `[SearchName]` trên property; `[SearchProfile(Display, DefaultMode, CodeHeader, NameHeader)]` trên class.
4. **`ISearchItem`**, phải implement **public** vì DevExpress lọc theo tên property `SearchCode`/`SearchName`.
5. Không có gì ở trên: behavior không làm gì (`Profile == null`), editor chạy như mặc định.

### 3.2 Ví dụ

```csharp
[SearchProfile(Display = SearchDisplay.Name, DefaultMode = SearchMode.All,
               CodeHeader = "Mã SP", NameHeader = "Tên sản phẩm")]
public class Product { [SearchCode] public string Sku {get;set;} [SearchName] public string ProductName {get;set;} }

[SearchProfile(Display = SearchDisplay.Code, DefaultMode = SearchMode.Code,
               CodeHeader = "Số TK", NameHeader = "Tên tài khoản")]
public class Account { [SearchCode] public string SoTK {get;set;} [SearchName] public string TenTK {get;set;} }

// App.OnStartup
SearchProfiles.Register<CustomerDto>(x => x.MaKH, x => x.TenKH, SearchDisplay.Name, SearchMode.All, "Mã KH", "Tên khách hàng");
```

```xml
xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
xmlns:dxs="clr-namespace:MyApp.Controls.Dx"

<dxg:LookUpEdit ItemsSource="{Binding Accounts}" ValueMember="SoTK" EditValue="{Binding SoTK}">
    <dxmvvm:Interaction.Behaviors>
        <dxs:SearchLookUpBehavior SearchMode="{Binding AccountSearchMode}"/>
    </dxmvvm:Interaction.Behaviors>
</dxg:LookUpEdit>
```

## 4. Cơ chế hoạt động

1. Khi attach và mỗi khi `ItemsSource` đổi (theo dõi qua `DependencyPropertyDescriptor`), behavior xác định kiểu item: lấy `T` của `IEnumerable<T>`, nếu không có thì lấy kiểu của item đầu tiên. Sau đó gọi `SearchProfiles.Resolve(T)` và ghép với các giá trị ghi đè trên behavior thành `SearchProfile`.
2. Behavior gán bằng `SetCurrentValue`: `DisplayMember = profile.DisplayField`, `IncrementalFiltering = true`, `ImmediatePopup = true`, `AutoComplete = false`. Nếu editor là `LookUpEdit` thì gán thêm `AutoPopulateColumns = false`.
3. Cột: `LookUpEdit.GetGridControl()` trả `null` cho tới khi popup mở lần đầu, nên cột được dựng trong `PopupOpened`. Luôn có 2 cột, **Mã trước, Tên sau**, và `TableView.AutoWidth = true`. Cột chỉ dựng lại khi grid hoặc profile đổi.
4. Lọc: trong `SubstituteDisplayFilter`, `e.DisplayFilter` được thay theo `EffectiveMode`:
   - Code: `VnStartsWith([CodeField], text)`
   - Name: `VnContainsAll([NameField], text)`
   - All: `Or` của hai điều kiện trên.
   - Sau đó đặt `e.Handled = true`.
5. `VnSearchFunctions.EnsureRegistered()` chạy trong static ctor của behavior. Hàm này kiểm tra `GetCustomFunction` trước khi đăng ký, vì `RegisterCustomFunction` ném exception nếu tên đã có.

## 5. Luật so khớp (`SearchMatcher`)

- Chuẩn hóa: bỏ dấu (FormD, loại `NonSpacingMark`), `đ` thành `d`, chữ thường, trim.
- **Code**: mã đã chuẩn hóa `StartsWith` chuỗi gõ vào. `kh0` khớp `KH001`, không khớp `AKH001`.
- **Name**: tách chuỗi gõ vào theo khoảng trắng, **mọi từ** phải có trong tên. `ng an` khớp `Nguyễn Văn An`; thứ tự từ không quan trọng.
- Chuỗi rỗng thì khớp tất cả.
- Query đã chuẩn hóa được cache (chỉ giữ query gần nhất), vì custom function được gọi cho từng dòng với cùng một query.

## 6. Bất biến KHÔNG được phá vỡ

1. `SearchCore.cs` **không** `using System.Windows*` hay `DevExpress*`.
2. Trên editor đã gắn behavior, **không** set `DisplayMember`, `AutoPopulateColumns`, `IncrementalFiltering`, `AutoComplete`, `PopupContentTemplate` có cột. Behavior quản lý các thuộc tính này. Muốn đổi thì dùng thuộc tính của behavior hoặc cấu hình model.
3. Luôn dùng `SetCurrentValue`, không gán trực tiếp, để không phá Binding.
4. Cột luôn theo thứ tự **Mã | Tên**, bất kể `Display` là gì.
5. Tên trường đưa vào `OperandProperty` phải là **property public** của item, vì CriteriaOperator không thấy property implement tường minh.
6. Bỏ đăng ký mọi event và `DependencyPropertyDescriptor.RemoveValueChanged` trong `OnDetaching` để tránh rò rỉ bộ nhớ.

## 7. Điểm CHƯA kiểm chứng (phải build và chạy thử trước tiên)

Code được viết dựa trên tài liệu DevExpress, chưa build với v25 thật. Ưu tiên kiểm tra:

- [ ] Biên dịch được. Đặc biệt kiểm tra: event `PopupOpened` (method nhận `EventArgs`, dựa vào contravariance); tên các DP `ImmediatePopupProperty`, `AutoCompleteProperty`, `IncrementalFilteringProperty`; kiểu nullable của `IncrementalFiltering`.
- [ ] `SubstituteDisplayFilter` có được gọi khi `IncrementalFiltering = true` và custom function có được evaluate khi lọc in-memory hay không.
- [ ] Đổi `SearchMode` **khi popup đang mở và ô đang có chữ**: danh sách có tự lọc lại không. Nếu không, tìm API làm mới filter của `LookUpEditBase` (hoặc gán lại `EditText` / `DisplayText`) và gọi trong `OnConfigChanged`.
- [ ] Dropdown có bị trống/nhấp nháy ở lần mở đầu vì cột được dựng trong `PopupOpened` không. Nếu có, chuyển sang `PopupContentTemplate` dựng sẵn (xem T4).
- [ ] `Display = Code` với tài khoản: chọn xong ô hiện số TK; gõ lại thì lọc theo mode hiện tại.

## 8. Việc cần làm tiếp (theo thứ tự ưu tiên)

Mỗi task có tiêu chí hoàn thành (AC). Làm từng task, build và test xong mới sang task tiếp.

### T1. Build và sửa lỗi theo mục 7
- **AC:** mọi ô ở mục 7 được tick; ghi lại API thực tế đã dùng vào mục 4.

### T2. Unit test `SearchCore.cs`
- Project test (xUnit), chỉ tham chiếu core.
- `TextNormalizer`: `"Nguyễn Đức"` thành `"nguyen duc"`; null/rỗng.
- `SearchMatcher`: các ví dụ ở mục 5; query đổi liên tục (cache không trả sai kết quả).
- `SearchProfiles`: thứ tự ưu tiên Register > attribute > ISearchItem; Register cho lớp cha áp dụng cho lớp con; `GetPath` với `x => x.A.B` và với property kiểu value type (có `Convert`); model không cấu hình thì trả null.
- **AC:** pass hết; coverage `SearchCore.cs` ≥ 90%.

### T3. Test tích hợp custom function
- Dùng `ExpressionEvaluator` của DevExpress để evaluate `VnSearchFunctions.StartsWith/ContainsAll` trên list object, không cần UI.
- **AC:** kết quả trùng với `SearchMatcher` trên cùng bộ dữ liệu.

### T4. Độ rộng cột và template dựng sẵn
- Cột Mã: rộng cố định theo profile (thêm `CodeWidth` vào `SearchProfileAttribute`/`SearchProfile`, mặc định 100). Cột Tên lấp phần còn lại.
- Nếu ở T1 thấy nhấp nháy: tạo `ResourceDictionary` với `PopupContentTemplate` có `dxg:GridControl x:Name="PART_GridControl"` và để behavior chỉ cập nhật `FieldName`/`Header`.
- **AC:** mở lần đầu không nhấp nháy; số TK dài không bị cắt.

### T5. Áp dụng toàn ứng dụng bằng Style
- Implicit style cho `dxg:LookUpEdit` gắn behavior qua `dxmvvm:Interaction.BehaviorsTemplate`, để không phải khai báo ở từng chỗ.
- **AC:** LookUpEdit bind tới `Product`/`Account` tự có 2 cột và hiển thị đúng mà XAML không cần khai báo behavior.

### T6. Tách logic dùng chung và hỗ trợ lưới
Làm theo thiết kế ở mục 10.2.
- Tách `SearchLookUpApplier` (lớp tĩnh) ra khỏi `SearchLookUpBehavior`. Behavior chỉ còn gọi applier.
- Viết `GridLookUpHelper`: attached property trên cột hoặc `TableView`, bắt `ShownEditor` và gắn các behavior vào `ActiveEditor`.
- Dùng được cả hai cách cho lưới: `CellTemplate` có `PART_Editor`, và `GridLookUpHelper`.
- **AC:** cùng một loại dữ liệu, trong lưới và trên form có cùng hiển thị, cùng 2 cột, cùng cách lọc; không có logic tìm kiếm nào bị lặp code.

### T7. `LookUpActionBehavior` – khung cho hành vi nghiệp vụ
Làm theo đặc tả ở mục 10.3. Đây là khung chung; các ô nghiệp vụ cụ thể chỉ cần bind Command.
- **AC:** đạt đủ các tiêu chí trong bảng ở mục 10.3; có ít nhất một ô nghiệp vụ mẫu dùng được ở cả lưới lẫn form.

### T8. Đổi mode bằng phím tắt và placeholder
- `Ctrl+M` xoay vòng Code → Name → All (dùng `SetCurrentValue` lên `SearchModeProperty`). Cân nhắc `BindsTwoWayByDefault` để ViewModel nhận được giá trị mới.
- `NullText` của editor đổi theo mode: "Tìm theo mã…", "Tìm theo tên…", "Tìm mã hoặc tên…".
- **AC:** bấm phím thì danh sách lọc lại ngay (phụ thuộc kết quả mục 7) và placeholder đổi theo.

### T9. Hiệu năng danh sách lớn
- Mỗi dòng đang bị normalize lại mỗi lần lọc. Nếu > 20k dòng mà gõ bị giật: cache theo item (`ConditionalWeakTable<object, string[]>`), hoặc cho model expose sẵn property đã chuẩn hóa (VD `[SearchName(NormalizedProperty = "TenTK_KhongDau")]`).
- **AC:** 50k dòng, mỗi lần lọc dưới 50ms.

## 9. Kiểm tra thủ công sau mỗi thay đổi

1. Sản phẩm: gõ `ao thun`, thấy các dòng có cả "áo" và "thun"; chọn xong ô hiện **tên**.
2. Tài khoản: gõ `13`, thấy 131, 133, 1331…; chọn xong ô hiện **số TK**.
3. Tài khoản đổi sang Name, gõ `phai thu`, thấy "Phải thu của khách hàng".
4. Đổi `SearchMode` từ ViewModel khi ô đang có chữ: danh sách lọc lại.
5. Hai LookUpEdit bind **cùng** một collection: lọc ở cái này không ảnh hưởng cái kia.
6. ViewModel thay cả collection: cột và hiển thị vẫn đúng.
7. Model không có cấu hình: editor chạy như mặc định, không exception.
8. Đóng/mở cửa sổ nhiều lần: không rò rỉ bộ nhớ (kiểm tra bằng profiler, behavior phải được detach).

## 10. Dùng bộ này làm core để xây ô nghiệp vụ

Phần này dành cho việc xây các ô chọn cụ thể (tài khoản, khách hàng, vật tư, kho…). Ví dụ điển hình: một ô chọn dùng được **cả trong lưới lẫn trên form**, có **nút "..."** bên cạnh, và khi người dùng chọn xong thì chạy một **hành vi nghiệp vụ** (xác định loại đối tượng, mở form chi tiết…). Nút "..." chạy cùng hành vi đó với giá trị đang có sẵn (ví dụ dữ liệu cũ vừa load lên).

### 10.1 Nguyên tắc: ghép, không kế thừa

- **Không** kế thừa `LookUpEdit`, **không** tạo UserControl bọc LookUpEdit + Button, **không** dùng `partial` để chia logic.
  - UserControl không dùng làm editor trong lưới được, vì lưới DevExpress chỉ nhận editor kế thừa `BaseEdit`. Validation, focus và Tab cũng không đi qua UserControl gọn được.
  - `partial` chỉ chia một class ra nhiều file, không giải quyết việc dùng lại ở hai nơi.
- Một ô nghiệp vụ được **ghép** từ các mảnh độc lập:

| Mảnh | Nằm ở | Lo việc gì |
|---|---|---|
| `SearchLookUpBehavior` (core) | Controls.Dx | Tìm, lọc, 2 cột, hiển thị mã/tên. **Không chứa nghiệp vụ.** |
| `ButtonInfo` trong `Buttons` | XAML của editor / `EditSettings` | Nút "..." (có sẵn vì `LookUpEdit` kế thừa `ButtonEdit`). |
| `LookUpActionBehavior` (T7) | Controls.Dx | Chỉ **kích hoạt** Command khi người dùng chọn xong hoặc bấm nút. |
| Service nghiệp vụ (VD `IXxxDetailService`) | Tầng Service/ViewModel | Toàn bộ logic: xác định loại, mở form nào, ghi kết quả về đâu. Unit test được. |
| Command trong ViewModel | ViewModel | Nối behavior với service, truyền context. |

- Behavior nghiệp vụ cụ thể (nếu thật sự cần) **gắn thêm bên cạnh** core, không sửa core và không kế thừa core.
- Core (`SearchCore.cs`, `SearchLookUpBehavior`) **không được** tham chiếu tới bất kỳ model hay service nghiệp vụ nào.

### 10.2 Một ô, hai nơi đặt

Mục tiêu: cùng một khai báo cho ra cùng một hành vi, dù ô nằm trong lưới hay trên form.

**Trên form**
```xml
<dxg:LookUpEdit ItemsSource="{Binding Items}" EditValue="{Binding SelectedCode}">
    <dxg:LookUpEdit.Buttons>
        <dxe:ButtonInfo GlyphKind="Regular"/>
    </dxg:LookUpEdit.Buttons>
    <dxmvvm:Interaction.Behaviors>
        <dxs:SearchLookUpBehavior/>
        <dxs:LookUpActionBehavior Command="{Binding OpenDetailCommand}"/>
    </dxmvvm:Interaction.Behaviors>
</dxg:LookUpEdit>
```

**Trong lưới**: cột dùng `LookUpEditSettings`, cũng có `Buttons`. Behavior không gắn vào settings được vì editor trong ô được tạo động. Có hai cách:
- `CellTemplate` chứa `dxg:LookUpEdit x:Name="PART_Editor"` gắn các behavior như trên.
- `GridLookUpHelper` (T6): attached property trên cột hoặc view, bắt `ShownEditor` và gắn behavior vào `view.ActiveEditor`.

**Context:** trên form, context là ViewModel của form; trong lưới, context là **object của dòng đang sửa** (`view.FocusedRowData.Row` hoặc `DataContext` của editor trong ô). Service cần context để ghi kết quả (VD mã đối tượng chi tiết) về đúng nơi.

**Dùng lại nhiều chỗ:** khi một tổ hợp (core + nút + action) lặp lại nhiều nơi, đóng gói bằng **Style** trong ResourceDictionary (dùng `dxmvvm:Interaction.BehaviorsTemplate`) cho form, và **CellTemplate dùng chung** cho lưới. Chỉ khi Style không đủ mới cân nhắc cặp `XxxLookUpEdit` + `XxxLookUpEditSettings` kế thừa DevExpress; cặp này phải đăng ký editor với DevExpress để dùng được trong lưới.

### 10.3 Đặc tả `LookUpActionBehavior` (T7)

Behavior chung, không biết gì về nghiệp vụ. Thuộc tính:

| Thuộc tính | Kiểu | Ý nghĩa |
|---|---|---|
| `Command` | `ICommand` | Lệnh được gọi. Tham số là `LookUpActionArgs`. |
| `TriggerOnUserSelect` | `bool` (mặc định `true`) | Gọi khi người dùng chọn xong một giá trị mới. |
| `TriggerOnButton` | `bool` (mặc định `true`) | Gọi khi bấm nút "...". |
| `ButtonIndex` | `int` (mặc định: nút không phải dropdown đầu tiên) | Nút nào trong `Buttons` là nút "...". |
| `EmptyButtonMode` | enum `Disable` / `Invoke` | Khi ô trống: vô hiệu hóa nút, hay vẫn gọi Command với `Item = null` (để nghiệp vụ tự quyết, VD mở danh mục). |
| `Context` | `object` | Context tùy chọn. Nếu không set: trong lưới lấy object của dòng, ngoài lưới lấy `DataContext` của editor. |

`LookUpActionArgs`: `Item` (object đã chọn, lấy qua `GetItemFromValue`/`SelectedItem`), `Value` (`EditValue`), `Context`, `Trigger` (`UserSelect` / `Button`), `IsInGrid`.

Yêu cầu bắt buộc:

| # | Yêu cầu | Lý do |
|---|---|---|
| 1 | **Chỉ** kích hoạt `UserSelect` khi giá trị đổi do người dùng: editor đang có keyboard focus, và giá trị mới khác giá trị lúc nhận focus. | `EditValueChanged` cũng bắn khi load dữ liệu cũ hoặc ViewModel gán giá trị. Thiếu điều kiện này thì mở chứng từ cũ ra là form bật lên liên tục. |
| 2 | Không kích hoạt khi người dùng xóa trắng ô. | Xóa không phải là chọn. |
| 3 | Trong lưới, gọi Command **sau khi** giá trị đã ghi vào dòng (`Dispatcher.BeginInvoke` mức `Background`, hoặc qua `CellValueChanged` của view). | Mở dialog giữa lúc ô đang sửa dễ mất giá trị hoặc rối focus. |
| 4 | Nút "..." dùng giá trị **hiện có** của ô, kể cả giá trị load từ dữ liệu cũ. | Đúng nhu cầu xem/sửa chi tiết của dòng đã có. |
| 5 | Kiểm tra `Command.CanExecute(args)` trước khi gọi; nút "..." bị disable khi `CanExecute` trả false. | Nghiệp vụ tự quyết lúc nào cho phép. |
| 6 | Không gọi lặp: chọn bằng Enter rồi popup đóng chỉ được gọi **một** lần. | Tránh mở form hai lần. |
| 7 | Detach sạch: bỏ mọi event trong `OnDetaching`. | Trong lưới editor được tạo và hủy liên tục, không dọn sẽ rò rỉ bộ nhớ. |

### 10.4 Checklist khi xây một ô nghiệp vụ mới

1. Model đã có cấu hình tìm kiếm chưa (mục 3)? Nếu chưa thì thêm attribute, hoặc `SearchProfiles.Register`.
2. Viết service nghiệp vụ (interface + implementation) và unit test cho nó. **Không** viết logic nghiệp vụ trong behavior hay code-behind.
3. Thêm Command vào ViewModel, gọi service với `LookUpActionArgs`.
4. Khai báo ô trên form: core + `ButtonInfo` + `LookUpActionBehavior`.
5. Khai báo cột trong lưới bằng cùng tổ hợp đó (CellTemplate dùng chung hoặc `GridLookUpHelper`).
6. Nếu tổ hợp lặp lại nhiều nơi: đóng gói thành Style và CellTemplate trong ResourceDictionary.
7. Chạy kiểm tra thủ công:
   - chọn bằng chuột, bằng Enter, bằng Tab: mỗi lần chỉ gọi **một** lần;
   - load dữ liệu cũ: **không** gọi;
   - bấm "..." trên dòng có dữ liệu cũ: gọi với đúng item và đúng dòng;
   - xóa trắng ô: không gọi; nút xử lý theo `EmptyButtonMode`;
   - trong lưới: kết quả từ form chi tiết ghi đúng vào dòng đang sửa, không ghi nhầm sang dòng khác;
   - trên form và trong lưới: hành vi giống hệt nhau.

## 11. Môi trường

- DevExpress WPF **v25.x**: gói NuGet `DevExpress.Wpf.Core`, `DevExpress.Wpf.Grid.Core`, `DevExpress.Data`.
- .NET 8 (`net8.0-windows`, `<UseWPF>true</UseWPF>`) hoặc .NET Framework 4.6.2+ theo yêu cầu của DevExpress v25.
- Cú pháp C# 7.3+.
