using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class ObligationTaxFilterWindow : Views.Base.BaseWindow
    {
        public ObligationTaxFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = ReportSettingsHelper.Load();
            var now = DateTime.Now;

            dtFromDate.DateTime = settings.ObligationTaxFromDate ?? new DateTime(now.Year, 1, 1);
            dtToDate.DateTime = settings.ObligationTaxToDate ?? new DateTime(now.Year, 12, 31);
            
            if (!string.IsNullOrEmpty(settings.ObligationTaxPrevJsonPath)) txtPrevJsonPath.Text = settings.ObligationTaxPrevJsonPath;
            if (!string.IsNullOrEmpty(settings.ObligationTaxAccJsonPath)) txtAccJsonPath.Text = settings.ObligationTaxAccJsonPath;
        }

        private void BtnBrowsePrevJson_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Chọn file dữ liệu báo cáo Thuế (JSON) của kỳ trước"
            };

            if (dlg.ShowDialog() == true)
            {
                txtPrevJsonPath.Text = dlg.FileName;
            }
        }

        private void BtnClearPrevJson_Click(object sender, RoutedEventArgs e)
        {
            txtPrevJsonPath.Text = string.Empty;
        }

        private void BtnBrowseAccJson_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Chọn file dữ liệu báo cáo Thuế (JSON) lũy kế"
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
                var prevJsonPath = txtPrevJsonPath.Text;
                var accJsonPath = txtAccJsonPath.Text;
                
                var service = new NewaccNet.Reports.ObligationTax.ObligationTaxService();
                
                // Chạy tính toán ở Background Thread để không treo UI
                var data = await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                    {
                        return service.Calculate(adapter, fromDate, toDate, prevJsonPath, accJsonPath);
                    }
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                // Show the Report
                var report = new NewaccNet.Wpf.AppSystem.Reports.Fincance.ObligationTax();
                report.DataSource = data;
                
                if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                
                report.RequestParameters = false;
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow();
                previewWindow.Owner = this.Owner ?? this;
                previewWindow.Title = "Báo Cáo Nghĩa Vụ Thuế";
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
                
                var service = new NewaccNet.Reports.ObligationTax.ObligationTaxService();
                
                var data = await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                    {
                        // Lấy số liệu nhưng không cần jsonPath (không merge)
                        return service.Calculate(adapter, fromDate, toDate, null, null);
                    }
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                var exportData = data.Select(x => new { x.Code, x.EndTax }).ToList();

                var sfd = new SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json",
                    Title = "Lưu file dữ liệu Nghĩa Vụ Thuế",
                    FileName = $"NghiaVuThue_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.json"
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
            settings.ObligationTaxFromDate = dtFromDate.DateTime;
            settings.ObligationTaxToDate = dtToDate.DateTime;
            settings.ObligationTaxPrevJsonPath = txtPrevJsonPath.Text;
            settings.ObligationTaxAccJsonPath = txtAccJsonPath.Text;
            ReportSettingsHelper.Save(settings);
        }
    }
}
