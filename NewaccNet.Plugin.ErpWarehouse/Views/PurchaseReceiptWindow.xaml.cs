using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataAccess.EntityClasses;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using NewaccNet.Core.Data;
using NewaccNet.Plugin.ErpWarehouse.Models;
using NewaccNet.Plugin.ErpWarehouse.Services;
using NewaccNet.Plugin.ErpWarehouse.Views.Dialogs;

namespace NewaccNet.Plugin.ErpWarehouse.Views
{
    public partial class PurchaseReceiptWindow : ThemedWindow
    {
        public ObservableCollection<InventoryItemEntity> Materials { get; } = new();
        public ObservableCollection<WarehouseVoucherLineItem> Lines { get; } = new();

        private List<TaxRateOption> _taxRates = new();
        private List<PartnerEntity> _partners = new();
        private List<WarehouseEntity> _warehouses = new();

        private List<int> _voucherIds = new();
        private int _currentVoucherIndex = -1;
        private int _currentVoucherId = 0;
        private bool _isCurrentVoucherPosted = false;

        private readonly JournalEntryEntity _payMasterEntry = new() { Dbcr = -1, AccountId = "3311" };
        private readonly JournalEntryEntity _matEntry = new() { Dbcr = 1, AccountId = "1561" };
        private readonly JournalEntryEntity _taxEntry = new() { Dbcr = 1, AccountId = "13311" };

        public PurchaseReceiptWindow()
        {
            InitializeComponent();
            gridDetails.ItemsSource = Lines;
            InitData();
            RefreshVoucherList();
            RefreshNavigation();
        }

        private void RefreshVoucherList()
        {
            _voucherIds = WarehouseVoucherDataService.GetVoucherIds("PN");
        }

        private void RefreshNavigation()
        {
            bool hasList = _voucherIds.Count > 0;
            if (_currentVoucherIndex >= 0 && _currentVoucherIndex < _voucherIds.Count)
            {
                lblNavPosition.Content = $"Phiếu: {_currentVoucherIndex + 1} / {_voucherIds.Count}";
                btnFirst.IsEnabled = _currentVoucherIndex > 0;
                btnPrev.IsEnabled = _currentVoucherIndex > 0;
                btnNext.IsEnabled = _currentVoucherIndex < _voucherIds.Count - 1;
                btnLast.IsEnabled = _currentVoucherIndex < _voucherIds.Count - 1;
            }
            else
            {
                lblNavPosition.Content = hasList ? $"Phiếu mới (Tổng có: {_voucherIds.Count})" : "Phiếu mới (chưa lưu)";
                btnFirst.IsEnabled = hasList;
                btnPrev.IsEnabled = hasList;
                btnNext.IsEnabled = false;
                btnLast.IsEnabled = hasList;
            }
        }

        private void LoadVoucherAtIndex(int index)
        {
            if (index < 0 || index >= _voucherIds.Count) return;

            int vId = _voucherIds[index];
            var data = WarehouseVoucherDataService.LoadVoucherDetail(vId);
            if (data == null) return;

            _currentVoucherIndex = index;
            _currentVoucherId = data.VoucherId;

            txtVoucherNo.Text = data.VoucherNo;
            dpVoucherDate.DateTime = data.VoucherDate;
            lueWarehouse.EditValue = data.WarehouseId;
            txtCarrier.Text = data.Carrier;
            txtDescription.Text = data.Description;

            txtVatBillNo.Text = data.VatBillNo;
            txtSerialNo.Text = data.SerialNo;
            if (data.VatBillDate.HasValue) dpVatBillDate.DateTime = data.VatBillDate.Value;
            luePartner.EditValue = data.PartnerId;
            txtPartnerName.Text = data.PartnerName;
            txtAddress.Text = data.Address;
            txtTaxCode.Text = data.TaxCode;

            if (!string.IsNullOrEmpty(data.InventoryAccountId)) lueInventoryAccount.EditValue = data.InventoryAccountId;
            if (!string.IsNullOrEmpty(data.TaxAccountId)) lueTaxAccount.EditValue = data.TaxAccountId;
            if (!string.IsNullOrEmpty(data.TotalAccountId)) lueTotalAccount.EditValue = data.TotalAccountId;

            _payMasterEntry.DebtDetails.Clear();
            foreach (var dd in data.TotalDebtDetails) _payMasterEntry.DebtDetails.Add(dd);
            _payMasterEntry.CostDetails.Clear();
            foreach (var cd in data.TotalCostDetails) _payMasterEntry.CostDetails.Add(cd);

            _taxEntry.DebtDetails.Clear();
            foreach (var dd in data.TaxDebtDetails) _taxEntry.DebtDetails.Add(dd);
            _taxEntry.CostDetails.Clear();
            foreach (var cd in data.TaxCostDetails) _taxEntry.CostDetails.Add(cd);

            _matEntry.DebtDetails.Clear();
            foreach (var dd in data.InventoryDebtDetails) _matEntry.DebtDetails.Add(dd);
            _matEntry.CostDetails.Clear();
            foreach (var cd in data.InventoryCostDetails) _matEntry.CostDetails.Add(cd);

            // Khóa sửa nếu kế toán đã ghi sổ
            _isCurrentVoucherPosted = data.Bookflag;
            if (_isCurrentVoucherPosted)
            {
                lblNavPosition.Content = $"Phiếu: {index + 1} / {_voucherIds.Count} [ĐÃ GHI SỔ - KHÓA SỬA]";
                btnAccept.IsEnabled = false;
                btnSaveAndNew.IsEnabled = false;
                btnAddRow.IsEnabled = false;
                btnDeleteRow.IsEnabled = false;
                viewDetails.AllowEditing = false;
            }
            else
            {
                lblNavPosition.Content = $"Phiếu: {index + 1} / {_voucherIds.Count} (Chưa ghi sổ)";
                btnAccept.IsEnabled = true;
                btnSaveAndNew.IsEnabled = true;
                btnAddRow.IsEnabled = true;
                btnDeleteRow.IsEnabled = true;
                viewDetails.AllowEditing = true;
            }

            Lines.Clear();
            foreach (var l in data.Lines)
            {
                var mat = Materials.FirstOrDefault(m => m.MaterialId == l.MaterialId);
                if (mat != null) l.Unit = mat.Unit ?? "";
                Lines.Add(l);
            }
            UpdateTotals();
            lueInventoryAccount.UpdateDetailStatus();
            lueTaxAccount.UpdateDetailStatus();
            lueTotalAccount.UpdateDetailStatus();
            RefreshNavigation();
        }

        private void AddNew()
        {
            _currentVoucherId = 0;
            _currentVoucherIndex = -1;

            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("PN");
            dpVoucherDate.DateTime = DateTime.Today;
            dpVatBillDate.DateTime = DateTime.Today;
            txtCarrier.Text = "";
            txtDescription.Text = "";
            txtVatBillNo.Text = "";
            txtSerialNo.Text = "";
            luePartner.EditValue = null;
            txtPartnerName.Text = "";
            txtAddress.Text = "";
            txtTaxCode.Text = "";

            _payMasterEntry.DebtDetails.Clear();
            _payMasterEntry.CostDetails.Clear();
            _taxEntry.DebtDetails.Clear();
            _taxEntry.CostDetails.Clear();
            _matEntry.DebtDetails.Clear();
            _matEntry.CostDetails.Clear();

            Lines.Clear();
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0 });
            UpdateTotals();
            lueInventoryAccount.UpdateDetailStatus();
            lueTaxAccount.UpdateDetailStatus();
            lueTotalAccount.UpdateDetailStatus();

            _isCurrentVoucherPosted = false;
            btnAccept.IsEnabled = true;
            btnSaveAndNew.IsEnabled = true;
            btnAddRow.IsEnabled = true;
            btnDeleteRow.IsEnabled = true;
            viewDetails.AllowEditing = true;

            RefreshNavigation();
            txtVoucherNo.Focus();
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e)
        {
            if (_voucherIds.Count > 0) LoadVoucherAtIndex(0);
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVoucherIndex > 0)
                LoadVoucherAtIndex(_currentVoucherIndex - 1);
            else if (_currentVoucherIndex == -1 && _voucherIds.Count > 0)
                LoadVoucherAtIndex(_voucherIds.Count - 1);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVoucherIndex >= 0 && _currentVoucherIndex < _voucherIds.Count - 1)
                LoadVoucherAtIndex(_currentVoucherIndex + 1);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (_voucherIds.Count > 0) LoadVoucherAtIndex(_voucherIds.Count - 1);
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void InitData()
        {
            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("PN");
            dpVoucherDate.DateTime = DateTime.Today;
            dpVatBillDate.DateTime = DateTime.Today;

            // 1. Kho
            _warehouses = WarehouseVoucherDataService.LoadWarehouses();
            lueWarehouse.ItemsSource = _warehouses;
            if (_warehouses.Count > 0) lueWarehouse.EditValue = _warehouses[0].WarehouseId;

            // 2. Vật tư
            var mats = WarehouseVoucherDataService.LoadMaterials();
            Materials.Clear();
            foreach (var m in mats) Materials.Add(m);

            // 3. Đối tác
            _partners = WarehouseVoucherDataService.LoadPartners();
            luePartner.ItemsSource = _partners;

            // 4. Thuế suất
            _taxRates = WarehouseVoucherDataService.LoadTaxRates();
            cboTaxRates.ItemsSource = _taxRates;

            // 5. Kết nối JournalEntry chuẩn Accounting First và nạp tài khoản theo Slot
            _payMasterEntry.SubEntries.Clear();
            _payMasterEntry.SubEntries.Add(_matEntry);
            _payMasterEntry.SubEntries.Add(_taxEntry);

            lueInventoryAccount.JournalEntry = _matEntry;
            lueTaxAccount.JournalEntry = _taxEntry;
            lueTotalAccount.JournalEntry = _payMasterEntry;

            lueInventoryAccount.ReminderTextBlock = lblInventoryDetailReminder;
            lueTaxAccount.ReminderTextBlock = lblTaxDetailReminder;
            lueTotalAccount.ReminderTextBlock = lblTotalDetailReminder;

            lueTotalAccount.ExternalDetailChecker = () =>
            {
                string acc = lueTotalAccount.EditValue?.ToString() ?? "";
                if (acc.StartsWith("331") || acc.StartsWith("131"))
                {
                    return !string.IsNullOrEmpty(luePartner.EditValue as string);
                }
                return false;
            };

            luePartner.EditValueChanged += (s, e) => lueTotalAccount.UpdateDetailStatus();

            var rule = WarehouseVoucherDataService.GetPostingRule("NM");
            if (rule != null && !string.IsNullOrEmpty(rule.Name))
                Title = $"Phiếu nhập kho mua hàng - {rule.Name}";

            lueInventoryAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Inventory"), rule?.GetDefaultAccountIdForSlot("Inventory", "1561"));
            lueTaxAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Tax"), rule?.GetDefaultAccountIdForSlot("Tax", "13311"));
            lueTotalAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Total"), rule?.GetDefaultAccountIdForSlot("Total", "3311"));

            // Dòng mẫu ban đầu
            Lines.Clear();
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0 });
            UpdateTotals();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                e.Handled = true;
                BtnSaveAndNew_Click(sender, e);
            }
            else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                BtnAccept_Click(sender, e);
            }
            else if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                BtnNew_Click(sender, e);
            }
            else if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                e.Handled = true;
                BtnPrint_Click(sender, e);
            }
            else if (e.Key == Key.F3)
            {
                e.Handled = true;
                BtnSearch_Click(sender, e);
            }
            else if (e.Key == Key.Insert)
            {
                e.Handled = true;
                BtnAddRow_Click(sender, e);
            }
            else if (e.Key == Key.Delete && !viewDetails.IsEditing)
            {
                e.Handled = true;
                BtnDeleteRow_Click(sender, e);
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                BtnCancel_Click(sender, e);
            }
        }

        private void ViewDetails_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            var row = e.Row as WarehouseVoucherLineItem;
            if (row == null) return;

            if (e.Column.FieldName == "MaterialId")
            {
                var mat = Materials.FirstOrDefault(m => string.Equals(m.MaterialId?.Trim(), row.MaterialId?.Trim(), StringComparison.OrdinalIgnoreCase));
                if (mat != null)
                {
                    row.MaterialId = mat.MaterialId;
                    row.MaterialName = mat.MaterialName ?? "";
                    row.Unit = mat.Unit ?? "";
                }
            }
            else if (e.Column.FieldName == "Taxrateid")
            {
                if (row.Taxrateid.HasValue)
                {
                    var tr = _taxRates.FirstOrDefault(t => t.Taxrateid == row.Taxrateid.Value);
                    row.TaxRate = tr != null ? tr.Rate : 0;
                }
                else
                {
                    row.TaxRate = 0;
                }
            }

            row.Recalculate(isPurchase: true);
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal cost = Lines.Sum(l => l.Amount);
            decimal tax = Lines.Sum(l => l.TaxAmount);
            decimal total = cost + tax;

            txtTotalCost.EditValue = cost;
            txtTotalTax.EditValue = tax;
            txtTotalAll.EditValue = total;

            _matEntry.Amount = cost;
            _taxEntry.Amount = tax;
            _payMasterEntry.Amount = total;
            _matEntry.ParentEntry = _payMasterEntry;
            _taxEntry.ParentEntry = _payMasterEntry;
        }

        private void LuePartner_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (e.NewValue is string pId && !string.IsNullOrEmpty(pId))
            {
                var p = _partners.FirstOrDefault(x => x.Id == pId);
                if (p != null)
                {
                    txtPartnerName.Text = p.CustomerName ?? "";
                    txtAddress.Text = p.Address ?? "";
                    txtTaxCode.Text = p.Taxno ?? "";
                }
            }
        }

        private void BtnAddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new QuickAddWarehouseDialog { Owner = this };
            if (dlg.ShowDialog() == true && dlg.CreatedWarehouse != null)
            {
                _warehouses = WarehouseVoucherDataService.LoadWarehouses();
                lueWarehouse.ItemsSource = null;
                lueWarehouse.ItemsSource = _warehouses;
                lueWarehouse.EditValue = dlg.CreatedWarehouse.WarehouseId;
            }
        }

        private void BtnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new QuickAddPartnerDialog { Owner = this };
            if (dlg.ShowDialog() == true && dlg.CreatedPartner != null)
            {
                _partners = WarehouseVoucherDataService.LoadPartners();
                luePartner.ItemsSource = null;
                luePartner.ItemsSource = _partners;
                luePartner.EditValue = dlg.CreatedPartner.Id;
            }
        }

        private void BtnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new QuickAddMaterialDialog { Owner = this };
            if (dlg.ShowDialog() == true && dlg.CreatedMaterial != null)
            {
                Materials.Add(dlg.CreatedMaterial);
                if (gridDetails.SelectedItem is WarehouseVoucherLineItem line)
                {
                    line.MaterialId = dlg.CreatedMaterial.MaterialId;
                    line.MaterialName = dlg.CreatedMaterial.MaterialName ?? "";
                    line.Unit = dlg.CreatedMaterial.Unit ?? "";
                    gridDetails.RefreshData();
                }
            }
        }

        private void BtnAddRow_Click(object sender, RoutedEventArgs e)
        {
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0 });
            viewDetails.FocusedRowHandle = Lines.Count - 1;
            UpdateTotals();
        }

        private void BtnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (gridDetails.SelectedItem is WarehouseVoucherLineItem line)
            {
                Lines.Remove(line);
                UpdateTotals();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new WarehouseVoucherLookupDialog("NM") { Owner = this };
            if (dlg.ShowDialog() == true && dlg.SelectedVoucherId.HasValue)
            {
                int targetId = dlg.SelectedVoucherId.Value;
                RefreshVoucherList();
                int idx = _voucherIds.IndexOf(targetId);
                if (idx >= 0)
                {
                    LoadVoucherAtIndex(idx);
                }
                else
                {
                    _voucherIds.Add(targetId);
                    LoadVoucherAtIndex(_voucherIds.Count - 1);
                }
            }
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            PerformSave(isSaveAndNew: false);
        }

        private void BtnSaveAndNew_Click(object sender, RoutedEventArgs e)
        {
            PerformSave(isSaveAndNew: true);
        }

        private bool PerformSave(bool isSaveAndNew)
        {
            if (_isCurrentVoucherPosted)
            {
                MessageBox.Show("Chứng từ này đã được Kế toán ghi sổ. Bạn không có quyền chỉnh sửa.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            viewDetails.CommitEditing();

            string voucherNo = txtVoucherNo.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(voucherNo))
            {
                MessageBox.Show("Vui lòng nhập Số chứng từ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtVoucherNo.Focus();
                return false;
            }

            string whId = lueWarehouse.EditValue as string ?? "";
            if (string.IsNullOrEmpty(whId))
            {
                MessageBox.Show("Vui lòng chọn Kho nhập hàng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueWarehouse.Focus();
                return false;
            }

            var validLines = Lines.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một dòng vật tư có số lượng hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Kiểm soát chi tiết tài khoản theo kế toán: Cảnh báo và chặn không cho lưu nếu thiếu chi tiết bắt buộc
            if (!lueInventoryAccount.ValidateDetailRequirement(out string? errInv))
            {
                MessageBox.Show(errInv, "Cảnh báo thiếu chi tiết kế toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueInventoryAccount.Focus();
                return false;
            }
            if (!lueTaxAccount.ValidateDetailRequirement(out string? errTax))
            {
                MessageBox.Show(errTax, "Cảnh báo thiếu chi tiết kế toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueTaxAccount.Focus();
                return false;
            }
            if (!lueTotalAccount.ValidateDetailRequirement(out string? errTot))
            {
                MessageBox.Show(errTot, "Cảnh báo thiếu chi tiết kế toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueTotalAccount.Focus();
                return false;
            }

            string totalAcc = _payMasterEntry.AccountId ?? (lueTotalAccount.EditValue as string ?? "3311");
            string taxAcc = _taxEntry.AccountId ?? (lueTaxAccount.EditValue as string ?? "13311");
            string invAcc = _matEntry.AccountId ?? (lueInventoryAccount.EditValue as string ?? "1561");
            string partnerId = luePartner.EditValue as string ?? "";

            var totalDebt = _payMasterEntry.DebtDetails.ToList();
            var totalCost = _payMasterEntry.CostDetails.ToList();
            var taxDebt = _taxEntry.DebtDetails.ToList();
            var taxCost = _taxEntry.CostDetails.ToList();
            var invDebt = _matEntry.DebtDetails.ToList();
            var invCost = _matEntry.CostDetails.ToList();

            decimal totalAll = (_payMasterEntry.Amount ?? 0m) > 0 ? _payMasterEntry.Amount.GetValueOrDefault() : Lines.Sum(l => l.Amount + l.TaxAmount);
            if (totalDebt.Count == 0 && !string.IsNullOrEmpty(partnerId) && (totalAcc.StartsWith("331") || totalAcc.StartsWith("131")))
            {
                totalDebt.Add(new DebtDetailEntity
                {
                    PartnerId = partnerId,
                    Amount = totalAll,
                    Liabilitydate = dpVoucherDate.DateTime
                });
            }

            var p = new WarehouseVoucherSaveParams
            {
                VoucherId = _currentVoucherId,
                VoucherType = "NM",
                VoucherNo = voucherNo,
                VoucherDate = dpVoucherDate.DateTime,
                WarehouseId = whId,
                Carrier = txtCarrier.Text?.Trim() ?? "",
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? $"Nhập mua hàng theo HĐ {txtVatBillNo.Text}" : txtDescription.Text.Trim(),
                
                VatBillNo = txtVatBillNo.Text?.Trim() ?? "",
                SerialNo = txtSerialNo.Text?.Trim() ?? "",
                VatBillDate = dpVatBillDate.DateTime,
                PartnerId = partnerId,
                PartnerName = txtPartnerName.Text?.Trim() ?? "",
                Address = txtAddress.Text?.Trim() ?? "",
                TaxCode = txtTaxCode.Text?.Trim() ?? "",

                InventoryAccountId = invAcc,
                TaxAccountId = taxAcc,
                TotalAccountId = totalAcc,
                PaymentType = totalAcc.StartsWith("331") ? "Debt" : "Cash",

                TotalDebtDetails = totalDebt,
                TotalCostDetails = totalCost,
                OffsetDebtDetails = totalDebt,
                OffsetCostDetails = totalCost,

                TaxDebtDetails = taxDebt,
                TaxCostDetails = taxCost,

                InventoryDebtDetails = invDebt,
                InventoryCostDetails = invCost,

                Lines = validLines,
                Bookflag = false
            };

            if (WarehouseVoucherDataService.SaveVoucher(p, out string error, out int savedId))
            {
                _currentVoucherId = savedId;
                RefreshVoucherList();
                _currentVoucherIndex = _voucherIds.IndexOf(savedId);

                if (isSaveAndNew)
                {
                    AddNew();
                }
                else
                {
                    RefreshNavigation();
                    MessageBox.Show($"Đã lưu thành công Phiếu nhập mua '{voucherNo}'.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return true;
            }
            else
            {
                MessageBox.Show(error, "Lỗi lưu phiếu", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            ExecutePrint();
        }

        private void ExecutePrint()
        {
            string whId = lueWarehouse.EditValue as string ?? "";
            string whName = _warehouses.FirstOrDefault(w => w.WarehouseId == whId)?.WarehouseName ?? whId;

            string invAcc = _matEntry.AccountId ?? (lueInventoryAccount.EditValue as string ?? "1561");
            string taxAcc = _taxEntry.AccountId ?? (lueTaxAccount.EditValue as string ?? "13311");
            string totalAcc = _payMasterEntry.AccountId ?? (lueTotalAccount.EditValue as string ?? "3311");

            ErpInventoryPrintHelper.PrintPurchaseReceipt(
                this,
                txtVoucherNo.Text?.Trim() ?? "",
                dpVoucherDate.DateTime,
                txtCarrier.Text?.Trim() ?? "",
                txtVatBillNo.Text?.Trim() ?? "",
                txtSerialNo.Text?.Trim() ?? "",
                dpVatBillDate.DateTime,
                txtPartnerName.Text?.Trim() ?? "",
                txtAddress.Text?.Trim() ?? "",
                txtTaxCode.Text?.Trim() ?? "",
                whId,
                whName,
                invAcc,
                taxAcc,
                totalAcc,
                Lines);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
