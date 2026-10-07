using System;
using DevExpress.DataAccess.ObjectBinding;

namespace NewaccNet.Reports.ObligationTax
{
    [HighlightedClass]
    public class ObligationTaxResultDto
    {
        public ObligationTaxResultDto() { }
        
        public int ObligationId { get; set; }
        public string ObligationName { get; set; }
        public string Code { get; set; }
        public string ItemName { get; set; }
        public decimal BeginTax { get; set; }
        public decimal DebitTax { get; set; }
        public decimal CreditTax { get; set; }
        public decimal EndTax { get; set; }
        public decimal AccTax { get; set; }
    }
}
