using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DevExpress.Xpf.Core;
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
        private readonly ICostAllocationService _allocationService = new CostAllocationService();
        private readonly ObservableCollection<AccountFilterItem> _criteriaAccounts = new ObservableCollection<AccountFilterItem>();
        private Dictionary<string, string> _accountDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                    var allAccounts = accounts.OrderBy(a => a.AccountId).ToList();

                    _accountDict = allAccounts.ToDictionary(
                        a => a.AccountId.Trim(), 
                        a => a.AccountName ?? a.AccountId, 
                        StringComparer.OrdinalIgnoreCase);

                    // Xác định danh sách tài khoản lá (cấp dưới cùng / không có tài khoản con)
                    var parentIds = accounts
                        .Select(a => a.ParentId)
                        .Where(id => !string.IsNullOrEmpty(id))
                        .Distinct()
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _leafAccounts = allAccounts
                        .Where(a => !parentIds.Contains(a.AccountId))
                        .ToList();

                    // Tài khoản nguồn và đích: cho phép chọn đầy đủ
                    lueSourceAccount.ItemsSource = allAccounts;
                    lueDestAccount.ItemsSource = allAccounts;

                    // Tài khoản tiêu thức: Người dùng lọc trên danh sách tài khoản cấp dưới cùng
                    lueAddCriteriaAccount.ItemsSource = _leafAccounts;

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

        private void LoadFormSettings()
        {
            try
            {
                // 1. Khoảng thời gian
                string fromStr = UserPreferencesHelper.GetSetting("CostAllocation_FromDate");
                if (!string.IsNullOrEmpty(fromStr) && DateTime.TryParse(fromStr, out DateTime fromDt))
                {
                    dtFromDate.DateTime = fromDt;
                }
                else
                {
                    dtFromDate.DateTime = new DateTime(DateTime.Today.Year, 1, 1);
                }

                string toStr = UserPreferencesHelper.GetSetting("CostAllocation_ToDate");
                if (!string.IsNullOrEmpty(toStr) && DateTime.TryParse(toStr, out DateTime toDt))
                {
                    dtToDate.DateTime = toDt;
                }
                else
                {
                    dtToDate.DateTime = DateTime.Today;
                }

                // 2. Tài khoản nguồn & đích
                string srcAcc = UserPreferencesHelper.GetSetting("CostAllocation_SourceAccount");
                if (!string.IsNullOrEmpty(srcAcc))
                {
                    lueSourceAccount.EditValue = srcAcc;
                }
                else
                {
                    lueSourceAccount.EditValue = "1541";
                }

                string destAcc = UserPreferencesHelper.GetSetting("CostAllocation_DestAccount");
                if (!string.IsNullOrEmpty(destAcc))
                {
                    lueDestAccount.EditValue = destAcc;
                }
                else
                {
                    lueDestAccount.EditValue = "1541";
                }

                // 3. Yếu tố chi phí đích
                string destElem = UserPreferencesHelper.GetSetting("CostAllocation_DestCostElement");
                if (!string.IsNullOrEmpty(destElem))
                {
                    lueDestCostElement.EditValue = destElem;
                }
                else if (lueDestCostElement.ItemsSource is IList<CostElementEntity> elemList && elemList.Count > 0)
                {
                    lueDestCostElement.EditValue = elemList[0].Id;
                }

                // 4. Danh sách tài khoản tiêu thức cấp dưới cùng
                string savedCriteria = UserPreferencesHelper.GetSetting("CostAllocation_CriteriaAccounts");
                _criteriaAccounts.Clear();
                if (!string.IsNullOrWhiteSpace(savedCriteria))
                {
                    var accs = savedCriteria.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var acc in accs)
                    {
                        string trimmed = acc.Trim();
                        if (string.IsNullOrEmpty(trimmed)) continue;

                        // Nếu tài khoản được lưu là tài khoản cha (ví dụ 627), tự động bung ra các tài khoản con cấp dưới cùng (lá)
                        var childLeaves = _leafAccounts.Where(l => l.AccountId.StartsWith(trimmed, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (childLeaves.Count > 1)
                        {
                            foreach (var child in childLeaves)
                            {
                                if (!_criteriaAccounts.Any(x => x.AccountId.Equals(child.AccountId, StringComparison.OrdinalIgnoreCase)))
                                {
                                    _criteriaAccounts.Add(new AccountFilterItem { AccountId = child.AccountId, AccountName = child.AccountName });
                                }
                            }
                        }
                        else if (!_criteriaAccounts.Any(x => x.AccountId.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
                        {
                            string accName = _accountDict.TryGetValue(trimmed, out var name) ? name : trimmed;
                            _criteriaAccounts.Add(new AccountFilterItem { AccountId = trimmed, AccountName = accName });
                        }
                    }
                }

                // Nếu chưa có cấu hình tiêu thức nào được lưu, gán danh sách tài khoản lá mặc định
                if (_criteriaAccounts.Count == 0)
                {
                    SetDefaultCriteria();
                }
            }
            catch { }
        }

        private void SaveFormSettings()
        {
            if (_isInitializing) return;

            try
            {
                if (dtFromDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting("CostAllocation_FromDate", dtFromDate.DateTime.ToString("yyyy-MM-dd"));

                if (dtToDate.DateTime != DateTime.MinValue)
                    UserPreferencesHelper.SetSetting("CostAllocation_ToDate", dtToDate.DateTime.ToString("yyyy-MM-dd"));

                if (lueSourceAccount.EditValue != null)
                    UserPreferencesHelper.SetSetting("CostAllocation_SourceAccount", lueSourceAccount.EditValue.ToString());

                if (lueDestAccount.EditValue != null)
                    UserPreferencesHelper.SetSetting("CostAllocation_DestAccount", lueDestAccount.EditValue.ToString());

                if (lueDestCostElement.EditValue != null)
                    UserPreferencesHelper.SetSetting("CostAllocation_DestCostElement", lueDestCostElement.EditValue.ToString());

                if (_criteriaAccounts != null)
                {
                    string joined = string.Join(";", _criteriaAccounts.Select(x => x.AccountId));
                    UserPreferencesHelper.SetSetting("CostAllocation_CriteriaAccounts", joined);
                }
            }
            catch { }
        }

        private void SetDefaultCriteria()
        {
            _criteriaAccounts.Clear();
            // Tiêu thức mặc định chỉ gồm các tài khoản cấp dưới cùng (lá)
            var defaultCandidates = new[] { "1541", "1542", "1543", "621", "622" };
            foreach (var acc in defaultCandidates)
            {
                if (_leafAccounts.Any(l => l.AccountId.Equals(acc, StringComparison.OrdinalIgnoreCase)))
                {
                    string accName = _accountDict.TryGetValue(acc, out var name) ? name : acc;
                    _criteriaAccounts.Add(new AccountFilterItem { AccountId = acc, AccountName = accName });
                }
            }

            // Tự động thêm các tài khoản chi phí sản xuất chung cấp lá (6271, 6272...)
            var leaf627 = _leafAccounts.Where(l => l.AccountId.StartsWith("627")).ToList();
            foreach (var leaf in leaf627)
            {
                if (!_criteriaAccounts.Any(x => x.AccountId.Equals(leaf.AccountId, StringComparison.OrdinalIgnoreCase)))
                {
                    _criteriaAccounts.Add(new AccountFilterItem { AccountId = leaf.AccountId, AccountName = leaf.AccountName });
                }
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

        private void BtnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            string accId = lueAddCriteriaAccount.EditValue?.ToString()?.Trim();
            if (string.IsNullOrEmpty(accId))
            {
                MessageBox.Show("Vui lòng chọn một tài khoản tiêu thức để thêm.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!_criteriaAccounts.Any(x => x.AccountId.Equals(accId, StringComparison.OrdinalIgnoreCase)))
            {
                string accName = _accountDict.TryGetValue(accId, out var name) ? name : accId;
                _criteriaAccounts.Add(new AccountFilterItem { AccountId = accId, AccountName = accName });
                SaveFormSettings();
                ResetCalculationState();
            }
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

                // Lưu lại các tham số người dùng vừa tính toán
                SaveFormSettings();

                // 1. KIỂM TRA DÒNG TỔNG CHI PHÍ NGUỒN = 0: HẾT CHI PHÍ ĐỂ PHÂN BỔ
                if (Math.Abs(result.TotalPoolAmount) < 0.0001)
                {
                    gridCriteria.ItemsSource = null;
                    btnDistribute.IsEnabled = false;
                    lblTotalPool.Text = "0 đ";
                    lblStatus.Text = "⚠️ Dòng tổng chi phí nguồn bằng 0 (đã kết chuyển hết hoặc không có chi phí) - Không có chi phí để phân bổ!";
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

                // Kiểm tra nghiêm ngặt dòng tổng trước khi thực hiện
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

                // Tính toán lại: số dư chi phí nguồn sẽ tự động trở về 0 và khóa nút Phân bổ
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
