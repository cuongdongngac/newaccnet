using System;
using System.Collections.Generic;

namespace Hddt
{
    /// <summary>Khoảng ngày [From, To] (chỉ lấy phần ngày).</summary>
    public readonly struct DateRange
    {
        public DateRange(DateTime from, DateTime to)
        {
            From = from.Date; To = to.Date;
        }

        public DateTime From { get; }
        public DateTime To { get; }
        /// <summary>Nhãn tháng của khoảng, dạng yyyy-MM.</summary>
        public string MonthLabel => From.ToString("yyyy-MM");
        public override string ToString() => From.ToString("dd/MM/yyyy") + " - " + To.ToString("dd/MM/yyyy");
    }

    public static class DateRanges
    {
        /// <summary>
        /// Tách [from, to] thành từng đoạn không vượt quá một tháng dương lịch
        /// (cổng giới hạn khoảng tìm kiếm; bản VBA cũng tải theo từng tháng).
        /// Ví dụ 15/01 - 10/03 → 15/01-31/01, 01/02-29/02, 01/03-10/03.
        /// </summary>
        public static IReadOnlyList<DateRange> SplitByMonth(DateTime from, DateTime to)
        {
            from = from.Date; to = to.Date;
            if (to < from) throw new ArgumentException("Ngày kết thúc nhỏ hơn ngày bắt đầu.");

            var list = new List<DateRange>();
            var start = from;
            while (start <= to)
            {
                var monthEnd = new DateTime(start.Year, start.Month, 1).AddMonths(1).AddDays(-1);
                var end = monthEnd < to ? monthEnd : to;
                list.Add(new DateRange(start, end));
                start = end.AddDays(1);
            }
            return list;
        }
    }
}
