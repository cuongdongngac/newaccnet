using System.Collections.Generic;
using System.Windows.Controls;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance
{
    public partial class NormalAccountFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "GENERAL";
        public string CategoryTitle => "Tài khoản thông thường";

        public NormalAccountFilterContainer()
        {
            InitializeComponent();
        }

        public void LoadData() { }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            return new Dictionary<string, List<object>>();
        }
    }
}
