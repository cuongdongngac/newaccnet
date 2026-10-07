using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Reports.ObligationTax
{
    public class ObligationTaxService
    {
        public List<ObligationTaxResultDto> Calculate(IDataAccessAdapter adapter, DateTime fromDate, DateTime toDate, string prevDataJsonPath = null, string accDataJsonPath = null)
        {
            var result = new List<ObligationTaxResultDto>();

            // 1. Fetch Configuration
            var obligations = FetchConfiguration(adapter);
            
            // 2. Determine if we need to fetch BeginTax from DB (only if PrevJson is not provided or invalid)
            bool calculateBeginTaxFromDb = string.IsNullOrWhiteSpace(prevDataJsonPath) || !File.Exists(prevDataJsonPath);

            // 3. Fetch Journal Entries
            // Current Period
            var currentPeriodPairs = FetchJournalEntryPairs(adapter, fromDate.Date, toDate.Date, false);
            
            // Before Period (only if we need to calculate BeginTax from DB)
            List<AccountPair> beforePeriodPairs = new List<AccountPair>();
            if (calculateBeginTaxFromDb)
            {
                beforePeriodPairs = FetchJournalEntryPairs(adapter, fromDate.Date, null, true);
            }

            // 4. Process each configuration
            foreach (var obligation in obligations)
            {
                // In case there is no items loaded, this will just skip gracefully
                foreach (var item in obligation.TaxObligationItems)
                {
                    decimal beginTax = 0;
                    decimal debitTax = 0;
                    decimal creditTax = 0;

                    foreach (var rule in item.AccountTaxObligations)
                    {
                        var accountId = rule.AccountId?.Trim() ?? "";
                        var counterAccountId = rule.CounterAccountId?.Trim() ?? "";
                        
                        // Handle wildcard matching by removing trailing '*'
                        if (accountId.EndsWith("*")) accountId = accountId.TrimEnd('*');
                        if (counterAccountId.EndsWith("*")) counterAccountId = counterAccountId.TrimEnd('*');

                        int method = rule.Method ?? 0;
                        bool isAdd = rule.IsAdd; // IsAdd is already a non-nullable bool
                        int signMultiplier = isAdd ? -1 : 1; // isAdd = true means negate
                        int colIndex = rule.ColIndex ?? 0;

                        if (colIndex == 0 && calculateBeginTaxFromDb)
                        {
                            var matchingPairs = FilterPairs(beforePeriodPairs, accountId, counterAccountId, method);
                            beginTax += CalculateTotal(matchingPairs, method) * signMultiplier;
                        }
                        else if (colIndex == 1)
                        {
                            var matchingPairs = FilterPairs(currentPeriodPairs, accountId, counterAccountId, method);
                            creditTax += CalculateTotal(matchingPairs, method) * signMultiplier;
                        }
                        else if (colIndex == 2)
                        {
                            var matchingPairs = FilterPairs(currentPeriodPairs, accountId, counterAccountId, method);
                            debitTax += CalculateTotal(matchingPairs, method) * signMultiplier;
                        }
                    }

                    result.Add(new ObligationTaxResultDto
                    {
                        ObligationId = obligation.Id,
                        ObligationName = obligation.ObligationName,
                        Code = item.Code,
                        ItemName = item.ItemName,
                        BeginTax = beginTax,
                        DebitTax = debitTax,
                        CreditTax = creditTax,
                        EndTax = 0, // Calculated after merge
                        AccTax = 0  // Merged from JSON later
                    });
                }
            }

            // 5. Merge JSONs
            // If PrevJson was provided, we get BeginTax from it. We take EndTax of previous JSON and set it to BeginTax
            if (!calculateBeginTaxFromDb)
            {
                MergeJson(result, prevDataJsonPath, (dto, endTaxVal) => dto.BeginTax = endTaxVal);
            }
            
            // For Accumulated Tax, we take EndTax of the Acc JSON and set it to AccTax
            MergeJson(result, accDataJsonPath, (dto, endTaxVal) => dto.AccTax = endTaxVal);

            // 6. Calculate EndTax
            // As discussed: EndTax = BeginTax + DebitTax - CreditTax
            foreach (var r in result)
            {
                r.EndTax = r.BeginTax + r.DebitTax - r.CreditTax;
            }

            return result;
        }

        private IEnumerable<AccountPair> FilterPairs(List<AccountPair> pairs, string accountId, string counterAccountId, int method)
        {
            return pairs.Where(p =>
                (string.IsNullOrEmpty(accountId) || p.AccountId.StartsWith(accountId)) &&
                (string.IsNullOrEmpty(counterAccountId) || p.CorAccountId.StartsWith(counterAccountId)) &&
                (method == 0 || p.Dbcr == method)
            );
        }

        private decimal CalculateTotal(IEnumerable<AccountPair> pairs, int method)
        {
            if (method == 0)
            {
                // Method 0: amount * dbcr
                return pairs.Sum(p => p.Amount * p.Dbcr);
            }
            else
            {
                // Method != 0: just amount
                return pairs.Sum(p => p.Amount);
            }
        }

        private List<AccountPair> FetchJournalEntryPairs(IDataAccessAdapter adapter, DateTime? fromDate, DateTime? toDate, bool isBeforePeriod = false)
        {
            var bucket = new RelationPredicateBucket();
            bucket.Relations.Add(JournalEntryEntity.Relations.JournalVoucherEntityUsingJournalVoucherId, JoinHint.Left);
            
            if (isBeforePeriod && fromDate.HasValue)
            {
                // Số dư đầu kỳ: VoucherDate < BeginDate HOẶC VoucherDate IS NULL HOẶC không có Voucher (JournalVoucherId IS NULL)
                var datePredicate = new PredicateExpression(JournalVoucherFields.VoucherDate < fromDate.Value);
                datePredicate.AddWithOr(JournalVoucherFields.VoucherDate == DBNull.Value);
                datePredicate.AddWithOr(JournalEntryFields.JournalVoucherId == DBNull.Value);
                bucket.PredicateExpression.Add(datePredicate);
            }
            else
            {
                if (fromDate.HasValue)
                    bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate >= fromDate.Value);
                
                if (toDate.HasValue)
                    bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate <= toDate.Value);
            }

            var prefetch = new PrefetchPath2(EntityType.JournalEntryEntity);
            prefetch.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var entries = new EntityCollection<JournalEntryEntity>();
            adapter.FetchEntityCollection(entries, bucket, prefetch);

            var parentIdsWithChildren = new HashSet<object>();
            foreach (var e in entries)
            {
                if (e.ParentId != null)
                    parentIdsWithChildren.Add(e.ParentId);
            }

            var pairs = new List<AccountPair>();
            foreach (var entry in entries)
            {
                var amount = (decimal)(entry.Amount ?? 0);
                
                if (entry.ParentEntry != null)
                {
                    // Giao dịch kép (có đối ứng)
                    pairs.Add(new AccountPair(entry.AccountId, entry.ParentEntry.AccountId, entry.Dbcr, amount));
                    pairs.Add(new AccountPair(entry.ParentEntry.AccountId, entry.AccountId, entry.ParentEntry.Dbcr, amount));
                }
                else if (entry.ParentId == null && !parentIdsWithChildren.Contains(entry.Id))
                {
                    // Giao dịch đơn (không có đối ứng) - Thường là số dư đầu kỳ
                    pairs.Add(new AccountPair(entry.AccountId, "", entry.Dbcr, amount));
                }
            }
            return pairs;
        }

        private void MergeJson(List<ObligationTaxResultDto> currentResult, string jsonPath, Action<ObligationTaxResultDto, decimal> setAction)
        {
            if (string.IsNullOrWhiteSpace(jsonPath) || !File.Exists(jsonPath))
                return;

            try
            {
                var json = File.ReadAllText(jsonPath);
                var prevData = JsonSerializer.Deserialize<List<ObligationTaxResultDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (prevData != null)
                {
                    // Merge EndTax from JSON based on the Code
                    var prevDict = prevData.GroupBy(x => x.Code).ToDictionary(g => g.Key, g => g.Sum(x => x.EndTax));

                    foreach (var item in currentResult)
                    {
                        if (!string.IsNullOrEmpty(item.Code) && prevDict.TryGetValue(item.Code, out var val))
                        {
                            setAction(item, val);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error merging JSON: " + ex.Message);
            }
        }

        private EntityCollection<TaxObligationEntity> FetchConfiguration(IDataAccessAdapter adapter)
        {
            var obligations = new EntityCollection<TaxObligationEntity>();
            var prefetch = new PrefetchPath2(EntityType.TaxObligationEntity);
            
            // Assuming the relations are defined with these exact names in LLBLGen
            var itemNode = prefetch.Add(TaxObligationEntity.PrefetchPathTaxObligationItems);
            itemNode.SubPath.Add(TaxObligationItemEntity.PrefetchPathAccountTaxObligations);

            adapter.FetchEntityCollection(obligations, null, prefetch);
            return obligations;
        }

        private class AccountPair
        {
            public string AccountId { get; }
            public string CorAccountId { get; }
            public short Dbcr { get; }
            public decimal Amount { get; }

            public AccountPair(string accountId, string corAccountId, short? dbcr, decimal amount)
            {
                AccountId = accountId?.Trim() ?? "";
                CorAccountId = corAccountId?.Trim() ?? "";
                Dbcr = dbcr ?? 0;
                Amount = amount;
            }
        }
    }
}
