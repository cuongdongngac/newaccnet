using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core;
using Microsoft.Win32;
using SysDir = System.IO.Directory;

namespace NewaccNet.Wpf.AppSystem.DbMigration
{
    public partial class ConvertDbWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        private System.Diagnostics.Process? _currentProcess;
        private CancellationTokenSource? _cts;
        private string _migrationDir = string.Empty;
        private string _migrateExePath = string.Empty;

        public ConvertDbWindow()
        {
            InitializeComponent();
            Loaded += ConvertDbWindow_Loaded;
            Closing += ConvertDbWindow_Closing;
        }

        private void ConvertDbWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitEnvironment();
        }

        private void ConvertDbWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_currentProcess != null && !_currentProcess.HasExited)
            {
                var result = ThemedMessageBox.Show(
                    "Cảnh báo",
                    "Quá trình chuyển đổi CSDL đang chạy. Bạn có chắc chắn muốn dừng lại và thoát không?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    StopCurrentProcess();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void InitEnvironment()
        {
            _migrationDir = FindMigrationFolder();
            txtMigrationDir.Text = !string.IsNullOrEmpty(_migrationDir) ? _migrationDir : "Không tìm thấy thư mục Migration";

            if (!string.IsNullOrEmpty(_migrationDir))
            {
                _migrateExePath = Path.Combine(_migrationDir, "migrate.exe");
                bool hasExe = File.Exists(_migrateExePath);

                string mappingDir = Path.Combine(_migrationDir, "mapping");
                bool hasMapping = SysDir.Exists(mappingDir);
                bool hasCols = File.Exists(Path.Combine(mappingDir, "mapping_columns.csv"));
                bool hasTables = File.Exists(Path.Combine(mappingDir, "mapping_tables.csv"));
                bool hasRel = File.Exists(Path.Combine(mappingDir, "mapping_relationships.csv"));
                bool hasOrder = File.Exists(Path.Combine(mappingDir, "migration_order.csv"));

                if (hasExe && hasMapping && hasCols && hasTables && hasRel && hasOrder)
                {
                    lblEngineStatus.Text = "✓ Đã tìm thấy migrate.exe và đầy đủ 4 tệp cấu hình mapping CSV.";
                    lblEngineStatus.Foreground = System.Windows.Media.Brushes.ForestGreen;
                }
                else
                {
                    lblEngineStatus.Text = $"⚠ Cảnh báo: Thiếu file cấu hình (Exe: {hasExe}, Mapping: {hasMapping}, CSVs: {hasCols && hasTables && hasRel && hasOrder})";
                    lblEngineStatus.Foreground = System.Windows.Media.Brushes.Crimson;
                }

                // Tự động tìm tệp template.accdb mặc định
                string defaultTemplate = FindDefaultTemplatePath(_migrationDir);
                if (!string.IsNullOrEmpty(defaultTemplate))
                {
                    txtTemplatePath.Text = defaultTemplate;
                }
            }
            else
            {
                lblEngineStatus.Text = "❌ Không tìm thấy thư mục Migration chứa migrate.exe!";
                lblEngineStatus.Foreground = System.Windows.Media.Brushes.Crimson;
            }
        }

        private string FindMigrationFolder()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] candidates = new[]
            {
                baseDir,
                Path.Combine(baseDir, "Migration"),
                Path.Combine(baseDir, @"..\..\..\Migration"),
                @"D:\NewaccNet\NewaccNet.Wpf\Migration",
                @"D:\mdb-migration-project"
            };

            foreach (var dir in candidates)
            {
                if (SysDir.Exists(dir) && File.Exists(Path.Combine(dir, "migrate.exe")))
                    return Path.GetFullPath(dir);
            }
            return string.Empty;
        }

        private string FindDefaultTemplatePath(string migrationFolder)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] candidates = new[]
            {
                Path.Combine(migrationFolder, "mapping", "template.accdb"),
                Path.Combine(migrationFolder, "template.accdb"),
                Path.Combine(baseDir, "Template", "template.accdb"),
                Path.Combine(baseDir, @"..\..\..\Template\template.accdb"),
                @"D:\NewaccNet\NewaccNet.Wpf\Template\template.accdb"
            };

            foreach (var path in candidates)
            {
                if (File.Exists(path))
                    return Path.GetFullPath(path);
            }
            return string.Empty;
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Chọn tệp CSDL Access nguồn (.mdb / .accdb)",
                Filter = "Access Database (*.mdb;*.accdb)|*.mdb;*.accdb|Tất cả tệp (*.*)|*.*",
                CheckFileExists = true
            };

            if (ofd.ShowDialog() == true)
            {
                txtSourcePath.Text = ofd.FileName;
            }
        }

        private void TxtSourcePath_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            string path = txtSourcePath.Text?.Trim() ?? string.Empty;
            if (File.Exists(path))
            {
                var fi = new FileInfo(path);
                double mb = fi.Length / (1024.0 * 1024.0);
                lblSourceFileInfo.Text = $"Dung lượng: {mb:N2} MB | Sửa đổi: {fi.LastWriteTime:dd/MM/yyyy HH:mm:ss} | Tên: {fi.Name}";

                // Gợi ý tên file đích nếu chưa có
                if (string.IsNullOrWhiteSpace(txtDestPath.Text))
                {
                    string dir = fi.DirectoryName ?? string.Empty;
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(path);
                    txtDestPath.Text = Path.Combine(dir, $"{nameWithoutExt}_New.accdb");
                }
            }
            else
            {
                lblSourceFileInfo.Text = string.IsNullOrEmpty(path) ? "Chưa chọn tệp CSDL nguồn." : "Tệp không tồn tại!";
            }
        }

        private void BtnBrowseTemplate_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Chọn tệp CSDL mẫu (template.accdb)",
                Filter = "Access Template (*.accdb;*.mdb)|*.accdb;*.mdb|Tất cả tệp (*.*)|*.*",
                CheckFileExists = true
            };

            if (ofd.ShowDialog() == true)
            {
                txtTemplatePath.Text = ofd.FileName;
            }
        }

        private void BtnBrowseDest_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "Chọn hoặc đặt tên tệp CSDL đích mới (.accdb)",
                Filter = "Access Database (*.accdb)|*.accdb|Tất cả tệp (*.*)|*.*",
                DefaultExt = ".accdb"
            };

            if (!string.IsNullOrWhiteSpace(txtDestPath.Text))
            {
                try
                {
                    sfd.InitialDirectory = Path.GetDirectoryName(txtDestPath.Text);
                    sfd.FileName = Path.GetFileName(txtDestPath.Text);
                }
                catch { }
            }

            if (sfd.ShowDialog() == true)
            {
                txtDestPath.Text = sfd.FileName;
            }
        }

        private void BtnCopyFromTemplate_Click(object sender, RoutedEventArgs e)
        {
            string templatePath = txtTemplatePath.Text?.Trim() ?? string.Empty;
            string destPath = txtDestPath.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
            {
                ThemedMessageBox.Show("Thông báo", "Vui lòng chọn tệp mẫu template.accdb hợp lệ trước!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(destPath))
            {
                ThemedMessageBox.Show("Thông báo", "Vui lòng chọn đường dẫn tệp CSDL đích (.accdb) trước!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (File.Exists(destPath))
                {
                    var confirm = ThemedMessageBox.Show(
                        "Xác nhận ghi đè",
                        $"Tệp đích '{Path.GetFileName(destPath)}' đã tồn tại.\nBạn có muốn ghi đè bằng tệp mẫu trắng không?",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirm != MessageBoxResult.Yes) return;
                }

                string? destDir = Path.GetDirectoryName(destPath);
                if (!string.IsNullOrEmpty(destDir) && !SysDir.Exists(destDir))
                {
                    SysDir.CreateDirectory(destDir);
                }

                File.Copy(templatePath, destPath, overwrite: true);
                AppendLog($"[HỆ THỐNG] Đã khởi tạo tệp đích mới từ template: {destPath}");
                ThemedMessageBox.Show("Thành công", "Đã tạo tệp đích mới từ template thành công!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ThemedMessageBox.Show("Lỗi", $"Không thể tạo tệp đích: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = txtSourcePath.Text?.Trim() ?? string.Empty;
            string templatePath = txtTemplatePath.Text?.Trim() ?? string.Empty;
            string destPath = txtDestPath.Text?.Trim() ?? string.Empty;

            // 1. Kiểm tra đầu vào
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                ThemedMessageBox.Show("Thông báo", "Vui lòng chọn tệp CSDL nguồn (.mdb / .accdb) hợp lệ!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(destPath))
            {
                ThemedMessageBox.Show("Thông báo", "Vui lòng chọn đường dẫn CSDL đích mới (.accdb)!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.Equals(Path.GetFullPath(sourcePath), Path.GetFullPath(destPath), StringComparison.OrdinalIgnoreCase))
            {
                ThemedMessageBox.Show("Cảnh báo", "Tệp nguồn và tệp đích không được trùng nhau!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_migrateExePath) || !File.Exists(_migrateExePath))
            {
                ThemedMessageBox.Show("Lỗi", $"Không tìm thấy công cụ migrate.exe trong '{_migrationDir}'!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Đảm bảo file đích sẵn sàng từ template
            if (!File.Exists(destPath))
            {
                if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                {
                    ThemedMessageBox.Show("Lỗi", "Tệp đích chưa tồn tại và không tìm thấy tệp mẫu template.accdb để khởi tạo!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    string? destDir = Path.GetDirectoryName(destPath);
                    if (!string.IsNullOrEmpty(destDir) && !SysDir.Exists(destDir))
                    {
                        SysDir.CreateDirectory(destDir);
                    }
                    File.Copy(templatePath, destPath, overwrite: true);
                    AppendLog($"[HỆ THỐNG] Tự động khởi tạo file đích từ template: {destPath}");
                }
                catch (Exception ex)
                {
                    ThemedMessageBox.Show("Lỗi", $"Không thể khởi tạo file đích từ template: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                var askOverwrite = ThemedMessageBox.Show(
                    "Tệp đích đã tồn tại",
                    $"Tệp đích '{Path.GetFileName(destPath)}' đã có sẵn trên ổ đĩa.\n" +
                    "Bạn có muốn làm mới (ghi đè bằng tệp mẫu trắng) trước khi chuyển đổi không?\n\n" +
                    "- Bấm 'Yes' để ghi đè làm mới từ template.\n" +
                    "- Bấm 'No' để giữ nguyên tệp đích hiện có và migrate vào.\n" +
                    "- Bấm 'Cancel' để hủy bỏ.",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (askOverwrite == MessageBoxResult.Cancel) return;

                if (askOverwrite == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                    {
                        ThemedMessageBox.Show("Lỗi", "Không tìm thấy tệp mẫu template.accdb để ghi đè!", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    try
                    {
                        File.Copy(templatePath, destPath, overwrite: true);
                        AppendLog($"[HỆ THỐNG] Đã làm mới tệp đích từ template: {destPath}");
                    }
                    catch (Exception ex)
                    {
                        ThemedMessageBox.Show("Lỗi", $"Không thể ghi đè tệp đích: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            // 3. Đồng bộ tệp mẫu được chọn vào thư mục mapping/template.accdb để migrate.exe đọc đúng file đã chọn
            if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
            {
                try
                {
                    string mappingDir = Path.Combine(_migrationDir, "mapping");
                    if (!SysDir.Exists(mappingDir)) SysDir.CreateDirectory(mappingDir);

                    string targetMappingTemplate = Path.Combine(mappingDir, "template.accdb");
                    if (!string.Equals(Path.GetFullPath(templatePath), Path.GetFullPath(targetMappingTemplate), StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(templatePath, targetMappingTemplate, overwrite: true);
                        AppendLog($"[HỆ THỐNG] Đã đồng bộ tệp mẫu vào: {targetMappingTemplate}");
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"[CẢNH BÁO] Không thể đồng bộ template: {ex.Message}");
                }
            }

            // 4. Khởi chạy tiến trình Go Migrate
            SetRunningState(true);
            _cts = new CancellationTokenSource();
            lblCurrentStep.Text = "Đang thực hiện chuyển đổi dữ liệu...";
            AppendLog($"--- BẮT ĐẦU CHUYỂN ĐỔI: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ---");
            AppendLog($"Nguồn: {sourcePath}");
            AppendLog($"Đích:  {destPath}");
            AppendLog($"Engine: {_migrateExePath}");

            try
            {
                await RunMigrateProcessAsync(sourcePath, destPath, templatePath, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                AppendLog("[HỆ THỐNG] Tiến trình chuyển đổi đã bị người dùng hủy bỏ.");
                lblCurrentStep.Text = "Đã dừng tiến trình.";
            }
            catch (Exception ex)
            {
                AppendLog($"[LỖI NGOẠI LỆ] {ex.Message}");
                lblCurrentStep.Text = "Có lỗi xảy ra trong quá trình chuyển đổi.";
                ThemedMessageBox.Show("Lỗi", $"Quá trình chuyển đổi thất bại: {ex.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetRunningState(false);
            }
        }

        private async Task RunMigrateProcessAsync(string sourceMdb, string destAccdb, string templatePath, CancellationToken cancellationToken)
        {
            string mappingDir = Path.Combine(_migrationDir, "mapping");
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = _migrateExePath,
                Arguments = $"\"{sourceMdb}\" \"{destAccdb}\" \"{templatePath}\" \"{mappingDir}\"",
                WorkingDirectory = _migrationDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            _currentProcess = new System.Diagnostics.Process { StartInfo = psi, EnableRaisingEvents = true };

            var tcs = new TaskCompletionSource<int>();

            _currentProcess.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        AppendLog(e.Data);
                        ParseProgressMessage(e.Data);
                    });
                }
            };

            _currentProcess.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        AppendLog($"[ERR] {e.Data}");
                    });
                }
            };

            _currentProcess.Exited += (s, e) =>
            {
                int exitCode = _currentProcess.ExitCode;
                tcs.TrySetResult(exitCode);
            };

            _currentProcess.Start();
            _currentProcess.BeginOutputReadLine();
            _currentProcess.BeginErrorReadLine();

            using (cancellationToken.Register(() =>
            {
                try
                {
                    if (_currentProcess != null && !_currentProcess.HasExited)
                    {
                        _currentProcess.Kill(entireProcessTree: true);
                    }
                }
                catch { }
                tcs.TrySetCanceled();
            }))
            {
                int exitCode = await tcs.Task;

                if (exitCode == 0)
                {
                    lblCurrentStep.Text = "Chuyển đổi CSDL thành công!";
                    AppendLog($"--- HOÀN TẤT CHUYỂN ĐỔI: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ---");
                    ThemedMessageBox.Show("Thành công", "Đã hoàn tất chuyển đổi cơ sở dữ liệu sang định dạng ACCDB mới!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    lblCurrentStep.Text = $"Quá trình kết thúc với mã lỗi ({exitCode}).";
                    AppendLog($"[CẢNH BÁO] migrate.exe kết thúc với mã lỗi {exitCode}.");
                    ThemedMessageBox.Show("Cảnh báo", $"Quá trình chuyển đổi kết thúc với mã lỗi {exitCode}. Vui lòng xem chi tiết nhật ký bên dưới.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void ParseProgressMessage(string line)
        {
            string trimmed = line.Trim();
            if (trimmed.StartsWith("Migrating ", StringComparison.OrdinalIgnoreCase))
            {
                lblCurrentStep.Text = trimmed;
            }
            else if (trimmed.StartsWith("-> Inserted", StringComparison.OrdinalIgnoreCase))
            {
                lblCurrentStep.Text = trimmed;
            }
            else if (trimmed.StartsWith("-> Done", StringComparison.OrdinalIgnoreCase))
            {
                lblCurrentStep.Text = "Đã hoàn tất các bảng.";
            }
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            var confirm = ThemedMessageBox.Show(
                "Xác nhận dừng",
                "Bạn có chắc chắn muốn dừng tiến trình chuyển đổi CSDL không?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                StopCurrentProcess();
            }
        }

        private void StopCurrentProcess()
        {
            try
            {
                _cts?.Cancel();
                if (_currentProcess != null && !_currentProcess.HasExited)
                {
                    _currentProcess.Kill(entireProcessTree: true);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[LỖI DỪNG] {ex.Message}");
            }
        }

        private void SetRunningState(bool isRunning)
        {
            btnStart.IsEnabled = !isRunning;
            btnStop.IsEnabled = isRunning;
            btnClose.IsEnabled = !isRunning;
            btnCopyFromTemplate.IsEnabled = !isRunning;
            txtSourcePath.IsEnabled = !isRunning;
            txtTemplatePath.IsEnabled = !isRunning;
            txtDestPath.IsEnabled = !isRunning;

            if (isRunning)
            {
                pbProgress.Value = 50;
            }
            else
            {
                pbProgress.Value = 100;
            }
        }

        private void AppendLog(string message)
        {
            txtLogs.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLogs.ScrollToEnd();
        }

        private void BtnClearLog_Click(object sender, RoutedEventArgs e)
        {
            txtLogs.Clear();
            lblCurrentStep.Text = "Sẵn sàng thực hiện chuyển đổi...";
            pbProgress.Value = 0;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
