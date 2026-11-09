using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.Contracts.Plugins
{
    public enum DatabaseDialect
    {
        Access,
        SqlServer,
        PostgreSql
    }

    public interface IDraftPostingService
    {
        Task<DraftPostingResult> PostDraftAsync(VoucherDraft draft, CancellationToken ct = default);
    }

    public sealed record DraftPostingResult(bool Success, int? VoucherId, IReadOnlyList<string> Errors);

    public interface IDraftService
    {
        Task<VoucherDraft?> GetAsync(string draftId, CancellationToken ct = default);
        Task SaveAsync(VoucherDraft draft, CancellationToken ct = default);
    }

    public interface IPluginDbSession
    {
        DatabaseDialect Dialect { get; }
        Task<int> ExecuteAsync(string sql, IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken ct = default);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object?>>> QueryAsync(string sql, IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken ct = default);
    }
}
