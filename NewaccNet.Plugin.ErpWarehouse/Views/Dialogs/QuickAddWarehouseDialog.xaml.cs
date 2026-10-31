using System;
using System.Windows;
using DataAccess.EntityClasses;
using DevExpress.Xpf.Core;
using NewaccNet.Core.Data;

namespace NewaccNet.Plugin.ErpWarehouse.Views.Dialogs
{
    public partial class QuickAddWarehouseDialog : ThemedWindow
    {
        public WarehouseEntity? CreatedWarehouse { get; private set; }

        public QuickAddWarehouseDialog()
        {
            InitializeComponent();
            Loaded += (_, _) => txtId.Focus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string id = txtId.Text?.Trim() ?? "";
            string name = txtName.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(id))
            {
                MessageBox.Show("Vui lòng nhập Mã kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập Tên kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                var wh = new WarehouseEntity
                {
                    WarehouseId = id,
                    WarehouseName = name
                };

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(wh, true);
                }

                CreatedWarehouse = wh;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu kho hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
