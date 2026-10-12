using NewaccNet.Wpf.Controls.Search;

namespace NewaccNet.Wpf.Controls.Search.Examples
{
    // ── Danh mục sản phẩm: hiển thị TÊN, mặc định tìm cả mã lẫn tên ─────────
    [SearchProfile(Display = SearchDisplay.Name, DefaultMode = SearchMode.All,
                   CodeHeader = "Mã SP", NameHeader = "Tên sản phẩm")]
    public class Product
    {
        public int Id { get; set; }
        [SearchCode] public string Sku { get; set; }
        [SearchName] public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    // ── Tài khoản kế toán: hiển thị MÃ (số TK), mặc định tìm theo mã ────────
    [SearchProfile(Display = SearchDisplay.Code, DefaultMode = SearchMode.Code,
                   CodeHeader = "Số TK", NameHeader = "Tên tài khoản")]
    public class Account
    {
        [SearchCode] public string SoTK { get; set; }
        [SearchName] public string TenTK { get; set; }
        public int CapTK { get; set; }
    }

    // ── Model dùng ISearchItem (phải implement PUBLIC) ──────────────────────
    public class Warehouse : ISearchItem
    {
        public string MaKho { get; set; }
        public string TenKho { get; set; }

        public string SearchCode => MaKho;
        public string SearchName => TenKho;
    }

    // ── DTO sinh tự động, không sửa được -> đăng ký bằng code ───────────────
    public class CustomerDto
    {
        public string MaKH { get; set; }
        public string TenKH { get; set; }
    }

    public static class SearchExamplesSetup
    {
        /// <summary>Gọi trong App.OnStartup.</summary>
        public static void Configure()
        {
            SearchProfiles.DefaultCodeHeader = "Mã";
            SearchProfiles.DefaultNameHeader = "Tên";

            SearchProfiles.Register<CustomerDto>(
                x => x.MaKH, x => x.TenKH,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã KH", nameHeader: "Tên khách hàng");
        }
    }
}

/* ── XAML ────────────────────────────────────────────────────────────────
xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
xmlns:dxs="clr-namespace:MyApp.Controls.Dx"

<!-- Sản phẩm: hiện tên, cột "Mã SP | Tên sản phẩm" -->
<dxg:LookUpEdit ItemsSource="{Binding Products}"
                ValueMember="Id" EditValue="{Binding ProductId}">
    <dxmvvm:Interaction.Behaviors>
        <dxs:SearchLookUpBehavior/>
    </dxmvvm:Interaction.Behaviors>
</dxg:LookUpEdit>

<!-- Tài khoản: hiện số TK; cho người dùng đổi mode tìm qua ViewModel -->
<dxg:LookUpEdit ItemsSource="{Binding Accounts}"
                ValueMember="SoTK" EditValue="{Binding SoTK}">
    <dxmvvm:Interaction.Behaviors>
        <dxs:SearchLookUpBehavior SearchMode="{Binding AccountSearchMode}"/>
    </dxmvvm:Interaction.Behaviors>
</dxg:LookUpEdit>

<!-- Ghi đè tại chỗ: màn hình này muốn tài khoản hiện TÊN -->
<dxs:SearchLookUpBehavior Display="Name" SearchMode="All"/>
*/
