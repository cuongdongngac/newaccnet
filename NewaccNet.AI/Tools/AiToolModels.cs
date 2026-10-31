using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NewaccNet.AI.Tools
{
    public enum AiToolAccess
    {
        Read = 0,
        DraftWrite = 1
    }

    public interface IAiTool
    {
        string Name { get; }
        string Description { get; }
        string ParametersJsonSchema { get; }
        AiToolAccess Access { get; }
        Task<string> ExecuteAsync(JsonElement args, CancellationToken ct = default);
    }
}
