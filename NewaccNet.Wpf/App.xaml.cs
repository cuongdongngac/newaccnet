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
        NewaccNet.Core.UI.AccountLookUpEditBase.DetailWindowOpener = (entry, owner, showMsg) => NewaccNet.Wpf.AppSystem.Voucher.AccountLookUpEdit.OpenDetailWindow(entry, owner, false, showMsg);
        ConfigManager.LoadConfig();

        // 2. Xử lý kết nối CSDL:
        // - Lần đầu chạy (chưa có tệp cấu hình): tự động kết nối vào app\sampledb\mydata.accdb
        // - Đã có cấu hình nhưng bị sai / không kết nối được: tự động fallback về mydata.accdb
        if (!ConfigManager.HasConfigFile())
        {
            ConfigManager.ConfigureSampleDatabase();
        }
        else
        {
            if (!NewaccNet.Core.Data.AppDataAccessAdapter.TestConnection(out string testError))
            {
                System.Diagnostics.Debug.WriteLine($"[Startup] CSDL cấu hình không hợp lệ ({testError}). Tự động fallback về sampledb\\mydata.accdb.");
                ConfigManager.ConfigureSampleDatabase();
            }
        }

        // 3. Mở cửa sổ Đăng nhập
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



