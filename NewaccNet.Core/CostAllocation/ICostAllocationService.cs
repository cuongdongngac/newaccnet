using System.Threading.Tasks;
using DataAccess.EntityClasses;

namespace NewaccNet.Core.CostAllocation
{
    /// <summary>
    /// Interface nghiệp vụ Phân bổ / Kết chuyển Chi phí kế toán
    /// </summary>
    public interface ICostAllocationService
    {
        /// <summary>
        /// Tính toán xem trước chi phí nguồn và tiêu thức phân bổ theo tỷ lệ bình quân gia quyền
        /// </summary>
        Task<CostAllocationCalculationResult> CalculateAllocationAsync(CostAllocationParams param);

        /// <summary>
        /// Sinh chứng từ kế toán phân bổ tự động vào hệ thống (Transactional)
        /// </summary>
        Task<JournalVoucherEntity> ExecuteAllocationAsync(CostAllocationParams param, CostAllocationCalculationResult calculation);
    }
}

