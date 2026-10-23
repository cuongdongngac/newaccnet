namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    partial class CashFlow
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
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.pageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSubTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDateRange = new DevExpress.XtraReports.UI.XRLabel();
            this.lblUnit = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHIllu = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHPrevAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.groupHeaderCategory = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tableGroupHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowGroupHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellGroupCatName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupIllu = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellGroupPrevAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.rowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellDetailItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailIllu = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailPrevAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.groupFooterCategory = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.tableGroupFooter = new DevExpress.XtraReports.UI.XRTable();
            this.rowGroupFooter = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellFooterName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellFooterCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellFooterIllu = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellFooterAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellFooterPrevAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.lblSignDate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner1Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner2Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSigner3Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmFromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmToDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmCurrentTitle = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmPrevTitle = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableGroupHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableGroupFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.CashFlow.CashFlowReportDTO);
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
            this.BottomMargin.HeightF = 30F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // pageInfo1
            // 
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 5F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.pageInfo1.SizeF = new System.Drawing.SizeF(766.7717F, 20F);
            this.pageInfo1.StylePriority.UseTextAlignment = false;
            this.pageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.pageInfo1.TextFormatString = "Trang {0}/{1}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblCompanyName,
            this.lblTitle,
            this.lblSubTitle,
            this.lblDateRange,
            this.lblUnit});
            this.ReportHeader.HeightF = 115F;
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
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 26F);
            this.lblTitle.Multiline = true;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(766.7717F, 24F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "BÁO CÁO LƯU CHUYỂN TIỀN TỆ";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Font = new DevExpress.Drawing.DXFont("Arial", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSubTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 50F);
            this.lblSubTitle.Multiline = true;
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSubTitle.SizeF = new System.Drawing.SizeF(766.7717F, 20F);
            this.lblSubTitle.StylePriority.UseFont = false;
            this.lblSubTitle.StylePriority.UseTextAlignment = false;
            this.lblSubTitle.Text = "PHƯƠNG PHÁP TRỰC TIẾP";
            this.lblSubTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDateRange
            // 
            this.lblDateRange.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Từ ngày {0:d/M/yyyy} Đến ngày {1:d/M/yyyy}\', ?prmFromDate, ?prmToDate)" +
                "")});
            this.lblDateRange.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateRange.LocationFloat = new DevExpress.Utils.PointFloat(0F, 70F);
            this.lblDateRange.Multiline = true;
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDateRange.SizeF = new System.Drawing.SizeF(766.7717F, 20F);
            this.lblDateRange.StylePriority.UseFont = false;
            this.lblDateRange.StylePriority.UseTextAlignment = false;
            this.lblDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblUnit
            // 
            this.lblUnit.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblUnit.LocationFloat = new DevExpress.Utils.PointFloat(566.7717F, 94F);
            this.lblUnit.Multiline = true;
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblUnit.SizeF = new System.Drawing.SizeF(200F, 18F);
            this.lblUnit.StylePriority.UseFont = false;
            this.lblUnit.StylePriority.UseTextAlignment = false;
            this.lblUnit.Text = "Đơn vị tính: đồng";
            this.lblUnit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableHeader});
            this.PageHeader.HeightF = 28F;
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
            this.tableHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowHeader});
            this.tableHeader.SizeF = new System.Drawing.SizeF(766.7717F, 28F);
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
            this.cellHItemName,
            this.cellHCode,
            this.cellHIllu,
            this.cellHAmount,
            this.cellHPrevAmount});
            this.rowHeader.Name = "rowHeader";
            this.rowHeader.Weight = 1D;
            // 
            // cellHItemName
            // 
            this.cellHItemName.Name = "cellHItemName";
            this.cellHItemName.Text = "Chỉ tiêu";
            this.cellHItemName.Weight = 386.7717D;
            // 
            // cellHCode
            // 
            this.cellHCode.Name = "cellHCode";
            this.cellHCode.Text = "Mã số";
            this.cellHCode.Weight = 50D;
            // 
            // cellHIllu
            // 
            this.cellHIllu.Name = "cellHIllu";
            this.cellHIllu.Text = "Thuyết minh";
            this.cellHIllu.Weight = 70D;
            // 
            // cellHAmount
            // 
            this.cellHAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif(IsNullOrEmpty(?prmCurrentTitle), \'Kỳ này\', ?prmCurrentTitle)")});
            this.cellHAmount.Multiline = true;
            this.cellHAmount.Name = "cellHAmount";
            this.cellHAmount.Text = "Kỳ này";
            this.cellHAmount.Weight = 130D;
            // 
            // cellHPrevAmount
            // 
            this.cellHPrevAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif(IsNullOrEmpty(?prmPrevTitle), \'Kỳ trước\', ?prmPrevTitle)")});
            this.cellHPrevAmount.Multiline = true;
            this.cellHPrevAmount.Name = "cellHPrevAmount";
            this.cellHPrevAmount.Text = "Kỳ trước";
            this.cellHPrevAmount.Weight = 130D;
            // 
            // groupHeaderCategory
            // 
            this.groupHeaderCategory.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableGroupHeader});
            this.groupHeaderCategory.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("CashFlowCategoryId", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.groupHeaderCategory.HeightF = 24F;
            this.groupHeaderCategory.Name = "groupHeaderCategory";
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
            this.tableGroupHeader.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableGroupHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowGroupHeader});
            this.tableGroupHeader.SizeF = new System.Drawing.SizeF(766.7717F, 24F);
            this.tableGroupHeader.StylePriority.UseBorderColor = false;
            this.tableGroupHeader.StylePriority.UseBorders = false;
            this.tableGroupHeader.StylePriority.UseBorderWidth = false;
            this.tableGroupHeader.StylePriority.UseFont = false;
            // 
            // rowGroupHeader
            // 
            this.rowGroupHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellGroupCatName,
            this.cellGroupCode,
            this.cellGroupIllu,
            this.cellGroupAmount,
            this.cellGroupPrevAmount});
            this.rowGroupHeader.Name = "rowGroupHeader";
            this.rowGroupHeader.Weight = 1D;
            // 
            // cellGroupCatName
            // 
            this.cellGroupCatName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CashFlowName]")});
            this.cellGroupCatName.Multiline = true;
            this.cellGroupCatName.Name = "cellGroupCatName";
            this.cellGroupCatName.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 2F, 0F, 0F, 100F);
            this.cellGroupCatName.StylePriority.UsePadding = false;
            this.cellGroupCatName.StylePriority.UseTextAlignment = false;
            this.cellGroupCatName.Text = "CashFlowName";
            this.cellGroupCatName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellGroupCatName.Weight = 386.7717D;
            // 
            // cellGroupCode
            // 
            this.cellGroupCode.Name = "cellGroupCode";
            this.cellGroupCode.Weight = 50D;
            // 
            // cellGroupIllu
            // 
            this.cellGroupIllu.Name = "cellGroupIllu";
            this.cellGroupIllu.Weight = 70D;
            // 
            // cellGroupAmount
            // 
            this.cellGroupAmount.Name = "cellGroupAmount";
            this.cellGroupAmount.Weight = 130D;
            // 
            // cellGroupPrevAmount
            // 
            this.cellGroupPrevAmount.Name = "cellGroupPrevAmount";
            this.cellGroupPrevAmount.Weight = 130D;
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
            this.tableDetail.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F);
            this.tableDetail.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableDetail.Name = "tableDetail";
            this.tableDetail.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableDetail.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowDetail});
            this.tableDetail.SizeF = new System.Drawing.SizeF(766.7717F, 24F);
            this.tableDetail.StylePriority.UseBorderColor = false;
            this.tableDetail.StylePriority.UseBorders = false;
            this.tableDetail.StylePriority.UseBorderWidth = false;
            this.tableDetail.StylePriority.UseFont = false;
            // 
            // rowDetail
            // 
            this.rowDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellDetailItemName,
            this.cellDetailCode,
            this.cellDetailIllu,
            this.cellDetailAmount,
            this.cellDetailPrevAmount});
            this.rowDetail.Name = "rowDetail";
            this.rowDetail.Weight = 1D;
            // 
            // cellDetailItemName
            // 
            this.cellDetailItemName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ItemName]")});
            this.cellDetailItemName.Multiline = true;
            this.cellDetailItemName.Name = "cellDetailItemName";
            this.cellDetailItemName.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 2F, 0F, 0F, 100F);
            this.cellDetailItemName.StylePriority.UsePadding = false;
            this.cellDetailItemName.StylePriority.UseTextAlignment = false;
            this.cellDetailItemName.Text = "ItemName";
            this.cellDetailItemName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellDetailItemName.Weight = 386.7717D;
            // 
            // cellDetailCode
            // 
            this.cellDetailCode.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Code]")});
            this.cellDetailCode.Name = "cellDetailCode";
            this.cellDetailCode.StylePriority.UseTextAlignment = false;
            this.cellDetailCode.Text = "Code";
            this.cellDetailCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellDetailCode.Weight = 50D;
            // 
            // cellDetailIllu
            // 
            this.cellDetailIllu.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Illustration]")});
            this.cellDetailIllu.Name = "cellDetailIllu";
            this.cellDetailIllu.StylePriority.UseTextAlignment = false;
            this.cellDetailIllu.Text = "Illustration";
            this.cellDetailIllu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellDetailIllu.Weight = 70D;
            // 
            // cellDetailAmount
            // 
            this.cellDetailAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Amount]")});
            this.cellDetailAmount.Multiline = true;
            this.cellDetailAmount.Name = "cellDetailAmount";
            this.cellDetailAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailAmount.StylePriority.UsePadding = false;
            this.cellDetailAmount.StylePriority.UseTextAlignment = false;
            this.cellDetailAmount.Text = "Amount";
            this.cellDetailAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailAmount.Weight = 130D;
            // 
            // cellDetailPrevAmount
            // 
            this.cellDetailPrevAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PrevAmount]")});
            this.cellDetailPrevAmount.Multiline = true;
            this.cellDetailPrevAmount.Name = "cellDetailPrevAmount";
            this.cellDetailPrevAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailPrevAmount.StylePriority.UsePadding = false;
            this.cellDetailPrevAmount.StylePriority.UseTextAlignment = false;
            this.cellDetailPrevAmount.Text = "PrevAmount";
            this.cellDetailPrevAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailPrevAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailPrevAmount.Weight = 130D;
            // 
            // groupFooterCategory
            // 
            this.groupFooterCategory.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableGroupFooter});
            this.groupFooterCategory.HeightF = 25F;
            this.groupFooterCategory.Name = "groupFooterCategory";
            // 
            // tableGroupFooter
            // 
            this.tableGroupFooter.BorderColor = System.Drawing.Color.LightGray;
            this.tableGroupFooter.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableGroupFooter.BorderWidth = 1F;
            this.tableGroupFooter.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableGroupFooter.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableGroupFooter.Name = "tableGroupFooter";
            this.tableGroupFooter.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableGroupFooter.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowGroupFooter});
            this.tableGroupFooter.SizeF = new System.Drawing.SizeF(766.7717F, 25F);
            this.tableGroupFooter.StylePriority.UseBorderColor = false;
            this.tableGroupFooter.StylePriority.UseBorders = false;
            this.tableGroupFooter.StylePriority.UseBorderWidth = false;
            this.tableGroupFooter.StylePriority.UseFont = false;
            // 
            // rowGroupFooter
            // 
            this.rowGroupFooter.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellFooterName,
            this.cellFooterCode,
            this.cellFooterIllu,
            this.cellFooterAmount,
            this.cellFooterPrevAmount});
            this.rowGroupFooter.Name = "rowGroupFooter";
            this.rowGroupFooter.Weight = 1D;
            // 
            // cellFooterName
            // 
            this.cellFooterName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CashFlowNameRepeate]")});
            this.cellFooterName.Font = new DevExpress.Drawing.DXFont("Arial", 9F, ((DevExpress.Drawing.DXFontStyle)((DevExpress.Drawing.DXFontStyle.Bold | DevExpress.Drawing.DXFontStyle.Italic))));
            this.cellFooterName.Multiline = true;
            this.cellFooterName.Name = "cellFooterName";
            this.cellFooterName.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 2F, 0F, 0F, 100F);
            this.cellFooterName.StylePriority.UseFont = false;
            this.cellFooterName.StylePriority.UsePadding = false;
            this.cellFooterName.StylePriority.UseTextAlignment = false;
            this.cellFooterName.Text = "CashFlowNameRepeate";
            this.cellFooterName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellFooterName.Weight = 386.7717D;
            // 
            // cellFooterCode
            // 
            this.cellFooterCode.Name = "cellFooterCode";
            this.cellFooterCode.Weight = 50D;
            // 
            // cellFooterIllu
            // 
            this.cellFooterIllu.Name = "cellFooterIllu";
            this.cellFooterIllu.Weight = 70D;
            // 
            // cellFooterAmount
            // 
            this.cellFooterAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([SignedAmount])")});
            this.cellFooterAmount.Multiline = true;
            this.cellFooterAmount.Name = "cellFooterAmount";
            this.cellFooterAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellFooterAmount.StylePriority.UsePadding = false;
            this.cellFooterAmount.StylePriority.UseTextAlignment = false;
            xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.cellFooterAmount.Summary = xrSummary1;
            this.cellFooterAmount.Text = "0";
            this.cellFooterAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellFooterAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellFooterAmount.Weight = 130D;
            // 
            // cellFooterPrevAmount
            // 
            this.cellFooterPrevAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([SignedPrevAmount])")});
            this.cellFooterPrevAmount.Multiline = true;
            this.cellFooterPrevAmount.Name = "cellFooterPrevAmount";
            this.cellFooterPrevAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellFooterPrevAmount.StylePriority.UsePadding = false;
            this.cellFooterPrevAmount.StylePriority.UseTextAlignment = false;
            xrSummary2.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.cellFooterPrevAmount.Summary = xrSummary2;
            this.cellFooterPrevAmount.Text = "0";
            this.cellFooterPrevAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellFooterPrevAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellFooterPrevAmount.Weight = 130D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblSignDate,
            this.lblSigner1,
            this.lblSigner1Sub,
            this.lblSigner2,
            this.lblSigner2Sub,
            this.lblSigner3,
            this.lblSigner3Sub});
            this.ReportFooter.HeightF = 130F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // lblSignDate
            // 
            this.lblSignDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Hà Nội, ngày {0:dd} tháng {0:MM} năm {0:yyyy}\', ?prmToDate)")});
            this.lblSignDate.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSignDate.LocationFloat = new DevExpress.Utils.PointFloat(500F, 10F);
            this.lblSignDate.Multiline = true;
            this.lblSignDate.Name = "lblSignDate";
            this.lblSignDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSignDate.SizeF = new System.Drawing.SizeF(266.7717F, 20F);
            this.lblSignDate.StylePriority.UseFont = false;
            this.lblSignDate.StylePriority.UseTextAlignment = false;
            this.lblSignDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSigner1
            // 
            this.lblSigner1.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSigner1.LocationFloat = new DevExpress.Utils.PointFloat(10F, 35F);
            this.lblSigner1.Multiline = true;
            this.lblSigner1.Name = "lblSigner1";
            this.lblSigner1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSigner1.SizeF = new System.Drawing.SizeF(230F, 20F);
            this.lblSigner1.StylePriority.UseFont = false;
            this.lblSigner1.StylePriority.UseTextAlignment = false;
            this.lblSigner1.Text = "Người lập biểu";
            this.lblSigner1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSigner1Sub
            // 
            this.lblSigner1Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSigner1Sub.LocationFloat = new DevExpress.Utils.PointFloat(10F, 55F);
            this.lblSigner1Sub.Multiline = true;
            this.lblSigner1Sub.Name = "lblSigner1Sub";
            this.lblSigner1Sub.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSigner1Sub.SizeF = new System.Drawing.SizeF(230F, 18F);
            this.lblSigner1Sub.StylePriority.UseFont = false;
            this.lblSigner1Sub.StylePriority.UseTextAlignment = false;
            this.lblSigner1Sub.Text = "(Ký, họ tên)";
            this.lblSigner1Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSigner2
            // 
            this.lblSigner2.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSigner2.LocationFloat = new DevExpress.Utils.PointFloat(265F, 35F);
            this.lblSigner2.Multiline = true;
            this.lblSigner2.Name = "lblSigner2";
            this.lblSigner2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSigner2.SizeF = new System.Drawing.SizeF(235F, 20F);
            this.lblSigner2.StylePriority.UseFont = false;
            this.lblSigner2.StylePriority.UseTextAlignment = false;
            this.lblSigner2.Text = "Kế toán trưởng";
            this.lblSigner2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSigner2Sub
            // 
            this.lblSigner2Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSigner2Sub.LocationFloat = new DevExpress.Utils.PointFloat(265F, 55F);
            this.lblSigner2Sub.Multiline = true;
            this.lblSigner2Sub.Name = "lblSigner2Sub";
            this.lblSigner2Sub.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSigner2Sub.SizeF = new System.Drawing.SizeF(235F, 18F);
            this.lblSigner2Sub.StylePriority.UseFont = false;
            this.lblSigner2Sub.StylePriority.UseTextAlignment = false;
            this.lblSigner2Sub.Text = "(Ký, họ tên)";
            this.lblSigner2Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSigner3
            // 
            this.lblSigner3.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSigner3.LocationFloat = new DevExpress.Utils.PointFloat(520F, 35F);
            this.lblSigner3.Multiline = true;
            this.lblSigner3.Name = "lblSigner3";
            this.lblSigner3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSigner3.SizeF = new System.Drawing.SizeF(240F, 20F);
            this.lblSigner3.StylePriority.UseFont = false;
            this.lblSigner3.StylePriority.UseTextAlignment = false;
            this.lblSigner3.Text = "Giám đốc";
            this.lblSigner3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSigner3Sub
            // 
            this.lblSigner3Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSigner3Sub.LocationFloat = new DevExpress.Utils.PointFloat(520F, 55F);
            this.lblSigner3Sub.Multiline = true;
            this.lblSigner3Sub.Name = "lblSigner3Sub";
            this.lblSigner3Sub.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSigner3Sub.SizeF = new System.Drawing.SizeF(240F, 18F);
            this.lblSigner3Sub.StylePriority.UseFont = false;
            this.lblSigner3Sub.StylePriority.UseTextAlignment = false;
            this.lblSigner3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSigner3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
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
            // prmCurrentTitle
            // 
            this.prmCurrentTitle.Description = "Tiêu đề cột kỳ báo cáo";
            this.prmCurrentTitle.Name = "prmCurrentTitle";
            this.prmCurrentTitle.ValueInfo = "Kỳ này";
            this.prmCurrentTitle.Visible = false;
            // 
            // prmPrevTitle
            // 
            this.prmPrevTitle.Description = "Tiêu đề cột kỳ so sánh";
            this.prmPrevTitle.Name = "prmPrevTitle";
            this.prmPrevTitle.ValueInfo = "Kỳ trước";
            this.prmPrevTitle.Visible = false;
            // 
            // CashFlow
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageHeader,
            this.groupHeaderCategory,
            this.Detail,
            this.groupFooterCategory,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9.75F);
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 30F);
            this.PageHeightF = 1169.291F;
            this.PageWidthF = 826.7717F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName,
            this.prmFromDate,
            this.prmToDate,
            this.prmCurrentTitle,
            this.prmPrevTitle});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableGroupHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableGroupFooter)).EndInit();
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
        private DevExpress.XtraReports.UI.XRLabel lblSubTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDateRange;
        private DevExpress.XtraReports.UI.XRLabel lblUnit;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable tableHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowHeader;
        private DevExpress.XtraReports.UI.XRTableCell cellHItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellHCode;
        private DevExpress.XtraReports.UI.XRTableCell cellHIllu;
        private DevExpress.XtraReports.UI.XRTableCell cellHAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellHPrevAmount;
        private DevExpress.XtraReports.UI.GroupHeaderBand groupHeaderCategory;
        private DevExpress.XtraReports.UI.XRTable tableGroupHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowGroupHeader;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupCatName;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupCode;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupIllu;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellGroupPrevAmount;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow rowDetail;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailCode;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailIllu;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailPrevAmount;
        private DevExpress.XtraReports.UI.GroupFooterBand groupFooterCategory;
        private DevExpress.XtraReports.UI.XRTable tableGroupFooter;
        private DevExpress.XtraReports.UI.XRTableRow rowGroupFooter;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterName;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterCode;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterIllu;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterPrevAmount;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel lblSignDate;
        private DevExpress.XtraReports.UI.XRLabel lblSigner1;
        private DevExpress.XtraReports.UI.XRLabel lblSigner1Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSigner2;
        private DevExpress.XtraReports.UI.XRLabel lblSigner2Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSigner3;
        private DevExpress.XtraReports.UI.XRLabel lblSigner3Sub;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        private DevExpress.XtraReports.Parameters.Parameter prmFromDate;
        private DevExpress.XtraReports.Parameters.Parameter prmToDate;
        private DevExpress.XtraReports.Parameters.Parameter prmCurrentTitle;
        private DevExpress.XtraReports.Parameters.Parameter prmPrevTitle;
    }
}
