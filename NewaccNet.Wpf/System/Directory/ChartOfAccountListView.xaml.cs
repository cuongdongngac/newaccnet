using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class ChartOfAccountListView : BaseDictionaryWindow
    {
        public ChartOfAccountListView()
        {
            InitializeComponent();
            cboGridCategoryId.ItemsSource = AccountTypeMapping.GetAccountTypes();
            // Việc gọi LoadData() bây giờ sẽ do form cha (MainWindow) quyết định
        }

                private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var focusedAccount = treeListControl.SelectedItem as ChartOfAccountEntity;
            var newAccount = new ChartOfAccountEntity();
            
            if (focusedAccount != null)
            {
                newAccount.ParentId = focusedAccount.AccountId;
            }

            var editWin = new NewaccNet.Wpf.AppSystem.Directory.AccountEditorWindow(newAccount, false);
            editWin.Owner = this;
            
            if (editWin.ShowDialog() == true)
            {
                SaveAndRefresh(newAccount, true);
            }
        }

        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var focusedAccount = treeListControl.SelectedItem as ChartOfAccountEntity;
            if (focusedAccount == null) return;

            var editWin = new NewaccNet.Wpf.AppSystem.Directory.AccountEditorWindow(focusedAccount, true);
            editWin.Owner = this;
            
            if (editWin.ShowDialog() == true)
            {
                SaveAndRefresh(focusedAccount, false);
            }
        }

        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var focusedAccount = treeListControl.SelectedItem as ChartOfAccountEntity;
            if (focusedAccount == null) return;

            var result = System.Windows.MessageBox.Show($"Bạn có chắc muốn xóa tài khoản {focusedAccount.AccountId}?", "Xác nhận", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(focusedAccount);
                        
                        var dataSource = treeListControl.ItemsSource as EntityCollection<ChartOfAccountEntity>;
                        if (dataSource != null)
                        {
                            dataSource.Remove(focusedAccount);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void SaveAndRefresh(ChartOfAccountEntity account, bool isNew)
        {
            try
            {
                using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                {
                    adapter.SaveEntity(account, true, false);
                }

                var dataSource = treeListControl.ItemsSource as EntityCollection<ChartOfAccountEntity>;
                if (dataSource != null)
                {
                    if (isNew && !dataSource.Contains(account))
                    {
                        dataSource.Add(account);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

                private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var dataSource = treeListControl.ItemsSource as EntityCollection<ChartOfAccountEntity>;
            if (dataSource == null || dataSource.Count == 0)
            {
                System.Windows.MessageBox.Show("Không có dữ liệu để in.", "Thông báo");
                return;
            }

            DevExpress.XtraReports.UI.XtraReport report = NewaccNet.Wpf.AppSystem.Reports.ChartOfAccountReportFactory.CreateReport(dataSource);
            DevExpress.Xpf.Printing.PrintHelper.ShowPrintPreview(this, report);
        }

        public override void LoadData()
        {
            // Nếu chưa cấu hình, cảnh báo người dùng
            NewaccNet.Wpf.AppSystem.ConfigManager.BuildConnectionString();
            string connectionString = NewaccNet.Wpf.AppSystem.ConfigManager.Current.ConnectionString;
            
            if (string.IsNullOrWhiteSpace(connectionString) || (NewaccNet.Wpf.AppSystem.ConfigManager.Current.DbType == NewaccNet.Wpf.AppSystem.DatabaseType.Access && string.IsNullOrWhiteSpace(NewaccNet.Wpf.AppSystem.ConfigManager.Current.AccessFilePath)))
            {
                System.Windows.MessageBox.Show("Bạn chưa cấu hình cơ sở dữ liệu. Vui lòng bấm 'Cấu hình Kết nối' trước.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Hiện vòng quay tải dữ liệu
                ShowLoading("Đang tải dữ liệu từ cơ sở dữ liệu...");

                // Sử dụng Lớp trung gian AppDataAccessAdapter.
                // Lớp này tự động lấy chuỗi kết nối và xử lý Schema Routing ngầm bên dưới.
                using (var adapter = NewaccNet.Wpf.AppSystem.AppDataAccessAdapter.Create())
                {
                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);
                    
                    if (accounts.Count > 0)
                    {
                        treeListControl.ItemsSource = accounts;
                    }
                    else
                    {
                        treeListControl.ItemsSource = null;
                        System.Windows.MessageBox.Show("Không có dữ liệu trong bảng ChartOfAccounts.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                // Tắt vòng quay khi xong hoặc có lỗi
                HideLoading();
            }
        }
    }
}



