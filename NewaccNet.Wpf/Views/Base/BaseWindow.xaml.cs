using DevExpress.Xpf.Core;
using System.Windows;

namespace NewaccNet.Wpf.Views.Base
{
    public partial class BaseWindow : ThemedWindow
    {
        public BaseWindow()
        {
            InitializeComponent();
        }

        public virtual void LoadData() { }
        public virtual void Save() { }
        public virtual void Refresh() { }
        public virtual void Print() { }
        public virtual bool ValidateData() { return true; }
    }
}