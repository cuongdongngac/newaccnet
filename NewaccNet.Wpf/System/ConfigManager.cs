using System;
using System.IO;
using System.Text.Json;

namespace NewaccNet.Wpf.AppSystem
{
    /// <summary>
    /// Lớp quản lý cấu hình dùng chung, có thể tái sử dụng ở dự án khác.
    /// </summary>
    public static class ConfigManager
    {
        private static readonly string ConfigFilePath = "appsettings.json";
        public static AppConfig Current { get; private set; } = new AppConfig();

        public static void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    if (config != null)
                    {
                        Current = config;
                    }
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
                Current.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Current.AccessFilePath}";
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


