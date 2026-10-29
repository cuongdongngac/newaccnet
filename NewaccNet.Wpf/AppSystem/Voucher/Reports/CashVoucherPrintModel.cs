using System;

namespace NewaccNet.Wpf.AppSystem.Voucher.Reports
{
    public class CashVoucherPrintModel
    {
        public string CompanyName { get; set; } = "";
        public string CompanyAddress { get; set; } = "";
        public string CompanyTaxCode { get; set; } = "";

        public string CircularNote { get; set; } = "Mẫu số 01-TT\n(Ban hành theo TT số 200/2014/TT-BTC\nNgày 22/12/2014 của Bộ Tài chính)";
        public string BookNo { get; set; } = "";
        public string VoucherNo { get; set; } = "";
        public DateTime VoucherDate { get; set; } = DateTime.Today;

        public string PersonName { get; set; } = "";
        public string PersonAddress { get; set; } = "";
        public string Contents { get; set; } = "";
        public decimal Amount { get; set; }
        public string AmountInWords { get; set; } = "";
        public string OriginalVouchersCount { get; set; } = "";

        // Định khoản Nợ / Có
        public string DebitAccountsText { get; set; } = "";
        public string CreditAccountsText { get; set; } = "";
    }
}
