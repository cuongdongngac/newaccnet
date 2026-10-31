using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NewaccNet.Core.Data
{
    public class ConfigOptionItem
    {
        public short Value { get; set; }
        public string Name { get; set; } = "";

        public ConfigOptionItem(short value, string name)
        {
            Value = value;
            Name = name;
        }

        public override string ToString() => Name;
    }

    public static class DbConfigConstants
    {
        // 1. Hình thức ghi sổ kế toán (AccountMethod)
        public static readonly ConfigOptionItem[] AccountMethods = new[]
        {
            new ConfigOptionItem(1, "1 - Nhật ký chung (Khuyên dùng)"),
            new ConfigOptionItem(2, "2 - Chứng từ ghi sổ"),
            new ConfigOptionItem(3, "3 - Nhật ký - Sổ cái"),
            new ConfigOptionItem(4, "4 - Nhật ký chứng từ")
        };

        // 2. Phương pháp tính giá xuất kho vật tư (Material)
        public static readonly ConfigOptionItem[] MaterialMethods = new[]
        {
            new ConfigOptionItem(1, "1 - Bình quân gia quyền"),
            new ConfigOptionItem(2, "2 - Nhập trước, xuất trước (FIFO)"),
            new ConfigOptionItem(3, "3 - Giá thực tế đích danh")
        };
    }

    public class DbConfigDto
    {
        public int Id { get; set; } = 1;
        public string CompanyName { get; set; } = "";
        public string DepartmentName { get; set; } = "Phòng Kế Toán";
        public string Address { get; set; } = "";
        public string BankAccount { get; set; } = "";
        public string TaxCode { get; set; } = "";
        public int AccountingYear { get; set; } = DateTime.Today.Year;
        public string RegistrationCode { get; set; } = "";
        public bool IsCompanyLicense { get; set; } = false; // False = Theo máy; True = Theo thông tin đơn vị (MST)
        public string ExpiryDate { get; set; } = "20991231"; // YYYYMMDD (20991231 = Dài hạn / Vĩnh viễn)
        public short AccountMethod { get; set; } = 1; // 1 = Nhật ký chung, 2 = CT ghi sổ, ...
        public short Material { get; set; } = 1; // 1 = Bình quân, 2 = FIFO, 3 = Đích danh
        public int MaxRecords { get; set; } = 100; // Hạn mức bản ghi JournalVoucher (Mặc định dùng thử = 100)
        public string VoucherPostingConfig { get; set; } = ""; // Chuỗi JSON cấu hình định khoản ngầm định cho các loại phiếu
    }

    public class VoucherPostingConfigDto
    {
        public Dictionary<string, InventoryVoucherConfigItemDto> Inventory { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, CashVoucherConfigItemDto> Receipt { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, CashVoucherConfigItemDto> Payment { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }

    public class InventoryVoucherConfigItemDto
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public short Dbcr { get; set; } = 1;
        public bool Taxflag { get; set; } = false;
        public string TotalAccountId { get; set; } = "";
        public string TaxAccountId { get; set; } = "";
        public string CostAccountId { get; set; } = "";
        public string RevenueAccountId { get; set; } = "";
        public string PartnerType { get; set; } = "";
    }

    public class CashVoucherConfigItemDto
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string DebitAcc { get; set; } = "";
        public string CreditAcc { get; set; } = "";
        public string VatAcc { get; set; } = "";
        public string PartnerType { get; set; } = "";
    }

    public static class DbConfigService
    {
        // Khóa kiểm soát bí mật (Secret Salt) của tác giả Phạm Việt Cường
        public const string SecretSalt = "PVCUONG#NEWACC70@2026$KEY";
        public const string RecordTierSalt = "PVCUONG#NEWACC2026$RECORD_TIER";
        public const int RecordMaskVal = 0xA75C31;

        // Ma trận vị trí đan xen 6 ký tự số lượng và 10 ký tự chữ ký số MD5 (Đồng bộ 100% với Tool KeyGen & Google Apps Script)
        private static readonly int[] LimitIndices = new int[] { 2, 5, 8, 11, 13, 15 };
        private static readonly int[] HashIndices  = new int[] { 0, 1, 3, 4, 6, 7, 9, 10, 12, 14 };

        public static string LastSaveError { get; set; } = "";

        private static DbConfigDto? _cachedConfig;

        public static DbConfigDto FromEntity(DataAccess.EntityClasses.DbConfigEntity entity)
        {
            if (entity == null) return new DbConfigDto();
            return new DbConfigDto
            {
                Id = entity.Id,
                CompanyName = entity.CompanyName ?? "",
                DepartmentName = entity.DepartmentName ?? "",
                Address = entity.Address ?? "",
                BankAccount = entity.BankAccount ?? "",
                TaxCode = entity.TaxCode ?? "",
                AccountingYear = entity.AccountingYear ?? DateTime.Today.Year,
                RegistrationCode = entity.RegistrationCode ?? "",
                IsCompanyLicense = entity.IsCompanyLicense,
                ExpiryDate = entity.ExpiryDate ?? "20991231",
                AccountMethod = (short)(entity.AccountMethod ?? 1),
                Material = (short)(entity.Material ?? 1),
                VoucherPostingConfig = entity.VoucherPostingConfig ?? ""
            };
        }

        public static VoucherPostingConfigDto GetDefaultVoucherPostingConfig()
        {
            var cfg = new VoucherPostingConfigDto();
            cfg.Inventory["NM"] = new InventoryVoucherConfigItemDto
            {
                Code = "NM",
                Name = "Nhập mua",
                Dbcr = 1,
                Taxflag = true,
                TotalAccountId = "3311",
                TaxAccountId = "13311",
                CostAccountId = "6321",
                RevenueAccountId = "5111",
                PartnerType = "Vendor"
            };
            cfg.Inventory["XB"] = new InventoryVoucherConfigItemDto
            {
                Code = "XB",
                Name = "Xuất bán",
                Dbcr = -1,
                Taxflag = true,
                TotalAccountId = "131",
                TaxAccountId = "33311",
                CostAccountId = "6321",
                RevenueAccountId = "5111",
                PartnerType = "Customer"
            };
            cfg.Inventory["NNB"] = new InventoryVoucherConfigItemDto
            {
                Code = "NNB",
                Name = "Nhập nội bộ",
                Dbcr = 1,
                Taxflag = false,
                TotalAccountId = "154",
                TaxAccountId = "",
                CostAccountId = "",
                RevenueAccountId = "",
                PartnerType = "Internal"
            };
            cfg.Inventory["XNB"] = new InventoryVoucherConfigItemDto
            {
                Code = "XNB",
                Name = "Xuất nội bộ",
                Dbcr = -1,
                Taxflag = false,
                TotalAccountId = "642",
                TaxAccountId = "",
                CostAccountId = "",
                RevenueAccountId = "",
                PartnerType = "Department"
            };
            return cfg;
        }

        public static VoucherPostingConfigDto GetVoucherPostingConfig(this DbConfigDto cfg)
        {
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.VoucherPostingConfig))
            {
                return GetDefaultVoucherPostingConfig();
            }

            try
            {
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var res = System.Text.Json.JsonSerializer.Deserialize<VoucherPostingConfigDto>(cfg.VoucherPostingConfig, options);
                if (res != null && res.Inventory != null && res.Inventory.Count > 0)
                {
                    return res;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Deserialize VoucherPostingConfig error: " + ex.Message);
            }

            return GetDefaultVoucherPostingConfig();
        }

        public static void SetVoucherPostingConfig(this DbConfigDto cfg, VoucherPostingConfigDto postingCfg)
        {
            if (cfg == null || postingCfg == null) return;
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                cfg.VoucherPostingConfig = System.Text.Json.JsonSerializer.Serialize(postingCfg, options);
            }
            catch { }
        }

        public static DataAccess.EntityClasses.DbConfigEntity ToEntity(DbConfigDto dto)
        {
            return new DataAccess.EntityClasses.DbConfigEntity
            {
                Id = dto.Id,
                CompanyName = dto.CompanyName,
                DepartmentName = dto.DepartmentName,
                Address = dto.Address,
                BankAccount = dto.BankAccount,
                TaxCode = dto.TaxCode,
                AccountingYear = dto.AccountingYear,
                RegistrationCode = dto.RegistrationCode,
                IsCompanyLicense = dto.IsCompanyLicense,
                ExpiryDate = dto.ExpiryDate,
                AccountMethod = dto.AccountMethod,
                Material = dto.Material,
                VoucherPostingConfig = dto.VoucherPostingConfig ?? "",
                UpdatedDate = DateTime.Now
            };
        }

        public static DbConfigDto Current
        {
            get
            {
                if (_cachedConfig == null)
                {
                    _cachedConfig = LoadDbConfig();
                }
                return _cachedConfig;
            }
        }

        public static void ClearCache()
        {
            _cachedConfig = null;
        }

        /// <summary>
        /// Đảm bảo bảng DbConfig luôn tồn tại trong CSDL đang kết nối (Access, SQL Server, Postgres)
        /// </summary>
        public static void EnsureDbConfigTable()
        {
            try
            {
                if (ConfigManager.Current.DbType == DatabaseType.Access && 
                    (string.IsNullOrWhiteSpace(ConfigManager.Current.AccessFilePath) || !System.IO.File.Exists(ConfigManager.Current.AccessFilePath)))
                {
                    return;
                }

                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    bool tableExists = CheckTableExists(conn, "DbConfig");
                    if (!tableExists)
                    {
                        CreateTable(conn);
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        // Đảm bảo có dòng Id = 1
                        EnsureDefaultRowExists(conn);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EnsureDbConfigTable error: " + ex.Message);
            }
        }

        private static bool CheckTableExists(DbConnection conn, string tableName)
        {
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    if (ConfigManager.Current.DbType == DatabaseType.PostgreSql)
                    {
                        cmd.CommandText = $@"SELECT COUNT(*) FROM ""{tableName}""";
                    }
                    else
                    {
                        cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                    }
                    cmd.ExecuteScalar();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void CreateTable(DbConnection conn)
        {
            string dbType = ConfigManager.Current.DbType.ToString();
            string sql;

            if (dbType == "SqlServer")
            {
                sql = @"
CREATE TABLE DbConfig (
    Id INT PRIMARY KEY,
    CompanyName NVARCHAR(255),
    DepartmentName NVARCHAR(150),
    Address NVARCHAR(255),
    BankAccount NVARCHAR(50),
    TaxCode NVARCHAR(50),
    AccountingYear INT,
    RegistrationCode NVARCHAR(100),
    IsCompanyLicense BIT,
    ExpiryDate NVARCHAR(10),
    AccountMethod SMALLINT DEFAULT 1,
    Material SMALLINT DEFAULT 1,
    VoucherPostingConfig NVARCHAR(MAX),
    CreatedDate DATETIME,
    UpdatedDate DATETIME
)";
            }
            else if (dbType == "PostgreSql")
            {
                sql = @"
CREATE TABLE IF NOT EXISTS ""DbConfig"" (
    ""Id"" INT PRIMARY KEY,
    ""CompanyName"" VARCHAR(255),
    ""DepartmentName"" VARCHAR(150),
    ""Address"" VARCHAR(255),
    ""BankAccount"" VARCHAR(50),
    ""TaxCode"" VARCHAR(50),
    ""AccountingYear"" INT,
    ""RegistrationCode"" VARCHAR(100),
    ""IsCompanyLicense"" BOOLEAN,
    ""ExpiryDate"" VARCHAR(10),
    ""AccountMethod"" SMALLINT DEFAULT 1,
    ""Material"" SMALLINT DEFAULT 1,
    ""VoucherPostingConfig"" TEXT,
    ""CreatedDate"" TIMESTAMP,
    ""UpdatedDate"" TIMESTAMP
)";
            }
            else // Access
            {
                sql = @"
CREATE TABLE DbConfig (
    Id INT PRIMARY KEY,
    CompanyName VARCHAR(255),
    DepartmentName VARCHAR(150),
    Address VARCHAR(255),
    BankAccount VARCHAR(50),
    TaxCode VARCHAR(50),
    AccountingYear INT,
    RegistrationCode VARCHAR(100),
    IsCompanyLicense YESNO,
    ExpiryDate VARCHAR(10),
    AccountMethod INTEGER,
    Material INTEGER,
    VoucherPostingConfig LONGTEXT,
    CreatedDate DATETIME,
    UpdatedDate DATETIME
)";
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private static void InsertDefaultRow(DbConnection conn)
        {
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                        ? @"INSERT INTO ""DbConfig"" (""Id"", ""CompanyName"", ""DepartmentName"", ""Address"", ""BankAccount"", ""TaxCode"", ""AccountingYear"", ""RegistrationCode"", ""IsCompanyLicense"", ""ExpiryDate"", ""AccountMethod"", ""Material"")
                            VALUES (1, '', 'Phòng Kế Toán', '', '', '', " + DateTime.Today.Year + @", '', false, '20991231', 1, 1)"
                        : @"INSERT INTO DbConfig (Id, CompanyName, DepartmentName, Address, BankAccount, TaxCode, AccountingYear, RegistrationCode, IsCompanyLicense, ExpiryDate, AccountMethod, Material)
                            VALUES (1, '', 'Phòng Kế Toán', '', '', '', " + DateTime.Today.Year + @", '', 0, '20991231', 1, 1)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private static void EnsureDefaultRowExists(DbConnection conn)
        {
            try
            {
                // Đảm bảo các cột mới AccountMethod và Material tồn tại nếu bảng đã tạo từ trước
                EnsureColumnsExist(conn);

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                        ? @"SELECT COUNT(*) FROM ""DbConfig"""
                        : "SELECT COUNT(*) FROM DbConfig";
                    var obj = cmd.ExecuteScalar();
                    int count = obj != null && int.TryParse(obj.ToString(), out int c) ? c : 0;
                    if (count == 0)
                    {
                        InsertDefaultRow(conn);
                    }
                }
            }
            catch { }
        }

        private static void EnsureColumnsExist(DbConnection conn)
        {
            string dbType = ConfigManager.Current.DbType.ToString();
            string[] alterStatements;

            if (dbType == "SqlServer")
            {
                alterStatements = new[]
                {
                    "IF COL_LENGTH('DbConfig', 'AccountMethod') IS NULL ALTER TABLE DbConfig ADD AccountMethod SMALLINT DEFAULT 1;",
                    "IF COL_LENGTH('DbConfig', 'Material') IS NULL ALTER TABLE DbConfig ADD Material SMALLINT DEFAULT 1;",
                    "IF COL_LENGTH('DbConfig', 'VoucherPostingConfig') IS NULL ALTER TABLE DbConfig ADD VoucherPostingConfig NVARCHAR(MAX);"
                };
            }
            else if (dbType == "PostgreSql")
            {
                alterStatements = new[]
                {
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"AccountMethod\" SMALLINT DEFAULT 1;",
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"Material\" SMALLINT DEFAULT 1;",
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"VoucherPostingConfig\" TEXT;"
                };
            }
            else // Access
            {
                alterStatements = new[]
                {
                    "ALTER TABLE DbConfig ADD COLUMN AccountMethod INTEGER;",
                    "ALTER TABLE DbConfig ADD COLUMN Material INTEGER;",
                    "ALTER TABLE DbConfig ADD COLUMN VoucherPostingConfig LONGTEXT;"
                };
            }

            foreach (var sql in alterStatements)
            {
                try
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }

        public static DbConfigDto LoadDbConfig()
        {
            var dto = new DbConfigDto();
            try
            {
                if (ConfigManager.Current.DbType == DatabaseType.Access && 
                    (string.IsNullOrWhiteSpace(ConfigManager.Current.AccessFilePath) || !System.IO.File.Exists(ConfigManager.Current.AccessFilePath)))
                {
                    return dto;
                }

                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    if (!CheckTableExists(conn, "DbConfig"))
                    {
                        CreateTable(conn);
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        EnsureDefaultRowExists(conn);
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                            ? @"SELECT * FROM ""DbConfig"" ORDER BY ""Id"" ASC LIMIT 1"
                            : "SELECT TOP 1 * FROM DbConfig ORDER BY Id ASC";

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto.Id = HasColumn(reader, "Id") && reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 1;
                                dto.CompanyName = HasColumn(reader, "CompanyName") ? (reader["CompanyName"]?.ToString() ?? "") : "";
                                dto.DepartmentName = HasColumn(reader, "DepartmentName") ? (reader["DepartmentName"]?.ToString() ?? "") : "";
                                dto.Address = HasColumn(reader, "Address") ? (reader["Address"]?.ToString() ?? "") : "";
                                dto.BankAccount = HasColumn(reader, "BankAccount") ? (reader["BankAccount"]?.ToString() ?? "") : "";
                                dto.TaxCode = HasColumn(reader, "TaxCode") ? (reader["TaxCode"]?.ToString() ?? "") : "";
                                dto.AccountingYear = HasColumn(reader, "AccountingYear") && reader["AccountingYear"] != DBNull.Value ? Convert.ToInt32(reader["AccountingYear"]) : DateTime.Today.Year;
                                dto.RegistrationCode = HasColumn(reader, "RegistrationCode") ? (reader["RegistrationCode"]?.ToString() ?? "") : "";
                                dto.IsCompanyLicense = HasColumn(reader, "IsCompanyLicense") && reader["IsCompanyLicense"] != DBNull.Value && Convert.ToBoolean(reader["IsCompanyLicense"]);
                                dto.ExpiryDate = HasColumn(reader, "ExpiryDate") ? (reader["ExpiryDate"]?.ToString() ?? "20991231") : "20991231";
                                dto.AccountMethod = HasColumn(reader, "AccountMethod") && reader["AccountMethod"] != DBNull.Value ? Convert.ToInt16(reader["AccountMethod"]) : (short)1;
                                dto.Material = HasColumn(reader, "Material") && reader["Material"] != DBNull.Value ? Convert.ToInt16(reader["Material"]) : (short)1;
                                dto.VoucherPostingConfig = HasColumn(reader, "VoucherPostingConfig") ? (reader["VoucherPostingConfig"]?.ToString() ?? "") : "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadDbConfig error: " + ex.Message);
            }

            try
            {
                string machineCode = License.HardwareHelper.GetMachineCode();
                ValidateRegistrationCode(dto, machineCode, out int maxRecords);
                dto.MaxRecords = maxRecords;
            }
            catch { }

            return dto;
        }

        private static bool HasColumn(IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool SaveDbConfig(DbConfigDto dto)
        {
            LastSaveError = "";
            try
            {
                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    if (!CheckTableExists(conn, "DbConfig"))
                    {
                        CreateTable(conn);
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        EnsureDefaultRowExists(conn);
                    }

                    int existingCount = 0;
                    using (var checkCmd = conn.CreateCommand())
                    {
                        checkCmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                            ? @"SELECT COUNT(*) FROM ""DbConfig"""
                            : "SELECT COUNT(*) FROM DbConfig";
                        var res = checkCmd.ExecuteScalar();
                        if (res != null && int.TryParse(res.ToString(), out int c)) existingCount = c;
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        var dbType = ConfigManager.Current.DbType;

                        if (dbType == DatabaseType.Access)
                        {
                            if (existingCount == 0)
                            {
                                cmd.CommandText = @"INSERT INTO [DbConfig] ([Id], [CompanyName], [DepartmentName], [Address], [BankAccount], [TaxCode], [AccountingYear], [RegistrationCode], [IsCompanyLicense], [ExpiryDate], [AccountMethod], [Material], [VoucherPostingConfig], [CreatedDate], [UpdatedDate])
                                    VALUES (1, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, Now(), Now())";
                            }
                            else
                            {
                                cmd.CommandText = @"UPDATE [DbConfig] 
                                    SET [CompanyName] = ?,
                                        [DepartmentName] = ?,
                                        [Address] = ?,
                                        [BankAccount] = ?,
                                        [TaxCode] = ?,
                                        [AccountingYear] = ?,
                                        [RegistrationCode] = ?,
                                        [IsCompanyLicense] = ?,
                                        [ExpiryDate] = ?,
                                        [AccountMethod] = ?,
                                        [Material] = ?,
                                        [VoucherPostingConfig] = ?,
                                        [UpdatedDate] = Now()
                                    WHERE [Id] = 1";
                            }
                            AddParam(cmd, "?", dto.CompanyName ?? "");
                            AddParam(cmd, "?", dto.DepartmentName ?? "");
                            AddParam(cmd, "?", dto.Address ?? "");
                            AddParam(cmd, "?", dto.BankAccount ?? "");
                            AddParam(cmd, "?", dto.TaxCode ?? "");
                            AddParam(cmd, "?", dto.AccountingYear);
                            AddParam(cmd, "?", dto.RegistrationCode ?? "");
                            AddParam(cmd, "?", dto.IsCompanyLicense);
                            AddParam(cmd, "?", dto.ExpiryDate ?? "20991231");
                            AddParam(cmd, "?", (int)dto.AccountMethod);
                            AddParam(cmd, "?", (int)dto.Material);
                            AddParam(cmd, "?", dto.VoucherPostingConfig ?? "");
                        }
                        else if (dbType == DatabaseType.PostgreSql)
                        {
                            if (existingCount == 0)
                            {
                                cmd.CommandText = @"INSERT INTO ""DbConfig"" (""Id"", ""CompanyName"", ""DepartmentName"", ""Address"", ""BankAccount"", ""TaxCode"", ""AccountingYear"", ""RegistrationCode"", ""IsCompanyLicense"", ""ExpiryDate"", ""AccountMethod"", ""Material"", ""VoucherPostingConfig"", ""CreatedDate"", ""UpdatedDate"")
                                    VALUES (1, @CompanyName, @DepartmentName, @Address, @BankAccount, @TaxCode, @AccountingYear, @RegistrationCode, @IsCompanyLicense, @ExpiryDate, @AccountMethod, @Material, @VoucherPostingConfig, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                            }
                            else
                            {
                                cmd.CommandText = @"UPDATE ""DbConfig"" 
                                    SET ""CompanyName"" = @CompanyName,
                                        ""DepartmentName"" = @DepartmentName,
                                        ""Address"" = @Address,
                                        ""BankAccount"" = @BankAccount,
                                        ""TaxCode"" = @TaxCode,
                                        ""AccountingYear"" = @AccountingYear,
                                        ""RegistrationCode"" = @RegistrationCode,
                                        ""IsCompanyLicense"" = @IsCompanyLicense,
                                        ""ExpiryDate"" = @ExpiryDate,
                                        ""AccountMethod"" = @AccountMethod,
                                        ""Material"" = @Material,
                                        ""VoucherPostingConfig"" = @VoucherPostingConfig,
                                        ""UpdatedDate"" = CURRENT_TIMESTAMP
                                    WHERE ""Id"" = 1";
                            }
                            AddParam(cmd, "@CompanyName", dto.CompanyName ?? "");
                            AddParam(cmd, "@DepartmentName", dto.DepartmentName ?? "");
                            AddParam(cmd, "@Address", dto.Address ?? "");
                            AddParam(cmd, "@BankAccount", dto.BankAccount ?? "");
                            AddParam(cmd, "@TaxCode", dto.TaxCode ?? "");
                            AddParam(cmd, "@AccountingYear", dto.AccountingYear);
                            AddParam(cmd, "@RegistrationCode", dto.RegistrationCode ?? "");
                            AddParam(cmd, "@IsCompanyLicense", dto.IsCompanyLicense);
                            AddParam(cmd, "@ExpiryDate", dto.ExpiryDate ?? "20991231");
                            AddParam(cmd, "@AccountMethod", dto.AccountMethod);
                            AddParam(cmd, "@Material", dto.Material);
                            AddParam(cmd, "@VoucherPostingConfig", dto.VoucherPostingConfig ?? "");
                        }
                        else // SQL Server
                        {
                            if (existingCount == 0)
                            {
                                cmd.CommandText = @"INSERT INTO DbConfig (Id, CompanyName, DepartmentName, Address, BankAccount, TaxCode, AccountingYear, RegistrationCode, IsCompanyLicense, ExpiryDate, AccountMethod, Material, VoucherPostingConfig, CreatedDate, UpdatedDate)
                                    VALUES (1, @CompanyName, @DepartmentName, @Address, @BankAccount, @TaxCode, @AccountingYear, @RegistrationCode, @IsCompanyLicense, @ExpiryDate, @AccountMethod, @Material, @VoucherPostingConfig, GETDATE(), GETDATE())";
                            }
                            else
                            {
                                cmd.CommandText = @"UPDATE DbConfig 
                                    SET CompanyName = @CompanyName,
                                        DepartmentName = @DepartmentName,
                                        Address = @Address,
                                        BankAccount = @BankAccount,
                                        TaxCode = @TaxCode,
                                        AccountingYear = @AccountingYear,
                                        RegistrationCode = @RegistrationCode,
                                        IsCompanyLicense = @IsCompanyLicense,
                                        ExpiryDate = @ExpiryDate,
                                        AccountMethod = @AccountMethod,
                                        Material = @Material,
                                        VoucherPostingConfig = @VoucherPostingConfig,
                                        UpdatedDate = GETDATE()
                                    WHERE Id = 1";
                            }
                            AddParam(cmd, "@CompanyName", dto.CompanyName ?? "");
                            AddParam(cmd, "@DepartmentName", dto.DepartmentName ?? "");
                            AddParam(cmd, "@Address", dto.Address ?? "");
                            AddParam(cmd, "@BankAccount", dto.BankAccount ?? "");
                            AddParam(cmd, "@TaxCode", dto.TaxCode ?? "");
                            AddParam(cmd, "@AccountingYear", dto.AccountingYear);
                            AddParam(cmd, "@RegistrationCode", dto.RegistrationCode ?? "");
                            AddParam(cmd, "@IsCompanyLicense", dto.IsCompanyLicense);
                            AddParam(cmd, "@ExpiryDate", dto.ExpiryDate ?? "20991231");
                            AddParam(cmd, "@AccountMethod", dto.AccountMethod);
                            AddParam(cmd, "@Material", dto.Material);
                            AddParam(cmd, "@VoucherPostingConfig", dto.VoucherPostingConfig ?? "");
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                string machineCode = License.HardwareHelper.GetMachineCode();
                ValidateRegistrationCode(dto, machineCode, out int maxRecords);
                dto.MaxRecords = maxRecords;

                _cachedConfig = dto;
                return true;
            }
            catch (Exception ex)
            {
                LastSaveError = ex.Message;
                if (ex.InnerException != null) LastSaveError += $" ({ex.InnerException.Message})";
                System.Diagnostics.Debug.WriteLine("SaveDbConfig error: " + LastSaveError);
                return false;
            }
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private const ushort DateMask = 0x7A6C;

        /// <summary>
        /// Mã hóa ngày hết hạn YYYYMMDD thành cụm 4 ký tự hex bảo mật
        /// </summary>
        public static string EncodeDate(string expiryDate)
        {
            if (string.IsNullOrWhiteSpace(expiryDate) || expiryDate.Length != 8)
                expiryDate = "20991231";

            try
            {
                int y = int.Parse(expiryDate.Substring(0, 4)) - 2020;
                int m = int.Parse(expiryDate.Substring(4, 2));
                int d = int.Parse(expiryDate.Substring(6, 2));
                int val = ((y & 0x7F) << 9) | ((m & 0x0F) << 5) | (d & 0x1F);
                int masked = val ^ DateMask;
                return masked.ToString("X4");
            }
            catch
            {
                return "E5F3"; // 20991231
            }
        }

        /// <summary>
        /// Giải mã ngày hết hạn từ cụm 4 ký tự hex
        /// </summary>
        public static bool TryDecodeDate(string dateHex, out string expiryDateStr, out DateTime expirationDate)
        {
            expiryDateStr = "20991231";
            expirationDate = new DateTime(2099, 12, 31);
            if (string.IsNullOrWhiteSpace(dateHex) || dateHex.Length != 4)
                return false;

            try
            {
                int masked = Convert.ToUInt16(dateHex, 16);
                int val = masked ^ DateMask;
                int d = val & 0x1F;
                int m = (val >> 5) & 0x0F;
                int y = ((val >> 9) & 0x7F) + 2020;

                if (y < 2020 || y > 2120 || m < 1 || m > 12 || d < 1 || d > DateTime.DaysInMonth(y, m))
                    return false;

                expirationDate = new DateTime(y, m, d);
                expiryDateStr = $"{y:D4}{m:D2}{d:D2}";
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thuật toán tạo Số đăng ký ngắn gọn (12 ký tự: XXXX-XXXX-XXXX)
        /// Cụm 1: Nhúng ngày hết hạn mã hóa
        /// Cụm 2 & 3: Chữ ký cryptographic băm MD5 với SecretSalt
        /// </summary>
        public static string GenerateRegistrationCode(string targetId, string expiryDate = "20991231")
        {
            targetId = (targetId ?? "").Trim().ToUpperInvariant();
            expiryDate = (expiryDate ?? "20991231").Trim();
            if (expiryDate.Length != 8) expiryDate = "20991231";

            string dateHex = EncodeDate(expiryDate);
            string raw = $"{targetId}#{expiryDate}#{SecretSalt}";

            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
                string hex = Convert.ToHexString(hash).ToUpperInvariant();

                string p1 = hex.Substring(0, 4);
                string p2 = hex.Substring(8, 4);

                return $"{dateHex}-{p1}-{p2}";
            }
        }

        /// <summary>
        /// Giải mã và xác thực Mã đăng ký 16 ký tự phân bổ hạn mức bản ghi JournalVoucher (Khớp 100% với Tool KeyGen & Google Apps Script)
        /// </summary>
        public static bool DecodeRecordLimit(string machineCode, string code, out int limit)
        {
            limit = 0;
            if (string.IsNullOrWhiteSpace(code)) return false;

            machineCode = (machineCode ?? "").Replace("-", "").Trim().ToUpperInvariant();
            string clean = code.Replace("-", "").Trim().ToUpperInvariant();
            if (clean.Length != 16) return false;

            // 1. Bóc tách 6 ký tự limit
            char[] limitChars = new char[6];
            for (int i = 0; i < LimitIndices.Length; i++)
            {
                limitChars[i] = clean[LimitIndices[i]];
            }

            try
            {
                int masked = Convert.ToInt32(new string(limitChars), 16);
                limit = (masked ^ RecordMaskVal) & 0xFFFFFF;
            }
            catch
            {
                return false;
            }

            // 2. Bóc tách 10 ký tự hash
            char[] hashChars = new char[10];
            for (int i = 0; i < HashIndices.Length; i++)
            {
                hashChars[i] = clean[HashIndices[i]];
            }
            string extractedHash = new string(hashChars);

            // 3. Tính chữ ký kỳ vọng
            string raw = $"{machineCode}#{limit}#{RecordTierSalt}";
            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            string expectedHash = Convert.ToHexString(hashBytes).ToUpperInvariant().Substring(0, 10);

            return string.Equals(extractedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Số đăng ký và tự động bóc tách hạn mức số bản ghi JournalVoucher
        /// </summary>
        public static bool ValidateRegistrationCode(DbConfigDto config, string machineCode, out int maxRecords)
        {
            maxRecords = 100; // Mặc định dùng thử 100 bản ghi
            if (config == null || string.IsNullOrWhiteSpace(config.RegistrationCode))
                return false;

            string reg = config.RegistrationCode.Trim().ToUpperInvariant();
            string clean = reg.Replace("-", "");

            // 1. Mã mới 16 ký tự (XXXX-YYYY-ZZZZ-WWWW) phân bổ số lượng bản ghi JournalVoucher
            if (clean.Length == 16)
            {
                if (DecodeRecordLimit(machineCode, reg, out int limit))
                {
                    maxRecords = limit;
                    config.MaxRecords = limit;
                    config.ExpiryDate = "20991231";
                    return true;
                }
            }

            // 2. Tương thích ngược: Mã cũ 12 ký tự (XXXX-XXXX-XXXX) nhúng ngày hết hạn
            var parts = reg.Split('-');
            if (parts.Length == 3 && parts[0].Length == 4 && parts[1].Length == 4 && parts[2].Length == 4)
            {
                if (TryDecodeDate(parts[0], out string decodedExpStr, out DateTime decodedExpDate))
                {
                    string expectedCode = GenerateRegistrationCode(machineCode, decodedExpStr);
                    if (string.Equals(reg, expectedCode, StringComparison.OrdinalIgnoreCase))
                    {
                        maxRecords = 999999; // Không giới hạn bản ghi, chỉ ràng buộc ngày
                        config.MaxRecords = 999999;
                        config.ExpiryDate = decodedExpStr;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Quá tải kiểm tra tính hợp lệ theo thời hạn ngày (Tương thích ngược)
        /// </summary>
        public static bool ValidateRegistrationCode(DbConfigDto config, string machineCode, out DateTime? expirationDate)
        {
            expirationDate = null;
            if (ValidateRegistrationCode(config, machineCode, out int maxRecords))
            {
                if (DateTime.TryParseExact(config.ExpiryDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime exp))
                {
                    expirationDate = exp;
                }
                else
                {
                    expirationDate = new DateTime(2099, 12, 31);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra quy tắc hạn dùng chứng từ: Không được nhập quá ngày hết hạn bản quyền (nếu có thiết lập)
        /// </summary>
        public static bool CanUpdateVoucher(DateTime voucherDate, out string warningMessage)
        {
            warningMessage = "";
            var cfg = Current;
            string machineCode = License.HardwareHelper.GetMachineCode();

            bool isValid = ValidateRegistrationCode(cfg, machineCode, out int maxRecords);

            if (isValid && DateTime.TryParseExact(cfg.ExpiryDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime expDate))
            {
                if (expDate.Year < 2090 && voucherDate.Date > expDate.Date)
                {
                    warningMessage = $"Phiên bản có thời hạn sử dụng đến ngày {expDate:dd/MM/yyyy}.\nBạn không thể cập nhật các chứng từ có ngày ({voucherDate:dd/MM/yyyy}) lớn hơn thời điểm trên!\n\nVui lòng gia hạn bản quyền để tiếp tục làm việc.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Lấy tổng số lượng chứng từ thực tế trong bảng JournalVoucher (truy vấn trực tiếp CSDL bảo đảm chính xác 100% trên cả Access, SQL Server, PostgreSQL)
        /// </summary>
        public static int GetJournalVoucherCount()
        {
            // 1. Thử truy vấn trực tiếp bằng Raw SQL (tối ưu tốc độ, tương thích hoàn hảo mọi hệ quản trị CSDL)
            try
            {
                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        var dbType = ConfigManager.Current.DbType;
                        cmd.CommandText = dbType == DatabaseType.PostgreSql
                            ? @"SELECT COUNT(*) FROM ""JournalVoucher"""
                            : "SELECT COUNT(*) FROM [JournalVoucher]";
                        var res = cmd.ExecuteScalar();
                        if (res != null && int.TryParse(res.ToString(), out int cnt))
                        {
                            return cnt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetJournalVoucherCount raw sql error: " + ex.Message);
            }

            // 2. Dự phòng bằng LLBLGen GetDbCount
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var col = new DataAccess.HelperClasses.EntityCollection<DataAccess.EntityClasses.JournalVoucherEntity>();
                    return adapter.GetDbCount(col, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetJournalVoucherCount LLBLGen GetDbCount error: " + ex.Message);
            }

            // 3. Dự phòng bằng LLBLGen LinqMetaData
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var metaData = new DataAccess.Linq.LinqMetaData(adapter);
                    return metaData.JournalVoucher.Count();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetJournalVoucherCount Linq error: " + ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// Kiểm tra hạn mức số lượng chứng từ thực tế trong bảng JournalVoucher
        /// </summary>
        public static bool CheckRecordLimit(out int currentCount, out int maxLimit, out string warningMessage, int additionalRecords = 0)
        {
            warningMessage = "";
            var cfg = Current;
            string machineCode = License.HardwareHelper.GetMachineCode();
            bool isValid = ValidateRegistrationCode(cfg, machineCode, out int limit);
            maxLimit = isValid ? limit : 100;

            currentCount = GetJournalVoucherCount();

            if (maxLimit < 999999 && (currentCount + additionalRecords) >= maxLimit)
            {
                string tierDesc = isValid ? $"gói bản quyền ({maxLimit:N0} chứng từ)" : $"bản Dùng Thử miễn phí ({maxLimit} chứng từ)";
                if (currentCount >= maxLimit)
                {
                    warningMessage = $"Số lượng chứng từ hiện tại trong CSDL ({currentCount:N0}) đã đạt hoặc vượt mức cho phép của {tierDesc}.\n\nVui lòng nâng cấp gói bản quyền để tiếp tục nhập thêm chứng từ mới.";
                }
                else
                {
                    warningMessage = $"Số lượng chứng từ hiện tại ({currentCount:N0}) cùng với các chứng từ mới ({additionalRecords}) sẽ vượt quá giới hạn của {tierDesc}.\n\nVui lòng nâng cấp gói bản quyền để lưu thêm chứng từ mới.";
                }
                return false;
            }

            return true;
        }
    }
}

