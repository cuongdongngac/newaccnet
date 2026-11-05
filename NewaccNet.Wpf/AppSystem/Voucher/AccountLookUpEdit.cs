using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Core.Data;
using NewaccNet.Core.UI;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    /// <summary>
    /// Khối Lego chuẩn hóa chọn tài khoản kế toán trong NewaccNet.Wpf:
    /// - Kế thừa trực tiếp từ AccountLookUpEditBase (Core.UI) để thống nhất 100% chuẩn kiến trúc hệ thống.
    /// - Quản lý mở form phụ chi tiết (A: Công nợ, B: Vật tư, C: Ngoại tệ, D: TSCĐ, E: Chi phí, S: Cổ phiếu).
    /// - Tự động đối ứng Nợ/Có (Dbcr) và ParentId khi đứng ở lưới đối ứng (gridRight) trên VoucherEntryWindow.
    /// - Bảo vệ dữ liệu kế toán: Cảnh báo xóa chi tiết con khi đổi tài khoản khác nhóm phân loại.
    /// </summary>
    public class AccountLookUpEdit : AccountLookUpEditBase
    {
        public AccountLookUpEdit() : base()
        {
        }

        protected override bool BeforeAccountSelectionChanged(string? oldAccountId, string? newAccountId)
        {
            bool isUserAction = this.IsKeyboardFocusWithin || this.IsPopupOpen || _wasPopupOpened;
            if (!isUserAction)
            {
                return true;
            }

            if (string.IsNullOrEmpty(oldAccountId) || oldAccountId == newAccountId)
            {
                return true;
            }

            var currentEntry = GetCurrentEntry();
            if (currentEntry != null && currentEntry.HasChildDetails())
            {
                string oldCategory = GetCategoryForAccount(oldAccountId);
                string newCategory = GetCategoryForAccount(newAccountId);

                if (oldCategory != newCategory)
                {
                    var confirmResult = MessageBox.Show(
                        $"Tài khoản hiện tại ({oldAccountId}) đang có dữ liệu chi tiết con.\n" +
                        $"Bạn vừa đổi sang tài khoản ({newAccountId}) thuộc nhóm khác.\n\n" +
                        $"Hệ thống sẽ XÓA toàn bộ chi tiết con cũ để chuyển sang nhóm mới.\n" +
                        $"Bạn có muốn tiếp tục không?",
                        "Xác nhận thay đổi tài khoản",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (confirmResult != MessageBoxResult.Yes)
                    {
                        return false;
                    }

                    // Người dùng ĐỒNG Ý: Xóa toàn bộ con cũ!
                    var ownerWindow = Window.GetWindow(this) as VoucherEntryWindow;
                    if (ownerWindow != null && ownerWindow.CurrentVoucher != null && currentEntry.SubEntries != null)
                    {
                        foreach (var sub in currentEntry.SubEntries.ToList())
                        {
                            ownerWindow.CurrentVoucher.JournalEntries.Remove(sub);
                        }
                    }

                    currentEntry.ClearChildDetails();

                    if (ownerWindow != null)
                    {
                        ownerWindow.RefreshVoucherUI();
                        ownerWindow.CalculateBalance();
                    }
                }
            }

            return true;
        }

        protected override void OnAccountSelectionChanged(string? newAccountId)
        {
            base.OnAccountSelectionChanged(newAccountId);

            bool isUserAction = this.IsKeyboardFocusWithin || this.IsPopupOpen || _wasPopupOpened;
            if (isUserAction && !string.IsNullOrEmpty(newAccountId))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    TriggerDetailForm();
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        public override void TriggerDetailForm()
        {
            if (!ShowDetailButton) return;
            if (this.EditValue == null) return;
            string accountId = this.EditValue.ToString() ?? "";

            var currentEntry = GetCurrentEntry();
            if (currentEntry == null)
            {
                currentEntry = new JournalEntryEntity();
            }

            currentEntry.AccountId = accountId;
            var owner = Window.GetWindow(this);
            OpenDetailWindow(currentEntry, owner, this.ExcludeMaterialAccounts, false);
        }

        #region Điều hướng Form chi tiết kế toán chuẩn (Accounting First)
        public static bool OpenDetailWindow(JournalEntryEntity currentEntry, Window owner, bool excludeMaterialAccounts = false, bool showNotSupportedMessage = false)
        {
            if (currentEntry == null || string.IsNullOrEmpty(currentEntry.AccountId))
            {
                if (showNotSupportedMessage)
                {
                    MessageBox.Show("Vui lòng chọn tài khoản trước khi nhập chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return false;
            }

            string accountId = currentEntry.AccountId;
            var voucherWindow = owner as VoucherEntryWindow;
            var master = (voucherWindow?.gridLeft?.SelectedItem ?? voucherWindow?.gridLeft?.CurrentItem ?? voucherWindow?.MasterEntries?.FirstOrDefault()) as JournalEntryEntity;

            // Nếu đang đứng ở lưới con (gridRight) trên VoucherEntryWindow và dòng này không phải dòng Master
            // thì tự động đảo chiều Dbcr theo dòng Master và liên kết Parent!
            if (voucherWindow != null && master != null && currentEntry != master)
            {
                if (master.Dbcr.HasValue && master.Dbcr.Value != 0)
                {
                    currentEntry.Dbcr = (short)-master.Dbcr.Value;
                }
                if (currentEntry.ParentEntry == null)
                {
                    currentEntry.ParentEntry = master;
                }
                if (master.Id != 0 && (!currentEntry.ParentId.HasValue || currentEntry.ParentId == 0))
                {
                    currentEntry.ParentId = master.Id;
                }
                if ((!currentEntry.Amount.HasValue || currentEntry.Amount.Value == 0) && master.Amount.HasValue && master.Amount.Value > 0)
                {
                    decimal alreadyAllocated = master.SubEntries?.Where(s => s != currentEntry).Sum(s => s.Amount ?? 0m) ?? 0m;
                    decimal defaultAmt = master.Amount.Value - alreadyAllocated;
                    if (defaultAmt > 0)
                    {
                        currentEntry.Amount = defaultAmt;
                    }
                }
            }

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var account = new ChartOfAccountEntity(accountId);
                if (!adapter.FetchEntity(account)) return false;

                account.CategoryId = ResolveCategory(accountId, account.CategoryId);
                string cat = account.CategoryId;

                bool isDetailCategory = cat == "A" || cat == "B" || cat == "C" || cat == "D" || cat == "E" || cat == "S";
                if (!isDetailCategory)
                {
                    if (showNotSupportedMessage)
                    {
                        MessageBox.Show($"Tài khoản {accountId} ({account.AccountName}) là tài khoản hạch toán thông thường, không có bảng chi tiết phụ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return false;
                }

                // Nếu là tài khoản Vật tư (B) nhưng đang ở form vật tư (ExcludeMaterialAccounts == true, UpdateFilter == true hoặc owner is MaterialDetailWindow)
                // thì tuyệt đối KHÔNG mở form lồng đệ quy!
                if (cat == "B" && (excludeMaterialAccounts || currentEntry.UpdateFilter || owner is Details.MaterialDetailWindow))
                {
                    return false;
                }

                // Chỉ khi tài khoản THỰC SỰ có Form phụ con thì mới yêu cầu Dbcr:
                if (!currentEntry.Dbcr.HasValue || currentEntry.Dbcr == 0)
                {
                    if (!currentEntry.UpdateFilter && !excludeMaterialAccounts)
                    {
                        MessageBox.Show("Vui lòng chọn Nợ hoặc Có trước khi mở chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }

                return RouteToDetailForm(account, currentEntry, owner, excludeMaterialAccounts);
            }
        }

        private static bool RouteToDetailForm(ChartOfAccountEntity account, JournalEntryEntity entry, Window owner, bool excludeMaterialAccounts)
        {
            var voucherWindow = owner as VoucherEntryWindow;
            string cat = account.CategoryId;
            bool? result = false;

            if (cat == "A")
            {
                var debtForm = new Details.DebtDetailWindow(entry);
                if (owner != null) debtForm.Owner = owner;
                result = debtForm.ShowDialog();
            }
            else if (cat == "B")
            {
                if (owner is Details.MaterialDetailWindow || excludeMaterialAccounts || entry.UpdateFilter)
                {
                    return false;
                }

                var voucher = voucherWindow?.CurrentVoucher;

                bool hasExistingDetails = (entry.InventoryVouchers != null && entry.InventoryVouchers.Count > 0)
                    || (entry.SubEntries != null && entry.SubEntries.Any(s => s.InventoryVouchers != null && s.InventoryVouchers.Count > 0));

                if (!hasExistingDetails && voucher != null)
                {
                    hasExistingDetails = voucher.JournalEntries.Any(j => j.InventoryVouchers != null && j.InventoryVouchers.Count > 0
                        && (j == entry || j.ParentEntry == entry || (j.ParentId.HasValue && entry.Id != 0 && j.ParentId == entry.Id) || entry.ParentEntry == j));
                }

                bool isNewMaterial = !hasExistingDetails;
                var context = isNewMaterial
                    ? Details.MaterialTransactionContext.CreateNew(entry, voucher)
                    : Details.MaterialTransactionContext.FromExisting(entry, voucher);

                var materialForm = new Details.MaterialDetailWindow(context);
                if (owner != null) materialForm.Owner = owner;
                result = materialForm.ShowDialog();

                if (result == true)
                {
                    context.Apply();
                    if (voucherWindow != null)
                    {
                        voucherWindow.RefreshVoucherUI();
                    }
                }
                else if (isNewMaterial)
                {
                    context.Discard();
                    if (voucherWindow != null)
                    {
                        voucherWindow.RefreshVoucherUI();
                    }
                }
            }
            else if (cat == "C")
            {
                var currencyForm = new Details.CurrencyDetailWindow(entry);
                if (owner != null) currencyForm.Owner = owner;
                result = currencyForm.ShowDialog();
            }
            else if (cat == "D")
            {
                var assetForm = new Details.AssetDetailWindow(entry);
                if (owner != null) assetForm.Owner = owner;
                result = assetForm.ShowDialog();
            }
            else if (cat == "E")
            {
                bool hasTax = account.Taxflag && voucherWindow != null;
                var costForm = new Details.CostDetailWindow(entry, hasTax);
                if (owner != null) costForm.Owner = owner;
                result = costForm.ShowDialog();
            }
            else if (cat == "S")
            {
                var stockForm = new Details.StockDetailWindow(entry);
                if (owner != null) stockForm.Owner = owner;
                result = stockForm.ShowDialog();
            }

            if (voucherWindow != null)
            {
                voucherWindow.CalculateBalance();
            }

            return result == true;
        }
        #endregion
    }
}
