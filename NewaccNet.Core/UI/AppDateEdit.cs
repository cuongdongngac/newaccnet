using System;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;

namespace NewaccNet.Core.UI
{
    /// <summary>
    /// Base Control cho việc nhập liệu Ngày tháng trong toàn hệ thống NewaccNet:
    /// - Kế thừa từ DevExpress DateEdit.
    /// - Chuẩn hóa định dạng hiển thị và nhập liệu thống nhất: dd/MM/yyyy.
    /// - Hỗ trợ MaskType.DateTimeAdvancingCaret giúp tự động chuyển con trỏ khi gõ đủ ngày/tháng.
    /// </summary>
    public class AppDateEdit : DateEdit
    {
        public const string DefaultDateFormat = "dd/MM/yyyy";
        public const double DefaultDateWidth = 125;

        public AppDateEdit()
        {
            ApplyDefaultDateFormat(this);
        }

        public static void ApplyDefaultDateFormat(DateEdit editor)
        {
            if (editor == null) return;
            editor.Mask = DefaultDateFormat;
            editor.MaskType = MaskType.DateTimeAdvancingCaret;
            editor.MaskUseAsDisplayFormat = true;
            editor.DisplayFormatString = DefaultDateFormat;
            if (double.IsNaN(editor.MinWidth) || editor.MinWidth < DefaultDateWidth)
            {
                editor.MinWidth = DefaultDateWidth;
            }
            if (double.IsNaN(editor.Width))
            {
                editor.Width = DefaultDateWidth;
                editor.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
    }

    /// <summary>
    /// Base Inplace EditSettings cho các cột Ngày tháng trên GridControl / TreeListControl:
    /// - Kế thừa từ DevExpress DateEditSettings.
    /// - Chuẩn hóa định dạng dd/MM/yyyy và căn giữa mặc định.
    /// </summary>
    public class AppDateEditSettings : DateEditSettings
    {
        public const string DefaultDateFormat = "dd/MM/yyyy";

        public AppDateEditSettings()
        {
            ApplyDefaultDateFormat(this);
        }

        public static void ApplyDefaultDateFormat(DateEditSettings settings)
        {
            if (settings == null) return;
            settings.Mask = DefaultDateFormat;
            settings.MaskType = MaskType.DateTimeAdvancingCaret;
            settings.MaskUseAsDisplayFormat = true;
            settings.DisplayFormat = DefaultDateFormat;
        }
    }
}

