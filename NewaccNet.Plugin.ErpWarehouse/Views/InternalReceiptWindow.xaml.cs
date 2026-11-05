using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataAccess.EntityClasses;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using NewaccNet.Plugin.ErpWarehouse.Models;
using NewaccNet.Plugin.ErpWarehouse.Services;
using NewaccNet.Plugin.ErpWarehouse.Views.Dialogs;

namespace NewaccNet.Plugin.ErpWarehouse.Views
{
    public partial class InternalReceiptWindow : ThemedWindow
    {
        public ObservableCollection<InventoryItemEntity> Materials { get; } = new();
        public ObservableCollection<WarehouseVoucherLineItem> Lines { get; } = new();

        private List<PartnerEntity> _partners = new();
        private List<WarehouseEntity> _warehouses = new();

        private List<int> _voucherIds = new();
        private int _currentVoucherIndex = -1;
        private int _currentVoucherId = 0;
        private bool _isCurrentVoucherPosted = false;

        private readonly JournalEntryEntity _offsetEntry = new() { Dbcr = -1, AccountId = "154" };
        private readonly JournalEntryEntity _invEntry = new() { Dbcr = 1, AccountId = "1561" };

        public InternalReceiptWindow()
        {
            InitializeComponent();
            gridDetails.ItemsSource = Lines;
            InitData();
            RefreshVoucherList();
            RefreshNavigation();
        }

        private void RefreshVoucherList()
        {
            _voucherIds = WarehouseVoucherDataService.GetVoucherIds("NN");
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

            if (data.Bookflag)
            {
                _isCurrentVoucherPosted = true;
                lblNavPosition.Content = $"Phiếu: {index + 1} / {_voucherIds.Count} (Đã ghi sổ)";
                btnAccept.IsEnabled = false;
                btnSaveAndNew.IsEnabled = false;
                btnAddRow.IsEnabled = false;
                btnDeleteRow.IsEnabled = false;
                viewDetails.AllowEditing = false;
            }
            else
            {
                _isCurrentVoucherPosted = false;
                lblNavPosition.Content = $"Phiếu: {index + 1} / {_voucherIds.Count} (Chưa ghi sổ)";
                btnAccept.IsEnabled = true;
                btnSaveAndNew.IsEnabled = true;
                btnAddRow.IsEnabled = true;
                btnDeleteRow.IsEnabled = true;
                viewDetails.AllowEditing = true;
            }

            txtVoucherNo.Text = data.VoucherNo;
            dpVoucherDate.DateTime = data.VoucherDate;
            lueWarehouse.EditValue = data.WarehouseId;
            txtCarrier.Text = data.Carrier;
            txtDescription.Text = data.Description;
            luePartner.EditValue = data.PartnerId;

            if (!string.IsNullOrEmpty(data.InventoryAccountId)) lueInventoryAccount.EditValue = data.InventoryAccountId;
            if (!string.IsNullOrEmpty(data.OffsetAccountId)) lueOffsetAccount.EditValue = data.OffsetAccountId;

            _offsetEntry.CostDetails.Clear();
            if (data.OffsetCostDetails != null)
            {
                foreach (var cd in data.OffsetCostDetails) _offsetEntry.CostDetails.Add(cd);
            }
            _offsetEntry.DebtDetails.Clear();
            if (data.OffsetDebtDetails != null)
            {
                foreach (var dd in data.OffsetDebtDetails) _offsetEntry.DebtDetails.Add(dd);
            }

            _invEntry.CostDetails.Clear();
            if (data.InventoryCostDetails != null)
            {
                foreach (var cd in data.InventoryCostDetails) _invEntry.CostDetails.Add(cd);
            }
            _invEntry.DebtDetails.Clear();
            if (data.InventoryDebtDetails != null)
            {
                foreach (var dd in data.InventoryDebtDetails) _invEntry.DebtDetails.Add(dd);
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
            lueOffsetAccount.UpdateDetailStatus();
            RefreshNavigation();
        }

        private void AddNew()
        {
            _currentVoucherId = 0;
            _currentVoucherIndex = -1;

            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("NN");
            dpVoucherDate.DateTime = DateTime.Today;
            txtCarrier.Text = "";
            txtDescription.Text = "";
            luePartner.EditValue = null;

            _offsetEntry.CostDetails.Clear();
            _offsetEntry.DebtDetails.Clear();
            _invEntry.CostDetails.Clear();
            _invEntry.DebtDetails.Clear();

            Lines.Clear();
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0 });
            UpdateTotals();
            lueInventoryAccount.UpdateDetailStatus();
            lueOffsetAccount.UpdateDetailStatus();

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
            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("NN");
            dpVoucherDate.DateTime = DateTime.Today;

            // 1. Kho
            _warehouses = WarehouseVoucherDataService.LoadWarehouses();
            lueWarehouse.ItemsSource = _warehouses;
            if (_warehouses.Count > 0) lueWarehouse.EditValue = _warehouses[0].WarehouseId;

            // 2. Vật tư
            var mats = WarehouseVoucherDataService.LoadMaterials();
            Materials.Clear();
            foreach (var m in mats) Materials.Add(m);

            // 3. Đối tượng / Phân xưởng
            _partners = WarehouseVoucherDataService.LoadPartners();
            luePartner.ItemsSource = _partners;

            // 4. Kết nối JournalEntry chuẩn Accounting First và nạp tài khoản theo Slot
            _offsetEntry.SubEntries.Clear();
            _offsetEntry.SubEntries.Add(_invEntry);

            lueInventoryAccount.JournalEntry = _invEntry;
            lueOffsetAccount.JournalEntry = _offsetEntry;

            lueInventoryAccount.ReminderTextBlock = lblInventoryDetailReminder;
            lueOffsetAccount.ReminderTextBlock = lblOffsetDetailReminder;

            var rule = WarehouseVoucherDataService.GetPostingRule("NNB");
            if (rule != null && !string.IsNullOrEmpty(rule.Name))
                Title = $"Phiếu nhập kho nội bộ - {rule.Name}";

            lueInventoryAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Inventory"), rule?.GetDefaultAccountIdForSlot("Inventory", "1561"));
            lueOffsetAccount.SetSlotOptions(rule?.AccountSlots.GetValueOrDefault("Offset"), rule?.GetDefaultAccountIdForSlot("Offset", "154"));

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

            row.Recalculate();
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal cost = Lines.Sum(l => l.Amount);
            txtTotalCost.EditValue = cost;
            _invEntry.Amount = cost;
            _offsetEntry.Amount = cost;
        }

        private void LuePartner_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            // Do not force overwrite text carrier
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
            if (!lueOffsetAccount.ValidateDetailRequirement(out string? errOffset))
            {
                MessageBox.Show(errOffset, "Cảnh báo thiếu chi tiết kế toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueOffsetAccount.Focus();
                return false;
            }

            var p = new WarehouseVoucherSaveParams
            {
                VoucherId = _currentVoucherId,
                VoucherType = "NNB",
                VoucherNo = voucherNo,
                VoucherDate = dpVoucherDate.DateTime,
                WarehouseId = whId,
                Carrier = txtCarrier.Text?.Trim() ?? "",
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? $"Nhập kho nội bộ/thành phẩm vào kho {whId}" : txtDescription.Text.Trim(),

                PartnerId = luePartner.EditValue as string ?? "",
                PartnerName = luePartner.Text ?? "",

                InventoryAccountId = _invEntry.AccountId ?? (lueInventoryAccount.EditValue as string ?? "1561"),
                OffsetAccountId = _offsetEntry.AccountId ?? (lueOffsetAccount.EditValue as string ?? "154"),

                OffsetCostDetails = _offsetEntry.CostDetails.ToList(),
                OffsetDebtDetails = _offsetEntry.DebtDetails.ToList(),
                InventoryCostDetails = _invEntry.CostDetails.ToList(),
                InventoryDebtDetails = _invEntry.DebtDetails.ToList(),

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
                    MessageBox.Show($"Đã lưu thành công Phiếu nhập nội bộ '{voucherNo}'.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string partnerId = luePartner.EditValue as string ?? "";
            string partnerName = _partners.FirstOrDefault(p => p.Id == partnerId)?.CustomerName ?? (luePartner.Text ?? partnerId);

            string invAcc = _invEntry.AccountId ?? (lueInventoryAccount.EditValue as string ?? "1561");
            string offsetAcc = _offsetEntry.AccountId ?? (lueOffsetAccount.EditValue as string ?? "154");

            ErpInventoryPrintHelper.PrintInternalReceipt(
                this,
                txtVoucherNo.Text?.Trim() ?? "",
                dpVoucherDate.DateTime,
                txtCarrier.Text?.Trim() ?? "",
                partnerName,
                whId,
                whName,
                invAcc,
                offsetAcc,
                Lines);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
