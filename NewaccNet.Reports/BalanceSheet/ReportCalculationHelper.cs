using System;

namespace NewaccNet.Reports.BalanceSheet
{
    public static class ReportCalculationHelper
    {
        /// <summary>
        /// Xác định công nợ là Ngắn hạn (1) hay Dài hạn (2) dựa vào khoảng cách ngày.
        /// Mặc định (0) được dùng khi không cần chia kỳ hạn.
        /// </summary>
        public static int GetLongtermFlag(DateTime currentDate, DateTime? endDate)
        {
            if (!endDate.HasValue)
                return 2; // Trả về Dài hạn (tương đương logic IsNull -> Năm 3000)

            TimeSpan diff = currentDate - endDate.Value;

            // Đã quá hạn -> Ngắn hạn
            if (diff.TotalDays > 0)
                return 1;

            // Chưa đến hạn -> Xét xem còn cách bao xa
            double daysUntilDue = Math.Abs(diff.TotalDays);
            
            if (daysUntilDue <= 365)
                return 1; // Trong vòng 1 năm -> Ngắn hạn
            else
                return 2; // Trên 1 năm -> Dài hạn
        }

        /// <summary>
        /// Hàm bóc tách số dư Nợ (Dùng tương đương hàm DebitValue trong Access)
        /// </summary>
        public static decimal GetDebitValue(decimal netBalance)
        {
            return netBalance > 0 ? netBalance : 0m;
        }

        /// <summary>
        /// Hàm bóc tách số dư Có (Dùng tương đương hàm CreditValue trong Access)
        /// </summary>
        public static decimal GetCreditValue(decimal netBalance)
        {
            return netBalance < 0 ? Math.Abs(netBalance) : 0m;
        }
    }
}
