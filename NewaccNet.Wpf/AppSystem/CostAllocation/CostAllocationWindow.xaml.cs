using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Wpf.AppSystem.Helpers;

namespace NewaccNet.Wpf.AppSystem.CostAllocation
{
    public class AccountFilterItem
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;

        public override string ToString() => AccountId;
    }

    public partial class CostAllocationWindow : NewaccNet.Wpf.Views.Base.BaseWindow
    {
        private const string KeyFromDate = "CostAllocation_FromDate";
        private const string KeyToDate = "CostAllocation_ToDate";
        private const string KeySourceAccount = "CostAllocation_SourceAccount";
        private const string KeyDestPrefix = "CostAllocation_Dest_";
        private const string KeyElemPrefix = "CostAllocation_Elem_";
        private const string KeyCriteriaPrefix = "CostAllocation_Criteria_";

        private readonly ICostAllocationService _allocationService = new CostAllocationService();
        private readonly ObservableCollection<AccountFilterItem> _criteriaAccounts = new ObservableCollection<AccountFilterItem>();
        private Dictionary<string, string> _accountDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private List<ChartOfAccountEntity> _allAccounts = new List<ChartOfAccountEntity>();
        private List<ChartOfAccountEntity> _leafAccounts = new List<ChartOfAccountEntity>();
        private CostAllocationCalculationResult _currentCalculation;
        private bool _isInitializing = true;

        public CostAllocationWindow()
        {
            InitializeComponent();
            lstCriteriaAccounts.ItemsSource = _criteriaAccounts;

            this.Loaded += CostAllocationWindow_Loaded;
            this.Closing += CostAllocationWindow_Closing;
        }

        private void CostAllocationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _isInitializing = true;
            try
            {
                LoadLookups();
                LoadFormSettings();
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private void CostAllocationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFormSettings();
        }

        private void LoadLookups()
        {
            try
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    // 1. Tải toàn bộ danh mục tài khoản
                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);
                    _allAccounts = accounts
                        .Where(a => !string.IsNullOrWhiteSpace(a.AccountId))
                        .OrderBy(a => a.AccountId)
                        .ToList();

                    _accountDict = _allAccounts.ToDictionary(
                        a => a.AccountId.Trim(),
                        a => a.AccountName ?? a.AccountId,
                        StringComparer.OrdinalIgnoreCase);

                    // Xác định danh sách tài khoản lá (cấp dưới cùng / không có tài khoản con)
                    var parentIds = _allAccounts
                        .Select(a => a.ParentId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Select(id => id.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _leafAccounts = _allAccounts
                        .Where(a => !parentIds.Contains(a.AccountId.Trim()))
                        .ToList();

                    // Tài khoản nguồn, đích và bộ chọn tài khoản tiêu thức (cho phép chọn cả TK tổng hợp như 154, 627 để bung ra TK con giống VBA)
                    lueSourceAccount.ItemsSource = _allAccounts;
                    lueDestAccount.ItemsSource = _allAccounts;
                    lueAddCriteriaAccount.ItemsSource = _allAccounts;

                    // 2. Nạp danh mục Yếu tố chi phí
                    var costElements = new EntityCollection<CostElementEntity>();
                    adapter.FetchEntityCollection(costElements, null);
                    var sortedCostElements = costElements.OrderBy(e => e.Id).ToList();
                    lueDestCostElement.ItemsSource = sortedCostElements;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ---- Lưu & Nạp quy tắc (Rule Persistence) theo từng Tài khoản nguồn ----

        private void LoadFormSettings()
        {
            try
            {
                // 1. Khoảng thời gian
                string fromStr = UserPreferencesHelper.GetSetting(KeyFromDate);
                dtFromDate.DateTime = !string.IsNullOrEmpty(fromStr) && DateTime.TryParse(fromStr, out DateTime fromDt)
                    ? fromDt
                    : new DateTime(DateTime.Today.Year, 1, 1);

                string toStr = UserPreferencesHelper.GetSetting(KeyToDate);
                dtToDate.DateTime = !string.IsNullOrEmpty(toStr) && DateTime.TryParse(toStr, out DateTime toDt)
                    ? toDt
                    : DateTime.Today;

                // 2. Tài khoản nguồn
                string srcAcc = UserPreferencesHelper.GetSetting(KeySourceAccount);
                if (string.IsNullOrWhiteSpace(srcAcc))
                {
                    srcAcc = "1541";
                }
                lueSourceAccount.EditValue = srcAcc;

                // 3. Áp dụng Rule của Tài khoản nguồn đang chọn
                ApplyRuleForSourceAccount(srcAcc);
            }
            catch { }
        }

        private void ApplyRuleForSourceAccount(string sourceAccountId)
        {
            if (string.IsNullOrWhiteSpace(sourceAccountId)) return;
            string key = sourceAccountId.Trim();

            // 1. Tài khoản đích (ưu tiên theo TK nguồn, fallback về cài đặt chung)
            string destAcc = UserPreferencesHelper.GetSetting(KeyDestPrefix + key);
            if (string.IsNullOrWhiteSpace(destAcc))
                destAcc = UserPreferencesHelper.GetSetting("CostAllocation_DestAccount");
            lueDestAccount.EditValue = !string.IsNullOrWhiteSpace(destAcc) ? destAcc : key;

            // 2. Yếu tố chi phí đích
            string destElem = UserPreferencesHelper.GetSetting(KeyElemPrefix + key);
            if (string.IsNullOrWhiteSpace(destElem))
                destElem = UserPreferencesHelper.GetSetting("CostAllocation_DestCostElement");

            if (!string.IsNullOrWhiteSpace(destElem))
            {
                lueDestCostElement.EditValue = destElem;
            }
            else if (lueDestCostElement.ItemsSource is IList<CostElementEntity> elemList && elemList.Count > 0)
            {
                lueDestCostElement.EditValue = elemList[0].Id;
            }

            // 3. Danh sách tài khoản tiêu thức
            string savedCriteria = UserPreferencesHelper.GetSetting(KeyCriteriaPrefix + key);
            if (string.IsNullOrWhiteSpace(savedCriteria))
                savedCriteria = UserPreferencesHelper.GetSetting("CostAllocation_CriteriaAccounts");

            _criteriaAccounts.Clear();
            if (!string.IsNullOrWhiteSpace(savedCriteria))
            {
                var accs = savedCriteria.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var acc in accs)
                {
                    AddAccountsByPrefix(acc.Trim());
                }
            }

            if (_criteriaAccounts.Count == 0)
            {
                SetDefaultCriteria();
            }
        }

        private void SaveFormSettings()
        {
            if (_isInitializing) return;

            try
            {
                if (dtFromDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting(KeyFromDate, dtFromDate.DateTime.ToString("yyyy-MM-dd"));

                if (dtToDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting(KeyToDate, dtToDate.DateTime.ToString("yyyy-MM-dd"));

                string src = lueSourceAccount.EditValue?.ToString()?.Trim();
                if (string.IsNullOrEmpty(src)) return;

                UserPreferencesHelper.SetSetting(KeySourceAccount, src);

                string dest = lueDestAccount.EditValue?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(dest))
                {
                    UserPreferencesHelper.SetSetting(KeyDestPrefix + src, dest);
                    UserPreferencesHelper.SetSetting("CostAllocation_DestAccount", dest);
                }

                string elem = lueDestCostElement.EditValue?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(elem))
                {
                    UserPreferencesHelper.SetSetting(KeyElemPrefix + src, elem);
                    UserPreferencesHelper.SetSetting("CostAllocation_DestCostElement", elem);
                }

                if (_criteriaAccounts != null)
                {
                    string joined = string.Join(";", _criteriaAccounts.Select(x => x.AccountId));
                    UserPreferencesHelper.SetSetting(KeyCriteriaPrefix + src, joined);
                    UserPreferencesHelper.SetSetting("CostAllocation_CriteriaAccounts", joined);
                }
            }
            catch { }
        }

        /// <summary>
        /// Thêm tài khoản vào danh sách tiêu thức theo đúng cơ chế VBA (AddAccount_AfterUpdate):
        /// INSERT INTO [Temp Account List] SELECT [Account ID] FROM [Account List] WHERE [Account ID] Like 'AddAccount*'
        /// </summary>
        private void AddAccountsByPrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return;

            var matchingLeaves = _leafAccounts
                .Where(l => l.AccountId.Trim().StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matchingLeaves.Count > 0)
            {
                foreach (var leaf in matchingLeaves)
                {
                    string id = leaf.AccountId.Trim();
                    if (!_criteriaAccounts.Any(x => x.AccountId.Equals(id, StringComparison.OrdinalIgnoreCase)))
                    {
                        _criteriaAccounts.Add(new AccountFilterItem
                        {
                            AccountId = id,
                            AccountName = leaf.AccountName ?? id
                        });
                    }
                }
            }
            else if (!_criteriaAccounts.Any(x => x.AccountId.Equals(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                string accName = _accountDict.TryGetValue(prefix, out var name) ? name : prefix;
                _criteriaAccounts.Add(new AccountFilterItem { AccountId = prefix, AccountName = accName });
            }
        }

        private void SetDefaultCriteria()
        {
            _criteriaAccounts.Clear();
            var defaultCandidates = new[] { "1541", "1542", "1543", "621", "622", "627" };
            foreach (var acc in defaultCandidates)
            {
                AddAccountsByPrefix(acc);
            }
        }

        private void ResetCalculationState(string reason = "Tham số đã thay đổi. Vui lòng bấm 'Tính toán' để tổng hợp số liệu.")
        {
            _currentCalculation = null;
            if (btnDistribute != null) btnDistribute.IsEnabled = false;
            if (lblStatus != null)
            {
                lblStatus.Text = reason;
                lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(110, 110, 110));
            }
        }

        private void SourceAccount_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;

            string src = lueSourceAccount.EditValue?.ToString()?.Trim();
            _isInitializing = true;
            try
            {
                if (!string.IsNullOrEmpty(src))
                {
                    ApplyRuleForSourceAccount(src);
                }
            }
            finally
            {
                _isInitializing = false;
            }

            SaveFormSettings();
            ResetCalculationState();
        }

        private void BtnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            string accId = lueAddCriteriaAccount.EditValue?.ToString()?.Trim();
            if (string.IsNullOrEmpty(accId))
            {
                MessageBox.Show("Vui lòng chọn một tài khoản tiêu thức để thêm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            AddAccountsByPrefix(accId);
            SaveFormSettings();
            ResetCalculationState();
        }

        private void BtnRemoveAccount_Click(object sender, RoutedEventArgs e)
        {
            if (lstCriteriaAccounts.SelectedItems.Count > 0)
            {
                var itemsToRemove = lstCriteriaAccounts.SelectedItems.Cast<AccountFilterItem>().ToList();
                foreach (var item in itemsToRemove)
                {
                    _criteriaAccounts.Remove(item);
                }
                SaveFormSettings();
                ResetCalculationState();
            }
            else if (lstCriteriaAccounts.SelectedItem is AccountFilterItem selectedItem)
            {
                _criteriaAccounts.Remove(selectedItem);
                SaveFormSettings();
                ResetCalculationState();
            }
        }

        private void LstCriteriaAccounts_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
            {
                BtnRemoveAccount_Click(sender, e);
                e.Handled = true;
            }
        }

        private void LstCriteriaAccounts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnRemoveAccount_Click(sender, e);
        }

        private void BtnClearAccounts_Click(object sender, RoutedEventArgs e)
        {
            _criteriaAccounts.Clear();
            SaveFormSettings();
            ResetCalculationState();
        }

        private void FilterParam_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;

            SaveFormSettings();
            ResetCalculationState();
        }

        private CostAllocationParams BuildParams()
        {
            return new CostAllocationParams
            {
                FromDate = dtFromDate.DateTime,
                ToDate = dtToDate.DateTime,
                SourceAccountId = lueSourceAccount.EditValue?.ToString()?.Trim() ?? string.Empty,
                DestinationAccountId = lueDestAccount.EditValue?.ToString()?.Trim() ?? string.Empty,
                DestinationCostElementId = lueDestCostElement.EditValue?.ToString()?.Trim() ?? string.Empty,
                AllocationBaseAccountIds = _criteriaAccounts.Select(x => x.AccountId).ToList()
            };
        }

        private async void BtnRefreshPool_Click(object sender, RoutedEventArgs e)
        {
            await RunCalculationAsync();
        }

        private async void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            await RunCalculationAsync();
        }

        private async System.Threading.Tasks.Task<bool> RunCalculationAsync()
        {
            try
            {
                var param = BuildParams();
                if (string.IsNullOrWhiteSpace(param.SourceAccountId))
                {
                    MessageBox.Show("Vui lòng chọn Tài khoản nguồn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                btnDistribute.IsEnabled = false;
                lblStatus.Text = "Đang tổng hợp số liệu phân bổ...";
                lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243));

                var result = await _allocationService.CalculateAllocationAsync(param);

                _currentCalculation = result;
                gridSourcePool.ItemsSource = result.SourcePoolItems;
                lblTotalPool.Text = $"{result.TotalPoolAmount:#,##0} đ";

                SaveFormSettings();

                // 1. KIỂM TRA DÒNG TỔNG CHI PHÍ NGUỒN = 0: HẾT CHI PHÍ ĐỂ PHÂN BỔ
                if (Math.Abs(result.TotalPoolAmount) < 0.0001)
                {
                    gridCriteria.ItemsSource = null;
                    btnDistribute.IsEnabled = false;
                    lblTotalPool.Text = "0 đ";
                    lblStatus.Text = "⚠️ Dòng tổng chi phí nguồn bằng 0 (đã kết chuyển/phân bổ hết hoặc không có chi phí) - Không có chi phí để phân bổ!";
                    lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
                    return false;
                }

                gridCriteria.ItemsSource = result.CriteriaItems;

                // 2. KIỂM TRA DÒNG TỔNG TIÊU THỨC = 0: KHÔNG CÓ CƠ SỞ CHIA TỶ LỆ
                if (Math.Abs(result.TotalBaseAmount) < 0.0001)
                {
                    btnDistribute.IsEnabled = false;
                    lblStatus.Text = "⚠️ Tổng tiêu thức phân bổ bằng 0 (không có chi phí trực tiếp để chia tỷ lệ) - Không thể phân bổ!";
                    lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
                    return false;
                }

                if (result.HasError)
                {
                    btnDistribute.IsEnabled = false;
                    lblStatus.Text = $"⚠️ {result.ErrorMessage}";
                    lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
                    MessageBox.Show(result.ErrorMessage, "Thông báo tính toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // 3. HỢP LỆ VÀ CÓ DÒNG TỔNG DƯƠNG -> CHO PHÉP PHÂN BỔ
                btnDistribute.IsEnabled = true;
                lblStatus.Text = $"✓ Sẵn sàng phân bổ: {result.CriteriaItems.Count} đối tượng chi phí | Tổng số tiền: {result.TotalPoolAmount:#,##0} đ";
                lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(46, 125, 50));

                return true;
            }
            catch (Exception ex)
            {
                btnDistribute.IsEnabled = false;
                lblStatus.Text = "Lỗi tính toán: " + ex.Message;
                lblStatus.Foreground = new SolidColorBrush(Color.FromRgb(211, 47, 47));
                MessageBox.Show("Lỗi tính toán: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async void BtnDistribute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var param = BuildParams();

                if (string.IsNullOrWhiteSpace(param.DestinationCostElementId))
                {
                    MessageBox.Show("Vui lòng chọn Yếu tố đích trước khi thực hiện phân bổ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lueDestCostElement.Focus();
                    return;
                }

                if (_currentCalculation == null || _currentCalculation.HasError)
                {
                    bool ok = await RunCalculationAsync();
                    if (!ok) return;
                }

                if (_currentCalculation == null || Math.Abs(_currentCalculation.TotalPoolAmount) < 0.0001)
                {
                    MessageBox.Show("Dòng tổng chi phí bằng 0 (đã kết chuyển hết hoặc không có chi phí). Không còn chi phí để phân bổ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnDistribute.IsEnabled = false;
                    return;
                }

                if (Math.Abs(_currentCalculation.TotalBaseAmount) < 0.0001 || _currentCalculation.CriteriaItems.Count == 0)
                {
                    MessageBox.Show("Tổng tiêu thức phân bổ bằng 0. Không có cơ sở để tính tỷ lệ phân bổ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnDistribute.IsEnabled = false;
                    return;
                }

                string msg = $"Bạn có chắc chắn muốn thực hiện phân bổ chi phí?\n\n" +
                             $"- Tài khoản nguồn (có): {param.SourceAccountId}\n" +
                             $"- Tài khoản đích (nợ): {param.DestinationAccountId}\n" +
                             $"- Tổng số tiền phân bổ: {_currentCalculation.TotalPoolAmount:#,##0} đ\n" +
                             $"- Số đối tượng nhận phân bổ: {_currentCalculation.CriteriaItems.Count} đối tượng\n" +
                             $"- Ngày hạch toán: {param.ToDate:dd/MM/yyyy}\n\n" +
                             $"(Hệ thống sẽ tự động tạo chứng từ phân bổ vào sổ kế toán)";

                var confirm = MessageBox.Show(msg, "Xác nhận phân bổ chi phí", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirm != MessageBoxResult.Yes) return;

                btnDistribute.IsEnabled = false;
                btnCalculate.IsEnabled = false;
                lblStatus.Text = "Đang sinh chứng từ phân bổ...";

                var voucher = await _allocationService.ExecuteAllocationAsync(param, _currentCalculation);

                MessageBox.Show($"Phân bổ chi phí thành công!\n\nSố chứng từ tự động: {voucher.VoucherNo}\nTổng số tiền: {_currentCalculation.TotalPoolAmount:#,##0} đ",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Tính toán lại: số dư chi phí nguồn sẽ tự động trở về 0 (do bút toán Ghi Có DBCR = -1 bù trừ) và khóa nút Phân bổ
                await RunCalculationAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi trong quá trình sinh chứng từ phân bổ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnCalculate.IsEnabled = true;
                btnDistribute.IsEnabled = (_currentCalculation != null
                                           && _currentCalculation.TotalPoolAmount > 0.0001
                                           && _currentCalculation.TotalBaseAmount > 0.0001
                                           && !_currentCalculation.HasError);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (gridCriteria.ItemsSource == null)
            {
                MessageBox.Show("Vui lòng bấm 'Tính toán' để có số liệu phân bổ trước khi in.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var view = gridCriteria.View as DevExpress.Xpf.Grid.TableView;
                if (view != null)
                {
                    view.ShowPrintPreview(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị bản in: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
