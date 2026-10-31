using System;
using System.IO;
using System.Text.Json;

namespace NewaccNet.Core.Data
{
    /// <summary>
    /// Lớp quản lý cấu hình dùng chung, lưu trữ tại %APPDATA%\NewaccNet\appsettings.json
    /// để đảm bảo quyền ghi và an toàn dữ liệu trên mọi phiên bản Windows.
    /// </summary>
    public static class ConfigManager
    {
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NewaccNet");

        public static readonly string ConfigFilePath = Path.Combine(AppDataFolder, "appsettings.json");
        private static readonly string LegacyConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        public static AppConfig Current { get; private set; } = new AppConfig();

        public static bool HasConfigFile()
        {
            return File.Exists(ConfigFilePath) || File.Exists(LegacyConfigFilePath);
        }

        public static void LoadConfig()
        {
            try
            {
                System.IO.Directory.CreateDirectory(AppDataFolder);

                string targetPath = ConfigFilePath;
                // Nếu chưa có trong AppData nhưng có trong thư mục cài đặt cũ -> Tự động migrate sang AppData
                if (!File.Exists(ConfigFilePath) && File.Exists(LegacyConfigFilePath))
                {
                    try
                    {
                        File.Copy(LegacyConfigFilePath, ConfigFilePath, overwrite: true);
                    }
                    catch { }
                    targetPath = File.Exists(ConfigFilePath) ? ConfigFilePath : LegacyConfigFilePath;
                }

                if (File.Exists(targetPath))
                {
                    string json = File.ReadAllText(targetPath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    if (config != null)
                    {
                        Current = config;
                        Staging.DraftStoreFactory.ConfiguredMode = Current.StagingStorageMode;
                    }
                }
                else
                {
                    Staging.DraftStoreFactory.ConfiguredMode = Current.StagingStorageMode;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi đọc file (nếu cần)
                System.Diagnostics.Debug.WriteLine("Load config error: " + ex.Message);
            }
        }

        public static void SaveConfig()
        {
            try
            {
                Staging.DraftStoreFactory.ConfiguredMode = Current.StagingStorageMode;
                System.IO.Directory.CreateDirectory(AppDataFolder);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Current, options);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ghi file (nếu cần)
                System.Diagnostics.Debug.WriteLine("Save config error: " + ex.Message);
            }
        }
        
        // Sinh ConnectionString từ các thành phần chi tiết
        public static void BuildConnectionString()
        {
            if (Current.DbType == DatabaseType.Access)
            {
                Current.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={Current.AccessFilePath};Persist Security Info=False;";
            }
            else if (Current.DbType == DatabaseType.SqlServer)
            {
                if (Current.SqlIntegratedSecurity)
                {
                    Current.ConnectionString = $"data source={Current.SqlServerName};initial catalog={Current.SqlDatabaseName};Integrated Security=SSPI;TrustServerCertificate=True;";
                }
                else
                {
                    Current.ConnectionString = $"data source={Current.SqlServerName};initial catalog={Current.SqlDatabaseName};User ID={Current.SqlUsername};Password={Current.SqlPassword};TrustServerCertificate=True;";
                }
            }
            else if (Current.DbType == DatabaseType.PostgreSql)
            {
                Current.ConnectionString = $"Host={Current.PgHost};Port={Current.PgPort};Database={Current.PgDatabase};Username={Current.PgUsername};Password={Current.PgPassword};";
            }
        }
    }
}

