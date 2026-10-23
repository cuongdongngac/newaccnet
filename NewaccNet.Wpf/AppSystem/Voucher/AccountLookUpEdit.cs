using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.LookUp;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using NewaccNet.Wpf.AppSystem;

namespace NewaccNet.Wpf.AppSystem.Voucher
{
    /// <summary>
    /// Khối Lego độc lập chọn tài khoản kế toán (AccountLookUpEdit):
    /// - Tự có PopupContentTemplate hiển thị Mã TK và Tên TK gọn gàng.
    /// - Tự nạp và lọc danh sách tài khoản lá từ Cache hệ thống.
    /// - Khi ExcludeMaterialAccounts = true: tự động loại bỏ các tài khoản vật tư (CategoryId B, 15x).
    /// - Có DependencyProperty JournalEntry: bind trực tiếp 2 chiều với JournalEntryEntity.
    /// - Tự động điều hướng mở Form phụ chi tiết (A: Công nợ, B: Vật tư, E: Chi phí).
    /// - Khi UpdateFilter = true trên entity: các form con (công nợ/chi phí) không cập nhật ngược đè số tiền do vật tư quản lý.
    /// </summary>
    public class AccountLookUpEdit : LookUpEdit
    {
        #region Dependency Properties
        public static readonly DependencyProperty JournalEntryProperty =
            DependencyProperty.Register(nameof(JournalEntry), typeof(JournalEntryEntity), typeof(AccountLookUpEdit),
                new FrameworkPropertyMetadata(null, OnJournalEntryChanged));

        public static readonly DependencyProperty ExcludeMaterialAccountsProperty =
            DependencyProperty.Register(nameof(ExcludeMaterialAccounts), typeof(bool), typeof(AccountLookUpEdit),
                new FrameworkPropertyMetadata(false, OnExcludeMaterialAccountsChanged));

        public static readonly DependencyProperty ShowDetailButtonProperty =
            DependencyProperty.Register(nameof(ShowDetailButton), typeof(bool), typeof(AccountLookUpEdit),
                new FrameworkPropertyMetadata(true, OnShowDetailButtonChanged));

        public JournalEntryEntity JournalEntry
        {
            get => (JournalEntryEntity)GetValue(JournalEntryProperty);
            set => SetValue(JournalEntryProperty, value);
        }

        public bool ExcludeMaterialAccounts
        {
            get => (bool)GetValue(ExcludeMaterialAccountsProperty);
            set => SetValue(ExcludeMaterialAccountsProperty, value);
        }

        public bool ShowDetailButton
        {
            get => (bool)GetValue(ShowDetailButtonProperty);
            set => SetValue(ShowDetailButtonProperty, value);
        }

        private static void OnShowDetailButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEdit editor && editor._detailButton != null)
            {
                editor._detailButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region Default Popup Template
        private static readonly Lazy<ControlTemplate> _defaultPopupTemplate = new Lazy<ControlTemplate>(() =>
        {
            string xaml = @"
                <ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:dx=""http://schemas.devexpress.com/winfx/2008/xaml/core""
                                 xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid"">
                    <dxg:GridControl Name=""PART_GridControl"">
                        <dxg:GridControl.Columns>
                            <dxg:GridColumn FieldName=""AccountId"" Header=""Mã TK"" Width=""100""/>
                            <dxg:GridColumn FieldName=""AccountName"" Header=""Tên tài khoản"" Width=""*""/>
                            <dxg:GridColumn FieldName=""CategoryId"" Header=""Loại"" Width=""55""/>
                        </dxg:GridControl.Columns>
                        <dxg:GridControl.View>
                            <dxg:TableView ShowGroupPanel=""False"" AutoWidth=""True"">
                                <dxg:TableView.FormatConditions>
                                    <dxg:FormatCondition Expression=""[CategoryId] != 'I' and [CategoryId] is not null and [CategoryId] != ''"" ApplyToRow=""True"">
                                        <dx:Format Foreground=""#1565C0"" FontWeight=""SemiBold""/>
                                    </dxg:FormatCondition>
                                </dxg:TableView.FormatConditions>
                            </dxg:TableView>
                        </dxg:GridControl.View>
                    </dxg:GridControl>
                </ControlTemplate>";
            return (ControlTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        });
        #endregion

        #region Leaf Accounts Cache
        private static List<ChartOfAccountEntity> _cachedLeafAccounts = null;

        public static List<ChartOfAccountEntity> GetLeafAccounts()
        {
            if (_cachedLeafAccounts == null)
            {
                using (var adapter = AppDataAccessAdapter.Create())
                {
                    var accounts = new EntityCollection<ChartOfAccountEntity>();
                    adapter.FetchEntityCollection(accounts, null);
                    foreach (var a in accounts)
                    {
                        a.CategoryId = ResolveCategory(a.AccountId, a.CategoryId);
                    }
                    var parentIds = accounts.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
                    _cachedLeafAccounts = accounts.Where(a => !parentIds.Contains(a.AccountId)).OrderBy(a => a.AccountId).ToList();
                }
            }
            return _cachedLeafAccounts;
        }
        #endregion

        private bool _isInternalUpdating = false;
        private bool _wasPopupOpened = false;
        private ButtonInfo _detailButton;

        public static bool IsDetailAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId)) return false;
            string cat = GetCategoryForAccount(accountId);
            return !string.IsNullOrEmpty(cat) && cat != "I";
        }

        public void UpdateAccountAppearance(string accountId)
        {
            bool hasDetail = IsDetailAccount(accountId);
            if (hasDetail)
            {
                this.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(21, 101, 192)); // Xanh đậm #1565C0
                this.FontWeight = FontWeights.SemiBold;
                if (_detailButton != null)
                {
                    _detailButton.IsEnabled = true;
                    _detailButton.ToolTip = "Tài khoản có chi tiết (Bấm để xem/sửa chi tiết con)";
                }
            }
            else
            {
                this.ClearValue(ForegroundProperty);
                this.ClearValue(FontWeightProperty);
                if (_detailButton != null)
                {
                    _detailButton.IsEnabled = false;
                    _detailButton.ToolTip = "Tài khoản không theo dõi chi tiết";
                }
            }
        }

        public AccountLookUpEdit()
        {
            this.DisplayMember = "AccountId";
            this.ValueMember = "AccountId";
            this.AutoPopulateColumns = false;
            this.AutoComplete = true;
            this.ImmediatePopup = true;
            this.IsTextEditable = true;
            this.IncrementalFiltering = true;
            this.FilterCondition = DevExpress.Data.Filtering.FilterCondition.StartsWith;
            this.PopupWidth = 500;

            if (this.PopupContentTemplate == null)
            {
                this.PopupContentTemplate = _defaultPopupTemplate.Value;
            }

            _detailButton = new ButtonInfo();
            _detailButton.Content = "...";
            _detailButton.ToolTip = "Chi tiết tài khoản (Mở form phụ)";
            _detailButton.GlyphKind = GlyphKind.Regular;
            _detailButton.Click += Btn_Click;
            _detailButton.Visibility = ShowDetailButton ? Visibility.Visible : Visibility.Collapsed;
            this.Buttons.Add(_detailButton);

            this.PopupOpened += (s, e) => _wasPopupOpened = true;
            this.PopupClosed += (s, e) => { Dispatcher.BeginInvoke(new Action(() => _wasPopupOpened = false)); };
            this.EditValueChanged += AccountLookUpEdit_EditValueChanged;
            this.Loaded += AccountLookUpEdit_Loaded;
        }

        private void AccountLookUpEdit_Loaded(object sender, RoutedEventArgs e)
        {
            if (_detailButton != null)
            {
                _detailButton.Visibility = ShowDetailButton ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.ItemsSource == null)
            {
                RefreshItemsSource();
            }
            UpdateAccountAppearance(this.EditValue?.ToString());
        }

        private static void OnExcludeMaterialAccountsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEdit editor)
            {
                editor.RefreshItemsSource();
            }
        }

        public void RefreshItemsSource()
        {
            var list = GetLeafAccounts();
            if (this.ExcludeMaterialAccounts)
            {
                // Lọc loại bỏ hoàn toàn các tài khoản vật tư (CategoryId = B hoặc bắt đầu bằng 15x)
                this.ItemsSource = list.Where(a => a.CategoryId != "B" && !a.AccountId.StartsWith("15")).ToList();
            }
            else
            {
                this.ItemsSource = list;
            }
        }

        private static void OnJournalEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEdit editor)
            {
                if (e.OldValue is JournalEntryEntity oldEntry)
                {
                    oldEntry.PropertyChanged -= editor.Entry_PropertyChanged;
                }

                if (e.NewValue is JournalEntryEntity newEntry)
                {
                    newEntry.PropertyChanged += editor.Entry_PropertyChanged;

                    if (editor.EditValue?.ToString() != newEntry.AccountId)
                    {
                        editor._isInternalUpdating = true;
                        try
                        {
                            editor.EditValue = newEntry.AccountId;
                        }
                        finally
                        {
                            editor._isInternalUpdating = false;
                        }
                    }
                }
                else
                {
                    editor._isInternalUpdating = true;
                    try
                    {
                        editor.EditValue = null;
                    }
                    finally
                    {
                        editor._isInternalUpdating = false;
                    }
                }
            }
        }

        private void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isInternalUpdating) return;
            if (e.PropertyName == nameof(JournalEntryEntity.AccountId))
            {
                var entry = sender as JournalEntryEntity;
                if (entry != null && this.EditValue?.ToString() != entry.AccountId)
                {
                    _isInternalUpdating = true;
                    try
                    {
                        this.EditValue = entry.AccountId;
                    }
                    finally
                    {
                        _isInternalUpdating = false;
                    }
                }
            }
        }

        private JournalEntryEntity GetCurrentEntry()
        {
            var currentEntry = this.JournalEntry;
            if (currentEntry == null)
            {
                if (this.DataContext is DevExpress.Xpf.Grid.EditGridCellData cellData)
                {
                    currentEntry = cellData.RowData.Row as JournalEntryEntity;
                }
                else
                {
                    currentEntry = this.DataContext as JournalEntryEntity;
                }
            }
            return currentEntry;
        }

        public static string ResolveCategory(string accountId, string categoryId)
        {
            string cat = categoryId?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(cat)) return cat;
            if (string.IsNullOrEmpty(accountId)) return string.Empty;
            if (accountId.StartsWith("131") || accountId.StartsWith("331") || accountId.StartsWith("138") || accountId.StartsWith("338")) return "A";
            if (accountId.StartsWith("152") || accountId.StartsWith("153") || accountId.StartsWith("155") || accountId.StartsWith("156") || accountId.StartsWith("157")) return "B";
            if (accountId.StartsWith("1112") || accountId.StartsWith("1122")) return "C";
            if (accountId.StartsWith("211") || accountId.StartsWith("213") || accountId.StartsWith("214") || accountId.StartsWith("242")) return "D";
            if (accountId.StartsWith("6") || accountId.StartsWith("154")) return "E";
            if (accountId.StartsWith("121") || accountId.StartsWith("221") || accountId.StartsWith("222") || accountId.StartsWith("228")) return "S";
            return string.Empty;
        }

        public static string GetCategoryForAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId)) return string.Empty;

            var cached = GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == accountId);
            if (cached != null)
            {
                return ResolveCategory(accountId, cached.CategoryId);
            }

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var acc = new ChartOfAccountEntity(accountId);
                if (adapter.FetchEntity(acc))
                {
                    return ResolveCategory(accountId, acc.CategoryId);
                }
            }
            return ResolveCategory(accountId, string.Empty);
        }

        private void AccountLookUpEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            UpdateAccountAppearance(e.NewValue?.ToString());

            if (_isInternalUpdating) return;

            var currentEntry = GetCurrentEntry();
            string oldAccountId = currentEntry?.AccountId;
            string newAccountId = e.NewValue?.ToString();

            bool isUserAction = this.IsKeyboardFocusWithin || this.IsPopupOpen || _wasPopupOpened;
            if (!isUserAction)
            {
                // Khi binding ngầm từ bên ngoài: chỉ đồng bộ nếu JournalEntry != null
                if (JournalEntry != null && newAccountId != oldAccountId)
                {
                    _isInternalUpdating = true;
                    try
                    {
                        JournalEntry.AccountId = newAccountId;
                    }
                    finally
                    {
                        _isInternalUpdating = false;
                    }
                }
                return;
            }

            if (oldAccountId == newAccountId) return;

            // ĐỐI CHỨNG VỚI TÀI KHOẢN CŨ CỦA JOURNALENTRY
            if (currentEntry != null && !string.IsNullOrEmpty(oldAccountId))
            {
                if (currentEntry.HasChildDetails())
                {
                    string oldCategory = GetCategoryForAccount(oldAccountId);
                    string newCategory = GetCategoryForAccount(newAccountId);

                    if (oldCategory != newCategory)
                    {
                        var confirmResult = MessageBox.Show(
                            $"Tài khoản hiện tại ({oldAccountId}) đang có dữ liệu chi tiết con.\n" +
                            $"Bạn vừa đổi sang tài khoản ({newAccountId}) thuộc nhóm khác.\n\n" +
                            $"Hệ thống sẽ XÓA toàn bộ chi tiết con cũ để chuyển sang nhóm mới.\n" +
                            $"Bạn có muốn tiếp tục không?",
                            "Xác nhận thay đổi tài khoản",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (confirmResult != MessageBoxResult.Yes)
                        {
                            // Người dùng KHÔNG đồng ý: Khôi phục lại tài khoản cũ cho control!
                            _isInternalUpdating = true;
                            try
                            {
                                this.EditValue = oldAccountId;
                            }
                            finally
                            {
                                _isInternalUpdating = false;
                            }
                            return;
                        }

                        // Người dùng ĐỒNG Ý: Xóa toàn bộ con cũ!
                        var ownerWindow = Window.GetWindow(this) as VoucherEntryWindow;
                        if (ownerWindow != null && ownerWindow.CurrentVoucher != null && currentEntry.SubEntries != null)
                        {
                            foreach (var sub in currentEntry.SubEntries.ToList())
                            {
                                ownerWindow.CurrentVoucher.JournalEntries.Remove(sub);
                            }
                        }

                        currentEntry.ClearChildDetails();

                        if (ownerWindow != null)
                        {
                            ownerWindow.RefreshVoucherUI();
                            ownerWindow.CalculateBalance();
                        }
                    }
                }
            }

            // Gán tài khoản mới vào currentEntry
            if (currentEntry != null)
            {
                currentEntry.AccountId = newAccountId;
            }
            if (JournalEntry != null)
            {
                JournalEntry.AccountId = newAccountId;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                TriggerDetailForm();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            TriggerDetailForm();
        }

        private void TriggerDetailForm()
        {
            if (!ShowDetailButton) return;
            if (this.EditValue == null) return;
            string accountId = this.EditValue.ToString();

            var currentEntry = GetCurrentEntry();
            if (currentEntry == null)
            {
                currentEntry = new JournalEntryEntity();
            }

            currentEntry.AccountId = accountId;
            var owner = Window.GetWindow(this);
            OpenDetailWindow(currentEntry, owner, this.ExcludeMaterialAccounts, false);
        }

        public static bool OpenDetailWindow(JournalEntryEntity currentEntry, Window owner, bool excludeMaterialAccounts = false, bool showNotSupportedMessage = false)
        {
            if (currentEntry == null || string.IsNullOrEmpty(currentEntry.AccountId))
            {
                if (showNotSupportedMessage)
                {
                    MessageBox.Show("Vui lòng chọn tài khoản trước khi nhập chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return false;
            }

            string accountId = currentEntry.AccountId;
            var voucherWindow = owner as VoucherEntryWindow;
            var master = (voucherWindow?.gridLeft?.SelectedItem ?? voucherWindow?.gridLeft?.CurrentItem ?? voucherWindow?.MasterEntries?.FirstOrDefault()) as JournalEntryEntity;

            // Nếu đang đứng ở lưới con (gridRight) trên VoucherEntryWindow và dòng này không phải dòng Master
            // thì tự động đảo chiều Dbcr theo dòng Master và liên kết Parent!
            if (voucherWindow != null && master != null && currentEntry != master)
            {
                if (master.Dbcr.HasValue && master.Dbcr.Value != 0)
                {
                    currentEntry.Dbcr = (short)-master.Dbcr.Value;
                }
                if (currentEntry.ParentEntry == null)
                {
                    currentEntry.ParentEntry = master;
                }
                if (master.Id != 0 && (!currentEntry.ParentId.HasValue || currentEntry.ParentId == 0))
                {
                    currentEntry.ParentId = master.Id;
                }
            }

            using (var adapter = AppDataAccessAdapter.Create())
            {
                var account = new ChartOfAccountEntity(accountId);
                if (!adapter.FetchEntity(account)) return false;

                account.CategoryId = ResolveCategory(accountId, account.CategoryId);
                string cat = account.CategoryId;

                bool isDetailCategory = cat == "A" || cat == "B" || cat == "C" || cat == "D" || cat == "E" || cat == "S";
                if (!isDetailCategory)
                {
                    if (showNotSupportedMessage)
                    {
                        MessageBox.Show($"Tài khoản {accountId} ({account.AccountName}) là tài khoản hạch toán thông thường, không có bảng chi tiết phụ.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    return false;
                }

                // Nếu là tài khoản Vật tư (B) nhưng đang ở form vật tư (ExcludeMaterialAccounts == true, UpdateFilter == true hoặc owner is MaterialDetailWindow)
                // thì tuyệt đối KHÔNG mở form lồng đệ quy!
                if (cat == "B" && (excludeMaterialAccounts || currentEntry.UpdateFilter || owner is Details.MaterialDetailWindow))
                {
                    return false;
                }

                // Chỉ khi tài khoản THỰC SỰ có Form phụ con thì mới yêu cầu Dbcr:
                if (!currentEntry.Dbcr.HasValue || currentEntry.Dbcr == 0)
                {
                    if (!currentEntry.UpdateFilter && !excludeMaterialAccounts)
                    {
                        MessageBox.Show("Vui lòng chọn Nợ hoặc Có trước khi mở chi tiết.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }

                return RouteToDetailForm(account, currentEntry, owner, excludeMaterialAccounts);
            }
        }

        private static bool RouteToDetailForm(ChartOfAccountEntity account, JournalEntryEntity entry, Window owner, bool excludeMaterialAccounts)
        {
            var voucherWindow = owner as VoucherEntryWindow;
            string cat = account.CategoryId;
            bool? result = false;

            if (cat == "A")
            {
                var debtForm = new Details.DebtDetailWindow(entry);
                if (owner != null) debtForm.Owner = owner;
                result = debtForm.ShowDialog();
            }
            else if (cat == "B")
            {
                if (owner is Details.MaterialDetailWindow || excludeMaterialAccounts || entry.UpdateFilter)
                {
                    return false;
                }

                var voucher = voucherWindow?.CurrentVoucher;

                bool hasExistingDetails = (entry.InventoryVouchers != null && entry.InventoryVouchers.Count > 0)
                    || (entry.SubEntries != null && entry.SubEntries.Any(s => s.InventoryVouchers != null && s.InventoryVouchers.Count > 0));

                if (!hasExistingDetails && voucher != null)
                {
                    hasExistingDetails = voucher.JournalEntries.Any(j => j.InventoryVouchers != null && j.InventoryVouchers.Count > 0
                        && (j == entry || j.ParentEntry == entry || (j.ParentId.HasValue && entry.Id != 0 && j.ParentId == entry.Id) || entry.ParentEntry == j));
                }

                bool isNewMaterial = !hasExistingDetails;
                var context = isNewMaterial
                    ? Details.MaterialTransactionContext.CreateNew(entry, voucher)
                    : Details.MaterialTransactionContext.FromExisting(entry, voucher);

                var materialForm = new Details.MaterialDetailWindow(context);
                if (owner != null) materialForm.Owner = owner;
                result = materialForm.ShowDialog();

                if (result == true)
                {
                    context.Apply();
                    if (voucherWindow != null)
                    {
                        voucherWindow.RefreshVoucherUI();
                    }
                }
                else if (isNewMaterial)
                {
                    context.Discard();
                    if (voucherWindow != null)
                    {
                        voucherWindow.RefreshVoucherUI();
                    }
                }
            }
            else if (cat == "C")
            {
                var currencyForm = new Details.CurrencyDetailWindow(entry);
                if (owner != null) currencyForm.Owner = owner;
                result = currencyForm.ShowDialog();
            }
            else if (cat == "D")
            {
                var assetForm = new Details.AssetDetailWindow(entry);
                if (owner != null) assetForm.Owner = owner;
                result = assetForm.ShowDialog();
            }
            else if (cat == "E")
            {
                bool hasTax = account.Taxflag && voucherWindow != null;
                var costForm = new Details.CostDetailWindow(entry, hasTax);
                if (owner != null) costForm.Owner = owner;
                result = costForm.ShowDialog();
            }
            else if (cat == "S")
            {
                var stockForm = new Details.StockDetailWindow(entry);
                if (owner != null) stockForm.Owner = owner;
                result = stockForm.ShowDialog();
            }

            if (voucherWindow != null)
            {
                voucherWindow.CalculateBalance();
            }

            return result == true;
        }
    }
}
