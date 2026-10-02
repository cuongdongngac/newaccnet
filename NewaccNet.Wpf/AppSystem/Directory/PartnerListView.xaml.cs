using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class PartnerListView : BaseDictionaryWindow
    {
        public PartnerListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu đối tượng...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var partners = new EntityCollection<PartnerEntity>();
                    adapter.FetchEntityCollection(partners, null);
                    gridControl.ItemsSource = partners.Count > 0 ? partners : null;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideLoading();
            }
        }

        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new PartnerEntity(), true);
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
                private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();

        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH SÁCH ĐỐI TƯỢNG CÔNG NỢ");
        }
        private void TableView_RowDoubleClick(object sender, DevExpress.Xpf.Grid.RowDoubleClickEventArgs e) => EditSelected();

        private void EditSelected()
        {
            var selectedPartner = gridControl.SelectedItem as PartnerEntity;
            if (selectedPartner == null) return;
            OpenEditor(selectedPartner, false);
        }

        private void DeleteSelected()
        {
            var selectedPartner = gridControl.SelectedItem as PartnerEntity;
            if (selectedPartner == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa đối tượng '{selectedPartner.Id} - {selectedPartner.CustomerName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedPartner);
                        var dataSource = gridControl.ItemsSource as EntityCollection<PartnerEntity>;
                        if (dataSource != null) dataSource.Remove(selectedPartner);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(PartnerEntity partner, bool isNew)
        {
            var editor = new PartnerEditorWindow(partner, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(partner, true, false);
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<PartnerEntity>;
                    if (dataSource != null)
                    {
                        if (isNew && !dataSource.Contains(partner)) dataSource.Add(partner);
                    }
                    else if (isNew)
                    {
                        LoadData(); // Reload if collection was null
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
