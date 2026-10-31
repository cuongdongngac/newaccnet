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

            // 5. Tài khoản & Nạp cấu hình Rule định khoản ngầm định (DbConfig)
            var accounts = WarehouseVoucherDataService.LoadAccounts();
            lueInventoryAccount.ItemsSource = accounts.Where(a => a.AccountId.StartsWith("15")).ToList();
            lueInventoryAccount.EditValue = accounts.FirstOrDefault(a => a.AccountId == "1561" || a.AccountId == "152")?.AccountId ?? "1561";

            lueTaxAccount.ItemsSource = accounts.Where(a => a.AccountId.StartsWith("133")).ToList();
            lueTaxAccount.EditValue = accounts.FirstOrDefault(a => a.AccountId == "13311" || a.AccountId == "1331")?.AccountId ?? "13311";

            lueTotalAccount.ItemsSource = accounts.Where(a => a.AccountId.StartsWith("331") || a.AccountId.StartsWith("111") || a.AccountId.StartsWith("112")).ToList();
            lueTotalAccount.EditValue = accounts.FirstOrDefault(a => a.AccountId == "3311" || a.AccountId == "331")?.AccountId ?? "3311";

            var rule = WarehouseVoucherDataService.GetPostingRule("NM");
            if (rule != null)
            {
                if (!string.IsNullOrEmpty(rule.Name)) Title = $"Phiếu nhập kho mua hàng - {rule.Name}";
                if (!string.IsNullOrEmpty(rule.TaxAccountId)) lueTaxAccount.EditValue = rule.TaxAccountId;
                if (!string.IsNullOrEmpty(rule.TotalAccountId)) lueTotalAccount.EditValue = rule.TotalAccountId;
            }

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
            double cost = Lines.Sum(l => l.Amount);
            double tax = Lines.Sum(l => l.TaxAmount);
            double total = cost + tax;

            txtTotalCost.EditValue = cost;
            txtTotalTax.EditValue = tax;
            txtTotalAll.EditValue = total;
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
                PartnerId = luePartner.EditValue as string ?? "",
                PartnerName = txtPartnerName.Text?.Trim() ?? "",
                Address = txtAddress.Text?.Trim() ?? "",
                TaxCode = txtTaxCode.Text?.Trim() ?? "",

                InventoryAccountId = lueInventoryAccount.EditValue as string ?? "1561",
                TaxAccountId = lueTaxAccount.EditValue as string ?? "13311",
                TotalAccountId = lueTotalAccount.EditValue as string ?? "3311",

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
            MessageBox.Show($"In phiếu nhập kho: {txtVoucherNo.Text}\n(Chức năng in phiếu sẵn sàng)", "In phiếu", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
