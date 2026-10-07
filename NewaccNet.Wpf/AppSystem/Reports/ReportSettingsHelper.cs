using System;
using System.IO;
using System.Text.Json;

namespace NewaccNet.Wpf.AppSystem.Reports
{
    public class ReportFilterSettings
    {
        public DateTime? CashFlowFromDate { get; set; }
        public DateTime? CashFlowToDate { get; set; }
        public string CashFlowCurrentTitle { get; set; }
        public string CashFlowPrevTitle { get; set; }
        public string CashFlowPrevJsonPath { get; set; }

        public DateTime? KQKDFromDate { get; set; }
        public DateTime? KQKDToDate { get; set; }
        public string KQKDCurrentTitle { get; set; }
        public string KQKDPrevTitle { get; set; }
        public string KQKDPrevJsonPath { get; set; }

        public DateTime? ObligationTaxFromDate { get; set; }
        public DateTime? ObligationTaxToDate { get; set; }
        public string ObligationTaxPrevJsonPath { get; set; }
        public string ObligationTaxAccJsonPath { get; set; }
    }

    public static class ReportSettingsHelper
    {
        private static readonly string SettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NewaccNet", "ReportSettings.json");

        public static ReportFilterSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    return JsonSerializer.Deserialize<ReportFilterSettings>(json) ?? new ReportFilterSettings();
                }
            }
            catch { }
            return new ReportFilterSettings();
        }

        public static void Save(ReportFilterSettings settings)
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsFile);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }
    }
}
