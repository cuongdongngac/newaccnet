using System;
using System.Windows;

namespace NewaccNet.Contracts
{
    /// <summary>
    /// Cấu trúc định nghĩa một nút bấm hành động của Plugin trên thanh Ribbon của Core.
    /// </summary>
    public class PluginRibbonAction
    {
        /// <summary>
        /// Tên Tab lớn trên Ribbon (Mặc định: "Tiện ích mở rộng")
        /// </summary>
        public string Category { get; set; } = "Tiện ích mở rộng";

        /// <summary>
        /// Tên nhóm nút bấm trên Ribbon (VD: "Trí tuệ nhân tạo AI", "Quản lý công trình")
        /// </summary>
        public string GroupName { get; set; } = "Plugin";

        /// <summary>
        /// Nhãn hiển thị của nút bấm
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Chú giải công cụ khi rê chuột
        /// </summary>
        public string? ToolTip { get; set; }

        /// <summary>
        /// Đường dẫn Pack URI của Icon (Svg/Png)
        /// </summary>
        public string? GlyphUri { get; set; }

        /// <summary>
        /// Hành động thực thi khi người dùng click vào nút (truyền cửa sổ chính làm Owner)
        /// </summary>
        public Action<Window?> ExecuteAction { get; set; } = _ => { };
    }
}
