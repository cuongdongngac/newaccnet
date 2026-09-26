using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using System.Security.Cryptography;
using System.Text;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class UserEditorWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        private SystemUserEntity _user;
        private bool _isNew;

        public UserEditorWindow(SystemUserEntity user, bool isNew)
        {
            InitializeComponent();
            _user = user;
            _isNew = isNew;

            if (_isNew)
            {
                Title = "Thêm Người dùng mới";
            }
            else
            {
                Title = "Sửa Người dùng";
                txtUsername.IsReadOnly = true;
            }

            LoadDataToUI();
        }

        private void LoadDataToUI()
        {
            txtUsername.Text = _user.Username;
            txtFullName.Text = _user.FullName;

            int mask = _user.RoleMask;
            chkRole0.IsChecked = (mask & 1) != 0;
            chkRole1.IsChecked = (mask & 2) != 0;
            chkRole2.IsChecked = (mask & 4) != 0;
            chkRole3.IsChecked = (mask & 8) != 0;
            chkRole4.IsChecked = (mask & 16) != 0;
            chkRole5.IsChecked = (mask & 32) != 0;
            chkRole6.IsChecked = (mask & 64) != 0;
            chkRole7.IsChecked = (mask & 128) != 0;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string userStr = txtUsername.Text?.Trim();
            if (string.IsNullOrEmpty(userStr))
            {
                MessageBox.Show("Tên đăng nhập không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string pwd = txtPassword.Password;
            string confirmPwd = txtConfirmPassword.Password;

            if (_isNew && string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu cho người dùng mới.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(pwd))
            {
                if (pwd != confirmPwd)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _user.Password = HashPassword(pwd);
            }

            _user.Username = userStr;
            _user.FullName = txtFullName.Text?.Trim();
            
            int newMask = 0;
            if (chkRole0.IsChecked == true) newMask |= 1;
            if (chkRole1.IsChecked == true) newMask |= 2;
            if (chkRole2.IsChecked == true) newMask |= 4;
            if (chkRole3.IsChecked == true) newMask |= 8;
            if (chkRole4.IsChecked == true) newMask |= 16;
            if (chkRole5.IsChecked == true) newMask |= 32;
            if (chkRole6.IsChecked == true) newMask |= 64;
            if (chkRole7.IsChecked == true) newMask |= 128;

            _user.RoleMask = newMask;

            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
