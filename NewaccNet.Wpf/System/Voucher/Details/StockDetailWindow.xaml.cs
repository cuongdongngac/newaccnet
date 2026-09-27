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
    public partial class StockDetailWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
    {
        private JournalEntryEntity _parentEntry;
        private System.Collections.Generic.List<InvestmentDetailEntity> _originalList;
        private bool _isSaved = false;

        public StockDetailWindow(JournalEntryEntity parentEntry)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            this.DataContext = _parentEntry;

            _originalList = _parentEntry.InvestmentDetails.ToList();
            foreach (var item in _originalList) item.SaveFields("undo");

            LoadDictionaries();
        }

        private void LoadDictionaries()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var stocks = new EntityCollection<StockEntity>();
                    adapter.FetchEntityCollection(stocks, null);
                    lookupStock.ItemsSource = stocks;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void TableViewStock_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newRow = GridStockDetails.GetRow(e.RowHandle) as InvestmentDetailEntity;
            if (newRow != null)
            {
                newRow.Quantity = 0;
                newRow.Inprice = 0;
            }
        }

        protected override void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TableViewStock.CloseEditor();
            TableViewStock.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.InvalidRowHandle;
            TableViewStock.CommitEditing();

            double totalAmount = 0;
            foreach (var item in _parentEntry.InvestmentDetails)
            {
                item.Originamount = (item.Quantity ?? 0) * (item.Inprice ?? 0);
                totalAmount += item.Originamount ?? 0;
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
                foreach (var item in _originalList) item.RollbackFields("undo");
                _parentEntry.InvestmentDetails.Clear();
                foreach (var item in _originalList) _parentEntry.InvestmentDetails.Add(item);
            }
            base.OnClosing(e);
        }
    }
}
