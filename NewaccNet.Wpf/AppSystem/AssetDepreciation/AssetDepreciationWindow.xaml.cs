using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.AppSystem.Helpers;
using NewaccNet.Wpf.AppSystem.Voucher;

namespace NewaccNet.Wpf.AppSystem.AssetDepreciation
{
    /// <summary>
    /// MANH 6 - GIAO DIEN: doc/luu tham so (kiem luon rule theo tai khoan khau hao), goi service, hien ket qua.
    /// </summary>
    public partial class AssetDepreciationWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        private const string KeyBeginDate = "AssetDep_BeginDate";
        private const string KeyEndDate = "AssetDep_EndDate";
        private const string KeyLastAccount = "AssetDep_DepAccount";

        // Tham so luu chung (fallback) va luu rieng theo tung Tai khoan khau hao (dong vai tro nhu Rule)
        private const string KeySource = "AssetDep_Source";
        private const string KeyCostElement = "AssetDep_CostElement";
        private const string KeyReason = "AssetDep_Reason";
        private const string KeyContents = "AssetDep_Contents";

        private const string KeySourcePrefix = "AssetDep_Source_";
        private const string KeyCostElementPrefix = "AssetDep_CostElement_";
        private const string KeyReasonPrefix = "AssetDep_Reason_";
        private const string KeyContentsPrefix = "AssetDep_Contents_";

        private readonly IAssetDepreciationService _service = new AssetDepreciationService();
        private readonly ObservableCollection<AssetDepreciationRow> _rows = new ObservableCollection<AssetDepreciationRow>();
        private int? _lastVoucherId;
        private bool _isInitializing = true;

        public AssetDepreciationWindow()
        {
            InitializeComponent();
            gridRows.ItemsSource = _rows;

            Loaded += AssetDepreciationWindow_Loaded;
            Closing += (s, e) => SaveSettings();
        }

        private void AssetDepreciationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitializing = true;
            try
            {
                LoadLookups();
                LoadSettings();
            }
            finally
            {
                _isInitializing = false;
            }
        }

        /// <summary>
        /// Nap danh muc Tai khoan (loc TK la Category 'D' hoac co khai bao trong AssetTracking.AsaccountId),
        /// Nguon, Yeu to chi phi, Ly do cho cac LookUpEdit 2 cot.
        /// </summary>
        private void LoadLookups()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);

                    var trackings = new EntityCollection<AssetTrackingEntity>();
                    adapter.FetchEntityCollection(trackings, null);
                    var trackedAsAccounts = trackings
                        .Where(t => !string.IsNullOrWhiteSpace(t.AsaccountId))
                        .Select(t => t.AsaccountId.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var parentIds = accounts
                        .Select(a => a.ParentId)
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .Select(id => id.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    var leafAccounts = accounts
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId) && !parentIds.Contains(a.AccountId.Trim()))
                        .OrderBy(a => a.AccountId)
                        .ToList();

                    // Bo loc tai khoan khau hao: Uu tien cac TK la loai D (TSCD) hoac da gan trong AssetTracking.AsaccountId
                    var depAccounts = leafAccounts
                        .Where(a => string.Equals(a.CategoryId?.Trim(), "D", StringComparison.OrdinalIgnoreCase)
                                    || trackedAsAccounts.Contains(a.AccountId.Trim()))
                        .ToList();

                    lueDepAccount.ItemsSource = depAccounts.Count > 0 ? depAccounts : leafAccounts;

                    var sources = new EntityCollection<SourceEntity>();
                    adapter.FetchEntityCollection(sources, null);
                    lueSource.ItemsSource = sources.OrderBy(s => s.Srcode).ToList();

                    var elements = new EntityCollection<CostElementEntity>();
                    adapter.FetchEntityCollection(elements, null);
                    lueCostElement.ItemsSource = elements.OrderBy(el => el.Id).ToList();

                    var reasons = new EntityCollection<ReasonEntity>();
                    adapter.FetchEntityCollection(reasons, null);
                    lueReason.ItemsSource = reasons.OrderBy(r => r.Reasonid).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ---- UserSettings: luu & nap tham so sau khi dung, dong vai tro nhu Rule theo Tai khoan khau hao ----

        private void LoadSettings()
        {
            try
            {
                var today = DateTime.Today;
                string beginStr = UserPreferencesHelper.GetSetting(KeyBeginDate);
                dtBeginDate.DateTime = !string.IsNullOrEmpty(beginStr) && DateTime.TryParse(beginStr, out var bDt)
                    ? bDt
                    : new DateTime(today.Year, today.Month, 1);

                string endStr = UserPreferencesHelper.GetSetting(KeyEndDate);
                dtEndDate.DateTime = !string.IsNullOrEmpty(endStr) && DateTime.TryParse(endStr, out var eDt)
                    ? eDt
                    : today;

                string lastAcc = UserPreferencesHelper.GetSetting(KeyLastAccount);
                if (!string.IsNullOrEmpty(lastAcc))
                {
                    lueDepAccount.EditValue = lastAcc;
                    ApplyRuleOfAccount(lastAcc);
                }
                else if (lueDepAccount.ItemsSource is IList<ChartOfAccountEntity> list && list.Count > 0)
                {
                    var defaultAcc = list.FirstOrDefault(a => a.AccountId.Trim().StartsWith("214")) ?? list[0];
                    lueDepAccount.EditValue = defaultAcc.AccountId;
                    ApplyRuleOfAccount(defaultAcc.AccountId.Trim());
                }
            }
            catch { }
        }

        private void ApplyRuleOfAccount(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId)) return;
            string key = accountId.Trim();

            string src = UserPreferencesHelper.GetSetting(KeySourcePrefix + key)
                         ?? UserPreferencesHelper.GetSetting(KeySource);
            lueSource.EditValue = string.IsNullOrEmpty(src) ? null : src;

            string elem = UserPreferencesHelper.GetSetting(KeyCostElementPrefix + key)
                          ?? UserPreferencesHelper.GetSetting(KeyCostElement);
            lueCostElement.EditValue = string.IsNullOrEmpty(elem) ? null : elem;

            string reason = UserPreferencesHelper.GetSetting(KeyReasonPrefix + key)
                            ?? UserPreferencesHelper.GetSetting(KeyReason);
            lueReason.EditValue = string.IsNullOrEmpty(reason) ? null : reason;

            string contents = UserPreferencesHelper.GetSetting(KeyContentsPrefix + key)
                              ?? UserPreferencesHelper.GetSetting(KeyContents);
            txtContents.EditValue = string.IsNullOrEmpty(contents) ? null : contents;
        }

        private void SaveSettings()
        {
            if (_isInitializing) return;
            try
            {
                if (dtBeginDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting(KeyBeginDate, dtBeginDate.DateTime.ToString("yyyy-MM-dd"));

                if (dtEndDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting(KeyEndDate, dtEndDate.DateTime.ToString("yyyy-MM-dd"));

                string acc = lueDepAccount.EditValue?.ToString()?.Trim() ?? string.Empty;
                string src = lueSource.EditValue?.ToString()?.Trim() ?? string.Empty;
                string elem = lueCostElement.EditValue?.ToString()?.Trim() ?? string.Empty;
                string reason = lueReason.EditValue?.ToString()?.Trim() ?? string.Empty;
                string contents = txtContents.EditValue?.ToString() ?? string.Empty;

                UserPreferencesHelper.SetSetting(KeySource, src);
                UserPreferencesHelper.SetSetting(KeyCostElement, elem);
                UserPreferencesHelper.SetSetting(KeyReason, reason);
                UserPreferencesHelper.SetSetting(KeyContents, contents);

                if (!string.IsNullOrEmpty(acc))
                {
                    UserPreferencesHelper.SetSetting(KeyLastAccount, acc);
                    UserPreferencesHelper.SetSetting(KeySourcePrefix + acc, src);
                    UserPreferencesHelper.SetSetting(KeyCostElementPrefix + acc, elem);
                    UserPreferencesHelper.SetSetting(KeyReasonPrefix + acc, reason);
                    UserPreferencesHelper.SetSetting(KeyContentsPrefix + acc, contents);
                }
            }
            catch { }
        }

        private AssetDepreciationParams BuildParams()
        {
            var selectedElement = lueCostElement.SelectedItem as CostElementEntity;
            return new AssetDepreciationParams
            {
                BeginDate = dtBeginDate.DateTime,
                EndDate = dtEndDate.DateTime,
                DepAccountId = lueDepAccount.EditValue?.ToString()?.Trim() ?? string.Empty,
                SourceCode = lueSource.EditValue?.ToString()?.Trim() ?? string.Empty,
                CostElementId = lueCostElement.EditValue?.ToString()?.Trim() ?? string.Empty,
                CostElementName = selectedElement?.ElementName ?? string.Empty,
                ReasonId = lueReason.EditValue?.ToString()?.Trim() ?? string.Empty,
                Contents = txtContents.EditValue?.ToString() ?? string.Empty
            };
        }

        private void DepAccount_Changed(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (_isInitializing || !IsLoaded) return;

            string acc = lueDepAccount.EditValue?.ToString()?.Trim() ?? string.Empty;
            _isInitializing = true;
            try
            {
                if (!string.IsNullOrEmpty(acc))
                    ApplyRuleOfAccount(acc);
            }
            finally
            {
                _isInitializing = false;
            }

            SaveSettings();
            ResetCalculationState();
        }

        private void Param_Changed(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (_isInitializing || !IsLoaded) return;
            SaveSettings();
            ResetCalculationState();
        }

        private void ResetCalculationState()
        {
            _rows.Clear();
            btnPost.IsEnabled = false;
            lblStatus.Text = "Tham số đã thay đổi. Vui lòng bấm 'Tính toán'.";
        }

        private void Row_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AssetDepreciationRow.Selected)
                || e.PropertyName == nameof(AssetDepreciationRow.DepAmount))
            {
                UpdateSummaryStatus();
            }
        }

        private void UpdateSummaryStatus()
        {
            var chosen = _rows.Where(r => r.Selected && r.DepAmount > 0.0001).ToList();
            btnPost.IsEnabled = chosen.Count > 0;
            lblStatus.Text = $"{_rows.Count} tài sản | Đã chọn {chosen.Count} tài sản | Tổng khấu hao: {chosen.Sum(r => r.DepAmount):#,##0}";
        }

        private async void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            var param = BuildParams();
            if (string.IsNullOrWhiteSpace(param.DepAccountId))
            {
                MessageBox.Show("Vui lòng chọn Tài khoản khấu hao trước khi tính toán.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveSettings();

            try
            {
                btnCalculate.IsEnabled = false;
                lblStatus.Text = "Đang tính khấu hao...";

                var result = await _service.CalculateAsync(param);

                foreach (var oldRow in _rows) oldRow.PropertyChanged -= Row_PropertyChanged;
                _rows.Clear();

                foreach (var r in result)
                {
                    r.PropertyChanged += Row_PropertyChanged;
                    _rows.Add(r);
                }

                if (_rows.Count == 0)
                {
                    btnPost.IsEnabled = false;
                    lblStatus.Text = $"Không có tài sản nào thuộc TK {param.DepAccountId} cần trích khấu hao trong kỳ.";
                }
                else
                {
                    UpdateSummaryStatus();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Lỗi tính toán: " + ex.Message;
                MessageBox.Show("Lỗi tính toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnCalculate.IsEnabled = true;
            }
        }

        private async void BtnPost_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            try
            {
                viewRows.CommitEditing();
                var param = BuildParams();

                int voucherId = await _service.PostAsync(param, _rows);
                _lastVoucherId = voucherId;
                btnRestore.IsEnabled = voucherId > 0;
                btnViewVoucher.IsEnabled = voucherId > 0;

                MessageBox.Show(
                    $"Quá trình tự động tính khấu hao và sinh chứng từ kế toán đã được thực hiện cho tài khoản {param.DepAccountId}!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Tinh toan lai luoi de cap nhat Tri gia con lai (Rest) sau khi da trich khau hao
                BtnCalculate_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (_lastVoucherId == null || _lastVoucherId <= 0) return;

            var confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn khôi phục (xóa chứng từ khấu hao vừa sinh) không?",
                "Xác nhận khôi phục",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                await _service.RestoreAsync(_lastVoucherId.Value);
                _lastVoucherId = null;
                btnRestore.IsEnabled = false;
                btnViewVoucher.IsEnabled = false;
                lblStatus.Text = "Đã khôi phục chứng từ khấu hao vừa sinh.";

                BtnCalculate_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi khôi phục: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnViewVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (_lastVoucherId == null || _lastVoucherId <= 0) return;
            var win = new VoucherEntryWindow(_lastVoucherId.Value) { Owner = this };
            win.ShowDialog();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            viewRows.ShowPrintPreview(this);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
