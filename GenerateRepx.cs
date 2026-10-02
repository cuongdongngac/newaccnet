using System;
using DevExpress.XtraReports.UI;

class Program
{
    static void Main()
    {
        XtraReport report = new XtraReport();
        report.Bands.Add(new DetailBand());
        report.Bands.Add(new ReportHeaderBand());
        report.SaveLayoutToXml(@"D:\NewaccNet\NewaccNet.Wpf\System\Reports\DiaryReport.repx");
    }
}
