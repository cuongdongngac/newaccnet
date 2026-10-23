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
            this.ApplySubtleBorders();
            WireDrillDown();
            WireFooterBalance();
        }

        private decimal? _lastBalance;

        private void WireFooterBalance()
        {
            this.Detail.BeforePrint += (s, e) =>
            {
                var val = GetCurrentColumnValue("Balance");
                if (val != null && decimal.TryParse(val.ToString(), out decimal b))
                {
                    _lastBalance = b;
                }
            };

            this.tableFooter.BeforePrint += (s, e) =>
            {
                if (!_lastBalance.HasValue && this.DataSource is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        if (item is NewaccNet.Reports.LedgerReportDTO dto)
                        {
                            _lastBalance = dto.Balance;
                        }
                    }
                }
            };

            this.tfCellBalanceDebit.PrintOnPage += (s, e) =>
            {
                if (_lastBalance.HasValue && string.IsNullOrWhiteSpace(this.tfCellBalanceDebit.Text))
                {
                    if (_lastBalance.Value > 0)
                    {
                        this.tfCellBalanceDebit.Text = string.Format("{0:#,##0;(#,##0);\"\"}", _lastBalance.Value);
                    }
                }
            };

            this.tfCellBalanceCredit.PrintOnPage += (s, e) =>
            {
                if (_lastBalance.HasValue && string.IsNullOrWhiteSpace(this.tfCellBalanceCredit.Text))
                {
                    if (_lastBalance.Value < 0)
                    {
                        this.tfCellBalanceCredit.Text = string.Format("{0:#,##0;(#,##0);\"\"}", Math.Abs(_lastBalance.Value));
                    }
                }
            };
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
