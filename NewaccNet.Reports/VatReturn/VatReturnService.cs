using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports.VatReturn
{
    public class VatReturnService
    {
        public List<VatReturnReportDTO> Calculate(SD.LLBLGen.Pro.ORMSupportClasses.IDataAccessAdapter adapter, DateTime beginDate, DateTime endDate)
        {
            var qf = new QueryFactory();

            // 1. Fetch VatDeclareItems and AccountVatDeclareItems
            var itemsQ = qf.VatDeclareItem.WithPath(VatDeclareItemEntity.PrefetchPathAccountVatDeclareItems);
            var items = adapter.FetchQuery(itemsQ).Cast<VatDeclareItemEntity>().ToList();

            // 2. Fetch JournalEntries
            DateTime yearBeginDate = new DateTime(beginDate.Year, 1, 1);
            
            var entryQ = qf.JournalEntry.WithPath(
                JournalEntryEntity.PrefetchPathJournalVoucher,
                JournalEntryEntity.PrefetchPathParentEntry,
                JournalEntryEntity.PrefetchPathSubEntries
            );
            
            var rawEntries = adapter.FetchQuery(entryQ).Cast<JournalEntryEntity>().ToList();

            // 3. LÀM PHẲNG
            // Trục "yearBeginDate" làm mốc đầu năm cho OtherAmount
            var flattenedYear = FlattenedLedgerService.Flatten(rawEntries, yearBeginDate); 
            // Trục "beginDate" làm mốc đầu kỳ cho Amount
            var flattenedPeriod = FlattenedLedgerService.Flatten(rawEntries, beginDate); 

            var results = new List<VatReturnReportDTO>();

            foreach (var item in items)
            {
                decimal amount = 0;       // Quý này
                decimal otherAmount = 0;  // Luỹ kế từ đầu năm

                foreach (var accRule in item.AccountVatDeclareItems)
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

                    // == TÍNH OTHER AMOUNT (Luỹ kế từ đầu năm) ==
                    var ruleEntriesYear = flattenedYear.Where(e => 
                        e.AccountId != null && e.AccountId.StartsWith(accRule.AccountId)
                    ).ToList();

                    if (!string.IsNullOrEmpty(accRule.CounterAccountId) && accRule.CounterAccountId != "*")
                    {
                        ruleEntriesYear = ruleEntriesYear.Where(e => 
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

                    // Tính Luỹ kế từ đầu năm
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

                // Sau rút lại nhân với -1
                amount = amount * -1;
                otherAmount = otherAmount * -1;

                results.Add(new VatReturnReportDTO
                {
                    Code = item.Code,
                    ItemName = item.ItemName,
                    IsBold = item.IsBold,
                    IsItalic = item.IsItalic,
                    Amount = amount,
                    OtherAmount = otherAmount
                });
            }

            return results.OrderBy(x => x.Code).ToList();
        }
    }
}
