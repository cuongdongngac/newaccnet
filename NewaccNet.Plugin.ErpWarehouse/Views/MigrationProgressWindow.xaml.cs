using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;
using NewaccNet.Core.Posting;

namespace NewaccNet.Plugin.ErpWarehouse.Views
{
    public partial class MigrationProgressWindow : ThemedWindow
    {
        private readonly CancellationTokenSource _cts = new();
        private bool _isCompleted = false;

        public (int totalProcessed, int successCount, List<string> errors) Result { get; private set; }

        public MigrationProgressWindow()
        {
            InitializeComponent();
            this.Loaded += MigrationProgressWindow_Loaded;
            this.Closing += MigrationProgressWindow_Closing;
        }

        private async void MigrationProgressWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var progress = new Progress<MigrationProgressInfo>(ReportProgress);

            try
            {
                Result = await Task.Run(() =>
                {
                    return WarehousePostingGateway.MigrateLegacyEntryGraphs(progress, _cts.Token);
                }, _cts.Token);

                _isCompleted = true;
                progressBar.Value = 100;
                lblTitle.Text = "Đã hoàn thành chuyển đổi đồ thị kho!";
                lblSubTitle.Text = $"Xử lý {Result.successCount}/{Result.totalProcessed} phiếu thành công.";
                lblMessage.Text = Result.errors.Count == 0 
                    ? "Tất cả các phiếu xuất nhập đã được liên kết đồ thị JSON 100%!" 
                    : $"Hoàn tất với {Result.errors.Count} cảnh báo/lỗi.";

                lblStats.Text = "Tiến độ: 100% hoàn tất";
                lblResultStats.Text = $"Thành công: {Result.successCount} | Lỗi: {Result.errors.Count}";

                btnCancel.Visibility = Visibility.Collapsed;
                btnClose.Visibility = Visibility.Visible;
            }
            catch (OperationCanceledException)
            {
                lblTitle.Text = "Đã dừng chuyển đổi theo yêu cầu";
                lblMessage.Text = "Tiến trình đã được dừng an toàn. Các phiếu đã xử lý được lưu giữ nguyên vẹn.";
                btnCancel.Visibility = Visibility.Collapsed;
                btnClose.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                lblTitle.Text = "Xảy ra lỗi trong quá trình chuyển đổi";
                lblMessage.Text = ex.Message;
                btnCancel.Visibility = Visibility.Collapsed;
                btnClose.Visibility = Visibility.Visible;
            }
        }

        private void ReportProgress(MigrationProgressInfo info)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = info.Percent;
                lblMessage.Text = info.CurrentMessage;
                lblStats.Text = $"Tiến độ: {info.Percent}% ({info.Current:N0} / {info.Total:N0} phiếu)";
                lblResultStats.Text = $"Thành công: {info.Success:N0} | Lỗi: {info.Failed:N0}";
            });
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            btnCancel.Content = "Đang dừng...";
            _cts.Cancel();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void MigrationProgressWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isCompleted && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
    }
}
