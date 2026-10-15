using System;
using System.IO;
using System.Windows;
using System.Linq;
using DevExpress.Xpf.Core;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using DataAccess.Linq;

namespace NewaccNet.Wpf.AppSystem
{
    public partial class LoginWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        /// <summary>Thông tin user sau khi đăng nhập thành công</summary>
        public string LoggedUsername { get; private set; } = "";
        public string LoggedFullName { get; private set; } = "";
        public int LoggedRoleMask { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            this.Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDbConfigLinkVisibility();
        }

        /// <summary>
        /// Chỉ hiển thị link "Cấu hình Cơ sở dữ liệu" khi ở thư mục cài đặt đã sinh ra tệp appsettings.json
        /// nhưng kết nối tới CSDL bị lỗi (do tệp DB bị hỏng, bị di chuyển hoặc sai thông số kết nối).
        /// </summary>
        private void UpdateDbConfigLinkVisibility()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                layoutItemDbConfig.Visibility = Visibility.Collapsed;
                return;
            }

            bool isConnectionValid = TestConfiguredDatabaseConnection();
            layoutItemDbConfig.Visibility = isConnectionValid ? Visibility.Collapsed : Visibility.Visible;
        }

        private static bool TestConfiguredDatabaseConnection()
        {
            try
            {
                ConfigManager.LoadConfig();
                var cfg = ConfigManager.Current;
                if (cfg == null || string.IsNullOrWhiteSpace(cfg.ConnectionString))
                    return false;

                if (cfg.DbType == DatabaseType.Access)
                {
                    if (string.IsNullOrWhiteSpace(cfg.AccessFilePath) || !File.Exists(cfg.AccessFilePath))
                        return false;

                    using (var conn = new System.Data.OleDb.OleDbConnection(cfg.ConnectionString))
                    {
                        conn.Open();
                        return true;
                    }
                }
                else if (cfg.DbType == DatabaseType.SqlServer)
                {
                    using (var conn = new Microsoft.Data.SqlClient.SqlConnection(cfg.ConnectionString))
                    {
                        conn.Open();
                        return true;
                    }
                }
                else if (cfg.DbType == DatabaseType.PostgreSql)
                {
                    using (var conn = new Npgsql.NpgsqlConnection(cfg.ConnectionString))
                    {
                        conn.Open();
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text?.Trim() ?? "";
            string pass = "";
            if (txtPassword.EditValue != null) 
                pass = txtPassword.EditValue.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var metaData = new LinqMetaData(adapter);
                    // Tìm user theo username
                    var systemUser = metaData.SystemUser.FirstOrDefault(u => u.Username == user);

                    // So sánh mật khẩu đã mã hóa
                    if (systemUser != null && SecurityHelper.Verify(pass, systemUser.Password))
                    {
                        // Lưu thông tin user để truyền sang MainWindow
                        LoggedUsername = systemUser.Username;
                        LoggedFullName = systemUser.FullName ?? "";
                        LoggedRoleMask = systemUser.RoleMask;
                        
                        // Gán RoleMask toàn cục cho Attached Property phân quyền
                        AuthHelper.CurrentUserMask = systemUser.RoleMask;

                        this.DialogResult = true;
                        // this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UpdateDbConfigLinkVisibility();
                MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}\n\nVui lòng kiểm tra lại cấu hình DB.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LnkConfig_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new DbConfigWindow();
            cfg.ShowDialog();
            UpdateDbConfigLinkVisibility();
        }
    }
}

