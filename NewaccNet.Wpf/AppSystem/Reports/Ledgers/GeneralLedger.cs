using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using NewaccNet.Wpf.AppSystem.Voucher;

namespace NewaccNet.Wpf.AppSystem.Reports.Ledgers
{
    public partial class GeneralLedger : DevExpress.XtraReports.UI.XtraReport
    {
        public GeneralLedger()
        {
            InitializeComponent();
            WireDrillDown();
        }

        private void WireDrillDown()
        {
            foreach (XRControl control in this.AllControls<XRControl>())
            {
                if (control.Band is DetailBand || control.Band is GroupHeaderBand || control.Band is GroupFooterBand)
                {
                    control.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]"));
                    
                    // Giống hệt bên DiaryReport
                    control.PreviewClick += (s, e) =>
                    {
                        if (e.Brick != null && e.Brick.Value is int id && id > 0)
                        {
                            OpenVoucher(id);
                        }
                        else if (e.Brick != null && int.TryParse(e.Brick.Value?.ToString(), out int parsedId) && parsedId > 0)
                        {
                            OpenVoucher(parsedId);
                        }
                    };
                }
            }
        }

        private void OpenVoucher(int id)
        {
            var app = System.Windows.Application.Current;
            if (app == null) return;

            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                var win = new VoucherEntryWindow(id);
                win.Owner = app.MainWindow;
                win.Show();
            }));
        }
    }
}
