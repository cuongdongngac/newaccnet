using DevExpress.XtraReports.UI;
using System.Drawing;

namespace NewaccNet.Wpf.AppSystem.Voucher.Reports
{
    partial class InventoryVoucherReport
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

            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();

            // Parameter pCircularNote
            this.pCircularNote = new DevExpress.XtraReports.Parameters.Parameter();
            this.pCircularNote.Name = "pCircularNote";
            this.pCircularNote.Description = "Mẫu số & Thông tư:";
            this.pCircularNote.Type = typeof(string);
            this.pCircularNote.ValueInfo = "Mẫu số 01-VT\n(Ban hành theo Thông tư số 200/2014/TT-BTC\nNgày 22/12/2014 của BTC)";
            this.pCircularNote.Visible = true;

            // Header labels
            this.lblCompany = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAddress = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTaxCode = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCircular = new DevExpress.XtraReports.UI.XRLabel();
            this.lblVoucherNo = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDebitAcc = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCreditAcc = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSubTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();

            this.lblCarrier = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBill = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPartner = new DevExpress.XtraReports.UI.XRLabel();
            this.lblWarehouse = new DevExpress.XtraReports.UI.XRLabel();

            // Table Header
            this.tableHeader = new DevExpress.XtraReports.UI.XRTable();
            this.rowHeader = new DevExpress.XtraReports.UI.XRTableRow();
            this.colH_Stt = new DevExpress.XtraReports.UI.XRTableCell();
            this.colH_Name = new DevExpress.XtraReports.UI.XRTableCell();
            this.colH_Id = new DevExpress.XtraReports.UI.XRTableCell();
            this.colH_Unit = new DevExpress.XtraReports.UI.XRTableCell();
            this.colH_Qty = new DevExpress.XtraReports.UI.XRTableCell();
            this.colH_Price = new DevExpress.XtraReports.UI.XRTableCell();
            this.colH_Amount = new DevExpress.XtraReports.UI.XRTableCell();

            // Table Detail
            this.tableDetail = new DevExpress.XtraReports.UI.XRTable();
            this.rowDetail = new DevExpress.XtraReports.UI.XRTableRow();
            this.colD_Stt = new DevExpress.XtraReports.UI.XRTableCell();
            this.colD_Name = new DevExpress.XtraReports.UI.XRTableCell();
            this.colD_Id = new DevExpress.XtraReports.UI.XRTableCell();
            this.colD_Unit = new DevExpress.XtraReports.UI.XRTableCell();
            this.colD_Qty = new DevExpress.XtraReports.UI.XRTableCell();
            this.colD_Price = new DevExpress.XtraReports.UI.XRTableCell();
            this.colD_Amount = new DevExpress.XtraReports.UI.XRTableCell();

            // Table Footer (Cân khớp hoàn hảo: 869 + 220 = 1089)
            this.tableFooter = new DevExpress.XtraReports.UI.XRTable();
            this.rowFooterGoods = new DevExpress.XtraReports.UI.XRTableRow();
            this.colF_GoodsLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.colF_GoodsAmount = new DevExpress.XtraReports.UI.XRTableCell();

            this.rowFooterTax = new DevExpress.XtraReports.UI.XRTableRow();
            this.colF_TaxLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.colF_TaxAmount = new DevExpress.XtraReports.UI.XRTableCell();

            this.rowFooterTotal = new DevExpress.XtraReports.UI.XRTableRow();
            this.colF_TotalLabel = new DevExpress.XtraReports.UI.XRTableCell();
            this.colF_TotalAmount = new DevExpress.XtraReports.UI.XRTableCell();

            // Footer labels
            this.lblAmountInWords = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAttach = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignDate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign5 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub5 = new DevExpress.XtraReports.UI.XRLabel();

            // TopMargin
            this.TopMargin.HeightF = 35F;
            this.TopMargin.Name = "TopMargin";

            // BottomMargin
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Name = "BottomMargin";

            // ReportHeader
            this.ReportHeader.HeightF = 215F;
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.lblCompany, this.lblAddress, this.lblTaxCode,
                this.lblCircular, this.lblVoucherNo, this.lblDebitAcc, this.lblCreditAcc,
                this.lblTitle, this.lblSubTitle, this.lblDate,
                this.lblCarrier, this.lblBill, this.lblPartner, this.lblWarehouse
            });

            // lblCompany
            this.lblCompany.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.lblCompany.SizeF = new System.Drawing.SizeF(450F, 20F);
            this.lblCompany.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblCompany.Text = "CÔNG TY BÌNH MINH";

            // lblAddress
            this.lblAddress.LocationFloat = new DevExpress.Utils.PointFloat(0F, 20F);
            this.lblAddress.SizeF = new System.Drawing.SizeF(450F, 18F);
            this.lblAddress.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            this.lblAddress.Text = "Địa chỉ: 5/8 Đông Ngạc";

            // lblTaxCode
            this.lblTaxCode.LocationFloat = new DevExpress.Utils.PointFloat(0F, 38F);
            this.lblTaxCode.SizeF = new System.Drawing.SizeF(450F, 18F);
            this.lblTaxCode.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            this.lblTaxCode.Text = "Mã số thuế: 010100101";

            // lblCircular
            this.lblCircular.LocationFloat = new DevExpress.Utils.PointFloat(739F, 0F);
            this.lblCircular.SizeF = new System.Drawing.SizeF(350F, 46F);
            this.lblCircular.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8.5F);
            this.lblCircular.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.lblCircular.Multiline = true;
            this.lblCircular.CanGrow = true;
            this.lblCircular.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?pCircularNote"));

            // lblVoucherNo
            this.lblVoucherNo.LocationFloat = new DevExpress.Utils.PointFloat(789F, 48F);
            this.lblVoucherNo.SizeF = new System.Drawing.SizeF(300F, 18F);
            this.lblVoucherNo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblVoucherNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.lblVoucherNo.Text = "Số: PNK0001";

            // lblDebitAcc
            this.lblDebitAcc.LocationFloat = new DevExpress.Utils.PointFloat(789F, 66F);
            this.lblDebitAcc.SizeF = new System.Drawing.SizeF(300F, 18F);
            this.lblDebitAcc.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            this.lblDebitAcc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.lblDebitAcc.Text = "Nợ: 1521";

            // lblCreditAcc
            this.lblCreditAcc.LocationFloat = new DevExpress.Utils.PointFloat(789F, 84F);
            this.lblCreditAcc.SizeF = new System.Drawing.SizeF(300F, 18F);
            this.lblCreditAcc.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            this.lblCreditAcc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.lblCreditAcc.Text = "Có: 3311";

            // lblTitle
            this.lblTitle.LocationFloat = new DevExpress.Utils.PointFloat(300F, 50F);
            this.lblTitle.SizeF = new System.Drawing.SizeF(489F, 28F);
            this.lblTitle.Font = new DevExpress.Drawing.DXFont("Times New Roman", 16F, DevExpress.Drawing.DXFontStyle.Bold);
            this.lblTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblTitle.Text = "PHIẾU NHẬP KHO";

            // lblSubTitle
            this.lblSubTitle.LocationFloat = new DevExpress.Utils.PointFloat(300F, 78F);
            this.lblSubTitle.SizeF = new System.Drawing.SizeF(489F, 18F);
            this.lblSubTitle.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSubTitle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblSubTitle.Text = "(Nhập mua hàng)";

            // lblDate
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(300F, 96F);
            this.lblDate.SizeF = new System.Drawing.SizeF(489F, 18F);
            this.lblDate.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblDate.Text = "Ngày ... tháng ... năm ...";

            // Metadata Lines
            this.lblCarrier.LocationFloat = new DevExpress.Utils.PointFloat(0F, 125F);
            this.lblCarrier.SizeF = new System.Drawing.SizeF(1089F, 20F);
            this.lblCarrier.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.lblCarrier.Text = "Họ và tên người giao: ";

            this.lblBill.LocationFloat = new DevExpress.Utils.PointFloat(0F, 145F);
            this.lblBill.SizeF = new System.Drawing.SizeF(1089F, 20F);
            this.lblBill.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.lblBill.Text = "Theo hóa đơn số: ........ ngày ........ tháng ........ năm ........";

            this.lblPartner.LocationFloat = new DevExpress.Utils.PointFloat(0F, 165F);
            this.lblPartner.SizeF = new System.Drawing.SizeF(1089F, 20F);
            this.lblPartner.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.lblPartner.Text = "Đơn vị / Người mua bán: ";

            this.lblWarehouse.LocationFloat = new DevExpress.Utils.PointFloat(0F, 185F);
            this.lblWarehouse.SizeF = new System.Drawing.SizeF(1089F, 20F);
            this.lblWarehouse.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.lblWarehouse.Text = "Nhập tại kho: ";

            // PageHeader Band
            this.PageHeader.HeightF = 30F;
            this.PageHeader.Name = "PageHeader";
            this.PageHeader.Controls.Add(this.tableHeader);

            // Table Header
            this.tableHeader.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableHeader.SizeF = new System.Drawing.SizeF(1089F, 30F);
            this.tableHeader.Borders = DevExpress.XtraPrinting.BorderSide.All;
            this.tableHeader.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.tableHeader.Rows.Add(this.rowHeader);

            this.rowHeader.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colH_Stt, this.colH_Name, this.colH_Id, this.colH_Unit,
                this.colH_Qty, this.colH_Price, this.colH_Amount
            });

            this.colH_Stt.WidthF = 45F;
            this.colH_Stt.Text = "STT";

            this.colH_Name.WidthF = 384F;
            this.colH_Name.Text = "Tên, nhãn hiệu, quy cách vật tư, hàng hóa";

            this.colH_Id.WidthF = 110F;
            this.colH_Id.Text = "Mã số";

            this.colH_Unit.WidthF = 70F;
            this.colH_Unit.Text = "ĐVT";

            this.colH_Qty.WidthF = 120F;
            this.colH_Qty.Text = "Số lượng";

            this.colH_Price.WidthF = 140F;
            this.colH_Price.Text = "Đơn giá";

            this.colH_Amount.WidthF = 220F;
            this.colH_Amount.Text = "Thành tiền";

            // Detail Band
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.Controls.Add(this.tableDetail);

            this.tableDetail.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableDetail.SizeF = new System.Drawing.SizeF(1089F, 25F);
            this.tableDetail.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
            this.tableDetail.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.tableDetail.Rows.Add(this.rowDetail);

            this.rowDetail.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colD_Stt, this.colD_Name, this.colD_Id, this.colD_Unit,
                this.colD_Qty, this.colD_Price, this.colD_Amount
            });

            this.colD_Stt.WidthF = 45F;
            this.colD_Stt.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.colD_Stt.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Stt]"));

            this.colD_Name.WidthF = 384F;
            this.colD_Name.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0, 100F);
            this.colD_Name.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.colD_Name.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MaterialName]"));

            this.colD_Id.WidthF = 110F;
            this.colD_Id.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.colD_Id.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MaterialId]"));

            this.colD_Unit.WidthF = 70F;
            this.colD_Unit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.colD_Unit.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Unit]"));

            this.colD_Qty.WidthF = 120F;
            this.colD_Qty.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100F);
            this.colD_Qty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colD_Qty.TextFormatString = "{0:N2}";
            this.colD_Qty.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Quantity]"));

            this.colD_Price.WidthF = 140F;
            this.colD_Price.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100F);
            this.colD_Price.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colD_Price.TextFormatString = "{0:N0}";
            this.colD_Price.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Price]"));

            this.colD_Amount.WidthF = 220F;
            this.colD_Amount.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100F);
            this.colD_Amount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colD_Amount.TextFormatString = "{0:N0}";
            this.colD_Amount.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Amount]"));

            // ReportFooter Band
            this.ReportFooter.HeightF = 220F;
            this.ReportFooter.Name = "ReportFooter";
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.tableFooter,
                this.lblAmountInWords, this.lblAttach, this.lblSignDate,
                this.lblSign1, this.lblSignSub1, this.lblSign2, this.lblSignSub2,
                this.lblSign3, this.lblSignSub3, this.lblSign4, this.lblSignSub4,
                this.lblSign5, this.lblSignSub5
            });

            // Table Footer (Cân khớp: 869 + 220 = 1089)
            this.tableFooter.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.tableFooter.SizeF = new System.Drawing.SizeF(1089F, 78F);
            this.tableFooter.Borders = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right | DevExpress.XtraPrinting.BorderSide.Bottom;
            this.tableFooter.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            this.tableFooter.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
                this.rowFooterGoods,
                this.rowFooterTax,
                this.rowFooterTotal
            });

            // Row 1: Tiền hàng
            this.rowFooterGoods.HeightF = 26F;
            this.rowFooterGoods.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colF_GoodsLabel, this.colF_GoodsAmount
            });
            this.colF_GoodsLabel.WidthF = 869F;
            this.colF_GoodsLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 15, 0, 0, 100F);
            this.colF_GoodsLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colF_GoodsLabel.Text = "Cộng tiền hàng:";

            this.colF_GoodsAmount.WidthF = 220F;
            this.colF_GoodsAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100F);
            this.colF_GoodsAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colF_GoodsAmount.TextFormatString = "{0:N0}";

            // Row 2: Tiền thuế GTGT
            this.rowFooterTax.HeightF = 26F;
            this.rowFooterTax.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colF_TaxLabel, this.colF_TaxAmount
            });
            this.colF_TaxLabel.WidthF = 869F;
            this.colF_TaxLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 15, 0, 0, 100F);
            this.colF_TaxLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colF_TaxLabel.Text = "Tiền thuế GTGT:";

            this.colF_TaxAmount.WidthF = 220F;
            this.colF_TaxAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100F);
            this.colF_TaxAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colF_TaxAmount.TextFormatString = "{0:N0}";

            // Row 3: Tổng cộng thanh toán
            this.rowFooterTotal.HeightF = 26F;
            this.rowFooterTotal.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
                this.colF_TotalLabel, this.colF_TotalAmount
            });
            this.colF_TotalLabel.WidthF = 869F;
            this.colF_TotalLabel.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 15, 0, 0, 100F);
            this.colF_TotalLabel.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colF_TotalLabel.Text = "Tổng cộng tiền thanh toán:";

            this.colF_TotalAmount.WidthF = 220F;
            this.colF_TotalAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 5, 0, 0, 100F);
            this.colF_TotalAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.colF_TotalAmount.TextFormatString = "{0:N0}";

            // lblAmountInWords
            this.lblAmountInWords.LocationFloat = new DevExpress.Utils.PointFloat(0F, 88F);
            this.lblAmountInWords.SizeF = new System.Drawing.SizeF(1089F, 20F);
            this.lblAmountInWords.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.lblAmountInWords.Text = "Tổng số tiền (viết bằng chữ): ";

            // lblAttach
            this.lblAttach.LocationFloat = new DevExpress.Utils.PointFloat(0F, 108F);
            this.lblAttach.SizeF = new System.Drawing.SizeF(1089F, 18F);
            this.lblAttach.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            this.lblAttach.Text = "Kèm theo: ........ chứng từ gốc";

            // lblSignDate
            this.lblSignDate.LocationFloat = new DevExpress.Utils.PointFloat(800F, 128F);
            this.lblSignDate.SizeF = new System.Drawing.SizeF(289F, 18F);
            this.lblSignDate.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            this.lblSignDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.lblSignDate.Text = "Ngày ... tháng ... năm ...";

            // Signatures (5 cols across 1089 width: 217.8 per col)
            float signW = 217.8F;
            SetupSignColumn(lblSign1, lblSignSub1, 0F, signW, "Người lập biểu", "(Ký, họ tên)");
            SetupSignColumn(lblSign2, lblSignSub2, 218F, signW, "Người giao hàng", "(Ký, họ tên)");
            SetupSignColumn(lblSign3, lblSignSub3, 436F, signW, "Thủ kho", "(Ký, họ tên)");
            SetupSignColumn(lblSign4, lblSignSub4, 654F, signW, "Kế toán trưởng", "(Ký, họ tên)");
            SetupSignColumn(lblSign5, lblSignSub5, 872F, 217F, "Giám đốc", "(Ký, họ tên, đóng dấu)");

            // Report settings (A4 Landscape)
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.TopMargin,
                this.ReportHeader,
                this.PageHeader,
                this.Detail,
                this.ReportFooter,
                this.BottomMargin
            });
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
                this.pCircularNote
            });
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Landscape = true;
            this.Margins = new DevExpress.Drawing.DXMargins(40, 40, 35, 35);
            this.PageWidth = 1169;
            this.PageHeight = 827;
            this.Version = "25.2";
        }

        private void SetupSignColumn(XRLabel titleLbl, XRLabel subLbl, float left, float width, string titleText, string subText)
        {
            titleLbl.LocationFloat = new DevExpress.Utils.PointFloat(left, 148F);
            titleLbl.SizeF = new System.Drawing.SizeF(width, 18F);
            titleLbl.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            titleLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            titleLbl.Text = titleText;

            subLbl.LocationFloat = new DevExpress.Utils.PointFloat(left, 166F);
            subLbl.SizeF = new System.Drawing.SizeF(width, 15F);
            subLbl.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8F, DevExpress.Drawing.DXFontStyle.Italic);
            subLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            subLbl.Text = subText;
        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.Parameters.Parameter pCircularNote;

        // Header controls
        private DevExpress.XtraReports.UI.XRLabel lblCompany;
        private DevExpress.XtraReports.UI.XRLabel lblAddress;
        private DevExpress.XtraReports.UI.XRLabel lblTaxCode;
        private DevExpress.XtraReports.UI.XRLabel lblCircular;
        private DevExpress.XtraReports.UI.XRLabel lblVoucherNo;
        private DevExpress.XtraReports.UI.XRLabel lblDebitAcc;
        private DevExpress.XtraReports.UI.XRLabel lblCreditAcc;
        private DevExpress.XtraReports.UI.XRLabel lblTitle;
        private DevExpress.XtraReports.UI.XRLabel lblSubTitle;
        private DevExpress.XtraReports.UI.XRLabel lblDate;

        private DevExpress.XtraReports.UI.XRLabel lblCarrier;
        private DevExpress.XtraReports.UI.XRLabel lblBill;
        private DevExpress.XtraReports.UI.XRLabel lblPartner;
        private DevExpress.XtraReports.UI.XRLabel lblWarehouse;

        // Table Header
        private DevExpress.XtraReports.UI.XRTable tableHeader;
        private DevExpress.XtraReports.UI.XRTableRow rowHeader;
        private DevExpress.XtraReports.UI.XRTableCell colH_Stt;
        private DevExpress.XtraReports.UI.XRTableCell colH_Name;
        private DevExpress.XtraReports.UI.XRTableCell colH_Id;
        private DevExpress.XtraReports.UI.XRTableCell colH_Unit;
        private DevExpress.XtraReports.UI.XRTableCell colH_Qty;
        private DevExpress.XtraReports.UI.XRTableCell colH_Price;
        private DevExpress.XtraReports.UI.XRTableCell colH_Amount;

        // Table Detail
        private DevExpress.XtraReports.UI.XRTable tableDetail;
        private DevExpress.XtraReports.UI.XRTableRow rowDetail;
        private DevExpress.XtraReports.UI.XRTableCell colD_Stt;
        private DevExpress.XtraReports.UI.XRTableCell colD_Name;
        private DevExpress.XtraReports.UI.XRTableCell colD_Id;
        private DevExpress.XtraReports.UI.XRTableCell colD_Unit;
        private DevExpress.XtraReports.UI.XRTableCell colD_Qty;
        private DevExpress.XtraReports.UI.XRTableCell colD_Price;
        private DevExpress.XtraReports.UI.XRTableCell colD_Amount;

        // Table Footer
        private DevExpress.XtraReports.UI.XRTable tableFooter;
        private DevExpress.XtraReports.UI.XRTableRow rowFooterGoods;
        private DevExpress.XtraReports.UI.XRTableCell colF_GoodsLabel;
        private DevExpress.XtraReports.UI.XRTableCell colF_GoodsAmount;

        private DevExpress.XtraReports.UI.XRTableRow rowFooterTax;
        private DevExpress.XtraReports.UI.XRTableCell colF_TaxLabel;
        private DevExpress.XtraReports.UI.XRTableCell colF_TaxAmount;

        private DevExpress.XtraReports.UI.XRTableRow rowFooterTotal;
        private DevExpress.XtraReports.UI.XRTableCell colF_TotalLabel;
        private DevExpress.XtraReports.UI.XRTableCell colF_TotalAmount;

        // Footer controls
        private DevExpress.XtraReports.UI.XRLabel lblAmountInWords;
        private DevExpress.XtraReports.UI.XRLabel lblAttach;
        private DevExpress.XtraReports.UI.XRLabel lblSignDate;
        private DevExpress.XtraReports.UI.XRLabel lblSign1;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub1;
        private DevExpress.XtraReports.UI.XRLabel lblSign2;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub2;
        private DevExpress.XtraReports.UI.XRLabel lblSign3;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub3;
        private DevExpress.XtraReports.UI.XRLabel lblSign4;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub4;
        private DevExpress.XtraReports.UI.XRLabel lblSign5;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub5;
    }
}
