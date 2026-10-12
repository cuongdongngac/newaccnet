using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports.VatDeclare
{
    public class VatDeclareService
    {
        public List<VatDeclareReportDTO> Calculate(IDataAccessAdapter adapter, DateTime beginDate, DateTime endDate)
        {
            var qf = new QueryFactory();

            // 1. Nạp danh mục Chỉ tiêu và Chi tiết tài khoản liên kết
            var itemsQ = qf.VatDeclareItem
                .OrderBy(DataAccess.HelperClasses.VatDeclareItemFields.Id.Ascending())
                .WithPath(VatDeclareItemEntity.PrefetchPathAccountVatDeclareItems);
            var items = adapter.FetchQuery(itemsQ).Cast<VatDeclareItemEntity>().ToList();

            // 2. Nạp toàn bộ JournalEntry liên quan
            var entryQ = qf.JournalEntry.WithPath(
                JournalEntryEntity.PrefetchPathJournalVoucher,
                JournalEntryEntity.PrefetchPathParentEntry,
                JournalEntryEntity.PrefetchPathSubEntries
            );
            var rawEntries = adapter.FetchQuery(entryQ).Cast<JournalEntryEntity>().ToList();

            // 3. Làm phẳng dữ liệu sổ cái thông qua FlattenedLedgerService
            var flattenedPeriod = FlattenedLedgerService.Flatten(rawEntries, beginDate.Date);

            var results = new List<VatDeclareReportDTO>();

            foreach (var item in items)
            {
                decimal turnoverAmount = 0; // Cột 1: Doanh số (chưa có VAT)
                decimal taxAmount = 0;      // Cột 2: Thuế VAT

                foreach (var accRule in item.AccountVatDeclareItems)
                {
                    var accountId = accRule.AccountId?.Trim() ?? "";
                    if (accountId.EndsWith("*")) accountId = accountId.TrimEnd('*');

                    var counterAccountId = accRule.CounterAccountId?.Trim() ?? "";
                    if (counterAccountId.EndsWith("*")) counterAccountId = counterAccountId.TrimEnd('*');

                    // Lọc theo AccountId
                    var matchedEntries = flattenedPeriod.Where(e =>
                        e.AccountId != null && e.AccountId.StartsWith(accountId)
                    ).ToList();

                    // Lọc theo CounterAccountId nếu có chỉ định
                    if (!string.IsNullOrEmpty(counterAccountId))
                    {
                        matchedEntries = matchedEntries.Where(e =>
                            e.CounterAccountId != null && e.CounterAccountId.StartsWith(counterAccountId)
                        ).ToList();
                    }

                    int method = accRule.Method ?? 0;
                    bool isAdd = accRule.IsAdd;
                    int signMultiplier = isAdd ? -1 : 1; // isAdd = true thì nhân -1 (đổi dấu / trừ)
                    int columnNo = accRule.ColumnNo ?? 1;

                    decimal calculatedTotal = 0;

                    foreach (var e in matchedEntries)
                    {
                        if (method == 0) // Số dư đầu kỳ
                        {
                            if (e.IsOpeningBalance(beginDate.Date))
                            {
                                calculatedTotal += (e.Dbcr == 1 ? e.Amount : -e.Amount);
                            }
                        }
                        else if (method == 1) // Phát sinh Nợ trong kỳ
                        {
                            if (e.Dbcr == 1 && !e.IsOpeningBalance(beginDate.Date) && e.VoucherDate <= endDate.Date)
                            {
                                calculatedTotal += e.Amount;
                            }
                        }
                        else if (method == -1) // Phát sinh Có trong kỳ
                        {
                            if (e.Dbcr != 1 && !e.IsOpeningBalance(beginDate.Date) && e.VoucherDate <= endDate.Date)
                            {
                                calculatedTotal += e.Amount;
                            }
                        }
                    }

                    decimal finalAmount = calculatedTotal * signMultiplier;

                    if (columnNo == 1)
                    {
                        turnoverAmount += finalAmount;
                    }
                    else if (columnNo == 2)
                    {
                        taxAmount += finalAmount;
                    }
                }

                results.Add(new VatDeclareReportDTO
                {
                    Code = item.Code ?? string.Empty,
                    ItemName = item.ItemName ?? string.Empty,
                    IsBold = item.IsBold,
                    IsItalic = item.IsItalic,
                    TurnoverAmount = turnoverAmount,
                    VatAmount = taxAmount
                });
            }

            return results;
        }
    }
}

