using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using MiniExcelLibs;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.AppSystem;
using NewaccNet.Wpf.AppSystem.Directory;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public partial class InventoryItemExtractWindow : BaseWindow
    {
        private readonly List<TaxInvoiceDto> _invoices;
        private readonly string? _databasePath;
        private readonly ObservableCollection<ExtractedItemViewModel> _allItems = new();
        private readonly ObservableCollection<ExtractedItemViewModel> _displayedItems = new();

        private readonly Dictionary<string, (string MaterialId, string? CategoryId)> _dbItemsByName = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _dbItemsById = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<CategoryComboItem> _categories = new();

        public InventoryItemExtractWindow(IEnumerable<TaxInvoiceDto>? invoices, string? databasePath = null)
        {
            InitializeComponent();

            _invoices = invoices?.ToList() ?? new List<TaxInvoiceDto>();
            _databasePath = databasePath;
            if (string.IsNullOrEmpty(_databasePath))
            {
                var curPath = ConfigManager.Current.AccessFilePath;
                if (!string.IsNullOrEmpty(curPath) && File.Exists(curPath))
                {
                    _databasePath = curPath;
                }
                else
                {
                    var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.accdb");
                    if (File.Exists(templatePath)) _databasePath = templatePath;
                }
            }

            gridItems.ItemsSource = _displayedItems;
            lblFooter.Text = $"CSDL: {_databasePath ?? "Chưa kết nối"}";

            this.Loaded += InventoryItemExtractWindow_Loaded;
        }

        private async void InventoryItemExtractWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await AnalyzeAndLoadItemsAsync();
        }

        private async Task AnalyzeAndLoadItemsAsync()
        {
            _allItems.Clear();
            _displayedItems.Clear();
            lblStats.Text = "Đang phân tích hóa đơn và đối chiếu CSDL...";

            // 1. Load Categories and Existing InventoryItems from DB
            await Task.Run(() =>
            {
                LoadDbCategoriesAndItems();
            });

            cboCategories.ItemsSource = _categories;

            // 2. Ensure Details are loaded if invoices came from DB without details
            if (_invoices.Any(i => i.Details == null || i.Details.Count == 0) && !string.IsNullOrEmpty(_databasePath) && File.Exists(_databasePath))
            {
                try
                {
                    var dbService = new TaxInvoiceDbService(_databasePath);
                    await dbService.EnsureDetailsLoadedAsync(_invoices);
                }
                catch { }
            }

            // 3. Extract and group detail items from invoices
            var grouped = new Dictionary<string, ExtractedItemGroup>(StringComparer.OrdinalIgnoreCase);

            foreach (var inv in _invoices)
            {
                if (inv.Details == null || inv.Details.Count == 0) continue;

                foreach (var d in inv.Details)
                {
                    var rawName = d.ItemName?.Trim();
                    if (string.IsNullOrWhiteSpace(rawName)) continue;

                    var rawUnit = d.UnitName?.Trim() ?? "";
                    var groupKey = $"{NormalizeName(rawName)}|{rawUnit.ToLowerInvariant()}";

                    if (!grouped.TryGetValue(groupKey, out var grp))
                    {
                        grp = new ExtractedItemGroup
                        {
                            ItemName = rawName,
                            Unit = rawUnit,
                            OriginalItemCode = d.ItemCode?.Trim() ?? "",
                            SampleInvoice = $"HĐ {inv.InvoiceNo} ({inv.SellerName ?? inv.BuyerName})",
                            SampleUnitPrice = d.UnitPrice,
                            Count = 1
                        };
                        grouped[groupKey] = grp;
                    }
                    else
                    {
                        grp.Count++;
                        if (string.IsNullOrEmpty(grp.OriginalItemCode) && !string.IsNullOrEmpty(d.ItemCode))
                        {
                            grp.OriginalItemCode = d.ItemCode.Trim();
                        }
                    }
                }
            }

            // 4. Build ViewModels and match with DB
            var usedCodes = new HashSet<string>(_dbItemsById, StringComparer.OrdinalIgnoreCase);
            int ruleIndex = cboNamingRule.SelectedIndex;
            var prefix = txtPrefix.Text.Trim();
            int counter = 1;

            foreach (var kvp in grouped.OrderBy(x => x.Value.ItemName))
            {
                var grp = kvp.Value;
                var normName = NormalizeName(grp.ItemName);

                bool inDb = false;
                string assignedCode = "";
                string? assignedCat = null;

                if (_dbItemsByName.TryGetValue(normName, out var existing))
                {
                    inDb = true;
                    assignedCode = existing.MaterialId;
                    assignedCat = existing.CategoryId;
                }
                else
                {
                    // Generate new code based on rule
                    assignedCode = GenerateCodeByRule(grp.ItemName, grp.OriginalItemCode, ruleIndex, prefix, ref counter);
                    assignedCode = MaterialCodeGenerator.MakeUniqueCode(assignedCode, usedCodes);
                }

                var vm = new ExtractedItemViewModel
                {
                    IsSelected = !inDb, // Mặc định chỉ chọn các mặt hàng mới
                    MaterialId = assignedCode,
                    OriginalItemCode = grp.OriginalItemCode,
                    MaterialName = grp.ItemName,
                    Unit = grp.Unit,
                    CategoryId = assignedCat,
                    DbStatus = inDb ? "Đã có trong CSDL" : "Chưa có",
                    IsInDb = inDb,
                    OccurrenceCount = grp.Count,
                    SampleInvoice = grp.SampleInvoice,
                    SampleUnitPrice = grp.SampleUnitPrice
                };

                _allItems.Add(vm);
            }

            ApplyFilter();
        }

        private void LoadDbCategoriesAndItems()
        {
            _categories.Clear();
            _dbItemsByName.Clear();
            _dbItemsById.Clear();

            if (string.IsNullOrEmpty(_databasePath) || !File.Exists(_databasePath)) return;

            try
            {
                var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                using var conn = new OleDbConnection(connStr);
                conn.Open();

                // 1. Categories
                try
                {
                    using var cmdCat = new OleDbCommand("SELECT [Categoryid], [Categoryname] FROM [Category] ORDER BY [Categoryid]", conn);
                    using var readerCat = cmdCat.ExecuteReader();
                    while (readerCat.Read())
                    {
                        var cid = readerCat.IsDBNull(0) ? "" : readerCat.GetString(0).Trim();
                        var cname = readerCat.IsDBNull(1) ? "" : readerCat.GetString(1).Trim();
                        _categories.Add(new CategoryComboItem { Categoryid = cid, Categoryname = $"{cid} - {cname}" });
                    }
                }
                catch { }

                // 2. Existing InventoryItems
                try
                {
                    using var cmdMat = new OleDbCommand("SELECT [MaterialId], [MaterialName], [Categoryid] FROM [InventoryItem]", conn);
                    using var readerMat = cmdMat.ExecuteReader();
                    while (readerMat.Read())
                    {
                        var mid = readerMat.IsDBNull(0) ? "" : readerMat.GetString(0).Trim();
                        var mname = readerMat.IsDBNull(1) ? "" : readerMat.GetString(1).Trim();
                        var mcat = readerMat.IsDBNull(2) ? null : readerMat.GetString(2)?.Trim();

                        if (!string.IsNullOrEmpty(mid))
                        {
                            _dbItemsById.Add(mid);
                        }
                        if (!string.IsNullOrEmpty(mname))
                        {
                            var norm = NormalizeName(mname);
                            if (!_dbItemsByName.ContainsKey(norm))
                            {
                                _dbItemsByName[norm] = (mid, mcat);
                            }
                        }
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi đọc CSDL vật tư: {ex.Message}");
            }
        }

        private string GenerateCodeByRule(string itemName, string itemCode, int ruleIndex, string prefix, ref int counter)
        {
            switch (ruleIndex)
            {
                case 1: // Tiền tố + Số tự tăng
                    var codeNum = MaterialCodeGenerator.GeneratePrefixNumber(prefix, counter++);
                    return codeNum;

                case 2: // Ưu tiên mã trên HĐĐT
                    if (!string.IsNullOrWhiteSpace(itemCode))
                    {
                        var sanitized = MaterialCodeGenerator.SanitizeCode(itemCode);
                        if (!string.IsNullOrEmpty(sanitized)) return sanitized;
                    }
                    return MaterialCodeGenerator.GenerateAcronym(itemName);

                case 3: // Tên viết liền không dấu
                    return MaterialCodeGenerator.GenerateCondensed(itemName);

                case 0: // Viết tắt chữ cái đầu (Mặc định)
                default:
                    return MaterialCodeGenerator.GenerateAcronym(itemName);
            }
        }

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            var clean = Regex.Replace(name.Trim(), @"\s+", " ");
            return clean.ToLowerInvariant();
        }

        private void CboNamingRule_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (txtPrefix == null) return;
            // Cho phép sửa tiền tố nếu chọn quy tắc 2 (Tiền tố + Số)
            txtPrefix.IsEnabled = cboNamingRule.SelectedIndex == 1;
        }

        private void BtnApplyRule_Click(object sender, RoutedEventArgs e)
        {
            if (_allItems.Count == 0) return;

            int ruleIndex = cboNamingRule.SelectedIndex;
            var prefix = txtPrefix.Text.Trim();
            bool keepExisting = chkKeepExistingDbCodes.IsChecked == true;

            var usedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Reserve existing DB codes if keeping
            if (keepExisting)
            {
                foreach (var item in _allItems.Where(x => x.IsInDb))
                {
                    if (!string.IsNullOrEmpty(item.MaterialId))
                    {
                        usedCodes.Add(item.MaterialId);
                    }
                }
            }

            int counter = 1;
            foreach (var item in _allItems)
            {
                if (keepExisting && item.IsInDb) continue;

                var code = GenerateCodeByRule(item.MaterialName, item.OriginalItemCode, ruleIndex, prefix, ref counter);
                code = MaterialCodeGenerator.MakeUniqueCode(code, usedCodes);
                item.MaterialId = code;
            }

            MessageBox.Show("Đã áp dụng quy tắc sinh mã vật tư thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSelectNew_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _allItems)
            {
                item.IsSelected = !item.IsInDb;
            }
            UpdateStats();
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _displayedItems)
            {
                item.IsSelected = true;
            }
            UpdateStats();
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _displayedItems)
            {
                item.IsSelected = false;
            }
            UpdateStats();
        }

        private void CboFilterStatus_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            _displayedItems.Clear();
            int filterIndex = cboFilterStatus.SelectedIndex;

            foreach (var item in _allItems)
            {
                if (filterIndex == 1 && item.IsInDb) continue; // Chỉ mới
                if (filterIndex == 2 && !item.IsInDb) continue; // Chỉ đã có

                _displayedItems.Add(item);
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            var total = _allItems.Count;
            var selected = _allItems.Count(x => x.IsSelected);
            var newCount = _allItems.Count(x => !x.IsInDb);
            var existingCount = total - newCount;

            lblStats.Text = $"Tổng số: {total} mặt hàng (Mới: {newCount}, Đã có: {existingCount}) | Đã chọn: {selected} mặt hàng";
        }

        // ==============================================================
        // ACTION 1: EXPORT TO EXCEL
        // ==============================================================

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var selected = _allItems.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một vật tư để xuất ra tệp Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra trùng mã
            var dupCode = selected.GroupBy(x => x.MaterialId, StringComparer.OrdinalIgnoreCase)
                                  .FirstOrDefault(g => g.Count() > 1);
            if (dupCode != null)
            {
                MessageBox.Show($"Phát hiện mã vật tư trùng lặp: '{dupCode.Key}'. Vui lòng điều chỉnh lại mã trước khi xuất Excel.", "Cảnh báo trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"DanhMuc_VatTu_Tu_HDDT_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                Title = "Lưu danh mục vật tư ra tệp Excel"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var exportRows = selected.Select(x => new
                    {
                        MaterialId = x.MaterialId,
                        MaterialName = x.MaterialName,
                        Unit = x.Unit,
                        Categoryid = x.CategoryId ?? ""
                    }).ToList();

                    MiniExcel.SaveAs(dialog.FileName, exportRows, overwriteFile: true);

                    var result = MessageBox.Show(
                        $"Đã xuất thành công {selected.Count} vật tư ra tệp Excel:\n{dialog.FileName}\n\nBạn có muốn mở ngay chức năng 'Nhập từ Excel' của Danh mục vật tư để nạp vào CSDL không?",
                        "Xuất Excel thành công",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var importWin = new CategoryExcelImportWindow(CategoryImportType.Stock, _databasePath, dialog.FileName);
                        importWin.Owner = this.Owner ?? this;
                        importWin.ShowDialog();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xuất tệp Excel:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ==============================================================
        // ACTION 2: DIRECT SAVE TO DATABASE
        // ==============================================================

        private void BtnSaveToDb_Click(object sender, RoutedEventArgs e)
        {
            var selected = _allItems.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một vật tư để lưu vào CSDL.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_databasePath) || !File.Exists(_databasePath))
            {
                MessageBox.Show("Không tìm thấy đường dẫn CSDL Access hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn nạp/cập nhật {selected.Count} vật tư vào CSDL:\n{_databasePath}?",
                "Xác nhận lưu CSDL", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            btnSaveToDb.IsEnabled = false;
            int inserted = 0;
            int updated = 0;

            try
            {
                var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                using var conn = new OleDbConnection(connStr);
                conn.Open();

                const string checkSql = "SELECT [MaterialId] FROM [InventoryItem] WHERE [MaterialId] = ?";
                const string insertSql = "INSERT INTO [InventoryItem] ([MaterialId], [MaterialName], [Unit], [Categoryid]) VALUES (?, ?, ?, ?)";
                const string updateSql = "UPDATE [InventoryItem] SET [MaterialName] = ?, [Unit] = ?, [Categoryid] = ? WHERE [MaterialId] = ?";

                foreach (var item in selected)
                {
                    var id = item.MaterialId.Trim();
                    if (id.Length > 20) id = id.Substring(0, 20);

                    var name = item.MaterialName.Trim();
                    if (name.Length > 255) name = name.Substring(0, 255);

                    var unit = item.Unit.Trim();
                    if (unit.Length > 10) unit = unit.Substring(0, 10);

                    var cat = item.CategoryId?.Trim() ?? "";
                    if (cat.Length > 10) cat = cat.Substring(0, 10);

                    bool exists = false;
                    using (var cmd = new OleDbCommand(checkSql, conn))
                    {
                        cmd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = id;
                        exists = cmd.ExecuteScalar() != null;
                    }

                    if (!exists)
                    {
                        using var ins = new OleDbCommand(insertSql, conn);
                        ins.Parameters.Add("?", OleDbType.VarWChar, 20).Value = id;
                        ins.Parameters.Add("?", OleDbType.VarWChar, 255).Value = name;
                        ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(unit) ? (object)DBNull.Value : unit;
                        ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(cat) ? (object)DBNull.Value : cat;
                        ins.ExecuteNonQuery();
                        inserted++;
                    }
                    else
                    {
                        using var upd = new OleDbCommand(updateSql, conn);
                        upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = name;
                        upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(unit) ? (object)DBNull.Value : unit;
                        upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(cat) ? (object)DBNull.Value : cat;
                        upd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = id;
                        upd.ExecuteNonQuery();
                        updated++;
                    }

                    item.DbStatus = "Đã có trong CSDL";
                    item.IsInDb = true;
                }

                MessageBox.Show($"Đã lưu thành công vào CSDL:\n- Thêm mới: {inserted} vật tư\n- Cập nhật: {updated} vật tư", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ghi dữ liệu vào CSDL:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnSaveToDb.IsEnabled = true;
                UpdateStats();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private class ExtractedItemGroup
        {
            public string ItemName { get; set; } = string.Empty;
            public string Unit { get; set; } = string.Empty;
            public string OriginalItemCode { get; set; } = string.Empty;
            public string SampleInvoice { get; set; } = string.Empty;
            public decimal SampleUnitPrice { get; set; }
            public int Count { get; set; }
        }
    }

    public class CategoryComboItem
    {
        public string Categoryid { get; set; } = string.Empty;
        public string Categoryname { get; set; } = string.Empty;
    }

    public class ExtractedItemViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _materialId = string.Empty;
        private string _materialName = string.Empty;
        private string _unit = string.Empty;
        private string? _categoryId;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public string MaterialId
        {
            get => _materialId;
            set { if (_materialId != value) { _materialId = value; OnPropertyChanged(); } }
        }

        public string OriginalItemCode { get; set; } = string.Empty;

        public string MaterialName
        {
            get => _materialName;
            set { if (_materialName != value) { _materialName = value; OnPropertyChanged(); } }
        }

        public string Unit
        {
            get => _unit;
            set { if (_unit != value) { _unit = value; OnPropertyChanged(); } }
        }

        public string? CategoryId
        {
            get => _categoryId;
            set { if (_categoryId != value) { _categoryId = value; OnPropertyChanged(); } }
        }

        public string DbStatus { get; set; } = "Chưa có";
        public bool IsInDb { get; set; }
        public int OccurrenceCount { get; set; }
        public string SampleInvoice { get; set; } = string.Empty;
        public decimal SampleUnitPrice { get; set; }
    }
}
