using System;
using DevExpress.XtraReports.UI;
using NewaccNet.Reports;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Currency
{
    /// <summary>
    /// Sổ chi tiết Ngoại tệ (CurrencyDetailReport).
    /// Khổ A4 Ngang, lề 30pt.
    /// Click đúp vào dòng chứng từ để mở VoucherEntryWindow (Drill-down cấp 2).
    /// </summary>
    public partial class CurrencyDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        private int _sttCounter = 0;

        public CurrencyDetailReport()
        {
            InitializeComponent();
            this.BeforePrint += CurrencyDetailReport_BeforePrint;
        }

        private void CurrencyDetailReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _sttCounter = 0;
        }

        private void Detail_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var row = GetCurrentRow() as CurrencyDetailReportDTO;
            if (row != null && row.IsOpeningRow)
            {
                // Dòng số dư đầu kỳ: để trống STT, in đậm diễn giải
                this.cellStt.Text = string.Empty;
                this.cellContents.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            }
            else
            {
                _sttCounter++;
                this.cellStt.Text = _sttCounter.ToString();
                this.cellContents.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Regular);
            }
        }

        public void SetReportHeader(
            string accountId,
            string accountName,
            string currencyId,
            string currencyName,
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

            if (lblPeriod != null)
            {
                lblPeriod.Text = $"Từ ngày {fromDate:dd/MM/yyyy} Đến ngày {toDate:dd/MM/yyyy}";
            }

            if (lblAccount != null)
            {
                lblAccount.Text = $"Tài khoản: {accountId} - {accountName}";
            }

            if (lblCurrency != null)
            {
                lblCurrency.Text = $"Ngoại tệ: {currencyId} - {currencyName}";
            }
        }
    }
}
