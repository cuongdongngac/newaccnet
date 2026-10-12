using System.Windows;
using DataAccess.EntityClasses;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class WarehouseEditorWindow : BaseWindow
    {
        private WarehouseEntity _entity;
        private bool _isNew;

        public WarehouseEditorWindow(WarehouseEntity entity, bool isNew)
        {
            InitializeComponent();
            _entity = entity;
            _isNew = isNew;

            BindData();
        }

        private void BindData()
        {
            if (!_isNew)
            {
                txtWarehouseId.Text = _entity.WarehouseId;
                txtWarehouseId.IsReadOnly = true; 
            }

            txtWarehouseName.Text = _entity.WarehouseName;
            txtPlace.Text = _entity.Place;

            txtWarehouseId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWarehouseId.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã kho.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtWarehouseId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWarehouseName.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên kho.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtWarehouseName.Focus();
                return;
            }

            _entity.WarehouseId = txtWarehouseId.Text.Trim();
            _entity.WarehouseName = txtWarehouseName.Text.Trim();
            _entity.Place = txtPlace.Text?.Trim();

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
