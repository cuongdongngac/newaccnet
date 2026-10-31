using System;
using System.Collections.Generic;

namespace NewaccNet.Plugin.ErpWarehouse.Models
{
    /// <summary>
    /// 5 loại nghiệp vụ phiếu kho cốt lõi của cổng ERP.
    /// </summary>
    public enum ErpInventoryVoucherType
    {
        /// <summary>Điều chuyển kho nội bộ (Nợ 152/156 Kho nhận - Có 152/156 Kho xuất)</summary>
        Transfer = 0,

        /// <summary>Nhập mua hàng / vật tư (Nợ 152/156/153 - Có 331/111...)</summary>
        PurchaseReceipt = 1,

        /// <summary>Xuất bán hàng hóa (Nợ 632 - Có 156)</summary>
        SalesDelivery = 2,

        /// <summary>Xuất dùng nội bộ / sản xuất / chi phí (Nợ 621/627/642 - Có 152/153)</summary>
        ProductionOrExpenseIssue = 3,

        /// <summary>Nhập/Xuất kho khác hoặc hàng trả lại</summary>
        OtherMovement = 4
    }

    /// <summary>
    /// Dòng chi tiết vật tư / hàng hóa trong phiếu kho ERP.
    /// </summary>
    public class ErpInventoryItemDto
    {
        public int LineNo { get; set; }
        public string MaterialId { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Amount => Math.Round(Quantity * UnitPrice, 0);

        /// <summary>Tài khoản Nợ (VD: 152, 156, 632, 642...)</summary>
        public string DebitAccount { get; set; } = "156";

        /// <summary>Tài khoản Có (VD: 156, 152, 331, 111...)</summary>
        public string CreditAccount { get; set; } = "156";

        public string Note { get; set; } = string.Empty;
    }

    /// <summary>
    /// Chứng từ kho hoàn chỉnh nhận từ ERP hoặc AI để chuẩn bị nạp vào CSDL hoặc mở giao diện.
    /// </summary>
    public class ErpInventoryVoucherDto
    {
        public string VoucherNo { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; } = DateTime.Today;
        public ErpInventoryVoucherType VoucherType { get; set; } = ErpInventoryVoucherType.Transfer;

        /// <summary>Mã kho xuất (Bắt buộc với Chuyển kho, Xuất bán, Xuất dùng)</summary>
        public string FromWarehouseId { get; set; } = string.Empty;

        /// <summary>Mã kho nhập (Bắt buộc với Chuyển kho, Nhập mua)</summary>
        public string ToWarehouseId { get; set; } = string.Empty;

        /// <summary>Diễn giải nghiệp vụ</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Người giao / Người vận chuyển</summary>
        public string DelivererName { get; set; } = string.Empty;

        /// <summary>Người nhận hàng</summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>Mã đối tác (NCC hoặc Khách hàng nếu là Nhập mua / Xuất bán)</summary>
        public string PartnerId { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>Mã tham chiếu từ hệ thống ERP bên ngoài</summary>
        public string ExternalRefNo { get; set; } = string.Empty;

        public List<ErpInventoryItemDto> Items { get; set; } = new();

        public double TotalQuantity
        {
            get
            {
                double sum = 0;
                foreach (var i in Items) sum += i.Quantity;
                return sum;
            }
        }

        public double TotalAmount
        {
            get
            {
                double sum = 0;
                foreach (var i in Items) sum += i.Amount;
                return sum;
            }
        }
    }

    /// <summary>
    /// Kết quả xử lý nạp chứng từ ERP chạy ngầm hoặc qua giao diện.
    /// </summary>
    public class ErpExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? VoucherId { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public List<string> ValidationErrors { get; set; } = new();

        public static ErpExecutionResult Success(int voucherId, string voucherNo, string message = "Thành công") =>
            new() { IsSuccess = true, VoucherId = voucherId, VoucherNo = voucherNo, Message = message };

        public static ErpExecutionResult Failed(IEnumerable<string> errors) =>
            new() { IsSuccess = false, ValidationErrors = new List<string>(errors), Message = string.Join("; ", errors) };

        public static ErpExecutionResult Failed(string error) =>
            new() { IsSuccess = false, ValidationErrors = new() { error }, Message = error };
    }
}
