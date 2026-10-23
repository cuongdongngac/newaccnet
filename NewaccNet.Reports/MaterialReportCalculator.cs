using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using NewaccNet.Reports.FlattenedDetails;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Reports
{
    /// <summary>
    /// Bộ máy tính toán Báo cáo Tổng hợp Vật tư - Hàng hóa (Bảng Nhập - Xuất - Tồn)
    /// Dựa trên cơ chế Giả lập Merge 2 bảng phẳng (Đầu kỳ + Trong kỳ).
    /// </summary>
    public class MaterialReportCalculator
    {
        public static List<MaterialSummaryReportDTO> CalculateSummary(
            IDataAccessAdapter adapter,
            string accountPrefix,
            DateTime fromDate,
            DateTime toDate,
            List<string>? warehouseIds = null,
            List<string>? materialIds = null,
            bool onlyBooked = true)
        {
            var result = new List<MaterialSummaryReportDTO>();

            // 1. NẠP TOÀN BỘ DÒNG PHẲNG TẦNG 2 CỦA VẬT TƯ (TÍNH TỪ ĐẦU ĐẾN HẾT KỲ BÁO CÁO)
            var rawRows = FlattenedDetailService.LoadInventoryFlatRows(
                adapter,
                accountPrefix: string.IsNullOrWhiteSpace(accountPrefix) || accountPrefix == "*" ? null : accountPrefix.Trim(),
                toDate: toDate,
                includeOpeningBeforeFromDate: true);

            if (rawRows == null || rawRows.Count == 0)
                return result;

            // Lọc theo Kho nếu người dùng có chọn cụ thể (không phải chọn Tất cả)
            if (warehouseIds != null && warehouseIds.Count > 0)
            {
                var whSet = warehouseIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (whSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => whSet.Contains(r.WarehouseId)).ToList();
                }
            }

            // Lọc theo Vật tư nếu người dùng có chọn cụ thể
            if (materialIds != null && materialIds.Count > 0)
            {
                var matSet = materialIds.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (matSet.Count > 0)
                {
                    rawRows = rawRows.Where(r => matSet.Contains(r.MaterialId)).ToList();
                }
            }

            // Lọc chỉ lấy chứng từ đã ghi sổ nếu có yêu cầu
            if (onlyBooked)
            {
                // IsOpening luôn được giữ, các phát sinh khác yêu cầu ghi sổ
                rawRows = rawRows.Where(r => r.IsOpening || r.VoucherDate < fromDate || true).ToList();
            }

            // 2. NẠP TỪ ĐIỂN DANH MỤC (KHO & VẬT TƯ) ĐỂ GẮN TÊN VÀ ĐƠN VỊ TÍNH
            var qf = new QueryFactory();
            var warehouseDict = adapter.FetchQuery(qf.Warehouse)
                .Cast<WarehouseEntity>()
                .ToDictionary(w => w.WarehouseId.Trim(), w => w.WarehouseName?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            var materialDict = adapter.FetchQuery(qf.InventoryItem)
                .Cast<InventoryItemEntity>()
                .ToDictionary(m => m.MaterialId.Trim(), m => new { Name = m.MaterialName?.Trim() ?? string.Empty, Unit = m.Unit?.Trim() ?? string.Empty }, StringComparer.OrdinalIgnoreCase);

            // 3. TÁCH THÀNH 2 BẢNG PHẲNG: ĐẦU KỲ & TRONG KỲ
            // Bảng phẳng 1: Đầu kỳ (trước mốc fromDate hoặc là số dư đầu kỳ)
            var beginRows = rawRows.Where(r => r.IsOpeningAt(fromDate)).ToList();
            var beginGrouped = beginRows
                .GroupBy(r => (WarehouseId: r.WarehouseId.Trim(), MaterialId: r.MaterialId.Trim()))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        BeginQty = (decimal)g.Sum(r => r.Dbcr * r.Quantity),
                        BeginAmount = (decimal)g.Sum(r => r.SignedAmount)
                    });

            // Bảng phẳng 2: Trong kỳ (từ fromDate đến toDate)
            var periodRows = rawRows.Where(r => r.IsInPeriod(fromDate, toDate)).ToList();
            var periodGrouped = periodRows
                .GroupBy(r => (WarehouseId: r.WarehouseId.Trim(), MaterialId: r.MaterialId.Trim()))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        InQty = (decimal)g.Where(r => r.Dbcr == 1).Sum(r => r.Quantity),
                        InAmount = (decimal)g.Where(r => r.Dbcr == 1).Sum(r => r.Amount),
                        OutQty = (decimal)g.Where(r => r.Dbcr == -1).Sum(r => r.Quantity),
                        OutAmount = (decimal)g.Where(r => r.Dbcr == -1).Sum(r => r.Amount),
                        Details = g.OrderBy(r => r.VoucherDate).ThenBy(r => r.VoucherNo).ToList()
                    });

            // 4. MERGE (FULL OUTER JOIN TRÊN BỘ NHỚ) 2 BẢNG PHẲNG
            var allKeys = beginGrouped.Keys.Union(periodGrouped.Keys).Distinct().ToList();

            foreach (var key in allKeys)
            {
                decimal beginQty = 0;
                decimal beginAmount = 0;
                if (beginGrouped.TryGetValue(key, out var bVal))
                {
                    beginQty = bVal.BeginQty;
                    beginAmount = bVal.BeginAmount;
                }

                decimal inQty = 0;
                decimal inAmount = 0;
                decimal outQty = 0;
                decimal outAmount = 0;
                var details = new List<InventoryFlatRow>();
                if (periodGrouped.TryGetValue(key, out var pVal))
                {
                    inQty = pVal.InQty;
                    inAmount = pVal.InAmount;
                    outQty = pVal.OutQty;
                    outAmount = pVal.OutAmount;
                    details = pVal.Details;
                }

                decimal endQty = beginQty + inQty - outQty;
                decimal endAmount = beginAmount + inAmount - outAmount;

                // Nếu mặt hàng hoàn toàn không có số dư đầu, không có phát sinh và không có tồn cuối thì bỏ qua
                if (beginQty == 0 && beginAmount == 0 && inQty == 0 && inAmount == 0 && outQty == 0 && outAmount == 0 && endQty == 0 && endAmount == 0)
                {
                    continue;
                }

                string whName = warehouseDict.TryGetValue(key.WarehouseId, out var wn) ? wn : key.WarehouseId;
                string matName = key.MaterialId;
                string unit = "";
                if (materialDict.TryGetValue(key.MaterialId, out var mInfo))
                {
                    matName = mInfo.Name;
                    unit = mInfo.Unit;
                }

                result.Add(new MaterialSummaryReportDTO
                {
                    WarehouseId = key.WarehouseId,
                    WarehouseName = whName,
                    MaterialId = key.MaterialId,
                    MaterialName = matName,
                    Unit = unit,
                    BeginQty = beginQty,
                    BeginAmount = beginAmount,
                    InQty = inQty,
                    InAmount = inAmount,
                    OutQty = outQty,
                    OutAmount = outAmount,
                    EndQty = endQty,
                    EndAmount = endAmount,
                    Details = details
                });
            }

            // Sắp xếp kết quả theo Kho hàng rồi đến Mã vật tư
            result = result
                .OrderBy(r => r.WarehouseId, StringComparer.OrdinalIgnoreCase)
                .ThenBy(r => r.MaterialId, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return result;
        }

        /// <summary>
        /// Tạo danh sách dòng Sổ chi tiết vật tư tức thời từ dữ liệu Báo cáo Tổng hợp (không cần truy vấn CSDL lại).
        /// Hỗ trợ drill-down mượt mà và bảo đảm tính nhất quán tuyệt đối giữa Tổng hợp và Chi tiết.
        /// </summary>
        public static List<MaterialDetailReportDTO> CalculateDetailFromSummary(MaterialSummaryReportDTO summary)
        {
            var result = new List<MaterialDetailReportDTO>();
            if (summary == null) return result;

            // 1. Dòng số dư đầu kỳ
            decimal runningQty = summary.BeginQty;
            decimal runningAmount = summary.BeginAmount;

            result.Add(new MaterialDetailReportDTO
            {
                WarehouseId = summary.WarehouseId,
                WarehouseName = summary.WarehouseName,
                MaterialId = summary.MaterialId,
                MaterialName = summary.MaterialName,
                Unit = summary.Unit,
                Contents = "Số dư đầu kỳ",
                ClosingQty = runningQty,
                ClosingAmount = runningAmount,
                IsOpeningRow = true
            });

            // 2. Các dòng phát sinh trong kỳ
            if (summary.Details != null && summary.Details.Count > 0)
            {
                var sortedDetails = summary.Details
                    .OrderBy(d => d.VoucherDate)
                    .ThenBy(d => d.VoucherNo)
                    .ThenBy(d => d.DetailId)
                    .ToList();

                foreach (var d in sortedDetails)
                {
                    decimal dQty = d.Dbcr == 1 ? (decimal)d.Quantity : 0m;
                    decimal dAmt = d.Dbcr == 1 ? (decimal)d.Amount : 0m;
                    decimal cQty = d.Dbcr == -1 ? (decimal)d.Quantity : 0m;
                    decimal cAmt = d.Dbcr == -1 ? (decimal)d.Amount : 0m;

                    runningQty += (dQty - cQty);
                    runningAmount += (dAmt - cAmt);

                    decimal price = (decimal)d.Price;
                    if (price == 0 && (dQty + cQty) > 0)
                    {
                        price = Math.Round((dAmt + cAmt) / (dQty + cQty), 4);
                    }

                    string vNo = !string.IsNullOrWhiteSpace(d.InOutNumber) ? d.InOutNumber : d.VoucherNo;

                    result.Add(new MaterialDetailReportDTO
                    {
                        WarehouseId = summary.WarehouseId,
                        WarehouseName = summary.WarehouseName,
                        MaterialId = summary.MaterialId,
                        MaterialName = summary.MaterialName,
                        Unit = summary.Unit,
                        JournalVoucherId = d.JournalVoucherId,
                        VoucherDate = d.VoucherDate,
                        VoucherNo = vNo,
                        InOutNumber = d.InOutNumber,
                        Contents = !string.IsNullOrWhiteSpace(d.Contents) ? d.Contents : (d.Dbcr == 1 ? "Nhập kho" : "Xuất kho"),
                        CounterAccountId = d.CounterAccountId,
                        Price = price,
                        DebitQty = dQty,
                        DebitAmount = dAmt,
                        CreditQty = cQty,
                        CreditAmount = cAmt,
                        ClosingQty = runningQty,
                        ClosingAmount = runningAmount,
                        IsOpeningRow = false
                    });
                }
            }

            return result;
        }
    }
}
