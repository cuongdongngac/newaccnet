using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Core.AssetDepreciation
{
    /// <summary>
    /// MANH 2 - NAP DU LIEU & LOC:
    /// 1. Tao AssetTrackingDep: lay ban ghi cuoi cung (Last) cua tung AssetId trong bang AssetTracking.
    /// 2. Bo loc Link Master/Child Fields: loc theo AsaccountId == param.DepAccountId (va SourceCode neu co).
    /// 3. Bang phang Union Asset Line Query: ket hop AssetDetail + JournalEntry + JournalVoucher + Reason (VoucherDate <= EndDate),
    ///    gom nhom theo (AssetId, Srcode) de tinh RestQty va Rest (Tri gia con lai).
    /// </summary>
    public interface IAssetSnapshotLoader
    {
        Task<List<AssetSnapshot>> LoadAsync(AssetDepreciationParams param);
    }

    public class AssetSnapshotLoader : IAssetSnapshotLoader
    {
        private const double Epsilon = 0.0001;

        public async Task<List<AssetSnapshot>> LoadAsync(AssetDepreciationParams param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));

            return await Task.Run(() =>
            {
                string filterAccount = param.DepAccountId?.Trim() ?? string.Empty;
                string filterSource = param.SourceCode?.Trim() ?? string.Empty;
                DateTime endDate = param.EndDate.Date;

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // 1. Nap danh muc Asset & Reason
                    var assets = new EntityCollection<AssetEntity>();
                    adapter.FetchEntityCollection(assets, null);
                    var assetDict = assets
                        .Where(a => !string.IsNullOrWhiteSpace(a.AssetId))
                        .ToDictionary(a => a.AssetId.Trim(), a => a, StringComparer.OrdinalIgnoreCase);

                    var reasons = new EntityCollection<ReasonEntity>();
                    adapter.FetchEntityCollection(reasons, null);
                    var reasonFlagDict = reasons
                        .Where(r => !string.IsNullOrWhiteSpace(r.Reasonid))
                        .ToDictionary(r => r.Reasonid.Trim(), r => r.Flag, StringComparer.OrdinalIgnoreCase);

                    // 2. Tao AssetTrackingDep: chi lay ban ghi cuoi cung (theo Assettrackingid) cua moi AssetId
                    var trackings = new EntityCollection<AssetTrackingEntity>();
                    adapter.FetchEntityCollection(trackings, null);

                    var latestTrackingByAsset = trackings
                        .Where(t => !string.IsNullOrWhiteSpace(t.AssetId))
                        .GroupBy(t => t.AssetId.Trim(), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderBy(t => t.Assettrackingid).Last(),
                            StringComparer.OrdinalIgnoreCase);

                    // Bo loc Link Master/Child Fields: AsAccount ID == SrAccountID (param.DepAccountId)
                    if (!string.IsNullOrEmpty(filterAccount))
                    {
                        latestTrackingByAsset = latestTrackingByAsset
                            .Where(kv => string.Equals(kv.Value.AsaccountId?.Trim(), filterAccount, StringComparison.OrdinalIgnoreCase))
                            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                    }

                    if (latestTrackingByAsset.Count == 0)
                        return new List<AssetSnapshot>();

                    // 3. Dung bang phang giao dich tai san (Union Asset Line Query)
                    var prefetch = new PrefetchPath2(EntityType.AssetDetailEntity);
                    var pEntry = prefetch.Add(AssetDetailEntity.PrefetchPathJournalEntry);
                    pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);

                    var details = new EntityCollection<AssetDetailEntity>();
                    adapter.FetchEntityCollection(details, null, prefetch);

                    var flatLines = details
                        .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.AssetId))
                        .Select(d =>
                        {
                            var je = d.JournalEntry;
                            var voucher = je.JournalVoucher;
                            bool isOpening = je.JournalVoucherId == null || voucher == null;
                            DateTime voucherDate = isOpening
                                ? DateTime.MinValue
                                : (voucher.VoucherDate ?? DateTime.MinValue).Date;

                            string assetId = d.AssetId.Trim();
                            string srcode = d.Srcode?.Trim() ?? string.Empty;
                            string reasonId = d.Reasonid?.Trim() ?? string.Empty;
                            bool reasonFlag = !string.IsNullOrEmpty(reasonId)
                                              && reasonFlagDict.TryGetValue(reasonId, out bool flg)
                                              && flg;

                            return new
                            {
                                AssetId = assetId,
                                Srcode = srcode,
                                Quantity = d.Quantity ?? 0.0,
                                Price = d.Price ?? 0.0,
                                Dbcr = (int)(je.Dbcr ?? 0),
                                VoucherDate = voucherDate,
                                ReasonFlag = reasonFlag
                            };
                        })
                        .Where(x => x.Dbcr != 0
                                    && x.VoucherDate <= endDate
                                    && latestTrackingByAsset.ContainsKey(x.AssetId)
                                    && (string.IsNullOrEmpty(filterSource) || string.Equals(x.Srcode, filterSource, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    // 4. Gom nhom theo (AssetId, Srcode) va tinh Rest, RestQty
                    var result = new List<AssetSnapshot>();

                    foreach (var grp in flatLines
                        .GroupBy(x => (AssetId: x.AssetId.ToUpperInvariant(), Srcode: x.Srcode.ToUpperInvariant()))
                        .OrderBy(g => g.Key.AssetId)
                        .ThenBy(g => g.Key.Srcode))
                    {
                        var sample = grp.First();
                        if (!latestTrackingByAsset.TryGetValue(sample.AssetId, out var tracking))
                            continue;

                        assetDict.TryGetValue(sample.AssetId, out var asset);

                        double remainValue = tracking.Remainvalue ?? 0.0;

                        // RestQty = Sum(Quantity * Dbcr) voi cac dong co Reason.Flag == true
                        double restQty = grp
                            .Where(x => x.ReasonFlag)
                            .Sum(x => x.Quantity * x.Dbcr);

                        // Rest = Sum(Quantity * (Price - (ReasonFlag ? RemainValue : 0)) * Dbcr)
                        // Tru Luy ke hao mon ban dau (RemainValue) tren dong tang tai san, va tru cac lan da khau hao tren so
                        double rest = grp.Sum(x =>
                        {
                            double effectivePrice = x.ReasonFlag ? (x.Price - remainValue) : x.Price;
                            return x.Quantity * effectivePrice * x.Dbcr;
                        });

                        if (restQty <= Epsilon && rest <= Epsilon)
                            continue;

                        // Quy doi Thoi gian su dung: trong Asset List / AssetTracking co the luu so nam hoac so thang
                        double rawTimeUsing = asset?.Timeusing ?? tracking.Timeusing ?? 0.0;
                        double depRate = tracking.Deprate ?? 0.0;
                        double usingYears = rawTimeUsing;
                        if (usingYears <= 0 && depRate > 0)
                        {
                            usingYears = 1.0 / depRate;
                        }

                        DateTime purchaseDate = asset?.Date ?? tracking.Begindate ?? param.BeginDate;
                        double depPrice = tracking.Depprice ?? asset?.Price ?? 0.0;

                        result.Add(new AssetSnapshot
                        {
                            AssetId = sample.AssetId,
                            AssetName = asset?.AssetName ?? sample.AssetId,
                            Unit = asset?.Unit ?? string.Empty,
                            SourceCode = sample.Srcode,
                            Quantity = restQty > Epsilon ? restQty : 1.0,
                            OriginalValue = depPrice,
                            InitialAccumulatedDep = remainValue,
                            RestValue = Math.Max(0.0, Math.Round(rest, 0, MidpointRounding.AwayFromZero)),
                            PurchaseDate = purchaseDate,
                            UsingYears = usingYears,
                            DepRate = depRate,
                            AssetAccountId = tracking.AsaccountId?.Trim() ?? string.Empty,
                            ExpenseAccountId = tracking.ExaccountId?.Trim() ?? string.Empty,
                            CostObjectId = tracking.ExobjectId?.Trim() ?? string.Empty,
                            DepartmentId = tracking.Deptid?.Trim() ?? string.Empty
                        });
                    }

                    return result;
                }
            });
        }
    }
}

