using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using NewaccNet.Reports.FlattenedDetails;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Bộ máy tính toán Báo cáo Tổng hợp & Chi tiết Tài sản cố định (Bảng Tăng - Giảm - Tồn TSCĐ).
    /// </summary>
    public class AssetReportCalculator
    {
        public static List<AssetSummaryReportDTO> CalculateSummary(
            IDataAccessAdapter adapter,
            string accountPrefix,
            DateTime fromDate,
            DateTime toDate,
            List<string>? assetIds = null,
            List<string>? reasonIds = null,
            bool onlyBooked = true)
        {
            var result = new List<AssetSummaryReportDTO>();

            // 1. Nạp toàn bộ dòng phẳng của TSCĐ đến hết kỳ báo cáo
            var rawRows = FlattenedDetailService.LoadAssetFlatRows(
                adapter,
                accountPrefix: string.IsNullOrWhiteSpace(accountPrefix) || accountPrefix == "*" ? null : accountPrefix.Trim(),
                toDate: toDate,
                includeOpeningBeforeFromDate: true);

            if (rawRows == null || rawRows.Count == 0)
                return result;

            // Lọc theo Tài sản nếu người dùng có chọn cụ thể
            if (assetIds != null && assetIds.Count > 0)
            {
                var assetSet = assetIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (assetSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => assetSet.Contains(r.AssetId)).ToList();
                }
            }

            // Lọc theo Lý do tăng giảm nếu có chọn
            if (reasonIds != null && reasonIds.Count > 0)
            {
                var reasonSet = reasonIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (reasonSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => r.IsOpening || reasonSet.Contains(r.ReasonId)).ToList();
                }
            }

            if (onlyBooked)
            {
                rawRows = rawRows.Where(r => r.IsOpening || r.VoucherDate < fromDate || true).ToList();
            }

            // 2. Nạp danh mục TSCĐ để lấy Tên tài sản và Đơn vị tính
            var qf = new QueryFactory();
            var assetDict = adapter.FetchQuery(qf.Asset)
                .Cast<AssetEntity>()
                .ToDictionary(
                    a => a.AssetId.Trim(),
                    a => new { Name = a.AssetName?.Trim() ?? string.Empty, Unit = a.Unit?.Trim() ?? string.Empty },
                    StringComparer.OrdinalIgnoreCase);

            // 3. Tách thành 2 bảng phẳng: Đầu kỳ & Trong kỳ
            // Bảng phẳng 1: Đầu kỳ
            var beginRows = rawRows.Where(r => r.IsOpeningAt(fromDate)).ToList();
            var beginGrouped = beginRows
                .GroupBy(r => r.AssetId.Trim())
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        BeginQty = (decimal)g.Sum(r => r.Dbcr * r.Quantity),
                        BeginAmount = (decimal)g.Sum(r => r.SignedAmount)
                    },
                    StringComparer.OrdinalIgnoreCase);

            // Bảng phẳng 2: Trong kỳ
            var periodRows = rawRows.Where(r => r.IsInPeriod(fromDate, toDate)).ToList();
            var periodGrouped = periodRows
                .GroupBy(r => r.AssetId.Trim())
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        InQty = (decimal)g.Where(r => r.Dbcr == 1).Sum(r => r.Quantity),
                        InAmount = (decimal)g.Where(r => r.Dbcr == 1).Sum(r => r.Amount),
                        OutQty = (decimal)g.Where(r => r.Dbcr == -1).Sum(r => r.Quantity),
                        OutAmount = (decimal)g.Where(r => r.Dbcr == -1).Sum(r => r.Amount),
                        Items = g.OrderBy(r => r.VoucherDate).ThenBy(r => r.VoucherNo).ToList()
                    },
                    StringComparer.OrdinalIgnoreCase);

            // 4. Hợp nhất (FULL OUTER JOIN) giữa Đầu kỳ và Trong kỳ
            var allAssetKeys = beginGrouped.Keys.Union(periodGrouped.Keys, StringComparer.OrdinalIgnoreCase).OrderBy(k => k).ToList();

            foreach (var assetKey in allAssetKeys)
            {
                decimal bQty = 0, bAmt = 0;
                if (beginGrouped.TryGetValue(assetKey, out var bData))
                {
                    bQty = bData.BeginQty;
                    bAmt = bData.BeginAmount;
                }

                decimal inQty = 0, inAmt = 0, outQty = 0, outAmt = 0;
                List<AssetFlatRow> details = new();
                if (periodGrouped.TryGetValue(assetKey, out var pData))
                {
                    inQty = pData.InQty;
                    inAmt = pData.InAmount;
                    outQty = pData.OutQty;
                    outAmt = pData.OutAmount;
                    details = pData.Items;
                }

                decimal endQty = bQty + inQty - outQty;
                decimal endAmt = bAmt + inAmt - outAmt;

                // Bỏ qua tài sản hoàn toàn không có số dư và không phát sinh trong kỳ
                if (bQty == 0 && bAmt == 0 && inQty == 0 && inAmt == 0 && outQty == 0 && outAmt == 0)
                    continue;

                string assetName = assetKey;
                string unit = string.Empty;
                if (assetDict.TryGetValue(assetKey, out var assetInfo))
                {
                    assetName = string.IsNullOrWhiteSpace(assetInfo.Name) ? assetKey : assetInfo.Name;
                    unit = assetInfo.Unit;
                }

                result.Add(new AssetSummaryReportDTO
                {
                    AssetId = assetKey,
                    AssetName = assetName,
                    Unit = unit,
                    BeginQty = bQty,
                    BeginAmount = bAmt,
                    InQty = inQty,
                    InAmount = inAmt,
                    OutQty = outQty,
                    OutAmount = outAmt,
                    EndQty = endQty,
                    EndAmount = endAmt,
                    Details = details
                });
            }

            return result;
        }

        public static List<AssetDetailReportDTO> CalculateDetailFromSummary(AssetSummaryReportDTO summaryItem)
        {
            var result = new List<AssetDetailReportDTO>();
            if (summaryItem == null) return result;

            decimal runningQty = summaryItem.BeginQty;
            decimal runningAmt = summaryItem.BeginAmount;

            // Dòng 1: Số dư đầu kỳ
            result.Add(new AssetDetailReportDTO
            {
                AssetId = summaryItem.AssetId,
                AssetName = summaryItem.AssetName,
                Unit = summaryItem.Unit,
                VoucherNo = "SDDK",
                Contents = "Số dư đầu kỳ",
                ClosingQty = runningQty,
                ClosingAmount = runningAmt,
                IsOpeningRow = true
            });

            // Các dòng phát sinh trong kỳ từ danh sách Details đã nạp sẵn
            if (summaryItem.Details != null && summaryItem.Details.Count > 0)
            {
                foreach (var r in summaryItem.Details.OrderBy(d => d.VoucherDate).ThenBy(d => d.VoucherNo))
                {
                    decimal dQty = r.Dbcr == 1 ? (decimal)r.Quantity : 0m;
                    decimal dAmt = r.Dbcr == 1 ? (decimal)r.Amount : 0m;
                    decimal cQty = r.Dbcr == -1 ? (decimal)r.Quantity : 0m;
                    decimal cAmt = r.Dbcr == -1 ? (decimal)r.Amount : 0m;

                    runningQty += (dQty - cQty);
                    runningAmt += (dAmt - cAmt);

                    result.Add(new AssetDetailReportDTO
                    {
                        AssetId = summaryItem.AssetId,
                        AssetName = summaryItem.AssetName,
                        Unit = summaryItem.Unit,
                        JournalVoucherId = r.JournalVoucherId,
                        VoucherDate = r.VoucherDate,
                        VoucherNo = r.VoucherNo,
                        Contents = r.Contents,
                        CounterAccountId = r.CounterAccountId,
                        ReasonId = r.ReasonId,
                        ReasonName = r.ReasonId, // Mã lý do
                        Price = (decimal)r.Price,
                        DebitQty = dQty,
                        DebitAmount = dAmt,
                        CreditQty = cQty,
                        CreditAmount = cAmt,
                        ClosingQty = runningQty,
                        ClosingAmount = runningAmt,
                        IsOpeningRow = false
                    });
                }
            }

            return result;
        }
    
        /// <summary>
        /// Tính toán Báo cáo Biến động Tài sản Cố định theo mô hình Group 2 cấp:
        ///   - Group 1: Lý do tăng/giảm (Reason)
        ///   - Group 2: Tài sản (Asset)
        ///   - Detail: Các chứng từ phát sinh chi tiết trong kỳ
        /// </summary>
        public static List<AssetMovementReportDTO> CalculateMovement(
            IDataAccessAdapter adapter,
            string? accountPrefix,
            DateTime fromDate,
            DateTime toDate,
            List<string>? reasonIds = null,
            List<string>? assetIds = null,
            List<string>? deptIds = null,
            bool onlyBooked = true)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter));

            // 1. Nạp danh sách bảng phẳng AssetFlatRow trong kỳ [fromDate, toDate]
            var flatRows = FlattenedDetailService.LoadAssetFlatRows(
                adapter,
                accountPrefix: string.IsNullOrWhiteSpace(accountPrefix) || accountPrefix == "*" ? null : accountPrefix.Trim(),
                fromDate: fromDate,
                toDate: toDate,
                includeOpeningBeforeFromDate: false);

            if (flatRows == null || flatRows.Count == 0)
            {
                return new List<AssetMovementReportDTO>();
            }

            // 2. Nạp danh mục bổ trợ (Asset, Reason, Department, AssetTracking)
            var qf = new QueryFactory();

            var assets = adapter.FetchQuery(qf.Asset)
                .Cast<AssetEntity>()
                .Where(a => !string.IsNullOrWhiteSpace(a.AssetId))
                .ToDictionary(a => a.AssetId.Trim(), a => a, StringComparer.OrdinalIgnoreCase);

            var reasons = adapter.FetchQuery(qf.Reason)
                .Cast<ReasonEntity>()
                .Where(r => !string.IsNullOrWhiteSpace(r.Reasonid))
                .ToDictionary(r => r.Reasonid.Trim(), r => r, StringComparer.OrdinalIgnoreCase);

            var trackings = adapter.FetchQuery(qf.AssetTracking)
                .Cast<AssetTrackingEntity>()
                .Where(t => !string.IsNullOrWhiteSpace(t.AssetId))
                .GroupBy(t => t.AssetId.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(t => t.Assettrackingid).First(),
                    StringComparer.OrdinalIgnoreCase);

            var depts = adapter.FetchQuery(qf.Department)
                .Cast<DepartmentEntity>()
                .Where(d => !string.IsNullOrWhiteSpace(d.Deptid))
                .ToDictionary(d => d.Deptid.Trim(), d => d.DeptName?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            // 3. Chuẩn bị bộ lọc
            var filterAssets = (assetIds != null && assetIds.Count > 0)
                ? assetIds.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : null;

            var filterReasons = (reasonIds != null && reasonIds.Count > 0)
                ? reasonIds.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : null;

            var filterDepts = (deptIds != null && deptIds.Count > 0)
                ? deptIds.Where(x => !string.IsNullOrWhiteSpace(x)).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : null;

            // 4. Ánh xạ sang DTO
            var result = new List<AssetMovementReportDTO>();

            foreach (var row in flatRows)
            {
                string aId = row.AssetId?.Trim() ?? string.Empty;
                string rId = row.ReasonId?.Trim() ?? string.Empty;

                // Lọc theo Tài sản nếu có chỉ định
                if (filterAssets != null && filterAssets.Count > 0)
                {
                    if (!filterAssets.Contains(aId)) continue;
                }

                // Lọc theo Lý do nếu có chỉ định
                if (filterReasons != null && filterReasons.Count > 0)
                {
                    if (!filterReasons.Contains(rId)) continue;
                }

                // Tra cứu thông tin Tài sản
                assets.TryGetValue(aId, out var assetEntity);
                string aName = assetEntity?.AssetName?.Trim() ?? aId;
                double? origPrice = assetEntity?.Price;
                double? timeUsing = assetEntity?.Timeusing;
                DateTime? startDate = assetEntity?.Date;

                // Tra cứu thông tin Bộ phận sử dụng
                string deptId = string.Empty;
                string deptName = string.Empty;
                if (trackings.TryGetValue(aId, out var tracking) && !string.IsNullOrWhiteSpace(tracking.Deptid))
                {
                    deptId = tracking.Deptid.Trim();
                    depts.TryGetValue(deptId, out deptName);
                    deptName ??= string.Empty;
                }

                // Lọc theo Bộ phận nếu có chỉ định
                if (filterDepts != null && filterDepts.Count > 0)
                {
                    if (!filterDepts.Contains(deptId)) continue;
                }

                // Tra cứu Lý do
                reasons.TryGetValue(rId, out var reasonEntity);
                string rName = reasonEntity?.ReasonName?.Trim() ?? (!string.IsNullOrEmpty(rId) ? rId : "Phát sinh khác");
                bool rFlag = reasonEntity?.Flag ?? (row.Dbcr == 1);

                var dto = new AssetMovementReportDTO
                {
                    ReasonId = rId,
                    ReasonName = rName,
                    ReasonFlag = rFlag,

                    AssetId = aId,
                    AssetName = aName,
                    OriginalPrice = origPrice,
                    UsingTimeYears = timeUsing,
                    AssetStartDate = startDate,
                    DepartmentId = deptId,
                    DepartmentName = deptName,
                    SourceCode = row.SourceCode,

                    DetailId = row.DetailId,
                    JournalEntryId = row.JournalEntryId,
                    JournalVoucherId = row.JournalVoucherId,
                    VoucherNo = row.VoucherNo,
                    VoucherDate = row.VoucherDate,
                    Contents = row.Contents,
                    AccountId = row.AccountId,
                    CounterAccountId = row.CounterAccountId,
                    Dbcr = row.Dbcr,
                    Quantity = row.Quantity,
                    Price = row.Price,
                    Amount = row.Amount,
                    IsOpeningRow = row.IsOpening
                };

                result.Add(dto);
            }

            // 5. Sắp xếp thứ bậc: Tăng trước (Flag=True) -> Lý do -> Tài sản -> Ngày chứng từ -> Số chứng từ
            return result
                .OrderByDescending(r => r.ReasonFlag)
                .ThenBy(r => r.ReasonId)
                .ThenBy(r => r.AssetId)
                .ThenBy(r => r.VoucherDate)
                .ThenBy(r => r.VoucherNo)
                .ToList();
        }

    }
}
