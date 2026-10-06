using DevExpress.DataAccess.ObjectBinding;
using System;
using System.Collections.Generic;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Data Transfer Object cho Sổ nhật ký (Diary).
    /// </summary>
    [HighlightedClass]
    public class DiaryReportDTO
    {
        public DiaryReportDTO()
        {
        }
        public int JournalVoucherId { get; set; }
        public string? VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string? Contents { get; set; }
        public string? AccountId { get; set; }
        public string? AccountName { get; set; }
        public short Dbcr { get; set; }
        public decimal Amount { get; set; }
    }
}
