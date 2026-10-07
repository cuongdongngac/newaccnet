using DevExpress.DataAccess.ObjectBinding;
using System;

namespace NewaccNet.Reports.BusinessResult
{
    [HighlightedClass]
    public class BusinessResultReportDTO
    {
        public BusinessResultReportDTO() { }

        public string Code { get; set; }
        public string ItemName { get; set; }
        public string Illustration { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }

        // Số phát sinh kỳ báo cáo
        public decimal Amount { get; set; }

        // Số liệu cột còn lại (kỳ trước / lũy kế / năm trước) lấy từ JSON merge
        public decimal OtherAmount { get; set; }
    }
}
