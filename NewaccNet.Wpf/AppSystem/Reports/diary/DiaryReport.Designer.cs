namespace NewaccNet.Wpf.AppSystem.Reports.diary
{
    partial class DiaryReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraReports.UI.XRSummary xrSummaryDebit = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryCredit = new DevExpress.XtraReports.UI.XRSummary();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.label1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPeriodTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.tableHeaderRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.thCellVoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.thCellVoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.thCellContents = new DevExpress.XtraReports.UI.XRTableCell();
            this.thCellAccountId = new DevExpress.XtraReports.UI.XRTableCell();
            this.thCellDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.thCellCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tableVoucherHeader = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowVoucherHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.vhCellVoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.vhCellVoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.vhCellContents = new DevExpress.XtraReports.UI.XRTableCell();
            this.vhCellAccountId = new DevExpress.XtraReports.UI.XRTableCell();
            this.vhCellDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.vhCellCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.dCellVoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.dCellVoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.dCellAccountName = new DevExpress.XtraReports.UI.XRTableCell();
            this.dCellAccountId = new DevExpress.XtraReports.UI.XRTableCell();
            this.dCellDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.dCellCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.tableFooter = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowFooter = new DevExpress.XtraReports.UI.XRTableRow();
            this.tfCellTotalLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellDebitTotal = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellCreditTotal = new DevExpress.XtraReports.UI.XRTableCell();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.pCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.pPeriodTitle = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableVoucherHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.DiaryReportDTO);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 25F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1,
            this.pageInfo2});
            this.BottomMargin.HeightF = 30F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo1
            // 
            this.pageInfo1.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F);
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 5F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(393.3858F, 20F);
            this.pageInfo1.StylePriority.UseFont = false;
            this.pageInfo1.TextFormatString = "{0:dd/MM/yyyy HH:mm}";
            // 
            // pageInfo2
            // 
            this.pageInfo2.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F);
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(393.3858F, 5F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(393.3859F, 20F);
            this.pageInfo2.StylePriority.UseFont = false;
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.pageInfo2.TextFormatString = "Trang {0} / {1}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblCompanyName,
            this.label1,
            this.lblPeriodTitle});
            this.ReportHeader.HeightF = 85F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?pCompanyName")});
            this.lblCompanyName.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompanyName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompanyName.Multiline = true;
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblCompanyName.SizeF = new System.Drawing.SizeF(500F, 22F);
            this.lblCompanyName.StylePriority.UseFont = false;
            this.lblCompanyName.StylePriority.UseTextAlignment = false;
            this.lblCompanyName.Text = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
            this.lblCompanyName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new DevExpress.Drawing.DXFont("Arial", 14.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.label1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 26F);
            this.label1.Name = "label1";
            this.label1.SizeF = new System.Drawing.SizeF(786.7717F, 26F);
            this.label1.StylePriority.UseFont = false;
            this.label1.StylePriority.UseTextAlignment = false;
            this.label1.Text = "NHẬT KÝ CHUNG";
            this.label1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblPeriodTitle
            // 
            this.lblPeriodTitle.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?pPeriodTitle")});
            this.lblPeriodTitle.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblPeriodTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 54F);
            this.lblPeriodTitle.Multiline = true;
            this.lblPeriodTitle.Name = "lblPeriodTitle";
            this.lblPeriodTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblPeriodTitle.SizeF = new System.Drawing.SizeF(786.7717F, 22F);
            this.lblPeriodTitle.StylePriority.UseFont = false;
            this.lblPeriodTitle.StylePriority.UseTextAlignment = false;
            this.lblPeriodTitle.Text = "Từ ngày: 01/01/2026 Đến ngày: 31/12/2026";
            this.lblPeriodTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // GroupHeader2
            // 
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableHeader});
            this.GroupHeader2.HeightF = 28F;
            this.GroupHeader2.Level = 1;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.RepeatEveryPage = true;
            // 
            // tableHeader
            // 
            this.tableHeader.BorderColor = System.Drawing.Color.LightGray;
            this.tableHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableHeader.BorderWidth = 1F;
            this.tableHeader.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableHeader.Name = "tableHeader";
            this.tableHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableHeaderRow});
            this.tableHeader.SizeF = new System.Drawing.SizeF(786.7717F, 28F);
            this.tableHeader.StylePriority.UseBorderColor = false;
            this.tableHeader.StylePriority.UseBorders = false;
            this.tableHeader.StylePriority.UseBorderWidth = false;
            this.tableHeader.StylePriority.UseFont = false;
            this.tableHeader.StylePriority.UseTextAlignment = false;
            this.tableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // tableHeaderRow
            // 
            this.tableHeaderRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.thCellVoucherNo,
            this.thCellVoucherDate,
            this.thCellContents,
            this.thCellAccountId,
            this.thCellDebit,
            this.thCellCredit});
            this.tableHeaderRow.Name = "tableHeaderRow";
            this.tableHeaderRow.Weight = 1D;
            // 
            // thCellVoucherNo
            // 
            this.thCellVoucherNo.Name = "thCellVoucherNo";
            this.thCellVoucherNo.Text = "Số CT";
            this.thCellVoucherNo.Weight = 60D;
            // 
            // thCellVoucherDate
            // 
            this.thCellVoucherDate.Name = "thCellVoucherDate";
            this.thCellVoucherDate.Text = "Ngày CT";
            this.thCellVoucherDate.Weight = 75D;
            // 
            // thCellContents
            // 
            this.thCellContents.Name = "thCellContents";
            this.thCellContents.Text = "Nội dung";
            this.thCellContents.Weight = 311.7717D;
            // 
            // thCellAccountId
            // 
            this.thCellAccountId.Name = "thCellAccountId";
            this.thCellAccountId.Text = "Tài khoản";
            this.thCellAccountId.Weight = 100D;
            // 
            // thCellDebit
            // 
            this.thCellDebit.Name = "thCellDebit";
            this.thCellDebit.Text = "Số tiền nợ";
            this.thCellDebit.Weight = 120D;
            // 
            // thCellCredit
            // 
            this.thCellCredit.Name = "thCellCredit";
            this.thCellCredit.Text = "Số tiền có";
            this.thCellCredit.Weight = 120D;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableVoucherHeader});
            this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("JournalVoucherId", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader1.HeightF = 22F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // tableVoucherHeader
            // 
            this.tableVoucherHeader.BorderColor = System.Drawing.Color.LightGray;
            this.tableVoucherHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right)));
            this.tableVoucherHeader.BorderWidth = 1F;
            this.tableVoucherHeader.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F);
            this.tableVoucherHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableVoucherHeader.Name = "tableVoucherHeader";
            this.tableVoucherHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableVoucherHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRowVoucherHeader});
            this.tableVoucherHeader.SizeF = new System.Drawing.SizeF(786.7717F, 22F);
            this.tableVoucherHeader.StylePriority.UseBorderColor = false;
            this.tableVoucherHeader.StylePriority.UseBorders = false;
            this.tableVoucherHeader.StylePriority.UseBorderWidth = false;
            this.tableVoucherHeader.StylePriority.UseFont = false;
            // 
            // tableRowVoucherHeader
            // 
            this.tableRowVoucherHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.vhCellVoucherNo,
            this.vhCellVoucherDate,
            this.vhCellContents,
            this.vhCellAccountId,
            this.vhCellDebit,
            this.vhCellCredit});
            this.tableRowVoucherHeader.Name = "tableRowVoucherHeader";
            this.tableRowVoucherHeader.Weight = 1D;
            // 
            // vhCellVoucherNo
            // 
            this.vhCellVoucherNo.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherNo]")});
            this.vhCellVoucherNo.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.vhCellVoucherNo.Multiline = true;
            this.vhCellVoucherNo.Name = "vhCellVoucherNo";
            this.vhCellVoucherNo.StylePriority.UseFont = false;
            this.vhCellVoucherNo.StylePriority.UseTextAlignment = false;
            this.vhCellVoucherNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.vhCellVoucherNo.Weight = 60D;
            // 
            // vhCellVoucherDate
            // 
            this.vhCellVoucherDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherDate]")});
            this.vhCellVoucherDate.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.vhCellVoucherDate.Multiline = true;
            this.vhCellVoucherDate.Name = "vhCellVoucherDate";
            this.vhCellVoucherDate.StylePriority.UseFont = false;
            this.vhCellVoucherDate.StylePriority.UseTextAlignment = false;
            this.vhCellVoucherDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.vhCellVoucherDate.TextFormatString = "{0:dd/MM/yyyy}";
            this.vhCellVoucherDate.Weight = 75D;
            // 
            // vhCellContents
            // 
            this.vhCellContents.CanShrink = true;
            this.vhCellContents.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Contents]")});
            this.vhCellContents.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, ((DevExpress.Drawing.DXFontStyle)((DevExpress.Drawing.DXFontStyle.Bold | DevExpress.Drawing.DXFontStyle.Italic))));
            this.vhCellContents.Multiline = true;
            this.vhCellContents.Name = "vhCellContents";
            this.vhCellContents.Padding = new DevExpress.XtraPrinting.PaddingInfo(5F, 2F, 0F, 0F, 100F);
            this.vhCellContents.StylePriority.UseFont = false;
            this.vhCellContents.StylePriority.UsePadding = false;
            this.vhCellContents.StylePriority.UseTextAlignment = false;
            this.vhCellContents.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.vhCellContents.Weight = 311.7717D;
            // 
            // vhCellAccountId
            // 
            this.vhCellAccountId.Name = "vhCellAccountId";
            this.vhCellAccountId.Weight = 100D;
            // 
            // vhCellDebit
            // 
            this.vhCellDebit.Name = "vhCellDebit";
            this.vhCellDebit.Weight = 120D;
            // 
            // vhCellCredit
            // 
            this.vhCellCredit.Name = "vhCellCredit";
            this.vhCellCredit.Weight = 120D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableDetail});
            this.Detail.HeightF = 20F;
            this.Detail.Name = "Detail";
            // 
            // tableDetail
            // 
            this.tableDetail.BorderColor = System.Drawing.Color.LightGray;
            this.tableDetail.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)));
            this.tableDetail.BorderWidth = 1F;
            this.tableDetail.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F);
            this.tableDetail.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableDetail.Name = "tableDetail";
            this.tableDetail.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableDetail.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRowDetail});
            this.tableDetail.SizeF = new System.Drawing.SizeF(786.7717F, 20F);
            this.tableDetail.StylePriority.UseBorderColor = false;
            this.tableDetail.StylePriority.UseBorders = false;
            this.tableDetail.StylePriority.UseBorderWidth = false;
            this.tableDetail.StylePriority.UseFont = false;
            // 
            // tableRowDetail
            // 
            this.tableRowDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.dCellVoucherNo,
            this.dCellVoucherDate,
            this.dCellAccountName,
            this.dCellAccountId,
            this.dCellDebit,
            this.dCellCredit});
            this.tableRowDetail.Name = "tableRowDetail";
            this.tableRowDetail.Weight = 1D;
            // 
            // dCellVoucherNo
            // 
            this.dCellVoucherNo.Name = "dCellVoucherNo";
            this.dCellVoucherNo.Weight = 60D;
            // 
            // dCellVoucherDate
            // 
            this.dCellVoucherDate.Name = "dCellVoucherDate";
            this.dCellVoucherDate.Weight = 75D;
            // 
            // dCellAccountName
            // 
            this.dCellAccountName.CanShrink = true;
            this.dCellAccountName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AccountName]")});
            this.dCellAccountName.Multiline = true;
            this.dCellAccountName.Name = "dCellAccountName";
            this.dCellAccountName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5F, 2F, 0F, 0F, 100F);
            this.dCellAccountName.StylePriority.UsePadding = false;
            this.dCellAccountName.StylePriority.UseTextAlignment = false;
            this.dCellAccountName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.dCellAccountName.Weight = 311.7717D;
            // 
            // dCellAccountId
            // 
            this.dCellAccountId.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AccountId]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "TextAlignment", "Iif([Dbcr] <  0, \'MiddleRight\', \'MiddleLeft\')")});
            this.dCellAccountId.Multiline = true;
            this.dCellAccountId.Name = "dCellAccountId";
            this.dCellAccountId.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 4F, 0F, 0F, 100F);
            this.dCellAccountId.StylePriority.UsePadding = false;
            this.dCellAccountId.StylePriority.UseTextAlignment = false;
            this.dCellAccountId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.dCellAccountId.Weight = 100D;
            // 
            // dCellDebit
            // 
            this.dCellDebit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([Dbcr]==1,[Amount], 0 )")});
            this.dCellDebit.Multiline = true;
            this.dCellDebit.Name = "dCellDebit";
            this.dCellDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.dCellDebit.StylePriority.UsePadding = false;
            this.dCellDebit.StylePriority.UseTextAlignment = false;
            this.dCellDebit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.dCellDebit.TextFormatString = "{0:#,#}";
            this.dCellDebit.Weight = 120D;
            // 
            // dCellCredit
            // 
            this.dCellCredit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([Dbcr]==-1,[Amount], 0 )\n")});
            this.dCellCredit.Multiline = true;
            this.dCellCredit.Name = "dCellCredit";
            this.dCellCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.dCellCredit.StylePriority.UsePadding = false;
            this.dCellCredit.StylePriority.UseTextAlignment = false;
            this.dCellCredit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.dCellCredit.TextFormatString = "{0:#,#}";
            this.dCellCredit.Weight = 120D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableFooter,
            this.lblDate,
            this.lblSign1Title,
            this.lblSign1Sub,
            this.lblSign2Title,
            this.lblSign2Sub,
            this.lblSign3Title,
            this.lblSign3Sub});
            this.ReportFooter.HeightF = 140F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // tableFooter
            // 
            this.tableFooter.BorderColor = System.Drawing.Color.LightGray;
            this.tableFooter.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableFooter.BorderWidth = 1F;
            this.tableFooter.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableFooter.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableFooter.Name = "tableFooter";
            this.tableFooter.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableFooter.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRowFooter});
            this.tableFooter.SizeF = new System.Drawing.SizeF(786.7717F, 25F);
            this.tableFooter.StylePriority.UseBorderColor = false;
            this.tableFooter.StylePriority.UseBorders = false;
            this.tableFooter.StylePriority.UseBorderWidth = false;
            this.tableFooter.StylePriority.UseFont = false;
            // 
            // tableRowFooter
            // 
            this.tableRowFooter.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tfCellTotalLabel,
            this.tfCellDebitTotal,
            this.tfCellCreditTotal});
            this.tableRowFooter.Name = "tableRowFooter";
            this.tableRowFooter.Weight = 1D;
            // 
            // tfCellTotalLabel
            // 
            this.tfCellTotalLabel.Name = "tfCellTotalLabel";
            this.tfCellTotalLabel.StylePriority.UseTextAlignment = false;
            this.tfCellTotalLabel.Text = "Tổng cộng";
            this.tfCellTotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tfCellTotalLabel.Weight = 546.7717D;
            // 
            // tfCellDebitTotal
            // 
            this.tfCellDebitTotal.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum(Iif([Dbcr]==1,[Amount], 0 ))")});
            this.tfCellDebitTotal.Multiline = true;
            this.tfCellDebitTotal.Name = "tfCellDebitTotal";
            this.tfCellDebitTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellDebitTotal.StylePriority.UsePadding = false;
            this.tfCellDebitTotal.StylePriority.UseTextAlignment = false;
            xrSummaryDebit.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.tfCellDebitTotal.Summary = xrSummaryDebit;
            this.tfCellDebitTotal.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellDebitTotal.TextFormatString = "{0:#,#}";
            this.tfCellDebitTotal.Weight = 120D;
            // 
            // tfCellCreditTotal
            // 
            this.tfCellCreditTotal.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum(Iif([Dbcr]==-1,[Amount], 0 ))")});
            this.tfCellCreditTotal.Multiline = true;
            this.tfCellCreditTotal.Name = "tfCellCreditTotal";
            this.tfCellCreditTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellCreditTotal.StylePriority.UsePadding = false;
            this.tfCellCreditTotal.StylePriority.UseTextAlignment = false;
            xrSummaryCredit.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.tfCellCreditTotal.Summary = xrSummaryCredit;
            this.tfCellCreditTotal.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellCreditTotal.TextFormatString = "{0:#,#}";
            this.tfCellCreditTotal.Weight = 120D;
            // 
            // lblDate
            // 
            this.lblDate.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(520F, 35F);
            this.lblDate.Multiline = true;
            this.lblDate.Name = "lblDate";
            this.lblDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDate.SizeF = new System.Drawing.SizeF(266.7717F, 20F);
            this.lblDate.StylePriority.UseFont = false;
            this.lblDate.StylePriority.UseTextAlignment = false;
            this.lblDate.Text = "Ngày ..... tháng ..... năm .....";
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign1Title
            // 
            this.lblSign1Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign1Title.LocationFloat = new DevExpress.Utils.PointFloat(0F, 60F);
            this.lblSign1Title.Name = "lblSign1Title";
            this.lblSign1Title.SizeF = new System.Drawing.SizeF(240F, 20F);
            this.lblSign1Title.StylePriority.UseFont = false;
            this.lblSign1Title.StylePriority.UseTextAlignment = false;
            this.lblSign1Title.Text = "Người lập biểu";
            this.lblSign1Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign1Sub
            // 
            this.lblSign1Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign1Sub.LocationFloat = new DevExpress.Utils.PointFloat(0F, 80F);
            this.lblSign1Sub.Name = "lblSign1Sub";
            this.lblSign1Sub.SizeF = new System.Drawing.SizeF(240F, 18F);
            this.lblSign1Sub.StylePriority.UseFont = false;
            this.lblSign1Sub.StylePriority.UseTextAlignment = false;
            this.lblSign1Sub.Text = "(Ký, họ tên)";
            this.lblSign1Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign2Title
            // 
            this.lblSign2Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign2Title.LocationFloat = new DevExpress.Utils.PointFloat(273F, 60F);
            this.lblSign2Title.Name = "lblSign2Title";
            this.lblSign2Title.SizeF = new System.Drawing.SizeF(240F, 20F);
            this.lblSign2Title.StylePriority.UseFont = false;
            this.lblSign2Title.StylePriority.UseTextAlignment = false;
            this.lblSign2Title.Text = "Kế toán trưởng";
            this.lblSign2Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign2Sub
            // 
            this.lblSign2Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign2Sub.LocationFloat = new DevExpress.Utils.PointFloat(273F, 80F);
            this.lblSign2Sub.Name = "lblSign2Sub";
            this.lblSign2Sub.SizeF = new System.Drawing.SizeF(240F, 18F);
            this.lblSign2Sub.StylePriority.UseFont = false;
            this.lblSign2Sub.StylePriority.UseTextAlignment = false;
            this.lblSign2Sub.Text = "(Ký, họ tên)";
            this.lblSign2Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Title
            // 
            this.lblSign3Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign3Title.LocationFloat = new DevExpress.Utils.PointFloat(546F, 60F);
            this.lblSign3Title.Name = "lblSign3Title";
            this.lblSign3Title.SizeF = new System.Drawing.SizeF(240F, 20F);
            this.lblSign3Title.StylePriority.UseFont = false;
            this.lblSign3Title.StylePriority.UseTextAlignment = false;
            this.lblSign3Title.Text = "Giám đốc";
            this.lblSign3Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Sub
            // 
            this.lblSign3Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign3Sub.LocationFloat = new DevExpress.Utils.PointFloat(546F, 80F);
            this.lblSign3Sub.Name = "lblSign3Sub";
            this.lblSign3Sub.SizeF = new System.Drawing.SizeF(240F, 18F);
            this.lblSign3Sub.StylePriority.UseFont = false;
            this.lblSign3Sub.StylePriority.UseTextAlignment = false;
            this.lblSign3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSign3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // pCompanyName
            // 
            this.pCompanyName.Description = "Tên công ty";
            this.pCompanyName.Name = "pCompanyName";
            this.pCompanyName.ValueInfo = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
            this.pCompanyName.Visible = false;
            // 
            // pPeriodTitle
            // 
            this.pPeriodTitle.Description = "Kỳ báo cáo";
            this.pPeriodTitle.Name = "pPeriodTitle";
            this.pPeriodTitle.ValueInfo = "Từ ngày: 01/01/2026 Đến ngày: 31/12/2026";
            this.pPeriodTitle.Visible = false;
            // 
            // DiaryReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.GroupHeader2,
            this.GroupHeader1,
            this.Detail,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Margins = new DevExpress.Drawing.DXMargins(20F, 20F, 25F, 30F);
            this.PageHeightF = 1169.291F;
            this.PageWidthF = 826.7717F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.pCompanyName,
            this.pPeriodTitle});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableVoucherHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel lblCompanyName;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.XRLabel lblPeriodTitle;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader2;
        private DevExpress.XtraReports.UI.XRTable tableHeader;
        private DevExpress.XtraReports.UI.XRTableRow tableHeaderRow;
        private DevExpress.XtraReports.UI.XRTableCell thCellVoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell thCellVoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell thCellContents;
        private DevExpress.XtraReports.UI.XRTableCell thCellAccountId;
        private DevExpress.XtraReports.UI.XRTableCell thCellDebit;
        private DevExpress.XtraReports.UI.XRTableCell thCellCredit;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable tableVoucherHeader;
        private DevExpress.XtraReports.UI.XRTableRow tableRowVoucherHeader;
        private DevExpress.XtraReports.UI.XRTableCell vhCellVoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell vhCellVoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell vhCellContents;
        private DevExpress.XtraReports.UI.XRTableCell vhCellAccountId;
        private DevExpress.XtraReports.UI.XRTableCell vhCellDebit;
        private DevExpress.XtraReports.UI.XRTableCell vhCellCredit;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow tableRowDetail;
        private DevExpress.XtraReports.UI.XRTableCell dCellVoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell dCellVoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell dCellAccountName;
        private DevExpress.XtraReports.UI.XRTableCell dCellAccountId;
        private DevExpress.XtraReports.UI.XRTableCell dCellDebit;
        private DevExpress.XtraReports.UI.XRTableCell dCellCredit;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable tableFooter;
        private DevExpress.XtraReports.UI.XRTableRow tableRowFooter;
        private DevExpress.XtraReports.UI.XRTableCell tfCellTotalLabel;
        private DevExpress.XtraReports.UI.XRTableCell tfCellDebitTotal;
        private DevExpress.XtraReports.UI.XRTableCell tfCellCreditTotal;
        private DevExpress.XtraReports.UI.XRLabel lblDate;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Sub;
        private DevExpress.XtraReports.Parameters.Parameter pCompanyName;
        private DevExpress.XtraReports.Parameters.Parameter pPeriodTitle;
    }
}

