using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;

namespace NewaccNet.Reports
{
    public class FlattenedLedgerEntry
    {
        public int EntryId { get; set; }
        public string AccountId { get; set; }
        public string CounterAccountId { get; set; }
        public short Dbcr { get; set; }
        public decimal Amount { get; set; }
        public JournalVoucherEntity JournalVoucher { get; set; }
        public DateTime? VoucherDate => JournalVoucher?.VoucherDate;

        public bool IsOpeningBalance(DateTime beginDate)
        {
            return JournalVoucher == null || VoucherDate < beginDate;
        }
    }

    public static class FlattenedLedgerService
    {
        public static List<FlattenedLedgerEntry> Flatten(List<JournalEntryEntity> rawEntries, DateTime beginDate)
        {
            var result = new List<FlattenedLedgerEntry>();

            var parentIdsWithChildren = new HashSet<long>();
            foreach (var e in rawEntries)
            {
                if (e.ParentId.HasValue) parentIdsWithChildren.Add(e.ParentId.Value);
            }

            foreach (var entry in rawEntries)
            {
                decimal amount = (decimal)(entry.Amount ?? 0);
                short dbcr = entry.Dbcr ?? -1;

                if (entry.JournalVoucherId == null)
                {
                    // ĐÂY LÀ SỐ DƯ ĐẦU KỲ: Không cần tìm đối ứng, gán ngày lập tức!
                    result.Add(new FlattenedLedgerEntry
                    {
                        EntryId = entry.Id,
                        AccountId = entry.AccountId,
                        CounterAccountId = "",
                        Dbcr = dbcr,
                        Amount = amount,
                        JournalVoucher = new JournalVoucherEntity { 
                            VoucherDate = beginDate.AddDays(-1), 
                            VoucherNo = "SDDK" 
                        }
                    });
                }
                else
                {
                    // ĐÂY LÀ PHÁT SINH TRONG KỲ: Tìm ngày ở Voucher và tìm đối ứng
                    if (entry.ParentEntry != null)
                    {
                        var parentVoucher = entry.ParentEntry.JournalVoucher ?? entry.JournalVoucher;
                        
                        // Cặp 1: Phía của tài khoản con
                        result.Add(new FlattenedLedgerEntry
                        {
                            EntryId = entry.Id,
                            AccountId = entry.AccountId,
                            CounterAccountId = entry.ParentEntry.AccountId,
                            Dbcr = dbcr,
                            Amount = amount,
                            JournalVoucher = entry.JournalVoucher
                        });

                        // Cặp 2: Phía của tài khoản cha
                        result.Add(new FlattenedLedgerEntry
                        {
                            EntryId = entry.ParentEntry.Id,
                            AccountId = entry.ParentEntry.AccountId,
                            CounterAccountId = entry.AccountId,
                            Dbcr = entry.ParentEntry.Dbcr ?? -1,
                            Amount = amount,
                            JournalVoucher = parentVoucher
                        });
                    }
                    else if (entry.ParentId == null && !parentIdsWithChildren.Contains(entry.Id))
                    {
                        // Giao dịch đơn côi (chứng từ có thể bị lỗi mất đối ứng)
                        result.Add(new FlattenedLedgerEntry
                        {
                            EntryId = entry.Id,
                            AccountId = entry.AccountId,
                            CounterAccountId = "",
                            Dbcr = dbcr,
                            Amount = amount,
                            JournalVoucher = entry.JournalVoucher
                        });
                    }
                }
            }

            return result;
        }
    }
}
