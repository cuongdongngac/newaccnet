using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.AssetDepreciation
{
    /// <summary>
    /// MANH 4 - GHI SO (ExTransfer & AddingExAccountID trong vba.txt):
    /// - Kiem tra du 4 dieu kien: DepAccountId (SrAccountID), CostElementId (ExAtribute ID), SourceCode (SrCode), ReasonId (ReasonID).
    /// - Sinh 1 JournalVoucher (Bills) tai EndDate, Bookflag = true.
    /// - Sinh 1 JournalEntry CHA (Ghi CO - Dbcr = -1) cho DepAccountId, kem cac dong AssetDetailEntity.
    /// - Voi moi tai san duoc chon, sinh 1 JournalEntry CON (Ghi NO - Dbcr = +1, ParentEntry = creditEntry)
    ///   cho ExaccountId cua tai san do, kem 1 dong CostDetailEntity (CostObjectId = ExobjectId, CostElementId = CostElementId tren form).
    /// - Ho tro Khoi phuc (Undo): xoa chung tu khau hao vua sinh.
    /// </summary>
    public interface IAssetDepreciationVoucherBuilder
    {
        Task<int> CreateVoucherAsync(AssetDepreciationParams param, IReadOnlyList<AssetDepreciationRow> selectedRows);
        Task RestoreVoucherAsync(int voucherId);
    }

    public class AssetDepreciationVoucherBuilder : IAssetDepreciationVoucherBuilder
    {
        public async Task<int> CreateVoucherAsync(AssetDepreciationParams param, IReadOnlyList<AssetDepreciationRow> selectedRows)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            // Kiem tra dieu kien bat buoc giong dong 69-75 trong vba.txt
            if (string.IsNullOrWhiteSpace(param.DepAccountId)
                || string.IsNullOrWhiteSpace(param.CostElementId)
                || string.IsNullOrWhiteSpace(param.ReasonId))
            {
                throw new InvalidOperationException("Hãy điền đầy đủ các dữ kiện (Tài khoản khấu hao, Yếu tố chi phí, Lý do) trước khi khấu hao!");
            }

            if (selectedRows == null || selectedRows.Count == 0)
            {
                throw new InvalidOperationException("Chưa chọn tài sản nào có số tiền khấu hao > 0 để trích khấu hao.");
            }

            // Kiem tra tung dong tai san duoc chon phai co TK chi phi dich (ExaccountId) trong AssetTracking
            var missingExAccount = selectedRows.FirstOrDefault(r => string.IsNullOrWhiteSpace(r.ExpenseAccountId));
            if (missingExAccount != null)
            {
                throw new InvalidOperationException(
                    $"Tài sản '{missingExAccount.AssetName}' ({missingExAccount.AssetId}) chưa được khai báo Tài khoản chi phí (ExAccount ID) trong Sổ theo dõi TSCĐ.");
            }

            return await Task.Run(() =>
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    double allAmount = selectedRows.Sum(r => r.DepAmount);
                    if (allAmount <= 0.0001)
                    {
                        throw new InvalidOperationException("Tổng số tiền khấu hao bằng 0.");
                    }

                    // 1. Tao JournalVoucher (Bills)
                    string contents = !string.IsNullOrWhiteSpace(param.Contents)
                        ? param.Contents.Trim()
                        : (!string.IsNullOrWhiteSpace(param.CostElementName)
                            ? param.CostElementName.Trim()
                            : $"Trích khấu hao TSCĐ TK {param.DepAccountId.Trim()}");

                    var voucher = new JournalVoucherEntity
                    {
                        VoucherDate = param.EndDate.Date,
                        VoucherNo = GenerateNextVoucherNo(adapter, param.EndDate.Date),
                        Contents = contents,
                        Bookflag = true
                    };

                    // 2. Tao but toan CHA - Ghi CO (Credit Site - Asset Account: SrAccountID)
                    var creditEntry = new JournalEntryEntity
                    {
                        AccountId = param.DepAccountId.Trim(),
                        Dbcr = -1,
                        Amount = allAmount,
                        Createdate = DateTime.Now
                    };

                    foreach (var row in selectedRows)
                    {
                        double qty = row.Quantity > 0.0001 ? row.Quantity : 1.0;
                        double unitPrice = row.UnitDepAmount > 0.0001
                            ? row.UnitDepAmount
                            : Math.Round(row.DepAmount / qty, 0, MidpointRounding.AwayFromZero);

                        string srcode = !string.IsNullOrWhiteSpace(row.SourceCode)
                            ? row.SourceCode.Trim()
                            : param.SourceCode.Trim();

                        var assetDetail = new AssetDetailEntity
                        {
                            AssetId = row.AssetId.Trim(),
                            Quantity = qty,
                            Price = unitPrice,
                            Reasonid = param.ReasonId.Trim()
                        };
                        if (!string.IsNullOrEmpty(srcode))
                        {
                            assetDetail.Srcode = srcode;
                        }

                        creditEntry.AssetDetails.Add(assetDetail);
                    }

                    voucher.JournalEntries.Add(creditEntry);

                    // 3. Tao cac but toan CON doi ung - Ghi NO (AddingExAccountID)
                    foreach (var row in selectedRows)
                    {
                        var debitEntry = new JournalEntryEntity
                        {
                            AccountId = row.ExpenseAccountId.Trim(),
                            Dbcr = 1,
                            Amount = row.DepAmount,
                            ParentEntry = creditEntry,
                            Createdate = DateTime.Now
                        };

                        var costDetail = new CostDetailEntity
                        {
                            CostElementId = param.CostElementId.Trim(),
                            Amount = row.DepAmount,
                            Transflag = true
                        };
                        if (!string.IsNullOrWhiteSpace(row.CostObjectId))
                        {
                            costDetail.CostObjectId = row.CostObjectId.Trim();
                        }

                        debitEntry.CostDetails.Add(costDetail);
                        voucher.JournalEntries.Add(debitEntry);
                    }

                    // 4. Luu toan bo graph trong 1 transaction
                    adapter.SaveEntity(voucher, true, recurse: true);
                    return voucher.Id;
                }
            });
        }

        public async Task RestoreVoucherAsync(int voucherId)
        {
            if (voucherId <= 0) return;

            await Task.Run(() =>
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var prefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
                    var pEntries = prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                    pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathAssetDetails);
                    pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathCostDetails);

                    var voucher = new JournalVoucherEntity(voucherId);
                    if (!adapter.FetchEntity(voucher, prefetch)) return;

                    var assetDetails = new EntityCollection<AssetDetailEntity>();
                    var costDetails = new EntityCollection<CostDetailEntity>();

                    foreach (var je in voucher.JournalEntries)
                    {
                        foreach (var ad in je.AssetDetails) assetDetails.Add(ad);
                        foreach (var cd in je.CostDetails) costDetails.Add(cd);
                    }

                    if (assetDetails.Count > 0) adapter.DeleteEntityCollection(assetDetails);
                    if (costDetails.Count > 0) adapter.DeleteEntityCollection(costDetails);

                    // Xoa but toan con truoc (co ParentId), but toan cha sau
                    var childEntries = voucher.JournalEntries.Where(e => e.ParentId != null).ToList();
                    if (childEntries.Count > 0)
                    {
                        adapter.DeleteEntityCollection(new EntityCollection<JournalEntryEntity>(childEntries));
                    }

                    var parentEntries = voucher.JournalEntries.Where(e => e.ParentId == null).ToList();
                    if (parentEntries.Count > 0)
                    {
                        adapter.DeleteEntityCollection(new EntityCollection<JournalEntryEntity>(parentEntries));
                    }

                    adapter.DeleteEntity(voucher);
                }
            });
        }

        private static string GenerateNextVoucherNo(IDataAccessAdapter adapter, DateTime voucherDate)
        {
            try
            {
                string prefix = "KH" + voucherDate.ToString("yyMM");
                var bucket = new RelationPredicateBucket(JournalVoucherFields.VoucherNo % (prefix + "%"));
                var existing = new EntityCollection<JournalVoucherEntity>();
                adapter.FetchEntityCollection(existing, bucket);

                int maxSeq = 0;
                foreach (var v in existing)
                {
                    if (string.IsNullOrWhiteSpace(v.VoucherNo)) continue;
                    string vn = v.VoucherNo.Trim();
                    int dashIdx = vn.LastIndexOf('-');
                    string numPart = dashIdx >= 0 ? vn.Substring(dashIdx + 1) : vn.Substring(prefix.Length);
                    if (int.TryParse(numPart, out int seq) && seq > maxSeq)
                        maxSeq = seq;
                }

                return $"{prefix}-{(maxSeq + 1):D2}";
            }
            catch
            {
                return "KH" + DateTime.Now.ToString("yyMMddHHmm");
            }
        }
    }
}
