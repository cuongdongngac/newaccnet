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
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.Directory
{
    public enum CategoryImportType
    {
        Partner,
        DebtType,
        Stock,
        CostObject,
        CostElement
    }

    public class CategoryPreviewRowViewModel : INotifyPropertyChanged
    {
        private bool _isSelected = true;
        private string _code = string.Empty;
        private string _name = string.Empty;
        private string? _field1;
        private string? _field2;
        private string? _field3;
        private string _status = "Hợp lệ";

        public int RowIndex { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string? Field1
        {
            get => _field1;
            set { _field1 = value; OnPropertyChanged(); }
        }

        public string? Field2
        {
            get => _field2;
            set { _field2 = value; OnPropertyChanged(); }
        }

        public string? Field3
        {
            get => _field3;
            set { _field3 = value; OnPropertyChanged(); }
        }

        public string? Field4 { get; set; }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public bool IsValid { get; set; } = true;
        public bool IsExisting { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class CategoryExcelImportWindow : BaseWindow
    {
        private readonly string? _databasePath;
        private readonly ObservableCollection<CategoryPreviewRowViewModel> _previewRows = new ObservableCollection<CategoryPreviewRowViewModel>();

        private List<Dictionary<string, object?>> _excelRawRows = new List<Dictionary<string, object?>>();
        private List<string> _excelColumns = new List<string>();
        private bool _isUpdatingMapping;

        public CategoryExcelImportWindow(CategoryImportType initialType = CategoryImportType.Partner, string? databasePath = null, string? initialExcelPath = null)
        {
            InitializeComponent();

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

            gridPreview.ItemsSource = _previewRows;
            lblFooter.Text = $"CSDL: {_databasePath ?? "Chưa kết nối"}";

            // Set initial category
            int catIndex = initialType switch
            {
                CategoryImportType.Partner => 0,
                CategoryImportType.DebtType => 1,
                CategoryImportType.Stock => 2,
                CategoryImportType.CostObject => 3,
                CategoryImportType.CostElement => 4,
                _ => 0
            };
            cboCategoryType.SelectedIndex = catIndex;
            UpdateCategorySchemaUI();

            if (!string.IsNullOrEmpty(initialExcelPath) && File.Exists(initialExcelPath))
            {
                txtExcelPath.Text = initialExcelPath;
                this.Loaded += (s, e) => _ = ReadExcelFileAsync(initialExcelPath);
            }
        }

        private CategoryImportType CurrentCategory
        {
            get
            {
                return cboCategoryType.SelectedIndex switch
                {
                    0 => CategoryImportType.Partner,
                    1 => CategoryImportType.DebtType,
                    2 => CategoryImportType.Stock,
                    3 => CategoryImportType.CostObject,
                    4 => CategoryImportType.CostElement,
                    _ => CategoryImportType.Partner
                };
            }
        }

        private void CboCategoryType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            UpdateCategorySchemaUI();
            if (_excelRawRows.Count > 0)
            {
                AutoMatchColumns();
                BuildPreview();
            }
        }

        private void UpdateCategorySchemaUI()
        {
            switch (CurrentCategory)
            {
                case CategoryImportType.Partner:
                    lblTarget1.Text = "* Mã đối tác (Id = MST):";
                    lblTarget2.Text = "* Tên đối tác (Tên hiển thị):";
                    lblTarget3.Text = "Mã số thuế (Taxno):";
                    lblTarget4.Text = "Địa chỉ (Address):";
                    lblTarget5.Text = "Điện thoại (Phonenumber):";
                    lblTarget6.Text = "Ghi chú (Note):";

                    colCode.Header = "Mã Đối tác (MST)";
                    colName.Header = "Tên Đối tác";
                    colField1.Header = "Mã số thuế";
                    colField2.Header = "Địa chỉ";
                    colField3.Header = "Điện thoại";
                    break;

                case CategoryImportType.DebtType:
                    lblTarget1.Text = "* Mã loại công nợ (Id):";
                    lblTarget2.Text = "* Tên loại công nợ (TypeName):";
                    lblTarget3.Text = "Cờ đồng bộ (Syncronizeflag):";
                    lblTarget4.Text = "(Không dùng):";
                    lblTarget5.Text = "(Không dùng):";
                    lblTarget6.Text = "(Không dùng):";

                    colCode.Header = "Mã loại công nợ";
                    colName.Header = "Tên loại công nợ";
                    colField1.Header = "Đồng bộ";
                    colField2.Header = "Trường phụ 2";
                    colField3.Header = "Trường phụ 3";
                    break;

                case CategoryImportType.Stock:
                    lblTarget1.Text = "* Mã vật tư (MaterialId):";
                    lblTarget2.Text = "* Tên vật tư (MaterialName):";
                    lblTarget3.Text = "Đơn vị tính (Unit):";
                    lblTarget4.Text = "Nhóm vật tư (Categoryid):";
                    lblTarget5.Text = "(Không dùng):";
                    lblTarget6.Text = "(Không dùng):";

                    colCode.Header = "Mã vật tư";
                    colName.Header = "Tên vật tư, hàng hóa";
                    colField1.Header = "Đơn vị tính";
                    colField2.Header = "Mã nhóm VT";
                    colField3.Header = "Trường phụ 3";
                    break;

                case CategoryImportType.CostObject:
                    lblTarget1.Text = "* Mã đối tượng CP (Id):";
                    lblTarget2.Text = "* Tên đối tượng CP (ObjectName):";
                    lblTarget3.Text = "Cờ đồng bộ (Syncronizeflag):";
                    lblTarget4.Text = "(Không dùng):";
                    lblTarget5.Text = "(Không dùng):";
                    lblTarget6.Text = "(Không dùng):";

                    colCode.Header = "Mã đối tượng CP";
                    colName.Header = "Tên đối tượng CP (Công trình/Sản phẩm)";
                    colField1.Header = "Đồng bộ";
                    colField2.Header = "Trường phụ 2";
                    colField3.Header = "Trường phụ 3";
                    break;

                case CategoryImportType.CostElement:
                    lblTarget1.Text = "* Mã yếu tố CP (Id):";
                    lblTarget2.Text = "* Tên yếu tố CP (ElementName):";
                    lblTarget3.Text = "Cờ tính thuế (Taxflag):";
                    lblTarget4.Text = "(Không dùng):";
                    lblTarget5.Text = "(Không dùng):";
                    lblTarget6.Text = "(Không dùng):";

                    colCode.Header = "Mã yếu tố CP";
                    colName.Header = "Tên yếu tố CP (Khoản mục chi phí)";
                    colField1.Header = "Tính thuế";
                    colField2.Header = "Trường phụ 2";
                    colField3.Header = "Trường phụ 3";
                    break;
            }
        }

        private void BtnBrowseExcel_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*",
                Title = "Chọn tệp Excel dữ liệu"
            };

            if (dialog.ShowDialog() == true)
            {
                txtExcelPath.Text = dialog.FileName;
                _ = ReadExcelFileAsync(dialog.FileName);
            }
        }

        private void BtnReadExcel_Click(object sender, RoutedEventArgs e)
        {
            var path = txtExcelPath.Text.Trim();
            if (File.Exists(path))
            {
                _ = ReadExcelFileAsync(path);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tệp Excel hợp lệ trước.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task ReadExcelFileAsync(string filePath)
        {
            try
            {
                lblImportStats.Text = "Đang đọc tệp Excel...";
                _previewRows.Clear();
                _excelRawRows.Clear();
                _excelColumns.Clear();

                await Task.Run(() =>
                {
                    var rows = MiniExcel.Query(filePath, useHeaderRow: true).ToList();
                    foreach (IDictionary<string, object?> row in rows)
                    {
                        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kvp in row)
                        {
                            dict[kvp.Key] = kvp.Value;
                        }
                        _excelRawRows.Add(dict);
                    }

                    if (_excelRawRows.Count > 0)
                    {
                        _excelColumns = _excelRawRows[0].Keys.ToList();
                    }
                });

                if (_excelColumns.Count == 0)
                {
                    lblImportStats.Text = "Tệp Excel rỗng hoặc không có cột tiêu đề.";
                    return;
                }

                // Populate Dropdowns
                PopulateMappingDropdowns();
                AutoMatchColumns();
                BuildPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc tệp Excel:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                lblImportStats.Text = "Lỗi đọc tệp Excel.";
            }
        }

        private void PopulateMappingDropdowns()
        {
            _isUpdatingMapping = true;

            var combos = new[] { cboMap1, cboMap2, cboMap3, cboMap4, cboMap5, cboMap6 };
            foreach (var cbo in combos)
            {
                cbo.Items.Clear();
                cbo.Items.Add("-- Không chọn --");
                foreach (var col in _excelColumns)
                {
                    cbo.Items.Add(col);
                }
                cbo.SelectedIndex = 0;
            }

            _isUpdatingMapping = false;
        }

        private void AutoMatchColumns()
        {
            if (_excelColumns.Count == 0) return;
            _isUpdatingMapping = true;

            switch (CurrentCategory)
            {
                case CategoryImportType.Partner:
                    MatchCombo(cboMap1, "^id$|^partnerid$|mã|mst|code|số hiệu");
                    MatchCombo(cboMap2, "^customername$|tên|name|khách hàng|nhà cung cấp|đối tác");
                    MatchCombo(cboMap3, "^taxno$|mst|tax|thuế|mã số thuế");
                    MatchCombo(cboMap4, "^address$|địa chỉ|dchi");
                    MatchCombo(cboMap5, "^phonenumber$|điện thoại|phone|tel|sđt");
                    MatchCombo(cboMap6, "^note$|ghi chú|diễn giải");
                    break;

                case CategoryImportType.DebtType:
                    MatchCombo(cboMap1, "^id$|mã|code");
                    MatchCombo(cboMap2, "^typename$|tên|name|loại công nợ|nội dung");
                    MatchCombo(cboMap3, "^syncronizeflag$|đồng bộ|sync");
                    break;

                case CategoryImportType.Stock:
                    MatchCombo(cboMap1, "^materialid$|mã|code|stockid|vật tư|hàng hóa");
                    MatchCombo(cboMap2, "^materialname$|tên|name|vật tư|hàng hóa|diễn giải|description");
                    MatchCombo(cboMap3, "^unit$|đvt|đơn vị");
                    MatchCombo(cboMap4, "^categoryid$|^category$|nhóm|loại");
                    break;

                case CategoryImportType.CostObject:
                    MatchCombo(cboMap1, "mã|code|id|đối tượng");
                    MatchCombo(cboMap2, "tên|objectname|name|công trình|hạng mục|sản phẩm|đối tượng");
                    MatchCombo(cboMap3, "đồng bộ|sync");
                    break;

                case CategoryImportType.CostElement:
                    MatchCombo(cboMap1, "mã|code|id|yếu tố");
                    MatchCombo(cboMap2, "tên|elementname|name|khoản mục|chi phí|yếu tố");
                    MatchCombo(cboMap3, "thuế|taxflag");
                    break;
            }

            _isUpdatingMapping = false;
        }

        private void MatchCombo(DevExpress.Xpf.Editors.ComboBoxEdit cbo, string patternRegex)
        {
            for (int i = 1; i < cbo.Items.Count; i++)
            {
                var colName = cbo.Items[i]?.ToString() ?? "";
                if (Regex.IsMatch(colName, patternRegex, RegexOptions.IgnoreCase))
                {
                    cbo.SelectedIndex = i;
                    return;
                }
            }
            cbo.SelectedIndex = 0;
        }

        private void CboMapping_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingMapping) return;
            BuildPreview();
        }

        private void BuildPreview()
        {
            _previewRows.Clear();
            if (_excelRawRows.Count == 0) return;

            string col1 = GetSelectedColName(cboMap1);
            string col2 = GetSelectedColName(cboMap2);
            string col3 = GetSelectedColName(cboMap3);
            string col4 = GetSelectedColName(cboMap4);
            string col5 = GetSelectedColName(cboMap5);
            string col6 = GetSelectedColName(cboMap6);

            // Get existing IDs from Access DB
            var existingIds = LoadExistingIdsFromDb();

            int rowIdx = 1;
            int validCount = 0;
            int existingCount = 0;

            foreach (var raw in _excelRawRows)
            {
                rowIdx++;
                string code = GetVal(raw, col1);
                string name = GetVal(raw, col2);
                string f1 = GetVal(raw, col3);
                string f2 = GetVal(raw, col4);
                string f3 = GetVal(raw, col5);
                string f4 = GetVal(raw, col6);

                // If Partner and Code is empty but f1 (Taxno) has value, use Taxno as Code!
                if (CurrentCategory == CategoryImportType.Partner && string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(f1))
                {
                    code = f1;
                }

                // Skip completely empty rows
                if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var vm = new CategoryPreviewRowViewModel
                {
                    RowIndex = rowIdx,
                    Code = code,
                    Name = name,
                    Field1 = f1,
                    Field2 = f2,
                    Field3 = f3,
                    Field4 = f4
                };

                if (string.IsNullOrWhiteSpace(code))
                {
                    vm.Status = "Lỗi: Thiếu Mã (Id)";
                    vm.IsValid = false;
                    vm.IsSelected = false;
                }
                else if (existingIds.Contains(code))
                {
                    vm.IsExisting = true;
                    vm.Status = chkUpdateExisting.IsChecked == true ? "Đã có (Sẽ cập nhật)" : "Đã có (Bỏ qua)";
                    vm.IsSelected = chkUpdateExisting.IsChecked == true;
                    existingCount++;
                    validCount++;
                }
                else
                {
                    vm.IsExisting = false;
                    vm.Status = "Hợp lệ (Mới)";
                    vm.IsSelected = true;
                    validCount++;
                }

                _previewRows.Add(vm);
            }

            lblImportStats.Text = $"Tổng số: {_previewRows.Count} dòng | Hợp lệ: {validCount} ({existingCount} trùng mã DB) | Đã chọn: {_previewRows.Count(x => x.IsSelected)}";
        }

        private string GetSelectedColName(DevExpress.Xpf.Editors.ComboBoxEdit cbo)
        {
            if (cbo.SelectedIndex > 0 && cbo.SelectedItem != null)
            {
                return cbo.SelectedItem.ToString() ?? "";
            }
            return "";
        }

        private static string GetVal(Dictionary<string, object?> row, string colName)
        {
            if (string.IsNullOrEmpty(colName)) return "";
            if (row.TryGetValue(colName, out var val) && val != null)
            {
                return val.ToString()?.Trim() ?? "";
            }
            return "";
        }

        private HashSet<string> LoadExistingIdsFromDb()
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(_databasePath) || !File.Exists(_databasePath)) return ids;

            try
            {
                var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                using var conn = new OleDbConnection(connStr);
                conn.Open();

                string tableName = CurrentCategory switch
                {
                    CategoryImportType.Partner => "Partner",
                    CategoryImportType.DebtType => "DebtType",
                    CategoryImportType.Stock => "InventoryItem",
                    CategoryImportType.CostObject => "CostObject",
                    CategoryImportType.CostElement => "CostElement",
                    _ => "Partner"
                };

                string idColumn = CurrentCategory switch
                {
                    CategoryImportType.Stock => "MaterialId",
                    _ => "Id"
                };

                using var cmd = new OleDbCommand($"SELECT [{idColumn}] FROM [{tableName}]", conn);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    if (!r.IsDBNull(0))
                    {
                        ids.Add(r.GetValue(0)?.ToString()?.Trim() ?? "");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Load existing IDs error: " + ex.Message);
            }
            return ids;
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var r in _previewRows) if (r.IsValid) r.IsSelected = true;
            UpdateSelectedStats();
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var r in _previewRows) r.IsSelected = false;
            UpdateSelectedStats();
        }

        private void UpdateSelectedStats()
        {
            lblImportStats.Text = $"Tổng số: {_previewRows.Count} dòng | Đã chọn: {_previewRows.Count(x => x.IsSelected)} dòng nạp vào CSDL.";
        }

        private async void BtnExecuteImport_Click(object sender, RoutedEventArgs e)
        {
            var selectedRows = _previewRows.Where(x => x.IsSelected && x.IsValid).ToList();
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một dòng hợp lệ để nạp vào CSDL.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_databasePath) || !File.Exists(_databasePath))
            {
                MessageBox.Show("Không tìm thấy CSDL Access.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn nạp {selectedRows.Count} dòng dữ liệu vào danh mục {CurrentCategory}?",
                "Xác nhận nạp dữ liệu", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            btnExecuteImport.IsEnabled = false;
            int inserted = 0;
            int updated = 0;

            try
            {
                await Task.Run(() =>
                {
                    var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                    using var conn = new OleDbConnection(connStr);
                    conn.Open();

                    switch (CurrentCategory)
                    {
                        case CategoryImportType.Partner:
                            ExecutePartnerImport(conn, selectedRows, out inserted, out updated);
                            break;
                        case CategoryImportType.DebtType:
                            ExecuteDebtTypeImport(conn, selectedRows, out inserted, out updated);
                            break;
                        case CategoryImportType.Stock:
                            ExecuteStockImport(conn, selectedRows, out inserted, out updated);
                            break;
                        case CategoryImportType.CostObject:
                            ExecuteCostObjectImport(conn, selectedRows, out inserted, out updated);
                            break;
                        case CategoryImportType.CostElement:
                            ExecuteCostElementImport(conn, selectedRows, out inserted, out updated);
                            break;
                    }
                });

                MessageBox.Show($"Nạp dữ liệu thành công!\n- Thêm mới: {inserted}\n- Cập nhật: {updated}", "Hoàn thành", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh preview
                BuildPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi nạp CSDL:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnExecuteImport.IsEnabled = true;
            }
        }

        private void ExecutePartnerImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT [Id] FROM [Partner] WHERE [Id] = ?";
            const string insertSql = "INSERT INTO [Partner] ([Id], [CustomerName], [Taxno], [Address], [Phonenumber], [Faxnumber], [Note], [Isemployee]) VALUES (?, ?, ?, ?, ?, '', ?, false)";
            const string updateSql = "UPDATE [Partner] SET [CustomerName] = ?, [Taxno] = ?, [Address] = ?, [Phonenumber] = ?, [Note] = ? WHERE [Id] = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 20 ? r.Code.Substring(0, 20) : r.Code;
                var rawName = r.Name?.Trim() ?? "";
                var shortName = rawName.Length > 50 ? rawName.Substring(0, 50) : rawName;
                var taxno = (r.Field1?.Trim() ?? "");
                if (string.IsNullOrEmpty(taxno)) taxno = id; // MST default to Id
                if (taxno.Length > 50) taxno = taxno.Substring(0, 50);
                var address = (r.Field2?.Trim() ?? "");
                if (address.Length > 255) address = address.Substring(0, 255);
                var phone = (r.Field3?.Trim() ?? "");
                if (phone.Length > 30) phone = phone.Substring(0, 30);
                var note = rawName.Length > 50 ? $"Tên đầy đủ: {rawName}" : "";

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
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = shortName;
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = taxno;
                    ins.Parameters.Add("?", OleDbType.VarWChar, 255).Value = address;
                    ins.Parameters.Add("?", OleDbType.VarWChar, 30).Value = phone;
                    ins.Parameters.Add("?", OleDbType.LongVarWChar).Value = note;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = shortName;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = taxno;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = address;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 30).Value = phone;
                    upd.Parameters.Add("?", OleDbType.LongVarWChar).Value = note;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void ExecuteDebtTypeImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT [Id] FROM [DebtType] WHERE [Id] = ?";
            const string insertSql = "INSERT INTO [DebtType] ([Id], [TypeName], [Syncronizeflag]) VALUES (?, ?, false)";
            const string updateSql = "UPDATE [DebtType] SET [TypeName] = ? WHERE [Id] = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 10 ? r.Code.Substring(0, 10) : r.Code;
                var name = r.Name.Length > 50 ? r.Name.Substring(0, 50) : r.Name;

                bool exists = false;
                using (var cmd = new OleDbCommand(checkSql, conn))
                {
                    cmd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    exists = cmd.ExecuteScalar() != null;
                }

                if (!exists)
                {
                    using var ins = new OleDbCommand(insertSql, conn);
                    ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void ExecuteStockImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT [MaterialId] FROM [InventoryItem] WHERE [MaterialId] = ?";
            const string insertSql = "INSERT INTO [InventoryItem] ([MaterialId], [MaterialName], [Unit], [Categoryid]) VALUES (?, ?, ?, ?)";
            const string updateSql = "UPDATE [InventoryItem] SET [MaterialName] = ?, [Unit] = ?, [Categoryid] = ? WHERE [MaterialId] = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 20 ? r.Code.Substring(0, 20) : r.Code;
                var name = r.Name.Length > 255 ? r.Name.Substring(0, 255) : r.Name;
                var unit = (r.Field1?.Trim() ?? "");
                if (unit.Length > 10) unit = unit.Substring(0, 10);
                var catId = (r.Field2?.Trim() ?? "");
                if (catId.Length > 10) catId = catId.Substring(0, 10);

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
                    ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(catId) ? (object)DBNull.Value : catId;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = name;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(unit) ? (object)DBNull.Value : unit;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = string.IsNullOrEmpty(catId) ? (object)DBNull.Value : catId;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void ExecuteCostObjectImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT [Id] FROM [CostObject] WHERE [Id] = ?";
            const string insertSql = "INSERT INTO [CostObject] ([Id], [ObjectName], [Syncronizeflag]) VALUES (?, ?, false)";
            const string updateSql = "UPDATE [CostObject] SET [ObjectName] = ? WHERE [Id] = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 10 ? r.Code.Substring(0, 10) : r.Code;
                var name = r.Name.Length > 50 ? r.Name.Substring(0, 50) : r.Name;

                bool exists = false;
                using (var cmd = new OleDbCommand(checkSql, conn))
                {
                    cmd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    exists = cmd.ExecuteScalar() != null;
                }

                if (!exists)
                {
                    using var ins = new OleDbCommand(insertSql, conn);
                    ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void ExecuteCostElementImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT [Id] FROM [CostElement] WHERE [Id] = ?";
            const string insertSql = "INSERT INTO [CostElement] ([Id], [ElementName], [Taxflag]) VALUES (?, ?, false)";
            const string updateSql = "UPDATE [CostElement] SET [ElementName] = ? WHERE [Id] = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 10 ? r.Code.Substring(0, 10) : r.Code;
                var name = r.Name.Length > 50 ? r.Name.Substring(0, 50) : r.Name;

                bool exists = false;
                using (var cmd = new OleDbCommand(checkSql, conn))
                {
                    cmd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    exists = cmd.ExecuteScalar() != null;
                }

                if (!exists)
                {
                    using var ins = new OleDbCommand(insertSql, conn);
                    ins.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void BtnDownloadTemplate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"Mau_Nhap_{CurrentCategory}.xlsx",
                Title = "Lưu tệp mẫu Excel"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    switch (CurrentCategory)
                    {
                        case CategoryImportType.Partner:
                            var samplePartners = new[]
                            {
                                new { Id = "0101234567", CustomerName = "Công ty Cổ phần Mẫu A", Taxno = "0101234567", Address = "Hà Nội", Phonenumber = "0243123456", Note = "Nhà cung cấp" },
                                new { Id = "0209876543", CustomerName = "Công ty TNHH Mẫu B", Taxno = "0209876543", Address = "Hải Phòng", Phonenumber = "0225123456", Note = "Khách hàng" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, samplePartners, overwriteFile: true);
                            break;

                        case CategoryImportType.DebtType:
                            var sampleDebtTypes = new[]
                            {
                                new { Id = "LCN01", TypeName = "Công nợ khách hàng mua buôn", Syncronizeflag = "false" },
                                new { Id = "LCN02", TypeName = "Công nợ nhà cung cấp dịch vụ", Syncronizeflag = "false" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleDebtTypes, overwriteFile: true);
                            break;

                        case CategoryImportType.Stock:
                            var sampleStocks = new[]
                            {
                                new { MaterialId = "VT001", MaterialName = "Thép cuộn phi 6", Unit = "Kg", Categoryid = "NVL" },
                                new { MaterialId = "VT002", MaterialName = "Xi măng PCB40", Unit = "Tấn", Categoryid = "NVL" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleStocks, overwriteFile: true);
                            break;

                        case CategoryImportType.CostObject:
                            var sampleCostObjects = new[]
                            {
                                new { Ma_Doi_Tuong_CP = "DTCP01", Ten_Doi_Tuong_CP = "Công trình Nhà máy Xi măng" },
                                new { Ma_Doi_Tuong_CP = "DTCP02", Ten_Doi_Tuong_CP = "Sản phẩm A - Lô 2026" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleCostObjects, overwriteFile: true);
                            break;

                        case CategoryImportType.CostElement:
                            var sampleCostElements = new[]
                            {
                                new { Ma_Yeu_To_CP = "YTCP01", Ten_Yeu_To_CP = "Chi phí nhân công trực tiếp" },
                                new { Ma_Yeu_To_CP = "YTCP02", Ten_Yeu_To_CP = "Chi phí nguyên vật liệu chính" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleCostElements, overwriteFile: true);
                            break;
                    }

                    MessageBox.Show($"Đã tạo tệp mẫu thành công tại:\n{dialog.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tạo tệp mẫu:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}