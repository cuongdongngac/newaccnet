using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Reports.FlattenedDetails
{
    /// <summary>
    /// Dịch vụ dựng Bảng phẳng cấp chi tiết (Tầng 2 - theo chuẩn FlatTableRules.md).
    /// Cung cấp các hàm nạp và làm phẳng thống nhất cho toàn bộ 7 loại chi tiết:
    ///   1. Công nợ (DebtFlatRow)
    ///   2. Chi phí (CostFlatRow)
    ///   3. Tài sản cố định (AssetFlatRow)
    ///   4. Vật tư / Hàng tồn kho (InventoryFlatRow)
    ///   5. Ngoại tệ (CurrencyFlatRow)
    ///   6. Đầu tư / Chứng khoán (InvestmentFlatRow)
    ///   7. Thuế GTGT (VatFlatRow)
    /// </summary>
    public static class FlattenedDetailService
    {
        // =====================================================================
        // TIỆN ÍCH CHUNG: ĐIỀN THÔNG TIN BÚT TOÁN & CHỨNG TỪ VÀO DÒNG CHI TIẾT
        // =====================================================================

        private static void PopulateBaseFields(
            FlatDetailRowBase row,
            int detailId,
            JournalEntryEntity entry,
            double detailAmount)
        {
            var voucher = entry.JournalVoucher ?? entry.ParentEntry?.JournalVoucher;
            bool isOpening = entry.JournalVoucherId == null || voucher == null;

            row.DetailId = detailId;
            row.JournalEntryId = entry.Id;
            row.JournalVoucherId = isOpening ? (int?)null : voucher.Id;
            row.VoucherNo = isOpening ? "SDDK" : (voucher.VoucherNo?.Trim() ?? string.Empty);
            row.VoucherDate = isOpening ? (DateTime?)null : voucher.VoucherDate;
            row.Contents = isOpening ? "Số dư đầu kỳ" : (voucher.Contents?.Trim() ?? string.Empty);
            row.AccountId = entry.AccountId?.Trim() ?? string.Empty;
            row.CounterAccountId = ResolveCounterAccountId(entry);
            row.Dbcr = entry.Dbcr.GetValueOrDefault(1) == -1 ? -1 : 1;
            row.Amount = detailAmount;
        }

        private static string ResolveCounterAccountId(JournalEntryEntity entry)
        {
            if (entry.ParentEntry != null && !string.IsNullOrWhiteSpace(entry.ParentEntry.AccountId))
            {
                return entry.ParentEntry.AccountId.Trim();
            }

            if (entry.SubEntries != null && entry.SubEntries.Count > 0)
            {
                var firstSub = entry.SubEntries.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.AccountId));
                if (firstSub != null)
                {
                    return firstSub.AccountId.Trim();
                }
            }

            return string.Empty;
        }

        private static bool MatchesFilters(
            FlatDetailRowBase row,
            string? accountPrefix,
            DateTime? fromDate,
            DateTime? toDate,
            bool includeOpeningBeforeFromDate = true)
        {
            if (!string.IsNullOrWhiteSpace(accountPrefix))
            {
                string prefix = accountPrefix.Trim();
                if (!row.AccountId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            if (toDate.HasValue && !row.IsUpToDate(toDate.Value))
            {
                return false;
            }

            if (fromDate.HasValue && !includeOpeningBeforeFromDate)
            {
                if (row.IsOpening || row.VoucherDate!.Value.Date < fromDate.Value.Date)
                    return false;
            }

            return true;
        }

        // =====================================================================
        // 1. CHI TIẾT CÔNG NỢ (DebtDetailEntity -> DebtFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Công nợ (DebtFlatRow) trực tiếp từ CSDL.
        /// Hỗ trợ lọc theo Tài khoản (accountPrefix), khoảng thời gian và kiểm tra đối chiếu bút toán thiếu chi tiết (includeUnmappedEntries).
        /// </summary>
        public static List<DebtFlatRow> LoadDebtFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true,
            bool includeUnmappedEntries = false)
        {
            var prefetch = new PrefetchPath2(EntityType.DebtDetailEntity);
            var pEntry = prefetch.Add(DebtDetailEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);
            prefetch.Add(DebtDetailEntity.PrefetchPathCurrencyLiabilityLine);

            var details = new EntityCollection<DebtDetailEntity>();
            adapter.FetchEntityCollection(details, null, prefetch);

            var rows = details
                .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                .Select(CreateDebtFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();

            if (includeUnmappedEntries)
            {
                AppendUnmappedDebtEntries(adapter, rows, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate);
            }

            return rows;
        }

        /// <summary>
        /// Làm phẳng danh sách JournalEntryEntity đã nạp sẵn trong bộ nhớ thành danh sách DebtFlatRow.
        /// Nếu <paramref name="includeUnmappedEntries"/> = true, các bút toán trên tài khoản công nợ không có dòng DebtDetail
        /// sẽ sinh ra 1 dòng có PartnerId = "" và IsUnmappedEntry = true (theo quy tắc Mục 1.4 của FlatTableRules.md).
        /// </summary>
        public static List<DebtFlatRow> FlattenDebtEntries(
            IEnumerable<JournalEntryEntity> entries,
            bool includeUnmappedEntries = true)
        {
            var result = new List<DebtFlatRow>();
            if (entries == null) return result;

            foreach (var entry in entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.AccountId)) continue;

                if (entry.DebtDetails != null && entry.DebtDetails.Count > 0)
                {
                    foreach (var d in entry.DebtDetails)
                    {
                        result.Add(CreateDebtFlatRow(d));
                    }
                }
                else if (includeUnmappedEntries)
                {
                    var unmapped = new DebtFlatRow
                    {
                        PartnerId = string.Empty,
                        DebtTypeId = string.Empty,
                        IsUnmappedEntry = true
                    };
                    PopulateBaseFields(unmapped, 0, entry, (double)(entry.Amount ?? 0m));
                    result.Add(unmapped);
                }
            }

            return result;
        }

        private static DebtFlatRow CreateDebtFlatRow(DebtDetailEntity d)
        {
            var entry = d.JournalEntry;
            double amount = (double)(d.Amount ?? entry.Amount ?? 0m);
            var row = new DebtFlatRow
            {
                PartnerId = d.PartnerId?.Trim() ?? string.Empty,
                DebtTypeId = d.DebtTypeId?.Trim() ?? string.Empty,
                DebtReasonId = d.DebtReasonId,
                LiabilityDuesId = d.LiabilityDuesId,
                LiabilityDate = d.Liabilitydate,
                Currency = d.Currency,
                InterestFlag = d.Interestflag,
                CurrencyId = d.CurrencyLiabilityLine?.CurrencyId?.Trim() ?? string.Empty,
                ExchangeRate = (double)(d.CurrencyLiabilityLine?.ExchangeRate ?? 0m),
                ForeignQuantity = (double)(d.CurrencyLiabilityLine?.Quantity ?? 0m)
            };

            PopulateBaseFields(row, d.Id, entry, amount);
            return row;
        }

        private static void AppendUnmappedDebtEntries(
            IDataAccessAdapter adapter,
            List<DebtFlatRow> rows,
            string? accountPrefix,
            DateTime? fromDate,
            DateTime? toDate,
            bool includeOpeningBeforeFromDate)
        {
            var mappedEntryIds = rows.Select(r => r.JournalEntryId).ToHashSet();

            var prefetch = new PrefetchPath2(EntityType.JournalEntryEntity);
            prefetch.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            prefetch.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var allEntries = new EntityCollection<JournalEntryEntity>();
            adapter.FetchEntityCollection(allEntries, null, prefetch);

            foreach (var entry in allEntries)
            {
                if ( string.IsNullOrWhiteSpace(entry.AccountId) || mappedEntryIds.Contains(entry.Id))
                    continue;

                if (!string.IsNullOrWhiteSpace(accountPrefix) &&
                    !entry.AccountId.Trim().StartsWith(accountPrefix.Trim(), StringComparison.OrdinalIgnoreCase))
                    continue;

                var unmapped = new DebtFlatRow
                {
                    PartnerId = string.Empty,
                    DebtTypeId = string.Empty,
                    IsUnmappedEntry = true
                };
                PopulateBaseFields(unmapped, 0, entry, (double)(entry.Amount ?? 0m));

                if (MatchesFilters(unmapped, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                {
                    rows.Add(unmapped);
                }
            }
        }

        // =====================================================================
        // 2. CHI TIẾT CHI PHÍ (CostDetailEntity -> CostFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Chi phí (CostFlatRow) trực tiếp từ CSDL.
        /// Tương đương [Union Expense Object Line Query] trong Access.
        /// </summary>
        public static List<CostFlatRow> LoadCostFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true)
        {
            var prefetch = new PrefetchPath2(EntityType.CostDetailEntity);
            var pEntry = prefetch.Add(CostDetailEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var details = new EntityCollection<CostDetailEntity>();
            adapter.FetchEntityCollection(details, null, prefetch);

            return details
                .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                .Select(CreateCostFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();
        }

        private static CostFlatRow CreateCostFlatRow(CostDetailEntity d)
        {
            var entry = d.JournalEntry;
            double amount = (double)(d.Amount ?? entry.Amount ?? 0m);
            var row = new CostFlatRow
            {
                CostObjectId = d.CostObjectId?.Trim() ?? string.Empty,
                CostElementId = d.CostElementId?.Trim() ?? string.Empty,
                SerialNo = d.Serialno?.Trim() ?? string.Empty,
                TransFlag = d.Transflag,
                LiabilityFlag = d.Liabilityflag
            };

            PopulateBaseFields(row, d.Id, entry, amount);
            return row;
        }

        // =====================================================================
        // 3. CHI TIẾT TÀI SẢN CỐ ĐỊNH (AssetDetailEntity -> AssetFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Tài sản cố định (AssetFlatRow) trực tiếp từ CSDL.
        /// Tương đương [Union Fixed Assets Line Query] trong Access.
        /// </summary>
        public static List<AssetFlatRow> LoadAssetFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true)
        {
            var prefetch = new PrefetchPath2(EntityType.AssetDetailEntity);
            var pEntry = prefetch.Add(AssetDetailEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);
            prefetch.Add(AssetDetailEntity.PrefetchPathReason);

            var details = new EntityCollection<AssetDetailEntity>();
            adapter.FetchEntityCollection(details, null, prefetch);

            return details
                .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                .Select(CreateAssetFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();
        }

        private static AssetFlatRow CreateAssetFlatRow(AssetDetailEntity d)
        {
            var entry = d.JournalEntry;
            double qty = (double)(d.Quantity ?? 0m);
            double price = (double)(d.Price ?? 0m);
            double amount = (qty != 0 && price != 0) ? (qty * price) : (price != 0 ? price : (double)(entry.Amount ?? 0m));

            var row = new AssetFlatRow
            {
                AssetId = d.AssetId?.Trim() ?? string.Empty,
                SourceCode = d.Srcode?.Trim() ?? string.Empty,
                ReasonId = d.Reasonid?.Trim() ?? string.Empty,
                ReasonFlag = d.Reason?.Flag == true,
                Quantity = qty,
                Price = price
            };

            PopulateBaseFields(row, d.Id, entry, amount);
            return row;
        }

        // =====================================================================
        // 4. CHI TIẾT VẬT Tư / HÀNG TỒN KHO (InventoryVoucherLineEntity -> InventoryFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Vật tư / Hàng tồn kho (InventoryFlatRow) trực tiếp từ CSDL.
        /// Nối từ InventoryVoucherLineEntity -> InventoryVoucherEntity -> JournalEntryEntity -> JournalVoucherEntity.
        /// </summary>
        public static List<InventoryFlatRow> LoadInventoryFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            string? warehouseId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true)
        {
            var prefetch = new PrefetchPath2(EntityType.InventoryVoucherLineEntity);
            var pInv = prefetch.Add(InventoryVoucherLineEntity.PrefetchPathInventoryVoucher);
            var pEntry = pInv.SubPath.Add(InventoryVoucherEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var lines = new EntityCollection<InventoryVoucherLineEntity>();
            adapter.FetchEntityCollection(lines, null, prefetch);

            var rows = lines
                .Where(l => l.InventoryVoucher?.JournalEntry != null
                         && !string.IsNullOrWhiteSpace(l.InventoryVoucher.JournalEntry.AccountId))
                .Select(CreateInventoryFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();

            if (!string.IsNullOrWhiteSpace(warehouseId))
            {
                string wh = warehouseId.Trim();
                rows = rows.Where(r => string.Equals(r.WarehouseId, wh, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return rows;
        }

        private static InventoryFlatRow CreateInventoryFlatRow(InventoryVoucherLineEntity l)
        {
            var inv = l.InventoryVoucher;
            var entry = inv.JournalEntry;
            double qty = (double)(l.Quantity ?? 0m);
            double price = (double)(l.Price ?? 0m);
            double amount = (qty != 0 && price != 0) ? (qty * price) : (double)(entry.Amount ?? 0m);

            var row = new InventoryFlatRow
            {
                InventoryVoucherId = inv.Id,
                InOutNumber = inv.Inoutnumber?.Trim() ?? string.Empty,
                WarehouseId = inv.WarehouseId?.Trim() ?? string.Empty,
                CostObjectId = inv.ExobjectId?.Trim() ?? string.Empty,
                MaterialId = l.MaterialId?.Trim() ?? string.Empty,
                Quantity = qty,
                Price = price,
                Purpose = l.Purpose,
                Carrier = inv.Carrier?.Trim() ?? string.Empty
            };

            PopulateBaseFields(row, l.Id, entry, amount);
            return row;
        }

        // =====================================================================
        // 5. CHI TIẾT NGOẠI TỆ (CurrencyDetailEntity -> CurrencyFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Ngoại tệ (CurrencyFlatRow) trực tiếp từ CSDL.
        /// </summary>
        public static List<CurrencyFlatRow> LoadCurrencyFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            string? currencyId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true)
        {
            var prefetch = new PrefetchPath2(EntityType.CurrencyDetailEntity);
            var pEntry = prefetch.Add(CurrencyDetailEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var details = new EntityCollection<CurrencyDetailEntity>();
            adapter.FetchEntityCollection(details, null, prefetch);

            var rows = details
                .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                .Select(CreateCurrencyFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();

            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                string curr = currencyId.Trim();
                rows = rows.Where(r => string.Equals(r.CurrencyId, curr, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return rows;
        }

        private static CurrencyFlatRow CreateCurrencyFlatRow(CurrencyDetailEntity d)
        {
            var entry = d.JournalEntry;
            double qty = (double)(d.Quantity ?? 0m);
            double rate = (double)(d.ExchangeRate ?? 0m);
            double amount = (double)(entry.Amount ?? (decimal)(qty * rate));

            var row = new CurrencyFlatRow
            {
                CurrencyId = d.CurrencyId?.Trim() ?? string.Empty,
                Quantity = qty,
                ExchangeRate = rate
            };

            PopulateBaseFields(row, d.Id, entry, amount);
            return row;
        }

        // =====================================================================
        // 6. CHI TIẾT ĐẦU Tư / CHỨNG KHOÁN (InvestmentDetailEntity -> InvestmentFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Đầu tư / Chứng khoán (InvestmentFlatRow) trực tiếp từ CSDL.
        /// </summary>
        public static List<InvestmentFlatRow> LoadInvestmentFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            string? stockId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true)
        {
            var prefetch = new PrefetchPath2(EntityType.InvestmentDetailEntity);
            var pEntry = prefetch.Add(InvestmentDetailEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var details = new EntityCollection<InvestmentDetailEntity>();
            adapter.FetchEntityCollection(details, null, prefetch);

            var rows = details
                .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                .Select(CreateInvestmentFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();

            if (!string.IsNullOrWhiteSpace(stockId))
            {
                string stk = stockId.Trim();
                rows = rows.Where(r => string.Equals(r.StockId, stk, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return rows;
        }

        private static InvestmentFlatRow CreateInvestmentFlatRow(InvestmentDetailEntity d)
        {
            var entry = d.JournalEntry;
            double amount = (double)(entry.Amount ?? d.Originamount ?? 0m);

            var row = new InvestmentFlatRow
            {
                StockId = d.Stockid?.Trim() ?? string.Empty,
                CustomerId = d.CustomerId?.Trim() ?? string.Empty,
                StockStoreId = d.Stockstoreid,
                Quantity = (double)(d.Quantity ?? 0m),
                InPrice = (double)(d.Inprice ?? 0m),
                OutPrice = (double)(d.Outprice ?? 0m),
                OriginAmount = (double)(d.Originamount ?? 0m),
                OtherExpense = (double)(d.Otherexpense ?? 0m)
            };

            PopulateBaseFields(row, d.Lineid, entry, amount);
            return row;
        }

        // =====================================================================
        // 7. CHI TIẾT THUẾ GTGT (VatEntryEntity -> VatFlatRow)
        // =====================================================================

        /// <summary>
        /// Nạp và dựng bảng phẳng Chi tiết Thuế GTGT (VatFlatRow) trực tiếp từ CSDL.
        /// </summary>
        public static List<VatFlatRow> LoadVatFlatRows(
            IDataAccessAdapter adapter,
            string? accountPrefix = null,
            short? vatType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOpeningBeforeFromDate = true)
        {
            var prefetch = new PrefetchPath2(EntityType.VatEntryEntity);
            var pEntry = prefetch.Add(VatEntryEntity.PrefetchPathJournalEntry);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
            pEntry.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);

            var details = new EntityCollection<VatEntryEntity>();
            adapter.FetchEntityCollection(details, null, prefetch);

            var rows = details
                .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                .Select(CreateVatFlatRow)
                .Where(r => MatchesFilters(r, accountPrefix, fromDate, toDate, includeOpeningBeforeFromDate))
                .ToList();

            if (vatType.HasValue)
            {
                rows = rows.Where(r => r.VatType == vatType.Value).ToList();
            }

            return rows;
        }

        private static VatFlatRow CreateVatFlatRow(VatEntryEntity d)
        {
            var entry = d.JournalEntry;
            double amount = (double)(entry.Amount ?? 0m);

            var row = new VatFlatRow
            {
                VatType = d.Type,
                CatId = d.Catid,
                TaxRateId = d.Taxrateid,
                Rate = (double)(d.Rate ?? 0m),
                BeforeTaxAmount = (double)(d.Beforetaxamount ?? 0m),
                VatBillNo = d.Vatbillno?.Trim() ?? string.Empty,
                VatBillDate = d.Vatbilldate,
                SerialNo = d.Serialno?.Trim() ?? string.Empty,
                VatObjectId = d.Vatobjectid?.Trim() ?? string.Empty,
                VatObjectName = d.Vatobjectname?.Trim() ?? string.Empty,
                TaxNo = d.Taxno?.Trim() ?? string.Empty,
                VatDescription = d.Vatdescription?.Trim() ?? string.Empty,
                Remark = d.Remark?.Trim() ?? string.Empty
            };

            PopulateBaseFields(row, d.Lineid, entry, amount);
            return row;
        }
    }
}
