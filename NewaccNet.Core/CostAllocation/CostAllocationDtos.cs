using System;
using System.Collections.Generic;

namespace NewaccNet.Core.CostAllocation
{
    /// <summary>
    /// DTO hiển thị dòng chi phí nguồn cần phân bổ (Lưới trên - [Distribute Expense SubForm])
    /// </summary>
    public class CostSourceItemDto
    {
        public string CostObjectId { get; set; } = string.Empty;
        public string CostObjectName { get; set; } = string.Empty;
        public string CostElementId { get; set; } = string.Empty;
        public string CostElementName { get; set; } = string.Empty;
        public double Amount { get; set; }
    }

    /// <summary>
    /// DTO hiển thị dòng tiêu thức và kết quả phân bổ theo từng đối tượng chi phí (Lưới dưới - [Distribute Criteria Form])
    /// </summary>
    public class CostCriteriaItemDto
    {
        public string CostObjectId { get; set; } = string.Empty;
        public string CostObjectName { get; set; } = string.Empty;
        public double BaseAmount { get; set; }
        public double RatioPercent { get; set; }
        public double AllocatedAmount { get; set; }
    }

    /// <summary>
    /// Tham số đầu vào cho quá trình tính toán và phân bổ chi phí
    /// </summary>
    public class CostAllocationParams
    {
        public DateTime FromDate { get; set; } = DateTime.Today;
        public DateTime ToDate { get; set; } = DateTime.Today;
        public string SourceAccountId { get; set; } = string.Empty;
        public string DestinationAccountId { get; set; } = string.Empty;
        public string DestinationCostElementId { get; set; } = string.Empty;
        public List<string> AllocationBaseAccountIds { get; set; } = new List<string>();
    }

    /// <summary>
    /// Kết quả tính toán phân bổ chi phí để hiển thị lên giao diện trước khi sinh chứng từ
    /// </summary>
    public class CostAllocationCalculationResult
    {
        public List<CostSourceItemDto> SourcePoolItems { get; set; } = new List<CostSourceItemDto>();
        public double TotalPoolAmount { get; set; }

        public List<CostCriteriaItemDto> CriteriaItems { get; set; } = new List<CostCriteriaItemDto>();
        public double TotalBaseAmount { get; set; }
        public double TotalAllocatedAmount { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

