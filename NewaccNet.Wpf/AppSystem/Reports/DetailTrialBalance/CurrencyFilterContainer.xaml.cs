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
    public partial class CurrencyFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "CURRENCY";
        public string CategoryTitle => "Ngoại tệ";

        public CurrencyFilterContainer()
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

                var currencies = adapter.FetchQuery(qf.Currency)
                    .Cast<CurrencyEntity>()
                    .OrderBy(c => c.CurrencyId)
                    .Select(c => new { c.CurrencyId, DisplayText = $"{c.CurrencyId} - {c.CurrencyName}" })
                    .ToList();
                cboCurrency.ItemsSource = currencies;
            }
            catch (Exception)
            {
            }
        }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            var dict = new Dictionary<string, List<object>>();
            dict["CurrencyId"] = cboCurrency.GetSelectedKeysList();
            return dict;
        }
    }
}
