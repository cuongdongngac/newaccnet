using System;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf.AppSystem.License
{
    public partial class LicenseWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public bool IsActivationSuccess { get; private set; } = false;

        public LicenseWindow()
        {
            InitializeComponent();
            this.Loaded += LicenseWindow_Loaded;
        }

        private void LicenseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Hiển thị mã máy tính
            txtMachineCode.Text = HardwareHelper.GetMachineCode();
            UpdateStatusDisplay();
        }

        private void UpdateStatusDisplay()
        {
            txtCurrentStatus.Text = LicenseManager.LicenseStatusText;
            if (LicenseManager.IsLicensed)
            {
                txtCurrentStatus.Foreground = new SolidColorBrush(Color.FromRgb(22, 101, 52)); // Xanh lá
            }
            else
            {
                txtCurrentStatus.Foreground = new SolidColorBrush(Color.FromRgb(217, 119, 6)); // Cam cảnh báo
            }
        }

        private void BtnCopyMachineCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(txtMachineCode.Text);
                ThemedMessageBox.Show(
                    "Thông báo",
                    $"Đã sao chép mã máy: {txtMachineCode.Text}\n\nHãy gửi mã này qua Zalo: 0903210335 (Phạm Việt Cường) để nhận mã kích hoạt bản quyền.",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Lỗi", "Không thể sao chép: " + ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPasteKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    txtLicenseKey.Text = Clipboard.GetText().Trim();
                }
            }
            catch { }
        }

        private void BtnActivate_Click(object sender, RoutedEventArgs e)
        {
            string key = txtLicenseKey.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(key))
            {
                ThemedMessageBox.Show("Cảnh báo", "Vui lòng nhập mã kích hoạt bản quyền do tác giả cấp!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (LicenseManager.Activate(key, out string errorMessage))
            {
                IsActivationSuccess = true;
                UpdateStatusDisplay();
                ThemedMessageBox.Show(
                    "Chúc mừng!",
                    "KÍCH HOẠT BẢN QUYỀN THÀNH CÔNG!\n\nCảm ơn bạn đã tin tưởng và sử dụng phần mềm Kế toán NEWACCNET.",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                ThemedMessageBox.Show("Lỗi kích hoạt", errorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
