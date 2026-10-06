using DevExpress.DataAccess.ObjectBinding;
using System;
using System.Collections.Generic;

namespace NewaccNet.Reports
{
    [HighlightedClass]
    public class LedgerReportDTO
    {
        // Constructor rỗng bắt buộc cho DevExpress Report Designer
        public LedgerReportDTO() { }

        /// <summary>
        /// Loại dòng: 0 - Dư đầu kỳ, 1 - Phát sinh, 2 - Cộng phát sinh, 3 - Dư cuối kỳ
        /// </summary>
        public int RowType { get; set; }

        /// <summary>
        /// ID chứng từ gốc (để có thể mở lại chứng từ khi click vào báo cáo)
        /// </summary>
        public int? JournalVoucherId { get; set; }

        /// <summary>
        /// ID dòng chứng từ chi tiết
        /// </summary>
        public int? JournalEntryId { get; set; }

        /// <summary>
        /// Số chứng từ
        /// </summary>
        public string VoucherNo { get; set; }

        /// <summary>
        /// Ngày chứng từ
        /// </summary>
        public DateTime? VoucherDate { get; set; }

        /// <summary>
        /// Diễn giải
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// Tài khoản
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Tài khoản đối ứng
        /// </summary>
        public string CounterAccountId { get; set; }

        /// <summary>
        /// Tên tài khoản đối ứng
        /// </summary>
        public string CounterAccountName { get; set; }

        /// <summary>
        /// Số tiền Nợ
        /// </summary>
        public decimal DebitAmount { get; set; }

        /// <summary>
        /// Số tiền Có
        /// </summary>
        public decimal CreditAmount { get; set; }

        /// <summary>
        /// Số dư lũy kế
        /// </summary>
        public decimal Balance { get; set; }
    }
}
