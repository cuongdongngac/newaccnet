namespace NewaccNet.Wpf.AppSystem.Reports.Ledgers
{
    partial class LedgerSummary
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
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAccount = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDateRange = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDuDauKyTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDuDauKyValue = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDuCuoiKyTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDuCuoiKyValue = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellHeaderAccount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellHeaderCredit = new DevExpress.XtraReports.UI.XRTableCell();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellCounterAccountId = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCounterAccountName = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDebitAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCreditAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.tableFooter = new DevExpress.XtraReports.UI.XRTable();
            this.tableRowFooter = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellFooterTotalLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellFooterDebitTotal = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellFooterCreditTotal = new DevExpress.XtraReports.UI.XRTableCell();
            this.lblSignDate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Title = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3Sub = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmAccountID = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmAccountName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmBeginDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmEndDate = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(global::NewaccNet.Reports.LedgerReportDTO);
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
            this.lblAccount,
            this.lblDateRange,
            this.lblDuDauKyTitle,
            this.lblDuDauKyValue,
            this.lblDuCuoiKyTitle,
            this.lblDuCuoiKyValue});
            this.ReportHeader.HeightF = 150F;
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
            this.lblCompanyName.SizeF = new System.Drawing.SizeF(350F, 20F);
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
            this.lblTitle.SizeF = new System.Drawing.SizeF(766.7717F, 26F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "LIỆT KÊ TÀI KHOẢN ĐỐI ỨNG";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblAccount
            // 
            this.lblAccount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Tài khoản : {0} - {1}\', ?prmAccountID, ?prmAccountName)")});
            this.lblAccount.Font = new DevExpress.Drawing.DXFont("Arial", 11F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblAccount.LocationFloat = new DevExpress.Utils.PointFloat(0F, 50F);
            this.lblAccount.Multiline = true;
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblAccount.SizeF = new System.Drawing.SizeF(766.7717F, 22F);
            this.lblAccount.StylePriority.UseFont = false;
            this.lblAccount.StylePriority.UseTextAlignment = false;
            this.lblAccount.Text = "Tài khoản : 111 - Tiền";
            this.lblAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDateRange
            // 
            this.lblDateRange.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "FormatString(\'Từ ngày {0:d/M/yyyy} Đến ngày {1:d/M/yyyy}\', ?prmBeginDate, ?prmEndDate" +
                    ")")});
            this.lblDateRange.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateRange.LocationFloat = new DevExpress.Utils.PointFloat(0F, 72F);
            this.lblDateRange.Multiline = true;
            this.lblDateRange.Name = "lblDateRange";
            this.lblDateRange.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDateRange.SizeF = new System.Drawing.SizeF(766.7717F, 20F);
            this.lblDateRange.StylePriority.UseFont = false;
            this.lblDateRange.StylePriority.UseTextAlignment = false;
            this.lblDateRange.Text = "Từ ngày 01/01/2018 Đến ngày 31/12/2021";
            this.lblDateRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDuDauKyTitle
            // 
            this.lblDuDauKyTitle.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDuDauKyTitle.LocationFloat = new DevExpress.Utils.PointFloat(250F, 100F);
            this.lblDuDauKyTitle.Multiline = true;
            this.lblDuDauKyTitle.Name = "lblDuDauKyTitle";
            this.lblDuDauKyTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDuDauKyTitle.SizeF = new System.Drawing.SizeF(120F, 20F);
            this.lblDuDauKyTitle.StylePriority.UseFont = false;
            this.lblDuDauKyTitle.StylePriority.UseTextAlignment = false;
            this.lblDuDauKyTitle.Text = "Dư đầu kỳ:";
            this.lblDuDauKyTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblDuDauKyValue
            // 
            this.lblDuDauKyValue.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDuDauKyValue.LocationFloat = new DevExpress.Utils.PointFloat(370F, 100F);
            this.lblDuDauKyValue.Multiline = true;
            this.lblDuDauKyValue.Name = "lblDuDauKyValue";
            this.lblDuDauKyValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.lblDuDauKyValue.SizeF = new System.Drawing.SizeF(180F, 20F);
            this.lblDuDauKyValue.StylePriority.UseFont = false;
            this.lblDuDauKyValue.StylePriority.UsePadding = false;
            this.lblDuDauKyValue.StylePriority.UseTextAlignment = false;
            this.lblDuDauKyValue.Text = "0";
            this.lblDuDauKyValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // lblDuCuoiKyTitle
            // 
            this.lblDuCuoiKyTitle.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDuCuoiKyTitle.LocationFloat = new DevExpress.Utils.PointFloat(250F, 122F);
            this.lblDuCuoiKyTitle.Multiline = true;
            this.lblDuCuoiKyTitle.Name = "lblDuCuoiKyTitle";
            this.lblDuCuoiKyTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblDuCuoiKyTitle.SizeF = new System.Drawing.SizeF(120F, 20F);
            this.lblDuCuoiKyTitle.StylePriority.UseFont = false;
            this.lblDuCuoiKyTitle.StylePriority.UseTextAlignment = false;
            this.lblDuCuoiKyTitle.Text = "Dư cuối kỳ:";
            this.lblDuCuoiKyTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lblDuCuoiKyValue
            // 
            this.lblDuCuoiKyValue.Font = new DevExpress.Drawing.DXFont("Arial", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblDuCuoiKyValue.LocationFloat = new DevExpress.Utils.PointFloat(370F, 122F);
            this.lblDuCuoiKyValue.Multiline = true;
            this.lblDuCuoiKyValue.Name = "lblDuCuoiKyValue";
            this.lblDuCuoiKyValue.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.lblDuCuoiKyValue.SizeF = new System.Drawing.SizeF(180F, 20F);
            this.lblDuCuoiKyValue.StylePriority.UseFont = false;
            this.lblDuCuoiKyValue.StylePriority.UsePadding = false;
            this.lblDuCuoiKyValue.StylePriority.UseTextAlignment = false;
            this.lblDuCuoiKyValue.Text = "0";
            this.lblDuCuoiKyValue.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
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
            this.tableRowHeader});
            this.tableHeader.SizeF = new System.Drawing.SizeF(766.7717F, 28F);
            this.tableHeader.StylePriority.UseBorderColor = false;
            this.tableHeader.StylePriority.UseBorders = false;
            this.tableHeader.StylePriority.UseBorderWidth = false;
            this.tableHeader.StylePriority.UseFont = false;
            this.tableHeader.StylePriority.UseTextAlignment = false;
            this.tableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // tableRowHeader
            // 
            this.tableRowHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellHeaderAccount,
            this.cellHeaderDebit,
            this.cellHeaderCredit});
            this.tableRowHeader.Name = "tableRowHeader";
            this.tableRowHeader.Weight = 1D;
            // 
            // cellHeaderAccount
            // 
            this.cellHeaderAccount.Name = "cellHeaderAccount";
            this.cellHeaderAccount.Text = "Tài khoản đối ứng";
            this.cellHeaderAccount.Weight = 426.7717D;
            // 
            // cellHeaderDebit
            // 
            this.cellHeaderDebit.Name = "cellHeaderDebit";
            this.cellHeaderDebit.Text = "Phát sinh nợ";
            this.cellHeaderDebit.Weight = 170D;
            // 
            // cellHeaderCredit
            // 
            this.cellHeaderCredit.Name = "cellHeaderCredit";
            this.cellHeaderCredit.Text = "Phát sinh có";
            this.cellHeaderCredit.Weight = 170D;
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.tableDetail});
            this.Detail.HeightF = 22F;
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
            this.tableRowDetail});
            this.tableDetail.SizeF = new System.Drawing.SizeF(766.7717F, 22F);
            this.tableDetail.StylePriority.UseBorderColor = false;
            this.tableDetail.StylePriority.UseBorders = false;
            this.tableDetail.StylePriority.UseBorderWidth = false;
            this.tableDetail.StylePriority.UseFont = false;
            // 
            // tableRowDetail
            // 
            this.tableRowDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellCounterAccountId,
            this.cellCounterAccountName,
            this.cellDebitAmount,
            this.cellCreditAmount});
            this.tableRowDetail.Name = "tableRowDetail";
            this.tableRowDetail.Weight = 1D;
            // 
            // cellCounterAccountId
            // 
            this.cellCounterAccountId.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountId]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[CounterAccountId]")});
            this.cellCounterAccountId.Multiline = true;
            this.cellCounterAccountId.Name = "cellCounterAccountId";
            this.cellCounterAccountId.StylePriority.UseTextAlignment = false;
            this.cellCounterAccountId.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellCounterAccountId.Weight = 90D;
            // 
            // cellCounterAccountName
            // 
            this.cellCounterAccountName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountName]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[CounterAccountId]")});
            this.cellCounterAccountName.Multiline = true;
            this.cellCounterAccountName.Name = "cellCounterAccountName";
            this.cellCounterAccountName.Padding = new DevExpress.XtraPrinting.PaddingInfo(4F, 2F, 0F, 0F, 100F);
            this.cellCounterAccountName.StylePriority.UsePadding = false;
            this.cellCounterAccountName.StylePriority.UseTextAlignment = false;
            this.cellCounterAccountName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellCounterAccountName.Weight = 336.7717D;
            // 
            // cellDebitAmount
            // 
            this.cellDebitAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[CounterAccountId]")});
            this.cellDebitAmount.Multiline = true;
            this.cellDebitAmount.Name = "cellDebitAmount";
            this.cellDebitAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellDebitAmount.StylePriority.UsePadding = false;
            this.cellDebitAmount.StylePriority.UseTextAlignment = false;
            this.cellDebitAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDebitAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellDebitAmount.Weight = 170D;
            // 
            // cellCreditAmount
            // 
            this.cellCreditAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[CounterAccountId]")});
            this.cellCreditAmount.Multiline = true;
            this.cellCreditAmount.Name = "cellCreditAmount";
            this.cellCreditAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellCreditAmount.StylePriority.UsePadding = false;
            this.cellCreditAmount.StylePriority.UseTextAlignment = false;
            this.cellCreditAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellCreditAmount.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellCreditAmount.Weight = 170D;
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
            this.ReportFooter.HeightF = 155F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // tableFooter
            // 
            this.tableFooter.BorderColor = System.Drawing.Color.LightGray;
            this.tableFooter.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableFooter.BorderWidth = 1F;
            this.tableFooter.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableFooter.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableFooter.Name = "tableFooter";
            this.tableFooter.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.tableFooter.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.tableRowFooter});
            this.tableFooter.SizeF = new System.Drawing.SizeF(766.7717F, 25F);
            this.tableFooter.StylePriority.UseBorderColor = false;
            this.tableFooter.StylePriority.UseBorders = false;
            this.tableFooter.StylePriority.UseBorderWidth = false;
            this.tableFooter.StylePriority.UseFont = false;
            // 
            // tableRowFooter
            // 
            this.tableRowFooter.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellFooterTotalLabel,
            this.cellFooterDebitTotal,
            this.cellFooterCreditTotal});
            this.tableRowFooter.Name = "tableRowFooter";
            this.tableRowFooter.Weight = 1D;
            // 
            // cellFooterTotalLabel
            // 
            this.cellFooterTotalLabel.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellFooterTotalLabel.Multiline = true;
            this.cellFooterTotalLabel.Name = "cellFooterTotalLabel";
            this.cellFooterTotalLabel.StylePriority.UseFont = false;
            this.cellFooterTotalLabel.StylePriority.UseTextAlignment = false;
            this.cellFooterTotalLabel.Text = "Cộng:";
            this.cellFooterTotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellFooterTotalLabel.Weight = 426.7717D;
            // 
            // cellFooterDebitTotal
            // 
            this.cellFooterDebitTotal.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")});
            this.cellFooterDebitTotal.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellFooterDebitTotal.Multiline = true;
            this.cellFooterDebitTotal.Name = "cellFooterDebitTotal";
            this.cellFooterDebitTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellFooterDebitTotal.StylePriority.UseFont = false;
            this.cellFooterDebitTotal.StylePriority.UsePadding = false;
            this.cellFooterDebitTotal.StylePriority.UseTextAlignment = false;
            xrSummaryDebit.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellFooterDebitTotal.Summary = xrSummaryDebit;
            this.cellFooterDebitTotal.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellFooterDebitTotal.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellFooterDebitTotal.Weight = 170D;
            // 
            // cellFooterCreditTotal
            // 
            this.cellFooterCreditTotal.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")});
            this.cellFooterCreditTotal.Font = new DevExpress.Drawing.DXFont("Arial", 8.75F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellFooterCreditTotal.Multiline = true;
            this.cellFooterCreditTotal.Name = "cellFooterCreditTotal";
            this.cellFooterCreditTotal.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 4F, 0F, 0F, 100F);
            this.cellFooterCreditTotal.StylePriority.UseFont = false;
            this.cellFooterCreditTotal.StylePriority.UsePadding = false;
            this.cellFooterCreditTotal.StylePriority.UseTextAlignment = false;
            xrSummaryCredit.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellFooterCreditTotal.Summary = xrSummaryCredit;
            this.cellFooterCreditTotal.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellFooterCreditTotal.TextFormatString = "{0:#,##0;(#,##0);\"\"}";
            this.cellFooterCreditTotal.Weight = 170D;
            // 
            // lblSignDate
            // 
            this.lblSignDate.Font = new DevExpress.Drawing.DXFont("Arial", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSignDate.LocationFloat = new DevExpress.Utils.PointFloat(466.7717F, 35F);
            this.lblSignDate.Multiline = true;
            this.lblSignDate.Name = "lblSignDate";
            this.lblSignDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2F, 2F, 0F, 0F, 100F);
            this.lblSignDate.SizeF = new System.Drawing.SizeF(300F, 20F);
            this.lblSignDate.StylePriority.UseFont = false;
            this.lblSignDate.StylePriority.UseTextAlignment = false;
            this.lblSignDate.Text = "Ngày ..... tháng ..... năm .....";
            this.lblSignDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
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
            this.lblSign3Title.SizeF = new System.Drawing.SizeF(246.7717F, 20F);
            this.lblSign3Title.StylePriority.UseFont = false;
            this.lblSign3Title.StylePriority.UseTextAlignment = false;
            this.lblSign3Title.Text = "Giám đốc";
            this.lblSign3Title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblSign3Sub
            // 
            this.lblSign3Sub.Font = new DevExpress.Drawing.DXFont("Arial", 8.25F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSign3Sub.LocationFloat = new DevExpress.Utils.PointFloat(520F, 80F);
            this.lblSign3Sub.Name = "lblSign3Sub";
            this.lblSign3Sub.SizeF = new System.Drawing.SizeF(246.7717F, 18F);
            this.lblSign3Sub.StylePriority.UseFont = false;
            this.lblSign3Sub.StylePriority.UseTextAlignment = false;
            this.lblSign3Sub.Text = "(Ký, họ tên, đóng dấu)";
            this.lblSign3Sub.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Name of Company";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "";
            this.prmCompanyName.Visible = false;
            // 
            // prmAccountID
            // 
            this.prmAccountID.Description = "Mã tài khoản";
            this.prmAccountID.Name = "prmAccountID";
            this.prmAccountID.ValueInfo = "111";
            this.prmAccountID.Visible = false;
            // 
            // prmAccountName
            // 
            this.prmAccountName.Description = "Name of Account";
            this.prmAccountName.Name = "prmAccountName";
            this.prmAccountName.ValueInfo = "Tiền";
            this.prmAccountName.Visible = false;
            // 
            // prmBeginDate
            // 
            this.prmBeginDate.Description = "BeginDate";
            this.prmBeginDate.Name = "prmBeginDate";
            this.prmBeginDate.Type = typeof(global::System.DateTime);
            this.prmBeginDate.ValueInfo = "2026-01-01";
            this.prmBeginDate.Visible = false;
            // 
            // prmEndDate
            // 
            this.prmEndDate.Description = "EndDate";
            this.prmEndDate.Name = "prmEndDate";
            this.prmEndDate.Type = typeof(global::System.DateTime);
            this.prmEndDate.ValueInfo = "2026-12-31";
            this.prmEndDate.Visible = false;
            // 
            // LedgerSummary
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
            this.Landscape = false;
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 30F);
            this.PageHeightF = 1169.291F;
            this.PageWidthF = 826.7717F;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName,
            this.prmAccountID,
            this.prmAccountName,
            this.prmBeginDate,
            this.prmEndDate});
            this.Version = "25.2";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooter)).EndInit();
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
        private DevExpress.XtraReports.UI.XRLabel lblAccount;
        private DevExpress.XtraReports.UI.XRLabel lblDateRange;
        private DevExpress.XtraReports.UI.XRLabel lblDuDauKyTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDuDauKyValue;
        private DevExpress.XtraReports.UI.XRLabel lblDuCuoiKyTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDuCuoiKyValue;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable tableHeader;
        private DevExpress.XtraReports.UI.XRTableRow tableRowHeader;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderAccount;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderDebit;
        private DevExpress.XtraReports.UI.XRTableCell cellHeaderCredit;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow tableRowDetail;
        private DevExpress.XtraReports.UI.XRTableCell cellCounterAccountId;
        private DevExpress.XtraReports.UI.XRTableCell cellCounterAccountName;
        private DevExpress.XtraReports.UI.XRTableCell cellDebitAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellCreditAmount;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable tableFooter;
        private DevExpress.XtraReports.UI.XRTableRow tableRowFooter;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterTotalLabel;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterDebitTotal;
        private DevExpress.XtraReports.UI.XRTableCell cellFooterCreditTotal;
        private DevExpress.XtraReports.UI.XRLabel lblSignDate;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign1Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign2Sub;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Title;
        private DevExpress.XtraReports.UI.XRLabel lblSign3Sub;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        private DevExpress.XtraReports.Parameters.Parameter prmAccountID;
        private DevExpress.XtraReports.Parameters.Parameter prmAccountName;
        private DevExpress.XtraReports.Parameters.Parameter prmBeginDate;
        private DevExpress.XtraReports.Parameters.Parameter prmEndDate;
    }
}

