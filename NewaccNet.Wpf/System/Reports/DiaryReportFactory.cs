using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DataAccess.TypedListClasses;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using NewaccNet.Wpf.AppSystem.Voucher;
using FontStyle = System.Drawing.FontStyle;
using TextAlignment = DevExpress.XtraPrinting.TextAlignment;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public static class DiaryReportFactory
    {
        private static readonly Font FontRegular = new Font("Times New Roman", 9, FontStyle.Regular);
        private static readonly Font FontBold = new Font("Times New Roman", 9, FontStyle.Bold);
        private static readonly Font FontHeader = new Font("Times New Roman", 9, FontStyle.Bold);
        private static readonly Font FontContents = new Font("Times New Roman", 9, FontStyle.Bold | FontStyle.Italic);

        public static XtraReport CreateReport(
            IEnumerable<DiaryRow> dataSource,
            DateTime fromDate,
            DateTime toDate,
            string? companyName = null)
        {
            var rows = dataSource?.ToList() ?? new List<DiaryRow>();
            decimal sumDebit = rows.Sum(r => r.Debit);
            decimal sumCredit = rows.Sum(r => r.Credit);

            XtraReport report = new XtraReport
            {
                Landscape = false,
                PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4,
                Margins = new DevExpress.Drawing.DXMargins(40, 40, 40, 40),
                DisplayName = "Nhật ký chung"
            };

            float pageWidth = report.PageWidth - report.Margins.Left - report.Margins.Right;
            float[] weights = { 0.7f, 1.05f, 3.35f, 0.75f, 0.75f, 1.2f, 1.2f };

            var reportHeader = new ReportHeaderBand { HeightF = 78 };
            var pageHeader = new PageHeaderBand { HeightF = 26 };
            var groupHeader = new GroupHeaderBand
            {
                HeightF = 20,
                RepeatEveryPage = false,
                KeepTogether = true,
                GroupUnion = GroupUnion.WithFirstDetail
            };
            var detailBand = new DetailBand
            {
                HeightF = 18,
                KeepTogether = true
            };
            var reportFooter = new ReportFooterBand { HeightF = 22 };
            var pageFooter = new PageFooterBand { HeightF = 20 };

            groupHeader.GroupFields.Add(new GroupField("JournalVoucherId"));
            report.Bands.AddRange(new Band[]
            {
                reportHeader, pageHeader, groupHeader, detailBand, reportFooter, pageFooter
            });

            float y = 4;
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                reportHeader.Controls.Add(CreateLabel(companyName.ToUpperInvariant(), 0, y, pageWidth, 16,
                    TextAlignment.MiddleCenter, 10, FontStyle.Regular));
                y += 18;
            }

            reportHeader.Controls.Add(CreateLabel("NHẬT KÝ CHUNG", 0, y, pageWidth, 22,
                TextAlignment.MiddleCenter, 14, FontStyle.Bold));
            y += 24;
            reportHeader.Controls.Add(CreateLabel(
                $"Từ ngày: {fromDate:dd/MM/yyyy} Đến ngày: {toDate:dd/MM/yyyy}",
                0, y, pageWidth, 16, TextAlignment.MiddleCenter, 9, FontStyle.Regular));

            var headerTable = CreateTable(pageWidth, 24);
            headerTable.Borders = BorderSide.All;
            headerTable.Font = FontHeader;
            headerTable.TextAlignment = TextAlignment.MiddleCenter;
            var headerRow = new XRTableRow();
            headerTable.Rows.Add(headerRow);

            headerRow.Cells.Add(CreateCell("Số CT", weights[0], TextAlignment.MiddleCenter, FontHeader));
            headerRow.Cells.Add(CreateCell("Ngày CT", weights[1], TextAlignment.MiddleCenter, FontHeader));
            headerRow.Cells.Add(CreateCell("Nội dung", weights[2], TextAlignment.MiddleCenter, FontHeader));
            var cellTkHeader = CreateCell("Tài khoản", weights[3] + weights[4], TextAlignment.MiddleCenter, FontHeader);
            headerRow.Cells.Add(cellTkHeader);
            headerRow.Cells.Add(CreateCell("Số tiền nợ", weights[5], TextAlignment.MiddleCenter, FontHeader));
            headerRow.Cells.Add(CreateCell("Số tiền có", weights[6], TextAlignment.MiddleCenter, FontHeader));
            pageHeader.Controls.Add(headerTable);

            var groupTable = CreateTable(pageWidth, 20);
            groupTable.Borders = BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
            var groupRow = new XRTableRow();
            groupTable.Rows.Add(groupRow);

            var cellNo = CreateCell(string.Empty, weights[0], TextAlignment.MiddleCenter, FontBold);
            cellNo.BeforePrint += (s, e) =>
            {
                var row = report.GetCurrentRow() as DiaryRow;
                string no = row?.VoucherNo;
                ((XRTableCell)s).Text = string.IsNullOrWhiteSpace(no) ? "." : no;
            };

            var cellDate = BoundCell("VoucherDate", "{0:dd/MM/yyyy}", weights[1], TextAlignment.MiddleCenter, FontBold);
            var cellContents = BoundCell("Contents", null, weights[2], TextAlignment.MiddleLeft, FontContents);
            cellContents.CanGrow = true;

            groupRow.Cells.AddRange(new[]
            {
                cellNo,
                cellDate,
                cellContents,
                CreateCell(string.Empty, weights[3], TextAlignment.MiddleLeft, FontRegular),
                CreateCell(string.Empty, weights[4], TextAlignment.MiddleRight, FontRegular),
                CreateCell(string.Empty, weights[5], TextAlignment.MiddleRight, FontRegular),
                CreateCell(string.Empty, weights[6], TextAlignment.MiddleRight, FontRegular)
            });
            groupHeader.Controls.Add(groupTable);
            WireDrillDown(groupHeader, report);
            WireDrillDown(groupTable, report);
            foreach (XRTableCell cell in groupRow.Cells)
                WireDrillDown(cell, report);

            var detailTable = CreateTable(pageWidth, 18);
            detailTable.Borders = BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
            var detailRow = new XRTableRow();
            detailTable.Rows.Add(detailRow);

            var cellAccountName = BoundCell("AccountName", null, weights[2], TextAlignment.MiddleLeft, FontRegular);
            cellAccountName.CanGrow = true;

            detailRow.Cells.AddRange(new[]
            {
                CreateCell(string.Empty, weights[0], TextAlignment.MiddleCenter, FontRegular),
                CreateCell(string.Empty, weights[1], TextAlignment.MiddleCenter, FontRegular),
                cellAccountName,
                BoundCell("DebitAccount", null, weights[3], TextAlignment.MiddleLeft, FontRegular),
                BoundCell("CreditAccount", null, weights[4], TextAlignment.MiddleRight, FontRegular),
                CreateAmountCell(report, isDebit: true, weights[5]),
                CreateAmountCell(report, isDebit: false, weights[6])
            });
            detailBand.Controls.Add(detailTable);

            var footerTable = CreateTable(pageWidth, 20);
            footerTable.Borders = BorderSide.All;
            var footerRow = new XRTableRow();
            footerTable.Rows.Add(footerRow);
            footerRow.Cells.AddRange(new[]
            {
                CreateCell(string.Empty, weights[0], TextAlignment.MiddleCenter, FontBold),
                CreateCell(string.Empty, weights[1], TextAlignment.MiddleCenter, FontBold),
                CreateCell("Cộng phát sinh", weights[2], TextAlignment.MiddleLeft, FontBold),
                CreateCell(string.Empty, weights[3], TextAlignment.MiddleCenter, FontBold),
                CreateCell(string.Empty, weights[4], TextAlignment.MiddleCenter, FontBold),
                CreateCell(sumDebit.ToString("N0"), weights[5], TextAlignment.MiddleRight, FontBold),
                CreateCell(sumCredit.ToString("N0"), weights[6], TextAlignment.MiddleRight, FontBold)
            });
            reportFooter.Controls.Add(footerTable);

            var pageInfo = new XRPageInfo
            {
                PageInfo = PageInfo.NumberOfTotal,
                Format = "Trang {0}/{1}",
                Font = FontRegular,
                TextAlignment = TextAlignment.MiddleRight,
                LocationF = new PointF(0, 2),
                SizeF = new SizeF(pageWidth, 16)
            };
            pageFooter.Controls.Add(pageInfo);

            report.DataSource = rows;
            return report;
        }

        private static void WireDrillDown(XRControl control, XtraReport report)
        {
            control.BeforePrint += (s, e) =>
            {
                var row = report.GetCurrentRow() as DiaryRow;
                ((XRControl)s).Tag = row?.JournalVoucherId;
            };
            control.PreviewClick += (s, e) => OpenVoucher(((XRControl)s).Tag);
        }

        private static void OpenVoucher(object tag)
        {
            int? id = tag as int?;
            if (id == null && tag is int boxed)
                id = boxed;
            if (id == null || id.Value <= 0) return;

            var app = System.Windows.Application.Current;
            if (app == null) return;

            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                var win = new VoucherEntryWindow(id.Value);
                win.Owner = app.MainWindow;
                win.Show();
            }));
        }

        private static XRTableCell CreateAmountCell(XtraReport report, bool isDebit, float weight)
        {
            var cell = CreateCell(string.Empty, weight, TextAlignment.MiddleRight, FontRegular);
            cell.BeforePrint += (s, e) =>
            {
                var row = report.GetCurrentRow() as DiaryRow;
                var target = (XRTableCell)s;
                decimal value = isDebit ? (row?.Debit ?? 0) : (row?.Credit ?? 0);
                target.Text = value == 0m ? string.Empty : value.ToString("N0");
            };
            return cell;
        }

        private static XRTable CreateTable(float width, float height)
        {
            return new XRTable
            {
                LocationF = new PointF(0, 0),
                SizeF = new SizeF(width, height),
                Font = FontRegular
            };
        }

        private static XRTableCell BoundCell(string field, string format, float weight,
            TextAlignment align, Font font)
        {
            var cell = CreateCell(string.Empty, weight, align, font);
            if (string.IsNullOrEmpty(format))
                cell.DataBindings.Add("Text", null, field);
            else
                cell.DataBindings.Add(new XRBinding("Text", null, field, format));
            return cell;
        }

        private static XRTableCell CreateCell(string text, float weight, TextAlignment align, Font font)
        {
            return new XRTableCell
            {
                Text = text,
                Weight = weight,
                TextAlignment = align,
                Font = font,
                Padding = new PaddingInfo(3, 3, 1, 1),
                CanGrow = true
            };
        }

        private static XRLabel CreateLabel(string text, float x, float y, float width, float height,
            TextAlignment align, float fontSize, FontStyle style)
        {
            return new XRLabel
            {
                Text = text,
                LocationF = new PointF(x, y),
                SizeF = new SizeF(width, height),
                Font = new Font("Times New Roman", fontSize, style),
                TextAlignment = align,
                CanGrow = true
            };
        }
    }
}
