using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class BusinessResultFilterWindow : Views.Base.BaseWindow
    {
        public BusinessResultFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = ReportSettingsHelper.Load();
            var today = DateTime.Today;

            dtFromDate.DateTime = settings.KQKDFromDate ?? new DateTime(today.Year, 1, 1);
            dtToDate.DateTime = settings.KQKDToDate ?? today;
            
            if (!string.IsNullOrEmpty(settings.KQKDCurrentTitle)) txtCurrentColumnName.Text = settings.KQKDCurrentTitle;
            if (!string.IsNullOrEmpty(settings.KQKDPrevTitle)) txtPrevColumnName.Text = settings.KQKDPrevTitle;
            if (!string.IsNullOrEmpty(settings.KQKDPrevJsonPath)) txtPrevJsonPath.Text = settings.KQKDPrevJsonPath;
        }

        private void BtnBrowseJson_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Chọn file JSON dữ liệu kỳ trước/lũy kế"
            };

            if (ofd.ShowDialog() == true)
            {
                txtPrevJsonPath.Text = ofd.FileName;
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
                var currentTitle = txtCurrentColumnName.Text.Replace("\\n", Environment.NewLine);
                var prevTitle = txtPrevColumnName.Text.Replace("\\n", Environment.NewLine);
                
                var service = new NewaccNet.Reports.BusinessResult.BusinessResultService();
                using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                {
                    var data = service.Calculate(adapter, fromDate, toDate, jsonPath);

                    // Show the Report
                    var report = new NewaccNet.Wpf.AppSystem.Reports.Fincance.BusinessResult();
                    report.DataSource = data;
                    if (report.Parameters["prmFromDate"] != null) report.Parameters["prmFromDate"].Value = fromDate;
                    if (report.Parameters["prmToDate"] != null) report.Parameters["prmToDate"].Value = toDate;
                    if (report.Parameters["prmCurrentTitle"] != null) report.Parameters["prmCurrentTitle"].Value = currentTitle;
                    if (report.Parameters["prmPrevTitle"] != null) report.Parameters["prmPrevTitle"].Value = prevTitle;
                    report.RequestParameters = false;
                    report.CreateDocument(false);

                    var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow();
                    previewWindow.Owner = this.Owner ?? this;
                    previewWindow.Title = "Báo Cáo Kết Quả Hoạt Động Kinh Doanh (B02-DN)";
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
                
                var service = new NewaccNet.Reports.BusinessResult.BusinessResultService();
                using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                {
                    // Lấy số liệu nhưng không cần jsonPath (không merge)
                    var data = service.Calculate(adapter, fromDate, toDate, null);

                    // Chỉ lưu Code và Amount theo yêu cầu
                    var exportData = data.Select(x => new { x.Code, x.Amount }).ToList();

                    var sfd = new SaveFileDialog
                    {
                        Filter = "JSON Files (*.json)|*.json",
                        Title = "Lưu file dữ liệu KQKD",
                        FileName = $"KQKD_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.json"
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
            settings.KQKDFromDate = dtFromDate.DateTime;
            settings.KQKDToDate = dtToDate.DateTime;
            settings.KQKDCurrentTitle = txtCurrentColumnName.Text;
            settings.KQKDPrevTitle = txtPrevColumnName.Text;
            settings.KQKDPrevJsonPath = txtPrevJsonPath.Text;
            ReportSettingsHelper.Save(settings);
        }
    }
}
