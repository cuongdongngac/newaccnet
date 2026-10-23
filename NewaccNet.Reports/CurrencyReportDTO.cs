using System;
using System.Collections.Generic;

namespace NewaccNet.Reports
{
    /// <summary>
    /// DTO dòng Báo cáo Tổng hợp Ngoại tệ (theo chuẩn report.png và schema.png trong thư mục Curency):
    /// - STT, Tên ngoại tệ
    /// - Dư đầu kỳ: Nguyên tệ & VNĐ
    /// - Thu trong kỳ (Nợ): Nguyên tệ & VNĐ
    /// - Chi trong kỳ (Có): Nguyên tệ & VNĐ
    /// - Dư cuối kỳ: Nguyên tệ & VNĐ
    /// </summary>
    public class CurrencyReportDTO
    {
        public string Stt { get; set; } = string.Empty;

        public string CurrencyId { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;

        public string DisplayName => string.IsNullOrWhiteSpace(CurrencyName) 
            ? CurrencyId 
            : $"{CurrencyId} - {CurrencyName}";

        // Dư đầu kỳ
        public double BeginQuantity { get; set; }
        public decimal BeginAmount { get; set; }

        // Thu trong kỳ (Phát sinh Nợ)
        public double ReceiptQuantity { get; set; }
        public decimal ReceiptAmount { get; set; }

        // Chi trong kỳ (Phát sinh Có)
        public double IssueQuantity { get; set; }
        public decimal IssueAmount { get; set; }

        // Dư cuối kỳ
        public double EndQuantity { get; set; }
        public decimal EndAmount { get; set; }

        /// <summary>
        /// Danh sách chứng từ chi tiết đi kèm phục vụ Drill-down sang Sổ chi tiết ngoại tệ
        /// </summary>
        public List<CurrencyDetailReportDTO> Details { get; set; } = new();
    }

    /// <summary>
    /// DTO dòng Sổ chi tiết Ngoại tệ (phục vụ Drill-down cấp 1 từ Tổng hợp ngoại tệ).
    /// Click vào dòng chứng từ để mở VoucherEntryWindow (Drill-down cấp 2).
    /// </summary>
    public class CurrencyDetailReportDTO
    {
        public int Stt { get; set; }

        public int? JournalVoucherId { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string Contents { get; set; } = string.Empty;

        public string AccountId { get; set; } = string.Empty;
        public string CounterAccountId { get; set; } = string.Empty;

        public string CurrencyId { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;

        public double ExchangeRate { get; set; }

        // Thu trong kỳ
        public double ReceiptQuantity { get; set; }
        public decimal ReceiptAmount { get; set; }

        // Chi trong kỳ
        public double IssueQuantity { get; set; }
        public decimal IssueAmount { get; set; }

        // Số dư lũy kế
        public double ClosingQuantity { get; set; }
        public decimal ClosingAmount { get; set; }

        public bool IsOpeningRow { get; set; }
    }
}
