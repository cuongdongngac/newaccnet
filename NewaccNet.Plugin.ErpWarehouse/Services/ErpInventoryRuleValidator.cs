using System;
using System.Collections.Generic;
using NewaccNet.Plugin.ErpWarehouse.Models;

namespace NewaccNet.Plugin.ErpWarehouse.Services
{
    /// <summary>
    /// Bộ quy tắc nghiệp vụ chuyên biệt dành cho Cổng nạp ERP Kho & Điều chuyển nội bộ.
    /// Tách bạch hoàn toàn khỏi các rule rườm rà của màn hình nhập liệu thông thường.
    /// </summary>
    public static class ErpInventoryRuleValidator
    {
        public static ErpExecutionResult Validate(ErpInventoryVoucherDto voucher)
        {
            var errors = new List<string>();

            if (voucher == null)
            {
                return ErpExecutionResult.Failed("Dữ liệu chứng từ ERP bị null.");
            }

            // 1. Kiểm tra ngày chứng từ
            if (voucher.VoucherDate == DateTime.MinValue || voucher.VoucherDate.Year < 1990 || voucher.VoucherDate.Year > 2099)
            {
                errors.Add("Ngày chứng từ không hợp lệ.");
            }

            // 2. Kiểm tra danh sách mặt hàng
            if (voucher.Items == null || voucher.Items.Count == 0)
            {
                errors.Add("Phiếu kho bắt buộc phải có ít nhất một dòng mặt hàng/vật tư.");
            }
            else
            {
                int lineIndex = 1;
                foreach (var item in voucher.Items)
                {
                    if (string.IsNullOrWhiteSpace(item.MaterialId))
                    {
                        errors.Add($"Dòng {lineIndex}: Chưa có mã vật tư/hàng hóa (MaterialId).");
                    }

                    if (item.Quantity <= 0)
                    {
                        errors.Add($"Dòng {lineIndex} ({item.MaterialId}): Số lượng phải lớn hơn 0 (hiện tại: {item.Quantity}).");
                    }

                    if (string.IsNullOrWhiteSpace(item.DebitAccount))
                    {
                        errors.Add($"Dòng {lineIndex} ({item.MaterialId}): Chưa có Tài khoản Nợ.");
                    }

                    if (string.IsNullOrWhiteSpace(item.CreditAccount))
                    {
                        errors.Add($"Dòng {lineIndex} ({item.MaterialId}): Chưa có Tài khoản Có.");
                    }

                    lineIndex++;
                }
            }

            // 3. Quy tắc riêng cho từng loại nghiệp vụ kho
            switch (voucher.VoucherType)
            {
                case ErpInventoryVoucherType.Transfer:
                    // ĐIỀU CHUYỂN KHO NỘI BỘ
                    if (string.IsNullOrWhiteSpace(voucher.FromWarehouseId))
                    {
                        errors.Add("Nghiệp vụ Chuyển kho bắt buộc phải có Kho xuất (FromWarehouseId).");
                    }

                    if (string.IsNullOrWhiteSpace(voucher.ToWarehouseId))
                    {
                        errors.Add("Nghiệp vụ Chuyển kho bắt buộc phải có Kho nhập (ToWarehouseId).");
                    }

                    if (!string.IsNullOrWhiteSpace(voucher.FromWarehouseId) &&
                        !string.IsNullOrWhiteSpace(voucher.ToWarehouseId) &&
                        string.Equals(voucher.FromWarehouseId.Trim(), voucher.ToWarehouseId.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add("Kho xuất và Kho nhập không được trùng nhau.");
                    }

                    // ĐẶC THÙ: Chuyển kho nội bộ CHO PHÉP DebitAccount == CreditAccount (152 đối ứng 152, 156 đối ứng 156)
                    // và KHÔNG bắt buộc đối tác ngoài!
                    break;

                case ErpInventoryVoucherType.PurchaseReceipt:
                    if (string.IsNullOrWhiteSpace(voucher.ToWarehouseId))
                    {
                        errors.Add("Phiếu Nhập mua bắt buộc phải có Kho nhập.");
                    }
                    break;

                case ErpInventoryVoucherType.SalesDelivery:
                case ErpInventoryVoucherType.ProductionOrExpenseIssue:
                    if (string.IsNullOrWhiteSpace(voucher.FromWarehouseId))
                    {
                        errors.Add("Phiếu Xuất kho bắt buộc phải có Kho xuất.");
                    }
                    break;
            }

            return errors.Count > 0 ? ErpExecutionResult.Failed(errors) : ErpExecutionResult.Success(0, voucher.VoucherNo);
        }
    }
}
