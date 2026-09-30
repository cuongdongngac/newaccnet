using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ThemedWindow
{
    private void BtnVoucherTest_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) { new NewaccNet.Wpf.AppSystem.Voucher.VoucherListView().Show(); }
    public MainWindow(string username, string fullName, int roleMask)
    {
        InitializeComponent();

        // Hiển thị thông tin user đăng nhập trên StatusBar
        txtStatusUsername.Text = username;
        txtStatusFullName.Text = fullName;
        // Hiển thị RoleMask dạng chuỗi bit (32-bit)
        txtStatusRoleMask.Text = $"{roleMask} → {Convert.ToString(roleMask, 2).PadLeft(32, '0')}";

        this.Closed += (_, _) => Application.Current.Shutdown();

        LoadBackground();
    }


    private void LoadBackground()
    {
        try
        {
            // Tìm ảnh newacc.jpg ở thư mục chạy hoặc thư mục project
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string imgPath = System.IO.Path.Combine(basePath, "newacc.jpg");

            if (!System.IO.File.Exists(imgPath))
            {
                imgPath = System.IO.Path.Combine(basePath, @"..\..\..\newacc.jpg");
            }

            if (System.IO.File.Exists(imgPath))
            {
                var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(System.IO.Path.GetFullPath(imgPath));
                bitmap.EndInit();
                imgBackground.Source = bitmap;
            }
        }
        catch { }
    }

    private void BtnCategoryAccount_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        // Mở form danh mục tài khoản (dạng danh sách)
        var win = new NewaccNet.Wpf.AppSystem.Directory.ChartOfAccountListView();
        win.LoadData();
        win.Show();
    }
    private void BtnCategoryCostElement_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.CostElementListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategorySource_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.SourceListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryCurrency_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.CurrencyListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryStockType_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.StockTypeListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryStock_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.StockListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryExchangeRate_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.ExchangeRateListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryWarehouse_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.WarehouseListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryCategory_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.CategoryListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryInventoryItem_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.InventoryItemListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryTaxRate_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.TaxRateListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryReason_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.ReasonListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryCostObject_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.CostObjectListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryDebtReason_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.DebtReasonListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryDebtType_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.DebtTypeListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryPartner_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.PartnerListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryDepartment_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.DepartmentListView();
        win.LoadData();
        win.Show();
    }

    private void BtnCategoryAsset_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.AssetListView();
        win.LoadData();
        win.Show();
    }

    private void BtnConfig_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var configWin = new NewaccNet.Wpf.AppSystem.DbConfigWindow();
        configWin.Owner = this;
        configWin.ShowDialog();
    }
    private void BtnUserManagement_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.UserListView();
        win.LoadData();
        win.Show();
    }

    private void BtnDesignBalanceSheet_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.BalanceSheetDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnDesignBusinessResult_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.BusinessResultDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnDesignCashFlow_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.CashFlowDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnReportDiary_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DiaryFilterWindow { Owner = this };
        if (filter.ShowDialog() != true) return;

        var args = (filter.FromDate, filter.ToDate, filter.OnlyBooked);
        System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
            new Action(() => NewaccNet.Wpf.AppSystem.Reports.DiaryReportService.ShowPreview(
                this, args.FromDate, args.ToDate, args.OnlyBooked)),
            System.Windows.Threading.DispatcherPriority.Background);
    }
}
