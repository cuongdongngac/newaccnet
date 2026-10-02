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
            
            // Lọc các giao dịch thuộc các tài khoản thường
            var targetEntries = rawEntries.Where(e => accountIds.Contains(e.AccountId)).ToList();

            foreach (var accountId in accountIds)
            {
                var accountEntries = targetEntries.Where(e => e.AccountId == accountId).ToList();

                // Dư đầu kỳ (trước beginDate hoặc là số dư đầu kỳ không có Voucher)
                var beginEntries = accountEntries.Where(e => e.JournalVoucher == null || e.JournalVoucher.VoucherDate < beginDate).ToList();
                double sumBeginDebit = beginEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount ?? 0);
                double sumBeginCredit = beginEntries.Where(e => e.Dbcr != 1).Sum(e => e.Amount ?? 0);
                
                double netBeginDebit = 0;
                double netBeginCredit = 0;
                if (sumBeginDebit > sumBeginCredit) netBeginDebit = sumBeginDebit - sumBeginCredit;
                else netBeginCredit = sumBeginCredit - sumBeginDebit;

                // Phát sinh trong kỳ (beginDate -> endDate)
                var intEntries = accountEntries.Where(e => e.JournalVoucher != null && e.JournalVoucher.VoucherDate >= beginDate && e.JournalVoucher.VoucherDate <= endDate).ToList();
                double intDebit = intEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount ?? 0);
                double intCredit = intEntries.Where(e => e.Dbcr != 1).Sum(e => e.Amount ?? 0);

                // Dư cuối kỳ = (Dư Nợ đầu + Phát sinh Nợ) - (Dư Có đầu + Phát sinh Có)
                double totalDebit = netBeginDebit + intDebit;
                double totalCredit = netBeginCredit + intCredit;
                
                double endDebit = 0;
                double endCredit = 0;
                if (totalDebit > totalCredit) endDebit = totalDebit - totalCredit;
                else endCredit = totalCredit - totalDebit;

                // Chỉ tạo record nếu có số liệu
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
                        Splite = 0
                    });
                }
            }
            
            return results;
        }
    }
}
