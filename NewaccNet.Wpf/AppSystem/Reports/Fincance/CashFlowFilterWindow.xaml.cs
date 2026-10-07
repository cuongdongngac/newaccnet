using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class CashFlowFilterWindow : Views.Base.BaseWindow
    {
        public CashFlowFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = ReportSettingsHelper.Load();
            var now = DateTime.Now;

            dtFromDate.DateTime = settings.CashFlowFromDate ?? new DateTime(now.Year, 1, 1);
            dtToDate.DateTime = settings.CashFlowToDate ?? new DateTime(now.Year, 12, 31);
            
            if (!string.IsNullOrEmpty(settings.CashFlowCurrentTitle)) txtCurrentColumnName.Text = settings.CashFlowCurrentTitle;
            if (!string.IsNullOrEmpty(settings.CashFlowPrevTitle)) txtPrevColumnName.Text = settings.CashFlowPrevTitle;
            if (!string.IsNullOrEmpty(settings.CashFlowPrevJsonPath)) txtPrevJsonPath.Text = settings.CashFlowPrevJsonPath;
        }

        private void BtnBrowseJson_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Chọn file dữ liệu báo cáo (JSON) của kỳ trước"
            };

            if (dlg.ShowDialog() == true)
            {
                txtPrevJsonPath.Text = dlg.FileName;
            }
        }

        private void BtnClearJson_Click(object sender, RoutedEventArgs e)
        {
            txtPrevJsonPath.Text = string.Empty;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
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
                var fromDate = dtFromDate.DateTime.Date;
                var toDate = dtToDate.DateTime.Date;
                var jsonPath = txtPrevJsonPath.Text;
                var currentTitle = txtCurrentColumnName.Text;
                var prevTitle = txtPrevColumnName.Text;
                
                var service = new NewaccNet.Reports.CashFlow.CashFlowService();
                using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                {
                    var data = service.Calculate(adapter, fromDate, toDate, jsonPath);

                    // Show the Report
                    var report = new NewaccNet.Wpf.AppSystem.Reports.Fincance.CashFlow();
                    report.DataSource = data;
                    if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                    if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                    if (report.Parameters["prmCurrentTitle"] != null) report.Parameters["prmCurrentTitle"].Value = currentTitle;
                    if (report.Parameters["prmPrevTitle"] != null) report.Parameters["prmPrevTitle"].Value = prevTitle;
                    report.RequestParameters = false;
                    report.CreateDocument(false);

                    var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow();
                    previewWindow.Owner = this.Owner ?? this;
                    previewWindow.Title = "Báo Cáo Lưu Chuyển Tiền Tệ (B03-DN)";
                    previewWindow.PreviewControl.DocumentSource = report;
                    
                    previewWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Lỗi khi tính toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportJson_Click(object sender, RoutedEventArgs e)
        {
            if (dtFromDate.DateTime == DateTime.MinValue || dtToDate.DateTime == DateTime.MinValue)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var fromDate = dtFromDate.DateTime.Date;
                var toDate = dtToDate.DateTime.Date;
                
                var service = new NewaccNet.Reports.CashFlow.CashFlowService();
                using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                {
                    // Lấy số liệu nhưng không cần jsonPath (không merge)
                    var data = service.Calculate(adapter, fromDate, toDate, null);

                    // Chỉ lưu Code và Amount theo yêu cầu
                    var exportData = data.Select(x => new { x.Code, x.Amount }).ToList();

                    var sfd = new SaveFileDialog
                    {
                        Filter = "JSON Files (*.json)|*.json",
                        Title = "Lưu file dữ liệu LCTT",
                        FileName = $"LCTT_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.json"
                    };

                    if (sfd.ShowDialog() == true)
                    {
                        var jsonStr = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                        System.IO.File.WriteAllText(sfd.FileName, jsonStr);
                        MessageBox.Show("Đã xuất JSON thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Lỗi khi xuất JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            var settings = ReportSettingsHelper.Load();
            settings.CashFlowFromDate = dtFromDate.DateTime;
            settings.CashFlowToDate = dtToDate.DateTime;
            settings.CashFlowCurrentTitle = txtCurrentColumnName.Text;
            settings.CashFlowPrevTitle = txtPrevColumnName.Text;
            settings.CashFlowPrevJsonPath = txtPrevJsonPath.Text;
            ReportSettingsHelper.Save(settings);
        }
    }
}
