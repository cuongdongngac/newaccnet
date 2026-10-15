using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewaccNet.Wpf.AppSystem.CostTransfer
{
    /// <summary>
    /// Nghiệp vụ kết chuyển chi phí. Quy tắc dựng dữ liệu: .ai_context/FlatTableRules.md
    /// </summary>
    public interface ICostTransferService
    {
        /// <summary>
        /// Tính số dư chi phí của tài khoản nguồn đến "Đến ngày", gom theo (đối tượng, yếu tố).
        /// Chỉ trả về các dòng có số dư khác 0.
        /// </summary>
        Task<List<CostTransferRow>> CalculateAsync(CostTransferParams param);
    }
}
