using System;
using System.Collections.Generic;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.AssetDepreciation
{
    /// <summary>
    /// MANH 3 - TINH TOAN: bien AssetSnapshot thanh dong luoi kem so tien khau hao.
    /// Goi GetDepreciation (AssetDepreciationHelper.CalculateDepreciation) theo [BeginDate, EndDate].
    /// </summary>
    public interface IAssetDepreciationCalculator
    {
        List<AssetDepreciationRow> Calculate(IEnumerable<AssetSnapshot> snapshots, AssetDepreciationParams param);
    }

    public class AssetDepreciationCalculator : IAssetDepreciationCalculator
    {
        public List<AssetDepreciationRow> Calculate(IEnumerable<AssetSnapshot> snapshots, AssetDepreciationParams param)
        {
            var rows = new List<AssetDepreciationRow>();
            if (snapshots == null || param == null) return rows;

            foreach (var s in snapshots)
            {
                // Tinh muc khau hao cho 1 don vi tai san (Amount trong VBA)
                double unitAmount = AssetDepreciationHelper.CalculateDepreciation(
                    param.BeginDate,
                    param.EndDate,
                    s.PurchaseDate,
                    s.UsingYears,
                    s.DepRate,
                    s.OriginalValue);

                double qty = s.Quantity > 0 ? s.Quantity : 1.0;
                double totalAmount = Math.Round(unitAmount * qty, 0, MidpointRounding.AwayFromZero);

                // Khong khau hao vuot qua tri gia con lai (RestValue)
                if (s.RestValue > 0 && totalAmount > s.RestValue)
                {
                    totalAmount = s.RestValue;
                    unitAmount = Math.Round(totalAmount / qty, 0, MidpointRounding.AwayFromZero);
                }

                rows.Add(new AssetDepreciationRow
                {
                    Snapshot = s,
                    Selected = totalAmount > 0 && s.RestValue > 0,
                    UnitDepAmount = unitAmount,
                    DepAmount = totalAmount
                });
            }

            return rows;
        }
    }
}
