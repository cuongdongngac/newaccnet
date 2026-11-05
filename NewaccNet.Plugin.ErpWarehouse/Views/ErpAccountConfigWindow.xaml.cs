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

namespace NewaccNet.Plugin.ErpWarehouse.Views
{
    public class VoucherSlotItem
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public override string ToString() => Name;
    }

    public partial class ErpAccountConfigWindow : ThemedWindow
    {
        private List<ChartOfAccountEntity> _baseAccounts = new();
        private Dictionary<string, ChartOfAccountEntity> _baseAccountMap = new(StringComparer.OrdinalIgnoreCase);

        // Cấu trúc dữ liệu: VoucherCode ("NM", "XB", "NNB", "XNB") -> SlotKey ("Revenue", "Cost"...) -> List Accounts
        private readonly Dictionary<string, Dictionary<string, ObservableCollection<VoucherPostingAccountOptionDto>>> _voucherSlots = new(StringComparer.OrdinalIgnoreCase);

        // Danh sách các Slot của từng loại phiếu
        private static readonly List<VoucherSlotItem> NmSlots = new()
        {
            new VoucherSlotItem { Key = "Inventory", Name = "🏬 Kho hàng nhập (Nợ TK 15x)" },
            new VoucherSlotItem { Key = "Tax", Name = "🧾 Thuế GTGT mua vào (Nợ TK 133)" },
            new VoucherSlotItem { Key = "Total", Name = "💳 Thanh toán / Phải trả NCC (Có TK 331, 111...)" }
        };

        private static readonly List<VoucherSlotItem> XbSlots = new()
        {
            new VoucherSlotItem { Key = "Revenue", Name = "🛒 Doanh thu bán hàng (Có TK 511, 711)" },
            new VoucherSlotItem { Key = "Cost", Name = "📦 Giá vốn hàng bán (Nợ TK 632, 811)" },
            new VoucherSlotItem { Key = "Tax", Name = "🧾 Thuế GTGT bán ra (Có TK 3331)" },
            new VoucherSlotItem { Key = "Total", Name = "💳 Phải thu / Thu tiền (Nợ TK 131, 111...)" },
            new VoucherSlotItem { Key = "Inventory", Name = "🏬 Kho hàng xuất (Có TK 15x)" }
        };

        private static readonly List<VoucherSlotItem> NnbSlots = new()
        {
            new VoucherSlotItem { Key = "Inventory", Name = "🏬 Kho hàng nhập (Nợ TK 15x)" },
            new VoucherSlotItem { Key = "Offset", Name = "🔄 TK đối ứng NNB (Có TK 154, 711...)" }
        };

        private static readonly List<VoucherSlotItem> XnbSlots = new()
        {
            new VoucherSlotItem { Key = "Inventory", Name = "🏬 Kho hàng xuất (Có TK 15x)" },
            new VoucherSlotItem { Key = "Offset", Name = "🔄 TK đối ứng XNB (Nợ TK 642, 641, 154...)" }
        };

        public ErpAccountConfigWindow()
        {
            InitializeComponent();
            LoadBaseAccounts();
            InitSlotListBoxes();
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

                // Gán ComboBoxEditSettings chuẩn màn hình thiết kế (kèm template 2 cột Mã TK | Tên TK) cho các cột AccountId
                AssignDesignAccountEditSettings(gridNmAccounts);
                AssignDesignAccountEditSettings(gridXbAccounts);
                AssignDesignAccountEditSettings(gridNnbAccounts);
                AssignDesignAccountEditSettings(gridXnbAccounts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục tài khoản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AssignDesignAccountEditSettings(GridControl grid)
        {
            if (grid.Columns["AccountId"] is GridColumn col)
            {
                col.EditSettings = NewaccNet.Core.UI.AccountDesignHelper.CreateAccountComboBoxEditSettings(_baseAccounts);
            }
        }

        private void InitSlotListBoxes()
        {
            lstSlotsNm.ItemsSource = NmSlots;
            lstSlotsXb.ItemsSource = XbSlots;
            lstSlotsNnb.ItemsSource = NnbSlots;
            lstSlotsXnb.ItemsSource = XnbSlots;

            lstSlotsNm.SelectedIndex = 0;
            lstSlotsXb.SelectedIndex = 0;
            lstSlotsNnb.SelectedIndex = 0;
            lstSlotsXnb.SelectedIndex = 0;
        }

        private void LoadConfigData()
        {
            var cfg = DbConfigService.Current.GetVoucherPostingConfig();
            var defCfg = DbConfigService.GetDefaultVoucherPostingConfig();

            _voucherSlots.Clear();
            foreach (var code in new[] { "NM", "XB", "NNB", "XNB" })
            {
                _voucherSlots[code] = new Dictionary<string, ObservableCollection<VoucherPostingAccountOptionDto>>(StringComparer.OrdinalIgnoreCase);

                var itemSlots = cfg.Inventory.TryGetValue(code, out var item) && item.AccountSlots != null && item.AccountSlots.Count > 0
                    ? item.AccountSlots
                    : (defCfg.Inventory.TryGetValue(code, out var defItem) ? defItem.AccountSlots : new());

                foreach (var kvp in itemSlots)
                {
                    var col = new ObservableCollection<VoucherPostingAccountOptionDto>();
                    foreach (var acc in kvp.Value)
                    {
                        string cat = acc.CategoryId;
                        string name = acc.AccountName;
                        if (_baseAccountMap.TryGetValue(acc.AccountId, out var baseAcc))
                        {
                            if (string.IsNullOrWhiteSpace(name)) name = baseAcc.AccountName ?? "";
                            if (string.IsNullOrEmpty(cat)) cat = baseAcc.CategoryId ?? "";
                        }

                        col.Add(new VoucherPostingAccountOptionDto
                        {
                            AccountId = acc.AccountId,
                            AccountName = name,
                            CategoryId = cat,
                            ParentId = acc.ParentId,
                            IsDefault = acc.IsDefault,
                            AllowDetail = (cat == "B") ? false : acc.AllowDetail
                        });
                    }
                    _voucherSlots[code][kvp.Key] = col;
                }
            }

            // Đồng bộ hiển thị lên các grid
            RefreshSlotGrid("NM");
            RefreshSlotGrid("XB");
            RefreshSlotGrid("NNB");
            RefreshSlotGrid("XNB");
        }

        private void RefreshSlotGrid(string voucherCode)
        {
            if (!_voucherSlots.TryGetValue(voucherCode, out var slots)) return;

            string slotKey = "";
            switch (voucherCode)
            {
                case "NM":
                    slotKey = (lstSlotsNm.SelectedItem as VoucherSlotItem)?.Key ?? "Inventory";
                    lblSlotTitleNm.Text = (lstSlotsNm.SelectedItem as VoucherSlotItem)?.Name ?? "Danh sách tài khoản";
                    gridNmAccounts.ItemsSource = GetOrCreateSlotCollection(slots, slotKey);
                    break;
                case "XB":
                    slotKey = (lstSlotsXb.SelectedItem as VoucherSlotItem)?.Key ?? "Revenue";
                    lblSlotTitleXb.Text = (lstSlotsXb.SelectedItem as VoucherSlotItem)?.Name ?? "Danh sách tài khoản";
                    gridXbAccounts.ItemsSource = GetOrCreateSlotCollection(slots, slotKey);
                    break;
                case "NNB":
                    slotKey = (lstSlotsNnb.SelectedItem as VoucherSlotItem)?.Key ?? "Inventory";
                    lblSlotTitleNnb.Text = (lstSlotsNnb.SelectedItem as VoucherSlotItem)?.Name ?? "Danh sách tài khoản";
                    gridNnbAccounts.ItemsSource = GetOrCreateSlotCollection(slots, slotKey);
                    break;
                case "XNB":
                    slotKey = (lstSlotsXnb.SelectedItem as VoucherSlotItem)?.Key ?? "Inventory";
                    lblSlotTitleXnb.Text = (lstSlotsXnb.SelectedItem as VoucherSlotItem)?.Name ?? "Danh sách tài khoản";
                    gridXnbAccounts.ItemsSource = GetOrCreateSlotCollection(slots, slotKey);
                    break;
            }
        }

        private ObservableCollection<VoucherPostingAccountOptionDto> GetOrCreateSlotCollection(
            Dictionary<string, ObservableCollection<VoucherPostingAccountOptionDto>> slots, string slotKey)
        {
            if (!slots.TryGetValue(slotKey, out var list))
            {
                list = new ObservableCollection<VoucherPostingAccountOptionDto>();
                slots[slotKey] = list;
            }
            return list;
        }

        private string GetCurrentVoucherCode()
        {
            return tabControl.SelectedIndex switch
            {
                0 => "NM",
                1 => "XB",
                2 => "NNB",
                3 => "XNB",
                _ => "XB"
            };
        }

        private GridControl GetCurrentGrid()
        {
            return tabControl.SelectedIndex switch
            {
                0 => gridNmAccounts,
                1 => gridXbAccounts,
                2 => gridNnbAccounts,
                3 => gridXnbAccounts,
                _ => gridXbAccounts
            };
        }

        private void LstSlotsNm_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e) => RefreshSlotGrid("NM");
        private void LstSlotsXb_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e) => RefreshSlotGrid("XB");
        private void LstSlotsNnb_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e) => RefreshSlotGrid("NNB");
        private void LstSlotsXnb_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e) => RefreshSlotGrid("XNB");
        private void TabControl_SelectionChanged(object sender, DevExpress.Xpf.Core.TabControlSelectionChangedEventArgs e)
        {
            RefreshSlotGrid(GetCurrentVoucherCode());
        }

        private void ViewAccounts_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (e.Row is not VoucherPostingAccountOptionDto row) return;

            if (e.Column.FieldName == "AccountId")
            {
                string accId = e.Value?.ToString() ?? "";
                if (_baseAccountMap.TryGetValue(accId, out var baseAcc))
                {
                    row.AccountName = baseAcc.AccountName ?? "";
                    row.CategoryId = baseAcc.CategoryId ?? "";
                    row.ParentId = baseAcc.ParentId;
                    row.AllowDetail = (baseAcc.CategoryId != "B"); // Nhóm B (Vật tư) mặc định tắt chi tiết con
                }
            }
            else if (e.Column.FieldName == "IsDefault")
            {
                bool isDef = (bool)(e.Value ?? false);
                if (isDef)
                {
                    // Đảm bảo trong mỗi mục chỉ có duy nhất 1 tài khoản làm ngầm định
                    var grid = GetCurrentGrid();
                    if (grid.ItemsSource is ObservableCollection<VoucherPostingAccountOptionDto> list)
                    {
                        foreach (var item in list)
                        {
                            if (!ReferenceEquals(item, row))
                            {
                                item.IsDefault = false;
                            }
                        }
                        grid.RefreshData();
                    }
                }
            }
        }

        private void BtnAddSlotAccount_Click(object sender, RoutedEventArgs e)
        {
            var grid = GetCurrentGrid();
            if (grid.ItemsSource is ObservableCollection<VoucherPostingAccountOptionDto> list)
            {
                bool isFirst = list.Count == 0;
                var newItem = new VoucherPostingAccountOptionDto
                {
                    AccountId = "",
                    AccountName = "",
                    CategoryId = "",
                    IsDefault = isFirst,
                    AllowDetail = true
                };
                list.Add(newItem);
                grid.SelectedItem = newItem;
            }
        }

        private void BtnDeleteSlotAccount_Click(object sender, RoutedEventArgs e)
        {
            var grid = GetCurrentGrid();
            if (grid.SelectedItem is VoucherPostingAccountOptionDto selected && grid.ItemsSource is ObservableCollection<VoucherPostingAccountOptionDto> list)
            {
                list.Remove(selected);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng tài khoản cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnResetAllDefault_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn khôi phục toàn bộ danh sách tài khoản cho cả 4 loại phiếu về mặc định ban đầu?",
                "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            var defCfg = DbConfigService.GetDefaultVoucherPostingConfig();
            var cfg = DbConfigService.Current.GetVoucherPostingConfig();

            foreach (var code in new[] { "NM", "XB", "NNB", "XNB" })
            {
                if (defCfg.Inventory.TryGetValue(code, out var defItem) && cfg.Inventory.TryGetValue(code, out var item))
                {
                    item.AccountSlots = defItem.AccountSlots;
                }
            }

            LoadConfigData();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cfg = DbConfigService.Current.GetVoucherPostingConfig();

                foreach (var vKvp in _voucherSlots)
                {
                    string vCode = vKvp.Key;
                    if (!cfg.Inventory.TryGetValue(vCode, out var invItem))
                    {
                        invItem = new InventoryVoucherConfigItemDto { Code = vCode };
                        cfg.Inventory[vCode] = invItem;
                    }

                    invItem.AccountSlots.Clear();
                    foreach (var sKvp in vKvp.Value)
                    {
                        string sKey = sKvp.Key;
                        var validList = sKvp.Value.Where(x => !string.IsNullOrWhiteSpace(x.AccountId)).ToList();

                        // Kiểm tra nếu chưa có dòng nào làm default thì đặt dòng đầu tiên
                        if (validList.Count > 0 && !validList.Any(x => x.IsDefault))
                        {
                            validList[0].IsDefault = true;
                        }

                        invItem.AccountSlots[sKey] = validList;

                        // Đồng bộ giá trị mặc định vào các trường gốc tương ứng
                        var defAcc = validList.FirstOrDefault(x => x.IsDefault)?.AccountId ?? (validList.Count > 0 ? validList[0].AccountId : "");
                        if (!string.IsNullOrEmpty(defAcc))
                        {
                            if (sKey.Equals("Total", StringComparison.OrdinalIgnoreCase)) invItem.TotalAccountId = defAcc;
                            else if (sKey.Equals("Tax", StringComparison.OrdinalIgnoreCase)) invItem.TaxAccountId = defAcc;
                            else if (sKey.Equals("Cost", StringComparison.OrdinalIgnoreCase)) invItem.CostAccountId = defAcc;
                            else if (sKey.Equals("Revenue", StringComparison.OrdinalIgnoreCase)) invItem.RevenueAccountId = defAcc;
                        }
                    }
                }

                bool ok = DbConfigService.SaveVoucherPostingConfig(cfg);
                if (ok)
                {
                    MessageBox.Show("Đã lưu cấu hình danh mục tài khoản định khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Không thể lưu cấu hình: " + DbConfigService.LastSaveError, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu cấu hình: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Export / Import JSON
        private void BtnExportJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exportData = CollectCurrentSlotsData();

                var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    Title = "Xuất cấu hình tài khoản định khoản ERP ra file JSON",
                    FileName = $"ErpPostingConfig_{DateTime.Now:yyyyMMdd_HHmmss}.json"
                };

                if (sfd.ShowDialog() == true)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };
                    string json = System.Text.Json.JsonSerializer.Serialize(exportData, options);
                    System.IO.File.WriteAllText(sfd.FileName, json);

                    MessageBox.Show(
                        $"Đã xuất cấu hình thành công ra file JSON:\n{sfd.FileName}\n\nBạn có thể lưu trữ hoặc nạp (import) vào các CSDL khác.",
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
                    Title = "Nạp cấu hình tài khoản định khoản ERP từ file JSON"
                };

                if (ofd.ShowDialog() != true) return;

                string json = System.IO.File.ReadAllText(ofd.FileName);
                if (string.IsNullOrWhiteSpace(json))
                {
                    MessageBox.Show("Tệp JSON được chọn rỗng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Hỗ trợ 2 dạng:
                // Dạng 1: Dictionary<string, Dictionary<string, List<VoucherPostingAccountOptionDto>>> (của riêng form này)
                // Dạng 2: VoucherPostingConfigDto (toàn bộ cấu hình hệ thống)
                Dictionary<string, Dictionary<string, List<VoucherPostingAccountOptionDto>>>? importedSlots = null;

                try
                {
                    importedSlots = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<VoucherPostingAccountOptionDto>>>>(json);
                }
                catch
                {
                    var fullCfg = System.Text.Json.JsonSerializer.Deserialize<VoucherPostingConfigDto>(json);
                    if (fullCfg != null && fullCfg.Inventory != null)
                    {
                        importedSlots = new Dictionary<string, Dictionary<string, List<VoucherPostingAccountOptionDto>>>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kvp in fullCfg.Inventory)
                        {
                            if (kvp.Value?.AccountSlots != null)
                            {
                                importedSlots[kvp.Key] = kvp.Value.AccountSlots;
                            }
                        }
                    }
                }

                if (importedSlots == null || importedSlots.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy cấu hình định khoản phù hợp trong file JSON!", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cập nhật vào cấu trúc _voucherSlots
                foreach (var vKvp in importedSlots)
                {
                    string vCode = vKvp.Key;
                    if (!_voucherSlots.ContainsKey(vCode))
                    {
                        _voucherSlots[vCode] = new Dictionary<string, ObservableCollection<VoucherPostingAccountOptionDto>>(StringComparer.OrdinalIgnoreCase);
                    }

                    foreach (var sKvp in vKvp.Value)
                    {
                        string sKey = sKvp.Key;
                        var obsList = new ObservableCollection<VoucherPostingAccountOptionDto>();
                        foreach (var item in sKvp.Value)
                        {
                            string cat = item.CategoryId;
                            string name = item.AccountName;
                            if (_baseAccountMap.TryGetValue(item.AccountId, out var bAcc))
                            {
                                if (string.IsNullOrEmpty(cat)) cat = bAcc.CategoryId ?? "";
                                if (string.IsNullOrWhiteSpace(name)) name = bAcc.AccountName ?? "";
                            }

                            obsList.Add(new VoucherPostingAccountOptionDto
                            {
                                AccountId = item.AccountId,
                                AccountName = name,
                                CategoryId = cat,
                                ParentId = item.ParentId,
                                IsDefault = item.IsDefault,
                                AllowDetail = (cat == "B") ? false : item.AllowDetail
                            });
                        }

                        _voucherSlots[vCode][sKey] = obsList;
                    }
                }

                // Cập nhật lại toàn bộ grid trên 4 tab
                RefreshSlotGrid("NM");
                RefreshSlotGrid("XB");
                RefreshSlotGrid("NNB");
                RefreshSlotGrid("XNB");

                var askSave = MessageBox.Show(
                    "Đã nạp thành công cấu hình từ file JSON lên các tab giao diện.\n\nBạn có muốn LƯU NGAY cấu hình này vào Cơ sở dữ liệu (DbConfig) không?",
                    "Nạp thành công",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (askSave == MessageBoxResult.Yes)
                {
                    BtnSave_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Dữ liệu đã được nạp lên lưới. Bạn có thể kiểm tra, chỉnh sửa và bấm 'Lưu cấu hình' bất cứ lúc nào.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp file JSON: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Dictionary<string, Dictionary<string, List<VoucherPostingAccountOptionDto>>> CollectCurrentSlotsData()
        {
            var result = new Dictionary<string, Dictionary<string, List<VoucherPostingAccountOptionDto>>>(StringComparer.OrdinalIgnoreCase);

            foreach (var vKvp in _voucherSlots)
            {
                string vCode = vKvp.Key;
                result[vCode] = new Dictionary<string, List<VoucherPostingAccountOptionDto>>(StringComparer.OrdinalIgnoreCase);

                foreach (var sKvp in vKvp.Value)
                {
                    string sKey = sKvp.Key;
                    var validList = sKvp.Value
                        .Where(x => !string.IsNullOrWhiteSpace(x.AccountId))
                        .Select(x => new VoucherPostingAccountOptionDto
                        {
                            AccountId = x.AccountId,
                            AccountName = x.AccountName,
                            CategoryId = x.CategoryId,
                            ParentId = x.ParentId,
                            AllowDetail = x.AllowDetail,
                            IsDefault = x.IsDefault
                        })
                        .ToList();

                    if (validList.Count > 0 && !validList.Any(x => x.IsDefault))
                    {
                        validList[0].IsDefault = true;
                    }

                    result[vCode][sKey] = validList;
                }
            }

            return result;
        }
        #endregion

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
