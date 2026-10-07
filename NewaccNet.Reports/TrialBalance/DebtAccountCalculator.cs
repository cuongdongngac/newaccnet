using SD.LLBLGen.Pro.ORMSupportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;

namespace NewaccNet.Reports.TrialBalance
{
    public class DebtAccountCalculator : ITrialBalanceStrategy
    {
        public List<TrialBalanceEntity> CalculateLeafBalances(List<JournalEntryEntity> rawEntries, List<string> accountIds, DateTime beginDate, DateTime endDate)
        {
            var results = new List<TrialBalanceEntity>();
            
            // SỬ DỤNG TRỤC TRUNG GIAN ĐỂ LÀM PHẲNG
            var flattenedSource = FlattenedLedgerService.Flatten(rawEntries, beginDate);

            // Tạo từ điển map ngược EntryId -> JournalEntryEntity gốc để lấy DebtDetails
            var rawDict = rawEntries.ToDictionary(e => e.Id);
            
            var targetEntries = flattenedSource.Where(e => accountIds.Contains(e.AccountId)).ToList();

            foreach (var accountId in accountIds)
            {
                var accountEntries = targetEntries.Where(e => e.AccountId == accountId).ToList();

                // Lấy chi tiết công nợ hoặc lấy trực tiếp từ bảng phẳng nếu không có
                var flattenedDebtEntries = accountEntries.SelectMany(e => 
                {
                    JournalEntryEntity rawE = null;
                    rawDict.TryGetValue(e.EntryId, out rawE);

                    if (rawE != null && rawE.DebtDetails != null && rawE.DebtDetails.Count > 0)
                    {
                        return rawE.DebtDetails.Select(d => new {
                            PartnerId = d.PartnerId ?? string.Empty,
                            Dbcr = e.Dbcr,
                            Amount = (double)(d.Amount ?? ((decimal?)e.Amount ?? 0)),
                            IsOpening = e.IsOpeningBalance(beginDate),
                            VoucherDate = e.VoucherDate
                        });
                    }
                    else
                    {
                        return new[] { new {
                            PartnerId = string.Empty,
                            Dbcr = e.Dbcr,
                            Amount = (double)e.Amount,
                            IsOpening = e.IsOpeningBalance(beginDate),
                            VoucherDate = e.VoucherDate
                        } }.AsEnumerable();
                    }
                }).ToList();

                // Dư đầu kỳ
                var beginEntries = flattenedDebtEntries.Where(e => e.IsOpening).ToList();
                var partnerBeginBalances = beginEntries
                    .GroupBy(e => e.PartnerId)
                    .Select(g => new {
                        PartnerId = g.Key,
                        Debit = g.Where(x => x.Dbcr == 1).Sum(x => x.Amount),
                        Credit = g.Where(x => x.Dbcr != 1).Sum(x => x.Amount)
                    }).ToList();

                double sumBeginDebit = 0;
                double sumBeginCredit = 0;
                foreach (var pb in partnerBeginBalances)
                {
                    if (pb.Debit > pb.Credit) sumBeginDebit += (pb.Debit - pb.Credit);
                    else sumBeginCredit += (pb.Credit - pb.Debit);
                }

                // Phát sinh trong kỳ
                var intEntries = flattenedDebtEntries.Where(e => !e.IsOpening && e.VoucherDate <= endDate).ToList();
                double intDebit = intEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount);
                double intCredit = intEntries.Where(e => e.Dbcr != 1).Sum(e => e.Amount);

                // Dư cuối kỳ
                var endEntries = flattenedDebtEntries.Where(e => e.IsOpening || e.VoucherDate <= endDate).ToList();
                var partnerEndBalances = endEntries
                    .GroupBy(e => e.PartnerId)
                    .Select(g => new {
                        PartnerId = g.Key,
                        Debit = g.Where(x => x.Dbcr == 1).Sum(x => x.Amount),
                        Credit = g.Where(x => x.Dbcr != 1).Sum(x => x.Amount)
                    }).ToList();

                double sumEndDebit = 0;
                double sumEndCredit = 0;
                foreach (var pb in partnerEndBalances)
                {
                    if (pb.Debit > pb.Credit) sumEndDebit += (pb.Debit - pb.Credit);
                    else sumEndCredit += (pb.Credit - pb.Debit);
                }

                if (sumBeginDebit > 0 || sumBeginCredit > 0 || intDebit > 0 || intCredit > 0 || sumEndDebit > 0 || sumEndCredit > 0)
                {
                    results.Add(new TrialBalanceEntity
                    {
                        AccountId = accountId,
                        Begindebit = sumBeginDebit,
                        Begincredit = sumBeginCredit,
                        Intdebit = intDebit,
                        Intcredit = intCredit,
                        Enddebit = sumEndDebit,
                        Endcredit = sumEndCredit,
                        IsSummary = false
                    });
                }
            }

            return results;
        }
    }
}
