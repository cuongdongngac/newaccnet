using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.AppSystem.Services; // Thêm refer đến Service đã tạo

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class MaterialPriceListView : BaseDictionaryWindow
    {
        private readonly IInventoryValuationService _valuationService;

        public MaterialPriceListView()
        {
            InitializeComponent();
            
            // Khởi tạo Service (trong thực tế có thể dùng DI Container)
            _valuationService = new InventoryValuationService();

            Loaded += MaterialPriceListView_Loaded;
        }

        private void MaterialPriceListView_Loaded(object sender, RoutedEventArgs e)
        {
            ((DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings)cboCalcMethod.EditSettings).ItemsSource = new[]
            {
                new { Value = CostingMethod.MovingAverage, Text = "Bình quân gia quyền tức thời" },
                new { Value = CostingMethod.Fifo,          Text = "Nhập trước - Xuất trước (FIFO)" }
            };
            cboCalcMethod.EditValue = CostingMethod.MovingAverage;
            dteAsOfDate.EditValue = DateTime.Today;

            LoadComboData();
            LoadData();
        }

        private void LoadComboData()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var warehouses = new EntityCollection<WarehouseEntity>();
                    adapter.FetchEntityCollection(warehouses, null);
                    
                    // Bind cho bộ lọc Toolbar
                    ((DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings)cboWarehouseFilter.EditSettings).ItemsSource = warehouses;
                    
                    // Bind cho cột Grid để hiển thị Tên kho
                    cboGridWarehouse.ItemsSource = warehouses;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu Kho: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string SelectedWarehouseId
        {
            get
            {
                var id = cboWarehouseFilter.EditValue as string;
                return string.IsNullOrWhiteSpace(id) ? null : id;
            }
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu giá vật tư...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var prices = new EntityCollection<MaterialPriceEntity>();
                    
                    // Nếu có filter kho
                    string selectedWarehouse = SelectedWarehouseId;
                    IRelationPredicateBucket filter = null;
                    if (selectedWarehouse != null)
                    {
                        filter = new RelationPredicateBucket(MaterialPriceFields.WarehouseId == selectedWarehouse);
                    }

                    var path = new PrefetchPath2(DataAccess.EntityType.MaterialPriceEntity);
                    path.Add(MaterialPriceEntity.PrefetchPathInventoryItem);

                    adapter.FetchEntityCollection(prices, filter, path);
                    gridControl.ItemsSource = prices.Count > 0 ? prices : null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                LoadData();
            }
        }

        private void BtnRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void BtnCalculatePrice_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (!(cboCalcMethod.EditValue is CostingMethod method)) return;
            DateTime asOfDate = dteAsOfDate.EditValue is DateTime d ? d.Date : DateTime.Today;
            string selectedWarehouse = SelectedWarehouseId;

            try
            {
                ShowLoading("Hệ thống đang chạy thuật toán tính giá...");

                // Tạm thời trên giao diện, ta có thể dùng CheckBox "Tính lại từ đầu" để lấy rebuildFromScratch.
                // Ở đây mình ví dụ gọi Chế độ 2: Tính từ ngày X (fromDate) dựa trên bảng giá.
                DateTime? fromDate = null; // Hoặc lấy từ 1 dteFromDate.EditValue
                bool rebuildFromScratch = true; // Hoặc false tuỳ theo chế độ bạn chọn

                var result = _valuationService.Recalculate(method, fromDate, asOfDate, rebuildFromScratch, null, selectedWarehouse);

                string msg = $"Đã tính xong giá tồn đến ngày {asOfDate:dd/MM/yyyy}.\n" +
                             $"- Số cặp Vật tư/Kho: {result.PairCount}\n" +
                             $"- Số dòng nhập/xuất đã xử lý: {result.MovementCount}";
                if (result.SkippedVoucherCount > 0)
                    msg += $"\n- Bỏ qua {result.SkippedVoucherCount} phiếu kho thiếu ngày/kho hoặc không xác định được Nhập/Xuất.";

                MessageBox.Show(msg, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Refresh lại Grid sau khi tính
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình tính giá: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }
        private void TableView_RowUpdated(object sender, DevExpress.Xpf.Grid.RowEventArgs e)
        {
            if (e.Row is MaterialPriceEntity row)
            {
                SaveRow(row);
            }
        }

        private void TableView_InitNewRow(object sender, DevExpress.Xpf.Grid.InitNewRowEventArgs e)
        {
            var row = gridControl.GetRowByListIndex(e.RowHandle) as MaterialPriceEntity;
            if (row != null)
            {
                // Nếu đang lọc một kho cụ thể, tự động gán kho đó cho dòng mới để kế toán đỡ phải chọn lại
                var currentWarehouse = SelectedWarehouseId;
                if (!string.IsNullOrEmpty(currentWarehouse))
                {
                    row.WarehouseId = currentWarehouse;
                }
            }
        }

        private void BtnSave_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridControl.View.CommitEditing();
            // Lưới đã auto-save qua sự kiện RowUpdated, nút lưu giúp chốt ô đang gõ dở
        }

        private void SaveRow(MaterialPriceEntity row)
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(row, true, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
