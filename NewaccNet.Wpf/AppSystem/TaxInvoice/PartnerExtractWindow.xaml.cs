using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using NewaccNet.TaxInvoice;
using NewaccNet.Wpf.Views.Base;

namespace NewaccNet.Wpf.AppSystem.TaxInvoice
{
    public class ExtractedPartnerItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _customerName = string.Empty;
        private string _address = string.Empty;
        private string _dbStatus = "Đang kiểm tra...";

        public string PartnerId { get; set; } = string.Empty;
        public string Taxno { get; set; } = string.Empty;

        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); }
        }

        public string FullName { get; set; } = string.Empty;

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public string RoleDisplay { get; set; } = "Nhà cung cấp";
        public bool IsExisting { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

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

    public partial class PartnerExtractWindow : BaseWindow
    {
        private readonly List<TaxInvoiceDto> _invoices;
        private readonly string? _databasePath;
        private readonly ObservableCollection<ExtractedPartnerItem> _partners = new ObservableCollection<ExtractedPartnerItem>();

        public PartnerExtractWindow(IEnumerable<TaxInvoiceDto> invoices, string? databasePath = null)
        {
            InitializeComponent();

            _invoices = invoices?.ToList() ?? new List<TaxInvoiceDto>();
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

            gridPartners.ItemsSource = _partners;
            lblFooter.Text = $"CSDL: {_databasePath ?? "Chưa kết nối"}";

            this.Loaded += PartnerExtractWindow_Loaded;
        }

        private async void PartnerExtractWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await AnalyzeAndLoadPartnersAsync();
        }

        private async Task AnalyzeAndLoadPartnersAsync()
        {
            _partners.Clear();
            lblStats.Text = "Đang trích xuất đối tác từ hóa đơn...";

            var dict = new Dictionary<string, ExtractedPartnerItem>(StringComparer.OrdinalIgnoreCase);

            // 1. Group invoices into unique partners by tax code
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
            var existingIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(_databasePath) && System.IO.File.Exists(_databasePath))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var connStr = $"Provider=Microsoft.ACE.OLEDB.16.0;Data Source={_databasePath};Persist Security Info=False;";
                        using var conn = new OleDbConnection(connStr);
                        conn.Open();

                        using var cmd = new OleDbCommand("SELECT Id FROM Partner", conn);
                        using var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                existingIds.Add(reader.GetString(0).Trim());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Lỗi đọc danh mục Partner: " + ex.Message);
                    }
                });
            }

            // 3. Mark existing vs new
            foreach (var item in dict.Values.OrderBy(x => x.CustomerName))
            {
                if (existingIds.Contains(item.PartnerId) || existingIds.Contains(item.Taxno))
                {
                    item.IsExisting = true;
                    item.DbStatus = "Đã có trong DM";
                    item.IsSelected = false;
                }
                else
                {
                    item.IsExisting = false;
                    item.DbStatus = "Mới (Chưa có)";
                    item.IsSelected = true;
                }
                _partners.Add(item);
            }

            UpdateStats();
        }

        private void UpdateStats()
        {
            var total = _partners.Count;
            var newCount = _partners.Count(x => !x.IsExisting);
            var selected = _partners.Count(x => x.IsSelected);

            lblStats.Text = $"Tổng số: {total} đối tác ({newCount} đối tác mới chưa có trong DM) | Đã chọn: {selected}";
        }

        private void BtnSelectNew_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in _partners)
            {
                p.IsSelected = !p.IsExisting;
            }
            UpdateStats();
        }

        private void BtnUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in _partners) p.IsSelected = false;
            UpdateStats();
        }

        private async void BtnSaveToDb_Click(object sender, RoutedEventArgs e)
        {
            var selected = _partners.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất một đối tác để lưu vào danh mục.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_databasePath) || !System.IO.File.Exists(_databasePath))
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

                    const string checkSql = "SELECT Id FROM Partner WHERE Id = ?";
                    const string insertSql = @"INSERT INTO Partner 
                        (Id, CustomerName, Address, Phonenumber, Faxnumber, Note, Taxno, Isemployee) 
                        VALUES (?, ?, ?, '', '', ?, ?, false)";
                    const string updateSql = @"UPDATE Partner SET 
                        CustomerName = ?, Address = ?, Note = ?, Taxno = ? 
                        WHERE Id = ?";

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

                        // Check existence
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
                            item.DbStatus = "Đã lưu vào DM";
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