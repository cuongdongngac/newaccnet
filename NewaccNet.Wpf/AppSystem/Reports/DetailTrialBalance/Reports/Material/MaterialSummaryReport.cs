using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Material
{
    /// <summary>
    /// Báo cáo Tổng hợp Nhập - Xuất - Tồn (XtraReport hỗ trợ Visual Designer).
    /// Chuẩn hóa theo mẫu chuẩn kế toán và ảnh mẫu giao diện người dùng:
    /// - Tiêu đề chính: TÀI KHOẢN [AccountId]
    /// - Tiêu đề phụ: CT [WarehouseName]
    /// - Dòng ngày: Từ ngày: [FromDate:dd/MM/yy] Đến ngày: [ToDate:dd/MM/yy]
    /// - Tự động ẩn thanh nhóm kho khi lọc 1 kho để bảng báo cáo trực quan, liền mạch.
    /// </summary>
    public partial class MaterialSummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        public MaterialSummaryReport()
        {
            InitializeComponent();
        }

        public void SetReportHeader(
            string accountId,
            string accountName,
            DateTime fromDate,
            DateTime toDate,
            string? companyName = null,
            string? warehouseName = null)
        {
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                if (Parameters["prmCompanyName"] != null)
                {
                    Parameters["prmCompanyName"].Value = companyName;
                }
                if (Parameters["companyName"] != null)
                {
                    Parameters["companyName"].Value = companyName;
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

            if (lblWarehouse != null)
            {
                if (!string.IsNullOrWhiteSpace(warehouseName))
                {
                    lblWarehouse.Text = warehouseName.StartsWith("CT ", StringComparison.OrdinalIgnoreCase)
                        ? warehouseName
                        : $"CT {warehouseName}";
                    lblWarehouse.Visible = true;

                    // Khi đã hiển thị tên kho ở tiêu đề báo cáo, ẩn thanh nhóm lặp lại
                    if (GroupHeaderWarehouse != null)
                    {
                        GroupHeaderWarehouse.Visible = false;
                    }
                    if (GroupFooterWarehouse != null)
                    {
                        GroupFooterWarehouse.Visible = false;
                    }
                }
                else
                {
                    lblWarehouse.Text = "Toàn bộ các kho";
                    lblWarehouse.Visible = true;

                    if (GroupHeaderWarehouse != null)
                    {
                        GroupHeaderWarehouse.Visible = true;
                    }
                    if (GroupFooterWarehouse != null)
                    {
                        GroupFooterWarehouse.Visible = true;
                    }
                }
            }

            if (lblDate != null)
            {
                lblDate.Text = $"Từ ngày: {fromDate:dd/MM/yy} Đến ngày: {toDate:dd/MM/yy}";
            }
        }
    }
}
