using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace NewaccNet.Wpf.AppSystem.BalanceTransfer
{
    public partial class TransferBalanceWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        private readonly IBalanceTransferService _transferService;

        public TransferBalanceWindow()
        {
            InitializeComponent();
            _transferService = new BalanceTransferService();

            InitSourceAndDestination();
        }

        private void InitSourceAndDestination()
        {
            var src = ConfigManager.Current;

            // 1. CSDL Nguồn
            txtSourceDbType.Text = src.DbType == DatabaseType.Access ? "Microsoft Access (.accdb)"
                                : src.DbType == DatabaseType.SqlServer ? "Microsoft SQL Server"
                                : "PostgreSQL";

            int currentYear = DateTime.Now.Year;
            spnSourceYear.Value = currentYear;

            if (src.DbType == DatabaseType.Access)
                txtSourceDbInfo.Text = src.AccessFilePath;
            else if (src.DbType == DatabaseType.SqlServer)
                txtSourceDbInfo.Text = $"Server: {src.SqlServerName} | Database: {src.SqlDatabaseName}";
            else
                txtSourceDbInfo.Text = $"Host: {src.PgHost}:{src.PgPort} | Database: {src.PgDatabase}";

            // 2. CSDL Đích (Mặc định chọn cùng loại hệ quản trị CSDL với nguồn)
            cboDestDbType.SelectedIndex = src.DbType == DatabaseType.Access ? 0
                                        : src.DbType == DatabaseType.SqlServer ? 1
                                        : 2;

            // Năm mới & Ngày đầu kỳ
            int nextYear = currentYear + 1;
            spnDestYear.Value = nextYear;
            dtOpeningDate.DateTime = new DateTime(nextYear, 1, 1);

            // Khởi tạo các ô CSDL đích
            if (src.DbType == DatabaseType.Access)
            {
                string dir = !string.IsNullOrEmpty(src.AccessFilePath) ? Path.GetDirectoryName(src.AccessFilePath) : "";
                txtDestAccessPath.Text = !string.IsNullOrEmpty(dir) ? Path.Combine(dir, $"Data_{nextYear}.accdb") : "";
            }
            else if (src.DbType == DatabaseType.SqlServer)
            {
                txtDestServer.Text = src.SqlServerName;
                txtDestDatabase.Text = $"{src.SqlDatabaseName}_{nextYear}";
                chkDestIntegrated.IsChecked = src.SqlIntegratedSecurity;
                txtDestUser.Text = src.SqlUsername;
                txtDestPassword.EditValue = src.SqlPassword;
            }
            else
            {
                txtDestPgHost.Text = src.PgHost;
                txtDestPgPort.Text = src.PgPort.ToString();
                txtDestPgDatabase.Text = $"{src.PgDatabase}_{nextYear}";
                txtDestPgUsername.Text = src.PgUsername;
                txtDestPgPassword.EditValue = src.PgPassword;
            }

            UpdateDestVisibility();
        }

        private void CboDestDbType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            UpdateDestVisibility();
        }

        private void ChkDestIntegrated_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateDestVisibility();
        }

        private void UpdateDestVisibility()
        {
            if (grpDestAccess == null || grpDestSqlServer == null || grpDestPostgreSql == null) return;

            int idx = cboDestDbType.SelectedIndex;
            grpDestAccess.Visibility = idx == 0 ? Visibility.Visible : Visibility.Collapsed;
            grpDestSqlServer.Visibility = idx == 1 ? Visibility.Visible : Visibility.Collapsed;
            grpDestPostgreSql.Visibility = idx == 2 ? Visibility.Visible : Visibility.Collapsed;

            if (idx == 1)
            {
                bool useIntegrated = chkDestIntegrated.IsChecked ?? false;
                txtDestUser.IsEnabled = !useIntegrated;
                txtDestPassword.IsEnabled = !useIntegrated;
            }

            UpdateDestPreview();
        }

        private void DestField_Changed(object sender, RoutedEventArgs e)
        {
            UpdateDestPreview();
        }

        private void SpnDestYear_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (spnDestYear?.Value != null && int.TryParse(spnDestYear.Value.ToString(), out int y))
            {
                dtOpeningDate.DateTime = new DateTime(y, 1, 1);
            }
        }

        private void UpdateDestPreview()
        {
            if (txtDestConnPreview == null) return;

            int idx = cboDestDbType.SelectedIndex;
            string preview = string.Empty;

            if (idx == 0) // Access
            {
                string path = txtDestAccessPath?.Text ?? "";
                preview = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path}";
            }
            else if (idx == 1) // SQL Server
            {
                string server = txtDestServer?.Text ?? "";
                string db = txtDestDatabase?.Text ?? "";
                bool integrated = chkDestIntegrated?.IsChecked ?? false;

                if (integrated)
                    preview = $"data source={server};initial catalog={db};Integrated Security=SSPI;TrustServerCertificate=True;";
                else
                {
                    string user = txtDestUser?.Text ?? "";
                    string pass = txtDestPassword?.EditValue as string ?? "";
                    preview = $"data source={server};initial catalog={db};User ID={user};Password={pass};TrustServerCertificate=True;";
                }
            }
            else if (idx == 2) // PostgreSQL
            {
                string host = txtDestPgHost?.Text ?? "";
                string port = txtDestPgPort?.Text ?? "5432";
                string db = txtDestPgDatabase?.Text ?? "";
                string user = txtDestPgUsername?.Text ?? "";
                string pass = txtDestPgPassword?.EditValue as string ?? "";
                preview = $"Host={host};Port={port};Database={db};Username={user};Password={pass};";
            }

            txtDestConnPreview.Text = preview;
        }

        private void BtnBrowseDestAccess_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp CSDL Access đích (.accdb / .mdb)",
                Filter = "Access Database|*.accdb;*.mdb|All Files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                txtDestAccessPath.Text = dlg.FileName;
            }
        }

        private AppConfig BuildDestConfig()
        {
            var cfg = new AppConfig();
            int idx = cboDestDbType.SelectedIndex;
            cfg.DbType = idx == 0 ? DatabaseType.Access
                       : idx == 1 ? DatabaseType.SqlServer
                       : DatabaseType.PostgreSql;

            cfg.AccessFilePath = txtDestAccessPath.Text?.Trim() ?? "";

            cfg.SqlServerName = txtDestServer.Text?.Trim() ?? "";
            cfg.SqlDatabaseName = txtDestDatabase.Text?.Trim() ?? "";
            cfg.SqlIntegratedSecurity = chkDestIntegrated.IsChecked ?? false;
            cfg.SqlUsername = txtDestUser.Text?.Trim() ?? "";
            cfg.SqlPassword = txtDestPassword.EditValue as string ?? "";

            cfg.PgHost = txtDestPgHost.Text?.Trim() ?? "";
            cfg.PgPort = int.TryParse(txtDestPgPort.Text, out int port) ? port : 5432;
            cfg.PgDatabase = txtDestPgDatabase.Text?.Trim() ?? "";
            cfg.PgUsername = txtDestPgUsername.Text?.Trim() ?? "";
            cfg.PgPassword = txtDestPgPassword.EditValue as string ?? "";

            return cfg;
        }

        private async void BtnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var destCfg = BuildDestConfig();
                AppendLog("Đang kiểm tra kết nối tới CSDL đích...");

                bool ok = await _transferService.TestDestinationConnectionAsync(destCfg);
                if (ok)
                {
                    AppendLog("=> Kết nối CSDL đích THÀNH CÔNG!");
                    MessageBox.Show("Kết nối tới Cơ sở dữ liệu đích thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    AppendLog("=> Kết nối CSDL đích THẤT BẠI. Vui lòng kiểm tra lại đường dẫn/server/database!");
                    MessageBox.Show("Không thể kết nối tới Cơ sở dữ liệu đích.\nVui lòng kiểm tra lại thông số cấu hình!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Lỗi kiểm tra kết nối: {ex.Message}");
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            int destYear = Convert.ToInt32(spnDestYear.Value);
            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn thực hiện chuyển đổi toàn bộ số dư sang niên độ mới ({destYear})?\n\n(Lưu ý: Quá trình này sẽ tổng hợp hàng loạt số dư cuối kỳ và chuyển vào CSDL đích)",
                "Xác nhận chuyển đổi số dư",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            btnStart.IsEnabled = false;
            btnClose.IsEnabled = false;
            pbProgress.Value = 0;
            lblCurrentStep.Text = "Đang bắt đầu tiến trình...";
            txtLogs.Clear();

            try
            {
                var context = new TransferBalanceContext
                {
                    SourceConfig = ConfigManager.Current,
                    DestinationConfig = BuildDestConfig(),
                    SourceYear = Convert.ToInt32(spnSourceYear.Value),
                    DestinationYear = destYear,
                    OpeningDate = dtOpeningDate.DateTime
                };

                var progress = new Progress<TransferProgressReport>(report =>
                {
                    pbProgress.Value = report.Percent;
                    if (!string.IsNullOrWhiteSpace(report.StepTitle))
                    {
                        lblCurrentStep.Text = report.StepTitle;
                    }
                    AppendLog(report.ToString());
                });

                await _transferService.ExecuteTransferAsync(context, progress);

                MessageBox.Show("Tiến trình điều phối chuyển đổi số dư hoàn tất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"[NGOẠI LỆ] {ex.Message}");
                MessageBox.Show($"Có lỗi trong quá trình thực hiện: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnStart.IsEnabled = true;
                btnClose.IsEnabled = true;
            }
        }

        private void AppendLog(string message)
        {
            txtLogs.AppendText($"{message}\n");
            txtLogs.ScrollToEnd();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
