using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private string FindDefaultTemplatePath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] candidates = new[]
            {
                Path.Combine(baseDir, "Template", "template.accdb"),
                Path.Combine(baseDir, "template.accdb"),
                Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\Template\template.accdb")),
                @"D:\NewaccNet\NewaccNet.Wpf\Template\template.accdb"
            };

            foreach (var path in candidates)
            {
                if (File.Exists(path))
                    return path;
            }
            return string.Empty;
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

            // Đường dẫn tệp mẫu template.accdb mặc định
            txtTemplateAccessPath.Text = FindDefaultTemplatePath();

            // Khởi tạo các ô CSDL đích
            if (src.DbType == DatabaseType.Access)
            {
                string dir = !string.IsNullOrEmpty(src.AccessFilePath) ? Path.GetDirectoryName(src.AccessFilePath) ?? "" : "";
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
            if (spnDestYear?.Value != null && int.TryParse(spnDestYear.Value.ToString(), out int y) && y >= 1900 && y <= 2100)
            {
                dtOpeningDate.DateTime = new DateTime(y, 1, 1);
            }
        }

        private void UpdateDestPreview()
        {
            if (txtDestConnPreview == null) return;

            var cfg = BuildDestConfig();
            txtDestConnPreview.Text = cfg.ConnectionString;
        }

        private void BtnBrowseTemplateAccess_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp mẫu Access (template.accdb / .mdb)",
                Filter = "Access Database|*.accdb;*.mdb|All Files|*.*"
            };
            if (!string.IsNullOrWhiteSpace(txtTemplateAccessPath.Text) && File.Exists(txtTemplateAccessPath.Text))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(txtTemplateAccessPath.Text);
                dlg.FileName = Path.GetFileName(txtTemplateAccessPath.Text);
            }
            if (dlg.ShowDialog() == true)
            {
                txtTemplateAccessPath.Text = dlg.FileName;
            }
        }

        private void BtnBrowseDestAccess_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Chọn hoặc đặt tên tệp CSDL Access đích (.accdb / .mdb)",
                Filter = "Access Database (*.accdb)|*.accdb|Access 2000-2003 (*.mdb)|*.mdb|All Files|*.*",
                DefaultExt = ".accdb",
                OverwritePrompt = false
            };
            if (!string.IsNullOrWhiteSpace(txtDestAccessPath.Text))
            {
                string? dir = Path.GetDirectoryName(txtDestAccessPath.Text);
                if (!string.IsNullOrEmpty(dir) && System.IO.Directory.Exists(dir))
                    dlg.InitialDirectory = dir;
                dlg.FileName = Path.GetFileName(txtDestAccessPath.Text);
            }
            if (dlg.ShowDialog() == true)
            {
                txtDestAccessPath.Text = dlg.FileName;
                UpdateDestPreview();
            }
        }

        private void BtnCopyFromTemplate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string templatePath = txtTemplateAccessPath.Text?.Trim() ?? "";
                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                {
                    MessageBox.Show("Vui lòng chọn đường dẫn tệp mẫu (template.accdb) hợp lệ trước!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string destPath = txtDestAccessPath.Text?.Trim() ?? "";
                if (string.IsNullOrEmpty(destPath))
                {
                    BtnBrowseDestAccess_Click(sender, e);
                    destPath = txtDestAccessPath.Text?.Trim() ?? "";
                    if (string.IsNullOrEmpty(destPath)) return;
                }

                string srcPath = ConfigManager.Current.AccessFilePath?.Trim() ?? "";
                if (string.Equals(destPath, srcPath, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Đường dẫn tệp CSDL đích không được trùng với tệp CSDL nguồn đang làm việc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.Equals(destPath, templatePath, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Đường dẫn tệp CSDL đích không được trùng với tệp mẫu (template.accdb)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (File.Exists(destPath))
                {
                    var res = MessageBox.Show(
                        $"Tệp đích '{Path.GetFileName(destPath)}' đã tồn tại.\nBạn có muốn ghi đè (tạo mới lại) từ tệp mẫu template.accdb không?",
                        "Xác nhận tạo tệp từ Template",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (res != MessageBoxResult.Yes) return;
                }

                string? destDir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(destDir) && !System.IO.Directory.Exists(destDir))
                {
                    System.IO.Directory.CreateDirectory(destDir);
                }

                File.Copy(templatePath, destPath, overwrite: true);
                AppendLog($"[TEMPLATE] Đã tạo tệp CSDL đích mới từ mẫu: {destPath}");
                MessageBox.Show($"Đã nhân bản thành công tệp CSDL mới từ Template:\n{destPath}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"[LỖI TEMPLATE] Không thể copy từ tệp mẫu: {ex.Message}");
                MessageBox.Show($"Lỗi khi tạo tệp từ Template: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private AppConfig BuildDestConfig()
        {
            var cfg = new AppConfig();
            int idx = cboDestDbType.SelectedIndex;
            cfg.DbType = idx == 0 ? DatabaseType.Access
                       : idx == 1 ? DatabaseType.SqlServer
                       : DatabaseType.PostgreSql;

            cfg.AccessFilePath = txtDestAccessPath?.Text?.Trim() ?? "";

            cfg.SqlServerName = txtDestServer?.Text?.Trim() ?? "";
            cfg.SqlDatabaseName = txtDestDatabase?.Text?.Trim() ?? "";
            cfg.SqlIntegratedSecurity = chkDestIntegrated?.IsChecked ?? false;
            cfg.SqlUsername = txtDestUser?.Text?.Trim() ?? "";
            cfg.SqlPassword = txtDestPassword?.EditValue as string ?? "";

            cfg.PgHost = txtDestPgHost?.Text?.Trim() ?? "";
            cfg.PgPort = int.TryParse(txtDestPgPort?.Text, out int port) ? port : 5432;
            cfg.PgDatabase = txtDestPgDatabase?.Text?.Trim() ?? "";
            cfg.PgUsername = txtDestPgUsername?.Text?.Trim() ?? "";
            cfg.PgPassword = txtDestPgPassword?.EditValue as string ?? "";

            if (cfg.DbType == DatabaseType.Access)
            {
                cfg.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={cfg.AccessFilePath};Persist Security Info=False;";
            }
            else if (cfg.DbType == DatabaseType.SqlServer)
            {
                cfg.ConnectionString = cfg.SqlIntegratedSecurity
                    ? $"data source={cfg.SqlServerName};initial catalog={cfg.SqlDatabaseName};Integrated Security=SSPI;TrustServerCertificate=True;"
                    : $"data source={cfg.SqlServerName};initial catalog={cfg.SqlDatabaseName};User ID={cfg.SqlUsername};Password={cfg.SqlPassword};TrustServerCertificate=True;";
            }
            else
            {
                cfg.ConnectionString = $"Host={cfg.PgHost};Port={cfg.PgPort};Database={cfg.PgDatabase};Username={cfg.PgUsername};Password={cfg.PgPassword};";
            }

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
                    MessageBox.Show("Không thể kết nối tới Cơ sở dữ liệu đích.\n(Nếu là Access và tệp chưa tồn tại, bạn có thể nhấn nút 'Tạo file đích từ Template' trước).", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            var destCfg = BuildDestConfig();

            // Kiểm tra riêng cho MS Access: tự động copy từ Template nếu file đích chưa tồn tại
            if (destCfg.DbType == DatabaseType.Access)
            {
                if (string.IsNullOrWhiteSpace(destCfg.AccessFilePath))
                {
                    MessageBox.Show("Vui lòng chọn hoặc nhập đường dẫn tệp CSDL Access đích!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string srcPath = ConfigManager.Current.AccessFilePath?.Trim() ?? "";
                if (string.Equals(destCfg.AccessFilePath, srcPath, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Đường dẫn tệp CSDL đích không được trùng với tệp CSDL nguồn đang làm việc!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!File.Exists(destCfg.AccessFilePath))
                {
                    string templatePath = txtTemplateAccessPath.Text?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                    {
                        var createConfirm = MessageBox.Show(
                            $"Tệp CSDL đích '{Path.GetFileName(destCfg.AccessFilePath)}' chưa tồn tại.\nBạn có muốn tự động tạo tệp này từ tệp mẫu (template.accdb) và bắt đầu chuyển số dư ngay không?",
                            "Tạo CSDL đích từ Template",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (createConfirm != MessageBoxResult.Yes) return;

                        string? destDir = Path.GetDirectoryName(destCfg.AccessFilePath);
                        if (!string.IsNullOrEmpty(destDir) && !System.IO.Directory.Exists(destDir))
                        {
                            System.IO.Directory.CreateDirectory(destDir);
                        }

                        File.Copy(templatePath, destCfg.AccessFilePath, overwrite: true);
                        AppendLog($"[TEMPLATE] Đã tự động tạo tệp CSDL đích từ mẫu: {destCfg.AccessFilePath}");
                    }
                    else
                    {
                        MessageBox.Show("Tệp CSDL đích chưa tồn tại và đường dẫn tệp mẫu (template.accdb) không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            bool hasAnyModule = (chkTransferLibrary.IsChecked == true) ||
                                (chkTransferDesign.IsChecked == true) ||
                                (chkTransferNormal.IsChecked == true) ||
                                (chkTransferDebt.IsChecked == true) ||
                                (chkTransferMaterial.IsChecked == true) ||
                                (chkTransferCost.IsChecked == true) ||
                                (chkTransferAsset.IsChecked == true) ||
                                (chkTransferStock.IsChecked == true) ||
                                (chkTransferCurrency.IsChecked == true);

            if (!hasAnyModule)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một phân hệ để thực hiện chuyển đổi!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int destYear = Convert.ToInt32(spnDestYear.Value);

            var selectedModules = new List<string>();
            if (chkTransferLibrary.IsChecked == true) selectedModules.Add("1. Danh mục");
            if (chkTransferDesign.IsChecked == true) selectedModules.Add("2. Thiết kế");
            if (chkTransferNormal.IsChecked == true) selectedModules.Add("3. TK thường (Loại I)");
            if (chkTransferDebt.IsChecked == true) selectedModules.Add("4. Công nợ (Loại A)");
            if (chkTransferMaterial.IsChecked == true) selectedModules.Add("5. Vật tư (Loại B)");
            if (chkTransferCost.IsChecked == true) selectedModules.Add("6. Chi phí (Loại E)");
            if (chkTransferAsset.IsChecked == true) selectedModules.Add("7. TSCĐ (Loại D)");
            if (chkTransferStock.IsChecked == true) selectedModules.Add("8. Cổ phiếu (Loại S)");
            if (chkTransferCurrency.IsChecked == true) selectedModules.Add("9. Ngoại tệ (Loại C)");

            string moduleListStr = string.Join("\n - ", selectedModules);

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn thực hiện chuyển đổi các phân hệ sau sang niên độ mới ({destYear})?\n\n - {moduleListStr}\n\n(Lưu ý: Nếu phân hệ đã có số dư trong CSDL đích, hệ thống sẽ tự động dọn dẹp số dư cũ của phân hệ đó trước khi nạp mới)",
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
                    DestinationConfig = destCfg,
                    SourceYear = Convert.ToInt32(spnSourceYear.Value),
                    DestinationYear = destYear,
                    OpeningDate = dtOpeningDate.DateTime,
                    TransferLibrary = chkTransferLibrary.IsChecked == true,
                    TransferDesign = chkTransferDesign.IsChecked == true,
                    TransferNormal = chkTransferNormal.IsChecked == true,
                    TransferDebt = chkTransferDebt.IsChecked == true,
                    TransferMaterial = chkTransferMaterial.IsChecked == true,
                    TransferCost = chkTransferCost.IsChecked == true,
                    TransferAsset = chkTransferAsset.IsChecked == true,
                    TransferStock = chkTransferStock.IsChecked == true,
                    TransferCurrency = chkTransferCurrency.IsChecked == true
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

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SetAllModulesChecked(true);
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            SetAllModulesChecked(false);
        }

        private void ModuleCheck_Changed(object sender, RoutedEventArgs e)
        {
            UpdateModuleHeader();
        }

        private void UpdateModuleHeader()
        {
            if (grpModularTransfer == null) return;

            int count = 0;
            if (chkTransferLibrary?.IsChecked == true) count++;
            if (chkTransferDesign?.IsChecked == true) count++;
            if (chkTransferNormal?.IsChecked == true) count++;
            if (chkTransferDebt?.IsChecked == true) count++;
            if (chkTransferMaterial?.IsChecked == true) count++;
            if (chkTransferCost?.IsChecked == true) count++;
            if (chkTransferAsset?.IsChecked == true) count++;
            if (chkTransferStock?.IsChecked == true) count++;
            if (chkTransferCurrency?.IsChecked == true) count++;

            if (count == 9)
            {
                grpModularTransfer.Header = "4. Tùy chọn Phân hệ chuyển đổi (Mặc định: Tất cả 9 phân hệ - Bấm để mở rộng)";
            }
            else if (count == 0)
            {
                grpModularTransfer.Header = "4. Tùy chọn Phân hệ chuyển đổi (Chưa chọn phân hệ nào - Bấm để mở rộng)";
            }
            else
            {
                grpModularTransfer.Header = $"4. Tùy chọn Phân hệ chuyển đổi (Đã chọn: {count}/9 phân hệ - Bấm để mở rộng)";
            }
        }

        private void SetAllModulesChecked(bool isChecked)
        {
            chkTransferLibrary.IsChecked = isChecked;
            chkTransferDesign.IsChecked = isChecked;
            chkTransferNormal.IsChecked = isChecked;
            chkTransferDebt.IsChecked = isChecked;
            chkTransferMaterial.IsChecked = isChecked;
            chkTransferCost.IsChecked = isChecked;
            chkTransferAsset.IsChecked = isChecked;
            chkTransferStock.IsChecked = isChecked;
            chkTransferCurrency.IsChecked = isChecked;
            UpdateModuleHeader();
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
