using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Core.Data;
using NewaccNet.Plugin.CashBank.Services;

namespace NewaccNet.Plugin.CashBank.Views
{
    public partial class CashAccountConfigWindow : ThemedWindow
    {
        private List<ChartOfAccountEntity> _baseAccounts = new();
        private Dictionary<string, ChartOfAccountEntity> _baseAccountMap = new(StringComparer.OrdinalIgnoreCase);

        private ObservableCollection<PluginAccountConfigItemDto> _cashAccounts = new();
        private ObservableCollection<CashAliasItemDto> _receiptAliases = new();
        private ObservableCollection<CashAliasItemDto> _paymentAliases = new();

        public List<string> CashCategories { get; } = new() { "Cash", "Bank" };

        public CashAccountConfigWindow()
        {
            InitializeComponent();
            LoadBaseAccounts();
            LoadConfigData();
        }

        private void LoadBaseAccounts()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();
                var accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accounts, null);

                var parentIds = accounts.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
                _baseAccounts = accounts.Where(a => !parentIds.Contains(a.AccountId))
                                        .OrderBy(a => a.AccountId)
                                        .ToList();

                _baseAccountMap = _baseAccounts.ToDictionary(a => a.AccountId, a => a, StringComparer.OrdinalIgnoreCase);

                // Gán ComboBox chọn tài khoản cho các cột
                if (gridCashAccounts.Columns["AccountId"] is GridColumn colCash)
                {
                    colCash.EditSettings = NewaccNet.Core.UI.AccountDesignHelper.CreateAccountComboBoxEditSettings(_baseAccounts);
                }
                if (gridReceiptAliases.Columns["OffsetAccount"] is GridColumn colRec)
                {
                    colRec.EditSettings = NewaccNet.Core.UI.AccountDesignHelper.CreateAccountComboBoxEditSettings(_baseAccounts);
                }
                if (gridPaymentAliases.Columns["OffsetAccount"] is GridColumn colPay)
                {
                    colPay.EditSettings = NewaccNet.Core.UI.AccountDesignHelper.CreateAccountComboBoxEditSettings(_baseAccounts);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục tài khoản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadConfigData()
        {
            try
            {
                var cfg = CashVoucherDataService.GetPostingConfig();

                _cashAccounts = new ObservableCollection<PluginAccountConfigItemDto>(cfg.CashAccounts ?? new List<PluginAccountConfigItemDto>());
                _receiptAliases = new ObservableCollection<CashAliasItemDto>(cfg.ReceiptAliases ?? new List<CashAliasItemDto>());
                _paymentAliases = new ObservableCollection<CashAliasItemDto>(cfg.PaymentAliases ?? new List<CashAliasItemDto>());

                gridCashAccounts.ItemsSource = _cashAccounts;
                gridReceiptAliases.ItemsSource = _receiptAliases;
                gridPaymentAliases.ItemsSource = _paymentAliases;

                cboDefaultCash.ItemsSource = _cashAccounts.Where(x => x.Category == "Cash").ToList();
                cboDefaultCash.EditValue = cfg.DefaultCashAccount;

                cboDefaultBank.ItemsSource = _cashAccounts.Where(x => x.Category == "Bank").ToList();
                cboDefaultBank.EditValue = cfg.DefaultBankAccount;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp cấu hình tiền mặt & ngân hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddCashAccount_Click(object sender, RoutedEventArgs e)
        {
            var item = new PluginAccountConfigItemDto
            {
                AccountId = "1111",
                BaseName = "Tiền Việt Nam",
                CustomName = "Tiền mặt tại quỹ",
                Category = "Cash",
                BaseCategoryId = "A",
                IsActive = true
            };
            _cashAccounts.Add(item);
            gridCashAccounts.SelectedItem = item;
        }

        private void BtnDeleteCashAccount_Click(object sender, RoutedEventArgs e)
        {
            if (gridCashAccounts.SelectedItem is PluginAccountConfigItemDto item)
            {
                _cashAccounts.Remove(item);
            }
        }

        private void BtnAddReceiptAlias_Click(object sender, RoutedEventArgs e)
        {
            var item = new CashAliasItemDto
            {
                Code = "THU_MOI",
                Name = "Nghiệp vụ thu tiền mới",
                OffsetAccount = "131",
                Category = "Debt",
                RequireDetail = true,
                DefaultDescription = "Thu tiền...",
                IsActive = true
            };
            _receiptAliases.Add(item);
            gridReceiptAliases.SelectedItem = item;
        }

        private void BtnDeleteReceiptAlias_Click(object sender, RoutedEventArgs e)
        {
            if (gridReceiptAliases.SelectedItem is CashAliasItemDto item)
            {
                _receiptAliases.Remove(item);
            }
        }

        private void BtnAddPaymentAlias_Click(object sender, RoutedEventArgs e)
        {
            var item = new CashAliasItemDto
            {
                Code = "CHI_MOI",
                Name = "Nghiệp vụ chi tiền mới",
                OffsetAccount = "3311",
                Category = "Debt",
                RequireDetail = true,
                DefaultDescription = "Chi tiền...",
                IsActive = true
            };
            _paymentAliases.Add(item);
            gridPaymentAliases.SelectedItem = item;
        }

        private void BtnDeletePaymentAlias_Click(object sender, RoutedEventArgs e)
        {
            if (gridPaymentAliases.SelectedItem is CashAliasItemDto item)
            {
                _paymentAliases.Remove(item);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cfg = new CashPostingConfigDto
                {
                    DefaultCashAccount = cboDefaultCash.EditValue?.ToString() ?? "1111",
                    DefaultBankAccount = cboDefaultBank.EditValue?.ToString() ?? "1121",
                    CashAccounts = _cashAccounts.ToList(),
                    ReceiptAliases = _receiptAliases.ToList(),
                    PaymentAliases = _paymentAliases.ToList()
                };

                bool ok = DbConfigService.SaveCashPostingConfig(cfg);
                if (ok)
                {
                    MessageBox.Show("Đã lưu cấu hình nghiệp vụ tiền mặt & ngân hàng thành công vào hệ thống!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Không thể lưu cấu hình! Vui lòng kiểm tra lại kết nối CSDL.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu cấu hình: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cfg = new CashPostingConfigDto
                {
                    DefaultCashAccount = cboDefaultCash.EditValue?.ToString() ?? "1111",
                    DefaultBankAccount = cboDefaultBank.EditValue?.ToString() ?? "1121",
                    CashAccounts = _cashAccounts.ToList(),
                    ReceiptAliases = _receiptAliases.ToList(),
                    PaymentAliases = _paymentAliases.ToList()
                };

                var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    Title = "Xuất cấu hình nghiệp vụ tiền ra file JSON",
                    FileName = $"CashPostingConfig_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (sfd.ShowDialog() == true)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };
                    string json = System.Text.Json.JsonSerializer.Serialize(cfg, options);
                    System.IO.File.WriteAllText(sfd.FileName, json);

                    MessageBox.Show(
                        $"Đã xuất cấu hình thành công ra file JSON:\n{sfd.FileName}\n\nBạn có thể lưu trữ hoặc nạp vào các CSDL khác.",
                        "Xuất JSON thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất file JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnImportJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    Title = "Nạp cấu hình nghiệp vụ tiền từ file JSON"
                };

                if (ofd.ShowDialog() != true) return;

                string json = System.IO.File.ReadAllText(ofd.FileName);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("Tệp JSON được chọn rỗng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var imported = System.Text.Json.JsonSerializer.Deserialize<CashPostingConfigDto>(json, options);

                if (imported != null)
                {
                    _cashAccounts.Clear();
                    foreach (var a in imported.CashAccounts ?? new List<PluginAccountConfigItemDto>()) _cashAccounts.Add(a);

                    _receiptAliases.Clear();
                    foreach (var r in imported.ReceiptAliases ?? new List<CashAliasItemDto>()) _receiptAliases.Add(r);

                    _paymentAliases.Clear();
                    foreach (var p in imported.PaymentAliases ?? new List<CashAliasItemDto>()) _paymentAliases.Add(p);

                    cboDefaultCash.EditValue = imported.DefaultCashAccount;
                    cboDefaultBank.EditValue = imported.DefaultBankAccount;

                    MessageBox.Show("Đã nạp cấu hình thành công từ file JSON! Bấm 'Lưu cấu hình' để áp dụng vào hệ thống.", "Nạp JSON thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp file JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnResetAllDefault_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(
                "Bạn có chắc chắn muốn khôi phục toàn bộ cấu hình nghiệp vụ tiền về mặc định của hệ thống?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res != MessageBoxResult.Yes) return;

            var def = DbConfigService.GetDefaultCashPostingConfig();
            _cashAccounts.Clear();
            foreach (var a in def.CashAccounts) _cashAccounts.Add(a);

            _receiptAliases.Clear();
            foreach (var r in def.ReceiptAliases) _receiptAliases.Add(r);

            _paymentAliases.Clear();
            foreach (var p in def.PaymentAliases) _paymentAliases.Add(p);

            cboDefaultCash.EditValue = def.DefaultCashAccount;
            cboDefaultBank.EditValue = def.DefaultBankAccount;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
