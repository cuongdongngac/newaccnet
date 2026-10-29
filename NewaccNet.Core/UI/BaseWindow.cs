using DevExpress.Xpf.Core;
using System.Windows;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace NewaccNet.Core.UI
{
    /// <summary>
    /// Lớp hỗ trợ Trợ giúp trực tuyến theo ngữ cảnh (Context-sensitive Help)
    /// Hoạt động tương tự AuthHelper (RoleMask) với Dependency/Attached Property
    /// </summary>
    public static class HelpHelper
    {
        public const string DefaultHelpBaseUrl = "https://newacc.learning.io.vn";

        public static readonly DependencyProperty HelpTopicProperty =
            DependencyProperty.RegisterAttached("HelpTopic", typeof(string), typeof(HelpHelper), new PropertyMetadata(null));

        public static string GetHelpTopic(DependencyObject obj) => (string)obj.GetValue(HelpTopicProperty);
        public static void SetHelpTopic(DependencyObject obj, string value) => obj.SetValue(HelpTopicProperty, value);

        public static void OpenUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) url = DefaultHelpBaseUrl;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                try
                {
                    Process.Start("explorer.exe", url);
                }
                catch { }
            }
        }

        public static string ResolveUrl(string? baseUrl, string? topic)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) baseUrl = DefaultHelpBaseUrl;
            if (string.IsNullOrWhiteSpace(topic)) return baseUrl;

            topic = topic.Trim();
            if (topic.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                topic.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return topic;
            }

            return baseUrl.TrimEnd('/') + "/" + topic.TrimStart('/');
        }
    }

    public static class AppIconHelper
    {
        public const string DefaultIconUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/xaf/action_report_showdesigner.svg";
        private static ImageSource _cachedIcon;

        public static ImageSource GetAppIcon()
        {
            if (_cachedIcon == null)
            {
                try
                {
                    var ext = new SvgImageSourceExtension { Uri = new Uri(DefaultIconUri) };
                    _cachedIcon = (ImageSource)ext.ProvideValue(null);
                }
                catch { }
            }
            return _cachedIcon;
        }
    }

    public class BaseWindow : ThemedWindow
    {
        public static readonly DependencyProperty HelpUrlProperty =
            DependencyProperty.Register(nameof(HelpUrl), typeof(string), typeof(BaseWindow),
                new PropertyMetadata(HelpHelper.DefaultHelpBaseUrl));

        public static readonly DependencyProperty HelpTopicProperty =
            DependencyProperty.Register(nameof(HelpTopic), typeof(string), typeof(BaseWindow),
                new PropertyMetadata(null));

        /// <summary>
        /// Đường dẫn cơ sở (Mặc định: https://newacc.learning.io.vn)
        /// </summary>
        public string HelpUrl
        {
            get => (string)GetValue(HelpUrlProperty);
            set => SetValue(HelpUrlProperty, value);
        }

        /// <summary>
        /// Đường dẫn chuyên đề trợ giúp cụ thể cho form này (ví dụ: "/guide/chung-tu.html" hoặc "vouchers")
        /// </summary>
        public string? HelpTopic
        {
            get => (string?)GetValue(HelpTopicProperty);
            set => SetValue(HelpTopicProperty, value);
        }

        public BaseWindow()
        {
            var icon = AppIconHelper.GetAppIcon();
            if (icon != null)
            {
                this.Icon = icon;
            }
            this.Loaded += BaseWindow_Loaded;
            this.PreviewKeyDown += BaseWindow_PreviewKeyDown;
        }

        /// <summary>
        /// Lắng nghe phím tắt F1 để tự động kích hoạt Help theo ngữ cảnh
        /// </summary>
        private void BaseWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1 && Keyboard.Modifiers == ModifierKeys.None)
            {
                e.Handled = true;

                // 1. Nếu phần tử đang được focus có khai báo HelpTopic riêng (như gắn trên Button/Control)
                if (Keyboard.FocusedElement is DependencyObject focusedDep)
                {
                    string focusedTopic = HelpHelper.GetHelpTopic(focusedDep);
                    if (!string.IsNullOrWhiteSpace(focusedTopic))
                    {
                        string target = HelpHelper.ResolveUrl(HelpUrl, focusedTopic);
                        HelpHelper.OpenUrl(target);
                        return;
                    }
                }

                // 2. Mặc định mở theo thiết lập của Form hiện tại
                OpenHelp();
            }
        }

        /// <summary>
        /// Mở tài liệu trợ giúp trực tuyến cho Form
        /// </summary>
        /// <param name="customUrl">Nếu truyền url cụ thể, mở thẳng url này; nếu null, dùng HelpTopic/HelpUrl của form</param>
        public virtual void OpenHelp(string? customUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(customUrl))
            {
                HelpHelper.OpenUrl(customUrl);
                return;
            }

            string targetUrl = HelpHelper.ResolveUrl(HelpUrl, HelpTopic);
            HelpHelper.OpenUrl(targetUrl);
        }

        private void BaseWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureGridHeadersCentered(this);
        }

        private static void EnsureGridHeadersCentered(DependencyObject parent)
        {
            if (parent == null) return;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is DevExpress.Xpf.Grid.GridControl grid)
                {
                    if (grid.Columns != null)
                    {
                        foreach (var col in grid.Columns)
                        {
                            col.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                        }
                    }
                    if (grid.Bands != null)
                    {
                        foreach (var band in grid.Bands)
                        {
                            band.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                            if (band.Columns != null)
                            {
                                foreach (var col in band.Columns)
                                {
                                    col.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                                }
                            }
                        }
                    }
                }
                else if (child is DevExpress.Xpf.Grid.TreeListControl tree)
                {
                    if (tree.Columns != null)
                    {
                        foreach (var col in tree.Columns)
                        {
                            col.HorizontalHeaderContentAlignment = HorizontalAlignment.Center;
                        }
                    }
                }
                EnsureGridHeadersCentered(child);
            }
        }

        public virtual void LoadData() { }
        public virtual void Save() { }
        public virtual void Delete() { }
        public virtual void Refresh() { }
        public virtual void Print() { }
        public virtual bool ValidateData() { return true; }
    }
}

