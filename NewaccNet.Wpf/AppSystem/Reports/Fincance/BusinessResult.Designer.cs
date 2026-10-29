namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    partial class BusinessResult
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
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDateRange = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHeaderItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderIllu = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderOtherAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.tableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellDetailItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailIllu = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailOtherAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
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
            this.prmCurrentTitle = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmPrevTitle = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.BusinessResult.BusinessResultReportDTO);
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
            this.pageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(466.7717F, 5F);
            this.pageInfo1.Name = "pageInfo1";
            this.pageInfo1.SizeF = new System.Drawing.SizeF(300F, 20F);
            this.pageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.pageInfo1.TextFormatString = "Trang {0}/{1}";
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
            this.lblCompanyName.Text = "";
            this.lblCompanyName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Arial", 14F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 24F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(766.7717F, 25F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "BÁO CÁO KẾT QUẢ HOẠT ĐỘNG KINH DOANH";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDateRange
            // 
            this.lblDateRange.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Từ ngày {0:d/M/yyyy} Đến ngày {1:d/M/yyyy}\', ?prmFromDate, ?prmToDa" +
                    "te)")});
            this.lblDateRange.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateRange.LocationFloat = new DevExpress.Utils.PointFloat(0F, 50F);
            this.lblDateRange.Multiline = true;
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDateRange.SizeF = new System.Drawing.SizeF(766.7717F, 20F);
            this.lblDateRange.StylePriority.UseFont = false;
            this.lblDateRange.StylePriority.UseTextAlignment = false;
            this.lblDateRange.Text = "Từ ngày 01/01/2026 Đến ngày 31/12/2026";
            this.lblDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableHeader});
            this.PageHeader.HeightF = 30F;
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
            this.tableHeader.SizeF = new System.Drawing.SizeF(766.7717F, 30F);
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
            this.cellHeaderCode,
            this.cellHeaderIllu,
            this.cellHeaderAmount,
            this.cellHeaderOtherAmount});
            this.rowHeader.Name = "rowHeader";
            this.rowHeader.Weight = 1D;
            // 
            // cellHeaderItemName
            // 
            this.cellHeaderItemName.Name = "cellHeaderItemName";
            this.cellHeaderItemName.Text = "Chỉ tiêu";
            this.cellHeaderItemName.Weight = 386.7717D;
            // 
            // cellHeaderCode
            // 
            this.cellHeaderCode.Name = "cellHeaderCode";
            this.cellHeaderCode.Text = "Mã số";
            this.cellHeaderCode.Weight = 50D;
            // 
            // cellHeaderIllu
            // 
            this.cellHeaderIllu.Name = "cellHeaderIllu";
            this.cellHeaderIllu.Text = "Thuyết minh";
            this.cellHeaderIllu.Weight = 70D;
            // 
            // cellHeaderAmount
            // 
            this.cellHeaderAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif(IsNullOrEmpty(?prmCurrentTitle), \'Năm nay\', ?prmCurrentTitle)")});
            this.cellHeaderAmount.Multiline = true;
            this.cellHeaderAmount.Name = "cellHeaderAmount";
            this.cellHeaderAmount.Text = "Năm nay";
            this.cellHeaderAmount.Weight = 130D;
            // 
            // cellHeaderOtherAmount
            // 
            this.cellHeaderOtherAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif(IsNullOrEmpty(?prmPrevTitle), \'Năm trước\', ?prmPrevTitle)")});
            this.cellHeaderOtherAmount.Multiline = true;
            this.cellHeaderOtherAmount.Name = "cellHeaderOtherAmount";
            this.cellHeaderOtherAmount.Text = "Năm trước";
            this.cellHeaderOtherAmount.Weight = 130D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableDetail});
            this.Detail.HeightF = 24F;
            this.Detail.Name = "Detail";
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
            this.tableRow2});
            this.tableDetail.SizeF = new System.Drawing.SizeF(766.7717F, 24F);
            this.tableDetail.StylePriority.UseBorderColor = false;
            this.tableDetail.StylePriority.UseBorders = false;
            this.tableDetail.StylePriority.UseBorderWidth = false;
            this.tableDetail.StylePriority.UseFont = false;
            // 
            // tableRow2
            // 
            this.tableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellDetailItemName,
            this.cellDetailCode,
            this.cellDetailIllu,
            this.cellDetailAmount,
            this.cellDetailOtherAmount});
            this.tableRow2.Name = "tableRow2";
            this.tableRow2.Weight = 1D;
            // 
            // cellDetailItemName
            // 
            this.cellDetailItemName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ItemName]")});
            this.cellDetailItemName.Multiline = true;
            this.cellDetailItemName.Name = "cellDetailItemName";
            this.cellDetailItemName.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 2F, 0F, 0F, 100F);
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
            // cellDetailOtherAmount
            // 
            this.cellDetailOtherAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OtherAmount]")});
            this.cellDetailOtherAmount.Multiline = true;
            this.cellDetailOtherAmount.Name = "cellDetailOtherAmount";
            this.cellDetailOtherAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailOtherAmount.StylePriority.UsePadding = false;
            this.cellDetailOtherAmount.StylePriority.UseTextAlignment = false;
            this.cellDetailOtherAmount.Text = "OtherAmount";
            this.cellDetailOtherAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailOtherAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailOtherAmount.Weight = 130D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblSignDate,
            this.lblSign1Title,
            this.lblSign1Sub,
            this.lblSign2Title,
            this.lblSign2Sub,
            this.lblSign3Title,
            this.lblSign3Sub});
            this.ReportFooter.HeightF = 130F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // lblSignDate
            // 
            this.lblSignDate.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSignDate.LocationFloat = new DevExpress.Utils.PointFloat(466.7717F, 15F);
            this.lblSignDate.Multiline = true;
            this.lblSignDate.Name = "lblSignDate";
            this.lblSignDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSignDate.SizeF = new System.Drawing.SizeF(300F, 20F);
            this.lblSignDate.StylePriority.UseFont = false;
            this.lblSignDate.StylePriority.UseTextAlignment = false;
            this.lblSignDate.Text = "Lập biểu ngày ..... tháng ..... năm .....";
            this.lblSignDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign1Title
            // 
            this.lblSign1Title.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSign1Title.LocationFloat = new DevExpress.Utils.PointFloat(0F, 40F);
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
            this.lblSign1Sub.LocationFloat = new DevExpress.Utils.PointFloat(0F, 60F);
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
            this.lblSign2Title.LocationFloat = new DevExpress.Utils.PointFloat(260F, 40F);
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
            this.lblSign2Sub.LocationFloat = new DevExpress.Utils.PointFloat(260F, 60F);
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
            this.lblSign3Title.LocationFloat = new DevExpress.Utils.PointFloat(520F, 40F);
            this.lblSign3Title.Name = "lblSign3Title";
            this.lblSign3Title.SizeF = new System.Drawing.SizeF(246.7717F, 20F);
            this.lblSign3Title.StylePriority.UseFont = false;
            this.lblSign3Title.StylePriority.UseTextAlignment = false;
            this.lblSign3Title.Text = "Thủ trưởng đơn vị";
            this.lblSign3Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Sub
            // 
            this.lblSign3Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign3Sub.LocationFloat = new DevExpress.Utils.PointFloat(520F, 60F);
            this.lblSign3Sub.Name = "lblSign3Sub";
            this.lblSign3Sub.SizeF = new System.Drawing.SizeF(246.7717F, 18F);
            this.lblSign3Sub.StylePriority.UseFont = false;
            this.lblSign3Sub.StylePriority.UseTextAlignment = false;
            this.lblSign3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSign3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Tên công ty";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "";
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
            this.prmCurrentTitle.ValueInfo = "Năm nay";
            this.prmCurrentTitle.Visible = false;
            // 
            // prmPrevTitle
            // 
            this.prmPrevTitle.Description = "Tiêu đề cột kỳ so sánh";
            this.prmPrevTitle.Name = "prmPrevTitle";
            this.prmPrevTitle.ValueInfo = "Năm trước";
            this.prmPrevTitle.Visible = false;
            // 
            // BusinessResult
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
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 35F);
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
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).EndInit();
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
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderCode;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderIllu;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderOtherAmount;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow tableRow2;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailCode;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailIllu;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailOtherAmount;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
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
        private DevExpress.XtraReports.Parameters.Parameter prmCurrentTitle;
        private DevExpress.XtraReports.Parameters.Parameter prmPrevTitle;
    }
}

