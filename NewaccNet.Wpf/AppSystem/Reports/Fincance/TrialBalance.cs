using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Drawing;
using NewaccNet.Reports;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public partial class TrialBalance : DevExpress.XtraReports.UI.XtraReport
    {
        private decimal _totalBeginDebit;
        private decimal _totalBeginCredit;
        private decimal _totalIntDebit;
        private decimal _totalIntCredit;
        private decimal _totalEndDebit;
        private decimal _totalEndCredit;
        private bool _isTotalCalculated;

        public TrialBalance()
        {
            InitializeComponent();
            this.ApplySubtleBorders();

            // Xử lý sự kiện BeforePrint của row để in đậm các tài khoản cha (Splite = 1)
            this.tableRow2.BeforePrint += TableRow2_BeforePrint;

            // Tính toán tổng số phát sinh và số dư
            this.BeforePrint += TrialBalance_BeforePrint;
            this.tableFooter.BeforePrint += TableFooter_BeforePrint;
        }

        private void TableRow2_BeforePrint(object sender, CancelEventArgs e)
        {
            var row = sender as XRTableRow;
            if (row != null)
            {
                var spliteVal = GetCurrentColumnValue("Splite");
                bool isSplite = spliteVal != null && Convert.ToBoolean(spliteVal);

                row.Font = isSplite
                    ? new DXFont("Arial", 9F, DXFontStyle.Bold)
                    : new DXFont("Arial", 8.75F, DXFontStyle.Regular);
            }
        }

        private void TrialBalance_BeforePrint(object sender, CancelEventArgs e)
        {
            CalculateTotals();
        }

        private void TableFooter_BeforePrint(object sender, CancelEventArgs e)
        {
            CalculateTotals();
            ApplyTotalsToCells();
        }

        private void CalculateTotals()
        {
            if (_isTotalCalculated) return;

            if (this.DataSource is IEnumerable enumerable)
            {
                var list = enumerable.OfType<TrialBalanceDTO>().ToList();
                if (list.Count > 0)
                {
                    _isTotalCalculated = true;

                    // Chỉ cộng các tài khoản lá (Splite == false) để tránh cộng trùng lặp tài khoản cha
                    var leafList = list.Where(x => !x.Splite).ToList();
                    var targetList = leafList.Count > 0 ? leafList : list;

                    _totalBeginDebit = targetList.Sum(x => x.BeginDebit);
                    _totalBeginCredit = targetList.Sum(x => x.BeginCredit);
                    _totalIntDebit = targetList.Sum(x => x.IntDebit);
                    _totalIntCredit = targetList.Sum(x => x.IntCredit);
                    _totalEndDebit = targetList.Sum(x => x.EndDebit);
                    _totalEndCredit = targetList.Sum(x => x.EndCredit);
                }
            }
        }

        private void ApplyTotalsToCells()
        {
            this.tfCellBeginDebit.Text = _totalBeginDebit != 0 ? string.Format("{0:#,##0;(#,##0);\"\"}", _totalBeginDebit) : "";
            this.tfCellBeginCredit.Text = _totalBeginCredit != 0 ? string.Format("{0:#,##0;(#,##0);\"\"}", _totalBeginCredit) : "";
            this.tfCellIntDebit.Text = string.Format("{0:#,##0;(#,##0);\"\"}", _totalIntDebit);
            this.tfCellIntCredit.Text = string.Format("{0:#,##0;(#,##0);\"\"}", _totalIntCredit);
            this.tfCellEndDebit.Text = string.Format("{0:#,##0;(#,##0);\"\"}", _totalEndDebit);
            this.tfCellEndCredit.Text = string.Format("{0:#,##0;(#,##0);\"\"}", _totalEndCredit);
        }
    }
}

