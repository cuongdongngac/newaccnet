using NewaccNet.Wpf.AppSystem.Directory;
using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    public partial class VoucherListView : BaseDictionaryWindow
    {
        public VoucherListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải danh sách chứng từ...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var vouchers = new EntityCollection<JournalVoucherEntity>();
                    adapter.FetchEntityCollection(vouchers, null);
                    
                    gridControl.ItemsSource = vouchers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => LoadData();

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var frm = new VoucherEntryWindow(null);
            frm.ShowDialog();
            LoadData(); // Reload sau khi đóng form
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e) => EditSelected();

        private void GridControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var view = gridControl.View as DevExpress.Xpf.Grid.TableView;
            if (view != null && view.CalcHitInfo(e.OriginalSource as DependencyObject).InRowCell)
            {
                EditSelected();
            }
        }

        private void EditSelected()
        {
            var selected = gridControl.SelectedItem as JournalVoucherEntity;
            if (selected != null)
            {
                // Mở chứng từ với ID truyền vào để test Prefetch
                var frm = new VoucherEntryWindow(selected.Id);
                frm.ShowDialog();
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = gridControl.SelectedItem as JournalVoucherEntity;
            if (selected != null)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa chứng từ '{selected.VoucherNo}'?\n(Hệ thống sẽ tự động xóa các bút toán chi tiết liên quan)", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (var adapter = AppDataAccessAdapter.Create())
                        {
                            adapter.DeleteEntity(selected);
                        }
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
