using System;
using System.Windows;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ThemedWindow {
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
}
