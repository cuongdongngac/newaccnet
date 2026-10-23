using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Cost
{
    /// <summary>
    /// Báo cáo Tổng hợp Chi phí (CostReport).
    /// Khổ A4 Ngang, hỗ trợ Visual Designer trong Visual Studio.
    /// Chuẩn hóa theo mẫu legacy CostReport:
    /// - STT, Tên đối tượng (Đối tượng chi phí / Yếu tố chi phí)
    /// - Dư đầu kỳ (Nợ / Có)
    /// - Phát sinh trong kỳ (Nợ / Có)
    /// - Dư cuối kỳ (Nợ / Có)
    /// - Drill-down cấp 1: Click dòng chi phí để mở Sổ chi tiết chi phí.
    /// </summary>
    public partial class CostSummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        private int CostObjectCounter = 0;
        private int ElementObjectCounter = 0;

        public CostSummaryReport()
        {
            InitializeComponent();
            this.BeforePrint += CostSummaryReport_BeforePrint;
        }

        private void CostSummaryReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CostObjectCounter = 0;
            ElementObjectCounter = 0;
        }

        public void SetReportHeader(
            string accountId,
            string accountName,
            DateTime fromDate,
            DateTime toDate,
            string? companyName = null)
        {
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                if (Parameters["prmCompanyName"] != null)
                {
                    Parameters["prmCompanyName"].Value = companyName;
                }
                if (lblCompany != null)
                {
                    lblCompany.Text = companyName;
                }
            }

            if (lblTitle != null)
            {
                lblTitle.Text = $"TÀI KHOẢN {accountId}";
            }

            if (lblDate != null)
            {
                lblDate.Text = $"Từ ngày {fromDate:dd/MM/yy} Đến ngày {toDate:dd/MM/yy}";
            }
        }

        private void ObjectCost_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CostObjectCounter++;
            this.tgCellStt.Text = CostObjectCounter.ToString();

            // Reset số thứ tự yếu tố chi phí về 0 khi bắt đầu sang nhóm Đối tượng chi phí mới
            ElementObjectCounter = 0;
        }

        private void groupHeaderBand1_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ElementObjectCounter++;
            // Gán vào ô STT của dòng yếu tố chi phí
            this.cellStt.Text = ElementObjectCounter.ToString();
        }
    }
}
