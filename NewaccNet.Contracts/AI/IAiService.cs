using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.Contracts.AI
{
    /// <summary>
    /// Cổng duy nhất mà UI và Plugin được dùng để gọi AI.
    /// </summary>
    public interface IAiService
    {
        bool IsEnabled { get; }   // false khi người dùng tắt AI hoặc chưa cấu hình

        /// <summary>
        /// Chạy một skill đã đăng ký (có tool-loop). Dùng cho gợi ý mapping, phân tích…
        /// </summary>
        Task<AiResult<TOut>> RunSkillAsync<TIn, TOut>(
            string skillId, TIn input, AiCallOptions? options = null, CancellationToken ct = default);

        /// <summary>
        /// Gọi một lần, trả JSON có cấu trúc, không tool. Dùng cho tác vụ đơn giản.
        /// </summary>
        Task<AiResult<TOut>> CompleteStructuredAsync<TOut>(
            AiPrompt prompt, AiCallOptions? options = null, CancellationToken ct = default);

        /// <summary>
        /// Đọc ảnh/PDF (vision) trả JSON có cấu trúc.
        /// </summary>
        Task<AiResult<TOut>> ExtractFromDocumentAsync<TOut>(
            AiPrompt prompt, IReadOnlyList<AiAttachment> attachments,
            AiCallOptions? options = null, CancellationToken ct = default);
    }

    public sealed record AiPrompt(
        string SystemInstruction,          // lời dặn nghiệp vụ
        string UserInstruction,            // yêu cầu của người dùng
        IReadOnlyList<AiDataBlock>? Data   // dữ liệu đầu vào — luôn được bọc phân định
    );

    public sealed record AiDataBlock(string Label, string Content, AiDataSensitivity Sensitivity);

    public enum AiDataSensitivity
    {
        Public,
        Business,
        Personal   // Personal → bắt buộc che
    }

    public sealed record AiAttachment(string FileName, string MimeType, byte[] Content);

    public sealed class AiCallOptions
    {
        public string Profile { get; init; } = "default";   // tên profile, không phải tên model
        public string CallerId { get; init; } = "core";     // "core" hoặc Id plugin, Core tự gán cho plugin
        public string? CorrelationId { get; init; }         // vd: DraftId để truy vết
        public int? MaxToolRounds { get; init; }
        public TimeSpan? Timeout { get; init; }
    }

    public enum AiErrorKind
    {
        None = 0,
        Disabled,
        PolicyDenied,
        BudgetExceeded,
        Offline,
        Timeout,
        InvalidOutput,
        ProviderError
    }

    /// <summary>
    /// Phong bì kết quả — KHÔNG ném exception cho lỗi nghiệp vụ/mạng.
    /// </summary>
    public sealed class AiResult<T>
    {
        public bool Success { get; init; }
        public T? Value { get; init; }
        public AiErrorKind Error { get; init; }   // None, Disabled, PolicyDenied, BudgetExceeded,
                                                  // Offline, Timeout, InvalidOutput, ProviderError
        public string? ErrorMessage { get; init; }
        public string CallId { get; init; } = ""; // khóa tra nhật ký AiCallLog
        public string ModelUsed { get; init; } = "";
        public int PromptTokens { get; init; }
        public int CompletionTokens { get; init; }
    }
}
