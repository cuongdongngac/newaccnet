using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Core.Data;
using NewaccNet.Core.Posting;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Plugin.CashBank.Services
{
    public static class CashVoucherDataService
    {
        public static CashPostingConfigDto GetPostingConfig()
        {
            return DbConfigService.Current.GetCashPostingConfig();
        }

        public static List<PluginAccountConfigItemDto> LoadCashAccounts()
        {
            var config = GetPostingConfig();
            if (config.CashAccounts != null && config.CashAccounts.Count > 0)
            {
                return config.CashAccounts.Where(x => x.IsActive).ToList();
            }

            // Fallback: Tự động tải các tài khoản cấp chi tiết bắt đầu bằng 111 hoặc 112
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<ChartOfAccountEntity>();
            adapter.FetchEntityCollection(list, null);

            var parentIds = list.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
            return list.Where(a => !parentIds.Contains(a.AccountId) && (a.AccountId.StartsWith("111") || a.AccountId.StartsWith("112")))
                       .OrderBy(a => a.AccountId)
                       .Select(a => new PluginAccountConfigItemDto
                       {
                           AccountId = a.AccountId,
                           BaseName = a.AccountName ?? "",
                           CustomName = a.AccountName ?? "",
                           Category = a.AccountId.StartsWith("111") ? "Cash" : "Bank",
                           BaseCategoryId = "A",
                           IsActive = true
                       }).ToList();
        }

        public static List<PartnerEntity> LoadPartners()
        {
            using var adapter = AppDataAccessAdapter.Create();
            var list = new EntityCollection<PartnerEntity>();
            adapter.FetchEntityCollection(list, null);
            return list.OrderBy(p => p.Id).ToList();
        }

        public static List<AccountOption> LoadAccounts(string? prefixFilter = null)
        {
            return WarehousePostingGateway.LoadAccounts(prefixFilter);
        }

        public static string GenerateNextVoucherNo(string prefix)
        {
            string fullPrefix = prefix.ToUpperInvariant();
            int currentYear = DateTime.Today.Year;
            int maxSeq = 0;

            using var adapter = AppDataAccessAdapter.Create();
            var col = new EntityCollection<JournalVoucherEntity>();
            var bucket = new RelationPredicateBucket(
                new FieldLikePredicate(JournalVoucherFields.VoucherNo, null, fullPrefix + "%") &
                (JournalVoucherFields.VoucherDate >= new DateTime(currentYear, 1, 1)) &
                (JournalVoucherFields.VoucherDate <= new DateTime(currentYear, 12, 31)));

            adapter.FetchEntityCollection(col, bucket);

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

            return $"{fullPrefix}{(maxSeq + 1):D3}";
        }

        public static List<int> GetVoucherIds(string prefix)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var col = new EntityCollection<JournalVoucherEntity>();
                var bucket = new RelationPredicateBucket(new FieldLikePredicate(JournalVoucherFields.VoucherNo, null, prefix.ToUpperInvariant() + "%"));
                var sorter = new SortExpression(JournalVoucherFields.VoucherDate | SortOperator.Ascending);
                sorter.Add(JournalVoucherFields.Id | SortOperator.Ascending);
                adapter.FetchEntityCollection(col, bucket, 0, sorter);
                return col.Select(v => v.Id).ToList();
            }
            catch
            {
                return new List<int>();
            }
        }

        public static List<CashVoucherLookupDto> SearchVouchers(string prefix, bool unpostedOnly = false, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var results = new List<CashVoucherLookupDto>();
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var col = new EntityCollection<JournalVoucherEntity>();
                var bucket = new RelationPredicateBucket(new FieldLikePredicate(JournalVoucherFields.VoucherNo, null, prefix.ToUpperInvariant() + "%"));

                if (unpostedOnly)
                {
                    bucket.PredicateExpression.Add(JournalVoucherFields.Bookflag == false);
                }
                if (fromDate.HasValue)
                {
                    bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    bucket.PredicateExpression.Add(JournalVoucherFields.VoucherDate <= toDate.Value.Date.AddDays(1).AddTicks(-1));
                }

                var prefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
                prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);

                var sorter = new SortExpression(JournalVoucherFields.VoucherDate | SortOperator.Descending);
                sorter.Add(JournalVoucherFields.Id | SortOperator.Descending);

                adapter.FetchEntityCollection(col, bucket, 100, sorter, prefetch);

                foreach (var jv in col)
                {
                    var master = jv.JournalEntries.FirstOrDefault(e => e.ParentId == null || e.ParentId == 0);
                    decimal total = master?.Amount ?? jv.JournalEntries.Where(e => e.Dbcr == 1).Sum(e => e.Amount ?? 0);

                    results.Add(new CashVoucherLookupDto
                    {
                        Id = jv.Id,
                        VoucherNo = jv.VoucherNo ?? "",
                        VoucherDate = jv.VoucherDate ?? DateTime.Today,
                        VoucherType = prefix,
                        Description = jv.Contents ?? "",
                        PartnerName = jv.Personname ?? "",
                        CashAccount = master?.AccountId ?? "",
                        TotalAmount = total,
                        Bookflag = jv.Bookflag
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SearchVouchers error: " + ex.Message);
            }
            return results;
        }

        public static CashVoucherSaveParams? LoadVoucherDetail(int voucherId)
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var jv = new JournalVoucherEntity(voucherId);

                var prefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
                var pJe = prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                pJe.SubPath.Add(JournalEntryEntity.PrefetchPathDebtDetails);
                pJe.SubPath.Add(JournalEntryEntity.PrefetchPathCostDetails);

                if (!adapter.FetchEntity(jv, prefetch)) return null;

                string vNo = jv.VoucherNo ?? "";
                string vType = "PT";
                if (vNo.StartsWith("PC", StringComparison.OrdinalIgnoreCase)) vType = "PC";
                else if (vNo.StartsWith("BC", StringComparison.OrdinalIgnoreCase)) vType = "BC";
                else if (vNo.StartsWith("BN", StringComparison.OrdinalIgnoreCase)) vType = "BN";
                else if (vNo.StartsWith("CKT", StringComparison.OrdinalIgnoreCase) || vNo.StartsWith("CK", StringComparison.OrdinalIgnoreCase)) vType = "CKT";

                string method = (vType == "BC" || vType == "BN" || (vNo.Length > 0 && vNo.Contains("NH"))) ? "Bank" : "Cash";

                // Phân loại master entry và sub entries
                var master = jv.JournalEntries.FirstOrDefault(e => e.ParentId == null || e.ParentId == 0)
                             ?? jv.JournalEntries.FirstOrDefault(e => (e.AccountId ?? "").StartsWith("111") || (e.AccountId ?? "").StartsWith("112"));

                var p = new CashVoucherSaveParams
                {
                    VoucherId = jv.Id,
                    VoucherType = vType,
                    VoucherNo = jv.VoucherNo ?? "",
                    VoucherDate = jv.VoucherDate ?? DateTime.Today,
                    Bookflag = jv.Bookflag,
                    Method = method,
                    CashAccountId = master?.AccountId ?? (method == "Cash" ? "1111" : "1121"),
                    PersonName = jv.Personname ?? "",
                    PersonAddress = jv.Personaddress ?? "",
                    InvoicesNumber = jv.Invoicesnumber ?? "",
                    Description = jv.Contents ?? "",
                    Paymentacc = jv.Paymentacc ?? "",
                    Paymentbank = jv.Paymentbank ?? "",
                    Recievedcorp = jv.Recievedcorp ?? "",
                    Recievedacc = jv.Recievedacc ?? "",
                    Recievedbank = jv.Recievedbank ?? ""
                };

                var subEntries = jv.JournalEntries.Where(e => e != master).ToList();
                if (subEntries.Count == 0 && master != null)
                {
                    // Nếu chỉ có 1 dòng hoặc không có cấu trúc master/sub rõ ràng
                    subEntries = jv.JournalEntries.ToList();
                }

                int lineId = 1;
                foreach (var se in subEntries)
                {
                    if (se == master && subEntries.Count > 1) continue;

                    var line = new CashVoucherLineDto
                    {
                        LineId = lineId++,
                        AccountId = se.AccountId ?? "",
                        Description = p.Description,
                        Amount = se.Amount ?? 0m
                    };

                    // Nạp chi tiết công nợ
                    if (se.DebtDetails != null && se.DebtDetails.Count > 0)
                    {
                        line.DebtDetails = se.DebtDetails.ToList();
                    }

                    // Nạp chi tiết chi phí
                    if (se.CostDetails != null && se.CostDetails.Count > 0)
                    {
                        line.CostDetails = se.CostDetails.ToList();
                    }

                    p.Lines.Add(line);
                }

                return p;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadVoucherDetail error: " + ex.Message);
                return null;
            }
        }

        public static bool SaveVoucher(CashVoucherSaveParams p, out string errorMessage, out int savedVoucherId)
        {
            errorMessage = "";
            savedVoucherId = 0;

            if (p == null)
            {
                errorMessage = "Tham số chứng từ không hợp lệ!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(p.VoucherNo))
            {
                errorMessage = "Số chứng từ không được để trống!";
                return false;
            }

            if (p.Lines == null || p.Lines.Count == 0)
            {
                errorMessage = "Chứng từ phải có ít nhất 1 dòng định khoản chi tiết!";
                return false;
            }

            try
            {
                using var adapter = AppDataAccessAdapter.Create();

                if (p.VoucherId > 0)
                {
                    DeleteVoucherInternal(adapter, p.VoucherId);
                }

                // 1. Tạo JournalVoucherEntity
                var jv = new JournalVoucherEntity
                {
                    VoucherNo = p.VoucherNo.Trim(),
                    VoucherDate = p.VoucherDate,
                    Contents = p.Description.Trim(),
                    Personname = p.PersonName.Trim(),
                    Personaddress = p.PersonAddress.Trim(),
                    Invoicesnumber = p.InvoicesNumber.Trim(),
                    Paymentacc = p.Paymentacc.Trim(),
                    Paymentbank = p.Paymentbank.Trim(),
                    Recievedcorp = p.Recievedcorp.Trim(),
                    Recievedacc = p.Recievedacc.Trim(),
                    Recievedbank = p.Recievedbank.Trim(),
                    Bookflag = p.Bookflag // Mặc định là false theo yêu cầu ERP
                };

                decimal total = p.TotalAmount;
                bool isReceipt = p.VoucherType == "PT" || p.VoucherType == "BC";
                bool isTransfer = p.VoucherType == "CKT";

                if (isTransfer)
                {
                    // Chuyển tiền nội bộ: Có TK nguồn, Nợ TK đích
                    var outMaster = new JournalEntryEntity
                    {
                        AccountId = p.CashAccountId,
                        Dbcr = -1,
                        Amount = total
                    };
                    jv.JournalEntries.Add(outMaster);

                    var inSub = new JournalEntryEntity
                    {
                        AccountId = p.DestinationAccountId,
                        Dbcr = 1,
                        Amount = total
                    };
                    outMaster.SubEntries.Add(inSub);
                    jv.JournalEntries.Add(inSub);
                }
                else if (isReceipt)
                {
                    // Thu tiền: Nợ TK tiền (1111/1121), Có các TK đối ứng
                    var master = new JournalEntryEntity
                    {
                        AccountId = p.CashAccountId,
                        Dbcr = 1,
                        Amount = total
                    };
                    jv.JournalEntries.Add(master);

                    foreach (var line in p.Lines)
                    {
                        if (string.IsNullOrWhiteSpace(line.AccountId)) continue;

                        var sub = new JournalEntryEntity
                        {
                            AccountId = line.AccountId,
                            Dbcr = -1,
                            Amount = line.Amount
                        };
                        master.SubEntries.Add(sub);
                        jv.JournalEntries.Add(sub);

                        AttachLineDetails(sub, line, p.PartnerId, p.VoucherDate);
                    }
                }
                else // Payment: PC / BN
                {
                    // Chi tiền: Có TK tiền (1111/1121), Nợ các TK đối ứng
                    var master = new JournalEntryEntity
                    {
                        AccountId = p.CashAccountId,
                        Dbcr = -1,
                        Amount = total
                    };
                    jv.JournalEntries.Add(master);

                    foreach (var line in p.Lines)
                    {
                        if (string.IsNullOrWhiteSpace(line.AccountId)) continue;

                        var sub = new JournalEntryEntity
                        {
                            AccountId = line.AccountId,
                            Dbcr = 1,
                            Amount = line.Amount
                        };
                        master.SubEntries.Add(sub);
                        jv.JournalEntries.Add(sub);

                        AttachLineDetails(sub, line, p.PartnerId, p.VoucherDate);
                    }
                }

                // Ghi nhận vào CSDL
                adapter.SaveEntity(jv, true, true);
                savedVoucherId = jv.Id;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private static void AttachLineDetails(JournalEntryEntity entry, CashVoucherLineDto line, string? defaultPartnerId, DateTime voucherDate)
        {
            if (entry == null || line == null) return;

            string pid = !string.IsNullOrWhiteSpace(line.PartnerId) ? line.PartnerId : (defaultPartnerId ?? "");

            if (line.DebtDetails != null && line.DebtDetails.Count > 0)
            {
                foreach (var d in line.DebtDetails)
                {
                    entry.DebtDetails.Add(new DebtDetailEntity
                    {
                        PartnerId = !string.IsNullOrWhiteSpace(d.PartnerId) ? d.PartnerId : pid,
                        DebtTypeId = d.DebtTypeId,
                        DebtReasonId = d.DebtReasonId,
                        Amount = d.Amount > 0 ? d.Amount : line.Amount,
                        Liabilitydate = d.Liabilitydate ?? voucherDate
                    });
                }
            }
            else if (!string.IsNullOrWhiteSpace(pid) && (line.CategoryId == "A" || (line.AccountId.StartsWith("131") || line.AccountId.StartsWith("331") || line.AccountId.StartsWith("141"))))
            {
                entry.DebtDetails.Add(new DebtDetailEntity
                {
                    PartnerId = pid,
                    Amount = line.Amount,
                    Liabilitydate = voucherDate
                });
            }

            if (line.CostDetails != null && line.CostDetails.Count > 0)
            {
                foreach (var c in line.CostDetails)
                {
                    entry.CostDetails.Add(new CostDetailEntity
                    {
                        CostObjectId = c.CostObjectId,
                        CostElementId = c.CostElementId,
                        Amount = c.Amount > 0 ? c.Amount : line.Amount,
                        Serialno = c.Serialno,
                        Transflag = c.Transflag,
                        Liabilityflag = c.Liabilityflag
                    });
                }
            }
        }

        public static bool DeleteVoucher(int voucherId, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                DeleteVoucherInternal(adapter, voucherId);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private static void DeleteVoucherInternal(IDataAccessAdapter adapter, int voucherId)
        {
            var jv = new JournalVoucherEntity(voucherId);
            var prefetch = new PrefetchPath2(EntityType.JournalVoucherEntity);
            var pJe = prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
            pJe.SubPath.Add(JournalEntryEntity.PrefetchPathDebtDetails);
            pJe.SubPath.Add(JournalEntryEntity.PrefetchPathCostDetails);

            if (adapter.FetchEntity(jv, prefetch))
            {
                foreach (var je in jv.JournalEntries)
                {
                    adapter.DeleteEntityCollection(je.DebtDetails);
                    adapter.DeleteEntityCollection(je.CostDetails);
                }
                adapter.DeleteEntityCollection(jv.JournalEntries);
                adapter.DeleteEntity(jv);
            }
        }
    }
}
