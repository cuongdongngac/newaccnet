using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Core.Data;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Core.Posting
{
    /// <summary>
    /// Cổng hạch toán chứng từ kho cốt lõi của NewaccNet.
    /// Quản lý toàn bộ đồ thị thực thể (JournalVoucher -> JournalEntries -> InventoryVoucher -> Lines)
    /// và ghi vết tường minh 100% qua EntryGraph JSON trong một Transaction duy nhất.
    /// </summary>
    public static class WarehousePostingGateway
    {
        public static InventoryVoucherConfigItemDto? GetPostingRule(string type)
        {
            try
            {
                var cfg = DbConfigService.Current.GetVoucherPostingConfig();
                if (cfg?.Inventory != null && cfg.Inventory.TryGetValue(type, out var item))
                {
                    return item;
                }
            }
            catch { }
            return null;
        }

        public static List<WarehouseEntity> LoadWarehouses()
        {
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<WarehouseEntity>();
            adapter.FetchEntityCollection(list, null);
            return list.OrderBy(w => w.WarehouseId).ToList();
        }

        public static List<InventoryItemEntity> LoadMaterials()
        {
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<InventoryItemEntity>();
            adapter.FetchEntityCollection(list, null);
            return list.OrderBy(m => m.MaterialId).ToList();
        }

        public static List<PartnerEntity> LoadPartners()
        {
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<PartnerEntity>();
            adapter.FetchEntityCollection(list, null);
            return list.OrderBy(p => p.Id).ToList();
        }

        public static List<TaxRateOption> LoadTaxRates()
        {
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<TaxRateEntity>();
            adapter.FetchEntityCollection(list, null);
            return list.Select(t => new TaxRateOption
            {
                Taxrateid = t.Taxrateid,
                Rate = t.Rate ?? 0
            }).OrderBy(t => t.Rate).ToList();
        }

        public static List<AccountOption> LoadAccounts(string? prefixFilter = null)
        {
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<ChartOfAccountEntity>();
            adapter.FetchEntityCollection(list, null);

            var parentIds = list.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
            var query = list.Where(a => !parentIds.Contains(a.AccountId) && a.CategoryId != "B");

            if (!string.IsNullOrEmpty(prefixFilter))
            {
                query = query.Where(a => a.AccountId.StartsWith(prefixFilter));
            }

            return query.OrderBy(a => a.AccountId)
                        .Select(a => new AccountOption
                        {
                            AccountId = a.AccountId,
                            AccountName = a.AccountName ?? string.Empty
                        }).ToList();
        }

        public static string GenerateNextVoucherNo(string prefix)
        {
            string datePart = DateTime.Today.ToString("yyMMdd");
            string fullPrefix = $"{prefix}{datePart}";

            using var adapter = AppDataAccessAdapter.Create();
            var col = new EntityCollection<JournalVoucherEntity>();
            var bucket = new RelationPredicateBucket(new FieldLikePredicate(JournalVoucherFields.VoucherNo, null, fullPrefix + "%"));
            adapter.FetchEntityCollection(col, bucket);

            int maxSeq = 0;
            foreach (var item in col)
            {
                if (!string.IsNullOrEmpty(item.VoucherNo) && item.VoucherNo.Length > fullPrefix.Length)
                {
                    string seqPart = item.VoucherNo.Substring(fullPrefix.Length);
                    if (int.TryParse(seqPart, out int s) && s > maxSeq)
                    {
                        maxSeq = s;
                    }
                }
            }

            return $"{fullPrefix}{(maxSeq + 1):D2}";
        }

        public static List<int> GetVoucherIds(string typeOrPrefix)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();

                string normType = typeOrPrefix switch
                {
                    "PN" or "NM" => "NM",
                    "PX" or "XB" => "XB",
                    "NN" or "NNB" => "NNB",
                    "XN" or "XNB" => "XNB",
                    "CK" => "CK",
                    _ => typeOrPrefix.ToUpperInvariant()
                };

                // 1. Đi từ InventoryVoucher: Dò lên JournalEntry (quan hệ 1-1 tự nhiên)
                var ivCol = new EntityCollection<InventoryVoucherEntity>();
                var prefetch = new PrefetchPath2(EntityType.InventoryVoucherEntity);
                var pJe = prefetch.Add(InventoryVoucherEntity.PrefetchPathJournalEntry);
                pJe.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);
                pJe.SubPath.Add(JournalEntryEntity.PrefetchPathJournalVoucher);
                var sorter = new SortExpression(InventoryVoucherFields.Id | SortOperator.Ascending);

                adapter.FetchEntityCollection(ivCol, null, 0, sorter, prefetch);

                var resultIds = new List<int>();
                var seenJvIds = new HashSet<int>();

                // Đếm số lượng iv trên mỗi jv để nhận diện phiếu chuyển kho CK
                var jvIvCount = new Dictionary<int, int>();
                foreach (var iv in ivCol)
                {
                    var je = iv.JournalEntry;
                    int? jvId = je?.JournalVoucherId ?? je?.ParentEntry?.JournalVoucherId;
                    if (jvId.HasValue && jvId.Value > 0)
                    {
                        jvIvCount[jvId.Value] = jvIvCount.GetValueOrDefault(jvId.Value, 0) + 1;
                    }
                }

                foreach (var iv in ivCol)
                {
                    var je = iv.JournalEntry;
                    int? jvId = je?.JournalVoucherId ?? je?.ParentEntry?.JournalVoucherId;
                    if (!jvId.HasValue || jvId.Value <= 0) continue;

                    // Xác định loại phiếu bằng thuật toán 1-1 tự nhiên
                    string computedType;
                    if (jvIvCount.TryGetValue(jvId.Value, out int count) && count >= 2)
                    {
                        computedType = "CK";
                    }
                    else if (je != null)
                    {
                        computedType = je.Dbcr == -1 
                            ? (iv.Taxflag ? "XB" : "XNB") 
                            : (iv.Taxflag ? "NM" : "NNB");
                    }
                    else
                    {
                        computedType = iv.VoucherType ?? "NM";
                    }

                    if (computedType.Equals(normType, StringComparison.OrdinalIgnoreCase))
                    {
                        if (seenJvIds.Add(jvId.Value))
                        {
                            resultIds.Add(jvId.Value);
                        }
                    }
                }

                // 2. Fallback nếu không có InventoryVoucher mà chỉ có JournalVoucher với tiền tố số chứng từ
                if (resultIds.Count == 0)
                {
                    var col = new EntityCollection<JournalVoucherEntity>();
                    var jvBucket = new RelationPredicateBucket(new FieldLikePredicate(JournalVoucherFields.VoucherNo, null, typeOrPrefix + "%"));
                    var jvSorter = new SortExpression(JournalVoucherFields.VoucherDate | SortOperator.Ascending);
                    jvSorter.Add(JournalVoucherFields.Id | SortOperator.Ascending);
                    adapter.FetchEntityCollection(col, jvBucket, 0, jvSorter);
                    return col.Select(v => v.Id).ToList();
                }

                return resultIds;
            }
            catch
            {
                return new List<int>();
            }
        }

        public static WarehouseVoucherSaveParams? LoadVoucherDetailByInventoryVoucher(int inventoryVoucherId)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var iv = new InventoryVoucherEntity(inventoryVoucherId);
                var prefetch = new PrefetchPath2(EntityType.InventoryVoucherEntity);
                var pJe = prefetch.Add(InventoryVoucherEntity.PrefetchPathJournalEntry);
                pJe.SubPath.Add(JournalEntryEntity.PrefetchPathParentEntry);

                if (adapter.FetchEntity(iv, prefetch))
                {
                    var je = iv.JournalEntry;
                    int? jvId = je?.JournalVoucherId ?? je?.ParentEntry?.JournalVoucherId;
                    if (jvId.HasValue && jvId.Value > 0)
                    {
                        return LoadVoucherDetail(jvId.Value);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static WarehouseVoucherSaveParams? LoadVoucherDetail(int journalVoucherId)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var jv = new JournalVoucherEntity(journalVoucherId);
                var prefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
                var pEntries = prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathSubEntries);
                var pIv = pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                pIv.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);

                if (!adapter.FetchEntity(jv, prefetch)) return null;

                var p = new WarehouseVoucherSaveParams
                {
                    VoucherId = jv.Id,
                    VoucherNo = jv.VoucherNo ?? string.Empty,
                    VoucherDate = jv.VoucherDate ?? DateTime.Today,
                    Description = jv.Contents ?? string.Empty,
                    Carrier = jv.Personname ?? string.Empty
                };

                InventoryVoucherEntity? iv = null;
                InventoryVoucherEntity? ivTo = null;
                JournalEntryEntity? matEntry = null;

                foreach (var je in jv.JournalEntries)
                {
                    if (je.InventoryVouchers.Count > 0)
                    {
                        if (iv == null)
                        {
                            iv = je.InventoryVouchers[0];
                            matEntry = je;
                        }
                        else
                        {
                            ivTo = je.InventoryVouchers[0];
                        }
                    }

                    foreach (var sub in je.SubEntries)
                    {
                        if (sub.InventoryVouchers.Count > 0)
                        {
                            if (iv == null)
                            {
                                iv = sub.InventoryVouchers[0];
                                matEntry = sub;
                            }
                            else
                            {
                                ivTo = sub.InventoryVouchers[0];
                            }
                        }
                    }
                }

                if (iv != null)
                {
                    p.VoucherType = iv.VoucherType ?? "NM";
                    p.WarehouseId = iv.WarehouseId ?? string.Empty;
                    p.FromWarehouseId = iv.WarehouseId ?? string.Empty;
                    if (ivTo != null) p.ToWarehouseId = ivTo.WarehouseId ?? string.Empty;

                    p.Carrier = string.IsNullOrEmpty(iv.Carrier) ? p.Carrier : iv.Carrier;
                    p.VatBillNo = iv.Vatbillno ?? string.Empty;
                    p.SerialNo = iv.Serialno ?? string.Empty;
                    p.VatBillDate = iv.Vatbilldate;
                    p.PartnerId = iv.Vatobjectid ?? string.Empty;
                    p.PartnerName = iv.Vatobjectname ?? string.Empty;
                    p.Address = iv.Vataddress ?? string.Empty;
                    p.TaxCode = iv.Taxno ?? string.Empty;

                    foreach (var line in iv.InventoryVoucherLines)
                    {
                        var item = new WarehouseVoucherLineItem
                        {
                            MaterialId = line.MaterialId ?? string.Empty,
                            Quantity = line.Quantity ?? 0,
                            Price = line.Price ?? 0,
                            SellPrice = line.SellPrice ?? 0,
                            Taxrateid = line.Taxrateid
                        };
                        if (line.Rate.HasValue) item.TaxRate = line.Rate.Value;
                        p.Lines.Add(item);
                    }
                }

                if (matEntry != null)
                {
                    p.InventoryAccountId = matEntry.AccountId ?? "1561";
                }

                // Tường minh 100% nếu có EntryGraph JSON
                bool loadedFromGraph = false;
                if (iv != null && !string.IsNullOrWhiteSpace(iv.EntryGraph))
                {
                    try
                    {
                        var graph = JsonSerializer.Deserialize<VoucherEntryGraphDto>(iv.EntryGraph);
                        if (graph != null)
                        {
                            var allEntries = new List<JournalEntryEntity>();
                            allEntries.AddRange(jv.JournalEntries);
                            foreach (var j in jv.JournalEntries) allEntries.AddRange(j.SubEntries);

                            if (graph.CostEntryId.HasValue)
                            {
                                var e = allEntries.FirstOrDefault(x => x.Id == graph.CostEntryId.Value);
                                if (e != null && !string.IsNullOrEmpty(e.AccountId)) p.CostAccountId = e.AccountId;
                            }
                            if (graph.RevEntryId.HasValue)
                            {
                                var e = allEntries.FirstOrDefault(x => x.Id == graph.RevEntryId.Value);
                                if (e != null && !string.IsNullOrEmpty(e.AccountId)) p.RevenueAccountId = e.AccountId;
                            }
                            if (graph.TaxEntryId.HasValue)
                            {
                                var e = allEntries.FirstOrDefault(x => x.Id == graph.TaxEntryId.Value);
                                if (e != null && !string.IsNullOrEmpty(e.AccountId)) p.TaxAccountId = e.AccountId;
                            }
                            if (graph.TotalEntryId.HasValue || graph.RevMasterEntryId.HasValue)
                            {
                                int totId = graph.TotalEntryId ?? graph.RevMasterEntryId!.Value;
                                var e = allEntries.FirstOrDefault(x => x.Id == totId);
                                if (e != null && !string.IsNullOrEmpty(e.AccountId)) p.TotalAccountId = e.AccountId;
                            }
                            if (graph.OffsetEntryId.HasValue)
                            {
                                var e = allEntries.FirstOrDefault(x => x.Id == graph.OffsetEntryId.Value);
                                if (e != null && !string.IsNullOrEmpty(e.AccountId)) p.OffsetAccountId = e.AccountId;
                            }
                            if (graph.MaterialEntryId.HasValue)
                            {
                                var e = allEntries.FirstOrDefault(x => x.Id == graph.MaterialEntryId.Value);
                                if (e != null && !string.IsNullOrEmpty(e.AccountId)) p.InventoryAccountId = e.AccountId;
                            }

                            loadedFromGraph = true;
                        }
                    }
                    catch { }
                }

                // Fallback nếu là dữ liệu cũ chưa có EntryGraph
                if (!loadedFromGraph)
                {
                    foreach (var je in jv.JournalEntries)
                    {
                        if (je.AccountId != null)
                        {
                            if (je.AccountId.StartsWith("331") || je.AccountId.StartsWith("111") || je.AccountId.StartsWith("112"))
                                p.TotalAccountId = je.AccountId;
                            else if (je.AccountId.StartsWith("131"))
                                p.TotalAccountId = je.AccountId;
                            else if (je.AccountId.StartsWith("632"))
                                p.CostAccountId = je.AccountId;
                            else if (je.AccountId.StartsWith("511"))
                                p.RevenueAccountId = je.AccountId;
                            else if (je.AccountId.StartsWith("154") || je.AccountId.StartsWith("621") || je.AccountId.StartsWith("642") || je.AccountId.StartsWith("711") || je.AccountId.StartsWith("811"))
                                p.OffsetAccountId = je.AccountId;
                        }

                        foreach (var sub in je.SubEntries)
                        {
                            if (sub.AccountId != null)
                            {
                                if (sub.AccountId.StartsWith("133") || sub.AccountId.StartsWith("333"))
                                    p.TaxAccountId = sub.AccountId;
                                else if (sub.AccountId.StartsWith("511"))
                                    p.RevenueAccountId = sub.AccountId;
                                else if (sub.AccountId.StartsWith("154") || sub.AccountId.StartsWith("621") || sub.AccountId.StartsWith("642") || sub.AccountId.StartsWith("711") || sub.AccountId.StartsWith("811"))
                                    p.OffsetAccountId = sub.AccountId;
                            }
                        }
                    }
                }

                return p;
            }
            catch
            {
                return null;
            }
        }

        public static bool DeleteVoucher(int journalVoucherId, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (journalVoucherId <= 0) return true;

            using var adapter = AppDataAccessAdapter.Create();
            try
            {
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "DeleteWarehouseVoucher");
                DeleteVoucherInternal(adapter, journalVoucherId);
                adapter.Commit();
                return true;
            }
            catch (Exception ex)
            {
                adapter.Rollback();
                errorMessage = $"Lỗi khi xóa chứng từ: {ex.Message}";
                return false;
            }
        }

        private static void DeleteVoucherInternal(IDataAccessAdapter adapter, int jvId)
        {
            var jv = new JournalVoucherEntity(jvId);
            var prefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
            var pEntries = prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
            pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathSubEntries);
            pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathDebtDetails);
            pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathVatEntries);
            var pIv = pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
            var pLines = pIv.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);
            pLines.SubPath.Add(InventoryVoucherLineEntity.PrefetchPathMaterialTaxLine);

            if (adapter.FetchEntity(jv, prefetch))
            {
                foreach (var je in jv.JournalEntries)
                {
                    DeleteEntryChildren(adapter, je);
                    foreach (var sub in je.SubEntries)
                    {
                        DeleteEntryChildren(adapter, sub);
                        adapter.DeleteEntity(sub);
                    }
                    adapter.DeleteEntity(je);
                }
                adapter.DeleteEntity(jv);
            }
        }

        private static void DeleteEntryChildren(IDataAccessAdapter adapter, JournalEntryEntity je)
        {
            foreach (var d in je.DebtDetails) adapter.DeleteEntity(d);
            foreach (var v in je.VatEntries) adapter.DeleteEntity(v);
            foreach (var iv in je.InventoryVouchers)
            {
                foreach (var l in iv.InventoryVoucherLines)
                {
                    if (l.MaterialTaxLine != null) adapter.DeleteEntity(l.MaterialTaxLine);
                    adapter.DeleteEntity(l);
                }
                adapter.DeleteEntity(iv);
            }
        }

        public static bool SaveVoucher(WarehouseVoucherSaveParams p, out string errorMessage, out int voucherId)
        {
            errorMessage = string.Empty;
            voucherId = 0;

            if (p.Lines == null || p.Lines.Count == 0)
            {
                errorMessage = "Vui lòng nhập ít nhất một dòng chi tiết vật tư, hàng hóa.";
                return false;
            }

            using var adapter = AppDataAccessAdapter.Create();

            try
            {
                adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "SaveWarehouseVoucher");

                // Nếu là cập nhật phiếu cũ: Xóa graph cũ trong cùng transaction trước khi lưu graph mới
                if (p.VoucherId > 0)
                {
                    DeleteVoucherInternal(adapter, p.VoucherId);
                }

                // 1. Tạo JournalVoucherEntity (chứng từ kế toán cha)
                var jv = new JournalVoucherEntity
                {
                    VoucherNo = p.VoucherNo.Trim(),
                    VoucherDate = p.VoucherDate,
                    Contents = p.Description.Trim(),
                    Personname = p.Carrier.Trim(),
                    Bookflag = true
                };

                double totalCost = p.Lines.Sum(l => l.Amount);
                double totalSell = p.Lines.Sum(l => l.SellAmount);
                double totalTax = p.Lines.Sum(l => l.TaxAmount);
                double totalAll = (p.VoucherType == "XB") ? (totalSell + totalTax) : (totalCost + totalTax);

                string whId = !string.IsNullOrEmpty(p.WarehouseId) ? p.WarehouseId : p.FromWarehouseId;

                // 2. Tạo InventoryVoucherEntity
                var iv = new InventoryVoucherEntity
                {
                    WarehouseId = whId,
                    VoucherType = p.VoucherType,
                    Carrier = p.Carrier,
                    Taxflag = (p.VoucherType == "NM" || p.VoucherType == "XB"),
                    Printflag = !string.IsNullOrEmpty(p.VatBillNo),
                    Vatbillno = p.VatBillNo,
                    Serialno = p.SerialNo,
                    Vatbilldate = p.VatBillDate,
                    Vatobjectid = p.PartnerId,
                    Vatobjectname = p.PartnerName,
                    Vataddress = p.Address,
                    Taxno = p.TaxCode
                };

                // Dòng chi tiết InventoryVoucherLineEntity
                foreach (var line in p.Lines)
                {
                    if (string.IsNullOrWhiteSpace(line.MaterialId)) continue;

                    var ivLine = new InventoryVoucherLineEntity
                    {
                        MaterialId = line.MaterialId.Trim(),
                        Quantity = line.Quantity,
                        Price = line.Price
                    };

                    if (iv.Taxflag)
                    {
                        ivLine.SellPrice = line.SellPrice;
                        if (line.Taxrateid.HasValue)
                        {
                            ivLine.Taxrateid = (short)line.Taxrateid.Value;
                        }
                        ivLine.Rate = line.TaxRate;
                    }

                    iv.InventoryVoucherLines.Add(ivLine);
                }

                // 3. Ráp nối JournalEntryEntity chuẩn Nợ/Có theo từng loại phiếu
                var graphDto = new VoucherEntryGraphDto();

                if (p.VoucherType == "NM") // Nhập mua
                {
                    var master = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.TotalAccountId) ? "3311" : p.TotalAccountId,
                        Dbcr = -1,
                        Amount = totalAll
                    };
                    jv.JournalEntries.Add(master);

                    var subMat = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.InventoryAccountId) ? "1561" : p.InventoryAccountId,
                        Dbcr = 1,
                        Amount = totalCost
                    };
                    subMat.InventoryVouchers.Add(iv);
                    master.SubEntries.Add(subMat);

                    JournalEntryEntity? subTax = null;
                    if (totalTax > 0)
                    {
                        subTax = new JournalEntryEntity
                        {
                            AccountId = string.IsNullOrWhiteSpace(p.TaxAccountId) ? "13311" : p.TaxAccountId,
                            Dbcr = 1,
                            Amount = totalTax
                        };
                        master.SubEntries.Add(subTax);

                        if (!string.IsNullOrEmpty(p.VatBillNo))
                        {
                            subTax.VatEntries.Add(new VatEntryEntity
                            {
                                Vatbillno = p.VatBillNo,
                                Serialno = p.SerialNo,
                                Vatbilldate = p.VatBillDate ?? p.VoucherDate,
                                Vatobjectid = p.PartnerId,
                                Vatobjectname = p.PartnerName,
                                Taxno = p.TaxCode,
                                Beforetaxamount = totalCost,
                                Amount = totalTax,
                                Rate = (p.Lines.FirstOrDefault(l => l.TaxRate > 0)?.TaxRate ?? 0.1) * 100,
                                Vatdescription = p.Description,
                                Type = 1 // Đầu vào
                            });
                        }
                    }

                    if (!string.IsNullOrEmpty(p.PartnerId) && master.AccountId.StartsWith("331"))
                    {
                        master.DebtDetails.Add(new DebtDetailEntity
                        {
                            PartnerId = p.PartnerId,
                            Amount = totalAll,
                            Liabilitydate = p.VoucherDate
                        });
                    }
                }
                else if (p.VoucherType == "XB") // Xuất bán
                {
                    var costMaster = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.CostAccountId) ? "6321" : p.CostAccountId,
                        Dbcr = 1,
                        Amount = totalCost
                    };
                    jv.JournalEntries.Add(costMaster);

                    var costSub = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.InventoryAccountId) ? "1561" : p.InventoryAccountId,
                        Dbcr = -1,
                        Amount = totalCost
                    };
                    costSub.InventoryVouchers.Add(iv);
                    costMaster.SubEntries.Add(costSub);

                    var revMaster = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.TotalAccountId) ? "131" : p.TotalAccountId,
                        Dbcr = 1,
                        Amount = totalAll
                    };
                    jv.JournalEntries.Add(revMaster);

                    var revSub = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.RevenueAccountId) ? "5111" : p.RevenueAccountId,
                        Dbcr = -1,
                        Amount = totalSell
                    };
                    revMaster.SubEntries.Add(revSub);

                    if (totalTax > 0)
                    {
                        var revTaxSub = new JournalEntryEntity
                        {
                            AccountId = string.IsNullOrWhiteSpace(p.TaxAccountId) ? "33311" : p.TaxAccountId,
                            Dbcr = -1,
                            Amount = totalTax
                        };
                        revMaster.SubEntries.Add(revTaxSub);

                        if (!string.IsNullOrEmpty(p.VatBillNo))
                        {
                            revTaxSub.VatEntries.Add(new VatEntryEntity
                            {
                                Vatbillno = p.VatBillNo,
                                Serialno = p.SerialNo,
                                Vatbilldate = p.VatBillDate ?? p.VoucherDate,
                                Vatobjectid = p.PartnerId,
                                Vatobjectname = p.PartnerName,
                                Taxno = p.TaxCode,
                                Beforetaxamount = totalSell,
                                Amount = totalTax,
                                Rate = (p.Lines.FirstOrDefault(l => l.TaxRate > 0)?.TaxRate ?? 0.1) * 100,
                                Vatdescription = p.Description,
                                Type = 2 // Đầu ra
                            });
                        }
                    }

                    if (!string.IsNullOrEmpty(p.PartnerId) && revMaster.AccountId.StartsWith("131"))
                    {
                        revMaster.DebtDetails.Add(new DebtDetailEntity
                        {
                            PartnerId = p.PartnerId,
                            Amount = totalAll,
                            Liabilitydate = p.VoucherDate
                        });
                    }
                }
                else if (p.VoucherType == "NNB") // Nhập nội bộ
                {
                    var master = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.OffsetAccountId) ? "1541" : p.OffsetAccountId,
                        Dbcr = -1,
                        Amount = totalCost
                    };
                    jv.JournalEntries.Add(master);

                    var sub = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.InventoryAccountId) ? "1561" : p.InventoryAccountId,
                        Dbcr = 1,
                        Amount = totalCost
                    };
                    sub.InventoryVouchers.Add(iv);
                    master.SubEntries.Add(sub);
                }
                else if (p.VoucherType == "XNB") // Xuất nội bộ / sản xuất / chi phí
                {
                    var master = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.OffsetAccountId) ? "6211" : p.OffsetAccountId,
                        Dbcr = 1,
                        Amount = totalCost
                    };
                    jv.JournalEntries.Add(master);

                    var sub = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.InventoryAccountId) ? "152" : p.InventoryAccountId,
                        Dbcr = -1,
                        Amount = totalCost
                    };
                    sub.InventoryVouchers.Add(iv);
                    master.SubEntries.Add(sub);
                }
                else // CK - Điều chuyển kho nội bộ
                {
                    var master = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.InventoryAccountId) ? "1561" : p.InventoryAccountId,
                        Dbcr = -1, // Xuất kho nguồn
                        Amount = totalCost
                    };
                    iv.WarehouseId = p.FromWarehouseId;
                    master.InventoryVouchers.Add(iv);
                    jv.JournalEntries.Add(master);

                    var sub = new JournalEntryEntity
                    {
                        AccountId = string.IsNullOrWhiteSpace(p.InventoryAccountId) ? "1561" : p.InventoryAccountId,
                        Dbcr = 1, // Nhập kho đích
                        Amount = totalCost
                    };
                    var ivTo = new InventoryVoucherEntity
                    {
                        WarehouseId = p.ToWarehouseId,
                        VoucherType = "CK",
                        Carrier = p.Carrier,
                        Taxflag = false,
                        Printflag = false
                    };
                    foreach (var l in p.Lines)
                    {
                        if (string.IsNullOrWhiteSpace(l.MaterialId)) continue;
                        ivTo.InventoryVoucherLines.Add(new InventoryVoucherLineEntity
                        {
                            MaterialId = l.MaterialId.Trim(),
                            Quantity = l.Quantity,
                            Price = l.Price
                        });
                    }
                    sub.InventoryVouchers.Add(ivTo);
                    master.SubEntries.Add(sub);
                }

                // 4. Lưu toàn bộ đồ thị thực thể vào CSDL an toàn 100%
                bool success = adapter.SaveEntity(jv, true, true);
                if (success)
                {
                    try
                    {
                        if (p.VoucherType == "NM")
                        {
                            var master = jv.JournalEntries.FirstOrDefault();
                            var subMat = master?.SubEntries.FirstOrDefault(s => s.Dbcr == 1 && s.InventoryVouchers.Contains(iv));
                            var subTax = master?.SubEntries.FirstOrDefault(s => s != subMat);

                            graphDto.TotalEntryId = master?.Id;
                            graphDto.MaterialEntryId = subMat?.Id;
                            graphDto.TaxEntryId = subTax?.Id;
                        }
                        else if (p.VoucherType == "XB")
                        {
                            var costMaster = jv.JournalEntries.FirstOrDefault(j => j.AccountId != null && j.AccountId.StartsWith("632"));
                            var costSub = costMaster?.SubEntries.FirstOrDefault();
                            var revMaster = jv.JournalEntries.FirstOrDefault(j => j != costMaster);
                            var revSub = revMaster?.SubEntries.FirstOrDefault(s => s.AccountId != null && s.AccountId.StartsWith("511"));
                            var revTax = revMaster?.SubEntries.FirstOrDefault(s => s != revSub);

                            graphDto.CostEntryId = costMaster?.Id;
                            graphDto.MaterialEntryId = costSub?.Id;
                            graphDto.RevMasterEntryId = revMaster?.Id;
                            graphDto.RevEntryId = revSub?.Id;
                            graphDto.TaxEntryId = revTax?.Id;
                        }
                        else if (p.VoucherType == "NNB" || p.VoucherType == "XNB")
                        {
                            var master = jv.JournalEntries.FirstOrDefault();
                            var sub = master?.SubEntries.FirstOrDefault();

                            graphDto.OffsetEntryId = master?.Id;
                            graphDto.MaterialEntryId = sub?.Id;
                        }
                        else if (p.VoucherType == "CK")
                        {
                            var master = jv.JournalEntries.FirstOrDefault(j => j.Dbcr == -1);
                            var sub = jv.JournalEntries.SelectMany(j => j.SubEntries).FirstOrDefault();

                            graphDto.OffsetEntryId = master?.Id;
                            graphDto.MaterialEntryId = sub?.Id;
                        }

                        iv.EntryGraph = JsonSerializer.Serialize(graphDto);
                        adapter.SaveEntity(iv, false, false);
                    }
                    catch { }

                    adapter.Commit();
                    voucherId = jv.Id;
                    return true;
                }
                else
                {
                    adapter.Rollback();
                    errorMessage = "Không thể lưu chứng từ vào cơ sở dữ liệu.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                try { adapter.Rollback(); } catch { }
                errorMessage = "Lỗi khi lưu phiếu kho: " + ex.Message;
                return false;
            }
        }

        public static (int totalProcessed, int successCount, List<string> errors) MigrateLegacyEntryGraphs(
            IProgress<MigrationProgressInfo>? progress = null,
            System.Threading.CancellationToken cancellationToken = default)
        {
            var errors = new List<string>();
            int totalProcessed = 0;
            int successCount = 0;

            try
            {
                using var adapter = AppDataAccessAdapter.Create();

                var ivList = new EntityCollection<InventoryVoucherEntity>();
                var bucket = new RelationPredicateBucket(
                    (InventoryVoucherFields.EntryGraph == DBNull.Value) | (InventoryVoucherFields.EntryGraph == "")
                );
                var prefetch = new PrefetchPath2(EntityType.InventoryVoucherEntity);
                prefetch.Add(InventoryVoucherEntity.PrefetchPathJournalEntry);

                adapter.FetchEntityCollection(ivList, bucket, prefetch);
                totalProcessed = ivList.Count;

                if (totalProcessed == 0)
                {
                    progress?.Report(new MigrationProgressInfo
                    {
                        Current = 0,
                        Total = 0,
                        Success = 0,
                        Failed = 0,
                        CurrentMessage = "Tất cả các phiếu kho đã có cấu trúc đồ thị JSON đầy đủ."
                    });
                    return (0, 0, errors);
                }

                const int batchSize = 50;
                for (int i = 0; i < ivList.Count; i += batchSize)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    var batch = ivList.Skip(i).Take(batchSize).ToList();

                    // 1. Gom tất cả các jvId cần fetch trong lô này
                    var jvIdsToFetch = new HashSet<int>();
                    foreach (var iv in batch)
                    {
                        var je = iv.JournalEntry;
                        if (je == null && iv.JournalEntryId.HasValue)
                        {
                            je = new JournalEntryEntity(iv.JournalEntryId.Value);
                            adapter.FetchEntity(je);
                        }
                        if (je == null) continue;

                        int? jvId = je.JournalVoucherId ?? je.ParentEntry?.JournalVoucherId;
                        if (jvId.HasValue && jvId.Value > 0)
                        {
                            jvIdsToFetch.Add(jvId.Value);
                        }
                    }

                    // 2. Fetch toàn bộ JournalVoucher trong batch bằng 1 query duy nhất!
                    var jvDict = new Dictionary<int, JournalVoucherEntity>();
                    if (jvIdsToFetch.Count > 0)
                    {
                        var jvCollection = new EntityCollection<JournalVoucherEntity>();
                        var jvBucket = new RelationPredicateBucket(new FieldCompareRangePredicate(JournalVoucherFields.Id, null, jvIdsToFetch.ToArray()));
                        var jvPrefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
                        var pEntries = jvPrefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                        pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathSubEntries);
                        pEntries.SubPath.Add(JournalEntryEntity.PrefetchPathVatEntries);

                        adapter.FetchEntityCollection(jvCollection, jvBucket, jvPrefetch);
                        foreach (var item in jvCollection)
                        {
                            jvDict[item.Id] = item;
                        }
                    }

                    // 3. Phân tích đồ thị trên RAM và chuẩn bị batch lưu
                    var batchToSave = new EntityCollection<InventoryVoucherEntity>();

                    foreach (var iv in batch)
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                        try
                        {
                            var je = iv.JournalEntry;
                            if (je == null && iv.JournalEntryId.HasValue)
                            {
                                je = new JournalEntryEntity(iv.JournalEntryId.Value);
                                adapter.FetchEntity(je);
                            }
                            if (je == null) continue;

                            int? jvId = je.JournalVoucherId ?? je.ParentEntry?.JournalVoucherId;
                            if (!jvId.HasValue || !jvDict.TryGetValue(jvId.Value, out var jv)) continue;

                            var graphDto = new VoucherEntryGraphDto
                            {
                                MaterialEntryId = je.Id
                            };

                            string vType = iv.VoucherType ?? (je.Dbcr == -1 ? (iv.Taxflag ? "XB" : "XNB") : (iv.Taxflag ? "NM" : "NNB"));

                            if (vType == "NM")
                            {
                                var payMaster = je.ParentEntry ?? jv.JournalEntries.FirstOrDefault(x => x.Dbcr == -1);
                                var taxSub = payMaster?.SubEntries.FirstOrDefault(s => s.Id != je.Id)
                                             ?? jv.JournalEntries.FirstOrDefault(x => x.Dbcr == 1 && x.Id != je.Id && x.AccountId != null && x.AccountId.StartsWith("133"));

                                graphDto.TotalEntryId = payMaster?.Id;
                                graphDto.TaxEntryId = taxSub?.Id;
                            }
                            else if (vType == "XB")
                            {
                                var costMaster = je.ParentEntry ?? jv.JournalEntries.FirstOrDefault(x => x.AccountId != null && x.AccountId.StartsWith("632"));
                                var revMaster = jv.JournalEntries.FirstOrDefault(x => x != costMaster && (x.AccountId != null && (x.AccountId.StartsWith("131") || x.AccountId.StartsWith("111") || x.AccountId.StartsWith("112"))));
                                var revSub = revMaster?.SubEntries.FirstOrDefault(s => s.AccountId != null && s.AccountId.StartsWith("511"));
                                var taxSub = revMaster?.SubEntries.FirstOrDefault(s => s != revSub && (s.VatEntries.Count > 0 || (s.AccountId != null && s.AccountId.StartsWith("333"))));

                                graphDto.CostEntryId = costMaster?.Id;
                                graphDto.RevMasterEntryId = revMaster?.Id;
                                graphDto.RevEntryId = revSub?.Id;
                                graphDto.TaxEntryId = taxSub?.Id;
                            }
                            else if (vType == "NNB")
                            {
                                var offset = je.ParentEntry ?? jv.JournalEntries.FirstOrDefault(x => x.Dbcr == -1);
                                graphDto.OffsetEntryId = offset?.Id;
                            }
                            else if (vType == "XNB")
                            {
                                var offset = je.ParentEntry ?? jv.JournalEntries.FirstOrDefault(x => x.Dbcr == 1);
                                graphDto.OffsetEntryId = offset?.Id;
                            }
                            else if (vType == "CK")
                            {
                                var master = jv.JournalEntries.FirstOrDefault(x => x.Dbcr == -1);
                                graphDto.OffsetEntryId = master?.Id;
                            }

                            iv.EntryGraph = JsonSerializer.Serialize(graphDto);
                            batchToSave.Add(iv);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Phiếu kho ID {iv.Id}: {ex.Message}");
                        }
                    }

                    // 4. Lưu cả batch cập nhật bằng 1 lệnh duy nhất!
                    if (batchToSave.Count > 0)
                    {
                        adapter.SaveEntityCollection(batchToSave, false, false);
                    }

                    int processedSoFar = Math.Min(i + batch.Count, totalProcessed);
                    progress?.Report(new MigrationProgressInfo
                    {
                        Current = processedSoFar,
                        Total = totalProcessed,
                        Success = successCount,
                        Failed = errors.Count,
                        CurrentMessage = $"Đang xử lý {processedSoFar}/{totalProcessed} phiếu kho..."
                    });
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Lỗi truy vấn danh sách chuyển đổi: {ex.Message}");
            }

            return (totalProcessed, successCount, errors);
        }
    }
}
