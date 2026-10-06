using System;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;
using DataAccess.EntityClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using NewaccNet.Reports;
using NewaccNet.Reports.BalanceSheet;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class BalanceSheetFilterWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public BalanceSheetFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var qf = new QueryFactory();
                    var accounts = adapter.FetchQuery(qf.ChartOfAccount).Cast<ChartOfAccountEntity>().ToList();
                    cboSelectedAccounts.ItemsSource = accounts.OrderBy(a => a.AccountId).ToList();
                    cboSelectedAccounts.SelectAll();
                }
            }
            catch {}

            DateTime today = DateTime.Today;
            dtFromDate.DateTime = new DateTime(today.Year, 1, 1);
            dtToDate.DateTime = today;
            dtReportDate.DateTime = today;

            // Load settings
            UserPreferencesHelper.LoadState(this);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save settings
                UserPreferencesHelper.SaveState(this);

                Mouse.OverrideCursor = Cursors.Wait;
                btnOK.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnOK.Content = "Đang tính...";

                DateTime fromDate = dtFromDate.DateTime;
                DateTime toDate = dtToDate.DateTime;
                DateTime reportDate = dtReportDate.DateTime;
                bool useOldData = chkUseOldData.IsChecked ?? false;
                bool onlyBooked = chkOnlyBooked.IsChecked ?? true;

                // Lấy danh sách AccountId người dùng chọn từ TokenEdit
                List<string> selectedAccounts = new List<string>();
                if (cboSelectedAccounts.EditValue is List<object> selList)
                {
                    selectedAccounts = selList.Select(x => x?.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                }
                else if (cboSelectedAccounts.EditValue != null)
                {
                    // Đôi khi DevExpress trả về chuỗi các ID phân cách bằng dấu phẩy
                    string val = cboSelectedAccounts.EditValue.ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        selectedAccounts = val.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(x => x.Trim()).ToList();
                    }
                }

                int allAccountsCount = ((List<ChartOfAccountEntity>)cboSelectedAccounts.ItemsSource)?.Count ?? 0;

                var dtoList = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    
                    if (!useOldData)
                    {
                        var trialService = new TrialBalanceService();
                        var newTb = trialService.GenerateTrialBalance(adapter, fromDate, toDate, onlyBooked);
                        adapter.DeleteEntitiesDirectly(typeof(DataAccess.EntityClasses.TrialBalanceEntity), null);
                        adapter.SaveEntityCollection(new DataAccess.HelperClasses.EntityCollection<DataAccess.EntityClasses.TrialBalanceEntity>(newTb));

                        var debtService = new DebtBalanceService();
                        var newDebt = debtService.GenerateDebtBalances(adapter, fromDate, toDate, reportDate, onlyBooked);
                        adapter.DeleteEntitiesDirectly(typeof(DataAccess.EntityClasses.DebtBalanceEntity), null);
                        adapter.SaveEntityCollection(new DataAccess.HelperClasses.EntityCollection<DataAccess.EntityClasses.DebtBalanceEntity>(newDebt));
                    }

                    var bsService = new BalanceSheetService();
                    var calculatedList = bsService.GenerateReport(adapter);

                    // Lọc theo selectedAccounts nếu cần (có thể B01-DN không có nhu cầu lọc này, nhưng giữ lại cũng không sao nếu DTO có AccountId)
                    if (selectedAccounts.Count > 0 && selectedAccounts.Count < allAccountsCount)
                    {
                        calculatedList = calculatedList.Where(x => x.AccountId != null && selectedAccounts.Contains(x.AccountId)).ToList();
                    }

                    return calculatedList;
                });

                Mouse.OverrideCursor = null;
                btnOK.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnOK.Content = "Đồng ý";

                var report = new BalanceSheet();
                report.DataSource = dtoList;
                if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                report.RequestParameters = false;

                report.CreateDocument(false);

                var previewWindow = new DocumentPreviewWindow();
                previewWindow.Owner = this.Owner ?? this;
                previewWindow.Title = "Bảng Cân Đối Kế Toán (B01-DN)";
                previewWindow.PreviewControl.DocumentSource = report;
                
                previewWindow.Show();
                // this.Close(); // Giữ lại form filter theo yêu cầu
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                btnOK.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnOK.Content = "Đồng ý";
                MessageBox.Show(this, "Lỗi khi xử lý: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}



