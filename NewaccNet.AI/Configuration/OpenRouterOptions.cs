using System;

namespace NewaccNet.AI.Configuration
{
    public class OpenRouterOptions
    {
        public const string DefaultBaseUrl = "https://openrouter.ai/api/v1";
        public const string DefaultChatModel = "google/gemini-2.5-flash";
        public const string DefaultVisionModel = "google/gemini-2.5-flash";
        public const string DefaultProModel = "google/gemini-2.5-pro";
        public const string DefaultClaudeModel = "anthropic/claude-3.5-sonnet";

        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = DefaultBaseUrl;
        public string Model { get; set; } = DefaultChatModel;
        public string VisionModel { get; set; } = DefaultVisionModel;
        public string HttpReferer { get; set; } = "https://newaccnet.local";
        public string AppTitle { get; set; } = "NewaccNet Accounting AI";

        public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
    }
}
