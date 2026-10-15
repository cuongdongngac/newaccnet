using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.CostTransfer
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

                    // 2. Tầng 2 của bảng phẳng: CostDetail -> JournalEntry -> JournalVoucher (không cần đối ứng)
                    var prefetch = new PrefetchPath2(EntityType.CostDetailEntity);
                    var pEntry = prefetch.Add(CostDetailEntity.PrefetchPathJournalEntry);
                    pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);

                    var details = new EntityCollection<CostDetailEntity>();
                    adapter.FetchEntityCollection(details, null, prefetch);

                    string sourceAccount = param.SourceAccountId.Trim();
                    DateTime endDate = param.EndDate.Date;

                    var flat = details
                        .Where(d => d.JournalEntry != null
                                    && string.Equals(d.JournalEntry.AccountId?.Trim(), sourceAccount, StringComparison.OrdinalIgnoreCase))
                        .Select(d => new
                        {
                            CostObjectId = d.CostObjectId?.Trim() ?? string.Empty,
                            CostElementId = d.CostElementId?.Trim() ?? string.Empty,
                            Dbcr = d.JournalEntry.Dbcr.GetValueOrDefault(1) == -1 ? -1 : 1,
                            Amount = d.Amount ?? 0.0,
                            // Không có chứng từ = số dư đầu kỳ: luôn nằm trong phạm vi.
                            VoucherDate = d.JournalEntry.JournalVoucher?.VoucherDate
                        })
                        .Where(x => x.VoucherDate == null || x.VoucherDate.Value.Date <= endDate)
                        .ToList();

                    // 3. Tầng 3: gom theo (đối tượng, yếu tố), Rest = Σ(Dbcr × Amount)
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
                            Rest = g.Sum(x => x.Dbcr * x.Amount)
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
