using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DataAccess.EntityClasses;
using DataAccess.FactoryClasses;
using NewaccNet.Reports;
using NewaccNet.Reports.FlattenedDetails;
using NewaccNet.Wpf.AppSystem.Helpers;
using NewaccNet.Wpf.AppSystem.Reports;
using NewaccNet.Wpf.AppSystem.Reports.Debt;
using NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Cost;
using NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Currency;
using NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Asset;
using NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance.Reports.Material;
using NewaccNet.Wpf.Views.Base;
using DevExpress.XtraReports.UI;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace NewaccNet.Wpf.AppSystem.Reports.DetailTrialBalance
{
    public class AccountCategoryItem
    {
        public string Code { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public override string ToString() => DisplayName;
    }

    /// <summary>
    /// Đối tượng tài khoản riêng biệt để hiển thị danh sách tài khoản (LookUp),
    /// độc lập hoàn toàn, không join bảng ngoài.
    /// </summary>
    public class AccountItem
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? CategoryId { get; set; }
        public string? ParentId { get; set; }
        public string CategoryCode { get; set; } = "GENERAL";

        public override string ToString() => $"{AccountId} - {AccountName}";
    }

    public partial class DetailTrialBalanceFilterWindow : BaseWindow
    {
        private readonly Dictionary<string, IDetailFilterContainer> _containers = new();
        private readonly Dictionary<string, Func<IDetailFilterContainer>> _containerFactories = new();
        private List<AccountItem> _accounts = new();

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public string SelectedAccountId { get; private set; }
        public bool AutoFilter { get; private set; }
        public Dictionary<string, List<object>> SelectedCriteria { get; private set; } = new();

        private readonly string? _initialAccountPrefix;
        private readonly string? _initialCategory;
        private bool _isFilteringCategory = false;

        public DetailTrialBalanceFilterWindow(string? initialAccountPrefix = null, string? initialCategory = null)
        {
            _initialAccountPrefix = initialAccountPrefix;
            _initialCategory = initialCategory;
            InitializeComponent();
            InitContainers();
        }

        private void InitContainers()
        {
            // Sử dụng Lazy Loading để không nạp dữ liệu của tất cả phân hệ ngay từ đầu
            _containerFactories["MATERIAL"] = () => new InventoryFilterContainer();
            _containerFactories["DEBT"] = () => new DebtFilterContainer();
            _containerFactories["COST"] = () => new CostFilterContainer();
            _containerFactories["CURRENCY"] = () => new CurrencyFilterContainer();
            _containerFactories["STOCK"] = () => new StockFilterContainer();
            _containerFactories["ASSET"] = () => new AssetFilterContainer();
            _containerFactories["GENERAL"] = () => new NormalAccountFilterContainer();
        }

        private IDetailFilterContainer GetOrCreateContainer(string category)
        {
            if (!_containers.TryGetValue(category, out var container))
            {
                if (_containerFactories.TryGetValue(category, out var factory))
                {
                    container = factory();
                    _containers[category] = container;
                }
                else
                {
                    container = GetOrCreateContainer("GENERAL");
                }
            }
            return container;
        }

        private void InitCategories()
        {
            var categories = new List<AccountCategoryItem>
            {
                new AccountCategoryItem { Code = "ALL", DisplayName = "--- Tất cả tài khoản ---" },
                new AccountCategoryItem { Code = "MATERIAL", DisplayName = "Vật tư, hàng hóa (152, 153, 155, 156...)" },
                new AccountCategoryItem { Code = "DEBT", DisplayName = "Công nợ (131, 331, 138, 338, 141...)" },
                new AccountCategoryItem { Code = "COST", DisplayName = "Chi phí, giá thành (154, 621, 622, 627...)" },
                new AccountCategoryItem { Code = "CURRENCY", DisplayName = "Ngoại tệ (1112, 1122...)" },
                new AccountCategoryItem { Code = "STOCK", DisplayName = "Cổ phiếu, đầu tư (121, 221, 228...)" },
                new AccountCategoryItem { Code = "ASSET", DisplayName = "Tài sản cố định (211, 213, 214...)" },
                new AccountCategoryItem { Code = "GENERAL", DisplayName = "Sổ cái tài khoản thông thường" }
            };
            cboCategory.ItemsSource = categories;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;
            dtFrom.DateTime = new DateTime(today.Year, today.Month, 1);
            dtTo.DateTime = today;

            InitCategories();
            await LoadAccountsAsync();
            UserPreferencesHelper.LoadState(this);

            string targetCategory = "ALL";
            if (!string.IsNullOrWhiteSpace(_initialCategory))
            {
                targetCategory = _initialCategory;
            }
            else if (!string.IsNullOrWhiteSpace(_initialAccountPrefix))
            {
                string detected = DetectAccountCategory(_initialAccountPrefix);
                if (detected != "GENERAL")
                {
                    targetCategory = detected;
                }
            }

            cboCategory.EditValue = targetCategory;
            ApplyCategoryFilter(targetCategory, _initialAccountPrefix);
        }

        private void ApplyCategoryFilter(string categoryCode, string? preferredPrefix = null)
        {
            _isFilteringCategory = true;
            try
            {
                // Lọc 100% In-Memory cực nhanh, không truy vấn database
                List<AccountItem> filtered;
                if (string.IsNullOrEmpty(categoryCode) || categoryCode == "ALL")
                {
                    filtered = _accounts;
                }
                else
                {
                    filtered = _accounts.Where(a => a.CategoryCode == categoryCode).ToList();
                }

                lkAccount.ItemsSource = filtered;

                string currentAcc = lkAccount.EditValue?.ToString() ?? string.Empty;
                AccountItem? targetAcc = null;

                if (!string.IsNullOrWhiteSpace(preferredPrefix))
                {
                    targetAcc = filtered.FirstOrDefault(a => a.AccountId.StartsWith(preferredPrefix.Trim(), StringComparison.OrdinalIgnoreCase));
                }

                if (targetAcc == null && !string.IsNullOrEmpty(currentAcc))
                {
                    targetAcc = filtered.FirstOrDefault(a => a.AccountId == currentAcc);
                }

                if (targetAcc == null && filtered.Count > 0)
                {
                    targetAcc = filtered[0];
                }

                if (targetAcc != null)
                {
                    lkAccount.EditValue = targetAcc.AccountId;
                    SwitchContainerByAccount(targetAcc.AccountId);
                }
                else
                {
                    lkAccount.EditValue = null;
                    SwitchContainerByAccount(null);
                }
            }
            finally
            {
                _isFilteringCategory = false;
            }
        }

        private void CboCategory_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (_isFilteringCategory) return;
            string catCode = e.NewValue?.ToString() ?? "ALL";
            ApplyCategoryFilter(catCode);
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                // Nạp bất đồng bộ để UI không bị khựng: truy vấn xong là giải phóng adapter ngay!
                var rawAccounts = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var qf = new QueryFactory();
                    return adapter.FetchQuery(qf.ChartOfAccount)
                        .Cast<ChartOfAccountEntity>()
                        .OrderBy(a => a.AccountId)
                        .ToList();
                });

                _accounts = rawAccounts.Select(a => new AccountItem
                {
                    AccountId = a.AccountId ?? string.Empty,
                    AccountName = a.AccountName ?? string.Empty,
                    CategoryId = a.CategoryId,
                    ParentId = a.ParentId,
                    CategoryCode = ClassifyCategory(a.AccountId ?? string.Empty, a.CategoryId)
                }).ToList();

                lkAccount.ItemsSource = _accounts;
            }
            catch (Exception)
            {
                _accounts = new List<AccountItem>();
            }
        }

        private void LkAccount_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            var accId = e.NewValue?.ToString();
            SwitchContainerByAccount(accId);
        }

        private void SwitchContainerByAccount(string accId)
        {
            if (string.IsNullOrWhiteSpace(accId))
            {
                txtTitle.Text = "Sổ cái cho tài khoản";
                var defaultContainer = GetOrCreateContainer("GENERAL");
                filterContainerPresenter.Content = defaultContainer;
                lblCategoryIndicator.Text = "Đối tượng theo dõi: (Chưa chọn tài khoản)";
                return;
            }

            txtTitle.Text = $"Sổ cái cho tài khoản {accId}";

            string category = DetectAccountCategory(accId);
            var container = GetOrCreateContainer(category);

            filterContainerPresenter.Content = container;
            lblCategoryIndicator.Text = $"Đối tượng theo dõi: {container.CategoryTitle}";
        }

        /// <summary>
        /// Phân loại nhóm tài khoản thuần túy In-Memory, tuyệt đối không truy vấn DB
        /// </summary>
        public static string ClassifyCategory(string accId, string? catId)
        {
            if (string.IsNullOrWhiteSpace(accId)) return "GENERAL";

            string cat = catId?.Trim() ?? string.Empty;
            string id = accId.Trim();

            if (string.Equals(cat, "A", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("131") || id.StartsWith("331") || id.StartsWith("138") || id.StartsWith("338") || id.StartsWith("141"))
            {
                return "DEBT";
            }
            if (string.Equals(cat, "B", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("152") || id.StartsWith("153") || id.StartsWith("155") || id.StartsWith("156") || id.StartsWith("157") || id.StartsWith("158"))
            {
                return "MATERIAL";
            }
            if (string.Equals(cat, "C", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("1112") || id.StartsWith("1122"))
            {
                return "CURRENCY";
            }
            if (string.Equals(cat, "D", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("211") || id.StartsWith("213") || id.StartsWith("214") || id.StartsWith("242"))
            {
                return "ASSET";
            }
            if (string.Equals(cat, "E", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("6") || id.StartsWith("154"))
            {
                return "COST";
            }
            if (string.Equals(cat, "S", StringComparison.OrdinalIgnoreCase)
                || id.StartsWith("121") || id.StartsWith("221") || id.StartsWith("222") || id.StartsWith("228"))
            {
                return "STOCK";
            }

            return "GENERAL";
        }

        private string DetectAccountCategory(string accId)
        {
            if (string.IsNullOrWhiteSpace(accId)) return "GENERAL";
            var match = _accounts.FirstOrDefault(a => a.AccountId == accId);
            if (match != null) return match.CategoryCode;
            return ClassifyCategory(accId, null);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (lkAccount.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xem sổ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                lkAccount.Focus();
                return;
            }

            if (dtFrom.EditValue == null || dtTo.EditValue == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian Từ ngày - Đến ngày!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dtFrom.DateTime > dtTo.DateTime)
            {
                MessageBox.Show("Từ ngày không được lớn hơn Đến ngày!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedAccountId = lkAccount.EditValue.ToString();
            FromDate = dtFrom.DateTime.Date;
            ToDate = dtTo.DateTime.Date;
            AutoFilter = chkAutoFilter.IsChecked == true;

            if (filterContainerPresenter.Content is IDetailFilterContainer activeContainer)
            {
                SelectedCriteria = activeContainer.GetSelectedCriteria();
            }

            UserPreferencesHelper.SaveState(this);

            string category = DetectAccountCategory(SelectedAccountId);

            // Bật WaitIndicator & khóa nút bấm để tránh người dùng click nhiều lần
            btnOK.IsEnabled = false;
            btnCancel.IsEnabled = false;
            waitIndicator.DeferedVisibility = true;

            try
            {
                if (category == "MATERIAL")
                {
                    await ShowMaterialReportAsync();
                    return;
                }

                if (category == "COST")
                {
                    await ShowCostReportAsync();
                    return;
                }

                if (category == "DEBT")
                {
                    await ShowDebtReportAsync();
                    return;
                }

                if (category == "CURRENCY")
                {
                    await ShowCurrencyReportAsync();
                    return;
                }

                if (category == "ASSET")
                {
                    await ShowAssetReportAsync();
                    return;
                }

                // Với các phân hệ khác (Công nợ, Chi phí, Ngoại tệ, Cổ phiếu, TSCĐ)
                var criteriaSummary = string.Join(", ", SelectedCriteria.Select(kv => $"{kv.Key}: {(kv.Value.Count == 0 ? "Tất cả (*)" : string.Join(";", kv.Value))}"));
                MessageBox.Show(
                    $"Đã tiếp nhận tham số xem sổ cho nhóm {category}:\n" +
                    $"- Tài khoản: {SelectedAccountId}\n" +
                    $"- Thời gian: {FromDate:dd/MM/yyyy} -> {ToDate:dd/MM/yyyy}\n" +
                    $"- Chi tiết: {criteriaSummary}\n" +
                    $"- Tự động lọc: {AutoFilter}\n\n" +
                    $"(Đang hoàn thiện hiển thị cho nhóm {category}.)",
                    "Thông số báo cáo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            finally
            {
                waitIndicator.DeferedVisibility = false;
                btnOK.IsEnabled = true;
                btnCancel.IsEnabled = true;
            }
        }

        private async Task ShowMaterialReportAsync()
        {
            try
            {
                waitIndicator.Content = "Đang tổng hợp số liệu Nhập - Xuất - Tồn...";

                List<string>? whIds = null;
                if (SelectedCriteria.TryGetValue("WarehouseId", out var rawWh) && rawWh.Count > 0)
                {
                    whIds = rawWh.Select(x => x.ToString()!).ToList();
                }

                List<string>? matIds = null;
                if (SelectedCriteria.TryGetValue("MaterialId", out var rawMat) && rawMat.Count > 0)
                {
                    matIds = rawMat.Select(x => x.ToString()!).ToList();
                }

                // Chạy trên Background Task: Mở adapter -> tính toán/join -> ngắt kết nối ngay khi xong!
                var dtoList = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    return MaterialReportCalculator.CalculateSummary(
                        adapter,
                        accountPrefix: SelectedAccountId,
                        fromDate: FromDate,
                        toDate: ToDate,
                        warehouseIds: whIds,
                        materialIds: matIds,
                        onlyBooked: true);
                });

                if (dtoList == null || dtoList.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu tồn kho hoặc phát sinh nhập/xuất nào thỏa mãn điều kiện lọc trong kỳ!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var accObj = _accounts.FirstOrDefault(a => a.AccountId == SelectedAccountId);
                string accName = accObj?.AccountName ?? "";

                string companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                }

                var distinctWarehouses = dtoList.Select(d => d.WarehouseName).Where(w => !string.IsNullOrEmpty(w)).Distinct().ToList();
                string? singleWhName = distinctWarehouses.Count == 1 ? distinctWarehouses[0] : null;

                var report = new MaterialSummaryReport
                {
                    DataSource = dtoList,
                    RequestParameters = false
                };
                report.SetReportHeader(SelectedAccountId, accName, FromDate, ToDate, companyName, singleWhName);
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = $"Bảng tổng hợp Nhập - Xuất - Tồn ({FromDate:dd/MM/yyyy} - {ToDate:dd/MM/yyyy})"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;

                // DRILL-DOWN CẤP 1: Click vào bất kỳ dòng vật tư nào -> Mở Sổ chi tiết vật tư (Thẻ kho)
                ReportDrillDownHelper.WireDrillDownKeys(previewWindow, (whId, matId) =>
                {
                    var clickedItem = dtoList.FirstOrDefault(d =>
                        string.Equals(d.WarehouseId, whId, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(d.MaterialId, matId, StringComparison.OrdinalIgnoreCase));

                    if (clickedItem == null && !string.IsNullOrEmpty(matId))
                    {
                        clickedItem = dtoList.FirstOrDefault(d => string.Equals(d.MaterialId, matId, StringComparison.OrdinalIgnoreCase));
                    }

                    if (clickedItem != null)
                    {
                        ShowMaterialDetailReport(clickedItem, accName, companyName, previewWindow);
                    }
                });

                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lập báo cáo vật tư: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Mở Sổ chi tiết Vật tư (Thẻ kho) theo cặp (Kho hàng, Vật tư) đã chọn từ Drill-down.
        /// </summary>
        private void ShowMaterialDetailReport(
            MaterialSummaryReportDTO summaryItem,
            string accName,
            string companyName,
            Window ownerWindow)
        {
            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var detailDtoList = MaterialReportCalculator.CalculateDetailFromSummary(summaryItem);
                System.Windows.Input.Mouse.OverrideCursor = null;

                if (detailDtoList == null || detailDtoList.Count == 0)
                {
                    MessageBox.Show(ownerWindow, "Không có dữ liệu chi tiết cho vật tư này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var detailReport = new MaterialDetailReport
                {
                    DataSource = detailDtoList,
                    RequestParameters = false
                };

                detailReport.SetReportHeader(
                    SelectedAccountId,
                    accName,
                    summaryItem.WarehouseId,
                    summaryItem.WarehouseName,
                    summaryItem.MaterialId,
                    summaryItem.MaterialName,
                    summaryItem.Unit,
                    FromDate,
                    ToDate,
                    companyName);

                detailReport.CreateDocument(false);

                var detailWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Sổ chi tiết vật tư: {summaryItem.MaterialName} ({summaryItem.WarehouseName}) - {FromDate:dd/MM/yyyy} -> {ToDate:dd/MM/yyyy}"
                };
                detailWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                detailWindow.PreviewControl.DocumentSource = detailReport;

                // DRILL-DOWN CẤP 2: Click vào dòng chứng từ chi tiết mở VoucherEntryWindow!
                ReportDrillDownHelper.WireVoucherClick(detailWindow);

                detailWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, $"Lỗi khi mở Sổ chi tiết vật tư: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ShowCostReportAsync()
        {
            try
            {
                waitIndicator.Content = "Đang tổng hợp số liệu chi phí, giá thành...";

                List<string>? costObjIds = null;
                if (SelectedCriteria.TryGetValue("CostObjectId", out var rawObj) && rawObj.Count > 0)
                {
                    costObjIds = rawObj.Select(x => x.ToString()!).ToList();
                }

                List<string>? costElemIds = null;
                if (SelectedCriteria.TryGetValue("CostElementId", out var rawElem) && rawElem.Count > 0)
                {
                    costElemIds = rawElem.Select(x => x.ToString()!).ToList();
                }

                // Chạy trên Background Task: Mở adapter -> tính toán/join -> ngắt kết nối ngay khi xong!
                var dtoList = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    return CostReportCalculator.CalculateSummary(
                        adapter,
                        accountPrefix: SelectedAccountId,
                        fromDate: FromDate,
                        toDate: ToDate,
                        costObjectIds: costObjIds,
                        costElementIds: costElemIds,
                        onlyBooked: true);
                });

                if (dtoList == null || dtoList.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu chi phí nào thỏa mãn điều kiện lọc trong kỳ!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var accObj = _accounts.FirstOrDefault(a => a.AccountId == SelectedAccountId);
                string accName = accObj?.AccountName ?? "";

                string companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                }

                var report = new CostSummaryReport
                {
                    DataSource = dtoList,
                    RequestParameters = false
                };
                report.SetReportHeader(SelectedAccountId, accName, FromDate, ToDate, companyName);
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = $"Báo cáo tổng hợp chi phí TK {SelectedAccountId} ({FromDate:dd/MM/yyyy} - {ToDate:dd/MM/yyyy})"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;

                // DRILL-DOWN CẤP 1: Click vào bất kỳ dòng chi phí nào -> Mở Sổ chi tiết chi phí
                ReportDrillDownHelper.WireDrillDownKeys(previewWindow, (costObjId, costElemId) =>
                {
                    if (string.IsNullOrEmpty(costElemId))
                    {
                        var objItems = dtoList.Where(d => string.Equals(d.CostObjectId, costObjId, StringComparison.OrdinalIgnoreCase)).ToList();
                        if (objItems.Count > 0)
                        {
                            var allDetails = objItems.SelectMany(d => d.Details).ToList();
                            var summaryObj = new CostReportDTO
                            {
                                CostObjectId = costObjId,
                                CostObjectName = objItems[0].CostObjectName,
                                CostElementId = string.Empty,
                                CostElementName = string.Empty,
                                Details = allDetails
                            };
                            ShowCostDetailReport(summaryObj, accName, companyName, previewWindow);
                        }
                    }
                    else
                    {
                        var clickedItem = dtoList.FirstOrDefault(d =>
                            string.Equals(d.CostObjectId, costObjId, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(d.CostElementId, costElemId, StringComparison.OrdinalIgnoreCase));

                        if (clickedItem != null && clickedItem.Details.Count > 0)
                        {
                            ShowCostDetailReport(clickedItem, accName, companyName, previewWindow);
                        }
                    }
                });

                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lập báo cáo chi phí: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowCostDetailReport(
            CostReportDTO summaryItem,
            string accName,
            string companyName,
            Window ownerWindow)
        {
            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var detailDtoList = CostReportCalculator.CalculateDetailFromSummary(summaryItem);
                System.Windows.Input.Mouse.OverrideCursor = null;

                if (detailDtoList == null || detailDtoList.Count == 0)
                {
                    MessageBox.Show(ownerWindow, "Không có dữ liệu chi tiết cho đối tượng chi phí này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var detailReport = new CostDetailReport
                {
                    DataSource = detailDtoList,
                    RequestParameters = false
                };

                detailReport.SetReportHeader(
                    SelectedAccountId,
                    accName,
                    summaryItem.CostObjectId,
                    summaryItem.CostObjectName,
                    summaryItem.CostElementId,
                    summaryItem.CostElementName,
                    FromDate,
                    ToDate,
                    companyName);

                detailReport.CreateDocument(false);

                var detailWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Sổ chi tiết chi phí: {summaryItem.CostObjectName} - TK {SelectedAccountId} ({FromDate:dd/MM/yyyy} -> {ToDate:dd/MM/yyyy})"
                };
                detailWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                detailWindow.PreviewControl.DocumentSource = detailReport;

                // DRILL-DOWN CẤP 2: Click vào dòng chứng từ chi tiết mở VoucherEntryWindow!
                ReportDrillDownHelper.WireVoucherClick(detailWindow);

                detailWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, $"Lỗi khi mở Sổ chi tiết chi phí: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ShowDebtReportAsync()
        {
            try
            {
                waitIndicator.Content = "Đang tổng hợp số liệu công nợ...";

                HashSet<string>? partnerIds = null;
                if (SelectedCriteria.TryGetValue("PartnerId", out var rawPartner) && rawPartner.Count > 0)
                {
                    partnerIds = rawPartner.Select(x => x.ToString()!).ToHashSet(StringComparer.OrdinalIgnoreCase);
                }

                HashSet<string>? debtTypeIds = null;
                if (SelectedCriteria.TryGetValue("DebtReasonId", out var rawDebtType) && rawDebtType.Count > 0)
                {
                    debtTypeIds = rawDebtType.Select(x => x.ToString()!).ToHashSet(StringComparer.OrdinalIgnoreCase);
                }

                var accObj = _accounts.FirstOrDefault(a => a.AccountId == SelectedAccountId);
                string accName = accObj?.AccountName ?? "";

                var matchingAccounts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { SelectedAccountId };
                foreach (var a in _accounts)
                {
                    string id = a.AccountId.Trim();
                    if (id.StartsWith(SelectedAccountId, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingAccounts.Add(id);
                    }
                }
                bool added;
                do
                {
                    added = false;
                    foreach (var a in _accounts)
                    {
                        string id = a.AccountId.Trim();
                        string? parentId = a.ParentId?.Trim();
                        if (!matchingAccounts.Contains(id) && !string.IsNullOrWhiteSpace(parentId) && matchingAccounts.Contains(parentId))
                        {
                            matchingAccounts.Add(id);
                            added = true;
                        }
                    }
                } while (added);

                // Chạy trên Background Task: Mở adapter -> tính toán/join -> ngắt kết nối ngay khi xong!
                var (dtoList, context) = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    var flatRows = FlattenedDetailService.LoadDebtFlatRows(
                        adapter,
                        accountPrefix: SelectedAccountId,
                        toDate: ToDate);

                    var ctx = DebtReportCalculator.BuildContext(
                        rawFlatRows: flatRows,
                        fromDate: FromDate,
                        toDate: ToDate,
                        matchingAccountIds: matchingAccounts,
                        selectedPartnerIds: partnerIds,
                        selectedDebtTypeIds: debtTypeIds,
                        sortOrder: DebtReportSortOrder.ByPartnerThenDebtType,
                        accountFilterDisplay: SelectedAccountId,
                        accountNameDisplay: accName);

                    var list = DebtReportCalculator.CalculateSummary(ctx);
                    return (list, ctx);
                });

                if (dtoList == null || dtoList.Count == 0)
                {
                    MessageBox.Show("Không có số dư hoặc phát sinh công nợ nào thỏa mãn điều kiện lọc trong kỳ!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                }

                var report = new SummaryDebtReport
                {
                    DataSource = dtoList,
                    RequestParameters = false
                };

                string accountTitle = $"Tài khoản - {context.AccountFilterDisplay} ({context.AccountNameDisplay})";
                string periodTitle = $"Từ ngày: {context.FromDate:dd/MM/yyyy} Đến ngày: {context.ToDate:dd/MM/yyyy}";

                if (report.Parameters["pCompanyName"] != null)
                    report.Parameters["pCompanyName"].Value = companyName;
                if (report.Parameters["pAccountTitle"] != null)
                    report.Parameters["pAccountTitle"].Value = accountTitle;
                if (report.Parameters["pPeriodTitle"] != null)
                    report.Parameters["pPeriodTitle"].Value = periodTitle;

                // Gắn Tag Drill-down [Group1Id] lên các ô dòng chi tiết & nhóm đối tượng
                AttachDebtDrillDownTag(report);

                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = $"Báo cáo tổng hợp công nợ TK {SelectedAccountId} ({FromDate:dd/MM/yyyy} - {ToDate:dd/MM/yyyy})"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;

                // DRILL-DOWN CẤP 1: Click vào đối tượng/dòng trên Báo cáo tổng hợp -> Mở Báo cáo chi tiết công nợ của đối tượng đó
                ReportDrillDownHelper.WireDrillDownKeys(previewWindow, (group1Id, _) =>
                {
                    if (!string.IsNullOrEmpty(group1Id))
                    {
                        ShowDetailDebtReport(context, group1Id, previewWindow);
                    }
                });

                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lập báo cáo công nợ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowDetailDebtReport(
            DebtReportContext context,
            string? drillDownGroup1Id,
            Window ownerWindow)
        {
            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var detailList = DebtReportCalculator.CalculateDetail(context, drillDownGroup1Id);
                System.Windows.Input.Mouse.OverrideCursor = null;

                if (detailList == null || detailList.Count == 0)
                {
                    string filterNote = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                        ? $" cho đối tượng '{drillDownGroup1Id}'"
                        : string.Empty;
                    MessageBox.Show(ownerWindow,
                        $"Không có chứng từ phát sinh công nợ nào trong kỳ ({context.FromDate:dd/MM/yyyy} - {context.ToDate:dd/MM/yyyy}){filterNote}.",
                        "Dữ liệu trống",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                string companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                }

                var detailReport = new DeptDetailReport
                {
                    DataSource = detailList,
                    RequestParameters = false
                };

                string accountTitle = $"Tài khoản - {context.AccountFilterDisplay} ({context.AccountNameDisplay})";
                string periodTitle = $"Từ ngày: {context.FromDate:dd/MM/yyyy} Đến ngày: {context.ToDate:dd/MM/yyyy}";

                if (detailReport.Parameters["pCompanyName"] != null)
                    detailReport.Parameters["pCompanyName"].Value = companyName;
                if (detailReport.Parameters["pAccountTitle"] != null)
                    detailReport.Parameters["pAccountTitle"].Value = accountTitle;
                if (detailReport.Parameters["pPeriodTitle"] != null)
                    detailReport.Parameters["pPeriodTitle"].Value = periodTitle;

                detailReport.CreateDocument(false);

                string titleSuffix = !string.IsNullOrWhiteSpace(drillDownGroup1Id)
                    ? $" - Đối tượng: {drillDownGroup1Id}"
                    : string.Empty;

                var detailPreviewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Báo cáo chi tiết công nợ ({context.FromDate:dd/MM/yyyy} - {context.ToDate:dd/MM/yyyy}){titleSuffix}"
                };
                detailPreviewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                detailPreviewWindow.PreviewControl.DocumentSource = detailReport;

                // DRILL-DOWN CẤP 2: Click vào dòng chứng từ chi tiết mở VoucherEntryWindow!
                ReportDrillDownHelper.WireVoucherClick(detailPreviewWindow);

                detailPreviewWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, $"Lỗi khi mở Báo cáo chi tiết công nợ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void AttachDebtDrillDownTag(XtraReport report)
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

        #region Báo cáo Ngoại tệ (CurrencyReport)

        private async Task ShowCurrencyReportAsync()
        {
            try
            {
                waitIndicator.Content = "Đang tổng hợp số liệu phát sinh ngoại tệ...";

                List<string>? curIds = null;
                if (SelectedCriteria.TryGetValue("CurrencyId", out var rawCur) && rawCur.Count > 0)
                {
                    curIds = rawCur.Select(x => x.ToString()!).ToList();
                }

                // Chạy trên Background Task: Mở adapter -> tính toán/join -> ngắt kết nối ngay khi xong!
                var dtoList = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    return CurrencyReportCalculator.CalculateSummary(
                        adapter,
                        accountPrefix: SelectedAccountId,
                        fromDate: FromDate,
                        toDate: ToDate,
                        currencyIds: curIds,
                        onlyBooked: true);
                });

                if (dtoList == null || dtoList.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu phát sinh ngoại tệ trong khoảng thời gian đã chọn.",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var accObj = _accounts.FirstOrDefault(a => a.AccountId == SelectedAccountId);
                string accName = accObj?.AccountName ?? "";

                string companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                }

                var report = new CurrencySummaryReport
                {
                    DataSource = dtoList,
                    RequestParameters = false
                };
                report.SetReportHeader(SelectedAccountId, accName, FromDate, ToDate, companyName);
                AttachCurrencyDrillDownTag(report);
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = $"Báo cáo Tổng hợp Ngoại tệ: TK {SelectedAccountId} ({FromDate:dd/MM/yyyy} -> {ToDate:dd/MM/yyyy})"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;

                // DRILL-DOWN CẤP 1: Click vào dòng ngoại tệ -> Mở Sổ chi tiết ngoại tệ
                ReportDrillDownHelper.WireDrillDownKeys(previewWindow, (currencyId, _) =>
                {
                    if (!string.IsNullOrEmpty(currencyId))
                    {
                        var clickedItem = dtoList.FirstOrDefault(d => string.Equals(d.CurrencyId, currencyId, StringComparison.OrdinalIgnoreCase));
                        if (clickedItem != null && clickedItem.Details.Count > 0)
                        {
                            ShowCurrencyDetailReport(clickedItem, accName, companyName, previewWindow);
                        }
                    }
                });

                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lập Báo cáo Ngoại tệ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowCurrencyDetailReport(
            CurrencyReportDTO summaryItem,
            string accName,
            string companyName,
            Window ownerWindow)
        {
            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var detailDtoList = CurrencyReportCalculator.CalculateDetailFromSummary(summaryItem);
                System.Windows.Input.Mouse.OverrideCursor = null;

                if (detailDtoList == null || detailDtoList.Count == 0)
                {
                    MessageBox.Show(ownerWindow, "Không có chứng từ chi tiết phát sinh cho ngoại tệ này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var detailReport = new CurrencyDetailReport
                {
                    DataSource = detailDtoList,
                    RequestParameters = false
                };
                detailReport.SetReportHeader(
                    SelectedAccountId,
                    accName,
                    summaryItem.CurrencyId,
                    summaryItem.CurrencyName,
                    FromDate,
                    ToDate,
                    companyName);
                AttachCurrencyDetailDrillDownTag(detailReport);
                detailReport.CreateDocument(false);

                var detailWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Sổ chi tiết ngoại tệ: {summaryItem.CurrencyId} - TK {SelectedAccountId} ({FromDate:dd/MM/yyyy} -> {ToDate:dd/MM/yyyy})"
                };
                detailWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                detailWindow.PreviewControl.DocumentSource = detailReport;

                // DRILL-DOWN CẤP 2: Click vào dòng chứng từ chi tiết mở VoucherEntryWindow!
                ReportDrillDownHelper.WireVoucherClick(detailWindow);

                detailWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, $"Lỗi mở Sổ chi tiết ngoại tệ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void AttachCurrencyDrillDownTag(XtraReport report)
        {
            foreach (XRControl control in report.AllControls<XRControl>())
            {
                if (control is XRLabel || control is XRTableCell)
                {
                    if (control.Band is DetailBand)
                    {
                        control.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Tag", "[CurrencyId]"));
                    }
                }
            }
        }

        private static void AttachCurrencyDetailDrillDownTag(XtraReport report)
        {
            foreach (XRControl control in report.AllControls<XRControl>())
            {
                if (control is XRLabel || control is XRTableCell)
                {
                    if (control.Band is DetailBand)
                    {
                        control.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Tag", "[JournalVoucherId]"));
                    }
                }
            }
        }

        #endregion

        #region Báo cáo Tài sản cố định (AssetReport)

        private async Task ShowAssetReportAsync()
        {
            try
            {
                waitIndicator.Content = "Đang tổng hợp số liệu Tài sản cố định...";

                List<string>? assetIds = null;
                if (SelectedCriteria.TryGetValue("AssetId", out var rawAsset) && rawAsset.Count > 0)
                {
                    assetIds = rawAsset.Select(x => x.ToString()!).ToList();
                }

                List<string>? reasonIds = null;
                if (SelectedCriteria.TryGetValue("ReasonId", out var rawReason) && rawReason.Count > 0)
                {
                    reasonIds = rawReason.Select(x => x.ToString()!).ToList();
                }

                // Chạy trên Background Task: Nạp dữ liệu biến động TSCĐ theo Group 1 (Lý do) -> Group 2 (Tài sản) -> Detail (Bút toán)
                var dtoList = await Task.Run(() =>
                {
                    using var adapter = AppDataAccessAdapter.Create();
                    return AssetReportCalculator.CalculateMovement(
                        adapter,
                        accountPrefix: SelectedAccountId,
                        fromDate: FromDate,
                        toDate: ToDate,
                        reasonIds: reasonIds,
                        assetIds: assetIds,
                        onlyBooked: true);
                });

                if (dtoList == null || dtoList.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu phát sinh biến động tài sản cố định nào thỏa mãn điều kiện lọc trong kỳ!",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var accObj = _accounts.FirstOrDefault(a => a.AccountId == SelectedAccountId);
                string accName = accObj?.AccountName ?? "";

                string companyName = UserPreferencesHelper.GetSetting("CompanyName");
                if (string.IsNullOrWhiteSpace(companyName))
                {
                    companyName = DbConfigService.Current.CompanyName ?? string.Empty;
                }

                var report = new AssetMovementReport
                {
                    DataSource = dtoList,
                    RequestParameters = false
                };
                report.SetReportHeader(SelectedAccountId, accName, FromDate, ToDate, companyName);
                report.CreateDocument(false);

                var previewWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = this.Owner ?? this,
                    Title = $"Báo cáo Biến động Tài sản cố định ({FromDate:dd/MM/yyyy} - {ToDate:dd/MM/yyyy})"
                };
                previewWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                previewWindow.PreviewControl.DocumentSource = report;

                // DRILL-DOWN: Click đúp vào bất kỳ dòng chứng từ chi tiết nào -> Mở trực tiếp VoucherEntryWindow!
                ReportDrillDownHelper.WireVoucherClick(previewWindow);

                previewWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lập Báo cáo Tài sản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAssetDetailReport(
            AssetSummaryReportDTO summaryItem,
            string accName,
            string companyName,
            Window ownerWindow)
        {
            try
            {
                System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                var detailDtoList = AssetReportCalculator.CalculateDetailFromSummary(summaryItem);
                System.Windows.Input.Mouse.OverrideCursor = null;

                if (detailDtoList == null || detailDtoList.Count == 0)
                {
                    MessageBox.Show(ownerWindow, "Không có dữ liệu chi tiết cho tài sản này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var detailReport = new AssetDetailReport
                {
                    DataSource = detailDtoList,
                    RequestParameters = false
                };

                detailReport.SetReportHeader(
                    SelectedAccountId,
                    accName,
                    summaryItem.AssetId,
                    summaryItem.AssetName,
                    summaryItem.Unit,
                    FromDate,
                    ToDate,
                    companyName);

                detailReport.CreateDocument(false);

                var detailWindow = new DevExpress.Xpf.Printing.DocumentPreviewWindow
                {
                    Owner = ownerWindow,
                    Title = $"Sổ chi tiết TSCĐ: {summaryItem.AssetName} - TK {SelectedAccountId} ({FromDate:dd/MM/yyyy} -> {ToDate:dd/MM/yyyy})"
                };
                detailWindow.PreviewControl.CommandBarStyle = DevExpress.Xpf.DocumentViewer.CommandBarStyle.Ribbon;
                detailWindow.PreviewControl.DocumentSource = detailReport;

                // DRILL-DOWN CẤP 2: Click vào dòng chứng từ chi tiết mở VoucherEntryWindow!
                ReportDrillDownHelper.WireVoucherClick(detailWindow);

                detailWindow.Show();
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                MessageBox.Show(ownerWindow, $"Lỗi khi mở Sổ chi tiết TSCĐ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

    }
}
