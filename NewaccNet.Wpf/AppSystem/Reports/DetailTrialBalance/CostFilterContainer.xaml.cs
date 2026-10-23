using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance
{
    public partial class CostFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "COST";
        public string CategoryTitle => "Chi phí";

        public CostFilterContainer()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var qf = new QueryFactory();

                var costObjects = adapter.FetchQuery(qf.CostObject)
                    .Cast<CostObjectEntity>()
                    .OrderBy(o => o.Id)
                    .Select(o => new { o.Id, DisplayText = $"{o.Id} - {o.ObjectName}" })
                    .ToList();
                cboCostObject.ItemsSource = costObjects;

                var costElements = adapter.FetchQuery(qf.CostElement)
                    .Cast<CostElementEntity>()
                    .OrderBy(e => e.Id)
                    .Select(e => new { e.Id, DisplayText = $"{e.Id} - {e.ElementName}" })
                    .ToList();
                cboCostElement.ItemsSource = costElements;
            }
            catch (Exception)
            {
            }
        }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            var dict = new Dictionary<string, List<object>>();
            dict["CostObjectId"] = cboCostObject.GetSelectedKeysList();
            dict["CostElementId"] = cboCostElement.GetSelectedKeysList();
            return dict;
        }
    }
}
