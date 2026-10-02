using System;

namespace NewaccNet.Wpf.AppSystem.Helpers
{
    public static class NumberHelper
    {
        private static readonly string[] unitNumbers = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] placeValues = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ", "tỷ tỷ" };

        public static string NumberToWord(decimal amount)
        {
            amount = Math.Truncate(amount);
            if (amount == 0) return "Không đồng.";

            bool isNegative = amount < 0;
            decimal number = Math.Abs(amount);

            string result = "";
            int placeValueIndex = 0;

            while (number > 0)
            {
                int group = (int)(number % 1000);
                number = Math.Truncate(number / 1000);

                if (group > 0)
                {
                    string groupText = ReadGroup(group, number > 0);
                    result = groupText + " " + placeValues[placeValueIndex] + " " + result;
                }
                placeValueIndex++;
            }

            result = result.Trim();
            
            // Xóa khoảng trắng thừa
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }

            if (result.Length > 0)
                result = char.ToUpper(result[0]) + result.Substring(1);

            if (isNegative)
                result = "Âm " + char.ToLower(result[0]) + result.Substring(1);

            return result + " đồng.";
        }

        private static string ReadGroup(int number, bool hasMorePlaceValue)
        {
            int h = number / 100;
            int t = (number % 100) / 10;
            int u = number % 10;

            string res = "";

            if (h > 0 || hasMorePlaceValue)
            {
                res += unitNumbers[h] + " trăm ";
            }

            if (t == 0)
            {
                if (u > 0 && (h > 0 || hasMorePlaceValue))
                {
                    res += "lẻ ";
                }
            }
            else if (t == 1)
            {
                res += "mười ";
            }
            else
            {
                res += unitNumbers[t] + " mươi ";
            }

            if (u == 1 && t > 1)
            {
                res += "mốt ";
            }
            else if (u == 4 && t > 1)
            {
                res += "tư ";
            }
            else if (u == 5 && t > 0)
            {
                res += "lăm ";
            }
            else if (u > 0)
            {
                res += unitNumbers[u] + " ";
            }

            return res.Trim();
        }
    }
}
