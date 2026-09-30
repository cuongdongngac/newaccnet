using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DataAccess.Linq;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class AssetListView : BaseDictionaryWindow
    {
        public AssetListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải danh mục Tài sản cố định...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var items = new EntityCollection<AssetEntity>();
                    adapter.FetchEntityCollection(items, null);
                    gridControl.ItemsSource = items.Count > 0 ? items : null;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new AssetEntity(), true);
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();
        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e) => EditSelected();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH MỤC TÀI SẢN CỐ ĐỊNH");
        }

        private void EditSelected()
        {
            var selectedItem = gridControl.SelectedItem as AssetEntity;
            if (selectedItem == null) return;

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.AssetEntity);
                prefetch.Add(AssetEntity.PrefetchPathAssetTrackings);
                adapter.FetchEntity(selectedItem, prefetch);
            }

            OpenEditor(selectedItem, false);
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as AssetEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa tài sản '{selectedItem.AssetId} - {selectedItem.AssetName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<AssetEntity>;
                        if (dataSource != null) dataSource.Remove(selectedItem);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(AssetEntity entity, bool isNew)
        {
            var editor = new AssetEditorWindow(entity, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.StartTransaction(System.Data.IsolationLevel.ReadCommitted, "SaveAssetUOW");
                        try
                        {
                            if (!isNew)
                            {
                                var dbAsset = new AssetEntity(entity.AssetId);
                                var prefetch = new PrefetchPath2((int)DataAccess.EntityType.AssetEntity);
                                prefetch.Add(AssetEntity.PrefetchPathAssetTrackings);
                                if (adapter.FetchEntity(dbAsset, prefetch))
                                {
                                    foreach (var dbTrack in dbAsset.AssetTrackings)
                                    {
                                        var kept = entity.AssetTrackings.FirstOrDefault(t => !t.IsNew && t.Assettrackingid == dbTrack.Assettrackingid);
                                        if (kept == null)
                                        {
                                            adapter.DeleteEntity(dbTrack);
                                        }
                                    }
                                }
                            }

                            adapter.SaveEntity(entity, refetchAfterSave: true, recurse: true);
                            adapter.Commit();
                        }
                        catch
                        {
                            adapter.Rollback();
                            throw;
                        }
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<AssetEntity>;
                    if (dataSource != null)
                    {
                        if (isNew && !dataSource.Contains(entity)) dataSource.Add(entity);
                    }
                    else if (isNew)
                    {
                        LoadData();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
