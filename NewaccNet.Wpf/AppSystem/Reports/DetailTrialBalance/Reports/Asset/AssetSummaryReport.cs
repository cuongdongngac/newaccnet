using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset
{
    /// <summary>
    /// Báo cáo Tổng hợp Tài sản cố định (Tăng - Giảm - Dư TSCĐ).
    /// Khổ giấy A4 Ngang, thiết kế tương tự Báo cáo Tổng hợp Vật tư.
    /// Theo mẫu AccessReport (Tài khoản 211x).
    /// </summary>
    public partial class AssetSummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        public AssetSummaryReport()
        {
            InitializeComponent();
        }

        public void SetReportHeader(
            string accountId,
            string accountName,
            DateTime fromDate,
            DateTime toDate,
            string? companyName = null)
        {
            if (!string.IsNullOrWhiteSpace(companyName) && lblCompany != null)
            {
                lblCompany.Text = companyName;
            }

            if (lblTitle != null)
            {
                lblTitle.Text = $"TÀI KHOẢN {accountId}";
            }

            if (lblDate != null)
            {
                lblDate.Text = $"Từ ngày: {fromDate:dd/MM/yy} Đến ngày: {toDate:dd/MM/yy}";
            }
        }
    }
}
