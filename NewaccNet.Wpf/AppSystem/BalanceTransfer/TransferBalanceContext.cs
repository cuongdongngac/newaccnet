using System;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.BalanceTransfer
{
    public class TransferProgressReport
    {
        public int Percent { get; set; }
        public string StepTitle { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {(IsError ? "[LỖI] " : "")}{Message}";
        }
    }

    public class TransferBalanceContext
    {
        public AppConfig SourceConfig { get; set; }
        public AppConfig DestinationConfig { get; set; }

        public int SourceYear { get; set; } = DateTime.Now.Year;
        public int DestinationYear { get; set; } = DateTime.Now.Year + 1;
        public DateTime OpeningDate { get; set; } = new DateTime(DateTime.Now.Year + 1, 1, 1);

        // Các cờ tùy chọn phân hệ chuyển đổi (Modular Transfer)
        public bool TransferLibrary { get; set; } = true;
        public bool TransferDesign { get; set; } = true;
        public bool TransferNormal { get; set; } = true;
        public bool TransferDebt { get; set; } = true;
        public bool TransferMaterial { get; set; } = true;
        public bool TransferCost { get; set; } = true;
        public bool TransferAsset { get; set; } = true;
        public bool TransferStock { get; set; } = true;
        public bool TransferCurrency { get; set; } = true;
    }
}
