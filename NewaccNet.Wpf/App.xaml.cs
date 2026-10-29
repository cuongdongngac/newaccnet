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

        // Đồng bộ Icon ứng dụng cho tất cả các cửa sổ khi mở lên
        EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent, new RoutedEventHandler((s, args) =>
        {
            if (s is Window win && win.Icon == null)
            {
                win.Icon = NewaccNet.Wpf.Views.Base.AppIconHelper.GetAppIcon();
            }
        }));

        // Tự động chuẩn hóa định dạng dd/MM/yyyy cho mọi DateEdit khi Loaded
        EventManager.RegisterClassHandler(typeof(DevExpress.Xpf.Editors.DateEdit), FrameworkElement.LoadedEvent, new RoutedEventHandler((s, args) =>
        {
            if (s is DevExpress.Xpf.Editors.DateEdit dateEdit)
            {
                NewaccNet.Wpf.Views.Base.AppDateEdit.ApplyDefaultDateFormat(dateEdit);
            }
        }));

        // Khởi tạo cấu hình tìm kiếm danh mục (Mã/Tên, tiếng Việt không dấu)
        NewaccNet.Wpf.Controls.Search.SearchSetup.Initialize();

        // 1. Tải cấu hình từ hệ thống
        ConfigManager.LoadConfig();

        // 2. Kiểm tra xem file cấu hình đã tồn tại chưa (Lần chạy đầu tiên)
        if (!ConfigManager.HasConfigFile())
        {
            // Gợi ý mặc định trỏ vào file mẫu template.accdb
            var config = ConfigManager.Current;
            config.DbType = DatabaseType.Access;
            string tPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Template", "template.accdb");
            config.AccessFilePath = File.Exists(tPath) ? tPath : Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "template.accdb");

            // Mở cửa sổ cấu hình DB
            var configWindow = new DbConfigWindow();
            configWindow.ShowDialog();
            
            // Sau khi form cấu hình đóng (hoặc tự restart sau khi lưu), tắt luồng hiện tại.
            System.Environment.Exit(0);
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
                    DbConfigService.EnsureDbConfigTable();
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
                System.Environment.Exit(0);
            }
        }
    }
}



