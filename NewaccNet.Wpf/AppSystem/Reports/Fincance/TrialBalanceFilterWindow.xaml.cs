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
using NewaccNet.Reports.TrialBalance;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class TrialBalanceFilterWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public TrialBalanceFilterWindow()
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
                    var qf = new QueryFactory();

                    if (!useOldData)
                    {
                        var service = new TrialBalanceService();
                        var newBalances = service.GenerateTrialBalance(adapter, fromDate, toDate, onlyBooked);

                        adapter.DeleteEntitiesDirectly(typeof(DataAccess.EntityClasses.TrialBalanceEntity), null);
                        adapter.SaveEntityCollection(new DataAccess.HelperClasses.EntityCollection<DataAccess.EntityClasses.TrialBalanceEntity>(newBalances));
                    }

                    var q = qf.TrialBalance.WithPath(DataAccess.EntityClasses.TrialBalanceEntity.PrefetchPathChartOfAccount);
                    var entities = adapter.FetchQuery(q).Cast<DataAccess.EntityClasses.TrialBalanceEntity>().ToList();

                    var calculatedList = entities.Select(en => new TrialBalanceDTO
                    {
                        AccountId = en.AccountId,
                        AccountName = en.ChartOfAccount?.AccountName,
                        BeginDebit = (decimal)(en.Begindebit ?? 0),
                        BeginCredit = (decimal)(en.Begincredit ?? 0),
                        IntDebit = (decimal)(en.Intdebit ?? 0),
                        IntCredit = (decimal)(en.Intcredit ?? 0),
                        EndDebit = (decimal)(en.Enddebit ?? 0),
                        EndCredit = (decimal)(en.Endcredit ?? 0),
                        Splite = (en.Splite == 1),
                        Highlight = (en.Splite == 1)
                    }).OrderBy(x => x.AccountId).ToList();

                    // Lọc theo selectedAccounts (nếu có và không phải là chọn tất cả)
                    
                    if (selectedAccounts.Count > 0 && selectedAccounts.Count < allAccountsCount)
                    {
                        calculatedList = calculatedList.Where(x => selectedAccounts.Contains(x.AccountId)).ToList();
                    }

                    return calculatedList;
                });

                Mouse.OverrideCursor = null;
                btnOK.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnOK.Content = "Đồng ý";

                var report = new TrialBalance();
                report.DataSource = dtoList;
                if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                report.RequestParameters = false;
                
                // Add bold formatting for Splite row
                var detailBand = report.Bands.GetBandByType(typeof(DetailBand));
                if (detailBand != null)
                {
                    var tableRow2 = detailBand.FindControl("tableRow2", true) as XRTableRow;
                    if (tableRow2 != null)
                    {
                        tableRow2.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Font.Bold", "Iif([Splite], True, False)"));
                        foreach(XRTableCell cell in tableRow2.Cells) {
                            cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Tag", "[AccountId]"));
                        }
                    }
                }

                report.CreateDocument(false);

                var previewWindow = new DocumentPreviewWindow();
                previewWindow.Owner = this.Owner ?? this;
                previewWindow.Title = "Bảng Cân Đối Phát Sinh";
                previewWindow.PreviewControl.DocumentSource = report;
                
                // Drill-down to Ledger
                previewWindow.PreviewControl.DocumentPreviewMouseDoubleClick += (sPreview, eClick) =>
                {
                    var visualBrick = eClick.Brick as DevExpress.XtraPrinting.VisualBrick;
                    if (visualBrick != null && visualBrick.Value != null)
                    {
                        string clickedAccountId = visualBrick.Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(clickedAccountId))
                        {
                            if (!string.IsNullOrEmpty(clickedAccountId))
                            {
                                // Open Ledger report
                                NewaccNet.Wpf.AppSystem.Reports.LedgerReportService.ShowPreview(previewWindow, fromDate, toDate, clickedAccountId, true);
                            }
                        }
                    }
                };

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



