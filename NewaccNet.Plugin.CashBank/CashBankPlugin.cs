using System;
using System.Windows;
using NewaccNet.Contracts;
using NewaccNet.Plugin.CashBank.Views;

namespace NewaccNet.Plugin.CashBank
{
    /// <summary>
    /// Điểm nhập (Entrypoint) của Plugin Phân hệ Vốn bằng tiền (Cash & Bank Management).
    /// Quản lý dòng tiền vào (Phiếu thu / Báo có), dòng tiền ra (Phiếu chi / Báo nợ / UNC),
    /// luân chuyển tiền nội bộ và cơ chế mapping nghiệp vụ minh bạch (Accounting First).
    /// </summary>
    public class CashBankPlugin : IPlugin
    {
        public string Id => "plugin.cash.bank";

        public string Name => "Phân hệ Vốn bằng tiền";

        public string Version => "1.0.0";

        public string Description => "Quản lý phiếu thu, phiếu chi, giấy báo có, giấy báo nợ, ủy nhiệm chi và luân chuyển quỹ - ngân hàng theo mô hình mapping nghiệp vụ kế toán.";

        public string Author => "NewaccNet Enterprise Team";

        private IPluginContext? _context;

        public void Initialize(IPluginContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            const string category = "Vốn bằng tiền";
            const string groupVouchers = "Chứng từ thu chi";
            const string groupConfig = "Cấu hình";

            // 1. Phiếu Thu / Báo Có (Dòng tiền đi vào)
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = groupVouchers,
                Title = "Phiếu thu / Báo có",
                ToolTip = "Lập Phiếu thu tiền mặt hoặc Giấy Báo có ngân hàng (PT / BC)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_sale.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new CashReceiptWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 2. Phiếu Chi / Báo Nợ / UNC (Dòng tiền đi ra)
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = groupVouchers,
                Title = "Phiếu chi / Báo nợ",
                ToolTip = "Lập Phiếu chi tiền mặt, Giấy Báo nợ hoặc Ủy nhiệm chi ngân hàng (PC / BN / UNC)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_order.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new CashPaymentWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 3. Chuyển tiền nội bộ (Quỹ <-> Ngân hàng)
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = groupVouchers,
                Title = "Chuyển tiền nội bộ",
                ToolTip = "Rút tiền ngân hàng về quỹ, nộp quỹ vào ngân hàng hoặc chuyển tiền giữa các tài khoản ngân hàng (CKT)",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_transition.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new CashTransferWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });

            // 4. Cấu hình nghiệp vụ tiền mặt & ngân hàng (Alias & Accounts)
            _context.RegisterRibbonAction(new PluginRibbonAction
            {
                Category = category,
                GroupName = groupConfig,
                Title = "Nghiệp vụ tiền",
                ToolTip = "Khai báo các mối nghiệp vụ thu chi (Alias), tài khoản đối ứng và thiết lập tài khoản quỹ / ngân hàng",
                GlyphUri = "pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_security_permission.svg",
                ExecuteAction = (ownerWindow) =>
                {
                    var win = new CashAccountConfigWindow();
                    if (ownerWindow != null) win.Owner = ownerWindow;
                    win.ShowDialog();
                }
            });
        }

        public void Shutdown()
        {
            _context = null;
        }
    }
}
