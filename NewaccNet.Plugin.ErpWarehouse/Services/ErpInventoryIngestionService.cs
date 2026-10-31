using System;
using System.Linq;
using DataAccess.EntityClasses;
using NewaccNet.Core.Data;
using NewaccNet.Plugin.ErpWarehouse.Models;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Plugin.ErpWarehouse.Services
{
    /// <summary>
    /// Dịch vụ xử lý nạp chứng từ kho ERP chạy ngầm (Headless Ingestion Pipeline).
    /// Áp dụng đầy đủ quy tắc kế toán và lưu thẳng vào CSDL qua LLBLGen Pro.
    /// </summary>
    public class ErpInventoryIngestionService
    {
        /// <summary>
        /// Nạp ngầm một chứng từ kho từ ERP hoặc AI vào CSDL.
        /// </summary>
        public static ErpExecutionResult ProcessSilently(ErpInventoryVoucherDto dto, IDataAccessAdapter? externalAdapter = null)
        {
            // 1. Kiểm tra toàn bộ quy tắc nghiệp vụ
            var valResult = ErpInventoryRuleValidator.Validate(dto);
            if (!valResult.IsSuccess)
            {
                return valResult;
            }

            IDataAccessAdapter? localAdapter = null;
            var adapter = externalAdapter;
            if (adapter == null)
            {
                localAdapter = AppDataAccessAdapter.Create();
                adapter = localAdapter;
            }

            try
            {
                // 2. Tự sinh số chứng từ nếu chưa có
                string prefix = dto.VoucherType switch
                {
                    ErpInventoryVoucherType.Transfer => "CK",
                    ErpInventoryVoucherType.PurchaseReceipt => "PN",
                    ErpInventoryVoucherType.SalesDelivery => "PX",
                    ErpInventoryVoucherType.ProductionOrExpenseIssue => "XD",
                    _ => "PK"
                };

                string voucherNo = string.IsNullOrWhiteSpace(dto.VoucherNo)
                    ? $"{prefix}{DateTime.Now:yyMMddHHmmss}"
                    : dto.VoucherNo.Trim();

                string defaultDesc = dto.VoucherType switch
                {
                    ErpInventoryVoucherType.Transfer => $"Điều chuyển kho từ {dto.FromWarehouseId} sang {dto.ToWarehouseId}",
                    ErpInventoryVoucherType.PurchaseReceipt => $"Nhập mua hàng hóa/vật tư vào {dto.ToWarehouseId}",
                    ErpInventoryVoucherType.SalesDelivery => $"Xuất bán hàng hóa từ {dto.FromWarehouseId}",
                    ErpInventoryVoucherType.ProductionOrExpenseIssue => $"Xuất kho dùng cho sản xuất/chi phí từ {dto.FromWarehouseId}",
                    _ => "Phiếu biến động kho ERP"
                };

                string description = string.IsNullOrWhiteSpace(dto.Description) ? defaultDesc : dto.Description.Trim();

                // 3. Khởi tạo Thực thể Chứng từ kế toán (JournalVoucherEntity)
                var jv = new JournalVoucherEntity
                {
                    VoucherNo = voucherNo,
                    VoucherDate = dto.VoucherDate,
                    Contents = description,
                    Personname = string.IsNullOrWhiteSpace(dto.DelivererName) ? dto.ReceiverName : dto.DelivererName,
                    Bookflag = true
                };

                // 4. Xử lý các dòng định khoản và phiếu kho tương ứng
                if (dto.VoucherType == ErpInventoryVoucherType.Transfer)
                {
                    BuildTransferEntries(jv, dto, voucherNo);
                }
                else if (dto.VoucherType == ErpInventoryVoucherType.PurchaseReceipt)
                {
                    BuildPurchaseReceiptEntries(jv, dto, voucherNo);
                }
                else if (dto.VoucherType == ErpInventoryVoucherType.SalesDelivery)
                {
                    BuildSalesDeliveryEntries(jv, dto, voucherNo);
                }
                else
                {
                    BuildGeneralIssueEntries(jv, dto, voucherNo);
                }

                // 5. Lưu toàn bộ đồ thị thực thể vào CSDL an toàn 100%
                bool saved = adapter.SaveEntity(jv, true, true);
                if (saved)
                {
                    return ErpExecutionResult.Success(jv.Id, jv.VoucherNo, $"Đã lưu chứng từ {jv.VoucherNo} vào sổ kế toán thành công.");
                }
                else
                {
                    return ErpExecutionResult.Failed("Lưu chứng từ vào CSDL thất bại (adapter.SaveEntity trả về false).");
                }
            }
            catch (Exception ex)
            {
                return ErpExecutionResult.Failed($"Lỗi ngoại lệ khi lưu CSDL: {ex.Message}");
            }
            finally
            {
                localAdapter?.Dispose();
            }
        }

        /// <summary>
        /// Tạo cặp định khoản điều chuyển kho: Nợ 152/156 (Kho nhận) - Có 152/156 (Kho xuất)
        /// Cùng tài khoản vật tư đối ứng, 2 phiếu kho 2 bên.
        /// </summary>
        private static void BuildTransferEntries(JournalVoucherEntity jv, ErpInventoryVoucherDto dto, string voucherNo)
        {
            double totalAmount = dto.TotalAmount;
            string primaryAccount = dto.Items.FirstOrDefault()?.DebitAccount ?? "156";

            // Dòng Xuất kho A (Có TK Vật tư)
            var issueEntry = new JournalEntryEntity
            {
                Dbcr = -1, // Có
                AccountId = primaryAccount,
                Amount = totalAmount
            };

            // Gắn Phiếu kho xuất (Kho A)
            var issueIv = new InventoryVoucherEntity
            {
                WarehouseId = dto.FromWarehouseId,
                Inoutnumber = $"{voucherNo}-X",
                VoucherType = "XUAT_CHUYEN_KHO",
                Vatmaterialdes = $"Xuất chuyển sang kho {dto.ToWarehouseId}"
            };
            foreach (var item in dto.Items)
            {
                issueIv.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }
            issueEntry.InventoryVouchers.Add(issueIv);
            jv.JournalEntries.Add(issueEntry);

            // Dòng Nhập kho B (Nợ TK Vật tư - đối ứng chính nó!)
            var receiptEntry = new JournalEntryEntity
            {
                Dbcr = 1, // Nợ
                AccountId = primaryAccount,
                Amount = totalAmount,
                ParentEntry = issueEntry
            };

            // Gắn Phiếu kho nhập (Kho B)
            var receiptIv = new InventoryVoucherEntity
            {
                WarehouseId = dto.ToWarehouseId,
                Inoutnumber = $"{voucherNo}-N",
                VoucherType = "NHAP_CHUYEN_KHO",
                Vatmaterialdes = $"Nhập chuyển từ kho {dto.FromWarehouseId}"
            };
            foreach (var item in dto.Items)
            {
                receiptIv.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }
            receiptEntry.InventoryVouchers.Add(receiptIv);
            issueEntry.SubEntries.Add(receiptEntry);
            jv.JournalEntries.Add(receiptEntry);
        }

        private static void BuildPurchaseReceiptEntries(JournalVoucherEntity jv, ErpInventoryVoucherDto dto, string voucherNo)
        {
            double totalAmount = dto.TotalAmount;
            string creditAcc = dto.Items.FirstOrDefault()?.CreditAccount ?? "331";
            string debitAcc = dto.Items.FirstOrDefault()?.DebitAccount ?? "156";

            var payMaster = new JournalEntryEntity
            {
                Dbcr = -1, // Có
                AccountId = creditAcc,
                Amount = totalAmount
            };
            jv.JournalEntries.Add(payMaster);

            var goodsEntry = new JournalEntryEntity
            {
                Dbcr = 1, // Nợ
                AccountId = debitAcc,
                Amount = totalAmount,
                ParentEntry = payMaster
            };

            var iv = new InventoryVoucherEntity
            {
                WarehouseId = dto.ToWarehouseId,
                Inoutnumber = voucherNo,
                VoucherType = "NHAP_MUA_HANG",
                Vatobjectid = dto.PartnerId,
                Vatobjectname = dto.PartnerName
            };
            foreach (var item in dto.Items)
            {
                iv.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }
            goodsEntry.InventoryVouchers.Add(iv);
            payMaster.SubEntries.Add(goodsEntry);
            jv.JournalEntries.Add(goodsEntry);
        }

        private static void BuildSalesDeliveryEntries(JournalVoucherEntity jv, ErpInventoryVoucherDto dto, string voucherNo)
        {
            double totalAmount = dto.TotalAmount;
            string debitAcc = "632";
            string creditAcc = dto.Items.FirstOrDefault()?.CreditAccount ?? "156";

            var costMaster = new JournalEntryEntity
            {
                Dbcr = 1, // Nợ Giá vốn
                AccountId = debitAcc,
                Amount = totalAmount
            };
            jv.JournalEntries.Add(costMaster);

            var ivEntry = new JournalEntryEntity
            {
                Dbcr = -1, // Có Kho
                AccountId = creditAcc,
                Amount = totalAmount,
                ParentEntry = costMaster
            };

            var iv = new InventoryVoucherEntity
            {
                WarehouseId = dto.FromWarehouseId,
                Inoutnumber = voucherNo,
                VoucherType = "XUAT_BAN_HANG",
                Vatobjectid = dto.PartnerId,
                Vatobjectname = dto.PartnerName
            };
            foreach (var item in dto.Items)
            {
                iv.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }
            ivEntry.InventoryVouchers.Add(iv);
            costMaster.SubEntries.Add(ivEntry);
            jv.JournalEntries.Add(ivEntry);
        }

        private static void BuildGeneralIssueEntries(JournalVoucherEntity jv, ErpInventoryVoucherDto dto, string voucherNo)
        {
            double totalAmount = dto.TotalAmount;
            string debitAcc = dto.Items.FirstOrDefault()?.DebitAccount ?? "642";
            string creditAcc = dto.Items.FirstOrDefault()?.CreditAccount ?? "152";

            var expenseMaster = new JournalEntryEntity
            {
                Dbcr = 1,
                AccountId = debitAcc,
                Amount = totalAmount
            };
            jv.JournalEntries.Add(expenseMaster);

            var ivEntry = new JournalEntryEntity
            {
                Dbcr = -1,
                AccountId = creditAcc,
                Amount = totalAmount,
                ParentEntry = expenseMaster
            };

            var iv = new InventoryVoucherEntity
            {
                WarehouseId = dto.FromWarehouseId,
                Inoutnumber = voucherNo,
                VoucherType = "XUAT_KHO_CHI_PHI"
            };
            foreach (var item in dto.Items)
            {
                iv.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                {
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }
            ivEntry.InventoryVouchers.Add(iv);
            expenseMaster.SubEntries.Add(ivEntry);
            jv.JournalEntries.Add(ivEntry);
        }
    }
}
