using System;
using System.Windows;
using DataAccess.EntityClasses;
using DevExpress.Xpf.Core;
using NewaccNet.Core.Data;

namespace NewaccNet.Plugin.ErpWarehouse.Views.Dialogs
{
    public partial class QuickAddMaterialDialog : ThemedWindow
    {
        public InventoryItemEntity? CreatedMaterial { get; private set; }

        public QuickAddMaterialDialog()
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
                MessageBox.Show("Vui lòng nhập Mã vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập Tên vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                var mat = new InventoryItemEntity
                {
                    MaterialId = id,
                    MaterialName = name,
                    Unit = txtUnit.Text?.Trim() ?? ""
                };

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(mat, true);
                }

                CreatedMaterial = mat;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vật tư: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
