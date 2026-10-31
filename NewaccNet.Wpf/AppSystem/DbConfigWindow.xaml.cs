using System;
using System.Windows;
using DevExpress.Xpf.Core;
using NewaccNet.Wpf.AppSystem.Helpers;

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

            // Thông tin công ty (Lấy từ DbConfig trong CSDL)
            string companyName = DbConfigService.Current.CompanyName;
            txtCompanyName.Text = companyName ?? "";

            int currentYear = DbConfigService.Current.AccountingYear;
            if (currentYear >= 1900 && currentYear <= 2100)
            {
                spnAccountingYear.Value = currentYear;
            }
            else
            {
                spnAccountingYear.Value = DateTime.Today.Year;
            }

            // Kho lưu nháp Staging
            cboStagingStorageMode.SelectedIndex = config.StagingStorageMode == NewaccNet.Core.Staging.StagingStorageMode.Database ? 1 : 0;

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
                preview = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={path};Persist Security Info=False;";
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
            // Access
            if (idx == 0)
            {
                if (string.IsNullOrWhiteSpace(txtAccessPath.Text) || !System.IO.File.Exists(txtAccessPath.Text.Trim()))
                {
                    System.Windows.MessageBox.Show(
                        $"Tệp cơ sở dữ liệu Access không tồn tại:\n{txtAccessPath.Text}\n\nVui lòng kiểm tra hoặc chọn lại đúng đường dẫn tệp .accdb!",
                        "Cảnh báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }
            }
            config.AccessFilePath = txtAccessPath.Text.Trim();

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

            // Kho lưu nháp Staging
            config.StagingStorageMode = cboStagingStorageMode.SelectedIndex == 1
                ? NewaccNet.Core.Staging.StagingStorageMode.Database
                : NewaccNet.Core.Staging.StagingStorageMode.JsonFile;

            // 1. Áp dụng cấu hình kết nối CSDL trước
            ConfigManager.BuildConnectionString();
            ConfigManager.SaveConfig();
            DbConfigService.ClearCache();

            // 2. Lưu thông tin công ty vào DbConfig trong CSDL vừa kết nối
            string newCompany = txtCompanyName.Text?.Trim() ?? "";
            try
            {
                var dbCfg = DbConfigService.LoadDbConfig();
                dbCfg.CompanyName = newCompany;
                if (int.TryParse(spnAccountingYear.Value.ToString(), out int accYear))
                {
                    dbCfg.AccountingYear = accYear;
                }
                DbConfigService.SaveDbConfig(dbCfg);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SaveDbConfig error in DbConfigWindow: " + ex.Message);
            }

            System.Windows.MessageBox.Show(
                "Đã lưu cấu hình kết nối CSDL và thông tin công ty thành công. Hệ thống sẽ tự động khởi động lại để áp dụng thay đổi.",
                "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            
            // Tự khởi động lại ứng dụng mà không hiển thị hộp thoại hỏi thoát
            MainWindow.SuppressExitPrompt = true;
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            System.Windows.Application.Current.Shutdown();
        }

        private async void BtnMigrateJsonToDb_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có muốn nạp toàn bộ chứng từ nháp từ tệp JSON trong máy vào bảng StagingDraft trong CSDL đang kết nối?\n\n" +
                "Các chứng từ đã có trong CSDL sẽ được giữ nguyên (không bị ghi đè).",
                "Đồng bộ JSON ➔ CSDL", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            btnMigrateJsonToDb.IsEnabled = false;
            try
            {
                var migration = new NewaccNet.Core.Staging.DraftMigrationService();
                var result = await migration.MigrateJsonToDbAsync(overwrite: false);
                MessageBox.Show(
                    $"Đã hoàn tất đồng bộ JSON ➔ CSDL:\n" +
                    $"- Tổng số tìm thấy: {result.TotalFound}\n" +
                    $"- Đã nạp thành công: {result.SuccessCount}\n" +
                    $"- Đã bỏ qua (đã có sẵn): {result.SkippedCount}\n" +
                    $"- Lỗi: {result.ErrorCount}",
                    "Kết quả đồng bộ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đồng bộ JSON ➔ CSDL: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnMigrateJsonToDb.IsEnabled = true;
            }
        }

        private async void BtnMigrateDbToJson_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có muốn xuất toàn bộ chứng từ nháp từ bảng StagingDraft trong CSDL ra các tệp JSON trong thư mục AppData/NewaccNet/Staging?\n\n" +
                "Các tệp đã tồn tại sẽ được giữ nguyên.",
                "Đồng bộ CSDL ➔ JSON", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            btnMigrateDbToJson.IsEnabled = false;
            try
            {
                var migration = new NewaccNet.Core.Staging.DraftMigrationService();
                var result = await migration.MigrateDbToJsonAsync(overwrite: false);
                MessageBox.Show(
                    $"Đã hoàn tất xuất CSDL ➔ JSON:\n" +
                    $"- Tổng số tìm thấy: {result.TotalFound}\n" +
                    $"- Đã xuất thành công: {result.SuccessCount}\n" +
                    $"- Đã bỏ qua (đã có sẵn): {result.SkippedCount}\n" +
                    $"- Lỗi: {result.ErrorCount}",
                    "Kết quả xuất dữ liệu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất CSDL ➔ JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnMigrateDbToJson.IsEnabled = true;
            }
        }
    }
}
