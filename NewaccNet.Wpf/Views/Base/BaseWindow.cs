using DevExpress.Xpf.Core;
using System.Windows;
using System;
using System.Windows.Media;

namespace NewaccNet.Wpf.Views.Base
{
    public class BaseWindow : ThemedWindow
    {
        public BaseWindow()
        {
            try
            {
                var ext = new SvgImageSourceExtension() { Uri = new Uri("pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_mydetails.svg") };
                this.Icon = (ImageSource)ext.ProvideValue(null);
            }
            catch {}
        }

        public virtual void LoadData() { }
        public virtual void Save() { }
        public virtual void Delete() { }
        public virtual void Refresh() { }
        public virtual void Print() { }
        public virtual bool ValidateData() { return true; }
    }
}
