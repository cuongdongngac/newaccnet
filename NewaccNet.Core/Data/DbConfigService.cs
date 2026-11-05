using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NewaccNet.Core.Data
{
    public class ConfigOptionItem
    {
        public short Value { get; set; }
        public string Name { get; set; } = "";

        public ConfigOptionItem(short value, string name)
        {
            Value = value;
            Name = name;
        }

        public override string ToString() => Name;
    }

    public static class DbConfigConstants
    {
        // 1. Hình thức ghi sổ kế toán (AccountMethod)
        public static readonly ConfigOptionItem[] AccountMethods = new[]
        {
            new ConfigOptionItem(1, "1 - Nhật ký chung (Khuyên dùng)"),
            new ConfigOptionItem(2, "2 - Chứng từ ghi sổ"),
            new ConfigOptionItem(3, "3 - Nhật ký - Sổ cái"),
            new ConfigOptionItem(4, "4 - Nhật ký chứng từ")
        };

        // 2. Phương pháp tính giá xuất kho vật tư (Material)
        public static readonly ConfigOptionItem[] MaterialMethods = new[]
        {
            new ConfigOptionItem(1, "1 - Bình quân gia quyền"),
            new ConfigOptionItem(2, "2 - Nhập trước, xuất trước (FIFO)"),
            new ConfigOptionItem(3, "3 - Giá thực tế đích danh")
        };
    }

    public class DbConfigDto
    {
        public int Id { get; set; } = 1;
        public string CompanyName { get; set; } = "";
        public string DepartmentName { get; set; } = "Phòng Kế Toán";
        public string Address { get; set; } = "";
        public string BankAccount { get; set; } = "";
        public string TaxCode { get; set; } = "";
        public int AccountingYear { get; set; } = DateTime.Today.Year;
        public string RegistrationCode { get; set; } = "";
        public bool IsCompanyLicense { get; set; } = false; // False = Theo máy; True = Theo thông tin đơn vị (MST)
        public string ExpiryDate { get; set; } = "20991231"; // YYYYMMDD (20991231 = Dài hạn / Vĩnh viễn)
        public short AccountMethod { get; set; } = 1; // 1 = Nhật ký chung, 2 = CT ghi sổ, ...
        public short Material { get; set; } = 1; // 1 = Bình quân, 2 = FIFO, 3 = Đích danh
        public int MaxRecords { get; set; } = 100; // Hạn mức bản ghi JournalVoucher (Mặc định dùng thử = 100)
        public string VoucherPostingConfig { get; set; } = ""; // Chuỗi JSON cấu hình định khoản ngầm định cho các loại phiếu
        public string CashPostingConfig { get; set; } = ""; // Chuỗi JSON cấu hình định khoản cho phân hệ Tiền mặt & Ngân hàng
    }

    public class VoucherPostingConfigDto
    {
        public Dictionary<string, InventoryVoucherConfigItemDto> Inventory { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, CashVoucherConfigItemDto> Receipt { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, CashVoucherConfigItemDto> Payment { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        // Danh sách thiết lập tài khoản đặc thù cho phân hệ kho (ERP Warehouse) kèm tên định nghĩa lại (CustomName)
        public List<PluginAccountConfigItemDto> ErpAccounts { get; set; } = new();
    }

    /// <summary>
    /// Phần tử cấu hình tài khoản đặc thù theo từng plugin (VD: ERP Warehouse).
    /// Giữ nguyên AccountId làm gốc kế toán, cho phép định nghĩa lại Tên hiển thị (CustomName)
    /// và phân nhóm nghiệp vụ (Category).
    /// </summary>
    public class PluginAccountConfigItemDto
    {
        public string AccountId { get; set; } = "";
        public string BaseName { get; set; } = "";
        public string CustomName { get; set; } = "";
        public string Category { get; set; } = ""; // "Revenue", "Cost", "Inventory", "Tax", "Debt", "Payment", "Internal"
        public string BaseCategoryId { get; set; } = ""; // Category gốc trong COA (A, B, E, V, I...) để đối chiếu
        public bool IsActive { get; set; } = true;

        public string DisplayName => !string.IsNullOrWhiteSpace(CustomName) ? CustomName : (!string.IsNullOrWhiteSpace(BaseName) ? BaseName : AccountId);
        public string FullDisplay => !string.IsNullOrWhiteSpace(CustomName) ? $"{CustomName} ({AccountId})" : $"{AccountId} - {BaseName}";
    }

    /// <summary>
    /// Tùy chọn tài khoản trong danh sách được phép của một mục định khoản cụ thể (Slot-based).
    /// Cấu trúc chuẩn tương tự thực thể Account đầy đủ (AccountId, AccountName, CategoryId, ParentId, IsDefault).
    /// </summary>
    public class VoucherPostingAccountOptionDto
    {
        public string AccountId { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string CategoryId { get; set; } = "";
        public string? ParentId { get; set; } = "";
        public bool IsDefault { get; set; } = false;
        public bool AllowDetail { get; set; } = true; // Cho phép bật form chi tiết (nếu = false: cấm mở form chi tiết)

        // Thuộc tính tương thích ngược
        [System.Text.Json.Serialization.JsonIgnore]
        public string BaseName { get => AccountName; set { if (string.IsNullOrEmpty(AccountName)) AccountName = value; } }
        [System.Text.Json.Serialization.JsonIgnore]
        public string CustomName { get => AccountName; set => AccountName = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string BaseCategoryId { get => CategoryId; set => CategoryId = value; }

        public string DisplayName => !string.IsNullOrWhiteSpace(AccountName) ? AccountName : AccountId;
        public string FullDisplay => !string.IsNullOrWhiteSpace(AccountName) ? $"{AccountId} - {AccountName}" : AccountId;
    }

    public class PaymentMethodOptionDto
    {
        public string Code { get; set; } = "";        // "Debt", "Cash", "Bank", "Advance", ...
        public string Name { get; set; } = "";        // "Chưa thanh toán (Công nợ)", "Tiền mặt", ...
        public string AccountId { get; set; } = "";   // "3311", "1111", "1121", "141", ...
        public bool IsDebt { get; set; } = false;     // True nếu theo dõi công nợ đối tác
    }

    public class InventoryVoucherConfigItemDto
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public short Dbcr { get; set; } = 1;
        public bool Taxflag { get; set; } = false;
        public string TotalAccountId { get; set; } = "";
        public string TaxAccountId { get; set; } = "";
        public string CostAccountId { get; set; } = "";
        public string RevenueAccountId { get; set; } = "";
        public string PartnerType { get; set; } = "";

        // Danh sách tài khoản được phép cấu hình theo từng mục định khoản (Slot-based)
        // Key: "Revenue", "Cost", "Tax", "Total", "Inventory", "Offset"...
        public Dictionary<string, List<VoucherPostingAccountOptionDto>> AccountSlots { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public List<NewaccNet.Core.Posting.AccountOption> GetAccountsForSlot(string slotKey, string? fallbackPrefix = null)
        {
            if (AccountSlots != null && AccountSlots.TryGetValue(slotKey, out var list) && list != null && list.Count > 0)
            {
                return list.Select(x => new NewaccNet.Core.Posting.AccountOption
                {
                    AccountId = x.AccountId,
                    AccountName = !string.IsNullOrWhiteSpace(x.AccountName) ? x.AccountName : x.AccountId,
                    CustomName = !string.IsNullOrWhiteSpace(x.AccountName) ? x.AccountName : x.AccountId
                }).ToList();
            }

            return NewaccNet.Core.Posting.WarehousePostingGateway.LoadAccounts(fallbackPrefix);
        }

        public string GetDefaultAccountIdForSlot(string slotKey, string fallbackAccId)
        {
            if (AccountSlots != null && AccountSlots.TryGetValue(slotKey, out var list) && list != null && list.Count > 0)
            {
                var def = list.FirstOrDefault(x => x.IsDefault);
                if (def != null && !string.IsNullOrEmpty(def.AccountId)) return def.AccountId;
                return list[0].AccountId;
            }
            return fallbackAccId;
        }

        // Danh sách các tình huống thanh toán được cấu hình động
        public List<PaymentMethodOptionDto> PaymentMethods { get; set; } = new();

        public List<PaymentMethodOptionDto> GetEffectivePaymentMethods()
        {
            if (PaymentMethods != null && PaymentMethods.Count > 0)
                return PaymentMethods;

            if (Code == "NM")
            {
                return new List<PaymentMethodOptionDto>
                {
                    new PaymentMethodOptionDto { Code = "Debt", Name = "Chưa thanh toán (Công nợ)", AccountId = !string.IsNullOrEmpty(TotalAccountId) ? TotalAccountId : "3311", IsDebt = true },
                    new PaymentMethodOptionDto { Code = "Cash", Name = "Tiền mặt", AccountId = "1111", IsDebt = false },
                    new PaymentMethodOptionDto { Code = "Bank", Name = "Chuyển khoản", AccountId = "1121", IsDebt = false },
                    new PaymentMethodOptionDto { Code = "Advance", Name = "Tạm ứng nhân viên", AccountId = "141", IsDebt = false }
                };
            }
            else
            {
                return new List<PaymentMethodOptionDto>
                {
                    new PaymentMethodOptionDto { Code = "Debt", Name = "Chưa thu tiền (Công nợ)", AccountId = !string.IsNullOrEmpty(TotalAccountId) ? TotalAccountId : "131", IsDebt = true },
                    new PaymentMethodOptionDto { Code = "Cash", Name = "Tiền mặt", AccountId = "1111", IsDebt = false },
                    new PaymentMethodOptionDto { Code = "Bank", Name = "Chuyển khoản", AccountId = "1121", IsDebt = false }
                };
            }
        }

        public string GetAccountIdForPaymentType(string? paymentType)
        {
            var methods = GetEffectivePaymentMethods();
            if (string.IsNullOrEmpty(paymentType))
                return methods.FirstOrDefault()?.AccountId ?? TotalAccountId;

            var match = methods.FirstOrDefault(m => m.Code.Equals(paymentType, StringComparison.OrdinalIgnoreCase));
            return match != null ? match.AccountId : TotalAccountId;
        }
    }

    public class CashVoucherConfigItemDto
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string DebitAcc { get; set; } = "";
        public string CreditAcc { get; set; } = "";
        public string VatAcc { get; set; } = "";
        public string PartnerType { get; set; } = "";
    }

    public class CashPostingConfigDto
    {
        public string DefaultCashAccount { get; set; } = "1111";
        public string DefaultBankAccount { get; set; } = "1121";
        public List<PluginAccountConfigItemDto> CashAccounts { get; set; } = new();
        public List<CashAliasItemDto> ReceiptAliases { get; set; } = new();
        public List<CashAliasItemDto> PaymentAliases { get; set; } = new();
    }

    public class CashAliasItemDto
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string OffsetAccount { get; set; } = "";
        public string VatAccount { get; set; } = "";
        public string Category { get; set; } = ""; // Debt, Cost, Revenue, Inventory, Internal, Other
        public bool RequireDetail { get; set; } = false;
        public string DefaultDescription { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;

        public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : Code;
        public string FullDisplay => !string.IsNullOrWhiteSpace(OffsetAccount) ? $"{Name} ({OffsetAccount})" : Name;
    }

    public static class DbConfigService
    {
        // Khóa kiểm soát bí mật (Secret Salt) của tác giả Phạm Việt Cường
        public const string SecretSalt = "PVCUONG#NEWACC70@2026$KEY";
        public const string RecordTierSalt = "PVCUONG#NEWACC2026$RECORD_TIER";
        public const int RecordMaskVal = 0xA75C31;

        // Ma trận vị trí đan xen 6 ký tự số lượng và 10 ký tự chữ ký số MD5 (Đồng bộ 100% với Tool KeyGen & Google Apps Script)
        private static readonly int[] LimitIndices = new int[] { 2, 5, 8, 11, 13, 15 };
        private static readonly int[] HashIndices  = new int[] { 0, 1, 3, 4, 6, 7, 9, 10, 12, 14 };

        public static string LastSaveError { get; set; } = "";

        private static DbConfigDto? _cachedConfig;

        public static DbConfigDto FromEntity(DataAccess.EntityClasses.DbConfigEntity entity)
        {
            if (entity == null) return new DbConfigDto();
            return new DbConfigDto
            {
                Id = entity.Id,
                CompanyName = entity.CompanyName ?? "",
                DepartmentName = entity.DepartmentName ?? "",
                Address = entity.Address ?? "",
                BankAccount = entity.BankAccount ?? "",
                TaxCode = entity.TaxCode ?? "",
                AccountingYear = entity.AccountingYear ?? DateTime.Today.Year,
                RegistrationCode = entity.RegistrationCode ?? "",
                IsCompanyLicense = entity.IsCompanyLicense,
                ExpiryDate = entity.ExpiryDate ?? "20991231",
                AccountMethod = (short)(entity.AccountMethod ?? 1),
                Material = (short)(entity.Material ?? 1),
                VoucherPostingConfig = entity.VoucherPostingConfig ?? "",
                CashPostingConfig = entity.CashPostingConfig ?? ""
            };
        }

        public static VoucherPostingConfigDto GetDefaultVoucherPostingConfig()
        {
            var cfg = new VoucherPostingConfigDto();
            cfg.Inventory["NM"] = new InventoryVoucherConfigItemDto
            {
                Code = "NM",
                Name = "Nhập mua",
                Dbcr = 1,
                Taxflag = true,
                TotalAccountId = "3311",
                TaxAccountId = "13311",
                CostAccountId = "6321",
                RevenueAccountId = "5111",
                PartnerType = "Vendor",
                PaymentMethods = new List<PaymentMethodOptionDto>
                {
                    new PaymentMethodOptionDto { Code = "Debt", Name = "Chưa thanh toán (Công nợ)", AccountId = "3311", IsDebt = true },
                    new PaymentMethodOptionDto { Code = "Cash", Name = "Tiền mặt", AccountId = "1111", IsDebt = false },
                    new PaymentMethodOptionDto { Code = "Bank", Name = "Chuyển khoản", AccountId = "1121", IsDebt = false },
                    new PaymentMethodOptionDto { Code = "Advance", Name = "Tạm ứng nhân viên", AccountId = "141", IsDebt = false }
                }
            };
            cfg.Inventory["XB"] = new InventoryVoucherConfigItemDto
            {
                Code = "XB",
                Name = "Xuất bán",
                Dbcr = -1,
                Taxflag = true,
                TotalAccountId = "131",
                TaxAccountId = "33311",
                CostAccountId = "6321",
                RevenueAccountId = "5111",
                PartnerType = "Customer",
                PaymentMethods = new List<PaymentMethodOptionDto>
                {
                    new PaymentMethodOptionDto { Code = "Debt", Name = "Chưa thu tiền (Công nợ)", AccountId = "131", IsDebt = true },
                    new PaymentMethodOptionDto { Code = "Cash", Name = "Tiền mặt", AccountId = "1111", IsDebt = false },
                    new PaymentMethodOptionDto { Code = "Bank", Name = "Chuyển khoản", AccountId = "1121", IsDebt = false }
                }
            };
            cfg.Inventory["NNB"] = new InventoryVoucherConfigItemDto
            {
                Code = "NNB",
                Name = "Nhập nội bộ",
                Dbcr = 1,
                Taxflag = false,
                TotalAccountId = "154",
                TaxAccountId = "",
                CostAccountId = "",
                RevenueAccountId = "",
                PartnerType = "Internal"
            };
            cfg.Inventory["XNB"] = new InventoryVoucherConfigItemDto
            {
                Code = "XNB",
                Name = "Xuất nội bộ",
                Dbcr = -1,
                Taxflag = false,
                TotalAccountId = "642",
                TaxAccountId = "",
                CostAccountId = "",
                RevenueAccountId = "",
                PartnerType = "Department"
            };

            // 1. Cấu hình danh sách tài khoản theo từng mục định khoản (Slot-based) cho NM
            cfg.Inventory["NM"].AccountSlots["Inventory"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "1561", BaseName = "Giá mua hàng hóa", CustomName = "Hàng hóa kho chính", IsDefault = true, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "152", BaseName = "Nguyên liệu, vật liệu", CustomName = "Nguyên vật liệu", IsDefault = false, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "153", BaseName = "Công cụ, dụng cụ", CustomName = "Công cụ, dụng cụ", IsDefault = false, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "155", BaseName = "Thành phẩm", CustomName = "Thành phẩm", IsDefault = false, BaseCategoryId = "B", AllowDetail = false }
            };
            cfg.Inventory["NM"].AccountSlots["Tax"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "13311", BaseName = "Thuế GTGT đầu vào của hàng hóa, dịch vụ", CustomName = "Thuế GTGT mua vào", IsDefault = true, BaseCategoryId = "V" },
                new() { AccountId = "13312", BaseName = "Thuế GTGT đầu vào của TSCĐ", CustomName = "Thuế GTGT mua vào TSCĐ", IsDefault = false, BaseCategoryId = "V" }
            };
            cfg.Inventory["NM"].AccountSlots["Total"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "3311", BaseName = "Phải trả cho người bán", CustomName = "Phải trả người bán", IsDefault = true, BaseCategoryId = "A" },
                new() { AccountId = "1111", BaseName = "Tiền mặt Việt Nam", CustomName = "Tiền mặt", IsDefault = false, BaseCategoryId = "I" },
                new() { AccountId = "1121", BaseName = "Tiền gửi ngân hàng Việt Nam", CustomName = "Tiền gửi ngân hàng", IsDefault = false, BaseCategoryId = "I" },
                new() { AccountId = "141", BaseName = "Tạm ứng", CustomName = "Tạm ứng nhân viên", IsDefault = false, BaseCategoryId = "I" }
            };

            // 2. Cấu hình danh sách tài khoản theo từng mục định khoản (Slot-based) cho XB
            cfg.Inventory["XB"].AccountSlots["Revenue"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "5111", BaseName = "Doanh thu bán hàng hóa", CustomName = "Doanh thu bán hàng hóa", IsDefault = true, BaseCategoryId = "I" },
                new() { AccountId = "5112", BaseName = "Doanh thu bán các thành phẩm", CustomName = "Doanh thu bán thành phẩm", IsDefault = false, BaseCategoryId = "I" },
                new() { AccountId = "5113", BaseName = "Doanh thu cung cấp dịch vụ", CustomName = "Doanh thu dịch vụ", IsDefault = false, BaseCategoryId = "I" },
                new() { AccountId = "711", BaseName = "Thu nhập khác", CustomName = "Thu nhập khác", IsDefault = false, BaseCategoryId = "I" }
            };
            cfg.Inventory["XB"].AccountSlots["Cost"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "6321", BaseName = "Giá vốn hàng hóa", CustomName = "Giá vốn hàng hóa", IsDefault = true, BaseCategoryId = "E" },
                new() { AccountId = "6322", BaseName = "Giá vốn thành phẩm", CustomName = "Giá vốn thành phẩm", IsDefault = false, BaseCategoryId = "E" },
                new() { AccountId = "811", BaseName = "Chi phí khác", CustomName = "Chi phí khác", IsDefault = false, BaseCategoryId = "E" }
            };
            cfg.Inventory["XB"].AccountSlots["Tax"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "33311", BaseName = "Thuế GTGT đầu ra", CustomName = "Thuế GTGT đầu ra", IsDefault = true, BaseCategoryId = "V" }
            };
            cfg.Inventory["XB"].AccountSlots["Total"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "131", BaseName = "Phải thu của khách hàng", CustomName = "Phải thu khách hàng", IsDefault = true, BaseCategoryId = "A" },
                new() { AccountId = "1111", BaseName = "Tiền mặt Việt Nam", CustomName = "Tiền mặt", IsDefault = false, BaseCategoryId = "I" },
                new() { AccountId = "1121", BaseName = "Tiền gửi ngân hàng Việt Nam", CustomName = "Tiền gửi ngân hàng", IsDefault = false, BaseCategoryId = "I" }
            };
            cfg.Inventory["XB"].AccountSlots["Inventory"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "1561", BaseName = "Giá mua hàng hóa", CustomName = "Hàng hóa kho chính", IsDefault = true, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "155", BaseName = "Thành phẩm", CustomName = "Thành phẩm", IsDefault = false, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "152", BaseName = "Nguyên liệu, vật liệu", CustomName = "Nguyên vật liệu", IsDefault = false, BaseCategoryId = "B", AllowDetail = false }
            };

            // 3. Cấu hình danh sách tài khoản theo từng mục định khoản (Slot-based) cho NNB
            cfg.Inventory["NNB"].AccountSlots["Inventory"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "1561", BaseName = "Giá mua hàng hóa", CustomName = "Hàng hóa kho chính", IsDefault = true, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "155", BaseName = "Thành phẩm", CustomName = "Thành phẩm", IsDefault = false, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "152", BaseName = "Nguyên liệu, vật liệu", CustomName = "Nguyên vật liệu", IsDefault = false, BaseCategoryId = "B", AllowDetail = false }
            };
            cfg.Inventory["NNB"].AccountSlots["Offset"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "154", BaseName = "Chi phí sản xuất, kinh doanh dở dang", CustomName = "Chi phí SXKD dở dang", IsDefault = true, BaseCategoryId = "E" },
                new() { AccountId = "711", BaseName = "Thu nhập khác", CustomName = "Thu nhập khác", IsDefault = false, BaseCategoryId = "I" }
            };

            // 4. Cấu hình danh sách tài khoản theo từng mục định khoản (Slot-based) cho XNB
            cfg.Inventory["XNB"].AccountSlots["Inventory"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "152", BaseName = "Nguyên liệu, vật liệu", CustomName = "Nguyên vật liệu", IsDefault = true, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "153", BaseName = "Công cụ, dụng cụ", CustomName = "Công cụ dụng cụ", IsDefault = false, BaseCategoryId = "B", AllowDetail = false },
                new() { AccountId = "1561", BaseName = "Giá mua hàng hóa", CustomName = "Hàng hóa", IsDefault = false, BaseCategoryId = "B", AllowDetail = false }
            };
            cfg.Inventory["XNB"].AccountSlots["Offset"] = new List<VoucherPostingAccountOptionDto>
            {
                new() { AccountId = "642", BaseName = "Chi phí quản lý doanh nghiệp", CustomName = "Chi phí quản lý", IsDefault = true, BaseCategoryId = "E" },
                new() { AccountId = "641", BaseName = "Chi phí bán hàng", CustomName = "Chi phí bán hàng", IsDefault = false, BaseCategoryId = "E" },
                new() { AccountId = "621", BaseName = "Chi phí nguyên liệu, vật liệu trực tiếp", CustomName = "Chi phí NVL trực tiếp", IsDefault = false, BaseCategoryId = "E" },
                new() { AccountId = "154", BaseName = "Chi phí sản xuất, kinh doanh dở dang", CustomName = "Chi phí SXKD dở dang", IsDefault = false, BaseCategoryId = "E" }
            };

            // Tập tài khoản ERP chuẩn mặc định
            cfg.ErpAccounts = new List<PluginAccountConfigItemDto>
            {
                // 1. Doanh thu (Revenue)
                new() { AccountId = "5111", BaseName = "Doanh thu bán hàng hóa", CustomName = "Doanh thu bán hàng hóa", Category = "Revenue" },
                new() { AccountId = "5112", BaseName = "Doanh thu bán các thành phẩm", CustomName = "Doanh thu bán thành phẩm", Category = "Revenue" },
                new() { AccountId = "5113", BaseName = "Doanh thu cung cấp dịch vụ", CustomName = "Doanh thu dịch vụ", Category = "Revenue" },
                new() { AccountId = "711", BaseName = "Thu nhập khác", CustomName = "Thu nhập khác", Category = "Revenue" },

                // 2. Giá vốn (Cost)
                new() { AccountId = "6321", BaseName = "Giá vốn hàng hóa", CustomName = "Giá vốn hàng hóa", Category = "Cost" },
                new() { AccountId = "6322", BaseName = "Giá vốn thành phẩm", CustomName = "Giá vốn thành phẩm", Category = "Cost" },
                new() { AccountId = "632", BaseName = "Giá vốn hàng bán", CustomName = "Giá vốn hàng bán", Category = "Cost" },
                new() { AccountId = "811", BaseName = "Chi phí khác", CustomName = "Chi phí khác", Category = "Cost" },

                // 3. Kho vật tư / Hàng hóa (Inventory)
                new() { AccountId = "1561", BaseName = "Giá mua hàng hóa", CustomName = "Hàng hóa kho chính", Category = "Inventory" },
                new() { AccountId = "152", BaseName = "Nguyên liệu, vật liệu", CustomName = "Nguyên vật liệu", Category = "Inventory" },
                new() { AccountId = "153", BaseName = "Công cụ, dụng cụ", CustomName = "Công cụ dụng cụ", Category = "Inventory" },
                new() { AccountId = "155", BaseName = "Thành phẩm", CustomName = "Thành phẩm", Category = "Inventory" },
                new() { AccountId = "156", BaseName = "Hàng hóa", CustomName = "Hàng hóa", Category = "Inventory" },

                // 4. Thuế GTGT (Tax)
                new() { AccountId = "13311", BaseName = "Thuế GTGT đầu vào của hàng hóa, dịch vụ", CustomName = "Thuế GTGT mua vào", Category = "Tax" },
                new() { AccountId = "33311", BaseName = "Thuế GTGT đầu ra", CustomName = "Thuế GTGT bán ra", Category = "Tax" },

                // 5. Công nợ & Thanh toán (Debt / Payment)
                new() { AccountId = "131", BaseName = "Phải thu của khách hàng", CustomName = "Phải thu khách hàng", Category = "Debt" },
                new() { AccountId = "3311", BaseName = "Phải trả cho người bán", CustomName = "Phải trả người bán", Category = "Debt" },
                new() { AccountId = "1111", BaseName = "Tiền mặt Việt Nam", CustomName = "Tiền mặt", Category = "Payment" },
                new() { AccountId = "1121", BaseName = "Tiền gửi ngân hàng Việt Nam", CustomName = "Tiền gửi ngân hàng", Category = "Payment" },
                new() { AccountId = "141", BaseName = "Tạm ứng", CustomName = "Tạm ứng nhân viên", Category = "Payment" },

                // 6. Nội bộ (Internal)
                new() { AccountId = "154", BaseName = "Chi phí sản xuất, kinh doanh dở dang", CustomName = "Chi phí SXKD dở dang", Category = "Internal" },
                new() { AccountId = "642", BaseName = "Chi phí quản lý doanh nghiệp", CustomName = "Chi phí quản lý doanh nghiệp", Category = "Internal" }
            };

            return cfg;
        }

        public static VoucherPostingConfigDto GetVoucherPostingConfig(this DbConfigDto cfg)
        {
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.VoucherPostingConfig))
            {
                return GetDefaultVoucherPostingConfig();
            }

            try
            {
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var res = System.Text.Json.JsonSerializer.Deserialize<VoucherPostingConfigDto>(cfg.VoucherPostingConfig, options);
                if (res != null && res.Inventory != null && res.Inventory.Count > 0)
                {
                    var defCfg = GetDefaultVoucherPostingConfig();
                    foreach (var kvp in defCfg.Inventory)
                    {
                        if (res.Inventory.TryGetValue(kvp.Key, out var item))
                        {
                            if (item.AccountSlots == null || item.AccountSlots.Count == 0)
                            {
                                item.AccountSlots = kvp.Value.AccountSlots;
                            }
                        }
                        else
                        {
                            res.Inventory[kvp.Key] = kvp.Value;
                        }
                    }

                    if (res.ErpAccounts == null || res.ErpAccounts.Count == 0)
                    {
                        res.ErpAccounts = defCfg.ErpAccounts;
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Deserialize VoucherPostingConfig error: " + ex.Message);
            }

            return GetDefaultVoucherPostingConfig();
        }

        public static void SetVoucherPostingConfig(this DbConfigDto cfg, VoucherPostingConfigDto postingCfg)
        {
            if (cfg == null || postingCfg == null) return;
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                cfg.VoucherPostingConfig = System.Text.Json.JsonSerializer.Serialize(postingCfg, options);
            }
            catch { }
        }

        public static bool SaveVoucherPostingConfig(VoucherPostingConfigDto postingCfg)
        {
            var cfg = Current;
            cfg.SetVoucherPostingConfig(postingCfg);
            return SaveDbConfig(cfg);
        }

        public static CashPostingConfigDto GetDefaultCashPostingConfig()
        {
            var cfg = new CashPostingConfigDto();
            cfg.CashAccounts = new List<PluginAccountConfigItemDto>
            {
                new PluginAccountConfigItemDto { AccountId = "1111", BaseName = "Tiền Việt Nam", CustomName = "Tiền mặt tại quỹ", Category = "Cash", BaseCategoryId = "A", IsActive = true },
                new PluginAccountConfigItemDto { AccountId = "1112", BaseName = "Ngoại tệ", CustomName = "Ngoại tệ tại quỹ", Category = "Cash", BaseCategoryId = "A", IsActive = true },
                new PluginAccountConfigItemDto { AccountId = "1121", BaseName = "Tiền gửi ngân hàng", CustomName = "Tài khoản ngân hàng", Category = "Bank", BaseCategoryId = "A", IsActive = true }
            };

            cfg.ReceiptAliases = new List<CashAliasItemDto>
            {
                new CashAliasItemDto { Code = "THU_NO_KH", Name = "Thu nợ khách hàng", OffsetAccount = "131", Category = "Debt", RequireDetail = true, DefaultDescription = "Thu tiền nợ của khách hàng", IsDefault = true },
                new CashAliasItemDto { Code = "THU_BAN_HANG", Name = "Thu tiền bán hàng trực tiếp", OffsetAccount = "5111", VatAccount = "33311", Category = "Revenue", RequireDetail = false, DefaultDescription = "Thu tiền bán hàng trực tiếp" },
                new CashAliasItemDto { Code = "THU_TAM_UNG", Name = "Thu hoàn ứng nhân viên", OffsetAccount = "141", Category = "Debt", RequireDetail = true, DefaultDescription = "Thu hoàn tiền tạm ứng của nhân viên" },
                new CashAliasItemDto { Code = "THU_RUT_NH", Name = "Rút tiền ngân hàng về quỹ tiền mặt", OffsetAccount = "1121", Category = "Internal", RequireDetail = false, DefaultDescription = "Rút tiền gửi ngân hàng về nhập quỹ tiền mặt" },
                new CashAliasItemDto { Code = "THU_LAI_NH", Name = "Thu lãi tiền gửi ngân hàng", OffsetAccount = "515", Category = "Revenue", RequireDetail = false, DefaultDescription = "Thu lãi tiền gửi ngân hàng" },
                new CashAliasItemDto { Code = "THU_KHAC", Name = "Thu nhập khác", OffsetAccount = "711", Category = "Other", RequireDetail = false, DefaultDescription = "Thu tiền khác" }
            };

            cfg.PaymentAliases = new List<CashAliasItemDto>
            {
                new CashAliasItemDto { Code = "CHI_TRA_NCC", Name = "Trả nợ tiền mua hàng cho NCC", OffsetAccount = "3311", Category = "Debt", RequireDetail = true, DefaultDescription = "Chi trả tiền nợ cho nhà cung cấp", IsDefault = true },
                new CashAliasItemDto { Code = "CHI_TAM_UNG", Name = "Tạm ứng cho nhân viên", OffsetAccount = "141", Category = "Debt", RequireDetail = true, DefaultDescription = "Chi tạm ứng cho nhân viên" },
                new CashAliasItemDto { Code = "CHI_PHI_QL", Name = "Chi phí quản lý doanh nghiệp", OffsetAccount = "642", Category = "Cost", RequireDetail = true, DefaultDescription = "Chi phí quản lý doanh nghiệp" },
                new CashAliasItemDto { Code = "CHI_PHI_BH", Name = "Chi phí bán hàng", OffsetAccount = "641", Category = "Cost", RequireDetail = true, DefaultDescription = "Chi phí bán hàng" },
                new CashAliasItemDto { Code = "CHI_MUA_HANG", Name = "Mua hàng/vật tư trả tiền ngay", OffsetAccount = "1561", VatAccount = "13311", Category = "Inventory", RequireDetail = false, DefaultDescription = "Chi tiền mua hàng hóa, vật tư trả ngay" },
                new CashAliasItemDto { Code = "CHI_LUONG", Name = "Chi trả lương cán bộ công nhân viên", OffsetAccount = "3341", Category = "Debt", RequireDetail = false, DefaultDescription = "Chi trả lương cán bộ công nhân viên" },
                new CashAliasItemDto { Code = "CHI_NOP_NH", Name = "Nộp tiền mặt vào ngân hàng", OffsetAccount = "1111", Category = "Internal", RequireDetail = false, DefaultDescription = "Nộp tiền mặt vào tài khoản ngân hàng" },
                new CashAliasItemDto { Code = "CHI_NOP_THUE", Name = "Nộp thuế vào NSNN", OffsetAccount = "333", Category = "Debt", RequireDetail = false, DefaultDescription = "Nộp thuế vào ngân sách nhà nước" },
                new CashAliasItemDto { Code = "CHI_KHAC", Name = "Chi phí khác bằng tiền", OffsetAccount = "811", Category = "Other", RequireDetail = false, DefaultDescription = "Chi phí khác bằng tiền" }
            };

            return cfg;
        }

        public static CashPostingConfigDto GetCashPostingConfig(this DbConfigDto cfg)
        {
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.CashPostingConfig))
            {
                return GetDefaultCashPostingConfig();
            }

            try
            {
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var res = System.Text.Json.JsonSerializer.Deserialize<CashPostingConfigDto>(cfg.CashPostingConfig, options);
                if (res != null)
                {
                    var def = GetDefaultCashPostingConfig();
                    if (res.ReceiptAliases == null || res.ReceiptAliases.Count == 0) res.ReceiptAliases = def.ReceiptAliases;
                    if (res.PaymentAliases == null || res.PaymentAliases.Count == 0) res.PaymentAliases = def.PaymentAliases;
                    if (res.CashAccounts == null || res.CashAccounts.Count == 0) res.CashAccounts = def.CashAccounts;
                    if (string.IsNullOrWhiteSpace(res.DefaultCashAccount)) res.DefaultCashAccount = def.DefaultCashAccount;
                    if (string.IsNullOrWhiteSpace(res.DefaultBankAccount)) res.DefaultBankAccount = def.DefaultBankAccount;
                    return res;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Deserialize CashPostingConfig error: " + ex.Message);
            }

            return GetDefaultCashPostingConfig();
        }

        public static void SetCashPostingConfig(this DbConfigDto cfg, CashPostingConfigDto postingCfg)
        {
            if (cfg == null || postingCfg == null) return;
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                cfg.CashPostingConfig = System.Text.Json.JsonSerializer.Serialize(postingCfg, options);
            }
            catch { }
        }

        public static bool SaveCashPostingConfig(CashPostingConfigDto postingCfg)
        {
            var cfg = Current;
            cfg.SetCashPostingConfig(postingCfg);
            return SaveDbConfig(cfg);
        }

        public static DataAccess.EntityClasses.DbConfigEntity ToEntity(DbConfigDto dto)
        {
            return new DataAccess.EntityClasses.DbConfigEntity
            {
                Id = dto.Id,
                CompanyName = dto.CompanyName,
                DepartmentName = dto.DepartmentName,
                Address = dto.Address,
                BankAccount = dto.BankAccount,
                TaxCode = dto.TaxCode,
                AccountingYear = dto.AccountingYear,
                RegistrationCode = dto.RegistrationCode,
                IsCompanyLicense = dto.IsCompanyLicense,
                ExpiryDate = dto.ExpiryDate,
                AccountMethod = dto.AccountMethod,
                Material = dto.Material,
                VoucherPostingConfig = dto.VoucherPostingConfig ?? "",
                CashPostingConfig = dto.CashPostingConfig ?? "",
                UpdatedDate = DateTime.Now
            };
        }

        public static DbConfigDto Current
        {
            get
            {
                if (_cachedConfig == null)
                {
                    _cachedConfig = LoadDbConfig();
                }
                return _cachedConfig;
            }
        }

        public static void ClearCache()
        {
            _cachedConfig = null;
        }

        /// <summary>
        /// Đảm bảo bảng DbConfig luôn tồn tại trong CSDL đang kết nối (Access, SQL Server, Postgres)
        /// </summary>
        public static void EnsureDbConfigTable()
        {
            try
            {
                if (ConfigManager.Current.DbType == DatabaseType.Access && 
                    (string.IsNullOrWhiteSpace(ConfigManager.Current.AccessFilePath) || !System.IO.File.Exists(ConfigManager.Current.AccessFilePath)))
                {
                    return;
                }

                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    bool tableExists = CheckTableExists(conn, "DbConfig");
                    if (!tableExists)
                    {
                        CreateTable(conn);
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        // Đảm bảo có dòng Id = 1
                        EnsureDefaultRowExists(conn);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("EnsureDbConfigTable error: " + ex.Message);
            }
        }

        private static bool CheckTableExists(DbConnection conn, string tableName)
        {
            try
            {
                using (var cmd = conn.CreateCommand())
                {
                    if (ConfigManager.Current.DbType == DatabaseType.PostgreSql)
                    {
                        cmd.CommandText = $@"SELECT COUNT(*) FROM ""{tableName}""";
                    }
                    else
                    {
                        cmd.CommandText = $"SELECT COUNT(*) FROM {tableName}";
                    }
                    cmd.ExecuteScalar();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void CreateTable(DbConnection conn)
        {
            string dbType = ConfigManager.Current.DbType.ToString();
            string sql;

            if (dbType == "SqlServer")
            {
                sql = @"
CREATE TABLE DbConfig (
    Id INT PRIMARY KEY,
    CompanyName NVARCHAR(255),
    DepartmentName NVARCHAR(150),
    Address NVARCHAR(255),
    BankAccount NVARCHAR(50),
    TaxCode NVARCHAR(50),
    AccountingYear INT,
    RegistrationCode NVARCHAR(100),
    IsCompanyLicense BIT,
    ExpiryDate NVARCHAR(10),
    AccountMethod SMALLINT DEFAULT 1,
    Material SMALLINT DEFAULT 1,
    VoucherPostingConfig NVARCHAR(MAX),
    CreatedDate DATETIME,
    UpdatedDate DATETIME
)";
            }
            else if (dbType == "PostgreSql")
            {
                sql = @"
CREATE TABLE IF NOT EXISTS ""DbConfig"" (
    ""Id"" INT PRIMARY KEY,
    ""CompanyName"" VARCHAR(255),
    ""DepartmentName"" VARCHAR(150),
    ""Address"" VARCHAR(255),
    ""BankAccount"" VARCHAR(50),
    ""TaxCode"" VARCHAR(50),
    ""AccountingYear"" INT,
    ""RegistrationCode"" VARCHAR(100),
    ""IsCompanyLicense"" BOOLEAN,
    ""ExpiryDate"" VARCHAR(10),
    ""AccountMethod"" SMALLINT DEFAULT 1,
    ""Material"" SMALLINT DEFAULT 1,
    ""VoucherPostingConfig"" TEXT,
    ""CreatedDate"" TIMESTAMP,
    ""UpdatedDate"" TIMESTAMP
)";
            }
            else // Access
            {
                sql = @"
CREATE TABLE DbConfig (
    Id INT PRIMARY KEY,
    CompanyName VARCHAR(255),
    DepartmentName VARCHAR(150),
    Address VARCHAR(255),
    BankAccount VARCHAR(50),
    TaxCode VARCHAR(50),
    AccountingYear INT,
    RegistrationCode VARCHAR(100),
    IsCompanyLicense YESNO,
    ExpiryDate VARCHAR(10),
    AccountMethod INTEGER,
    Material INTEGER,
    VoucherPostingConfig LONGTEXT,
    CreatedDate DATETIME,
    UpdatedDate DATETIME
)";
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        private static string GetDefaultVoucherPostingConfigJson()
        {
            try
            {
                var defaultCfg = GetDefaultVoucherPostingConfig();
                return System.Text.Json.JsonSerializer.Serialize(defaultCfg, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                return "";
            }
        }

        private static void InsertDefaultRow(DbConnection conn)
        {
            try
            {
                string defaultJson = GetDefaultVoucherPostingConfigJson();
                using (var cmd = conn.CreateCommand())
                {
                    if (ConfigManager.Current.DbType == DatabaseType.PostgreSql)
                    {
                        cmd.CommandText = @"INSERT INTO ""DbConfig"" (""Id"", ""CompanyName"", ""DepartmentName"", ""Address"", ""BankAccount"", ""TaxCode"", ""AccountingYear"", ""RegistrationCode"", ""IsCompanyLicense"", ""ExpiryDate"", ""AccountMethod"", ""Material"", ""VoucherPostingConfig"")
                            VALUES (1, '', 'Phòng Kế Toán', '', '', '', " + DateTime.Today.Year + @", '', false, '20991231', 1, 1, @cfg)";
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@cfg";
                        p.Value = defaultJson;
                        cmd.Parameters.Add(p);
                    }
                    else
                    {
                        cmd.CommandText = @"INSERT INTO DbConfig (Id, CompanyName, DepartmentName, Address, BankAccount, TaxCode, AccountingYear, RegistrationCode, IsCompanyLicense, ExpiryDate, AccountMethod, Material, VoucherPostingConfig)
                            VALUES (1, '', 'Phòng Kế Toán', '', '', '', " + DateTime.Today.Year + @", '', 0, '20991231', 1, 1, @cfg)";
                        var p = cmd.CreateParameter();
                        p.ParameterName = "@cfg";
                        p.Value = defaultJson;
                        cmd.Parameters.Add(p);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private static void EnsureDefaultRowExists(DbConnection conn)
        {
            try
            {
                // Đảm bảo các cột mới AccountMethod và Material tồn tại nếu bảng đã tạo từ trước
                EnsureColumnsExist(conn);

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                        ? @"SELECT COUNT(*) FROM ""DbConfig"""
                        : "SELECT COUNT(*) FROM DbConfig";
                    var obj = cmd.ExecuteScalar();
                    int count = obj != null && int.TryParse(obj.ToString(), out int c) ? c : 0;
                    if (count == 0)
                    {
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        // Kiểm tra nếu VoucherPostingConfig đang NULL hoặc rỗng thì tự động nạp cấu hình default
                        try
                        {
                            using var checkCmd = conn.CreateCommand();
                            checkCmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                                ? @"SELECT ""VoucherPostingConfig"" FROM ""DbConfig"" WHERE ""Id"" = 1"
                                : "SELECT VoucherPostingConfig FROM DbConfig WHERE Id = 1";
                            var val = checkCmd.ExecuteScalar();
                            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()))
                            {
                                string defaultJson = GetDefaultVoucherPostingConfigJson();
                                if (!string.IsNullOrEmpty(defaultJson))
                                {
                                    using var updateCmd = conn.CreateCommand();
                                    updateCmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                                        ? @"UPDATE ""DbConfig"" SET ""VoucherPostingConfig"" = @cfg WHERE ""Id"" = 1"
                                        : "UPDATE DbConfig SET VoucherPostingConfig = @cfg WHERE Id = 1";
                                    var p = updateCmd.CreateParameter();
                                    p.ParameterName = "@cfg";
                                    p.Value = defaultJson;
                                    updateCmd.Parameters.Add(p);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch { }

                        // Tự động chuẩn hóa liên kết JournalVoucherId cho các dòng con (SubEntries) có ParentId hợp lệ
                        EnsureSubEntriesLinkedToVoucher(conn);
                    }
                }
            }
            catch { }
        }

        private static void EnsureSubEntriesLinkedToVoucher(DbConnection conn)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                if (ConfigManager.Current.DbType == DatabaseType.Access)
                {
                    cmd.CommandText = "UPDATE Journal INNER JOIN Journal AS p ON Journal.ParentId = p.Id SET Journal.JournalVoucherId = p.JournalVoucherId WHERE (Journal.JournalVoucherId IS NULL OR Journal.JournalVoucherId = 0) AND (p.JournalVoucherId IS NOT NULL AND p.JournalVoucherId > 0)";
                }
                else if (ConfigManager.Current.DbType == DatabaseType.SqlServer)
                {
                    cmd.CommandText = "UPDATE c SET c.JournalVoucherId = p.JournalVoucherId FROM Journal c INNER JOIN Journal p ON c.ParentId = p.Id WHERE (c.JournalVoucherId IS NULL OR c.JournalVoucherId = 0) AND (p.JournalVoucherId IS NOT NULL AND p.JournalVoucherId > 0)";
                }
                else if (ConfigManager.Current.DbType == DatabaseType.PostgreSql)
                {
                    cmd.CommandText = @"UPDATE ""Journal"" c SET ""JournalVoucherId"" = p.""JournalVoucherId"" FROM ""Journal"" p WHERE c.""ParentId"" = p.""Id"" AND (c.""JournalVoucherId"" IS NULL OR c.""JournalVoucherId"" = 0) AND (p.""JournalVoucherId"" IS NOT NULL AND p.""JournalVoucherId"" > 0)";
                }
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        private static void EnsureColumnsExist(DbConnection conn)
        {
            string dbType = ConfigManager.Current.DbType.ToString();
            string[] alterStatements;

            if (dbType == "SqlServer")
            {
                alterStatements = new[]
                {
                    "IF COL_LENGTH('DbConfig', 'AccountMethod') IS NULL ALTER TABLE DbConfig ADD AccountMethod SMALLINT DEFAULT 1;",
                    "IF COL_LENGTH('DbConfig', 'Material') IS NULL ALTER TABLE DbConfig ADD Material SMALLINT DEFAULT 1;",
                    "IF COL_LENGTH('DbConfig', 'VoucherPostingConfig') IS NULL ALTER TABLE DbConfig ADD VoucherPostingConfig NVARCHAR(MAX);",
                    "IF COL_LENGTH('DbConfig', 'CashPostingConfig') IS NULL ALTER TABLE DbConfig ADD CashPostingConfig NVARCHAR(MAX);"
                };
            }
            else if (dbType == "PostgreSql")
            {
                alterStatements = new[]
                {
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"AccountMethod\" SMALLINT DEFAULT 1;",
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"Material\" SMALLINT DEFAULT 1;",
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"VoucherPostingConfig\" TEXT;",
                    "ALTER TABLE \"DbConfig\" ADD COLUMN IF NOT EXISTS \"CashPostingConfig\" TEXT;"
                };
            }
            else // Access
            {
                alterStatements = new[]
                {
                    "ALTER TABLE DbConfig ADD COLUMN AccountMethod INTEGER;",
                    "ALTER TABLE DbConfig ADD COLUMN Material INTEGER;",
                    "ALTER TABLE DbConfig ADD COLUMN VoucherPostingConfig LONGTEXT;",
                    "ALTER TABLE DbConfig ADD COLUMN CashPostingConfig LONGTEXT;"
                };
            }

            foreach (var sql in alterStatements)
            {
                try
                {
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
        }

        public static DbConfigDto LoadDbConfig()
        {
            var dto = new DbConfigDto();
            try
            {
                if (ConfigManager.Current.DbType == DatabaseType.Access && 
                    (string.IsNullOrWhiteSpace(ConfigManager.Current.AccessFilePath) || !System.IO.File.Exists(ConfigManager.Current.AccessFilePath)))
                {
                    return dto;
                }

                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    if (!CheckTableExists(conn, "DbConfig"))
                    {
                        CreateTable(conn);
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        EnsureDefaultRowExists(conn);
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                            ? @"SELECT * FROM ""DbConfig"" ORDER BY ""Id"" ASC LIMIT 1"
                            : "SELECT TOP 1 * FROM DbConfig ORDER BY Id ASC";

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto.Id = HasColumn(reader, "Id") && reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 1;
                                dto.CompanyName = HasColumn(reader, "CompanyName") ? (reader["CompanyName"]?.ToString() ?? "") : "";
                                dto.DepartmentName = HasColumn(reader, "DepartmentName") ? (reader["DepartmentName"]?.ToString() ?? "") : "";
                                dto.Address = HasColumn(reader, "Address") ? (reader["Address"]?.ToString() ?? "") : "";
                                dto.BankAccount = HasColumn(reader, "BankAccount") ? (reader["BankAccount"]?.ToString() ?? "") : "";
                                dto.TaxCode = HasColumn(reader, "TaxCode") ? (reader["TaxCode"]?.ToString() ?? "") : "";
                                dto.AccountingYear = HasColumn(reader, "AccountingYear") && reader["AccountingYear"] != DBNull.Value ? Convert.ToInt32(reader["AccountingYear"]) : DateTime.Today.Year;
                                dto.RegistrationCode = HasColumn(reader, "RegistrationCode") ? (reader["RegistrationCode"]?.ToString() ?? "") : "";
                                dto.IsCompanyLicense = HasColumn(reader, "IsCompanyLicense") && reader["IsCompanyLicense"] != DBNull.Value && Convert.ToBoolean(reader["IsCompanyLicense"]);
                                dto.ExpiryDate = HasColumn(reader, "ExpiryDate") ? (reader["ExpiryDate"]?.ToString() ?? "20991231") : "20991231";
                                dto.AccountMethod = HasColumn(reader, "AccountMethod") && reader["AccountMethod"] != DBNull.Value ? Convert.ToInt16(reader["AccountMethod"]) : (short)1;
                                dto.Material = HasColumn(reader, "Material") && reader["Material"] != DBNull.Value ? Convert.ToInt16(reader["Material"]) : (short)1;
                                dto.VoucherPostingConfig = HasColumn(reader, "VoucherPostingConfig") ? (reader["VoucherPostingConfig"]?.ToString() ?? "") : "";
                                dto.CashPostingConfig = HasColumn(reader, "CashPostingConfig") ? (reader["CashPostingConfig"]?.ToString() ?? "") : "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadDbConfig error: " + ex.Message);
            }

            try
            {
                string machineCode = License.HardwareHelper.GetMachineCode();
                ValidateRegistrationCode(dto, machineCode, out int maxRecords);
                dto.MaxRecords = maxRecords;
            }
            catch { }

            return dto;
        }

        private static bool HasColumn(IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (string.Equals(reader.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool SaveDbConfig(DbConfigDto dto)
        {
            LastSaveError = "";
            try
            {
                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();

                    if (!CheckTableExists(conn, "DbConfig"))
                    {
                        CreateTable(conn);
                        InsertDefaultRow(conn);
                    }
                    else
                    {
                        EnsureDefaultRowExists(conn);
                    }

                    int existingCount = 0;
                    using (var checkCmd = conn.CreateCommand())
                    {
                        checkCmd.CommandText = ConfigManager.Current.DbType == DatabaseType.PostgreSql
                            ? @"SELECT COUNT(*) FROM ""DbConfig"""
                            : "SELECT COUNT(*) FROM DbConfig";
                        var res = checkCmd.ExecuteScalar();
                        if (res != null && int.TryParse(res.ToString(), out int c)) existingCount = c;
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        var dbType = ConfigManager.Current.DbType;

                        if (dbType == DatabaseType.Access)
                        {
                            if (existingCount == 0)
                            {
                                cmd.CommandText = @"INSERT INTO [DbConfig] ([Id], [CompanyName], [DepartmentName], [Address], [BankAccount], [TaxCode], [AccountingYear], [RegistrationCode], [IsCompanyLicense], [ExpiryDate], [AccountMethod], [Material], [VoucherPostingConfig], [CashPostingConfig], [CreatedDate], [UpdatedDate])
                                    VALUES (1, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, Now(), Now())";
                            }
                            else
                            {
                                cmd.CommandText = @"UPDATE [DbConfig] 
                                    SET [CompanyName] = ?,
                                        [DepartmentName] = ?,
                                        [Address] = ?,
                                        [BankAccount] = ?,
                                        [TaxCode] = ?,
                                        [AccountingYear] = ?,
                                        [RegistrationCode] = ?,
                                        [IsCompanyLicense] = ?,
                                        [ExpiryDate] = ?,
                                        [AccountMethod] = ?,
                                        [Material] = ?,
                                        [VoucherPostingConfig] = ?,
                                        [CashPostingConfig] = ?,
                                        [UpdatedDate] = Now()
                                    WHERE [Id] = 1";
                            }
                            AddParam(cmd, "?", dto.CompanyName ?? "");
                            AddParam(cmd, "?", dto.DepartmentName ?? "");
                            AddParam(cmd, "?", dto.Address ?? "");
                            AddParam(cmd, "?", dto.BankAccount ?? "");
                            AddParam(cmd, "?", dto.TaxCode ?? "");
                            AddParam(cmd, "?", dto.AccountingYear);
                            AddParam(cmd, "?", dto.RegistrationCode ?? "");
                            AddParam(cmd, "?", dto.IsCompanyLicense);
                            AddParam(cmd, "?", dto.ExpiryDate ?? "20991231");
                            AddParam(cmd, "?", (int)dto.AccountMethod);
                            AddParam(cmd, "?", (int)dto.Material);
                            AddParam(cmd, "?", dto.VoucherPostingConfig ?? "");
                            AddParam(cmd, "?", dto.CashPostingConfig ?? "");
                        }
                        else if (dbType == DatabaseType.PostgreSql)
                        {
                            if (existingCount == 0)
                            {
                                cmd.CommandText = @"INSERT INTO ""DbConfig"" (""Id"", ""CompanyName"", ""DepartmentName"", ""Address"", ""BankAccount"", ""TaxCode"", ""AccountingYear"", ""RegistrationCode"", ""IsCompanyLicense"", ""ExpiryDate"", ""AccountMethod"", ""Material"", ""VoucherPostingConfig"", ""CashPostingConfig"", ""CreatedDate"", ""UpdatedDate"")
                                    VALUES (1, @CompanyName, @DepartmentName, @Address, @BankAccount, @TaxCode, @AccountingYear, @RegistrationCode, @IsCompanyLicense, @ExpiryDate, @AccountMethod, @Material, @VoucherPostingConfig, @CashPostingConfig, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";
                            }
                            else
                            {
                                cmd.CommandText = @"UPDATE ""DbConfig"" 
                                    SET ""CompanyName"" = @CompanyName,
                                        ""DepartmentName"" = @DepartmentName,
                                        ""Address"" = @Address,
                                        ""BankAccount"" = @BankAccount,
                                        ""TaxCode"" = @TaxCode,
                                        ""AccountingYear"" = @AccountingYear,
                                        ""RegistrationCode"" = @RegistrationCode,
                                        ""IsCompanyLicense"" = @IsCompanyLicense,
                                        ""ExpiryDate"" = @ExpiryDate,
                                        ""AccountMethod"" = @AccountMethod,
                                        ""Material"" = @Material,
                                        ""VoucherPostingConfig"" = @VoucherPostingConfig,
                                        ""UpdatedDate"" = CURRENT_TIMESTAMP
                                    WHERE ""Id"" = 1";
                            }
                            AddParam(cmd, "@CompanyName", dto.CompanyName ?? "");
                            AddParam(cmd, "@DepartmentName", dto.DepartmentName ?? "");
                            AddParam(cmd, "@Address", dto.Address ?? "");
                            AddParam(cmd, "@BankAccount", dto.BankAccount ?? "");
                            AddParam(cmd, "@TaxCode", dto.TaxCode ?? "");
                            AddParam(cmd, "@AccountingYear", dto.AccountingYear);
                            AddParam(cmd, "@RegistrationCode", dto.RegistrationCode ?? "");
                            AddParam(cmd, "@IsCompanyLicense", dto.IsCompanyLicense);
                            AddParam(cmd, "@ExpiryDate", dto.ExpiryDate ?? "20991231");
                            AddParam(cmd, "@AccountMethod", dto.AccountMethod);
                            AddParam(cmd, "@Material", dto.Material);
                            AddParam(cmd, "@VoucherPostingConfig", dto.VoucherPostingConfig ?? "");
                            AddParam(cmd, "@CashPostingConfig", dto.CashPostingConfig ?? "");
                        }
                        else // SQL Server
                        {
                            if (existingCount == 0)
                            {
                                cmd.CommandText = @"INSERT INTO DbConfig (Id, CompanyName, DepartmentName, Address, BankAccount, TaxCode, AccountingYear, RegistrationCode, IsCompanyLicense, ExpiryDate, AccountMethod, Material, VoucherPostingConfig, CashPostingConfig, CreatedDate, UpdatedDate)
                                    VALUES (1, @CompanyName, @DepartmentName, @Address, @BankAccount, @TaxCode, @AccountingYear, @RegistrationCode, @IsCompanyLicense, @ExpiryDate, @AccountMethod, @Material, @VoucherPostingConfig, @CashPostingConfig, GETDATE(), GETDATE())";
                            }
                            else
                            {
                                cmd.CommandText = @"UPDATE DbConfig 
                                    SET CompanyName = @CompanyName,
                                        DepartmentName = @DepartmentName,
                                        Address = @Address,
                                        BankAccount = @BankAccount,
                                        TaxCode = @TaxCode,
                                        AccountingYear = @AccountingYear,
                                        RegistrationCode = @RegistrationCode,
                                        IsCompanyLicense = @IsCompanyLicense,
                                        ExpiryDate = @ExpiryDate,
                                        AccountMethod = @AccountMethod,
                                        Material = @Material,
                                        VoucherPostingConfig = @VoucherPostingConfig,
                                        UpdatedDate = GETDATE()
                                    WHERE Id = 1";
                            }
                            AddParam(cmd, "@CompanyName", dto.CompanyName ?? "");
                            AddParam(cmd, "@DepartmentName", dto.DepartmentName ?? "");
                            AddParam(cmd, "@Address", dto.Address ?? "");
                            AddParam(cmd, "@BankAccount", dto.BankAccount ?? "");
                            AddParam(cmd, "@TaxCode", dto.TaxCode ?? "");
                            AddParam(cmd, "@AccountingYear", dto.AccountingYear);
                            AddParam(cmd, "@RegistrationCode", dto.RegistrationCode ?? "");
                            AddParam(cmd, "@IsCompanyLicense", dto.IsCompanyLicense);
                            AddParam(cmd, "@ExpiryDate", dto.ExpiryDate ?? "20991231");
                            AddParam(cmd, "@AccountMethod", dto.AccountMethod);
                            AddParam(cmd, "@Material", dto.Material);
                            AddParam(cmd, "@VoucherPostingConfig", dto.VoucherPostingConfig ?? "");
                            AddParam(cmd, "@CashPostingConfig", dto.CashPostingConfig ?? "");
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                string machineCode = License.HardwareHelper.GetMachineCode();
                ValidateRegistrationCode(dto, machineCode, out int maxRecords);
                dto.MaxRecords = maxRecords;

                _cachedConfig = dto;
                return true;
            }
            catch (Exception ex)
            {
                LastSaveError = ex.Message;
                if (ex.InnerException != null) LastSaveError += $" ({ex.InnerException.Message})";
                System.Diagnostics.Debug.WriteLine("SaveDbConfig error: " + LastSaveError);
                return false;
            }
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private const ushort DateMask = 0x7A6C;

        /// <summary>
        /// Mã hóa ngày hết hạn YYYYMMDD thành cụm 4 ký tự hex bảo mật
        /// </summary>
        public static string EncodeDate(string expiryDate)
        {
            if (string.IsNullOrWhiteSpace(expiryDate) || expiryDate.Length != 8)
                expiryDate = "20991231";

            try
            {
                int y = int.Parse(expiryDate.Substring(0, 4)) - 2020;
                int m = int.Parse(expiryDate.Substring(4, 2));
                int d = int.Parse(expiryDate.Substring(6, 2));
                int val = ((y & 0x7F) << 9) | ((m & 0x0F) << 5) | (d & 0x1F);
                int masked = val ^ DateMask;
                return masked.ToString("X4");
            }
            catch
            {
                return "E5F3"; // 20991231
            }
        }

        /// <summary>
        /// Giải mã ngày hết hạn từ cụm 4 ký tự hex
        /// </summary>
        public static bool TryDecodeDate(string dateHex, out string expiryDateStr, out DateTime expirationDate)
        {
            expiryDateStr = "20991231";
            expirationDate = new DateTime(2099, 12, 31);
            if (string.IsNullOrWhiteSpace(dateHex) || dateHex.Length != 4)
                return false;

            try
            {
                int masked = Convert.ToUInt16(dateHex, 16);
                int val = masked ^ DateMask;
                int d = val & 0x1F;
                int m = (val >> 5) & 0x0F;
                int y = ((val >> 9) & 0x7F) + 2020;

                if (y < 2020 || y > 2120 || m < 1 || m > 12 || d < 1 || d > DateTime.DaysInMonth(y, m))
                    return false;

                expirationDate = new DateTime(y, m, d);
                expiryDateStr = $"{y:D4}{m:D2}{d:D2}";
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Thuật toán tạo Số đăng ký ngắn gọn (12 ký tự: XXXX-XXXX-XXXX)
        /// Cụm 1: Nhúng ngày hết hạn mã hóa
        /// Cụm 2 & 3: Chữ ký cryptographic băm MD5 với SecretSalt
        /// </summary>
        public static string GenerateRegistrationCode(string targetId, string expiryDate = "20991231")
        {
            targetId = (targetId ?? "").Trim().ToUpperInvariant();
            expiryDate = (expiryDate ?? "20991231").Trim();
            if (expiryDate.Length != 8) expiryDate = "20991231";

            string dateHex = EncodeDate(expiryDate);
            string raw = $"{targetId}#{expiryDate}#{SecretSalt}";

            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
                string hex = Convert.ToHexString(hash).ToUpperInvariant();

                string p1 = hex.Substring(0, 4);
                string p2 = hex.Substring(8, 4);

                return $"{dateHex}-{p1}-{p2}";
            }
        }

        /// <summary>
        /// Giải mã và xác thực Mã đăng ký 16 ký tự phân bổ hạn mức bản ghi JournalVoucher (Khớp 100% với Tool KeyGen & Google Apps Script)
        /// </summary>
        public static bool DecodeRecordLimit(string machineCode, string code, out int limit)
        {
            limit = 0;
            if (string.IsNullOrWhiteSpace(code)) return false;

            machineCode = (machineCode ?? "").Replace("-", "").Trim().ToUpperInvariant();
            string clean = code.Replace("-", "").Trim().ToUpperInvariant();
            if (clean.Length != 16) return false;

            // 1. Bóc tách 6 ký tự limit
            char[] limitChars = new char[6];
            for (int i = 0; i < LimitIndices.Length; i++)
            {
                limitChars[i] = clean[LimitIndices[i]];
            }

            try
            {
                int masked = Convert.ToInt32(new string(limitChars), 16);
                limit = (masked ^ RecordMaskVal) & 0xFFFFFF;
            }
            catch
            {
                return false;
            }

            // 2. Bóc tách 10 ký tự hash
            char[] hashChars = new char[10];
            for (int i = 0; i < HashIndices.Length; i++)
            {
                hashChars[i] = clean[HashIndices[i]];
            }
            string extractedHash = new string(hashChars);

            // 3. Tính chữ ký kỳ vọng
            string raw = $"{machineCode}#{limit}#{RecordTierSalt}";
            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(raw));
            string expectedHash = Convert.ToHexString(hashBytes).ToUpperInvariant().Substring(0, 10);

            return string.Equals(extractedHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của Số đăng ký và tự động bóc tách hạn mức số bản ghi JournalVoucher
        /// </summary>
        public static bool ValidateRegistrationCode(DbConfigDto config, string machineCode, out int maxRecords)
        {
            maxRecords = 100; // Mặc định dùng thử 100 bản ghi
            if (config == null || string.IsNullOrWhiteSpace(config.RegistrationCode))
                return false;

            string reg = config.RegistrationCode.Trim().ToUpperInvariant();
            string clean = reg.Replace("-", "");

            // 1. Mã mới 16 ký tự (XXXX-YYYY-ZZZZ-WWWW) phân bổ số lượng bản ghi JournalVoucher
            if (clean.Length == 16)
            {
                if (DecodeRecordLimit(machineCode, reg, out int limit))
                {
                    maxRecords = limit;
                    config.MaxRecords = limit;
                    config.ExpiryDate = "20991231";
                    return true;
                }
            }

            // 2. Tương thích ngược: Mã cũ 12 ký tự (XXXX-XXXX-XXXX) nhúng ngày hết hạn
            var parts = reg.Split('-');
            if (parts.Length == 3 && parts[0].Length == 4 && parts[1].Length == 4 && parts[2].Length == 4)
            {
                if (TryDecodeDate(parts[0], out string decodedExpStr, out DateTime decodedExpDate))
                {
                    string expectedCode = GenerateRegistrationCode(machineCode, decodedExpStr);
                    if (string.Equals(reg, expectedCode, StringComparison.OrdinalIgnoreCase))
                    {
                        maxRecords = 999999; // Không giới hạn bản ghi, chỉ ràng buộc ngày
                        config.MaxRecords = 999999;
                        config.ExpiryDate = decodedExpStr;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Quá tải kiểm tra tính hợp lệ theo thời hạn ngày (Tương thích ngược)
        /// </summary>
        public static bool ValidateRegistrationCode(DbConfigDto config, string machineCode, out DateTime? expirationDate)
        {
            expirationDate = null;
            if (ValidateRegistrationCode(config, machineCode, out int maxRecords))
            {
                if (DateTime.TryParseExact(config.ExpiryDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime exp))
                {
                    expirationDate = exp;
                }
                else
                {
                    expirationDate = new DateTime(2099, 12, 31);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra quy tắc hạn dùng chứng từ: Không được nhập quá ngày hết hạn bản quyền (nếu có thiết lập)
        /// </summary>
        public static bool CanUpdateVoucher(DateTime voucherDate, out string warningMessage)
        {
            warningMessage = "";
            var cfg = Current;
            string machineCode = License.HardwareHelper.GetMachineCode();

            bool isValid = ValidateRegistrationCode(cfg, machineCode, out int maxRecords);

            if (isValid && DateTime.TryParseExact(cfg.ExpiryDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime expDate))
            {
                if (expDate.Year < 2090 && voucherDate.Date > expDate.Date)
                {
                    warningMessage = $"Phiên bản có thời hạn sử dụng đến ngày {expDate:dd/MM/yyyy}.\nBạn không thể cập nhật các chứng từ có ngày ({voucherDate:dd/MM/yyyy}) lớn hơn thời điểm trên!\n\nVui lòng gia hạn bản quyền để tiếp tục làm việc.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Lấy tổng số lượng chứng từ thực tế trong bảng JournalVoucher (truy vấn trực tiếp CSDL bảo đảm chính xác 100% trên cả Access, SQL Server, PostgreSQL)
        /// </summary>
        public static int GetJournalVoucherCount()
        {
            // 1. Thử truy vấn trực tiếp bằng Raw SQL (tối ưu tốc độ, tương thích hoàn hảo mọi hệ quản trị CSDL)
            try
            {
                using (var conn = AppDataAccessAdapter.CreateRawConnection())
                {
                    if (conn.State != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        var dbType = ConfigManager.Current.DbType;
                        cmd.CommandText = dbType == DatabaseType.PostgreSql
                            ? @"SELECT COUNT(*) FROM ""JournalVoucher"""
                            : "SELECT COUNT(*) FROM [JournalVoucher]";
                        var res = cmd.ExecuteScalar();
                        if (res != null && int.TryParse(res.ToString(), out int cnt))
                        {
                            return cnt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetJournalVoucherCount raw sql error: " + ex.Message);
            }

            // 2. Dự phòng bằng LLBLGen GetDbCount
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var col = new DataAccess.HelperClasses.EntityCollection<DataAccess.EntityClasses.JournalVoucherEntity>();
                    return adapter.GetDbCount(col, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetJournalVoucherCount LLBLGen GetDbCount error: " + ex.Message);
            }

            // 3. Dự phòng bằng LLBLGen LinqMetaData
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var metaData = new DataAccess.Linq.LinqMetaData(adapter);
                    return metaData.JournalVoucher.Count();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetJournalVoucherCount Linq error: " + ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// Kiểm tra hạn mức số lượng chứng từ thực tế trong bảng JournalVoucher
        /// </summary>
        public static bool CheckRecordLimit(out int currentCount, out int maxLimit, out string warningMessage, int additionalRecords = 0)
        {
            warningMessage = "";
            var cfg = Current;
            string machineCode = License.HardwareHelper.GetMachineCode();
            bool isValid = ValidateRegistrationCode(cfg, machineCode, out int limit);
            maxLimit = isValid ? limit : 100;

            currentCount = GetJournalVoucherCount();

            if (maxLimit < 999999 && (currentCount + additionalRecords) >= maxLimit)
            {
                string tierDesc = isValid ? $"gói bản quyền ({maxLimit:N0} chứng từ)" : $"bản Dùng Thử miễn phí ({maxLimit} chứng từ)";
                if (currentCount >= maxLimit)
                {
                    warningMessage = $"Số lượng chứng từ hiện tại trong CSDL ({currentCount:N0}) đã đạt hoặc vượt mức cho phép của {tierDesc}.\n\nVui lòng nâng cấp gói bản quyền để tiếp tục nhập thêm chứng từ mới.";
                }
                else
                {
                    warningMessage = $"Số lượng chứng từ hiện tại ({currentCount:N0}) cùng với các chứng từ mới ({additionalRecords}) sẽ vượt quá giới hạn của {tierDesc}.\n\nVui lòng nâng cấp gói bản quyền để lưu thêm chứng từ mới.";
                }
                return false;
            }

            return true;
        }
    }
}

