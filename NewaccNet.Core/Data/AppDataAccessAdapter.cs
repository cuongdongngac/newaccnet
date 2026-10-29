// extern alias SqlAdapterAlias;
// extern alias AccessAdapterAlias;

using System;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Core.Data
{
    public static class AppDataAccessAdapter
    {
        public static IDataAccessAdapter Create()
        {
            ConfigManager.BuildConnectionString();
            return CreateCustom(ConfigManager.Current.ConnectionString, ConfigManager.Current.DbType);
        }

        public static IDataAccessAdapter CreateCustom(string conn, DatabaseType dbType)
        {
            try
            {
                if (dbType == DatabaseType.SqlServer)
                {
                    if (string.IsNullOrWhiteSpace(conn)) conn = @"Server=.\SQLEXPRESS;Database=Greeneast;Integrated Security=SSPI;TrustServerCertificate=True;";

                    System.Data.Common.DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", Microsoft.Data.SqlClient.SqlClientFactory.Instance);
                    SD.LLBLGen.Pro.ORMSupportClasses.RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.SqlServer.SQLServerDQEConfiguration>(
                        c => c.AddDbProviderFactory(typeof(Microsoft.Data.SqlClient.SqlClientFactory)));

                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(conn);
                    string dbName = builder.InitialCatalog;
                    var sqlAdapter = new DataAccess.SqlServer.DatabaseSpecific.DataAccessAdapter(conn, false, CatalogNameUsage.ForceName, dbName);
                    return sqlAdapter;
                }
                else if (dbType == DatabaseType.PostgreSql)
                {
                    if (string.IsNullOrWhiteSpace(conn)) conn = "Host=localhost;Port=5432;Database=Greeneast;Username=postgres;Password=;";

                    System.Data.Common.DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
                    SD.LLBLGen.Pro.ORMSupportClasses.RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.PostgreSql.PostgreSqlDQEConfiguration>(
                        c => c.AddDbProviderFactory(typeof(Npgsql.NpgsqlFactory)));

                    var pgAdapter = new DataAccess.PostgreSql.DatabaseSpecific.DataAccessAdapter(conn);
                    return pgAdapter;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(conn)) conn = @"Provider=Microsoft.ACE.OLEDB.16.0;Data Source=d:\WpfDemo\data\Greeneast.accdb";

                    System.Data.Common.DbProviderFactories.RegisterFactory("System.Data.OleDb", System.Data.OleDb.OleDbFactory.Instance);
                    SD.LLBLGen.Pro.ORMSupportClasses.RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.Access.AccessDQEConfiguration>(
                        c => c.AddDbProviderFactory(typeof(System.Data.OleDb.OleDbFactory)));

                    var accessAdapter = new DataAccess.MsAccess.DatabaseSpecific.DataAccessAdapter(conn);
                    return accessAdapter;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cấu hình DQE nội bộ: {ex.Message}");
                throw;
            }
        }

        public static System.Data.Common.DbConnection CreateRawConnection()
        {
            var cfg = ConfigManager.Current;
            ConfigManager.BuildConnectionString();
            if (cfg.DbType == DatabaseType.SqlServer)
            {
                return new Microsoft.Data.SqlClient.SqlConnection(cfg.ConnectionString);
            }
            else if (cfg.DbType == DatabaseType.PostgreSql)
            {
                return new Npgsql.NpgsqlConnection(cfg.ConnectionString);
            }
            else
            {
                return new System.Data.OleDb.OleDbConnection(cfg.ConnectionString);
            }
        }
    }
}

