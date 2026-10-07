using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Services
{
    /// <summary>
    /// Kết quả sau khi ghi nhận 1 nghiệp vụ NHẬP KHO.
    /// </summary>
    public sealed class GoodsReceiptResult
    {
        public bool IsNewRecord { get; set; }
        public double OldQuantity { get; set; }
        public double OldPrice { get; set; }
        public double NewQuantity { get; set; }
        public double NewPrice { get; set; }
    }

    /// <summary>
    /// Kết quả sau khi ghi nhận 1 nghiệp vụ XUẤT KHO.
    /// </summary>
    public sealed class GoodsIssueResult
    {
        /// <summary>Đơn giá xuất kho = Current_Price tại thời điểm xuất → dùng ghi vào phiếu xuất / bút toán giá vốn.</summary>
        public double IssuePrice { get; set; }

        /// <summary>Giá trị xuất kho = Output_Quantity * IssuePrice.</summary>
        public double IssueAmount { get; set; }

        public double OldQuantity { get; set; }
        public double RemainQuantity { get; set; }

        /// <summary>Phần xuất vượt tồn (Output_Quantity - Current_Quantity) nếu có, để cảnh báo người dùng.</summary>
        public double ShortageQuantity { get; set; }

        /// <summary>false = chưa từng có record (Vật tư, Kho) trong MaterialPrice → không có giá xuất.</summary>
        public bool HasPriceRecord { get; set; }
    }

    /// <summary>
    /// "Kho giá" trong bộ nhớ cho phương pháp Bình quân gia quyền liên hoàn theo từng kho.
    ///
    /// Cách dùng (kiểu document/NoSQL – làm việc trên object, không SELECT lẻ từng dòng):
    ///   1. Load(...)        : nạp MaterialPrice vào bộ nhớ MỘT lần, đánh index theo (Vật tư, Kho).
    ///   2. ApplyReceipt / ApplyIssue : tính toán thuần trên entity trong bộ nhớ, không chạm DB.
    ///   3. SaveChanges(...) : ORM tự sinh INSERT (entity mới) / UPDATE (entity dirty) trong 1 transaction.
    /// </summary>
    public sealed class MaterialPriceStore
    {
        /// <summary>Số lẻ giữ lại cho đơn giá bình quân.</summary>
        private const int PricePrecision = 6;

        /// <summary>Ngưỡng coi như bằng 0 (chống rác số thực).</summary>
        private const decimal Epsilon = 0.000001m;

        private readonly EntityCollection<MaterialPriceEntity> _items = new EntityCollection<MaterialPriceEntity>();
        private readonly Dictionary<string, MaterialPriceEntity> _index = new Dictionary<string, MaterialPriceEntity>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// true  = khi kho về 0 thì giữ nguyên Price cũ làm giá tham chiếu (mặc định).
        /// false = khi kho về 0 thì reset Price = 0.
        /// </summary>
        public bool KeepPriceWhenEmpty { get; set; } = true;

        /// <summary>Toàn bộ object giá đang nằm trong bộ nhớ.</summary>
        public IEnumerable<MaterialPriceEntity> Items => _items;

        private MaterialPriceStore() { }

        // =====================================================================
        // Nạp / Lưu
        // =====================================================================

        /// <summary>
        /// Nạp MaterialPrice vào bộ nhớ (1 lần fetch). materialId / warehouseId = null → nạp toàn bộ.
        /// Dữ liệu cũ bị trùng (Vật tư, Kho) → dùng dòng Id nhỏ nhất.
        /// </summary>
        public static MaterialPriceStore Load(IDataAccessAdapter adapter, string materialId = null, string warehouseId = null)
        {
            var store = new MaterialPriceStore();

            var bucket = new RelationPredicateBucket();
            if (!string.IsNullOrWhiteSpace(materialId)) bucket.PredicateExpression.Add(MaterialPriceFields.MaterialId == materialId.Trim());
            if (!string.IsNullOrWhiteSpace(warehouseId)) bucket.PredicateExpression.Add(MaterialPriceFields.WarehouseId == warehouseId.Trim());

            var sorter = new SortExpression(MaterialPriceFields.Id | SortOperator.Ascending);
            adapter.FetchEntityCollection(store._items, bucket, 0, sorter);

            foreach (var item in store._items)
            {
                if (string.IsNullOrWhiteSpace(item.MaterialId) || string.IsNullOrWhiteSpace(item.WarehouseId)) continue;
                string key = Key(item.MaterialId, item.WarehouseId);
                if (!store._index.ContainsKey(key)) store._index[key] = item;
            }

            return store;
        }

        /// <summary>
        /// Lưu các object đã thay đổi / thêm mới. ORM chỉ ghi entity dirty hoặc new.
        /// Nếu adapter đang trong transaction của người gọi (VD: lưu chứng từ) → chạy chung, không tự commit.
        /// </summary>
        public int SaveChanges(IDataAccessAdapter adapter)
        {
            if (adapter.IsTransactionInProgress)
                return adapter.SaveEntityCollection(_items, false, false);

            adapter.StartTransaction(IsolationLevel.ReadCommitted, "MaterialPriceStore");
            try
            {
                int saved = adapter.SaveEntityCollection(_items, false, false);
                adapter.Commit();
                return saved;
            }
            catch
            {
                adapter.Rollback();
                throw;
            }
        }

        // =====================================================================
        // Truy cập object trong bộ nhớ
        // =====================================================================

        public MaterialPriceEntity Find(string materialId, string warehouseId)
        {
            ValidateKey(materialId, warehouseId);
            return _index.TryGetValue(Key(materialId, warehouseId), out var e) ? e : null;
        }

        public MaterialPriceEntity GetOrAdd(string materialId, string warehouseId)
        {
            var entity = Find(materialId, warehouseId);
            if (entity != null) return entity;

            entity = new MaterialPriceEntity
            {
                MaterialId = materialId.Trim(),
                WarehouseId = warehouseId.Trim(),
                Quantity = 0,
                Price = 0
            };
            _items.Add(entity);
            _index[Key(materialId, warehouseId)] = entity;
            return entity;
        }

        // =====================================================================
        // 1. TÍNH LIÊN HOÀN: BÌNH QUÂN GIA QUYỀN (Moving Average)
        // =====================================================================
        public GoodsReceiptResult ApplyMovingAverageReceipt(string materialId, string warehouseId, double quantity, double price)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng nhập phải lớn hơn 0.", nameof(quantity));
            if (price < 0) throw new ArgumentException("Đơn giá nhập không được âm.", nameof(price));

            var record = Find(materialId, warehouseId);
            var result = new GoodsReceiptResult();

            if (record == null)
            {
                record = GetOrAdd(materialId, warehouseId);
                record.Quantity = quantity;
                record.Price = price;
                result.IsNewRecord = true;
            }
            else
            {
                decimal curQty = (decimal)(record.Quantity ?? 0);
                decimal curPrice = (decimal)record.Price;
                if (curQty < 0) curQty = 0;

                result.OldQuantity = (double)curQty;
                result.OldPrice = (double)curPrice;

                decimal inQty = (decimal)quantity;
                decimal inPrice = (decimal)price;

                decimal newQty = curQty + inQty;
                decimal newTotalValue = (curQty * curPrice) + (inQty * inPrice);

                decimal newPrice = Math.Abs(newQty) > Epsilon
                    ? Math.Round(newTotalValue / newQty, PricePrecision)
                    : inPrice;

                record.Quantity = (double)newQty;
                record.Price = (double)newPrice;
            }

            result.NewQuantity = record.Quantity ?? 0;
            result.NewPrice = record.Price;
            return result;
        }

        public GoodsIssueResult ApplyMovingAverageIssue(string materialId, string warehouseId, double quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Số lượng xuất phải lớn hơn 0.", nameof(quantity));

            var record = Find(materialId, warehouseId);
            if (record == null)
            {
                return new GoodsIssueResult { HasPriceRecord = false, ShortageQuantity = quantity };
            }

            decimal curQty = (decimal)(record.Quantity ?? 0);
            decimal curPrice = (decimal)record.Price;
            decimal outQty = (decimal)quantity;

            var result = new GoodsIssueResult
            {
                HasPriceRecord = true,
                IssuePrice = (double)curPrice,
                IssueAmount = (double)Math.Round(outQty * curPrice, PricePrecision),
                OldQuantity = (double)curQty
            };

            decimal remainQty = curQty - outQty;

            if (remainQty <= Epsilon)
            {
                result.ShortageQuantity = remainQty < -Epsilon ? (double)(-remainQty) : 0;
                remainQty = 0;
                if (!KeepPriceWhenEmpty) record.Price = 0;
            }

            record.Quantity = (double)remainQty;
            result.RemainQuantity = (double)remainQty;
            return result;
        }

        // =====================================================================
        // 2. TÍNH LIÊN HOÀN (BACKGROUND): NHẬP TRƯỚC XUẤT TRƯỚC (FIFO)
        // =====================================================================
        /// <summary>
        /// Kích hoạt chạy ngầm sau khi lưu phiếu. Sẽ query lại lịch sử của riêng 1 vật tư + kho 
        /// để chốt ra tồn/giá FIFO hiện tại, đưa vào in-memory index và sẵn sàng save xuống DB.
        /// </summary>
        public void ApplyFifoBackground(IDataAccessAdapter adapter, string materialId, string warehouseId)
        {
            ValidateKey(materialId, warehouseId);

            // Truy vấn lịch sử chỉ của đúng mã vật tư & kho này
            var bucket = new RelationPredicateBucket();
            bucket.Relations.Add(InventoryVoucherEntity.Relations.JournalEntryEntityUsingJournalEntryId);
            bucket.Relations.Add(JournalEntryEntity.Relations.JournalVoucherEntityUsingJournalVoucherId);
            
            bucket.PredicateExpression.Add(InventoryVoucherFields.WarehouseId == warehouseId);
            bucket.PredicateExpression.Add(InventoryVoucherLineFields.MaterialId == materialId);

            var lines = new EntityCollection<InventoryVoucherLineEntity>();
            var prefetch = new PrefetchPath2(EntityType.InventoryVoucherLineEntity);
            prefetch.Add(InventoryVoucherLineEntity.PrefetchPathInventoryVoucher)
                    .SubPath.Add(InventoryVoucherEntity.PrefetchPathJournalEntry)
                    .SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);

            adapter.FetchEntityCollection(lines, bucket, prefetch);

            // Chuyển về dạng phẳng
            var movements = new List<StockMovement>();
            foreach (var line in lines)
            {
                var voucher = line.InventoryVoucher;
                if (voucher?.JournalEntry?.JournalVoucher == null) continue;
                
                bool isReceipt = false;
                if (!string.IsNullOrEmpty(voucher.ExobjectId))
                    isReceipt = voucher.ExobjectId.Equals("NM", StringComparison.OrdinalIgnoreCase) || 
                                voucher.ExobjectId.Equals("NNB", StringComparison.OrdinalIgnoreCase);
                else
                    isReceipt = voucher.JournalEntry.Dbcr == 1;

                movements.Add(new StockMovement
                {
                    VoucherDate = voucher.JournalEntry?.JournalVoucher?.VoucherDate ?? DateTime.MinValue,
                    VoucherId = voucher.Id,
                    LineId = line.Id,
                    IsReceipt = isReceipt,
                    Quantity = (decimal)(line.Quantity ?? 0),
                    Price = (decimal)(line.Price ?? 0),
                    MaterialId = line.MaterialId,
                    WarehouseId = voucher.WarehouseId
                });
            }

            var ordered = movements
                .OrderBy(m => m.VoucherDate.Date)
                .ThenBy(m => m.IsReceipt ? 0 : 1)
                .ThenBy(m => m.VoucherDate)
                .ThenBy(m => m.VoucherId)
                .ThenBy(m => m.LineId)
                .ToList();

            // Tính toán FIFO
            var balance = InventoryCostCalculator.CalculateFifo(ordered);

            // Cập nhật kết quả vào Store (để save xuống MaterialPrice)
            var record = GetOrAdd(materialId, warehouseId);
            record.Quantity = (double)balance.Quantity;
            record.Price = (double)balance.Price;
        }

        // =====================================================================
        // Helpers
        // =====================================================================

        private static string Key(string materialId, string warehouseId)
            => materialId.Trim() + "\u001F" + warehouseId.Trim();

        private static void ValidateKey(string materialId, string warehouseId)
        {
            if (string.IsNullOrWhiteSpace(materialId)) throw new ArgumentException("Thiếu mã vật tư.", nameof(materialId));
            if (string.IsNullOrWhiteSpace(warehouseId)) throw new ArgumentException("Thiếu mã kho.", nameof(warehouseId));
        }
    }
}
