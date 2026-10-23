using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace NewaccNet.TaxInvoice
{
    public enum InvoiceDirection
    {
        Purchase,
        Sold,
    }

    public enum InvoiceSource
    {
        Query,
        Sco,
    }

    public sealed class CaptchaChallenge
    {
        public CaptchaChallenge(string key, string svgContent, string recognizedValue)
        {
            Key = key;
            SvgContent = svgContent;
            RecognizedValue = recognizedValue;
        }

        public string Key { get; }
        public string SvgContent { get; }
        public string RecognizedValue { get; }
    }

    public sealed class TokenStatusInfo
    {
        public bool HasToken { get; set; }
        public bool IsExpired { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public TimeSpan? RemainingTime { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }

    public sealed class TaxInvoiceToken
    {
        public TaxInvoiceToken(string value, DateTimeOffset obtainedAt)
        {
            Value = value;
            ObtainedAt = obtainedAt;
            ExpiresAt = ReadJwtExpiry(value);
        }

        public string Value { get; }
        public DateTimeOffset ObtainedAt { get; }
        public DateTimeOffset? ExpiresAt { get; }

        public bool IsValid(TimeSpan? margin = null)
        {
            if (ExpiresAt == null) return true;
            return DateTimeOffset.UtcNow + (margin ?? TimeSpan.FromMinutes(5)) < ExpiresAt.Value;
        }

        public TokenStatusInfo GetStatus()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return new TokenStatusInfo
                {
                    HasToken = false,
                    IsExpired = true,
                    StatusMessage = "No authentication token found."
                };
            }

            if (ExpiresAt == null)
            {
                return new TokenStatusInfo
                {
                    HasToken = true,
                    IsExpired = false,
                    StatusMessage = "Token has no expiration timestamp."
                };
            }

            var now = DateTimeOffset.UtcNow;
            if (now >= ExpiresAt.Value)
            {
                return new TokenStatusInfo
                {
                    HasToken = true,
                    IsExpired = true,
                    ExpiresAt = ExpiresAt.Value.ToLocalTime(),
                    StatusMessage = $"Token expired at {ExpiresAt.Value.ToLocalTime():dd/MM/yyyy HH:mm:ss}. Please log in again."
                };
            }

            var remaining = ExpiresAt.Value - now;
            return new TokenStatusInfo
            {
                HasToken = true,
                IsExpired = false,
                ExpiresAt = ExpiresAt.Value.ToLocalTime(),
                RemainingTime = remaining,
                StatusMessage = $"Token valid until {ExpiresAt.Value.ToLocalTime():dd/MM/yyyy HH:mm:ss} ({(int)remaining.TotalHours}h {remaining.Minutes}m remaining)."
            };
        }

        public static DateTimeOffset? ReadJwtExpiry(string jwtToken)
        {
            try
            {
                var parts = jwtToken.Split('.');
                if (parts.Length < 2) return null;
                var base64 = parts[1].Replace('-', '+').Replace('_', '/');
                base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("exp", out var exp) && exp.TryGetInt64(out var seconds))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(seconds);
                }
            }
            catch { }
            return null;
        }
    }

    public sealed class InvoiceKey : IEquatable<InvoiceKey>
    {
        public InvoiceKey(string sellerTaxCode, string invoiceTemplate, string invoiceSeries, string invoiceNo, InvoiceSource source)
        {
            SellerTaxCode = sellerTaxCode ?? string.Empty;
            InvoiceTemplate = invoiceTemplate ?? string.Empty;
            InvoiceSeries = invoiceSeries ?? string.Empty;
            InvoiceNo = invoiceNo ?? string.Empty;
            Source = source;
        }

        public string SellerTaxCode { get; }
        public string InvoiceTemplate { get; }
        public string InvoiceSeries { get; }
        public string InvoiceNo { get; }
        public InvoiceSource Source { get; }

        public string FileStem => $"{Sanitize(SellerTaxCode)}_{Sanitize(InvoiceTemplate)}_{Sanitize(InvoiceSeries)}_{Sanitize(InvoiceNo)}";

        internal string QueryString =>
            "nbmst=" + Uri.EscapeDataString(SellerTaxCode) +
            "&khhdon=" + Uri.EscapeDataString(InvoiceSeries) +
            "&shdon=" + Uri.EscapeDataString(InvoiceNo) +
            "&khmshdon=" + Uri.EscapeDataString(InvoiceTemplate);

        private static string Sanitize(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                sb.Append(char.IsLetterOrDigit(ch) || ch == '-' ? ch : '-');
            }
            return sb.ToString();
        }

        public bool Equals(InvoiceKey? other)
        {
            if (other is null) return false;
            return SellerTaxCode == other.SellerTaxCode &&
                   InvoiceTemplate == other.InvoiceTemplate &&
                   InvoiceSeries == other.InvoiceSeries &&
                   InvoiceNo == other.InvoiceNo;
        }

        public override bool Equals(object? obj) => Equals(obj as InvoiceKey);
        public override int GetHashCode() => (SellerTaxCode + "|" + InvoiceTemplate + "|" + InvoiceSeries + "|" + InvoiceNo).GetHashCode();
        public override string ToString() => $"{FileStem} ({Source})";
    }

    public sealed class InvoiceListPage
    {
        public InvoiceListPage(string rawJson, IReadOnlyList<InvoiceListItem> items, string? nextState)
        {
            RawJson = rawJson;
            Items = items;
            NextState = nextState;
        }

        public string RawJson { get; }
        public IReadOnlyList<InvoiceListItem> Items { get; }
        public string? NextState { get; }
    }

    public sealed class InvoiceListItem
    {
        public InvoiceListItem(InvoiceKey key, string? createdDateText)
        {
            Key = key;
            CreatedDateText = createdDateText;
        }

        public InvoiceKey Key { get; }
        public string? CreatedDateText { get; }
    }

    public class TaxInvoiceException : Exception
    {
        public TaxInvoiceException(string message, int? statusCode = null, string? responseBody = null, Exception? inner = null)
            : base(message, inner)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public int? StatusCode { get; }
        public string? ResponseBody { get; }
    }

    public sealed class TaxInvoiceLoginException : TaxInvoiceException
    {
        public TaxInvoiceLoginException(string message, bool isCaptchaError, int? statusCode = null, string? body = null)
            : base(message, statusCode, body)
        {
            IsCaptchaError = isCaptchaError;
        }

        public bool IsCaptchaError { get; }
    }

    public sealed class DownloadRequest
    {
        public string TaxCode { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public IList<InvoiceDirection> Directions { get; set; } = new List<InvoiceDirection> { InvoiceDirection.Purchase };
        public bool DownloadDetail { get; set; } = true;
        public bool DownloadXml { get; set; } = false;
        public bool ExtractXml { get; set; } = false;
        public bool SkipExistingDetail { get; set; } = false;
        public bool IncludeSco { get; set; } = true;
        public string? InvoiceStatusFilter { get; set; }
        public string? ProcessStatusFilter { get; set; }
        public string TargetDirectory { get; set; } = string.Empty;
    }

    public sealed class DownloadProgress
    {
        public DownloadProgress() { }

        public DownloadProgress(string stage, string message, int done = 0, int total = 0)
        {
            Stage = stage;
            Message = message;
            Done = done;
            Total = total;
            ProcessedCount = done;
            TotalCount = total;
        }

        public string Stage { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Done { get; set; }
        public int Total { get; set; }
        public int TotalCount { get; set; }
        public int ProcessedCount { get; set; }
        public int SucceededCount { get; set; }
        public int FailedCount { get; set; }
        public string CurrentItem { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        public override string ToString() => Total > 0 ? $"[{Stage} {Done}/{Total}] {Message}" : $"[{Stage}] {Message}";
    }

    // ==========================================
    // DTOs for Database Storage
    // ==========================================

    public sealed class TaxAccountConfigDto
    {
        public int Id { get; set; }
        public string TaxCode { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public sealed class TaxInvoiceDto
    {
        public long Id { get; set; }
        public string Direction { get; set; } = "purchase";
        public string SellerTaxCode { get; set; } = string.Empty;
        public string InvoiceTemplate { get; set; } = string.Empty;
        public string InvoiceSeries { get; set; } = string.Empty;
        public long InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? SellerName { get; set; }
        public string? SellerAddress { get; set; }
        public string? BuyerTaxCode { get; set; }
        public string? BuyerName { get; set; }
        public string? BuyerAddress { get; set; }
        public string CurrencyId { get; set; } = "VND";
        public double ExchangeRate { get; set; } = 1.0;
        public decimal TotalAmountWithoutTax { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public int InvoiceStatus { get; set; }
        public int ProcessStatus { get; set; }
        public string InvoiceSource { get; set; } = "query";
        public string? TaxAuthorityCode { get; set; }
        public string? RawJsonPath { get; set; }
        public string? XmlFilePath { get; set; }
        public DateTime DownloadedAt { get; set; } = DateTime.Now;
        public string? VoucherId { get; set; }
        public string? ReconcileNote { get; set; }

        public string DirectionDisplay => Direction == "purchase" ? "Mua vào" : "Bán ra";

        public string StatusDescription => InvoiceStatus switch
        {
            1 => "Hóa đơn mới",
            2 => "HĐ thay thế",
            3 => "HĐ điều chỉnh",
            4 => "Bị thay thế",
            5 => "Bị điều chỉnh",
            6 => "Bị hủy",
            _ => $"Mã {InvoiceStatus}"
        };

        public string VoucherStatus => !string.IsNullOrEmpty(VoucherId) ? $"Đã lập ({VoucherId})" : "Chưa lập";

        public List<TaxInvoiceTaxRateDto> TaxRates { get; set; } = new List<TaxInvoiceTaxRateDto>();
        public List<TaxInvoiceDetailDto> Details { get; set; } = new List<TaxInvoiceDetailDto>();
    }

    public sealed class TaxInvoiceTaxRateDto
    {
        public long Id { get; set; }
        public long TaxInvoiceId { get; set; }
        public string TaxRate { get; set; } = string.Empty;
        public decimal AmountWithoutTax { get; set; }
        public decimal TaxAmount { get; set; }
    }

    public sealed class TaxInvoiceDetailDto
    {
        public long Id { get; set; }
        public long TaxInvoiceId { get; set; }
        public int LineNo { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UnitName { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public string? TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
