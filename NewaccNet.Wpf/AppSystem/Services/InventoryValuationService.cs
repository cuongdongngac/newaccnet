using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Services
{
    /// <summary>
    /// Phương pháp tính giá xuất kho.
    /// </summary>
    public enum CostingMethod
    {
        /// <summary>Bình quân gia quyền tức thời (tính tới thời điểm phát sinh).</summary>
        MovingAverage = 0,

        /// <summary>Nhập trước - Xuất trước (FIFO).</summary>
        Fifo = 1
    }

    /// <summary>
    /// Một dòng biến động kho (nhập hoặc xuất) của 1 vật tư tại 1 kho.
    /// </summary>
    public sealed class StockMovement
    {
        public string MaterialId { get; set; } = "";
        public string WarehouseId { get; set; } = "";
        public DateTime VoucherDate { get; set; }
        public int VoucherId { get; set; }
        public int LineId { get; set; }
        public bool IsReceipt { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Kết quả tồn kho cuối cùng của 1 cặp (Vật tư, Kho) tại thời điểm tính.
    /// </summary>
    public sealed class StockBalance
    {
        public string MaterialId { get; set; } = "";
        public string WarehouseId { get; set; } = "";
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public decimal Price { get; set; }
    }

    /// <summary>
    /// Kết quả tổng hợp sau khi chạy tính giá.
    /// </summary>
    public sealed class ValuationResult
    {
        public int PairCount { get; set; }
        public int MovementCount { get; set; }
        public int SkippedVoucherCount { get; set; }
    }

    public interface IInventoryValuationService
    {
        /// <summary>
        /// Tính lại giá tồn kho theo phương pháp đã chọn, tách riêng từng cặp (Vật tư, Kho),
        /// chỉ lấy các giao dịch có ngày chứng từ &lt;= asOfDate, rồi ghi kết quả vào bảng MaterialPrice.
        /// materialId / warehouseId = null → tính cho toàn bộ.
        /// </summary>
        ValuationResult Recalculate(CostingMethod method, DateTime? fromDate, DateTime asOfDate, bool rebuildFromScratch, string materialId = null, string warehouseId = null);
    }

    /// <summary>
    /// Thuật toán tính giá thuần (không phụ thuộc DB) – mỗi lần gọi xử lý 1 cặp (Vật tư, Kho).
    /// Input bắt buộc đã được sắp xếp đúng trình tự thời gian.
    /// </summary>
    public static class InventoryCostCalculator
    {
        private const int Precision = 6;

        /// <summary>
        /// Bình quân gia quyền tức thời:
        ///  - Nhập: Giá trị += SL * Đơn giá; SL += SL nhập; Giá BQ = Giá trị / SL.
        ///  - Xuất: xuất theo Giá BQ hiện tại; Giá trị -= SL xuất * Giá BQ; SL -= SL xuất.
        /// </summary>
        public static StockBalance CalculateMovingAverage(IEnumerable<StockMovement> orderedMovements)
        {
            decimal qty = 0, value = 0, avgPrice = 0;

            foreach (var m in orderedMovements)
            {
                if (m.Quantity == 0) continue;

                if (m.IsReceipt)
                {
                    qty += m.Quantity;
                    value += m.Quantity * m.Price;

                    // Nếu sau khi nhập vẫn âm/0 (đang âm kho) → lấy giá của lần nhập này làm giá BQ
                    avgPrice = qty > 0 ? Math.Round(value / qty, Precision) : m.Price;
                }
                else
                {
                    qty -= m.Quantity;
                    value -= m.Quantity * avgPrice;
                }

                // Chống rác số: khi về 0 hoặc âm, giá trị luôn = SL * Giá BQ cuối
                if (qty <= 0) value = qty * avgPrice;
            }

            return new StockBalance { Quantity = qty, Value = Math.Round(value, Precision), Price = avgPrice };
        }

        /// <summary>
        /// Nhập trước - Xuất trước:
        ///  - Mỗi lần nhập tạo 1 "lớp" tồn (SL, Đơn giá) xếp hàng theo thời gian.
        ///  - Xuất lấy dần từ lớp cũ nhất. Nếu xuất vượt tồn → ghi nhận lớp âm theo giá lớp cuối,
        ///    lần nhập kế tiếp sẽ bù vào lớp âm trước.
        ///  - Đơn giá tồn cuối = Tổng giá trị các lớp còn lại / Tổng SL còn lại.
        /// </summary>
        public static StockBalance CalculateFifo(IEnumerable<StockMovement> orderedMovements)
        {
            var layers = new LinkedList<(decimal Qty, decimal Price)>();
            decimal lastPrice = 0;

            foreach (var m in orderedMovements)
            {
                if (m.Quantity == 0) continue;

                if (m.IsReceipt)
                {
                    lastPrice = m.Price;
                    decimal remain = m.Quantity;

                    // Bù phần đang âm kho (nếu có)
                    if (layers.First != null && layers.First.Value.Qty < 0)
                    {
                        decimal shortage = -layers.First.Value.Qty;
                        decimal offset = Math.Min(shortage, remain);
                        remain -= offset;
                        if (offset == shortage) layers.RemoveFirst();
                        else layers.First.Value = (-(shortage - offset), layers.First.Value.Price);
                    }

                    if (remain > 0) layers.AddLast((remain, m.Price));
                }
                else
                {
                    decimal need = m.Quantity;

                    while (need > 0 && layers.First != null && layers.First.Value.Qty > 0)
                    {
                        var head = layers.First.Value;
                        decimal take = Math.Min(head.Qty, need);
                        need -= take;
                        lastPrice = head.Price;

                        if (take == head.Qty) layers.RemoveFirst();
                        else layers.First.Value = (head.Qty - take, head.Price);
                    }

                    // Xuất vượt tồn → lớp âm theo giá gần nhất
                    if (need > 0)
                    {
                        if (layers.First != null && layers.First.Value.Qty < 0)
                            layers.First.Value = (layers.First.Value.Qty - need, layers.First.Value.Price);
                        else
                            layers.AddFirst((-need, lastPrice));
                    }
                }
            }

            decimal qty = layers.Sum(l => l.Qty);
            decimal value = layers.Sum(l => l.Qty * l.Price);
            decimal price = qty != 0 ? Math.Round(value / qty, Precision) : lastPrice;

            return new StockBalance { Quantity = qty, Value = Math.Round(value, Precision), Price = price };
        }
    }

    /// <summary>
    /// Service tính giá và refresh lại bảng MaterialPrice (giá trị quy chiếu theo từng kho).
    /// </summary>
    public class InventoryValuationService : IInventoryValuationService
    {
        public ValuationResult Recalculate(CostingMethod method, DateTime? fromDate, DateTime asOfDate, bool rebuildFromScratch, string materialId = null, string warehouseId = null)
        {
            materialId = string.IsNullOrWhiteSpace(materialId) ? null : materialId.Trim();
            warehouseId = string.IsNullOrWhiteSpace(warehouseId) ? null : warehouseId.Trim();

            using (var adapter = AppDataAccessAdapter.Create())
            {
                // STEP 1: Thu thập toàn bộ biến động kho trong khoảng thời gian cần tính
                var movements = FetchMovements(adapter, fromDate, asOfDate, materialId, warehouseId, out int skipped);
                
                // Chuẩn bị tồn đầu kỳ nếu KHÔNG tính lại từ đầu
                Dictionary<(string, string), MaterialPriceEntity> existingPrices = null;
                if (!rebuildFromScratch)
                {
                    existingPrices = FetchCurrentPrices(adapter, materialId, warehouseId);
                }

                // STEP 2: Tính theo từng cặp (Vật tư, Kho)
                var balances = movements
                    .GroupBy(m => (m.MaterialId, m.WarehouseId))
                    .Select(g =>
                    {
                        var ordered = g
                            .OrderBy(m => m.VoucherDate.Date)
                            .ThenBy(m => m.IsReceipt ? 0 : 1)   // Cùng ngày: nhập trước, xuất sau
                            .ThenBy(m => m.VoucherDate)
                            .ThenBy(m => m.VoucherId)
                            .ThenBy(m => m.LineId);

                        StockBalance balance;
                        if (rebuildFromScratch)
                        {
                            balance = method == CostingMethod.Fifo
                                ? InventoryCostCalculator.CalculateFifo(ordered)
                                : InventoryCostCalculator.CalculateMovingAverage(ordered);
                        }
                        else
                        {
                            // Lấy số dư hiện tại làm gốc
                            var key = (g.Key.MaterialId, g.Key.WarehouseId);
                            decimal startQty = 0;
                            decimal startPrice = 0;
                            
                            if (existingPrices != null && existingPrices.TryGetValue(key, out var p))
                            {
                                startQty = (decimal)(p.Quantity ?? 0);
                                startPrice = (decimal)p.Price;
                            }
                            
                            // Tạo một movement giả đóng vai trò tồn đầu kỳ để mớm vào hàm tính
                            var openingMovement = new StockMovement
                            {
                                IsReceipt = true,
                                Quantity = startQty,
                                Price = startPrice,
                                VoucherDate = DateTime.MinValue
                            };
                            
                            var combined = new[] { openingMovement }.Concat(ordered);
                            
                            balance = method == CostingMethod.Fifo
                                ? InventoryCostCalculator.CalculateFifo(combined)
                                : InventoryCostCalculator.CalculateMovingAverage(combined);
                        }

                        balance.MaterialId = g.Key.MaterialId;
                        balance.WarehouseId = g.Key.WarehouseId;
                        return balance;
                    })
                    .ToList();

                // STEP 3: Ghi (UPSERT) vào MaterialPrice trong 1 transaction
                SaveMaterialPrices(adapter, balances, materialId, warehouseId);

                return new ValuationResult
                {
                    PairCount = balances.Count,
                    MovementCount = movements.Count,
                    SkippedVoucherCount = skipped
                };
            }
        }

        private static Dictionary<(string, string), MaterialPriceEntity> FetchCurrentPrices(IDataAccessAdapter adapter, string materialId, string warehouseId)
        {
            var filter = new RelationPredicateBucket();
            if (materialId != null) filter.PredicateExpression.Add(MaterialPriceFields.MaterialId == materialId);
            if (warehouseId != null) filter.PredicateExpression.Add(MaterialPriceFields.WarehouseId == warehouseId);

            var existing = new EntityCollection<MaterialPriceEntity>();
            adapter.FetchEntityCollection(existing, filter);

            return existing
                .Where(p => !string.IsNullOrWhiteSpace(p.MaterialId) && !string.IsNullOrWhiteSpace(p.WarehouseId))
                .GroupBy(p => (p.MaterialId.Trim(), p.WarehouseId.Trim()))
                .ToDictionary(g => g.Key, g => g.First());
        }

        /// <summary>
        /// Đọc InventoryVoucher (kèm dòng chi tiết, bút toán, chứng từ) và trải phẳng thành StockMovement.
        /// </summary>
        private static List<StockMovement> FetchMovements(IDataAccessAdapter adapter, DateTime? fromDate, DateTime asOfDate,
            string materialId, string warehouseId, out int skippedVoucherCount)
        {
            var bucket = new RelationPredicateBucket();
            bucket.Relations.Add(InventoryVoucherEntity.Relations.JournalEntryEntityUsingJournalEntryId);
            bucket.Relations.Add(JournalEntryEntity.Relations.JournalVoucherEntityUsingJournalVoucherId);
            
            bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate < asOfDate.Date.AddDays(1));
            if (fromDate.HasValue)
            {
                bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate >= fromDate.Value.Date);
            }
            if (warehouseId != null)
                bucket.PredicateExpression.Add(InventoryVoucherFields.WarehouseId == warehouseId);

            var path = new PrefetchPath2(EntityType.InventoryVoucherEntity);
            if (materialId != null)
                path.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines, 0,
                    new PredicateExpression(InventoryVoucherLineFields.MaterialId == materialId));
            else
                path.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);
            path.Add(InventoryVoucherEntity.PrefetchPathJournalEntry)
                .SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);

            var vouchers = new EntityCollection<InventoryVoucherEntity>();
            adapter.FetchEntityCollection(vouchers, bucket, path);

            var result = new List<StockMovement>();
            skippedVoucherCount = 0;

            foreach (var v in vouchers)
            {
                var jv = v.JournalEntry?.JournalVoucher;
                bool? isReceipt = ResolveIsReceipt(v);

                if (jv?.VoucherDate == null || isReceipt == null || string.IsNullOrWhiteSpace(v.WarehouseId))
                {
                    skippedVoucherCount++;
                    continue;
                }

                foreach (var line in v.InventoryVoucherLines)
                {
                    if (string.IsNullOrWhiteSpace(line.MaterialId)) continue;

                    result.Add(new StockMovement
                    {
                        MaterialId = line.MaterialId.Trim(),
                        WarehouseId = v.WarehouseId.Trim(),
                        VoucherDate = jv.VoucherDate.Value,
                        VoucherId = jv.Id,
                        LineId = line.Id,
                        IsReceipt = isReceipt.Value,
                        Quantity = (decimal)(line.Quantity ?? 0),
                        Price = (decimal)(line.Price ?? 0)
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Xác định phiếu Nhập/Xuất:
        ///  1. Ưu tiên loại phiếu do màn hình nhập liệu mới lưu (NM, NNB = Nhập; XB, XNB = Xuất).
        ///  2. Dữ liệu chuyển đổi từ hệ thống cũ: dựa vào chiều Nợ/Có của bút toán kho (1 = Nhập, -1 = Xuất).
        /// </summary>
        private static bool? ResolveIsReceipt(InventoryVoucherEntity v)
        {
            switch (v.ExobjectId?.Trim().ToUpperInvariant())
            {
                case "NM":
                case "NNB":
                    return true;
                case "XB":
                case "XNB":
                    return false;
            }

            var dbcr = v.JournalEntry?.Dbcr;
            if (dbcr == 1) return true;
            if (dbcr == -1) return false;
            return null;
        }

        /// <summary>
        /// UPSERT kết quả vào MaterialPrice. Cặp (Vật tư, Kho) trong phạm vi tính mà không còn phát sinh
        /// đến ngày tính → đưa SL tồn về 0 (giữ nguyên đơn giá cũ để tham chiếu).
        /// </summary>
        private static void SaveMaterialPrices(IDataAccessAdapter adapter, List<StockBalance> balances,
            string materialId, string warehouseId)
        {
            var filter = new RelationPredicateBucket();
            if (materialId != null) filter.PredicateExpression.Add(MaterialPriceFields.MaterialId == materialId);
            if (warehouseId != null) filter.PredicateExpression.Add(MaterialPriceFields.WarehouseId == warehouseId);

            var existing = new EntityCollection<MaterialPriceEntity>();
            adapter.FetchEntityCollection(existing, filter);

            var lookup = existing
                .Where(p => !string.IsNullOrWhiteSpace(p.MaterialId) && !string.IsNullOrWhiteSpace(p.WarehouseId))
                .GroupBy(p => (p.MaterialId.Trim(), p.WarehouseId.Trim()))
                .ToDictionary(g => g.Key, g => g.First());

            var uow = new UnitOfWork2();
            var touched = new HashSet<MaterialPriceEntity>();

            foreach (var b in balances)
            {
                if (!lookup.TryGetValue((b.MaterialId, b.WarehouseId), out var row))
                {
                    row = new MaterialPriceEntity { MaterialId = b.MaterialId, WarehouseId = b.WarehouseId };
                }

                row.Quantity = (double)b.Quantity;
                row.Price = (double)b.Price;
                uow.AddForSave(row);
                touched.Add(row);
            }

            foreach (var row in lookup.Values.Where(r => !touched.Contains(r)))
            {
                row.Quantity = 0;
                uow.AddForSave(row);
            }

            uow.Commit(adapter, true);
        }
    }
}
