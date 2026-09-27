using DevExpress.Xpf.Core;
using System.Windows;
using System;
using System.Windows.Media;

namespace NewaccNet.Wpf.Views.Base
{
    public class BaseDetailWindow : BaseWindow
    {
        public BaseDetailWindow()
        {
            // Set some default properties for detail windows if needed
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ShowIcon = false;
            this.ResizeMode = ResizeMode.NoResize;
        }

        // Virtual methods for standard Accept and Cancel actions
        protected virtual void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        protected virtual void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
