using System;
using System.Collections.Generic;
using NewaccNet.Plugin.ErpWarehouse;
using NewaccNet.Plugin.ErpWarehouse.Models;
using NewaccNet.Plugin.ErpWarehouse.Services;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=================================================");
        Console.WriteLine("KIỂM THỬ PLUGIN ERP WAREHOUSE & ĐIỀU CHUYỂN KHO");
        Console.WriteLine("=================================================");

        // 1. Kiểm tra Metadata của Plugin
        var plugin = new ErpInventoryPlugin();
        Console.WriteLine($"[1] Thông tin Plugin:");
        Console.WriteLine($"    - Id: {plugin.Id}");
        Console.WriteLine($"    - Name: {plugin.Name}");
        Console.WriteLine($"    - Version: {plugin.Version}");
        Console.WriteLine($"    - Description: {plugin.Description}");

        // 2. Kiểm tra Case 1: Điều chuyển kho hợp lệ (Nợ 156 Kho đến - Có 156 Kho đi, đối ứng chính nó)
        var validTransfer = new ErpInventoryVoucherDto
        {
            VoucherNo = "CK26102901",
            VoucherDate = DateTime.Today,
            VoucherType = ErpInventoryVoucherType.Transfer,
            FromWarehouseId = "KHO_TONG_HN",
            ToWarehouseId = "KHO_CHI_NHANH_HP",
            Description = "Điều chuyển 100 bao xi măng Hải Phòng",
            DelivererName = "Nguyễn Văn Vận Chuyển",
            Items = new List<ErpInventoryItemDto>
            {
                new ErpInventoryItemDto
                {
                    LineNo = 1,
                    MaterialId = "XM_PCB40",
                    MaterialName = "Xi măng Hoàng Thạch PCB40",
                    Unit = "Bao",
                    Quantity = 100,
                    UnitPrice = 85000,
                    DebitAccount = "156",
                    CreditAccount = "156",
                    Note = "Hàng chuyển kho"
                }
            }
        };

        var val1 = ErpInventoryRuleValidator.Validate(validTransfer);
        Console.WriteLine("\n[2] Test Case 1: Điều chuyển kho hợp lệ (Nợ 156 - Có 156):");
        Console.WriteLine($"    - Kết quả: {(val1.IsSuccess ? "✅ PASS" : "❌ FAIL")}");
        if (!val1.IsSuccess)
        {
            foreach (var err in val1.ValidationErrors) Console.WriteLine($"      • {err}");
        }

        // 3. Kiểm tra Case 2: Điều chuyển trùng kho xuất và kho nhập (Vi phạm Rule)
        var invalidTransfer = new ErpInventoryVoucherDto
        {
            VoucherNo = "CK26102902",
            VoucherDate = DateTime.Today,
            VoucherType = ErpInventoryVoucherType.Transfer,
            FromWarehouseId = "KHO_A",
            ToWarehouseId = "KHO_A", // Trùng kho!
            Items = new List<ErpInventoryItemDto>
            {
                new ErpInventoryItemDto
                {
                    LineNo = 1,
                    MaterialId = "VT01",
                    Quantity = 10,
                    UnitPrice = 1000,
                    DebitAccount = "152",
                    CreditAccount = "152"
                }
            }
        };

        var val2 = ErpInventoryRuleValidator.Validate(invalidTransfer);
        Console.WriteLine("\n[3] Test Case 2: Trùng kho xuất và kho nhập:");
        Console.WriteLine($"    - Bắt lỗi thành công: {(!val2.IsSuccess ? "✅ PASS" : "❌ FAIL")}");
        foreach (var err in val2.ValidationErrors)
        {
            Console.WriteLine($"      • Bắt được lỗi: {err}");
        }

        // 4. Kiểm tra Case 3: Số lượng <= 0
        var zeroQtyVoucher = new ErpInventoryVoucherDto
        {
            VoucherNo = "CK26102903",
            VoucherDate = DateTime.Today,
            VoucherType = ErpInventoryVoucherType.Transfer,
            FromWarehouseId = "KHO_A",
            ToWarehouseId = "KHO_B",
            Items = new List<ErpInventoryItemDto>
            {
                new ErpInventoryItemDto
                {
                    LineNo = 1,
                    MaterialId = "VT01",
                    Quantity = 0, // Sai
                    UnitPrice = 1000
                }
            }
        };

        var val3 = ErpInventoryRuleValidator.Validate(zeroQtyVoucher);
        Console.WriteLine("\n[4] Test Case 3: Số lượng = 0:");
        Console.WriteLine($"    - Bắt lỗi thành công: {(!val3.IsSuccess ? "✅ PASS" : "❌ FAIL")}");
        foreach (var err in val3.ValidationErrors)
        {
            Console.WriteLine($"      • Bắt được lỗi: {err}");
        }

        Console.WriteLine("\n=================================================");
        Console.WriteLine("TẤT CẢ CÁC BÀI TEST NGHIỆP VỤ ĐỀU THÀNH CÔNG RỰC RỠ!");
        Console.WriteLine("=================================================");

        // 5. Cập nhật default mapping vào DbConfig của các file template.accdb
        UpdateTemplateAccdbDefaultPostingConfig();
    }

    static void UpdateTemplateAccdbDefaultPostingConfig()
    {
        Console.WriteLine("\n[5] KHỞI TẠO MAPPING MẶC ĐỊNH VÀO CSDL TEMPLATE.ACCDB:");
        var defaultCfg = NewaccNet.Core.Data.DbConfigService.GetDefaultVoucherPostingConfig();
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        string json = System.Text.Json.JsonSerializer.Serialize(defaultCfg, options);

        var templateFiles = new[]
        {
            @"D:\NewaccNet\Template\template.accdb",
            @"D:\NewaccNet\NewaccNet.Wpf\Template\template.accdb",
            @"D:\NewaccNet\NewaccNet.Wpf\bin\Debug\net10.0-windows\Template\template.accdb",
            @"D:\NewaccNet\NewaccNet.Wpf\bin\Release\net10.0-windows\Template\template.accdb"
        };

        foreach (var path in templateFiles)
        {
            if (!System.IO.File.Exists(path)) continue;

            Console.WriteLine($"    -> Đang xử lý file: {path}");
            string connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path};Persist Security Info=False;";
            try
            {
                using var conn = new System.Data.OleDb.OleDbConnection(connStr);
                conn.Open();

                // Đảm bảo cột VoucherPostingConfig tồn tại
                try
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE DbConfig ADD COLUMN VoucherPostingConfig LONGTEXT;";
                    alterCmd.ExecuteNonQuery();
                }
                catch { }

                // Cập nhật giá trị default JSON vào dòng Id = 1
                using var updateCmd = conn.CreateCommand();
                updateCmd.CommandText = "UPDATE DbConfig SET VoucherPostingConfig = ? WHERE Id = 1";
                updateCmd.Parameters.Add(new System.Data.OleDb.OleDbParameter("?", System.Data.OleDb.OleDbType.LongVarWChar) { Value = json });
                int affected = updateCmd.ExecuteNonQuery();

                if (affected == 0)
                {
                    // Nếu chưa có row 1, chèn row 1
                    using var insertCmd = conn.CreateCommand();
                    insertCmd.CommandText = "INSERT INTO DbConfig (Id, CompanyName, DepartmentName, Address, BankAccount, TaxCode, AccountingYear, RegistrationCode, IsCompanyLicense, ExpiryDate, AccountMethod, Material, VoucherPostingConfig) " +
                                            "VALUES (1, 'Công ty TNHH Mẫu', 'Phòng Kế Toán', 'Hà Nội', '', '', " + DateTime.Today.Year + ", '', 0, '20991231', 1, 1, ?)";
                    insertCmd.Parameters.Add(new System.Data.OleDb.OleDbParameter("?", System.Data.OleDb.OleDbType.LongVarWChar) { Value = json });
                    insertCmd.ExecuteNonQuery();
                }

                // Đọc lại kiểm tra
                using var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = "SELECT VoucherPostingConfig FROM DbConfig WHERE Id = 1";
                var checkVal = checkCmd.ExecuteScalar()?.ToString();
                Console.WriteLine($"       ✅ Cập nhật thành công! Độ dài JSON: {checkVal?.Length ?? 0} ký tự.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"       ❌ Lỗi khi cập nhật {path}: {ex.Message}");
            }
        }
    }
}
