using System;
using System.Collections.Generic;

namespace NewaccNet.AI.Contracts
{
    /// <summary>
    /// Thông tin mô hình AI lấy từ OpenRouter Gateway (Giá, Ngữ cảnh, Tính năng OCR/JSON).
    /// </summary>
    public class OpenRouterModelDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public int? ContextLength { get; set; }
        public decimal? PriceIn { get; set; }
        public decimal? PriceOut { get; set; }
        public bool IsFree { get; set; }
        public bool SupportsVision { get; set; }
        public bool SupportsJson { get; set; }
        public string Description { get; set; } = string.Empty;
        public long? Created { get; set; }

        public string DisplayContext
        {
            get
            {
                if (!ContextLength.HasValue || ContextLength.Value <= 0) return "—";
                if (ContextLength.Value >= 1_000_000)
                    return $"{ContextLength.Value / 1_000_000.0:0.#}M";
                return $"{Math.Round(ContextLength.Value / 1000.0)}K";
            }
        }

        public string DisplayPrice
        {
            get
            {
                if (IsFree) return "Miễn phí (Free)";
                if (!PriceIn.HasValue && !PriceOut.HasValue) return "Thay đổi linh hoạt";
                string inStr = FormatPrice(PriceIn);
                string outStr = FormatPrice(PriceOut);
                return $"Vào ${inStr} · Ra ${outStr} / 1M token";
            }
        }

        public string DisplayBadge
        {
            get
            {
                var badges = new List<string>();
                if (IsFree) badges.Add("🆓 Miễn phí");
                if (SupportsVision) badges.Add("🖼 Đọc ảnh/OCR");
                if (SupportsJson) badges.Add("⚙ JSON chuẩn");
                return string.Join("  |  ", badges);
            }
        }

        private static string FormatPrice(decimal? val)
        {
            if (!val.HasValue) return "—";
            if (val.Value == 0) return "0";
            if (val.Value < 0.01m) return val.Value.ToString("0.000");
            if (val.Value < 10m) return val.Value.ToString("0.00");
            return val.Value.ToString("0.#");
        }
    }
}
