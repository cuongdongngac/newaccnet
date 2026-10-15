using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.CostAllocation
{
    /// <summary>
    /// Triển khai dịch vụ Phân bổ / Kết chuyển Chi phí theo tỷ lệ bình quân gia quyền
    /// </summary>
    public class CostAllocationService : ICostAllocationService
    {
        public async Task<CostAllocationCalculationResult> CalculateAllocationAsync(CostAllocationParams param)
        {
            return await Task.Run(() =>
            {
                var result = new CostAllocationCalculationResult();

                if (param == null)
                {
                    result.HasError = true;
                    result.ErrorMessage = "Tham số phân bổ không hợp lệ (null).";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(param.SourceAccountId))
                {
                    result.HasError = true;
                    result.ErrorMessage = "Vui lòng chọn Tài khoản nguồn (có).";
                    return result;
                }

                if (string.IsNullOrWhiteSpace(param.DestinationAccountId))
                {
                    result.HasError = true;
                    result.ErrorMessage = "Vui lòng chọn Tài khoản đích (nợ).";
                    return result;
                }

                if (param.AllocationBaseAccountIds == null || param.AllocationBaseAccountIds.Count == 0)
                {
                    result.HasError = true;
                    result.ErrorMessage = "Vui lòng chọn ít nhất một Tài khoản làm tiêu thức phân bổ.";
                    return result;
                }

                if (param.FromDate > param.ToDate)
                {
                    result.HasError = true;
                    result.ErrorMessage = "Khoảng thời gian không hợp lệ (Từ ngày phải nhỏ hơn hoặc bằng Đến ngày).";
                    return result;
                }

                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        // 1. Tải danh mục tra cứu Tên Đối tượng & Yếu tố chi phí
                        var costObjects = new EntityCollection<CostObjectEntity>();
                        adapter.FetchEntityCollection(costObjects, null);
                        var costObjDict = costObjects
                            .Where(o => !string.IsNullOrWhiteSpace(o.Id))
                            .ToDictionary(o => o.Id.Trim(), o => o.ObjectName ?? o.Id, StringComparer.OrdinalIgnoreCase);

                        var costElements = new EntityCollection<CostElementEntity>();
                        adapter.FetchEntityCollection(costElements, null);
                        var costElemDict = costElements
                            .Where(e => !string.IsNullOrWhiteSpace(e.Id))
                            .ToDictionary(e => e.Id.Trim(), e => e.ElementName ?? e.Id, StringComparer.OrdinalIgnoreCase);

                        // 2. Tải toàn bộ CostDetail trong kỳ phân bổ kèm JournalEntry và JournalVoucher
                        var prefetch = new PrefetchPath2(EntityType.CostDetailEntity);
                        var pJe = prefetch.Add(CostDetailEntity.PrefetchPathJournalEntry);
                        pJe.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);

                        var allCostDetails = new EntityCollection<CostDetailEntity>();
                        adapter.FetchEntityCollection(allCostDetails, null, prefetch);

                        DateTime start = param.FromDate.Date;
                        DateTime end = param.ToDate.Date.AddDays(1).AddSeconds(-1);

                        // LỌC GIAO DỊCH GỐC (Transflag == false) TRONG KỲ [FromDate, ToDate]
                        var validDetails = allCostDetails
                            .Where(d => d.Transflag == false)
                            .Where(d => d.JournalEntry != null)
                            .Where(d =>
                            {
                                DateTime entryDate = d.JournalEntry.JournalVoucher?.VoucherDate 
                                                  ?? d.JournalEntry.Createdate 
                                                  ?? DateTime.MinValue;
                                return entryDate >= start && entryDate <= end;
                            })
                            .ToList();

                        // 3. BƯỚC 1: TÍNH TỔNG CHI PHÍ NGUỒN CẦN PHÂN BỔ (TỬ SỐ CHUNG - TotalPoolAmount)
                        string srcAcc = param.SourceAccountId.Trim();
                        var sourceLines = validDetails
                            .Where(d => !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId) 
                                     && d.JournalEntry.AccountId.Trim().StartsWith(srcAcc, StringComparison.OrdinalIgnoreCase))
                            .ToList();

                        var poolGrouped = sourceLines
                            .GroupBy(d => new
                            {
                                CostObjectId = d.CostObjectId?.Trim() ?? string.Empty,
                                CostElementId = d.CostElementId?.Trim() ?? string.Empty
                            })
                            .Select(g =>
                            {
                                double netAmt = g.Sum(x => (x.Amount ?? 0.0) * (x.JournalEntry.Dbcr.GetValueOrDefault(1) == -1 ? -1 : 1));
                                string objId = g.Key.CostObjectId;
                                string objName = string.IsNullOrEmpty(objId) 
                                    ? "(Chi phí dùng chung)" 
                                    : (costObjDict.TryGetValue(objId, out var oName) ? oName : objId);

                                return new CostSourceItemDto
                                {
                                    CostObjectId = objId,
                                    CostObjectName = objName,
                                    CostElementId = g.Key.CostElementId,
                                    CostElementName = costElemDict.TryGetValue(g.Key.CostElementId, out var eName) ? eName : g.Key.CostElementId,
                                    Amount = netAmt
                                };
                            })
                            .Where(x => Math.Abs(x.Amount) > 0.0001)
                            .OrderBy(x => x.CostObjectId)
                            .ThenBy(x => x.CostElementId)
                            .ToList();

                        result.SourcePoolItems = poolGrouped;
                        result.TotalPoolAmount = poolGrouped.Sum(x => x.Amount);

                        if (Math.Abs(result.TotalPoolAmount) < 0.0001)
                        {
                            result.HasError = true;
                            result.ErrorMessage = $"Tổng chi phí nguồn bằng 0 (đã kết chuyển/phân bổ hết hoặc không có chi phí phát sinh tại tài khoản {param.SourceAccountId}). Không còn chi phí để phân bổ.";
                            return result;
                        }

                        // 4. BƯỚC 2: TẬP HỢP TIÊU THỨC PHÂN BỔ (MẪU SỐ - AllocationBase)
                        var baseAccSet = param.AllocationBaseAccountIds
                            .Where(a => !string.IsNullOrWhiteSpace(a))
                            .Select(a => a.Trim())
                            .ToList();

                        var criteriaLines = validDetails
                            .Where(d => !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId)
                                     && baseAccSet.Any(crit => d.JournalEntry.AccountId.Trim().StartsWith(crit, StringComparison.OrdinalIgnoreCase)))
                            .Where(d => !string.IsNullOrWhiteSpace(d.CostObjectId))
                            .ToList();

                        var criteriaGrouped = criteriaLines
                            .GroupBy(d => d.CostObjectId.Trim())
                            .Select(g =>
                            {
                                string objId = g.Key;
                                double baseAmt = g.Sum(x => (x.Amount ?? 0.0) * (x.JournalEntry.Dbcr.GetValueOrDefault(1) == -1 ? -1 : 1));
                                return new CostCriteriaItemDto
                                {
                                    CostObjectId = objId,
                                    CostObjectName = costObjDict.TryGetValue(objId, out var oName) ? oName : objId,
                                    BaseAmount = baseAmt,
                                    RatioPercent = 0,
                                    AllocatedAmount = 0
                                };
                            })
                            .Where(x => x.BaseAmount > 0.0001)
                            .OrderBy(x => x.CostObjectId)
                            .ToList();

                        result.CriteriaItems = criteriaGrouped;
                        result.TotalBaseAmount = criteriaGrouped.Sum(x => x.BaseAmount);

                        // BẪY LỖI DIVIDE BY ZERO: Mẫu số = 0
                        if (result.TotalBaseAmount <= 0.0001 || result.CriteriaItems.Count == 0)
                        {
                            result.HasError = true;
                            result.ErrorMessage = "Tổng tiêu thức phân bổ bằng 0 (các đối tượng chi phí không có phát sinh chi phí tiêu thức trong kỳ). Vui lòng chọn lại danh sách tài khoản tiêu thức hoặc mở rộng khoảng thời gian.";
                            return result;
                        }

                        // 5. BƯỚC 3: TÍNH TOÁN TỶ LỆ (%) VÀ SỐ TIỀN PHÂN BỔ
                        // Công thức: Tỷ trọng % = Base / TotalBase; Số tiền = Tỷ trọng % * TotalPool
                        double pool = result.TotalPoolAmount;
                        double totalBase = result.TotalBaseAmount;
                        double runningAllocated = 0;

                        for (int i = 0; i < result.CriteriaItems.Count; i++)
                        {
                            var item = result.CriteriaItems[i];
                            item.RatioPercent = item.BaseAmount / totalBase;
                            item.AllocatedAmount = Math.Round(item.RatioPercent * pool, 0);
                            runningAllocated += item.AllocatedAmount;
                        }

                        // XỬ LÝ LÀM TRÒN CHÊNH LỆCH ĐỒNG LẺ (Rounding Adjustment):
                        // Đảm bảo tổng số tiền phân bổ vừa khít 100% bằng TotalPoolAmount
                        double diff = pool - runningAllocated;
                        if (Math.Abs(diff) > 0.0001 && result.CriteriaItems.Count > 0)
                        {
                            // Điều chỉnh vào đối tượng có giá trị phân bổ lớn nhất
                            var largestItem = result.CriteriaItems.OrderByDescending(x => x.AllocatedAmount).First();
                            largestItem.AllocatedAmount += diff;
                        }

                        result.TotalAllocatedAmount = result.CriteriaItems.Sum(x => x.AllocatedAmount);
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = "Lỗi khi tính toán phân bổ: " + ex.Message;
                }

                return result;
            });
        }

        public async Task<JournalVoucherEntity> ExecuteAllocationAsync(CostAllocationParams param, CostAllocationCalculationResult calculation)
        {
            return await Task.Run(() =>
            {
                if (param == null || calculation == null)
                {
                    throw new ArgumentException("Tham số hoặc dữ liệu tính toán phân bổ không hợp lệ.");
                }

                if (calculation.HasError || calculation.CriteriaItems.Count == 0 
                    || Math.Abs(calculation.TotalPoolAmount) < 0.0001 
                    || Math.Abs(calculation.TotalBaseAmount) < 0.0001)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(calculation.ErrorMessage) 
                        ? "Dòng tổng chi phí nguồn hoặc tổng tiêu thức bằng 0. Không có chi phí để phân bổ." 
                        : calculation.ErrorMessage);
                }

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // 1. TẠO CHỨNG TỪ PHÂN BỔ (JournalVoucher)
                    var voucher = new JournalVoucherEntity();
                    voucher.VoucherDate = param.ToDate;
                    voucher.VoucherNo = GenerateNextAllocationVoucherNo(adapter, param.ToDate);
                    voucher.Contents = $"Phân bổ chi phí từ TK {param.SourceAccountId.Trim()} sang TK {param.DestinationAccountId.Trim()}";
                    voucher.Bookflag = true;

                    // 2. BÚT TOÁN GHI CÓ (GIẢM TRỪ TÀI KHOẢN NGUỒN)
                    var creditEntry = new JournalEntryEntity();
                    creditEntry.AccountId = param.SourceAccountId.Trim();
                    creditEntry.Dbcr = -1; // Có
                    creditEntry.Amount = calculation.TotalPoolAmount;
                    creditEntry.Createdate = DateTime.Now;

                    foreach (var srcItem in calculation.SourcePoolItems)
                    {
                        var cd = new CostDetailEntity
                        {
                            CostObjectId = srcItem.CostObjectId,
                            CostElementId = srcItem.CostElementId,
                            Amount = srcItem.Amount,
                            Transflag = true // ĐÁNH DẤU BÚT TOÁN TỰ ĐỘNG PHÂN BỔ
                        };
                        creditEntry.CostDetails.Add(cd);
                    }
                    voucher.JournalEntries.Add(creditEntry);

                    // 3. BÚT TOÁN GHI NỢ (TẬP HỢP TÀI KHOẢN ĐÍCH THEO TỪNG ĐỐI TƯỢNG)
                    var debitEntry = new JournalEntryEntity();
                    debitEntry.AccountId = param.DestinationAccountId.Trim();
                    debitEntry.Dbcr = 1; // Nợ
                    debitEntry.Amount = calculation.TotalPoolAmount;
                    debitEntry.ParentEntry = creditEntry;
                    debitEntry.Createdate = DateTime.Now;

                    string desCostElem = param.DestinationCostElementId?.Trim() ?? string.Empty;
                    foreach (var critItem in calculation.CriteriaItems)
                    {
                        if (Math.Abs(critItem.AllocatedAmount) < 0.0001) continue;

                        var cd = new CostDetailEntity
                        {
                            CostObjectId = critItem.CostObjectId,
                            CostElementId = desCostElem,
                            Amount = critItem.AllocatedAmount,
                            Transflag = true // ĐÁNH DẤU BÚT TOÁN TỰ ĐỘNG PHÂN BỔ
                        };
                        debitEntry.CostDetails.Add(cd);
                    }
                    voucher.JournalEntries.Add(debitEntry);

                    // 4. LƯU TOÀN BỘ TRONG TRANSACTION THỐNG NHẤT
                    adapter.SaveEntity(voucher, true, recurse: true);

                    return voucher;
                }
            });
        }

        private string GenerateNextAllocationVoucherNo(IDataAccessAdapter adapter, DateTime voucherDate)
        {
            try
            {
                string prefix = "PB" + voucherDate.ToString("yyMM");
                var bucket = new RelationPredicateBucket(JournalVoucherFields.VoucherNo % (prefix + "%"));
                var existingVouchers = new EntityCollection<JournalVoucherEntity>();
                adapter.FetchEntityCollection(existingVouchers, bucket);

                int maxNum = 0;
                foreach (var v in existingVouchers)
                {
                    string no = v.VoucherNo ?? "";
                    if (no.StartsWith(prefix) && no.Length > prefix.Length)
                    {
                        string suffix = no.Substring(prefix.Length);
                        if (int.TryParse(suffix, out int n) && n > maxNum)
                        {
                            maxNum = n;
                        }
                    }
                }
                return $"{prefix}{(maxNum + 1):D3}";
            }
            catch
            {
                return $"PB{voucherDate:yyMMdd}01";
            }
        }
    }
}
