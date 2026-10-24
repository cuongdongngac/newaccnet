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
        Account,
        Stock,
        Warehouse
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

        public CategoryExcelImportWindow(CategoryImportType initialType = CategoryImportType.Partner, string? databasePath = null)
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
                CategoryImportType.Account => 1,
                CategoryImportType.Stock => 2,
                CategoryImportType.Warehouse => 3,
                _ => 0
            };
            cboCategoryType.SelectedIndex = catIndex;
            UpdateCategorySchemaUI();
        }

        private CategoryImportType CurrentCategory
        {
            get
            {
                return cboCategoryType.SelectedIndex switch
                {
                    0 => CategoryImportType.Partner,
                    1 => CategoryImportType.Account,
                    2 => CategoryImportType.Stock,
                    3 => CategoryImportType.Warehouse,
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
                    lblTarget1.Text = "* Mã đối tác (Id):";
                    lblTarget2.Text = "* Tên đối tác:";
                    lblTarget3.Text = "Mã số thuế:";
                    lblTarget4.Text = "Địa chỉ:";
                    lblTarget5.Text = "Điện thoại:";
                    lblTarget6.Text = "Ghi chú:";

                    colCode.Header = "Mã Đối tác (Id)";
                    colName.Header = "Tên Đối tác";
                    colField1.Header = "Mã số thuế";
                    colField2.Header = "Địa chỉ";
                    colField3.Header = "Điện thoại";
                    break;

                case CategoryImportType.Account:
                    lblTarget1.Text = "* Số hiệu TK (Id):";
                    lblTarget2.Text = "* Tên tài khoản:";
                    lblTarget3.Text = "Tính chất (Nature):";
                    lblTarget4.Text = "Loại TK (AccountType):";
                    lblTarget5.Text = "Cấp TK (Level):";
                    lblTarget6.Text = "Ghi chú:";

                    colCode.Header = "Số hiệu TK";
                    colName.Header = "Tên tài khoản";
                    colField1.Header = "Tính chất";
                    colField2.Header = "Loại TK";
                    colField3.Header = "Cấp TK";
                    break;

                case CategoryImportType.Stock:
                    lblTarget1.Text = "* Mã vật tư (Stockid):";
                    lblTarget2.Text = "* Tên vật tư (Description):";
                    lblTarget3.Text = "Mã loại VT (Stocktypeid):";
                    lblTarget4.Text = "Trường phụ 2:";
                    lblTarget5.Text = "Trường phụ 3:";
                    lblTarget6.Text = "Ghi chú:";

                    colCode.Header = "Mã vật tư";
                    colName.Header = "Tên vật tư";
                    colField1.Header = "Mã loại VT";
                    colField2.Header = "Trường phụ 2";
                    colField3.Header = "Trường phụ 3";
                    break;

                case CategoryImportType.Warehouse:
                    lblTarget1.Text = "* Mã kho (WarehouseId):";
                    lblTarget2.Text = "* Tên kho (WarehouseName):";
                    lblTarget3.Text = "Địa chỉ kho (Place):";
                    lblTarget4.Text = "Trường phụ 2:";
                    lblTarget5.Text = "Trường phụ 3:";
                    lblTarget6.Text = "Ghi chú:";

                    colCode.Header = "Mã kho";
                    colName.Header = "Tên kho";
                    colField1.Header = "Địa chỉ kho";
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
                    MatchCombo(cboMap1, "mã|code|id|mst|số hiệu");
                    MatchCombo(cboMap2, "tên|name|khách hàng|nhà cung cấp|đối tác");
                    MatchCombo(cboMap3, "mst|tax|thuế|mã số thuế");
                    MatchCombo(cboMap4, "địa chỉ|dchi|address");
                    MatchCombo(cboMap5, "điện thoại|phone|tel|sđt");
                    MatchCombo(cboMap6, "ghi chú|note|diễn giải");
                    break;

                case CategoryImportType.Account:
                    MatchCombo(cboMap1, "số hiệu|tài khoản|tk|code|id");
                    MatchCombo(cboMap2, "tên|name|diễn giải");
                    MatchCombo(cboMap3, "tính chất|nature|loại");
                    MatchCombo(cboMap4, "loại tk|accounttype|loại");
                    MatchCombo(cboMap5, "cấp|level");
                    MatchCombo(cboMap6, "ghi chú|note");
                    break;

                case CategoryImportType.Stock:
                    MatchCombo(cboMap1, "mã|stockid|code|vật tư|hàng hóa");
                    MatchCombo(cboMap2, "tên|description|name|diễn giải");
                    MatchCombo(cboMap3, "loại|stocktypeid|loại vt");
                    MatchCombo(cboMap4, "đvt|unit|đơn vị");
                    break;

                case CategoryImportType.Warehouse:
                    MatchCombo(cboMap1, "mã kho|warehouseid|code|id");
                    MatchCombo(cboMap2, "tên kho|warehousename|name");
                    MatchCombo(cboMap3, "địa chỉ|place|address");
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
                    CategoryImportType.Account => "ChartOfAccount",
                    CategoryImportType.Stock => "Stock",
                    CategoryImportType.Warehouse => "Warehouse",
                    _ => "Partner"
                };

                string idColumn = CurrentCategory switch
                {
                    CategoryImportType.Stock => "Stockid",
                    CategoryImportType.Warehouse => "WarehouseId",
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
                        case CategoryImportType.Account:
                            ExecuteAccountImport(conn, selectedRows, out inserted, out updated);
                            break;
                        case CategoryImportType.Stock:
                            ExecuteStockImport(conn, selectedRows, out inserted, out updated);
                            break;
                        case CategoryImportType.Warehouse:
                            ExecuteWarehouseImport(conn, selectedRows, out inserted, out updated);
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

            const string checkSql = "SELECT Id FROM Partner WHERE Id = ?";
            const string insertSql = "INSERT INTO Partner (Id, CustomerName, Taxno, Address, Phonenumber, Faxnumber, Note, Isemployee) VALUES (?, ?, ?, ?, ?, '', ?, false)";
            const string updateSql = "UPDATE Partner SET CustomerName = ?, Taxno = ?, Address = ?, Phonenumber = ?, Note = ? WHERE Id = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 20 ? r.Code.Substring(0, 20) : r.Code;
                var rawName = r.Name?.Trim() ?? "";
                var shortName = rawName.Length > 50 ? rawName.Substring(0, 50) : rawName;
                var taxno = (r.Field1?.Trim() ?? "");
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

        private void ExecuteAccountImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT Id FROM ChartOfAccount WHERE Id = ?";
            const string insertSql = "INSERT INTO ChartOfAccount (Id, AccountName, Nature) VALUES (?, ?, ?)";
            const string updateSql = "UPDATE ChartOfAccount SET AccountName = ?, Nature = ? WHERE Id = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 20 ? r.Code.Substring(0, 20) : r.Code;
                var name = r.Name?.Trim() ?? "";
                var nature = r.Field1?.Trim() ?? "";

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
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = nature;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = name;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = nature;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void ExecuteStockImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT Stockid FROM Stock WHERE Stockid = ?";
            const string insertSql = "INSERT INTO Stock (Stockid, Description, Stocktypeid) VALUES (?, ?, ?)";
            const string updateSql = "UPDATE Stock SET Description = ?, Stocktypeid = ? WHERE Stockid = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 10 ? r.Code.Substring(0, 10) : r.Code;
                var desc = r.Name.Length > 50 ? r.Name.Substring(0, 50) : r.Name;
                short typeId = short.TryParse(r.Field1, out var t) ? t : (short)1;

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
                    ins.Parameters.Add("?", OleDbType.VarWChar, 50).Value = desc;
                    ins.Parameters.Add("?", OleDbType.SmallInt).Value = typeId;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = desc;
                    upd.Parameters.Add("?", OleDbType.SmallInt).Value = typeId;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 10).Value = id;
                    upd.ExecuteNonQuery();
                    updated++;
                }
            }
        }

        private void ExecuteWarehouseImport(OleDbConnection conn, List<CategoryPreviewRowViewModel> rows, out int inserted, out int updated)
        {
            inserted = 0;
            updated = 0;

            const string checkSql = "SELECT WarehouseId FROM Warehouse WHERE WarehouseId = ?";
            const string insertSql = "INSERT INTO Warehouse (WarehouseId, WarehouseName, Place) VALUES (?, ?, ?)";
            const string updateSql = "UPDATE Warehouse SET WarehouseName = ?, Place = ? WHERE WarehouseId = ?";

            foreach (var r in rows)
            {
                var id = r.Code.Length > 10 ? r.Code.Substring(0, 10) : r.Code;
                var name = r.Name.Length > 50 ? r.Name.Substring(0, 50) : r.Name;
                var place = (r.Field1?.Trim() ?? "");
                if (place.Length > 255) place = place.Substring(0, 255);

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
                    ins.Parameters.Add("?", OleDbType.VarWChar, 255).Value = place;
                    ins.ExecuteNonQuery();
                    inserted++;
                }
                else if (chkUpdateExisting.IsChecked == true)
                {
                    using var upd = new OleDbCommand(updateSql, conn);
                    upd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = name;
                    upd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = place;
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
                                new { Ma_Doi_Tac = "0101234567", Ten_Doi_Tac = "Công ty Cổ phần Mẫu A", Ma_So_Thue = "0101234567", Dia_Chi = "Hà Nội", Dien_Thoai = "0243123456" },
                                new { Ma_Doi_Tac = "0209876543", Ten_Doi_Tac = "Công ty TNHH Mẫu B", Ma_So_Thue = "0209876543", Dia_Chi = "Hải Phòng", Dien_Thoai = "0225123456" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, samplePartners, overwriteFile: true);
                            break;

                        case CategoryImportType.Account:
                            var sampleAccounts = new[]
                            {
                                new { So_Hieu_TK = "1111", Ten_Tai_Khoan = "Tiền Việt Nam", Tinh_Chat = "Dư Nợ" },
                                new { So_Hieu_TK = "1121", Ten_Tai_Khoan = "Tiền gửi ngân hàng VND", Tinh_Chat = "Dư Nợ" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleAccounts, overwriteFile: true);
                            break;

                        case CategoryImportType.Stock:
                            var sampleStocks = new[]
                            {
                                new { Ma_Vat_Tu = "VT001", Ten_Vat_Tu = "Sắt thép xây dựng", Ma_Loai_VT = 1 },
                                new { Ma_Vat_Tu = "VT002", Ten_Vat_Tu = "Xi măng Hoàng Thạch", Ma_Loai_VT = 1 }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleStocks, overwriteFile: true);
                            break;

                        case CategoryImportType.Warehouse:
                            var sampleWarehouses = new[]
                            {
                                new { Ma_Kho = "KHO01", Ten_Kho = "Kho Tổng", Dia_Chi = "Kho số 1 Cảng" },
                                new { Ma_Kho = "KHO02", Ten_Kho = "Kho Phụ", Dia_Chi = "Chi nhánh 2" }
                            };
                            MiniExcel.SaveAs(dialog.FileName, sampleWarehouses, overwriteFile: true);
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