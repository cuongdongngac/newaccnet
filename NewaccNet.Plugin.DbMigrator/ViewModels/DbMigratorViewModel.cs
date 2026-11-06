using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using NewaccNet.Core.Data;
using NewaccNet.Plugin.DbMigrator.Services;

namespace NewaccNet.Plugin.DbMigrator.ViewModels
{
    public class DbMigratorViewModel : INotifyPropertyChanged
    {
        private readonly DbProvisioningService _provisioningService = new();
        private readonly DataReplicationService _replicationService = new();
        private CancellationTokenSource? _cts;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public DbMigratorViewModel()
        {
            InitDefaultPaths();

            TestConnectionCommand = new RelayCommand(async () => await TestConnectionAsync(), () => !IsBusy);
            BrowseScriptCommand = new RelayCommand(BrowseScript, () => !IsBusy);
            BrowseCsvCommand = new RelayCommand(BrowseCsv, () => !IsBusy);
            CreateDatabaseCommand = new RelayCommand(async () => await CreateDatabaseAsync(), () => !IsBusy);
            ReplicateDataCommand = new RelayCommand(async () => await ReplicateDataAsync(), () => !IsBusy);
            CreateAndReplicateAllCommand = new RelayCommand(async () => await CreateAndReplicateAllAsync(), () => !IsBusy);
            CancelCommand = new RelayCommand(CancelOperation, () => IsBusy);
        }

        private void InitDefaultPaths()
        {
            // 1. Thông tin CSDL nguồn hiện tại
            var cfg = ConfigManager.Current;
            SourceDatabaseInfo = cfg.DbType == DatabaseType.SqlServer
                ? $"SQL Server (Catalog: {cfg.SqlDatabaseName})"
                : cfg.DbType == DatabaseType.PostgreSql
                    ? $"PostgreSQL (Catalog: {cfg.PgDatabase})"
                    : $"MS Access: {Path.GetFileName(cfg.AccessFilePath)}";

            // 2. Thu thập các thư mục cha để tìm kiếm tự động
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var searchRoots = new System.Collections.Generic.List<string> { baseDir, Directory.GetCurrentDirectory() };
            var curDir = new DirectoryInfo(baseDir);
            for (int i = 0; i < 6 && curDir != null; i++)
            {
                if (!searchRoots.Contains(curDir.FullName))
                    searchRoots.Add(curDir.FullName);
                curDir = curDir.Parent;
            }

            // Tìm tệp script SQL Server DDL chuẩn
            foreach (var root in searchRoots)
            {
                string sqlDir = Path.Combine(root, "Template", "SQL Server (SqlClient)");
                if (Directory.Exists(sqlDir))
                {
                    var sqlFile = Directory.GetFiles(sqlDir, "*.sql").FirstOrDefault();
                    if (!string.IsNullOrEmpty(sqlFile) && File.Exists(sqlFile))
                    {
                        ScriptPath = sqlFile;
                        break;
                    }
                }
            }

            // 3. Tìm tệp replication_order.csv
            foreach (var root in searchRoots)
            {
                string[] candidates = new[]
                {
                    Path.Combine(root, "mapping", "replication_order.csv"),
                    Path.Combine(root, "Migration", "mapping", "replication_order.csv")
                };

                foreach (var csv in candidates)
                {
                    if (File.Exists(csv))
                    {
                        ReplicationCsvPath = csv;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(ReplicationCsvPath))
                    break;
            }
        }

        #region Properties

        private string _sourceDatabaseInfo = "Đang xác định...";
        public string SourceDatabaseInfo
        {
            get => _sourceDatabaseInfo;
            set { _sourceDatabaseInfo = value; OnPropertyChanged(); }
        }

        private string _targetServer = @".\SQLEXPRESS";
        public string TargetServer
        {
            get => _targetServer;
            set { _targetServer = value; OnPropertyChanged(); }
        }

        private bool _isWindowsAuth = true;
        public bool IsWindowsAuth
        {
            get => _isWindowsAuth;
            set
            {
                if (_isWindowsAuth != value)
                {
                    _isWindowsAuth = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsSqlAuth));
                }
            }
        }

        public bool IsSqlAuth
        {
            get => !_isWindowsAuth;
            set
            {
                if (_isWindowsAuth == value)
                {
                    _isWindowsAuth = !value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsWindowsAuth));
                }
            }
        }

        private string _targetUser = "sa";
        public string TargetUser
        {
            get => _targetUser;
            set { _targetUser = value; OnPropertyChanged(); }
        }

        private string _targetPassword = string.Empty;
        public string TargetPassword
        {
            get => _targetPassword;
            set { _targetPassword = value; OnPropertyChanged(); }
        }

        private string _targetDatabase = "NewaccNet_2026";
        public string TargetDatabase
        {
            get => _targetDatabase;
            set { _targetDatabase = value; OnPropertyChanged(); }
        }

        private string _scriptPath = string.Empty;
        public string ScriptPath
        {
            get => _scriptPath;
            set { _scriptPath = value; OnPropertyChanged(); }
        }

        private string _replicationCsvPath = string.Empty;
        public string ReplicationCsvPath
        {
            get => _replicationCsvPath;
            set { _replicationCsvPath = value; OnPropertyChanged(); }
        }

        private bool _includeVouchers = true;
        public bool IncludeVouchers
        {
            get => _includeVouchers;
            set { _includeVouchers = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsNotBusy => !IsBusy;

        private double _progressPercent;
        public double ProgressPercent
        {
            get => _progressPercent;
            set { _progressPercent = value; OnPropertyChanged(); }
        }

        private string _statusMessage = "Sẵn sàng.";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private readonly StringBuilder _logBuilder = new();
        public string LogText => _logBuilder.ToString();

        #endregion

        #region Commands

        public ICommand TestConnectionCommand { get; }
        public ICommand BrowseScriptCommand { get; }
        public ICommand BrowseCsvCommand { get; }
        public ICommand CreateDatabaseCommand { get; }
        public ICommand ReplicateDataCommand { get; }
        public ICommand CreateAndReplicateAllCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Actions

        private void AppendLog(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _logBuilder.AppendLine(line);
            OnPropertyChanged(nameof(LogText));
            StatusMessage = message;
        }

        private void BrowseScript()
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp script cấu trúc DDL SQL Server",
                Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                ScriptPath = dlg.FileName;
                AppendLog($"Đã chọn script: {ScriptPath}");
            }
        }

        private void BrowseCsv()
        {
            var dlg = new OpenFileDialog
            {
                Title = "Chọn tệp danh mục thứ tự replication (CSV)",
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                ReplicationCsvPath = dlg.FileName;
                AppendLog($"Đã chọn tệp thứ tự: {ReplicationCsvPath}");
            }
        }

        private async Task TestConnectionAsync()
        {
            IsBusy = true;
            ProgressPercent = 0;
            AppendLog($"Đang kiểm tra kết nối tới máy chủ '{TargetServer}'...");

            try
            {
                bool ok = await _provisioningService.TestConnectionAsync(
                    TargetServer, IsWindowsAuth, TargetUser, TargetPassword, 5);
                if (ok)
                {
                    AppendLog("✅ Kết nối máy chủ SQL Server thành công!");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"❌ Kết nối thất bại: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreateDatabaseAsync()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath) || !File.Exists(ScriptPath))
            {
                AppendLog("❌ Vui lòng chọn tệp script DDL SQL cấu trúc hợp lệ.");
                return;
            }

            IsBusy = true;
            ProgressPercent = 0;
            _cts = new CancellationTokenSource();

            var logProg = new Progress<string>(AppendLog);
            var pctProg = new Progress<double>(p => ProgressPercent = p);

            try
            {
                await _provisioningService.CreateDatabaseAndExecuteScriptAsync(
                    TargetServer, IsWindowsAuth, TargetUser, TargetPassword,
                    TargetDatabase, ScriptPath, logProg, pctProg, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                AppendLog("⚠️ Đã hủy thao tác khởi tạo CSDL.");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ Lỗi khởi tạo CSDL: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                _cts = null;
            }
        }

        private async Task ReplicateDataAsync()
        {
            if (string.IsNullOrWhiteSpace(ReplicationCsvPath) || !File.Exists(ReplicationCsvPath))
            {
                AppendLog("❌ Vui lòng chọn tệp thứ tự replication_order.csv.");
                return;
            }

            IsBusy = true;
            ProgressPercent = 0;
            _cts = new CancellationTokenSource();

            var logProg = new Progress<string>(AppendLog);
            var pctProg = new Progress<double>(p => ProgressPercent = p);

            try
            {
                await _replicationService.ReplicateDataAsync(
                    ReplicationCsvPath, TargetServer, TargetDatabase, IsWindowsAuth, TargetUser, TargetPassword,
                    IncludeVouchers, logProg, pctProg, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                AppendLog("⚠️ Đã hủy thao tác nhân bản dữ liệu.");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ Lỗi nhân bản dữ liệu: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                _cts = null;
            }
        }

        private async Task CreateAndReplicateAllAsync()
        {
            if (string.IsNullOrWhiteSpace(ScriptPath) || !File.Exists(ScriptPath))
            {
                AppendLog("❌ Vui lòng chọn tệp script DDL SQL cấu trúc hợp lệ.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ReplicationCsvPath) || !File.Exists(ReplicationCsvPath))
            {
                AppendLog("❌ Vui lòng chọn tệp thứ tự replication_order.csv.");
                return;
            }

            IsBusy = true;
            ProgressPercent = 0;
            _cts = new CancellationTokenSource();

            var logProg = new Progress<string>(AppendLog);
            var pctProg = new Progress<double>(p => ProgressPercent = p);

            try
            {
                // Bước 1: Khởi tạo DB
                AppendLog("══════════ BƯỚC 1: KHỞI TẠO CSDL ĐÍCH VÀ CẤU TRÚC BẢNG ══════════");
                await _provisioningService.CreateDatabaseAndExecuteScriptAsync(
                    TargetServer, IsWindowsAuth, TargetUser, TargetPassword,
                    TargetDatabase, ScriptPath, logProg, pctProg, _cts.Token);

                // Bước 2: Bơm dữ liệu
                AppendLog("══════════ BƯỚC 2: SAO CHÉP DỮ LIỆU TỪ NGUỒN SANG ĐÍCH ══════════");
                await _replicationService.ReplicateDataAsync(
                    ReplicationCsvPath, TargetServer, TargetDatabase, IsWindowsAuth, TargetUser, TargetPassword,
                    IncludeVouchers, logProg, pctProg, _cts.Token);

                AppendLog("══════════ ✅ HOÀN TẤT TOÀN BỘ QUÁ TRÌNH THÀNH CÔNG! ══════════");
            }
            catch (OperationCanceledException)
            {
                AppendLog("⚠️ Đã hủy thao tác.");
            }
            catch (Exception ex)
            {
                AppendLog($"❌ Quá trình thất bại: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                _cts = null;
            }
        }

        private void CancelOperation()
        {
            _cts?.Cancel();
            AppendLog("Đang gửi yêu cầu dừng...");
        }

        #endregion
    }
}
