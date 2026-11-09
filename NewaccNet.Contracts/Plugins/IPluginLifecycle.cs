using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.Contracts.Plugins
{
    /// <summary>
    /// Vòng đời — plugin tự làm phần của nó, Core điều phối.
    /// </summary>
    public interface IPluginLifecycle
    {
        Task InstallAsync(IPluginInstallContext ctx, CancellationToken ct);          // tạo bảng, dữ liệu mặc định
        Task UpgradeAsync(IPluginInstallContext ctx, int fromVersion, CancellationToken ct);
        Task<PluginUninstallCheck> CheckUninstallAsync(IPluginInstallContext ctx, CancellationToken ct);
        Task ExportDataAsync(IPluginInstallContext ctx, string folder, CancellationToken ct);   // sao lưu JSON
        Task PurgeAsync(IPluginInstallContext ctx, CancellationToken ct);            // xóa dữ liệu phụ ngoài bảng (file, cấu hình)
    }

    public interface IPluginInstallContext
    {
        DatabaseDialect Dialect { get; }            // Access | SqlServer | PostgreSql
        IPluginDbSession Db { get; }                // CHỈ thao tác được bảng Plg_<Key>_*
        string PluginKey { get; }
    }

    public sealed record PluginUninstallCheck(
        bool CanUninstall,
        IReadOnlyList<string> Warnings,
        IReadOnlyDictionary<string, long> RowCounts
    );
}
