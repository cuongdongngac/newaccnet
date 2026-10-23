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
    public partial class DebtFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "DEBT";
        public string CategoryTitle => "Công nợ";

        public DebtFilterContainer()
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

                var partners = adapter.FetchQuery(qf.Partner)
                    .Cast<PartnerEntity>()
                    .OrderBy(p => p.Id)
                    .Select(p => new { p.Id, DisplayText = $"{p.Id} - {p.CustomerName}" })
                    .ToList();
                cboPartner.ItemsSource = partners;

                var debtTypes = adapter.FetchQuery(qf.DebtType)
                    .Cast<DebtTypeEntity>()
                    .OrderBy(r => r.Id)
                    .Select(r => new { r.Id, ReasonName = $"{r.Id} - {r.TypeName}" })
                    .ToList();

                if (debtTypes.Count > 0)
                {
                    cboDebtReason.ItemsSource = debtTypes;
                }
                else
                {
                    var reasons = adapter.FetchQuery(qf.DebtReason)
                        .Cast<DebtReasonEntity>()
                        .OrderBy(r => r.Id)
                        .Select(r => new { Id = r.Id.ToString(), ReasonName = $"{r.Id} - {r.ReasonName}" })
                        .ToList();
                    cboDebtReason.ItemsSource = reasons;
                }
            }
            catch (Exception)
            {
            }
        }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            var dict = new Dictionary<string, List<object>>();
            dict["PartnerId"] = cboPartner.GetSelectedKeysList();
            dict["DebtReasonId"] = cboDebtReason.GetSelectedKeysList();
            return dict;
        }
    }
}
