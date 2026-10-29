using System;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Giao diện chuẩn mực mà mọi Plugin của hệ sinh thái NewaccNet bắt buộc phải thực thi.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Mã định danh duy nhất (VD: "plugin.ai", "plugin.construction")
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Tên hiển thị thân thiện của Plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Phiên bản (Semantic Versioning, VD: "1.0.0")
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Mô tả chi tiết tính năng của Plugin
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Tác giả / Đơn vị phát triển
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Khởi tạo Plugin khi được nạp vào Core
        /// </summary>
        void Initialize(IPluginContext context);

        /// <summary>
        /// Dọn dẹp tài nguyên khi Plugin bị tắt hoặc gỡ bỏ
        /// </summary>
        void Shutdown();
    }
}
