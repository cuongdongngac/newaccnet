using System;

namespace NewaccNet.Wpf.AppSystem.BalanceTransfer
{
    /// <summary>
    /// DTO số dư tài khoản thông thường không chi tiết (Category 'I')
    /// </summary>
    public class NormalBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public short Dbcr { get; set; } = 1; // 1: Nợ, -1: Có
    }

    /// <summary>
    /// DTO số dư chi tiết công nợ (Category 'A')
    /// </summary>
    public class DebtBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string PartnerId { get; set; } = string.Empty;
        public int? DebtReasonId { get; set; }
        public string DebtTypeId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public short Dbcr { get; set; } = 1;
    }

    /// <summary>
    /// DTO số dư vật tư hàng hóa tồn kho (Category 'B') -> Sinh phiếu NNB
    /// </summary>
    public class MaterialBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string WarehouseId { get; set; } = string.Empty;
        public string MaterialId { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }

    /// <summary>
    /// DTO số dư chi phí dở dang (Category 'E')
    /// </summary>
    public class CostBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string CostObjectId { get; set; } = string.Empty;
        public string CostElementId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public short Dbcr { get; set; } = 1;
    }

    /// <summary>
    /// DTO số dư tài sản cố định (Category 'D')
    /// </summary>
    public class AssetBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string AssetId { get; set; } = string.Empty;
        public string SrCode { get; set; } = string.Empty;
        public string ReasonId { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }

    /// <summary>
    /// DTO số dư cổ phiếu, đầu tư chứng khoán (Category 'S')
    /// </summary>
    public class StockBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string StockId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
    }

    /// <summary>
    /// DTO số dư ngoại tệ (Category 'C')
    /// </summary>
    public class CurrencyBalanceDto
    {
        public string AccountId { get; set; } = string.Empty;
        public string CurrencyId { get; set; } = string.Empty;
        public double Quantity { get; set; } // Số tiền nguyên tệ
        public double Amount { get; set; } // Số tiền quy đổi VND
        public double ExchangeRate { get; set; } // Tỷ giá ghi sổ bình quân = Amount / Quantity
        public short Dbcr { get; set; } = 1;
    }
}
