using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Editors;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class MaterialDetailWindow : Window
    {
        private InventoryVoucherEntity _currentEntity;
        private JournalEntryEntity _parentEntry;
        private bool _isNew;

        private List<InventoryItemEntity> _materials = new List<InventoryItemEntity>();
        private List<TaxRateEntity> _taxRates = new List<TaxRateEntity>();
        private List<PartnerEntity> _partners = new List<PartnerEntity>();

        public string SelectedCostAccount { get; private set; } = "";
        public string SelectedRevenueAccount { get; private set; } = "";
        public string SelectedTaxAccount { get; private set; } = "";
        public string SelectedTotalAccount { get; private set; } = "";
        
        public string VoucherType { get; private set; } = "";
        public double TotalCostAmount { get; private set; } = 0;
        public double TotalSellAmount { get; private set; } = 0;
        public double TotalTaxAmount { get; private set; } = 0;

        public class VoucherTypeInfo
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public int Dbcr { get; set; }
        }

        public MaterialDetailWindow(JournalEntryEntity parentEntry, bool isNew)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            _isNew = isNew;

            LoadLookupData();
            InitData();
        }

        private void LoadLookupData()
        {
            var voucherTypes = new List<VoucherTypeInfo>();
            if (_parentEntry.Dbcr == 1)
            {
                voucherTypes.Add(new VoucherTypeInfo { Id = "NM", Name = "Nhập mua", Dbcr = 1 });
                voucherTypes.Add(new VoucherTypeInfo { Id = "NNB", Name = "Nhập nội bộ", Dbcr = 1 });
            }
            else
            {
                voucherTypes.Add(new VoucherTypeInfo { Id = "XB", Name = "Xuất bán", Dbcr = -1 });
                voucherTypes.Add(new VoucherTypeInfo { Id = "XNB", Name = "Xuất nội bộ", Dbcr = -1 });
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
                lueMaterial.ItemsSource = _materials;

                var taxRates = new EntityCollection<TaxRateEntity>();
                adapter.FetchEntityCollection(taxRates, null);
                _taxRates = taxRates.ToList();
                lueTaxRate.ItemsSource = _taxRates.Select(t => new { Taxrateid = (short)t.Taxrateid, Rate = t.Rate }).ToList();

                var partners = new EntityCollection<PartnerEntity>();
                adapter.FetchEntityCollection(partners, null);
                _partners = partners.ToList();
                lueVatObject.ItemsSource = _partners;
            }
        }

        private void InitData()
        {
            if (_isNew || _parentEntry.InventoryVouchers.Count == 0)
            {
                _currentEntity = new InventoryVoucherEntity();
                _parentEntry.InventoryVouchers.Add(_currentEntity);
                
                if (cboVoucherType.ItemsSource is List<VoucherTypeInfo> list && list.Count > 0)
                {
                    _currentEntity.ExobjectId = list[0].Id;
                    cboVoucherType.EditValue = list[0].Id;
                }
            }
            else
            {
                _currentEntity = _parentEntry.InventoryVouchers[0];
                cboVoucherType.EditValue = _currentEntity.ExobjectId;
            }

            this.DataContext = _currentEntity;
            UpdateUIVisibility();
            CalculateTotals();
        }

        private void cboVoucherType_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (_currentEntity == null) return;
            string type = cboVoucherType.EditValue as string ?? "";
            _currentEntity.ExobjectId = type;
            
            if (type == "NM" || type == "XB")
                chkHasTax.IsChecked = true;
            else
                chkHasTax.IsChecked = false;

            UpdateUIVisibility();
        }

        private void chkHasTax_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateUIVisibility();
        }

        private void UpdateUIVisibility()
        {
            string type = cboVoucherType.EditValue as string ?? "";
            bool hasTax = chkHasTax.IsChecked == true;

            if (hasTax)
                pnlTaxInfo.Visibility = Visibility.Visible;
            else
                pnlTaxInfo.Visibility = Visibility.Collapsed;

            if (type == "XB")
            {
                bandSalesAndTax.Visible = true;
                itemTotalSell.Visibility = Visibility.Visible;
                itemTotalTax.Visibility = Visibility.Visible;
                itemTotalAll.Visibility = Visibility.Visible;
                GridMaterialDetails.Columns["MaterialTaxLine.SellPrice"].Visible = true;
                GridMaterialDetails.Columns["SellAmount"].Visible = true;
            }
            else if (type == "NM")
            {
                bandSalesAndTax.Visible = true;
                GridMaterialDetails.Columns["MaterialTaxLine.SellPrice"].Visible = false;
                GridMaterialDetails.Columns["SellAmount"].Visible = false;
                GridMaterialDetails.Columns["MaterialTaxLine.Taxrateid"].Visible = true;
                GridMaterialDetails.Columns["TaxAmount"].Visible = true;
                
                itemTotalSell.Visibility = Visibility.Collapsed;
                itemTotalTax.Visibility = Visibility.Visible;
                itemTotalAll.Visibility = Visibility.Visible;
            }
            else
            {
                bandSalesAndTax.Visible = false;
                itemTotalSell.Visibility = Visibility.Collapsed;
                itemTotalTax.Visibility = Visibility.Collapsed;
                itemTotalAll.Visibility = Visibility.Collapsed;
            }

            EnsureTaxLines(type);
        }

        private void EnsureTaxLines(string type)
        {
            if (_currentEntity == null) return;
            bool needsTaxLine = (type == "XB" || type == "NM");

            foreach (var line in _currentEntity.InventoryVoucherLines)
            {
                if (needsTaxLine && line.MaterialTaxLine == null)
                {
                    line.MaterialTaxLine = new MaterialTaxLineEntity();
                }
            }
        }

        private void viewDetails_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var row = GridMaterialDetails.GetRowByListIndex(e.RowHandle) as InventoryVoucherLineEntity;
            if (row != null)
            {
                string type = cboVoucherType.EditValue as string ?? "";
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
                string type = cboVoucherType.EditValue as string ?? "";
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
                    var mat = _materials.FirstOrDefault(m => m.MaterialId == row.MaterialId);
                    e.Value = mat?.MaterialName;
                }
            }
            else if (e.Column.FieldName == "Unit" && e.IsGetData)
            {
                if (!string.IsNullOrEmpty(row.MaterialId) && _materials != null)
                {
                    var mat = _materials.FirstOrDefault(m => m.MaterialId == row.MaterialId);
                    e.Value = mat?.Unit;
                }
            }
            else if (e.Column.FieldName == "Amount" && e.IsGetData)
            {
                e.Value = (row.Quantity ?? 0) * (row.Price ?? 0);
            }
            else if (e.Column.FieldName == "SellAmount" && e.IsGetData)
            {
                if (row.MaterialTaxLine != null)
                    e.Value = (row.Quantity ?? 0) * (row.MaterialTaxLine.SellPrice ?? 0);
                else
                    e.Value = 0;
            }
            else if (e.Column.FieldName == "TaxAmount" && e.IsGetData)
            {
                if (row.MaterialTaxLine != null)
                {
                    string type = cboVoucherType.EditValue as string ?? "";
                    double baseAmount = (type == "XB") ? ((row.Quantity ?? 0) * (row.MaterialTaxLine.SellPrice ?? 0)) : ((row.Quantity ?? 0) * (row.Price ?? 0));
                    e.Value = baseAmount * (row.MaterialTaxLine.Rate ?? 0) / 100.0;
                }
                else
                    e.Value = 0;
            }
        }

        private void viewDetails_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "MaterialId")
            {
                GridMaterialDetails.RefreshRow(e.RowHandle);
            }
            else if (e.Column.FieldName == "Quantity" || e.Column.FieldName == "Price" || 
                     e.Column.FieldName == "MaterialTaxLine.SellPrice" || e.Column.FieldName == "MaterialTaxLine.Taxrateid")
            {
                if (e.Column.FieldName == "MaterialTaxLine.Taxrateid")
                {
                    var row = e.Row as InventoryVoucherLineEntity;
                    if (row != null && row.MaterialTaxLine != null && _taxRates != null)
                    {
                        var rate = _taxRates.FirstOrDefault(t => t.Taxrateid == row.MaterialTaxLine.Taxrateid);
                        if (rate != null)
                            row.MaterialTaxLine.Rate = rate.Rate;
                    }
                }
                GridMaterialDetails.RefreshRow(e.RowHandle);
                CalculateTotals();
            }
        }

        private void lueVatObject_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (lueVatObject.SelectedItem is PartnerEntity partner)
            {
                _currentEntity.Vatobjectname = partner.CustomerName;
                _currentEntity.Vataddress = partner.Address;
                _currentEntity.Taxno = partner.Taxno;
            }
        }

        private void CalculateTotals()
        {
            double totalCost = 0;
            double totalSell = 0;
            double totalTax = 0;

            string type = cboVoucherType.EditValue as string ?? "";

            foreach (var line in _currentEntity.InventoryVoucherLines)
            {
                double cost = (line.Quantity ?? 0) * (line.Price ?? 0);
                totalCost += cost;

                if (line.MaterialTaxLine != null)
                {
                    if (type == "XB")
                    {
                        double sell = (line.Quantity ?? 0) * (line.MaterialTaxLine.SellPrice ?? 0);
                        totalSell += sell;
                        totalTax += sell * (line.MaterialTaxLine.Rate ?? 0) / 100.0;
                    }
                    else if (type == "NM")
                    {
                        totalTax += cost * (line.MaterialTaxLine.Rate ?? 0) / 100.0;
                    }
                }
            }

            txtTotalCost.EditValue = totalCost;
            txtTotalSell.EditValue = totalSell;
            txtTotalTax.EditValue = totalTax;
            
            if (type == "XB")
                txtTotalAll.EditValue = totalSell + totalTax;
            else if (type == "NM")
                txtTotalAll.EditValue = totalCost + totalTax;
            else
                txtTotalAll.EditValue = totalCost;
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            viewDetails.CommitEditing();
            GridMaterialDetails.View.CommitEditing();

            if (string.IsNullOrEmpty(_currentEntity.WarehouseId))
            {
                MessageBox.Show("Vui lòng chọn Kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var emptyLines = _currentEntity.InventoryVoucherLines.Where(l => string.IsNullOrEmpty(l.MaterialId)).ToList();
            foreach (var line in emptyLines)
                _currentEntity.InventoryVoucherLines.Remove(line);

            if (_currentEntity.InventoryVoucherLines.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập ít nhất một dòng chi tiết vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.VoucherType = cboVoucherType.EditValue as string ?? "";
            this.TotalCostAmount = Convert.ToDouble(txtTotalCost.EditValue);
            this.TotalSellAmount = Convert.ToDouble(txtTotalSell.EditValue);
            this.TotalTaxAmount = Convert.ToDouble(txtTotalTax.EditValue);

            if (_isNew)
            {
                var accountPopup = new MaterialAccountSelectionWindow(VoucherType, TotalCostAmount, TotalSellAmount, TotalTaxAmount);
                accountPopup.Owner = this;
                if (accountPopup.ShowDialog() == true)
                {
                    this.SelectedCostAccount = accountPopup.CostAccount;
                    this.SelectedRevenueAccount = accountPopup.RevenueAccount;
                    this.SelectedTaxAccount = accountPopup.TaxAccount;
                    this.SelectedTotalAccount = accountPopup.TotalAccount;
                    
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
