using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Printing;
using NewaccNet.Core.Data;
using NewaccNet.Core.Helpers;
using NewaccNet.Core.Posting;
using NewaccNet.Reports.Vouchers;

namespace NewaccNet.Plugin.ErpWarehouse.Services
{
    public static class ErpInventoryPrintHelper
    {
        private const string Circular01VT = "Mẫu số 01-VT\n(Ban hành theo Thông tư số 200/2014/TT-BTC\nNgày 22/12/2014 của BTC)";
        private const string Circular02VT = "Mẫu số 02-VT\n(Ban hành theo Thông tư số 200/2014/TT-BTC\nNgày 22/12/2014 của BTC)";

        /// <summary>
        /// Khởi tạo và hiển thị DocumentPreviewWindow cho chứng từ kho.
        /// </summary>
        public static void PrintVoucher(Window owner, InventoryVoucherPrintModel model)
        {
            if (model == null) return;

            // Nạp thông tin công ty từ cấu hình hệ thống nếu model chưa có
            var comp = DbConfigService.LoadDbConfig();
            if (comp != null)
            {
                if (string.IsNullOrWhiteSpace(model.CompanyName)) model.CompanyName = comp.CompanyName;
                if (string.IsNullOrWhiteSpace(model.CompanyAddress)) model.CompanyAddress = comp.Address;
                if (string.IsNullOrWhiteSpace(model.CompanyTaxCode)) model.CompanyTaxCode = comp.TaxCode;
            }

            // Tự động chuyển đổi số tiền sang chữ
            if (string.IsNullOrWhiteSpace(model.AmountInWords))
            {
                decimal total = model.HasTax ? model.TotalPaymentAmount : model.TotalGoodsAmount;
                model.AmountInWords = NumberToWordsConverter.ConvertToWords(total, "đồng");
            }

            var report = new InventoryVoucherReport();
            report.SetModel(model);
            report.CreateDocument(false);

            var previewWindow = new DocumentPreviewWindow
            {
                Owner = owner,
                Title = $"In {model.ReportTitle} - {model.VoucherNo}"
            };
            previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
            previewWindow.PreviewControl.DocumentSource = report;
            previewWindow.Show();
        }

        /// <summary>
        /// In Phiếu nhập mua hàng hóa, vật tư (Mẫu 01-VT).
        /// </summary>
        public static void PrintPurchaseReceipt(
            Window owner,
            string voucherNo,
            DateTime voucherDate,
            string carrier,
            string vatBillNo,
            string serialNo,
            DateTime? vatBillDate,
            string partnerName,
            string partnerAddress,
            string partnerTaxCode,
            string warehouseId,
            string warehouseName,
            string inventoryAccount,
            string taxAccount,
            string totalAccount,
            IEnumerable<WarehouseVoucherLineItem> lines)
        {
            var validLines = lines?.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList() ?? new List<WarehouseVoucherLineItem>();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Phiếu chưa có dòng hàng nào hợp lệ để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal goodsAmount = validLines.Sum(l => l.Amount);
            decimal taxAmount = validLines.Sum(l => l.TaxAmount);
            decimal totalPayment = goodsAmount + taxAmount;
            bool hasTax = taxAmount > 0;

            string billText = "";
            if (!string.IsNullOrWhiteSpace(vatBillNo))
            {
                billText = $"Theo HĐ số: {vatBillNo}";
                if (!string.IsNullOrWhiteSpace(serialNo)) billText += $" (Ký hiệu: {serialNo})";
                if (vatBillDate.HasValue) billText += $" ngày {vatBillDate.Value:dd/MM/yyyy}";
            }

            string debitAcc = inventoryAccount;
            if (hasTax && !string.IsNullOrWhiteSpace(taxAccount))
            {
                debitAcc = $"{inventoryAccount}, {taxAccount}";
            }

            var model = new InventoryVoucherPrintModel
            {
                CircularNote = Circular01VT,
                ReportTitle = "PHIẾU NHẬP KHO",
                SubTitle = "(Nhập mua)",
                VoucherNo = voucherNo,
                VoucherDate = voucherDate,
                DebitAccount = debitAcc,
                CreditAccount = totalAccount,
                DelivererOrReceiverLabel = "Họ và tên người giao:",
                DelivererOrReceiverName = carrier,
                SignTitle2 = "Người giao hàng",
                BillNoAndDate = billText,
                PartnerName = partnerName,
                PartnerAddress = partnerAddress,
                PartnerTaxCode = partnerTaxCode,
                WarehouseName = warehouseName,
                WarehouseId = warehouseId,
                HasTax = hasTax,
                TotalGoodsAmount = goodsAmount,
                TotalTaxAmount = taxAmount,
                TotalPaymentAmount = totalPayment,
                Lines = validLines.Select((l, idx) => new InventoryVoucherPrintLine
                {
                    Stt = idx + 1,
                    MaterialId = l.MaterialId,
                    MaterialName = l.MaterialName,
                    Unit = l.Unit,
                    Quantity = l.Quantity,
                    Price = l.Price,
                    Amount = l.Amount
                }).ToList()
            };

            PrintVoucher(owner, model);
        }

        /// <summary>
        /// In Phiếu xuất kho bán hàng (Mẫu 02-VT).
        /// </summary>
        public static void PrintSalesDelivery(
            Window owner,
            string voucherNo,
            DateTime voucherDate,
            string receiver,
            string vatBillNo,
            string serialNo,
            DateTime? vatBillDate,
            string customerName,
            string customerAddress,
            string customerTaxCode,
            string warehouseId,
            string warehouseName,
            string revenueAccount,
            string taxAccount,
            string totalAccount,
            IEnumerable<WarehouseVoucherLineItem> lines)
        {
            var validLines = lines?.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList() ?? new List<WarehouseVoucherLineItem>();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Phiếu chưa có dòng hàng nào hợp lệ để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal goodsAmount = validLines.Sum(l => l.SellAmount > 0 ? l.SellAmount : l.Amount);
            decimal taxAmount = validLines.Sum(l => l.TaxAmount);
            decimal totalPayment = goodsAmount + taxAmount;
            bool hasTax = taxAmount > 0 || validLines.Any(l => l.Taxrateid.HasValue);

            string billText = "";
            if (!string.IsNullOrWhiteSpace(vatBillNo))
            {
                billText = $"Theo HĐ số: {vatBillNo}";
                if (!string.IsNullOrWhiteSpace(serialNo)) billText += $" (Ký hiệu: {serialNo})";
                if (vatBillDate.HasValue) billText += $" ngày {vatBillDate.Value:dd/MM/yyyy}";
            }

            string creditAcc = revenueAccount;
            if (hasTax && !string.IsNullOrWhiteSpace(taxAccount))
            {
                creditAcc = $"{revenueAccount}, {taxAccount}";
            }

            var model = new InventoryVoucherPrintModel
            {
                CircularNote = Circular02VT,
                ReportTitle = "PHIẾU XUẤT KHO",
                SubTitle = "(Xuất bán)",
                VoucherNo = voucherNo,
                VoucherDate = voucherDate,
                DebitAccount = totalAccount,
                CreditAccount = creditAcc,
                DelivererOrReceiverLabel = "Họ và tên người nhận hàng:",
                DelivererOrReceiverName = receiver,
                SignTitle2 = "Người nhận hàng",
                BillNoAndDate = billText,
                PartnerName = customerName,
                PartnerAddress = customerAddress,
                PartnerTaxCode = customerTaxCode,
                WarehouseName = warehouseName,
                WarehouseId = warehouseId,
                HasTax = hasTax,
                TotalGoodsAmount = goodsAmount,
                TotalTaxAmount = taxAmount,
                TotalPaymentAmount = totalPayment,
                Lines = validLines.Select((l, idx) => new InventoryVoucherPrintLine
                {
                    Stt = idx + 1,
                    MaterialId = l.MaterialId,
                    MaterialName = l.MaterialName,
                    Unit = l.Unit,
                    Quantity = l.Quantity,
                    Price = (l.SellPrice > 0) ? l.SellPrice : l.Price,
                    Amount = (l.SellAmount > 0) ? l.SellAmount : l.Amount
                }).ToList()
            };

            PrintVoucher(owner, model);
        }

        /// <summary>
        /// In Phiếu nhập kho nội bộ (Mẫu 01-VT).
        /// </summary>
        public static void PrintInternalReceipt(
            Window owner,
            string voucherNo,
            DateTime voucherDate,
            string carrier,
            string partnerName,
            string warehouseId,
            string warehouseName,
            string inventoryAccount,
            string offsetAccount,
            IEnumerable<WarehouseVoucherLineItem> lines)
        {
            var validLines = lines?.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList() ?? new List<WarehouseVoucherLineItem>();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Phiếu chưa có dòng hàng nào hợp lệ để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal goodsAmount = validLines.Sum(l => l.Amount);

            var model = new InventoryVoucherPrintModel
            {
                CircularNote = Circular01VT,
                ReportTitle = "PHIẾU NHẬP KHO",
                SubTitle = "(Nhập nội bộ)",
                VoucherNo = voucherNo,
                VoucherDate = voucherDate,
                DebitAccount = inventoryAccount,
                CreditAccount = offsetAccount,
                DelivererOrReceiverLabel = "Họ và tên người giao:",
                DelivererOrReceiverName = carrier,
                SignTitle2 = "Người giao hàng",
                PartnerName = partnerName,
                WarehouseName = warehouseName,
                WarehouseId = warehouseId,
                HasTax = false,
                TotalGoodsAmount = goodsAmount,
                TotalPaymentAmount = goodsAmount,
                Lines = validLines.Select((l, idx) => new InventoryVoucherPrintLine
                {
                    Stt = idx + 1,
                    MaterialId = l.MaterialId,
                    MaterialName = l.MaterialName,
                    Unit = l.Unit,
                    Quantity = l.Quantity,
                    Price = l.Price,
                    Amount = l.Amount
                }).ToList()
            };

            PrintVoucher(owner, model);
        }

        /// <summary>
        /// In Phiếu xuất kho nội bộ (Mẫu 02-VT).
        /// </summary>
        public static void PrintInternalIssue(
            Window owner,
            string voucherNo,
            DateTime voucherDate,
            string receiver,
            string partnerName,
            string warehouseId,
            string warehouseName,
            string offsetAccount,
            string inventoryAccount,
            IEnumerable<WarehouseVoucherLineItem> lines)
        {
            var validLines = lines?.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList() ?? new List<WarehouseVoucherLineItem>();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Phiếu chưa có dòng hàng nào hợp lệ để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal goodsAmount = validLines.Sum(l => l.Amount);

            var model = new InventoryVoucherPrintModel
            {
                CircularNote = Circular02VT,
                ReportTitle = "PHIẾU XUẤT KHO",
                SubTitle = "(Xuất nội bộ)",
                VoucherNo = voucherNo,
                VoucherDate = voucherDate,
                DebitAccount = offsetAccount,
                CreditAccount = inventoryAccount,
                DelivererOrReceiverLabel = "Họ và tên người nhận hàng:",
                DelivererOrReceiverName = receiver,
                SignTitle2 = "Người nhận hàng",
                PartnerName = partnerName,
                WarehouseName = warehouseName,
                WarehouseId = warehouseId,
                HasTax = false,
                TotalGoodsAmount = goodsAmount,
                TotalPaymentAmount = goodsAmount,
                Lines = validLines.Select((l, idx) => new InventoryVoucherPrintLine
                {
                    Stt = idx + 1,
                    MaterialId = l.MaterialId,
                    MaterialName = l.MaterialName,
                    Unit = l.Unit,
                    Quantity = l.Quantity,
                    Price = l.Price,
                    Amount = l.Amount
                }).ToList()
            };

            PrintVoucher(owner, model);
        }

        /// <summary>
        /// In Phiếu xuất kho kiêm điều chuyển nội bộ (Mẫu 02-VT).
        /// </summary>
        public static void PrintWarehouseTransfer(
            Window owner,
            string voucherNo,
            DateTime voucherDate,
            string carrier,
            string fromWarehouseId,
            string fromWarehouseName,
            string toWarehouseId,
            string toWarehouseName,
            string debitAccount,
            string creditAccount,
            IEnumerable<WarehouseVoucherLineItem> lines)
        {
            var validLines = lines?.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList() ?? new List<WarehouseVoucherLineItem>();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Phiếu chưa có dòng hàng nào hợp lệ để in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal goodsAmount = validLines.Sum(l => l.Amount);
            string whDescription = $"{fromWarehouseName} (Kho đi) ➔ {toWarehouseName} (Kho đến)";

            var model = new InventoryVoucherPrintModel
            {
                CircularNote = Circular02VT,
                ReportTitle = "PHIẾU XUẤT KHO KIÊM ĐIỀU CHUYỂN NỘI BỘ",
                SubTitle = "(Điều chuyển kho)",
                VoucherNo = voucherNo,
                VoucherDate = voucherDate,
                DebitAccount = debitAccount,
                CreditAccount = creditAccount,
                DelivererOrReceiverLabel = "Họ và tên người vận chuyển:",
                DelivererOrReceiverName = carrier,
                SignTitle2 = "Người vận chuyển",
                WarehouseName = whDescription,
                WarehouseId = $"{fromWarehouseId} ➔ {toWarehouseId}",
                HasTax = false,
                TotalGoodsAmount = goodsAmount,
                TotalPaymentAmount = goodsAmount,
                Lines = validLines.Select((l, idx) => new InventoryVoucherPrintLine
                {
                    Stt = idx + 1,
                    MaterialId = l.MaterialId,
                    MaterialName = l.MaterialName,
                    Unit = l.Unit,
                    Quantity = l.Quantity,
                    Price = l.Price,
                    Amount = l.Amount
                }).ToList()
            };

            PrintVoucher(owner, model);
        }
    }
}
