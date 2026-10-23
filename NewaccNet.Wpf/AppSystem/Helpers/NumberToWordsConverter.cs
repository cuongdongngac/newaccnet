using System;
using System.Text.RegularExpressions;

namespace NewaccNet.Wpf.AppSystem.Helpers
{
    public static class NumberToWordsConverter
    {
        private static readonly string[] DigitWords = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] ScaleWords = { "", "nghìn", "triệu", "tỷ", "nghìn tỷ", "triệu tỷ", "tỷ tỷ" };

        /// <summary>
        /// Chuyển đổi số tiền sang chuỗi đọc bằng chữ tiếng Việt (VD: 100,000 -> Một trăm nghìn đồng chẵn).
        /// </summary>
        public static string ConvertToWords(decimal amount, string currencySuffix = "đồng")
        {
            long number = (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
            if (number == 0) return $"Không {currencySuffix}".Trim();

            string numberString = number.ToString();
            bool isNegative = false;

            if (numberString.StartsWith("-"))
            {
                isNegative = true;
                numberString = numberString.Substring(1);
            }

            int groupCount = (int)Math.Ceiling(numberString.Length / 3.0);
            numberString = numberString.PadLeft(groupCount * 3, '0');

            string result = "";

            for (int i = 0; i < groupCount; i++)
            {
                int scaleIndex = groupCount - i - 1;
                string currentBlock = numberString.Substring(i * 3, 3);

                int hundred = currentBlock[0] - '0';
                int ten = currentBlock[1] - '0';
                int unit = currentBlock[2] - '0';

                if (hundred == 0 && ten == 0 && unit == 0)
                {
                    continue;
                }

                string blockText = "";
                bool isFirstBlock = (i == 0);

                // 1. Hàng trăm
                if (!isFirstBlock || hundred != 0)
                {
                    blockText += DigitWords[hundred] + " trăm ";
                }

                // 2. Hàng chục
                if (ten == 0)
                {
                    if (unit != 0 && (!isFirstBlock || hundred != 0))
                    {
                        blockText += "lẻ ";
                    }
                }
                else if (ten == 1)
                {
                    blockText += "mười ";
                }
                else
                {
                    blockText += DigitWords[ten] + " mươi ";
                }

                // 3. Hàng đơn vị
                if (unit != 0)
                {
                    if (unit == 1 && ten > 1)
                    {
                        blockText += "mốt";
                    }
                    else if (unit == 5 && ten > 0)
                    {
                        blockText += "lăm";
                    }
                    else if (unit == 4 && ten > 1)
                    {
                        blockText += "tư";
                    }
                    else
                    {
                        blockText += DigitWords[unit];
                    }
                }

                int safeScaleIndex = scaleIndex % ScaleWords.Length;
                result += blockText.Trim() + " " + ScaleWords[safeScaleIndex] + " ";
            }

            if (isNegative) result = "Âm " + result;

            result = Regex.Replace(result.Trim(), @"\s+", " ");

            if (result.Length > 0)
            {
                result = char.ToUpper(result[0]) + result.Substring(1);
            }

            if (!string.IsNullOrWhiteSpace(currencySuffix))
            {
                result += " " + currencySuffix;
            }

            return result.Trim();
        }
    }
}
