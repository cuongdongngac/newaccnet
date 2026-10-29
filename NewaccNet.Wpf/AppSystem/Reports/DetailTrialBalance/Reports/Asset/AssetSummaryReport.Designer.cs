using DevExpress.XtraReports.UI;
using System.Drawing;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset
{
    partial class AssetSummaryReport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraReports.UI.XRSummary xrSummaryStt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryBeginAmt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryInAmt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryOutAmt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryEndAmt = new DevExpress.XtraReports.UI.XRSummary();

            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompany = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.rowH1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Name = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Unit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Begin = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_In = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Out = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_End = new DevExpress.XtraReports.UI.XRTableCell();
            this.rowH2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Name2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Unit2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_BeginQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_BeginAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellUnit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellBeginQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellBeginAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellInQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellInAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellOutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellOutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellEndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellEndAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.rfTable = new DevExpress.XtraReports.UI.XRTable();
            this.rfRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.rfBlankStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankUnit = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankBeginQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBeginAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankInQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfInAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankOutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfOutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankEndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfEndAmt = new DevExpress.XtraReports.UI.XRTableCell();

            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rfTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // TopMargin & BottomMargin
            this.TopMargin.HeightF = 30F;
            this.BottomMargin.HeightF = 30F;

            // ReportHeader
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.lblCompany,
                this.lblTitle,
                this.lblDate});
            this.ReportHeader.HeightF = 85F;

            this.lblCompany.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.SizeF = new System.Drawing.SizeF(400F, 20F);
            this.lblCompany.Text = "";

            this.lblTitle.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 25F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(1060F, 28F);
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblTitle.Text = "TÀI KHOẢN 211";

            this.lblDate.Font = new System.Drawing.Font("Arial", 9.5F, System.Drawing.FontStyle.Italic);
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(0F, 53F);
            this.lblDate.SizeF = new System.Drawing.SizeF(1060F, 22F);
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblDate.Text = "Từ ngày: 01/01/26 Đến ngày: 31/12/26";

            // PageHeader
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.headerTable });
            this.PageHeader.HeightF = 50F;

            this.headerTable.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.headerTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.headerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.headerTable.SizeF = new System.Drawing.SizeF(1060F, 50F);
            this.headerTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.headerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.rowH1, this.rowH2 });

            // rowH1
            this.rowH1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellH_Stt, this.cellH_Name, this.cellH_Unit, this.cellH_Begin, this.cellH_In, this.cellH_Out, this.cellH_End
            });
            this.rowH1.HeightF = 25F;
            this.cellH_Stt.Text = "STT"; this.cellH_Stt.WidthF = 35F;
            this.cellH_Name.Text = "Tên tài sản cố định"; this.cellH_Name.WidthF = 265F;
            this.cellH_Unit.Text = "ĐVT"; this.cellH_Unit.WidthF = 50F;
            this.cellH_Begin.Text = "Dư đầu kỳ"; this.cellH_Begin.WidthF = 175F;
            this.cellH_In.Text = "Tăng trong kỳ"; this.cellH_In.WidthF = 175F;
            this.cellH_Out.Text = "Giảm trong kỳ"; this.cellH_Out.WidthF = 175F;
            this.cellH_End.Text = "Dư cuối kỳ"; this.cellH_End.WidthF = 185F;

            // rowH2
            this.rowH2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellH_Stt2, this.cellH_Name2, this.cellH_Unit2,
                this.cellH_BeginQty, this.cellH_BeginAmt,
                this.cellH_InQty, this.cellH_InAmt,
                this.cellH_OutQty, this.cellH_OutAmt,
                this.cellH_EndQty, this.cellH_EndAmt
            });
            this.rowH2.HeightF = 25F;
            this.cellH_Stt2.Text = ""; this.cellH_Stt2.WidthF = 35F;
            this.cellH_Name2.Text = ""; this.cellH_Name2.WidthF = 265F;
            this.cellH_Unit2.Text = ""; this.cellH_Unit2.WidthF = 50F;
            this.cellH_BeginQty.Text = "Số lượng"; this.cellH_BeginQty.WidthF = 65F;
            this.cellH_BeginAmt.Text = "Thành tiền"; this.cellH_BeginAmt.WidthF = 110F;
            this.cellH_InQty.Text = "Số lượng"; this.cellH_InQty.WidthF = 65F;
            this.cellH_InAmt.Text = "Thành tiền"; this.cellH_InAmt.WidthF = 110F;
            this.cellH_OutQty.Text = "Số lượng"; this.cellH_OutQty.WidthF = 65F;
            this.cellH_OutAmt.Text = "Thành tiền"; this.cellH_OutAmt.WidthF = 110F;
            this.cellH_EndQty.Text = "Số lượng"; this.cellH_EndQty.WidthF = 65F;
            this.cellH_EndAmt.Text = "Thành tiền"; this.cellH_EndAmt.WidthF = 120F;

            // DetailBand
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.detailTable });
            this.Detail.HeightF = 24F;

            this.detailTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.detailTable.Font = new System.Drawing.Font("Arial", 9F);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.SizeF = new System.Drawing.SizeF(1060F, 24F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.detailRow });

            this.detailRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellStt, this.cellName, this.cellUnit,
                this.cellBeginQty, this.cellBeginAmt,
                this.cellInQty, this.cellInAmt,
                this.cellOutQty, this.cellOutAmt,
                this.cellEndQty, this.cellEndAmt
            });
            this.detailRow.HeightF = 24F;

            this.cellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            xrSummaryStt.Func = DevExpress.XtraReports.UI.SummaryFunc.RecordNumber;
            xrSummaryStt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellStt.Summary = xrSummaryStt;
            this.cellStt.WidthF = 35F;

            this.cellName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AssetName]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AssetId]")
            });
            this.cellName.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 2, 0, 0, 100F);
            this.cellName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellName.WidthF = 265F;

            this.cellUnit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Unit]")
            });
            this.cellUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellUnit.WidthF = 50F;

            this.cellBeginQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([BeginQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [BeginQty]))")
            });
            this.cellBeginQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellBeginQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellBeginQty.WidthF = 65F;

            this.cellBeginAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([BeginAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [BeginAmount]))")
            });
            this.cellBeginAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellBeginAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellBeginAmt.WidthF = 110F;

            this.cellInQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([InQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [InQty]))")
            });
            this.cellInQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellInQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellInQty.WidthF = 65F;

            this.cellInAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([InAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [InAmount]))")
            });
            this.cellInAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellInAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellInAmt.WidthF = 110F;

            this.cellOutQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([OutQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [OutQty]))")
            });
            this.cellOutQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellOutQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellOutQty.WidthF = 65F;

            this.cellOutAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([OutAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [OutAmount]))")
            });
            this.cellOutAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellOutAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellOutAmt.WidthF = 110F;

            this.cellEndQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([EndQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [EndQty]))")
            });
            this.cellEndQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellEndQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellEndQty.WidthF = 65F;

            this.cellEndAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([EndAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [EndAmount]))")
            });
            this.cellEndAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellEndAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellEndAmt.WidthF = 120F;

            // ReportFooter
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.rfTable });
            this.ReportFooter.HeightF = 28F;

            this.rfTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.rfTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rfTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.rfTable.SizeF = new System.Drawing.SizeF(1060F, 28F);
            this.rfTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.rfRow });

            this.rfRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.rfBlankStt, this.rfLabel, this.rfBlankUnit,
                this.rfBlankBeginQty, this.rfBeginAmt,
                this.rfBlankInQty, this.rfInAmt,
                this.rfBlankOutQty, this.rfOutAmt,
                this.rfBlankEndQty, this.rfEndAmt
            });
            this.rfRow.HeightF = 28F;

            this.rfBlankStt.WidthF = 35F;
            this.rfLabel.Text = "Cộng";
            this.rfLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.rfLabel.WidthF = 265F;
            this.rfBlankUnit.WidthF = 50F;

            this.rfBlankBeginQty.WidthF = 65F;
            this.rfBeginAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([BeginAmount])")
            });
            xrSummaryBeginAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            xrSummaryBeginAmt.FormatString = "{0:#,##0}";
            this.rfBeginAmt.Summary = xrSummaryBeginAmt;
            this.rfBeginAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfBeginAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfBeginAmt.WidthF = 110F;

            this.rfBlankInQty.WidthF = 65F;
            this.rfInAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([InAmount])")
            });
            xrSummaryInAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            xrSummaryInAmt.FormatString = "{0:#,##0}";
            this.rfInAmt.Summary = xrSummaryInAmt;
            this.rfInAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfInAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfInAmt.WidthF = 110F;

            this.rfBlankOutQty.WidthF = 65F;
            this.rfOutAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([OutAmount])")
            });
            xrSummaryOutAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            xrSummaryOutAmt.FormatString = "{0:#,##0}";
            this.rfOutAmt.Summary = xrSummaryOutAmt;
            this.rfOutAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfOutAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfOutAmt.WidthF = 110F;

            this.rfBlankEndQty.WidthF = 65F;
            this.rfEndAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([EndAmount])")
            });
            xrSummaryEndAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            xrSummaryEndAmt.FormatString = "{0:#,##0}";
            this.rfEndAmt.Summary = xrSummaryEndAmt;
            this.rfEndAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfEndAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfEndAmt.WidthF = 120F;

            // Page properties: A4 Landscape
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.TopMargin,
                this.BottomMargin,
                this.ReportHeader,
                this.PageHeader,
                this.Detail,
                this.ReportFooter
            });
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(50, 50, 30, 30);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Version = "25.2";

            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rfTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel lblCompany;
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDate;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow rowH1;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Name;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Unit;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Begin;
        private DevExpress.XtraReports.UI.XRTableCell cellH_In;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Out;
        private DevExpress.XtraReports.UI.XRTableCell cellH_End;
        private DevExpress.XtraReports.UI.XRTableRow rowH2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Name2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Unit2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_BeginQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_BeginAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndAmt;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailRow;
        private DevExpress.XtraReports.UI.XRTableCell cellStt;
        private DevExpress.XtraReports.UI.XRTableCell cellName;
        private DevExpress.XtraReports.UI.XRTableCell cellUnit;
        private DevExpress.XtraReports.UI.XRTableCell cellBeginQty;
        private DevExpress.XtraReports.UI.XRTableCell cellBeginAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellInQty;
        private DevExpress.XtraReports.UI.XRTableCell cellInAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellOutQty;
        private DevExpress.XtraReports.UI.XRTableCell cellOutAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellEndQty;
        private DevExpress.XtraReports.UI.XRTableCell cellEndAmt;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable rfTable;
        private DevExpress.XtraReports.UI.XRTableRow rfRow;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankStt;
        private DevExpress.XtraReports.UI.XRTableCell rfLabel;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankUnit;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankBeginQty;
        private DevExpress.XtraReports.UI.XRTableCell rfBeginAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankInQty;
        private DevExpress.XtraReports.UI.XRTableCell rfInAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankOutQty;
        private DevExpress.XtraReports.UI.XRTableCell rfOutAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankEndQty;
        private DevExpress.XtraReports.UI.XRTableCell rfEndAmt;
    }
}
