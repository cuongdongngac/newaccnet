using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class BalanceSheet : DevExpress.XtraReports.UI.XtraReport
    {
        public BalanceSheet()
        {
            InitializeComponent();
            WireDrillDown();
        }

        private void WireDrillDown()
        {
            this.Detail.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]"));
            this.Detail.PreviewDoubleClick += (s, e) =>
            {
                string accountId = e.Brick?.Value?.ToString();
                if (!string.IsNullOrEmpty(accountId))
                {
                    OpenLedger(accountId);
                }
            };

            this.AccountId.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]"));
            this.AccountId.PreviewDoubleClick += (s, e) =>
            {
                string accountId = e.Brick?.Value?.ToString();
                if (!string.IsNullOrEmpty(accountId))
                {
                    OpenLedger(accountId);
                }
            };
        }

        private void OpenLedger(string accountId)
        {
            var app = System.Windows.Application.Current;
            if (app == null || app.MainWindow == null) return;
            
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                DateTime fromDate = new DateTime(DateTime.Today.Year, 1, 1);
                DateTime toDate = DateTime.Today;
                
                // Thử lấy từ Parameters (nếu có)
                if (this.Parameters["prmFromDate"] != null && this.Parameters["prmFromDate"].Value is DateTime f)
                    fromDate = f;
                else
                {
                    // Fallback lấy từ UserSettings.json nếu không có parameters
                    string savedFrom = NewaccNet.Wpf.AppSystem.Helpers.UserPreferencesHelper.GetSetting("Shared_dtFromDate");
                    if (!string.IsNullOrEmpty(savedFrom) && DateTime.TryParse(savedFrom, out DateTime df))
                        fromDate = df;
                }

                if (this.Parameters["prmToDate"] != null && this.Parameters["prmToDate"].Value is DateTime t)
                    toDate = t;
                else
                {
                    string savedTo = NewaccNet.Wpf.AppSystem.Helpers.UserPreferencesHelper.GetSetting("Shared_dtToDate");
                    if (!string.IsNullOrEmpty(savedTo) && DateTime.TryParse(savedTo, out DateTime dt))
                        toDate = dt;
                }

                LedgerReportService.ShowPreview(app.MainWindow, fromDate, toDate, accountId, true);
            }));
        }
    }
}
