using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports
{
    public class LedgerCalculator
    {
        public List<LedgerReportDTO> Calculate(SD.LLBLGen.Pro.ORMSupportClasses.IDataAccessAdapter adapter, List<JournalEntryEntity> rawEntries, string accountId, DateTime beginDate, DateTime endDate, bool onlyBooked = true)
        {
            var result = new List<LedgerReportDTO>();
            decimal currentBalance = 0;
            
            var qf = new DataAccess.FactoryClasses.QueryFactory();
            var accountsDict = adapter.FetchQuery(qf.ChartOfAccount).Cast<ChartOfAccountEntity>().ToDictionary(a => a.AccountId, a => a.AccountName);

            // 1. Tính số dư đầu kỳ (Trước ngày beginDate)
            var openingEntries = rawEntries.Where(x => 
                (x.JournalVoucher == null || x.JournalVoucher.VoucherDate < beginDate) && 
                x.AccountId != null && x.AccountId.StartsWith(accountId)).ToList();

            decimal openingDebit = 0;
            decimal openingCredit = 0;

            foreach (var entry in openingEntries)
            {
                // Nếu Entry là Parent (có con) => Bỏ qua vì các dòng con đã chứa số tiền chi tiết
                if (entry.SubEntries != null && entry.SubEntries.Count > 0)
                    continue;

                decimal amount = (decimal)(entry.Amount ?? 0);
                if (entry.Dbcr == 1) openingDebit += amount;
                else if (entry.Dbcr == -1 || entry.Dbcr == 0 || entry.Dbcr == 2) openingCredit += amount; 
                // Legacy thường Dbcr = 1 (Nợ), -1 hoặc 2 hoặc 0 (Có)
            }

            currentBalance = openingDebit - openingCredit;

            // Thêm dòng Dư đầu kỳ
            result.Add(new LedgerReportDTO
            {
                RowType = 0,
                VoucherDate = beginDate.AddDays(-1),
                VoucherNo = "",
                Contents = "Số dư đầu kỳ",
                AccountId = accountId,
                CounterAccountId = "",
                DebitAmount = 0,
                CreditAmount = 0,
                Balance = currentBalance,
                JournalVoucherId = null
            });

            // 2. Tính phát sinh trong kỳ
            var periodEntries = rawEntries.Where(x => 
                x.JournalVoucher != null && 
                x.JournalVoucher.VoucherDate >= beginDate && 
                x.JournalVoucher.VoucherDate <= endDate &&
                (!onlyBooked || x.JournalVoucher.Bookflag == true)).ToList();

            decimal totalPeriodDebit = 0;
            decimal totalPeriodCredit = 0;

            foreach (var entry in periodEntries)
            {
                if (entry.AccountId == null || !entry.AccountId.StartsWith(accountId))
                    continue;

                // Nếu có dòng con => split
                if (entry.SubEntries != null && entry.SubEntries.Count > 0)
                {
                    foreach (var child in entry.SubEntries)
                    {
                        AddPeriodRow(result, accountsDict, entry.JournalVoucher, entry.Dbcr, (decimal)(child.Amount ?? 0), entry.AccountId, child.AccountId, child.Id, ref currentBalance, ref totalPeriodDebit, ref totalPeriodCredit);
                    }
                }
                else
                {
                    // Nếu là dòng con, đối ứng là cha
                    if (entry.ParentId.HasValue && entry.ParentEntry != null)
                    {
                        AddPeriodRow(result, accountsDict, entry.JournalVoucher, entry.Dbcr, (decimal)(entry.Amount ?? 0), entry.AccountId, entry.ParentEntry.AccountId, entry.Id, ref currentBalance, ref totalPeriodDebit, ref totalPeriodCredit);
                    }
                    else
                    {
                        // Dòng độc lập
                        AddPeriodRow(result, accountsDict, entry.JournalVoucher, entry.Dbcr, (decimal)(entry.Amount ?? 0), entry.AccountId, "", entry.Id, ref currentBalance, ref totalPeriodDebit, ref totalPeriodCredit);
                    }
                }
            }

            // Sắp xếp lại theo thời gian và journalentryid
            result = result.OrderBy(x => x.RowType).ThenBy(x => x.VoucherDate).ThenBy(x => x.JournalEntryId).ToList();

            // Thêm dòng Cộng phát sinh
            result.Add(new LedgerReportDTO
            {
                RowType = 2,
                VoucherDate = endDate,
                VoucherNo = "",
                Contents = "Cộng phát sinh",
                AccountId = "",
                CounterAccountId = "",
                DebitAmount = totalPeriodDebit,
                CreditAmount = totalPeriodCredit,
                Balance = currentBalance,
                JournalVoucherId = null,
                JournalEntryId = null
            });

            return result;
        }

        private void AddPeriodRow(List<LedgerReportDTO> result, Dictionary<string, string> accountsDict, JournalVoucherEntity voucher, short? dbcr, decimal amount, string accountId, string counterAcc, int? entryId, ref decimal currentBalance, ref decimal totalDebit, ref decimal totalCredit)
        {
            decimal debit = 0;
            decimal credit = 0;

            if (dbcr == 1)
            {
                debit = amount;
                totalDebit += amount;
                currentBalance += amount;
            }
            else
            {
                credit = amount;
                totalCredit += amount;
                currentBalance -= amount;
            }

            string counterAccountName = "";
            if (!string.IsNullOrEmpty(counterAcc) && accountsDict.TryGetValue(counterAcc, out var accName))
            {
                counterAccountName = accName;
            }

            result.Add(new LedgerReportDTO
            {
                RowType = 1,
                JournalVoucherId = voucher?.Id,
                JournalEntryId = entryId,
                VoucherNo = voucher?.VoucherNo ?? "",
                VoucherDate = voucher?.VoucherDate,
                Contents = voucher?.Contents ?? "",
                AccountId = accountId,
                CounterAccountId = counterAcc,
                CounterAccountName = counterAccountName,
                DebitAmount = debit,
                CreditAmount = credit,
                Balance = currentBalance
            });
        }
    }
}

