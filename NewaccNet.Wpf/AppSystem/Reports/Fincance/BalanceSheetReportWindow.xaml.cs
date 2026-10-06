using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NewaccNet.Wpf.Views.Base;
using DataAccess.FactoryClasses;
using NewaccNet.Reports.BalanceSheet;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class BalanceSheetReportWindow : BaseWindow
    {
        public BalanceSheetReportWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Auto load on open
            BtnGenerate_Click(null, null);
        }

        private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                btnGenerate.IsEnabled = false;
                btnGenerate.Content = "Đang xử lý...";

                var dtos = await System.Threading.Tasks.Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var service = new BalanceSheetService();
                    return service.GenerateReport(adapter);
                });

                if (dgData != null)
                {
                    // Lọc những dòng có dữ liệu hoặc để tất cả
                    dgData.ItemsSource = dtos.Select(x => new 
                    {
                        x.SectionName,
                        x.CategoryName,
                        ChỉTiêu = x.ItemsName,
                        TàiKhoản = x.AccountId,
                        DưĐầu = x.BeginAmount,
                        DưCuối = x.EndAmount
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnGenerate.IsEnabled = true;
                btnGenerate.Content = "Lấy Dữ Liệu DTO";
                Mouse.OverrideCursor = null;
            }
        }

        private async void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                btnPrint.IsEnabled = false;
                btnPrint.Content = "Đang xử lý...";

                var dtos = await System.Threading.Tasks.Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var service = new BalanceSheetService();
                    return service.GenerateReport(adapter);
                });

                var report = new BalanceSheet();
                report.DataSource = dtos;
                report.CreateDocument(false);

                var previewWindow = new DocumentPreviewWindow();
                previewWindow.Owner = this.Owner ?? this;
                previewWindow.Title = "Bảng Cân Đối Kế Toán";
                previewWindow.PreviewControl.DocumentSource = report;
                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnPrint.IsEnabled = true;
                btnPrint.Content = "Mở Báo Cáo";
                Mouse.OverrideCursor = null;
            }
        }
    }
}
