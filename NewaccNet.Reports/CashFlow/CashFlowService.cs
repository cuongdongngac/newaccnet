using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports.CashFlow
{
    public class CashFlowService
    {
        public List<CashFlowReportDTO> Calculate(IDataAccessAdapter adapter, DateTime fromDate, DateTime toDate, string prevDataJsonPath = null)
        {
            var result = new List<CashFlowReportDTO>();
            var qf = new QueryFactory();

            // 1. Tải cấu hình danh mục LCTT (Nhóm -> Chỉ tiêu -> Cấu hình tài khoản) qua ORM
            var categories = FetchConfiguration(adapter);

            // 2. Lấy toàn bộ chứng từ phát sinh trong kỳ báo cáo qua ORM (Prefetch đầy đủ bút toán)
            DateTime fromDt = fromDate.Date;
            DateTime toDt = toDate.Date.AddDays(1).AddTicks(-1);

            var voucherQuery = qf.JournalVoucher
                .Where(JournalVoucherFields.VoucherDate.GreaterEqual(fromDt)
                    .And(JournalVoucherFields.VoucherDate.LesserEqual(toDt)))
                .WithPath(
                    JournalVoucherEntity.PrefetchPathJournalEntries.WithSubPath(
                        JournalEntryEntity.PrefetchPathParentEntry,
                        JournalEntryEntity.PrefetchPathSubEntries
                    )
                );

            var vouchers = adapter.FetchQuery(voucherQuery).Cast<JournalVoucherEntity>().ToList();

            // 3. Thu thập các dòng bút toán và làm phẳng theo chuẩn FlattenedLedgerService (Flat Table)
            var rawEntries = new List<JournalEntryEntity>();
            var seenEntryIds = new HashSet<int>();

            foreach (var voucher in vouchers)
            {
                var voucherEntries = voucher.JournalEntries.ToList();
                foreach (var entry in voucherEntries)
                {
                    if (entry.JournalVoucher == null)
                    {
                        entry.JournalVoucher = voucher;
                    }

                    if (seenEntryIds.Add(entry.Id))
                    {
                        rawEntries.Add(entry);
                    }

                    if (entry.SubEntries != null && entry.SubEntries.Count > 0)
                    {
                        var subEntries = entry.SubEntries.ToList();
                        foreach (var sub in subEntries)
                        {
                            if (sub.JournalVoucher == null)
                            {
                                sub.JournalVoucher = voucher;
                            }

                            if (seenEntryIds.Add(sub.Id))
                            {
                                rawEntries.Add(sub);
                            }
                        }
                    }
                }
            }

            var flattened = FlattenedLedgerService.Flatten(rawEntries, fromDate);

            // Lọc các giao dịch phát sinh trong kỳ (bỏ số dư đầu kỳ)
            var periodEntries = flattened.Where(x =>
                x.JournalVoucher != null &&
                x.VoucherDate.HasValue &&
                x.VoucherDate.Value >= fromDt &&
                x.VoucherDate.Value <= toDt
            ).ToList();

            // 4. Tính toán cho từng chỉ tiêu LCTT
            foreach (var category in categories.OrderBy(c => c.Id))
            {
                foreach (var item in category.CashFlowItems.OrderBy(i => i.Code))
                {
                    decimal itemAmount = 0;
                    bool isThu = item.Dbcr; // True: Thu, False: Chi

                    if (item.AccountCashFlowItems != null && item.AccountCashFlowItems.Count > 0)
                    {
                        foreach (var accConf in item.AccountCashFlowItems)
                        {
                            var targetAccountId = accConf.AccountId?.Trim();
                            var targetCorAccountId = accConf.CounterAccountId?.Trim();

                            var matching = periodEntries.Where(p =>
                                (string.IsNullOrEmpty(targetAccountId) || (p.AccountId != null && p.AccountId.StartsWith(targetAccountId))) &&
                                (string.IsNullOrEmpty(targetCorAccountId) || (p.CounterAccountId != null && p.CounterAccountId.StartsWith(targetCorAccountId)))
                            );

                            // Lọc theo chiều Thu/Chi: Thu = Nợ tiền (Dbcr == 1), Chi = Có tiền (Dbcr != 1)
                            if (isThu)
                                matching = matching.Where(p => p.Dbcr == 1);
                            else
                                matching = matching.Where(p => p.Dbcr != 1);

                            itemAmount += matching.Sum(p => p.Amount);
                        }
                    }

                    result.Add(new CashFlowReportDTO
                    {
                        CashFlowCategoryId = category.Id,
                        CashFlowName = category.CashFlowName,
                        CashFlowNameRepeate = category.CashFlowNameRepeate,
                        Code = item.Code?.Trim(),
                        Illustration = item.Illustration,
                        IsBold = item.IsBold,
                        IsItalic = item.IsItalic,
                        ItemName = item.ItemName,
                        Dbcr = isThu,
                        Amount = Math.Abs(itemAmount),
                        PrevAmount = 0
                    });
                }
            }

            // 5. Trộn dữ liệu kỳ trước từ file JSON (nếu có)
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
    }
}
