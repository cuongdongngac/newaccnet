using DevExpress.XtraReports.UI;
using System.Drawing;

namespace NewaccNet.Wpf.AppSystem.Voucher.Reports
{
    partial class CashPaymentReport
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
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.pnlLien1 = new DevExpress.XtraReports.UI.XRPanel();
            this.pnlLien2 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLineDivider = new DevExpress.XtraReports.UI.XRLine();

            // Labels Lien 1
            this.lblCompany1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAddress1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTaxCode1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCircular1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBookNo1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblVoucherNo1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDebitAcc1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCreditAcc1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPerson1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPersonAddress1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblReason1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAmount1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAmountWords1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAttach1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignDate1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1_1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub1_1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1_2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub1_2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1_3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub1_3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1_4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub1_4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign1_5 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub1_5 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBottomNote1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBottomExRate1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBottomConv1 = new DevExpress.XtraReports.UI.XRLabel();

            // Labels Lien 2
            this.lblCompany2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAddress2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTaxCode2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCircular2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBookNo2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblVoucherNo2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDebitAcc2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblCreditAcc2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitle2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPerson2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPersonAddress2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblReason2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAmount2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAmountWords2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAttach2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignDate2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2_1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub2_1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2_2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub2_2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2_3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub2_3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2_4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub2_4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSign2_5 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblSignSub2_5 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBottomNote2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBottomExRate2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblBottomConv2 = new DevExpress.XtraReports.UI.XRLabel();

            // Parameter pCircularNote
            this.pCircularNote = new DevExpress.XtraReports.Parameters.Parameter();
            this.pCircularNote.Name = "pCircularNote";
            this.pCircularNote.Description = "Mẫu số & Thông tư:";
            this.pCircularNote.Type = typeof(string);
            this.pCircularNote.ValueInfo = "Mẫu số 02-TT\n(Ban hành theo Thông tư số 200/2014/TT-BTC\nNgày 22/12/2014 của BTC)";
            this.pCircularNote.Visible = true;

            // TopMargin
            this.TopMargin.HeightF = 35F;
            this.TopMargin.Name = "TopMargin";

            // BottomMargin
            this.BottomMargin.HeightF = 35F;
            this.BottomMargin.Name = "BottomMargin";

            // Detail
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                this.pnlLien1,
                this.xrLineDivider,
                this.pnlLien2
            });
            this.Detail.HeightF = 1040F;
            this.Detail.Name = "Detail";

            // pnlLien1
            this.pnlLien1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.pnlLien1.SizeF = new System.Drawing.SizeF(747F, 495F);
            this.pnlLien1.Name = "pnlLien1";
            BuildPanelControls(this.pnlLien1, 1,
                lblCompany1, lblAddress1, lblTaxCode1, lblCircular1, lblBookNo1, lblVoucherNo1,
                lblDebitAcc1, lblCreditAcc1, lblTitle1, lblDate1, lblPerson1, lblPersonAddress1,
                lblReason1, lblAmount1, lblAmountWords1, lblAttach1, lblSignDate1,
                lblSign1_1, lblSignSub1_1, lblSign1_2, lblSignSub1_2, lblSign1_3, lblSignSub1_3,
                lblSign1_4, lblSignSub1_4, lblSign1_5, lblSignSub1_5,
                lblBottomNote1, lblBottomExRate1, lblBottomConv1, "PHIẾU CHI", "Thủ quỹ", "Người nhận tiền");

            // xrLineDivider
            this.xrLineDivider.LocationFloat = new DevExpress.Utils.PointFloat(0F, 502F);
            this.xrLineDivider.SizeF = new System.Drawing.SizeF(747F, 10F);
            this.xrLineDivider.LineStyle = DevExpress.Drawing.DXDashStyle.Dash;
            this.xrLineDivider.Name = "xrLineDivider";

            // pnlLien2
            this.pnlLien2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 518F);
            this.pnlLien2.SizeF = new System.Drawing.SizeF(747F, 495F);
            this.pnlLien2.Name = "pnlLien2";
            BuildPanelControls(this.pnlLien2, 2,
                lblCompany2, lblAddress2, lblTaxCode2, lblCircular2, lblBookNo2, lblVoucherNo2,
                lblDebitAcc2, lblCreditAcc2, lblTitle2, lblDate2, lblPerson2, lblPersonAddress2,
                lblReason2, lblAmount2, lblAmountWords2, lblAttach2, lblSignDate2,
                lblSign2_1, lblSignSub2_1, lblSign2_2, lblSignSub2_2, lblSign2_3, lblSignSub2_3,
                lblSign2_4, lblSignSub2_4, lblSign2_5, lblSignSub2_5,
                lblBottomNote2, lblBottomExRate2, lblBottomConv2, "PHIẾU CHI", "Thủ quỹ", "Người nhận tiền");

            // Report settings
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
                this.TopMargin,
                this.Detail,
                this.BottomMargin
            });
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
                this.pCircularNote
            });
            this.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            this.Margins = new DevExpress.Drawing.DXMargins(40, 40, 35, 35);
            this.PageWidth = 827;
            this.PageHeight = 1169;
            this.Version = "25.2";
        }

        private void BuildPanelControls(
            XRPanel panel,
            int lien,
            XRLabel comp, XRLabel addr, XRLabel mst, XRLabel circ, XRLabel book, XRLabel vNo,
            XRLabel drAcc, XRLabel crAcc, XRLabel title, XRLabel date, XRLabel pers, XRLabel pAddr,
            XRLabel rsn, XRLabel amt, XRLabel amtWords, XRLabel att, XRLabel sDate,
            XRLabel s1, XRLabel ss1, XRLabel s2, XRLabel ss2, XRLabel s3, XRLabel ss3,
            XRLabel s4, XRLabel ss4, XRLabel s5, XRLabel ss5,
            XRLabel bNote, XRLabel bEx, XRLabel bConv,
            string titleText, string sign3Text, string sign5Text)
        {
            // Company info
            comp.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            comp.SizeF = new System.Drawing.SizeF(400F, 20F);
            comp.Font = new DevExpress.Drawing.DXFont("Times New Roman", 10F, DevExpress.Drawing.DXFontStyle.Bold);
            comp.Text = "CÔNG TY BÌNH MINH";

            addr.LocationFloat = new DevExpress.Utils.PointFloat(0F, 20F);
            addr.SizeF = new System.Drawing.SizeF(400F, 18F);
            addr.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            addr.Text = "Địa chỉ: 5/8 Đông Ngạc";

            mst.LocationFloat = new DevExpress.Utils.PointFloat(0F, 38F);
            mst.SizeF = new System.Drawing.SizeF(400F, 18F);
            mst.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            mst.Text = "Mã số thuế: 010100101";

            // Circular / Decision
            circ.LocationFloat = new DevExpress.Utils.PointFloat(410F, 0F);
            circ.SizeF = new System.Drawing.SizeF(337F, 46F);
            circ.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8.5F);
            circ.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            circ.Multiline = true;
            circ.CanGrow = true;
            circ.ExpressionBindings.Add(new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?pCircularNote"));

            book.LocationFloat = new DevExpress.Utils.PointFloat(480F, 48F);
            book.SizeF = new System.Drawing.SizeF(267F, 16F);
            book.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            book.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            book.Text = "Quyển số: ..............";

            vNo.LocationFloat = new DevExpress.Utils.PointFloat(480F, 64F);
            vNo.SizeF = new System.Drawing.SizeF(267F, 18F);
            vNo.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            vNo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            vNo.Text = "Số: 01-0001";

            drAcc.LocationFloat = new DevExpress.Utils.PointFloat(480F, 82F);
            drAcc.SizeF = new System.Drawing.SizeF(267F, 28F);
            drAcc.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            drAcc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            drAcc.Multiline = true;
            drAcc.CanGrow = true;
            drAcc.Text = "Nợ 6422 : 100,000 đ";

            crAcc.LocationFloat = new DevExpress.Utils.PointFloat(480F, 110F);
            crAcc.SizeF = new System.Drawing.SizeF(267F, 28F);
            crAcc.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F);
            crAcc.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            crAcc.Multiline = true;
            crAcc.CanGrow = true;
            crAcc.Text = "Có 1111 : 100,000 đ";

            // Title
            title.LocationFloat = new DevExpress.Utils.PointFloat(150F, 58F);
            title.SizeF = new System.Drawing.SizeF(320F, 28F);
            title.Font = new DevExpress.Drawing.DXFont("Times New Roman", 16F, DevExpress.Drawing.DXFontStyle.Bold);
            title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            title.Text = titleText;

            date.LocationFloat = new DevExpress.Utils.PointFloat(150F, 86F);
            date.SizeF = new System.Drawing.SizeF(320F, 18F);
            date.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Italic);
            date.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            date.Text = "Ngày ... tháng ... năm ...";

            // Body
            pers.LocationFloat = new DevExpress.Utils.PointFloat(0F, 142F);
            pers.SizeF = new System.Drawing.SizeF(747F, 20F);
            pers.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            pers.Text = "Họ tên người nhận tiền:";

            pAddr.LocationFloat = new DevExpress.Utils.PointFloat(0F, 162F);
            pAddr.SizeF = new System.Drawing.SizeF(747F, 20F);
            pAddr.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            pAddr.Text = "Địa chỉ:";

            rsn.LocationFloat = new DevExpress.Utils.PointFloat(0F, 182F);
            rsn.SizeF = new System.Drawing.SizeF(747F, 20F);
            rsn.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            rsn.Multiline = true;
            rsn.CanGrow = true;
            rsn.Text = "Lý do chi:";

            amt.LocationFloat = new DevExpress.Utils.PointFloat(0F, 202F);
            amt.SizeF = new System.Drawing.SizeF(747F, 20F);
            amt.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            amt.Text = "Số tiền:";

            amtWords.LocationFloat = new DevExpress.Utils.PointFloat(0F, 222F);
            amtWords.SizeF = new System.Drawing.SizeF(747F, 20F);
            amtWords.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            amtWords.Multiline = true;
            amtWords.CanGrow = true;
            amtWords.Text = "Viết bằng chữ:";

            att.LocationFloat = new DevExpress.Utils.PointFloat(0F, 242F);
            att.SizeF = new System.Drawing.SizeF(747F, 18F);
            att.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F);
            att.Text = "Kèm theo: ........ chứng từ gốc";

            // Date line above signatures
            sDate.LocationFloat = new DevExpress.Utils.PointFloat(450F, 260F);
            sDate.SizeF = new System.Drawing.SizeF(297F, 18F);
            sDate.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9F, DevExpress.Drawing.DXFontStyle.Italic);
            sDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            sDate.Text = "Ngày ... tháng ... năm ...";

            // Signatures (5 cols)
            float colW = 149F;
            SetupSignColumn(s1, ss1, 0F, colW, "Giám đốc", "(Ký, họ tên, đóng dấu)");
            SetupSignColumn(s2, ss2, 149F, colW, "Kế toán trưởng", "(Ký, họ tên)");
            SetupSignColumn(s3, ss3, 298F, colW, sign3Text, "(Ký, họ tên)");
            SetupSignColumn(s4, ss4, 447F, colW, "Người lập phiếu", "(Ký, họ tên)");
            SetupSignColumn(s5, ss5, 596F, 151F, sign5Text, "(Ký, họ tên)");

            // Bottom notes
            bNote.LocationFloat = new DevExpress.Utils.PointFloat(0F, 415F);
            bNote.SizeF = new System.Drawing.SizeF(747F, 18F);
            bNote.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            bNote.Text = "Đã nhận đủ số tiền (viết bằng chữ): ............................................................................................................................................";

            bEx.LocationFloat = new DevExpress.Utils.PointFloat(0F, 433F);
            bEx.SizeF = new System.Drawing.SizeF(747F, 18F);
            bEx.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            bEx.Text = "+ Tỷ giá ngoại tệ (vàng bạc, đá quý): .............................................................................................................................................";

            bConv.LocationFloat = new DevExpress.Utils.PointFloat(0F, 451F);
            bConv.SizeF = new System.Drawing.SizeF(747F, 18F);
            bConv.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8.5F, DevExpress.Drawing.DXFontStyle.Italic);
            bConv.Text = "+ Số tiền quy đổi: ...................................................................................................................................................................";

            panel.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
                comp, addr, mst, circ, book, vNo, drAcc, crAcc, title, date,
                pers, pAddr, rsn, amt, amtWords, att, sDate,
                s1, ss1, s2, ss2, s3, ss3, s4, ss4, s5, ss5,
                bNote, bEx, bConv
            });
        }

        private void SetupSignColumn(XRLabel titleLbl, XRLabel subLbl, float left, float width, string titleText, string subText)
        {
            titleLbl.LocationFloat = new DevExpress.Utils.PointFloat(left, 280F);
            titleLbl.SizeF = new System.Drawing.SizeF(width, 18F);
            titleLbl.Font = new DevExpress.Drawing.DXFont("Times New Roman", 9.5F, DevExpress.Drawing.DXFontStyle.Bold);
            titleLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            titleLbl.Text = titleText;

            subLbl.LocationFloat = new DevExpress.Utils.PointFloat(left, 298F);
            subLbl.SizeF = new System.Drawing.SizeF(width, 15F);
            subLbl.Font = new DevExpress.Drawing.DXFont("Times New Roman", 8F, DevExpress.Drawing.DXFontStyle.Italic);
            subLbl.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            subLbl.Text = subText;
        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRPanel pnlLien1;
        private DevExpress.XtraReports.UI.XRPanel pnlLien2;
        private DevExpress.XtraReports.UI.XRLine xrLineDivider;
        private DevExpress.XtraReports.Parameters.Parameter pCircularNote;

        // Lien 1 controls
        private DevExpress.XtraReports.UI.XRLabel lblCompany1;
        private DevExpress.XtraReports.UI.XRLabel lblAddress1;
        private DevExpress.XtraReports.UI.XRLabel lblTaxCode1;
        private DevExpress.XtraReports.UI.XRLabel lblCircular1;
        private DevExpress.XtraReports.UI.XRLabel lblBookNo1;
        private DevExpress.XtraReports.UI.XRLabel lblVoucherNo1;
        private DevExpress.XtraReports.UI.XRLabel lblDebitAcc1;
        private DevExpress.XtraReports.UI.XRLabel lblCreditAcc1;
        private DevExpress.XtraReports.UI.XRLabel lblTitle1;
        private DevExpress.XtraReports.UI.XRLabel lblDate1;
        private DevExpress.XtraReports.UI.XRLabel lblPerson1;
        private DevExpress.XtraReports.UI.XRLabel lblPersonAddress1;
        private DevExpress.XtraReports.UI.XRLabel lblReason1;
        private DevExpress.XtraReports.UI.XRLabel lblAmount1;
        private DevExpress.XtraReports.UI.XRLabel lblAmountWords1;
        private DevExpress.XtraReports.UI.XRLabel lblAttach1;
        private DevExpress.XtraReports.UI.XRLabel lblSignDate1;
        private DevExpress.XtraReports.UI.XRLabel lblSign1_1;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub1_1;
        private DevExpress.XtraReports.UI.XRLabel lblSign1_2;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub1_2;
        private DevExpress.XtraReports.UI.XRLabel lblSign1_3;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub1_3;
        private DevExpress.XtraReports.UI.XRLabel lblSign1_4;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub1_4;
        private DevExpress.XtraReports.UI.XRLabel lblSign1_5;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub1_5;
        private DevExpress.XtraReports.UI.XRLabel lblBottomNote1;
        private DevExpress.XtraReports.UI.XRLabel lblBottomExRate1;
        private DevExpress.XtraReports.UI.XRLabel lblBottomConv1;

        // Lien 2 controls
        private DevExpress.XtraReports.UI.XRLabel lblCompany2;
        private DevExpress.XtraReports.UI.XRLabel lblAddress2;
        private DevExpress.XtraReports.UI.XRLabel lblTaxCode2;
        private DevExpress.XtraReports.UI.XRLabel lblCircular2;
        private DevExpress.XtraReports.UI.XRLabel lblBookNo2;
        private DevExpress.XtraReports.UI.XRLabel lblVoucherNo2;
        private DevExpress.XtraReports.UI.XRLabel lblDebitAcc2;
        private DevExpress.XtraReports.UI.XRLabel lblCreditAcc2;
        private DevExpress.XtraReports.UI.XRLabel lblTitle2;
        private DevExpress.XtraReports.UI.XRLabel lblDate2;
        private DevExpress.XtraReports.UI.XRLabel lblPerson2;
        private DevExpress.XtraReports.UI.XRLabel lblPersonAddress2;
        private DevExpress.XtraReports.UI.XRLabel lblReason2;
        private DevExpress.XtraReports.UI.XRLabel lblAmount2;
        private DevExpress.XtraReports.UI.XRLabel lblAmountWords2;
        private DevExpress.XtraReports.UI.XRLabel lblAttach2;
        private DevExpress.XtraReports.UI.XRLabel lblSignDate2;
        private DevExpress.XtraReports.UI.XRLabel lblSign2_1;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub2_1;
        private DevExpress.XtraReports.UI.XRLabel lblSign2_2;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub2_2;
        private DevExpress.XtraReports.UI.XRLabel lblSign2_3;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub2_3;
        private DevExpress.XtraReports.UI.XRLabel lblSign2_4;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub2_4;
        private DevExpress.XtraReports.UI.XRLabel lblSign2_5;
        private DevExpress.XtraReports.UI.XRLabel lblSignSub2_5;
        private DevExpress.XtraReports.UI.XRLabel lblBottomNote2;
        private DevExpress.XtraReports.UI.XRLabel lblBottomExRate2;
        private DevExpress.XtraReports.UI.XRLabel lblBottomConv2;
    }
}
