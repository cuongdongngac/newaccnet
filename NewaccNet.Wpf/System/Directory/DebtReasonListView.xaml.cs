using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class DebtReasonListView : BaseDictionaryWindow
    {
        public DebtReasonListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu lý do công nợ...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var items = new EntityCollection<DebtReasonEntity>();
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

        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new DebtReasonEntity() { Visible = true }, true);
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();
        private void TableView_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e) => EditSelected();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH SÁCH LÝ DO CÔNG NỢ");
        }

        private void EditSelected()
        {
            var selectedItem = gridControl.SelectedItem as DebtReasonEntity;
            if (selectedItem == null) return;
            OpenEditor(selectedItem, false);
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as DebtReasonEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa lý do '{selectedItem.Id} - {selectedItem.ReasonName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<DebtReasonEntity>;
                        if (dataSource != null) dataSource.Remove(selectedItem);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(DebtReasonEntity entity, bool isNew)
        {
            var editor = new DebtReasonEditorWindow(entity, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(entity, true, false);
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<DebtReasonEntity>;
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
