using SD.LLBLGen.Pro.ORMSupportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;

namespace NewaccNet.Reports.TrialBalance
{
    public class NormalAccountCalculator : ITrialBalanceStrategy
    {
        public List<TrialBalanceEntity> CalculateLeafBalances(List<JournalEntryEntity> rawEntries, List<string> accountIds, DateTime beginDate, DateTime endDate)
        {
            var results = new List<TrialBalanceEntity>();
            
            // SỬ DỤNG TRỤC TRUNG GIAN ĐỂ LÀM PHẲNG
            var flattenedSource = FlattenedLedgerService.Flatten(rawEntries, beginDate);
            
            // Lọc các giao dịch thuộc các tài khoản thường
            var targetEntries = flattenedSource.Where(e => accountIds.Contains(e.AccountId)).ToList();

            foreach (var accountId in accountIds)
            {
                var accountEntries = targetEntries.Where(e => e.AccountId == accountId).ToList();

                // Số dư đầu kỳ
                var beginEntries = accountEntries.Where(e => e.IsOpeningBalance(beginDate)).ToList();
                double sumBeginDebit = (double)beginEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount);
                double sumBeginCredit = (double)beginEntries.Where(e => e.Dbcr != 1).Sum(e => e.Amount);
                
                double netBeginDebit = 0;
                double netBeginCredit = 0;
                if (sumBeginDebit > sumBeginCredit) netBeginDebit = sumBeginDebit - sumBeginCredit;
                else netBeginCredit = sumBeginCredit - sumBeginDebit;

                // Phát sinh trong kỳ
                var intEntries = accountEntries.Where(e => !e.IsOpeningBalance(beginDate) && e.VoucherDate <= endDate).ToList();
                double intDebit = (double)intEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount);
                double intCredit = (double)intEntries.Where(e => e.Dbcr != 1).Sum(e => e.Amount);

                double totalDebit = netBeginDebit + intDebit;
                double totalCredit = netBeginCredit + intCredit;
                
                double endDebit = 0;
                double endCredit = 0;
                if (totalDebit > totalCredit) endDebit = totalDebit - totalCredit;
                else endCredit = totalCredit - totalDebit;

                if (netBeginDebit > 0 || netBeginCredit > 0 || intDebit > 0 || intCredit > 0 || endDebit > 0 || endCredit > 0)
                {
                    results.Add(new TrialBalanceEntity
                    {
                        AccountId = accountId,
                        Begindebit = netBeginDebit,
                        Begincredit = netBeginCredit,
                        Intdebit = intDebit,
                        Intcredit = intCredit,
                        Enddebit = endDebit,
                        Endcredit = endCredit,
                        IsSummary = false
                    });
                }
            }

            return results;
        }
    }
}
