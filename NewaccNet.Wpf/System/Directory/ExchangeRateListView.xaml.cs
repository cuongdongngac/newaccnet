using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class ExchangeRateListView : BaseDictionaryWindow
    {
        public ExchangeRateListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu tỷ giá...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // 1. Tải danh sách tiền tệ để đổ vào ComboBoxEditSettings
                    var currencies = new EntityCollection<CurrencyEntity>();
                    adapter.FetchEntityCollection(currencies, null);
                    cboCurrency.ItemsSource = currencies;

                    // 2. Tải danh sách tỷ giá gợi ý
                    var items = new EntityCollection<ExchangeRateEntity>();
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
            var entity = e.Row as ExchangeRateEntity;
            if (entity == null) return;

            if (string.IsNullOrWhiteSpace(entity.CurrencyId))
            {
                MessageBox.Show("Mã tiền tệ không được để trống!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH MỤC TỶ GIÁ GỢI Ý");
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as ExchangeRateEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa tỷ giá này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<ExchangeRateEntity>;
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
