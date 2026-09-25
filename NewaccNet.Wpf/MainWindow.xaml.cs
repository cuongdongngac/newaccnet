using System;
using System.Windows;
using DevExpress.Xpf.Core;

namespace NewaccNet.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ThemedWindow
{
    public MainWindow(string username, string fullName, int roleMask)
    {
        InitializeComponent();

        // Hiển thị thông tin user đăng nhập trên StatusBar
        txtStatusUsername.Text = username;
        txtStatusFullName.Text = fullName;
        // Hiển thị RoleMask dạng chuỗi bit (32-bit)
        txtStatusRoleMask.Text = $"{roleMask} → {Convert.ToString(roleMask, 2).PadLeft(32, '0')}";

        this.Closed += (_, _) => Application.Current.Shutdown();
    }

    private void BtnCategoryAccount_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        // Mở form danh mục tài khoản (dạng danh sách)
        var win = new NewaccNet.Wpf.AppSystem.Directory.ChartOfAccountListView();
        win.LoadData();
        win.Show();
    }
    private void BtnConfig_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var configWin = new NewaccNet.Wpf.AppSystem.DbConfigWindow();
        configWin.Owner = this;
        configWin.ShowDialog();
    }
    private void BtnUserManagement_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
    {
        var win = new NewaccNet.Wpf.AppSystem.Directory.UserListView();
        win.LoadData();
        win.Show();
    }
}
