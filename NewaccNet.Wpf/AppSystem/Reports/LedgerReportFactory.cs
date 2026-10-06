using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using NewaccNet.Reports;
using NewaccNet.Wpf.AppSystem.Voucher;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public static class LedgerReportFactory
    {
        public static void ShowPreview(Window owner, string templatePath, List<LedgerReportDTO> dataSource, 
            string companyName, string accountId, string accountName, DateTime beginDate, DateTime endDate, bool onlyBooked = true)
        {
            XtraReport report = new NewaccNet.Wpf.AppSystem.Reports.Ledgers.GeneralLedger();

            if (File.Exists(templatePath))
            {
                report.LoadLayout(templatePath);
            }

            report.DataSource = dataSource;

            SetParameter(report, "prmCompanyName", companyName);
            SetParameter(report, "prmAccountID", accountId);
            SetParameter(report, "prmAccountName", accountName);
            SetParameter(report, "prmBeginDate", beginDate);
            SetParameter(report, "prmEndDate", endDate);

            report.RequestParameters = false;

                        report.CreateDocument(false);
            
            // Ẩn thanh bar bên trái
            report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.Parameters, DevExpress.XtraPrinting.CommandVisibility.None);
            report.PrintingSystem.SetCommandVisibility(DevExpress.XtraPrinting.PrintingSystemCommand.DocumentMap, DevExpress.XtraPrinting.CommandVisibility.None);

            var window = new DocumentPreviewWindow
            {
                Owner = owner,
                Title = "Sổ chi tiết tài khoản"
            };
            
            window.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;

            // Fix Ribbon Actions: Sử dụng DocumentCommandProvider và đúng ContainerName (chữ thường)
            if (window.PreviewControl.CommandProvider == null)
                window.PreviewControl.CommandProvider = new DevExpress.Xpf.Printing.DocumentCommandProvider();

            // --- THÊM CHỨC NĂNG TỔNG HỢP (SUMMARY) ---
            var btnSummary = new DevExpress.Xpf.Bars.BarButtonItem { 
                Content = "Tổng hợp",
                RibbonStyle = DevExpress.Xpf.Bars.RibbonItemStyles.Large
            };
            
            try 
            {
                var ext = new DevExpress.Xpf.Core.SvgImageSourceExtension() { Uri = new Uri("pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_pivotchart.svg") };
                btnSummary.LargeGlyph = ext.ProvideValue(null) as System.Windows.Media.ImageSource;
            } 
            catch { /* fallback if svg load fails */ }

            btnSummary.ItemClick += (s, e) => 
            {
                // Áp dụng CÁCH B: Mở cửa sổ Preview mới độc lập để người dùng có thể đối chiếu
                var summaryReport = new NewaccNet.Wpf.AppSystem.Reports.Ledgers.LedgerSummary();
                summaryReport.DataSource = dataSource; // Dùng chung dữ liệu (Shared DataSource)
                SetParameter(summaryReport, "prmCompanyName", companyName);
                SetParameter(summaryReport, "prmAccountID", accountId);
                SetParameter(summaryReport, "prmAccountName", accountName);
                SetParameter(summaryReport, "prmBeginDate", beginDate);
                SetParameter(summaryReport, "prmEndDate", endDate);
                summaryReport.RequestParameters = false;

                // Lấy đích danh Band và Cell theo tên bạn đã đặt
                var groupBand = summaryReport.Bands["CounterAccountBand"] as DevExpress.XtraReports.UI.GroupHeaderBand;
                if (groupBand != null)
                {
                    // Quét các Table/Cell bên trong Band này để gán Tag, giúp click vào BẤT KỲ ô nào trên dòng này cũng nhận được ID
                    foreach (XRControl control in groupBand.Controls)
                    {
                        if (control is XRTable table)
                        {
                            foreach (XRTableRow row in table.Rows)
                            {
                                foreach (XRTableCell cell in row.Cells)
                                {
                                    cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Tag", "[CounterAccountId]"));
                                }
                            }
                        }
                    }
                }
                
                // (Tuỳ chọn) Nếu bạn chỉ muốn gán cho đúng cell CounterAccountId thì dùng FindControl
                // var idCell = summaryReport.FindControl("CounterAccountId", true) as XRTableCell;
                // if (idCell != null) idCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Tag", "[CounterAccountId]"));

                summaryReport.CreateDocument(false);
                
                DocumentPreviewWindow newWindow = new DocumentPreviewWindow();
                newWindow.Owner = window;
                newWindow.Title = "Báo cáo Tổng hợp (Summary)";
                
                // --- THÊM CHỨC NĂNG DRILL-DOWN TRÊN CỬA SỔ SUMMARY MỚI ---
                newWindow.PreviewControl.DocumentPreviewMouseClick += (sPreview, eClick) =>
                {
                    var visualBrick = eClick.Brick as DevExpress.XtraPrinting.VisualBrick;
                    if (visualBrick != null && visualBrick.Value != null)
                    {
                        var brickOwner = visualBrick.BrickOwner as DevExpress.XtraReports.UI.XRControl;

                        // Chỉ cho phép drill-down nếu click trúng ô CounterAccountId HOẶC nằm trong CounterAccountBand
                        // Vì bạn muốn "click vào band là quay ngược lại", ta ưu tiên kiểm tra dữ liệu Value đã được gán Tag.
                        string clickedAccountId = visualBrick.Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(clickedAccountId))
                        {
                            // Mở sổ cái chi tiết cho tài khoản vừa click
                            LedgerReportService.ShowPreview(window, beginDate, endDate, clickedAccountId, onlyBooked);
                        }
                    }
                };

                newWindow.PreviewControl.DocumentSource = summaryReport;
                newWindow.Show();
            };

            // Tên container chuẩn của DevExpress 25.2 là chữ thường "documentGroup" hoặc "exportGroup"
            window.PreviewControl.CommandProvider.RibbonActions.Add(new DevExpress.Xpf.Bars.InsertAction 
            { 
                Element = btnSummary, 
                ContainerName = "documentGroup"
            });
            
            // Gán DocumentSource sau khi đã thiết lập CommandProvider
            window.PreviewControl.DocumentSource = report;
            
            window.ShowDialog();
        }

        private static void SetParameter(XtraReport report, string paramName, object value)
        {
            if (report.Parameters[paramName] != null)
            {
                report.Parameters[paramName].Value = value;
                report.Parameters[paramName].Visible = false;
            }
            else
            {
                var p = new DevExpress.XtraReports.Parameters.Parameter();
                p.Name = paramName;
                p.Value = value;
                p.Visible = false;
                report.Parameters.Add(p);
            }
        }
    }
    
    public static class CursorUtility
    {
        public static void SetCursor(System.Windows.Input.Cursor cursor)
        {
            System.Windows.Input.Mouse.OverrideCursor = cursor == System.Windows.Input.Cursors.Arrow ? null : cursor;
        }
    }
}





