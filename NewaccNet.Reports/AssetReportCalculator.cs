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
    }
}
