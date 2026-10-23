using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset
{
    /// <summary>
    /// Sổ chi tiết Tài sản cố định (Thẻ TSCĐ).
    /// Khổ giấy A4 Ngang, hỗ trợ Drill-down mở VoucherEntryWindow khi click vào dòng chứng từ.
    /// </summary>
    public partial class AssetDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public AssetDetailReport()
        {
            InitializeComponent();
        }

        public void SetReportHeader(
            string accountId,
            string accountName,
            string assetId,
            string assetName,
            string unit,
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
                lblTitle.Text = $"SỔ CHI TIẾT TÀI SẢN CỐ ĐỊNH";
            }

            if (lblSubtitle != null)
            {
                lblSubtitle.Text = $"Tài sản: {assetId} - {assetName} (ĐVT: {unit}) | Tài khoản: {accountId}";
            }

            if (lblDate != null)
            {
                lblDate.Text = $"Từ ngày: {fromDate:dd/MM/yyyy} Đến ngày: {toDate:dd/MM/yyyy}";
            }
        }
    }
}
