using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class UserListView : BaseDictionaryWindow
    {
        public UserListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu người dùng...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var users = new EntityCollection<SystemUserEntity>();
                    adapter.FetchEntityCollection(users, null);
                    
                    if (users.Count > 0)
                        gridControl.ItemsSource = users;
                    else
                        gridControl.ItemsSource = null;
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => OpenEditor(new SystemUserEntity(), true);
        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new SystemUserEntity(), true);

        private void BtnEdit_Click(object sender, RoutedEventArgs e) => EditSelected();
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();

        private void BtnDelete_Click(object sender, RoutedEventArgs e) => DeleteSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();

        private void EditSelected()
        {
            var selectedUser = gridControl.SelectedItem as SystemUserEntity;
            if (selectedUser == null) return;
            OpenEditor(selectedUser, false);
        }

        private void DeleteSelected()
        {
            var selectedUser = gridControl.SelectedItem as SystemUserEntity;
            if (selectedUser == null) return;

            if (System.Windows.MessageBox.Show($"Bạn có chắc muốn xóa người dùng '{selectedUser.Username}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedUser);
                        var dataSource = gridControl.ItemsSource as EntityCollection<SystemUserEntity>;
                        if (dataSource != null) dataSource.Remove(selectedUser);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(SystemUserEntity user, bool isNew)
        {
            var editor = new UserEditorWindow(user, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(user, true, false);
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<SystemUserEntity>;
                    if (dataSource != null)
                    {
                        if (isNew && !dataSource.Contains(user)) dataSource.Add(user);
                    }
                    else if (isNew)
                    {
                        LoadData(); // Reload if collection was null
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
