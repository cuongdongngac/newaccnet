using DevExpress.DataAccess.ObjectBinding;

namespace NewaccNet.Reports.VatReturn
{
    [HighlightedClass]

    public class VatReturnReportDTO
    {
        public VatReturnReportDTO(){ }

        public string Code { get; set; }
        public string ItemName { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public decimal Amount { get; set; }
        public decimal OtherAmount { get; set; }
    }
}
