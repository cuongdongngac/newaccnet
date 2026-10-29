using System;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Bản ghi trạng thái lưu trữ của Plugin trong %APPDATA%\NewaccNet\plugins.json
    /// </summary>
    public class PluginRecord
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string DllFileName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } = true;
        public DateTime InstalledDate { get; set; } = DateTime.Now;
    }
}
