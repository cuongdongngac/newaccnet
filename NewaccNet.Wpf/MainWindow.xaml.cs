using System;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ThemedWindow {
    private bool _isExiting = false;
    private void BtnVoucherTest_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) { new NewaccNet.Wpf.AppSystem.Voucher.VoucherListView().Show(); }
    private void BtnOpeningBalance_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) { new NewaccNet.Wpf.AppSystem.Voucher.OpeningBalanceWindow().Show(); }
    private void BtnCostAllocation_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) { new NewaccNet.Wpf.AppSystem.CostAllocation.CostAllocationWindow().Show(); }
    private void BtnCostTransfer_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) { new NewaccNet.Wpf.AppSystem.CostTransfer.CostTransferWindow().Show(); }
        private void BtnAssetDepreciation_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) { new NewaccNet.Wpf.AppSystem.AssetDepreciation.AssetDepreciationWindow().Show(); }
    public MainWindow(string username, string fullName, int roleMask)
    {
        InitializeComponent();
        this.Closing += MainWindow_Closing;

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

    private void BtnTransferBalance_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var transferWin = new NewaccNet.Wpf.AppSystem.BalanceTransfer.TransferBalanceWindow();
        transferWin.Owner = this;
        transferWin.ShowDialog();
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

    private void BtnDesignTaxObligation_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.TaxObligationDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnDesignCashFlow_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.CashFlowDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnDesignVatReturn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.VatReturnDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnDesignVatDeclare_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Design.VatDeclareDesignWindow();
        win.Owner = this;
        win.Show();
    }

    private void BtnReportDiary_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DiaryFilterWindow { Owner = this };
        filter.Show();
    }
    
        private void BtnReportTrialBalance_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var filterForm = new NewaccNet.Wpf.AppSystem.Reports.Fincance.TrialBalanceFilterWindow();
        filterForm.Owner = this;
        filterForm.ShowDialog();
    }
    private void BtnReportLedger_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.GeneralLedgerFilterWindow { Owner = this };
        filter.Show();
    }
    private void BtnExit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        _isExiting = true;
        if (MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            Application.Current.Shutdown();
        }
        else
        {
            _isExiting = false;
        }
    }

    public static bool SuppressExitPrompt { get; set; } = false;

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (SuppressExitPrompt) return;

        if (!_isExiting)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }

    private void BtnCategoryMaterialPrice_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.MaterialPriceListView();
        win.Show();
    }

    private void BtnReportBalanceSheet_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.BalanceSheetFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnReportBusinessResult_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.BusinessResultFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnReportCashFlow_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.CashFlowFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnReportObligationTax_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.ObligationTaxFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnReportVatReturn_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.VatReturnFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnReportVatDeclare_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.VatDeclareFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnOpenDebtBalanceFilter_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Reports.Fincance.DebtBalanceFilterWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    // =========================================================================
    // KHUNG HÀM BÁO CÁO CHI TIẾT (STUB METHODS - SẼ TRIỂN KHAI CHI TIẾT SAU)
    // Dữ liệu nguồn: NewaccNet.Reports.FlattenedDetails.FlattenedDetailService
    // =========================================================================

    private void BtnReportDetailTrialBalance_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.DetailTrialBalanceFilterWindow { Owner = this };
        filter.ShowDialog();
    }

    private void BtnReportDebtDetail_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowDebtDetailReportStub();
    }

    private void BtnReportInventoryStock_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowInventoryStockReportStub();
    }

    private void BtnReportInventoryInOut_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowInventoryInOutReportStub();
    }

    private void BtnReportCostDetail_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowCostDetailReportStub();
    }

    private void BtnReportAssetDetail_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowAssetDetailReportStub();
    }

    private void BtnReportCurrencyDetail_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowCurrencyDetailReportStub();
    }

    private void BtnReportInvestmentDetail_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        ShowInvestmentDetailReportStub();
    }

    private void ShowDebtDetailReportStub()
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.Debt.DebtReportFilterWindow
        {
            Owner = this
        };
        filter.ShowDialog();
    }

    private void ShowInventoryStockReportStub()
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.DetailTrialBalanceFilterWindow(initialAccountPrefix: "152", initialCategory: "MATERIAL")
        {
            Owner = this
        };
        filter.ShowDialog();
    }

    private void ShowInventoryInOutReportStub()
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.DetailTrialBalanceFilterWindow(initialAccountPrefix: "152", initialCategory: "MATERIAL")
        {
            Owner = this
        };
        filter.ShowDialog();
    }

    private void ShowCostDetailReportStub()
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.DetailTrialBalanceFilterWindow(initialAccountPrefix: "154", initialCategory: "COST")
        {
            Owner = this
        };
        filter.ShowDialog();
    }

    private void ShowAssetDetailReportStub()
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.DetailTrialBalanceFilterWindow(initialAccountPrefix: "211", initialCategory: "ASSET")
        {
            Owner = this
        };
        filter.ShowDialog();
    }

    private void ShowCurrencyDetailReportStub()
    {
        var filter = new NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.DetailTrialBalanceFilterWindow(initialAccountPrefix: "1122", initialCategory: "CURRENCY")
        {
            Owner = this
        };
        filter.ShowDialog();
    }

    private void ShowInvestmentDetailReportStub()
    {
        MessageBox.Show(
            "[Khung chờ triển khai] Báo cáo Chi tiết Chứng khoán & Đầu tư.\n(Nguồn dữ liệu: FlattenedDetailService.LoadInvestmentFlatRows / InvestmentFlatRow)",
            "Báo cáo Chi tiết Chứng khoán",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void BtnHddtDownload_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.TaxInvoice.TaxInvoiceDownloadWindow();
        win.Owner = this;
        win.ShowDialog();
    }

    private void BtnHddtReview_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.TaxInvoice.TaxInvoiceReviewWindow();
        win.Owner = this;
        win.Show();
    }
}
