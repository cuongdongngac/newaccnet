using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class SourceListView : BaseDictionaryWindow
    {
        public SourceListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu nguồn tài sản...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var items = new EntityCollection<SourceEntity>();
                    adapter.FetchEntityCollection(items, null);
                    gridControl.ItemsSource = items;
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

        private void TableView_RowUpdated(object sender, RowEventArgs e)
        {
            SaveEntity(e.Row as SourceEntity, showEmptyCodeWarning: true);
        }

        private void MenuSave_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridControl.View.CommitEditing();

            var dataSource = gridControl.ItemsSource as EntityCollection<SourceEntity>;
            if (dataSource == null)
            {
                return;
            }

            foreach (var entity in dataSource)
            {
                if (!entity.IsNew && !entity.IsDirty)
                {
                    continue;
                }

                if (!SaveEntity(entity, showEmptyCodeWarning: true))
                {
                    return;
                }
            }

            MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool SaveEntity(SourceEntity entity, bool showEmptyCodeWarning)
        {
            if (entity == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(entity.Srcode))
            {
                if (showEmptyCodeWarning)
                {
                    MessageBox.Show("Mã nguồn không được để trống!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return false;
            }

            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(entity, true, false);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadData();
                return false;
            }
        }

        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH SÁCH NGUỒN TÀI SẢN");
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as SourceEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa nguồn tài sản '{selectedItem.Srcode}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<SourceEntity>;
                        if (dataSource != null) dataSource.Remove(selectedItem);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
