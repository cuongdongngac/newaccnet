namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Currency
{
    partial class CurrencySummaryReport
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary3 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary4 = new DevExpress.XtraReports.UI.XRSummary();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
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
            this.cellH_Begin = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Receipt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Issue = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_End = new DevExpress.XtraReports.UI.XRTableCell();
            this.rowH2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt_Spacer = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Name_Spacer = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_BeginQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_BeginAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_ReceiptQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_ReceiptAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_IssueQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_IssueAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellBeginQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellBeginAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellReceiptQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellReceiptAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellIssueQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellIssueAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellEndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellEndAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.rfTable = new DevExpress.XtraReports.UI.XRTable();
            this.rfRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.rfBlankStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBeginQtyBlank = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfBeginAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfReceiptQtyBlank = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfReceiptAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfIssueQtyBlank = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfIssueAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfEndQtyBlank = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfEndAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.lblDateSig = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig3 = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rfTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.CurrencyReportDTO);
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
            this.lblDate});
            this.ReportHeader.HeightF = 80F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblCompany
            // 
            this.lblCompany.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmCompanyName")});
            this.lblCompany.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.SizeF = new System.Drawing.SizeF(1100F, 24F);
            this.lblCompany.Text = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
            this.lblCompany.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Segoe UI", 16F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 24F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(1100F, 28F);
            this.lblTitle.Text = "TÀI KHOẢN TIỀN NGOẠI TỆ";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(0F, 52F);
            this.lblDate.Name = "lblDate";
            this.lblDate.SizeF = new System.Drawing.SizeF(1100F, 20F);
            this.lblDate.Text = "Từ ngày ... Đến ngày ...";
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.headerTable});
            this.PageHeader.HeightF = 52F;
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
            this.rowH1,
            this.rowH2});
            this.headerTable.SizeF = new System.Drawing.SizeF(1100F, 52F);
            this.headerTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // rowH1
            // 
            this.rowH1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellH_Stt,
            this.cellH_Name,
            this.cellH_Begin,
            this.cellH_Receipt,
            this.cellH_Issue,
            this.cellH_End});
            this.rowH1.Name = "rowH1";
            this.rowH1.Weight = 0.5D;
            // 
            // cellH_Stt
            // 
            this.cellH_Stt.Name = "cellH_Stt";
            this.cellH_Stt.RowSpan = 2;
            this.cellH_Stt.Text = "STT";
            this.cellH_Stt.Weight = 0.045454545454545456D;
            // 
            // cellH_Name
            // 
            this.cellH_Name.Name = "cellH_Name";
            this.cellH_Name.RowSpan = 2;
            this.cellH_Name.Text = "Tên ngoại tệ";
            this.cellH_Name.Weight = 0.22727272727272727D;
            // 
            // cellH_Begin
            // 
            this.cellH_Begin.Name = "cellH_Begin";
            this.cellH_Begin.Text = "Dư đầu kỳ";
            this.cellH_Begin.Weight = 0.18181818181818182D;
            // 
            // cellH_Receipt
            // 
            this.cellH_Receipt.Name = "cellH_Receipt";
            this.cellH_Receipt.Text = "Thu trong kỳ";
            this.cellH_Receipt.Weight = 0.18181818181818182D;
            // 
            // cellH_Issue
            // 
            this.cellH_Issue.Name = "cellH_Issue";
            this.cellH_Issue.Text = "Chi trong kỳ";
            this.cellH_Issue.Weight = 0.18181818181818182D;
            // 
            // cellH_End
            // 
            this.cellH_End.Name = "cellH_End";
            this.cellH_End.Text = "Dư cuối kỳ";
            this.cellH_End.Weight = 0.18181818181818182D;
            // 
            // rowH2
            // 
            this.rowH2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellH_Stt_Spacer,
            this.cellH_Name_Spacer,
            this.cellH_BeginQty,
            this.cellH_BeginAmt,
            this.cellH_ReceiptQty,
            this.cellH_ReceiptAmt,
            this.cellH_IssueQty,
            this.cellH_IssueAmt,
            this.cellH_EndQty,
            this.cellH_EndAmt});
            this.rowH2.Name = "rowH2";
            this.rowH2.Weight = 0.5D;
            // 
            // cellH_Stt_Spacer
            // 
            this.cellH_Stt_Spacer.Name = "cellH_Stt_Spacer";
            this.cellH_Stt_Spacer.Weight = 0.045454545454545456D;
            // 
            // cellH_Name_Spacer
            // 
            this.cellH_Name_Spacer.Name = "cellH_Name_Spacer";
            this.cellH_Name_Spacer.Weight = 0.22727272727272727D;
            // 
            // cellH_BeginQty
            // 
            this.cellH_BeginQty.Name = "cellH_BeginQty";
            this.cellH_BeginQty.Text = "Nguyên tệ";
            this.cellH_BeginQty.Weight = 0.090909090909090912D;
            // 
            // cellH_BeginAmt
            // 
            this.cellH_BeginAmt.Name = "cellH_BeginAmt";
            this.cellH_BeginAmt.Text = "VNĐ";
            this.cellH_BeginAmt.Weight = 0.090909090909090912D;
            // 
            // cellH_ReceiptQty
            // 
            this.cellH_ReceiptQty.Name = "cellH_ReceiptQty";
            this.cellH_ReceiptQty.Text = "Nguyên tệ";
            this.cellH_ReceiptQty.Weight = 0.090909090909090912D;
            // 
            // cellH_ReceiptAmt
            // 
            this.cellH_ReceiptAmt.Name = "cellH_ReceiptAmt";
            this.cellH_ReceiptAmt.Text = "VNĐ";
            this.cellH_ReceiptAmt.Weight = 0.090909090909090912D;
            // 
            // cellH_IssueQty
            // 
            this.cellH_IssueQty.Name = "cellH_IssueQty";
            this.cellH_IssueQty.Text = "Nguyên tệ";
            this.cellH_IssueQty.Weight = 0.090909090909090912D;
            // 
            // cellH_IssueAmt
            // 
            this.cellH_IssueAmt.Name = "cellH_IssueAmt";
            this.cellH_IssueAmt.Text = "VNĐ";
            this.cellH_IssueAmt.Weight = 0.090909090909090912D;
            // 
            // cellH_EndQty
            // 
            this.cellH_EndQty.Name = "cellH_EndQty";
            this.cellH_EndQty.Text = "Nguyên tệ";
            this.cellH_EndQty.Weight = 0.090909090909090912D;
            // 
            // cellH_EndAmt
            // 
            this.cellH_EndAmt.Name = "cellH_EndAmt";
            this.cellH_EndAmt.Text = "VNĐ";
            this.cellH_EndAmt.Weight = 0.090909090909090912D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.detailTable});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.BeforePrint += new DevExpress.XtraReports.UI.BeforePrintEventHandler(this.Detail_BeforePrint);
            // 
            // detailTable
            // 
            this.detailTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.detailTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.detailTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.Name = "detailTable";
            this.detailTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3F, 3F, 0F, 0F, 100F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.detailRow});
            this.detailTable.SizeF = new System.Drawing.SizeF(1100F, 25F);
            // 
            // detailRow
            // 
            this.detailRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellStt,
            this.cellName,
            this.cellBeginQty,
            this.cellBeginAmt,
            this.cellReceiptQty,
            this.cellReceiptAmt,
            this.cellIssueQty,
            this.cellIssueAmt,
            this.cellEndQty,
            this.cellEndAmt});
            this.detailRow.Name = "detailRow";
            this.detailRow.Weight = 1D;
            // 
            // cellStt
            // 
            this.cellStt.Name = "cellStt";
            this.cellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellStt.Weight = 0.045454545454545456D;
            // 
            // cellName
            // 
            this.cellName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DisplayName]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[CurrencyId]")});
            this.cellName.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellName.Name = "cellName";
            this.cellName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5F, 3F, 0F, 0F, 100F);
            this.cellName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellName.Weight = 0.22727272727272727D;
            // 
            // cellBeginQty
            // 
            this.cellBeginQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BeginQuantity]")});
            this.cellBeginQty.Name = "cellBeginQty";
            this.cellBeginQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellBeginQty.TextFormatString = "{0:#,##0.00;-#,##0.00;\'\'}";
            this.cellBeginQty.Weight = 0.090909090909090912D;
            // 
            // cellBeginAmt
            // 
            this.cellBeginAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BeginAmount]")});
            this.cellBeginAmt.Name = "cellBeginAmt";
            this.cellBeginAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellBeginAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.cellBeginAmt.Weight = 0.090909090909090912D;
            // 
            // cellReceiptQty
            // 
            this.cellReceiptQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ReceiptQuantity]")});
            this.cellReceiptQty.Name = "cellReceiptQty";
            this.cellReceiptQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellReceiptQty.TextFormatString = "{0:#,##0.00;-#,##0.00;\'\'}";
            this.cellReceiptQty.Weight = 0.090909090909090912D;
            // 
            // cellReceiptAmt
            // 
            this.cellReceiptAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ReceiptAmount]")});
            this.cellReceiptAmt.Name = "cellReceiptAmt";
            this.cellReceiptAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellReceiptAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.cellReceiptAmt.Weight = 0.090909090909090912D;
            // 
            // cellIssueQty
            // 
            this.cellIssueQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IssueQuantity]")});
            this.cellIssueQty.Name = "cellIssueQty";
            this.cellIssueQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellIssueQty.TextFormatString = "{0:#,##0.00;-#,##0.00;\'\'}";
            this.cellIssueQty.Weight = 0.090909090909090912D;
            // 
            // cellIssueAmt
            // 
            this.cellIssueAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IssueAmount]")});
            this.cellIssueAmt.Name = "cellIssueAmt";
            this.cellIssueAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellIssueAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.cellIssueAmt.Weight = 0.090909090909090912D;
            // 
            // cellEndQty
            // 
            this.cellEndQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EndQuantity]")});
            this.cellEndQty.Name = "cellEndQty";
            this.cellEndQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellEndQty.TextFormatString = "{0:#,##0.00;-#,##0.00;\'\'}";
            this.cellEndQty.Weight = 0.090909090909090912D;
            // 
            // cellEndAmt
            // 
            this.cellEndAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EndAmount]")});
            this.cellEndAmt.Name = "cellEndAmt";
            this.cellEndAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellEndAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.cellEndAmt.Weight = 0.090909090909090912D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.rfTable,
            this.lblDateSig,
            this.lblSig1,
            this.lblSig2,
            this.lblSig3});
            this.ReportFooter.HeightF = 120F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // rfTable
            // 
            this.rfTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.rfTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.rfTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.rfTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.rfTable.Name = "rfTable";
            this.rfTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3F, 3F, 0F, 0F, 100F);
            this.rfTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rfRow});
            this.rfTable.SizeF = new System.Drawing.SizeF(1100F, 26F);
            // 
            // rfRow
            // 
            this.rfRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.rfBlankStt,
            this.rfLabel,
            this.rfBeginQtyBlank,
            this.rfBeginAmt,
            this.rfReceiptQtyBlank,
            this.rfReceiptAmt,
            this.rfIssueQtyBlank,
            this.rfIssueAmt,
            this.rfEndQtyBlank,
            this.rfEndAmt});
            this.rfRow.Name = "rfRow";
            this.rfRow.Weight = 1D;
            // 
            // rfBlankStt
            // 
            this.rfBlankStt.Name = "rfBlankStt";
            this.rfBlankStt.Weight = 0.045454545454545456D;
            // 
            // rfLabel
            // 
            this.rfLabel.Name = "rfLabel";
            this.rfLabel.Text = "Cộng";
            this.rfLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.rfLabel.Weight = 0.22727272727272727D;
            // 
            // rfBeginQtyBlank
            // 
            this.rfBeginQtyBlank.Name = "rfBeginQtyBlank";
            this.rfBeginQtyBlank.Weight = 0.090909090909090912D;
            // 
            // rfBeginAmt
            // 
            this.rfBeginAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([BeginAmount])")});
            this.rfBeginAmt.Name = "rfBeginAmt";
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.rfBeginAmt.Summary = xrSummary1;
            this.rfBeginAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfBeginAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.rfBeginAmt.Weight = 0.090909090909090912D;
            // 
            // rfReceiptQtyBlank
            // 
            this.rfReceiptQtyBlank.Name = "rfReceiptQtyBlank";
            this.rfReceiptQtyBlank.Weight = 0.090909090909090912D;
            // 
            // rfReceiptAmt
            // 
            this.rfReceiptAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([ReceiptAmount])")});
            this.rfReceiptAmt.Name = "rfReceiptAmt";
            xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.rfReceiptAmt.Summary = xrSummary2;
            this.rfReceiptAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfReceiptAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.rfReceiptAmt.Weight = 0.090909090909090912D;
            // 
            // rfIssueQtyBlank
            // 
            this.rfIssueQtyBlank.Name = "rfIssueQtyBlank";
            this.rfIssueQtyBlank.Weight = 0.090909090909090912D;
            // 
            // rfIssueAmt
            // 
            this.rfIssueAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([IssueAmount])")});
            this.rfIssueAmt.Name = "rfIssueAmt";
            xrSummary3.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.rfIssueAmt.Summary = xrSummary3;
            this.rfIssueAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfIssueAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.rfIssueAmt.Weight = 0.090909090909090912D;
            // 
            // rfEndQtyBlank
            // 
            this.rfEndQtyBlank.Name = "rfEndQtyBlank";
            this.rfEndQtyBlank.Weight = 0.090909090909090912D;
            // 
            // rfEndAmt
            // 
            this.rfEndAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([EndAmount])")});
            this.rfEndAmt.Name = "rfEndAmt";
            xrSummary4.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.rfEndAmt.Summary = xrSummary4;
            this.rfEndAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.rfEndAmt.TextFormatString = "{0:#,##0;-#,##0;\'\'}";
            this.rfEndAmt.Weight = 0.090909090909090912D;
            // 
            // lblDateSig
            // 
            this.lblDateSig.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateSig.LocationFloat = new DevExpress.Utils.PointFloat(733.34F, 35F);
            this.lblDateSig.Name = "lblDateSig";
            this.lblDateSig.SizeF = new System.Drawing.SizeF(366.66F, 20F);
            this.lblDateSig.Text = "Lập biểu ngày      tháng     năm";
            this.lblDateSig.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSig1
            // 
            this.lblSig1.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 58F);
            this.lblSig1.Multiline = true;
            this.lblSig1.Name = "lblSig1";
            this.lblSig1.SizeF = new System.Drawing.SizeF(366.67F, 40F);
            this.lblSig1.Text = "Người lập biểu\r\n(Ký, họ tên)";
            this.lblSig1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSig2
            // 
            this.lblSig2.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig2.LocationFloat = new DevExpress.Utils.PointFloat(366.67F, 58F);
            this.lblSig2.Multiline = true;
            this.lblSig2.Name = "lblSig2";
            this.lblSig2.SizeF = new System.Drawing.SizeF(366.67F, 40F);
            this.lblSig2.Text = "Kế toán trưởng\r\n(Ký, họ tên)";
            this.lblSig2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSig3
            // 
            this.lblSig3.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig3.LocationFloat = new DevExpress.Utils.PointFloat(733.34F, 58F);
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
            // CurrencySummaryReport
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
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 30F);
            this.PageHeightF = 826.7717F;
            this.PageWidthF = 1169.291F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rfTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
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
        private DevExpress.XtraReports.UI.XRTableCell cellH_Begin;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Receipt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Issue;
        private DevExpress.XtraReports.UI.XRTableCell cellH_End;
        private DevExpress.XtraReports.UI.XRTableRow rowH2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt_Spacer;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Name_Spacer;
        private DevExpress.XtraReports.UI.XRTableCell cellH_BeginQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_BeginAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_ReceiptQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_ReceiptAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_IssueQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_IssueAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndAmt;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailRow;
        private DevExpress.XtraReports.UI.XRTableCell cellStt;
        private DevExpress.XtraReports.UI.XRTableCell cellName;
        private DevExpress.XtraReports.UI.XRTableCell cellBeginQty;
        private DevExpress.XtraReports.UI.XRTableCell cellBeginAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellReceiptQty;
        private DevExpress.XtraReports.UI.XRTableCell cellReceiptAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellIssueQty;
        private DevExpress.XtraReports.UI.XRTableCell cellIssueAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellEndQty;
        private DevExpress.XtraReports.UI.XRTableCell cellEndAmt;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable rfTable;
        private DevExpress.XtraReports.UI.XRTableRow rfRow;
        private DevExpress.XtraReports.UI.XRTableCell rfBlankStt;
        private DevExpress.XtraReports.UI.XRTableCell rfLabel;
        private DevExpress.XtraReports.UI.XRTableCell rfBeginQtyBlank;
        private DevExpress.XtraReports.UI.XRTableCell rfBeginAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfReceiptQtyBlank;
        private DevExpress.XtraReports.UI.XRTableCell rfReceiptAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfIssueQtyBlank;
        private DevExpress.XtraReports.UI.XRTableCell rfIssueAmt;
        private DevExpress.XtraReports.UI.XRTableCell rfEndQtyBlank;
        private DevExpress.XtraReports.UI.XRTableCell rfEndAmt;
        private DevExpress.XtraReports.UI.XRLabel lblDateSig;
        private DevExpress.XtraReports.UI.XRLabel lblSig1;
        private DevExpress.XtraReports.UI.XRLabel lblSig2;
        private DevExpress.XtraReports.UI.XRLabel lblSig3;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
    }
}
