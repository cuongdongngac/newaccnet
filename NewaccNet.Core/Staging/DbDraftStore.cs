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
            var doc = new StagingDocument
            {
                DraftId = draft.DraftId,
                DraftType = draft.DraftType.ToString(),
                Status = draft.Status,
                SourceKind = draft.Source.Kind,
                SourceRef = draft.Source.Ref,
                Title = draft.GetDisplayTitle(),
                PayloadJson = json,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PostedVoucherId = draft.PostedVoucherId
            };

            await SaveDocumentAsync(doc, ct);
        }

        public async Task<VoucherDraft?> GetAsync(string draftId, CancellationToken ct = default)
        {
            var doc = await GetDocumentAsync(draftId, ct);
            if (doc == null || string.IsNullOrWhiteSpace(doc.PayloadJson)) return null;

            try
            {
                return JsonSerializer.Deserialize<VoucherDraft>(doc.PayloadJson, JsonOptions);
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveDocumentAsync(StagingDocument doc, CancellationToken ct = default)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            DateTime now = DateTime.Now;

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            bool exists = false;
            using (var checkCmd = conn.CreateCommand())
            {
                checkCmd.CommandText = "SELECT COUNT(*) FROM StagingDraft WHERE DraftId = @DraftId";
                var p = checkCmd.CreateParameter();
                p.ParameterName = "@DraftId";
                p.Value = doc.DraftId;
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
                    pCreated.Value = doc.CreatedAt == default ? now : doc.CreatedAt;
                    cmd.Parameters.Add(pCreated);
                }

                AddParam(cmd, "@DraftId", doc.DraftId);
                AddParam(cmd, "@DraftType", doc.DraftType ?? "PurchaseInvoice");
                AddParam(cmd, "@Status", doc.Status.ToString());
                AddParam(cmd, "@SourceKind", doc.SourceKind ?? string.Empty);
                AddParam(cmd, "@SourceRef", doc.SourceRef ?? string.Empty);
                AddParam(cmd, "@PayloadJson", doc.PayloadJson ?? "{}");
                AddParam(cmd, "@UpdatedAt", now);
                AddParam(cmd, "@PostedVoucherId", (object?)doc.PostedVoucherId ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync(ct);
            }
        }

        public async Task<StagingDocument?> GetDocumentAsync(string draftId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(draftId)) return null;

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT DraftId, DraftType, Status, SourceKind, SourceRef, PayloadJson, CreatedAt, UpdatedAt, PostedVoucherId FROM StagingDraft WHERE DraftId = @DraftId";
            AddParam(cmd, "@DraftId", draftId);

            using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                return ReadDocument(reader);
            }

            return null;
        }

        public async Task<IReadOnlyList<StagingDocument>> QueryDocumentsAsync(DraftQueryFilter? filter = null, CancellationToken ct = default)
        {
            var list = new List<StagingDocument>();

            using var conn = AppDataAccessAdapter.CreateRawConnection();
            await conn.OpenAsync(ct);

            using var cmd = conn.CreateCommand();
            string sql = "SELECT DraftId, DraftType, Status, SourceKind, SourceRef, PayloadJson, CreatedAt, UpdatedAt, PostedVoucherId FROM StagingDraft WHERE 1=1";

            if (filter != null)
            {
                if (filter.Status.HasValue)
                {
                    sql += " AND Status = @Status";
                    AddParam(cmd, "@Status", filter.Status.Value.ToString());
                }
                if (!string.IsNullOrWhiteSpace(filter.DraftTypeString))
                {
                    sql += " AND DraftType = @DraftType";
                    AddParam(cmd, "@DraftType", filter.DraftTypeString);
                }
                else if (filter.DraftType.HasValue)
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
                list.Add(ReadDocument(reader));
            }

            return list;
        }

        private static StagingDocument ReadDocument(DbDataReader reader)
        {
            var doc = new StagingDocument
            {
                DraftId = reader.GetString(0),
                DraftType = reader.GetString(1),
                PayloadJson = reader.GetString(5),
                CreatedAt = reader.GetDateTime(6),
                UpdatedAt = reader.GetDateTime(7)
            };

            if (Enum.TryParse<DraftStatus>(reader.GetString(2), out var st))
                doc.Status = st;

            if (!reader.IsDBNull(3)) doc.SourceKind = reader.GetString(3);
            if (!reader.IsDBNull(4)) doc.SourceRef = reader.GetString(4);
            if (!reader.IsDBNull(8)) doc.PostedVoucherId = reader.GetInt32(8);

            return doc;
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
