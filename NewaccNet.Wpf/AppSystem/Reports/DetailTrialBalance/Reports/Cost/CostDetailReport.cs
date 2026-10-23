using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Cost
{
    /// <summary>
    /// Sổ chi tiết Chi phí theo Đối tượng tập hợp CP và Yếu tố CP.
    /// Hỗ trợ Visual Designer trong Visual Studio.
    /// Cho phép Drill-down cấp 2: Click vào dòng chứng từ để mở thẳng VoucherEntryWindow.
    /// </summary>
    public partial class CostDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public CostDetailReport()
        {
            InitializeComponent();
        }

        public void SetReportHeader(
            string accountId,
            string accountName,
            string costObjectId,
            string costObjectName,
            string? costElementId,
            string? costElementName,
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
                lblPeriod.Text = $"Từ ngày: {fromDate:dd/MM/yyyy}  Đến ngày: {toDate:dd/MM/yyyy}";
            }

            if (lblAccount != null)
            {
                lblAccount.Text = $"Tài khoản: {accountId} - {accountName}";
            }

            if (lblCostObject != null)
            {
                lblCostObject.Text = $"Đối tượng chi phí: {costObjectId} - {costObjectName}";
            }

            if (lblCostElement != null)
            {
                if (!string.IsNullOrWhiteSpace(costElementName))
                {
                    lblCostElement.Text = $"Yếu tố chi phí: {costElementId} - {costElementName}";
                    lblCostElement.Visible = true;
                }
                else
                {
                    lblCostElement.Text = string.Empty;
                    lblCostElement.Visible = false;
                }
            }
        }
    }
}
