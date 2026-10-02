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
    public partial class AssetDetailWindow : NewaccNet.Wpf.Views.Base.BaseDetailWindow
    {
        private JournalEntryEntity _parentEntry;
        private System.Collections.Generic.List<AssetDetailEntity> _originalList;
        private bool _isSaved = false;

        public AssetDetailWindow(JournalEntryEntity parentEntry)
        {
            InitializeComponent();
            _parentEntry = parentEntry;
            this.DataContext = _parentEntry;

            _originalList = _parentEntry.AssetDetails.ToList();
            foreach (var item in _originalList) item.SaveFields("undo");

            LoadDictionaries();
        }

        private void LoadDictionaries()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var assets = new EntityCollection<AssetEntity>();
                    adapter.FetchEntityCollection(assets, null);
                    lookupAsset.ItemsSource = assets;

                    var sources = new EntityCollection<SourceEntity>();
                    adapter.FetchEntityCollection(sources, null);
                    lookupSource.ItemsSource = sources;

                    var reasons = new EntityCollection<ReasonEntity>();
                    adapter.FetchEntityCollection(reasons, null);
                    lookupReason.ItemsSource = reasons;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void TableViewAsset_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newRow = GridAssetDetails.GetRow(e.RowHandle) as AssetDetailEntity;
            if (newRow != null)
            {
                newRow.Quantity = 1;
                newRow.Price = 0;
            }
        }

        protected override void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            TableViewAsset.CloseEditor();
            TableViewAsset.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.InvalidRowHandle;
            TableViewAsset.CommitEditing();

            double totalAmount = 0;
            foreach (var item in _parentEntry.AssetDetails)
            {
                totalAmount += (item.Quantity ?? 0) * (item.Price ?? 0);
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
                _parentEntry.AssetDetails.Clear();
                foreach (var item in _originalList) _parentEntry.AssetDetails.Add(item);
            }
            base.OnClosing(e);
        }
    }
}
