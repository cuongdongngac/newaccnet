namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset
{
    partial class AssetMovementReport
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
            DevExpress.XtraReports.UI.XRSummary xrSumAssetDr = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSumAssetCr = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSumReasonDr = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSumReasonCr = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSumGrandDr = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSumGrandCr = new DevExpress.XtraReports.UI.XRSummary();

            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblCompany = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAccount = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.headerRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_VoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_VoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Contents = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_CounterAccount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Quantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Price = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_DebitAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_CreditAmount = new DevExpress.XtraReports.UI.XRTableCell();

            // Group 1: Lý do tăng giảm (Reason)
            this.GroupHeaderReason = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tableReason = new DevExpress.XtraReports.UI.XRTable();
            this.rowReason = new DevExpress.XtraReports.UI.XRTableRow();
            this.tg1CellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.tg1CellReason = new DevExpress.XtraReports.UI.XRTableCell();

            // Group 2: Tài sản (Asset)
            this.GroupHeaderAsset = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.tableAsset = new DevExpress.XtraReports.UI.XRTable();
            this.rowAsset = new DevExpress.XtraReports.UI.XRTableRow();
            this.tg2CellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.tg2CellAsset = new DevExpress.XtraReports.UI.XRTableCell();

            // Detail
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellVoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellVoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellContents = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCounterAccount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellQuantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDebitAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCreditAmount = new DevExpress.XtraReports.UI.XRTableCell();

            // Group Footer 2: Cộng Tài sản
            this.GroupFooterAsset = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.tableFooterAsset = new DevExpress.XtraReports.UI.XRTable();
            this.rowFooterAsset = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellAssetFooterLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellAssetSumDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellAssetSumCredit = new DevExpress.XtraReports.UI.XRTableCell();

            // Group Footer 1: Tổng theo Lý do
            this.GroupFooterReason = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.tableFooterReason = new DevExpress.XtraReports.UI.XRTable();
            this.rowFooterReason = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellReasonFooterLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellReasonSumDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellReasonSumCredit = new DevExpress.XtraReports.UI.XRTableCell();

            // Report Footer
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.rfTable = new DevExpress.XtraReports.UI.XRTable();
            this.rfRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.rfLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfDebit = new DevExpress.XtraReports.UI.XRTableCell();
            this.rfCredit = new DevExpress.XtraReports.UI.XRTableCell();

            this.lblDateSig = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig3 = new DevExpress.XtraReports.UI.XRLabel();
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();

            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableAsset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooterAsset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooterReason)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rfTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // objectDataSource1
            this.objectDataSource1.DataSource = typeof(NewaccNet.Reports.AssetMovementReportDTO);
            this.objectDataSource1.Name = "objectDataSource1";

            // TopMargin & BottomMargin
            this.TopMargin.HeightF = 30F;
            this.TopMargin.Name = "TopMargin";
            this.BottomMargin.HeightF = 30F;
            this.BottomMargin.Name = "BottomMargin";

            // ReportHeader
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.lblCompany,
                this.lblTitle,
                this.lblAccount,
                this.lblDate});
            this.ReportHeader.HeightF = 115F;
            this.ReportHeader.Name = "ReportHeader";

            // lblCompany
            this.lblCompany.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblCompany.SizeF = new System.Drawing.SizeF(500F, 22F);
            this.lblCompany.Text = "";

            // lblTitle
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Segoe UI", 15F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(21, 101, 192);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 26F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(1100F, 32F);
            this.lblTitle.StylePriority.UseFont = false;
            this.lblTitle.StylePriority.UseForeColor = false;
            this.lblTitle.StylePriority.UseTextAlignment = false;
            this.lblTitle.Text = "BÁO CÁO BIẾN ĐỘNG TÀI SẢN CỐ ĐỊNH";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // lblAccount
            this.lblAccount.Font = new DevExpress.Drawing.DXFont("Segoe UI", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblAccount.ForeColor = System.Drawing.Color.FromArgb(51, 51, 51);
            this.lblAccount.LocationFloat = new DevExpress.Utils.PointFloat(0F, 60F);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblAccount.SizeF = new System.Drawing.SizeF(1100F, 22F);
            this.lblAccount.StylePriority.UseFont = false;
            this.lblAccount.StylePriority.UseForeColor = false;
            this.lblAccount.StylePriority.UseTextAlignment = false;
            this.lblAccount.Text = "Tài khoản: 211 - Tài sản cố định hữu hình";
            this.lblAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // lblDate
            this.lblDate.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(0F, 84F);
            this.lblDate.Name = "lblDate";
            this.lblDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblDate.SizeF = new System.Drawing.SizeF(1100F, 20F);
            this.lblDate.StylePriority.UseFont = false;
            this.lblDate.StylePriority.UseTextAlignment = false;
            this.lblDate.Text = "Từ ngày ... Đến ngày ...";
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // PageHeader
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.headerTable});
            this.PageHeader.HeightF = 32F;
            this.PageHeader.Name = "PageHeader";

            // headerTable
            this.headerTable.BackColor = System.Drawing.Color.FromArgb(230, 240, 250);
            this.headerTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.headerTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.headerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.headerTable.Name = "headerTable";
            this.headerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.headerRow});
            this.headerTable.SizeF = new System.Drawing.SizeF(1100F, 32F);
            this.headerTable.StylePriority.UseBackColor = false;
            this.headerTable.StylePriority.UseBorders = false;
            this.headerTable.StylePriority.UseFont = false;
            this.headerTable.StylePriority.UseTextAlignment = false;
            this.headerTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // headerRow (9 columns = 1100F)
            this.headerRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellH_Stt,
                this.cellH_VoucherDate,
                this.cellH_VoucherNo,
                this.cellH_Contents,
                this.cellH_CounterAccount,
                this.cellH_Quantity,
                this.cellH_Price,
                this.cellH_DebitAmount,
                this.cellH_CreditAmount});
            this.headerRow.Name = "headerRow";
            this.headerRow.Weight = 1D;

            this.cellH_Stt.Name = "cellH_Stt";
            this.cellH_Stt.Text = "STT";
            this.cellH_Stt.Weight = 45D;

            this.cellH_VoucherDate.Name = "cellH_VoucherDate";
            this.cellH_VoucherDate.Text = "Ngày CT";
            this.cellH_VoucherDate.Weight = 85D;

            this.cellH_VoucherNo.Name = "cellH_VoucherNo";
            this.cellH_VoucherNo.Text = "Số CT";
            this.cellH_VoucherNo.Weight = 90D;

            this.cellH_Contents.Name = "cellH_Contents";
            this.cellH_Contents.Text = "Diễn giải nội dung";
            this.cellH_Contents.Weight = 330D;

            this.cellH_CounterAccount.Name = "cellH_CounterAccount";
            this.cellH_CounterAccount.Text = "TK ĐƯ";
            this.cellH_CounterAccount.Weight = 60D;

            this.cellH_Quantity.Name = "cellH_Quantity";
            this.cellH_Quantity.Text = "Số lượng";
            this.cellH_Quantity.Weight = 60D;

            this.cellH_Price.Name = "cellH_Price";
            this.cellH_Price.Text = "Đơn giá / Nguyên giá";
            this.cellH_Price.Weight = 100D;

            this.cellH_DebitAmount.Name = "cellH_DebitAmount";
            this.cellH_DebitAmount.Text = "Ghi Nợ (Tăng)";
            this.cellH_DebitAmount.Weight = 165D;

            this.cellH_CreditAmount.Name = "cellH_CreditAmount";
            this.cellH_CreditAmount.Text = "Ghi Có (Giảm)";
            this.cellH_CreditAmount.Weight = 165D;

            // GroupHeaderReason (Level 1)
            this.GroupHeaderReason.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tableReason});
            this.GroupHeaderReason.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
                new DevExpress.XtraReports.UI.GroupField("ReasonId", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeaderReason.HeightF = 28F;
            this.GroupHeaderReason.Level = 1;
            this.GroupHeaderReason.Name = "GroupHeaderReason";
            this.GroupHeaderReason.BeforePrint += new DevExpress.XtraReports.UI.BeforePrintEventHandler(this.GroupHeaderReason_BeforePrint);

            // tableReason
            this.tableReason.BackColor = System.Drawing.Color.FromArgb(232, 238, 245);
            this.tableReason.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableReason.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableReason.Name = "tableReason";
            this.tableReason.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowReason});
            this.tableReason.SizeF = new System.Drawing.SizeF(1100F, 28F);
            this.tableReason.StylePriority.UseBackColor = false;
            this.tableReason.StylePriority.UseBorders = false;
            this.tableReason.StylePriority.UseTextAlignment = false;
            this.tableReason.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            this.rowReason.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.tg1CellStt,
                this.tg1CellReason});
            this.rowReason.Name = "rowReason";
            this.rowReason.Weight = 1D;

            this.tg1CellStt.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tg1CellStt.Name = "tg1CellStt";
            this.tg1CellStt.StylePriority.UseFont = false;
            this.tg1CellStt.StylePriority.UseTextAlignment = false;
            this.tg1CellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tg1CellStt.Weight = 45D;

            this.tg1CellReason.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Concat('LÝ DO: ', [ReasonDisplayName])")});
            this.tg1CellReason.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tg1CellReason.ForeColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.tg1CellReason.Name = "tg1CellReason";
            this.tg1CellReason.Padding = new DevExpress.XtraPrinting.PaddingInfo(6, 4, 0, 0, 100F);
            this.tg1CellReason.StylePriority.UseFont = false;
            this.tg1CellReason.StylePriority.UseForeColor = false;
            this.tg1CellReason.StylePriority.UsePadding = false;
            this.tg1CellReason.Weight = 1055D;

            // GroupHeaderAsset (Level 0)
            this.GroupHeaderAsset.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tableAsset});
            this.GroupHeaderAsset.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
                new DevExpress.XtraReports.UI.GroupField("AssetId", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeaderAsset.HeightF = 26F;
            this.GroupHeaderAsset.Level = 0;
            this.GroupHeaderAsset.Name = "GroupHeaderAsset";
            this.GroupHeaderAsset.BeforePrint += new DevExpress.XtraReports.UI.BeforePrintEventHandler(this.GroupHeaderAsset_BeforePrint);

            // tableAsset
            this.tableAsset.BackColor = System.Drawing.Color.FromArgb(247, 250, 252);
            this.tableAsset.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableAsset.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableAsset.Name = "tableAsset";
            this.tableAsset.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowAsset});
            this.tableAsset.SizeF = new System.Drawing.SizeF(1100F, 26F);
            this.tableAsset.StylePriority.UseBackColor = false;
            this.tableAsset.StylePriority.UseBorders = false;
            this.tableAsset.StylePriority.UseTextAlignment = false;
            this.tableAsset.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            this.rowAsset.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.tg2CellStt,
                this.tg2CellAsset});
            this.rowAsset.Name = "rowAsset";
            this.rowAsset.Weight = 1D;

            this.tg2CellStt.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tg2CellStt.Name = "tg2CellStt";
            this.tg2CellStt.StylePriority.UseFont = false;
            this.tg2CellStt.StylePriority.UseTextAlignment = false;
            this.tg2CellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tg2CellStt.Weight = 45D;

            this.tg2CellAsset.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tg2CellAsset.ForeColor = System.Drawing.Color.FromArgb(46, 125, 50);
            this.tg2CellAsset.Name = "tg2CellAsset";
            this.tg2CellAsset.Padding = new DevExpress.XtraPrinting.PaddingInfo(14, 4, 0, 0, 100F);
            this.tg2CellAsset.StylePriority.UseFont = false;
            this.tg2CellAsset.StylePriority.UseForeColor = false;
            this.tg2CellAsset.StylePriority.UsePadding = false;
            this.tg2CellAsset.Weight = 1055D;

            // DetailBand
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.detailTable});
            this.Detail.HeightF = 24F;
            this.Detail.Name = "Detail";
            this.Detail.BeforePrint += new DevExpress.XtraReports.UI.BeforePrintEventHandler(this.Detail_BeforePrint);

            // detailTable
            this.detailTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.detailTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.Name = "detailTable";
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.detailRow});
            this.detailTable.SizeF = new System.Drawing.SizeF(1100F, 24F);
            this.detailTable.StylePriority.UseBorders = false;
            this.detailTable.StylePriority.UseFont = false;
            this.detailTable.StylePriority.UseTextAlignment = false;
            this.detailTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // detailRow (9 columns = 1100F)
            this.detailRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellStt,
                this.cellVoucherDate,
                this.cellVoucherNo,
                this.cellContents,
                this.cellCounterAccount,
                this.cellQuantity,
                this.cellPrice,
                this.cellDebitAmount,
                this.cellCreditAmount});
            this.detailRow.Name = "detailRow";
            this.detailRow.Weight = 1D;

            // cellStt
            this.cellStt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellStt.Name = "cellStt";
            this.cellStt.StylePriority.UseTextAlignment = false;
            this.cellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellStt.Weight = 45D;

            // cellVoucherDate
            this.cellVoucherDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherDate]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellVoucherDate.Name = "cellVoucherDate";
            this.cellVoucherDate.StylePriority.UseTextAlignment = false;
            this.cellVoucherDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellVoucherDate.TextFormatString = "{0:dd/MM/yyyy}";
            this.cellVoucherDate.Weight = 85D;

            // cellVoucherNo
            this.cellVoucherNo.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherNo]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellVoucherNo.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellVoucherNo.ForeColor = System.Drawing.Color.FromArgb(21, 101, 192);
            this.cellVoucherNo.Name = "cellVoucherNo";
            this.cellVoucherNo.StylePriority.UseFont = false;
            this.cellVoucherNo.StylePriority.UseForeColor = false;
            this.cellVoucherNo.StylePriority.UseTextAlignment = false;
            this.cellVoucherNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellVoucherNo.Weight = 90D;

            // cellContents
            this.cellContents.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Contents]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellContents.Name = "cellContents";
            this.cellContents.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 2, 0, 0, 100F);
            this.cellContents.StylePriority.UsePadding = false;
            this.cellContents.Weight = 330D;

            // cellCounterAccount
            this.cellCounterAccount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountId]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCounterAccount.Name = "cellCounterAccount";
            this.cellCounterAccount.StylePriority.UseTextAlignment = false;
            this.cellCounterAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellCounterAccount.Weight = 60D;

            // cellQuantity
            this.cellQuantity.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Quantity]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellQuantity.Name = "cellQuantity";
            this.cellQuantity.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellQuantity.StylePriority.UsePadding = false;
            this.cellQuantity.StylePriority.UseTextAlignment = false;
            this.cellQuantity.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellQuantity.TextFormatString = "{0:#,##0.##}";
            this.cellQuantity.Weight = 60D;

            // cellPrice
            this.cellPrice.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Price]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellPrice.Name = "cellPrice";
            this.cellPrice.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellPrice.StylePriority.UsePadding = false;
            this.cellPrice.StylePriority.UseTextAlignment = false;
            this.cellPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellPrice.TextFormatString = "{0:#,##0}";
            this.cellPrice.Weight = 100D;

            // cellDebitAmount
            this.cellDebitAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitAmount]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellDebitAmount.Name = "cellDebitAmount";
            this.cellDebitAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellDebitAmount.StylePriority.UsePadding = false;
            this.cellDebitAmount.StylePriority.UseTextAlignment = false;
            this.cellDebitAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDebitAmount.TextFormatString = "{0:#,##0}";
            this.cellDebitAmount.Weight = 165D;

            // cellCreditAmount
            this.cellCreditAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditAmount]"),
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCreditAmount.Name = "cellCreditAmount";
            this.cellCreditAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellCreditAmount.StylePriority.UsePadding = false;
            this.cellCreditAmount.StylePriority.UseTextAlignment = false;
            this.cellCreditAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellCreditAmount.TextFormatString = "{0:#,##0}";
            this.cellCreditAmount.Weight = 165D;

            // GroupFooterAsset (Level 0)
            this.GroupFooterAsset.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tableFooterAsset});
            this.GroupFooterAsset.HeightF = 25F;
            this.GroupFooterAsset.Level = 0;
            this.GroupFooterAsset.Name = "GroupFooterAsset";

            // tableFooterAsset
            this.tableFooterAsset.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableFooterAsset.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableFooterAsset.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableFooterAsset.Name = "tableFooterAsset";
            this.tableFooterAsset.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowFooterAsset});
            this.tableFooterAsset.SizeF = new System.Drawing.SizeF(1100F, 25F);
            this.tableFooterAsset.StylePriority.UseBorders = false;
            this.tableFooterAsset.StylePriority.UseFont = false;
            this.tableFooterAsset.StylePriority.UseTextAlignment = false;
            this.tableFooterAsset.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;

            this.rowFooterAsset.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellAssetFooterLabel,
                this.cellAssetSumDebit,
                this.cellAssetSumCredit});
            this.rowFooterAsset.Name = "rowFooterAsset";
            this.rowFooterAsset.Weight = 1D;

            this.cellAssetFooterLabel.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, ((DevExpress.Drawing.DXFontStyle)((DevExpress.Drawing.DXFontStyle.Bold | DevExpress.Drawing.DXFontStyle.Italic))));
            this.cellAssetFooterLabel.Name = "cellAssetFooterLabel";
            this.cellAssetFooterLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 8, 0, 0, 100F);
            this.cellAssetFooterLabel.StylePriority.UseFont = false;
            this.cellAssetFooterLabel.StylePriority.UsePadding = false;
            this.cellAssetFooterLabel.Text = "Cộng tài sản:";
            this.cellAssetFooterLabel.Weight = 770D;

            this.cellAssetSumDebit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")});
            this.cellAssetSumDebit.Name = "cellAssetSumDebit";
            this.cellAssetSumDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellAssetSumDebit.StylePriority.UsePadding = false;
            xrSumAssetDr.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.cellAssetSumDebit.Summary = xrSumAssetDr;
            this.cellAssetSumDebit.TextFormatString = "{0:#,##0}";
            this.cellAssetSumDebit.Weight = 165D;

            this.cellAssetSumCredit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")});
            this.cellAssetSumCredit.Name = "cellAssetSumCredit";
            this.cellAssetSumCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellAssetSumCredit.StylePriority.UsePadding = false;
            xrSumAssetCr.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.cellAssetSumCredit.Summary = xrSumAssetCr;
            this.cellAssetSumCredit.TextFormatString = "{0:#,##0}";
            this.cellAssetSumCredit.Weight = 165D;

            // GroupFooterReason (Level 1)
            this.GroupFooterReason.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tableFooterReason});
            this.GroupFooterReason.HeightF = 26F;
            this.GroupFooterReason.Level = 1;
            this.GroupFooterReason.Name = "GroupFooterReason";

            // tableFooterReason
            this.tableFooterReason.BackColor = System.Drawing.Color.FromArgb(240, 244, 248);
            this.tableFooterReason.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.tableFooterReason.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableFooterReason.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableFooterReason.Name = "tableFooterReason";
            this.tableFooterReason.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowFooterReason});
            this.tableFooterReason.SizeF = new System.Drawing.SizeF(1100F, 26F);
            this.tableFooterReason.StylePriority.UseBackColor = false;
            this.tableFooterReason.StylePriority.UseBorders = false;
            this.tableFooterReason.StylePriority.UseFont = false;
            this.tableFooterReason.StylePriority.UseTextAlignment = false;
            this.tableFooterReason.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;

            this.rowFooterReason.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.cellReasonFooterLabel,
                this.cellReasonSumDebit,
                this.cellReasonSumCredit});
            this.rowFooterReason.Name = "rowFooterReason";
            this.rowFooterReason.Weight = 1D;

            this.cellReasonFooterLabel.Name = "cellReasonFooterLabel";
            this.cellReasonFooterLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 8, 0, 0, 100F);
            this.cellReasonFooterLabel.StylePriority.UsePadding = false;
            this.cellReasonFooterLabel.Text = "Tổng theo lý do:";
            this.cellReasonFooterLabel.Weight = 770D;

            this.cellReasonSumDebit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")});
            this.cellReasonSumDebit.Name = "cellReasonSumDebit";
            this.cellReasonSumDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellReasonSumDebit.StylePriority.UsePadding = false;
            xrSumReasonDr.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.cellReasonSumDebit.Summary = xrSumReasonDr;
            this.cellReasonSumDebit.TextFormatString = "{0:#,##0}";
            this.cellReasonSumDebit.Weight = 165D;

            this.cellReasonSumCredit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")});
            this.cellReasonSumCredit.Name = "cellReasonSumCredit";
            this.cellReasonSumCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.cellReasonSumCredit.StylePriority.UsePadding = false;
            xrSumReasonCr.Running = DevExpress.XtraReports.UI.SummaryRunning.Group;
            this.cellReasonSumCredit.Summary = xrSumReasonCr;
            this.cellReasonSumCredit.TextFormatString = "{0:#,##0}";
            this.cellReasonSumCredit.Weight = 165D;

            // ReportFooter
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.rfTable,
                this.lblDateSig,
                this.lblSig1,
                this.lblSig2,
                this.lblSig3});
            this.ReportFooter.HeightF = 140F;
            this.ReportFooter.Name = "ReportFooter";

            // rfTable
            this.rfTable.BackColor = System.Drawing.Color.FromArgb(235, 240, 248);
            this.rfTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.rfTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.rfTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.rfTable.Name = "rfTable";
            this.rfTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rfRow});
            this.rfTable.SizeF = new System.Drawing.SizeF(1100F, 30F);
            this.rfTable.StylePriority.UseBackColor = false;
            this.rfTable.StylePriority.UseBorders = false;
            this.rfTable.StylePriority.UseFont = false;
            this.rfTable.StylePriority.UseTextAlignment = false;
            this.rfTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;

            this.rfRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.rfLabel,
                this.rfDebit,
                this.rfCredit});
            this.rfRow.Name = "rfRow";
            this.rfRow.Weight = 1D;

            this.rfLabel.Name = "rfLabel";
            this.rfLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 8, 0, 0, 100F);
            this.rfLabel.StylePriority.UsePadding = false;
            this.rfLabel.Text = "TỔNG CỘNG TOÀN BỘ PHÁT SINH:";
            this.rfLabel.Weight = 770D;

            this.rfDebit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")});
            this.rfDebit.Name = "rfDebit";
            this.rfDebit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfDebit.StylePriority.UsePadding = false;
            xrSumGrandDr.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.rfDebit.Summary = xrSumGrandDr;
            this.rfDebit.TextFormatString = "{0:#,##0}";
            this.rfDebit.Weight = 165D;

            this.rfCredit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
                new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")});
            this.rfCredit.Name = "rfCredit";
            this.rfCredit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 4, 0, 0, 100F);
            this.rfCredit.StylePriority.UsePadding = false;
            xrSumGrandCr.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.rfCredit.Summary = xrSumGrandCr;
            this.rfCredit.TextFormatString = "{0:#,##0}";
            this.rfCredit.Weight = 165D;

            // Signatures
            this.lblDateSig.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDateSig.LocationFloat = new DevExpress.Utils.PointFloat(750F, 40F);
            this.lblDateSig.Name = "lblDateSig";
            this.lblDateSig.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblDateSig.SizeF = new System.Drawing.SizeF(350F, 20F);
            this.lblDateSig.StylePriority.UseFont = false;
            this.lblDateSig.StylePriority.UseTextAlignment = false;
            this.lblDateSig.Text = "Ngày ... tháng ... năm ...";
            this.lblDateSig.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            this.lblSig1.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig1.LocationFloat = new DevExpress.Utils.PointFloat(40F, 65F);
            this.lblSig1.Name = "lblSig1";
            this.lblSig1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblSig1.SizeF = new System.Drawing.SizeF(220F, 20F);
            this.lblSig1.StylePriority.UseFont = false;
            this.lblSig1.StylePriority.UseTextAlignment = false;
            this.lblSig1.Text = "Người lập biểu";
            this.lblSig1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            this.lblSig2.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig2.LocationFloat = new DevExpress.Utils.PointFloat(440F, 65F);
            this.lblSig2.Name = "lblSig2";
            this.lblSig2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblSig2.SizeF = new System.Drawing.SizeF(220F, 20F);
            this.lblSig2.StylePriority.UseFont = false;
            this.lblSig2.StylePriority.UseTextAlignment = false;
            this.lblSig2.Text = "Kế toán trưởng";
            this.lblSig2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            this.lblSig3.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig3.LocationFloat = new DevExpress.Utils.PointFloat(820F, 65F);
            this.lblSig3.Name = "lblSig3";
            this.lblSig3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblSig3.SizeF = new System.Drawing.SizeF(220F, 20F);
            this.lblSig3.StylePriority.UseFont = false;
            this.lblSig3.StylePriority.UseTextAlignment = false;
            this.lblSig3.Text = "Giám đốc";
            this.lblSig3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // Parameter
            this.prmCompanyName.Description = "Tên công ty";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.Visible = false;

            // Report Configuration (Landscape A4, Margins 30pt)
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.TopMargin,
                this.BottomMargin,
                this.ReportHeader,
                this.PageHeader,
                this.GroupHeaderReason,
                this.GroupHeaderAsset,
                this.Detail,
                this.GroupFooterAsset,
                this.GroupFooterReason,
                this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
                this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F);
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(30F, 30F, 30F, 30F);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
                this.prmCompanyName});
            this.Version = "24.1";

            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableReason)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableAsset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooterAsset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableFooterReason)).EndInit();
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
        private DevExpress.XtraReports.UI.XRLabel lblAccount;
        private DevExpress.XtraReports.UI.XRLabel lblDate;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow headerRow;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_VoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell cellH_VoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Contents;
        private DevExpress.XtraReports.UI.XRTableCell cellH_CounterAccount;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Quantity;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Price;
        private DevExpress.XtraReports.UI.XRTableCell cellH_DebitAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellH_CreditAmount;

        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeaderReason;
        private DevExpress.XtraReports.UI.XRTable tableReason;
        private DevExpress.XtraReports.UI.XRTableRow rowReason;
        private DevExpress.XtraReports.UI.XRTableCell tg1CellStt;
        private DevExpress.XtraReports.UI.XRTableCell tg1CellReason;

        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeaderAsset;
        private DevExpress.XtraReports.UI.XRTable tableAsset;
        private DevExpress.XtraReports.UI.XRTableRow rowAsset;
        private DevExpress.XtraReports.UI.XRTableCell tg2CellStt;
        private DevExpress.XtraReports.UI.XRTableCell tg2CellAsset;

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailRow;
        private DevExpress.XtraReports.UI.XRTableCell cellStt;
        private DevExpress.XtraReports.UI.XRTableCell cellVoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell cellVoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell cellContents;
        private DevExpress.XtraReports.UI.XRTableCell cellCounterAccount;
        private DevExpress.XtraReports.UI.XRTableCell cellQuantity;
        private DevExpress.XtraReports.UI.XRTableCell cellPrice;
        private DevExpress.XtraReports.UI.XRTableCell cellDebitAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellCreditAmount;

        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooterAsset;
        private DevExpress.XtraReports.UI.XRTable tableFooterAsset;
        private DevExpress.XtraReports.UI.XRTableRow rowFooterAsset;
        private DevExpress.XtraReports.UI.XRTableCell cellAssetFooterLabel;
        private DevExpress.XtraReports.UI.XRTableCell cellAssetSumDebit;
        private DevExpress.XtraReports.UI.XRTableCell cellAssetSumCredit;

        private DevExpress.XtraReports.UI.GroupFooterBand GroupFooterReason;
        private DevExpress.XtraReports.UI.XRTable tableFooterReason;
        private DevExpress.XtraReports.UI.XRTableRow rowFooterReason;
        private DevExpress.XtraReports.UI.XRTableCell cellReasonFooterLabel;
        private DevExpress.XtraReports.UI.XRTableCell cellReasonSumDebit;
        private DevExpress.XtraReports.UI.XRTableCell cellReasonSumCredit;

        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRTable rfTable;
        private DevExpress.XtraReports.UI.XRTableRow rfRow;
        private DevExpress.XtraReports.UI.XRTableCell rfLabel;
        private DevExpress.XtraReports.UI.XRTableCell rfDebit;
        private DevExpress.XtraReports.UI.XRTableCell rfCredit;

        private DevExpress.XtraReports.UI.XRLabel lblDateSig;
        private DevExpress.XtraReports.UI.XRLabel lblSig1;
        private DevExpress.XtraReports.UI.XRLabel lblSig2;
        private DevExpress.XtraReports.UI.XRLabel lblSig3;
        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
    }
}
