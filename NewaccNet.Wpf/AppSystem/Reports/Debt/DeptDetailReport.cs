using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using NewaccNet.Wpf.AppSystem.Voucher;

namespace NewaccNet.Wpf.AppSystem.Reports.Debt
{
    public partial class DeptDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public DeptDetailReport()
        {
            InitializeComponent();
            WireVoucherDrillDown();
        }

        /// <summary>
        /// Gắn Tag = [JournalVoucherId] vào các ô trên DetailBand để khi người dùng click vào dòng chứng từ
        /// sẽ mở ngược lại màn hình chứng từ gốc (VoucherEntryWindow).
        /// </summary>
        private void WireVoucherDrillDown()
        {
            foreach (XRControl control in this.AllControls<XRControl>())
            {
                if (control.Band is DetailBand)
                {
                    control.ExpressionBindings.Add(
                        new ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]"));

                    control.PreviewClick += (s, e) =>
                    {
                        if (e.Brick?.Value is int id && id > 0)
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

        private static void OpenVoucher(int voucherId)
        {
            var app = System.Windows.Application.Current;
            if (app == null) return;

            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                var win = new VoucherEntryWindow(voucherId)
                {
                    Owner = app.MainWindow
                };
                win.Show();
            }));
        }
    }
}
