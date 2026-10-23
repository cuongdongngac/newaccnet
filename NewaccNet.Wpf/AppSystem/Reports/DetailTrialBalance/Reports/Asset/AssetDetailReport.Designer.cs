using DevExpress.XtraReports.UI;
using System.Drawing;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset
{
    partial class AssetDetailReport
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
            DevExpress.XtraReports.UI.XRSummary xrSummaryDebitAmt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryCreditAmt = new DevExpress.XtraReports.UI.XRSummary();

            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompany = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSubtitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.rowH1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Date = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_No = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Desc = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Acc = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Reason = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Price = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_In = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Out = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Bal = new DevExpress.XtraReports.UI.XRTableCell();
            this.rowH2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Date2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_No2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Desc2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Acc2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Reason2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Price2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_BalQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_BalAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDesc = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellAcc = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellReason = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellInQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellInAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellOutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellOutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellBalQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellBalAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.rfTable = new DevExpress.XtraReports.UI.XRTable();
            this.rfRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.rfLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankInQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfInAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankOutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfOutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBlankBalQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBalAmt = new DevExpress.XtraReports.UI.XRTableCell();

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
                this.lblSubtitle,
                this.lblDate});
            this.ReportHeader.HeightF = 100F;

            this.lblCompany.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.SizeF = new System.Drawing.SizeF(400F, 20F);
            this.lblCompany.Text = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";

            this.lblTitle.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 22F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(1060F, 26F);
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblTitle.Text = "SỔ CHI TIẾT TÀI SẢN CỐ ĐỊNH";

            this.lblSubtitle.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.lblSubtitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 48F);
            this.lblSubtitle.SizeF = new System.Drawing.SizeF(1060F, 22F);
            this.lblSubtitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblSubtitle.Text = "Tài sản: ...";

            this.lblDate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Italic);
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(0F, 72F);
            this.lblDate.SizeF = new System.Drawing.SizeF(1060F, 20F);
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblDate.Text = "Từ ngày ... Đến ngày ...";

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
                this.cellH_Date, this.cellH_No, this.cellH_Desc, this.cellH_Acc, this.cellH_Reason, this.cellH_Price,
                this.cellH_In, this.cellH_Out, this.cellH_Bal
            });
            this.rowH1.HeightF = 25F;
            this.cellH_Date.Text = "Ngày CT"; this.cellH_Date.WidthF = 75F;
            this.cellH_No.Text = "Số CT"; this.cellH_No.WidthF = 75F;
            this.cellH_Desc.Text = "Diễn giải"; this.cellH_Desc.WidthF = 210F;
            this.cellH_Acc.Text = "TK ĐƯ"; this.cellH_Acc.WidthF = 55F;
            this.cellH_Reason.Text = "Lý do"; this.cellH_Reason.WidthF = 70F;
            this.cellH_Price.Text = "Đơn giá"; this.cellH_Price.WidthF = 85F;
            this.cellH_In.Text = "Tăng trong kỳ"; this.cellH_In.WidthF = 160F;
            this.cellH_Out.Text = "Giảm trong kỳ"; this.cellH_Out.WidthF = 160F;
            this.cellH_Bal.Text = "Số dư lũy kế"; this.cellH_Bal.WidthF = 170F;

            // rowH2
            this.rowH2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellH_Date2, this.cellH_No2, this.cellH_Desc2, this.cellH_Acc2, this.cellH_Reason2, this.cellH_Price2,
                this.cellH_InQty, this.cellH_InAmt,
                this.cellH_OutQty, this.cellH_OutAmt,
                this.cellH_BalQty, this.cellH_BalAmt
            });
            this.rowH2.HeightF = 25F;
            this.cellH_Date2.Text = ""; this.cellH_Date2.WidthF = 75F;
            this.cellH_No2.Text = ""; this.cellH_No2.WidthF = 75F;
            this.cellH_Desc2.Text = ""; this.cellH_Desc2.WidthF = 210F;
            this.cellH_Acc2.Text = ""; this.cellH_Acc2.WidthF = 55F;
            this.cellH_Reason2.Text = ""; this.cellH_Reason2.WidthF = 70F;
            this.cellH_Price2.Text = ""; this.cellH_Price2.WidthF = 85F;
            this.cellH_InQty.Text = "Số lượng"; this.cellH_InQty.WidthF = 55F;
            this.cellH_InAmt.Text = "Số tiền"; this.cellH_InAmt.WidthF = 105F;
            this.cellH_OutQty.Text = "Số lượng"; this.cellH_OutQty.WidthF = 55F;
            this.cellH_OutAmt.Text = "Số tiền"; this.cellH_OutAmt.WidthF = 105F;
            this.cellH_BalQty.Text = "Số lượng"; this.cellH_BalQty.WidthF = 55F;
            this.cellH_BalAmt.Text = "Số tiền"; this.cellH_BalAmt.WidthF = 115F;

            // DetailBand
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.detailTable });
            this.Detail.HeightF = 24F;

            this.detailTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.detailTable.Font = new System.Drawing.Font("Arial", 9F);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.SizeF = new System.Drawing.SizeF(1060F, 24F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.detailRow });

            this.detailRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellDate, this.cellNo, this.cellDesc, this.cellAcc, this.cellReason, this.cellPrice,
                this.cellInQty, this.cellInAmt,
                this.cellOutQty, this.cellOutAmt,
                this.cellBalQty, this.cellBalAmt
            });
            this.detailRow.HeightF = 24F;

            this.cellDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherDate]")
            });
            this.cellDate.TextFormatString = "{0:dd/MM/yyyy}";
            this.cellDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellDate.WidthF = 75F;

            this.cellNo.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherNo]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")
            });
            this.cellNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellNo.WidthF = 75F;

            this.cellDesc.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Contents]")
            });
            this.cellDesc.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 2, 0, 0, 100F);
            this.cellDesc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellDesc.WidthF = 210F;

            this.cellAcc.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountId]")
            });
            this.cellAcc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellAcc.WidthF = 55F;

            this.cellReason.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ReasonName]")
            });
            this.cellReason.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellReason.WidthF = 70F;

            this.cellPrice.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([Price] == 0, \'\', FormatString(\'{0:#,##0}\', [Price]))")
            });
            this.cellPrice.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellPrice.WidthF = 85F;

            this.cellInQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([DebitQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [DebitQty]))")
            });
            this.cellInQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellInQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellInQty.WidthF = 55F;

            this.cellInAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([DebitAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [DebitAmount]))")
            });
            this.cellInAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellInAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellInAmt.WidthF = 105F;

            this.cellOutQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([CreditQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [CreditQty]))")
            });
            this.cellOutQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellOutQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellOutQty.WidthF = 55F;

            this.cellOutAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([CreditAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [CreditAmount]))")
            });
            this.cellOutAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellOutAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellOutAmt.WidthF = 105F;

            this.cellBalQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([ClosingQty] == 0, \'\', FormatString(\'{0:#,##0.##}\', [ClosingQty]))")
            });
            this.cellBalQty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellBalQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellBalQty.WidthF = 55F;

            this.cellBalAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([ClosingAmount] == 0, \'\', FormatString(\'{0:#,##0}\', [ClosingAmount]))")
            });
            this.cellBalAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellBalAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellBalAmt.WidthF = 115F;

            // ReportFooter
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] { this.rfTable });
            this.ReportFooter.HeightF = 28F;

            this.rfTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.rfTable.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.rfTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.rfTable.SizeF = new System.Drawing.SizeF(1060F, 28F);
            this.rfTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] { this.rfRow });

            this.rfRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.rfLabel,
                this.rfBlankInQty, this.rfInAmt,
                this.rfBlankOutQty, this.rfOutAmt,
                this.rfBlankBalQty, this.rfBalAmt
            });
            this.rfRow.HeightF = 28F;

            this.rfLabel.Text = "Cộng phát sinh trong kỳ";
            this.rfLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.rfLabel.WidthF = 570F;

            this.rfBlankInQty.WidthF = 55F;
            this.rfInAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")
            });
            xrSummaryDebitAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            xrSummaryDebitAmt.FormatString = "{0:#,##0}";
            this.rfInAmt.Summary = xrSummaryDebitAmt;
            this.rfInAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfInAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfInAmt.WidthF = 105F;

            this.rfBlankOutQty.WidthF = 55F;
            this.rfOutAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")
            });
            xrSummaryCreditAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            xrSummaryCreditAmt.FormatString = "{0:#,##0}";
            this.rfOutAmt.Summary = xrSummaryCreditAmt;
            this.rfOutAmt.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfOutAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfOutAmt.WidthF = 105F;

            this.rfBlankBalQty.WidthF = 55F;
            this.rfBalAmt.WidthF = 115F;

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
        private DevExpress.XtraReports.UI.XRLabel lblSubtitle;
        private DevExpress.XtraReports.UI.XRLabel lblDate;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow rowH1;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Date;
        private DevExpress.XtraReports.UI.XRTableCell cellH_No;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Desc;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Acc;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Reason;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Price;
        private DevExpress.XtraReports.UI.XRTableCell cellH_In;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Out;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Bal;
        private DevExpress.XtraReports.UI.XRTableRow rowH2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Date2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_No2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Desc2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Acc2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Reason2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Price2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_BalQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_BalAmt;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailRow;
        private DevExpress.XtraReports.UI.XRTableCell cellDate;
        private DevExpress.XtraReports.UI.XRTableCell cellNo;
        private DevExpress.XtraReports.UI.XRTableCell cellDesc;
        private DevExpress.XtraReports.UI.XRTableCell cellAcc;
        private DevExpress.XtraReports.UI.XRTableCell cellReason;
        private DevExpress.XtraReports.UI.XRTableCell cellPrice;
        private DevExpress.XtraReports.UI.XRTableCell cellInQty;
        private DevExpress.XtraReports.UI.XRTableCell cellInAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellOutQty;
        private DevExpress.XtraReports.UI.XRTableCell cellOutAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellBalQty;
        private DevExpress.XtraReports.UI.XRTableCell cellBalAmt;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable rfTable;
        private DevExpress.XtraReports.UI.XRTableRow rfRow;
        private DevExpress.XtraReports.UI.XRTableCell rfLabel;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankInQty;
        private DevExpress.XtraReports.UI.XRTableCell rfInAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankOutQty;
        private DevExpress.XtraReports.UI.XRTableCell rfOutAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankBalQty;
        private DevExpress.XtraReports.UI.XRTableCell rfBalAmt;
    }
}
