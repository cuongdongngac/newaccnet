using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;

namespace NewaccNet.Reports.TrialBalance
{
    public class AccountHierarchyRoller
    {
        /// <summary>
        /// Cộng dồn số liệu từ các tài khoản con (leaf) lên các tài khoản cha (parent)
        /// thông qua cấu trúc cây ChartOfAccount.
        /// </summary>
        public List<TrialBalanceEntity> RollupToParents(List<TrialBalanceEntity> leafBalances, List<ChartOfAccountEntity> chartOfAccounts)
        {
            var fullList = new List<TrialBalanceEntity>();
            
            // Đưa các tài khoản lá vào danh sách kết quả trước (gắn cờ Splite = 0)
            foreach (var leaf in leafBalances)
            {
                leaf.Splite = 0; // Con
                fullList.Add(leaf);
            }

            // Lọc ra các tài khoản cha (có CategoryId != null hoặc ParentId != null, 
            // hoặc đơn giản là lọc tất cả các Account có xuất hiện dưới tư cách là ParentId của account khác)
            // Cách đơn giản nhất: Xét mọi AccountId trong ChartOfAccounts
            var accountDict = chartOfAccounts.ToDictionary(a => a.AccountId, a => a);
            
            // Tạo hàm đệ quy để lấy toàn bộ danh sách các con cháu (descendants) của một parent
            List<string> GetAllLeafDescendants(string parentId)
            {
                var children = chartOfAccounts.Where(a => a.ParentId == parentId).Select(a => a.AccountId).ToList();
                var leaves = new List<string>();
                foreach (var child in children)
                {
                    var childDescendants = GetAllLeafDescendants(child);
                    if (childDescendants.Count == 0) leaves.Add(child);
                    else leaves.AddRange(childDescendants);
                }
                return leaves;
            }

            // Tìm các tài khoản đóng vai trò là Cha
            var parentIds = chartOfAccounts.Where(a => chartOfAccounts.Any(child => child.ParentId == a.AccountId)).Select(a => a.AccountId).Distinct().ToList();

            foreach (var parentId in parentIds)
            {
                var allLeafIds = GetAllLeafDescendants(parentId);
                
                // Lấy các TrialBalance của các leaf thuộc về parent này
                var childBalances = leafBalances.Where(b => allLeafIds.Contains(b.AccountId)).ToList();

                if (childBalances.Any())
                {
                    var parentInfo = accountDict.ContainsKey(parentId) ? accountDict[parentId] : null;
                    bool isDebtAccount = parentInfo != null && parentInfo.CategoryId == "A";

                    double sumBeginDebit = childBalances.Sum(b => b.Begindebit ?? 0);
                    double sumBeginCredit = childBalances.Sum(b => b.Begincredit ?? 0);
                    double sumEndDebit = childBalances.Sum(b => b.Enddebit ?? 0);
                    double sumEndCredit = childBalances.Sum(b => b.Endcredit ?? 0);

                    if (!isDebtAccount)
                    {
                        // Tài khoản thường: bù trừ số dư
                        if (sumBeginDebit > sumBeginCredit) { sumBeginDebit -= sumBeginCredit; sumBeginCredit = 0; }
                        else { sumBeginCredit -= sumBeginDebit; sumBeginDebit = 0; }

                        if (sumEndDebit > sumEndCredit) { sumEndDebit -= sumEndCredit; sumEndCredit = 0; }
                        else { sumEndCredit -= sumEndDebit; sumEndDebit = 0; }
                    }

                    fullList.Add(new TrialBalanceEntity
                    {
                        AccountId = parentId,
                        Begindebit = sumBeginDebit,
                        Begincredit = sumBeginCredit,
                        Intdebit = childBalances.Sum(b => b.Intdebit ?? 0),
                        Intcredit = childBalances.Sum(b => b.Intcredit ?? 0),
                        Enddebit = sumEndDebit,
                        Endcredit = sumEndCredit,
                        Splite = 1 // Cha
                    });
                }
            }

            return fullList.OrderBy(x => x.AccountId).ToList();
        }
    }
}
