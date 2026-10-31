using System;
using System.Linq;
using System.Windows;
using NewaccNet.Contracts;
using NewaccNet.Plugin.ErpWarehouse.Views;

namespace NewaccNet.Plugin.ErpWarehouse
{
    /// <summary>
    /// Điểm nhập (Entrypoint) của Plugin Phân hệ Xuất nhập kho.
    /// Tái tạo form nhập liệu inventoryVoucher chuẩn DevExpress và nhân bản thành 5 loại phiếu xuất nhập độc lập,
    /// nạp cấu hình rule định khoản ngầm định trực tiếp từ CSDL.
    /// </summary>
    public class ErpInventoryPlugin : IPlugin
    {
        public string Id => "plugin.erp.warehouse";

        public string Name => "Phân hệ Xuất nhập kho";

        public string Version => "2.0.0";

        public string Description => "Quản lý và lập 5 loại chứng từ xuất nhập kho chuyên biệt (Nhập mua, Xuất bán, Nhập nội bộ, Xuất nội bộ, Điều chuyển kho) nạp cấu hình rule định khoản ngầm định từ hệ thống.";

        public string Author => "NewaccNet Enterprise Team";

        private IPluginContext? _context;

        public void Initialize(IPluginContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            const string category = "Xuất nhập";
            const string group = "Phiếu xuất nhập";

            // 1. Phiếu Nhập Mua Hàng (NM) - Biểu tượng Mua sắm / Nhập hàng
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = group,
                Title = "Nhập mua hàng",
                ToolTip = "Lập Phiếu nhập kho mua hàng từ nhà cung cấp (NM - kèm hóa đơn và thuế GTGT)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/shopping_shoppingcart.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new PurchaseReceiptWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 2. Phiếu Xuất Bán Hàng (XB) - Biểu tượng Xe tải giao vận / Giao hàng bán
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = group,
                Title = "Xuất bán hàng",
                ToolTip = "Lập Phiếu xuất kho bán hàng cho khách hàng (XB - theo dõi song song giá vốn và doanh thu/thuế)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/shopping_delivery.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new SalesDeliveryWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 3. Phiếu Nhập Kho Nội Bộ (NNB) - Biểu tượng Thùng hàng / Thành phẩm nhập kho
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = group,
                Title = "Nhập nội bộ",
                ToolTip = "Lập Phiếu nhập kho thành phẩm, nội bộ hoặc nhập khác (NNB)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/shopping_box.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new InternalReceiptWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 4. Phiếu Xuất Kho Nội Bộ (XNB) - Biểu tượng Kiện hàng / Xuất dùng nội bộ
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = group,
                Title = "Xuất nội bộ",
                ToolTip = "Lập Phiếu xuất kho dùng cho sản xuất, công trình hoặc chi phí quản lý (XNB)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/shopping_package.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new InternalIssueWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 5. Phiếu Điều Chuyển Kho (CK) - Biểu tượng Kho hàng / Luân chuyển giữa các kho
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = group,
                Title = "Điều chuyển kho",
                ToolTip = "Lập Phiếu xuất điều chuyển hàng hóa giữa các kho nội bộ (CK)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/shopping_store.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new WarehouseTransferWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 6. Tool Chuyển Đổi Dữ Liệu Cũ (Migration Tool) - Biểu tượng Sơ đồ đồ thị liên kết
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = "Công cụ dữ liệu",
                Title = "Chuyển đổi đồ thị kho",
                ToolTip = "Quét các phiếu xuất nhập cũ và tự động ghi vết liên kết đồ thị JSON (EntryGraph) với tiến độ trực quan",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/icon%20builder/business_diagram.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var confirm = MessageBox.Show(
                        "Hệ thống sẽ quét các phiếu xuất nhập trong CSDL chưa có EntryGraph và tự động liên kết các dòng định khoản tương ứng.\n\nBạn có muốn thực hiện không?",
                        "Xác nhận chuyển đổi dữ liệu",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirm != MessageBoxResult.Yes) return;

                    var progressWin = new MigrationProgressWindow();
                    if (ownerWindow != null) progressWin.Owner = ownerWindow;
                    progressWin.ShowDialog();
                }
            });
        }

        public void Shutdown()
        {
            _context = null;
        }
    }
}
