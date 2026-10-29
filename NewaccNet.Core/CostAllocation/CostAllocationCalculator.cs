using System;
using System.Collections.Generic;
using System.Linq;

namespace NewaccNet.Core.CostAllocation
{
    /// <summary>
    /// MẢNH GHÉP 3: Tính toán Tỷ trọng bình quân gia quyền (%) và Số tiền phân bổ cho từng đối tượng.
    /// Công thức gốc Access ([Distribute Criteria Form]):
    ///   - Tỷ trọng % = [Amount] / [Total]
    ///   - Số tiền phân bổ = ([Amount] / [Total]) * Forms![Distribute Ground Form]![Total]
    /// </summary>
    public class CostAllocationCalculator
    {
        private const double Epsilon = 0.0001;

        /// <summary>
        /// Tính tỷ lệ phần trăm và số tiền phân bổ trên danh sách đối tượng tiêu thức,
        /// đồng thời điều chỉnh chênh lệch làm tròn đồng lẻ vào đối tượng có giá trị lớn nhất.
        /// </summary>
        public double ApplyWeightedAllocation(
            List<CostCriteriaItemDto> criteriaItems,
            double totalPoolAmount,
            double totalBaseAmount)
        {
            if (criteriaItems == null || criteriaItems.Count == 0 || Math.Abs(totalBaseAmount) <= Epsilon)
            {
                return 0.0;
            }

            double runningAllocated = 0;

            for (int i = 0; i < criteriaItems.Count; i++)
            {
                var item = criteriaItems[i];
                item.RatioPercent = item.BaseAmount / totalBaseAmount;
                item.AllocatedAmount = Math.Round(item.RatioPercent * totalPoolAmount, 0, MidpointRounding.AwayFromZero);
                runningAllocated += item.AllocatedAmount;
            }

            // Điều chỉnh chênh lệch làm tròn đồng lẻ (Rounding Adjustment)
            // để đảm bảo Tổng phân bổ khớp tuyệt đối 100% với Tổng chi phí nguồn (TotalPoolAmount)
            double roundedPool = Math.Round(totalPoolAmount, 0, MidpointRounding.AwayFromZero);
            double diff = roundedPool - runningAllocated;
            if (Math.Abs(diff) > Epsilon && criteriaItems.Count > 0)
            {
                var largestItem = criteriaItems
                    .OrderByDescending(x => Math.Abs(x.AllocatedAmount))
                    .First();
                largestItem.AllocatedAmount += diff;
            }

            return criteriaItems.Sum(x => x.AllocatedAmount);
        }
    }
}

