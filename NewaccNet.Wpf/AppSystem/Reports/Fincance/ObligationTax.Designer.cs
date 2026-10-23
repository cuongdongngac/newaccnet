namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    partial class ObligationTax
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary2 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary3 = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummary4 = new DevExpress.XtraReports.UI.XRSummary();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDateRange = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHeaderItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderBeginTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderCreditTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderDebitTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderEndTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tableGroupHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowGroupHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellGroupObligationName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupBeginTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupCreditTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupDebitTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupEndTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.rowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellDetailItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailBeginTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailCreditTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailDebitTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailEndTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.tableTotal = new DevExpress.XtraReports.UI.XRTable();
            this.rowTotal = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellTotalLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellTotalBeginTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellTotalCreditTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellTotalDebitTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellTotalEndTax = new DevExpress.XtraReports.UI.XRTableCell();
            this.lblDateSignature = new DevExpress.XtraReports.UI.XRLabel();
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
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableGroupHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.ObligationTax.ObligationTaxResultDto);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 30F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.pageInfo1});
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo1
            // 
            this.pageInfo1.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(467F, 5F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.SizeF = new System.Drawing.SizeF(300F, 20F);
            this.pageInfo1.StylePriority.UseFont = false;
            this.pageInfo1.StylePriority.UseTextAlignment = false;
            this.pageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageInfo1.TextFormatString = "Trang {0}/{1}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblCompanyName,
            this.lblTitle,
            this.lblDateRange});
            this.ReportHeader.HeightF = 80F;
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
            this.lblCompanyName.Text = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
            this.lblCompanyName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Arial", 13.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 24F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(767F, 26F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "PHẦN II - TÌNH HÌNH THỰC HIỆN NGHĨA VỤ ĐỐI VỚI NHÀ NƯỚC";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDateRange
            // 
            this.lblDateRange.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Từ ngày {0:dd/MM/yyyy} Đến ngày {1:dd/MM/yyyy}\', ?prmFromDate, ?prmToDate)")});
            this.lblDateRange.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateRange.LocationFloat = new DevExpress.Utils.PointFloat(0F, 52F);
            this.lblDateRange.Multiline = true;
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDateRange.SizeF = new System.Drawing.SizeF(767F, 20F);
            this.lblDateRange.StylePriority.UseFont = false;
            this.lblDateRange.StylePriority.UseTextAlignment = false;
            this.lblDateRange.Text = "Từ ngày 01/01/2026 Đến ngày 31/12/2026";
            this.lblDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableHeader});
            this.PageHeader.HeightF = 38F;
            this.PageHeader.Name = "PageHeader";
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
            this.tableHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowHeader});
            this.tableHeader.SizeF = new System.Drawing.SizeF(767F, 38F);
            this.tableHeader.StylePriority.UseBorderColor = false;
            this.tableHeader.StylePriority.UseBorders = false;
            this.tableHeader.StylePriority.UseBorderWidth = false;
            this.tableHeader.StylePriority.UseFont = false;
            this.tableHeader.StylePriority.UseTextAlignment = false;
            this.tableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // rowHeader
            // 
            this.rowHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellHeaderItemName,
            this.cellHeaderBeginTax,
            this.cellHeaderCreditTax,
            this.cellHeaderDebitTax,
            this.cellHeaderEndTax});
            this.rowHeader.Name = "rowHeader";
            this.rowHeader.Weight = 1D;
            // 
            // cellHeaderItemName
            // 
            this.cellHeaderItemName.Multiline = true;
            this.cellHeaderItemName.Name = "cellHeaderItemName";
            this.cellHeaderItemName.Text = "Chỉ tiêu";
            this.cellHeaderItemName.Weight = 287D;
            // 
            // cellHeaderBeginTax
            // 
            this.cellHeaderBeginTax.Multiline = true;
            this.cellHeaderBeginTax.Name = "cellHeaderBeginTax";
            this.cellHeaderBeginTax.Text = "Số còn phải nộp\r\nkỳ trước";
            this.cellHeaderBeginTax.Weight = 120D;
            // 
            // cellHeaderCreditTax
            // 
            this.cellHeaderCreditTax.Multiline = true;
            this.cellHeaderCreditTax.Name = "cellHeaderCreditTax";
            this.cellHeaderCreditTax.Text = "Số phải nộp kỳ\r\nnày";
            this.cellHeaderCreditTax.Weight = 120D;
            // 
            // cellHeaderDebitTax
            // 
            this.cellHeaderDebitTax.Multiline = true;
            this.cellHeaderDebitTax.Name = "cellHeaderDebitTax";
            this.cellHeaderDebitTax.Text = "Số đã nộp trong\r\nkỳ";
            this.cellHeaderDebitTax.Weight = 120D;
            // 
            // cellHeaderEndTax
            // 
            this.cellHeaderEndTax.Multiline = true;
            this.cellHeaderEndTax.Name = "cellHeaderEndTax";
            this.cellHeaderEndTax.Text = "Số còn phải nộp\r\nđến cuối kỳ này";
            this.cellHeaderEndTax.Weight = 120D;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableGroupHeader});
            this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("ObligationId", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader1.GroupUnion = DevExpress.XtraReports.UI.GroupUnion.WithFirstDetail;
            this.GroupHeader1.HeightF = 25F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // tableGroupHeader
            // 
            this.tableGroupHeader.BorderColor = System.Drawing.Color.LightGray;
            this.tableGroupHeader.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableGroupHeader.BorderWidth = 1F;
            this.tableGroupHeader.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableGroupHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableGroupHeader.Name = "tableGroupHeader";
            this.tableGroupHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowGroupHeader});
            this.tableGroupHeader.SizeF = new System.Drawing.SizeF(767F, 25F);
            this.tableGroupHeader.StylePriority.UseBorderColor = false;
            this.tableGroupHeader.StylePriority.UseBorders = false;
            this.tableGroupHeader.StylePriority.UseBorderWidth = false;
            this.tableGroupHeader.StylePriority.UseFont = false;
            // 
            // rowGroupHeader
            // 
            this.rowGroupHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellGroupObligationName,
            this.cellGroupBeginTax,
            this.cellGroupCreditTax,
            this.cellGroupDebitTax,
            this.cellGroupEndTax});
            this.rowGroupHeader.Name = "rowGroupHeader";
            this.rowGroupHeader.Weight = 1D;
            // 
            // cellGroupObligationName
            // 
            this.cellGroupObligationName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ObligationName]")});
            this.cellGroupObligationName.Multiline = true;
            this.cellGroupObligationName.Name = "cellGroupObligationName";
            this.cellGroupObligationName.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 2F, 0F, 0F, 100F);
            this.cellGroupObligationName.StylePriority.UsePadding = false;
            this.cellGroupObligationName.StylePriority.UseTextAlignment = false;
            this.cellGroupObligationName.Text = "ObligationName";
            this.cellGroupObligationName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellGroupObligationName.Weight = 287D;
            // 
            // cellGroupBeginTax
            // 
            this.cellGroupBeginTax.Name = "cellGroupBeginTax";
            this.cellGroupBeginTax.Weight = 120D;
            // 
            // cellGroupCreditTax
            // 
            this.cellGroupCreditTax.Name = "cellGroupCreditTax";
            this.cellGroupCreditTax.Weight = 120D;
            // 
            // cellGroupDebitTax
            // 
            this.cellGroupDebitTax.Name = "cellGroupDebitTax";
            this.cellGroupDebitTax.Weight = 120D;
            // 
            // cellGroupEndTax
            // 
            this.cellGroupEndTax.Name = "cellGroupEndTax";
            this.cellGroupEndTax.Weight = 120D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableDetail});
            this.Detail.HeightF = 24F;
            this.Detail.Name = "Detail";
            this.Detail.SortFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("Code", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            // 
            // tableDetail
            // 
            this.tableDetail.BorderColor = System.Drawing.Color.LightGray;
            this.tableDetail.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableDetail.BorderWidth = 1F;
            this.tableDetail.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.tableDetail.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableDetail.Name = "tableDetail";
            this.tableDetail.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableDetail.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowDetail});
            this.tableDetail.SizeF = new System.Drawing.SizeF(767F, 24F);
            this.tableDetail.StylePriority.UseBorderColor = false;
            this.tableDetail.StylePriority.UseBorders = false;
            this.tableDetail.StylePriority.UseBorderWidth = false;
            this.tableDetail.StylePriority.UseFont = false;
            // 
            // rowDetail
            // 
            this.rowDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellDetailItemName,
            this.cellDetailBeginTax,
            this.cellDetailCreditTax,
            this.cellDetailDebitTax,
            this.cellDetailEndTax});
            this.rowDetail.Name = "rowDetail";
            this.rowDetail.Weight = 1D;
            // 
            // cellDetailItemName
            // 
            this.cellDetailItemName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ItemName]")});
            this.cellDetailItemName.Multiline = true;
            this.cellDetailItemName.Name = "cellDetailItemName";
            this.cellDetailItemName.Padding = new DevExpress.XtraPrinting.PaddingInfo(12F, 2F, 0F, 0F, 100F);
            this.cellDetailItemName.StylePriority.UsePadding = false;
            this.cellDetailItemName.StylePriority.UseTextAlignment = false;
            this.cellDetailItemName.Text = "ItemName";
            this.cellDetailItemName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellDetailItemName.Weight = 287D;
            // 
            // cellDetailBeginTax
            // 
            this.cellDetailBeginTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BeginTax]")});
            this.cellDetailBeginTax.Name = "cellDetailBeginTax";
            this.cellDetailBeginTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailBeginTax.StylePriority.UsePadding = false;
            this.cellDetailBeginTax.StylePriority.UseTextAlignment = false;
            this.cellDetailBeginTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailBeginTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailBeginTax.Weight = 120D;
            // 
            // cellDetailCreditTax
            // 
            this.cellDetailCreditTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditTax]")});
            this.cellDetailCreditTax.Name = "cellDetailCreditTax";
            this.cellDetailCreditTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailCreditTax.StylePriority.UsePadding = false;
            this.cellDetailCreditTax.StylePriority.UseTextAlignment = false;
            this.cellDetailCreditTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailCreditTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailCreditTax.Weight = 120D;
            // 
            // cellDetailDebitTax
            // 
            this.cellDetailDebitTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitTax]")});
            this.cellDetailDebitTax.Name = "cellDetailDebitTax";
            this.cellDetailDebitTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailDebitTax.StylePriority.UsePadding = false;
            this.cellDetailDebitTax.StylePriority.UseTextAlignment = false;
            this.cellDetailDebitTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailDebitTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailDebitTax.Weight = 120D;
            // 
            // cellDetailEndTax
            // 
            this.cellDetailEndTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[EndTax]")});
            this.cellDetailEndTax.Name = "cellDetailEndTax";
            this.cellDetailEndTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailEndTax.StylePriority.UsePadding = false;
            this.cellDetailEndTax.StylePriority.UseTextAlignment = false;
            this.cellDetailEndTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailEndTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailEndTax.Weight = 120D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableTotal,
            this.lblDateSignature,
            this.lblSign1Title,
            this.lblSign1Sub,
            this.lblSign2Title,
            this.lblSign2Sub,
            this.lblSign3Title,
            this.lblSign3Sub});
            this.ReportFooter.HeightF = 140F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // tableTotal
            // 
            this.tableTotal.BorderColor = System.Drawing.Color.LightGray;
            this.tableTotal.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableTotal.BorderWidth = 1F;
            this.tableTotal.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableTotal.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableTotal.Name = "tableTotal";
            this.tableTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableTotal.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowTotal});
            this.tableTotal.SizeF = new System.Drawing.SizeF(767F, 25F);
            this.tableTotal.StylePriority.UseBorderColor = false;
            this.tableTotal.StylePriority.UseBorders = false;
            this.tableTotal.StylePriority.UseBorderWidth = false;
            this.tableTotal.StylePriority.UseFont = false;
            // 
            // rowTotal
            // 
            this.rowTotal.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellTotalLabel,
            this.cellTotalBeginTax,
            this.cellTotalCreditTax,
            this.cellTotalDebitTax,
            this.cellTotalEndTax});
            this.rowTotal.Name = "rowTotal";
            this.rowTotal.Weight = 1D;
            // 
            // cellTotalLabel
            // 
            this.cellTotalLabel.Multiline = true;
            this.cellTotalLabel.Name = "cellTotalLabel";
            this.cellTotalLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 2F, 0F, 0F, 100F);
            this.cellTotalLabel.StylePriority.UsePadding = false;
            this.cellTotalLabel.StylePriority.UseTextAlignment = false;
            this.cellTotalLabel.Text = "Cộng:";
            this.cellTotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellTotalLabel.Weight = 287D;
            // 
            // cellTotalBeginTax
            // 
            this.cellTotalBeginTax.CanGrow = false;
            this.cellTotalBeginTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([BeginTax])")});
            this.cellTotalBeginTax.Name = "cellTotalBeginTax";
            this.cellTotalBeginTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellTotalBeginTax.StylePriority.UsePadding = false;
            this.cellTotalBeginTax.StylePriority.UseTextAlignment = false;
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellTotalBeginTax.Summary = xrSummary1;
            this.cellTotalBeginTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellTotalBeginTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellTotalBeginTax.Weight = 120D;
            this.cellTotalBeginTax.WordWrap = false;
            // 
            // cellTotalCreditTax
            // 
            this.cellTotalCreditTax.CanGrow = false;
            this.cellTotalCreditTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditTax])")});
            this.cellTotalCreditTax.Name = "cellTotalCreditTax";
            this.cellTotalCreditTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellTotalCreditTax.StylePriority.UsePadding = false;
            this.cellTotalCreditTax.StylePriority.UseTextAlignment = false;
            xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellTotalCreditTax.Summary = xrSummary2;
            this.cellTotalCreditTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellTotalCreditTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellTotalCreditTax.Weight = 120D;
            this.cellTotalCreditTax.WordWrap = false;
            // 
            // cellTotalDebitTax
            // 
            this.cellTotalDebitTax.CanGrow = false;
            this.cellTotalDebitTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitTax])")});
            this.cellTotalDebitTax.Name = "cellTotalDebitTax";
            this.cellTotalDebitTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellTotalDebitTax.StylePriority.UsePadding = false;
            this.cellTotalDebitTax.StylePriority.UseTextAlignment = false;
            xrSummary3.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellTotalDebitTax.Summary = xrSummary3;
            this.cellTotalDebitTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellTotalDebitTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellTotalDebitTax.Weight = 120D;
            this.cellTotalDebitTax.WordWrap = false;
            // 
            // cellTotalEndTax
            // 
            this.cellTotalEndTax.CanGrow = false;
            this.cellTotalEndTax.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([EndTax])")});
            this.cellTotalEndTax.Name = "cellTotalEndTax";
            this.cellTotalEndTax.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellTotalEndTax.StylePriority.UsePadding = false;
            this.cellTotalEndTax.StylePriority.UseTextAlignment = false;
            xrSummary4.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellTotalEndTax.Summary = xrSummary4;
            this.cellTotalEndTax.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellTotalEndTax.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellTotalEndTax.Weight = 120D;
            this.cellTotalEndTax.WordWrap = false;
            // 
            // lblDateSignature
            // 
            this.lblDateSignature.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateSignature.LocationFloat = new DevExpress.Utils.PointFloat(480F, 35F);
            this.lblDateSignature.Name = "lblDateSignature";
            this.lblDateSignature.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDateSignature.SizeF = new System.Drawing.SizeF(287F, 20F);
            this.lblDateSignature.StylePriority.UseFont = false;
            this.lblDateSignature.StylePriority.UseTextAlignment = false;
            this.lblDateSignature.Text = "Lập biểu ngày      tháng      năm    ";
            this.lblDateSignature.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign1Title
            // 
            this.lblSign1Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign1Title.LocationFloat = new DevExpress.Utils.PointFloat(0F, 60F);
            this.lblSign1Title.Name = "lblSign1Title";
            this.lblSign1Title.SizeF = new System.Drawing.SizeF(240F, 20F);
            this.lblSign1Title.StylePriority.UseFont = false;
            this.lblSign1Title.StylePriority.UseTextAlignment = false;
            this.lblSign1Title.Text = "Người lập";
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
            this.lblSign2Title.LocationFloat = new DevExpress.Utils.PointFloat(260F, 60F);
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
            this.lblSign2Sub.LocationFloat = new DevExpress.Utils.PointFloat(260F, 80F);
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
            this.lblSign3Title.LocationFloat = new DevExpress.Utils.PointFloat(520F, 60F);
            this.lblSign3Title.Name = "lblSign3Title";
            this.lblSign3Title.SizeF = new System.Drawing.SizeF(247F, 20F);
            this.lblSign3Title.StylePriority.UseFont = false;
            this.lblSign3Title.StylePriority.UseTextAlignment = false;
            this.lblSign3Title.Text = "Thủ trưởng đơn vị";
            this.lblSign3Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Sub
            // 
            this.lblSign3Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign3Sub.LocationFloat = new DevExpress.Utils.PointFloat(520F, 80F);
            this.lblSign3Sub.Name = "lblSign3Sub";
            this.lblSign3Sub.SizeF = new System.Drawing.SizeF(247F, 18F);
            this.lblSign3Sub.StylePriority.UseFont = false;
            this.lblSign3Sub.StylePriority.UseTextAlignment = false;
            this.lblSign3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSign3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Tên công ty";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
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
            // ObligationTax
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageHeader,
            this.GroupHeader1,
            this.Detail,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 35F);
            this.PageHeightF = 1169F;
            this.PageWidthF = 827F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableGroupHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel lblCompanyName;
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDateRange;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable tableHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowHeader;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderBeginTax;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderCreditTax;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderDebitTax;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderEndTax;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable tableGroupHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowGroupHeader;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupObligationName;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupBeginTax;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupCreditTax;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupDebitTax;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupEndTax;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow rowDetail;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailBeginTax;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailCreditTax;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailDebitTax;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailEndTax;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable tableTotal;
        private DevExpress.XtraReports.UI.XRTableRow rowTotal;
        private DevExpress.XtraReports.UI.XRTableCell cellTotalLabel;
        private DevExpress.XtraReports.UI.XRTableCell cellTotalBeginTax;
        private DevExpress.XtraReports.UI.XRTableCell cellTotalCreditTax;
        private DevExpress.XtraReports.UI.XRTableCell cellTotalDebitTax;
        private DevExpress.XtraReports.UI.XRTableCell cellTotalEndTax;
        private DevExpress.XtraReports.UI.XRLabel lblDateSignature;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Sub;
        public DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        public DevExpress.XtraReports.Parameters.Parameter prmFromDate;
        public DevExpress.XtraReports.Parameters.Parameter prmToDate;
    }
}

