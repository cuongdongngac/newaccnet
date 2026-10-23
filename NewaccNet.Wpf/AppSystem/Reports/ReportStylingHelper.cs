using System.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public static class ReportStylingHelper
    {
        public static readonly Color SubtleBorderColor = Color.LightGray;

        /// <summary>
        /// Áp dụng màu đường kẻ mờ (LightGray) cho tất cả bảng, ô và control có viền trong báo cáo
        /// </summary>
        public static void ApplySubtleBorders(this XtraReport report)
        {
            if (report == null) return;

            foreach (var control in report.AllControls<XRControl>())
            {
                if (control.Borders != BorderSide.None)
                {
                    control.BorderColor = SubtleBorderColor;
                    control.StylePriority.UseBorderColor = false;
                }
            }
        }
    }
}

