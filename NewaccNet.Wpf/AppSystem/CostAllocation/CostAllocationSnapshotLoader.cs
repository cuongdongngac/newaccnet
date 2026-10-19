using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.HelperClasses;
using NewaccNet.Reports.FlattenedDetails;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.CostAllocation
{
    /// <summary>
    /// MẢNH GHÉP 1 & 2: Dựng bảng phẳng chi phí (Tầng 2) và lọc dữ liệu Nguồn / Tiêu thức (Tầng 3).
    /// Sử dụng chung FlattenedDetailService.LoadCostFlatRows (NewaccNet.Reports.FlattenedDetails).
    /// </summary>
    public class CostAllocationSnapshotLoader
    {
        private const double Epsilon = 0.0001;

        /// <summary>
        /// Dựng bảng phẳng chi tiết chi phí Tầng 2 (CostFlatRow) thông qua FlattenedDetailService.
        /// Tương đương [Union Expense Object Line Query] trong Access.
        /// </summary>
        public List<CostFlatRow> LoadCostFlatRows(IDataAccessAdapter adapter)
        {
            return FlattenedDetailService.LoadCostFlatRows(adapter);
        }

        /// <summary>
        /// MẢNH GHÉP 1: Tập hợp Chi phí nguồn cần phân bổ (Lưới trên - [Distribute Expense SubForm]).
        /// - Lọc theo Tài khoản nguồn (SourceAccountId) và IsUpToDate(toDate) (bao gồm cả số dư đầu kỳ).
        /// - KHÔNG lọc theo TransFlag: vì số dư còn lại được tính bằng Sum(SignedAmount).
        ///   Khi đã chạy phân bổ/kết chuyển, dòng Ghi Có (Dbcr = -1) sẽ tự động bù trừ với dòng Ghi Nợ (+1)
        ///   làm số dư nguồn triệt tiêu về 0.
        /// </summary>
        public List<CostSourceItemDto> LoadSourcePool(
            List<CostFlatRow> flatRows,
            string sourceAccountId,
            DateTime toDate,
            Dictionary<string, string> costObjDict,
            Dictionary<string, string> costElemDict)
        {
            string srcAcc = sourceAccountId.Trim();

            return flatRows
                .Where(r => r.AccountId.StartsWith(srcAcc, StringComparison.OrdinalIgnoreCase))
                .Where(r => r.IsUpToDate(toDate))
                .GroupBy(r => new { r.CostObjectId, r.CostElementId })
                .Select(g =>
                {
                    double netAmt = g.Sum(x => x.SignedAmount);
                    string objId = g.Key.CostObjectId;
                    string objName = string.IsNullOrEmpty(objId)
                        ? "(Chi phí dùng chung)"
                        : (costObjDict.TryGetValue(objId, out var oName) ? oName : objId);

                    return new CostSourceItemDto
                    {
                        CostObjectId = objId,
                        CostObjectName = objName,
                        CostElementId = g.Key.CostElementId,
                        CostElementName = costElemDict.TryGetValue(g.Key.CostElementId, out var eName) ? eName : g.Key.CostElementId,
                        Amount = netAmt
                    };
                })
                .Where(x => Math.Abs(x.Amount) > Epsilon)
                .OrderBy(x => x.CostObjectId)
                .ThenBy(x => x.CostElementId)
                .ToList();
        }

        /// <summary>
        /// MẢNH GHÉP 2: Tập hợp Tiêu thức phân bổ (Lưới dưới - [Distribute Criteria Form]).
        /// Tương đương câu truy vấn trong Access:
        ///   WHERE [Bill Date] Between GetBeginDate() And GetEndDate()
        ///   GROUP BY [ExObject ID], [ExObject Name], TransFlag
        ///   HAVING TransFlag = False AND Sum(DBCR * Amount) <> 0
        /// - Chỉ lấy các giao dịch chi phí phát sinh gốc (TransFlag == false) trong kỳ [FromDate .. ToDate]
        ///   của các tài khoản thuộc danh sách tiêu thức (AllocationBaseAccountIds).
        /// </summary>
        public List<CostCriteriaItemDto> LoadCriteriaBase(
            List<CostFlatRow> flatRows,
            List<string> allocationBaseAccountIds,
            DateTime fromDate,
            DateTime toDate,
            Dictionary<string, string> costObjDict)
        {
            var baseAccSet = allocationBaseAccountIds
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .ToList();

            return flatRows
                .Where(r => r.TransFlag == false)
                .Where(r => !string.IsNullOrWhiteSpace(r.CostObjectId))
                .Where(r => r.IsInPeriod(fromDate, toDate))
                .Where(r => baseAccSet.Any(crit => r.AccountId.StartsWith(crit, StringComparison.OrdinalIgnoreCase)))
                .GroupBy(r => r.CostObjectId)
                .Select(g =>
                {
                    string objId = g.Key;
                    double baseAmt = g.Sum(x => x.SignedAmount);
                    return new CostCriteriaItemDto
                    {
                        CostObjectId = objId,
                        CostObjectName = costObjDict.TryGetValue(objId, out var oName) ? oName : objId,
                        BaseAmount = baseAmt,
                        RatioPercent = 0,
                        AllocatedAmount = 0
                    };
                })
                .Where(x => x.BaseAmount > Epsilon)
                .OrderBy(x => x.CostObjectId)
                .ToList();
        }
    }
}
