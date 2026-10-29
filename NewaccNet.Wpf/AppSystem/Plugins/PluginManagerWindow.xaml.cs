using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Plugins
{
    public class PluginItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string DllFileName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }

        public string StatusDisplay => IsEnabled ? "🟢 Đang kích hoạt" : "⚪ Đã tắt";
    }

    public partial class PluginManagerWindow : BaseWindow
    {
        private readonly ObservableCollection<PluginItemViewModel> _displayedPlugins = new();

        public PluginManagerWindow()
        {
            InitializeComponent();
            gridPlugins.ItemsSource = _displayedPlugins;
            LoadData();
        }

        public override void LoadData()
        {
            _displayedPlugins.Clear();
            var plugins = PluginLoader.Instance.Plugins;

            foreach (var p in plugins)
            {
                _displayedPlugins.Add(new PluginItemViewModel
                {
                    Id = p.Record.Id,
                    Name = p.Record.Name,
                    Version = p.Record.Version,
                    Description = p.Record.Description,
                    Author = p.Record.Author,
                    DllFileName = p.Record.DllFileName,
                    IsEnabled = p.Record.IsEnabled
                });
            }

            int activeCount = _displayedPlugins.Count(x => x.IsEnabled);
            lblPluginStats.Text = $"Tổng số: {_displayedPlugins.Count} plugin (Đang kích hoạt: {activeCount})";
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            PluginLoader.Instance.LoadAllPlugins();
            LoadData();
            MessageBox.Show("Đã quét và làm mới danh sách Plugin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dir = PluginLoader.Instance.PluginsDirectory;
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                Process.Start(new ProcessStartInfo("explorer.exe", dir) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở thư mục: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnInstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp DLL của Plugin để cài đặt",
                Filter = "Plugin Libraries (*.dll)|*.dll|All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                bool ok = PluginLoader.Instance.InstallPlugin(dlg.FileName);
                if (ok)
                {
                    LoadData();
                    MessageBox.Show($"Cài đặt Plugin thành công từ:\n{Path.GetFileName(dlg.FileName)}\n\nPlugin đã được sao chép vào thư mục Plugins của người dùng.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không thể cài đặt Plugin. Vui lòng kiểm tra xem tệp có hợp lệ hoặc đang bị khóa không.", "Lỗi cài đặt", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnToggleState_Click(object sender, RoutedEventArgs e)
        {
            var selected = gridPlugins.SelectedItem as PluginItemViewModel;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn một Plugin trong danh sách để bật/tắt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool newState = !selected.IsEnabled;
            PluginLoader.Instance.SetPluginState(selected.Id, newState);
            LoadData();

            string statusMsg = newState ? "kích hoạt" : "vô hiệu hóa";
            MessageBox.Show($"Đã {statusMsg} Plugin '{selected.Name}' thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnUninstallPlugin_Click(object sender, RoutedEventArgs e)
        {
            var selected = gridPlugins.SelectedItem as PluginItemViewModel;
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn một Plugin trong danh sách để gỡ bỏ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn gỡ bỏ hoàn toàn Plugin '{selected.Name}' ({selected.DllFileName}) khỏi hệ thống?\n\nTệp thư viện DLL và các tệp liên quan trong thư mục Plugins sẽ được xóa sạch khỏi ổ đĩa.",
                "Xác nhận gỡ bỏ", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            bool ok = PluginLoader.Instance.UninstallPlugin(selected.Id);
            if (ok)
            {
                LoadData();
                MessageBox.Show($"Đã gỡ bỏ và xóa sạch tệp DLL của Plugin '{selected.Name}' khỏi hệ thống thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Không thể gỡ bỏ Plugin. Tệp DLL có thể đang được tiến trình sử dụng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
