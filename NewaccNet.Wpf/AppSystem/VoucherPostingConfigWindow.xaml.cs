using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Core.Data;

namespace NewaccNet.Wpf.AppSystem
{
    public partial class VoucherPostingConfigWindow : ThemedWindow
    {
        private VoucherPostingConfigDto _config;

        public VoucherPostingConfigWindow()
        {
            InitializeComponent();
            LoadAccounts();
            LoadConfigData();
        }

        private void LoadAccounts()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);

                    var parentIds = accounts.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
                    var leafAccounts = accounts
                        .Where(a => !parentIds.Contains(a.AccountId))
                        .OrderBy(a => a.AccountId)
                        .ToList();

                    cboNmTotalAccount.ItemsSource = leafAccounts;
                    cboNmTaxAccount.ItemsSource = leafAccounts;

                    cboXbTotalAccount.ItemsSource = leafAccounts;
                    cboXbRevenueAccount.ItemsSource = leafAccounts;
                    cboXbTaxAccount.ItemsSource = leafAccounts;
                    cboXbCostAccount.ItemsSource = leafAccounts;

                    cboNnbTotalAccount.ItemsSource = leafAccounts;
                    cboXnbTotalAccount.ItemsSource = leafAccounts;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục tài khoản: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadConfigData()
        {
            _config = DbConfigService.Current.GetVoucherPostingConfig();
            BindConfigToUI(_config);
        }

        private void BindConfigToUI(VoucherPostingConfigDto cfg)
        {
            if (cfg == null) cfg = DbConfigService.GetDefaultVoucherPostingConfig();

            // 1. Nhập mua (NM)
            if (cfg.Inventory.TryGetValue("NM", out var nm))
            {
                txtNmName.Text = nm.Name;
                cboNmTotalAccount.EditValue = nm.TotalAccountId;
                cboNmTaxAccount.EditValue = nm.TaxAccountId;
            }
            else
            {
                txtNmName.Text = "Nhập mua";
                cboNmTotalAccount.EditValue = "3311";
                cboNmTaxAccount.EditValue = "13311";
            }

            // 2. Xuất bán (XB)
            if (cfg.Inventory.TryGetValue("XB", out var xb))
            {
                txtXbName.Text = xb.Name;
                cboXbTotalAccount.EditValue = xb.TotalAccountId;
                cboXbRevenueAccount.EditValue = xb.RevenueAccountId;
                cboXbTaxAccount.EditValue = xb.TaxAccountId;
                cboXbCostAccount.EditValue = xb.CostAccountId;
            }
            else
            {
                txtXbName.Text = "Xuất bán";
                cboXbTotalAccount.EditValue = "131";
                cboXbRevenueAccount.EditValue = "5111";
                cboXbTaxAccount.EditValue = "33311";
                cboXbCostAccount.EditValue = "6321";
            }

            // 3. Nhập nội bộ (NNB)
            if (cfg.Inventory.TryGetValue("NNB", out var nnb))
            {
                txtNnbName.Text = nnb.Name;
                cboNnbTotalAccount.EditValue = nnb.TotalAccountId;
            }
            else
            {
                txtNnbName.Text = "Nhập nội bộ";
                cboNnbTotalAccount.EditValue = "154";
            }

            // 4. Xuất nội bộ (XNB)
            if (cfg.Inventory.TryGetValue("XNB", out var xnb))
            {
                txtXnbName.Text = xnb.Name;
                cboXnbTotalAccount.EditValue = xnb.TotalAccountId;
            }
            else
            {
                txtXnbName.Text = "Xuất nội bộ";
                cboXnbTotalAccount.EditValue = "642";
            }
        }

        private void BtnResetDefault_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn khôi phục toàn bộ tài khoản ngầm định về giá trị tiêu chuẩn ban đầu?",
                "Xác nhận khôi phục", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                BindConfigToUI(DbConfigService.GetDefaultVoucherPostingConfig());
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_config == null) _config = new VoucherPostingConfigDto();

                // 1. Nhập mua (NM)
                _config.Inventory["NM"] = new InventoryVoucherConfigItemDto
                {
                    Code = "NM",
                    Name = string.IsNullOrWhiteSpace(txtNmName.Text) ? "Nhập mua" : txtNmName.Text.Trim(),
                    Dbcr = 1,
                    Taxflag = true,
                    TotalAccountId = cboNmTotalAccount.EditValue?.ToString() ?? "3311",
                    TaxAccountId = cboNmTaxAccount.EditValue?.ToString() ?? "13311",
                    CostAccountId = "6321",
                    RevenueAccountId = "5111",
                    PartnerType = "Vendor"
                };

                // 2. Xuất bán (XB)
                _config.Inventory["XB"] = new InventoryVoucherConfigItemDto
                {
                    Code = "XB",
                    Name = string.IsNullOrWhiteSpace(txtXbName.Text) ? "Xuất bán" : txtXbName.Text.Trim(),
                    Dbcr = -1,
                    Taxflag = true,
                    TotalAccountId = cboXbTotalAccount.EditValue?.ToString() ?? "131",
                    TaxAccountId = cboXbTaxAccount.EditValue?.ToString() ?? "33311",
                    RevenueAccountId = cboXbRevenueAccount.EditValue?.ToString() ?? "5111",
                    CostAccountId = cboXbCostAccount.EditValue?.ToString() ?? "6321",
                    PartnerType = "Customer"
                };

                // 3. Nhập nội bộ (NNB)
                _config.Inventory["NNB"] = new InventoryVoucherConfigItemDto
                {
                    Code = "NNB",
                    Name = string.IsNullOrWhiteSpace(txtNnbName.Text) ? "Nhập nội bộ" : txtNnbName.Text.Trim(),
                    Dbcr = 1,
                    Taxflag = false,
                    TotalAccountId = cboNnbTotalAccount.EditValue?.ToString() ?? "154",
                    TaxAccountId = "",
                    CostAccountId = "",
                    RevenueAccountId = "",
                    PartnerType = "Department"
                };

                // 4. Xuất nội bộ (XNB)
                _config.Inventory["XNB"] = new InventoryVoucherConfigItemDto
                {
                    Code = "XNB",
                    Name = string.IsNullOrWhiteSpace(txtXnbName.Text) ? "Xuất nội bộ" : txtXnbName.Text.Trim(),
                    Dbcr = -1,
                    Taxflag = false,
                    TotalAccountId = cboXnbTotalAccount.EditValue?.ToString() ?? "642",
                    TaxAccountId = "",
                    CostAccountId = "",
                    RevenueAccountId = "",
                    PartnerType = "Department"
                };

                var currentDbConfig = DbConfigService.LoadDbConfig();
                currentDbConfig.SetVoucherPostingConfig(_config);

                bool saved = DbConfigService.SaveDbConfig(currentDbConfig);
                if (saved)
                {
                    DbConfigService.ClearCache();
                    MessageBox.Show("Đã lưu cấu hình tài khoản ngầm định chứng từ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Lỗi khi lưu cấu hình: " + DbConfigService.LastSaveError, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
