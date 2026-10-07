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
        
        // Số phát sinh kỳ này (từ Database)
        public decimal Amount { get; set; }
        
        // Số phát sinh kỳ trước (từ JSON)
        public decimal PrevAmount { get; set; }
    }
}
