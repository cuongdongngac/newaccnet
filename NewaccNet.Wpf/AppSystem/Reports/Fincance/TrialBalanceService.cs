using System;
using System.Linq;
using System.Collections.Generic;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Reports.TrialBalance;

namespace NewaccNet.Wpf.AppSystem.Reports.Fincance
{
    public class TrialBalanceService
    {
        private readonly NormalAccountCalculator _normalCalculator;
        private readonly DebtAccountCalculator _debtCalculator;
        private readonly AccountHierarchyRoller _hierarchyRoller;

        public TrialBalanceService()
        {
            _normalCalculator = new NormalAccountCalculator();
            _debtCalculator = new DebtAccountCalculator();
            _hierarchyRoller = new AccountHierarchyRoller();
        }

        public List<TrialBalanceEntity> GenerateTrialBalance(IDataAccessAdapter adapter, DateTime beginDate, DateTime endDate, bool onlyBooked = true)
        {
            var qf = new QueryFactory();

            // 1. Tải danh mục tài khoản (Chart of Accounts)
            var chartOfAccounts = adapter.FetchQuery(qf.ChartOfAccount).Cast<ChartOfAccountEntity>().ToList();

            // Xác định các tài khoản con (Leaf Accounts)
            var parentIds = chartOfAccounts.Where(x => !string.IsNullOrEmpty(x.ParentId)).Select(x => x.ParentId).Distinct().ToList();
            var leafAccounts = chartOfAccounts.Where(x => !parentIds.Contains(x.AccountId)).ToList();

            // Phân loại tài khoản công nợ (CategoryId == "A") và tài khoản thường
            var debtAccountIds = leafAccounts.Where(x => x.CategoryId == "A").Select(x => x.AccountId).ToList();
            var normalAccountIds = leafAccounts.Where(x => x.CategoryId != "A").Select(x => x.AccountId).ToList();
            var allLeafIds = debtAccountIds.Concat(normalAccountIds).ToList();

            // 2. Tải toàn bộ giao dịch của các tài khoản con (bao gồm chi tiết công nợ)
            var journalQuery = qf.JournalEntry
                .Where(JournalEntryFields.AccountId.In(allLeafIds))
                .WithPath(
                    JournalEntryEntity.PrefetchPathJournalVoucher,
                    JournalEntryEntity.PrefetchPathDebtDetails // Prefetch để DebtAccountCalculator có thông tin Partner
                );
            var rawEntries = adapter.FetchQuery(journalQuery).Cast<JournalEntryEntity>().ToList();

            if (onlyBooked)
            {
                rawEntries = rawEntries.Where(e => e.JournalVoucher == null || e.JournalVoucher.Bookflag == true).ToList();
            }

            // 3. Tính toán song song bằng 2 Strategy (hoàn toàn trên RAM)
            var normalBalances = _normalCalculator.CalculateLeafBalances(rawEntries, normalAccountIds, beginDate, endDate);
            var debtBalances = _debtCalculator.CalculateLeafBalances(rawEntries, debtAccountIds, beginDate, endDate);

            // 4. Gộp kết quả tài khoản con
            var allLeafBalances = normalBalances.Concat(debtBalances).ToList();

            // 5. Cộng dồn lên tài khoản cha
            var fullBalances = _hierarchyRoller.RollupToParents(allLeafBalances, chartOfAccounts);

            return fullBalances;
        }
    }
}
