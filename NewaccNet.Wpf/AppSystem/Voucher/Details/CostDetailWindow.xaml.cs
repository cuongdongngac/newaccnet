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

        public CostDetailWindow(JournalEntryEntity parentEntry, bool hasTax)
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
                    var elements = new EntityCollection<CostElementEntity>();
                    adapter.FetchEntityCollection(elements, null);
                    lookupCostElement.ItemsSource = elements;

                    var objects = new EntityCollection<CostObjectEntity>();
                    adapter.FetchEntityCollection(objects, null);
                    lookupCostObject.ItemsSource = objects;

                    if (_hasTax)
                    {
                        var partners = new EntityCollection<PartnerEntity>();
                        adapter.FetchEntityCollection(partners, null);
                        lookupVatObject.ItemsSource = partners;

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

        private void GridCostDetails_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.IsGetData)
            {
                var row = GridCostDetails.GetRowByListIndex(e.ListSourceRowIndex) as CostDetailEntity;
                if (row == null) return;
                
                if (e.Column.FieldName == "CostObjectName")
                {
                    if (string.IsNullOrEmpty(row.CostObjectId)) return;
                    var obj = (lookupCostObject.ItemsSource as System.Collections.Generic.IEnumerable<CostObjectEntity>)?.FirstOrDefault(x => x.Id == row.CostObjectId);
                    e.Value = obj?.ObjectName;
                }
                else if (e.Column.FieldName == "CostElementName")
                {
                    if (string.IsNullOrEmpty(row.CostElementId)) return;
                    var el = (lookupCostElement.ItemsSource as System.Collections.Generic.IEnumerable<CostElementEntity>)?.FirstOrDefault(x => x.Id == row.CostElementId);
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

            double totalAmount = 0;
            foreach (var item in _parentEntry.CostDetails)
            {
                totalAmount += item.Amount ?? 0;
            }

            _parentEntry.Amount = totalAmount > 0 ? totalAmount : 0;

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








