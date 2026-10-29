using System;
using System.Windows;
using System.Windows.Media;
using NewaccNet.Core.UI;

namespace NewaccNet.Wpf.Views.Base
{
    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.BaseWindow.
    /// Ensures existing XAML views and code-behinds referencing NewaccNet.Wpf.Views.Base.BaseWindow continue working without modification.
    /// </summary>
    public class BaseWindow : NewaccNet.Core.UI.BaseWindow
    {
    }

    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.BaseDetailWindow.
    /// </summary>
    public class BaseDetailWindow : NewaccNet.Core.UI.BaseDetailWindow
    {
    }

    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.BaseReportFilterWindow.
    /// </summary>
    public class BaseReportFilterWindow : NewaccNet.Core.UI.BaseReportFilterWindow
    {
    }

    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.AppDateEdit.
    /// </summary>
    public class AppDateEdit : NewaccNet.Core.UI.AppDateEdit
    {
    }

    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.AppDateEditSettings.
    /// </summary>
    public class AppDateEditSettings : NewaccNet.Core.UI.AppDateEditSettings
    {
    }

    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.AppDetailTableView.
    /// </summary>
    public class AppDetailTableView : NewaccNet.Core.UI.AppDetailTableView
    {
        public new event EventHandler<RowDeletingEventArgs>? RowDeleting;
        public new event EventHandler<RowDeletedEventArgs>? RowDeleted;

        protected override void OnRowDeleting(NewaccNet.Core.UI.RowDeletingEventArgs e)
        {
            if (RowDeleting != null)
            {
                var shimArgs = new RowDeletingEventArgs(e.RowItem, e.RowHandle) { Cancel = e.Cancel };
                RowDeleting.Invoke(this, shimArgs);
                e.Cancel = shimArgs.Cancel;
            }
            base.OnRowDeleting(e);
        }

        protected override void OnRowDeleted(NewaccNet.Core.UI.RowDeletedEventArgs e)
        {
            if (RowDeleted != null)
            {
                var shimArgs = new RowDeletedEventArgs(e.RowItem, e.RowHandle);
                RowDeleted.Invoke(this, shimArgs);
            }
            base.OnRowDeleted(e);
        }
    }

    public class RowDeletingEventArgs : NewaccNet.Core.UI.RowDeletingEventArgs
    {
        public RowDeletingEventArgs(object rowItem, int rowHandle) : base(rowItem, rowHandle) { }
    }

    public class RowDeletedEventArgs : NewaccNet.Core.UI.RowDeletedEventArgs
    {
        public RowDeletedEventArgs(object rowItem, int rowHandle) : base(rowItem, rowHandle) { }
    }

    public static class HelpHelper
    {
        public const string DefaultHelpBaseUrl = NewaccNet.Core.UI.HelpHelper.DefaultHelpBaseUrl;
        public static readonly DependencyProperty HelpTopicProperty = NewaccNet.Core.UI.HelpHelper.HelpTopicProperty;
        public static string GetHelpTopic(DependencyObject obj) => NewaccNet.Core.UI.HelpHelper.GetHelpTopic(obj);
        public static void SetHelpTopic(DependencyObject obj, string value) => NewaccNet.Core.UI.HelpHelper.SetHelpTopic(obj, value);
        public static void OpenUrl(string? url) => NewaccNet.Core.UI.HelpHelper.OpenUrl(url);
        public static string ResolveUrl(string? baseUrl, string? topic) => NewaccNet.Core.UI.HelpHelper.ResolveUrl(baseUrl, topic);
    }

    public static class AppIconHelper
    {
        public const string DefaultIconUri = NewaccNet.Core.UI.AppIconHelper.DefaultIconUri;
        public static ImageSource GetAppIcon() => NewaccNet.Core.UI.AppIconHelper.GetAppIcon();
    }
}

namespace NewaccNet.Wpf.AppSystem.Directory
{
    /// <summary>
    /// Backward-compatibility shim inheriting from NewaccNet.Core.UI.BaseDictionaryWindow.
    /// </summary>
    public class BaseDictionaryWindow : NewaccNet.Core.UI.BaseDictionaryWindow
    {
    }
}
