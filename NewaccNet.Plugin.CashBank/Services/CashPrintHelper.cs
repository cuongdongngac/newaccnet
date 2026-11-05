using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Printing;
using NewaccNet.Core.Data;
using NewaccNet.Core.Helpers;
using NewaccNet.Reports.Vouchers;

namespace NewaccNet.Plugin.CashBank.Services
{
    public static class CashPrintHelper
    {
        public static void PrintReceipt(Window? owner, CashVoucherSaveParams p)
        {
            if (p == null) return;

            var comp = DbConfigService.LoadDbConfig();
            var model = new CashVoucherPrintModel
            {
                CompanyName = comp?.CompanyName ?? "",
                CompanyAddress = comp?.Address ?? "",
                CompanyTaxCode = comp?.TaxCode ?? "",
                VoucherNo = p.VoucherNo,
                VoucherDate = p.VoucherDate,
                PersonName = p.PersonName,
                PersonAddress = p.PersonAddress,
                Contents = p.Description,
                Amount = p.TotalAmount,
                OriginalVouchersCount = p.InvoicesNumber,
                AmountInWords = NumberToWordsConverter.ConvertToWords(p.TotalAmount, "đồng"),
                DebitAccountsText = $"Nợ: {p.CashAccountId}",
                CreditAccountsText = $"Có: {string.Join(", ", p.Lines.Select(l => l.AccountId).Where(a => !string.IsNullOrEmpty(a)).Distinct())}"
            };

            var report = new CashReceiptReport();
            report.SetModel(model);
            report.CreateDocument(false);

            var preview = new DocumentPreviewWindow
            {
                Owner = owner,
                Title = $"In Phiếu thu - {p.VoucherNo}"
            };
            preview.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
            preview.PreviewControl.DocumentSource = report;
            preview.Show();
        }

        public static void PrintPayment(Window? owner, CashVoucherSaveParams p)
        {
            if (p == null) return;

            var comp = DbConfigService.LoadDbConfig();
            var model = new CashVoucherPrintModel
            {
                CompanyName = comp?.CompanyName ?? "",
                CompanyAddress = comp?.Address ?? "",
                CompanyTaxCode = comp?.TaxCode ?? "",
                VoucherNo = p.VoucherNo,
                VoucherDate = p.VoucherDate,
                PersonName = p.PersonName,
                PersonAddress = p.PersonAddress,
                Contents = p.Description,
                Amount = p.TotalAmount,
                OriginalVouchersCount = p.InvoicesNumber,
                AmountInWords = NumberToWordsConverter.ConvertToWords(p.TotalAmount, "đồng"),
                DebitAccountsText = $"Nợ: {string.Join(", ", p.Lines.Select(l => l.AccountId).Where(a => !string.IsNullOrEmpty(a)).Distinct())}",
                CreditAccountsText = $"Có: {p.CashAccountId}"
            };

            var report = new CashPaymentReport();
            report.SetModel(model);
            report.CreateDocument(false);

            var preview = new DocumentPreviewWindow
            {
                Owner = owner,
                Title = $"In Phiếu chi - {p.VoucherNo}"
            };
            preview.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
            preview.PreviewControl.DocumentSource = report;
            preview.Show();
        }
    }
}
