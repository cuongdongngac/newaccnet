using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Reports.FlattenedDetails;

namespace NewaccNet.Core.CostTransfer
{
    public class CostTransferService : ICostTransferService
    {
        private const double Epsilon = 0.0001;

        public async Task<List<CostTransferRow>> CalculateAsync(CostTransferParams param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param));
            if (string.IsNullOrWhiteSpace(param.SourceAccountId))
                throw new ArgumentException("Chưa chọn tài khoản nguồn.");

            return await Task.Run(() =>
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // 1. Danh mục tên đối tượng / yếu tố chi phí
                    var costObjects = new EntityCollection<CostObjectEntity>();
                    adapter.FetchEntityCollection(costObjects, null);
                    var objNames = costObjects
                        .Where(o => !string.IsNullOrWhiteSpace(o.Id))
                        .ToDictionary(o => o.Id.Trim(), o => o.ObjectName ?? o.Id, StringComparer.OrdinalIgnoreCase);

                    var costElements = new EntityCollection<CostElementEntity>();
                    adapter.FetchEntityCollection(costElements, null);
                    var elemNames = costElements
                        .Where(e => !string.IsNullOrWhiteSpace(e.Id))
                        .ToDictionary(e => e.Id.Trim(), e => e.ElementName ?? e.Id, StringComparer.OrdinalIgnoreCase);

                    // 2. Tầng 2 của bảng phẳng: dùng chung FlattenedDetailService.LoadCostFlatRows
                    string sourceAccount = param.SourceAccountId.Trim();
                    var flat = FlattenedDetailService.LoadCostFlatRows(
                        adapter,
                        accountPrefix: sourceAccount,
                        toDate: param.EndDate.Date);

                    // 3. Tầng 3: gom theo (đối tượng, yếu tố), Rest = Σ(SignedAmount)
                    return flat
                        .GroupBy(x => new { x.CostObjectId, x.CostElementId })
                        .Select(g => new CostTransferRow
                        {
                            AccountId = sourceAccount,
                            CostObjectId = g.Key.CostObjectId,
                            CostObjectName = string.IsNullOrEmpty(g.Key.CostObjectId)
                                ? "(Chi phí dùng chung)"
                                : (objNames.TryGetValue(g.Key.CostObjectId, out var on) ? on : g.Key.CostObjectId),
                            CostElementId = g.Key.CostElementId,
                            CostElementName = elemNames.TryGetValue(g.Key.CostElementId, out var en) ? en : g.Key.CostElementId,
                            Rest = g.Sum(x => x.SignedAmount)
                        })
                        .Where(r => Math.Abs(r.Rest) > Epsilon)
                        .OrderBy(r => r.CostObjectId)
                        .ThenBy(r => r.CostElementId)
                        .ToList();
                }
            });
        }
    }
}

