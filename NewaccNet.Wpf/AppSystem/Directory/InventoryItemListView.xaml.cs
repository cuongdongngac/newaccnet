using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.AppSystem;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public partial class InventoryItemListView : BaseDictionaryWindow
    {
        public InventoryItemListView()
        {
            InitializeComponent();
        }

        public override void LoadData()
        {
            try
            {
                ShowLoading("Đang tải dữ liệu danh mục vật tư...");
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // Tải danh mục Category để hiển thị tên thay vì mã trên lưới
                    var categories = new EntityCollection<CategoryEntity>();
                    adapter.FetchEntityCollection(categories, null);
                    cboCategory.ItemsSource = categories.ToList();

                    var items = new EntityCollection<InventoryItemEntity>();
                    adapter.FetchEntityCollection(items, null);
                    gridControl.ItemsSource = items.Count > 0 ? items : null;
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

        private void MenuExcelImport_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var win = new CategoryExcelImportWindow(CategoryImportType.Stock);
            win.Owner = this;
            win.ShowDialog();
            LoadData();
        }

        private async void MenuExtractFromHddt_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            var dbPath = ConfigManager.Current.AccessFilePath;
            if (string.IsNullOrEmpty(dbPath) || !System.IO.File.Exists(dbPath))
            {
                var template = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "template.accdb");
                if (System.IO.File.Exists(template)) dbPath = template;
            }

            List<TaxInvoiceDto> invoices = new();

            if (!string.IsNullOrEmpty(dbPath) && System.IO.File.Exists(dbPath))
            {
                try
                {
                    var dbService = new TaxInvoiceDbService(dbPath);
                    var dbInvoices = await dbService.GetInvoicesAsync();
                    if (dbInvoices.Count > 0)
                    {
                        var choice = MessageBox.Show(
                            $"CSDL hiện tại đang có {dbInvoices.Count} hóa đơn điện tử.\n\n" +
                            "- Chọn [Yes] để trích xuất vật tư từ các hóa đơn trong CSDL\n" +
                            "- Chọn [No] để chọn thư mục chứa tệp hóa đơn JSON khác\n" +
                            "- Chọn [Cancel] để hủy thao tác",
                            "Chọn nguồn hóa đơn điện tử",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question);

                        if (choice == MessageBoxResult.Cancel) return;

                        if (choice == MessageBoxResult.Yes)
                        {
                            await dbService.EnsureDetailsLoadedAsync(dbInvoices);
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

            var win = new TaxInvoice.InventoryItemExtractWindow(invoices, dbPath);
            win.Owner = this;
            win.ShowDialog();
            LoadData();
        }

        private void MenuAdd_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => OpenEditor(new InventoryItemEntity(), true);
        private void MenuEdit_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => EditSelected();
        private void MenuDelete_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => DeleteSelected();
        private void MenuRefresh_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => LoadData();
        private void MenuPrint_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) => PrintRecord();
        private void TableView_RowDoubleClick(object sender, RowDoubleClickEventArgs e) => EditSelected();

        public override void PrintRecord()
        {
            NewaccNet.Wpf.Reports.ReportManager.PrintGridControl(gridControl, "DANH MỤC VẬT TƯ HÀNG HÓA");
        }

        private void EditSelected()
        {
            var selectedItem = gridControl.SelectedItem as InventoryItemEntity;
            if (selectedItem == null) return;
            OpenEditor(selectedItem, false);
        }

        private void DeleteSelected()
        {
            var selectedItem = gridControl.SelectedItem as InventoryItemEntity;
            if (selectedItem == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn xóa vật tư '{selectedItem.MaterialId} - {selectedItem.MaterialName}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.DeleteEntity(selectedItem);
                        var dataSource = gridControl.ItemsSource as EntityCollection<InventoryItemEntity>;
                        if (dataSource != null) dataSource.Remove(selectedItem);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Lỗi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OpenEditor(InventoryItemEntity entity, bool isNew)
        {
            var editor = new InventoryItemEditorWindow(entity, isNew);
            editor.Owner = this;
            if (editor.ShowDialog() == true)
            {
                try
                {
                    using (var adapter = AppDataAccessAdapter.Create())
                    {
                        adapter.SaveEntity(entity, true, false);
                    }

                    var dataSource = gridControl.ItemsSource as EntityCollection<InventoryItemEntity>;
                    if (dataSource != null)
                    {
                        if (isNew && !dataSource.Contains(entity)) dataSource.Add(entity);
                    }
                    else if (isNew)
                    {
                        LoadData(); 
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
