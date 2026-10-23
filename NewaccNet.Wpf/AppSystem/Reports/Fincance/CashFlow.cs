using DevExpress.XtraReports.UI;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Drawing;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class CashFlow : DevExpress.XtraReports.UI.XtraReport
    {
        public CashFlow()
        {
            InitializeComponent();
            this.ApplySubtleBorders();

            this.rowDetail.BeforePrint += RowDetail_BeforePrint;
        }

        private void RowDetail_BeforePrint(object sender, CancelEventArgs e)
        {
            if (sender is XRTableRow row)
            {
                var boldVal = GetCurrentColumnValue("IsBold");
                var italicVal = GetCurrentColumnValue("IsItalic");

                bool isBold = boldVal != null && Convert.ToBoolean(boldVal);
                bool isItalic = italicVal != null && Convert.ToBoolean(italicVal);

                DXFontStyle style = DXFontStyle.Regular;
                if (isBold && isItalic) style = DXFontStyle.Bold | DXFontStyle.Italic;
                else if (isBold) style = DXFontStyle.Bold;
                else if (isItalic) style = DXFontStyle.Italic;

                row.Font = new DXFont("Arial", 8.75F, style);
            }
        }
    }
}
