using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NewaccNet.Core.Posting
{
    /// <summary>
    /// Tùy chọn thuế suất dùng chung cho các màn hình và dịch vụ hạch toán.
    /// </summary>
    public class TaxRateOption
    {
        public int Taxrateid { get; set; }
        public double Rate { get; set; }
        public string Display => (Rate > 1) ? $"{Rate:G29}%" : $"{Rate * 100:G29}%";
    }

    /// <summary>
    /// Tùy chọn tài khoản dùng chung cho LookUp.
    /// </summary>
    public class AccountOption
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Display => $"{AccountId} - {AccountName}";
    }

    /// <summary>
    /// Dòng chi tiết vật tư, hàng hóa dùng chung cho tầng Core, ERP, AI và Hóa đơn điện tử.
    /// Có sẵn INotifyPropertyChanged để bind mượt mà lên bất kỳ DataGrid nào.
    /// </summary>
    public class WarehouseVoucherLineItem : INotifyPropertyChanged
    {
        private string _materialId = string.Empty;
        private string _materialName = string.Empty;
        private string _unit = string.Empty;
        private double _quantity = 1;
        private double _price = 0;
        private double _amount = 0;

        // Bán hàng & Thuế
        private double _sellPrice = 0;
        private double _sellAmount = 0;
        private int? _taxRateId = null;
        private double _taxRate = 0;
        private double _taxAmount = 0;

        private string _note = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public string MaterialId
        {
            get => _materialId;
            set
            {
                if (_materialId != value)
                {
                    _materialId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MaterialName
        {
            get => _materialName;
            set
            {
                if (_materialName != value)
                {
                    _materialName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Quantity
        {
            get => _quantity;
            set
            {
                if (Math.Abs(_quantity - value) > 0.0001)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    Recalculate();
                }
            }
        }

        public double Price
        {
            get => _price;
            set
            {
                if (Math.Abs(_price - value) > 0.0001)
                {
                    _price = value;
                    OnPropertyChanged();
                    Recalculate();
                }
            }
        }

        public double Amount
        {
            get => _amount;
            set
            {
                if (Math.Abs(_amount - value) > 0.0001)
                {
                    _amount = value;
                    OnPropertyChanged();
                }
            }
        }

        public double SellPrice
        {
            get => _sellPrice;
            set
            {
                if (Math.Abs(_sellPrice - value) > 0.0001)
                {
                    _sellPrice = value;
                    OnPropertyChanged();
                    Recalculate();
                }
            }
        }

        public double SellAmount
        {
            get => _sellAmount;
            set
            {
                if (Math.Abs(_sellAmount - value) > 0.0001)
                {
                    _sellAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? Taxrateid
        {
            get => _taxRateId;
            set
            {
                if (_taxRateId != value)
                {
                    _taxRateId = value;
                    OnPropertyChanged();
                    if (_taxRateId.HasValue)
                    {
                        TaxRate = _taxRateId.Value switch
                        {
                            1 => 0.0,
                            2 => 0.05,
                            3 => 0.10,
                            4 => 0.08,
                            _ => 0.0
                        };
                    }
                }
            }
        }

        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (Math.Abs(_taxRate - value) > 0.0001)
                {
                    _taxRate = value;
                    OnPropertyChanged();
                    Recalculate();
                }
            }
        }

        public double TaxAmount
        {
            get => _taxAmount;
            set
            {
                if (Math.Abs(_taxAmount - value) > 0.0001)
                {
                    _taxAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Note
        {
            get => _note;
            set
            {
                if (_note != value)
                {
                    _note = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Recalculate(bool isPurchase = false)
        {
            Amount = Math.Round(Quantity * Price, 0);
            SellAmount = Math.Round(Quantity * SellPrice, 0);

            if (isPurchase)
            {
                TaxAmount = Math.Round(Amount * TaxRate, 0);
            }
            else
            {
                TaxAmount = Math.Round(SellAmount * TaxRate, 0);
            }
        }
    }

    /// <summary>
    /// Tham số đầu vào chuẩn cho cổng ghi sổ chứng từ kho NewaccNet.Core.
    /// Dùng chung cho cả Form ERP, AI Agent, Import Excel, và Hóa đơn điện tử.
    /// </summary>
    public class WarehouseVoucherSaveParams
    {
        public int VoucherId { get; set; } = 0; // 0 = Tạo mới, > 0 = Cập nhật phiếu cũ
        public string VoucherType { get; set; } = "NM"; // NM, XB, NNB, XNB, CK
        public string VoucherNo { get; set; } = string.Empty;
        public DateTime VoucherDate { get; set; } = DateTime.Today;
        public string Description { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;

        // Kho
        public string WarehouseId { get; set; } = string.Empty;
        public string FromWarehouseId { get; set; } = string.Empty;
        public string ToWarehouseId { get; set; } = string.Empty;

        // Đối tác & Hóa đơn VAT (NM / XB)
        public string PartnerId { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public string VatBillNo { get; set; } = string.Empty;
        public string SerialNo { get; set; } = string.Empty;
        public DateTime? VatBillDate { get; set; }

        // Tài khoản đối ứng chân form
        public string CostAccountId { get; set; } = "6321";
        public string InventoryAccountId { get; set; } = "1561";
        public string RevenueAccountId { get; set; } = "5111";
        public string TaxAccountId { get; set; } = "13311";
        public string TotalAccountId { get; set; } = "3311";
        public string OffsetAccountId { get; set; } = "1541"; // Cho NNB hoặc XNB

        // Dòng chi tiết
        public List<WarehouseVoucherLineItem> Lines { get; set; } = new();
    }

    /// <summary>
    /// Cấu trúc đồ thị lưu vết liên kết giữa InventoryVoucher và các dòng hạch toán JournalEntries.
    /// Lưu vào cột EntryGraph (JSON) trên bảng InventoryVoucher để truy vấn tức thì, tường minh 100%.
    /// </summary>
    public class VoucherEntryGraphDto
    {
        public int? CostEntryId { get; set; }      // Dòng Giá vốn (632)
        public int? MaterialEntryId { get; set; }  // Dòng Kho mang InventoryVoucher (152/155/156)
        public int? RevMasterEntryId { get; set; } // Dòng Phải thu bán hàng (131/111/112)
        public int? RevEntryId { get; set; }        // Dòng Doanh thu (511)
        public int? TaxEntryId { get; set; }        // Dòng Thuế GTGT (133/3331)
        public int? TotalEntryId { get; set; }      // Dòng Phải trả/Tiền mua hàng (331/111/112)
        public int? OffsetEntryId { get; set; }     // Dòng Đối ứng nội bộ (154/621/642/711)
        public string? Note { get; set; }
    }

    /// <summary>
    /// Thông tin tiến độ chuyển đổi đồ thị kho.
    /// </summary>
    public class MigrationProgressInfo
    {
        public int Current { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public string CurrentMessage { get; set; } = string.Empty;
        public double Percent => Total > 0 ? Math.Min(100.0, Math.Round((double)Current / Total * 100, 1)) : 0;
    }
}
