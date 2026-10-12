using System;
using DataAccess.EntityClasses;

namespace NewaccNet.Wpf.Controls.Search
{
    /// <summary>
    /// Đăng ký SearchProfile cho các Danh mục chính trong NewaccNet.
    /// Gọi một lần lúc ứng dụng khởi động (App.OnStartup).
    /// </summary>
    public static class SearchSetup
    {
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            SearchProfiles.DefaultCodeHeader = "Mã";
            SearchProfiles.DefaultNameHeader = "Tên";

            // 1. Hệ thống Tài khoản kế toán: Sau khi chọn hiện Số TK, hỗ trợ tìm cả số TK lẫn tên
            SearchProfiles.Register<ChartOfAccountEntity>(
                x => x.AccountId, x => x.AccountName,
                display: SearchDisplay.Code,
                defaultMode: SearchMode.All,
                codeHeader: "Số TK", nameHeader: "Tên tài khoản");

            // 2. Đối tượng công nợ / Khách hàng / Nhà cung cấp: Sau khi chọn hiện Tên
            SearchProfiles.Register<PartnerEntity>(
                x => x.Id, x => x.CustomerName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã đối tượng", nameHeader: "Tên đối tượng");

            // 3. Kho hàng: Sau khi chọn hiện Tên kho
            SearchProfiles.Register<WarehouseEntity>(
                x => x.WarehouseId, x => x.WarehouseName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã kho", nameHeader: "Tên kho");

            // 4. Đối tượng chi phí & Yếu tố chi phí: Sau khi chọn hiện Tên
            SearchProfiles.Register<CostObjectEntity>(
                x => x.Id, x => x.ObjectName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã ĐTCP", nameHeader: "Tên đối tượng CP");

            SearchProfiles.Register<CostElementEntity>(
                x => x.Id, x => x.ElementName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã YTCP", nameHeader: "Tên yếu tố CP");

            // 5. Danh mục Vật tư / Hàng hóa: Sau khi chọn hiện Tên vật tư
            SearchProfiles.Register<InventoryItemEntity>(
                x => x.MaterialId, x => x.MaterialName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã vật tư", nameHeader: "Tên vật tư");

            // 6. Nội dung & Lý do công nợ
            SearchProfiles.Register<DebtTypeEntity>(
                x => x.Id, x => x.TypeName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã loại", nameHeader: "Nội dung công nợ");

            SearchProfiles.Register<DebtReasonEntity>(
                x => x.Id, x => x.ReasonName,
                display: SearchDisplay.Name,
                defaultMode: SearchMode.All,
                codeHeader: "Mã lý do", nameHeader: "Lý do công nợ");

            _initialized = true;
        }
    }
}
