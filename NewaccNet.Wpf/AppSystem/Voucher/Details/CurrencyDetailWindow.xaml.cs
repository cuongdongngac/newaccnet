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
    public partial class CurrencyDetailWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
    {
        private JournalEntryEntity _parentEntry;
        private System.Collections.Generic.List<CurrencyDetailEntity> _originalList;
        private bool _isSaved = false;

        public CurrencyDetailWindow(JournalEntryEntity parentEntry)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            this.DataContext = _parentEntry;

            _originalList = _parentEntry.CurrencyDetails.ToList();
            _parentEntry.SaveFields("undo_parent");
            foreach (var item in _originalList) item.SaveFields("undo");

            LoadDictionaries();
        }

        private void LoadDictionaries()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var currencies = new EntityCollection<CurrencyEntity>();
                    adapter.FetchEntityCollection(currencies, null);
                    lookupCurrency.ItemsSource = currencies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void TableViewCurrency_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newRow = GridCurrencyDetails.GetRow(e.RowHandle) as CurrencyDetailEntity;
            if (newRow != null)
            {
                newRow.Quantity = 0;
                newRow.ExchangeRate = 0;
            }
        }

        protected override void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TableViewCurrency.CloseEditor();
            TableViewCurrency.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.InvalidRowHandle;
            TableViewCurrency.CommitEditing();

            _parentEntry.NotifyChildCollectionChanged();

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
                _parentEntry.IsDominoSuspended = true;
                try
                {
                    foreach (var item in _originalList) item.RollbackFields("undo");
                    _parentEntry.CurrencyDetails.Clear();
                    foreach (var item in _originalList) _parentEntry.CurrencyDetails.Add(item);
                    _parentEntry.RollbackFields("undo_parent");
                }
                finally
                {
                    _parentEntry.IsDominoSuspended = false;
                }
            }
            base.OnClosing(e);
        }
    }
}
