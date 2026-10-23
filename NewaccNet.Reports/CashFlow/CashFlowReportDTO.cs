using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports.CashFlow
{
    [HighlightedClass]
    public class CashFlowReportDTO
    {
        public CashFlowReportDTO()
        { }
        public int? CashFlowCategoryId { get; set; }
        public string CashFlowName { get; set; }
        public string CashFlowNameRepeate { get; set; }
        public string Illustration { get; set; }
        public string Code { get; set; }
        public string ItemName { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool Dbcr { get; set; } = true; // True: Thu (+), False: Chi (-)
        
        // Số phát sinh kỳ này (từ Database) - hiển thị số dương trên dòng chi tiết
        public decimal Amount { get; set; }
        
        // Số phát sinh kỳ trước (từ JSON) - hiển thị số dương trên dòng chi tiết
        public decimal PrevAmount { get; set; }

        // Số có dấu dùng để sum ở Group Footer: Thu (+), Chi (-)
        public decimal SignedAmount => Dbcr ? Amount : -Amount;
        public decimal SignedPrevAmount => Dbcr ? PrevAmount : -PrevAmount;
    }
}
