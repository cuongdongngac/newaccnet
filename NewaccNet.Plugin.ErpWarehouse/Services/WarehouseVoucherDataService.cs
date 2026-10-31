using System;
using System.Collections.Generic;
using DataAccess.EntityClasses;
using NewaccNet.Core.Data;
using NewaccNet.Core.Posting;

namespace NewaccNet.Plugin.ErpWarehouse.Services
{
    /// <summary>
    /// Wrapper chuyển tiếp tới cổng hạch toán duy nhất tại NewaccNet.Core.Posting.WarehousePostingGateway
    /// Giữ tương thích hoàn toàn cho các giao diện trong Plugin kho.
    /// </summary>
    public static class WarehouseVoucherDataService
    {
        public static InventoryVoucherConfigItemDto? GetPostingRule(string type) => WarehousePostingGateway.GetPostingRule(type);
        public static List<WarehouseEntity> LoadWarehouses() => WarehousePostingGateway.LoadWarehouses();
        public static List<InventoryItemEntity> LoadMaterials() => WarehousePostingGateway.LoadMaterials();
        public static List<PartnerEntity> LoadPartners() => WarehousePostingGateway.LoadPartners();
        public static List<TaxRateOption> LoadTaxRates() => WarehousePostingGateway.LoadTaxRates();
        public static List<AccountOption> LoadAccounts() => WarehousePostingGateway.LoadAccounts();
        public static List<int> GetVoucherIds(string prefix) => WarehousePostingGateway.GetVoucherIds(prefix);
        public static string GenerateNextVoucherNo(string prefix) => WarehousePostingGateway.GenerateNextVoucherNo(prefix);
        public static WarehouseVoucherSaveParams? LoadVoucherDetail(int voucherId) => WarehousePostingGateway.LoadVoucherDetail(voucherId);
        public static WarehouseVoucherSaveParams? LoadVoucherDetailByInventoryVoucher(int inventoryVoucherId) => WarehousePostingGateway.LoadVoucherDetailByInventoryVoucher(inventoryVoucherId);
        
        public static bool SaveVoucher(WarehouseVoucherSaveParams p, out string errorMessage, out int savedVoucherId) 
            => WarehousePostingGateway.SaveVoucher(p, out errorMessage, out savedVoucherId);
            
        public static (int total, int success, List<string> errors) MigrateLegacyEntryGraphs(
            IProgress<MigrationProgressInfo>? progress = null,
            System.Threading.CancellationToken ct = default) 
            => WarehousePostingGateway.MigrateLegacyEntryGraphs(progress, ct);
    }
}
