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
            
            var targetEntries = rawEntries.Where(e => accountIds.Contains(e.AccountId)).ToList();

            foreach (var accountId in accountIds)
            {
                var accountEntries = targetEntries.Where(e => e.AccountId == accountId).ToList();

                // Lấy chi tiết công nợ hoặc lấy trực tiếp từ JournalEntry nếu không có
                var flattenedEntries = accountEntries.SelectMany(e => 
                {
                    if (e.DebtDetails != null && e.DebtDetails.Count > 0)
                    {
                        return e.DebtDetails.Select(d => new {
                            PartnerId = d.PartnerId ?? string.Empty,
                            Dbcr = e.Dbcr,
                            Amount = d.Amount ?? (e.Amount ?? 0),
                            VoucherDate = e.JournalVoucher?.VoucherDate,
                            HasVoucher = e.JournalVoucher != null
                        });
                    }
                    else
                    {
                        return new[] { new {
                            PartnerId = string.Empty,
                            Dbcr = e.Dbcr,
                            Amount = e.Amount ?? 0,
                            VoucherDate = e.JournalVoucher?.VoucherDate,
                            HasVoucher = e.JournalVoucher != null
                        } }.AsEnumerable();
                    }
                }).ToList();

                // Dư đầu kỳ
                var beginEntries = flattenedEntries.Where(e => !e.HasVoucher || e.VoucherDate < beginDate).ToList();
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
                var intEntries = flattenedEntries.Where(e => e.HasVoucher && e.VoucherDate >= beginDate && e.VoucherDate <= endDate).ToList();
                double intDebit = intEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount);
                double intCredit = intEntries.Where(e => e.Dbcr != 1).Sum(e => e.Amount);

                // Dư cuối kỳ
                var endEntries = flattenedEntries.Where(e => !e.HasVoucher || e.VoucherDate <= endDate).ToList();
                var partnerEndBalances = endEntries
                    .GroupBy(e => e.PartnerId)
                    .Select(g => new {
                        PartnerId = g.Key,
                        Debit = g.Where(x => x.Dbcr == 1).Sum(x => x.Amount),
                        Credit = g.Where(x => x.Dbcr != 1).Sum(x => x.Amount)
                    }).ToList();

                double sumEndDebit = 0;
                double sumEndCredit = 0;
                foreach (var pe in partnerEndBalances)
                {
                    if (pe.Debit > pe.Credit) sumEndDebit += (pe.Debit - pe.Credit);
                    else sumEndCredit += (pe.Credit - pe.Debit);
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
                        Splite = 0
                    });
                }
            }
            
            return results;
        }
    }
}
