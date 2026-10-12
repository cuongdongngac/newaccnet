using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.Voucher.Details
{
    public partial class CostDetailWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
    {
        private JournalEntryEntity _parentEntry;
        private System.Collections.Generic.List<CostDetailEntity> _originalList;
        private bool _isSaved = false;
        private bool _hasTax = false;

        public EntityCollection<CostObjectEntity> CostObjects { get; } = new EntityCollection<CostObjectEntity>();
        public EntityCollection<CostElementEntity> CostElements { get; } = new EntityCollection<CostElementEntity>();
        public EntityCollection<PartnerEntity> Partners { get; } = new EntityCollection<PartnerEntity>();

        public CostDetailWindow(JournalEntryEntity parentEntry, bool hasTax = false)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            _hasTax = hasTax;
            this.DataContext = _parentEntry;

            _originalList = _parentEntry.CostDetails.ToList();
            foreach (var item in _originalList) 
            {
                item.SaveFields("undo");
                if (item.ExpenseTaxLine != null) item.ExpenseTaxLine.SaveFields("undo");
            }

            if (!_hasTax)
            {
                colTax.Width = new GridLength(0);
                pnlTax.Visibility = Visibility.Collapsed;
            }

            LoadDictionaries();
        }

        private void LoadDictionaries()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    CostElements.Clear();
                    adapter.FetchEntityCollection(CostElements, null);

                    CostObjects.Clear();
                    adapter.FetchEntityCollection(CostObjects, null);

                    if (_hasTax)
                    {
                        Partners.Clear();
                        adapter.FetchEntityCollection(Partners, null);
                        lookupVatObject.ItemsSource = Partners;

                        var taxRates = new EntityCollection<TaxRateEntity>();
                        adapter.FetchEntityCollection(taxRates, null);
                        lookupTaxRate.ItemsSource = taxRates.Select(t => new { Taxrateid = (short)t.Taxrateid, Rate = t.Rate }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void TableViewCost_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newRow = GridCostDetails.GetRow(e.RowHandle) as CostDetailEntity;
            if (newRow != null)
            {
                newRow.Amount = 0;
                
                if (_hasTax && newRow.ExpenseTaxLine == null)
                {
                    newRow.ExpenseTaxLine = new ExpenseTaxLineEntity();
                }
            }
        }

        private void BtnAddCostObject_Click(object sender, RoutedEventArgs e)
        {
            var newObj = new CostObjectEntity();
            var editor = new Directory.CostObjectEditorWindow(newObj, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newObj, true);
                    }

                    if (!CostObjects.Any(x => x.Id == newObj.Id))
                    {
                        CostObjects.Add(newObj);
                    }

                    var buttonInfo = sender as DevExpress.Xpf.Editors.ButtonInfo;
                    var cellData = buttonInfo?.DataContext as EditGridCellData;
                    var row = cellData?.RowData?.Row as CostDetailEntity;
                    if (row == null)
                    {
                        row = GridCostDetails.SelectedItem as CostDetailEntity;
                    }

                    if (row != null)
                    {
                        row.CostObjectId = newObj.Id;
                    }
                    GridCostDetails.SetCellValue(TableViewCost.FocusedRowHandle, "CostObjectId", newObj.Id);
                    TableViewCost.CloseEditor();
                    GridCostDetails.RefreshRow(TableViewCost.FocusedRowHandle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu đối tượng chi phí: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddCostElement_Click(object sender, RoutedEventArgs e)
        {
            var newEl = new CostElementEntity();
            var editor = new Directory.CostElementEditorWindow(newEl, true);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(newEl, true);
                    }

                    if (!CostElements.Any(x => x.Id == newEl.Id))
                    {
                        CostElements.Add(newEl);
                    }

                    var buttonInfo = sender as DevExpress.Xpf.Editors.ButtonInfo;
                    var cellData = buttonInfo?.DataContext as EditGridCellData;
                    var row = cellData?.RowData?.Row as CostDetailEntity;
                    if (row == null)
                    {
                        row = GridCostDetails.SelectedItem as CostDetailEntity;
                    }

                    if (row != null)
                    {
                        row.CostElementId = newEl.Id;
                    }
                    GridCostDetails.SetCellValue(TableViewCost.FocusedRowHandle, "CostElementId", newEl.Id);
                    TableViewCost.CloseEditor();
                    GridCostDetails.RefreshRow(TableViewCost.FocusedRowHandle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu yếu tố chi phí: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddCostVatPartner_Click(object sender, RoutedEventArgs e)
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

                    if (!Partners.Any(x => x.Id == newPartner.Id))
                    {
                        Partners.Add(newPartner);
                    }

                    lookupVatObject.EditValue = newPartner.Id;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu đối tượng thuế: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GridCostDetails_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.IsGetData)
            {
                var row = GridCostDetails.GetRowByListIndex(e.ListSourceRowIndex) as CostDetailEntity;
                if (row == null) return;
                
                if (e.Column.FieldName == "CostObjectName")
                {
                    if (string.IsNullOrEmpty(row.CostObjectId)) return;
                    var obj = CostObjects.FirstOrDefault(x => x.Id == row.CostObjectId);
                    e.Value = obj?.ObjectName;
                }
                else if (e.Column.FieldName == "CostElementName")
                {
                    if (string.IsNullOrEmpty(row.CostElementId)) return;
                    var el = CostElements.FirstOrDefault(x => x.Id == row.CostElementId);
                    e.Value = el?.ElementName;
                }
            }
        }

        private void TableViewCost_CellValueChanged(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "CostObjectId" || e.Column.FieldName == "CostElementId")
            {
                GridCostDetails.RefreshRow(e.RowHandle);
            }
        }

        private void TableViewCost_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var currentRow = e.NewRow as CostDetailEntity;
            if (currentRow != null && _hasTax && currentRow.ExpenseTaxLine == null)
            {
                // Safety check in case it was created without tax line
                currentRow.ExpenseTaxLine = new ExpenseTaxLineEntity();
            }
        }

        private void lookupVatObject_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var editor = sender as DevExpress.Xpf.Grid.LookUp.LookUpEdit;
            if (editor == null) return;
            var partner = editor.SelectedItem as PartnerEntity;
            var taxLine = pnlTax.DataContext as ExpenseTaxLineEntity; if (taxLine == null) taxLine = (GridCostDetails.CurrentItem as CostDetailEntity)?.ExpenseTaxLine;
            if (partner != null && taxLine != null)
            {
                taxLine.Vatobjectname = partner.CustomerName;
                taxLine.Vataddress = partner.Address;
                taxLine.Taxno = partner.Taxno;
            }
        }

        private void lookupTaxRate_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var editor = sender as DevExpress.Xpf.Grid.LookUp.LookUpEdit;
            if (editor == null) return;
            dynamic taxRate = editor.SelectedItem;
            var taxLine = pnlTax.DataContext as ExpenseTaxLineEntity; if (taxLine == null) taxLine = (GridCostDetails.CurrentItem as CostDetailEntity)?.ExpenseTaxLine;
            if (taxRate != null && taxLine != null)
            {
                taxLine.Rate = taxRate.Rate;
            }
        }

        protected override void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TableViewCost.CloseEditor();
            TableViewCost.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.InvalidRowHandle;
            TableViewCost.CommitEditing();

            // Dùng Reflection kiểm tra UpdateFilter: Nếu UpdateFilter = true thì chỉ cập nhật từ con vật tư
            if (_parentEntry.CanUpdateFromChild(this))
            {
                double totalAmount = 0;
                foreach (var item in _parentEntry.CostDetails)
                {
                    totalAmount += item.Amount ?? 0;
                }

                _parentEntry.Amount = Math.Round(totalAmount > 0 ? totalAmount : 0, 0);
            }

            _isSaved = true;
            this.DialogResult = true;
            this.Close();
        }

        protected override void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isSaved)
            {
                foreach (var item in _originalList) 
                {
                    item.RollbackFields("undo");
                    if (item.ExpenseTaxLine != null) item.ExpenseTaxLine.RollbackFields("undo");
                }
                _parentEntry.CostDetails.Clear();
                foreach (var item in _originalList) _parentEntry.CostDetails.Add(item);
            }
            base.OnClosing(e);
        }
    }
}








