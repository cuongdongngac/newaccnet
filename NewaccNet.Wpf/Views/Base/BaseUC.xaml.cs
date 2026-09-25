using System.Windows.Controls;

namespace NewaccNet.Wpf.Views.Base
{
    public partial class BaseUC : UserControl
    {
        public BaseUC()
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