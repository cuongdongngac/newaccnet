using System;
using System.Windows;
using DataAccess.EntityClasses;
using DevExpress.Xpf.Core;
using NewaccNet.Core.Data;

namespace NewaccNet.Plugin.ErpWarehouse.Views.Dialogs
{
    public partial class QuickAddPartnerDialog : ThemedWindow
    {
        public PartnerEntity? CreatedPartner { get; private set; }

        public QuickAddPartnerDialog()
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
                MessageBox.Show("Vui lòng nhập Mã đối tượng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Vui lòng nhập Tên đối tượng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                var p = new PartnerEntity
                {
                    Id = id,
                    CustomerName = name,
                    Taxno = txtTaxNo.Text?.Trim() ?? "",
                    Address = txtAddress.Text?.Trim() ?? ""
                };

                using (var adapter = AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(p, true);
                }

                CreatedPartner = p;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu đối tượng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
