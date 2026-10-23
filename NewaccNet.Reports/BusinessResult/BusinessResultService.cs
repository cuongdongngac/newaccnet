using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using DataAccess.HelperClasses;
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
            var qf = new QueryFactory();

            // 1. Lấy cấu hình Báo cáo KQKD (BusinessResultItem và AccountBusinessResults) hoàn toàn qua ORM
            var items = adapter.FetchQuery(
                qf.BusinessResultItem.WithPath(BusinessResultItemEntity.PrefetchPathAccountBusinessResults)
            ).Cast<BusinessResultItemEntity>().ToList();

            // 2. Lấy dữ liệu phát sinh trong kỳ bằng ORM (tải các chứng từ trong kỳ và prefetch các bút toán)
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

            // Thu thập toàn bộ rawEntries của các chứng từ trong kỳ
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

            // 3. Quy trình làm phẳng theo chuẩn FlattenedLedgerService (Bảng phẳng chung của hệ thống)
            var flattened = FlattenedLedgerService.Flatten(rawEntries, fromDate);

            // Lọc các giao dịch phát sinh trong kỳ báo cáo (loại trừ số dư đầu kỳ)
            var periodEntries = flattened.Where(x =>
                x.JournalVoucher != null &&
                x.VoucherDate.HasValue &&
                x.VoucherDate.Value >= fromDt &&
                x.VoucherDate.Value <= toDt
            ).ToList();

            // 4. Tính toán cho từng chỉ tiêu (Item) dựa trên bảng phẳng
            foreach (var item in items)
            {
                decimal itemAmount = 0;

                if (item.AccountBusinessResults != null && item.AccountBusinessResults.Count > 0)
                {
                    foreach (var accConf in item.AccountBusinessResults)
                    {
                        var targetAccountId = accConf.AccountId?.Trim();
                        var targetCorAccountId = accConf.CounterAccountId?.Trim();

                        var matching = periodEntries.Where(p =>
                            (string.IsNullOrEmpty(targetAccountId) || (p.AccountId != null && p.AccountId.StartsWith(targetAccountId))) &&
                            (string.IsNullOrEmpty(targetCorAccountId) || (p.CounterAccountId != null && p.CounterAccountId.StartsWith(targetCorAccountId)))
                        );

                        if (accConf.Method.HasValue)
                        {
                            if (accConf.Method.Value == 1)
                                matching = matching.Where(p => p.Dbcr == 1);
                            else if (accConf.Method.Value == -1)
                                matching = matching.Where(p => p.Dbcr != 1);
                        }

                        // accConf.IsAdd: True -> -1 (Giảm trừ), False -> 1 (Theo cấu hình chuẩn)
                        int multiplier = accConf.IsAdd ? -1 : 1;
                        itemAmount += matching.Sum(p => p.Amount) * multiplier;
                    }
                }

                result.Add(new BusinessResultReportDTO
                {
                    Code = item.Code?.Trim(),
                    ItemName = item.ItemName,
                    Illustration = item.Illustration,
                    IsBold = item.IsBold,
                    IsItalic = item.IsItalic,
                    Amount = itemAmount
                });
            }

            // 5. Tự động tính các chỉ tiêu tổng hợp chuẩn B02-DN nếu chưa có số liệu trực tiếp
            CalculateSummaryFormulas(result);

            // 6. Trộn dữ liệu từ file JSON (Kỳ trước / Lũy kế)
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
                        var dict = prevData.ToDictionary(x => x.Code ?? "", x => x.Amount);
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

            return result.OrderBy(x => x.Code).ToList();
        }

        private void CalculateSummaryFormulas(List<BusinessResultReportDTO> list)
        {
            var map = list.ToDictionary(x => x.Code ?? "", x => x);

            decimal GetVal(string code) => map.TryGetValue(code, out var dto) ? dto.Amount : 0;
            void SetValIfZero(string code, decimal val)
            {
                if (map.TryGetValue(code, out var dto) && dto.Amount == 0)
                {
                    dto.Amount = val;
                }
            }

            // Mã 10 = Mã 01 - Mã 02 (Doanh thu thuần)
            SetValIfZero("10", GetVal("01") - GetVal("02"));

            // Mã 20 = Mã 10 - Mã 11 (Lợi nhuận gộp)
            SetValIfZero("20", GetVal("10") - GetVal("11"));

            // Mã 30 = Mã 20 + Mã 21 - Mã 22 - Mã 25 - Mã 26 (Lợi nhuận thuần từ HĐKD)
            SetValIfZero("30", GetVal("20") + GetVal("21") - GetVal("22") - GetVal("25") - GetVal("26"));

            // Mã 40 = Mã 31 - Mã 32 (Lợi nhuận khác)
            SetValIfZero("40", GetVal("31") - GetVal("32"));

            // Mã 50 = Mã 30 + Mã 40 (Tổng lợi nhuận trước thuế)
            SetValIfZero("50", GetVal("30") + GetVal("40"));

            // Mã 60 = Mã 50 - Mã 51 - Mã 52 (Lợi nhuận sau thuế)
            SetValIfZero("60", GetVal("50") - GetVal("51") - GetVal("52"));
        }
    }
}
