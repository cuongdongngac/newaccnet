using System.Collections.Generic;

namespace NewaccNet.Contracts.Plugins
{
    /// <summary>
    /// Plugin có bảng dữ liệu riêng thì PHẢI implement.
    /// </summary>
    public interface IPluginSchema
    {
        string PluginKey { get; }              // 2–12 ký tự [A-Za-z0-9], dùng làm tiền tố Plg_<Key>_
        int SchemaVersion { get; }             // tăng mỗi khi đổi bảng
        IReadOnlyList<PluginTableDef> Tables { get; }
        /// <summary>
        /// Script nâng cấp từ version n-1 lên n, cho từng phương ngữ.
        /// </summary>
        IReadOnlyList<PluginMigration> Migrations { get; }
    }

    public sealed record PluginTableDef(string TableName, string Description);   // TableName bắt đầu bằng Plg_<Key>_

    public sealed record PluginMigration(int ToVersion, string AccessSql, string SqlServerSql, string PostgreSqlSql);
}
