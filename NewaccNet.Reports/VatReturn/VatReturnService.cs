using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports.VatReturn
{
    public class VatReturnService
    {
        public List<VatReturnReportDTO> Calculate(SD.LLBLGen.Pro.ORMSupportClasses.IDataAccessAdapter adapter, DateTime beginDate, DateTime endDate, string accJsonPath = null)
        {
            var qf = new QueryFactory();

            // 1. Fetch Vat3TaxItems and AccountVat3Items
            var itemsQ = qf.Vat3TaxItem.WithPath(Vat3TaxItemEntity.PrefetchPathAccountVat3Items);
            var items = adapter.FetchQuery(itemsQ).Cast<Vat3TaxItemEntity>().ToList();

            // 2. Fetch JournalEntries
            DateTime yearBeginDate = new DateTime(beginDate.Year, 1, 1);
            
            var entryQ = qf.JournalEntry.WithPath(
                JournalEntryEntity.PrefetchPathJournalVoucher,
                JournalEntryEntity.PrefetchPathParentEntry,
                JournalEntryEntity.PrefetchPathSubEntries
            );
            
            var rawEntries = adapter.FetchQuery(entryQ).Cast<JournalEntryEntity>().ToList();

            // 3. LÀM PHẲNG
            // Nếu có accJsonPath thì ta không cần tính toán OtherAmount từ DB (có thể skip năm nay, nhưng ta cứ tính phẳng hết để tiện tái sử dụng logic)
            bool hasAccJson = !string.IsNullOrWhiteSpace(accJsonPath) && File.Exists(accJsonPath);

            var flattenedYear = FlattenedLedgerService.Flatten(rawEntries, yearBeginDate); 
            var flattenedPeriod = FlattenedLedgerService.Flatten(rawEntries, beginDate); 

            var results = new List<VatReturnReportDTO>();

            foreach (var item in items)
            {
                decimal amount = 0;       // Quý này
                decimal otherAmount = 0;  // Luỹ kế từ đầu năm (tính từ DB nếu ko có JSON)

                foreach (var accRule in item.AccountVat3Items)
                {
                    // == TÍNH AMOUNT (Quý này) ==
                    var ruleEntriesPeriod = flattenedPeriod.Where(e => 
                        e.AccountId != null && e.AccountId.StartsWith(accRule.AccountId)
                    ).ToList();

                    if (!string.IsNullOrEmpty(accRule.CounterAccountId) && accRule.CounterAccountId != "*")
                    {
                        ruleEntriesPeriod = ruleEntriesPeriod.Where(e => 
                            e.CounterAccountId != null && e.CounterAccountId.StartsWith(accRule.CounterAccountId)
                        ).ToList();
                    }

                    int method = accRule.Method ?? 0;
                    bool isAdd = accRule.IsAdd;
                    int gAdd = isAdd ? -1 : 1;

                    // Tính Quý này
                    foreach (var e in ruleEntriesPeriod)
                    {
                        if (method == 0) // Số dư đầu kỳ
                        {
                            if (e.IsOpeningBalance(beginDate)) amount += gAdd * (e.Dbcr == 1 ? e.Amount : -e.Amount);
                        }
                        else if (method == 1) // Phát sinh nợ
                        {
                            if (e.Dbcr == 1 && !e.IsOpeningBalance(beginDate) && e.VoucherDate <= endDate) amount += gAdd * e.Amount;
                        }
                        else if (method == -1) // Phát sinh có
                        {
                            if (e.Dbcr != 1 && !e.IsOpeningBalance(beginDate) && e.VoucherDate <= endDate) amount += gAdd * e.Amount;
                        }
                    }

                    // TÍNH OTHER AMOUNT (từ DB nếu không có JSON)
                    if (!hasAccJson)
                    {
                        var ruleEntriesYear = flattenedYear.Where(e => 
                            e.AccountId != null && e.AccountId.StartsWith(accRule.AccountId)
                        ).ToList();

                        if (!string.IsNullOrEmpty(accRule.CounterAccountId) && accRule.CounterAccountId != "*")
                        {
                            ruleEntriesYear = ruleEntriesYear.Where(e => 
                                e.CounterAccountId != null && e.CounterAccountId.StartsWith(accRule.CounterAccountId)
                            ).ToList();
                        }

                        foreach (var e in ruleEntriesYear)
                        {
                            if (method == 0) // Số dư đầu năm
                            {
                                if (e.IsOpeningBalance(yearBeginDate)) otherAmount += gAdd * (e.Dbcr == 1 ? e.Amount : -e.Amount);
                            }
                            else if (method == 1) // Phát sinh nợ từ đầu năm
                            {
                                if (e.Dbcr == 1 && !e.IsOpeningBalance(yearBeginDate) && e.VoucherDate <= endDate) otherAmount += gAdd * e.Amount;
                            }
                            else if (method == -1) // Phát sinh có từ đầu năm
                            {
                                if (e.Dbcr != 1 && !e.IsOpeningBalance(yearBeginDate) && e.VoucherDate <= endDate) otherAmount += gAdd * e.Amount;
                            }
                        }
                    }
                }

                // Sau rút lại nhân với -1
                amount = amount * -1;
                
                if (!hasAccJson)
                {
                    otherAmount = otherAmount * -1;
                }

                results.Add(new VatReturnReportDTO
                {
                    Code = item.Code,
                    ItemName = item.ItemName,
                    IsBold = item.IsBold,
                    IsItalic = item.IsItalic,
                    Amount = amount,
                    OtherAmount = otherAmount // Nếu có JSON thì sẽ ghi đè sau
                });
            }

            // 4. MERGE JSON
            if (hasAccJson)
            {
                try
                {
                    var json = File.ReadAllText(accJsonPath);
                    var prevData = JsonSerializer.Deserialize<List<VatReturnReportDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (prevData != null)
                    {
                        var prevDict = prevData.GroupBy(x => x.Code).ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));
                        
                        foreach (var res in results)
                        {
                            // Nhồi trực tiếp giá trị Amount từ JSON vào OtherAmount
                            res.OtherAmount = prevDict.ContainsKey(res.Code) ? prevDict[res.Code] : 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error merging JSON: " + ex.Message);
                }
            }

            return results.OrderBy(x => x.Code).ToList();
        }
    }
}
