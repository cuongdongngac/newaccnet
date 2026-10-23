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
    public partial class StockFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "STOCK";
        public string CategoryTitle => "Cổ phiếu / Chứng khoán";

        public StockFilterContainer()
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

                var stocks = adapter.FetchQuery(qf.Stock)
                    .Cast<StockEntity>()
                    .OrderBy(s => s.Stockid)
                    .Select(s => new { s.Stockid, DisplayText = $"{s.Stockid} - {s.Description}" })
                    .ToList();
                cboStock.ItemsSource = stocks;
            }
            catch (Exception)
            {
            }
        }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            var dict = new Dictionary<string, List<object>>();
            dict["StockId"] = cboStock.GetSelectedKeysList();
            return dict;
        }
    }
}
