namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Cost
{
    partial class CostDetailReport
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
            DevExpress.XtraReports.UI.XRSummary xrSummaryDr = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryCr = new DevExpress.XtraReports.UI.XRSummary();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompany = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPeriod = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAccount = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCostObject = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCostElement = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.rowH1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Date = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_No = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Contents = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_CounterAcc = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Dr = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Cr = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndDr = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndCr = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellVoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellVoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellContents = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCounterAccount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDebitAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCreditAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellClosingDebitAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellClosingCreditAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.footerTable = new DevExpress.XtraReports.UI.XRTable();
            this.footerRowTotal = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellF_Label = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_Dr = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_Cr = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_Blank1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_Blank2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.footerRowClosing = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellC_Label = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellC_EndDr = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellC_EndCr = new DevExpress.XtraReports.UI.XRTableCell();
            this.lblDateSig = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig3 = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.footerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.CostDetailReportDTO);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 30F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 30F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblCompany,
            this.lblTitle,
            this.lblPeriod,
            this.lblAccount,
            this.lblCostObject,
            this.lblCostElement});
            this.ReportHeader.HeightF = 130F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblCompany
            // 
            this.lblCompany.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmCompanyName")});
            this.lblCompany.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.SizeF = new System.Drawing.SizeF(600F, 20F);
            this.lblCompany.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Segoe UI", 16F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 24F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(1100F, 28F);
            this.lblTitle.Text = "SỔ CHI TIẾT CHI PHÍ";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblPeriod
            // 
            this.lblPeriod.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblPeriod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblPeriod.LocationFloat = new DevExpress.Utils.PointFloat(0F, 52F);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.SizeF = new System.Drawing.SizeF(1100F, 20F);
            this.lblPeriod.Text = "Từ ngày: ... Đến ngày: ...";
            this.lblPeriod.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblAccount
            // 
            this.lblAccount.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblAccount.LocationFloat = new DevExpress.Utils.PointFloat(0F, 78F);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.SizeF = new System.Drawing.SizeF(550F, 20F);
            this.lblAccount.Text = "Tài khoản: ...";
            this.lblAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblCostObject
            // 
            this.lblCostObject.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCostObject.LocationFloat = new DevExpress.Utils.PointFloat(550F, 78F);
            this.lblCostObject.Name = "lblCostObject";
            this.lblCostObject.SizeF = new System.Drawing.SizeF(550F, 20F);
            this.lblCostObject.Text = "Đối tượng chi phí: ...";
            this.lblCostObject.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblCostElement
            // 
            this.lblCostElement.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblCostElement.LocationFloat = new DevExpress.Utils.PointFloat(0F, 102F);
            this.lblCostElement.Name = "lblCostElement";
            this.lblCostElement.SizeF = new System.Drawing.SizeF(1100F, 20F);
            this.lblCostElement.Text = "Yếu tố chi phí: ...";
            this.lblCostElement.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.headerTable});
            this.PageHeader.HeightF = 32F;
            this.PageHeader.Name = "PageHeader";
            // 
            // headerTable
            // 
            this.headerTable.BackColor = System.Drawing.Color.White;
            this.headerTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.headerTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.headerTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, ((DevExpress.Drawing.DXFontStyle)((DevExpress.Drawing.DXFontStyle.Bold | DevExpress.Drawing.DXFontStyle.Italic))));
            this.headerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.headerTable.Name = "headerTable";
            this.headerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowH1});
            this.headerTable.SizeF = new System.Drawing.SizeF(1100F, 32F);
            this.headerTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // rowH1
            // 
            this.rowH1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellH_Stt,
            this.cellH_Date,
            this.cellH_No,
            this.cellH_Contents,
            this.cellH_CounterAcc,
            this.cellH_Dr,
            this.cellH_Cr,
            this.cellH_EndDr,
            this.cellH_EndCr});
            this.rowH1.Name = "rowH1";
            this.rowH1.Weight = 1D;
            // 
            // cellH_Stt
            // 
            this.cellH_Stt.Name = "cellH_Stt";
            this.cellH_Stt.Text = "STT";
            this.cellH_Stt.Weight = 0.036363636363636362D;
            // 
            // cellH_Date
            // 
            this.cellH_Date.Name = "cellH_Date";
            this.cellH_Date.Text = "Ngày CT";
            this.cellH_Date.Weight = 0.072727272727272724D;
            // 
            // cellH_No
            // 
            this.cellH_No.Name = "cellH_No";
            this.cellH_No.Text = "Số CT / Phiếu";
            this.cellH_No.Weight = 0.09090909090909091D;
            // 
            // cellH_Contents
            // 
            this.cellH_Contents.Name = "cellH_Contents";
            this.cellH_Contents.Text = "Diễn giải";
            this.cellH_Contents.Weight = 0.2909090909090909D;
            // 
            // cellH_CounterAcc
            // 
            this.cellH_CounterAcc.Name = "cellH_CounterAcc";
            this.cellH_CounterAcc.Text = "TK ĐƯ";
            this.cellH_CounterAcc.Weight = 0.05454545454545454D;
            // 
            // cellH_Dr
            // 
            this.cellH_Dr.Name = "cellH_Dr";
            this.cellH_Dr.Text = "Phát sinh Nợ";
            this.cellH_Dr.Weight = 0.11363636363636363D;
            // 
            // cellH_Cr
            // 
            this.cellH_Cr.Name = "cellH_Cr";
            this.cellH_Cr.Text = "Phát sinh Có";
            this.cellH_Cr.Weight = 0.11363636363636363D;
            // 
            // cellH_EndDr
            // 
            this.cellH_EndDr.Name = "cellH_EndDr";
            this.cellH_EndDr.Text = "Dư Nợ";
            this.cellH_EndDr.Weight = 0.11363636363636363D;
            // 
            // cellH_EndCr
            // 
            this.cellH_EndCr.Name = "cellH_EndCr";
            this.cellH_EndCr.Text = "Dư Có";
            this.cellH_EndCr.Weight = 0.11363636363636363D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.detailTable});
            this.Detail.HeightF = 24F;
            this.Detail.Name = "Detail";
            // 
            // detailTable
            // 
            this.detailTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.detailTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.detailTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.Name = "detailTable";
            this.detailTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3F, 3F, 0F, 0F, 100F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.detailRow});
            this.detailTable.SizeF = new System.Drawing.SizeF(1100F, 24F);
            // 
            // detailRow
            // 
            this.detailRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellStt,
            this.cellVoucherDate,
            this.cellVoucherNo,
            this.cellContents,
            this.cellCounterAccount,
            this.cellDebitAmount,
            this.cellCreditAmount,
            this.cellClosingDebitAmount,
            this.cellClosingCreditAmount});
            this.detailRow.Name = "detailRow";
            this.detailRow.Weight = 1D;
            // 
            // cellStt
            // 
            this.cellStt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([IsOpeningRow], \'\', sumRecordNumber())"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellStt.Name = "cellStt";
            xrSummaryStt.Func = DevExpress.XtraReports.UI.SummaryFunc.RecordNumber;
            xrSummaryStt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellStt.Summary = xrSummaryStt;
            this.cellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellStt.Weight = 0.036363636363636362D;
            // 
            // cellVoucherDate
            // 
            this.cellVoucherDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherDate]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellVoucherDate.Name = "cellVoucherDate";
            this.cellVoucherDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellVoucherDate.TextFormatString = "{0:dd/MM/yyyy}";
            this.cellVoucherDate.Weight = 0.072727272727272724D;
            // 
            // cellVoucherNo
            // 
            this.cellVoucherNo.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherNo]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellVoucherNo.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellVoucherNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.cellVoucherNo.Name = "cellVoucherNo";
            this.cellVoucherNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellVoucherNo.Weight = 0.09090909090909091D;
            // 
            // cellContents
            // 
            this.cellContents.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Contents]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellContents.Name = "cellContents";
            this.cellContents.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellContents.Weight = 0.2909090909090909D;
            // 
            // cellCounterAccount
            // 
            this.cellCounterAccount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountId]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCounterAccount.Name = "cellCounterAccount";
            this.cellCounterAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellCounterAccount.Weight = 0.05454545454545454D;
            // 
            // cellDebitAmount
            // 
            this.cellDebitAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellDebitAmount.Name = "cellDebitAmount";
            this.cellDebitAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDebitAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellDebitAmount.Weight = 0.11363636363636363D;
            // 
            // cellCreditAmount
            // 
            this.cellCreditAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCreditAmount.Name = "cellCreditAmount";
            this.cellCreditAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellCreditAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellCreditAmount.Weight = 0.11363636363636363D;
            // 
            // cellClosingDebitAmount
            // 
            this.cellClosingDebitAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingDebitAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellClosingDebitAmount.Name = "cellClosingDebitAmount";
            this.cellClosingDebitAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellClosingDebitAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellClosingDebitAmount.Weight = 0.11363636363636363D;
            // 
            // cellClosingCreditAmount
            // 
            this.cellClosingCreditAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingCreditAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellClosingCreditAmount.Name = "cellClosingCreditAmount";
            this.cellClosingCreditAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellClosingCreditAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellClosingCreditAmount.Weight = 0.11363636363636363D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.footerTable,
            this.lblDateSig,
            this.lblSig1,
            this.lblSig2,
            this.lblSig3});
            this.ReportFooter.HeightF = 140F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // footerTable
            // 
            this.footerTable.BackColor = System.Drawing.Color.Transparent;
            this.footerTable.BorderColor = System.Drawing.Color.Transparent;
            this.footerTable.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.footerTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.footerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.footerTable.Name = "footerTable";
            this.footerTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3F, 3F, 0F, 0F, 100F);
            this.footerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.footerRowTotal,
            this.footerRowClosing});
            this.footerTable.SizeF = new System.Drawing.SizeF(1100F, 52F);
            // 
            // footerRowTotal
            // 
            this.footerRowTotal.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellF_Label,
            this.cellF_Dr,
            this.cellF_Cr,
            this.cellF_Blank1,
            this.cellF_Blank2});
            this.footerRowTotal.Name = "footerRowTotal";
            this.footerRowTotal.Weight = 0.5D;
            // 
            // cellF_Label
            // 
            this.cellF_Label.Name = "cellF_Label";
            this.cellF_Label.Text = "Cộng phát sinh trong kỳ";
            this.cellF_Label.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellF_Label.Weight = 0.54545454545454541D;
            // 
            // cellF_Dr
            // 
            this.cellF_Dr.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")});
            this.cellF_Dr.Name = "cellF_Dr";
            xrSummaryDr.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellF_Dr.Summary = xrSummaryDr;
            this.cellF_Dr.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellF_Dr.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellF_Dr.Weight = 0.11363636363636363D;
            // 
            // cellF_Cr
            // 
            this.cellF_Cr.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")});
            this.cellF_Cr.Name = "cellF_Cr";
            xrSummaryCr.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellF_Cr.Summary = xrSummaryCr;
            this.cellF_Cr.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellF_Cr.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellF_Cr.Weight = 0.11363636363636363D;
            // 
            // cellF_Blank1
            // 
            this.cellF_Blank1.Name = "cellF_Blank1";
            this.cellF_Blank1.Weight = 0.11363636363636363D;
            // 
            // cellF_Blank2
            // 
            this.cellF_Blank2.Name = "cellF_Blank2";
            this.cellF_Blank2.Weight = 0.11363636363636363D;
            // 
            // footerRowClosing
            // 
            this.footerRowClosing.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellC_Label,
            this.cellC_EndDr,
            this.cellC_EndCr});
            this.footerRowClosing.Name = "footerRowClosing";
            this.footerRowClosing.Weight = 0.5D;
            // 
            // cellC_Label
            // 
            this.cellC_Label.Name = "cellC_Label";
            this.cellC_Label.Text = "Số dư cuối kỳ";
            this.cellC_Label.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellC_Label.Weight = 0.77272727272727271D;
            // 
            // cellC_EndDr
            // 
            this.cellC_EndDr.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingDebitAmount]")});
            this.cellC_EndDr.Name = "cellC_EndDr";
            this.cellC_EndDr.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellC_EndDr.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellC_EndDr.Weight = 0.11363636363636363D;
            // 
            // cellC_EndCr
            // 
            this.cellC_EndCr.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingCreditAmount]")});
            this.cellC_EndCr.Name = "cellC_EndCr";
            this.cellC_EndCr.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellC_EndCr.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellC_EndCr.Weight = 0.11363636363636363D;
            // 
            // lblDateSig
            // 
            this.lblDateSig.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateSig.LocationFloat = new DevExpress.Utils.PointFloat(733.34F, 40F);
            this.lblDateSig.Name = "lblDateSig";
            this.lblDateSig.SizeF = new System.Drawing.SizeF(366.66F, 20F);
            this.lblDateSig.Text = "Lập biểu ngày      tháng     năm";
            this.lblDateSig.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSig1
            // 
            this.lblSig1.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 65F);
            this.lblSig1.Multiline = true;
            this.lblSig1.Name = "lblSig1";
            this.lblSig1.SizeF = new System.Drawing.SizeF(366.67F, 40F);
            this.lblSig1.Text = "Người lập biểu\r\n(Ký, họ tên)";
            this.lblSig1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSig2
            // 
            this.lblSig2.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig2.LocationFloat = new DevExpress.Utils.PointFloat(366.67F, 65F);
            this.lblSig2.Multiline = true;
            this.lblSig2.Name = "lblSig2";
            this.lblSig2.SizeF = new System.Drawing.SizeF(366.67F, 40F);
            this.lblSig2.Text = "Kế toán trưởng\r\n(Ký, họ tên)";
            this.lblSig2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSig3
            // 
            this.lblSig3.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig3.LocationFloat = new DevExpress.Utils.PointFloat(733.34F, 65F);
            this.lblSig3.Multiline = true;
            this.lblSig3.Name = "lblSig3";
            this.lblSig3.SizeF = new System.Drawing.SizeF(366.66F, 40F);
            this.lblSig3.Text = "Thủ trưởng đơn vị\r\n(Ký, họ tên, đóng dấu)";
            this.lblSig3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Tên công ty";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
            this.prmCompanyName.Visible = false;
            // 
            // CostDetailReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageHeader,
            this.Detail,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F);
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(35F, 35F, 30F, 30F);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.footerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel lblCompany;
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblPeriod;
        private DevExpress.XtraReports.UI.XRLabel lblAccount;
        private DevExpress.XtraReports.UI.XRLabel lblCostObject;
        private DevExpress.XtraReports.UI.XRLabel lblCostElement;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow rowH1;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Date;
        private DevExpress.XtraReports.UI.XRTableCell cellH_No;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Contents;
        private DevExpress.XtraReports.UI.XRTableCell cellH_CounterAcc;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Dr;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Cr;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndDr;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndCr;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailRow;
        private DevExpress.XtraReports.UI.XRTableCell cellStt;
        private DevExpress.XtraReports.UI.XRTableCell cellVoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell cellVoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell cellContents;
        private DevExpress.XtraReports.UI.XRTableCell cellCounterAccount;
        private DevExpress.XtraReports.UI.XRTableCell cellDebitAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellCreditAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellClosingDebitAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellClosingCreditAmount;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable footerTable;
        private DevExpress.XtraReports.UI.XRTableRow footerRowTotal;
        private DevExpress.XtraReports.UI.XRTableCell cellF_Label;
        private DevExpress.XtraReports.UI.XRTableCell cellF_Dr;
        private DevExpress.XtraReports.UI.XRTableCell cellF_Cr;
        private DevExpress.XtraReports.UI.XRTableCell cellF_Blank1;
        private DevExpress.XtraReports.UI.XRTableCell cellF_Blank2;
        private DevExpress.XtraReports.UI.XRTableRow footerRowClosing;
        private DevExpress.XtraReports.UI.XRTableCell cellC_Label;
        private DevExpress.XtraReports.UI.XRTableCell cellC_EndDr;
        private DevExpress.XtraReports.UI.XRTableCell cellC_EndCr;
        private DevExpress.XtraReports.UI.XRLabel lblDateSig;
        private DevExpress.XtraReports.UI.XRLabel lblSig1;
        private DevExpress.XtraReports.UI.XRLabel lblSig2;
        private DevExpress.XtraReports.UI.XRLabel lblSig3;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
    }
}
