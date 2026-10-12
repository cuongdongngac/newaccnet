using DevExpress.Xpf.Core;
using System.Windows;
using System;
using System.Windows.Media;

namespace NewaccNet.Wpf.Views.Base
{
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
        public BaseWindow()
        {
            var icon = AppIconHelper.GetAppIcon();
            if (icon != null)
            {
                this.Icon = icon;
            }
            this.Loaded += BaseWindow_Loaded;
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
