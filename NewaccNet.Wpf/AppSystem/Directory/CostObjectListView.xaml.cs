using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class CostObjectListView : BaseDictionaryWindow
    {
        public CostObjectListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu đối tượng chi phí...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var items = new EntityCollection<CostObjectEntity>();
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

        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new CostObjectEntity(), true);
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();
        private void TableView_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e) => EditSelected();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH SÁCH ĐỐI TƯỢNG TẬP HỢP CHI PHÍ");
        }

        private void EditSelected()
        {
            var selectedItem = gridControl.SelectedItem as CostObjectEntity;
            if (selectedItem == null) return;
            OpenEditor(selectedItem, false);
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as CostObjectEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa đối tượng '{selectedItem.Id} - {selectedItem.ObjectName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<CostObjectEntity>;
                        if (dataSource != null) dataSource.Remove(selectedItem);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(CostObjectEntity entity, bool isNew)
        {
            var editor = new CostObjectEditorWindow(entity, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(entity, true, false);
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<CostObjectEntity>;
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
