using System.Windows.Controls;

namespace NewaccNet.Wpf.Views.Base
{
    public class BaseUC : UserControl
    {
        public BaseUC()
        {
            // Không có InitializeComponent() vì đây là class C# thuần (không có .xaml),
            // dùng để làm class cơ sở cho các UserControl khác kế thừa.
        }

        public virtual void LoadData() { }
        public virtual void Save() { }
        public virtual void Refresh() { }
        public virtual void Print() { }
        public virtual bool ValidateData() { return true; }
    }
}
