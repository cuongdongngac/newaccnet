using System.IO;
using System.Windows;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName = "Office2019Colorful";

        // 1. Tải cấu hình từ hệ thống
        ConfigManager.LoadConfig();

        // 2. Kiểm tra xem file cấu hình đã tồn tại chưa (Lần chạy đầu tiên)
        if (!File.Exists("appsettings.json"))
        {
            // Gợi ý mặc định trỏ vào file mẫu template.accdb
            var config = ConfigManager.Current;
            config.DbType = DatabaseType.Access;
            config.AccessFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "template.accdb");

            // Mở cửa sổ cấu hình DB
            var configWindow = new DbConfigWindow();
            configWindow.ShowDialog();
            
            // Sau khi form cấu hình đóng (hoặc tự restart sau khi lưu), tắt luồng hiện tại.
            Application.Current.Shutdown();
            return;
        }
        else
        {
            // 3. Đã có cấu hình: Mở cửa sổ Đăng nhập
            var loginWindow = new LoginWindow();
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                try 
                {
                    MessageBox.Show("Chuẩn bị mở MainWindow...", "Debug");
                    var mainWindow = new MainWindow(
                        loginWindow.LoggedUsername,
                        loginWindow.LoggedFullName,
                        loginWindow.LoggedRoleMask);
                    
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.ShowDialog();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi khi mở MainWindow:\n" + ex.ToString(), "Debug Error");
                }
            }
            else
            {
                // Người dùng bấm Hủy/Thoát hoặc đóng form Login
                Application.Current.Shutdown();
            }
        }
    }
}

