using DevExpress.XtraReports.UI;
using System;
using System.ComponentModel;

namespace NewaccNet.Wpf.AppSystem.Reports.Debt
{
    public partial class SummaryDebtReport : DevExpress.XtraReports.UI.XtraReport
    {
        private int _debtGroupCounter = 0;
        private int _subGroupCounter = 0;

        public SummaryDebtReport()
        {
            InitializeComponent();
            this.BeforePrint += SummaryDebtReport_BeforePrint;
        }

        private void SummaryDebtReport_BeforePrint(object sender, CancelEventArgs e)
        {
            _debtGroupCounter = 0;
            _subGroupCounter = 0;
        }

        private void GroupHeader2_BeforePrint(object sender, CancelEventArgs e)
        {
            _debtGroupCounter++;
            this.tg1CellStt.Text = _debtGroupCounter.ToString();
            _subGroupCounter = 0;
        }

        private void GroupHeader1_BeforePrint(object sender, CancelEventArgs e)
        {
            _subGroupCounter++;
            // Dòng nhóm 2 (khoản mục/hợp đồng con) để trống STT theo chuẩn báo cáo công nợ
            this.tg2CellSttBlank.Text = string.Empty;
        }
    }
}
