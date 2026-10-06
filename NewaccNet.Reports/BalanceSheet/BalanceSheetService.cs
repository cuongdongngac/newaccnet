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
    public class BalanceSheetService
    {
        public List<BalanceSheetReportDto> GenerateReport(IDataAccessAdapter adapter)
        {
            var result = new List<BalanceSheetReportDto>();
            var qf = new QueryFactory();

            // 1. Tải toàn bộ cấu trúc báo cáo (Section -> Category -> Item -> Formula -> Account)
            var prefetch = new PrefetchPath2((int)DataAccess.EntityType.ReportSectionEntity);
            var catNode = prefetch.Add(ReportSectionEntity.PrefetchPathReportCategories);
            var itemNode = catNode.SubPath.Add(ReportCategoryEntity.PrefetchPathReportItems);
            var formulaNode = itemNode.SubPath.Add(ReportItemEntity.PrefetchPathReportFormulas);
            formulaNode.SubPath.Add(ReportFormulaEntity.PrefetchPathReportFormulaAccounts);

            var sections = adapter.FetchQuery(qf.ReportSection.WithPath(ReportSectionEntity.PrefetchPathReportCategories.WithSubPath(ReportCategoryEntity.PrefetchPathReportItems.WithSubPath(ReportItemEntity.PrefetchPathReportFormulas.WithSubPath(ReportFormulaEntity.PrefetchPathReportFormulaAccounts))))).Cast<ReportSectionEntity>().ToList();

            // 2. Tải toàn bộ TrialBalance và DebtBalance lên RAM
            var trialBalances = adapter.FetchQuery(qf.TrialBalance).Cast<TrialBalanceEntity>().ToList();
            var debtBalances = adapter.FetchQuery(qf.DebtBalance).Cast<DebtBalanceEntity>().ToList();

            // 3. Duyệt cấu trúc và bung ra các DTO ở cấp độ Tài Khoản
            foreach (var section in sections.OrderBy(s => s.Id))
            {
                foreach (var category in section.ReportCategories.OrderBy(c => c.Id))
                {
                    foreach (var item in category.ReportItems.OrderBy(i => i.Id))
                    {
                        foreach (var formula in item.ReportFormulas.Where(f => f.Calculate).OrderBy(f => f.Id))
                        {
                            foreach (var acc in formula.ReportFormulaAccounts)
                            {
                                decimal beginAmount = 0;
                                decimal endAmount = 0;

                                // Xác định lấy dữ liệu từ đâu: TrialBalance hay DebtBalance
                                // Công nợ thường có Longorshortterm = 1 (Ngắn) hoặc 2 (Dài)
                                if (formula.Longorshortterm.HasValue && formula.Longorshortterm.Value > 0)
                                {
                                    // Lấy từ DebtBalance, match AccountId và LongtermFlag
                                    var matchingDebts = debtBalances.Where(d => d.AccountId == acc.AccountId && d.LongtermFlag == formula.Longorshortterm.Value);
                                    
                                    // Bù trừ dựa trên section.Indebit (Tài sản hay Nguồn vốn)
                                    foreach (var db in matchingDebts)
                                    {
                                        if (section.Indebit)
                                        {
                                            beginAmount += (decimal)((db.Begindebit ?? 0) - (db.Begincredit ?? 0));
                                            endAmount += (decimal)((db.Enddebit ?? 0) - (db.Endcredit ?? 0));
                                        }
                                        else
                                        {
                                            beginAmount += (decimal)((db.Begincredit ?? 0) - (db.Begindebit ?? 0));
                                            endAmount += (decimal)((db.Endcredit ?? 0) - (db.Enddebit ?? 0));
                                        }
                                    }
                                }
                                else
                                {
                                    // Lấy từ TrialBalance
                                    var tb = trialBalances.FirstOrDefault(t => t.AccountId == acc.AccountId);
                                    if (tb != null)
                                    {
                                        if (section.Indebit)
                                        {
                                            beginAmount = (decimal)((tb.Begindebit ?? 0) - (tb.Begincredit ?? 0));
                                            endAmount = (decimal)((tb.Enddebit ?? 0) - (tb.Endcredit ?? 0));
                                        }
                                        else
                                        {
                                            beginAmount = (decimal)((tb.Begincredit ?? 0) - (tb.Begindebit ?? 0));
                                            endAmount = (decimal)((tb.Endcredit ?? 0) - (tb.Enddebit ?? 0));
                                        }
                                    }
                                }

                                // Tính toán dấu (+) (-)
                                if (formula.Formula == "-")
                                {
                                    beginAmount *= -1;
                                    endAmount *= -1;
                                }

                                // Chỉ add vào DTO nếu có dữ liệu để giảm dung lượng? 
                                // User muốn click vào xem chi tiết, ta cứ add tất cả các account thuộc formula đó.
                                result.Add(new BalanceSheetReportDto
                                {
                                    SectionId = section.Id,
                                    SectionName = section.SectionName,
                                    InDebit = section.Indebit,

                                    CategoryId = category.Id,
                                    CategoryCode = category.Code,
                                    CategoryName = category.CategoryName,

                                    ItemId = item.Id,
                                    ItemCode = item.Code,
                                    EntryName = item.EntryName,

                                    FormulaId = formula.Id,
                                    FormulaCode = formula.Code,
                                    ItemsName = formula.ItemsName,
                                    Illu = formula.Illu,
                                    Order = formula.Order,
                                    Calculate = formula.Calculate,
                                    Print = formula.Print,

                                    AccountId = acc.AccountId,
                                    BeginAmount = beginAmount,
                                    EndAmount = endAmount
                                });
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}

