using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Voucher.Reports
{
    public partial class InventoryVoucherReport : DevExpress.XtraReports.UI.XtraReport
    {
        public InventoryVoucherReport()
        {
            InitializeComponent();
        }

        public void SetModel(InventoryVoucherPrintModel model)
        {
            if (model == null) return;

            // Parameter: Mẫu số & Thông tư
            if (!string.IsNullOrWhiteSpace(model.CircularNote))
            {
                pCircularNote.Value = model.CircularNote;
            }

            // Company info
            if (!string.IsNullOrWhiteSpace(model.CompanyName)) lblCompany.Text = model.CompanyName;
            if (!string.IsNullOrWhiteSpace(model.CompanyAddress)) lblAddress.Text = $"Địa chỉ: {model.CompanyAddress}";
            if (!string.IsNullOrWhiteSpace(model.CompanyTaxCode)) lblTaxCode.Text = $"Mã số thuế: {model.CompanyTaxCode}";

            // Header titles & date
            lblTitle.Text = model.ReportTitle;
            lblSubTitle.Text = string.IsNullOrWhiteSpace(model.SubTitle) ? "" : model.SubTitle;
            lblDate.Text = $"Ngày {model.VoucherDate:dd} tháng {model.VoucherDate:MM} năm {model.VoucherDate:yyyy}";
            lblSignDate.Text = $"Ngày {model.VoucherDate:dd} tháng {model.VoucherDate:MM} năm {model.VoucherDate:yyyy}";

            lblVoucherNo.Text = $"Số: {model.VoucherNo}";
            lblDebitAcc.Text = string.IsNullOrWhiteSpace(model.DebitAccount) ? "Nợ:" : $"Nợ: {model.DebitAccount}";
            lblCreditAcc.Text = string.IsNullOrWhiteSpace(model.CreditAccount) ? "Có:" : $"Có: {model.CreditAccount}";

            // Metadata Lines (mô phỏng Header form)
            lblCarrier.Text = $"{model.DelivererOrReceiverLabel}  {model.DelivererOrReceiverName}";

            if (!string.IsNullOrWhiteSpace(model.BillNoAndDate))
            {
                lblBill.Text = model.BillNoAndDate;
                lblBill.Visible = true;
            }
            else
            {
                lblBill.Text = "Theo hóa đơn số: ........ ngày ........ tháng ........ năm ........";
            }

            string partnerInfo = model.PartnerName;
            if (!string.IsNullOrWhiteSpace(model.PartnerTaxCode)) partnerInfo += $" - MST: {model.PartnerTaxCode}";
            if (!string.IsNullOrWhiteSpace(model.PartnerAddress)) partnerInfo += $" (Đ/c: {model.PartnerAddress})";
            lblPartner.Text = $"Đơn vị / Người mua bán:  {partnerInfo}";

            string whText = model.WarehouseName;
            if (!string.IsNullOrWhiteSpace(model.WarehouseId)) whText += $" (Mã kho: {model.WarehouseId})";
            lblWarehouse.Text = $"Nhập / Xuất tại kho:  {whText}";

            // Table Footer (Cân khớp với bảng và xử lý Thuế khi mua bán)
            if (model.HasTax)
            {
                colF_GoodsLabel.Text = "Cộng tiền hàng:";
                colF_GoodsAmount.Text = $"{model.TotalGoodsAmount:N0} đ";

                rowFooterTax.Visible = true;
                string taxRateDesc = string.IsNullOrWhiteSpace(model.TaxRateText) ? "Thuế GTGT" : model.TaxRateText;
                colF_TaxLabel.Text = $"Tiền thuế GTGT ({taxRateDesc}):";
                colF_TaxAmount.Text = $"{model.TotalTaxAmount:N0} đ";

                rowFooterTotal.Visible = true;
                colF_TotalLabel.Text = "Tổng cộng tiền thanh toán:";
                colF_TotalAmount.Text = $"{model.TotalPaymentAmount:N0} đ";
            }
            else
            {
                colF_GoodsLabel.Text = "Cộng:";
                colF_GoodsAmount.Text = $"{model.TotalGoodsAmount:N0} đ";

                rowFooterTax.Visible = false;
                rowFooterTotal.Visible = false;
            }

            // Amount in words & attachments
            lblAmountInWords.Text = $"Tổng số tiền (viết bằng chữ):  {model.AmountInWords}";
            string attach = string.IsNullOrWhiteSpace(model.OriginalVouchersCount) ? "........" : model.OriginalVouchersCount;
            lblAttach.Text = $"Kèm theo:  {attach}  chứng từ gốc";

            // Sign column 2 title (Người giao hàng / Người nhận hàng)
            if (!string.IsNullOrWhiteSpace(model.SignTitle2))
            {
                lblSign2.Text = model.SignTitle2;
            }

            // Bind Lines to DataSource
            this.DataSource = model.Lines;
        }
    }
}
