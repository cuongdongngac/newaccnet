using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using MiniExcelLibs;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.AppSystem;
using NewaccNet.Wpf.AppSystem.AI;
using NewaccNet.Wpf.AppSystem.Directory;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public class ExtractedPartnerItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _partnerId = string.Empty;
        private string _taxno = string.Empty;
        private string _customerName = string.Empty;
        private string _address = string.Empty;
        private string _dbStatus = "Đang kiểm tra...";

        public string PartnerId
        {
            get => _partnerId;
            set { if (_partnerId != value) { _partnerId = value; OnPropertyChanged(); } }
        }

        public string Taxno
        {
            get => _taxno;
            set { if (_taxno != value) { _taxno = value; OnPropertyChanged(); } }
        }

        public string CustomerName
        {
            get => _customerName;
            set { if (_customerName != value) { _customerName = value; OnPropertyChanged(); } }
        }

        public string FullName { get; set; } = string.Empty;

        public string Address
        {
            get => _address;
            set { if (_address != value) { _address = value; OnPropertyChanged(); } }
        }

        public string RoleDisplay { get; set; } = "Nhà cung cấp";
        public bool IsExisting { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public string DbStatus
        {
            get => _dbStatus;
            set { if (_dbStatus != value) { _dbStatus = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class PartnerExtractWindow : BaseWindow
    {
        private readonly List<TaxInvoiceDto> _invoices;
        private readonly string? _databasePath;
        private readonly ObservableCollection<ExtractedPartnerItem> _allPartners = new();
        private readonly ObservableCollection<ExtractedPartnerItem> _displayedPartners = new();

        private readonly HashSet<string> _dbPartnersById = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _dbPartnersByTax = new(StringComparer.OrdinalIgnoreCase);

        public PartnerExtractWindow(IEnumerable<TaxInvoiceDto>? invoices, string? databasePath = null)
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

            gridPartners.ItemsSource = _displayedPartners;
            lblFooter.Text = $"CSDL: {_databasePath ?? "Chưa kết nối"}";

            this.Loaded += PartnerExtractWindow_Loaded;
        }

        private async void PartnerExtractWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await AnalyzeAndLoadPartnersAsync();
        }

        private async Task AnalyzeAndLoadPartnersAsync()
        {
            _allPartners.Clear();
            _displayedPartners.Clear();
            _dbPartnersById.Clear();
            _dbPartnersByTax.Clear();
            lblStats.Text = "Đang trích xuất đối tác từ hóa đơn và đối chiếu CSDL...";

            // 1. Group invoices into unique partners by tax code
            var dict = new Dictionary<string, ExtractedPartnerItem>(StringComparer.OrdinalIgnoreCase);

            foreach (var inv in _invoices)
            {
                if (inv.Direction == "sold")
                {
                    var mst = inv.BuyerTaxCode?.Trim();
                    if (!string.IsNullOrWhiteSpace(mst) && !dict.ContainsKey(mst))
                    {
                        var name = inv.BuyerName?.Trim() ?? "";
                        dict[mst] = new ExtractedPartnerItem
                        {
                            PartnerId = mst,
                            Taxno = mst,
                            CustomerName = name,
                            FullName = name,
                            Address = inv.BuyerAddress?.Trim() ?? "",
                            RoleDisplay = "Khách hàng"
                        };
                    }
                }
                else
                {
                    var mst = inv.SellerTaxCode?.Trim();
                    if (!string.IsNullOrWhiteSpace(mst) && !dict.ContainsKey(mst))
                    {
                        var name = inv.SellerName?.Trim() ?? "";
                        dict[mst] = new ExtractedPartnerItem
                        {
                            PartnerId = mst,
                            Taxno = mst,
                            CustomerName = name,
                            FullName = name,
                            Address = inv.SellerAddress?.Trim() ?? "",
                            RoleDisplay = "Nhà cung cấp"
                        };
                    }
                }
            }

            // 2. Query existing partners from Access DB
            if (!string.IsNullOrEmpty(_databasePath) && File.Exists(_databasePath))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                        using var conn = new OleDbConnection(connStr);
                        conn.Open();

                        using var cmd = new OleDbCommand("SELECT [Id], [Taxno] FROM [Partner]", conn);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var id = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim();
                            var tax = reader.IsDBNull(1) ? "" : reader.GetString(1)?.Trim() ?? "";

                            if (!string.IsNullOrEmpty(id)) _dbPartnersById.Add(id);
                            if (!string.IsNullOrEmpty(tax) && !_dbPartnersByTax.ContainsKey(tax))
                            {
                                _dbPartnersByTax[tax] = id;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Lỗi đọc danh mục Partner: " + ex.Message);
                    }
                });
            }

            // 3. Mark existing vs new and propose codes
            var usedCodes = new HashSet<string>(_dbPartnersById, StringComparer.OrdinalIgnoreCase);
            int ruleIndex = cboNamingRule.SelectedIndex;
            var prefix = txtPrefix.Text.Trim();
            int nccCounter = 1;
            int khCounter = 1;
            int genCounter = 1;

            foreach (var item in dict.Values.OrderBy(x => x.CustomerName))
            {
                bool exists = false;
                string assignedCode = item.PartnerId;

                // Kiểm tra xem đối tác đã có trong CSDL theo Id hoặc MST chưa
                if (_dbPartnersById.Contains(item.PartnerId))
                {
                    exists = true;
                    assignedCode = item.PartnerId;
                }
                else if (!string.IsNullOrEmpty(item.Taxno) && _dbPartnersByTax.TryGetValue(item.Taxno, out var existingId))
                {
                    exists = true;
                    assignedCode = existingId;
                }
                else
                {
                    // Sinh mã theo quy tắc được chọn
                    assignedCode = GenerateCodeByRule(item, ruleIndex, prefix, ref nccCounter, ref khCounter, ref genCounter);
                    assignedCode = MaterialCodeGenerator.MakeUniqueCode(assignedCode, usedCodes);
                }

                item.PartnerId = assignedCode;
                item.IsExisting = exists;
                item.DbStatus = exists ? "Đã có trong CSDL" : "Mới (Chưa có)";
                item.IsSelected = !exists;

                _allPartners.Add(item);
            }

            ApplyFilter();
        }

        private string GenerateCodeByRule(ExtractedPartnerItem item, int ruleIndex, string prefix, ref int nccCounter, ref int khCounter, ref int genCounter)
        {
            switch (ruleIndex)
            {
                case 1: // Phân loại theo vai trò (NCC0001 / KH0001)
                    return PartnerCodeGenerator.GenerateRolePrefixedCode(item.RoleDisplay, nccCounter++, khCounter++);

                case 2: // Viết tắt tên đối tác (Bỏ Cty, TNHH, CP...)
                    var acronym = PartnerCodeGenerator.GenerateAcronymPartner(item.CustomerName);
                    return !string.IsNullOrEmpty(acronym) ? acronym : (!string.IsNullOrEmpty(item.Taxno) ? item.Taxno : "DT");

                case 3: // Tùy chỉnh tiền tố + Số tự tăng
                    return MaterialCodeGenerator.GeneratePrefixNumber(prefix, genCounter++);

                case 0: // Sử dụng MST (Mặc định)
                default:
                    if (!string.IsNullOrWhiteSpace(item.Taxno))
                    {
                        var safeMst = MaterialCodeGenerator.SanitizeCode(item.Taxno);
                        if (!string.IsNullOrEmpty(safeMst)) return safeMst;
                    }
                    return PartnerCodeGenerator.GenerateAcronymPartner(item.CustomerName);
            }
        }

        private void CboNamingRule_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (txtPrefix == null) return;
            txtPrefix.IsEnabled = cboNamingRule.SelectedIndex == 3;
        }

        private void BtnApplyRule_Click(object sender, RoutedEventArgs e)
        {
            if (_allPartners.Count == 0) return;

            int ruleIndex = cboNamingRule.SelectedIndex;
            var prefix = txtPrefix.Text.Trim();
            bool keepExisting = chkKeepExistingDbCodes.IsChecked == true;

            var usedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (keepExisting)
            {
                foreach (var item in _allPartners.Where(x => x.IsExisting))
                {
                    if (!string.IsNullOrEmpty(item.PartnerId))
                    {
                        usedCodes.Add(item.PartnerId);
                    }
                }
            }

            int nccCounter = 1;
            int khCounter = 1;
            int genCounter = 1;

            foreach (var item in _allPartners)
            {
                if (keepExisting && item.IsExisting) continue;

                var code = GenerateCodeByRule(item, ruleIndex, prefix, ref nccCounter, ref khCounter, ref genCounter);
                code = MaterialCodeGenerator.MakeUniqueCode(code, usedCodes);
                item.PartnerId = code;
            }

            MessageBox.Show("Đã áp dụng quy tắc sinh mã đối tác thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSelectNew_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in _allPartners)
            {
                p.IsSelected = !p.IsExisting;
            }
            UpdateStats();
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in _displayedPartners) p.IsSelected = true;
            UpdateStats();
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in _displayedPartners) p.IsSelected = false;
            UpdateStats();
        }

        private void CboFilterStatus_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            _displayedPartners.Clear();
            int filterIndex = cboFilterStatus.SelectedIndex;

            foreach (var item in _allPartners)
            {
                if (filterIndex == 1 && item.IsExisting) continue; // Chỉ mới
                if (filterIndex == 2 && !item.IsExisting) continue; // Chỉ đã có

                _displayedPartners.Add(item);
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            var total = _allPartners.Count;
            var newCount = _allPartners.Count(x => !x.IsExisting);
            var selected = _allPartners.Count(x => x.IsSelected);
            var existingCount = total - newCount;

            lblStats.Text = $"Tổng số: {total} đối tác (Mới: {newCount}, Đã có: {existingCount}) | Đã chọn: {selected} đối tác";
        }

        // ==============================================================
        // ACTION 0.5: OPEN IN OFFICE STUDIO (DATA HUB)
        // ==============================================================

        private void BtnOpenInOffice_Click(object sender, RoutedEventArgs e)
        {
            var selected = _allPartners.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0) selected = _allPartners.ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu đối tác nào để mở trong Office Studio.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var exportRows = selected.Select(x => new
                {
                    Id = x.PartnerId,
                    CustomerName = x.CustomerName,
                    Taxno = x.Taxno,
                    Address = x.Address,
                    Note = x.RoleDisplay,
                    DbStatus = x.DbStatus
                }).ToList();

                var ms = new MemoryStream();
                MiniExcel.SaveAs(ms, exportRows);
                ms.Position = 0;

                var officeWin = new OfficeStudioWindow(ms, OfficeDocumentType.Spreadsheet, "Danh mục Đối tác trích xuất từ HĐĐT", _databasePath);
                officeWin.Owner = this.Owner ?? this;
                officeWin.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở Office Studio: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==============================================================
        // ACTION 1: EXPORT TO EXCEL
        // ==============================================================

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var selected = _allPartners.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một đối tác để xuất ra tệp Excel.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dupCode = selected.GroupBy(x => x.PartnerId, StringComparer.OrdinalIgnoreCase)
                                  .FirstOrDefault(g => g.Count() > 1);
            if (dupCode != null)
            {
                MessageBox.Show($"Phát hiện mã đối tác trùng lặp: '{dupCode.Key}'. Vui lòng điều chỉnh lại mã trước khi xuất Excel.", "Cảnh báo trùng mã", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"DanhMuc_DoiTac_Tu_HDDT_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
                Title = "Lưu danh mục đối tác ra tệp Excel"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var exportRows = selected.Select(x => new
                    {
                        Id = x.PartnerId,
                        CustomerName = x.CustomerName,
                        Taxno = x.Taxno,
                        Address = x.Address,
                        Note = x.RoleDisplay
                    }).ToList();

                    MiniExcel.SaveAs(dialog.FileName, exportRows, overwriteFile: true);

                    var result = MessageBox.Show(
                        $"Đã xuất thành công {selected.Count} đối tác ra tệp Excel:\n{dialog.FileName}\n\nBạn có muốn mở ngay chức năng 'Nhập từ Excel' của Danh mục Đối tượng Công nợ để nạp vào CSDL không?",
                        "Xuất Excel thành công",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var importWin = new CategoryExcelImportWindow(CategoryImportType.Partner, _databasePath, dialog.FileName);
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

        private async void BtnSaveToDb_Click(object sender, RoutedEventArgs e)
        {
            var selected = _allPartners.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một đối tác để lưu vào danh mục.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_databasePath) || !File.Exists(_databasePath))
            {
                MessageBox.Show("Không tìm thấy tệp CSDL Access để lưu dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            btnSaveToDb.IsEnabled = false;
            int inserted = 0;
            int updated = 0;

            try
            {
                await Task.Run(() =>
                {
                    var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                    using var conn = new OleDbConnection(connStr);
                    conn.Open();

                    const string checkSql = "SELECT [Id] FROM [Partner] WHERE [Id] = ?";
                    const string insertSql = @"INSERT INTO [Partner] 
                        ([Id], [CustomerName], [Address], [Phonenumber], [Faxnumber], [Note], [Taxno], [Isemployee]) 
                        VALUES (?, ?, ?, '', '', ?, ?, false)";
                    const string updateSql = @"UPDATE [Partner] SET 
                        [CustomerName] = ?, [Address] = ?, [Note] = ?, [Taxno] = ? 
                        WHERE [Id] = ?";

                    foreach (var item in selected)
                    {
                        var partnerId = item.PartnerId.Length > 20 ? item.PartnerId.Substring(0, 20) : item.PartnerId;
                        var rawName = item.CustomerName?.Trim() ?? "";
                        var shortName = rawName.Length > 50 ? rawName.Substring(0, 50) : rawName;
                        var address = item.Address?.Trim() ?? "";
                        if (address.Length > 255) address = address.Substring(0, 255);
                        var taxNo = item.Taxno?.Trim() ?? "";
                        if (taxNo.Length > 50) taxNo = taxNo.Substring(0, 50);
                        var note = rawName.Length > 50 ? $"Tên đầy đủ: {rawName}" : "";

                        bool exists = false;
                        using (var checkCmd = new OleDbCommand(checkSql, conn))
                        {
                            checkCmd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = partnerId;
                            var scalar = checkCmd.ExecuteScalar();
                            exists = scalar != null && scalar != DBNull.Value;
                        }

                        if (!exists)
                        {
                            using var insCmd = new OleDbCommand(insertSql, conn);
                            insCmd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = partnerId;
                            insCmd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = shortName;
                            insCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = address;
                            insCmd.Parameters.Add("?", OleDbType.LongVarWChar).Value = note;
                            insCmd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = taxNo;
                            insCmd.ExecuteNonQuery();

                            item.IsExisting = true;
                            item.DbStatus = "Đã có trong CSDL";
                            inserted++;
                        }
                        else
                        {
                            using var updCmd = new OleDbCommand(updateSql, conn);
                            updCmd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = shortName;
                            updCmd.Parameters.Add("?", OleDbType.VarWChar, 255).Value = address;
                            updCmd.Parameters.Add("?", OleDbType.LongVarWChar).Value = note;
                            updCmd.Parameters.Add("?", OleDbType.VarWChar, 50).Value = taxNo;
                            updCmd.Parameters.Add("?", OleDbType.VarWChar, 20).Value = partnerId;
                            updCmd.ExecuteNonQuery();

                            item.DbStatus = "Đã cập nhật";
                            updated++;
                        }
                    }
                });

                MessageBox.Show($"Đã lưu thành công {inserted} đối tác mới và cập nhật {updated} đối tác vào CSDL!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi ghi danh mục đối tác:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
