using System;
using DevExpress.XtraReports.UI;
using NewaccNet.Reports;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset
{
    /// <summary>
    /// Báo cáo Biến động Tài sản Cố định (AssetMovementReport).
    /// Khổ A4 Ngang, lề 30pt.
    /// Cấu trúc Grouping 2 cấp:
    ///   - Group 1: Lý do tăng/giảm (Reason) - Đánh số La Mã: I, II, III...
    ///   - Group 2: Tài sản (Asset) - Đánh số: 1, 2, 3... (reset theo từng Lý do)
    ///   - Detail: Chứng từ chi tiết - Đánh số: 1, 2, 3... (reset theo từng Tài sản)
    /// Drill-down: Click đúp vào dòng chứng từ chi tiết mở VoucherEntryWindow.
    /// </summary>
    public partial class AssetMovementReport : DevExpress.XtraReports.UI.XtraReport
    {
        private int _reasonCounter = 0;
        private int _assetCounter = 0;
        private int _detailCounter = 0;

        public AssetMovementReport()
        {
            InitializeComponent();
            this.BeforePrint += AssetMovementReport_BeforePrint;
        }

        private void AssetMovementReport_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _reasonCounter = 0;
            _assetCounter = 0;
            _detailCounter = 0;
        }

        private void GroupHeaderReason_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _reasonCounter++;
            _assetCounter = 0;
            _detailCounter = 0;
            this.tg1CellStt.Text = ToRoman(_reasonCounter);
        }

        private void GroupHeaderAsset_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _assetCounter++;
            _detailCounter = 0;
            this.tg2CellStt.Text = $"{_reasonCounter}.{_assetCounter}";

            // Hiển thị thông tin chi tiết về tài sản: Nguyên giá, Thời gian sử dụng, Bộ phận
            var row = GetCurrentRow() as AssetMovementReportDTO;
            if (row != null)
            {
                string origPriceStr = row.OriginalPrice.HasValue ? $"{row.OriginalPrice.Value:#,##0} đ" : "-";
                string usingTimeStr = row.UsingTimeYears.HasValue ? $"{row.UsingTimeYears.Value} năm" : "-";
                string deptStr = !string.IsNullOrWhiteSpace(row.DepartmentName) ? row.DepartmentName : "Chưa xác định";

                this.tg2CellAsset.Text = $"Tài sản: {row.AssetDisplayName}   |   Nguyên giá: {origPriceStr}   |   Thời gian SD: {usingTimeStr}   |   Bộ phận: {deptStr}";
            }
        }

        private void Detail_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _detailCounter++;
            this.cellStt.Text = _detailCounter.ToString();
        }

        private static string ToRoman(int number)
        {
            if (number < 1) return string.Empty;
            string[] romans = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV" };
            return (number <= romans.Length) ? romans[number - 1] : number.ToString();
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

            if (lblAccount != null)
            {
                lblAccount.Text = $"Tài khoản: {accountId} - {accountName}";
            }

            if (lblDate != null)
            {
                lblDate.Text = $"Từ ngày {fromDate:dd/MM/yyyy} Đến ngày {toDate:dd/MM/yyyy}";
            }

            if (lblDateSig != null)
            {
                lblDateSig.Text = $"Ngày {toDate:dd} tháng {toDate:MM} năm {toDate:yyyy}";
            }
        }
    }
}
