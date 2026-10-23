using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace NewaccNet.TaxInvoice
{
    public static class TaxInvoiceParser
    {
        public static TaxInvoiceDto? ParseFromJson(string rawJson, string direction, string? jsonFilePath = null, string? xmlFilePath = null)
        {
            if (string.IsNullOrWhiteSpace(rawJson)) return null;

            try
            {
                using var doc = JsonDocument.Parse(rawJson);
                var root = doc.RootElement;
                JsonElement invoiceElement = root;

                if (root.TryGetProperty("datas", out var datasProp))
                {
                    if (datasProp.ValueKind == JsonValueKind.Array && datasProp.GetArrayLength() > 0)
                    {
                        invoiceElement = datasProp[0];
                    }
                    else if (datasProp.ValueKind == JsonValueKind.Object)
                    {
                        invoiceElement = datasProp;
                    }
                }

                var dto = new TaxInvoiceDto
                {
                    Direction = direction.ToLowerInvariant(),
                    SellerTaxCode = CleanString(GetString(invoiceElement, "nbmst")),
                    InvoiceTemplate = CleanString(GetString(invoiceElement, "khmshdon")),
                    InvoiceSeries = CleanString(GetString(invoiceElement, "khhdon")).ToUpperInvariant(),
                    InvoiceNo = ParseLong(GetString(invoiceElement, "shdon")),
                    InvoiceDate = ParseDate(GetString(invoiceElement, "tdlap")),
                    SignedDate = ParseNullableDate(GetString(invoiceElement, "nky")),
                    SellerName = CleanString(GetString(invoiceElement, "nbten")),
                    SellerAddress = CleanString(GetString(invoiceElement, "nbdchi")),
                    BuyerTaxCode = CleanString(GetString(invoiceElement, "nmmst")),
                    BuyerName = CleanString(GetString(invoiceElement, "nmten")),
                    BuyerAddress = CleanString(GetString(invoiceElement, "nmdchi")),
                    CurrencyId = CleanString(GetString(invoiceElement, "dvtte")) ?? "VND",
                    ExchangeRate = ParseDouble(GetString(invoiceElement, "tgia"), 1.0),
                    TotalAmountWithoutTax = ParseDecimal(GetString(invoiceElement, "tgtcthue")),
                    TotalTaxAmount = ParseDecimal(GetString(invoiceElement, "tgtthue")),
                    TotalAmount = ParseDecimal(GetString(invoiceElement, "tgtttbso")),
                    TotalDiscountAmount = ParseDecimal(GetString(invoiceElement, "ttcktmai")),
                    InvoiceStatus = (int)ParseLong(GetString(invoiceElement, "tthai")),
                    ProcessStatus = (int)ParseLong(GetString(invoiceElement, "ttxly")),
                    InvoiceSource = CleanString(GetString(invoiceElement, "source")) ?? "query",
                    TaxAuthorityCode = CleanString(GetString(invoiceElement, "mhdon")),
                    RawJsonPath = jsonFilePath,
                    XmlFilePath = xmlFilePath,
                    DownloadedAt = DateTime.Now
                };

                // Tax Rates: thttltsuat
                if (invoiceElement.TryGetProperty("thttltsuat", out var taxRatesArray) && taxRatesArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var tr in taxRatesArray.EnumerateArray())
                    {
                        var rate = CleanString(GetString(tr, "tsuat")) ?? "0%";
                        var amountWithoutTax = ParseDecimal(GetString(tr, "thtien"));
                        var taxAmount = ParseDecimal(GetString(tr, "tthue"));

                        dto.TaxRates.Add(new TaxInvoiceTaxRateDto
                        {
                            TaxRate = rate,
                            AmountWithoutTax = amountWithoutTax,
                            TaxAmount = taxAmount
                        });
                    }
                }

                // Line Items: hdhhdvu
                if (invoiceElement.TryGetProperty("hdhhdvu", out var detailsArray) && detailsArray.ValueKind == JsonValueKind.Array)
                {
                    int line = 1;
                    foreach (var d in detailsArray.EnumerateArray())
                    {
                        dto.Details.Add(new TaxInvoiceDetailDto
                        {
                            LineNo = (int)ParseLong(GetString(d, "stt"), line),
                            ItemCode = CleanString(GetString(d, "mhhdvu")),
                            ItemName = CleanString(GetString(d, "ten")),
                            UnitName = CleanString(GetString(d, "dvtinh")),
                            Quantity = ParseDouble(GetString(d, "sluong")),
                            UnitPrice = ParseDecimal(GetString(d, "dgia")),
                            Amount = ParseDecimal(GetString(d, "thtien")),
                            TaxRate = CleanString(GetString(d, "tsuat")),
                            TaxAmount = ParseDecimal(GetString(d, "tthue"))
                        });
                        line++;
                    }
                }

                return dto;
            }
            catch
            {
                return null;
            }
        }

        private static string? GetString(JsonElement element, string name) => TaxInvoiceClient.GetString(element, name);

        private static string CleanString(string? s) => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim();

        private static long ParseLong(string? s, long defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
            return defaultValue;
        }

        private static double ParseDouble(string? s, double defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) return v;
            return defaultValue;
        }

        private static decimal ParseDecimal(string? s, decimal defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            if (decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) return v;
            return defaultValue;
        }

        private static DateTime ParseDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return DateTime.Today;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) return dt;
            if (DateTime.TryParseExact(s, new[] { "dd/MM/yyyy", "yyyy-MM-dd", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
            return DateTime.Today;
        }

        private static DateTime? ParseNullableDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) return dt;
            return null;
        }
    }
}
