using DevExpress.DataAccess.ObjectBinding;

namespace NewaccNet.Reports.VatDeclare
{
    [HighlightedClass]
    public class VatDeclareReportDTO
    {
        public VatDeclareReportDTO() { }

        public string Code { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }

        /// <summary>
        /// Cột 1: Doanh số (chưa có VAT)
        /// </summary>
        public decimal TurnoverAmount { get; set; }

        /// <summary>
        /// Cột 2: Thuế VAT
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// Bí danh cho VatAmount
        /// </summary>
        public decimal TaxAmount
        {
            get => VatAmount;
            set => VatAmount = value;
        }

        /// <summary>
        /// Cột phụ / Số lũy kế hoặc giá trị so sánh
        /// </summary>
        public decimal OtherAmount { get; set; }

        public decimal Amount => TurnoverAmount;
    }
}
