using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Reports.CashFlow
{
    public class CashFlowService
    {
        public List<CashFlowReportDTO> Calculate(IDataAccessAdapter adapter, DateTime fromDate, DateTime toDate, string prevDataJsonPath = null)
        {
            var result = new List<CashFlowReportDTO>();

            // 1. Fetch Categories, Items, and Configured Accounts
            var categories = FetchConfiguration(adapter);

                // 2. Fetch JournalEntries for the period
                // To properly capture double-entry, we fetch all Child entries (which have a Parent)
                var bucket = new RelationPredicateBucket();
                bucket.Relations.Add(JournalEntryEntity.Relations.JournalVoucherEntityUsingJournalVoucherId);
                bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate >= fromDate.Date);
                bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate <= toDate.Date);
                bucket.PredicateExpression.Add(JournalEntryFields.ParentId != DBNull.Value);

                var prefetch = new PrefetchPath2(EntityType.JournalEntryEntity);
                prefetch.Add(JournalEntryEntity.PrefetchPathParentEntry);

                var children = new EntityCollection<JournalEntryEntity>();
                adapter.FetchEntityCollection(children, bucket, prefetch);

                // Flatten the double-entries into a list of generic pairs for easier matching
                var pairs = new List<AccountPair>();
                foreach (var child in children)
                {
                    if (child.ParentEntry == null) continue;
                    var amount = (decimal)(child.Amount ?? 0);
                    
                    // Pair 1: Child is the primary account
                    pairs.Add(new AccountPair(child.AccountId, child.ParentEntry.AccountId, child.Dbcr, amount));
                    // Pair 2: Parent is the primary account
                    pairs.Add(new AccountPair(child.ParentEntry.AccountId, child.AccountId, child.ParentEntry.Dbcr, amount));
                }

                // 3. Process each configured Item
                foreach (var category in categories)
                {
                    foreach (var item in category.CashFlowItems)
                    {
                        decimal itemAmount = 0;
                        
                        // item.Dbcr in DB is boolean. True = Thu (Inflow), False = Chi (Outflow)
                        bool isThu = item.Dbcr;
                        int multiplier = isThu ? 1 : -1;

                        foreach (var accConf in item.AccountCashFlowItems)
                        {
                            var targetAccountId = accConf.AccountId?.Trim();
                            var targetCorAccountId = accConf.CounterAccountId?.Trim();

                            // Find all pairs matching this configuration
                            var matchingPairs = pairs.Where(p => 
                                (string.IsNullOrEmpty(targetAccountId) || p.AccountId.StartsWith(targetAccountId)) &&
                                (string.IsNullOrEmpty(targetCorAccountId) || p.CorAccountId.StartsWith(targetCorAccountId)) &&
                                (isThu ? p.Dbcr == 1 : p.Dbcr != 1) // Thu = Nợ (1), Chi = Có (khác 1)
                            );

                            itemAmount += matchingPairs.Sum(p => p.Amount) * multiplier;
                        }

                        result.Add(new CashFlowReportDTO
                        {
                            CashFlowCategoryId = category.Id,
                            CashFlowName = category.CashFlowName,
                            CashFlowNameRepeate = category.CashFlowNameRepeate,
                            Code = item.Code,
                            Illustration = item.Illustration,
                            IsBold = item.IsBold,
                            IsItalic = item.IsItalic,
                            ItemName = item.ItemName,
                            Amount = itemAmount,
                            PrevAmount = 0 // Will be populated from JSON
                        });
                    }
                }

            // 4. Merge with JSON if provided
            MergeWithPreviousPeriod(result, prevDataJsonPath);

            return result;
        }

        private void MergeWithPreviousPeriod(List<CashFlowReportDTO> currentResult, string jsonPath)
        {
            if (string.IsNullOrWhiteSpace(jsonPath) || !File.Exists(jsonPath))
                return;

            try
            {
                var json = File.ReadAllText(jsonPath);
                var prevData = JsonSerializer.Deserialize<List<CashFlowReportDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (prevData != null)
                {
                    var prevDict = prevData.GroupBy(x => x.Code).ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                    foreach (var item in currentResult)
                    {
                        if (!string.IsNullOrEmpty(item.Code) && prevDict.TryGetValue(item.Code, out var prevAmount))
                        {
                            item.PrevAmount = prevAmount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In real world, log the exception
                Console.WriteLine("Error merging JSON: " + ex.Message);
            }
        }

        private EntityCollection<CashFlowCategoryEntity> FetchConfiguration(IDataAccessAdapter adapter)
        {
            var categories = new EntityCollection<CashFlowCategoryEntity>();
            var prefetch = new PrefetchPath2(EntityType.CashFlowCategoryEntity);
            var itemNode = prefetch.Add(CashFlowCategoryEntity.PrefetchPathCashFlowItems);
            itemNode.SubPath.Add(CashFlowItemEntity.PrefetchPathAccountCashFlowItems);

            adapter.FetchEntityCollection(categories, null, prefetch);
            return categories;
        }

        private class AccountPair
        {
            public string AccountId { get; }
            public string CorAccountId { get; }
            public int Dbcr { get; }
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
