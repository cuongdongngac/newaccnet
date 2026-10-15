using System;
using System.Threading.Tasks;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.BalanceTransfer
{
    public interface IBalanceTransferService
    {
        /// <summary>
        /// Kiểm tra kết nối tới Cơ sở dữ liệu đích trước khi thực hiện chuyển đổi.
        /// </summary>
        Task<bool> TestDestinationConnectionAsync(AppConfig destinationConfig);

        /// <summary>
        /// Bước tiên quyết: Chuyển toàn bộ các bảng Thư viện / Danh mục (Master Data) sang CSDL đích.
        /// Riêng SystemUser: Update cho tài khoản 'admin', Insert cho các tài khoản người dùng khác.
        /// </summary>
        Task TransferDictionariesAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// Bước chuyển các bảng Thiết kế báo cáo &amp; Công thức tính (CĐKT, KQKD, LCTT, Nghĩa vụ thuế, Tờ khai VAT, Thẻ giá thành...).
        /// </summary>
        Task TransferDesignTablesAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 1. Chuyển số dư tài khoản thông thường không theo dõi chi tiết (Category 'I' - Tiền mặt, Tiền gửi, Vốn...).
        /// </summary>
        Task TransferNoDetailsAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 2. Chuyển số dư công nợ chi tiết (Category 'A' - 131, 331, 141, 138, 338... -> DebtDetail).
        /// </summary>
        Task TransferDebtAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 3. Chuyển số dư vật tư, hàng hóa kho (Category 'B' - 152, 153, 155, 156... -> Tự động sinh Phiếu Nhập Kho Nội Bộ NNB ngày 01/01).
        /// </summary>
        Task TransferMaterialAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 4. Chuyển số dư chi phí dở dang (Category 'E' - 154, 621, 622, 627... -> CostDetail).
        /// </summary>
        Task TransferCostAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 5. Chuyển số dư tài sản cố định (Category 'D' - 211, 214... -> AssetDetail).
        /// </summary>
        Task TransferAssetAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 6. Chuyển số dư cổ phiếu, đầu tư chứng khoán (Category 'S' - 121, 221, 228... -> InvestmentDetail).
        /// </summary>
        Task TransferStockAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// 7. Chuyển số dư ngoại tệ (Category 'C' - CurrencyDetail).
        /// </summary>
        Task TransferCurrencyAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);

        /// <summary>
        /// Điều phối toàn bộ quá trình chuyển số dư hàng loạt sang niên độ mới.
        /// </summary>
        Task ExecuteTransferAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress);
    }
}
