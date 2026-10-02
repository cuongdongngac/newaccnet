namespace NewaccNet.Wpf.AppSystem.Reports.Ledgers
{
    partial class GeneralLedger
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
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.pageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.label1 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.table1 = new DevExpress.XtraReports.UI.XRTable();
            this.tableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell8 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell9 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell10 = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.table2 = new DevExpress.XtraReports.UI.XRTable();
            this.tableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.tableCell13 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell14 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell15 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell17 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell18 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell19 = new DevExpress.XtraReports.UI.XRTableCell();
            this.tableCell20 = new DevExpress.XtraReports.UI.XRTableCell();
            this.objGeneralLedger = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.Title = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DetailCaption1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DetailData1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.DetailData3_Odd = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageInfo = new DevExpress.XtraReports.UI.XRControlStyle();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmAccountID = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmAccountName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmBeginDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmEndDate = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.table1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objGeneralLedger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel3});
            this.TopMargin.HeightF = 80.20834F;
            this.TopMargin.Name = "TopMargin";
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmCompanyName")});
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 10.00001F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(256.25F, 23F);
            this.xrLabel3.Text = "xrLabel3";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1,
            this.pageInfo2});
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo1
            // 
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.pageInfo1.SizeF = new System.Drawing.SizeF(484.6457F, 23F);
            this.pageInfo1.StyleName = "PageInfo";
            // 
            // pageInfo2
            // 
            this.pageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(484.6457F, 0F);
            this.pageInfo2.Name = "pageInfo2";
            this.pageInfo2.SizeF = new System.Drawing.SizeF(484.6457F, 23F);
            this.pageInfo2.StyleName = "PageInfo";
            this.pageInfo2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.pageInfo2.TextFormatString = "Page {0} of {1}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLabel4,
            this.xrLabel2,
            this.xrLabel1,
            this.label1});
            this.ReportHeader.HeightF = 90.20834F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel5
            // 
            this.xrLabel5.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmEndDate")});
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(577.0833F, 57.20835F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel5.Text = "xrLabel5";
            this.xrLabel5.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // xrLabel4
            // 
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmBeginDate")});
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(414.5833F, 57.20835F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel4.Text = "xrLabel4";
            this.xrLabel4.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmAccountName")});
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(525.0425F, 24.19434F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(378.125F, 23F);
            this.xrLabel2.Text = "xrLabel2";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // xrLabel1
            // 
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmAccountID")});
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(339.5833F, 24.19434F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(100F, 23F);
            this.xrLabel1.Text = "xrLabel1";
            // 
            // label1
            // 
            this.label1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.label1.Name = "label1";
            this.label1.SizeF = new System.Drawing.SizeF(969.2914F, 24.19433F);
            this.label1.StyleName = "Title";
            this.label1.StylePriority.UseTextAlignment = false;
            this.label1.Text = "SỔ CÁI";
            this.label1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.table1});
            this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader1.HeightF = 28F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // table1
            // 
            this.table1.LocationFloat = new DevExpress.Utils.PointFloat(10.00003F, 0F);
            this.table1.Name = "table1";
            this.table1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRow1});
            this.table1.SizeF = new System.Drawing.SizeF(1017.456F, 28F);
            // 
            // tableRow1
            // 
            this.tableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tableCell3,
            this.tableCell4,
            this.tableCell5,
            this.tableCell7,
            this.tableCell8,
            this.tableCell9,
            this.tableCell10});
            this.tableRow1.Name = "tableRow1";
            this.tableRow1.Weight = 1D;
            // 
            // tableCell3
            // 
            this.tableCell3.Name = "tableCell3";
            this.tableCell3.StyleName = "DetailCaption1";
            this.tableCell3.StylePriority.UseTextAlignment = false;
            this.tableCell3.Text = "Số CT";
            this.tableCell3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tableCell3.Weight = 0.1114114806527D;
            // 
            // tableCell4
            // 
            this.tableCell4.Name = "tableCell4";
            this.tableCell4.StyleName = "DetailCaption1";
            this.tableCell4.Text = "Voucher Date";
            this.tableCell4.Weight = 0.097074105702638941D;
            // 
            // tableCell5
            // 
            this.tableCell5.Name = "tableCell5";
            this.tableCell5.StyleName = "DetailCaption1";
            this.tableCell5.Text = "Contents";
            this.tableCell5.Weight = 0.31208361762482312D;
            // 
            // tableCell7
            // 
            this.tableCell7.Name = "tableCell7";
            this.tableCell7.StyleName = "DetailCaption1";
            this.tableCell7.Text = "Counter Account Id";
            this.tableCell7.Weight = 0.10625978939878665D;
            // 
            // tableCell8
            // 
            this.tableCell8.Name = "tableCell8";
            this.tableCell8.StyleName = "DetailCaption1";
            this.tableCell8.StylePriority.UseTextAlignment = false;
            this.tableCell8.Text = "Debit Amount";
            this.tableCell8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell8.Weight = 0.13493733133851121D;
            // 
            // tableCell9
            // 
            this.tableCell9.Name = "tableCell9";
            this.tableCell9.StyleName = "DetailCaption1";
            this.tableCell9.StylePriority.UseTextAlignment = false;
            this.tableCell9.Text = "Credit Amount";
            this.tableCell9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell9.Weight = 0.14232866900781604D;
            // 
            // tableCell10
            // 
            this.tableCell10.Name = "tableCell10";
            this.tableCell10.StyleName = "DetailCaption1";
            this.tableCell10.StylePriority.UseTextAlignment = false;
            this.tableCell10.Text = "Balance";
            this.tableCell10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell10.Weight = 0.14559503292242579D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.table2});
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            // 
            // table2
            // 
            this.table2.LocationFloat = new DevExpress.Utils.PointFloat(10F, 0F);
            this.table2.Name = "table2";
            this.table2.OddStyleName = "DetailData3_Odd";
            this.table2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRow2});
            this.table2.SizeF = new System.Drawing.SizeF(1017.456F, 25F);
            // 
            // tableRow2
            // 
            this.tableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.tableCell13,
            this.tableCell14,
            this.tableCell15,
            this.tableCell17,
            this.tableCell18,
            this.tableCell19,
            this.tableCell20});
            this.tableRow2.Name = "tableRow2";
            this.tableRow2.Weight = 11.5D;
            // 
            // tableCell13
            // 
            this.tableCell13.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherNo]")});
            this.tableCell13.Name = "tableCell13";
            this.tableCell13.StyleName = "DetailData1";
            this.tableCell13.Weight = 0.1114115057318476D;
            // 
            // tableCell14
            // 
            this.tableCell14.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherDate]")});
            this.tableCell14.Name = "tableCell14";
            this.tableCell14.StyleName = "DetailData1";
            this.tableCell14.TextFormatString = "{0:dd/mm/yyyy}";
            this.tableCell14.Weight = 0.097074129242330678D;
            // 
            // tableCell15
            // 
            this.tableCell15.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Contents]")});
            this.tableCell15.Name = "tableCell15";
            this.tableCell15.StyleName = "DetailData1";
            this.tableCell15.Weight = 0.31208359682499776D;
            // 
            // tableCell17
            // 
            this.tableCell17.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountId]")});
            this.tableCell17.Name = "tableCell17";
            this.tableCell17.StyleName = "DetailData1";
            this.tableCell17.Weight = 0.1062599149071275D;
            // 
            // tableCell18
            // 
            this.tableCell18.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitAmount]")});
            this.tableCell18.Name = "tableCell18";
            this.tableCell18.StyleName = "DetailData1";
            this.tableCell18.StylePriority.UseTextAlignment = false;
            this.tableCell18.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell18.TextFormatString = "{0:C2}";
            this.tableCell18.Weight = 0.13493733172366137D;
            // 
            // tableCell19
            // 
            this.tableCell19.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditAmount]")});
            this.tableCell19.Name = "tableCell19";
            this.tableCell19.StyleName = "DetailData1";
            this.tableCell19.StylePriority.UseTextAlignment = false;
            this.tableCell19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell19.TextFormatString = "{0:C2}";
            this.tableCell19.Weight = 0.14232866942047787D;
            // 
            // tableCell20
            // 
            this.tableCell20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Balance]")});
            this.tableCell20.Name = "tableCell20";
            this.tableCell20.StyleName = "DetailData1";
            this.tableCell20.StylePriority.UseTextAlignment = false;
            this.tableCell20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.tableCell20.TextFormatString = "{0:C2}";
            this.tableCell20.Weight = 0.14559501817060377D;
            // 
            // objGeneralLedger
            // 
            this.objGeneralLedger.DataSource = typeof(global::NewaccNet.Reports.LedgerReportDTO);
            this.objGeneralLedger.Name = "objGeneralLedger";
            // 
            // Title
            // 
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.BorderColor = System.Drawing.Color.Black;
            this.Title.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.Title.BorderWidth = 1F;
            this.Title.Font = new DevExpress.Drawing.DXFont("Arial", 14.25F);
            this.Title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.Title.Name = "Title";
            this.Title.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 6F, 0F, 0F, 100F);
            // 
            // DetailCaption1
            // 
            this.DetailCaption1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(212)))), ((int)(((byte)(246)))));
            this.DetailCaption1.BorderColor = System.Drawing.Color.White;
            this.DetailCaption1.Borders = DevExpress.XtraPrinting.BorderSide.Left;
            this.DetailCaption1.BorderWidth = 2F;
            this.DetailCaption1.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Bold);
            this.DetailCaption1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.DetailCaption1.Name = "DetailCaption1";
            this.DetailCaption1.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 6F, 0F, 0F, 100F);
            this.DetailCaption1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // DetailData1
            // 
            this.DetailData1.BorderColor = System.Drawing.Color.Transparent;
            this.DetailData1.Borders = DevExpress.XtraPrinting.BorderSide.Left;
            this.DetailData1.BorderWidth = 2F;
            this.DetailData1.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F);
            this.DetailData1.ForeColor = System.Drawing.Color.Black;
            this.DetailData1.Name = "DetailData1";
            this.DetailData1.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 6F, 0F, 0F, 100F);
            this.DetailData1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // DetailData3_Odd
            // 
            this.DetailData3_Odd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(245)))), ((int)(((byte)(248)))));
            this.DetailData3_Odd.BorderColor = System.Drawing.Color.Transparent;
            this.DetailData3_Odd.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.DetailData3_Odd.BorderWidth = 1F;
            this.DetailData3_Odd.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F);
            this.DetailData3_Odd.ForeColor = System.Drawing.Color.Black;
            this.DetailData3_Odd.Name = "DetailData3_Odd";
            this.DetailData3_Odd.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 6F, 0F, 0F, 100F);
            this.DetailData3_Odd.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // PageInfo
            // 
            this.PageInfo.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Bold);
            this.PageInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(70)))), ((int)(((byte)(80)))));
            this.PageInfo.Name = "PageInfo";
            this.PageInfo.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 6F, 0F, 0F, 100F);
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Name of Company";
            this.prmCompanyName.Name = "prmCompanyName";
            // 
            // prmAccountID
            // 
            this.prmAccountID.Description = "Mã tài khoản";
            this.prmAccountID.Name = "prmAccountID";
            // 
            // prmAccountName
            // 
            this.prmAccountName.Description = "Name of Account";
            this.prmAccountName.Name = "prmAccountName";
            this.prmAccountName.Type = typeof(int);
            this.prmAccountName.ValueInfo = "0";
            // 
            // prmBeginDate
            // 
            this.prmBeginDate.Description = "BeginDate";
            this.prmBeginDate.Name = "prmBeginDate";
            this.prmBeginDate.Type = typeof(global::System.DateOnly);
            this.prmBeginDate.ValueInfo = "2026-10-01";
            // 
            // prmEndDate
            // 
            this.prmEndDate.Description = "EndDate";
            this.prmEndDate.Name = "prmEndDate";
            this.prmEndDate.Type = typeof(global::System.DateOnly);
            this.prmEndDate.ValueInfo = "2026-10-01";
            // 
            // GeneralLedger
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.GroupHeader1,
            this.Detail});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objGeneralLedger});
            this.DataSource = this.objGeneralLedger;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(72F, 48F, 80.20834F, 100F);
            this.PageHeightF = 826.7717F;
            this.PageWidthF = 1169.291F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.ParameterPanelLayoutItems.AddRange(new DevExpress.XtraReports.Parameters.ParameterPanelLayoutItem[] {
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.prmCompanyName, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.prmAccountID, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.prmAccountName, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.prmBeginDate, DevExpress.XtraReports.Parameters.Orientation.Horizontal),
            new DevExpress.XtraReports.Parameters.ParameterLayoutItem(this.prmEndDate, DevExpress.XtraReports.Parameters.Orientation.Horizontal)});
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName,
            this.prmAccountID,
            this.prmAccountName,
            this.prmBeginDate,
            this.prmEndDate});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.Title,
            this.DetailCaption1,
            this.DetailData1,
            this.DetailData3_Odd,
            this.PageInfo});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.table1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.table2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objGeneralLedger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo2;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRTableRow tableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.XtraReports.UI.XRTableCell tableCell4;
        private DevExpress.XtraReports.UI.XRTableCell tableCell5;
        private DevExpress.XtraReports.UI.XRTableCell tableCell7;
        private DevExpress.XtraReports.UI.XRTableCell tableCell8;
        private DevExpress.XtraReports.UI.XRTableCell tableCell9;
        private DevExpress.XtraReports.UI.XRTableCell tableCell10;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable table2;
        private DevExpress.XtraReports.UI.XRTableRow tableRow2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell13;
        private DevExpress.XtraReports.UI.XRTableCell tableCell14;
        private DevExpress.XtraReports.UI.XRTableCell tableCell15;
        private DevExpress.XtraReports.UI.XRTableCell tableCell17;
        private DevExpress.XtraReports.UI.XRTableCell tableCell18;
        private DevExpress.XtraReports.UI.XRTableCell tableCell19;
        private DevExpress.XtraReports.UI.XRTableCell tableCell20;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objGeneralLedger;
        private DevExpress.XtraReports.UI.XRControlStyle Title;
        private DevExpress.XtraReports.UI.XRControlStyle DetailCaption1;
        private DevExpress.XtraReports.UI.XRControlStyle DetailData1;
        private DevExpress.XtraReports.UI.XRControlStyle DetailData3_Odd;
        private DevExpress.XtraReports.UI.XRControlStyle PageInfo;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        private DevExpress.XtraReports.Parameters.Parameter prmAccountID;
        private DevExpress.XtraReports.Parameters.Parameter prmAccountName;
        private DevExpress.XtraReports.Parameters.Parameter prmBeginDate;
        private DevExpress.XtraReports.Parameters.Parameter prmEndDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
    }
}
