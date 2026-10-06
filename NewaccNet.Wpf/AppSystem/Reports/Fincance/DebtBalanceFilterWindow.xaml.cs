using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NewaccNet.Wpf.Views.Base;
using DataAccess.FactoryClasses;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class DebtBalanceFilterWindow : BaseWindow
    {
        public DebtBalanceFilterWindow()
        {
            InitializeComponent();
        }

                private string GetSettingsFilePath()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DebtBalanceSettings.json");
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new
                {
                    FromDate = dtFromDate.DateTime,
                    ToDate = dtToDate.DateTime,
                    ReportDate = dtReportDate.DateTime
                };
                string json = System.Text.Json.JsonSerializer.Serialize(settings);
                System.IO.File.WriteAllText(GetSettingsFilePath(), json);
            }
            catch { }
        }

        private void LoadSettings()
        {
            try
            {
                string path = GetSettingsFilePath();
                if (System.IO.File.Exists(path))
                {
                    string json = System.IO.File.ReadAllText(path);
                    var settings = System.Text.Json.JsonDocument.Parse(json);
                    
                    if (settings.RootElement.TryGetProperty("FromDate", out var fd)) dtFromDate.DateTime = fd.GetDateTime();
                    if (settings.RootElement.TryGetProperty("ToDate", out var td)) dtToDate.DateTime = td.GetDateTime();
                    if (settings.RootElement.TryGetProperty("ReportDate", out var rd)) dtReportDate.DateTime = rd.GetDateTime();
                    return;
                }
            }
            catch { }

            // Default
            DateTime today = DateTime.Today;
            if (dtFromDate != null) dtFromDate.DateTime = new DateTime(today.Year, 1, 1);
            if (dtToDate != null) dtToDate.DateTime = today;
            if (dtReportDate != null) dtReportDate.DateTime = today;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
        }

                private void BtnDesign_Click(object sender, RoutedEventArgs e)
        {
            var designWindow = new NewaccNet.Wpf.AppSystem.Design.BalanceSheetDesignWindow();
            designWindow.Show();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (btnOK != null) btnOK.IsEnabled = false;
                if (btnCancel != null) btnCancel.IsEnabled = false;
                if (btnOK != null) btnOK.Content = "Đang tính...";

                DateTime reportDate = dtReportDate.DateTime;
                DateTime fromDate = dtFromDate.DateTime;
                DateTime toDate = dtToDate.DateTime;

                SaveSettings();

                var debtBalances = await System.Threading.Tasks.Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var service = new NewaccNet.Reports.BalanceSheet.DebtBalanceService();
                    
                    var newBalances = service.GenerateDebtBalances(adapter, fromDate, toDate, reportDate, true);
                    
                    // Xóa dữ liệu cũ và lưu dữ liệu mới (Nếu bạn có bảng DebtBalance thực sự trong DB)
                    adapter.DeleteEntitiesDirectly(typeof(DataAccess.EntityClasses.DebtBalanceEntity), null);
                    adapter.SaveEntityCollection(new DataAccess.HelperClasses.EntityCollection<DataAccess.EntityClasses.DebtBalanceEntity>(newBalances));
                    
                    // Fetch lại từ DB để tránh lỗi OutOfSync của LLBLGen khi đọc property sau khi Save
                    var qf = new DataAccess.FactoryClasses.QueryFactory();
                    var savedBalances = adapter.FetchQuery(qf.DebtBalance).Cast<DataAccess.EntityClasses.DebtBalanceEntity>().ToList();
                    return savedBalances;

                    // return newBalances;
                });

                if (dgDebtBalance != null)
                {
                    dgDebtBalance.ItemsSource = debtBalances.Select(x => new 
                    {
                        TàiKhoản = x.AccountId,
                        KỳHạn = x.LongtermFlag == 0 ? "0 (Mặc định)" : x.LongtermFlag == 1 ? "1 (Ngắn)" : "2 (Dài)",
                        DưĐầuNợ = x.Begindebit,
                        DưĐầuCó = x.Begincredit,
                        PSTrongKỳNợ = x.Indebit,
                        PSTrongKỳCó = x.Incredit,
                        DưCuốiNợ = x.Enddebit,
                        DưCuốiCó = x.Endcredit
                    }).ToList();
                }
                MessageBox.Show("Đã tính toán và hiển thị dữ liệu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                MessageBox.Show(this, "Đã tạo và tính toán xong dữ liệu Debt Balance!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Lỗi khi xử lý: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                if (btnOK != null) btnOK.IsEnabled = true;
                if (btnCancel != null) btnCancel.IsEnabled = true;
                if (btnOK != null) btnOK.Content = "OK - Thực hiện (Enter)";
            }
        }
    }
}












