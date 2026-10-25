using System;
using System.Collections.Generic;

namespace NewaccNet.AI.Contracts
{
    public class OcrInvoiceResultDto
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceSeries { get; set; } = string.Empty;
        public string? InvoiceDate { get; set; } // yyyy-MM-dd
        public string InvoiceType { get; set; } = "Hóa đơn GTGT";

        public string SellerName { get; set; } = string.Empty;
        public string SellerTaxCode { get; set; } = string.Empty;
        public string SellerAddress { get; set; } = string.Empty;

        public string BuyerName { get; set; } = string.Empty;
        public string BuyerTaxCode { get; set; } = string.Empty;
        public string BuyerAddress { get; set; } = string.Empty;

        public decimal TotalAmountWithoutTax { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OcrInvoiceItemDto> Items { get; set; } = new();
        public string RawRecognizedText { get; set; } = string.Empty;
    }

    public class OcrInvoiceItemDto
    {
        public int LineNo { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string TaxRate { get; set; } = string.Empty;
        public decimal TaxAmount { get; set; }
        public string? SuggestedAccount { get; set; }
        public string? SuggestedWarehouse { get; set; }
    }
}
