using System;
using DevExpress.XtraReports.UI;

namespace NewaccNet.Wpf.AppSystem.Voucher.Reports
{
    public partial class CashReceiptReport : DevExpress.XtraReports.UI.XtraReport
    {
        public CashReceiptReport()
        {
            InitializeComponent();
        }

        public void SetModel(CashVoucherPrintModel model)
        {
            if (model == null) return;

            // Parameter: Mẫu số & Thông tư
            if (!string.IsNullOrWhiteSpace(model.CircularNote))
            {
                pCircularNote.Value = model.CircularNote;
            }

            // Gán dữ liệu cho cả Liên 1 và Liên 2
            BindPanel(
                lblCompany1, lblAddress1, lblTaxCode1, lblBookNo1, lblVoucherNo1,
                lblDebitAcc1, lblCreditAcc1, lblDate1, lblPerson1, lblPersonAddress1,
                lblReason1, lblAmount1, lblAmountWords1, lblAttach1, lblSignDate1,
                model);

            BindPanel(
                lblCompany2, lblAddress2, lblTaxCode2, lblBookNo2, lblVoucherNo2,
                lblDebitAcc2, lblCreditAcc2, lblDate2, lblPerson2, lblPersonAddress2,
                lblReason2, lblAmount2, lblAmountWords2, lblAttach2, lblSignDate2,
                model);
        }

        private void BindPanel(
            XRLabel comp, XRLabel addr, XRLabel mst, XRLabel book, XRLabel vNo,
            XRLabel drAcc, XRLabel crAcc, XRLabel date, XRLabel pers, XRLabel pAddr,
            XRLabel rsn, XRLabel amt, XRLabel amtWords, XRLabel att, XRLabel sDate,
            CashVoucherPrintModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.CompanyName)) comp.Text = model.CompanyName;
            if (!string.IsNullOrWhiteSpace(model.CompanyAddress)) addr.Text = $"Địa chỉ: {model.CompanyAddress}";
            if (!string.IsNullOrWhiteSpace(model.CompanyTaxCode)) mst.Text = $"Mã số thuế: {model.CompanyTaxCode}";

            book.Text = string.IsNullOrWhiteSpace(model.BookNo) ? "Quyển số: .............." : $"Quyển số: {model.BookNo}";
            vNo.Text = $"Số: {model.VoucherNo}";

            drAcc.Text = string.IsNullOrWhiteSpace(model.DebitAccountsText) ? "Nợ:" : model.DebitAccountsText;
            crAcc.Text = string.IsNullOrWhiteSpace(model.CreditAccountsText) ? "Có:" : model.CreditAccountsText;

            date.Text = $"Ngày {model.VoucherDate:dd} tháng {model.VoucherDate:MM} năm {model.VoucherDate:yyyy}";
            sDate.Text = $"Ngày {model.VoucherDate:dd} tháng {model.VoucherDate:MM} năm {model.VoucherDate:yyyy}";

            pers.Text = $"Họ tên người nộp tiền:  {model.PersonName}";
            pAddr.Text = $"Địa chỉ:  {model.PersonAddress}";
            rsn.Text = $"Lý do nộp:  {model.Contents}";
            amt.Text = $"Số tiền:  {model.Amount:N0} đ";
            amtWords.Text = $"Viết bằng chữ:  {model.AmountInWords}";

            string attachCount = string.IsNullOrWhiteSpace(model.OriginalVouchersCount) ? "........" : model.OriginalVouchersCount;
            att.Text = $"Kèm theo:  {attachCount}  chứng từ gốc";
        }
    }
}
