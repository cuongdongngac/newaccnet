using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace NewaccNet.Plugin.DbMigrator.Services
{
    public class DbProvisioningService
    {
        public static string BuildConnectionString(string server, string? database, bool isWindowsAuth, string? user, string? password, int timeout = 15)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                TrustServerCertificate = true,
                ConnectTimeout = timeout
            };

            if (!string.IsNullOrWhiteSpace(database))
            {
                builder.InitialCatalog = database;
            }

            if (isWindowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = user ?? string.Empty;
                builder.Password = password ?? string.Empty;
            }

            return builder.ConnectionString;
        }

        public async Task<bool> TestConnectionAsync(string server, bool isWindowsAuth, string? user, string? password, int timeoutSeconds = 5)
        {
            string connStr = BuildConnectionString(server, "master", isWindowsAuth, user, password, timeoutSeconds);
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            return true;
        }

        public async Task CreateDatabaseAndExecuteScriptAsync(
            string server,
            bool isWindowsAuth,
            string? user,
            string? password,
            string dbName,
            string scriptPath,
            IProgress<string>? logProgress,
            IProgress<double>? percentProgress,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(dbName))
                throw new ArgumentException("Tên CSDL không được để trống.", nameof(dbName));

            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Không tìm thấy tệp script cấu trúc: {scriptPath}");

            // 1. Kết nối master để tạo Database
            logProgress?.Report($"[1/3] Đang kết nối máy chủ '{server}' (master) để kiểm tra CSDL '{dbName}'...");
            string masterConnStr = BuildConnectionString(server, "master", isWindowsAuth, user, password, 30);

            await using (var masterConn = new SqlConnection(masterConnStr))
            {
                await masterConn.OpenAsync(cancellationToken);

                string checkDbSql = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
                await using var checkCmd = new SqlCommand(checkDbSql, masterConn);
                checkCmd.Parameters.AddWithValue("@dbName", dbName);
                int dbCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync(cancellationToken));

                if (dbCount == 0)
                {
                    logProgress?.Report($"[1/3] CSDL '{dbName}' chưa tồn tại. Đang tiến hành tạo mới...");
                    string createDbSql = $"CREATE DATABASE [{dbName}];";
                    await using var createCmd = new SqlCommand(createDbSql, masterConn);
                    await createCmd.ExecuteNonQueryAsync(cancellationToken);
                    logProgress?.Report($"[1/3] ✅ Tạo mới CSDL '{dbName}' thành công!");
                }
                else
                {
                    logProgress?.Report($"[1/3] ℹ️ CSDL '{dbName}' đã tồn tại sẵn trên máy chủ.");
                }
            }

            percentProgress?.Report(20);

            // 2. Đọc và tiền xử lý Script DDL
            logProgress?.Report($"[2/3] Đang nạp và phân tích tệp script cấu trúc DDL: {Path.GetFileName(scriptPath)}...");
            string scriptContent = await File.ReadAllTextAsync(scriptPath, cancellationToken);

            // 1. Xóa bỏ tất cả các khối comment /* ... */ trước để tránh lệnh GO bị kẹt bên trong comment
            scriptContent = Regex.Replace(scriptContent, @"/\*.*?\*/", "", RegexOptions.Singleline);

            // 2. Xóa triệt để mọi lệnh USE ... (bảo đảm luôn thực thi bên trong CSDL đích, không bao giờ nhảy sang master hay DataAccess)
            scriptContent = Regex.Replace(scriptContent, @"(?mi)^\s*USE\s+.*$", "");

            // 3. Xóa triệt để mọi lệnh CREATE DATABASE ...
            scriptContent = Regex.Replace(scriptContent, @"(?mi)^\s*CREATE\s+DATABASE\s+.*$", "");

            // Tách các lệnh theo từ khóa GO
            string[] batches = Regex.Split(scriptContent, @"(?m)^\s*GO\s*$", RegexOptions.IgnoreCase);
            logProgress?.Report($"[2/3] Tổng số khối lệnh DDL cần thực thi: {batches.Length} khối.");

            // 3. Thực thi từng batch trên CSDL đích
            logProgress?.Report($"[3/3] Bắt đầu dựng bảng, khóa chính, khóa ngoại trên '{dbName}'...");
            string targetConnStr = BuildConnectionString(server, dbName, isWindowsAuth, user, password, 60);

            await using (var targetConn = new SqlConnection(targetConnStr))
            {
                await targetConn.OpenAsync(cancellationToken);

                int executedBatches = 0;
                int totalBatches = batches.Length;

                for (int i = 0; i < totalBatches; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string batch = batches[i].Trim();
                    if (string.IsNullOrWhiteSpace(batch))
                    {
                        executedBatches++;
                        continue;
                    }

                    // Bỏ qua các lệnh USE master / USE DataAccess đơn lẻ vì đã kết nối thẳng vào dbName
                    if (Regex.IsMatch(batch, @"^\s*USE\s+", RegexOptions.IgnoreCase))
                    {
                        executedBatches++;
                        continue;
                    }

                    try
                    {
                        await using var cmd = new SqlCommand(batch, targetConn)
                        {
                            CommandTimeout = 120
                        };
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                    catch (SqlException ex)
                    {
                        // Nếu là lỗi đối tượng đã tồn tại (Object already exists), ta log cảnh báo nhưng tiếp tục
                        if (ex.Number == 2714) // There is already an object named '...' in the database
                        {
                            logProgress?.Report($"  [Cảnh báo] Đối tượng đã tồn tại, bỏ qua: {ex.Message}");
                        }
                        else
                        {
                            logProgress?.Report($"❌ Lỗi khối lệnh ({i + 1}/{totalBatches}): {ex.Message}");
                            throw;
                        }
                    }

                    executedBatches++;
                    double pct = 20 + ((double)executedBatches / totalBatches) * 80;
                    percentProgress?.Report(pct);
                }
            }

            percentProgress?.Report(100);
            logProgress?.Report($"✅ Khởi tạo CSDL '{dbName}' và toàn bộ cấu trúc bảng thành công!");
        }
    }
}
