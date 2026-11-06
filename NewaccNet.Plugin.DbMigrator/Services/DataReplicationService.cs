using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using NewaccNet.Core.Data;

namespace NewaccNet.Plugin.DbMigrator.Services
{
    public class ReplicationOrderItem
    {
        public int StepOrder { get; set; }
        public int Level { get; set; }
        public string TableName { get; set; } = string.Empty;
        public bool HasAutonumberPk { get; set; }
        public bool HasCompositePk { get; set; }
    }

    public class DataReplicationService
    {
        public static List<ReplicationOrderItem> LoadReplicationOrder(string csvPath)
        {
            var list = new List<ReplicationOrderItem>();
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"Không tìm thấy tệp thứ tự: {csvPath}");

            string[] lines = File.ReadAllLines(csvPath);
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length < 3) continue;

                if (!int.TryParse(parts[0].Trim(), out int stepOrder)) continue;
                int.TryParse(parts[1].Trim(), out int level);
                string tableName = parts[2].Trim();
                bool hasAutonumber = parts.Length > 3 && bool.TryParse(parts[3].Trim(), out bool bAuto) && bAuto;
                bool hasComposite = parts.Length > 4 && bool.TryParse(parts[4].Trim(), out bool bComp) && bComp;

                list.Add(new ReplicationOrderItem
                {
                    StepOrder = stepOrder,
                    Level = level,
                    TableName = tableName,
                    HasAutonumberPk = hasAutonumber,
                    HasCompositePk = hasComposite
                });
            }

            return list;
        }

        public async Task ReplicateDataAsync(
            string csvPath,
            string targetServer,
            string targetDatabase,
            bool isWindowsAuth,
            string? user,
            string? password,
            bool includeVouchers,
            IProgress<string>? logProgress,
            IProgress<double>? percentProgress,
            CancellationToken cancellationToken)
        {
            var items = LoadReplicationOrder(csvPath);
            logProgress?.Report($"[Bắt đầu] Đã nạp {items.Count} bảng từ danh mục thứ tự replication.");

            string targetConnStr = DbProvisioningService.BuildConnectionString(
                targetServer, targetDatabase, isWindowsAuth, user, password, 60);

            // 1. Mở kết nối nguồn và đích
            using DbConnection sourceConn = AppDataAccessAdapter.CreateRawConnection();
            if (sourceConn.State != ConnectionState.Open)
            {
                await sourceConn.OpenAsync(cancellationToken);
            }

            await using var targetConn = new SqlConnection(targetConnStr);
            await targetConn.OpenAsync(cancellationToken);

            logProgress?.Report($"Đã mở kết nối: Nguồn [{sourceConn.Database}] ──▶ Đích [{targetDatabase}].");

            // 2. Tạm thời vô hiệu hóa kiểm tra khóa ngoại ở DB đích để tối ưu tốc độ và an toàn
            try
            {
                await using var disableFkCmd = new SqlCommand("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'", targetConn);
                await disableFkCmd.ExecuteNonQueryAsync(cancellationToken);
                logProgress?.Report("ℹ️ Đã tạm thời vô hiệu hóa toàn bộ ràng buộc khóa ngoại trên CSDL đích.");
            }
            catch (Exception ex)
            {
                logProgress?.Report($"[Lưu ý] Không thể vô hiệu hóa FK: {ex.Message}");
            }

            // 3. Tiến hành sao chép từng bảng
            int total = items.Count;
            int processedCount = 0;
            int totalRowsCopied = 0;

            for (int i = 0; i < total; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = items[i];

                // Nếu người dùng không muốn chuyển chứng từ và bảng ở cấp 2 trở lên (vouchers, details)
                if (!includeVouchers && item.Level >= 2)
                {
                    processedCount++;
                    continue;
                }

                try
                {
                    using var selectCmd = sourceConn.CreateCommand();
                    selectCmd.CommandText = $"SELECT * FROM [{item.TableName}]";
                    selectCmd.CommandTimeout = 300;

                    using var reader = await selectCmd.ExecuteReaderAsync(cancellationToken);

                    // Đọc dữ liệu vào DataTable để kiểm tra số lượng và cấu trúc cột
                    var dt = new DataTable();
                    dt.Load(reader);
                    int rowCount = dt.Rows.Count;

                    if (rowCount == 0)
                    {
                        logProgress?.Report($"[{item.StepOrder:D2}/{total:D2}] Bảng '{item.TableName}': 0 bản ghi (trống).");
                    }
                    else
                    {
                        // Lấy danh sách cột thực tế của bảng đích trên SQL Server (so khớp hoa/thường không phân biệt)
                        var destCols = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        using (var destColsCmd = new SqlCommand(
                            "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'Newacc' AND TABLE_NAME = @tName", targetConn))
                        {
                            destColsCmd.Parameters.AddWithValue("@tName", item.TableName);
                            using var colReader = await destColsCmd.ExecuteReaderAsync(cancellationToken);
                            while (await colReader.ReadAsync(cancellationToken))
                            {
                                string cName = colReader.GetString(0);
                                destCols[cName] = cName;
                            }
                        }

                        var copyOptions = item.HasAutonumberPk
                            ? SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls
                            : SqlBulkCopyOptions.KeepNulls;

                        using var bulkCopy = new SqlBulkCopy(targetConn, copyOptions, null)
                        {
                            DestinationTableName = $"[Newacc].[{item.TableName}]",
                            BulkCopyTimeout = 300
                        };

                        // Khớp nối tên cột không phân biệt hoa thường (ví dụ DBCR -> Dbcr)
                        foreach (DataColumn sourceCol in dt.Columns)
                        {
                            if (destCols.TryGetValue(sourceCol.ColumnName, out string? realDestColName))
                            {
                                bulkCopy.ColumnMappings.Add(sourceCol.ColumnName, realDestColName);
                            }
                        }

                        await bulkCopy.WriteToServerAsync(dt, cancellationToken);
                        totalRowsCopied += rowCount;

                        logProgress?.Report($"[{item.StepOrder:D2}/{total:D2}] ✅ Bảng '{item.TableName}': {rowCount:N0} bản ghi.");

                        // Reseed Identity nếu bảng có khóa tự tăng
                        if (item.HasAutonumberPk && rowCount > 0)
                        {
                            try
                            {
                                await using var reseedCmd = new SqlCommand(
                                    $"DBCC CHECKIDENT ('[Newacc].[{item.TableName}]', RESEED);", targetConn);
                                await reseedCmd.ExecuteNonQueryAsync(cancellationToken);
                            }
                            catch
                            {
                                // Bỏ qua nếu bảng không hỗ trợ reseed
                            }
                        }
                    }
                }
                catch (DbException ex)
                {
                    // Trường hợp bảng không tồn tại trong DB nguồn
                    logProgress?.Report($"[{item.StepOrder:D2}/{total:D2}] ⚠️ Bảng '{item.TableName}' bỏ qua: {ex.Message}");
                }
                catch (Exception ex)
                {
                    logProgress?.Report($"[{item.StepOrder:D2}/{total:D2}] ❌ Lỗi sao chép bảng '{item.TableName}': {ex.Message}");
                    throw;
                }

                processedCount++;
                double pct = ((double)processedCount / total) * 100;
                percentProgress?.Report(pct);
            }

            // 4. Kích hoạt lại toàn bộ ràng buộc khóa ngoại
            try
            {
                await using var enableFkCmd = new SqlCommand("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'", targetConn);
                await enableFkCmd.ExecuteNonQueryAsync(cancellationToken);
                logProgress?.Report("ℹ️ Đã kích hoạt lại toàn bộ ràng buộc khóa ngoại trên CSDL đích.");
            }
            catch (Exception ex)
            {
                logProgress?.Report($"[Lưu ý] Không thể bật lại FK: {ex.Message}");
            }

            percentProgress?.Report(100);
            logProgress?.Report($"🎉 Hoàn thành nhân bản dữ liệu! Tổng số: {totalRowsCopied:N0} bản ghi đã được chuyển thành công.");
        }
    }
}
