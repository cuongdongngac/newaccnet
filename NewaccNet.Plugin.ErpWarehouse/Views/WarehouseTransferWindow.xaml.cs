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
    public partial class WarehouseTransferWindow : ThemedWindow
    {
        public ObservableCollection<InventoryItemEntity> Materials { get; } = new();
        public ObservableCollection<WarehouseVoucherLineItem> Lines { get; } = new();

        private List<WarehouseEntity> _warehouses = new();

        private List<int> _voucherIds = new();
        private int _currentVoucherIndex = -1;
        private int _currentVoucherId = 0;

        public WarehouseTransferWindow()
        {
            InitializeComponent();
            gridDetails.ItemsSource = Lines;
            InitData();
            RefreshVoucherList();
            RefreshNavigation();
        }

        private void RefreshVoucherList()
        {
            _voucherIds = WarehouseVoucherDataService.GetVoucherIds("CK");
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
            if (!string.IsNullOrEmpty(data.FromWarehouseId)) lueFromWarehouse.EditValue = data.FromWarehouseId;
            if (!string.IsNullOrEmpty(data.ToWarehouseId)) lueToWarehouse.EditValue = data.ToWarehouseId;
            txtCarrier.Text = data.Carrier;
            txtDescription.Text = data.Description;

            if (!string.IsNullOrEmpty(data.InventoryAccountId))
            {
                lueDebitAccount.EditValue = data.InventoryAccountId;
                lueCreditAccount.EditValue = data.InventoryAccountId;
            }

            Lines.Clear();
            foreach (var l in data.Lines)
            {
                var mat = Materials.FirstOrDefault(m => m.MaterialId == l.MaterialId);
                if (mat != null) l.Unit = mat.Unit ?? "";
                Lines.Add(l);
            }
            UpdateTotals();
            RefreshNavigation();
        }

        private void AddNew()
        {
            _currentVoucherId = 0;
            _currentVoucherIndex = -1;

            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("CK");
            dpVoucherDate.DateTime = DateTime.Today;
            txtCarrier.Text = "";
            txtDescription.Text = "";

            Lines.Clear();
            Lines.Add(new WarehouseVoucherLineItem { Quantity = 1, Price = 0 });
            UpdateTotals();

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
            txtVoucherNo.Text = WarehouseVoucherDataService.GenerateNextVoucherNo("CK");
            dpVoucherDate.DateTime = DateTime.Today;

            // 1. Danh sách kho
            _warehouses = WarehouseVoucherDataService.LoadWarehouses();
            lueFromWarehouse.ItemsSource = _warehouses;
            lueToWarehouse.ItemsSource = _warehouses;

            if (_warehouses.Count > 0)
            {
                lueFromWarehouse.EditValue = _warehouses[0].WarehouseId;
                if (_warehouses.Count > 1)
                {
                    lueToWarehouse.EditValue = _warehouses[1].WarehouseId;
                }
            }

            // 2. Vật tư
            var mats = WarehouseVoucherDataService.LoadMaterials();
            Materials.Clear();
            foreach (var m in mats) Materials.Add(m);

            // 3. Tài khoản
            var accounts = WarehouseVoucherDataService.LoadAccounts();
            var invAccs = accounts.Where(a => a.AccountId.StartsWith("15")).ToList();

            lueDebitAccount.ItemsSource = invAccs;
            lueDebitAccount.EditValue = invAccs.FirstOrDefault(a => a.AccountId == "1561" || a.AccountId == "152")?.AccountId ?? "1561";

            lueCreditAccount.ItemsSource = invAccs;
            lueCreditAccount.EditValue = invAccs.FirstOrDefault(a => a.AccountId == "1561" || a.AccountId == "152")?.AccountId ?? "1561";

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
                    row.MaterialName = mat.MaterialName ?? "";
                    row.Unit = mat.Unit ?? "";
                }
            }

            row.Recalculate();
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            double cost = Lines.Sum(l => l.Amount);
            txtTotalCost.EditValue = cost;
        }

        private void BtnAddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new QuickAddWarehouseDialog { Owner = this };
            if (dlg.ShowDialog() == true && dlg.CreatedWarehouse != null)
            {
                _warehouses = WarehouseVoucherDataService.LoadWarehouses();
                lueFromWarehouse.ItemsSource = null;
                lueFromWarehouse.ItemsSource = _warehouses;
                lueToWarehouse.ItemsSource = null;
                lueToWarehouse.ItemsSource = _warehouses;
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
            viewDetails.CommitEditing();

            string voucherNo = txtVoucherNo.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(voucherNo))
            {
                MessageBox.Show("Vui lòng nhập Số chứng từ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtVoucherNo.Focus();
                return false;
            }

            string fromWh = lueFromWarehouse.EditValue as string ?? "";
            string toWh = lueToWarehouse.EditValue as string ?? "";

            if (string.IsNullOrEmpty(fromWh))
            {
                MessageBox.Show("Vui lòng chọn Kho xuất hàng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueFromWarehouse.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(toWh))
            {
                MessageBox.Show("Vui lòng chọn Kho nhập hàng.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueToWarehouse.Focus();
                return false;
            }

            if (string.Equals(fromWh.Trim(), toWh.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Kho xuất hàng và Kho nhập hàng phải khác nhau.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                lueToWarehouse.Focus();
                return false;
            }

            var validLines = Lines.Where(l => !string.IsNullOrWhiteSpace(l.MaterialId) && l.Quantity > 0).ToList();
            if (validLines.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một dòng vật tư có số lượng hợp lệ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var p = new WarehouseVoucherSaveParams
            {
                VoucherId = _currentVoucherId,
                VoucherType = "CK",
                VoucherNo = voucherNo,
                VoucherDate = dpVoucherDate.DateTime,
                FromWarehouseId = fromWh,
                ToWarehouseId = toWh,
                Carrier = txtCarrier.Text?.Trim() ?? "",
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? $"Điều chuyển hàng hóa từ kho {fromWh} sang kho {toWh}" : txtDescription.Text.Trim(),

                InventoryAccountId = lueDebitAccount.EditValue as string ?? "1561",

                Lines = validLines
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
                    MessageBox.Show($"Đã lưu thành công Phiếu điều chuyển '{voucherNo}'.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
            MessageBox.Show($"In phiếu điều chuyển kho: {txtVoucherNo.Text}\n(Chức năng in phiếu sẵn sàng)", "In phiếu", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
