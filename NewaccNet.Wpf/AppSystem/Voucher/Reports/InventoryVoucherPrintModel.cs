using System;
using System.Collections.Generic;

namespace NewaccNet.Wpf.AppSystem.Voucher.Reports
{
    public class InventoryVoucherPrintLine
    {
        public int Stt { get; set; }
        public string MaterialName { get; set; } = "";
        public string MaterialId { get; set; } = "";
        public string Unit { get; set; } = "";
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }

    public class InventoryVoucherPrintModel
    {
        // Thông tin công ty
        public string CompanyName { get; set; } = "CÔNG TY BÌNH MINH";
        public string CompanyAddress { get; set; } = "5/8 Đông Ngạc";
        public string CompanyTaxCode { get; set; } = "010100101";

        // Tiêu đề & Thông tư
        public string CircularNote { get; set; } = "Mẫu số 01-VT\n(Ban hành theo Thông tư số 200/2014/TT-BTC\nNgày 22/12/2014 của Bộ Tài chính)";
        public string ReportTitle { get; set; } = "PHIẾU NHẬP KHO";
        public string SubTitle { get; set; } = ""; // (Nhập mua), (Nhập nội bộ), (Xuất bán), (Xuất nội bộ)
        public string VoucherNo { get; set; } = "";
        public DateTime VoucherDate { get; set; } = DateTime.Today;

        // Định khoản
        public string DebitAccount { get; set; } = "";
        public string CreditAccount { get; set; } = "";

        // Thông tin chung từ Header của form
        public string DelivererOrReceiverLabel { get; set; } = "Họ và tên người giao:";
        public string DelivererOrReceiverName { get; set; } = "";
        public string BillNoAndDate { get; set; } = "";
        public string PartnerName { get; set; } = "";
        public string PartnerAddress { get; set; } = "";
        public string PartnerTaxCode { get; set; } = "";
        public string WarehouseName { get; set; } = "";
        public string WarehouseId { get; set; } = "";
        public string Reason { get; set; } = "";

        // Danh sách dòng vật tư
        public List<InventoryVoucherPrintLine> Lines { get; set; } = new List<InventoryVoucherPrintLine>();

        // Thuế & Tổng tiền
        public bool HasTax { get; set; } = false;
        public string TaxRateText { get; set; } = "Thuế GTGT";
        public double TotalGoodsAmount { get; set; }
        public double TotalTaxAmount { get; set; }
        public double TotalPaymentAmount { get; set; }

        public string AmountInWords { get; set; } = "";
        public string OriginalVouchersCount { get; set; } = "";

        // Chữ ký
        public string SignTitle2 { get; set; } = "Người giao hàng";
    }
}
