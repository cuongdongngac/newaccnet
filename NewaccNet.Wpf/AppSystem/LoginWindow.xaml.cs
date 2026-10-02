using System.Windows;
using System.Linq;
using DevExpress.Xpf.Core;
using SD.LLBLGen.Pro.ORMSupportClasses;
using DataAccess.EntityClasses;
using DataAccess.Linq;

namespace NewaccNet.Wpf.AppSystem
{
    public partial class LoginWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        /// <summary>Thông tin user sau khi đăng nhập thành công</summary>
        public string LoggedUsername { get; private set; } = "";
        public string LoggedFullName { get; private set; } = "";
        public int LoggedRoleMask { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text?.Trim() ?? "";
            string pass = "";
            if (txtPassword.EditValue != null) 
                pass = txtPassword.EditValue.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var metaData = new LinqMetaData(adapter);
                    // Tìm user theo username
                    var systemUser = metaData.SystemUser.FirstOrDefault(u => u.Username == user);

                    // So sánh mật khẩu đã mã hóa
                    if (systemUser != null && SecurityHelper.Verify(pass, systemUser.Password))
                    {
                        // Lưu thông tin user để truyền sang MainWindow
                        LoggedUsername = systemUser.Username;
                        LoggedFullName = systemUser.FullName ?? "";
                        LoggedRoleMask = systemUser.RoleMask;
                        
                        // Gán RoleMask toàn cục cho Attached Property phân quyền
                        AuthHelper.CurrentUserMask = systemUser.RoleMask;

                        this.DialogResult = true;
                        // this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}\n\nVui lòng kiểm tra lại cấu hình DB.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LnkConfig_Click(object sender, RoutedEventArgs e)
        {
            var cfg = new DbConfigWindow();
            cfg.ShowDialog();
        }
    }
}

