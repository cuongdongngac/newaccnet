using System;
using System.Linq;
using System.Windows;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class VatReturnFilterWindow : Window
    {
        public VatReturnFilterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            dtFromDate.DateTime = new DateTime(now.Year, 1, 1);
            dtToDate.DateTime = new DateTime(now.Year, 12, 31);
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
                
                var service = new NewaccNet.Reports.VatReturn.VatReturnService();
                
                // Chạy tính toán ở Background Thread
                var data = await System.Threading.Tasks.Task.Run(() =>
                {
                    using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                    {
                        return service.Calculate(adapter, fromDate, toDate);
                    }
                });

                System.Windows.Input.Mouse.OverrideCursor = null;

                var report = new NewaccNet.Wpf.AppSystem.Reports.Fincance.VatReturn();
                report.DataSource = data;
                
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
    }
}
