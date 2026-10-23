using System;
using System.Collections.Generic;

namespace NewaccNet.TaxInvoice
{
    public readonly struct DateRange
    {
        public DateRange(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate.Date;
            ToDate = toDate.Date;
        }

        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string MonthLabel => FromDate.ToString("yyyy-MM");
        public override string ToString() => $"{FromDate:dd/MM/yyyy} - {ToDate:dd/MM/yyyy}";
    }

    public static class DateRangeHelper
    {
        public static IReadOnlyList<DateRange> SplitByMonth(DateTime fromDate, DateTime toDate)
        {
            fromDate = fromDate.Date;
            toDate = toDate.Date;
            if (toDate < fromDate)
            {
                throw new ArgumentException("The 'toDate' parameter must be greater than or equal to 'fromDate'.");
            }

            var rangeList = new List<DateRange>();
            var currentStart = fromDate;
            while (currentStart <= toDate)
            {
                var monthEnd = new DateTime(currentStart.Year, currentStart.Month, 1).AddMonths(1).AddDays(-1);
                var currentEnd = monthEnd < toDate ? monthEnd : toDate;
                rangeList.Add(new DateRange(currentStart, currentEnd));
                currentStart = currentEnd.AddDays(1);
            }
            return rangeList;
        }
    }
}
