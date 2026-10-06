using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports.BalanceSheet
{
    public class DebtBalanceService
    {
        public List<DebtBalanceEntity> GenerateDebtBalances(IDataAccessAdapter adapter, DateTime fromDate, DateTime toDate, DateTime reportDate, bool onlyBooked = true)
        {
            var debtBalances = new List<DebtBalanceEntity>();
            var qf = new QueryFactory();

            // 1. Kéo toàn bộ DebtDetail cùng với LiabilityDue, JournalEntry và JournalVoucher
            var debtQuery = qf.DebtDetail
                .WithPath(
                    DebtDetailEntity.PrefetchPathLiabilityDue,
                    DebtDetailEntity.PrefetchPathJournalEntry.WithSubPath(JournalEntryEntity.PrefetchPathJournalVoucher)
                );

            var rawDebts = adapter.FetchQuery(debtQuery).Cast<DebtDetailEntity>().ToList();

            // Chỉ lấy những DebtDetail hợp lệ, có JournalEntry và VoucherDate
            var validDebts = rawDebts.Where(d => d.JournalEntry != null && !string.IsNullOrEmpty(d.JournalEntry.AccountId));

            // 2. TÍNH DƯ ĐẦU, PHÁT SINH, DƯ CUỐI THEO TỪNG KHÁCH HÀNG
            var partnerBalances = validDebts
                .GroupBy(d => new 
                { 
                    AccountId = d.JournalEntry.AccountId, 
                    PartnerId = d.PartnerId 
                })
                .Select(g => 
                {
                    // Dư đầu kỳ (Trước fromDate)
                    decimal beginBalance = g.Where(x => x.JournalEntry.JournalVoucher == null || x.JournalEntry.JournalVoucher.VoucherDate < fromDate)
                                            .Sum(x => (x.JournalEntry.Dbcr == 1 ? 1m : -1m) * (decimal)(x.Amount ?? 0));
                                            
                    // Phát sinh trong kỳ
                    decimal inDebit = g.Where(x => x.JournalEntry.JournalVoucher != null && x.JournalEntry.JournalVoucher.VoucherDate >= fromDate && x.JournalEntry.JournalVoucher.VoucherDate <= toDate && x.JournalEntry.Dbcr == 1)
                                       .Sum(x => (decimal)(x.Amount ?? 0));
                    decimal inCredit = g.Where(x => x.JournalEntry.JournalVoucher != null && x.JournalEntry.JournalVoucher.VoucherDate >= fromDate && x.JournalEntry.JournalVoucher.VoucherDate <= toDate && x.JournalEntry.Dbcr != 1)
                                        .Sum(x => (decimal)(x.Amount ?? 0));
                                        
                    // Dư cuối kỳ = Dư đầu + PS Nợ - PS Có
                    decimal endBalance = beginBalance + inDebit - inCredit;

                    // Lấy ngày đáo hạn
                    DateTime? endDate = g.FirstOrDefault(x => x.LiabilityDue != null)?.LiabilityDue?.EndDate;
                    
                    // Xác định cờ kỳ hạn bằng Helper (sẽ ra 1 hoặc 2)
                    int termFlag = ReportCalculationHelper.GetLongtermFlag(reportDate, endDate);

                    return new 
                    {
                        AccountId = g.Key.AccountId,
                        PartnerId = g.Key.PartnerId,
                        BeginBalance = beginBalance,
                        InDebit = inDebit,
                        InCredit = inCredit,
                        EndBalance = endBalance,
                        LongtermFlag = termFlag
                    };
                }).ToList();

            // 3. GOM LÊN CẤP TÀI KHOẢN + KỲ HẠN
            var finalGroups = partnerBalances
                .GroupBy(p => new { p.AccountId, p.LongtermFlag });

            foreach (var g in finalGroups)
            {
                // Bù trừ số dư đầu theo từng Partner
                decimal sumBeginDebit = g.Where(x => x.BeginBalance > 0).Sum(x => x.BeginBalance);
                decimal sumBeginCredit = g.Where(x => x.BeginBalance < 0).Sum(x => Math.Abs(x.BeginBalance));
                
                // Phát sinh trong kỳ (Không bù trừ)
                decimal sumInDebit = g.Sum(x => x.InDebit);
                decimal sumInCredit = g.Sum(x => x.InCredit);
                
                // Bù trừ số dư cuối theo từng Partner
                decimal sumEndDebit = g.Where(x => x.EndBalance > 0).Sum(x => x.EndBalance);
                decimal sumEndCredit = g.Where(x => x.EndBalance < 0).Sum(x => Math.Abs(x.EndBalance));

                debtBalances.Add(new DebtBalanceEntity
                {
                    AccountId = g.Key.AccountId,
                    LongtermFlag = (short)g.Key.LongtermFlag,
                    Begindebit = (double)sumBeginDebit,
                    Begincredit = (double)sumBeginCredit,
                    Indebit = (double)sumInDebit,
                    Incredit = (double)sumInCredit,
                    Enddebit = (double)sumEndDebit,
                    Endcredit = (double)sumEndCredit,
                });
            }

            return debtBalances.OrderBy(x => x.AccountId).ThenBy(x => x.LongtermFlag).ToList();
        }
    }
}


