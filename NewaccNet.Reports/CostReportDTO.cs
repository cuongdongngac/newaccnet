using System;
using System.Collections.Generic;

namespace NewaccNet.Reports
{
    /// <summary>
    /// DTO dòng báo cáo Tổng hợp Chi phí theo Đối tượng tập hợp CP và Yếu tố chi phí.
    /// Chuẩn hóa theo mẫu legacy CostReport:
    /// - STT, Tên đối tượng (Đối tượng chi phí / Yếu tố chi phí)
    /// - Dư đầu kỳ (Nợ / Có)
    /// - Phát sinh trong kỳ (Nợ / Có)
    /// - Dư cuối kỳ (Nợ / Có)
    /// </summary>
    public class CostReportDTO
    {
        public string Stt { get; set; } = string.Empty;

        public string CostObjectId { get; set; } = string.Empty;
        public string CostObjectName { get; set; } = string.Empty;

        public string CostElementId { get; set; } = string.Empty;
        public string CostElementName { get; set; } = string.Empty;

        /// <summary>
        /// Tên hiển thị trên bảng báo cáo:
        /// - Dòng cấp 1 (CostObject): in đậm tên đối tượng
        /// - Dòng cấp 2 (CostElement): thụt vào 3 khoảng trắng tên yếu tố chi phí
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Cờ đánh dấu dòng cha (Đối tượng chi phí) hay dòng con (Yếu tố chi phí)
        /// </summary>
        public bool IsGroupHeader { get; set; }

        public decimal BeginDebitAmount { get; set; }
        public decimal BeginCreditAmount { get; set; }

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        public decimal EndDebitAmount { get; set; }
        public decimal EndCreditAmount { get; set; }

        /// <summary>
        /// Danh sách chứng từ chi tiết đi kèm phục vụ Drill-down cấp 1
        /// </summary>
        public List<CostDetailReportDTO> Details { get; set; } = new();
    }

    /// <summary>
    /// DTO dòng Sổ chi tiết Chi phí (phục vụ Drill-down cấp 1 từ Tổng hợp chi phí).
    /// Click vào bất kỳ dòng chứng từ nào để mở VoucherEntryWindow (Drill-down cấp 2).
    /// </summary>
    public class CostDetailReportDTO
    {
        public int Stt { get; set; }

        public int? JournalVoucherId { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string Contents { get; set; } = string.Empty;

        public string AccountId { get; set; } = string.Empty;
        public string CounterAccountId { get; set; } = string.Empty;

        public string CostObjectId { get; set; } = string.Empty;
        public string CostObjectName { get; set; } = string.Empty;

        public string CostElementId { get; set; } = string.Empty;
        public string CostElementName { get; set; } = string.Empty;

        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }

        public decimal ClosingDebitAmount { get; set; }
        public decimal ClosingCreditAmount { get; set; }

        public bool IsOpeningRow { get; set; }
    }
}
