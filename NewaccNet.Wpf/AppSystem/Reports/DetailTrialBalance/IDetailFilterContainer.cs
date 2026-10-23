using System;
using System.Collections.Generic;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance
{
    public interface IDetailFilterContainer
    {
        string CategoryCode { get; }
        string CategoryTitle { get; }
        void LoadData();
        Dictionary<string, List<object>> GetSelectedCriteria();
    }
}
