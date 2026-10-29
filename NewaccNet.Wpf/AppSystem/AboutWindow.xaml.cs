using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf.AppSystem
{
    public partial class AboutWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public const string DefaultHelpUrl = "https://newacc.learning.io.vn";

        public AboutWindow()
        {
            InitializeComponent();
            LoadVersionInfo();
        }

        private void LoadVersionInfo()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var ver = asm.GetName().Version;
                if (ver != null)
                {
                    txtVersion.Text = $"Phiên bản: {ver.Major}.{ver.Minor}.{ver.Build} (Build: {ver.Revision})";
                }
            }
            catch
            {
                txtVersion.Text = "Phiên bản: 1.0.0 (2026)";
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                OpenUrl(e.Uri.AbsoluteUri);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Thông báo", $"Không thể mở liên kết: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCopyPhone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText("0903210335");
                ThemedMessageBox.Show("Thông báo", "Đã sao chép số điện thoại 0903210335 vào bộ nhớ tạm (Clipboard).", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Lỗi", $"Không thể sao chép: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOnlineHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenUrl(DefaultHelpUrl);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Thông báo", $"Không thể mở trang trợ giúp: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void OpenUrl(string url)
        {
            NewaccNet.Wpf.Views.Base.HelpHelper.OpenUrl(url);
        }
    }
}
