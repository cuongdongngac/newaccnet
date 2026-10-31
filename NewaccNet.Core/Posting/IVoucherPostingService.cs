using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Contracts;
using NewaccNet.Core.Data;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Core.Posting
{
    public interface IVoucherPostingService
    {
        /// <summary>
        /// Lưu một chứng từ đơn lẻ vào CSDL trong 1 Transaction an toàn.
        /// </summary>
        PostingResult PostVoucher(JournalVoucherEntity voucher);

        /// <summary>
        /// Lưu một lô chứng từ vào CSDL trong 1 Transaction nguyên tử duy nhất.
        /// </summary>
        PostingResult PostBatch(IReadOnlyList<JournalVoucherEntity> vouchers, IReadOnlyList<JournalVoucherEntity>? deletedVouchers = null);

        /// <summary>
        /// Bất đồng bộ: Lưu một chứng từ đơn lẻ.
        /// </summary>
        Task<PostingResult> PostVoucherAsync(JournalVoucherEntity voucher, CancellationToken ct = default);

        /// <summary>
        /// Bất đồng bộ: Lưu một lô chứng từ.
        /// </summary>
        Task<PostingResult> PostBatchAsync(IReadOnlyList<JournalVoucherEntity> vouchers, IReadOnlyList<JournalVoucherEntity>? deletedVouchers = null, CancellationToken ct = default);
    }

    public class VoucherPostingService : IVoucherPostingService
    {
        private readonly IVoucherValidator _validator;

        public VoucherPostingService(IVoucherValidator? validator = null)
        {
            _validator = validator ?? new VoucherValidator();
        }

        public PostingResult PostVoucher(JournalVoucherEntity voucher)
        {
            if (voucher == null)
            {
                return new PostingResult
                {
                    Success = false,
                    ErrorMessage = "Chứng từ không tồn tại."
                };
            }

            return PostBatch(new[] { voucher });
        }

        public PostingResult PostBatch(IReadOnlyList<JournalVoucherEntity> vouchers, IReadOnlyList<JournalVoucherEntity>? deletedVouchers = null)
        {
            var result = new PostingResult();

            if (vouchers == null || vouchers.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Danh sách chứng từ cần lưu trống.";
                return result;
            }

            // 1. Kiểm tra tính hợp lệ bằng IVoucherValidator trước khi mở Transaction
            var issues = _validator.ValidateBatch(vouchers);
            var errors = issues.Where(i => i.Severity == IssueSeverity.Error).ToList();
            if (errors.Count > 0)
            {
                result.Success = false;
                result.Issues = issues;
                result.ErrorMessage = string.Join("\n", errors.Select(e => e.Message));
                return result;
            }

            // 2. Mở Transaction duy nhất trên AppDataAccessAdapter
            using var adapter = AppDataAccessAdapter.Create();
            adapter.StartTransaction(IsolationLevel.ReadCommitted, "VoucherPostingBatch");

            try
            {
                // Xóa các chứng từ bị đánh dấu xóa trong lô (nếu có)
                if (deletedVouchers != null)
                {
                    foreach (var del in deletedVouchers)
                    {
                        if (del.Id > 0)
                        {
                            var entity = new JournalVoucherEntity(del.Id) { IsNew = false };
                            adapter.DeleteEntity(entity);
                        }
                    }
                }

                // Lưu từng chứng từ trong lô với xử lý xóa vi sai (differential deletion)
                foreach (var v in vouchers)
                {
                    SaveSingleVoucherInternal(adapter, v);
                }

                adapter.Commit();

                result.Success = true;
                if (vouchers.Count == 1)
                {
                    result.VoucherId = vouchers[0].Id;
                    result.VoucherNo = vouchers[0].VoucherNo ?? string.Empty;
                }
                result.Issues = issues;
                return result;
            }
            catch (Exception ex)
            {
                try { adapter.Rollback(); } catch { }
                result.Success = false;
                result.ErrorMessage = "Lỗi khi ghi sổ chứng từ: " + ex.Message;
                return result;
            }
        }

        public Task<PostingResult> PostVoucherAsync(JournalVoucherEntity voucher, CancellationToken ct = default)
        {
            return Task.Run(() => PostVoucher(voucher), ct);
        }

        public Task<PostingResult> PostBatchAsync(IReadOnlyList<JournalVoucherEntity> vouchers, IReadOnlyList<JournalVoucherEntity>? deletedVouchers = null, CancellationToken ct = default)
        {
            return Task.Run(() => PostBatch(vouchers, deletedVouchers), ct);
        }

        /// <summary>
        /// Thuật toán xóa vi sai (Differential Deletion) chi tiết để không làm sót dòng mồ côi khi cập nhật chứng từ.
        /// </summary>
        private static void SaveSingleVoucherInternal(IDataAccessAdapter adapter, JournalVoucherEntity voucher)
        {
            if (!voucher.IsNew)
            {
                var dbVoucher = new JournalVoucherEntity(voucher.Id);
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.JournalVoucherEntity);
                var jeNode = prefetch.Add(JournalVoucherEntity.PrefetchPathJournalEntries);
                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathDebtDetails);
                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathCurrencyDetails);
                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathInvestmentDetails);
                jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathAssetDetails);
                var costNode = jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathCostDetails);
                costNode.SubPath.Add(CostDetailEntity.PrefetchPathExpenseTaxLine);
                var ivNode = jeNode.SubPath.Add(JournalEntryEntity.PrefetchPathInventoryVouchers);
                var ivlNode = ivNode.SubPath.Add(InventoryVoucherEntity.PrefetchPathInventoryVoucherLines);
                ivlNode.SubPath.Add(InventoryVoucherLineEntity.PrefetchPathMaterialTaxLine);

                if (adapter.FetchEntity(dbVoucher, prefetch))
                {
                    foreach (var dbJe in dbVoucher.JournalEntries)
                    {
                        var ramJe = voucher.JournalEntries.FirstOrDefault(x => x.Id == dbJe.Id && !x.IsNew);
                        if (ramJe == null)
                        {
                            foreach (var dbDebt in dbJe.DebtDetails) adapter.DeleteEntity(dbDebt);
                            foreach (var dbCurr in dbJe.CurrencyDetails) adapter.DeleteEntity(dbCurr);
                            foreach (var dbInv in dbJe.InvestmentDetails) adapter.DeleteEntity(dbInv);
                            foreach (var dbAsset in dbJe.AssetDetails) adapter.DeleteEntity(dbAsset);
                            foreach (var dbCost in dbJe.CostDetails)
                            {
                                if (dbCost.ExpenseTaxLine != null) adapter.DeleteEntity(dbCost.ExpenseTaxLine);
                                adapter.DeleteEntity(dbCost);
                            }
                            foreach (var dbIv in dbJe.InventoryVouchers)
                            {
                                foreach (var dbLine in dbIv.InventoryVoucherLines)
                                {
                                    if (dbLine.MaterialTaxLine != null) adapter.DeleteEntity(dbLine.MaterialTaxLine);
                                    adapter.DeleteEntity(dbLine);
                                }
                                adapter.DeleteEntity(dbIv);
                            }
                            adapter.DeleteEntity(dbJe);
                        }
                        else
                        {
                            foreach (var dbDebt in dbJe.DebtDetails)
                            {
                                if (!ramJe.DebtDetails.Any(x => x.Id == dbDebt.Id && !x.IsNew))
                                    adapter.DeleteEntity(dbDebt);
                            }
                            foreach (var dbCurr in dbJe.CurrencyDetails)
                            {
                                if (!ramJe.CurrencyDetails.Any(x => x.Id == dbCurr.Id && !x.IsNew))
                                    adapter.DeleteEntity(dbCurr);
                            }
                            foreach (var dbInv in dbJe.InvestmentDetails)
                            {
                                if (!ramJe.InvestmentDetails.Any(x => x.Lineid == dbInv.Lineid && !x.IsNew))
                                    adapter.DeleteEntity(dbInv);
                            }
                            foreach (var dbAsset in dbJe.AssetDetails)
                            {
                                if (!ramJe.AssetDetails.Any(x => x.Id == dbAsset.Id && !x.IsNew))
                                    adapter.DeleteEntity(dbAsset);
                            }
                            foreach (var dbCost in dbJe.CostDetails)
                            {
                                if (!ramJe.CostDetails.Any(x => x.Id == dbCost.Id && !x.IsNew))
                                {
                                    if (dbCost.ExpenseTaxLine != null) adapter.DeleteEntity(dbCost.ExpenseTaxLine);
                                    adapter.DeleteEntity(dbCost);
                                }
                            }
                            foreach (var dbIv in dbJe.InventoryVouchers)
                            {
                                var ramIv = ramJe.InventoryVouchers.FirstOrDefault(x => x.Id == dbIv.Id && !x.IsNew);
                                if (ramIv == null)
                                {
                                    foreach (var dbLine in dbIv.InventoryVoucherLines)
                                    {
                                        if (dbLine.MaterialTaxLine != null) adapter.DeleteEntity(dbLine.MaterialTaxLine);
                                        adapter.DeleteEntity(dbLine);
                                    }
                                    adapter.DeleteEntity(dbIv);
                                }
                                else
                                {
                                    foreach (var dbLine in dbIv.InventoryVoucherLines)
                                    {
                                        var ramLine = ramIv.InventoryVoucherLines.FirstOrDefault(x => x.Id == dbLine.Id && !x.IsNew);
                                        if (ramLine == null)
                                        {
                                            if (dbLine.MaterialTaxLine != null) adapter.DeleteEntity(dbLine.MaterialTaxLine);
                                            adapter.DeleteEntity(dbLine);
                                        }
                                        else if (dbLine.MaterialTaxLine != null && ramLine.MaterialTaxLine == null)
                                        {
                                            adapter.DeleteEntity(dbLine.MaterialTaxLine);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            adapter.SaveEntity(voucher, true, recurse: true);
        }
    }
}
