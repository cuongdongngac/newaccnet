using System;

namespace NewaccNet.Reports.FlattenedDetails
{
    /// <summary>
    /// Lớp cơ sở chung cho mọi dòng dữ liệu phẳng cấp chi tiết (Tầng 2 - theo chuẩn FlatTableRules.md).
    /// Mọi dòng chi tiết (Công nợ, Chi phí, TSCĐ, Vật tư, Ngoại tệ, Đầu tư, Thuế GTGT) đều kế thừa lớp này
    /// để có sẵn thông tin Chứng từ (Ngày, Số phiếu, Nội dung) và Bút toán (Tài khoản, TK đối ứng, Nợ/Có, Số tiền).
    /// </summary>
    public abstract class FlatDetailRowBase
    {
        /// <summary>
        /// Khóa chính của dòng bảng chi tiết (Id hoặc Lineid). Bằng 0 nếu là dòng bút toán thiếu chi tiết (unmapped).
        /// </summary>
        public int DetailId { get; set; }

        /// <summary>
        /// Khóa nối về bút toán gốc (JournalEntry.Id).
        /// </summary>
        public int JournalEntryId { get; set; }

        /// <summary>
        /// Khóa nối về chứng từ gốc (JournalVoucher.Id). Bằng null nếu là số dư đầu kỳ.
        /// </summary>
        public int? JournalVoucherId { get; set; }

        /// <summary>
        /// Số chứng từ (JournalVoucher.VoucherNo). Nếu là số dư đầu kỳ thì trả về "SDDK".
        /// </summary>
        public string VoucherNo { get; set; } = string.Empty;

        /// <summary>
        /// Ngày chứng từ (JournalVoucher.VoucherDate). Bằng null nếu là bút toán số dư đầu kỳ (JournalVoucherId == null).
        /// </summary>
        public DateTime? VoucherDate { get; set; }

        /// <summary>
        /// Nội dung / Diễn giải chứng từ (JournalVoucher.Contents).
        /// </summary>
        public string Contents { get; set; } = string.Empty;

        /// <summary>
        /// Tài khoản hạch toán của bút toán (JournalEntry.AccountId).
        /// </summary>
        public string AccountId { get; set; } = string.Empty;

        /// <summary>
        /// Tài khoản đối ứng (nếu có từ ParentEntry / SubEntries).
        /// </summary>
        public string CounterAccountId { get; set; } = string.Empty;

        /// <summary>
        /// Chiều định khoản: +1 = Ghi Nợ (Debit), -1 = Ghi Có (Credit).
        /// </summary>
        public int Dbcr { get; set; } = 1;

        /// <summary>
        /// Số tiền của dòng chi tiết (VNĐ).
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Số tiền phát sinh bên Nợ (Dbcr == +1 ? Amount : 0).
        /// </summary>
        public double DebitAmount => Dbcr == 1 ? Amount : 0.0;

        /// <summary>
        /// Số tiền phát sinh bên Có (Dbcr == -1 ? Amount : 0).
        /// </summary>
        public double CreditAmount => Dbcr == -1 ? Amount : 0.0;

        /// <summary>
        /// Số tiền có dấu theo chiều Nợ/Có (Dbcr * Amount). Dùng để cộng gộp số dư thuần Sum(SignedAmount).
        /// </summary>
        public double SignedAmount => Dbcr * Amount;

        /// <summary>
        /// Cờ đánh dấu bút toán số dư đầu kỳ gốc (không gắn với JournalVoucher).
        /// </summary>
        public bool IsOpening => JournalVoucherId == null || VoucherDate == null;

        /// <summary>
        /// Cờ đánh dấu bút toán tài khoản chi tiết nhưng bị thiếu dòng chi tiết (dùng cho chức năng Kiểm tra đối chiếu).
        /// </summary>
        public bool IsUnmappedEntry { get; set; }

        /// <summary>
        /// Kiểm tra dòng này có thuộc Số dư đầu kỳ tính tại mốc <paramref name="beginDate"/> hay không
        /// (bao gồm số dư đầu kỳ gốc hoặc chứng từ phát sinh trước ngày <paramref name="beginDate"/>).
        /// </summary>
        public bool IsOpeningAt(DateTime beginDate)
        {
            return IsOpening || VoucherDate!.Value.Date < beginDate.Date;
        }

        /// <summary>
        /// Kiểm tra dòng này có phát sinh trong khoảng thời gian [<paramref name="beginDate"/> .. <paramref name="endDate"/>] hay không.
        /// </summary>
        public bool IsInPeriod(DateTime beginDate, DateTime endDate)
        {
            if (IsOpening) return false;
            DateTime d = VoucherDate!.Value.Date;
            return d >= beginDate.Date && d <= endDate.Date;
        }

        /// <summary>
        /// Kiểm tra dòng này có hiệu lực tính đến ngày <paramref name="endDate"/> hay không (bao gồm cả đầu kỳ và phát sinh <= endDate).
        /// </summary>
        public bool IsUpToDate(DateTime endDate)
        {
            return IsOpening || VoucherDate!.Value.Date <= endDate.Date;
        }
    }

    /// <summary>
    /// 1. Bảng phẳng Chi tiết Công nợ (Tầng 2 - nối từ DebtDetailEntity).
    /// Phục vụ: Báo cáo công nợ chi tiết / tổng hợp (lọc theo Tài khoản, Đối tượng PartnerId, Nội dung DebtTypeId, Lý do, Hạn nợ),
    /// Kiểm tra công nợ, và Tính lãi công nợ.
    /// </summary>
    public class DebtFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Mã đối tượng công nợ / Khách hàng / Nhà cung cấp (DebtDetail.PartnerId).
        /// </summary>
        public string PartnerId { get; set; } = string.Empty;

        /// <summary>
        /// Mã nội dung công nợ (DebtDetail.DebtTypeId).
        /// </summary>
        public string DebtTypeId { get; set; } = string.Empty;

        /// <summary>
        /// Mã lý do công nợ (DebtDetail.DebtReasonId).
        /// </summary>
        public int? DebtReasonId { get; set; }

        /// <summary>
        /// Mã hạn nợ / Khế ước (DebtDetail.LiabilityDuesId).
        /// </summary>
        public int? LiabilityDuesId { get; set; }

        /// <summary>
        /// Ngày hạn thanh toán (DebtDetail.Liabilitydate).
        /// </summary>
        public DateTime? LiabilityDate { get; set; }

        /// <summary>
        /// Có theo dõi ngoại tệ (DebtDetail.Currency).
        /// </summary>
        public bool Currency { get; set; }

        /// <summary>
        /// Có tính lãi vay / lãi quá hạn (DebtDetail.Interestflag).
        /// </summary>
        public bool InterestFlag { get; set; }

        /// <summary>
        /// Mã ngoại tệ (nếu có liên kết CurrencyLiabilityLine).
        /// </summary>
        public string CurrencyId { get; set; } = string.Empty;

        /// <summary>
        /// Tỷ giá ngoại tệ (nếu có liên kết CurrencyLiabilityLine).
        /// </summary>
        public double ExchangeRate { get; set; }

        /// <summary>
        /// Số lượng nguyên tệ (nếu có liên kết CurrencyLiabilityLine).
        /// </summary>
        public double ForeignQuantity { get; set; }
    }

    /// <summary>
    /// 2. Bảng phẳng Chi tiết Chi phí (Tầng 2 - nối từ CostDetailEntity).
    /// Tương đương [Union Expense Object Line Query] trong Access.
    /// Phục vụ: Báo cáo chi phí theo Đối tượng (CostObjectId) & Yếu tố (CostElementId),
    /// Kết chuyển chi phí (CostTransfer), và Phân bổ chi phí (CostAllocation).
    /// </summary>
    public class CostFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Mã đối tượng tập hợp chi phí - Công trình / Sản phẩm / Phân xưởng (CostDetail.CostObjectId).
        /// </summary>
        public string CostObjectId { get; set; } = string.Empty;

        /// <summary>
        /// Mã yếu tố chi phí - Nguyên vật liệu, Nhân công, Máy, Chi phí chung... (CostDetail.CostElementId).
        /// </summary>
        public string CostElementId { get; set; } = string.Empty;

        /// <summary>
        /// Số hóa đơn / Sê-ri chứng từ chi phí (CostDetail.Serialno).
        /// </summary>
        public string SerialNo { get; set; } = string.Empty;

        /// <summary>
        /// Cờ đánh dấu bút toán tự động kết chuyển/phân bổ (CostDetail.Transflag).
        /// </summary>
        public bool TransFlag { get; set; }

        /// <summary>
        /// Cờ theo dõi công nợ đi kèm chi phí (CostDetail.Liabilityflag).
        /// </summary>
        public bool LiabilityFlag { get; set; }
    }

    /// <summary>
    /// 3. Bảng phẳng Chi tiết Tài sản cố định (Tầng 2 - nối từ AssetDetailEntity).
    /// Tương đương [Union Fixed Assets Line Query] trong Access.
    /// Phục vụ: Báo cáo tăng/giảm TSCĐ, Sổ tài sản cố định, và Trích khấu hao TSCĐ (AssetDepreciation).
    /// </summary>
    public class AssetFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Mã tài sản cố định (AssetDetail.AssetId).
        /// </summary>
        public string AssetId { get; set; } = string.Empty;

        /// <summary>
        /// Mã nguồn vốn hình thành tài sản (AssetDetail.Srcode).
        /// </summary>
        public string SourceCode { get; set; } = string.Empty;

        /// <summary>
        /// Mã lý do tăng / giảm / khấu hao TSCĐ (AssetDetail.Reasonid).
        /// </summary>
        public string ReasonId { get; set; } = string.Empty;

        /// <summary>
        /// Cờ tính chất lý do (Reason.Flag: True = Tăng/Ghi nhận nguyên giá, False = Khấu hao/Hao mòn).
        /// </summary>
        public bool ReasonFlag { get; set; }

        /// <summary>
        /// Số lượng tài sản (AssetDetail.Quantity).
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Đơn giá / Nguyên giá trên dòng chi tiết (AssetDetail.Price).
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Số lượng có dấu theo chiều Nợ/Có (Dbcr * Quantity).
        /// </summary>
        public double SignedQuantity => Dbcr * Quantity;
    }

    /// <summary>
    /// 4. Bảng phẳng Chi tiết Vật tư / Hàng tồn kho (Tầng 2 - nối từ InventoryVoucherLineEntity -> InventoryVoucherEntity -> JournalEntryEntity).
    /// Phục vụ: Báo cáo Nhập - Xuất - Tồn kho, Thẻ kho, Sổ chi tiết vật tư (lọc theo Kho WarehouseId, Vật tư MaterialId, Đối tượng chi phí CostObjectId).
    /// </summary>
    public class InventoryFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Khóa chính phiếu kho (InventoryVoucher.Id).
        /// </summary>
        public int InventoryVoucherId { get; set; }

        /// <summary>
        /// Số phiếu nhập / xuất kho (InventoryVoucher.Inoutnumber).
        /// </summary>
        public string InOutNumber { get; set; } = string.Empty;

        /// <summary>
        /// Mã kho hàng (InventoryVoucher.WarehouseId).
        /// </summary>
        public string WarehouseId { get; set; } = string.Empty;

        /// <summary>
        /// Mã đối tượng tập hợp chi phí gắn trên phiếu xuất kho (InventoryVoucher.ExobjectId).
        /// </summary>
        public string CostObjectId { get; set; } = string.Empty;

        /// <summary>
        /// Mã vật tư / hàng hóa (InventoryVoucherLine.MaterialId).
        /// </summary>
        public string MaterialId { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng nhập / xuất (InventoryVoucherLine.Quantity).
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Đơn giá nhập / xuất (InventoryVoucherLine.Price).
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Mục đích nhập / xuất (InventoryVoucherLine.Purpose).
        /// </summary>
        public short? Purpose { get; set; }

        /// <summary>
        /// Người giao / nhận hàng (InventoryVoucher.Carrier).
        /// </summary>
        public string Carrier { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng có dấu theo chiều Nhập (+1) / Xuất (-1): Dbcr * Quantity.
        /// </summary>
        public double SignedQuantity => Dbcr * Quantity;
    }

    /// <summary>
    /// 5. Bảng phẳng Chi tiết Ngoại tệ (Tầng 2 - nối từ CurrencyDetailEntity).
    /// Phục vụ: Sổ chi tiết ngoại tệ, Báo cáo tồn quỹ ngoại tệ, Đánh giá chênh lệch tỷ giá.
    /// </summary>
    public class CurrencyFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Mã ngoại tệ: USD, EUR, JPY... (CurrencyDetail.CurrencyId).
        /// </summary>
        public string CurrencyId { get; set; } = string.Empty;

        /// <summary>
        /// Số lượng nguyên tệ (CurrencyDetail.Quantity).
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Tỷ giá hạch toán (CurrencyDetail.ExchangeRate).
        /// </summary>
        public double ExchangeRate { get; set; }

        /// <summary>
        /// Số lượng nguyên tệ có dấu theo chiều Nợ/Có (Dbcr * Quantity).
        /// </summary>
        public double SignedQuantity => Dbcr * Quantity;
    }

    /// <summary>
    /// 6. Bảng phẳng Chi tiết Đầu tư / Chứng khoán (Tầng 2 - nối từ InvestmentDetailEntity).
    /// Phục vụ: Báo cáo danh mục đầu tư, Sổ chi tiết chứng khoán (lọc theo Mã chứng khoán StockId, Đối tượng CustomerId).
    /// </summary>
    public class InvestmentFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Mã chứng khoán / Khoản đầu tư (InvestmentDetail.Stockid).
        /// </summary>
        public string StockId { get; set; } = string.Empty;

        /// <summary>
        /// Mã đối tượng / Công ty chứng khoán / Bên nhận đầu tư (InvestmentDetail.CustomerId).
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Nơi lưu ký chứng khoán (InvestmentDetail.Stockstoreid).
        /// </summary>
        public short? StockStoreId { get; set; }

        /// <summary>
        /// Số lượng cổ phiếu / chứng khoán (InvestmentDetail.Quantity).
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Đơn giá mua (InvestmentDetail.Inprice).
        /// </summary>
        public double InPrice { get; set; }

        /// <summary>
        /// Đơn giá bán (InvestmentDetail.Outprice).
        /// </summary>
        public double OutPrice { get; set; }

        /// <summary>
        /// Giá trị gốc / Giá vốn (InvestmentDetail.Originamount).
        /// </summary>
        public double OriginAmount { get; set; }

        /// <summary>
        /// Chi phí giao dịch khác (InvestmentDetail.Otherexpense).
        /// </summary>
        public double OtherExpense { get; set; }

        /// <summary>
        /// Số lượng có dấu theo chiều Nợ/Có (Dbcr * Quantity).
        /// </summary>
        public double SignedQuantity => Dbcr * Quantity;
    }

    /// <summary>
    /// 7. Bảng phẳng Chi tiết Thuế GTGT (Tầng 2 - nối từ VatEntryEntity).
    /// Phục vụ: Bảng kê hóa đơn, chứng từ hàng hóa dịch vụ mua vào / bán ra, Tờ khai thuế GTGT.
    /// </summary>
    public class VatFlatRow : FlatDetailRowBase
    {
        /// <summary>
        /// Loại thuế: Đầu vào / Đầu ra (VatEntry.Type).
        /// </summary>
        public short? VatType { get; set; }

        /// <summary>
        /// Nhóm hàng hóa dịch vụ (VatEntry.Catid).
        /// </summary>
        public short? CatId { get; set; }

        /// <summary>
        /// Mã mức thuế suất (VatEntry.Taxrateid).
        /// </summary>
        public short? TaxRateId { get; set; }

        /// <summary>
        /// Tỷ lệ thuế suất % (VatEntry.Rate).
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Doanh số chưa có thuế GTGT (VatEntry.Beforetaxamount).
        /// </summary>
        public double BeforeTaxAmount { get; set; }

        /// <summary>
        /// Số hóa đơn GTGT (VatEntry.Vatbillno).
        /// </summary>
        public string VatBillNo { get; set; } = string.Empty;

        /// <summary>
        /// Ngày hóa đơn GTGT (VatEntry.Vatbilldate).
        /// </summary>
        public DateTime? VatBillDate { get; set; }

        /// <summary>
        /// Ký hiệu hóa đơn / Sê-ri (VatEntry.Serialno).
        /// </summary>
        public string SerialNo { get; set; } = string.Empty;

        /// <summary>
        /// Mã đối tượng khách hàng / nhà cung cấp trên hóa đơn (VatEntry.Vatobjectid).
        /// </summary>
        public string VatObjectId { get; set; } = string.Empty;

        /// <summary>
        /// Tên đối tượng trên hóa đơn (VatEntry.Vatobjectname).
        /// </summary>
        public string VatObjectName { get; set; } = string.Empty;

        /// <summary>
        /// Mã số thuế đối tượng (VatEntry.Taxno).
        /// </summary>
        public string TaxNo { get; set; } = string.Empty;

        /// <summary>
        /// Mặt hàng / Diễn giải trên hóa đơn (VatEntry.Vatdescription).
        /// </summary>
        public string VatDescription { get; set; } = string.Empty;

        /// <summary>
        /// Ghi chú (VatEntry.Remark).
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }
}
