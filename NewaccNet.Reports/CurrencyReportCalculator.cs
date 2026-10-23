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
    /// Bộ máy tính toán Báo cáo Ngoại tệ (Tổng hợp ngoại tệ & Sổ chi tiết ngoại tệ).
    /// Khai thác bảng phẳng CurrencyFlatRow từ FlattenedDetailService.
    /// </summary>
    public class CurrencyReportCalculator
    {
        public static List<CurrencyReportDTO> CalculateSummary(
            IDataAccessAdapter adapter,
            string? accountPrefix,
            DateTime fromDate,
            DateTime toDate,
            List<string>? currencyIds = null,
            bool onlyBooked = true)
        {
            var result = new List<CurrencyReportDTO>();

            // 1. Nạp toàn bộ dòng phẳng ngoại tệ đến hết ngày toDate
            var rawRows = FlattenedDetailService.LoadCurrencyFlatRows(
                adapter,
                accountPrefix: string.IsNullOrWhiteSpace(accountPrefix) || accountPrefix == "*" ? null : accountPrefix.Trim(),
                currencyId: null,
                toDate: toDate,
                includeOpeningBeforeFromDate: true);

            if (rawRows == null || rawRows.Count == 0)
                return result;

            // 2. Lọc theo danh sách Ngoại tệ nếu người dùng chọn
            if (currencyIds != null && currencyIds.Count > 0)
            {
                var curSet = currencyIds
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (curSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => curSet.Contains(r.CurrencyId)).ToList();
                }
            }

            // 3. Lấy từ điển Tên ngoại tệ từ bảng Currency
            var qf = new QueryFactory();
            var currencyDict = adapter.FetchQuery(qf.Currency)
                .Cast<CurrencyEntity>()
                .ToDictionary(c => c.CurrencyId.Trim(), c => c.CurrencyName?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            // 4. Tách thành Đầu kỳ & Trong kỳ
            var beginRows = rawRows.Where(r => r.IsOpeningAt(fromDate)).ToList();
            var periodRows = rawRows.Where(r => r.IsInPeriod(fromDate, toDate)).ToList();

            // Lấy danh sách tất cả mã ngoại tệ phát sinh
            var allCurrencyIds = rawRows
                .Select(r => r.CurrencyId?.Trim() ?? string.Empty)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();

            int sttCounter = 1;

            foreach (var curId in allCurrencyIds)
            {
                currencyDict.TryGetValue(curId, out var curName);
                if (string.IsNullOrWhiteSpace(curName))
                {
                    curName = curId;
                }

                // Tính số dư đầu kỳ
                var curBegin = beginRows.Where(r => string.Equals(r.CurrencyId, curId, StringComparison.OrdinalIgnoreCase)).ToList();
                double netBeginQty = curBegin.Sum(r => r.SignedQuantity);
                decimal netBeginAmount = (decimal)curBegin.Sum(r => r.SignedAmount);

                // Tính phát sinh trong kỳ: Thu (Dbcr = 1) / Chi (Dbcr != 1)
                var curPeriod = periodRows.Where(r => string.Equals(r.CurrencyId, curId, StringComparison.OrdinalIgnoreCase)).ToList();
                double receiptQty = curPeriod.Where(r => r.Dbcr == 1).Sum(r => r.Quantity);
                decimal receiptAmount = (decimal)curPeriod.Where(r => r.Dbcr == 1).Sum(r => r.Amount);

                double issueQty = curPeriod.Where(r => r.Dbcr != 1).Sum(r => r.Quantity);
                decimal issueAmount = (decimal)curPeriod.Where(r => r.Dbcr != 1).Sum(r => r.Amount);

                // Tính số dư cuối kỳ
                double endQty = netBeginQty + receiptQty - issueQty;
                decimal endAmount = netBeginAmount + receiptAmount - issueAmount;

                // Bỏ qua nếu hoàn toàn không có số dư và phát sinh
                if (netBeginQty == 0 && netBeginAmount == 0 &&
                    receiptQty == 0 && receiptAmount == 0 &&
                    issueQty == 0 && issueAmount == 0 &&
                    endQty == 0 && endAmount == 0)
                {
                    continue;
                }

                // Dựng danh sách chứng từ chi tiết phục vụ drill-down
                var detailList = new List<CurrencyDetailReportDTO>();
                int dStt = 1;
                foreach (var pr in curPeriod.OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNo))
                {
                    double prReceiptQty = pr.Dbcr == 1 ? pr.Quantity : 0.0;
                    decimal prReceiptAmount = pr.Dbcr == 1 ? (decimal)pr.Amount : 0m;

                    double prIssueQty = pr.Dbcr != 1 ? pr.Quantity : 0.0;
                    decimal prIssueAmount = pr.Dbcr != 1 ? (decimal)pr.Amount : 0m;

                    detailList.Add(new CurrencyDetailReportDTO
                    {
                        Stt = dStt++,
                        JournalVoucherId = pr.JournalVoucherId,
                        VoucherDate = pr.VoucherDate,
                        VoucherNo = pr.VoucherNo,
                        Contents = pr.Contents,
                        AccountId = pr.AccountId,
                        CounterAccountId = pr.CounterAccountId,
                        CurrencyId = curId,
                        CurrencyName = curName,
                        ExchangeRate = pr.ExchangeRate,
                        ReceiptQuantity = prReceiptQty,
                        ReceiptAmount = prReceiptAmount,
                        IssueQuantity = prIssueQty,
                        IssueAmount = prIssueAmount
                    });
                }

                result.Add(new CurrencyReportDTO
                {
                    Stt = (sttCounter++).ToString(),
                    CurrencyId = curId,
                    CurrencyName = curName,
                    BeginQuantity = netBeginQty,
                    BeginAmount = netBeginAmount,
                    ReceiptQuantity = receiptQty,
                    ReceiptAmount = receiptAmount,
                    IssueQuantity = issueQty,
                    IssueAmount = issueAmount,
                    EndQuantity = endQty,
                    EndAmount = endAmount,
                    Details = detailList
                });
            }

            return result;
        }

        /// <summary>
        /// Tạo danh sách dòng Sổ chi tiết Ngoại tệ từ dòng DTO tổng hợp ngoại tệ được click drill-down.
        /// </summary>
        public static List<CurrencyDetailReportDTO> CalculateDetailFromSummary(CurrencyReportDTO summaryItem)
        {
            var list = new List<CurrencyDetailReportDTO>();
            if (summaryItem == null) return list;

            double runningQty = summaryItem.BeginQuantity;
            decimal runningAmount = summaryItem.BeginAmount;

            // 1. Dòng số dư đầu kỳ
            list.Add(new CurrencyDetailReportDTO
            {
                Stt = 0,
                Contents = "Số dư đầu kỳ",
                CurrencyId = summaryItem.CurrencyId,
                CurrencyName = summaryItem.CurrencyName,
                ClosingQuantity = runningQty,
                ClosingAmount = runningAmount,
                IsOpeningRow = true
            });

            // 2. Các dòng phát sinh trong kỳ với số dư lũy kế
            int stt = 1;
            foreach (var item in summaryItem.Details)
            {
                runningQty += (item.ReceiptQuantity - item.IssueQuantity);
                runningAmount += (item.ReceiptAmount - item.IssueAmount);

                list.Add(new CurrencyDetailReportDTO
                {
                    Stt = stt++,
                    JournalVoucherId = item.JournalVoucherId,
                    VoucherDate = item.VoucherDate,
                    VoucherNo = item.VoucherNo,
                    Contents = item.Contents,
                    AccountId = item.AccountId,
                    CounterAccountId = item.CounterAccountId,
                    CurrencyId = item.CurrencyId,
                    CurrencyName = item.CurrencyName,
                    ExchangeRate = item.ExchangeRate,
                    ReceiptQuantity = item.ReceiptQuantity,
                    ReceiptAmount = item.ReceiptAmount,
                    IssueQuantity = item.IssueQuantity,
                    IssueAmount = item.IssueAmount,
                    ClosingQuantity = runningQty,
                    ClosingAmount = runningAmount,
                    IsOpeningRow = false
                });
            }

            return list;
        }
    }
}
