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
        private static string BuildConnectionString(AppConfig cfg)
        {
            if (cfg == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(cfg.ConnectionString))
                return cfg.ConnectionString;

            if (cfg.DbType == DatabaseType.Access)
            {
                return $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={cfg.AccessFilePath};Persist Security Info=False;";
            }
            else if (cfg.DbType == DatabaseType.SqlServer)
            {
                return cfg.SqlIntegratedSecurity
                    ? $"data source={cfg.SqlServerName};initial catalog={cfg.SqlDatabaseName};Integrated Security=SSPI;TrustServerCertificate=True;"
                    : $"data source={cfg.SqlServerName};initial catalog={cfg.SqlDatabaseName};User ID={cfg.SqlUsername};Password={cfg.SqlPassword};TrustServerCertificate=True;";
            }
            else
            {
                return $"Host={cfg.PgHost};Port={cfg.PgPort};Database={cfg.PgDatabase};Username={cfg.PgUsername};Password={cfg.PgPassword};";
            }
        }

        private static IDataAccessAdapter CreateDestAdapter(AppConfig destConfig)
        {
            string connStr = BuildConnectionString(destConfig);
            return AppDataAccessAdapter.CreateCustom(connStr, destConfig.DbType);
        }

        private static string? CleanStr(string? val)
        {
            return string.IsNullOrWhiteSpace(val) ? null : val.Trim();
        }

        private static void SaveWithoutRefetch(IDataAccessAdapter adapter, IEntity2 entity)
        {
            adapter.SaveEntity(entity, false, false);
            entity.Fields.State = EntityState.Fetched;
        }

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

                        string connStr = BuildConnectionString(destinationConfig);
                        using (var conn = new System.Data.OleDb.OleDbConnection(connStr))
                        {
                            conn.Open();
                            return true;
                        }
                    }
                    else if (destinationConfig.DbType == DatabaseType.SqlServer)
                    {
                        string connStr = BuildConnectionString(destinationConfig);
                        using (var conn = new Microsoft.Data.SqlClient.SqlConnection(connStr))
                        {
                            conn.Open();
                            return true;
                        }
                    }
                    else if (destinationConfig.DbType == DatabaseType.PostgreSql)
                    {
                        string connStr = BuildConnectionString(destinationConfig);
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
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

                        foreach (var su in srcUsers.Where(u => !string.IsNullOrWhiteSpace(u.Username)))
                        {
                            string uName = su.Username.Trim();
                            var existingDest = destUsers.FirstOrDefault(u => string.Equals(u.Username?.Trim(), uName, StringComparison.OrdinalIgnoreCase));
                            if (existingDest != null)
                            {
                                existingDest.Password = su.Password;
                                existingDest.FullName = su.FullName;
                                existingDest.RoleMask = su.RoleMask;
                                existingDest.IsActive = su.IsActive;
                                SaveWithoutRefetch(destAdapter, existingDest);
                                userUpdated++;
                            }
                            else
                            {
                                var nu = new SystemUserEntity
                                {
                                    Username = uName,
                                    Password = su.Password,
                                    FullName = su.FullName,
                                    RoleMask = su.RoleMask,
                                    IsActive = su.IsActive
                                };
                                SaveWithoutRefetch(destAdapter, nu);
                                destUsers.Add(nu);
                                userInserted++;
                            }
                        }

                        // 1b. SecurityPolicy (Chính sách bảo mật người dùng)
                        var srcSec = new EntityCollection<SecurityPolicyEntity>();
                        srcAdapter.FetchEntityCollection(srcSec, null);
                        var destSec = new EntityCollection<SecurityPolicyEntity>();
                        destAdapter.FetchEntityCollection(destSec, null);
                        var destSecByUser = destSec
                            .Where(s => !string.IsNullOrWhiteSpace(s.UserName))
                            .GroupBy(s => s.UserName.Trim(), StringComparer.OrdinalIgnoreCase)
                            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                        foreach (var sp in srcSec.Where(s => !string.IsNullOrWhiteSpace(s.UserName)))
                        {
                            string uName = sp.UserName.Trim();
                            if (destSecByUser.TryGetValue(uName, out var existingSp))
                            {
                                existingSp.SecurityLevel = sp.SecurityLevel;
                                SaveWithoutRefetch(destAdapter, existingSp);
                            }
                            else
                            {
                                var nsp = new SecurityPolicyEntity
                                {
                                    UserName = uName,
                                    SecurityLevel = sp.SecurityLevel
                                };
                                SaveWithoutRefetch(destAdapter, nsp);
                                destSecByUser[uName] = nsp;
                            }
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 8,
                            Message = $"[DANH MỤC] Người dùng hệ thống: Đã cập nhật {userUpdated} user (gồm admin), thêm mới {userInserted} user."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ SystemUser: {ex.Message}" });
                    }

                    // 2. DANH MỤC TÀI KHOẢN (ChartOfAccount) & PHÂN QUYỀN TÀI KHOẢN (UserAccount)
                    try
                    {
                        var srcAccounts = new EntityCollection<ChartOfAccountEntity>();
                        srcAdapter.FetchEntityCollection(srcAccounts, null);

                        var destAccounts = new EntityCollection<ChartOfAccountEntity>();
                        destAdapter.FetchEntityCollection(destAccounts, null);

                        var destAccIds = destAccounts
                            .Where(a => !string.IsNullOrWhiteSpace(a.AccountId))
                            .Select(a => a.AccountId.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        var missingAccounts = srcAccounts
                            .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && !destAccIds.Contains(a.AccountId.Trim()))
                            .OrderBy(a => a.AccountId.Trim().Length)
                            .ThenBy(a => a.AccountId.Trim())
                            .ToList();

                        int addedAcc = 0;
                        foreach (var a in missingAccounts)
                        {
                            string accId = a.AccountId.Trim();
                            if (destAccIds.Contains(accId)) continue;

                            var acc = new ChartOfAccountEntity
                            {
                                AccountId = accId,
                                AccountName = a.AccountName,
                                Taxflag = a.Taxflag,
                                Outtableflag = a.Outtableflag,
                                Splite = a.Splite,
                                Dispflag = a.Dispflag
                            };

                            string? catId = CleanStr(a.CategoryId);
                            if (catId != null) acc.CategoryId = catId;

                            string? parentId = CleanStr(a.ParentId);
                            if (parentId != null && destAccIds.Contains(parentId))
                            {
                                acc.ParentId = parentId;
                            }

                            SaveWithoutRefetch(destAdapter, acc);
                            destAccIds.Add(accId);
                            addedAcc++;
                        }

                        // 2b. UserAccount (Phân quyền tài khoản theo người dùng)
                        var srcUserAcc = new EntityCollection<UserAccountEntity>();
                        srcAdapter.FetchEntityCollection(srcUserAcc, null);
                        var destUserAcc = new EntityCollection<UserAccountEntity>();
                        destAdapter.FetchEntityCollection(destUserAcc, null);
                        var destUaKeys = destUserAcc
                            .Where(ua => !string.IsNullOrWhiteSpace(ua.User) && !string.IsNullOrWhiteSpace(ua.AccountId))
                            .Select(ua => $"{ua.User.Trim().ToUpperInvariant()}|{ua.AccountId.Trim().ToUpperInvariant()}")
                            .ToHashSet();

                        foreach (var ua in srcUserAcc.Where(x => !string.IsNullOrWhiteSpace(x.User) && !string.IsNullOrWhiteSpace(x.AccountId)))
                        {
                            string u = ua.User.Trim();
                            string accId = ua.AccountId.Trim();
                            string key = $"{u.ToUpperInvariant()}|{accId.ToUpperInvariant()}";
                            if (!destUaKeys.Contains(key) && destAccIds.Contains(accId))
                            {
                                SaveWithoutRefetch(destAdapter, new UserAccountEntity { User = u, AccountId = accId });
                                destUaKeys.Add(key);
                            }
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 10,
                            Message = $"[DANH MỤC] Hệ thống tài khoản: Đã bổ sung {addedAcc} tài khoản kế toán sang CSDL mới."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ ChartOfAccount: {ex.Message}" });
                    }

                    // 3. DANH MỤC CÔNG NỢ (Partner, DebtType, DebtReason)
                    try
                    {
                        // 3a. Partner
                        var srcPartners = new EntityCollection<PartnerEntity>();
                        srcAdapter.FetchEntityCollection(srcPartners, null);
                        var destPartners = new EntityCollection<PartnerEntity>();
                        destAdapter.FetchEntityCollection(destPartners, null);
                        var destPartnerIds = destPartners
                            .Where(p => !string.IsNullOrWhiteSpace(p.Id))
                            .Select(p => p.Id.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedPartners = 0;
                        foreach (var p in srcPartners.Where(p => !string.IsNullOrWhiteSpace(p.Id) && !destPartnerIds.Contains(p.Id.Trim())))
                        {
                            string pid = p.Id.Trim();
                            if (destPartnerIds.Contains(pid)) continue;

                            var np = new PartnerEntity
                            {
                                Id = pid,
                                Isemployee = p.Isemployee
                            };
                            if (!string.IsNullOrEmpty(p.CustomerName)) np.CustomerName = p.CustomerName;
                            if (!string.IsNullOrEmpty(p.Address)) np.Address = p.Address;
                            if (!string.IsNullOrEmpty(p.Taxno)) np.Taxno = p.Taxno;
                            if (!string.IsNullOrEmpty(p.Phonenumber)) np.Phonenumber = p.Phonenumber;
                            if (!string.IsNullOrEmpty(p.Faxnumber)) np.Faxnumber = p.Faxnumber;
                            if (!string.IsNullOrEmpty(p.Note)) np.Note = p.Note;

                            SaveWithoutRefetch(destAdapter, np);
                            destPartnerIds.Add(pid);
                            addedPartners++;
                        }

                        // 3b. DebtType (Nội dung công nợ)
                        var srcDt = new EntityCollection<DebtTypeEntity>();
                        srcAdapter.FetchEntityCollection(srcDt, null);
                        var destDt = new EntityCollection<DebtTypeEntity>();
                        destAdapter.FetchEntityCollection(destDt, null);
                        var destDtIds = destDt
                            .Where(d => !string.IsNullOrWhiteSpace(d.Id))
                            .Select(d => d.Id.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedDt = 0;
                        foreach (var dt in srcDt.Where(d => !string.IsNullOrWhiteSpace(d.Id) && !destDtIds.Contains(d.Id.Trim())))
                        {
                            string dtId = dt.Id.Trim();
                            if (destDtIds.Contains(dtId)) continue;

                            var ndt = new DebtTypeEntity
                            {
                                Id = dtId,
                                TypeName = dt.TypeName,
                                Syncronizeflag = dt.Syncronizeflag
                            };
                            SaveWithoutRefetch(destAdapter, ndt);
                            destDtIds.Add(dtId);
                            addedDt++;
                        }

                        // 3c. DebtReason (Lý do công nợ)
                        var srcDr = new EntityCollection<DebtReasonEntity>();
                        srcAdapter.FetchEntityCollection(srcDr, null);
                        var destDr = new EntityCollection<DebtReasonEntity>();
                        destAdapter.FetchEntityCollection(destDr, null);
                        var destDrNames = destDr
                            .Where(r => !string.IsNullOrWhiteSpace(r.ReasonName))
                            .Select(r => r.ReasonName.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        foreach (var dr in srcDr.Where(r => !string.IsNullOrWhiteSpace(r.ReasonName) && !destDrNames.Contains(r.ReasonName.Trim())))
                        {
                            string drName = dr.ReasonName.Trim();
                            if (destDrNames.Contains(drName)) continue;

                            var ndr = new DebtReasonEntity
                            {
                                ReasonName = drName,
                                Visible = dr.Visible
                            };
                            SaveWithoutRefetch(destAdapter, ndr);
                            destDrNames.Add(drName);
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 12,
                            Message = $"[DANH MỤC] Công nợ: Bổ sung {addedPartners} đối tác, {addedDt} nội dung công nợ."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Partner/DebtType: {ex.Message}" });
                    }

                    // 4. DANH MỤC KHO, VẬT TƯ & THUẾ SUẤT (TaxRate, Warehouse, Category, InventoryItem)
                    try
                    {
                        // 4.0. Thuế suất (TaxRate)
                        var srcTaxRates = new EntityCollection<TaxRateEntity>();
                        srcAdapter.FetchEntityCollection(srcTaxRates, null);
                        var destTaxRates = new EntityCollection<TaxRateEntity>();
                        destAdapter.FetchEntityCollection(destTaxRates, null);
                        var destTaxRateById = destTaxRates.ToDictionary(t => t.Taxrateid, t => t);
                        var destTaxRateDescSet = destTaxRates
                            .Where(t => !string.IsNullOrWhiteSpace(t.Taxratedes))
                            .Select(t => t.Taxratedes.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        foreach (var tr in srcTaxRates.OrderBy(t => t.Taxrateid))
                        {
                            if (destTaxRateById.TryGetValue(tr.Taxrateid, out var existingTr))
                            {
                                existingTr.Rate = tr.Rate;
                                if (!string.IsNullOrWhiteSpace(tr.Taxratedes)) existingTr.Taxratedes = tr.Taxratedes;
                                SaveWithoutRefetch(destAdapter, existingTr);
                            }
                            else if (!destTaxRateById.ContainsKey(tr.Taxrateid))
                            {
                                string trDesc = tr.Taxratedes?.Trim() ?? "";
                                var ntr = new TaxRateEntity
                                {
                                    Taxrateid = tr.Taxrateid,
                                    Rate = tr.Rate,
                                    Taxratedes = trDesc
                                };
                                SaveWithoutRefetch(destAdapter, ntr);
                                destTaxRateById[tr.Taxrateid] = ntr;
                                if (!string.IsNullOrEmpty(trDesc)) destTaxRateDescSet.Add(trDesc);
                            }
                        }

                        // 4a. Kho hàng (Warehouse)
                        var srcWh = new EntityCollection<WarehouseEntity>();
                        srcAdapter.FetchEntityCollection(srcWh, null);
                        var destWh = new EntityCollection<WarehouseEntity>();
                        destAdapter.FetchEntityCollection(destWh, null);
                        var destWhIds = destWh
                            .Where(w => !string.IsNullOrWhiteSpace(w.WarehouseId))
                            .Select(w => w.WarehouseId.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedWh = 0;
                        foreach (var w in srcWh.Where(w => !string.IsNullOrWhiteSpace(w.WarehouseId) && !destWhIds.Contains(w.WarehouseId.Trim())))
                        {
                            string whId = w.WarehouseId.Trim();
                            if (destWhIds.Contains(whId)) continue;

                            var nw = new WarehouseEntity
                            {
                                WarehouseId = whId,
                                WarehouseName = w.WarehouseName
                            };
                            if (!string.IsNullOrEmpty(w.Place)) nw.Place = w.Place;
                            SaveWithoutRefetch(destAdapter, nw);
                            destWhIds.Add(whId);
                            addedWh++;
                        }

                        // 4b. Nhóm vật tư (Category)
                        var srcCat = new EntityCollection<CategoryEntity>();
                        srcAdapter.FetchEntityCollection(srcCat, null);
                        var destCat = new EntityCollection<CategoryEntity>();
                        destAdapter.FetchEntityCollection(destCat, null);
                        var validCatIds = destCat
                            .Where(c => !string.IsNullOrWhiteSpace(c.Categoryid))
                            .Select(c => c.Categoryid.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedCat = 0;
                        foreach (var c in srcCat.Where(c => !string.IsNullOrWhiteSpace(c.Categoryid) && !validCatIds.Contains(c.Categoryid.Trim())))
                        {
                            string catId = c.Categoryid.Trim();
                            if (validCatIds.Contains(catId)) continue;

                            var nc = new CategoryEntity
                            {
                                Categoryid = catId,
                                Categoryname = string.IsNullOrWhiteSpace(c.Categoryname) ? catId : c.Categoryname,
                                Taxrateid = (c.Taxrateid.HasValue && destTaxRateById.ContainsKey(c.Taxrateid.Value)) ? c.Taxrateid : null
                            };
                            SaveWithoutRefetch(destAdapter, nc);
                            validCatIds.Add(catId);
                            addedCat++;
                        }

                        // 4c. Vật tư hàng hóa (InventoryItem)
                        var srcItems = new EntityCollection<InventoryItemEntity>();
                        srcAdapter.FetchEntityCollection(srcItems, null);
                        var destItems = new EntityCollection<InventoryItemEntity>();
                        destAdapter.FetchEntityCollection(destItems, null);
                        var destItemIds = destItems
                            .Where(i => !string.IsNullOrWhiteSpace(i.MaterialId))
                            .Select(i => i.MaterialId.Trim())
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        // Tự động bổ sung các Categoryid còn thiếu nếu vật tư tham chiếu tới mã nhóm chưa có trong bảng Category
                        var itemCatIds = srcItems
                            .Select(i => CleanStr(i.Categoryid))
                            .Where(cid => cid != null && !validCatIds.Contains(cid))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToList();

                        foreach (var missingCatId in itemCatIds)
                        {
                            if (missingCatId == null || validCatIds.Contains(missingCatId)) continue;
                            var autoCat = new CategoryEntity
                            {
                                Categoryid = missingCatId,
                                Categoryname = missingCatId
                            };
                            SaveWithoutRefetch(destAdapter, autoCat);
                            validCatIds.Add(missingCatId);
                            addedCat++;
                        }

                        int addedItems = 0;
                        foreach (var itm in srcItems.Where(i => !string.IsNullOrWhiteSpace(i.MaterialId) && !destItemIds.Contains(i.MaterialId.Trim())))
                        {
                            string matId = itm.MaterialId.Trim();
                            if (destItemIds.Contains(matId)) continue;

                            var ni = new InventoryItemEntity
                            {
                                MaterialId = matId
                            };
                            if (!string.IsNullOrEmpty(itm.MaterialName)) ni.MaterialName = itm.MaterialName;
                            if (!string.IsNullOrEmpty(itm.Unit)) ni.Unit = itm.Unit;

                            string? catId = CleanStr(itm.Categoryid);
                            if (catId != null && validCatIds.Contains(catId))
                            {
                                ni.Categoryid = catId;
                            }

                            SaveWithoutRefetch(destAdapter, ni);
                            destItemIds.Add(matId);
                            addedItems++;
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 14,
                            Message = $"[DANH MỤC] Kho & Vật tư: Bổ sung {addedWh} kho, {addedCat} nhóm vật tư, {addedItems} mã vật tư."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Kho & Vật tư: {ex.Message}" });
                    }

                    // 5. DANH MỤC TÀI SẢN CỐ ĐỊNH (Department, Source, Reason, Asset, AssetTracking)
                    try
                    {
                        // Department
                        var srcDept = new EntityCollection<DepartmentEntity>();
                        srcAdapter.FetchEntityCollection(srcDept, null);
                        var destDept = new EntityCollection<DepartmentEntity>();
                        destAdapter.FetchEntityCollection(destDept, null);
                        var destDeptIds = destDept.Where(d => !string.IsNullOrWhiteSpace(d.Deptid)).Select(d => d.Deptid.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                        foreach (var d in srcDept.Where(d => !string.IsNullOrWhiteSpace(d.Deptid) && !destDeptIds.Contains(d.Deptid.Trim())))
                        {
                            string deptId = d.Deptid.Trim();
                            if (destDeptIds.Contains(deptId)) continue;
                            SaveWithoutRefetch(destAdapter, new DepartmentEntity { Deptid = deptId, DeptName = d.DeptName });
                            destDeptIds.Add(deptId);
                        }

                        // Source
                        var srcSrc = new EntityCollection<SourceEntity>();
                        srcAdapter.FetchEntityCollection(srcSrc, null);
                        var destSrc = new EntityCollection<SourceEntity>();
                        destAdapter.FetchEntityCollection(destSrc, null);
                        var destSrcIds = destSrc.Where(s => !string.IsNullOrWhiteSpace(s.Srcode)).Select(s => s.Srcode.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                        foreach (var s in srcSrc.Where(s => !string.IsNullOrWhiteSpace(s.Srcode) && !destSrcIds.Contains(s.Srcode.Trim())))
                        {
                            string srcId = s.Srcode.Trim();
                            if (destSrcIds.Contains(srcId)) continue;
                            SaveWithoutRefetch(destAdapter, new SourceEntity { Srcode = srcId, Srname = s.Srname });
                            destSrcIds.Add(srcId);
                        }

                        // Reason
                        var srcRsn = new EntityCollection<ReasonEntity>();
                        srcAdapter.FetchEntityCollection(srcRsn, null);
                        var destRsn = new EntityCollection<ReasonEntity>();
                        destAdapter.FetchEntityCollection(destRsn, null);
                        var destRsnIds = destRsn.Where(r => !string.IsNullOrWhiteSpace(r.Reasonid)).Select(r => r.Reasonid.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                        foreach (var r in srcRsn.Where(r => !string.IsNullOrWhiteSpace(r.Reasonid) && !destRsnIds.Contains(r.Reasonid.Trim())))
                        {
                            string rsnId = r.Reasonid.Trim();
                            if (destRsnIds.Contains(rsnId)) continue;
                            SaveWithoutRefetch(destAdapter, new ReasonEntity { Reasonid = rsnId, ReasonName = r.ReasonName, Flag = r.Flag });
                            destRsnIds.Add(rsnId);
                        }

                        // Asset
                        var srcAsset = new EntityCollection<AssetEntity>();
                        srcAdapter.FetchEntityCollection(srcAsset, null);
                        var destAsset = new EntityCollection<AssetEntity>();
                        destAdapter.FetchEntityCollection(destAsset, null);
                        var destAssetIds = destAsset.Where(a => !string.IsNullOrWhiteSpace(a.AssetId)).Select(a => a.AssetId.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedAssets = 0;
                        foreach (var a in srcAsset.Where(a => !string.IsNullOrWhiteSpace(a.AssetId) && !destAssetIds.Contains(a.AssetId.Trim())))
                        {
                            string assetId = a.AssetId.Trim();
                            if (destAssetIds.Contains(assetId)) continue;

                            var na = new AssetEntity
                            {
                                AssetId = assetId,
                                Date = a.Date,
                                Price = a.Price,
                                Qty = a.Qty,
                                Timeusing = a.Timeusing
                            };
                            if (!string.IsNullOrEmpty(a.AssetName)) na.AssetName = a.AssetName;
                            if (!string.IsNullOrEmpty(a.Unit)) na.Unit = a.Unit;
                            if (!string.IsNullOrEmpty(a.AssetHandle)) na.AssetHandle = a.AssetHandle;
                            if (!string.IsNullOrEmpty(a.Country)) na.Country = a.Country;

                            SaveWithoutRefetch(destAdapter, na);
                            destAssetIds.Add(assetId);
                            addedAssets++;
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 16,
                            Message = $"[DANH MỤC] Tài sản cố định: Bổ sung {addedAssets} danh mục tài sản."
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
                        var destCoIds = destCo.Where(c => !string.IsNullOrWhiteSpace(c.Id)).Select(c => c.Id.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedCo = 0;
                        foreach (var co in srcCo.Where(c => !string.IsNullOrWhiteSpace(c.Id) && !destCoIds.Contains(c.Id.Trim())))
                        {
                            string coId = co.Id.Trim();
                            if (destCoIds.Contains(coId)) continue;

                            var nco = new CostObjectEntity
                            {
                                Id = coId,
                                Syncronizeflag = co.Syncronizeflag
                            };
                            if (!string.IsNullOrEmpty(co.ObjectName)) nco.ObjectName = co.ObjectName;
                            SaveWithoutRefetch(destAdapter, nco);
                            destCoIds.Add(coId);
                            addedCo++;
                        }

                        var srcCe = new EntityCollection<CostElementEntity>();
                        srcAdapter.FetchEntityCollection(srcCe, null);
                        var destCe = new EntityCollection<CostElementEntity>();
                        destAdapter.FetchEntityCollection(destCe, null);
                        var destCeIds = destCe.Where(c => !string.IsNullOrWhiteSpace(c.Id)).Select(c => c.Id.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedCe = 0;
                        foreach (var ce in srcCe.Where(c => !string.IsNullOrWhiteSpace(c.Id) && !destCeIds.Contains(c.Id.Trim())))
                        {
                            string ceId = ce.Id.Trim();
                            if (destCeIds.Contains(ceId)) continue;

                            var nce = new CostElementEntity
                            {
                                Id = ceId,
                                Taxflag = ce.Taxflag
                            };
                            if (!string.IsNullOrEmpty(ce.ElementName)) nce.ElementName = ce.ElementName;
                            SaveWithoutRefetch(destAdapter, nce);
                            destCeIds.Add(ceId);
                            addedCe++;
                        }

                        // Bổ sung đồng bộ Sổ theo dõi TSCĐ (AssetTracking & AssetTrackingDepreciation) sau khi đã có đủ Department, Asset, CostObject
                        try
                        {
                            var srcAt = new EntityCollection<AssetTrackingEntity>();
                            srcAdapter.FetchEntityCollection(srcAt, null);
                            var destAt = new EntityCollection<AssetTrackingEntity>();
                            destAdapter.FetchEntityCollection(destAt, null);
                            if (srcAt.Count > 0 && destAt.Count == 0)
                            {
                                var validAssetSet = new EntityCollection<AssetEntity>();
                                destAdapter.FetchEntityCollection(validAssetSet, null);
                                var assetSet = validAssetSet.Where(a => !string.IsNullOrWhiteSpace(a.AssetId)).Select(a => a.AssetId.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                                var validDeptSet = new EntityCollection<DepartmentEntity>();
                                destAdapter.FetchEntityCollection(validDeptSet, null);
                                var deptSet = validDeptSet.Where(d => !string.IsNullOrWhiteSpace(d.Deptid)).Select(d => d.Deptid.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                                foreach (var at in srcAt)
                                {
                                    var nat = new AssetTrackingEntity
                                    {
                                        Begindate = at.Begindate,
                                        Enddate = at.Enddate,
                                        Depprice = at.Depprice,
                                        Deprate = at.Deprate,
                                        Qty = at.Qty,
                                        Remainvalue = at.Remainvalue,
                                        Timeusing = at.Timeusing
                                    };
                                    string? assetId = CleanStr(at.AssetId);
                                    if (assetId != null && assetSet.Contains(assetId)) nat.AssetId = assetId;
                                    string? deptId = CleanStr(at.Deptid);
                                    if (deptId != null && deptSet.Contains(deptId)) nat.Deptid = deptId;
                                    string? exObjId = CleanStr(at.ExobjectId);
                                    if (exObjId != null && destCoIds.Contains(exObjId)) nat.ExobjectId = exObjId;
                                    string? asAcc = CleanStr(at.AsaccountId);
                                    if (asAcc != null) nat.AsaccountId = asAcc;
                                    string? exAcc = CleanStr(at.ExaccountId);
                                    if (exAcc != null) nat.ExaccountId = exAcc;
                                    string? country = CleanStr(at.Country);
                                    if (country != null) nat.Country = country;

                                    SaveWithoutRefetch(destAdapter, nat);
                                }
                            }
                        }
                        catch { }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 18,
                            Message = $"[DANH MỤC] Chi phí: Bổ sung {addedCo} đối tượng THCP, {addedCe} khoản mục chi phí."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Chi phí: {ex.Message}" });
                    }

                    // 7. DANH MỤC CỔ PHIẾU (StockType, Stock)
                    try
                    {
                        var srcSt = new EntityCollection<StockTypeEntity>();
                        srcAdapter.FetchEntityCollection(srcSt, null);
                        var destSt = new EntityCollection<StockTypeEntity>();
                        destAdapter.FetchEntityCollection(destSt, null);
                        var destStIds = destSt.Select(t => t.Stocktypeid).ToHashSet();

                        foreach (var st in srcSt.Where(t => !destStIds.Contains(t.Stocktypeid)))
                        {
                            short stId = st.Stocktypeid;
                            if (destStIds.Contains(stId)) continue;
                            SaveWithoutRefetch(destAdapter, new StockTypeEntity { Stocktypeid = stId, Description = st.Description });
                            destStIds.Add(stId);
                        }

                        var srcStk = new EntityCollection<StockEntity>();
                        srcAdapter.FetchEntityCollection(srcStk, null);
                        var destStk = new EntityCollection<StockEntity>();
                        destAdapter.FetchEntityCollection(destStk, null);
                        var destStkIds = destStk.Where(s => !string.IsNullOrWhiteSpace(s.Stockid)).Select(s => s.Stockid.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedStk = 0;
                        foreach (var s in srcStk.Where(s => !string.IsNullOrWhiteSpace(s.Stockid) && !destStkIds.Contains(s.Stockid.Trim())))
                        {
                            string stkId = s.Stockid.Trim();
                            if (destStkIds.Contains(stkId)) continue;

                            var ns = new StockEntity
                            {
                                Stockid = stkId,
                                Stocktypeid = s.Stocktypeid
                            };
                            if (!string.IsNullOrEmpty(s.Description)) ns.Description = s.Description;
                            SaveWithoutRefetch(destAdapter, ns);
                            destStkIds.Add(stkId);
                            addedStk++;
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 19,
                            Message = $"[DANH MỤC] Cổ phiếu: Bổ sung {addedStk} mã chứng khoán sang CSDL mới."
                        });
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Cổ phiếu: {ex.Message}" });
                    }

                    // 8. DANH MỤC NGOẠI TỆ, TỶ GIÁ & LÃI SUẤT (Currency, ExchangeRate, InterestRate)
                    try
                    {
                        var srcCur = new EntityCollection<CurrencyEntity>();
                        srcAdapter.FetchEntityCollection(srcCur, null);
                        var destCur = new EntityCollection<CurrencyEntity>();
                        destAdapter.FetchEntityCollection(destCur, null);
                        var destCurIds = destCur.Where(c => !string.IsNullOrWhiteSpace(c.CurrencyId)).Select(c => c.CurrencyId.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        int addedCur = 0;
                        foreach (var c in srcCur.Where(c => !string.IsNullOrWhiteSpace(c.CurrencyId) && !destCurIds.Contains(c.CurrencyId.Trim())))
                        {
                            string curId = c.CurrencyId.Trim();
                            if (destCurIds.Contains(curId)) continue;

                            var nc = new CurrencyEntity { CurrencyId = curId };
                            if (!string.IsNullOrEmpty(c.CurrencyName)) nc.CurrencyName = c.CurrencyName;
                            SaveWithoutRefetch(destAdapter, nc);
                            destCurIds.Add(curId);
                            addedCur++;
                        }

                        // 8b. ExchangeRate (Tỷ giá ngoại tệ theo tháng)
                        var srcExRates = new EntityCollection<ExchangeRateEntity>();
                        srcAdapter.FetchEntityCollection(srcExRates, null);
                        var destExRates = new EntityCollection<ExchangeRateEntity>();
                        destAdapter.FetchEntityCollection(destExRates, null);
                        var destExRateMap = destExRates
                            .Where(r => !string.IsNullOrWhiteSpace(r.CurrencyId))
                            .GroupBy(r => $"{r.CurrencyId.Trim().ToUpperInvariant()}|{r.Month}")
                            .ToDictionary(g => g.Key, g => g.First());

                        int syncedExRates = 0;
                        foreach (var er in srcExRates.Where(r => !string.IsNullOrWhiteSpace(r.CurrencyId)))
                        {
                            string cid = er.CurrencyId.Trim();
                            if (!destCurIds.Contains(cid)) continue;
                            string key = $"{cid.ToUpperInvariant()}|{er.Month}";
                            if (destExRateMap.TryGetValue(key, out var existingEr))
                            {
                                existingEr.ExchangeRate = er.ExchangeRate;
                                SaveWithoutRefetch(destAdapter, existingEr);
                            }
                            else
                            {
                                var ner = new ExchangeRateEntity
                                {
                                    CurrencyId = cid,
                                    Month = er.Month,
                                    ExchangeRate = er.ExchangeRate
                                };
                                SaveWithoutRefetch(destAdapter, ner);
                                destExRateMap[key] = ner;
                                syncedExRates++;
                            }
                        }

                        // 8c. InterestRate (Lãi suất)
                        var srcIntRates = new EntityCollection<InterestRateEntity>();
                        srcAdapter.FetchEntityCollection(srcIntRates, null);
                        var destIntRates = new EntityCollection<InterestRateEntity>();
                        destAdapter.FetchEntityCollection(destIntRates, null);
                        var destIntDates = destIntRates.Select(i => i.Enddate.Date).ToHashSet();

                        foreach (var ir in srcIntRates.Where(i => !destIntDates.Contains(i.Enddate.Date)))
                        {
                            DateTime dt = ir.Enddate.Date;
                            if (destIntDates.Contains(dt)) continue;
                            SaveWithoutRefetch(destAdapter, new InterestRateEntity { Enddate = ir.Enddate, Rate = ir.Rate });
                            destIntDates.Add(dt);
                        }

                        progress?.Report(new TransferProgressReport
                        {
                            Percent = 20,
                            Message = $"[DANH MỤC] Ngoại tệ & Tỷ giá: Bổ sung {addedCur} loại ngoại tệ, {syncedExRates} dòng tỷ giá."
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
                Percent = 20,
                StepTitle = "Tiền đề: Hoàn tất đồng bộ toàn bộ các bảng Thư viện / Danh mục!",
                Message = "[HOÀN THÀNH BƯỚC DANH MỤC] Toàn bộ các bảng danh mục đã sẵn sàng!"
            });
        }

        /// <summary>
        /// Đặt lại trạng thái toàn bộ cây Entity (Root + các nhánh con đã nạp qua PrefetchPath)
        /// về trạng thái thêm mới (IsNew = true, IsDirty = true) để truyền thẳng cả Collection
        /// sang destAdapter.SaveEntityCollection(rootCollection, false, true).
        /// LLBLGen sẽ tự động duyệt cây từ gốc xuống lá và tự đồng bộ khóa chính @@IDENTITY mới sang khóa ngoại.
        /// </summary>
        private static int ResetGraphForInsert(IEntityCollection2? collection)
        {
            if (collection == null || collection.Count == 0) return 0;
            int totalEntities = 0;

            foreach (IEntity2 entity in collection)
            {
                entity.IsNew = true;
                entity.IsDirty = true;

                for (int i = 0; i < entity.Fields.Count; i++)
                {
                    var field = entity.Fields[i];
                    if (field.IsPrimaryKey && field.IsReadOnly)
                    {
                        // Khóa chính tự tăng (@@IDENTITY): đặt về 0 và không đánh dấu IsChanged
                        // để khi lưu vào DB đích, giá trị @@IDENTITY mới luôn kích hoạt AfterSave đồng bộ PK -> FK
                        field.ForcedCurrentValueWrite(0);
                        field.IsChanged = false;
                    }
                    else if (!field.IsReadOnly)
                    {
                        var val = field.CurrentValue;
                        if (val is string s)
                        {
                            string? cleaned = CleanStr(s);
                            field.ForcedCurrentValueWrite(cleaned);
                            field.IsChanged = (cleaned != null);
                        }
                        else
                        {
                            field.IsChanged = (val != null);
                        }
                    }
                }

                totalEntities++;

                foreach (IEntityCollection2 childCol in entity.GetMemberEntityCollections())
                {
                    totalEntities += ResetGraphForInsert(childCol);
                }
            }

            return totalEntities;
        }

        // =========================================================================================
        // BƯỚC TIỀN ĐỀ 2: ĐỒNG BỘ TOÀN BỘ CÁC BẢNG THIẾT KẾ BÁO CÁO & CÔNG THỨC (DESIGN TABLES)
        // (Không chuyển các bảng tạm tính báo cáo như TrialBalance, GeneralLedger, DebtBalance...)
        // =========================================================================================
        public async Task TransferDesignTablesAsync(TransferBalanceContext context, IProgress<TransferProgressReport> progress)
        {
            progress?.Report(new TransferProgressReport
            {
                Percent = 21,
                StepTitle = "Thiết kế: Đang đồng bộ các bảng Thiết kế Báo cáo tài chính & Thuế...",
                Message = "[BẮT ĐẦU] Sao chép các bảng thiết kế công thức báo cáo (CĐKT, KQKD, LCTT, Thuế, Giá thành) sang CSDL đích..."
            });

            await Task.Run(() =>
            {
                using (var srcAdapter = AppDataAccessAdapter.Create())
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    // 1. THIẾT KẾ BẢNG CÂN ĐỐI KẾ TOÁN (B01-DN: ReportSection -> ReportCategory -> ReportItem -> ReportFormula -> ReportFormulaAccount)
                    try
                    {
                        var srcSections = new EntityCollection<ReportSectionEntity>();
                        var prefetch = new PrefetchPath2(EntityType.ReportSectionEntity);
                        prefetch.Add(ReportSectionEntity.PrefetchPathReportCategories)
                                .SubPath.Add(ReportCategoryEntity.PrefetchPathReportItems)
                                .SubPath.Add(ReportItemEntity.PrefetchPathReportFormulas)
                                .SubPath.Add(ReportFormulaEntity.PrefetchPathReportFormulaAccounts);
                        srcAdapter.FetchEntityCollection(srcSections, null, prefetch);

                        if (srcSections.Count > 0)
                        {
                            try { destAdapter.DeleteEntitiesDirectly(typeof(CashFlowPreviousPeriodEntity), null); } catch { }
                            try { destAdapter.DeleteEntitiesDirectly(typeof(PreviousBusinessPeriodEntity), null); } catch { }

                            destAdapter.DeleteEntitiesDirectly(typeof(ReportFormulaAccountEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(ReportFormulaEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(ReportItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(ReportCategoryEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(ReportSectionEntity), null);

                            int totalNodes = ResetGraphForInsert(srcSections);
                            destAdapter.SaveEntityCollection(srcSections, false, true);

                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 22,
                                Message = $"[THIẾT KẾ] Bảng CĐKT (B01-DN): Đã chuyển {srcSections.Count} phần gốc ({totalNodes} bản ghi toàn cây)."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế Bảng CĐKT: {ex.Message}" });
                    }

                    // 2. THIẾT KẾ BÁO CÁO KẾT QUẢ HĐKD (B02-DN: BusinessResultItem -> AccountBusinessResult)
                    try
                    {
                        var srcBrItems = new EntityCollection<BusinessResultItemEntity>();
                        var prefetch = new PrefetchPath2(EntityType.BusinessResultItemEntity);
                        prefetch.Add(BusinessResultItemEntity.PrefetchPathAccountBusinessResults);
                        srcAdapter.FetchEntityCollection(srcBrItems, null, prefetch);

                        if (srcBrItems.Count > 0)
                        {
                            destAdapter.DeleteEntitiesDirectly(typeof(AccountBusinessResultEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(BusinessResultItemEntity), null);

                            int totalNodes = ResetGraphForInsert(srcBrItems);
                            destAdapter.SaveEntityCollection(srcBrItems, false, true);

                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 23,
                                Message = $"[THIẾT KẾ] Báo cáo KQ HĐKD (B02-DN): Đã chuyển {srcBrItems.Count} chỉ tiêu ({totalNodes} bản ghi toàn cây)."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế KQ HĐKD: {ex.Message}" });
                    }

                    // 3. THIẾT KẾ BÁO CÁO LƯU CHUYỂN TIỀN TỆ (B03-DN: CashFlowCategory -> CashFlowItem -> AccountCashFlowItem)
                    try
                    {
                        var srcCfCats = new EntityCollection<CashFlowCategoryEntity>();
                        var prefetch = new PrefetchPath2(EntityType.CashFlowCategoryEntity);
                        prefetch.Add(CashFlowCategoryEntity.PrefetchPathCashFlowItems)
                                .SubPath.Add(CashFlowItemEntity.PrefetchPathAccountCashFlowItems);
                        srcAdapter.FetchEntityCollection(srcCfCats, null, prefetch);

                        if (srcCfCats.Count > 0)
                        {
                            try { destAdapter.DeleteEntitiesDirectly(typeof(CashFlowPreviousPeriodEntity), null); } catch { }
                            destAdapter.DeleteEntitiesDirectly(typeof(AccountCashFlowItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(CashFlowItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(CashFlowCategoryEntity), null);

                            int totalNodes = ResetGraphForInsert(srcCfCats);
                            destAdapter.SaveEntityCollection(srcCfCats, false, true);

                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 24,
                                Message = $"[THIẾT KẾ] Lưu chuyển tiền tệ (B03-DN): Đã chuyển {srcCfCats.Count} nhóm ({totalNodes} bản ghi toàn cây)."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế LCTT: {ex.Message}" });
                    }

                    // 4. THIẾT KẾ BÁO CÁO NGHĨA VỤ VỚI NSNN (TaxObligation -> TaxObligationItem -> AccountTaxObligation)
                    try
                    {
                        var srcTaxObs = new EntityCollection<TaxObligationEntity>();
                        var prefetch = new PrefetchPath2(EntityType.TaxObligationEntity);
                        prefetch.Add(TaxObligationEntity.PrefetchPathTaxObligationItems)
                                .SubPath.Add(TaxObligationItemEntity.PrefetchPathAccountTaxObligations);
                        srcAdapter.FetchEntityCollection(srcTaxObs, null, prefetch);

                        if (srcTaxObs.Count > 0)
                        {
                            destAdapter.DeleteEntitiesDirectly(typeof(AccountTaxObligationEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(TaxObligationItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(TaxObligationEntity), null);

                            int totalNodes = ResetGraphForInsert(srcTaxObs);
                            destAdapter.SaveEntityCollection(srcTaxObs, false, true);

                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 25,
                                Message = $"[THIẾT KẾ] Nghĩa vụ NSNN: Đã chuyển {srcTaxObs.Count} nhóm ({totalNodes} bản ghi toàn cây)."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế Nghĩa vụ NSNN: {ex.Message}" });
                    }

                    // 5. THIẾT KẾ TỜ KHAI THUẾ GTGT KHẤU TRỪ (Vat3TaxItem -> AccountVat3Item)
                    try
                    {
                        var srcVat3 = new EntityCollection<Vat3TaxItemEntity>();
                        var prefetch = new PrefetchPath2(EntityType.Vat3TaxItemEntity);
                        prefetch.Add(Vat3TaxItemEntity.PrefetchPathAccountVat3Items);
                        srcAdapter.FetchEntityCollection(srcVat3, null, prefetch);

                        if (srcVat3.Count > 0)
                        {
                            destAdapter.DeleteEntitiesDirectly(typeof(AccountVat3ItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(Vat3TaxItemEntity), null);

                            int totalNodes = ResetGraphForInsert(srcVat3);
                            destAdapter.SaveEntityCollection(srcVat3, false, true);

                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 26,
                                Message = $"[THIẾT KẾ] Tờ khai thuế GTGT: Đã chuyển {srcVat3.Count} chỉ tiêu ({totalNodes} bản ghi toàn cây)."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế Tờ khai VAT: {ex.Message}" });
                    }

                    // 6. THIẾT KẾ TỜ KHAI THUẾ GTGT MẪU 01/GTGT (VatDeclareItem -> AccountVatDeclareItem)
                    try
                    {
                        var srcVatDec = new EntityCollection<VatDeclareItemEntity>();
                        var prefetch = new PrefetchPath2(EntityType.VatDeclareItemEntity);
                        prefetch.Add(VatDeclareItemEntity.PrefetchPathAccountVatDeclareItems);
                        srcAdapter.FetchEntityCollection(srcVatDec, null, prefetch);

                        if (srcVatDec.Count > 0)
                        {
                            destAdapter.DeleteEntitiesDirectly(typeof(AccountVatDeclareItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(VatDeclareItemEntity), null);

                            int totalNodes = ResetGraphForInsert(srcVatDec);
                            destAdapter.SaveEntityCollection(srcVatDec, false, true);

                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 27,
                                Message = $"[THIẾT KẾ] Tờ khai 01/GTGT: Đã chuyển {srcVatDec.Count} chỉ tiêu ({totalNodes} bản ghi toàn cây)."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế Tờ khai 01/GTGT: {ex.Message}" });
                    }

                    // 7. THIẾT KẾ SỔ CHI PHÍ / THẺ TÍNH GIÁ THÀNH (CostCalculatingBook & CustomCostReport)
                    try
                    {
                        var srcCostBooks = new EntityCollection<CostCalculatingBookEntity>();
                        var prefetchBook = new PrefetchPath2(EntityType.CostCalculatingBookEntity);
                        prefetchBook.Add(CostCalculatingBookEntity.PrefetchPathCostCalculatingItems)
                                    .SubPath.Add(CostCalculatingItemEntity.PrefetchPathAccountCostCalculatingItems);
                        srcAdapter.FetchEntityCollection(srcCostBooks, null, prefetchBook);

                        if (srcCostBooks.Count > 0)
                        {
                            destAdapter.DeleteEntitiesDirectly(typeof(AccountCostCalculatingItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(CostCalculatingItemEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(CostCalculatingBookEntity), null);

                            ResetGraphForInsert(srcCostBooks);
                            destAdapter.SaveEntityCollection(srcCostBooks, false, true);
                        }

                        var srcCustomCost = new EntityCollection<CustomCostReportEntity>();
                        var prefetchCustom = new PrefetchPath2(EntityType.CustomCostReportEntity);
                        prefetchCustom.Add(CustomCostReportEntity.PrefetchPathCustomCostReportAccounts);
                        srcAdapter.FetchEntityCollection(srcCustomCost, null, prefetchCustom);

                        if (srcCustomCost.Count > 0)
                        {
                            destAdapter.DeleteEntitiesDirectly(typeof(CustomCostReportAccountEntity), null);
                            destAdapter.DeleteEntitiesDirectly(typeof(CustomCostReportEntity), null);

                            ResetGraphForInsert(srcCustomCost);
                            destAdapter.SaveEntityCollection(srcCustomCost, false, true);
                        }

                        if (srcCostBooks.Count > 0 || srcCustomCost.Count > 0)
                        {
                            progress?.Report(new TransferProgressReport
                            {
                                Percent = 27,
                                Message = $"[THIẾT KẾ] Sổ chi phí & Giá thành: Đã chuyển {srcCostBooks.Count} thẻ giá thành, {srcCustomCost.Count} mẫu báo cáo chi phí."
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        progress?.Report(new TransferProgressReport { IsError = true, Message = $"[CẢNH BÁO] Lỗi đồng bộ Thiết kế Giá thành: {ex.Message}" });
                    }
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 27,
                StepTitle = "Thiết kế: Hoàn tất đồng bộ toàn bộ các bảng thiết kế báo cáo!",
                Message = "[HOÀN THÀNH BƯỚC THIẾT KẾ] Toàn bộ các bảng thiết kế công thức báo cáo đã sẵn sàng!"
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var coa = new EntityCollection<ChartOfAccountEntity>();
                    srcAdapter.FetchEntityCollection(coa, null);

                    var noDetailAccountIds = coa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && (string.IsNullOrWhiteSpace(a.CategoryId) || a.CategoryId.Trim().ToUpper() == "I"))
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var entries = new EntityCollection<JournalEntryEntity>();
                    srcAdapter.FetchEntityCollection(entries, null);

                    var flatBalances = entries
                        .Where(e => !string.IsNullOrWhiteSpace(e.AccountId) && noDetailAccountIds.Contains(e.AccountId.Trim()))
                        .GroupBy(e => e.AccountId.Trim())
                        .Select(g =>
                        {
                            double net = g.Sum(x => (double)((x.Amount ?? 0.0) * (x.Dbcr ?? 0)));
                            return new NormalBalanceDto
                            {
                                AccountId = g.Key,
                                Amount = Math.Abs(net),
                                Dbcr = (short)(net >= 0 ? 1 : -1)
                            };
                        })
                        .Where(x => x.Amount > 0.001)
                        .ToList();

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destNormalAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && (string.IsNullOrWhiteSpace(a.CategoryId) || a.CategoryId.Trim().ToUpper() == "I"))
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter));

                    var oldNormalEntries = destEntries
                        .Where(e => !string.IsNullOrWhiteSpace(e.AccountId) && destNormalAccIds.Contains(e.AccountId.Trim()))
                        .ToList();

                    if (oldNormalEntries.Count > 0)
                    {
                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldNormalEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);
                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldNormalEntries.Count} dòng số dư đầu kỳ tài khoản thông thường cũ tại CSDL đích."
                        });
                    }

                    int saved = 0;
                    foreach (var b in flatBalances)
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = b.AccountId,
                            Dbcr = b.Dbcr,
                            Amount = b.Amount,
                            Createdate = context.OpeningDate
                        };
                        destAdapter.SaveEntity(je, true, true);
                        saved++;
                    }

                    return saved;
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var prefetch = new PrefetchPath2(EntityType.DebtDetailEntity);
                    prefetch.Add(DebtDetailEntity.PrefetchPathJournalEntry);

                    var debtDetails = new EntityCollection<DebtDetailEntity>();
                    srcAdapter.FetchEntityCollection(debtDetails, null, prefetch);

                    // Lấy tập hợp Partner, DebtType, DebtReason hợp lệ tại đích để đảm bảo an toàn khóa ngoại
                    var destPartners = new EntityCollection<PartnerEntity>();
                    destAdapter.FetchEntityCollection(destPartners, null);
                    var validPartnerIds = destPartners.Where(p => !string.IsNullOrWhiteSpace(p.Id)).Select(p => p.Id.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var destDt = new EntityCollection<DebtTypeEntity>();
                    destAdapter.FetchEntityCollection(destDt, null);
                    var validDtIds = destDt.Where(d => !string.IsNullOrWhiteSpace(d.Id)).Select(d => d.Id.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var destDr = new EntityCollection<DebtReasonEntity>();
                    destAdapter.FetchEntityCollection(destDr, null);
                    var validDrIds = destDr.Select(r => r.Id).ToHashSet();

                    var flatDebts = debtDetails
                        .Where(d => d.JournalEntry != null && !string.IsNullOrWhiteSpace(d.JournalEntry.AccountId))
                        .GroupBy(d => new
                        {
                            AccountId = d.JournalEntry.AccountId.Trim(),
                            PartnerId = CleanStr(d.PartnerId) ?? string.Empty,
                            DebtReasonId = d.DebtReasonId,
                            DebtTypeId = CleanStr(d.DebtTypeId) ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double net = g.Sum(x => (double)((x.Amount ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
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

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ CÔNG NỢ CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destDebtAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && a.CategoryId?.Trim().ToUpper() == "A")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var prefetchDestDebt = new PrefetchPath2(EntityType.JournalEntryEntity);
                    prefetchDestDebt.Add(JournalEntryEntity.PrefetchPathDebtDetails);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter), prefetchDestDebt);

                    var oldDebtEntries = destEntries
                        .Where(e => (!string.IsNullOrWhiteSpace(e.AccountId) && destDebtAccIds.Contains(e.AccountId.Trim())) || e.DebtDetails.Count > 0)
                        .ToList();

                    if (oldDebtEntries.Count > 0)
                    {
                        var oldDetails = new EntityCollection<DebtDetailEntity>();
                        foreach (var je in oldDebtEntries)
                        {
                            foreach (var dd in je.DebtDetails)
                            {
                                oldDetails.Add(dd);
                            }
                        }

                        if (oldDetails.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldDetails);
                        }

                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldDebtEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);

                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldDetails.Count} chi tiết công nợ và {oldDebtEntries.Count} bút toán số dư công nợ cũ tại CSDL đích."
                        });
                    }

                    int savedEntries = 0;
                    foreach (var grp in flatDebts.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            Createdate = context.OpeningDate
                        };

                        foreach (var item in grp)
                        {
                            var d = new DebtDetailEntity
                            {
                                Amount = item.Amount
                            };

                            if (!string.IsNullOrEmpty(item.PartnerId))
                            {
                                if (!validPartnerIds.Contains(item.PartnerId))
                                {
                                    destAdapter.SaveEntity(new PartnerEntity { Id = item.PartnerId, CustomerName = item.PartnerId }, false, false);
                                    validPartnerIds.Add(item.PartnerId);
                                }
                                d.PartnerId = item.PartnerId;
                            }

                            if (!string.IsNullOrEmpty(item.DebtTypeId))
                            {
                                if (!validDtIds.Contains(item.DebtTypeId))
                                {
                                    destAdapter.SaveEntity(new DebtTypeEntity { Id = item.DebtTypeId, TypeName = item.DebtTypeId }, false, false);
                                    validDtIds.Add(item.DebtTypeId);
                                }
                                d.DebtTypeId = item.DebtTypeId;
                            }

                            if (item.DebtReasonId.HasValue && validDrIds.Contains(item.DebtReasonId.Value))
                            {
                                d.DebtReasonId = item.DebtReasonId.Value;
                            }

                            je.DebtDetails.Add(d);
                        }

                        destAdapter.SaveEntity(je, true, true);
                        savedEntries++;
                    }

                    return (flatDebts.Count, savedEntries);
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var prefetch = new PrefetchPath2(EntityType.InventoryVoucherLineEntity);
                    var pVoucher = prefetch.Add(InventoryVoucherLineEntity.PrefetchPathInventoryVoucher);
                    pVoucher.SubPath.Add(InventoryVoucherEntity.PrefetchPathJournalEntry);

                    var lines = new EntityCollection<InventoryVoucherLineEntity>();
                    srcAdapter.FetchEntityCollection(lines, null, prefetch);

                    // Đảm bảo kho và vật tư tồn tại ở đích
                    var destWh = new EntityCollection<WarehouseEntity>();
                    destAdapter.FetchEntityCollection(destWh, null);
                    var validWhIds = destWh.Where(w => !string.IsNullOrWhiteSpace(w.WarehouseId)).Select(w => w.WarehouseId.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var destItems = new EntityCollection<InventoryItemEntity>();
                    destAdapter.FetchEntityCollection(destItems, null);
                    var validItemIds = destItems.Where(i => !string.IsNullOrWhiteSpace(i.MaterialId)).Select(i => i.MaterialId.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var flatMaterials = lines
                        .Where(l => l.InventoryVoucher != null && l.InventoryVoucher.JournalEntry != null
                                 && !string.IsNullOrWhiteSpace(l.InventoryVoucher.JournalEntry.AccountId)
                                 && !string.IsNullOrWhiteSpace(l.InventoryVoucher.WarehouseId)
                                 && !string.IsNullOrWhiteSpace(l.MaterialId))
                        .GroupBy(l => new
                        {
                            AccountId = l.InventoryVoucher.JournalEntry.AccountId.Trim(),
                            WarehouseId = l.InventoryVoucher.WarehouseId.Trim(),
                            MaterialId = l.MaterialId.Trim()
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.InventoryVoucher.JournalEntry.Dbcr ?? 0)));
                            double netAmt = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.Price ?? 0.0) * (x.InventoryVoucher.JournalEntry.Dbcr ?? 0)));
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

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ VẬT TƯ CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destMatAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && a.CategoryId?.Trim().ToUpper() == "B")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var prefetchDestMat = new PrefetchPath2(EntityType.JournalEntryEntity);
                    var pVoucherDest = prefetchDestMat.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                    pVoucherDest.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter), prefetchDestMat);

                    var oldMatEntries = destEntries
                        .Where(e => (!string.IsNullOrWhiteSpace(e.AccountId) && destMatAccIds.Contains(e.AccountId.Trim())) || e.InventoryVouchers.Count > 0)
                        .ToList();

                    if (oldMatEntries.Count > 0)
                    {
                        var oldLines = new EntityCollection<InventoryVoucherLineEntity>();
                        var oldVouchers = new EntityCollection<InventoryVoucherEntity>();

                        foreach (var je in oldMatEntries)
                        {
                            foreach (var iv in je.InventoryVouchers)
                            {
                                foreach (var line in iv.InventoryVoucherLines)
                                {
                                    oldLines.Add(line);
                                }
                                oldVouchers.Add(iv);
                            }
                        }

                        if (oldLines.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldLines);
                        }
                        if (oldVouchers.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldVouchers);
                        }

                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldMatEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);

                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldLines.Count} dòng chi tiết vật tư, {oldVouchers.Count} phiếu nhập NNB và {oldMatEntries.Count} bút toán số dư vật tư cũ tại CSDL đích."
                        });
                    }

                    int savedEntries = 0;
                    int savedVouchers = 0;

                    // Quan hệ JournalEntry - InventoryVoucher là 1-nhiều:
                    // Mỗi Tài khoản vật tư (AccountId) chỉ tạo 1 dòng JournalEntry duy nhất trên bảng Số dư đầu kỳ,
                    // bên trong chứa nhiều InventoryVoucher (mỗi kho là 1 InventoryVoucher).
                    foreach (var accGrp in flatMaterials.GroupBy(x => x.AccountId))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = accGrp.Key,
                            Dbcr = 1, // Tồn kho luôn là Debit
                            Amount = accGrp.Sum(x => x.Amount),
                            Createdate = context.OpeningDate
                        };

                        foreach (var whGrp in accGrp.GroupBy(x => x.WarehouseId))
                        {
                            if (!validWhIds.Contains(whGrp.Key))
                            {
                                destAdapter.SaveEntity(new WarehouseEntity { WarehouseId = whGrp.Key, WarehouseName = whGrp.Key }, false, false);
                                validWhIds.Add(whGrp.Key);
                            }

                            var iv = new InventoryVoucherEntity
                            {
                                WarehouseId = whGrp.Key,
                                Taxflag = false, // Phiếu nhập nội bộ NNB
                                Printflag = false
                            };

                            foreach (var mat in whGrp)
                            {
                                if (!validItemIds.Contains(mat.MaterialId))
                                {
                                    destAdapter.SaveEntity(new InventoryItemEntity { MaterialId = mat.MaterialId, MaterialName = mat.MaterialId }, false, false);
                                    validItemIds.Add(mat.MaterialId);
                                }

                                var line = new InventoryVoucherLineEntity
                                {
                                    MaterialId = mat.MaterialId,
                                    Quantity = mat.Quantity,
                                    Price = mat.Price
                                };
                                iv.InventoryVoucherLines.Add(line);
                            }

                            je.InventoryVouchers.Add(iv);
                            savedVouchers++;
                        }

                        destAdapter.SaveEntity(je, true, true);
                        savedEntries++;
                    }

                    return (flatMaterials.Count, savedVouchers);
                }
            });

            progress?.Report(new TransferProgressReport
            {
                Percent = 64,
                StepTitle = "3/7. Hoàn tất sinh phiếu nhập kho nội bộ (NNB) cho vật tư tồn kho.",
                Message = $"[THÀNH CÔNG] Đã sinh {voucherCount} phiếu nhập kho NNB (gắn vào các TK vật tư theo quan hệ 1-nhiều) với {matCount} dòng vật tư tồn kho sang CSDL mới."
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var prefetch = new PrefetchPath2(EntityType.CostDetailEntity);
                    prefetch.Add(CostDetailEntity.PrefetchPathJournalEntry);

                    var costs = new EntityCollection<CostDetailEntity>();
                    srcAdapter.FetchEntityCollection(costs, null, prefetch);

                    var destCo = new EntityCollection<CostObjectEntity>();
                    destAdapter.FetchEntityCollection(destCo, null);
                    var validCoIds = destCo.Where(c => !string.IsNullOrWhiteSpace(c.Id)).Select(c => c.Id.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var destCe = new EntityCollection<CostElementEntity>();
                    destAdapter.FetchEntityCollection(destCe, null);
                    var validCeIds = destCe.Where(c => !string.IsNullOrWhiteSpace(c.Id)).Select(c => c.Id.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var flatCosts = costs
                        .Where(c => c.JournalEntry != null && !string.IsNullOrWhiteSpace(c.JournalEntry.AccountId))
                        .GroupBy(c => new
                        {
                            AccountId = c.JournalEntry.AccountId.Trim(),
                            CostObjectId = CleanStr(c.CostObjectId) ?? string.Empty,
                            CostElementId = CleanStr(c.CostElementId) ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double net = g.Sum(x => (double)((x.Amount ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
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

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ CHI PHÍ CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destCostAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && a.CategoryId?.Trim().ToUpper() == "E")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var prefetchDestCost = new PrefetchPath2(EntityType.JournalEntryEntity);
                    prefetchDestCost.Add(JournalEntryEntity.PrefetchPathCostDetails);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter), prefetchDestCost);

                    var oldCostEntries = destEntries
                        .Where(e => (!string.IsNullOrWhiteSpace(e.AccountId) && destCostAccIds.Contains(e.AccountId.Trim())) || e.CostDetails.Count > 0)
                        .ToList();

                    if (oldCostEntries.Count > 0)
                    {
                        var oldDetails = new EntityCollection<CostDetailEntity>();
                        foreach (var je in oldCostEntries)
                        {
                            foreach (var cd in je.CostDetails)
                            {
                                oldDetails.Add(cd);
                            }
                        }

                        if (oldDetails.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldDetails);
                        }

                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldCostEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);

                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldDetails.Count} chi tiết chi phí và {oldCostEntries.Count} bút toán số dư chi phí cũ tại CSDL đích."
                        });
                    }

                    foreach (var grp in flatCosts.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            Createdate = context.OpeningDate
                        };

                        foreach (var item in grp)
                        {
                            var d = new CostDetailEntity
                            {
                                Amount = item.Amount
                            };

                            if (!string.IsNullOrEmpty(item.CostObjectId))
                            {
                                if (!validCoIds.Contains(item.CostObjectId))
                                {
                                    destAdapter.SaveEntity(new CostObjectEntity { Id = item.CostObjectId, ObjectName = item.CostObjectId }, false, false);
                                    validCoIds.Add(item.CostObjectId);
                                }
                                d.CostObjectId = item.CostObjectId;
                            }

                            if (!string.IsNullOrEmpty(item.CostElementId))
                            {
                                if (!validCeIds.Contains(item.CostElementId))
                                {
                                    destAdapter.SaveEntity(new CostElementEntity { Id = item.CostElementId, ElementName = item.CostElementId }, false, false);
                                    validCeIds.Add(item.CostElementId);
                                }
                                d.CostElementId = item.CostElementId;
                            }

                            je.CostDetails.Add(d);
                        }

                        destAdapter.SaveEntity(je, true, true);
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var prefetch = new PrefetchPath2(EntityType.AssetDetailEntity);
                    prefetch.Add(AssetDetailEntity.PrefetchPathJournalEntry);

                    var assets = new EntityCollection<AssetDetailEntity>();
                    srcAdapter.FetchEntityCollection(assets, null, prefetch);

                    var destAssets = new EntityCollection<AssetEntity>();
                    destAdapter.FetchEntityCollection(destAssets, null);
                    var validAssetIds = destAssets.Where(a => !string.IsNullOrWhiteSpace(a.AssetId)).Select(a => a.AssetId.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var flatAssets = assets
                        .Where(a => a.JournalEntry != null && !string.IsNullOrWhiteSpace(a.JournalEntry.AccountId) && !string.IsNullOrWhiteSpace(a.AssetId))
                        .GroupBy(a => new
                        {
                            AccountId = a.JournalEntry.AccountId.Trim(),
                            AssetId = a.AssetId.Trim(),
                            SrCode = CleanStr(a.Srcode) ?? string.Empty,
                            ReasonId = CleanStr(a.Reasonid) ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
                            double netAmt = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.Price ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
                            double price = Math.Abs(netQty) > 0.0001 ? Math.Abs(netAmt / netQty) : 0;
                            return new AssetBalanceDto
                            {
                                AccountId = g.Key.AccountId,
                                AssetId = g.Key.AssetId,
                                SrCode = g.Key.SrCode,
                                ReasonId = g.Key.ReasonId,
                                Quantity = Math.Abs(netQty),
                                Price = price,
                                Amount = Math.Abs(netAmt),
                                Dbcr = (short)(netAmt >= 0 ? 1 : -1)
                            };
                        })
                        .Where(x => x.Quantity > 0.0001 && x.Amount > 0.0001)
                        .ToList();

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ TSCĐ CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destAssetAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && a.CategoryId?.Trim().ToUpper() == "D")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var prefetchDestAsset = new PrefetchPath2(EntityType.JournalEntryEntity);
                    prefetchDestAsset.Add(JournalEntryEntity.PrefetchPathAssetDetails);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter), prefetchDestAsset);

                    var oldAssetEntries = destEntries
                        .Where(e => (!string.IsNullOrWhiteSpace(e.AccountId) && destAssetAccIds.Contains(e.AccountId.Trim())) || e.AssetDetails.Count > 0)
                        .ToList();

                    if (oldAssetEntries.Count > 0)
                    {
                        var oldDetails = new EntityCollection<AssetDetailEntity>();
                        foreach (var je in oldAssetEntries)
                        {
                            foreach (var ad in je.AssetDetails)
                            {
                                oldDetails.Add(ad);
                            }
                        }

                        if (oldDetails.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldDetails);
                        }

                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldAssetEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);

                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldDetails.Count} chi tiết TSCĐ và {oldAssetEntries.Count} bút toán số dư TSCĐ cũ tại CSDL đích."
                        });
                    }

                    int savedEntries = 0;
                    foreach (var grp in flatAssets.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            Createdate = context.OpeningDate
                        };

                        foreach (var item in grp)
                        {
                            if (!validAssetIds.Contains(item.AssetId))
                            {
                                destAdapter.SaveEntity(new AssetEntity { AssetId = item.AssetId, AssetName = item.AssetId }, false, false);
                                validAssetIds.Add(item.AssetId);
                            }

                            var d = new AssetDetailEntity
                            {
                                AssetId = item.AssetId,
                                Quantity = item.Quantity,
                                Price = item.Price
                            };
                            if (!string.IsNullOrEmpty(item.SrCode)) d.Srcode = item.SrCode;
                            if (!string.IsNullOrEmpty(item.ReasonId)) d.Reasonid = item.ReasonId;

                            je.AssetDetails.Add(d);
                        }

                        destAdapter.SaveEntity(je, true, true);
                        savedEntries++;
                    }

                    return (flatAssets.Count, savedEntries);
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var prefetch = new PrefetchPath2(EntityType.InvestmentDetailEntity);
                    prefetch.Add(InvestmentDetailEntity.PrefetchPathJournalEntry);

                    var stocks = new EntityCollection<InvestmentDetailEntity>();
                    srcAdapter.FetchEntityCollection(stocks, null, prefetch);

                    var flatStocks = stocks
                        .Where(s => s.JournalEntry != null && !string.IsNullOrWhiteSpace(s.JournalEntry.AccountId) && !string.IsNullOrWhiteSpace(s.Stockid))
                        .GroupBy(s => new
                        {
                            AccountId = s.JournalEntry.AccountId.Trim(),
                            StockId = s.Stockid.Trim(),
                            CustomerId = CleanStr(s.CustomerId) ?? string.Empty
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
                            double netAmt = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.Inprice ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
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

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ CỔ PHIẾU CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destStockAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && a.CategoryId?.Trim().ToUpper() == "S")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var prefetchDestStock = new PrefetchPath2(EntityType.JournalEntryEntity);
                    prefetchDestStock.Add(JournalEntryEntity.PrefetchPathInvestmentDetails);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter), prefetchDestStock);

                    var oldStockEntries = destEntries
                        .Where(e => (!string.IsNullOrWhiteSpace(e.AccountId) && destStockAccIds.Contains(e.AccountId.Trim())) || e.InvestmentDetails.Count > 0)
                        .ToList();

                    if (oldStockEntries.Count > 0)
                    {
                        var oldDetails = new EntityCollection<InvestmentDetailEntity>();
                        foreach (var je in oldStockEntries)
                        {
                            foreach (var id in je.InvestmentDetails)
                            {
                                oldDetails.Add(id);
                            }
                        }

                        if (oldDetails.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldDetails);
                        }

                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldStockEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);

                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldDetails.Count} chi tiết cổ phiếu và {oldStockEntries.Count} bút toán số dư cổ phiếu cũ tại CSDL đích."
                        });
                    }

                    foreach (var grp in flatStocks.GroupBy(x => x.AccountId))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key,
                            Dbcr = 1, // Cổ phiếu luôn là Debit
                            Amount = grp.Sum(x => x.Amount),
                            Createdate = context.OpeningDate
                        };

                        foreach (var item in grp)
                        {
                            var d = new InvestmentDetailEntity
                            {
                                Stockid = item.StockId,
                                Quantity = item.Quantity,
                                Inprice = item.Price
                            };
                            if (!string.IsNullOrEmpty(item.CustomerId)) d.CustomerId = item.CustomerId;
                            je.InvestmentDetails.Add(d);
                        }

                        destAdapter.SaveEntity(je, true, true);
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
                using (var destAdapter = CreateDestAdapter(context.DestinationConfig))
                {
                    var prefetch = new PrefetchPath2(EntityType.CurrencyDetailEntity);
                    prefetch.Add(CurrencyDetailEntity.PrefetchPathJournalEntry);

                    var currencies = new EntityCollection<CurrencyDetailEntity>();
                    srcAdapter.FetchEntityCollection(currencies, null, prefetch);

                    var flatCurrencies = currencies
                        .Where(c => c.JournalEntry != null && !string.IsNullOrWhiteSpace(c.JournalEntry.AccountId) && !string.IsNullOrWhiteSpace(c.CurrencyId))
                        .GroupBy(c => new
                        {
                            AccountId = c.JournalEntry.AccountId.Trim(),
                            CurrencyId = c.CurrencyId.Trim()
                        })
                        .Select(g =>
                        {
                            double netQty = g.Sum(x => (double)((x.Quantity ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
                            double netAmt = g.Sum(x => (double)((x.JournalEntry.Amount ?? 0.0) * (x.JournalEntry.Dbcr ?? 0)));
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

                    // DỌN DẸP SỐ DƯ ĐẦU KỲ NGOẠI TỆ CŨ TẠI CSDL ĐÍCH (IDEMPOTENT)
                    var destCoa = new EntityCollection<ChartOfAccountEntity>();
                    destAdapter.FetchEntityCollection(destCoa, null);
                    var destCurAccIds = destCoa
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && a.CategoryId?.Trim().ToUpper() == "C")
                        .Select(a => a.AccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var prefetchDestCur = new PrefetchPath2(EntityType.JournalEntryEntity);
                    prefetchDestCur.Add(JournalEntryEntity.PrefetchPathCurrencyDetails);

                    var destEntries = new EntityCollection<JournalEntryEntity>();
                    var filter = (JournalEntryFields.JournalVoucherId == DBNull.Value | JournalEntryFields.JournalVoucherId == 0);
                    destAdapter.FetchEntityCollection(destEntries, new RelationPredicateBucket(filter), prefetchDestCur);

                    var oldCurEntries = destEntries
                        .Where(e => (!string.IsNullOrWhiteSpace(e.AccountId) && destCurAccIds.Contains(e.AccountId.Trim())) || e.CurrencyDetails.Count > 0)
                        .ToList();

                    if (oldCurEntries.Count > 0)
                    {
                        var oldDetails = new EntityCollection<CurrencyDetailEntity>();
                        foreach (var je in oldCurEntries)
                        {
                            foreach (var cd in je.CurrencyDetails)
                            {
                                oldDetails.Add(cd);
                            }
                        }

                        if (oldDetails.Count > 0)
                        {
                            destAdapter.DeleteEntityCollection(oldDetails);
                        }

                        var colToDelete = new EntityCollection<JournalEntryEntity>(oldCurEntries);
                        destAdapter.DeleteEntityCollection(colToDelete);

                        progress?.Report(new TransferProgressReport
                        {
                            Message = $"[DỌN DẸP] Đã xóa {oldDetails.Count} chi tiết ngoại tệ và {oldCurEntries.Count} bút toán số dư ngoại tệ cũ tại CSDL đích."
                        });
                    }

                    foreach (var grp in flatCurrencies.GroupBy(x => new { x.AccountId, x.Dbcr }))
                    {
                        var je = new JournalEntryEntity
                        {
                            AccountId = grp.Key.AccountId,
                            Dbcr = grp.Key.Dbcr,
                            Amount = grp.Sum(x => x.Amount),
                            Createdate = context.OpeningDate
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

                        destAdapter.SaveEntity(je, true, true);
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
                Message = $"Đang kiểm tra kết nối tới CSDL đích ({BuildConnectionString(context.DestinationConfig)})..."
            });

            bool isConnected = await TestDestinationConnectionAsync(context.DestinationConfig);
            if (!isConnected)
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 0,
                    IsError = true,
                    StepTitle = "Kết nối CSDL đích thất bại!",
                    Message = "Không thể kết nối tới CSDL đích. Vui lòng kiểm tra lại cấu hình CSDL mới trước khi chuyển đổi!"
                });
                return;
            }

            progress?.Report(new TransferProgressReport
            {
                Percent = 5,
                StepTitle = "Kết nối CSDL đích thành công! Bắt đầu chuyển danh mục...",
                Message = "Kết nối CSDL đích thành công! Bắt đầu các bước đồng bộ..."
            });

            // 2. BƯỚC TIỀN ĐỀ 1: Đồng bộ các bảng Thư viện / Danh mục & Tài khoản người dùng
            if (context.TransferLibrary)
            {
                try
                {
                    await TransferDictionariesAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 15,
                    StepTitle = "Bỏ qua: Đồng bộ Thư viện / Danh mục (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua bước đồng bộ Thư viện / Danh mục."
                });
            }

            // 2b. BƯỚC TIỀN ĐỀ 2: Đồng bộ các bảng Thiết kế Báo cáo tài chính, Thuế & Giá thành
            if (context.TransferDesign)
            {
                try
                {
                    await TransferDesignTablesAsync(context, progress!);
                }
                catch (Exception ex)
                {
                    progress?.Report(new TransferProgressReport
                    {
                        IsError = true,
                        StepTitle = "Lỗi tại bước Đồng bộ các bảng Thiết kế báo cáo!",
                        Message = $"[LỖI] {ex.Message}"
                    });
                    throw;
                }
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 27,
                    StepTitle = "Bỏ qua: Đồng bộ Bảng thiết kế báo cáo (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua bước đồng bộ Thiết kế báo cáo."
                });
            }

            // 3. Chuyển số dư tài khoản thông thường (Category 'I')
            if (context.TransferNormal)
            {
                try
                {
                    await TransferNoDetailsAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 38,
                    StepTitle = "Bỏ qua: Số dư tài khoản thông thường (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển số dư tài khoản thông thường."
                });
            }

            // 4. Chuyển số dư công nợ (Category 'A' -> DebtDetail)
            if (context.TransferDebt)
            {
                try
                {
                    await TransferDebtAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 50,
                    StepTitle = "Bỏ qua: Số dư công nợ chi tiết (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển số dư công nợ chi tiết."
                });
            }

            // 5. Chuyển số dư vật tư hàng hóa (Category 'B' -> Phiếu nhập kho nội bộ NNB)
            if (context.TransferMaterial)
            {
                try
                {
                    await TransferMaterialAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 64,
                    StepTitle = "Bỏ qua: Tồn kho vật tư hàng hóa (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển tồn kho vật tư hàng hóa."
                });
            }

            // 6. Chuyển số dư chi phí dở dang (Category 'E' -> CostDetail)
            if (context.TransferCost)
            {
                try
                {
                    await TransferCostAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 74,
                    StepTitle = "Bỏ qua: Chi phí SXKD dở dang (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển chi phí SXKD dở dang."
                });
            }

            // 7. Chuyển số dư tài sản cố định (Category 'D' -> AssetDetail)
            if (context.TransferAsset)
            {
                try
                {
                    await TransferAssetAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 84,
                    StepTitle = "Bỏ qua: Tài sản cố định (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển tài sản cố định."
                });
            }

            // 8. Chuyển số dư cổ phiếu / đầu tư tài chính (Category 'S' -> InvestmentDetail)
            if (context.TransferStock)
            {
                try
                {
                    await TransferStockAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 92,
                    StepTitle = "Bỏ qua: Cổ phiếu & Đầu tư chứng khoán (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển cổ phiếu đầu tư."
                });
            }

            // 9. Chuyển số dư ngoại tệ (Category 'C' -> CurrencyDetail)
            if (context.TransferCurrency)
            {
                try
                {
                    await TransferCurrencyAsync(context, progress!);
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
            }
            else
            {
                progress?.Report(new TransferProgressReport
                {
                    Percent = 98,
                    StepTitle = "Bỏ qua: Ngoại tệ (theo tùy chọn)",
                    Message = "[BỎ QUA] Bỏ qua chuyển số dư ngoại tệ."
                });
            }

            // 10. Hoàn tất toàn bộ
            progress?.Report(new TransferProgressReport
            {
                Percent = 100,
                StepTitle = "ĐÃ HOÀN TẤT CHUYỂN ĐỔI SỐ DƯ CÁC PHÂN HỆ ĐÃ CHỌN!",
                Message = "Chúc mừng! Đã hoàn tất xử lý chuyển đổi số dư các phân hệ đã chọn sang niên độ mới an toàn và chính xác!"
            });
        }
    }
}
