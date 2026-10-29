using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.AppSystem.AI;
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

        private async void MenuExtractHddt_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var curPath = ConfigManager.Current.AccessFilePath;
            if (string.IsNullOrEmpty(curPath) || !System.IO.File.Exists(curPath))
            {
                var template = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "template.accdb");
                if (System.IO.File.Exists(template)) curPath = template;
            }

            List<TaxInvoiceDto> invoices = new();

            if (!string.IsNullOrEmpty(curPath) && System.IO.File.Exists(curPath))
            {
                try
                {
                    var dbService = new NewaccNet.TaxInvoice.TaxInvoiceDbService(curPath);
                    var dbInvoices = await dbService.GetInvoicesAsync();
                    if (dbInvoices.Count > 0)
                    {
                        var choice = MessageBox.Show(
                            $"CSDL hiện tại đang có {dbInvoices.Count} hóa đơn điện tử.\n\n" +
                            "- Chọn [Yes] để trích xuất đối tác từ các hóa đơn trong CSDL\n" +
                            "- Chọn [No] để chọn thư mục chứa tệp hóa đơn JSON khác\n" +
                            "- Chọn [Cancel] để hủy thao tác",
                            "Chọn nguồn hóa đơn điện tử",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question);

                        if (choice == MessageBoxResult.Cancel) return;

                        if (choice == MessageBoxResult.Yes)
                        {
                            invoices = dbInvoices;
                        }
                    }
                }
                catch { }
            }

            if (invoices.Count == 0)
            {
                var dialog = new Microsoft.Win32.OpenFolderDialog
                {
                    Title = "Chọn thư mục chứa tệp hóa đơn JSON"
                };

                if (dialog.ShowDialog() != true) return;

                var folder = dialog.FolderName;
                if (!System.IO.Directory.Exists(folder)) return;

                var files = System.IO.Directory.GetFiles(folder, "*.json", System.IO.SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var name = System.IO.Path.GetFileName(file);
                    if (name.Equals("index.json", System.StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("_log.json", System.StringComparison.OrdinalIgnoreCase) ||
                        name.StartsWith("list_", System.StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    try
                    {
                        var json = System.IO.File.ReadAllText(file);
                        var normPath = file.Replace('\\', '/').ToLowerInvariant();
                        var direction = normPath.Contains("/ban/") || normPath.Contains("sold") ? "sold" : "purchase";
                        var dto = TaxInvoiceParser.ParseFromJson(json, direction, file);
                        if (dto != null) invoices.Add(dto);
                    }
                    catch { }
                }

                if (invoices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn JSON hợp lệ nào trong thư mục đã chọn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var win = new TaxInvoice.PartnerExtractWindow(invoices, curPath);
            win.Owner = this;
            win.ShowDialog();
            LoadData();
        }

        private void MenuImportExcel_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Chọn tệp Excel Danh mục Đối tượng để mở trong Office Studio",
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                var officeWin = new OfficeStudioWindow();
                officeWin.Owner = this;
                officeWin.LoadSpreadsheetFile(dialog.FileName);
                officeWin.ShowDialog();
                LoadData();
            }
        }

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
