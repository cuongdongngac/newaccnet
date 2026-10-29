using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using Microsoft.Win32;
using MiniExcelLibs;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.AppSystem.AI;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public class ReviewInvoiceItemViewModel : INotifyPropertyChanged
    {
        private bool _isSelected = true;
        private string _dbStatus = "Chưa nạp";

        public TaxInvoiceDto Invoice { get; set; } = null!;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public string Direction => Invoice.Direction == "purchase" ? "Mua vào" : "Bán ra";
        public string InvoiceSeries => Invoice.InvoiceSeries;
        public long InvoiceNo => Invoice.InvoiceNo;
        public DateTime InvoiceDate => Invoice.InvoiceDate;
        public string SellerTaxCode => Invoice.SellerTaxCode;
        public string? SellerName => Invoice.SellerName;
        public string? BuyerTaxCode => Invoice.BuyerTaxCode;
        public string? BuyerName => Invoice.BuyerName;
        public decimal TotalAmountWithoutTax => Invoice.TotalAmountWithoutTax;
        public decimal TotalTaxAmount => Invoice.TotalTaxAmount;
        public decimal TotalAmount => Invoice.TotalAmount;

        public string StatusDescription => Invoice.StatusDescription;

        public string DbStatus
        {
            get => _dbStatus;
            set { _dbStatus = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class TaxInvoiceReviewWindow : BaseWindow
    {
        private readonly string? _databasePath;
        private readonly TaxInvoiceDbService? _dbService;

        // Tab 1 (JSON) in-memory cache and display items
        private readonly List<ReviewInvoiceItemViewModel> _rawItems = new List<ReviewInvoiceItemViewModel>();
        private readonly ObservableCollection<ReviewInvoiceItemViewModel> _items = new ObservableCollection<ReviewInvoiceItemViewModel>();

        // Tab 2 (DB) display items
        private readonly ObservableCollection<TaxInvoiceDto> _dbItems = new ObservableCollection<TaxInvoiceDto>();

        private bool _isLoaded;

        public TaxInvoiceReviewWindow(string? initialFolder = null, string? databasePath = null)
        {
            InitializeComponent();

            _databasePath = databasePath;
            if (string.IsNullOrEmpty(_databasePath))
            {
                var curPath = ConfigManager.Current.AccessFilePath;
                if (!string.IsNullOrEmpty(curPath) && System.IO.File.Exists(curPath))
                {
                    _databasePath = curPath;
                }
                else
                {
                    var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.accdb");
                    if (System.IO.File.Exists(templatePath)) _databasePath = templatePath;
                }
            }

            if (!string.IsNullOrEmpty(_databasePath) && System.IO.File.Exists(_databasePath))
            {
                _dbService = new TaxInvoiceDbService(_databasePath);
            }

            // Tab 1 setup
            if (!string.IsNullOrEmpty(initialFolder) && System.IO.Directory.Exists(initialFolder))
            {
                txtSourceFolder.Text = initialFolder;
            }
            else
            {
                txtSourceFolder.Text = string.Empty;
            }
            gridMaster.ItemsSource = _items;

            // Tab 2 setup
            var today = DateTime.Today;
            dtDbFrom.DateTime = new DateTime(today.Year, 1, 1);
            dtDbTo.DateTime = today;
            gridDbMaster.ItemsSource = _dbItems;

            lblWindowFooter.Text = $"CSDL: {_databasePath ?? "Chưa kết nối"}";

            this.Loaded += TaxInvoiceReviewWindow_Loaded;
        }

        private async void TaxInvoiceReviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;

            var folder = txtSourceFolder.Text.Trim();
            if (!string.IsNullOrEmpty(folder) && System.IO.Directory.Exists(folder))
            {
                await LoadJsonFilesAsync(folder);
            }
        }

        private async void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source != mainTabControl) return;

            if (mainTabControl.SelectedIndex == 1 && _dbItems.Count == 0)
            {
                await LoadDbInvoicesAsync();
            }
        }

        // ==============================================================
        // TAB 1: JSON REVIEW & IMPORT LOGIC
        // ==============================================================

        private async void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            var folder = txtSourceFolder.Text.Trim();
            if (System.IO.Directory.Exists(folder))
            {
                await LoadJsonFilesAsync(folder);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một thư mục hợp lệ chứa các tệp hóa đơn JSON.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Chọn thư mục chứa tệp hóa đơn JSON"
            };
            if (dialog.ShowDialog() == true)
            {
                txtSourceFolder.Text = dialog.FolderName;
                _ = LoadJsonFilesAsync(dialog.FolderName);
            }
        }

        private async Task LoadJsonFilesAsync(string folder)
        {
            _rawItems.Clear();
            _items.Clear();
            gridDetails.ItemsSource = null;
            gridTaxRates.ItemsSource = null;

            if (!System.IO.Directory.Exists(folder)) return;

            var files = System.IO.Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);
            var parsedList = new List<ReviewInvoiceItemViewModel>();

            await Task.Run(() =>
            {
                foreach (var file in files)
                {
                    var name = System.IO.Path.GetFileName(file);
                    if (name.Equals("index.json", StringComparison.OrdinalIgnoreCase) ||
                        name.Equals("_log.json", StringComparison.OrdinalIgnoreCase) ||
                        name.StartsWith("list_", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    try
                    {
                        var json = System.IO.File.ReadAllText(file);
                        // Check file path and content to determine direction correctly:
                        var normPath = file.Replace('\\', '/').ToLowerInvariant();
                        var direction = normPath.Contains("/ban/") || normPath.Contains("sold") ? "sold" : "purchase";
                        
                        var dto = TaxInvoiceParser.ParseFromJson(json, direction, file);
                        if (dto != null)
                        {
                            var vm = new ReviewInvoiceItemViewModel
                            {
                                Invoice = dto,
                                IsSelected = true
                            };
                            parsedList.Add(vm);
                        }
                    }
                    catch { }
                }
            });

            parsedList.Sort((a, b) => b.InvoiceDate.CompareTo(a.InvoiceDate));

            // Check against existing database
            if (_dbService != null)
            {
                try
                {
                    var existingInvoices = await _dbService.GetInvoicesAsync();
                    var existingKeys = new HashSet<string>(
                        existingInvoices.Select(x => $"{x.SellerTaxCode}_{x.InvoiceTemplate}_{x.InvoiceSeries}_{x.InvoiceNo}_{x.Direction}".ToUpperInvariant())
                    );

                    foreach (var item in parsedList)
                    {
                        var key = $"{item.SellerTaxCode}_{item.Invoice.InvoiceTemplate}_{item.InvoiceSeries}_{item.InvoiceNo}_{item.Invoice.Direction}".ToUpperInvariant();
                        if (existingKeys.Contains(key))
                        {
                            item.DbStatus = "Đã có trong DB";
                        }
                        else
                        {
                            item.DbStatus = "Chưa nạp";
                        }
                    }
                }
                catch { }
            }

            _rawItems.AddRange(parsedList);
            ApplyJsonFilter();
        }

        private void CboJsonDirection_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ApplyJsonFilter();
        }

        private void ApplyJsonFilter()
        {
            _items.Clear();
            gridDetails.ItemsSource = null;
            gridTaxRates.ItemsSource = null;

            int dirIndex = cboJsonDirection.SelectedIndex; // 0 = Tất cả, 1 = Mua vào, 2 = Bán ra
            foreach (var item in _rawItems)
            {
                if (dirIndex == 1 && item.Invoice.Direction != "purchase") continue;
                if (dirIndex == 2 && item.Invoice.Direction != "sold") continue;

                _items.Add(item);
            }

            UpdateStats();
        }

        private void GridMaster_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            if (e.NewItem is ReviewInvoiceItemViewModel vm)
            {
                gridDetails.ItemsSource = vm.Invoice.Details;
                gridTaxRates.ItemsSource = vm.Invoice.TaxRates;
            }
            else
            {
                gridDetails.ItemsSource = null;
                gridTaxRates.ItemsSource = null;
            }
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _items) item.IsSelected = true;
            UpdateStats();
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _items) item.IsSelected = false;
            UpdateStats();
        }

        private void UpdateStats()
        {
            var total = _items.Count;
            var selected = _items.Count(x => x.IsSelected);
            var totalAmount = _items.Where(x => x.IsSelected).Sum(x => x.TotalAmount);
            var totalTax = _items.Where(x => x.IsSelected).Sum(x => x.TotalTaxAmount);

            lblSelectedStats.Text = $"Đã chọn: {selected}/{total} hóa đơn";
            lblSummary.Text = $"Tổng số: {total} HĐ | Đã chọn: {selected} HĐ | Tổng tiền: {totalAmount:N0} đ (Thuế: {totalTax:N0} đ)";
        }

        private async void BtnImportToDb_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = _items.Where(x => x.IsSelected).ToList();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một hóa đơn để nhập vào CSDL.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_dbService == null)
            {
                MessageBox.Show("Không tìm thấy tệp CSDL Access để lưu dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn nạp {selectedItems.Count} hóa đơn vào CSDL:\n{_dbService.DatabasePath}?",
                "Xác nhận nhập dữ liệu", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            btnImportToDb.IsEnabled = false;
            int successCount = 0;

            try
            {
                foreach (var item in selectedItems)
                {
                    try
                    {
                        await _dbService.UpsertInvoiceAsync(item.Invoice);
                        item.DbStatus = "Đã có trong DB";
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi lưu hóa đơn {item.InvoiceNo}: {ex.Message}");
                    }
                }

                MessageBox.Show($"Đã nạp thành công {successCount}/{selectedItems.Count} hóa đơn vào CSDL!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Auto-refresh DB Tab if it has been loaded
                if (mainTabControl.SelectedIndex == 1 || _dbItems.Count > 0)
                {
                    await LoadDbInvoicesAsync();
                }
            }
            finally
            {
                btnImportToDb.IsEnabled = true;
                UpdateStats();
            }
        }

        private void BtnExtractPartners_Click(object sender, RoutedEventArgs e)
        {
            var invoices = _items.Where(x => x.IsSelected).Select(x => x.Invoice).ToList();
            if (invoices.Count == 0) invoices = _items.Select(x => x.Invoice).ToList();

            if (invoices.Count == 0)
            {
                MessageBox.Show("Chưa có hóa đơn nào để trích xuất đối tác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new PartnerExtractWindow(invoices, _databasePath);
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnExtractItems_Click(object sender, RoutedEventArgs e)
        {
            var invoices = _items.Where(x => x.IsSelected).Select(x => x.Invoice).ToList();
            if (invoices.Count == 0) invoices = _items.Select(x => x.Invoice).ToList();

            if (invoices.Count == 0)
            {
                MessageBox.Show("Chưa có hóa đơn nào để trích xuất vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new InventoryItemExtractWindow(invoices, _databasePath);
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnOpenDownloader_Click(object sender, RoutedEventArgs e)
        {
            var win = new TaxInvoiceDownloadWindow(_databasePath);
            win.Owner = this;
            win.ShowDialog();

            if (System.IO.Directory.Exists(txtSourceFolder.Text.Trim()))
            {
                _ = LoadJsonFilesAsync(txtSourceFolder.Text.Trim());
            }
        }

        // ==============================================================
        // TAB 2: DATABASE VIEW LOGIC
        // ==============================================================

        private async void FilterDbInvoices_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded) return;
            await LoadDbInvoicesAsync();
        }

        private async void TxtDbKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await LoadDbInvoicesAsync();
            }
        }

        private async void BtnDbSearch_Click(object sender, RoutedEventArgs e)
        {
            await LoadDbInvoicesAsync();
        }

        private async void BtnDbRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtDbKeyword.Text = string.Empty;
            cboDbDirection.SelectedIndex = 0;
            var today = DateTime.Today;
            dtDbFrom.DateTime = new DateTime(today.Year, 1, 1);
            dtDbTo.DateTime = today;

            await LoadDbInvoicesAsync();
        }

        private async Task LoadDbInvoicesAsync()
        {
            if (_dbService == null)
            {
                lblDbStats.Text = "Chưa kết nối CSDL.";
                return;
            }

            _dbItems.Clear();
            gridDbDetails.ItemsSource = null;
            gridDbTaxRates.ItemsSource = null;

            string? direction = null;
            if (cboDbDirection.SelectedIndex == 1) direction = "purchase";
            else if (cboDbDirection.SelectedIndex == 2) direction = "sold";

            DateTime? fromDate = dtDbFrom.DateTime > DateTime.MinValue ? dtDbFrom.DateTime : null;
            DateTime? toDate = dtDbTo.DateTime > DateTime.MinValue ? dtDbTo.DateTime : null;
            string? keyword = string.IsNullOrWhiteSpace(txtDbKeyword.Text) ? null : txtDbKeyword.Text.Trim();

            try
            {
                var list = await _dbService.GetInvoicesAsync(direction, fromDate, toDate, keyword);
                foreach (var inv in list)
                {
                    _dbItems.Add(inv);
                }

                var totalCount = _dbItems.Count;
                var totalAmount = _dbItems.Sum(x => x.TotalAmount);
                var totalTax = _dbItems.Sum(x => x.TotalTaxAmount);

                lblDbStats.Text = $"Tổng số: {totalCount} hóa đơn trong CSDL | Tiền hàng: {(totalAmount - totalTax):N0} đ | Thuế GTGT: {totalTax:N0} đ | Tổng thanh toán: {totalAmount:N0} đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi truy vấn dữ liệu từ CSDL:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GridDbMaster_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            if (e.NewItem is TaxInvoiceDto invoice && _dbService != null)
            {
                try
                {
                    await _dbService.LoadInvoiceDetailsAsync(invoice);
                    gridDbDetails.ItemsSource = invoice.Details;
                    gridDbTaxRates.ItemsSource = invoice.TaxRates;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi nạp chi tiết HĐ {invoice.Id}: {ex.Message}");
                }
            }
            else
            {
                gridDbDetails.ItemsSource = null;
                gridDbTaxRates.ItemsSource = null;
            }
        }

        private void BtnDbExtractPartners_Click(object sender, RoutedEventArgs e)
        {
            if (_dbItems.Count == 0)
            {
                MessageBox.Show("Chưa có hóa đơn nào trong CSDL để trích xuất đối tác.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new PartnerExtractWindow(_dbItems, _databasePath);
            win.Owner = this;
            win.ShowDialog();
        }

        private async void BtnDbExtractItems_Click(object sender, RoutedEventArgs e)
        {
            if (_dbItems.Count == 0)
            {
                MessageBox.Show("Chưa có hóa đơn nào trong CSDL để trích xuất vật tư.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_dbService != null)
            {
                try
                {
                    await _dbService.EnsureDetailsLoadedAsync(_dbItems);
                }
                catch { }
            }

            var win = new InventoryItemExtractWindow(_dbItems, _databasePath);
            win.Owner = this;
            win.ShowDialog();
        }

        private void BtnExportExcelJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_items.Count == 0)
                {
                    MessageBox.Show("Chưa có hóa đơn nào trên bảng để xuất Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                    FileName = $"BangKeHoaDon_JSON_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
                if (sfd.ShowDialog() == true)
                {
                    viewMaster.ExportToXlsx(sfd.FileName);
                    MessageBox.Show($"Đã xuất dữ liệu ra tệp Excel thành công:\n{sfd.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDbExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_dbItems.Count == 0)
                {
                    MessageBox.Show("Chưa có hóa đơn nào trong danh sách CSDL để xuất Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                    FileName = $"BangKeHoaDon_CSDL_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };
                if (sfd.ShowDialog() == true)
                {
                    viewDbMaster.ExportToXlsx(sfd.FileName);
                    MessageBox.Show($"Đã xuất dữ liệu ra tệp Excel thành công:\n{sfd.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOpenInOffice_Click(object sender, RoutedEventArgs e)
        {
            var selectedInvoices = _items.Where(x => x.IsSelected).Select(x => x.Invoice).ToList();
            if (selectedInvoices.Count == 0 && gridMaster.SelectedItem is ReviewInvoiceItemViewModel sel)
            {
                selectedInvoices = new List<TaxInvoiceDto> { sel.Invoice };
            }

            if (selectedInvoices.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hoặc tích chọn ít nhất một hóa đơn để mở trong Office Studio.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            OpenInOfficeStudio(selectedInvoices);
        }

        private async void BtnDbOpenInOffice_Click(object sender, RoutedEventArgs e)
        {
            var selectedInvoice = gridDbMaster.SelectedItem as TaxInvoiceDto;
            if (selectedInvoice == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng hóa đơn trong bảng CSDL để mở trong Office Studio.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_dbService != null)
            {
                try
                {
                    await _dbService.EnsureDetailsLoadedAsync(new List<TaxInvoiceDto> { selectedInvoice });
                }
                catch { }
            }

            OpenInOfficeStudio(new List<TaxInvoiceDto> { selectedInvoice });
        }

        private void OpenInOfficeStudio(List<TaxInvoiceDto> invoices)
        {
            try
            {
                var title = invoices.Count == 1 
                    ? $"HĐ {invoices[0].InvoiceSeries}-{invoices[0].InvoiceNo}" 
                    : $"Bảng kê {invoices.Count} Hóa đơn";

                var ms = CreateExcelStreamFromInvoices(invoices);
                var officeWin = new OfficeStudioWindow(ms, OfficeDocumentType.Spreadsheet, title, _databasePath);
                officeWin.Owner = this;
                officeWin.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi mở trong Office Studio: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static MemoryStream CreateExcelStreamFromInvoices(List<TaxInvoiceDto> invoices)
        {
            var rows = new List<Dictionary<string, object?>>();
            int stt = 1;

            foreach (var inv in invoices)
            {
                string partnerMst = inv.Direction == "purchase" ? inv.SellerTaxCode : (inv.BuyerTaxCode ?? "");
                string partnerName = inv.Direction == "purchase" ? (inv.SellerName ?? "") : (inv.BuyerName ?? "");

                if (inv.Details != null && inv.Details.Count > 0)
                {
                    foreach (var d in inv.Details)
                    {
                        var row = new Dictionary<string, object?>
                        {
                            ["STT"] = stt++,
                            ["Ký hiệu"] = inv.InvoiceSeries,
                            ["Số HĐ"] = inv.InvoiceNo,
                            ["Ngày lập"] = inv.InvoiceDate.ToString("dd/MM/yyyy"),
                            ["MST Đối tác"] = partnerMst,
                            ["Tên Đối tác"] = partnerName,
                            ["Tên mặt hàng"] = d.ItemName,
                            ["ĐVT"] = d.UnitName,
                            ["Số lượng"] = d.Quantity,
                            ["Đơn giá"] = (double)d.UnitPrice,
                            ["Thành tiền"] = (double)d.Amount,
                            ["Thuế %"] = d.TaxRate,
                            ["Tiền thuế"] = (double)d.TaxAmount,
                            ["TK Nợ"] = "156",
                            ["TK Có"] = "331"
                        };
                        rows.Add(row);
                    }
                }
                else
                {
                    var row = new Dictionary<string, object?>
                    {
                        ["STT"] = stt++,
                        ["Ký hiệu"] = inv.InvoiceSeries,
                        ["Số HĐ"] = inv.InvoiceNo,
                        ["Ngày lập"] = inv.InvoiceDate.ToString("dd/MM/yyyy"),
                        ["MST Đối tác"] = partnerMst,
                        ["Tên Đối tác"] = partnerName,
                        ["Tên mặt hàng"] = "Tổng tiền theo hóa đơn",
                        ["ĐVT"] = "",
                        ["Số lượng"] = 1,
                        ["Đơn giá"] = (double)inv.TotalAmountWithoutTax,
                        ["Thành tiền"] = (double)inv.TotalAmountWithoutTax,
                        ["Thuế %"] = "",
                        ["Tiền thuế"] = (double)inv.TotalTaxAmount,
                        ["TK Nợ"] = "156",
                        ["TK Có"] = "331"
                    };
                    rows.Add(row);
                }
            }

            var ms = new MemoryStream();
            MiniExcel.SaveAs(ms, rows);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}