using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class VatDeclareFilterWindow : Views.Base.BaseWindow
    {
        public VatDeclareFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = ReportSettingsHelper.Load();
            var now = DateTime.Now;

            dtFromDate.DateTime = settings.VatDeclareFromDate ?? new DateTime(now.Year, 1, 1);
            dtToDate.DateTime = settings.VatDeclareToDate ?? new DateTime(now.Year, 12, 31);

            txtReportTitle.Text = !string.IsNullOrEmpty(settings.VatDeclareTitle)
                ? settings.VatDeclareTitle
                : "TỜ KHAI THUẾ VAT (MẪU SỐ 01/GTGT)";
        }

        private void SaveSettings()
        {
            var settings = ReportSettingsHelper.Load();
            settings.VatDeclareFromDate = dtFromDate.DateTime;
            settings.VatDeclareToDate = dtToDate.DateTime;
            settings.VatDeclareTitle = txtReportTitle.Text;
            ReportSettingsHelper.Save(settings);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (dtFromDate.DateTime == DateTime.MinValue || dtToDate.DateTime == DateTime.MinValue)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dtFromDate.DateTime > dtToDate.DateTime)
            {
                MessageBox.Show("Từ ngày không được lớn hơn Đến ngày.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveSettings();

            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                var fromDate = dtFromDate.DateTime.Date;
                var toDate = dtToDate.DateTime.Date;
                var reportTitle = string.IsNullOrWhiteSpace(txtReportTitle.Text)
                    ? "TỜ KHAI THUẾ VAT (MẪU SỐ 01/GTGT)"
                    : txtReportTitle.Text.Trim();

                var service = new NewaccNet.Reports.VatDeclare.VatDeclareService();

                var data = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    return service.Calculate(adapter, fromDate, toDate);
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                var report = new VatAdded();
                report.DataSource = data;

                if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                if (report.Parameters["prmReportTitle"] != null) report.Parameters["prmReportTitle"].Value = reportTitle;

                var companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (!string.IsNullOrEmpty(companyName) && report.Parameters["prmCompanyName"] != null)
                {
                    report.Parameters["prmCompanyName"].Value = companyName;
                }

                var taxCode = UserPreferencesHelper.GetSetting("TaxCode");
                if (!string.IsNullOrEmpty(taxCode) && report.Parameters["prmTaxCode"] != null)
                {
                    report.Parameters["prmTaxCode"].Value = taxCode;
                }

                var address = UserPreferencesHelper.GetSetting("CompanyAddress");
                if (!string.IsNullOrEmpty(address) && report.Parameters["prmAddress"] != null)
                {
                    report.Parameters["prmAddress"].Value = address;
                }

                report.RequestParameters = false;
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = "Tờ Khai Thuế VAT (Mẫu số 01/GTGT)"
                };
                previewWindow.PreviewControl.DocumentSource = report;
                previewWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(this, "Lỗi khi tính toán báo cáo: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnExportJson_Click(object sender, RoutedEventArgs e)
        {
            if (dtFromDate.DateTime == DateTime.MinValue || dtToDate.DateTime == DateTime.MinValue)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                var fromDate = dtFromDate.DateTime.Date;
                var toDate = dtToDate.DateTime.Date;

                var service = new NewaccNet.Reports.VatDeclare.VatDeclareService();

                var data = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    return service.Calculate(adapter, fromDate, toDate);
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                var dlg = new SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    FileName = $"VatDeclare_01GTGT_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.json",
                    Title = "Xuất dữ liệu Tờ khai thuế GTGT ra file JSON"
                };

                if (dlg.ShowDialog() == true)
                {
                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(dlg.FileName, json);
                    MessageBox.Show("Xuất file JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(this, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
