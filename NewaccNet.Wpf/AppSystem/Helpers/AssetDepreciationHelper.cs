using System;

namespace NewaccNet.Wpf.AppSystem.Helpers
{
    /// <summary>
    /// Helper tính toán khấu hao tài sản cố định (Fixed Asset Depreciation Calculator).
    /// Hỗ trợ tính khấu hao theo số ngày thực tế trong kỳ [BeginDate, EndDate],
    /// tự động xử lý lệch pha (mua giữa kỳ, hết hạn giữa kỳ, hoặc nằm ngoài kỳ).
    /// </summary>
    public static class AssetDepreciationHelper
    {
        /// <summary>
        /// Tính ngày tài sản kết thúc vòng đời khấu hao dựa trên Ngày mua và Số năm sử dụng.
        /// </summary>
        /// <param name="purchaseDate">Ngày mua / bắt đầu tính khấu hao</param>
        /// <param name="usingYears">Thời gian sử dụng hữu ích (tính theo năm)</param>
        /// <returns>Ngày kết thúc vòng đời khấu hao</returns>
        public static DateTime GetAssetLifeEndDate(DateTime purchaseDate, double usingYears)
        {
            if (usingYears <= 0) return purchaseDate.Date;
            int totalMonths = (int)Math.Round(usingYears * 12);
            return purchaseDate.Date.AddMonths(totalMonths);
        }

        /// <summary>
        /// Tính số ngày thực tế được trích khấu hao trong kỳ [beginDate, endDate].
        /// </summary>
        /// <param name="beginDate">Ngày bắt đầu kỳ tính khấu hao</param>
        /// <param name="endDate">Ngày kết thúc kỳ tính khấu hao</param>
        /// <param name="purchaseDate">Ngày mua tài sản</param>
        /// <param name="usingYears">Thời gian sử dụng (năm)</param>
        /// <param name="includeLastDay">Có tính cả ngày cuối cùng không (mặc định true)</param>
        /// <returns>Số ngày khấu hao thực tế (>= 0)</returns>
        public static int GetDepreciationDays(
            DateTime beginDate,
            DateTime endDate,
            DateTime purchaseDate,
            double usingYears,
            bool includeLastDay = true)
        {
            if (usingYears <= 0 || beginDate > endDate)
                return 0;

            // Ngày kết thúc vòng đời tài sản
            DateTime assetLifeEndDate = GetAssetLifeEndDate(purchaseDate, usingYears);

            // Hiệu chỉnh 2 mốc thời gian (xử lý lệch pha):
            // - Nếu beginDate sớm hơn ngày mua -> bắt đầu tính từ ngày mua
            DateTime actualBeginDate = beginDate.Date < purchaseDate.Date ? purchaseDate.Date : beginDate.Date;

            // - Nếu endDate vượt quá ngày kết thúc khấu hao -> kết thúc tại ngày kết thúc khấu hao
            DateTime actualEndDate = endDate.Date > assetLifeEndDate ? assetLifeEndDate : endDate.Date;

            // Nếu sau khi hiệu chỉnh mà Ngày bắt đầu > Ngày kết thúc (chưa mua hoặc đã hết hạn trước kỳ)
            if (actualBeginDate > actualEndDate)
                return 0;

            int days = (actualEndDate - actualBeginDate).Days + (includeLastDay ? 1 : 0);
            return days > 0 ? days : 0;
        }

        /// <summary>
        /// Tính số tiền trích khấu hao của một tài sản trong khoảng thời gian [beginDate, endDate].
        /// </summary>
        /// <param name="beginDate">Ngày bắt đầu kỳ tính (Từ ngày)</param>
        /// <param name="endDate">Ngày kết thúc kỳ tính (Đến ngày)</param>
        /// <param name="purchaseDate">Ngày mua tài sản (T)</param>
        /// <param name="usingYears">Thời gian sử dụng hữu ích (số năm)</param>
        /// <param name="depRate">Tỷ lệ khấu hao năm (ví dụ: 0.2 tương ứng 20%/năm)</param>
        /// <param name="amount">Nguyên giá tài sản</param>
        /// <param name="includeLastDay">Có tính cả ngày cuối cùng không (mặc định true)</param>
        /// <returns>Số tiền trích khấu hao trong kỳ (đã làm tròn nguyên đồng)</returns>
        public static decimal CalculateDepreciation(
            DateTime beginDate,
            DateTime endDate,
            DateTime purchaseDate,
            double usingYears,
            double depRate,
            decimal amount,
            bool includeLastDay = true)
        {
            if (amount <= 0 || depRate <= 0)
                return 0m;

            int actualDays = GetDepreciationDays(beginDate, endDate, purchaseDate, usingYears, includeLastDay);
            if (actualDays <= 0)
                return 0m;

            // Công thức: Amount * (DepRate * Days) / 365
            decimal ratePerDay = (decimal)depRate / 365m;
            decimal depreciationAmount = amount * ratePerDay * actualDays;

            return Math.Round(depreciationAmount, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Quá tải hỗ trợ kiểu double cho Nguyên giá (tự động chuyển đổi sang decimal).
        /// </summary>
        public static double CalculateDepreciation(
            DateTime beginDate,
            DateTime endDate,
            DateTime purchaseDate,
            double usingYears,
            double depRate,
            double amount,
            bool includeLastDay = true)
        {
            if (amount <= 0) return 0.0;
            return (double)CalculateDepreciation(
                beginDate, 
                endDate, 
                purchaseDate, 
                usingYears, 
                depRate, 
                (decimal)amount, 
                includeLastDay);
        }
    }
}
