using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Material
{
    /// <summary>
    /// Sổ chi tiết Vật tư, dụng cụ, sản phẩm, hàng hóa (Mẫu số S10-DN theo TT 200/2014/TT-BTC).
    /// Hỗ trợ Visual Designer trong Visual Studio.
    /// Cho phép Drill-down cấp 2: Click vào dòng chứng từ để mở thẳng VoucherEntryWindow.
    /// </summary>
    public partial class MaterialDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public MaterialDetailReport()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Khởi tạo các thông số tiêu đề của Sổ chi tiết vật tư.
        /// </summary>
        public void SetReportHeader(
            string accountId,
            string accountName,
            string warehouseId,
            string warehouseName,
            string materialId,
            string materialName,
            string unit,
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

            if (Parameters["prmPeriodTitle"] != null)
            {
                Parameters["prmPeriodTitle"].Value = $"Từ ngày: {fromDate:dd/MM/yyyy}  Đến ngày: {toDate:dd/MM/yyyy}";
            }

            if (Parameters["prmAccountInfo"] != null)
            {
                Parameters["prmAccountInfo"].Value = $"Tài khoản: {accountId} - {accountName}";
            }

            if (Parameters["prmWarehouseInfo"] != null)
            {
                Parameters["prmWarehouseInfo"].Value = $"Kho hàng: {warehouseId} - {warehouseName}";
            }

            if (Parameters["prmMaterialInfo"] != null)
            {
                string uStr = string.IsNullOrWhiteSpace(unit) ? string.Empty : $" (ĐVT: {unit})";
                Parameters["prmMaterialInfo"].Value = $"Vật tư: {materialId} - {materialName}{uStr}";
            }
        }
    }
}
