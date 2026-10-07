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

            // BƯỚC 1: LÀM PHẲNG TOÀN BỘ GIAO DỊCH THÀNH TRỤC TRUNG GIAN
            var flattenedEntries = FlattenedLedgerService.Flatten(rawEntries, beginDate);

            // Lọc ra các dòng phẳng thuộc về AccountId đang xét
            var targetEntries = flattenedEntries.Where(x => x.AccountId != null && x.AccountId.StartsWith(accountId)).ToList();

            // BƯỚC 2: TÍNH SỐ DƯ ĐẦU KỲ
            var openingEntries = targetEntries.Where(x => x.IsOpeningBalance(beginDate)).ToList();

            decimal openingDebit = 0;
            decimal openingCredit = 0;

            foreach (var entry in openingEntries)
            {
                if (entry.Dbcr == 1) openingDebit += entry.Amount;
                else openingCredit += entry.Amount;
            }

            currentBalance = openingDebit - openingCredit;

            // Thêm dòng Dư đầu kỳ
            result.Add(new LedgerReportDTO
            {
                RowType = 0,
                VoucherDate = beginDate.AddDays(-1),
                VoucherNo = "SDDK",
                Contents = "Số dư đầu kỳ",
                AccountId = accountId,
                CounterAccountId = "",
                DebitAmount = 0,
                CreditAmount = 0,
                Balance = currentBalance,
                JournalVoucherId = null
            });

            // BƯỚC 3: TÍNH PHÁT SINH TRONG KỲ
            var periodEntries = targetEntries.Where(x => 
                !x.IsOpeningBalance(beginDate) && 
                x.VoucherDate <= endDate &&
                (!onlyBooked || (x.JournalVoucher != null && x.JournalVoucher.Bookflag == true))).ToList();

            decimal totalPeriodDebit = 0;
            decimal totalPeriodCredit = 0;

            foreach (var entry in periodEntries)
            {
                AddPeriodRow(result, accountsDict, entry.JournalVoucher, entry.Dbcr, entry.Amount, entry.AccountId, entry.CounterAccountId, (int)entry.EntryId, ref currentBalance, ref totalPeriodDebit, ref totalPeriodCredit);
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
