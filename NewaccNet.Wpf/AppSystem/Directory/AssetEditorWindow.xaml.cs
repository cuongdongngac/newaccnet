using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class AssetEditorWindow : BaseWindow
    {
        private AssetEntity _entity;
        private bool _isNew;

        public AssetEditorWindow(AssetEntity entity, bool isNew)
        {
            InitializeComponent();
            _entity = entity;
            _isNew = isNew;

            this.Title = isNew ? "Thêm Mới Tài Sản Cố Định" : $"Sửa Tài Sản: {entity.AssetId}";
            LoadLookups();
            BindData();
        }

        private void LoadLookups()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var depts = new EntityCollection<DepartmentEntity>();
                    adapter.FetchEntityCollection(depts, null);
                    cboDept.ItemsSource = depts;

                    var costObjs = new EntityCollection<CostObjectEntity>();
                    adapter.FetchEntityCollection(costObjs, null);
                    cboCostObj.ItemsSource = costObjs;

                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);
                    cboAsAcct.ItemsSource = accounts;
                    cboExAcct.ItemsSource = accounts;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu lookup: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BindData()
        {
            if (!_isNew)
            {
                txtAssetId.Text = _entity.AssetId;
                txtAssetId.IsReadOnly = true;
            }

            txtAssetHandle.Text = _entity.AssetHandle;
            txtAssetName.Text = _entity.AssetName;
            txtUnit.Text = _entity.Unit;
            dtDate.EditValue = _entity.Date;
            txtQty.EditValue = _entity.Qty;
            txtPrice.EditValue = _entity.Price;
            txtTimeUsing.EditValue = _entity.Timeusing;
            txtCountry.Text = _entity.Country;

            gridTracking.ItemsSource = _entity.AssetTrackings;

            txtAssetName.Focus();
            if (_isNew) txtAssetId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var view = gridTracking.View as DevExpress.Xpf.Grid.TableView;
            if (view != null)
            {
                view.CommitEditing();
                view.CloseEditor();
                view.FocusedRowHandle = DevExpress.Xpf.Grid.GridControl.InvalidRowHandle;
            }

            if (string.IsNullOrWhiteSpace(txtAssetId.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã tài sản.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAssetId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAssetName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên tài sản.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAssetName.Focus();
                return;
            }

            if (_isNew)
                _entity.AssetId = txtAssetId.Text.Trim();

            _entity.AssetHandle = string.IsNullOrWhiteSpace(txtAssetHandle.Text) ? null : txtAssetHandle.Text.Trim();
            _entity.AssetName = txtAssetName.Text.Trim();
            _entity.Unit = string.IsNullOrWhiteSpace(txtUnit.Text) ? null : txtUnit.Text.Trim();
            _entity.Date = (DateTime?)dtDate.EditValue;
            _entity.Country = string.IsNullOrWhiteSpace(txtCountry.Text) ? null : txtCountry.Text.Trim();

            double qty;
            double price;
            double timeUsing;
            _entity.Qty = double.TryParse(txtQty.Text, out qty) ? qty : (double?)null;
            _entity.Price = double.TryParse(txtPrice.Text, out price) ? price : (double?)null;
            _entity.Timeusing = double.TryParse(txtTimeUsing.Text, out timeUsing) ? timeUsing : (double?)null;

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnAddTracking_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var view = gridTracking.View as DevExpress.Xpf.Grid.TableView;
            view?.MoveLastRow();
            view?.AddNewRow();
        }

        private void BtnDeleteTracking_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var selected = gridTracking.SelectedItem as AssetTrackingEntity;
            if (selected == null) return;

            if (MessageBox.Show($"Xóa dòng luân chuyển bộ phận '{selected.Deptid}'?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var source = gridTracking.ItemsSource as EntityCollection<AssetTrackingEntity>;
                if (source != null) source.Remove(selected);
            }
        }

        private void BtnRefreshTracking_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            gridTracking.ItemsSource = null;
            gridTracking.ItemsSource = _entity.AssetTrackings;
        }

        private void BtnSaveTracking_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            if (gridTracking.View != null)
            {
                gridTracking.View.CommitEditing();
            }
            MessageBox.Show("Đã xác nhận các dòng thay đổi. Nhấn 'Lưu & Đóng' ở dưới để ghi xuống CSDL.",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBestFit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var view = gridTracking.View as DevExpress.Xpf.Grid.TableView;
            view?.BestFitColumns();
        }

        private void TableView_InitNewRow(object sender, InitNewRowEventArgs e)
        {
            var newRow = gridTracking.GetRow(e.RowHandle) as AssetTrackingEntity;
            if (newRow != null)
            {
                newRow.AssetId = _entity.AssetId;
                newRow.Begindate = DateTime.Today;
            }
        }

        private void TableView_RowUpdated(object sender, RowEventArgs e)
        {
            var entity = e.Row as AssetTrackingEntity;
            if (entity == null) return;

            if (string.IsNullOrWhiteSpace(entity.AssetId))
            {
                entity.AssetId = _entity.AssetId;
            }
        }
    }
}
