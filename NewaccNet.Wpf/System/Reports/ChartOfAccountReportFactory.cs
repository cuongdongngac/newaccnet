using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using System.Drawing;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.AppSystem.Directory;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public static class ChartOfAccountReportFactory
    {
        public static XtraReport CreateReport(IEnumerable<ChartOfAccountEntity> dataSource)
        {
            XtraReport report = new XtraReport();
            report.Landscape = true;
            report.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            report.Margins = new DevExpress.Drawing.DXMargins(50, 50, 50, 50);

            // Tạo các dải (Bands)
            ReportHeaderBand reportHeader = new ReportHeaderBand() { HeightF = 60 };
            PageHeaderBand pageHeader = new PageHeaderBand() { HeightF = 30 };
            DetailBand detailBand = new DetailBand() { HeightF = 25 };

            report.Bands.AddRange(new Band[] { reportHeader, pageHeader, detailBand });

            // 1. Tiêu đề Báo cáo (ReportHeader)
            XRLabel titleLabel = new XRLabel();
            titleLabel.Text = "HỆ THỐNG TÀI KHOẢN KẾ TOÁN";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlignment = TextAlignment.MiddleCenter;
            titleLabel.SizeF = new SizeF(report.PageWidth - report.Margins.Left - report.Margins.Right, 40);
            titleLabel.LocationF = new PointF(0, 10);
            reportHeader.Controls.Add(titleLabel);

            // 2. Tiêu đề Cột (PageHeader)
            XRTable headerTable = new XRTable();
            headerTable.SizeF = new SizeF(report.PageWidth - report.Margins.Left - report.Margins.Right, 30);
            headerTable.Borders = BorderSide.All;
            headerTable.Font = new Font("Arial", 10, FontStyle.Bold);
            headerTable.BackColor = Color.LightGray;
            headerTable.TextAlignment = TextAlignment.MiddleCenter;

            XRTableRow headerRow = new XRTableRow();
            headerTable.Rows.Add(headerRow);
            
            string[] captions = { "Mã TK", "Tên Tài Khoản", "Loại TK", "Mã cấp cha" };
            float[] weights = { 1.5f, 4f, 2f, 1.5f };

            for (int i = 0; i < captions.Length; i++)
            {
                XRTableCell cell = new XRTableCell();
                cell.Text = captions[i];
                cell.Weight = weights[i];
                headerRow.Cells.Add(cell);
            }
            pageHeader.Controls.Add(headerTable);

            // 3. Dữ liệu Chi tiết (Detail)
            XRTable detailTable = new XRTable();
            detailTable.SizeF = new SizeF(report.PageWidth - report.Margins.Left - report.Margins.Right, 25);
            detailTable.Borders = BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
            detailTable.Font = new Font("Arial", 10, FontStyle.Regular);
            detailTable.TextAlignment = TextAlignment.MiddleLeft;
            detailTable.Padding = new PaddingInfo(5, 5, 0, 0);

            XRTableRow detailRow = new XRTableRow();
            detailTable.Rows.Add(detailRow);

            var accountTypes = AccountTypeMapping.GetAccountTypes();

            for (int i = 0; i < captions.Length; i++)
            {
                XRTableCell cell = new XRTableCell();
                cell.Weight = weights[i];
                
                if (i == 0) // Mã TK
                {
                    cell.DataBindings.Add("Text", null, "AccountId");
                    cell.TextAlignment = TextAlignment.MiddleCenter;
                }
                else if (i == 1) // Tên TK
                {
                    cell.DataBindings.Add("Text", null, "AccountName");
                }
                else if (i == 2) // Loại TK (cần map tên)
                {
                    cell.TextAlignment = TextAlignment.MiddleCenter;
                    cell.BeforePrint += (s, e) =>
                    {
                        var currentRow = report.GetCurrentRow() as ChartOfAccountEntity;
                        if (currentRow != null && !string.IsNullOrEmpty(currentRow.CategoryId))
                        {
                            var mapping = accountTypes.FirstOrDefault(m => m.Code == currentRow.CategoryId);
                            ((XRTableCell)s).Text = mapping != null ? mapping.Name : currentRow.CategoryId;
                        }
                    };
                }
                else if (i == 3) // Mã cấp cha
                {
                    cell.DataBindings.Add("Text", null, "ParentId");
                    cell.TextAlignment = TextAlignment.MiddleCenter;
                }

                detailRow.Cells.Add(cell);
            }
            detailBand.Controls.Add(detailTable);

            // Xác định các tài khoản cha (có ít nhất 1 tài khoản con trỏ ParentId tới nó)
            var accounts = dataSource.ToList();
            var parentIds = new HashSet<string>(accounts.Where(a => !string.IsNullOrEmpty(a.ParentId)).Select(a => a.ParentId));

            // Hook sự kiện BeforePrint để in đậm dòng cha
            detailRow.BeforePrint += (s, e) =>
            {
                var currentRow = report.GetCurrentRow() as ChartOfAccountEntity;
                if (currentRow != null && parentIds.Contains(currentRow.AccountId))
                {
                    detailRow.Font = new Font("Arial", 10, FontStyle.Bold);
                }
                else
                {
                    detailRow.Font = new Font("Arial", 10, FontStyle.Regular);
                }
            };

            // 4. Gán Nguồn dữ liệu
            report.DataSource = accounts;

            return report;
        }
    }
}