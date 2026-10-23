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
    public partial class InventoryFilterContainer : UserControl, IDetailFilterContainer
    {
        public string CategoryCode => "MATERIAL";
        public string CategoryTitle => "Vật tư - Hàng hóa";

        public InventoryFilterContainer()
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

                var warehouses = adapter.FetchQuery(qf.Warehouse)
                    .Cast<WarehouseEntity>()
                    .OrderBy(w => w.WarehouseId)
                    .Select(w => new { w.WarehouseId, WarehouseName = $"{w.WarehouseId} - {w.WarehouseName}" })
                    .ToList();
                cboWarehouse.ItemsSource = warehouses;

                var materials = adapter.FetchQuery(qf.InventoryItem)
                    .Cast<InventoryItemEntity>()
                    .OrderBy(m => m.MaterialId)
                    .Select(m => new { m.MaterialId, DisplayText = $"{m.MaterialId} - {m.MaterialName} ({m.Unit})" })
                    .ToList();
                cboMaterial.ItemsSource = materials;
            }
            catch (Exception)
            {
                // Bỏ qua lỗi kết nối tạm thời
            }
        }

        public Dictionary<string, List<object>> GetSelectedCriteria()
        {
            var dict = new Dictionary<string, List<object>>();
            dict["WarehouseId"] = cboWarehouse.GetSelectedKeysList();
            dict["MaterialId"] = cboMaterial.GetSelectedKeysList();
            return dict;
        }
    }
}
