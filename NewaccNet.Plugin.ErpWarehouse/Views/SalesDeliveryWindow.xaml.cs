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
    public partial class SalesDeliveryWindow : ThemedWindow
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

        private readonly JournalEntryEntity _costMasterEntry = new() { Dbcr = 1, AccountId = "6321" };
        private readonly JournalEntryEntity _costSubEntry = new() { Dbcr = -1, AccountId = "1561" };
        private readonly JournalEntryEntity _revMasterEntry = new() { Dbcr = 1, AccountId = "131" };
        private readonly JournalEntryEntity _revEntry = new() { Dbcr = -1, AccountId = "5111" };
        private readonly JournalEntryEntity _taxEntry = new() { Dbcr = -1, AccountId = "33311" };

        public SalesDeliveryWindow()
        {
            InitializeComponent();
            gridDetails.ItemsSource = Lines;
            InitData();
            RefreshVoucherList();
            RefreshNavigation();
        }

        private void RefreshVoucherList()
        {
            _voucherIds = WarehouseVoucherDataService.GetVoucherIds("PX");
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

            if (!string.IsNullOrEmpty(data.CostAccountId)) lueCostAccount.EditValue = data.CostAccountId;
            if (!string.IsNullOrEmpty(data.RevenueAccountId)) lueRevenueAccount.EditValue = data.RevenueAccountId;
            if (!string.IsNullOrEmpty(data.TaxAccountId)) lueTaxAccount.EditValue = data.TaxAccountId;
            if (!string.IsNullOrEmpty(data.TotalAccountId)) lueTotalAccount.EditValue = data.TotalAccountId;

            _revMasterEntry.DebtDetails.Clear();
            foreach (var dd in data.TotalDebtDetails) _revMasterEntry.DebtDetails.Add(dd);
            _revMasterEntry.CostDetails.Clear();
            foreach (var cd in data.TotalCostDetails) _revMasterEntry.CostDetails.Add(cd);

            _revEntry.DebtDetails.Clear();
            foreach (var dd in data.RevenueDebtDetails) _revEntry.DebtDetails.Add(dd);
            _revEntry.CostDetails.Clear();
            foreach (var cd in data.RevenueCostDetails) _revEntry.CostDetails.Add(cd);

            _taxEntry.DebtDetails.Clear();
            foreach (var dd in data.TaxDebtDetails) _taxEntry.DebtDetails.Add(dd);
            _taxEntry.CostDetails.Clear();
            foreach (var cd in data.TaxCostDetails) _taxEntry.CostDetails.Add(cd);

            _costMasterEntry.DebtDetails.Clear();
            foreach (var dd in data.CostDebtDetails) _costMasterEntry.DebtDetails.Add(dd);
            _costMasterEntry.CostDetails.Clear();
            foreach (var cd in data.CostCostDetails) _costMasterEntry.CostDetails.Add(cd);

            _costSubEntry.DebtDetails.Clear();
            foreach (var dd in data.InventoryDebtDetails) _costSubEntry.DebtDetails.Add(dd);
            _costSubEntry.CostDetails.Clear();
            foreach (var cd in data.InventoryCostDetails) _costSubEntry.CostDetails.Add(cd);

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
            lueCostAccount.UpdateDetailStatus();
            lueRevenueAccount.UpdateDetailStatus();
            lueTaxAccount.UpdateDetailStatus();
            lueTotalAccount.UpdateDetailStatus();
            RefreshNavigation();
        }

        private void AddNew()
        {
            _currentVoucherId = 0;
            _currentVoucherIndex = -1;

            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("PX");
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

            _revMasterEntry.DebtDetails.Clear();
            _revMasterEntry.CostDetails.Clear();
            _revEntry.DebtDetails.Clear();
            _revEntry.CostDetails.Clear();
            _taxEntry.DebtDetails.Clear();
            _taxEntry.CostDetails.Clear();
            _costMasterEntry.DebtDetails.Clear();
            _costMasterEntry.CostDetails.Clear();
            _costSubEntry.DebtDetails.Clear();
            _costSubEntry.CostDetails.Clear();

            Lines.Clear();
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0, SellPrice = 0 });
            UpdateTotals();
            lueCostAccount.UpdateDetailStatus();
            lueRevenueAccount.UpdateDetailStatus();
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
            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("PX");
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

            // 3. Khách hàng
            _partners = WarehouseVoucherDataService.LoadPartners();
            luePartner.ItemsSource = _partners;

            // 4. Thuế suất
            _taxRates = WarehouseVoucherDataService.LoadTaxRates();
            cboTaxRates.ItemsSource = _taxRates;

            // 5. Kết nối JournalEntry chuẩn Accounting First và nạp tài khoản theo Slot
            _costMasterEntry.SubEntries.Clear();
            _costMasterEntry.SubEntries.Add(_costSubEntry);

            _revMasterEntry.SubEntries.Clear();
            _revMasterEntry.SubEntries.Add(_revEntry);
            _revMasterEntry.SubEntries.Add(_taxEntry);

            lueCostAccount.JournalEntry = _costMasterEntry;
            lueRevenueAccount.JournalEntry = _revEntry;
            lueTaxAccount.JournalEntry = _taxEntry;
            lueTotalAccount.JournalEntry = _revMasterEntry;

            lueCostAccount.ReminderTextBlock = lblCostDetailReminder;
            lueRevenueAccount.ReminderTextBlock = lblRevenueDetailReminder;
            lueTaxAccount.ReminderTextBlock = lblTaxDetailReminder;
            lueTotalAccount.ReminderTextBlock = lblTotalDetailReminder;

            lueTotalAccount.ExternalDetailChecker = () =>
            {
                string acc = lueTotalAccount.EditValue?.ToString() ?? "";
                if (acc.StartsWith("131") || acc.StartsWith("331"))
                {
                    return !string.IsNullOrEmpty(luePartner.EditValue as string);
                }
                return false;
            };

            luePartner.EditValueChanged += (s, e) => lueTotalAccount.UpdateDetailStatus();

            var rule = WarehouseVoucherDataService.GetPostingRule("XB");
            if (rule != null && !string.IsNullOrEmpty(rule.Name))
                Title = $"Phiếu xuất kho bán hàng - {rule.Name}";

            lueCostAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Cost"), rule?.GetDefaultAccountIdForSlot("Cost", "6321"));
            lueRevenueAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Revenue"), rule?.GetDefaultAccountIdForSlot("Revenue", "5111"));
            lueTaxAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Tax"), rule?.GetDefaultAccountIdForSlot("Tax", "33311"));
            lueTotalAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Total"), rule?.GetDefaultAccountIdForSlot("Total", "131"));

            // Dòng mẫu ban đầu
            Lines.Clear();
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0, SellPrice = 0 });
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

            row.Recalculate(isPurchase: false);
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal cost = Lines.Sum(l => l.Amount);
            decimal sell = Lines.Sum(l => l.SellAmount);
            decimal tax = Lines.Sum(l => l.TaxAmount);
            decimal total = sell + tax;

            txtTotalCost.EditValue = cost;
            txtTotalSell.EditValue = sell;
            txtTotalTax.EditValue = tax;
            txtTotalAll.EditValue = total;

            _costMasterEntry.Amount = cost;
            _costSubEntry.Amount = cost;
            _revMasterEntry.Amount = total;
            _revEntry.Amount = sell;
            _taxEntry.Amount = tax;
            _costSubEntry.ParentEntry = _costMasterEntry;
            _revEntry.ParentEntry = _revMasterEntry;
            _taxEntry.ParentEntry = _revMasterEntry;
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
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0, SellPrice = 0 });
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
            var dlg = new WarehouseVoucherLookupDialog("XB") { Owner = this };
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
                MessageBox.Show("Vui lòng chọn Kho xuất hàng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (!lueCostAccount.ValidateDetailRequirement(out string? errCost))
            {
                MessageBox.Show(errCost, "Cảnh báo thiếu chi tiết kế toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueCostAccount.Focus();
                return false;
            }
            if (!lueRevenueAccount.ValidateDetailRequirement(out string? errRev))
            {
                MessageBox.Show(errRev, "Cảnh báo thiếu chi tiết kế toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueRevenueAccount.Focus();
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

            string totalAcc = _revMasterEntry.AccountId ?? (lueTotalAccount.EditValue as string ?? "131");
            string partnerId = luePartner.EditValue as string ?? "";

            var totalDebt = _revMasterEntry.DebtDetails.ToList();
            var totalCost = _revMasterEntry.CostDetails.ToList();
            var revDebt = _revEntry.DebtDetails.ToList();
            var revCost = _revEntry.CostDetails.ToList();
            var taxDebt = _taxEntry.DebtDetails.ToList();
            var taxCost = _taxEntry.CostDetails.ToList();
            var costDebt = _costMasterEntry.DebtDetails.ToList();
            var costCost = _costMasterEntry.CostDetails.ToList();
            var invDebt = _costSubEntry.DebtDetails.ToList();
            var invCost = _costSubEntry.CostDetails.ToList();

            decimal totalAll = (_revMasterEntry.Amount ?? 0m) > 0 ? _revMasterEntry.Amount.GetValueOrDefault() : Lines.Sum(l => l.SellAmount + l.TaxAmount);
            if (totalDebt.Count == 0 && !string.IsNullOrEmpty(partnerId) && (totalAcc.StartsWith("131") || totalAcc.StartsWith("331")))
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
                VoucherType = "XB",
                VoucherNo = voucherNo,
                VoucherDate = dpVoucherDate.DateTime,
                WarehouseId = whId,
                Carrier = txtCarrier.Text?.Trim() ?? "",
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? $"Xuất bán hàng theo HĐ {txtVatBillNo.Text}" : txtDescription.Text.Trim(),
                
                VatBillNo = txtVatBillNo.Text?.Trim() ?? "",
                SerialNo = txtSerialNo.Text?.Trim() ?? "",
                VatBillDate = dpVatBillDate.DateTime,
                PartnerId = partnerId,
                PartnerName = txtPartnerName.Text?.Trim() ?? "",
                Address = txtAddress.Text?.Trim() ?? "",
                TaxCode = txtTaxCode.Text?.Trim() ?? "",

                CostAccountId = _costMasterEntry.AccountId ?? (lueCostAccount.EditValue as string ?? "6321"),
                InventoryAccountId = _costSubEntry.AccountId ?? "1561",
                RevenueAccountId = _revEntry.AccountId ?? (lueRevenueAccount.EditValue as string ?? "5111"),
                TaxAccountId = _taxEntry.AccountId ?? (lueTaxAccount.EditValue as string ?? "33311"),
                TotalAccountId = totalAcc,
                PaymentType = totalAcc.StartsWith("131") ? "Debt" : "Cash",

                TotalDebtDetails = totalDebt,
                TotalCostDetails = totalCost,
                OffsetDebtDetails = totalDebt,
                OffsetCostDetails = costCost,

                RevenueDebtDetails = revDebt,
                RevenueCostDetails = revCost,

                TaxDebtDetails = taxDebt,
                TaxCostDetails = taxCost,

                CostDebtDetails = costDebt,
                CostCostDetails = costCost,

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
                    MessageBox.Show($"Đã lưu thành công Phiếu xuất bán '{voucherNo}'.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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

            string revAcc = _revEntry.AccountId ?? (lueRevenueAccount.EditValue as string ?? "5111");
            string taxAcc = _taxEntry.AccountId ?? (lueTaxAccount.EditValue as string ?? "33311");
            string totalAcc = _revMasterEntry.AccountId ?? (lueTotalAccount.EditValue as string ?? "131");

            ErpInventoryPrintHelper.PrintSalesDelivery(
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
                revAcc,
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
