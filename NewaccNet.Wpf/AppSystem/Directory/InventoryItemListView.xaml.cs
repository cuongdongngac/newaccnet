using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class InventoryItemListView : BaseDictionaryWindow
    {
        public InventoryItemListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu danh mục vật tư...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // Tải danh mục Category để hiển thị tên thay vì mã trên lưới
                    var categories = new EntityCollection<CategoryEntity>();
                    adapter.FetchEntityCollection(categories, null);
                    cboCategory.ItemsSource = categories;

                    var items = new EntityCollection<InventoryItemEntity>();
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

        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new InventoryItemEntity(), true);
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();
        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e) => EditSelected();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH MỤC VẬT TƯ HÀNG HÓA");
        }

        private void EditSelected()
        {
            var selectedItem = gridControl.SelectedItem as InventoryItemEntity;
            if (selectedItem == null) return;
            OpenEditor(selectedItem, false);
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as InventoryItemEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa vật tư '{selectedItem.MaterialId} - {selectedItem.MaterialName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<InventoryItemEntity>;
                        if (dataSource != null) dataSource.Remove(selectedItem);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(InventoryItemEntity entity, bool isNew)
        {
            var editor = new InventoryItemEditorWindow(entity, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(entity, true, false);
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<InventoryItemEntity>;
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
