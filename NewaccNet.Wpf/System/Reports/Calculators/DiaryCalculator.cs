using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;
using DataAccess.TypedListClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Wpf.AppSystem.Reports.Calculators
{
    public class DiarySummaryModel
    {
        public int TotalVouchers { get; set; }
        public int TotalEntries { get; set; }
        public decimal SumDebit { get; set; }
        public decimal SumCredit { get; set; }
        public decimal BalanceDiff => SumDebit - SumCredit;
    }

    public class DiaryCalculator
    {
        public List<DiaryRow> Calculate(DateTime fromDate, DateTime toDate, bool onlyBooked = true)
        {
            using var adapter = AppDataAccessAdapter.Create();
            var qf = new QueryFactory();

            DateTime fromDt = fromDate.Date;
            DateTime toDt = toDate.Date.AddDays(1).AddTicks(-1);

            var predicate = JournalVoucherFields.VoucherDate.GreaterEqual(fromDt)
                .And(JournalVoucherFields.VoucherDate.LesserEqual(toDt));
            if (onlyBooked)
                predicate = predicate.And(JournalVoucherFields.Bookflag.Equal(true));

            var q = qf.GetDiaryTypedList()
                .Where(predicate)
                .OrderBy(JournalVoucherFields.VoucherDate.Ascending(), JournalVoucherFields.VoucherNo.Ascending(), JournalVoucherFields.Id.Ascending(), JournalEntryFields.Dbcr.Descending());

            return adapter.FetchQuery(q);
        }

        public DiarySummaryModel Summarize(List<DiaryRow> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return new DiarySummaryModel
                {
                    TotalVouchers = 0,
                    TotalEntries = 0,
                    SumDebit = 0,
                    SumCredit = 0
                };
            }

            return new DiarySummaryModel
            {
                TotalVouchers = rows.Select(x => x.JournalVoucherId).Distinct().Count(),
                TotalEntries = rows.Count,
                SumDebit = rows.Sum(x => x.Debit),
                SumCredit = rows.Sum(x => x.Credit)
            };
        }
    }
}
