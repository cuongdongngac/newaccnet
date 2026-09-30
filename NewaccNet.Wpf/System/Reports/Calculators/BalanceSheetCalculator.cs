using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Reports.Calculators
{
    public class BalanceSheetReportModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Illu { get; set; }
        public decimal BeginBalance { get; set; }
        public decimal EndBalance { get; set; }
        public bool IsTitle { get; set; } 
        public int Order { get; set; }
    }

    /// <summary>
    /// Class chuyên biệt xử lý tính toán Bảng Cân Đối Kế Toán (B01-DN).
    /// Độc lập hoàn toàn với UI (WPF), trả về dữ liệu thuần (DTO).
    /// </summary>
    public class BalanceSheetCalculator
    {
        /// <summary>
        /// Thực thi tính toán bảng Cân đối kế toán
        /// </summary>
        public List<BalanceSheetReportModel> Calculate(DateTime fromDate, DateTime toDate)
        {
            var result = new List<BalanceSheetReportModel>();
            
            using var adapter = AppDataAccessAdapter.Create();

            // 1. Load cấu trúc Bảng cân đối (Section -> Category -> Item -> Formula -> Account)
            var sections = new EntityCollection<ReportSectionEntity>();
            var prefetch = new PrefetchPath2((int)DataAccess.EntityType.ReportSectionEntity);
            var catNode = prefetch.Add(ReportSectionEntity.PrefetchPathReportCategories);
            var itemNode = catNode.SubPath.Add(ReportCategoryEntity.PrefetchPathReportItems);
            var formulaNode = itemNode.SubPath.Add(ReportItemEntity.PrefetchPathReportFormulas);
            formulaNode.SubPath.Add(ReportFormulaEntity.PrefetchPathReportFormulaAccounts);

            adapter.FetchEntityCollection(sections, null, prefetch);

            // 2. Lấy số dư các tài khoản (Query từ DB)
            var accountBalances = GetAccountBalances(adapter, fromDate, toDate);

            // 3. Duyệt cấu trúc và map dữ liệu
            foreach (var section in sections.OrderBy(x => x.Id))
            {
                // Tiêu đề Mục lớn (VD: TÀI SẢN, NGUỒN VỐN)
                result.Add(new BalanceSheetReportModel
                {
                    ItemName = section.SectionName,
                    IsTitle = true,
                    Order = result.Count
                });

                if (section.ReportCategories == null) continue;

                foreach (var category in section.ReportCategories)
                {
                    // Tiêu đề Nhóm chỉ tiêu (VD: A. TÀI SẢN NGẮN HẠN)
                    result.Add(new BalanceSheetReportModel
                    {
                        ItemName = category.CategoryName,
                        ItemCode = category.Code,
                        IsTitle = true,
                        Order = result.Count
                    });

                    if (category.ReportItems == null) continue;

                    foreach (var item in category.ReportItems)
                    {
                        var reportItem = new BalanceSheetReportModel
                        {
                            ItemName = item.EntryName,
                            ItemCode = item.Code,
                            IsTitle = false,
                            Order = result.Count
                        };

                        decimal endBal = 0;
                        decimal beginBal = 0;

                        if (item.ReportFormulas != null)
                        {
                            // Chỉ duyệt các công thức được đánh dấu tính toán
                            foreach (var formula in item.ReportFormulas.Where(x => x.Calculate))
                            {
                                reportItem.Illu = formula.Illu;
                                
                                if (formula.ReportFormulaAccounts == null) continue;

                                foreach (var acc in formula.ReportFormulaAccounts)
                                {
                                    if (accountBalances.TryGetValue(acc.AccountId, out var bal))
                                    {
                                        // Phân tích toán tử (+, -)
                                        if (formula.Formula == "+")
                                        {
                                            endBal += bal.EndBalance;
                                            beginBal += bal.BeginBalance;
                                        }
                                        else if (formula.Formula == "-")
                                        {
                                            endBal -= bal.EndBalance;
                                            beginBal -= bal.BeginBalance;
                                        }
                                    }
                                }
                            }
                        }
                        
                        reportItem.EndBalance = endBal;
                        reportItem.BeginBalance = beginBal;
                        
                        result.Add(reportItem);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Query tổng hợp số dư/phát sinh tài khoản
        /// </summary>
        private Dictionary<string, (decimal BeginBalance, decimal EndBalance)> GetAccountBalances(SD.LLBLGen.Pro.ORMSupportClasses.IDataAccessAdapter adapter, DateTime fromDate, DateTime toDate)
        {
            var dict = new Dictionary<string, (decimal BeginBalance, decimal EndBalance)>();
            
            // TODO: Viết truy vấn GroupBy / Aggregate lên bảng JournalEntry / AccountBalance
            // Tạm thời mock 1 vài tài khoản để test form
            dict["111"] = (15000000, 18000000);
            dict["112"] = (50000000, 42000000);
            dict["131"] = (10000000, 5000000);
            dict["331"] = (20000000, 25000000);

            return dict;
        }
    }
}