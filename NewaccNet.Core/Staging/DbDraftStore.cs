using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NewaccNet.Contracts;
using NewaccNet.Core.Data;

namespace NewaccNet.Core.Staging
{
    /// <summary>
    /// Lưu trữ các chứng từ nháp (VoucherDraft) trực tiếp vào bảng StagingDraft trong CSDL.
    /// Dành cho mô hình nhiều máy trạm (SQL Server / PostgreSQL) hoặc khi người dùng cấu hình lưu DB.
    /// </summary>
    public class DbDraftStore : IDraftStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        private static bool _tableInitialized = false;
        private static readonly object _initLock = new();

        public DbDraftStore()
        {
            EnsureTableCreated();
        }

        private static void EnsureTableCreated()
        {
            if (_tableInitialized) return;
            lock (_initLock)
            {
                if (_tableInitialized) return;
                try
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var dbType = ConfigManager.Current.DbType;

                    string sql;
                    if (dbType == DatabaseType.SqlServer)
                    {
                        sql = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StagingDraft]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StagingDraft](
        [DraftId] [nvarchar](64) NOT NULL PRIMARY KEY,
        [DraftType] [nvarchar](50) NOT NULL,
        [Status] [nvarchar](50) NOT NULL,
        [SourceKind] [nvarchar](50) NULL,
        [SourceRef] [nvarchar](255) NULL,
        [PayloadJson] [nvarchar](max) NOT NULL,
        [CreatedAt] [datetime] NOT NULL,
        [UpdatedAt] [datetime] NOT NULL,
        [PostedVoucherId] [int] NULL
    )
END";
                    }
                    else if (dbType == DatabaseType.PostgreSql)
                    {
                        sql = @"
CREATE TABLE IF NOT EXISTS ""StagingDraft"" (
    ""DraftId"" VARCHAR(64) PRIMARY KEY,
    ""DraftType"" VARCHAR(50) NOT NULL,
    ""Status"" VARCHAR(50) NOT NULL,
    ""SourceKind"" VARCHAR(50),
    ""SourceRef"" VARCHAR(255),
    ""PayloadJson"" TEXT NOT NULL,
    ""CreatedAt"" TIMESTAMP NOT NULL,
    ""UpdatedAt"" TIMESTAMP NOT NULL,
    ""PostedVoucherId"" INT
);";
                    }
                    else
                    {
                        // MS Access OleDb
                        sql = @"
CREATE TABLE StagingDraft (
    DraftId VARCHAR(64) PRIMARY KEY,
    DraftType VARCHAR(50),
    Status VARCHAR(50),
    SourceKind VARCHAR(50),
    SourceRef VARCHAR(255),
    PayloadJson LONGCHAR,
    CreatedAt DATETIME,
    UpdatedAt DATETIME,
    PostedVoucherId INT
)";
                    }

                    using var conn = AppDataAccessAdapter.CreateRawConnection();
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    try { cmd.ExecuteNonQuery(); } catch { }
                    _tableInitialized = true;
                }
                catch
                {
                    // Đã tồn tại bảng hoặc không có quyền DDL
                    _tableInitialized = true;
                }
            }
        }

        public async Task SaveAsync(VoucherDraft draft, CancellationToken ct = default)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));

            string json = JsonSerializer.Serialize(draft, JsonOptions);
            DateTime now = DateTime.Now;

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            // Kiểm tra xem đã tồn tại chưa
            bool exists = false;
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM StagingDraft WHERE DraftId = @DraftId";
                var p = checkCmd.CreateParameter();
                p.ParameterName = "@DraftId";
                p.Value = draft.DraftId;
                checkCmd.Parameters.Add(p);

                var countObj = await checkCmd.ExecuteScalarAsync(ct);
                exists = Convert.ToInt32(countObj) > 0;
            }

            using (var cmd = conn.CreateCommand())
            {
                if (exists)
                {
                    cmd.CommandText = @"
UPDATE StagingDraft 
SET DraftType = @DraftType, Status = @Status, SourceKind = @SourceKind, SourceRef = @SourceRef,
    PayloadJson = @PayloadJson, UpdatedAt = @UpdatedAt, PostedVoucherId = @PostedVoucherId
WHERE DraftId = @DraftId";
                }
                else
                {
                    cmd.CommandText = @"
INSERT INTO StagingDraft (DraftId, DraftType, Status, SourceKind, SourceRef, PayloadJson, CreatedAt, UpdatedAt, PostedVoucherId)
VALUES (@DraftId, @DraftType, @Status, @SourceKind, @SourceRef, @PayloadJson, @CreatedAt, @UpdatedAt, @PostedVoucherId)";

                    var pCreated = cmd.CreateParameter();
                    pCreated.ParameterName = "@CreatedAt";
                    pCreated.Value = now;
                    cmd.Parameters.Add(pCreated);
                }

                AddParam(cmd, "@DraftId", draft.DraftId);
                AddParam(cmd, "@DraftType", draft.DraftType.ToString());
                AddParam(cmd, "@Status", draft.Status.ToString());
                AddParam(cmd, "@SourceKind", draft.Source.Kind ?? string.Empty);
                AddParam(cmd, "@SourceRef", draft.Source.Ref ?? string.Empty);
                AddParam(cmd, "@PayloadJson", json);
                AddParam(cmd, "@UpdatedAt", now);
                AddParam(cmd, "@PostedVoucherId", (object?)draft.PostedVoucherId ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        public async Task<VoucherDraft?> GetAsync(string draftId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(draftId)) return null;

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PayloadJson FROM StagingDraft WHERE DraftId = @DraftId";
            AddParam(cmd, "@DraftId", draftId);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                string json = reader.GetString(0);
                return JsonSerializer.Deserialize<VoucherDraft>(json, JsonOptions);
            }

            return null;
        }

        public async Task<IReadOnlyList<VoucherDraft>> QueryAsync(DraftQueryFilter? filter = null, CancellationToken ct = default)
        {
            var list = new List<VoucherDraft>();

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            using var cmd = conn.CreateCommand();
            string sql = "SELECT PayloadJson FROM StagingDraft WHERE 1=1";

            if (filter != null)
            {
                if (filter.Status.HasValue)
                {
                    sql += " AND Status = @Status";
                    AddParam(cmd, "@Status", filter.Status.Value.ToString());
                }
                if (filter.DraftType.HasValue)
                {
                    sql += " AND DraftType = @DraftType";
                    AddParam(cmd, "@DraftType", filter.DraftType.Value.ToString());
                }
                if (!string.IsNullOrWhiteSpace(filter.SourceKind))
                {
                    sql += " AND SourceKind = @SourceKind";
                    AddParam(cmd, "@SourceKind", filter.SourceKind);
                }
            }

            sql += " ORDER BY UpdatedAt DESC";
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                string json = reader.GetString(0);
                try
                {
                    var draft = JsonSerializer.Deserialize<VoucherDraft>(json, JsonOptions);
                    if (draft != null)
                    {
                        if (filter != null)
                        {
                            if (filter.FromDate.HasValue && draft.Header.VoucherDate < filter.FromDate.Value.Date) continue;
                            if (filter.ToDate.HasValue && draft.Header.VoucherDate > filter.ToDate.Value.Date) continue;
                            if (!string.IsNullOrWhiteSpace(filter.SearchText))
                            {
                                string s = filter.SearchText.ToLowerInvariant();
                                bool match = (draft.Header.VoucherNo?.ToLowerInvariant().Contains(s) ?? false) ||
                                             (draft.Counterparty.Raw.Name?.ToLowerInvariant().Contains(s) ?? false) ||
                                             (draft.Counterparty.Raw.TaxCode?.ToLowerInvariant().Contains(s) ?? false) ||
                                             (draft.Invoice.Number?.ToLowerInvariant().Contains(s) ?? false);
                                if (!match) continue;
                            }
                        }
                        list.Add(draft);
                    }
                }
                catch { }
            }

            return list;
        }

        public async Task DeleteAsync(string draftId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(draftId)) return;

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM StagingDraft WHERE DraftId = @DraftId";
            AddParam(cmd, "@DraftId", draftId);

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task<bool> ExistsBySourceRefAsync(string sourceKind, string sourceRef, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sourceRef)) return false;

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM StagingDraft WHERE SourceKind = @SourceKind AND SourceRef = @SourceRef";
            AddParam(cmd, "@SourceKind", sourceKind);
            AddParam(cmd, "@SourceRef", sourceRef);

            var countObj = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt32(countObj) > 0;
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
