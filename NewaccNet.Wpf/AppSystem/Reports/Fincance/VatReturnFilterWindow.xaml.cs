using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class VatReturnFilterWindow : Views.Base.BaseWindow
    {
        public VatReturnFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = ReportSettingsHelper.Load();
            var now = DateTime.Now;

            dtFromDate.DateTime = settings.VatReturnFromDate ?? new DateTime(now.Year, 1, 1);
            dtToDate.DateTime = settings.VatReturnToDate ?? new DateTime(now.Year, 12, 31);
            
            if (!string.IsNullOrEmpty(settings.VatReturnAccJsonPath)) txtAccJsonPath.Text = settings.VatReturnAccJsonPath;
            if (!string.IsNullOrEmpty(settings.VatReturnTitle)) txtReportTitle.Text = settings.VatReturnTitle;
            if (!string.IsNullOrEmpty(settings.VatReturnOtherAmountTitle)) txtOtherAmountTitle.Text = settings.VatReturnOtherAmountTitle;
        }

        private void BtnBrowseAccJson_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Chọn file dữ liệu báo cáo Thuế (JSON) luỹ kế"
            };

            if (dlg.ShowDialog() == true)
            {
                txtAccJsonPath.Text = dlg.FileName;
            }
        }

        private void BtnClearAccJson_Click(object sender, RoutedEventArgs e)
        {
            txtAccJsonPath.Text = string.Empty;
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

            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                var fromDate = dtFromDate.DateTime.Date;
                var toDate = dtToDate.DateTime.Date;
                var accJsonPath = txtAccJsonPath.Text;
                
                var service = new NewaccNet.Reports.VatReturn.VatReturnService();
                
                // Chạy tính toán ở Background Thread
                var data = await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                    {
                        return service.Calculate(adapter, fromDate, toDate, accJsonPath);
                    }
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                var report = new NewaccNet.Wpf.AppSystem.Reports.Fincance.VatReturn();
                report.DataSource = data;
                
                if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                if (report.Parameters["prmTitle"] != null) report.Parameters["prmTitle"].Value = txtReportTitle.Text;
                if (report.Parameters["prmOtherAmount"] != null) report.Parameters["prmOtherAmount"].Value = txtOtherAmountTitle.Text;
                
                report.RequestParameters = false;
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow();
                previewWindow.Owner = this.Owner ?? this;
                previewWindow.Title = "Báo Cáo Thuế GTGT Khấu Trừ";
                previewWindow.PreviewControl.DocumentSource = report;
                
                previewWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(this, "Lỗi khi tính toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var accJsonPath = txtAccJsonPath.Text;
                
                var service = new NewaccNet.Reports.VatReturn.VatReturnService();
                
                var data = await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                    {
                        return service.Calculate(adapter, fromDate, toDate, accJsonPath);
                    }
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                // Xuất JSON chỉ mang theo Code và Amount để sau này nhồi vào OtherAmount của kỳ báo cáo khác (so sánh)
                var exportData = data.Select(x => new { x.Code, x.Amount }).ToList();

                var sfd = new SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Lưu file dữ liệu Thuế Khấu trừ (Luỹ kế)",
                    FileName = $"ThueKhauTru_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.json"
                };

                if (sfd.ShowDialog() == true)
                {
                    var jsonStr = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(sfd.FileName, jsonStr);
                    MessageBox.Show("Đã xuất JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(this, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            var settings = ReportSettingsHelper.Load();
            settings.VatReturnFromDate = dtFromDate.DateTime;
            settings.VatReturnToDate = dtToDate.DateTime;
            settings.VatReturnAccJsonPath = txtAccJsonPath.Text;
            settings.VatReturnTitle = txtReportTitle.Text;
            settings.VatReturnOtherAmountTitle = txtOtherAmountTitle.Text;
            ReportSettingsHelper.Save(settings);
        }
    }
}
