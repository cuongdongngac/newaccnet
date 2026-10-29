namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    partial class VatAdded
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
            this.lblNationalHeader = new DevExpress.XtraReports.UI.XRLabel();
            this.lblNationalMotto = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSubtitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTaxCode = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAddress = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCurrency = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHeaderItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderTurnover = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderVat = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.rowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellDetailItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailTurnover = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDetailVat = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.lblDateSignature = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmTaxCode = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmAddress = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmFromDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmToDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmReportTitle = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.VatDeclare.VatDeclareReportDTO);
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
            this.lblNationalHeader,
            this.lblNationalMotto,
            this.lblTitle,
            this.lblSubtitle,
            this.lblCompanyName,
            this.lblTaxCode,
            this.lblAddress,
            this.lblCurrency});
            this.ReportHeader.HeightF = 135F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblNationalHeader
            // 
            this.lblNationalHeader.Font = new DevExpress.Drawing.DXFont("Arial", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblNationalHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblNationalHeader.Name = "lblNationalHeader";
            this.lblNationalHeader.SizeF = new System.Drawing.SizeF(767F, 18F);
            this.lblNationalHeader.StylePriority.UseFont = false;
            this.lblNationalHeader.StylePriority.UseTextAlignment = false;
            this.lblNationalHeader.Text = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
            this.lblNationalHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblNationalMotto
            // 
            this.lblNationalMotto.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblNationalMotto.LocationFloat = new DevExpress.Utils.PointFloat(0F, 18F);
            this.lblNationalMotto.Name = "lblNationalMotto";
            this.lblNationalMotto.SizeF = new System.Drawing.SizeF(767F, 18F);
            this.lblNationalMotto.StylePriority.UseFont = false;
            this.lblNationalMotto.StylePriority.UseTextAlignment = false;
            this.lblNationalMotto.Text = "Độc lập - Tự do - Hạnh phúc";
            this.lblNationalMotto.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif(IsNullOrEmpty(?prmReportTitle), \'TỜ KHAI THUẾ VAT (MẪU SỐ 01/GTGT)\', ?prmRepo" +
                    "rtTitle)")});
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Arial", 13.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 42F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(767F, 24F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "TỜ KHAI THUẾ VAT (MẪU SỐ 01/GTGT)";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Từ ngày {0:d/M/yyyy} Đến ngày {1:d/M/yyyy}\', ?prmFromDate, ?prmToDa" +
                    "te)")});
            this.lblSubtitle.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSubtitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 68F);
            this.lblSubtitle.Multiline = true;
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSubtitle.SizeF = new System.Drawing.SizeF(767F, 18F);
            this.lblSubtitle.StylePriority.UseFont = false;
            this.lblSubtitle.StylePriority.UseTextAlignment = false;
            this.lblSubtitle.Text = "Từ ngày 1/1/2026 Đến ngày 31/12/2026";
            this.lblSubtitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Tên cơ sở kinh doanh: {0}\', ?prmCompanyName)")});
            this.lblCompanyName.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompanyName.LocationFloat = new DevExpress.Utils.PointFloat(0F, 92F);
            this.lblCompanyName.Multiline = true;
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblCompanyName.SizeF = new System.Drawing.SizeF(480F, 18F);
            this.lblCompanyName.StylePriority.UseFont = false;
            this.lblCompanyName.StylePriority.UseTextAlignment = false;
            this.lblCompanyName.Text = "Tên cơ sở kinh doanh: ";
            this.lblCompanyName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblTaxCode
            // 
            this.lblTaxCode.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Mã số thuế: {0}\', ?prmTaxCode)")});
            this.lblTaxCode.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTaxCode.LocationFloat = new DevExpress.Utils.PointFloat(490F, 92F);
            this.lblTaxCode.Multiline = true;
            this.lblTaxCode.Name = "lblTaxCode";
            this.lblTaxCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblTaxCode.SizeF = new System.Drawing.SizeF(277F, 18F);
            this.lblTaxCode.StylePriority.UseFont = false;
            this.lblTaxCode.StylePriority.UseTextAlignment = false;
            this.lblTaxCode.Text = "Mã số thuế: 010100101";
            this.lblTaxCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblAddress
            // 
            this.lblAddress.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Địa chỉ: {0}\', ?prmAddress)")});
            this.lblAddress.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblAddress.LocationFloat = new DevExpress.Utils.PointFloat(0F, 112F);
            this.lblAddress.Multiline = true;
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblAddress.SizeF = new System.Drawing.SizeF(550F, 18F);
            this.lblAddress.StylePriority.UseFont = false;
            this.lblAddress.StylePriority.UseTextAlignment = false;
            this.lblAddress.Text = "Địa chỉ: 57 Quang Trung - Hanoi";
            this.lblAddress.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblCurrency
            // 
            this.lblCurrency.Font = new DevExpress.Drawing.DXFont("Arial", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblCurrency.LocationFloat = new DevExpress.Utils.PointFloat(560F, 112F);
            this.lblCurrency.Multiline = true;
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblCurrency.SizeF = new System.Drawing.SizeF(207F, 18F);
            this.lblCurrency.StylePriority.UseFont = false;
            this.lblCurrency.StylePriority.UseTextAlignment = false;
            this.lblCurrency.Text = "Đơn vị tính: đồng Việt nam";
            this.lblCurrency.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
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
            this.tableHeader.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.rowHeader});
            this.tableHeader.SizeF = new System.Drawing.SizeF(767F, 30F);
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
            this.cellHeaderTurnover,
            this.cellHeaderVat});
            this.rowHeader.Name = "rowHeader";
            this.rowHeader.Weight = 1D;
            // 
            // cellHeaderItemName
            // 
            this.cellHeaderItemName.Multiline = true;
            this.cellHeaderItemName.Name = "cellHeaderItemName";
            this.cellHeaderItemName.Text = "Chỉ tiêu";
            this.cellHeaderItemName.Weight = 447D;
            // 
            // cellHeaderTurnover
            // 
            this.cellHeaderTurnover.Multiline = true;
            this.cellHeaderTurnover.Name = "cellHeaderTurnover";
            this.cellHeaderTurnover.Text = "Doanh số ( chưa có VAT)";
            this.cellHeaderTurnover.Weight = 160D;
            // 
            // cellHeaderVat
            // 
            this.cellHeaderVat.Multiline = true;
            this.cellHeaderVat.Name = "cellHeaderVat";
            this.cellHeaderVat.Text = "Thuế VAT";
            this.cellHeaderVat.Weight = 160D;
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
            this.cellDetailTurnover,
            this.cellDetailVat});
            this.rowDetail.Name = "rowDetail";
            this.rowDetail.Weight = 1D;
            // 
            // cellDetailItemName
            // 
            this.cellDetailItemName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ItemName]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[IsBold] == True"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Italic", "[IsItalic] == True")});
            this.cellDetailItemName.Multiline = true;
            this.cellDetailItemName.Name = "cellDetailItemName";
            this.cellDetailItemName.Padding = new DevExpress.XtraPrinting.PaddingInfo(6F, 2F, 0F, 0F, 100F);
            this.cellDetailItemName.StylePriority.UsePadding = false;
            this.cellDetailItemName.StylePriority.UseTextAlignment = false;
            this.cellDetailItemName.Text = "ItemName";
            this.cellDetailItemName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellDetailItemName.Weight = 447D;
            // 
            // cellDetailTurnover
            // 
            this.cellDetailTurnover.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TurnoverAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[IsBold] == True"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Italic", "[IsItalic] == True"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ForeColor", "Iif([TurnoverAmount] < 0, \'Red\', \'Black\')")});
            this.cellDetailTurnover.Name = "cellDetailTurnover";
            this.cellDetailTurnover.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailTurnover.StylePriority.UsePadding = false;
            this.cellDetailTurnover.StylePriority.UseTextAlignment = false;
            this.cellDetailTurnover.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailTurnover.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailTurnover.Weight = 160D;
            // 
            // cellDetailVat
            // 
            this.cellDetailVat.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VatAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Bold", "[IsBold] == True"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Font.Italic", "[IsItalic] == True"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "ForeColor", "Iif([VatAmount] < 0, \'Red\', \'Black\')")});
            this.cellDetailVat.Name = "cellDetailVat";
            this.cellDetailVat.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDetailVat.StylePriority.UsePadding = false;
            this.cellDetailVat.StylePriority.UseTextAlignment = false;
            this.cellDetailVat.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDetailVat.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDetailVat.Weight = 160D;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblDateSignature,
            this.lblSign1Title,
            this.lblSign1Sub,
            this.lblSign2Title,
            this.lblSign2Sub,
            this.lblSign3Title,
            this.lblSign3Sub});
            this.ReportFooter.HeightF = 120F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // lblDateSignature
            // 
            this.lblDateSignature.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateSignature.LocationFloat = new DevExpress.Utils.PointFloat(480F, 15F);
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
            this.lblSign1Title.LocationFloat = new DevExpress.Utils.PointFloat(0F, 40F);
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
            this.lblSign3Title.SizeF = new System.Drawing.SizeF(247F, 20F);
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
            this.lblSign3Sub.SizeF = new System.Drawing.SizeF(247F, 18F);
            this.lblSign3Sub.StylePriority.UseFont = false;
            this.lblSign3Sub.StylePriority.UseTextAlignment = false;
            this.lblSign3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSign3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Tên cơ sở kinh doanh";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "";
            this.prmCompanyName.Visible = false;
            // 
            // prmTaxCode
            // 
            this.prmTaxCode.Description = "Mã số thuế";
            this.prmTaxCode.Name = "prmTaxCode";
            this.prmTaxCode.ValueInfo = "010100101";
            this.prmTaxCode.Visible = false;
            // 
            // prmAddress
            // 
            this.prmAddress.Description = "Địa chỉ";
            this.prmAddress.Name = "prmAddress";
            this.prmAddress.ValueInfo = "57 Quang Trung - Hanoi";
            this.prmAddress.Visible = false;
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
            // prmReportTitle
            // 
            this.prmReportTitle.Description = "Tiêu đề tờ khai";
            this.prmReportTitle.Name = "prmReportTitle";
            this.prmReportTitle.ValueInfo = "TỜ KHAI THUẾ VAT (MẪU SỐ 01/GTGT)";
            this.prmReportTitle.Visible = false;
            // 
            // VatAdded
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
            this.Font = new DevExpress.Drawing.DXFont("Arial", 9F);
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 35F);
            this.PageHeightF = 1169F;
            this.PageWidthF = 827F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
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
        private DevExpress.XtraReports.UI.XRLabel lblNationalHeader;
        private DevExpress.XtraReports.UI.XRLabel lblNationalMotto;
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblSubtitle;
        private DevExpress.XtraReports.UI.XRLabel lblCompanyName;
        private DevExpress.XtraReports.UI.XRLabel lblTaxCode;
        private DevExpress.XtraReports.UI.XRLabel lblAddress;
        private DevExpress.XtraReports.UI.XRLabel lblCurrency;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable tableHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowHeader;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderTurnover;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderVat;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow rowDetail;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailItemName;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailTurnover;
        private DevExpress.XtraReports.UI.XRTableCell cellDetailVat;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel lblDateSignature;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Sub;
        public DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        public DevExpress.XtraReports.Parameters.Parameter prmTaxCode;
        public DevExpress.XtraReports.Parameters.Parameter prmAddress;
        public DevExpress.XtraReports.Parameters.Parameter prmFromDate;
        public DevExpress.XtraReports.Parameters.Parameter prmToDate;
        public DevExpress.XtraReports.Parameters.Parameter prmReportTitle;
    }
}

