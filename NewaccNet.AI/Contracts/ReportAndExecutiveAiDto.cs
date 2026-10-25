using System;
using System.Collections.Generic;

namespace NewaccNet.AI.Contracts
{
    public class ReportQueryAiDto
    {
        public string ReportType { get; set; } = "General"; // CashFlow, TrialBalance, DebtSummary, InventorySummary, SalesSummary, CostSummary
        public string ReportTitle { get; set; } = string.Empty;
        public string? FromDate { get; set; } // yyyy-MM-dd
        public string? ToDate { get; set; }   // yyyy-MM-dd
        public string? AccountCode { get; set; }
        public string? PartnerId { get; set; }
        public string? WarehouseId { get; set; }
        public string QuerySummary { get; set; } = string.Empty;
    }

    public class ExecutiveInsightAiDto
    {
        public string Title { get; set; } = "Báo Cáo Phân Tích Tài Chính & Điều Hành";
        public string ExecutiveSummary { get; set; } = string.Empty;
        public string FinancialHealthStatus { get; set; } = "Ổn định"; // Tốt, Ổn định, Cần lưu ý, Nguy cơ
        public string CashFlowAnalysis { get; set; } = string.Empty;
        public string RevenueProfitAnalysis { get; set; } = string.Empty;
        public string DebtRiskAnalysis { get; set; } = string.Empty;
        public List<string> KeyAlerts { get; set; } = new();
        public List<string> ActionableRecommendations { get; set; } = new();
        public List<ReportMetricItemDto> KeyMetrics { get; set; } = new();
    }

    public class ReportMetricItemDto
    {
        public string MetricName { get; set; } = string.Empty;
        public string MetricValue { get; set; } = string.Empty;
        public string Benchmark { get; set; } = string.Empty;
        public string Evaluation { get; set; } = string.Empty;
    }
}
