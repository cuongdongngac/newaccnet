using System.Windows;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf.AppSystem
{
    public partial class DbConfigWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public DbConfigWindow()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            var config = ConfigManager.Current;
            cboDbType.SelectedIndex = config.DbType == DatabaseType.Access     ? 0
                                    : config.DbType == DatabaseType.SqlServer  ? 1
                                    : 2; // PostgreSQL

            // Access
            txtAccessPath.Text = config.AccessFilePath;

            // SQL Server
            txtServer.Text           = config.SqlServerName;
            txtDatabase.Text         = config.SqlDatabaseName;
            chkIntegrated.IsChecked  = config.SqlIntegratedSecurity;
            txtUser.Text             = config.SqlUsername;
            txtPassword.EditValue    = config.SqlPassword;

            // PostgreSQL
            txtPgHost.Text          = config.PgHost;
            txtPgPort.Text          = config.PgPort.ToString();
            txtPgDatabase.Text      = config.PgDatabase;
            txtPgUsername.Text      = config.PgUsername;
            txtPgPassword.EditValue = config.PgPassword;

            UpdateVisibility();
        }

        private void CboDbType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            UpdateVisibility();
        }

        private void ChkIntegrated_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            if (grpAccess == null || grpSqlServer == null || grpPostgreSql == null) return;

            int idx = cboDbType.SelectedIndex;

            grpAccess.Visibility     = idx == 0 ? Visibility.Visible : Visibility.Collapsed;
            grpSqlServer.Visibility  = idx == 1 ? Visibility.Visible : Visibility.Collapsed;
            grpPostgreSql.Visibility = idx == 2 ? Visibility.Visible : Visibility.Collapsed;

            if (idx == 1)
            {
                bool useIntegrated    = chkIntegrated.IsChecked ?? false;
                txtUser.IsEnabled     = !useIntegrated;
                txtPassword.IsEnabled = !useIntegrated;
            }

            UpdatePreview();
        }

        /// <summary>Được gọi khi bất kỳ field nào thay đổi → cập nhật live preview connection string.</summary>
        private void AnyField_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (txtConnPreview == null) return;

            int    idx     = cboDbType.SelectedIndex;
            string preview = string.Empty;

            if (idx == 0) // Access
            {
                string path = txtAccessPath?.Text ?? "";
                preview = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path}";
            }
            else if (idx == 1) // SQL Server
            {
                string server = txtServer?.Text    ?? "";
                string db     = txtDatabase?.Text  ?? "";
                bool integrated = chkIntegrated?.IsChecked ?? false;

                if (integrated)
                    preview = $"data source={server};initial catalog={db};Integrated Security=SSPI;TrustServerCertificate=True;";
                else
                {
                    string user = txtUser?.Text ?? "";
                    string pass = txtPassword?.EditValue as string ?? "";
                    preview = $"data source={server};initial catalog={db};User ID={user};Password={pass};TrustServerCertificate=True;";
                }
            }
            else if (idx == 2) // PostgreSQL
            {
                string host = txtPgHost?.Text      ?? "";
                string port = txtPgPort?.Text      ?? "5432";
                string db   = txtPgDatabase?.Text  ?? "";
                string user = txtPgUsername?.Text  ?? "";
                string pass = txtPgPassword?.EditValue as string ?? "";
                preview = $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
            }

            txtConnPreview.Text = preview;
        }

        private void BtnBrowseAccess_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Access Database|*.accdb;*.mdb|All Files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                txtAccessPath.Text = dlg.FileName;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigManager.Current;

            int idx = cboDbType.SelectedIndex;
            config.DbType = idx == 0 ? DatabaseType.Access
                          : idx == 1 ? DatabaseType.SqlServer
                          : DatabaseType.PostgreSql;

            // Access
            config.AccessFilePath = txtAccessPath.Text;

            // SQL Server
            config.SqlServerName         = txtServer.Text;
            config.SqlDatabaseName       = txtDatabase.Text;
            config.SqlIntegratedSecurity = chkIntegrated.IsChecked ?? false;
            config.SqlUsername           = txtUser.Text;
            config.SqlPassword           = txtPassword.EditValue as string ?? "";

            // PostgreSQL
            config.PgHost     = txtPgHost.Text;
            config.PgPort     = int.TryParse(txtPgPort.Text, out int port) ? port : 5432;
            config.PgDatabase = txtPgDatabase.Text;
            config.PgUsername = txtPgUsername.Text;
            config.PgPassword = txtPgPassword.EditValue as string ?? "";

            ConfigManager.BuildConnectionString();
            ConfigManager.SaveConfig();

            System.Windows.MessageBox.Show(
                "Đã lưu cấu hình kết nối CSDL thành công. Hệ thống sẽ tự động khởi động lại để áp dụng thay đổi.",
                "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            
            // Tự khởi động lại ứng dụng
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            System.Windows.Application.Current.Shutdown();
        }
    }
}

