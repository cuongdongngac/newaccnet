using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DataAccess;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using NewaccNet.Reports;
using NewaccNet.Reports.FlattenedDetails;
using NewaccNet.Wpf.Views.Base;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace NewaccNet.Wpf.AppSystem.Reports.Debt
{
    /// <summary>
    /// 6 loại báo cáo công nợ điều hướng từ Form Filter.
    /// </summary>
    public enum DebtReportKind
    {
        /// <summary>1. Báo cáo tổng hợp công nợ (Thuần tài chính)</summary>
        FinancialSummary = 0,
        /// <summary>2. Báo cáo chi tiết công nợ (Thuần tài chính)</summary>
        FinancialDetail = 1,
        /// <summary>3. Báo cáo tổng hợp công nợ theo thời hạn</summary>
        DueSummary = 2,
        /// <summary>4. Báo cáo chi tiết công nợ theo thời hạn</summary>
        DueDetail = 3,
        /// <summary>5. Báo cáo tổng hợp công nợ theo ngoại tệ</summary>
        CurrencySummary = 4,
        /// <summary>6. Báo cáo chi tiết công nợ theo ngoại tệ</summary>
        CurrencyDetail = 5
    }

    /// <summary>
    /// Cơ chế sắp xếp / Gom nhóm 2 cấp trên báo cáo công nợ.
    /// </summary>
    public enum DebtSortMode
    {
        /// <summary>Đối tượng công nợ -> Nội dung công nợ</summary>
        ByPartnerThenDebtType = 0,
        /// <summary>Nội dung công nợ -> Đối tượng công nợ</summary>
        ByDebtTypeThenPartner = 1
    }

    /// <summary>
    /// Dòng chọn trong CheckedComboBox cho Đối tượng công nợ và Nội dung công nợ.
    /// </summary>
    public class SelectableFilterItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DisplayText => string.IsNullOrWhiteSpace(Name) || string.Equals(Code, Name, StringComparison.OrdinalIgnoreCase)
            ? Code
            : $"{Code} - {Name}";
    }

    public class DebtAccountLookupItem
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public bool IsParent { get; set; }
    }

    public class ReportTypeOptionItem
    {
        public DebtReportKind Code { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Form lọc và điều hướng 6 loại Báo cáo Công nợ.
    /// Kế thừa từ BaseReportFilterWindow để tự động nạp & lưu toàn bộ tham số lọc vào UserSettings.json.
    /// </summary>
    public partial class DebtReportFilterWindow : BaseReportFilterWindow
    {
        private List<SelectableFilterItem> _allPartners = new List<SelectableFilterItem>();
        private List<SelectableFilterItem> _allDebtTypes = new List<SelectableFilterItem>();
        private ObservableCollection<DebtAccountLookupItem> _debtAccounts = new ObservableCollection<DebtAccountLookupItem>();
        private List<ChartOfAccountEntity> _allChartOfAccounts = new List<ChartOfAccountEntity>();
        private List<DebtFlatRow> _cachedFlatRows = new List<DebtFlatRow>();

        private List<SelectableFilterItem> _visiblePartners = new List<SelectableFilterItem>();
        private List<SelectableFilterItem> _visibleDebtTypes = new List<SelectableFilterItem>();

        private readonly DebtReportKind? _initialReportKind;
        private bool _isUpdatingLists = false;

        public DebtReportFilterWindow(DebtReportKind? initialReportKind = null)
        {
            InitializeComponent();
            _initialReportKind = initialReportKind;
        }

        // =====================================================================
        // VÒNG ĐỜI KẾ THỪA TỪ BaseReportFilterWindow (NẠP DANH MỤC & USER SETTINGS)
        // =====================================================================

        protected override void InitializeFilterData()
        {
            // Thiết lập giá trị mặc định ban đầu trước khi BaseReportFilterWindow gọi LoadFilterState()
            DateTime today = DateTime.Today;
            dtFromDate.DateTime = new DateTime(today.Year, 1, 1);
            dtToDate.DateTime = today;

            InitReportTypeOptions();
            LoadMasterDataFromDatabase();

            _visiblePartners = _allPartners.ToList();
            _visibleDebtTypes = _allDebtTypes.ToList();
            cboSelectedPartners.ItemsSource = _visiblePartners;
            cboSelectedPartners.SelectAll();
            cboSelectedDebtTypes.ItemsSource = _visibleDebtTypes;
            cboSelectedDebtTypes.SelectAll();
        }

        protected override void OnAfterFilterStateLoaded()
        {
            if (_initialReportKind.HasValue)
            {
                cboReportType.EditValue = _initialReportKind.Value;
            }

            // Đảm bảo nếu tài khoản lưu trước đó không còn trong danh sách thì quay về '*'
            string currentAcc = GetSelectedAccountPrefixOrStar();
            if (!_debtAccounts.Any(a => string.Equals(a.AccountId, currentAcc, StringComparison.OrdinalIgnoreCase)))
            {
                lkAccount.EditValue = "*";
            }

            UpdateAccountLabel();
            UpdateCurrencyControlState();
            RefreshFilterLists();
        }

        private void InitReportTypeOptions()
        {
            var options = new List<ReportTypeOptionItem>
            {
                new ReportTypeOptionItem { Code = DebtReportKind.FinancialSummary, Name = "Báo cáo tổng hợp công nợ" },
                new ReportTypeOptionItem { Code = DebtReportKind.FinancialDetail,  Name = "Báo cáo chi tiết công nợ" },
                new ReportTypeOptionItem { Code = DebtReportKind.DueSummary,       Name = "Báo cáo tổng hợp công nợ theo thời hạn" },
                new ReportTypeOptionItem { Code = DebtReportKind.DueDetail,        Name = "Báo cáo chi tiết công nợ theo thời hạn" },
                new ReportTypeOptionItem { Code = DebtReportKind.CurrencySummary,  Name = "Báo cáo tổng hợp công nợ theo ngoại tệ" },
                new ReportTypeOptionItem { Code = DebtReportKind.CurrencyDetail,   Name = "Báo cáo chi tiết công nợ theo ngoại tệ" }
            };

            cboReportType.ItemsSource = options;
            cboReportType.EditValue = DebtReportKind.FinancialSummary;
        }

        private void LoadMasterDataFromDatabase()
        {
            try
            {
                using var adapter = AppDataAccessAdapter.Create();

                // 1. Nạp danh mục Tài khoản độc lập: Chỉ lấy các TK Công nợ (CategoryId == 'A' hoặc bắt đầu 131, 331, 138, 338, 141)
                //    Không nạp bảng phẳng giao dịch hay JOIN nghiệp vụ ở bước hiển thị danh mục.
                var allAccountsColl = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(allAccountsColl, null);

                _allChartOfAccounts = allAccountsColl
                    .Where(a => !string.IsNullOrWhiteSpace(a.AccountId))
                    .OrderBy(a => a.AccountId.Trim())
                    .ToList();

                // Giống hệt bên DetailTrialBalance: Nạp danh mục và lọc thuần túy theo CategoryId == "A"
                var debtAccountList = new ObservableCollection<DebtAccountLookupItem>
                {
                    new DebtAccountLookupItem
                    {
                        AccountId = "*",
                        AccountName = "Toàn bộ tài khoản công nợ",
                        IsParent = true
                    }
                };

                foreach (var acc in _allChartOfAccounts.Where(a => string.Equals(a.CategoryId?.Trim(), "A", StringComparison.OrdinalIgnoreCase)))
                {
                    debtAccountList.Add(new DebtAccountLookupItem
                    {
                        AccountId = acc.AccountId.Trim(),
                        AccountName = acc.AccountName?.Trim() ?? acc.AccountId.Trim(),
                        ParentId = acc.ParentId?.Trim()
                    });
                }

                _debtAccounts = debtAccountList;
                lkAccount.ItemsSource = _debtAccounts;
                lkAccount.EditValue = "*";

                // 3. Nạp danh mục Đối tượng công nợ (Partner)
                var partnersColl = new EntityCollection<PartnerEntity>();
                adapter.FetchEntityCollection(partnersColl, null);
                _allPartners = partnersColl
                    .Where(p => !string.IsNullOrWhiteSpace(p.Id))
                    .OrderBy(p => p.Id.Trim())
                    .Select(p => new SelectableFilterItem
                    {
                        Code = p.Id.Trim(),
                        Name = string.IsNullOrWhiteSpace(p.CustomerName) ? p.Id.Trim() : p.CustomerName.Trim()
                    })
                    .ToList();

                // 4. Nạp danh mục Nội dung / Loại hình công nợ (DebtType)
                var debtTypesColl = new EntityCollection<DebtTypeEntity>();
                adapter.FetchEntityCollection(debtTypesColl, null);
                _allDebtTypes = debtTypesColl
                    .Where(d => !string.IsNullOrWhiteSpace(d.Id))
                    .OrderBy(d => d.Id.Trim())
                    .Select(d => new SelectableFilterItem
                    {
                        Code = d.Id.Trim(),
                        Name = string.IsNullOrWhiteSpace(d.TypeName) ? d.Id.Trim() : d.TypeName.Trim()
                    })
                    .ToList();

                // 5. Nạp danh mục Ngoại tệ (Currency)
                var currenciesColl = new EntityCollection<CurrencyEntity>();
                adapter.FetchEntityCollection(currenciesColl, null);
                var currencies = currenciesColl
                    .Where(c => !string.IsNullOrWhiteSpace(c.CurrencyId))
                    .OrderBy(c => c.CurrencyId)
                    .ToList();
                lkCurrency.ItemsSource = currencies;
                if (currencies.Count > 0)
                {
                    lkCurrency.EditValue = currencies[0].CurrencyId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Lỗi khi tải danh mục bộ lọc công nợ: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // =====================================================================
        // LỌC DANH SÁCH ĐỐI TƯỢNG & NỘI DUNG TRÊN CHECKED COMBOBOX
        // =====================================================================

        private void RefreshFilterLists()
        {
            if (_isUpdatingLists) return;
            _isUpdatingLists = true;
            try
            {
                bool autoFilter = chkAutoFilter?.IsChecked == true;
                if (autoFilter && _cachedFlatRows.Count == 0)
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    _cachedFlatRows = FlattenedDetailService.LoadDebtFlatRows(adapter);
                }

                var matchingAccountIds = GetMatchingAccountIds(GetSelectedAccountPrefix());
                var sortMode = GetSelectedSortMode();

                var prevPartnerCodes = GetRawCheckedCodes(cboSelectedPartners);
                var prevDebtTypeCodes = GetRawCheckedCodes(cboSelectedDebtTypes);
                bool wasAllPartners = prevPartnerCodes.Count == 0 || (_visiblePartners.Count > 0 && prevPartnerCodes.Count >= _visiblePartners.Count);
                bool wasAllDebtTypes = prevDebtTypeCodes.Count == 0 || (_visibleDebtTypes.Count > 0 && prevDebtTypeCodes.Count >= _visibleDebtTypes.Count);

                IEnumerable<DebtFlatRow> activeRows = _cachedFlatRows;
                if (matchingAccountIds != null)
                {
                    activeRows = activeRows.Where(r => matchingAccountIds.Contains(r.AccountId?.Trim() ?? string.Empty));
                }
                var activeRowList = activeRows.ToList();

                if (!autoFilter)
                {
                    _visiblePartners = _allPartners.ToList();
                    _visibleDebtTypes = _allDebtTypes.ToList();
                }
                else if (sortMode == DebtSortMode.ByPartnerThenDebtType)
                {
                    var partnerSetInAccount = activeRowList
                        .Select(r => r.PartnerId)
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _visiblePartners = _allPartners.Where(p => partnerSetInAccount.Contains(p.Code)).ToList();

                    var effectivePartnerCodes = wasAllPartners
                        ? _visiblePartners.Select(p => p.Code).ToHashSet(StringComparer.OrdinalIgnoreCase)
                        : prevPartnerCodes;

                    var debtTypeSetForSelectedPartners = activeRowList
                        .Where(r => effectivePartnerCodes.Contains(r.PartnerId))
                        .Select(r => r.DebtTypeId)
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _visibleDebtTypes = _allDebtTypes.Where(d => debtTypeSetForSelectedPartners.Contains(d.Code)).ToList();
                }
                else
                {
                    var debtTypeSetInAccount = activeRowList
                        .Select(r => r.DebtTypeId)
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _visibleDebtTypes = _allDebtTypes.Where(d => debtTypeSetInAccount.Contains(d.Code)).ToList();

                    var effectiveDebtTypeCodes = wasAllDebtTypes
                        ? _visibleDebtTypes.Select(d => d.Code).ToHashSet(StringComparer.OrdinalIgnoreCase)
                        : prevDebtTypeCodes;

                    var partnerSetForSelectedDebtTypes = activeRowList
                        .Where(r => effectiveDebtTypeCodes.Contains(r.DebtTypeId))
                        .Select(r => r.PartnerId)
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    _visiblePartners = _allPartners.Where(p => partnerSetForSelectedDebtTypes.Contains(p.Code)).ToList();
                }

                cboSelectedPartners.ItemsSource = _visiblePartners;
                if (wasAllPartners)
                {
                    cboSelectedPartners.SelectAll();
                }
                else
                {
                    RestoreCheckedSelection(cboSelectedPartners, _visiblePartners, prevPartnerCodes);
                }

                cboSelectedDebtTypes.ItemsSource = _visibleDebtTypes;
                if (wasAllDebtTypes)
                {
                    cboSelectedDebtTypes.SelectAll();
                }
                else
                {
                    RestoreCheckedSelection(cboSelectedDebtTypes, _visibleDebtTypes, prevDebtTypeCodes);
                }

                UpdateCounters();
            }
            finally
            {
                _isUpdatingLists = false;
            }
        }

        private static HashSet<string> GetRawCheckedCodes(DevExpress.Xpf.Editors.ComboBoxEdit? combo)
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (combo?.EditValue == null) return result;

            if (combo.EditValue is IEnumerable enumerable and not string)
            {
                foreach (var item in enumerable)
                {
                    string? code = item is SelectableFilterItem sfi ? sfi.Code : item?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(code))
                    {
                        result.Add(code);
                    }
                }
            }
            else
            {
                string val = combo.EditValue.ToString() ?? string.Empty;
                foreach (var part in val.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string trimmed = part.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmed))
                    {
                        result.Add(trimmed);
                    }
                }
            }

            return result;
        }

        private static void RestoreCheckedSelection(
            DevExpress.Xpf.Editors.ComboBoxEdit combo,
            List<SelectableFilterItem> items,
            HashSet<string> selectedCodes)
        {
            var validCodes = items
                .Where(x => selectedCodes.Contains(x.Code))
                .Select(x => (object)x.Code)
                .ToList();

            if (validCodes.Count > 0)
            {
                combo.EditValue = validCodes;
            }
            else
            {
                combo.SelectAll();
            }
        }

        private void UpdateCounters()
        {
            int totalP = _visiblePartners.Count;
            var rawP = GetRawCheckedCodes(cboSelectedPartners);
            int selectedP = (rawP.Count == 0 || rawP.Count >= totalP)
                ? totalP
                : rawP.Count(c => _visiblePartners.Any(p => string.Equals(p.Code, c, StringComparison.OrdinalIgnoreCase)));
            if (lblPartnerSelected != null)
            {
                lblPartnerSelected.Text = (selectedP >= totalP && totalP > 0)
                    ? $"Đã chọn: Tất cả ({totalP})"
                    : $"Đã chọn: {selectedP} / {totalP}";
            }

            int totalD = _visibleDebtTypes.Count;
            var rawD = GetRawCheckedCodes(cboSelectedDebtTypes);
            int selectedD = (rawD.Count == 0 || rawD.Count >= totalD)
                ? totalD
                : rawD.Count(c => _visibleDebtTypes.Any(d => string.Equals(d.Code, c, StringComparison.OrdinalIgnoreCase)));
            if (lblDebtTypeSelected != null)
            {
                lblDebtTypeSelected.Text = (selectedD >= totalD && totalD > 0)
                    ? $"Đã chọn: Tất cả ({totalD})"
                    : $"Đã chọn: {selectedD} / {totalD}";
            }
        }

        // =====================================================================
        // SỰ KIỆN TƯƠNG TÁC TRÊN UI
        // =====================================================================

        private void CboSelectedPartners_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (IsInitializingFilter || _isUpdatingLists) return;
            if (chkAutoFilter?.IsChecked == true && GetSelectedSortMode() == DebtSortMode.ByPartnerThenDebtType)
            {
                RefreshFilterLists();
            }
            else
            {
                UpdateCounters();
            }
        }

        private void CboSelectedDebtTypes_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (IsInitializingFilter || _isUpdatingLists) return;
            if (chkAutoFilter?.IsChecked == true && GetSelectedSortMode() == DebtSortMode.ByDebtTypeThenPartner)
            {
                RefreshFilterLists();
            }
            else
            {
                UpdateCounters();
            }
        }

        private void LkAccount_ProcessNewValue(DependencyObject sender, DevExpress.Xpf.Editors.ProcessNewValueEventArgs e)
        {
            string typed = e.DisplayText?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(typed))
            {
                lkAccount.EditValue = "*";
                e.Handled = true;
                return;
            }

            // 1. Nếu gõ khớp trực tiếp một mã tài khoản (cha hoặc con) trong danh sách _debtAccounts
            var existing = _debtAccounts.FirstOrDefault(a =>
                string.Equals(a.AccountId, typed, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                lkAccount.EditValue = existing.AccountId;
                e.Handled = true;
                return;
            }

            // 2. Nếu gõ một tiền tố tài khoản cha (VD: "131", "13", "331"...) có chứa tài khoản công nợ con
            var childMatches = _debtAccounts
                .Where(a => a.AccountId != "*" && a.AccountId.StartsWith(typed, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (childMatches.Count > 0)
            {
                var accInDb = _allChartOfAccounts.FirstOrDefault(a =>
                    string.Equals(a.AccountId?.Trim(), typed, StringComparison.OrdinalIgnoreCase));

                var newParentItem = new DebtAccountLookupItem
                {
                    AccountId = accInDb?.AccountId?.Trim() ?? typed,
                    AccountName = accInDb?.AccountName?.Trim() ?? $"Tài khoản tổng hợp {typed}",
                    ParentId = accInDb?.ParentId?.Trim(),
                    IsParent = true
                };
                _debtAccounts.Add(newParentItem);
                lkAccount.EditValue = newParentItem.AccountId;
                e.Handled = true;
            }
        }

        private void LkAccount_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            UpdateAccountLabel();
            if (!IsInitializingFilter && chkAutoFilter?.IsChecked == true)
            {
                RefreshFilterLists();
            }
        }

        private void UpdateAccountLabel()
        {
            if (lblAccountName == null || lkAccount == null) return;
            string code = lkAccount.EditValue?.ToString()?.Trim() ?? "*";
            var found = _debtAccounts.FirstOrDefault(a => string.Equals(a.AccountId, code, StringComparison.OrdinalIgnoreCase));
            if (found != null)
            {
                lblAccountName.Text = found.IsParent && found.AccountId != "*"
                    ? $"{found.AccountName} (Bao gồm TK cha & các TK con)"
                    : found.AccountName;
            }
            else
            {
                lblAccountName.Text = $"Tài khoản {code}";
            }
        }

        private void SortRadio_Changed(object sender, RoutedEventArgs e)
        {
            if (IsInitializingFilter) return;
            if (chkAutoFilter?.IsChecked == true)
            {
                RefreshFilterLists();
            }
        }

        private void ChkAutoFilter_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (IsInitializingFilter) return;
            RefreshFilterLists();
        }

        private void CboReportType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            UpdateCurrencyControlState();
        }

        private void UpdateCurrencyControlState()
        {
            if (lkCurrency == null || lblCurrency == null) return;
            var kind = GetSelectedReportKind();
            bool isCurrencyReport = kind == DebtReportKind.CurrencySummary || kind == DebtReportKind.CurrencyDetail;
            lkCurrency.IsEnabled = isCurrencyReport;
            lblCurrency.Foreground = isCurrencyReport
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(13, 71, 161))
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(117, 117, 117));
        }

        // =====================================================================
        // LẤY THAM SỐ LỌC HIỆN TẠI & RẼ NHÁNH 6 LOẠI BÁO CÁO
        // =====================================================================

        public string GetSelectedAccountPrefixOrStar()
        {
            string val = lkAccount?.EditValue?.ToString()?.Trim() ?? "*";
            return string.IsNullOrEmpty(val) ? "*" : val;
        }

        public string? GetSelectedAccountPrefix()
        {
            string val = GetSelectedAccountPrefixOrStar();
            return val == "*" ? null : val;
        }

        /// <summary>
        /// Khi chọn/gõ một tài khoản (đặc biệt là Tài khoản Cha), trả về tập hợp bao gồm
        /// CHÍNH TÀI KHOẢN CHA ĐÓ và TẤT CẢ CÁC TÀI KHOẢN CON (theo cả tiền tố StartsWith lẫn liên kết ParentId).
        /// Trả về null nếu chọn "*" (tất cả tài khoản công nợ).
        /// </summary>
        public HashSet<string>? GetMatchingAccountIds(string? selectedAccount)
        {
            if (string.IsNullOrWhiteSpace(selectedAccount) || selectedAccount == "*")
                return null;

            string root = selectedAccount.Trim();
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { root };

            foreach (var item in _debtAccounts)
            {
                if (item.AccountId != "*" && item.AccountId.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(item.AccountId);
                }
            }
            foreach (var acc in _allChartOfAccounts)
            {
                string id = acc.AccountId.Trim();
                if (id.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(id);
                }
            }

            bool added;
            do
            {
                added = false;
                foreach (var acc in _allChartOfAccounts)
                {
                    string id = acc.AccountId.Trim();
                    string? parentId = acc.ParentId?.Trim();
                    if (!result.Contains(id) && !string.IsNullOrWhiteSpace(parentId) && result.Contains(parentId))
                    {
                        result.Add(id);
                        added = true;
                    }
                }
            } while (added);

            return result;
        }

        public DebtSortMode GetSelectedSortMode()
        {
            return radSortByDebtType?.IsChecked == true
                ? DebtSortMode.ByDebtTypeThenPartner
                : DebtSortMode.ByPartnerThenDebtType;
        }

        public DebtReportKind GetSelectedReportKind()
        {
            if (cboReportType?.EditValue is DebtReportKind kind)
                return kind;
            if (cboReportType?.SelectedItem is ReportTypeOptionItem opt)
                return opt.Code;
            return DebtReportKind.FinancialSummary;
        }

        /// <summary>
        /// Lấy danh sách mã Đối tượng công nợ được chọn (Nếu để trống = lấy tất cả đối tượng đang hiển thị).
        /// </summary>
        public HashSet<string> GetSelectedPartnerIds()
        {
            var raw = GetRawCheckedCodes(cboSelectedPartners);
            if (raw.Count == 0)
            {
                return _visiblePartners.Select(p => p.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            return raw;
        }

        /// <summary>
        /// Lấy danh sách mã Nội dung công nợ được chọn (Nếu để trống = lấy tất cả nội dung đang hiển thị).
        /// </summary>
        public HashSet<string> GetSelectedDebtTypeIds()
        {
            var raw = GetRawCheckedCodes(cboSelectedDebtTypes);
            if (raw.Count == 0)
            {
                return _visibleDebtTypes.Select(d => d.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            return raw;
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime fromDate = dtFromDate.DateTime.Date;
            DateTime toDate = dtToDate.DateTime.Date;
            if (fromDate > toDate)
            {
                MessageBox.Show(this, "'Từ ngày' phải nhỏ hơn hoặc bằng 'Đến ngày'!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var kind = GetSelectedReportKind();
            string? currencyId = null;
            if (kind == DebtReportKind.CurrencySummary || kind == DebtReportKind.CurrencyDetail)
            {
                currencyId = lkCurrency.EditValue?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(currencyId))
                {
                    MessageBox.Show(this, "Vui lòng chọn Loại ngoại tệ cho báo cáo ngoại tệ!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            // Tự động lưu toàn bộ tham số lọc vào UserSettings.json thông qua BaseReportFilterWindow
            SaveFilterState();

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (_cachedFlatRows.Count == 0)
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    _cachedFlatRows = FlattenedDetailService.LoadDebtFlatRows(adapter);
                }

                var selectedPartners = GetSelectedPartnerIds();
                var selectedDebtTypes = GetSelectedDebtTypeIds();
                var matchingAccounts = GetMatchingAccountIds(GetSelectedAccountPrefix());
                var sortOrder = GetSelectedSortMode() == DebtSortMode.ByDebtTypeThenPartner
                    ? DebtReportSortOrder.ByDebtTypeThenPartner
                    : DebtReportSortOrder.ByPartnerThenDebtType;

                // Dựng DebtReportContext giữ tập dữ liệu phẳng đã lọc trên bộ nhớ (phục vụ cả báo cáo chính lẫn Drill-down)
                var context = DebtReportCalculator.BuildContext(
                    rawFlatRows: _cachedFlatRows,
                    fromDate: fromDate,
                    toDate: toDate,
                    matchingAccountIds: matchingAccounts,
                    selectedPartnerIds: selectedPartners,
                    selectedDebtTypeIds: selectedDebtTypes,
                    sortOrder: sortOrder,
                    accountFilterDisplay: GetSelectedAccountPrefixOrStar(),
                    accountNameDisplay: lblAccountName.Text,
                    currencyId: currencyId);

                if (kind == DebtReportKind.FinancialSummary)
                {
                    ShowSummaryDebtReport(context);
                    return;
                }

                if (kind == DebtReportKind.FinancialDetail)
                {
                    ShowDetailDebtReport(context, this);
                    return;
                }

                if (kind == DebtReportKind.DueDetail)
                {
                    ShowDueDetailDebtReport(context, this);
                    return;
                }

                string sortText = sortOrder == DebtReportSortOrder.ByPartnerThenDebtType
                    ? "Đối tượng → Nội dung"
                    : "Nội dung → Đối tượng";
                string reportName = (cboReportType.SelectedItem as ReportTypeOptionItem)?.Name ?? kind.ToString();

                MessageBox.Show(this,
                    $"Đã chuẩn bị ngữ cảnh dữ liệu phẳng ({context.FilteredFlatRows.Count} dòng phát sinh) cho:\n\n" +
                    $"• Loại báo cáo: {reportName}\n" +
                    $"• Tài khoản: {context.AccountFilterDisplay} ({context.AccountNameDisplay})\n" +
                    $"• Kỳ báo cáo: {fromDate:dd/MM/yyyy} → {toDate:dd/MM/yyyy}\n" +
                    $"• Sắp xếp: {sortText}" +
                    (currencyId != null ? $"\n• Ngoại tệ: {currencyId}" : ""),
                    "Điều hướng Báo cáo Công nợ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ShowSummaryDebtReport(DebtReportContext context)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var dtoList = DebtReportCalculator.CalculateSummary(context);
                Mouse.OverrideCursor = null;

                if (dtoList.Count == 0)
                {
                    MessageBox.Show(this,
                        "Không có số dư hoặc phát sinh công nợ nào thỏa mãn điều kiện lọc trong kỳ báo cáo đã chọn.",
                        "Dữ liệu trống",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var report = new SummaryDebtReport
                {
                    DataSource = dtoList,
                    RequestParameters = false
                };

                string accountTitle = context.AccountFilterDisplay == "*"
                    ? $"Tài khoản - {context.AccountNameDisplay}"
                    : $"Tài khoản - {context.AccountFilterDisplay} ({context.AccountNameDisplay})";
                string periodTitle = $"Từ ngày: {context.FromDate:dd/MM/yyyy} Đến ngày: {context.ToDate:dd/MM/yyyy}";

                if (report.Parameters["pAccountTitle"] != null)
                    report.Parameters["pAccountTitle"].Value = accountTitle;
                if (report.Parameters["pPeriodTitle"] != null)
                    report.Parameters["pPeriodTitle"].Value = periodTitle;

                // Gắn Tag = [Group1Id] cho các control trên GroupHeader / Detail để phục vụ click Drill-down xuống báo cáo chi tiết
                AttachGroup1DrillDownTag(report);

                report.CreateDocument(false);

                var previewWindow = new DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = $"Báo cáo tổng hợp công nợ ({context.FromDate:dd/MM/yyyy} - {context.ToDate:dd/MM/yyyy})"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;

                // Sự kiện Drill-down: Click vào nhóm/dòng trên Báo cáo Tổng hợp -> lọc thứ cấp theo Group1Id trên Flat Rows và mở DeptDetailReport
                previewWindow.PreviewControl.DocumentPreviewMouseClick += (sPreview, eClick) =>
                {
                    if (eClick.Brick is DevExpress.XtraPrinting.VisualBrick visualBrick && visualBrick.Value != null)
                    {
                        string clickedGroup1Id = visualBrick.Value.ToString()?.Trim() ?? string.Empty;
                        if (!string.IsNullOrEmpty(clickedGroup1Id))
                        {
                            ShowDetailDebtReport(context, previewWindow, clickedGroup1Id);
                        }
                    }
                };

                previewWindow.Show();
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show(this, "Lỗi khi kết xuất Báo cáo Tổng hợp Công nợ: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowDetailDebtReport(
            DebtReportContext context,
            Window ownerWindow,
            string? drillDownGroup1Id = null)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var detailList = DebtReportCalculator.CalculateDetail(context, drillDownGroup1Id);
                Mouse.OverrideCursor = null;

                if (detailList.Count == 0)
                {
                    string filterNote = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                        ? $" cho mã nhóm '{drillDownGroup1Id}'"
                        : string.Empty;
                    MessageBox.Show(ownerWindow,
                        $"Không có chứng từ phát sinh công nợ nào trong kỳ ({context.FromDate:dd/MM/yyyy} - {context.ToDate:dd/MM/yyyy}){filterNote}.",
                        "Dữ liệu trống",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var detailReport = new DeptDetailReport
                {
                    DataSource = detailList,
                    RequestParameters = false
                };

                string accountTitle = context.AccountFilterDisplay == "*"
                    ? $"Tài khoản - {context.AccountNameDisplay}"
                    : $"Tài khoản - {context.AccountFilterDisplay} ({context.AccountNameDisplay})";
                string periodTitle = $"Từ ngày: {context.FromDate:dd/MM/yyyy} Đến ngày: {context.ToDate:dd/MM/yyyy}";

                if (detailReport.Parameters["pAccountTitle"] != null)
                    detailReport.Parameters["pAccountTitle"].Value = accountTitle;
                if (detailReport.Parameters["pPeriodTitle"] != null)
                    detailReport.Parameters["pPeriodTitle"].Value = periodTitle;

                detailReport.CreateDocument(false);

                string titleSuffix = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                    ? $" - Nhóm: {drillDownGroup1Id}"
                    : string.Empty;

                var detailPreviewWindow = new DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Báo cáo chi tiết công nợ ({context.FromDate:dd/MM/yyyy} - {context.ToDate:dd/MM/yyyy}){titleSuffix}"
                };
                detailPreviewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                detailPreviewWindow.PreviewControl.DocumentSource = detailReport;
                detailPreviewWindow.Show();
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, "Lỗi khi kết xuất Báo cáo Chi tiết Công nợ: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowDueDetailDebtReport(
            DebtReportContext context,
            Window ownerWindow,
            string? drillDownGroup1Id = null)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // 1. Tìm các Khế ước / Hạn nợ có Hạn thanh toán (EndDate) nằm trong khoảng [FromDate .. ToDate]
                using var adapter = AppDataAccessAdapter.Create();
                var dues = new EntityCollection<LiabilityDueEntity>();
                var filter = new RelationPredicateBucket();
                filter.PredicateExpression.Add(LiabilityDueFields.EndDate >= context.FromDate.Date);
                filter.PredicateExpression.AddWithAnd(LiabilityDueFields.EndDate <= context.ToDate.Date);

                if (!string.IsNullOrWhiteSpace(drillDownGroup1Id))
                {
                    if (context.SortOrder == DebtReportSortOrder.ByPartnerThenDebtType)
                    {
                        filter.PredicateExpression.AddWithAnd(LiabilityDueFields.CustomerId == drillDownGroup1Id.Trim());
                    }
                    else
                    {
                        filter.PredicateExpression.AddWithAnd(LiabilityDueFields.DebtTypeId == drillDownGroup1Id.Trim());
                    }
                }

                adapter.FetchEntityCollection(dues, filter);

                if (dues.Count == 0)
                {
                    Mouse.OverrideCursor = null;
                    string filterNote = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                        ? $" cho nhóm '{drillDownGroup1Id}'"
                        : string.Empty;
                    MessageBox.Show(ownerWindow,
                        $"Không có khế ước/khoản nợ nào đến hạn thanh toán trong khoảng thời gian từ {context.FromDate:dd/MM/yyyy} đến {context.ToDate:dd/MM/yyyy}{filterNote}.",
                        "Dữ liệu trống",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var dueMap = dues.ToDictionary(d => d.Id);

                // 2. Tính toán danh sách DTO theo cấu trúc nhóm Khế ước (Group 2 = Khế ước)
                // Lấy toàn bộ lịch sử phát sinh từ trước đến nay của các khế ước đến hạn này (lũy kế đến ToDate)
                var dueList = DebtReportCalculator.CalculateDueDetail(context, dueMap, drillDownGroup1Id);
                Mouse.OverrideCursor = null;

                if (dueList.Count == 0)
                {
                    string filterNote = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                        ? $" cho mã nhóm '{drillDownGroup1Id}'"
                        : string.Empty;
                    MessageBox.Show(ownerWindow,
                        $"Tìm thấy {dues.Count} khế ước/hạn nợ đến hạn trong kỳ, nhưng không có phát sinh chứng từ nào trên tài khoản {context.AccountFilterDisplay} ({context.AccountNameDisplay}){filterNote}.\n\nVui lòng kiểm tra lại tài khoản hạch toán (Ví dụ: đối tượng công nhân/nhà thầu thường hạch toán trên tài khoản 331 thay vì 131).",
                        "Không có phát sinh trên tài khoản đã chọn",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var dueReport = new DebtDueReport
                {
                    DataSource = dueList,
                    RequestParameters = false
                };

                string accountTitle = context.AccountFilterDisplay == "*"
                    ? $"Tài khoản - {context.AccountNameDisplay}"
                    : $"Tài khoản - {context.AccountFilterDisplay} ({context.AccountNameDisplay})";
                string periodTitle = $"Hạn thanh toán từ ngày: {context.FromDate:dd/MM/yyyy} đến ngày: {context.ToDate:dd/MM/yyyy}";

                if (dueReport.Parameters["pAccountTitle"] != null)
                    dueReport.Parameters["pAccountTitle"].Value = accountTitle;
                if (dueReport.Parameters["pPeriodTitle"] != null)
                    dueReport.Parameters["pPeriodTitle"].Value = periodTitle;

                dueReport.CreateDocument(false);

                string titleSuffix = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                    ? $" - Nhóm: {drillDownGroup1Id}"
                    : string.Empty;

                var duePreviewWindow = new DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Báo cáo chi tiết công nợ theo thời hạn ({context.FromDate:dd/MM/yyyy} - {context.ToDate:dd/MM/yyyy}){titleSuffix}"
                };
                duePreviewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                duePreviewWindow.PreviewControl.DocumentSource = dueReport;
                duePreviewWindow.Show();
            }
            catch (Exception ex)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, "Lỗi khi kết xuất Báo cáo Chi tiết Công nợ Theo Thời Hạn: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void AttachGroup1DrillDownTag(XtraReport report)
        {
            foreach (XRControl control in report.AllControls<XRControl>())
            {
                if (control is XRLabel || control is XRTableCell)
                {
                    if (control.Band is DetailBand ||
                        (control.Band is GroupHeaderBand gh && gh.GroupFields.Count > 0))
                    {
                        control.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Tag", "[Group1Id]"));
                    }
                }
            }
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this,
                "Chức năng Kiểm tra đối chiếu công nợ sẽ được kết nối ở bước tiếp theo.",
                "Kiểm tra công nợ",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnInterest_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this,
                "Chức năng Tính lãi công nợ sẽ được kết nối ở bước tiếp theo.",
                "Tính lãi công nợ",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
