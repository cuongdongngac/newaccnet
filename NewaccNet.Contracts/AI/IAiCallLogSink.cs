using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.Contracts.AI
{
    public sealed class AiCallLogEntry
    {
        public string CallId { get; init; } = Guid.NewGuid().ToString("D");
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string UserName { get; init; } = string.Empty;
        public string CallerId { get; init; } = "core";
        public string? SkillId { get; init; }
        public string Profile { get; init; } = string.Empty;
        public string ModelUsed { get; init; } = string.Empty;
        public string? CorrelationId { get; init; }
        public int PromptTokens { get; init; }
        public int CompletionTokens { get; init; }
        public int ToolRounds { get; init; }
        public long DurationMs { get; init; }
        public AiErrorKind ResultKind { get; init; }
        public string? ToolTrace { get; init; }
    }

    /// <summary>
    /// Ghi nhật ký kiểm toán và thống kê token cho mỗi lời gọi AI.
    /// </summary>
    public interface IAiCallLogSink
    {
        Task WriteLogAsync(AiCallLogEntry entry, CancellationToken ct = default);
    }
}
