using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DataAccess;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NewaccNet.Reports.BusinessResult
{
    public class BusinessResultService
    {
        public List<BusinessResultReportDTO> Calculate(IDataAccessAdapter adapter, DateTime fromDate, DateTime toDate, string jsonPath)
        {
            var result = new List<BusinessResultReportDTO>();

            // 1. Lấy cấu hình Báo cáo KQKD (BusinessResultItem và AccountBusinessResults)
            var items = GetConfiguredItems(adapter);

            // 2. Lấy dữ liệu phát sinh trong kỳ
            var bucket = new RelationPredicateBucket();
            bucket.PredicateExpression.Add(JournalEntryFields.ParentId != DBNull.Value);
            
            // Join với bảng Voucher để lọc theo VoucherDate
            bucket.Relations.Add(JournalEntryEntity.Relations.JournalVoucherEntityUsingJournalVoucherId);
            bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate >= fromDate);
            bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate <= toDate);

            var prefetch = new PrefetchPath2((int)EntityType.JournalEntryEntity);
            prefetch.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var children = new EntityCollection<JournalEntryEntity>();
            adapter.FetchEntityCollection(children, bucket, prefetch);

            // Chuyển đổi dữ liệu sổ kép thành danh sách AccountPair (2 chiều nhìn)
            var pairs = new List<AccountPair>();
            foreach (var child in children)
            {
                if (child.ParentEntry == null) continue;
                var amount = (decimal)(child.Amount ?? 0);
                
                // Chiều nhìn từ Con -> Cha
                pairs.Add(new AccountPair(child.AccountId, child.ParentEntry.AccountId, child.Dbcr, amount));
                // Chiều nhìn từ Cha -> Con
                pairs.Add(new AccountPair(child.ParentEntry.AccountId, child.AccountId, child.ParentEntry.Dbcr, amount));
            }

            // 3. Tính toán cho từng chỉ tiêu (Item)
            foreach (var item in items)
            {
                decimal itemAmount = 0;

                foreach (var accConf in item.AccountBusinessResults)
                {
                    var targetAccountId = accConf.AccountId?.Trim();
                    var targetCorAccountId = accConf.CounterAccountId?.Trim();

                    // Tìm tất cả các cặp khớp cấu hình (Khớp tài khoản và Khớp Method == Dbcr)
                    // Lưu ý: Dbcr trong JournalEntry có thể lưu 2, 0, -1 đại diện cho Có. Do đó dùng logic: 1 = Nợ, Khác 1 = Có.
                    var matchingPairs = pairs.Where(p => 
                        (string.IsNullOrEmpty(targetAccountId) || p.AccountId.StartsWith(targetAccountId)) &&
                        (string.IsNullOrEmpty(targetCorAccountId) || p.CorAccountId.StartsWith(targetCorAccountId)) &&
                        (!accConf.Method.HasValue || (accConf.Method.Value == 1 ? p.Dbcr == 1 : p.Dbcr != 1))
                    );

                    // accConf.IsAdd: True -> nhân -1, False -> nhân 1 (Theo yêu cầu)
                    int multiplier = accConf.IsAdd ? -1 : 1;
                    itemAmount += matchingPairs.Sum(p => p.Amount) * multiplier;
                }

                result.Add(new BusinessResultReportDTO
                {
                    Code = item.Code,
                    ItemName = item.ItemName,
                    Illustration = item.Illustration,
                    IsBold = item.IsBold,
                    IsItalic = item.IsItalic,
                    Amount = itemAmount
                });
            }

            // 4. Trộn dữ liệu từ file JSON (Kỳ trước / Lũy kế)
            if (!string.IsNullOrEmpty(jsonPath) && File.Exists(jsonPath))
            {
                try
                {
                    var json = File.ReadAllText(jsonPath);
                    var prevData = JsonSerializer.Deserialize<List<BusinessResultReportDTO>>(
                        json, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (prevData != null)
                    {
                        var dict = prevData.ToDictionary(x => x.Code ?? "", x => x.Amount); // Amount của kỳ trước lấy làm OtherAmount
                        foreach (var res in result)
                        {
                            if (dict.TryGetValue(res.Code ?? "", out decimal prevAmt))
                            {
                                res.OtherAmount = prevAmt;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Lỗi đọc file JSON -> bỏ qua
                }
            }

            // Mặc định sắp xếp theo Code để lên báo cáo chuẩn
            return result.OrderBy(x => x.Code).ToList();
        }

        private EntityCollection<BusinessResultItemEntity> GetConfiguredItems(IDataAccessAdapter adapter)
        {
            var items = new EntityCollection<BusinessResultItemEntity>();
            var prefetch = new PrefetchPath2((int)EntityType.BusinessResultItemEntity);
            prefetch.Add(BusinessResultItemEntity.PrefetchPathAccountBusinessResults);

            adapter.FetchEntityCollection(items, null, prefetch);
            return items;
        }

        private class AccountPair
        {
            public string AccountId { get; }
            public string CorAccountId { get; }
            public short? Dbcr { get; }
            public decimal Amount { get; }

            public AccountPair(string accountId, string corAccountId, short? dbcr, decimal amount)
            {
                AccountId = accountId?.Trim() ?? "";
                CorAccountId = corAccountId?.Trim() ?? "";
                Dbcr = dbcr;
                Amount = amount;
            }
        }
    }
}
