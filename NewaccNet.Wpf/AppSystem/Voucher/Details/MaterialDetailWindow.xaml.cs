using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class MaterialDetailWindow : ThemedWindow
    {
        private MaterialTransactionContext _context;
        private InventoryVoucherEntity _currentEntity;
        private bool _isNew;

        public System.Collections.ObjectModel.ObservableCollection<InventoryItemEntity> Materials { get; } = new System.Collections.ObjectModel.ObservableCollection<InventoryItemEntity>();
        private List<InventoryItemEntity> _materials = new List<InventoryItemEntity>();
        private List<TaxRateEntity> _taxRates = new List<TaxRateEntity>();
        private List<PartnerEntity> _partners = new List<PartnerEntity>();

        public MaterialTransactionContext Context => _context;

        public string SelectedCostAccount => _context?.CostAccountId ?? "";
        public string SelectedRevenueAccount => _context?.RevenueAccountId ?? "";
        public string SelectedTaxAccount => _context?.TaxAccountId ?? "";
        public string SelectedTotalAccount => _context?.TotalAccountId ?? "";
        
        public string VoucherType => _context?.VoucherType ?? "";
        public double TotalCostAmount => _context?.TotalCostAmount ?? 0;
        public double TotalSellAmount => _context?.TotalSellAmount ?? 0;
        public double TotalTaxAmount => _context?.TotalTaxAmount ?? 0;

        public class MaterialBillTypeItem
        {
            public bool Taxflag { get; set; }
            public string Description { get; set; } = "";
        }

        public class TaxRateItem
        {
            public int Taxrateid { get; set; }
            public double Rate { get; set; }
            public string Display => (Rate > 1) ? $"{Rate:G29}%" : $"{Rate * 100:G29}%";
        }

        public MaterialDetailWindow(JournalEntryEntity parentEntry, bool isNew)
            : this(isNew ? MaterialTransactionContext.CreateNew(parentEntry) : MaterialTransactionContext.FromExisting(parentEntry))
        {
        }

        public MaterialDetailWindow(JournalEntryEntity parentEntry, string voucherType, bool isNew = false)
            : this(isNew ? MaterialTransactionContext.CreateNew(parentEntry, voucherType) : MaterialTransactionContext.FromExisting(parentEntry))
        {
        }

        public MaterialDetailWindow(JournalEntryEntity parentEntry, JournalVoucherEntity voucher, bool isNew = false)
            : this(isNew ? MaterialTransactionContext.CreateNew(parentEntry, voucher) : MaterialTransactionContext.FromExisting(parentEntry, voucher))
        {
        }

        private bool _isLoading = true;

        public MaterialDetailWindow(MaterialTransactionContext context)
        {
            _isLoading = true;
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentEntity = _context.InventoryVoucher;
            _isNew = _context.IsNew;

            LoadLookupData();
            InitData();
            _isLoading = false;
        }

        private void LoadLookupData()
        {
            var voucherTypes = new List<MaterialBillTypeItem>();
            int dbcr = _context?.MaterialEntry?.Dbcr ?? 1;

            if (dbcr == 1)
            {
                voucherTypes.Add(new MaterialBillTypeItem { Taxflag = false, Description = "Nhập nội bộ" });
                voucherTypes.Add(new MaterialBillTypeItem { Taxflag = true, Description = "Nhập mua" });
            }
            else
            {
                voucherTypes.Add(new MaterialBillTypeItem { Taxflag = false, Description = "Xuất nội bộ" });
                voucherTypes.Add(new MaterialBillTypeItem { Taxflag = true, Description = "Xuất bán" });
            }
            cboVoucherType.ItemsSource = voucherTypes;

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var warehouses = new EntityCollection<WarehouseEntity>();
                adapter.FetchEntityCollection(warehouses, null);
                lueWarehouse.ItemsSource = warehouses;

                var materials = new EntityCollection<InventoryItemEntity>();
                adapter.FetchEntityCollection(materials, null);
                _materials = materials.ToList();
                Materials.Clear();
                foreach (var m in materials) Materials.Add(m);

                var taxRates = new EntityCollection<TaxRateEntity>();
                adapter.FetchEntityCollection(taxRates, null);
                _taxRates = taxRates.ToList();
                lueTaxRate.ItemsSource = _taxRates.Select(t => new TaxRateItem 
                { 
                    Taxrateid = t.Taxrateid, 
                    Rate = t.Rate ?? 0 
                }).ToList();

                var partners = new EntityCollection<PartnerEntity>();
                adapter.FetchEntityCollection(partners, null);
                _partners = partners.ToList();
                lueVatObject.ItemsSource = _partners;

                var accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accounts, null);
                var parentIds = accounts.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
                var nonMaterialAccList = accounts
                    .Where(a => !parentIds.Contains(a.AccountId) && a.CategoryId != "B" && !a.AccountId.StartsWith("15"))
                    .OrderBy(a => a.AccountId)
                    .ToList();
                lueCostAccount.ItemsSource = nonMaterialAccList;
                lueRevenueAccount.ItemsSource = nonMaterialAccList;
                lueTaxAccount.ItemsSource = nonMaterialAccList;
                lueTotalAccount.ItemsSource = nonMaterialAccList;
            }
        }

        private void BtnAddWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var newWh = new WarehouseEntity();
            var editor = new Directory.WarehouseEditorWindow(newWh, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newWh, true);
                    }

                    if (lueWarehouse.ItemsSource is EntityCollection<WarehouseEntity> whCollection)
                    {
                        if (!whCollection.Any(x => x.WarehouseId == newWh.WarehouseId))
                        {
                            whCollection.Add(newWh);
                        }
                    }
                    lueWarehouse.EditValue = newWh.WarehouseId;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu kho hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddVatPartner_Click(object sender, RoutedEventArgs e)
        {
            var newPartner = new PartnerEntity();
            var editor = new Directory.PartnerEditorWindow(newPartner, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newPartner, true);
                    }

                    _partners.Add(newPartner);
                    lueVatObject.ItemsSource = null;
                    lueVatObject.ItemsSource = _partners;
                    lueVatObject.EditValue = newPartner.Id;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu đối tượng thuế: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            var newMat = new InventoryItemEntity();
            var editor = new Directory.InventoryItemEditorWindow(newMat, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newMat, true);
                    }

                    if (!Materials.Any(x => x.MaterialId == newMat.MaterialId))
                    {
                        Materials.Add(newMat);
                    }

                    var buttonInfo = sender as DevExpress.Xpf.Editors.ButtonInfo;
                    var cellData = buttonInfo?.DataContext as EditGridCellData;
                    var row = cellData?.RowData?.Row as InventoryVoucherLineEntity;
                    if (row == null)
                    {
                        row = GridMaterialDetails.SelectedItem as InventoryVoucherLineEntity;
                    }

                    if (row != null)
                    {
                        row.MaterialId = newMat.MaterialId;
                    }
                    GridMaterialDetails.SetCellValue(viewDetails.FocusedRowHandle, "MaterialId", newMat.MaterialId);
                    viewDetails.CloseEditor();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu vật tư hàng hóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void InitData()
        {
            this.DataContext = _context;

            if (_isNew)
            {
                if (string.IsNullOrEmpty(_context.WarehouseId))
                {
                    if (lueWarehouse.ItemsSource is EntityCollection<WarehouseEntity> wList && wList.Count > 0)
                    {
                        _context.WarehouseId = wList[0].WarehouseId;
                    }
                }
            }
            else if (_context?.MaterialEntry?.InventoryVouchers != null)
            {
                // Đồng bộ Rate nếu dòng chi tiết đã có Taxrateid trên tất cả các phiếu kho
                foreach (var iv in _context.MaterialEntry.InventoryVouchers)
                {
                    if (iv?.InventoryVoucherLines == null) continue;
                    foreach (var line in iv.InventoryVoucherLines)
                    {
                        if (line.Taxrateid.HasValue && (line.Rate == null || line.Rate == 0))
                        {
                            var tr = _taxRates.FirstOrDefault(t => t.Taxrateid == line.Taxrateid.Value);
                            if (tr != null)
                                line.Rate = tr.Rate;
                        }
                    }
                }
            }

            UpdateUIVisibility();
            _context.RecalculateTotals();
        }

        private void CommitCurrentEditing()
        {
            viewDetails.CommitEditing();
            GridMaterialDetails.View.CommitEditing();
        }

        private void BtnAddVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_context == null) return;
            CommitCurrentEditing();

            string defaultWh = "";
            if (lueWarehouse.ItemsSource is EntityCollection<WarehouseEntity> wList && wList.Count > 0)
            {
                // Ưu tiên gợi ý kho chưa có trong danh sách phiếu kho của tài khoản này
                var usedWh = _context.MaterialEntry.InventoryVouchers
                    .Where(x => !string.IsNullOrWhiteSpace(x.WarehouseId))
                    .Select(x => x.WarehouseId.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var unused = wList.FirstOrDefault(w => !string.IsNullOrWhiteSpace(w.WarehouseId) && !usedWh.Contains(w.WarehouseId.Trim()));
                defaultWh = unused?.WarehouseId ?? wList[0].WarehouseId;
            }

            _isLoading = true;
            _context.AddNewInventoryVoucher(defaultWh);
            _currentEntity = _context.InventoryVoucher;
            GridMaterialDetails.ItemsSource = null;
            GridMaterialDetails.ItemsSource = _context.CurrentLines;
            GridMaterialDetails.RefreshData();
            UpdateUIVisibility();
            _isLoading = false;
        }

        private void BtnDeleteVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (_context == null || _context.InventoryVoucher == null) return;
            CommitCurrentEditing();

            string whName = _context.WarehouseId ?? "";
            int idx = _context.CurrentVoucherIndex + 1;
            int total = _context.VoucherCount;

            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu kho thứ {idx}/{total} (Kho: {whName})?",
                                "Xác nhận xóa phiếu kho", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            string defaultWh = "";
            if (lueWarehouse.ItemsSource is EntityCollection<WarehouseEntity> wList && wList.Count > 0)
            {
                defaultWh = wList[0].WarehouseId;
            }

            _isLoading = true;
            _context.RemoveCurrentInventoryVoucher(defaultWh);
            _currentEntity = _context.InventoryVoucher;
            GridMaterialDetails.ItemsSource = null;
            GridMaterialDetails.ItemsSource = _context.CurrentLines;
            GridMaterialDetails.RefreshData();
            UpdateUIVisibility();
            _isLoading = false;
        }

        private void SwitchVoucherTo(int targetIndex)
        {
            if (_context == null) return;
            CommitCurrentEditing();
            _isLoading = true;
            _context.NavigateTo(targetIndex);
            _currentEntity = _context.InventoryVoucher;
            GridMaterialDetails.ItemsSource = null;
            GridMaterialDetails.ItemsSource = _context.CurrentLines;
            GridMaterialDetails.RefreshData();
            UpdateUIVisibility();
            _isLoading = false;
        }

        private void BtnFirstVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => SwitchVoucherTo(0);
        private void BtnPrevVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => SwitchVoucherTo(_context.CurrentVoucherIndex - 1);
        private void BtnNextVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => SwitchVoucherTo(_context.CurrentVoucherIndex + 1);
        private void BtnLastVoucher_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => SwitchVoucherTo(_context.VoucherCount - 1);

        private void cboVoucherType_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (_isLoading) return;

            if (e.NewValue is bool val && _context != null)
            {
                if (_context.Taxflag != val)
                {
                    _context.Taxflag = val;
                }
            }

            UpdateUIVisibility();
        }

        private void UpdateUIVisibility()
        {
            string type = _context?.VoucherType ?? "";

            if (type == "XB")
            {
                bandSalesAndTax.Header = "Giá bán & Thuế";
                bandSalesAndTax.Visible = true;
                GridMaterialDetails.Columns["SellPrice"].Visible = true;
                GridMaterialDetails.Columns["SellAmount"].Visible = true;
                GridMaterialDetails.Columns["Taxrateid"].Visible = true;
                GridMaterialDetails.Columns["TaxAmount"].Visible = true;
            }
            else if (type == "NM")
            {
                bandSalesAndTax.Header = "Thuế GTGT";
                bandSalesAndTax.Visible = true;
                GridMaterialDetails.Columns["SellPrice"].Visible = false;
                GridMaterialDetails.Columns["SellAmount"].Visible = false;
                GridMaterialDetails.Columns["Taxrateid"].Visible = true;
                GridMaterialDetails.Columns["TaxAmount"].Visible = true;
            }
            else
            {
                bandSalesAndTax.Visible = false;
            }
        }

        private void viewDetails_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var row = GridMaterialDetails.GetRowByListIndex(e.RowHandle) as InventoryVoucherLineEntity;
            if (row != null)
            {
                string type = _context?.VoucherType ?? "";
                if (type == "XB" || type == "NM")
                {
                    row.MaterialTaxLine = new MaterialTaxLineEntity();
                }
            }
        }

        private void viewDetails_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var row = e.NewRow as InventoryVoucherLineEntity;
            if (row != null)
            {
                string type = _context?.VoucherType ?? "";
                if ((type == "XB" || type == "NM") && row.MaterialTaxLine == null)
                {
                    row.MaterialTaxLine = new MaterialTaxLineEntity();
                }
            }
        }

        private void GridMaterialDetails_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            var row = GridMaterialDetails.GetRowByListIndex(e.ListSourceRowIndex) as InventoryVoucherLineEntity;
            if (row == null) return;

            if (e.Column.FieldName == "MaterialName" && e.IsGetData)
            {
                if (!string.IsNullOrEmpty(row.MaterialId) && _materials != null)
                {
                    var mat = _materials.FirstOrDefault(m => string.Equals(m.MaterialId?.Trim(), row.MaterialId?.Trim(), StringComparison.OrdinalIgnoreCase));
                    e.Value = mat?.MaterialName;
                }
            }
            else if (e.Column.FieldName == "Unit" && e.IsGetData)
            {
                if (!string.IsNullOrEmpty(row.MaterialId) && _materials != null)
                {
                    var mat = _materials.FirstOrDefault(m => string.Equals(m.MaterialId?.Trim(), row.MaterialId?.Trim(), StringComparison.OrdinalIgnoreCase));
                    e.Value = mat?.Unit;
                }
            }
            else if (e.Column.FieldName == "Amount" && e.IsGetData)
            {
                e.Value = row.Amount;
            }
            else if (e.Column.FieldName == "SellAmount" && e.IsGetData)
            {
                e.Value = row.SellAmount;
            }
            else if (e.Column.FieldName == "TaxAmount" && e.IsGetData)
            {
                e.Value = row.TaxAmount;
            }
        }

        private void viewDetails_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "MaterialId")
            {
                GridMaterialDetails.RefreshRow(e.RowHandle);
            }
            else if (e.Column.FieldName == "Quantity" || e.Column.FieldName == "Price" || 
                     e.Column.FieldName == "SellPrice" || e.Column.FieldName == "Taxrateid")
            {
                if (e.Column.FieldName == "Taxrateid")
                {
                    var row = e.Row as InventoryVoucherLineEntity;
                    if (row != null)
                    {
                        int taxId = 0;
                        if (e.Value != null && int.TryParse(e.Value.ToString(), out int parsedId))
                        {
                            taxId = parsedId;
                            row.Taxrateid = parsedId;
                        }
                        else if (row.Taxrateid.HasValue)
                        {
                            taxId = row.Taxrateid.Value;
                        }

                        if (taxId > 0 && _taxRates != null)
                        {
                            var rate = _taxRates.FirstOrDefault(t => t.Taxrateid == taxId);
                            row.Rate = rate?.Rate ?? 0;
                        }
                        else
                        {
                            row.Rate = 0;
                        }
                    }
                }
                GridMaterialDetails.RefreshRow(e.RowHandle);
            }
        }

        private void lueVatObject_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (lueVatObject.SelectedItem is PartnerEntity partner && _context?.InventoryVoucher != null)
            {
                _context.InventoryVoucher.Vatobjectname = partner.CustomerName;
                _context.InventoryVoucher.Vataddress = partner.Address;
                _context.InventoryVoucher.Taxno = partner.Taxno;
            }
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            CommitCurrentEditing();

            var vouchers = _context.MaterialEntry.InventoryVouchers;
            for (int i = 0; i < vouchers.Count; i++)
            {
                var iv = vouchers[i];
                var emptyLines = iv.InventoryVoucherLines.Where(l => string.IsNullOrEmpty(l.MaterialId)).ToList();
                foreach (var line in emptyLines)
                    iv.InventoryVoucherLines.Remove(line);

                foreach (var line in iv.InventoryVoucherLines)
                    line.CleanUpTaxLine();
            }

            // Nếu có nhiều hơn 1 phiếu kho mà có phiếu mới thêm bị bỏ trống (0 dòng), tự động dọn bỏ phiếu trống đó
            if (vouchers.Count > 1)
            {
                var emptyVouchers = vouchers.Where(v => v.InventoryVoucherLines.Count == 0).ToList();
                if (emptyVouchers.Count < vouchers.Count)
                {
                    foreach (var ev in emptyVouchers)
                        vouchers.Remove(ev);
                    _context.SetActiveInventoryVoucher(0);
                }
            }

            for (int i = 0; i < vouchers.Count; i++)
            {
                var iv = vouchers[i];
                if (string.IsNullOrWhiteSpace(iv.WarehouseId))
                {
                    SwitchVoucherTo(i);
                    MessageBox.Show($"Vui lòng chọn Kho cho phiếu kho thứ {i + 1}/{vouchers.Count}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (iv.InventoryVoucherLines.Count == 0)
                {
                    SwitchVoucherTo(i);
                    MessageBox.Show($"Vui lòng nhập ít nhất một dòng chi tiết vật tư cho phiếu kho thứ {i + 1}/{vouchers.Count}.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            _context.Apply();

            this.DialogResult = true;
            this.Close();
        }

        private void viewDetails_RowDeleted(object sender, Views.Base.RowDeletedEventArgs e)
        {
            _context?.RecalculateTotals();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.S && (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                BtnAccept_Click(this, (RoutedEventArgs)null);
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.P && (System.Windows.Input.Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) == System.Windows.Input.ModifierKeys.Control)
            {
                ExecutePrint();
                e.Handled = true;
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                BtnCancel_Click(this, (RoutedEventArgs)null);
                e.Handled = true;
            }
        }

        private void BtnAddRow_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            viewDetails.AddNewRow();
        }

        private void BtnDeleteRow_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (viewDetails.FocusedRowHandle >= 0)
            {
                viewDetails.ExecuteDeleteRow(viewDetails.FocusedRowHandle);
            }
        }

        private void BtnPrint_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            ExecutePrint();
        }

        private void ExecutePrint()
        {
            string voucherTypeDesc = (cboVoucherType.SelectedItem as MaterialBillTypeItem)?.Description ?? "Phiếu kho";
            string billNo = _context?.InventoryVoucher?.Vatbillno ?? "";
            string title = (_context?.MaterialEntry?.Dbcr == 1) ? "Phiếu Nhập Kho" : "Phiếu Xuất Kho";
            string info = string.IsNullOrEmpty(billNo) ? "" : $" (Số HĐ/CT: {billNo})";
            MessageBox.Show($"Xem trước và In: {title} - {voucherTypeDesc}{info}.", "In phiếu kho", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAccept_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            BtnAccept_Click(sender, (RoutedEventArgs)null);
        }

        private void BtnCancel_Click(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            BtnCancel_Click(sender, (RoutedEventArgs)null);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
