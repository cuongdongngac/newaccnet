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
    public partial class AssetFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "ASSET";
        public string CategoryTitle => "Tài sản cố định";

        public AssetFilterContainer()
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

                var assets = adapter.FetchQuery(qf.Asset)
                    .Cast<AssetEntity>()
                    .OrderBy(a => a.AssetId)
                    .Select(a => new { a.AssetId, DisplayText = $"{a.AssetId} - {a.AssetName}" })
                    .ToList();
                cboAsset.ItemsSource = assets;

                var reasons = adapter.FetchQuery(qf.Reason)
                    .Cast<ReasonEntity>()
                    .OrderBy(r => r.Reasonid)
                    .Select(r => new { r.Reasonid, DisplayText = $"{r.Reasonid} - {r.ReasonName}" })
                    .ToList();
                cboReason.ItemsSource = reasons;
            }
            catch (Exception)
            {
            }
        }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            var dict = new Dictionary<string, List<object>>();
            dict["AssetId"] = cboAsset.GetSelectedKeysList();
            dict["ReasonId"] = cboReason.GetSelectedKeysList();
            return dict;
        }
    }
}
