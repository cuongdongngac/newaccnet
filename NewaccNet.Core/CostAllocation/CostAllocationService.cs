using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;

namespace NewaccNet.Core.CostAllocation
{
    /// <summary>
    /// Nhạc trưởng (Orchestrator) điều phối 4 mảnh ghép của nghiệp vụ Phân bổ Chi phí:
    ///   1. CostAllocationSnapshotLoader (Dựng bảng phẳng Tầng 2 & Lọc Chi phí nguồn + Tiêu thức)
    ///   2. CostAllocationCalculator (Tính tỷ trọng bình quân gia quyền & Làm tròn đồng lẻ)
    ///   3. CostAllocationVoucherBuilder (Sinh chứng từ kế toán phân bổ Nợ TK Đích / Có TK Nguồn)
    /// Xem chi tiết tại: .ai_context/CostAllocationArchitecture.md
    /// </summary>
    public class CostAllocationService : ICostAllocationService
    {
        private const double Epsilon = 0.0001;

        private readonly CostAllocationSnapshotLoader _snapshotLoader = new CostAllocationSnapshotLoader();
        private readonly CostAllocationCalculator _calculator = new CostAllocationCalculator();
        private readonly CostAllocationVoucherBuilder _voucherBuilder = new CostAllocationVoucherBuilder();

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

                if (param.FromDate.Date > param.ToDate.Date)
                {
                    result.HasError = true;
                    result.ErrorMessage = "Khoảng thời gian không hợp lệ (Từ ngày phải nhỏ hơn hoặc bằng Đến ngày).";
                    return result;
                }

                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        // 1. Nạp danh mục Đối tượng chi phí & Yếu tố chi phí để hiển thị tên
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

                        // 2. Dựng bảng phẳng chi phí Tầng 2 (CostFlatRow)
                        var flatRows = _snapshotLoader.LoadCostFlatRows(adapter);

                        // 3. MẢNH GHÉP 1: Tập hợp Chi phí nguồn cần phân bổ (Lưới trên - Tử số chung)
                        result.SourcePoolItems = _snapshotLoader.LoadSourcePool(
                            flatRows,
                            param.SourceAccountId,
                            param.ToDate,
                            costObjDict,
                            costElemDict);

                        result.TotalPoolAmount = result.SourcePoolItems.Sum(x => x.Amount);

                        if (Math.Abs(result.TotalPoolAmount) < Epsilon)
                        {
                            result.HasError = true;
                            result.ErrorMessage = $"Tổng chi phí nguồn bằng 0 (đã kết chuyển/phân bổ hết hoặc không có chi phí còn lại tại tài khoản {param.SourceAccountId} đến ngày {param.ToDate:dd/MM/yyyy}).";
                            return result;
                        }

                        // 4. MẢNH GHÉP 2: Tập hợp Tiêu thức phân bổ (Lưới dưới - Mẫu số)
                        result.CriteriaItems = _snapshotLoader.LoadCriteriaBase(
                            flatRows,
                            param.AllocationBaseAccountIds,
                            param.FromDate,
                            param.ToDate,
                            costObjDict);

                        result.TotalBaseAmount = result.CriteriaItems.Sum(x => x.BaseAmount);

                        if (result.TotalBaseAmount <= Epsilon || result.CriteriaItems.Count == 0)
                        {
                            result.HasError = true;
                            result.ErrorMessage = "Tổng tiêu thức phân bổ bằng 0 (các đối tượng chi phí không có phát sinh chi phí gốc tại các tài khoản tiêu thức trong kỳ). Vui lòng chọn lại danh sách tài khoản tiêu thức hoặc mở rộng khoảng thời gian.";
                            return result;
                        }

                        // 5. MẢNH GHÉP 3: Tính toán Tỷ trọng (%) và Số tiền phân bổ
                        result.TotalAllocatedAmount = _calculator.ApplyWeightedAllocation(
                            result.CriteriaItems,
                            result.TotalPoolAmount,
                            result.TotalBaseAmount);
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

        public async Task<JournalVoucherEntity> ExecuteAllocationAsync(
            CostAllocationParams param,
            CostAllocationCalculationResult calculation)
        {
            return await Task.Run(() =>
            {
                if (param == null || calculation == null)
                {
                    throw new ArgumentException("Tham số hoặc dữ liệu tính toán phân bổ không hợp lệ.");
                }

                if (calculation.HasError || calculation.CriteriaItems.Count == 0
                    || Math.Abs(calculation.TotalPoolAmount) < Epsilon
                    || Math.Abs(calculation.TotalBaseAmount) < Epsilon)
                {
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(calculation.ErrorMessage)
                        ? "Dòng tổng chi phí nguồn hoặc tổng tiêu thức bằng 0. Không có chi phí để phân bổ."
                        : calculation.ErrorMessage);
                }

                // MẢNH GHÉP 4: Sinh chứng từ kế toán phân bổ
                return _voucherBuilder.CreateAllocationVoucher(param, calculation);
            });
        }
    }
}

