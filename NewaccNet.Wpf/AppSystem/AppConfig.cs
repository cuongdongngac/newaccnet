namespace NewaccNet.Wpf.AppSystem
{
    public enum DatabaseType
    {
        Access,
        SqlServer,
        PostgreSql
    }

    public class AppConfig
    {
        public DatabaseType DbType { get; set; } = DatabaseType.Access;
        public string ConnectionString { get; set; } = "";

        // ── Access ───────────────────────────────────────────
        public string AccessFilePath { get; set; } = "";

        // ── SQL Server ───────────────────────────────────────
        public string SqlServerName { get; set; } = "";
        public string SqlDatabaseName { get; set; } = "";
        public string SqlUsername { get; set; } = "";
        public string SqlPassword { get; set; } = "";
        public bool SqlIntegratedSecurity { get; set; } = true;

        // ── PostgreSQL ───────────────────────────────────────
        public string PgHost { get; set; } = "localhost";
        public int PgPort { get; set; } = 5432;
        public string PgDatabase { get; set; } = "";
        public string PgUsername { get; set; } = "postgres";
        public string PgPassword { get; set; } = "";
    }
}


