using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.BalanceTransfer
{
    public class BalanceTransferService : IBalanceTransferService
    {
        public async Task<bool> TestDestinationConnectionAsync(AppConfig destinationConfig)
        {
            if (destinationConfig == null) return false;

            return await Task.Run(() =>
            {
                try
                {
                    if (destinationConfig.DbType == DatabaseType.Access)
                    {
                        if (string.IsNullOrWhiteSpace(destinationConfig.AccessFilePath) ||
                            !System.IO.File.Exists(destinationConfig.AccessFilePath))
                        {
                            return false;
                        }

                        string connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={destinationConfig.AccessFilePath}";
                        using (var conn = new System.Data.OleDb.OleDbConnection(connStr))
                        {
                            conn.Open();
                            return true;
                        }
                    }
                    else if (destinationConfig.DbType == DatabaseType.SqlServer)
                    {
                        string connStr = destinationConfig.SqlIntegratedSecurity
                            ? $"data source={destinationConfig.SqlServerName};initial catalog={destinationConfig.SqlDatabaseName};Integrated Security=SSPI;TrustServerCertificate=True;"
                            : $"data source={destinationConfig.SqlServerName};initial catalog={destinationConfig.SqlDatabaseName};User ID={destinationConfig.SqlUsername};Password={destinationConfig.SqlPassword};TrustServerCertificate=True;";

                        using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connStr))
                        {
                            conn.Open();
                            return true;
                        }
                    }
                    else if (destinationConfig.DbType == DatabaseType.PostgreSql)
                    {
                        string connStr = $"Host={destinationConfig.PgHost};Port={destinationConfig.PgPort};Database={destinationConfig.PgDatabase};Username={destinationConfig.PgUsername};Password={destinationConfig.PgPassword};";
                        using (var conn = new Npgsql.NpgsqlConnection(connStr))
                        {
                            conn.Open();
                            return true;
                        }
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            });
        }

        // =========================================================================================
        // BƯỚC TIÊN QUYẾT: ĐỒNG BỘ CÁC BẢNG THƯ VIỆN / DANH MỤC (MASTER DATA)
        // Riêng SystemUser: Update cho tài khoản 'admin', Insert cho các tài khoản mới
        // =========================================================================================
        public async Task TransferDictionariesAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 8,
                StepTitle = "Tiền đề: Đồng bộ các bảng Thư viện / Danh mục sang CSDL mới...",
                Message = "[BẮT ĐẦU] Sao chép các danh mục hệ thống sang CSDL đích để tránh lỗi ràng buộc khóa ngoại..."
            });

            await Task.Run(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    // 1. NGƯỜI DÙNG HỆ THỐNG (SystemUser)
                    try
                    {
                        var srcUsers = new EntityCollection<SystemUserEntity>();
                        srcAdapter.FetchEntityCollection(srcUsers, null);

                        var destUsers = new EntityCollection<SystemUserEntity>();
                        destAdapter.FetchEntityCollection(destUsers, null);

                        int userUpdated = 0;
                        int userInserted = 0;

                        foreach (var su in srcUsers)
                        {
                            var existingDest = destUsers.FirstOrDefault(u => string.Equals(u.Username?.Trim(), su.Username?.Trim(), StringComparison.OrdinalIgnoreCase));
                            if (existingDest != null)
                            {
                                // Đã tồn tại (đặc biệt là 'admin'): Cập nhật mật khẩu và thông tin thực tế từ nguồn
                                existingDest.Password = su.Password;
                                existingDest.FullName = su.FullName;
                                existingDest.RoleMask = su.RoleMask;
                                existingDest.IsActive = su.IsActive;
                                destAdapter.SaveEntity(existingDest, true);
                                userUpdated++;
                            }
                            else
                            {
                                // Chưa có: Thêm mới
                                var nu = new SystemUserEntity
                                {
                                    Username = su.Username,
                                    Password = su.Password,
                                    FullName = su.FullName,
                                    RoleMask = su.RoleMask,
                                    IsActive = su.IsActive
                                };
                                destAdapter.SaveEntity(nu, true);
                                userInserted++;
                            }
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 10,
                            Message = $"[DANH MỤC] Người dùng hệ thống: Đã cập nhật {userUpdated} user (gồm admin), thêm mới {userInserted} user."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ SystemUser: {ex.Message}" });
                    }

                    // 2. DANH MỤC TÀI KHOẢN (ChartOfAccount)
                    try
                    {
                        var srcAccounts = new EntityCollection<ChartOfAccountEntity>();
                        srcAdapter.FetchEntityCollection(srcAccounts, null);

                        var destAccounts = new EntityCollection<ChartOfAccountEntity>();
                        destAdapter.FetchEntityCollection(destAccounts, null);

                        var destAccIds = destAccounts.Select(a => a.AccountId?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        // Sắp xếp cha trước con sau theo độ dài mã tài khoản
                        var missingAccounts = srcAccounts
                            .Where(a => !string.IsNullOrEmpty(a.AccountId) && !destAccIds.Contains(a.AccountId.Trim()))
                            .OrderBy(a => a.AccountId.Length)
                            .ToList();

                        var newAccList = new EntityCollection<ChartOfAccountEntity>();
                        foreach (var a in missingAccounts)
                        {
                            newAccList.Add(new ChartOfAccountEntity
                            {
                                AccountId = a.AccountId.Trim(),
                                AccountName = a.AccountName,
                                CategoryId = a.CategoryId,
                                ParentId = a.ParentId,
                                Taxflag = a.Taxflag
                            });
                        }

                        if (newAccList.Count > 0)
                        {
                            destAdapter.SaveEntityCollection(newAccList, true, false);
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 12,
                            Message = $"[DANH MỤC] Hệ thống tài khoản: Đã bổ sung {newAccList.Count} tài khoản kế toán sang CSDL mới."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ ChartOfAccount: {ex.Message}" });
                    }

                    // 3. DANH MỤC ĐỐI TÁC (Partner)
                    try
                    {
                        var srcPartners = new EntityCollection<PartnerEntity>();
                        srcAdapter.FetchEntityCollection(srcPartners, null);

                        var destPartners = new EntityCollection<PartnerEntity>();
                        destAdapter.FetchEntityCollection(destPartners, null);

                        var destPartnerIds = destPartners.Select(p => p.Id?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newPartners = new EntityCollection<PartnerEntity>();
                        foreach (var p in srcPartners.Where(p => !string.IsNullOrEmpty(p.Id) && !destPartnerIds.Contains(p.Id.Trim())))
                        {
                            newPartners.Add(new PartnerEntity
                            {
                                Id = p.Id.Trim(),
                                CustomerName = p.CustomerName,
                                Address = p.Address,
                                Taxno = p.Taxno,
                                Phonenumber = p.Phonenumber
                            });
                        }

                        if (newPartners.Count > 0)
                        {
                            destAdapter.SaveEntityCollection(newPartners, true, false);
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 14,
                            Message = $"[DANH MỤC] Khách hàng & Nhà cung cấp: Đã bổ sung {newPartners.Count} đối tác sang CSDL mới."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Partner: {ex.Message}" });
                    }

                    // 4. DANH MỤC KHO & VẬT TƯ (Category, Warehouse, InventoryItem)
                    try
                    {
                        // Kho
                        var srcWh = new EntityCollection<WarehouseEntity>();
                        srcAdapter.FetchEntityCollection(srcWh, null);
                        var destWh = new EntityCollection<WarehouseEntity>();
                        destAdapter.FetchEntityCollection(destWh, null);
                        var destWhIds = destWh.Select(w => w.WarehouseId?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newWhList = new EntityCollection<WarehouseEntity>();
                        foreach (var w in srcWh.Where(w => !string.IsNullOrEmpty(w.WarehouseId) && !destWhIds.Contains(w.WarehouseId.Trim())))
                        {
                            newWhList.Add(new WarehouseEntity { WarehouseId = w.WarehouseId.Trim(), WarehouseName = w.WarehouseName });
                        }
                        if (newWhList.Count > 0) destAdapter.SaveEntityCollection(newWhList, true, false);

                        // Nhóm vật tư
                        var srcCat = new EntityCollection<CategoryEntity>();
                        srcAdapter.FetchEntityCollection(srcCat, null);
                        var destCat = new EntityCollection<CategoryEntity>();
                        destAdapter.FetchEntityCollection(destCat, null);
                        var destCatIds = destCat.Select(c => c.Categoryid?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newCatList = new EntityCollection<CategoryEntity>();
                        foreach (var c in srcCat.Where(c => !string.IsNullOrEmpty(c.Categoryid) && !destCatIds.Contains(c.Categoryid.Trim())))
                        {
                            newCatList.Add(new CategoryEntity { Categoryid = c.Categoryid.Trim(), Categoryname = c.Categoryname });
                        }
                        if (newCatList.Count > 0) destAdapter.SaveEntityCollection(newCatList, true, false);

                        // Vật tư hàng hóa
                        var srcItems = new EntityCollection<InventoryItemEntity>();
                        srcAdapter.FetchEntityCollection(srcItems, null);
                        var destItems = new EntityCollection<InventoryItemEntity>();
                        destAdapter.FetchEntityCollection(destItems, null);
                        var destItemIds = destItems.Select(i => i.MaterialId?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newItems = new EntityCollection<InventoryItemEntity>();
                        foreach (var itm in srcItems.Where(i => !string.IsNullOrEmpty(i.MaterialId) && !destItemIds.Contains(i.MaterialId.Trim())))
                        {
                            newItems.Add(new InventoryItemEntity
                            {
                                MaterialId = itm.MaterialId.Trim(),
                                MaterialName = itm.MaterialName,
                                Unit = itm.Unit,
                                Categoryid = itm.Categoryid
                            });
                        }
                        if (newItems.Count > 0) destAdapter.SaveEntityCollection(newItems, true, false);

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 16,
                            Message = $"[DANH MỤC] Kho & Vật tư: Bổ sung {newWhList.Count} kho, {newCatList.Count} nhóm, {newItems.Count} mã vật tư."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Kho & Vật tư: {ex.Message}" });
                    }

                    // 5. DANH MỤC TÀI SẢN CỐ ĐỊNH (Department, Reason, Source, Asset)
                    try
                    {
                        var srcAsset = new EntityCollection<AssetEntity>();
                        srcAdapter.FetchEntityCollection(srcAsset, null);
                        var destAsset = new EntityCollection<AssetEntity>();
                        destAdapter.FetchEntityCollection(destAsset, null);
                        var destAssetIds = destAsset.Select(a => a.AssetId?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newAssets = new EntityCollection<AssetEntity>();
                        foreach (var a in srcAsset.Where(a => !string.IsNullOrEmpty(a.AssetId) && !destAssetIds.Contains(a.AssetId.Trim())))
                        {
                            newAssets.Add(new AssetEntity
                            {
                                AssetId = a.AssetId.Trim(),
                                AssetName = a.AssetName,
                                Unit = a.Unit
                            });
                        }
                        if (newAssets.Count > 0) destAdapter.SaveEntityCollection(newAssets, true, false);

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 18,
                            Message = $"[DANH MỤC] Tài sản cố định: Bổ sung {newAssets.Count} danh mục tài sản."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Tài sản cố định: {ex.Message}" });
                    }

                    // 6. DANH MỤC CHI PHÍ (CostObject, CostElement)
                    try
                    {
                        var srcCo = new EntityCollection<CostObjectEntity>();
                        srcAdapter.FetchEntityCollection(srcCo, null);
                        var destCo = new EntityCollection<CostObjectEntity>();
                        destAdapter.FetchEntityCollection(destCo, null);
                        var destCoIds = destCo.Select(c => c.Id?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newCoList = new EntityCollection<CostObjectEntity>();
                        foreach (var co in srcCo.Where(c => !string.IsNullOrEmpty(c.Id) && !destCoIds.Contains(c.Id.Trim())))
                        {
                            newCoList.Add(new CostObjectEntity { Id = co.Id.Trim(), ObjectName = co.ObjectName });
                        }
                        if (newCoList.Count > 0) destAdapter.SaveEntityCollection(newCoList, true, false);

                        var srcCe = new EntityCollection<CostElementEntity>();
                        srcAdapter.FetchEntityCollection(srcCe, null);
                        var destCe = new EntityCollection<CostElementEntity>();
                        destAdapter.FetchEntityCollection(destCe, null);
                        var destCeIds = destCe.Select(c => c.Id?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newCeList = new EntityCollection<CostElementEntity>();
                        foreach (var ce in srcCe.Where(c => !string.IsNullOrEmpty(c.Id) && !destCeIds.Contains(c.Id.Trim())))
                        {
                            newCeList.Add(new CostElementEntity { Id = ce.Id.Trim(), ElementName = ce.ElementName });
                        }
                        if (newCeList.Count > 0) destAdapter.SaveEntityCollection(newCeList, true, false);

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 20,
                            Message = $"[DANH MỤC] Chi phí: Bổ sung {newCoList.Count} đối tượng THCP, {newCeList.Count} khoản mục chi phí."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Chi phí: {ex.Message}" });
                    }

                    // 7. DANH MỤC CỔ PHIẾU (StockType, Stock)
                    try
                    {
                        var srcStk = new EntityCollection<StockEntity>();
                        srcAdapter.FetchEntityCollection(srcStk, null);
                        var destStk = new EntityCollection<StockEntity>();
                        destAdapter.FetchEntityCollection(destStk, null);
                        var destStkIds = destStk.Select(s => s.Stockid?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newStocks = new EntityCollection<StockEntity>();
                        foreach (var s in srcStk.Where(s => !string.IsNullOrEmpty(s.Stockid) && !destStkIds.Contains(s.Stockid.Trim())))
                        {
                            newStocks.Add(new StockEntity
                            {
                                Stockid = s.Stockid.Trim(),
                                Description = s.Description,
                                Stocktypeid = s.Stocktypeid
                            });
                        }
                        if (newStocks.Count > 0) destAdapter.SaveEntityCollection(newStocks, true, false);

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 22,
                            Message = $"[DANH MỤC] Cổ phiếu: Bổ sung {newStocks.Count} mã chứng khoán sang CSDL mới."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Cổ phiếu: {ex.Message}" });
                    }

                    // 8. DANH MỤC NGOẠI TỆ (Currency)
                    try
                    {
                        var srcCur = new EntityCollection<CurrencyEntity>();
                        srcAdapter.FetchEntityCollection(srcCur, null);
                        var destCur = new EntityCollection<CurrencyEntity>();
                        destAdapter.FetchEntityCollection(destCur, null);
                        var destCurIds = destCur.Select(c => c.CurrencyId?.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var newCurs = new EntityCollection<CurrencyEntity>();
                        foreach (var c in srcCur.Where(c => !string.IsNullOrEmpty(c.CurrencyId) && !destCurIds.Contains(c.CurrencyId.Trim())))
                        {
                            newCurs.Add(new CurrencyEntity { CurrencyId = c.CurrencyId.Trim(), CurrencyName = c.CurrencyName });
                        }
                        if (newCurs.Count > 0) destAdapter.SaveEntityCollection(newCurs, true, false);

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 24,
                            Message = $"[DANH MỤC] Ngoại tệ: Bổ sung {newCurs.Count} loại ngoại tệ sang CSDL mới."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Ngoại tệ: {ex.Message}" });
                    }
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 25,
                StepTitle = "Tiền đề: Hoàn tất đồng bộ toàn bộ các bảng Thư viện / Danh mục!",
                Message = "[HOÀN THÀNH BƯỚC TIỀN ĐỀ] Toàn bộ các bảng danh mục đã sẵn sàng để tiếp nhận số dư!"
            });
        }

        // =========================================================================================
        // 1. TÀI KHOẢN THÔNG THƯỜNG (CATEGORY 'I' - KHÔNG CÓ CHI TIẾT)
        // =========================================================================================
        public async Task TransferNoDetailsAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 28,
                StepTitle = "1/7. Đang tổng hợp số dư tài khoản thông thường (Category 'I')...",
                Message = "[BẮT ĐẦU] Tổng hợp số dư tài khoản thông thường..."
            });

            int count = await Task.Run(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var coa = new EntityCollection<ChartOfAccountEntity>();
                    srcAdapter.FetchEntityCollection(coa, null);

                    var noDetailAccountIds = coa
                        .Where(a => string.IsNullOrEmpty(a.CategoryId) || a.CategoryId.Trim().ToUpper() == "I")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var entries = new EntityCollection<JournalEntryEntity>();
                    srcAdapter.FetchEntityCollection(entries, null);

                    var flatBalances = entries
                        .Where(e => !string.IsNullOrEmpty(e.AccountId) && noDetailAccountIds.Contains(e.AccountId.Trim()))
                        .GroupBy(e => e.AccountId.Trim())
                        .Select(g =>
                        {
                            double net = g.Sum(x => (double)((x.Amount ?? 0.0) * x.Dbcr));
                            return new NormalBalanceDto
                            {
                                AccountId = g.Key,
                                Amount = Math.Abs(net),
                                Dbcr = (short)(net >= 0 ? 1 : -1)
                            };
                        })
                        .Where(x => x.Amount > 0.001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var b in flatBalances)
                    {
                        destEntries.Add(new JournalEntryEntity
                        {
                            AccountId = b.AccountId,
                            Dbcr = b.Dbcr,
                            Amount = b.Amount,
                            JournalVoucherId = null
                        });
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return destEntries.Count;
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 38,
                StepTitle = "1/7. Hoàn tất chuyển số dư tài khoản thông thường.",
                Message = $"[THÀNH CÔNG] Đã chuyển {count} tài khoản thông thường sang CSDL mới."
            });
        }

        // =========================================================================================
        // 2. CHI TIẾT CÔNG NỢ (CATEGORY 'A' -> DebtDetail)
        // =========================================================================================
        public async Task TransferDebtAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 40,
                StepTitle = "2/7. Đang tổng hợp số dư chi tiết công nợ (Category 'A')...",
                Message = "[BẮT ĐẦU] Làm phẳng và cộng dồn số dư công nợ..."
            });

            (int objCount, int entryCount) = await Task.Run<(int, int)>(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var prefetch = new PrefetchPath2(EntityType.DebtDetailEntity);
                    prefetch.Add(DebtDetailEntity.PrefetchPathJournalEntry);

                    var debtDetails = new EntityCollection<DebtDetailEntity>();
                    srcAdapter.FetchEntityCollection(debtDetails, null, prefetch);

                    var flatDebts = debtDetails
                        .Where(d => d.JournalEntry != null && !string.IsNullOrEmpty(d.JournalEntry.AccountId))
                        .GroupBy(d => new
                        {
                            AccountId = d.JournalEntry.AccountId.Trim(),
                            PartnerId = d.PartnerId?.Trim() ?? string.Empty,
                            DebtReasonId = d.DebtReasonId,
                            DebtTypeId = d.DebtTypeId?.Trim() ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double net = g.Sum(x => (double)((x.Amount ?? 0.0) * x.JournalEntry.Dbcr));
                            return new DebtBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                PartnerId = g.Key.PartnerId,
                                DebtReasonId = g.Key.DebtReasonId,
                                DebtTypeId = g.Key.DebtTypeId,
                                Amount = Math.Abs(net),
                                Dbcr = (short)(net >= 0 ? 1 : -1)
                            };
                        })
                        .Where(x => x.Amount > 0.001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var grp in flatDebts.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            JournalVoucherId = null
                        };

                        foreach (var item in grp)
                        {
                            var d = new DebtDetailEntity
                            {
                                PartnerId = string.IsNullOrEmpty(item.PartnerId) ? null : item.PartnerId,
                                DebtReasonId = item.DebtReasonId,
                                DebtTypeId = string.IsNullOrEmpty(item.DebtTypeId) ? null : item.DebtTypeId,
                                Amount = item.Amount
                            };
                            je.DebtDetails.Add(d);
                        }

                        destEntries.Add(je);
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return (flatDebts.Count, destEntries.Count);
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 50,
                StepTitle = "2/7. Hoàn tất chuyển số dư công nợ chi tiết.",
                Message = $"[THÀNH CÔNG] Đã chuyển {objCount} đối tượng công nợ chi tiết ({entryCount} bút toán) sang CSDL mới."
            });
        }

        // =========================================================================================
        // 3. TỒN KHO VẬT TƯ & HÀNG HÓA (CATEGORY 'B' -> Phiếu nhập kho nội bộ NNB)
        // =========================================================================================
        public async Task TransferMaterialAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 52,
                StepTitle = "3/7. Đang tổng hợp tồn kho vật tư hàng hóa (Category 'B')...",
                Message = "[BẮT ĐẦU] Làm phẳng và cộng dồn tồn kho vật tư theo từng kho..."
            });

            (int matCount, int voucherCount) = await Task.Run<(int, int)>(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var prefetch = new PrefetchPath2(EntityType.InventoryVoucherLineEntity);
                    var pVoucher = prefetch.Add(InventoryVoucherLineEntity.PrefetchPathInventoryVoucher);
                    pVoucher.SubPath.Add(InventoryVoucherEntity.PrefetchPathJournalEntry);

                    var lines = new EntityCollection<InventoryVoucherLineEntity>();
                    srcAdapter.FetchEntityCollection(lines, null, prefetch);

                    var flatMaterials = lines
                        .Where(l => l.InventoryVoucher != null && l.InventoryVoucher.JournalEntry != null
                                 && !string.IsNullOrEmpty(l.InventoryVoucher.JournalEntry.AccountId)
                                 && !string.IsNullOrEmpty(l.InventoryVoucher.WarehouseId)
                                 && !string.IsNullOrEmpty(l.MaterialId))
                        .GroupBy(l => new
                        {
                            AccountId = l.InventoryVoucher.JournalEntry.AccountId.Trim(),
                            WarehouseId = l.InventoryVoucher.WarehouseId.Trim(),
                            MaterialId = l.MaterialId.Trim()
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * x.InventoryVoucher.JournalEntry.Dbcr));
                            double netAmt = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.Price ?? 0.0) * x.InventoryVoucher.JournalEntry.Dbcr));
                            double price = netQty > 0.0001 ? (netAmt / netQty) : 0;
                            return new MaterialBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                WarehouseId = g.Key.WarehouseId,
                                MaterialId = g.Key.MaterialId,
                                Quantity = netQty,
                                Price = price,
                                Amount = netAmt
                            };
                        })
                        .Where(x => x.Quantity > 0.0001 && x.Amount > 0.0001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var grp in flatMaterials.GroupBy(x => new { x.AccountId, x.WarehouseId }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = 1, // Tồn kho luôn là Debit
                            Amount = grp.Sum(x => x.Amount),
                            JournalVoucherId = null
                        };

                        var iv = new InventoryVoucherEntity
                        {
                            WarehouseId = grp.Key.WarehouseId,
                            Taxflag = false, // Phiếu nhập nội bộ NNB
                            JournalEntry = je
                        };

                        foreach (var mat in grp)
                        {
                            var line = new InventoryVoucherLineEntity
                            {
                                MaterialId = mat.MaterialId,
                                Quantity = mat.Quantity,
                                Price = mat.Price
                            };
                            iv.InventoryVoucherLines.Add(line);
                        }

                        je.InventoryVouchers.Add(iv);
                        destEntries.Add(je);
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return (flatMaterials.Count, destEntries.Count);
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 64,
                StepTitle = "3/7. Hoàn tất sinh phiếu nhập kho nội bộ (NNB) cho vật tư tồn kho.",
                Message = $"[THÀNH CÔNG] Đã sinh {voucherCount} phiếu nhập kho NNB với {matCount} dòng vật tư tồn kho sang CSDL mới."
            });
        }

        // =========================================================================================
        // 4. CHI PHÍ SXKD DỞ DANG (CATEGORY 'E' -> CostDetail)
        // =========================================================================================
        public async Task TransferCostAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 66,
                StepTitle = "4/7. Đang tổng hợp số dư chi phí SXKD dở dang (Category 'E')...",
                Message = "[BẮT ĐẦU] Làm phẳng và cộng dồn chi phí dở dang..."
            });

            int count = await Task.Run(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var prefetch = new PrefetchPath2(EntityType.CostDetailEntity);
                    prefetch.Add(CostDetailEntity.PrefetchPathJournalEntry);

                    var costs = new EntityCollection<CostDetailEntity>();
                    srcAdapter.FetchEntityCollection(costs, null, prefetch);

                    var flatCosts = costs
                        .Where(c => c.JournalEntry != null && !string.IsNullOrEmpty(c.JournalEntry.AccountId))
                        .GroupBy(c => new
                        {
                            AccountId = c.JournalEntry.AccountId.Trim(),
                            CostObjectId = c.CostObjectId?.Trim() ?? string.Empty,
                            CostElementId = c.CostElementId?.Trim() ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double net = g.Sum(x => (double)((x.Amount ?? 0.0) * x.JournalEntry.Dbcr));
                            return new CostBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                CostObjectId = g.Key.CostObjectId,
                                CostElementId = g.Key.CostElementId,
                                Amount = Math.Abs(net),
                                Dbcr = (short)(net >= 0 ? 1 : -1)
                            };
                        })
                        .Where(x => x.Amount > 0.001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var grp in flatCosts.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            JournalVoucherId = null
                        };

                        foreach (var item in grp)
                        {
                            var d = new CostDetailEntity
                            {
                                CostObjectId = string.IsNullOrEmpty(item.CostObjectId) ? null : item.CostObjectId,
                                CostElementId = string.IsNullOrEmpty(item.CostElementId) ? null : item.CostElementId,
                                Amount = item.Amount
                            };
                            je.CostDetails.Add(d);
                        }

                        destEntries.Add(je);
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return flatCosts.Count;
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 74,
                StepTitle = "4/7. Hoàn tất chuyển chi phí dở dang.",
                Message = $"[THÀNH CÔNG] Đã chuyển {count} dòng chi phí dở dang sang CSDL mới."
            });
        }

        // =========================================================================================
        // 5. TÀI SẢN CỐ ĐỊNH (CATEGORY 'D' -> AssetDetail)
        // =========================================================================================
        public async Task TransferAssetAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 76,
                StepTitle = "5/7. Đang tổng hợp số dư tài sản cố định (Category 'D')...",
                Message = "[BẮT ĐẦU] Làm phẳng và cộng dồn nguyên giá & tài sản cố định..."
            });

            (int assetCount, int entryCount) = await Task.Run<(int, int)>(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var prefetch = new PrefetchPath2(EntityType.AssetDetailEntity);
                    prefetch.Add(AssetDetailEntity.PrefetchPathJournalEntry);

                    var assets = new EntityCollection<AssetDetailEntity>();
                    srcAdapter.FetchEntityCollection(assets, null, prefetch);

                    var flatAssets = assets
                        .Where(a => a.JournalEntry != null && !string.IsNullOrEmpty(a.JournalEntry.AccountId) && !string.IsNullOrEmpty(a.AssetId))
                        .GroupBy(a => new
                        {
                            AccountId = a.JournalEntry.AccountId.Trim(),
                            AssetId = a.AssetId.Trim(),
                            SrCode = a.Srcode?.Trim() ?? string.Empty,
                            ReasonId = a.Reasonid?.Trim() ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * x.JournalEntry.Dbcr));
                            double netAmt = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.Price ?? 0.0) * x.JournalEntry.Dbcr));
                            double price = netQty > 0.0001 ? (netAmt / netQty) : 0;
                            return new AssetBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                AssetId = g.Key.AssetId,
                                SrCode = g.Key.SrCode,
                                ReasonId = g.Key.ReasonId,
                                Quantity = netQty,
                                Price = price,
                                Amount = netAmt
                            };
                        })
                        .Where(x => x.Quantity > 0.0001 && x.Amount > 0.0001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var grp in flatAssets.GroupBy(x => x.AccountId))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key,
                            Dbcr = 1, // Tài sản luôn là Debit
                            Amount = grp.Sum(x => x.Amount),
                            JournalVoucherId = null
                        };

                        foreach (var item in grp)
                        {
                            var d = new AssetDetailEntity
                            {
                                AssetId = item.AssetId,
                                Srcode = string.IsNullOrEmpty(item.SrCode) ? null : item.SrCode,
                                Reasonid = string.IsNullOrEmpty(item.ReasonId) ? null : item.ReasonId,
                                Quantity = item.Quantity,
                                Price = item.Price
                            };
                            je.AssetDetails.Add(d);
                        }

                        destEntries.Add(je);
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return (flatAssets.Count, destEntries.Count);
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 84,
                StepTitle = "5/7. Hoàn tất chuyển số dư tài sản cố định.",
                Message = $"[THÀNH CÔNG] Đã chuyển {assetCount} tài sản cố định ({entryCount} tài khoản) sang CSDL mới."
            });
        }

        // =========================================================================================
        // 6. CỔ PHIẾU & ĐẦU TƯ TÀI CHÍNH (CATEGORY 'S' -> InvestmentDetail)
        // =========================================================================================
        public async Task TransferStockAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 86,
                StepTitle = "6/7. Đang tổng hợp số dư cổ phiếu, chứng khoán (Category 'S')...",
                Message = "[BẮT ĐẦU] Làm phẳng và cộng dồn số dư cổ phiếu đầu tư..."
            });

            int count = await Task.Run(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var prefetch = new PrefetchPath2(EntityType.InvestmentDetailEntity);
                    prefetch.Add(InvestmentDetailEntity.PrefetchPathJournalEntry);

                    var stocks = new EntityCollection<InvestmentDetailEntity>();
                    srcAdapter.FetchEntityCollection(stocks, null, prefetch);

                    var flatStocks = stocks
                        .Where(s => s.JournalEntry != null && !string.IsNullOrEmpty(s.JournalEntry.AccountId) && !string.IsNullOrEmpty(s.Stockid))
                        .GroupBy(s => new
                        {
                            AccountId = s.JournalEntry.AccountId.Trim(),
                            StockId = s.Stockid.Trim(),
                            CustomerId = s.CustomerId?.Trim() ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * x.JournalEntry.Dbcr));
                            double netAmt = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.Inprice ?? 0.0) * x.JournalEntry.Dbcr));
                            double price = netQty > 0.0001 ? (netAmt / netQty) : 0;
                            return new StockBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                StockId = g.Key.StockId,
                                CustomerId = g.Key.CustomerId,
                                Quantity = netQty,
                                Price = price,
                                Amount = netAmt
                            };
                        })
                        .Where(x => x.Quantity > 0.0001 && x.Amount > 0.0001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var grp in flatStocks.GroupBy(x => x.AccountId))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key,
                            Dbcr = 1, // Cổ phiếu luôn là Debit
                            Amount = grp.Sum(x => x.Amount),
                            JournalVoucherId = null
                        };

                        foreach (var item in grp)
                        {
                            var d = new InvestmentDetailEntity
                            {
                                Stockid = item.StockId,
                                CustomerId = string.IsNullOrEmpty(item.CustomerId) ? null : item.CustomerId,
                                Quantity = item.Quantity,
                                Inprice = item.Price
                            };
                            je.InvestmentDetails.Add(d);
                        }

                        destEntries.Add(je);
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return flatStocks.Count;
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 92,
                StepTitle = "6/7. Hoàn tất chuyển số dư cổ phiếu, chứng khoán.",
                Message = $"[THÀNH CÔNG] Đã chuyển {count} danh mục cổ phiếu đầu tư sang CSDL mới."
            });
        }

        // =========================================================================================
        // 7. NGOẠI TỆ (CATEGORY 'C' -> CurrencyDetail & TÍNH NGƯỢC TỶ GIÁ)
        // =========================================================================================
        public async Task TransferCurrencyAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 94,
                StepTitle = "7/7. Đang tổng hợp số dư ngoại tệ & tính tỷ giá ghi sổ (Category 'C')...",
                Message = "[BẮT ĐẦU] Làm phẳng số dư ngoại tệ và tính tỷ giá bình quân..."
            });

            int count = await Task.Run(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = AppDataAccessAdapter.CreateCustom(context.DestinationConfig.ConnectionString, context.DestinationConfig.DbType))
                {
                    var prefetch = new PrefetchPath2(EntityType.CurrencyDetailEntity);
                    prefetch.Add(CurrencyDetailEntity.PrefetchPathJournalEntry);

                    var currencies = new EntityCollection<CurrencyDetailEntity>();
                    srcAdapter.FetchEntityCollection(currencies, null, prefetch);

                    var flatCurrencies = currencies
                        .Where(c => c.JournalEntry != null && !string.IsNullOrEmpty(c.JournalEntry.AccountId) && !string.IsNullOrEmpty(c.CurrencyId))
                        .GroupBy(c => new
                        {
                            AccountId = c.JournalEntry.AccountId.Trim(),
                            CurrencyId = c.CurrencyId.Trim()
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * x.JournalEntry.Dbcr));
                            double netAmt = g.Sum(x => (double)((x.JournalEntry.Amount ?? 0.0) * x.JournalEntry.Dbcr));
                            double rate = (Math.Abs(netQty) > 0.0001) ? Math.Abs(netAmt / netQty) : 0;
                            return new CurrencyBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                CurrencyId = g.Key.CurrencyId,
                                Quantity = Math.Abs(netQty),
                                Amount = Math.Abs(netAmt),
                                ExchangeRate = rate,
                                Dbcr = (short)(netAmt >= 0 ? 1 : -1)
                            };
                        })
                        .Where(x => x.Quantity > 0.0001 && x.Amount > 0.0001)
                        .ToList();

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    foreach (var grp in flatCurrencies.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            JournalVoucherId = null
                        };

                        foreach (var item in grp)
                        {
                            var d = new CurrencyDetailEntity
                            {
                                CurrencyId = item.CurrencyId,
                                Quantity = item.Quantity,
                                ExchangeRate = item.ExchangeRate
                            };
                            je.CurrencyDetails.Add(d);
                        }

                        destEntries.Add(je);
                    }

                    if (destEntries.Count > 0)
                    {
                        destAdapter.SaveEntityCollection(destEntries, true, false);
                    }

                    return flatCurrencies.Count;
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 98,
                StepTitle = "7/7. Hoàn tất chuyển số dư ngoại tệ.",
                Message = $"[THÀNH CÔNG] Đã chuyển {count} khoản mục ngoại tệ (tính ngược tỷ giá bình quân) sang CSDL mới."
            });
        }

        // =========================================================================================
        // ĐIỀU PHỐI CHẠY TUẦN TỰ TOÀN BỘ CÁC PHÂN HỆ (TÁCH BIỆT RÕ RÀNG ĐỂ BIẾT HỎNG Ở ĐÂU)
        // =========================================================================================
        public async Task ExecuteTransferAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 0,
                StepTitle = "Bắt đầu tiến trình chuyển đổi số dư hàng loạt...",
                Message = "Khởi động tiến trình chuyển đổi số dư sang niên độ mới..."
            });

            // 1. Kiểm tra kết nối CSDL đích
            progress?.Report(new TransferProgressReport
            {
                Percent = 3,
                StepTitle = "Đang kiểm tra kết nối tới CSDL đích...",
                Message = "Đang kiểm tra kết nối tới CSDL đích..."
            });

            bool isConnected = await TestDestinationConnectionAsync(context.DestinationConfig);
            if (!isConnected)
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 3,
                    IsError = true,
                    StepTitle = "Lỗi kết nối CSDL đích!",
                    Message = "Không thể kết nối tới CSDL đích. Vui lòng kiểm tra lại cấu hình kết nối!"
                });
                return;
            }

            progress?.Report(new TransferProgressReport
            {
                Percent = 5,
                StepTitle = "Kết nối CSDL đích thành công! Bắt đầu chuyển danh mục...",
                Message = "Kết nối CSDL đích thành công! Bắt đầu các bước đồng bộ..."
            });

            // 2. BƯỚC TIỀN ĐỀ: Đồng bộ các bảng Thư viện / Danh mục & Tài khoản người dùng
            try
            {
                await TransferDictionariesAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại bước Đồng bộ Thư viện / Danh mục!",
                    Message = $"[LỖI NGHIÊM TRỌNG] {ex.Message}"
                });
                throw;
            }

            // 3. Chuyển số dư tài khoản thông thường (Category 'I')
            try
            {
                await TransferNoDetailsAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Tài khoản thông thường!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 4. Chuyển số dư công nợ (Category 'A' -> DebtDetail)
            try
            {
                await TransferDebtAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Công nợ chi tiết!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 5. Chuyển số dư vật tư hàng hóa (Category 'B' -> Phiếu nhập kho nội bộ NNB)
            try
            {
                await TransferMaterialAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Tồn kho vật tư hàng hóa!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 6. Chuyển số dư chi phí dở dang (Category 'E' -> CostDetail)
            try
            {
                await TransferCostAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Chi phí dở dang!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 7. Chuyển số dư tài sản cố định (Category 'D' -> AssetDetail)
            try
            {
                await TransferAssetAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Tài sản cố định!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 8. Chuyển số dư cổ phiếu / đầu tư tài chính (Category 'S' -> InvestmentDetail)
            try
            {
                await TransferStockAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Cổ phiếu, chứng khoán!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 9. Chuyển số dư ngoại tệ (Category 'C' -> CurrencyDetail)
            try
            {
                await TransferCurrencyAsync(context, progress);
            }
            catch (Exception ex)
            {
                progress?.Report(new TransferProgressReport
                {
                    IsError = true,
                    StepTitle = "Lỗi tại phân hệ Ngoại tệ!",
                    Message = $"[LỖI] {ex.Message}"
                });
                throw;
            }

            // 10. Hoàn tất toàn bộ
            progress?.Report(new TransferProgressReport
            {
                Percent = 100,
                StepTitle = "ĐÃ HOÀN TẤT CHUYỂN ĐỔI SỐ DƯ TOÀN BỘ CÁC PHÂN HỆ!",
                Message = "Chúc mừng! Đã kết chuyển thành công toàn bộ số dư và danh mục sang niên độ mới một cách toàn vẹn!"
            });
        }
    }
}
