using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class CategoryListView : BaseDictionaryWindow
    {
        public CategoryListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu nhóm vật tư...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // 1. Tải danh mục Thuế suất để nạp vào ComboBoxEditSettings
                    var taxRates = new EntityCollection<TaxRateEntity>();
                    adapter.FetchEntityCollection(taxRates, null);
                    cboTaxRate.ItemsSource = taxRates;

                    // 2. Tải danh mục Nhóm vật tư
                    var items = new EntityCollection<CategoryEntity>();
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
            var entity = e.Row as CategoryEntity;
            if (entity == null) return;

            if (string.IsNullOrWhiteSpace(entity.Categoryid))
            {
                MessageBox.Show("Mã nhóm vật tư không được để trống!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(entity, true, false);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadData(); 
            }
        }

        private void MenuSave_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridControl.View.CommitEditing();
            MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH SÁCH NHÓM VẬT TƯ");
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as CategoryEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa nhóm '{selectedItem.Categoryid}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<CategoryEntity>;
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
