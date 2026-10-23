namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Material
{
    partial class MaterialDetailReport
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

        private void InitializeComponent()
        {
            DevExpress.XtraReports.UI.XRSummary xrSummaryStt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryInQty = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryInAmt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryOutQty = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryOutAmt = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryEndQty = new DevExpress.XtraReports.UI.XRSummary();
            DevExpress.XtraReports.UI.XRSummary xrSummaryEndAmt = new DevExpress.XtraReports.UI.XRSummary();

            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();

            // Report Header Controls
            this.lblCompany = new DevExpress.XtraReports.UI.XRLabel();
            this.lblFormInfo = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPeriod = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAccount = new DevExpress.XtraReports.UI.XRLabel();
            this.lblWarehouse = new DevExpress.XtraReports.UI.XRLabel();
            this.lblMaterial = new DevExpress.XtraReports.UI.XRLabel();

            // Page Header Table
            this.headerTable = new DevExpress.XtraReports.UI.XRTable();
            this.rowH1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_VoucherGroup = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Contents = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_CounterAcc = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Price = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InGroup = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutGroup = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_ClosingGroup = new DevExpress.XtraReports.UI.XRTableCell();

            this.rowH2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellH_Stt_Spacer = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Contents_Spacer = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_CounterAcc_Spacer = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Price_Spacer = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_Date = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_No = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_InAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_OutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellH_EndAmt = new DevExpress.XtraReports.UI.XRTableCell();

            // Detail Table
            this.detailTable = new DevExpress.XtraReports.UI.XRTable();
            this.detailRow = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellStt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellVoucherDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellVoucherNo = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellContents = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCounterAccount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDebitQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellDebitAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCreditQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellCreditAmount = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellClosingQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellClosingAmount = new DevExpress.XtraReports.UI.XRTableCell();

            // Footer Table
            this.footerTable = new DevExpress.XtraReports.UI.XRTable();
            this.footerRowTotal = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellF_Label = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_InQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_InAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_OutQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_OutAmt = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_EndQtyBlank = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellF_EndAmtBlank = new DevExpress.XtraReports.UI.XRTableCell();

            this.footerRowClosing = new DevExpress.XtraReports.UI.XRTableRow();
            this.cellC_Label = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellC_EndQty = new DevExpress.XtraReports.UI.XRTableCell();
            this.cellC_EndAmt = new DevExpress.XtraReports.UI.XRTableCell();

            // Signatures
            this.lblSig1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSig3 = new DevExpress.XtraReports.UI.XRLabel();

            // Parameters
            this.prmCompanyName = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmPeriodTitle = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmAccountInfo = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmWarehouseInfo = new DevExpress.XtraReports.Parameters.Parameter();
            this.prmMaterialInfo = new DevExpress.XtraReports.Parameters.Parameter();

            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.footerTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 35F;
            this.TopMargin.Name = "TopMargin";

            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Name = "BottomMargin";

            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblCompany,
            this.lblFormInfo,
            this.lblTitle,
            this.lblPeriod,
            this.lblAccount,
            this.lblWarehouse,
            this.lblMaterial});
            this.ReportHeader.HeightF = 135F;
            this.ReportHeader.Name = "ReportHeader";

            // 
            // lblCompany
            // 
            this.lblCompany.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmCompanyName")});
            this.lblCompany.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.SizeF = new System.Drawing.SizeF(600F, 20F);
            this.lblCompany.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // 
            // lblFormInfo
            // 
            this.lblFormInfo.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblFormInfo.LocationFloat = new DevExpress.Utils.PointFloat(800F, 0F);
            this.lblFormInfo.Multiline = true;
            this.lblFormInfo.Name = "lblFormInfo";
            this.lblFormInfo.SizeF = new System.Drawing.SizeF(300F, 30F);
            this.lblFormInfo.Text = "Mẫu số S10-DN\n(Ban hành theo TT số 200/2014/TT-BTC)";
            this.lblFormInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;

            // 
            // lblTitle
            // 
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Segoe UI", 15F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(0F, 28F);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.SizeF = new System.Drawing.SizeF(1100F, 28F);
            this.lblTitle.Text = "SỔ CHI TIẾT VẬT TƯ, DỤNG CỤ, SẢN PHẨM, HÀNG HÓA";
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblPeriod
            // 
            this.lblPeriod.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmPeriodTitle")});
            this.lblPeriod.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblPeriod.LocationFloat = new DevExpress.Utils.PointFloat(0F, 56F);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.SizeF = new System.Drawing.SizeF(1100F, 20F);
            this.lblPeriod.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblAccount
            // 
            this.lblAccount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmAccountInfo")});
            this.lblAccount.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblAccount.LocationFloat = new DevExpress.Utils.PointFloat(0F, 82F);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.SizeF = new System.Drawing.SizeF(550F, 20F);
            this.lblAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            // 
            // lblWarehouse
            // 
            this.lblWarehouse.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmWarehouseInfo")});
            this.lblWarehouse.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblWarehouse.LocationFloat = new DevExpress.Utils.PointFloat(550F, 82F);
            this.lblWarehouse.Name = "lblWarehouse";
            this.lblWarehouse.SizeF = new System.Drawing.SizeF(550F, 20F);
            this.lblWarehouse.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;

            // 
            // lblMaterial
            // 
            this.lblMaterial.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?prmMaterialInfo")});
            this.lblMaterial.Font = new DevExpress.Drawing.DXFont("Segoe UI", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblMaterial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblMaterial.LocationFloat = new DevExpress.Utils.PointFloat(0F, 105F);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.SizeF = new System.Drawing.SizeF(1100F, 22F);
            this.lblMaterial.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

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
            this.headerTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(244)))), ((int)(((byte)(248)))));
            this.headerTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.headerTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.headerTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
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
            this.cellH_VoucherGroup,
            this.cellH_Contents,
            this.cellH_CounterAcc,
            this.cellH_Price,
            this.cellH_InGroup,
            this.cellH_OutGroup,
            this.cellH_ClosingGroup});
            this.rowH1.Name = "rowH1";
            this.rowH1.Weight = 0.5D;

            // cellH_Stt
            this.cellH_Stt.Name = "cellH_Stt";
            this.cellH_Stt.RowSpan = 2;
            this.cellH_Stt.Text = "STT";
            this.cellH_Stt.Weight = 0.036363636363636362D;

            // cellH_VoucherGroup
            this.cellH_VoucherGroup.Name = "cellH_VoucherGroup";
            this.cellH_VoucherGroup.Text = "Chứng từ";
            this.cellH_VoucherGroup.Weight = 0.14545454545454545D;

            // cellH_Contents
            this.cellH_Contents.Name = "cellH_Contents";
            this.cellH_Contents.RowSpan = 2;
            this.cellH_Contents.Text = "Diễn giải";
            this.cellH_Contents.Weight = 0.19090909090909092D;

            // cellH_CounterAcc
            this.cellH_CounterAcc.Name = "cellH_CounterAcc";
            this.cellH_CounterAcc.RowSpan = 2;
            this.cellH_CounterAcc.Text = "TK ĐƯ";
            this.cellH_CounterAcc.Weight = 0.045454545454545456D;

            // cellH_Price
            this.cellH_Price.Name = "cellH_Price";
            this.cellH_Price.RowSpan = 2;
            this.cellH_Price.Text = "Đơn giá";
            this.cellH_Price.Weight = 0.063636363636363637D;

            // cellH_InGroup
            this.cellH_InGroup.Name = "cellH_InGroup";
            this.cellH_InGroup.Text = "Nhập";
            this.cellH_InGroup.Weight = 0.16818181818181818D;

            // cellH_OutGroup
            this.cellH_OutGroup.Name = "cellH_OutGroup";
            this.cellH_OutGroup.Text = "Xuất";
            this.cellH_OutGroup.Weight = 0.16818181818181818D;

            // cellH_ClosingGroup
            this.cellH_ClosingGroup.Name = "cellH_ClosingGroup";
            this.cellH_ClosingGroup.Text = "Tồn lũy kế";
            this.cellH_ClosingGroup.Weight = 0.18181818181818182D;

            // 
            // rowH2
            // 
            this.rowH2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellH_Stt_Spacer,
            this.cellH_Date,
            this.cellH_No,
            this.cellH_Contents_Spacer,
            this.cellH_CounterAcc_Spacer,
            this.cellH_Price_Spacer,
            this.cellH_InQty,
            this.cellH_InAmt,
            this.cellH_OutQty,
            this.cellH_OutAmt,
            this.cellH_EndQty,
            this.cellH_EndAmt});
            this.rowH2.Name = "rowH2";
            this.rowH2.Weight = 0.5D;

            // cellH_Stt_Spacer
            this.cellH_Stt_Spacer.Name = "cellH_Stt_Spacer";
            this.cellH_Stt_Spacer.Weight = 0.036363636363636362D;

            // cellH_Date
            this.cellH_Date.Name = "cellH_Date";
            this.cellH_Date.Text = "Ngày CT";
            this.cellH_Date.Weight = 0.068181818181818177D;

            // cellH_No
            this.cellH_No.Name = "cellH_No";
            this.cellH_No.Text = "Số CT / Phiếu";
            this.cellH_No.Weight = 0.07727272727272727D;

            // cellH_Contents_Spacer
            this.cellH_Contents_Spacer.Name = "cellH_Contents_Spacer";
            this.cellH_Contents_Spacer.Weight = 0.19090909090909092D;

            // cellH_CounterAcc_Spacer
            this.cellH_CounterAcc_Spacer.Name = "cellH_CounterAcc_Spacer";
            this.cellH_CounterAcc_Spacer.Weight = 0.045454545454545456D;

            // cellH_Price_Spacer
            this.cellH_Price_Spacer.Name = "cellH_Price_Spacer";
            this.cellH_Price_Spacer.Weight = 0.063636363636363637D;

            // cellH_InQty
            this.cellH_InQty.Name = "cellH_InQty";
            this.cellH_InQty.Text = "Số lượng";
            this.cellH_InQty.Weight = 0.068181818181818177D;

            // cellH_InAmt
            this.cellH_InAmt.Name = "cellH_InAmt";
            this.cellH_InAmt.Text = "Thành tiền";
            this.cellH_InAmt.Weight = 0.1D;

            // cellH_OutQty
            this.cellH_OutQty.Name = "cellH_OutQty";
            this.cellH_OutQty.Text = "Số lượng";
            this.cellH_OutQty.Weight = 0.068181818181818177D;

            // cellH_OutAmt
            this.cellH_OutAmt.Name = "cellH_OutAmt";
            this.cellH_OutAmt.Text = "Thành tiền";
            this.cellH_OutAmt.Weight = 0.1D;

            // cellH_EndQty
            this.cellH_EndQty.Name = "cellH_EndQty";
            this.cellH_EndQty.Text = "Số lượng";
            this.cellH_EndQty.Weight = 0.072727272727272724D;

            // cellH_EndAmt
            this.cellH_EndAmt.Name = "cellH_EndAmt";
            this.cellH_EndAmt.Text = "Thành tiền";
            this.cellH_EndAmt.Weight = 0.10909090909090909D;

            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.detailTable});
            this.Detail.HeightF = 24F;
            this.Detail.Name = "Detail";

            // 
            // detailTable
            // 
            this.detailTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.detailTable.Borders = ((DevExpress.XtraPrinting.BorderSide)(((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.detailTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F);
            this.detailTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.detailTable.Name = "detailTable";
            this.detailTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3F, 3F, 0F, 0F, 100F);
            this.detailTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.detailRow});
            this.detailTable.SizeF = new System.Drawing.SizeF(1100F, 24F);

            // 
            // detailRow
            // 
            this.detailRow.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellStt,
            this.cellVoucherDate,
            this.cellVoucherNo,
            this.cellContents,
            this.cellCounterAccount,
            this.cellPrice,
            this.cellDebitQty,
            this.cellDebitAmount,
            this.cellCreditQty,
            this.cellCreditAmount,
            this.cellClosingQty,
            this.cellClosingAmount});
            this.detailRow.Name = "detailRow";
            this.detailRow.Weight = 1D;

            // cellStt
            this.cellStt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([IsOpeningRow], \'\', sumRecordNumber())"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellStt.Name = "cellStt";
            xrSummaryStt.Func = DevExpress.XtraReports.UI.SummaryFunc.RecordNumber;
            xrSummaryStt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellStt.Summary = xrSummaryStt;
            this.cellStt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellStt.Weight = 0.036363636363636362D;

            // cellVoucherDate
            this.cellVoucherDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherDate]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellVoucherDate.Name = "cellVoucherDate";
            this.cellVoucherDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellVoucherDate.TextFormatString = "{0:dd/MM/yyyy}";
            this.cellVoucherDate.Weight = 0.068181818181818177D;

            // cellVoucherNo
            this.cellVoucherNo.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[VoucherNo]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellVoucherNo.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.cellVoucherNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.cellVoucherNo.Name = "cellVoucherNo";
            this.cellVoucherNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellVoucherNo.Weight = 0.07727272727272727D;

            // cellContents
            this.cellContents.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Contents]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellContents.Name = "cellContents";
            this.cellContents.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.cellContents.Weight = 0.19090909090909092D;

            // cellCounterAccount
            this.cellCounterAccount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CounterAccountId]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCounterAccount.Name = "cellCounterAccount";
            this.cellCounterAccount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellCounterAccount.Weight = 0.045454545454545456D;

            // cellPrice
            this.cellPrice.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Price]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellPrice.Name = "cellPrice";
            this.cellPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellPrice.TextFormatString = "{0:#,##0.##;-#,##0.##;''}";
            this.cellPrice.Weight = 0.063636363636363637D;

            // cellDebitQty
            this.cellDebitQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitQty]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellDebitQty.Name = "cellDebitQty";
            this.cellDebitQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDebitQty.TextFormatString = "{0:#,##0.##;-#,##0.##;''}";
            this.cellDebitQty.Weight = 0.068181818181818177D;

            // cellDebitAmount
            this.cellDebitAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[DebitAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellDebitAmount.Name = "cellDebitAmount";
            this.cellDebitAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellDebitAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellDebitAmount.Weight = 0.1D;

            // cellCreditQty
            this.cellCreditQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditQty]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCreditQty.Name = "cellCreditQty";
            this.cellCreditQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellCreditQty.TextFormatString = "{0:#,##0.##;-#,##0.##;''}";
            this.cellCreditQty.Weight = 0.068181818181818177D;

            // cellCreditAmount
            this.cellCreditAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CreditAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellCreditAmount.Name = "cellCreditAmount";
            this.cellCreditAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellCreditAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellCreditAmount.Weight = 0.1D;

            // cellClosingQty
            this.cellClosingQty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingQty]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellClosingQty.Name = "cellClosingQty";
            this.cellClosingQty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellClosingQty.TextFormatString = "{0:#,##0.##;-#,##0.##;''}";
            this.cellClosingQty.Weight = 0.072727272727272724D;

            // cellClosingAmount
            this.cellClosingAmount.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingAmount]"),
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]")});
            this.cellClosingAmount.Name = "cellClosingAmount";
            this.cellClosingAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellClosingAmount.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellClosingAmount.Weight = 0.10909090909090909D;

            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.footerTable,
            this.lblSig1,
            this.lblSig2,
            this.lblSig3});
            this.ReportFooter.HeightF = 140F;
            this.ReportFooter.Name = "ReportFooter";

            // 
            // footerTable
            // 
            this.footerTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.footerTable.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.footerTable.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.footerTable.Font = new DevExpress.Drawing.DXFont("Segoe UI", 8.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.footerTable.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.footerTable.Name = "footerTable";
            this.footerTable.Padding = new DevExpress.XtraPrinting.PaddingInfo(3F, 3F, 0F, 0F, 100F);
            this.footerTable.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.footerRowTotal,
            this.footerRowClosing});
            this.footerTable.SizeF = new System.Drawing.SizeF(1100F, 50F);

            // 
            // footerRowTotal
            // 
            this.footerRowTotal.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellF_Label,
            this.cellF_InQty,
            this.cellF_InAmt,
            this.cellF_OutQty,
            this.cellF_OutAmt,
            this.cellF_EndQtyBlank,
            this.cellF_EndAmtBlank});
            this.footerRowTotal.Name = "footerRowTotal";
            this.footerRowTotal.Weight = 0.5D;

            // cellF_Label
            this.cellF_Label.Name = "cellF_Label";
            this.cellF_Label.Text = "Cộng phát sinh trong kỳ";
            this.cellF_Label.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellF_Label.Weight = 0.48181818181818182D;

            // cellF_InQty
            this.cellF_InQty.Name = "cellF_InQty";
            this.cellF_InQty.Text = string.Empty;
            this.cellF_InQty.Weight = 0.068181818181818177D;

            // cellF_InAmt
            this.cellF_InAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([DebitAmount])")});
            this.cellF_InAmt.Name = "cellF_InAmt";
            xrSummaryInAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellF_InAmt.Summary = xrSummaryInAmt;
            this.cellF_InAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellF_InAmt.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellF_InAmt.Weight = 0.1D;

            // cellF_OutQty
            this.cellF_OutQty.Name = "cellF_OutQty";
            this.cellF_OutQty.Text = string.Empty;
            this.cellF_OutQty.Weight = 0.068181818181818177D;

            // cellF_OutAmt
            this.cellF_OutAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "sumSum([CreditAmount])")});
            this.cellF_OutAmt.Name = "cellF_OutAmt";
            xrSummaryOutAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellF_OutAmt.Summary = xrSummaryOutAmt;
            this.cellF_OutAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellF_OutAmt.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellF_OutAmt.Weight = 0.1D;

            // cellF_EndQtyBlank
            this.cellF_EndQtyBlank.Name = "cellF_EndQtyBlank";
            this.cellF_EndQtyBlank.Weight = 0.072727272727272724D;

            // cellF_EndAmtBlank
            this.cellF_EndAmtBlank.Name = "cellF_EndAmtBlank";
            this.cellF_EndAmtBlank.Weight = 0.10909090909090909D;

            // 
            // footerRowClosing
            // 
            this.footerRowClosing.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.cellC_Label,
            this.cellC_EndQty,
            this.cellC_EndAmt});
            this.footerRowClosing.Name = "footerRowClosing";
            this.footerRowClosing.Weight = 0.5D;

            // cellC_Label
            this.cellC_Label.Name = "cellC_Label";
            this.cellC_Label.Text = "Số dư cuối kỳ";
            this.cellC_Label.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.cellC_Label.Weight = 0.81818181818181823D;

            // cellC_EndQty
            this.cellC_EndQty.Name = "cellC_EndQty";
            this.cellC_EndQty.Text = string.Empty;
            this.cellC_EndQty.Weight = 0.072727272727272724D;

            // cellC_EndAmt
            this.cellC_EndAmt.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ClosingAmount]")});
            this.cellC_EndAmt.Name = "cellC_EndAmt";
            xrSummaryEndAmt.Func = DevExpress.XtraReports.UI.SummaryFunc.Custom;
            xrSummaryEndAmt.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
            this.cellC_EndAmt.Summary = xrSummaryEndAmt;
            this.cellC_EndAmt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.cellC_EndAmt.TextFormatString = "{0:#,##0;-#,##0;''}";
            this.cellC_EndAmt.Weight = 0.10909090909090909D;

            // 
            // lblSig1
            // 
            this.lblSig1.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig1.LocationFloat = new DevExpress.Utils.PointFloat(50F, 65F);
            this.lblSig1.Multiline = true;
            this.lblSig1.Name = "lblSig1";
            this.lblSig1.SizeF = new System.Drawing.SizeF(220F, 50F);
            this.lblSig1.Text = "Người ghi sổ\n(Ký, họ tên)";
            this.lblSig1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblSig2
            // 
            this.lblSig2.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig2.LocationFloat = new DevExpress.Utils.PointFloat(440F, 65F);
            this.lblSig2.Multiline = true;
            this.lblSig2.Name = "lblSig2";
            this.lblSig2.SizeF = new System.Drawing.SizeF(220F, 50F);
            this.lblSig2.Text = "Kế toán trưởng\n(Ký, họ tên)";
            this.lblSig2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // lblSig3
            // 
            this.lblSig3.Font = new DevExpress.Drawing.DXFont("Segoe UI", 9F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblSig3.LocationFloat = new DevExpress.Utils.PointFloat(830F, 65F);
            this.lblSig3.Multiline = true;
            this.lblSig3.Name = "lblSig3";
            this.lblSig3.SizeF = new System.Drawing.SizeF(220F, 50F);
            this.lblSig3.Text = "Giám đốc\n(Ký, họ tên, đóng dấu)";
            this.lblSig3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            // 
            // prmCompanyName
            // 
            this.prmCompanyName.Description = "Tên công ty";
            this.prmCompanyName.Name = "prmCompanyName";
            this.prmCompanyName.ValueInfo = "TỔNG CÔNG TY XÂY DỰNG HÀ NỘI";
            this.prmCompanyName.Visible = false;

            // 
            // prmPeriodTitle
            // 
            this.prmPeriodTitle.Description = "Kỳ báo cáo";
            this.prmPeriodTitle.Name = "prmPeriodTitle";
            this.prmPeriodTitle.ValueInfo = "";
            this.prmPeriodTitle.Visible = false;

            // 
            // prmAccountInfo
            // 
            this.prmAccountInfo.Description = "Tài khoản";
            this.prmAccountInfo.Name = "prmAccountInfo";
            this.prmAccountInfo.ValueInfo = "";
            this.prmAccountInfo.Visible = false;

            // 
            // prmWarehouseInfo
            // 
            this.prmWarehouseInfo.Description = "Kho hàng";
            this.prmWarehouseInfo.Name = "prmWarehouseInfo";
            this.prmWarehouseInfo.ValueInfo = "";
            this.prmWarehouseInfo.Visible = false;

            // 
            // prmMaterialInfo
            // 
            this.prmMaterialInfo.Description = "Vật tư";
            this.prmMaterialInfo.Name = "prmMaterialInfo";
            this.prmMaterialInfo.ValueInfo = "";
            this.prmMaterialInfo.Visible = false;

            // 
            // MaterialDetailReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageHeader,
            this.Detail,
            this.ReportFooter});
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(35F, 34F, 35F, 35F);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.prmCompanyName,
            this.prmPeriodTitle,
            this.prmAccountInfo,
            this.prmWarehouseInfo,
            this.prmMaterialInfo});
            this.Version = "25.2";

            ((System.ComponentModel.ISupportInitialize)(this.headerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.footerTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;

        private DevExpress.XtraReports.UI.XRLabel lblCompany;
        private DevExpress.XtraReports.UI.XRLabel lblFormInfo;
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblPeriod;
        private DevExpress.XtraReports.UI.XRLabel lblAccount;
        private DevExpress.XtraReports.UI.XRLabel lblWarehouse;
        private DevExpress.XtraReports.UI.XRLabel lblMaterial;

        private DevExpress.XtraReports.UI.XRTable headerTable;
        private DevExpress.XtraReports.UI.XRTableRow rowH1;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_VoucherGroup;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Contents;
        private DevExpress.XtraReports.UI.XRTableCell cellH_CounterAcc;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Price;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InGroup;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutGroup;
        private DevExpress.XtraReports.UI.XRTableCell cellH_ClosingGroup;

        private DevExpress.XtraReports.UI.XRTableRow rowH2;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Stt_Spacer;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Contents_Spacer;
        private DevExpress.XtraReports.UI.XRTableCell cellH_CounterAcc_Spacer;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Price_Spacer;
        private DevExpress.XtraReports.UI.XRTableCell cellH_Date;
        private DevExpress.XtraReports.UI.XRTableCell cellH_No;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_InAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_OutAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndQty;
        private DevExpress.XtraReports.UI.XRTableCell cellH_EndAmt;

        private DevExpress.XtraReports.UI.XRTable detailTable;
        private DevExpress.XtraReports.UI.XRTableRow detailRow;
        private DevExpress.XtraReports.UI.XRTableCell cellStt;
        private DevExpress.XtraReports.UI.XRTableCell cellVoucherDate;
        private DevExpress.XtraReports.UI.XRTableCell cellVoucherNo;
        private DevExpress.XtraReports.UI.XRTableCell cellContents;
        private DevExpress.XtraReports.UI.XRTableCell cellCounterAccount;
        private DevExpress.XtraReports.UI.XRTableCell cellPrice;
        private DevExpress.XtraReports.UI.XRTableCell cellDebitQty;
        private DevExpress.XtraReports.UI.XRTableCell cellDebitAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellCreditQty;
        private DevExpress.XtraReports.UI.XRTableCell cellCreditAmount;
        private DevExpress.XtraReports.UI.XRTableCell cellClosingQty;
        private DevExpress.XtraReports.UI.XRTableCell cellClosingAmount;

        private DevExpress.XtraReports.UI.XRTable footerTable;
        private DevExpress.XtraReports.UI.XRTableRow footerRowTotal;
        private DevExpress.XtraReports.UI.XRTableCell cellF_Label;
        private DevExpress.XtraReports.UI.XRTableCell cellF_InQty;
        private DevExpress.XtraReports.UI.XRTableCell cellF_InAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellF_OutQty;
        private DevExpress.XtraReports.UI.XRTableCell cellF_OutAmt;
        private DevExpress.XtraReports.UI.XRTableCell cellF_EndQtyBlank;
        private DevExpress.XtraReports.UI.XRTableCell cellF_EndAmtBlank;

        private DevExpress.XtraReports.UI.XRTableRow footerRowClosing;
        private DevExpress.XtraReports.UI.XRTableCell cellC_Label;
        private DevExpress.XtraReports.UI.XRTableCell cellC_EndQty;
        private DevExpress.XtraReports.UI.XRTableCell cellC_EndAmt;

        private DevExpress.XtraReports.UI.XRLabel lblSig1;
        private DevExpress.XtraReports.UI.XRLabel lblSig2;
        private DevExpress.XtraReports.UI.XRLabel lblSig3;

        private DevExpress.XtraReports.Parameters.Parameter prmCompanyName;
        private DevExpress.XtraReports.Parameters.Parameter prmPeriodTitle;
        private DevExpress.XtraReports.Parameters.Parameter prmAccountInfo;
        private DevExpress.XtraReports.Parameters.Parameter prmWarehouseInfo;
        private DevExpress.XtraReports.Parameters.Parameter prmMaterialInfo;
    }
}
