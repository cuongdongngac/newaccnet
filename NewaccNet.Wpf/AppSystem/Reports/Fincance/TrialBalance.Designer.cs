namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    partial class TrialBalance
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
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDateRange = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.lblHeaderAccountId = new DevExpress.XtraReports.UI.XRLabel();
            this.lblHeaderAccountName = new DevExpress.XtraReports.UI.XRLabel();
            this.tableHeaderTop = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeaderTop = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHeaderTopBegin = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderTopInt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderTopEnd = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableHeaderBottom = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeaderBottom = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHeaderBeginDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderBeginCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderIntDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderIntCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderEndDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderEndCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.table2 = new DevExpress.XtraReports.UI.XRTable();
            this.tableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tableCell11 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell12 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell16 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell17 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell18 = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.tableFooter = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowFooter = new DevExpress.XtraReports.UI.XRTableRow();
            this.tfCellTotalLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellBeginDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellBeginCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellIntDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellIntCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellEndDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.tfCellEndCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.lblSignDate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmFromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmToDate = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeaderTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeaderBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.TrialBalanceDTO);
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
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo1
            // 
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 5F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(564.6455F, 20F);
            this.pageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.pageInfo1.TextFormatString = "{0:dd/MM/yyyy HH:mm}";
            // 
            // pageInfo2
            // 
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(564.6455F, 5F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(564.6455F, 20F);
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageInfo2.TextFormatString = "Trang {0}/{1}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblCompanyName,
            this.lblTitle,
            this.lblDateRange});
            this.ReportHeader.HeightF = 75F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmCompanyName")});
            this.lblCompanyName.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompanyName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompanyName.Multiline = true;
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblCompanyName.SizeF = new System.Drawing.SizeF(450F, 20F);
            this.lblCompanyName.StylePriority.UseFont = false;
            this.lblCompanyName.StylePriority.UseTextAlignment = false;
            this.lblCompanyName.Text = "Công ty Bình Minh";
            this.lblCompanyName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Arial", 14F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 24F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(1129.291F, 25F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "BẢNG CÂN ĐỐI TÀI KHOẢN";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDateRange
            // 
            this.lblDateRange.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Từ ngày {0:d/M/yyyy} Đến ngày {1:d/M/yyyy}\', ?prmFromDate, ?prmToDate)" +
                    "")});
            this.lblDateRange.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateRange.LocationFloat = new DevExpress.Utils.PointFloat(0F, 50F);
            this.lblDateRange.Multiline = true;
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDateRange.SizeF = new System.Drawing.SizeF(1129.291F, 20F);
            this.lblDateRange.StylePriority.UseFont = false;
            this.lblDateRange.StylePriority.UseTextAlignment = false;
            this.lblDateRange.Text = "Từ ngày 01/01/2026 Đến ngày 31/12/2026";
            this.lblDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblHeaderAccountId,
            this.lblHeaderAccountName,
            this.tableHeaderTop,
            this.tableHeaderBottom});
            this.PageHeader.HeightF = 40F;
            this.PageHeader.Name = "PageHeader";
            // 
            // lblHeaderAccountId
            // 
            this.lblHeaderAccountId.BorderColor = System.Drawing.Color.LightGray;
            this.lblHeaderAccountId.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.lblHeaderAccountId.BorderWidth = 1F;
            this.lblHeaderAccountId.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblHeaderAccountId.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblHeaderAccountId.Name = "lblHeaderAccountId";
            this.lblHeaderAccountId.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblHeaderAccountId.SizeF = new System.Drawing.SizeF(85F, 40F);
            this.lblHeaderAccountId.StylePriority.UseBorderColor = false;
            this.lblHeaderAccountId.StylePriority.UseBorders = false;
            this.lblHeaderAccountId.StylePriority.UseBorderWidth = false;
            this.lblHeaderAccountId.StylePriority.UseFont = false;
            this.lblHeaderAccountId.StylePriority.UseTextAlignment = false;
            this.lblHeaderAccountId.Text = "Mã tài khoản";
            this.lblHeaderAccountId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblHeaderAccountName
            // 
            this.lblHeaderAccountName.BorderColor = System.Drawing.Color.LightGray;
            this.lblHeaderAccountName.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.lblHeaderAccountName.BorderWidth = 1F;
            this.lblHeaderAccountName.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblHeaderAccountName.LocationFloat = new DevExpress.Utils.PointFloat(85F, 0F);
            this.lblHeaderAccountName.Name = "lblHeaderAccountName";
            this.lblHeaderAccountName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblHeaderAccountName.SizeF = new System.Drawing.SizeF(324.291F, 40F);
            this.lblHeaderAccountName.StylePriority.UseBorderColor = false;
            this.lblHeaderAccountName.StylePriority.UseBorders = false;
            this.lblHeaderAccountName.StylePriority.UseBorderWidth = false;
            this.lblHeaderAccountName.StylePriority.UseFont = false;
            this.lblHeaderAccountName.StylePriority.UseTextAlignment = false;
            this.lblHeaderAccountName.Text = "Tên tài khoản";
            this.lblHeaderAccountName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // tableHeaderTop
            // 
            this.tableHeaderTop.BorderColor = System.Drawing.Color.LightGray;
            this.tableHeaderTop.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableHeaderTop.BorderWidth = 1F;
            this.tableHeaderTop.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableHeaderTop.LocationFloat = new DevExpress.Utils.PointFloat(409.291F, 0F);
            this.tableHeaderTop.Name = "tableHeaderTop";
            this.tableHeaderTop.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableHeaderTop.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowHeaderTop});
            this.tableHeaderTop.SizeF = new System.Drawing.SizeF(720F, 20F);
            this.tableHeaderTop.StylePriority.UseBorderColor = false;
            this.tableHeaderTop.StylePriority.UseBorders = false;
            this.tableHeaderTop.StylePriority.UseBorderWidth = false;
            this.tableHeaderTop.StylePriority.UseFont = false;
            this.tableHeaderTop.StylePriority.UseTextAlignment = false;
            this.tableHeaderTop.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // rowHeaderTop
            // 
            this.rowHeaderTop.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellHeaderTopBegin,
            this.cellHeaderTopInt,
            this.cellHeaderTopEnd});
            this.rowHeaderTop.Name = "rowHeaderTop";
            this.rowHeaderTop.Weight = 1D;
            // 
            // cellHeaderTopBegin
            // 
            this.cellHeaderTopBegin.Name = "cellHeaderTopBegin";
            this.cellHeaderTopBegin.Text = "Dư đầu kỳ";
            this.cellHeaderTopBegin.Weight = 240D;
            // 
            // cellHeaderTopInt
            // 
            this.cellHeaderTopInt.Name = "cellHeaderTopInt";
            this.cellHeaderTopInt.Text = "Phát sinh trong kỳ";
            this.cellHeaderTopInt.Weight = 240D;
            // 
            // cellHeaderTopEnd
            // 
            this.cellHeaderTopEnd.Name = "cellHeaderTopEnd";
            this.cellHeaderTopEnd.Text = "Dư cuối kỳ";
            this.cellHeaderTopEnd.Weight = 240D;
            // 
            // tableHeaderBottom
            // 
            this.tableHeaderBottom.BorderColor = System.Drawing.Color.LightGray;
            this.tableHeaderBottom.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableHeaderBottom.BorderWidth = 1F;
            this.tableHeaderBottom.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableHeaderBottom.LocationFloat = new DevExpress.Utils.PointFloat(409.291F, 20F);
            this.tableHeaderBottom.Name = "tableHeaderBottom";
            this.tableHeaderBottom.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableHeaderBottom.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowHeaderBottom});
            this.tableHeaderBottom.SizeF = new System.Drawing.SizeF(720F, 20F);
            this.tableHeaderBottom.StylePriority.UseBorderColor = false;
            this.tableHeaderBottom.StylePriority.UseBorders = false;
            this.tableHeaderBottom.StylePriority.UseBorderWidth = false;
            this.tableHeaderBottom.StylePriority.UseFont = false;
            this.tableHeaderBottom.StylePriority.UseTextAlignment = false;
            this.tableHeaderBottom.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // rowHeaderBottom
            // 
            this.rowHeaderBottom.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellHeaderBeginDebit,
            this.cellHeaderBeginCredit,
            this.cellHeaderIntDebit,
            this.cellHeaderIntCredit,
            this.cellHeaderEndDebit,
            this.cellHeaderEndCredit});
            this.rowHeaderBottom.Name = "rowHeaderBottom";
            this.rowHeaderBottom.Weight = 1D;
            // 
            // cellHeaderBeginDebit
            // 
            this.cellHeaderBeginDebit.Name = "cellHeaderBeginDebit";
            this.cellHeaderBeginDebit.Text = "Nợ";
            this.cellHeaderBeginDebit.Weight = 120D;
            // 
            // cellHeaderBeginCredit
            // 
            this.cellHeaderBeginCredit.Name = "cellHeaderBeginCredit";
            this.cellHeaderBeginCredit.Text = "Có";
            this.cellHeaderBeginCredit.Weight = 120D;
            // 
            // cellHeaderIntDebit
            // 
            this.cellHeaderIntDebit.Name = "cellHeaderIntDebit";
            this.cellHeaderIntDebit.Text = "Nợ";
            this.cellHeaderIntDebit.Weight = 120D;
            // 
            // cellHeaderIntCredit
            // 
            this.cellHeaderIntCredit.Name = "cellHeaderIntCredit";
            this.cellHeaderIntCredit.Text = "Có";
            this.cellHeaderIntCredit.Weight = 120D;
            // 
            // cellHeaderEndDebit
            // 
            this.cellHeaderEndDebit.Name = "cellHeaderEndDebit";
            this.cellHeaderEndDebit.Text = "Nợ";
            this.cellHeaderEndDebit.Weight = 120D;
            // 
            // cellHeaderEndCredit
            // 
            this.cellHeaderEndCredit.Name = "cellHeaderEndCredit";
            this.cellHeaderEndCredit.Text = "Có";
            this.cellHeaderEndCredit.Weight = 120D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.table2});
            this.Detail.HeightF = 22F;
            this.Detail.Name = "Detail";
            // 
            // table2
            // 
            this.table2.BorderColor = System.Drawing.Color.LightGray;
            this.table2.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.table2.BorderWidth = 1F;
            this.table2.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F);
            this.table2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.table2.Name = "table2";
            this.table2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.table2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRow2});
            this.table2.SizeF = new System.Drawing.SizeF(1129.291F, 22F);
            this.table2.StylePriority.UseBorderColor = false;
            this.table2.StylePriority.UseBorders = false;
            this.table2.StylePriority.UseBorderWidth = false;
            this.table2.StylePriority.UseFont = false;
            // 
            // tableRow2
            // 
            this.tableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tableCell11,
            this.tableCell12,
            this.tableCell13,
            this.tableCell14,
            this.tableCell15,
            this.tableCell16,
            this.tableCell17,
            this.tableCell18});
            this.tableRow2.Name = "tableRow2";
            this.tableRow2.Weight = 1D;
            // 
            // tableCell11
            // 
            this.tableCell11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AccountId]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell11.Multiline = true;
            this.tableCell11.Name = "tableCell11";
            this.tableCell11.StylePriority.UseTextAlignment = false;
            this.tableCell11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tableCell11.Weight = 85D;
            // 
            // tableCell12
            // 
            this.tableCell12.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AccountName]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell12.Multiline = true;
            this.tableCell12.Name = "tableCell12";
            this.tableCell12.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 2F, 0F, 0F, 100F);
            this.tableCell12.StylePriority.UsePadding = false;
            this.tableCell12.StylePriority.UseTextAlignment = false;
            this.tableCell12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.tableCell12.Weight = 324.291D;
            // 
            // tableCell13
            // 
            this.tableCell13.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BeginDebit]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell13.Multiline = true;
            this.tableCell13.Name = "tableCell13";
            this.tableCell13.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tableCell13.StylePriority.UsePadding = false;
            this.tableCell13.StylePriority.UseTextAlignment = false;
            this.tableCell13.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell13.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.tableCell13.Weight = 120D;
            // 
            // tableCell14
            // 
            this.tableCell14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BeginCredit]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell14.Multiline = true;
            this.tableCell14.Name = "tableCell14";
            this.tableCell14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tableCell14.StylePriority.UsePadding = false;
            this.tableCell14.StylePriority.UseTextAlignment = false;
            this.tableCell14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell14.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.tableCell14.Weight = 120D;
            // 
            // tableCell15
            // 
            this.tableCell15.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IntDebit]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell15.Multiline = true;
            this.tableCell15.Name = "tableCell15";
            this.tableCell15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tableCell15.StylePriority.UsePadding = false;
            this.tableCell15.StylePriority.UseTextAlignment = false;
            this.tableCell15.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell15.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.tableCell15.Weight = 120D;
            // 
            // tableCell16
            // 
            this.tableCell16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[IntCredit]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell16.Multiline = true;
            this.tableCell16.Name = "tableCell16";
            this.tableCell16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tableCell16.StylePriority.UsePadding = false;
            this.tableCell16.StylePriority.UseTextAlignment = false;
            this.tableCell16.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell16.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.tableCell16.Weight = 120D;
            // 
            // tableCell17
            // 
            this.tableCell17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EndDebit]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell17.Multiline = true;
            this.tableCell17.Name = "tableCell17";
            this.tableCell17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tableCell17.StylePriority.UsePadding = false;
            this.tableCell17.StylePriority.UseTextAlignment = false;
            this.tableCell17.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell17.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.tableCell17.Weight = 120D;
            // 
            // tableCell18
            // 
            this.tableCell18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EndCredit]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[AccountId]")});
            this.tableCell18.Multiline = true;
            this.tableCell18.Name = "tableCell18";
            this.tableCell18.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tableCell18.StylePriority.UsePadding = false;
            this.tableCell18.StylePriority.UseTextAlignment = false;
            this.tableCell18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell18.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.tableCell18.Weight = 120D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableFooter,
            this.lblSignDate,
            this.lblSign1Title,
            this.lblSign1Sub,
            this.lblSign2Title,
            this.lblSign2Sub,
            this.lblSign3Title,
            this.lblSign3Sub});
            this.ReportFooter.HeightF = 150F;
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
            this.tableFooter.ForeColor = System.Drawing.Color.Blue;
            this.tableFooter.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableFooter.Name = "tableFooter";
            this.tableFooter.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableFooter.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRowFooter});
            this.tableFooter.SizeF = new System.Drawing.SizeF(1129.291F, 25F);
            this.tableFooter.StylePriority.UseBorderColor = false;
            this.tableFooter.StylePriority.UseBorders = false;
            this.tableFooter.StylePriority.UseBorderWidth = false;
            this.tableFooter.StylePriority.UseFont = false;
            this.tableFooter.StylePriority.UseForeColor = false;
            // 
            // tableRowFooter
            // 
            this.tableRowFooter.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tfCellTotalLabel,
            this.tfCellBeginDebit,
            this.tfCellBeginCredit,
            this.tfCellIntDebit,
            this.tfCellIntCredit,
            this.tfCellEndDebit,
            this.tfCellEndCredit});
            this.tableRowFooter.Name = "tableRowFooter";
            this.tableRowFooter.Weight = 1D;
            // 
            // tfCellTotalLabel
            // 
            this.tfCellTotalLabel.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tfCellTotalLabel.Multiline = true;
            this.tfCellTotalLabel.Name = "tfCellTotalLabel";
            this.tfCellTotalLabel.StylePriority.UseFont = false;
            this.tfCellTotalLabel.StylePriority.UseTextAlignment = false;
            this.tfCellTotalLabel.Text = "Cộng:";
            this.tfCellTotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tfCellTotalLabel.Weight = 409.291D;
            // 
            // tfCellBeginDebit
            // 
            this.tfCellBeginDebit.Multiline = true;
            this.tfCellBeginDebit.Name = "tfCellBeginDebit";
            this.tfCellBeginDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellBeginDebit.StylePriority.UsePadding = false;
            this.tfCellBeginDebit.StylePriority.UseTextAlignment = false;
            this.tfCellBeginDebit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellBeginDebit.Weight = 120D;
            // 
            // tfCellBeginCredit
            // 
            this.tfCellBeginCredit.Multiline = true;
            this.tfCellBeginCredit.Name = "tfCellBeginCredit";
            this.tfCellBeginCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellBeginCredit.StylePriority.UsePadding = false;
            this.tfCellBeginCredit.StylePriority.UseTextAlignment = false;
            this.tfCellBeginCredit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellBeginCredit.Weight = 120D;
            // 
            // tfCellIntDebit
            // 
            this.tfCellIntDebit.Multiline = true;
            this.tfCellIntDebit.Name = "tfCellIntDebit";
            this.tfCellIntDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellIntDebit.StylePriority.UsePadding = false;
            this.tfCellIntDebit.StylePriority.UseTextAlignment = false;
            this.tfCellIntDebit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellIntDebit.Weight = 120D;
            // 
            // tfCellIntCredit
            // 
            this.tfCellIntCredit.Multiline = true;
            this.tfCellIntCredit.Name = "tfCellIntCredit";
            this.tfCellIntCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellIntCredit.StylePriority.UsePadding = false;
            this.tfCellIntCredit.StylePriority.UseTextAlignment = false;
            this.tfCellIntCredit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellIntCredit.Weight = 120D;
            // 
            // tfCellEndDebit
            // 
            this.tfCellEndDebit.Multiline = true;
            this.tfCellEndDebit.Name = "tfCellEndDebit";
            this.tfCellEndDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellEndDebit.StylePriority.UsePadding = false;
            this.tfCellEndDebit.StylePriority.UseTextAlignment = false;
            this.tfCellEndDebit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellEndDebit.Weight = 120D;
            // 
            // tfCellEndCredit
            // 
            this.tfCellEndCredit.Multiline = true;
            this.tfCellEndCredit.Name = "tfCellEndCredit";
            this.tfCellEndCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.tfCellEndCredit.StylePriority.UsePadding = false;
            this.tfCellEndCredit.StylePriority.UseTextAlignment = false;
            this.tfCellEndCredit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tfCellEndCredit.Weight = 120D;
            // 
            // lblSignDate
            // 
            this.lblSignDate.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSignDate.LocationFloat = new DevExpress.Utils.PointFloat(789.291F, 35F);
            this.lblSignDate.Multiline = true;
            this.lblSignDate.Name = "lblSignDate";
            this.lblSignDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSignDate.SizeF = new System.Drawing.SizeF(340F, 20F);
            this.lblSignDate.StylePriority.UseFont = false;
            this.lblSignDate.StylePriority.UseTextAlignment = false;
            this.lblSignDate.Text = "Lập biểu ngày ..... tháng ..... năm .....";
            this.lblSignDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign1Title
            // 
            this.lblSign1Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign1Title.LocationFloat = new DevExpress.Utils.PointFloat(0F, 60F);
            this.lblSign1Title.Name = "lblSign1Title";
            this.lblSign1Title.SizeF = new System.Drawing.SizeF(320F, 20F);
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
            this.lblSign1Sub.SizeF = new System.Drawing.SizeF(320F, 18F);
            this.lblSign1Sub.StylePriority.UseFont = false;
            this.lblSign1Sub.StylePriority.UseTextAlignment = false;
            this.lblSign1Sub.Text = "(Ký, họ tên)";
            this.lblSign1Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign2Title
            // 
            this.lblSign2Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign2Title.LocationFloat = new DevExpress.Utils.PointFloat(404.6455F, 60F);
            this.lblSign2Title.Name = "lblSign2Title";
            this.lblSign2Title.SizeF = new System.Drawing.SizeF(320F, 20F);
            this.lblSign2Title.StylePriority.UseFont = false;
            this.lblSign2Title.StylePriority.UseTextAlignment = false;
            this.lblSign2Title.Text = "Kế toán trưởng";
            this.lblSign2Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign2Sub
            // 
            this.lblSign2Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign2Sub.LocationFloat = new DevExpress.Utils.PointFloat(404.6455F, 80F);
            this.lblSign2Sub.Name = "lblSign2Sub";
            this.lblSign2Sub.SizeF = new System.Drawing.SizeF(320F, 18F);
            this.lblSign2Sub.StylePriority.UseFont = false;
            this.lblSign2Sub.StylePriority.UseTextAlignment = false;
            this.lblSign2Sub.Text = "(Ký, họ tên)";
            this.lblSign2Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Title
            // 
            this.lblSign3Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign3Title.LocationFloat = new DevExpress.Utils.PointFloat(789.291F, 60F);
            this.lblSign3Title.Name = "lblSign3Title";
            this.lblSign3Title.SizeF = new System.Drawing.SizeF(340F, 20F);
            this.lblSign3Title.StylePriority.UseFont = false;
            this.lblSign3Title.StylePriority.UseTextAlignment = false;
            this.lblSign3Title.Text = "Thủ trưởng đơn vị";
            this.lblSign3Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Sub
            // 
            this.lblSign3Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign3Sub.LocationFloat = new DevExpress.Utils.PointFloat(789.291F, 80F);
            this.lblSign3Sub.Name = "lblSign3Sub";
            this.lblSign3Sub.SizeF = new System.Drawing.SizeF(340F, 18F);
            this.lblSign3Sub.StylePriority.UseFont = false;
            this.lblSign3Sub.StylePriority.UseTextAlignment = false;
            this.lblSign3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSign3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Tên công ty";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "Công ty Bình Minh";
            this.prmCompanyName.Visible = false;
            // 
            // prmFromDate
            // 
            this.prmFromDate.Description = "Từ ngày";
            this.prmFromDate.Name = "prmFromDate";
            this.prmFromDate.Type = typeof(global::System.DateTime);
            this.prmFromDate.ValueInfo = "2026-01-01";
            this.prmFromDate.Visible = false;
            // 
            // prmToDate
            // 
            this.prmToDate.Description = "Đến ngày";
            this.prmToDate.Name = "prmToDate";
            this.prmToDate.Type = typeof(global::System.DateTime);
            this.prmToDate.ValueInfo = "2026-12-31";
            this.prmToDate.Visible = false;
            // 
            // TrialBalance
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
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(20F, 20F, 25F, 35F);
            this.PageHeightF = 826.7717F;
            this.PageWidthF = 1169.291F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName,
            this.prmFromDate,
            this.prmToDate});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeaderTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeaderBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).EndInit();
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
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDateRange;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRLabel lblHeaderAccountId;
        private DevExpress.XtraReports.UI.XRLabel lblHeaderAccountName;
        private DevExpress.XtraReports.UI.XRTable tableHeaderTop;
        private DevExpress.XtraReports.UI.XRTableRow rowHeaderTop;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderTopBegin;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderTopInt;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderTopEnd;
        private DevExpress.XtraReports.UI.XRTable tableHeaderBottom;
        private DevExpress.XtraReports.UI.XRTableRow rowHeaderBottom;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderBeginDebit;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderBeginCredit;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderIntDebit;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderIntCredit;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderEndDebit;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderEndCredit;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable table2;
        private DevExpress.XtraReports.UI.XRTableRow tableRow2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell11;
        private DevExpress.XtraReports.UI.XRTableCell tableCell12;
        private DevExpress.XtraReports.UI.XRTableCell tableCell13;
        private DevExpress.XtraReports.UI.XRTableCell tableCell14;
        private DevExpress.XtraReports.UI.XRTableCell tableCell15;
        private DevExpress.XtraReports.UI.XRTableCell tableCell16;
        private DevExpress.XtraReports.UI.XRTableCell tableCell17;
        private DevExpress.XtraReports.UI.XRTableCell tableCell18;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable tableFooter;
        private DevExpress.XtraReports.UI.XRTableRow tableRowFooter;
        private DevExpress.XtraReports.UI.XRTableCell tfCellTotalLabel;
        private DevExpress.XtraReports.UI.XRTableCell tfCellBeginDebit;
        private DevExpress.XtraReports.UI.XRTableCell tfCellBeginCredit;
        private DevExpress.XtraReports.UI.XRTableCell tfCellIntDebit;
        private DevExpress.XtraReports.UI.XRTableCell tfCellIntCredit;
        private DevExpress.XtraReports.UI.XRTableCell tfCellEndDebit;
        private DevExpress.XtraReports.UI.XRTableCell tfCellEndCredit;
        private DevExpress.XtraReports.UI.XRLabel lblSignDate;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Sub;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        private DevExpress.XtraReports.Parameters.Parameter prmFromDate;
        private DevExpress.XtraReports.Parameters.Parameter prmToDate;
    }
}

