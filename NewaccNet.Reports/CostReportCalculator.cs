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
    /// Bộ máy tính toán Báo cáo Chi phí (Tổng hợp chi phí và Sổ chi tiết chi phí).
    /// Khai thác bảng phẳng CostFlatRow từ FlattenedDetailService.
    /// </summary>
    public class CostReportCalculator
    {
        public static List<CostReportDTO> CalculateSummary(
            IDataAccessAdapter adapter,
            string accountPrefix,
            DateTime fromDate,
            DateTime toDate,
            List<string>? costObjectIds = null,
            List<string>? costElementIds = null,
            bool onlyBooked = true)
        {
            var result = new List<CostReportDTO>();

            // 1. Nạp toàn bộ dòng phẳng chi phí đến hết ngày toDate
            var rawRows = FlattenedDetailService.LoadCostFlatRows(
                adapter,
                accountPrefix: string.IsNullOrWhiteSpace(accountPrefix) || accountPrefix == "*" ? null : accountPrefix.Trim(),
                toDate: toDate,
                includeOpeningBeforeFromDate: true);

            if (rawRows == null || rawRows.Count == 0)
                return result;

            // 2. Lọc theo Đối tượng chi phí nếu người dùng chọn
            if (costObjectIds != null && costObjectIds.Count > 0)
            {
                var objSet = costObjectIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (objSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => objSet.Contains(r.CostObjectId)).ToList();
                }
            }

            // 3. Lọc theo Yếu tố chi phí nếu người dùng chọn
            if (costElementIds != null && costElementIds.Count > 0)
            {
                var elemSet = costElementIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (elemSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => elemSet.Contains(r.CostElementId)).ToList();
                }
            }

            // 4. Lấy từ điển tên Đối tượng CP và Yếu tố CP
            var qf = new QueryFactory();
            var costObjectDict = adapter.FetchQuery(qf.CostObject)
                .Cast<CostObjectEntity>()
                .ToDictionary(o => o.Id.Trim(), o => o.ObjectName?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            var costElementDict = adapter.FetchQuery(qf.CostElement)
                .Cast<CostElementEntity>()
                .ToDictionary(e => e.Id.Trim(), e => e.ElementName?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            // 5. Tách thành Đầu kỳ & Trong kỳ
            var beginRows = rawRows.Where(r => r.IsOpeningAt(fromDate)).ToList();
            var periodRows = rawRows.Where(r => r.IsInPeriod(fromDate, toDate)).ToList();

            // Gom nhóm tất cả cặp (CostObjectId, CostElementId) có phát sinh
            var allKeys = rawRows
                .Select(r => (CostObjectId: r.CostObjectId?.Trim() ?? string.Empty, CostElementId: r.CostElementId?.Trim() ?? string.Empty))
                .Distinct()
                .OrderBy(k => k.CostObjectId)
                .ThenBy(k => k.CostElementId)
                .ToList();

            // Gom nhóm theo CostObjectId
            var groupedByObject = allKeys.GroupBy(k => k.CostObjectId).ToList();
            int objectIndex = 1;

            foreach (var objGroup in groupedByObject)
            {
                string objId = objGroup.Key;
                costObjectDict.TryGetValue(objId, out var objName);
                if (string.IsNullOrWhiteSpace(objName))
                {
                    objName = string.IsNullOrWhiteSpace(objId) ? "(Không xác định ĐTCP)" : objId;
                }

                var objectItemRows = new List<CostReportDTO>();
                decimal objBeginDr = 0m;
                decimal objBeginCr = 0m;
                decimal objDebit = 0m;
                decimal objCredit = 0m;
                decimal objEndDr = 0m;
                decimal objEndCr = 0m;
                var allObjDetails = new List<CostDetailReportDTO>();

                foreach (var key in objGroup)
                {
                    string elemId = key.CostElementId;
                    costElementDict.TryGetValue(elemId, out var elemName);
                    if (string.IsNullOrWhiteSpace(elemName))
                    {
                        elemName = string.IsNullOrWhiteSpace(elemId) ? "(Không xác định YTCP)" : elemId;
                    }

                    // Tính đầu kỳ của cặp (objId, elemId)
                    var elBegin = beginRows.Where(r => string.Equals(r.CostObjectId, objId, StringComparison.OrdinalIgnoreCase) &&
                                                      string.Equals(r.CostElementId, elemId, StringComparison.OrdinalIgnoreCase)).ToList();
                    decimal netBegin = (decimal)elBegin.Sum(r => r.SignedAmount);
                    decimal elBeginDr = netBegin > 0 ? netBegin : 0m;
                    decimal elBeginCr = netBegin < 0 ? -netBegin : 0m;

                    // Tính trong kỳ của cặp (objId, elemId)
                    var elPeriod = periodRows.Where(r => string.Equals(r.CostObjectId, objId, StringComparison.OrdinalIgnoreCase) &&
                                                        string.Equals(r.CostElementId, elemId, StringComparison.OrdinalIgnoreCase)).ToList();
                    decimal elDebit = (decimal)elPeriod.Where(r => r.Dbcr == 1).Sum(r => r.Amount);
                    decimal elCredit = (decimal)elPeriod.Where(r => r.Dbcr != 1).Sum(r => r.Amount);

                    // Tính cuối kỳ
                    decimal netEnd = netBegin + elDebit - elCredit;
                    decimal elEndDr = netEnd > 0 ? netEnd : 0m;
                    decimal elEndCr = netEnd < 0 ? -netEnd : 0m;

                    // Bỏ qua nếu hoàn toàn không có số dư và phát sinh
                    if (elBeginDr == 0 && elBeginCr == 0 && elDebit == 0 && elCredit == 0 && elEndDr == 0 && elEndCr == 0)
                    {
                        continue;
                    }

                    // Dựng chi tiết chứng từ phục vụ drill-down
                    var elDetails = new List<CostDetailReportDTO>();
                    int dStt = 1;
                    foreach (var pr in elPeriod.OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNo))
                    {
                        elDetails.Add(new CostDetailReportDTO
                        {
                            Stt = dStt++,
                            JournalVoucherId = pr.JournalVoucherId,
                            VoucherDate = pr.VoucherDate,
                            VoucherNo = pr.VoucherNo,
                            Contents = pr.Contents,
                            AccountId = pr.AccountId,
                            CounterAccountId = pr.CounterAccountId,
                            CostObjectId = objId,
                            CostObjectName = objName,
                            CostElementId = elemId,
                            CostElementName = elemName,
                            DebitAmount = pr.Dbcr == 1 ? (decimal)pr.Amount : 0m,
                            CreditAmount = pr.Dbcr != 1 ? (decimal)pr.Amount : 0m
                        });
                    }

                    var elemDto = new CostReportDTO
                    {
                        Stt = string.Empty,
                        CostObjectId = objId,
                        CostObjectName = objName,
                        CostElementId = elemId,
                        CostElementName = elemName,
                        DisplayName = $"   {elemName}",
                        IsGroupHeader = false,
                        BeginDebitAmount = elBeginDr,
                        BeginCreditAmount = elBeginCr,
                        DebitAmount = elDebit,
                        CreditAmount = elCredit,
                        EndDebitAmount = elEndDr,
                        EndCreditAmount = elEndCr,
                        Details = elDetails
                    };

                    objectItemRows.Add(elemDto);
                    allObjDetails.AddRange(elDetails);

                    objBeginDr += elBeginDr;
                    objBeginCr += elBeginCr;
                    objDebit += elDebit;
                    objCredit += elCredit;
                    objEndDr += elEndDr;
                    objEndCr += elEndCr;
                }

                if (objectItemRows.Count > 0)
                {
                    result.AddRange(objectItemRows);
                    objectIndex++;
                }
            }

            return result;
        }

        /// <summary>
        /// Tạo danh sách dòng Sổ chi tiết Chi phí từ dòng DTO tổng hợp chi phí được click drill-down.
        /// </summary>
        public static List<CostDetailReportDTO> CalculateDetailFromSummary(CostReportDTO summaryItem)
        {
            var list = new List<CostDetailReportDTO>();
            if (summaryItem == null) return list;

            decimal runningBalance = summaryItem.BeginDebitAmount - summaryItem.BeginCreditAmount;

            // Dòng số dư đầu kỳ
            list.Add(new CostDetailReportDTO
            {
                Stt = 0,
                Contents = "Số dư đầu kỳ",
                CostObjectId = summaryItem.CostObjectId,
                CostObjectName = summaryItem.CostObjectName,
                CostElementId = summaryItem.CostElementId,
                CostElementName = summaryItem.CostElementName,
                ClosingDebitAmount = runningBalance > 0 ? runningBalance : 0m,
                ClosingCreditAmount = runningBalance < 0 ? -runningBalance : 0m,
                IsOpeningRow = true
            });

            int stt = 1;
            foreach (var item in summaryItem.Details)
            {
                runningBalance += (item.DebitAmount - item.CreditAmount);

                list.Add(new CostDetailReportDTO
                {
                    Stt = stt++,
                    JournalVoucherId = item.JournalVoucherId,
                    VoucherDate = item.VoucherDate,
                    VoucherNo = item.VoucherNo,
                    Contents = item.Contents,
                    AccountId = item.AccountId,
                    CounterAccountId = item.CounterAccountId,
                    CostObjectId = item.CostObjectId,
                    CostObjectName = item.CostObjectName,
                    CostElementId = item.CostElementId,
                    CostElementName = item.CostElementName,
                    DebitAmount = item.DebitAmount,
                    CreditAmount = item.CreditAmount,
                    ClosingDebitAmount = runningBalance > 0 ? runningBalance : 0m,
                    ClosingCreditAmount = runningBalance < 0 ? -runningBalance : 0m,
                    IsOpeningRow = false
                });
            }

            return list;
        }
    }
}
