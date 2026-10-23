using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using NewaccNet.Reports;

namespace NewaccNet.Wpf.AppSystem.Reports.Ledgers
{
    public partial class LedgerSummary : DevExpress.XtraReports.UI.XtraReport
    {
        private bool _isDataAggregated = false;

        public LedgerSummary()
        {
            InitializeComponent();
            this.ApplySubtleBorders();
            this.BeforePrint += LedgerSummary_BeforePrint;
        }

        private void LedgerSummary_BeforePrint(object sender, CancelEventArgs e)
        {
            if (_isDataAggregated) return;

            if (this.DataSource is IEnumerable enumerable)
            {
                var rawList = enumerable.OfType<LedgerReportDTO>().ToList();
                if (rawList.Count > 0)
                {
                    _isDataAggregated = true;

                    // 1. Số dư đầu kỳ (RowType == 0)
                    var openingItem = rawList.FirstOrDefault(x => x.RowType == 0);
                    decimal openingBalance = openingItem?.Balance ?? 0;

                    // 2. Phát sinh trong kỳ (RowType == 1)
                    var periodItems = rawList.Where(x => x.RowType == 1).ToList();
                    decimal totalDebit = periodItems.Sum(x => x.DebitAmount);
                    decimal totalCredit = periodItems.Sum(x => x.CreditAmount);

                    // 3. Số dư cuối kỳ = Dư đầu kỳ + Tổng phát sinh nợ - Tổng phát sinh có
                    decimal closingBalance = openingBalance + totalDebit - totalCredit;

                    // 4. Gán giá trị vào Header
                    this.lblDuDauKyValue.Text = string.Format("{0:#,##0;(#,##0);0}", openingBalance);
                    this.lblDuCuoiKyValue.Text = string.Format("{0:#,##0;(#,##0);0}", closingBalance);

                    // 5. Tổng hợp dữ liệu theo từng Tài khoản đối ứng
                    var summaryList = periodItems
                        .Where(x => !string.IsNullOrWhiteSpace(x.CounterAccountId))
                        .GroupBy(x => new { x.CounterAccountId, x.CounterAccountName })
                        .Select(g => new LedgerReportDTO
                        {
                            RowType = 1,
                            CounterAccountId = g.Key.CounterAccountId,
                            CounterAccountName = g.Key.CounterAccountName ?? "",
                            DebitAmount = g.Sum(x => x.DebitAmount),
                            CreditAmount = g.Sum(x => x.CreditAmount)
                        })
                        .OrderBy(x => x.CounterAccountId)
                        .ToList();

                    this.DataSource = summaryList;
                }
            }
        }
    }
}

