using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class AccountEditorWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        public ChartOfAccountEntity Account { get; private set; }
        private bool _isEditMode;

        public AccountEditorWindow(ChartOfAccountEntity account, bool isEditMode)
        {
            InitializeComponent();
            cboCategoryId.ItemsSource = AccountTypeMapping.GetAccountTypes();
            Account = account;
            _isEditMode = isEditMode;

            // Load dữ liệu lên form
            txtAccountId.Text = Account.AccountId;
            txtAccountName.Text = Account.AccountName;
            cboCategoryId.EditValue = Account.CategoryId;
            txtParentId.Text = Account.ParentId;
            chkTaxFlag.IsChecked = Account.Taxflag;

            if (_isEditMode)
            {
                Title = "Sửa Tài Khoản: " + Account.AccountId;
                txtAccountId.IsReadOnly = true; // Không cho sửa khóa chính
            }
            else
            {
                Title = "Thêm Tài Khoản Mới";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccountId.Text) || string.IsNullOrWhiteSpace(txtAccountName.Text))
            {
                System.Windows.MessageBox.Show("Mã tài khoản và Tên tài khoản không được để trống!", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            Account.AccountId = txtAccountId.Text.Trim();
            Account.AccountName = txtAccountName.Text.Trim();
            
            Account.CategoryId = cboCategoryId.EditValue?.ToString();
            Account.ParentId = string.IsNullOrWhiteSpace(txtParentId.Text) ? null : txtParentId.Text.Trim();
                
            Account.Taxflag = chkTaxFlag.IsChecked ?? false;
            
            if (!_isEditMode)
            {
                // Set các giá trị mặc định tránh lỗi DB
                //Account.Dispflag = 0;
                //Account.Outtableflag = false;
                Account.Splite = false;
            }

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
