using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Drawing;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class BusinessResult : DevExpress.XtraReports.UI.XtraReport
    {
        public BusinessResult()
        {
            InitializeComponent();
            this.ApplySubtleBorders();

            this.tableRow2.BeforePrint += TableRow2_BeforePrint;
        }

        private void TableRow2_BeforePrint(object sender, CancelEventArgs e)
        {
            var row = sender as XRTableRow;
            if (row != null)
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

