using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.Contracts.AI
{
    public enum ToolAccess
    {
        Read,
        DraftWrite // KHÔNG có Post
    }

    public sealed class ToolDefinition
    {
        public required string Name { get; init; }          // snake_case; tool plugin: "<pluginKey>.<name>"
        public required string Description { get; init; }   // tiếng Việt, rõ khi nào dùng
        public required ToolAccess Access { get; init; }
        public required string ParametersJsonSchema { get; init; }
        public required string OwnerId { get; init; }       // "core" hoặc Id plugin
    }

    public interface IAiTool
    {
        ToolDefinition Definition { get; }
        Task<ToolResult> ExecuteAsync(JsonElement args, ToolCallContext ctx, CancellationToken ct);
    }

    public sealed class ToolCallContext
    {
        public required string UserName { get; init; }
        public required string CallerId { get; init; }
        public string? DraftId { get; init; }               // phạm vi DraftWrite
        public required ISuggestionSink Suggestions { get; init; }  // nơi DraftWrite ghi đề xuất
    }

    public sealed record ToolResult(bool Ok, string Json, string? Error = null);

    /// <summary>
    /// Nơi lưu nhận các đề xuất gợi ý của AI cho draft hiện tại.
    /// </summary>
    public interface ISuggestionSink
    {
        void Add(AiSuggestion suggestion);
        IReadOnlyList<AiSuggestion> GetAll();
    }
}
