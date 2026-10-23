using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Currency
{
    /// <summary>
    /// Báo cáo Tổng hợp Ngoại tệ (CurrencySummaryReport).
    /// Chuẩn hóa theo mẫu legacy report.png trong Curency.
    /// Khổ A4 Ngang, lề 30pt (không bị trang trắng).
    /// Đánh số thứ tự tự động 1, 2, 3... bằng code trong BeforePrint.
    /// </summary>
    public partial class CurrencySummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        private int _sttCounter = 0;

        public CurrencySummaryReport()
        {
            InitializeComponent();
            this.BeforePrint += CurrencySummaryReport_BeforePrint;
        }

        private void CurrencySummaryReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _sttCounter = 0;
        }

        private void Detail_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _sttCounter++;
            this.cellStt.Text = _sttCounter.ToString();
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
    }
}
