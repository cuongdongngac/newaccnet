using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid.LookUp;
using DataAccess.EntityClasses;
using DataAccess.HelperClasses;
using NewaccNet.Core.Data;

namespace NewaccNet.Core.UI
{
    /// <summary>
    /// Lớp cơ sở chuẩn cho khối Lego chọn tài khoản kế toán trong NewaccNet:
    /// - Quản lý DependencyProperty JournalEntry bind 2 chiều với JournalEntryEntity (Accounting First).
    /// - Tự động nhận diện nhóm tài khoản (A: Công nợ, B: Vật tư, E: Chi phí...) để bật/tắt nút chi tiết '...'.
    /// - Cung cấp cơ chế mở form phụ qua Delegate DetailWindowOpener.
    /// </summary>
    public class AccountLookUpEditBase : LookUpEdit
    {
        #region Delegate mở form chi tiết từ tầng ngoài (WPF Host)
        public static Func<JournalEntryEntity, Window, bool, bool>? DetailWindowOpener { get; set; }
        #endregion

        static AccountLookUpEditBase()
        {
            JournalEntryEntity.AccountNameResolver = id => !string.IsNullOrEmpty(id) ? GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == id)?.AccountName : null;
        }

        #region Dependency Properties
        public static readonly DependencyProperty JournalEntryProperty =
            DependencyProperty.Register(nameof(JournalEntry), typeof(JournalEntryEntity), typeof(AccountLookUpEditBase),
                new FrameworkPropertyMetadata(null, OnJournalEntryChanged));

        public static readonly DependencyProperty ExcludeMaterialAccountsProperty =
            DependencyProperty.Register(nameof(ExcludeMaterialAccounts), typeof(bool), typeof(AccountLookUpEditBase),
                new FrameworkPropertyMetadata(false, OnExcludeMaterialAccountsChanged));

        public static readonly DependencyProperty ShowDetailButtonProperty =
            DependencyProperty.Register(nameof(ShowDetailButton), typeof(bool), typeof(AccountLookUpEditBase),
                new FrameworkPropertyMetadata(true, OnShowDetailButtonChanged));

        public static readonly DependencyProperty AllowDetailPromptProperty =
            DependencyProperty.Register(nameof(AllowDetailPrompt), typeof(bool), typeof(AccountLookUpEditBase),
                new FrameworkPropertyMetadata(true, (d, e) => (d as AccountLookUpEditBase)?.UpdateAccountAppearance((d as AccountLookUpEditBase)?.EditValue?.ToString())));

        public static readonly DependencyProperty ButtonOnlyModeProperty =
            DependencyProperty.Register(nameof(ButtonOnlyMode), typeof(bool), typeof(AccountLookUpEditBase),
                new FrameworkPropertyMetadata(false, OnButtonOnlyModeChanged));

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(AccountLookUpEditBase),
                new FrameworkPropertyMetadata("Chi tiết công nợ...", OnButtonTextChanged));

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

        public bool AllowDetailPrompt
        {
            get => (bool)GetValue(AllowDetailPromptProperty);
            set => SetValue(AllowDetailPromptProperty, value);
        }

        /// <summary>
        /// Danh sách các Category cấm không cho mở Form chi tiết con (Mặc định = null: cho phép tất cả các nhóm A, B, C, D, E, S mở chi tiết; chỉ các form kho đặc thù mới cấu hình = "B").
        /// </summary>
        public string? DisabledDetailCategories { get; set; } = null;

        public bool ButtonOnlyMode
        {
            get => (bool)GetValue(ButtonOnlyModeProperty);
            set => SetValue(ButtonOnlyModeProperty, value);
        }

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        private static void OnButtonOnlyModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEditBase editor)
            {
                editor.ApplyButtonOnlyMode((bool)e.NewValue);
            }
        }

        private static void OnButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEditBase editor && editor._detailButton != null && editor.ButtonOnlyMode)
            {
                editor._detailButton.Content = e.NewValue?.ToString() ?? "Chi tiết công nợ...";
            }
        }

        private static void OnShowDetailButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEditBase editor && editor._detailButton != null)
            {
                editor._detailButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region Leaf Accounts Cache
        protected static List<ChartOfAccountEntity>? _cachedLeafAccounts = null;

        public static List<ChartOfAccountEntity> GetLeafAccounts()
        {
            if (_cachedLeafAccounts == null)
            {
                using var adapter = AppDataAccessAdapter.Create();
                var accounts = new EntityCollection<ChartOfAccountEntity>();
                adapter.FetchEntityCollection(accounts, null);
                foreach (var a in accounts)
                {
                    a.CategoryId = ResolveCategory(a.AccountId, a.CategoryId);
                }
                var parentIds = accounts.Select(a => a.ParentId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToHashSet();
                _cachedLeafAccounts = accounts.Where(a => !parentIds.Contains(a.AccountId)).OrderBy(a => a.AccountId).ToList();
            }
            return _cachedLeafAccounts;
        }

        public static void ClearCache()
        {
            _cachedLeafAccounts = null;
        }
        #endregion

        protected bool _isInternalUpdating = false;
        protected bool _wasPopupOpened = false;
        protected ButtonInfo _detailButton;

        #region Default Popup Template
        protected static readonly Lazy<ControlTemplate> _defaultPopupTemplate = new Lazy<ControlTemplate>(() =>
        {
            string xaml = @"
                <ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:dx=""http://schemas.devexpress.com/winfx/2008/xaml/core""
                                 xmlns:dxe=""http://schemas.devexpress.com/winfx/2008/xaml/editors""
                                 xmlns:dxg=""http://schemas.devexpress.com/winfx/2008/xaml/grid"">
                    <dxg:GridControl Name=""PART_GridControl"" MinWidth=""500"">
                        <dxg:GridControl.Columns>
                            <dxg:GridColumn FieldName=""AccountId"" Header=""Mã TK"" Width=""85"" FixedWidth=""True""/>
                            <dxg:GridColumn FieldName=""AccountName"" Header=""Tên tài khoản kế toán"" Width=""*"" MinWidth=""240""/>
                            <dxg:GridColumn FieldName=""CategoryDescription"" Header=""Phân loại"" Width=""110"" FixedWidth=""True""/>
                        </dxg:GridControl.Columns>
                        <dxg:GridControl.View>
                            <dxg:TableView ShowGroupPanel=""False"" 
                                           AutoWidth=""True"" 
                                           ShowIndicator=""False"" 
                                           RowMinHeight=""28"" 
                                           AllowEditing=""False"" 
                                           NavigationStyle=""Row""
                                           AlternateRowBackground=""#F8F9FA"">
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

        protected static object CreateDetailGlyph()
        {
            try
            {
                var ext = new DevExpress.Xpf.Core.SvgImageSourceExtension
                {
                    Uri = new Uri("pack://application:,,,/DevExpress.Images.v25.2;component/svgimages/business%20objects/bo_list.svg")
                };
                var imgSource = ext.ProvideValue(null) as System.Windows.Media.ImageSource;
                if (imgSource != null)
                {
                    return new System.Windows.Controls.Image
                    {
                        Source = imgSource,
                        Width = 14,
                        Height = 14,
                        SnapsToDevicePixels = true
                    };
                }
            }
            catch { }
            return "...";
        }

        private static readonly Lazy<ControlTemplate> _emptyEditTemplate = new Lazy<ControlTemplate>(() =>
        {
            string xaml = @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Grid Width=""0"" Height=""0""/></ControlTemplate>";
            return (ControlTemplate)System.Windows.Markup.XamlReader.Parse(xaml);
        });

        private void ApplyButtonOnlyMode(bool enabled)
        {
            if (enabled)
            {
                this.AllowDefaultButton = false;
                this.ShowBorder = false;
                this.IsTextEditable = false;
                this.Focusable = false;
                this.EditTemplate = _emptyEditTemplate.Value;
                if (_detailButton != null)
                {
                    _detailButton.Content = ButtonText;
                    _detailButton.GlyphKind = GlyphKind.Regular;
                    _detailButton.ToolTip = "Mở form chi tiết công nợ";
                }
            }
            else
            {
                this.AllowDefaultButton = true;
                this.ShowBorder = true;
                this.IsTextEditable = true;
                this.Focusable = true;
                this.ClearValue(EditTemplateProperty);
                if (_detailButton != null)
                {
                    _detailButton.Content = CreateDetailGlyph();
                    _detailButton.GlyphKind = GlyphKind.Custom;
                    _detailButton.ToolTip = "Chi tiết tài khoản (Mở form phụ)";
                }
            }
            UpdateAccountAppearance(this.EditValue?.ToString());
        }

        public static bool IsDetailAccount(string? accountId)
        {
            if (string.IsNullOrEmpty(accountId)) return false;
            string cat = GetCategoryForAccount(accountId);
            return !string.IsNullOrEmpty(cat) && cat != "I";
        }

        public bool IsDetailAccountForControl(string? accountId)
        {
            if (!AllowDetailPrompt) return false;
            if (string.IsNullOrEmpty(accountId)) return false;
            string cat = GetCategoryForAccount(accountId);
            if (!string.IsNullOrEmpty(DisabledDetailCategories) && !string.IsNullOrEmpty(cat) && DisabledDetailCategories.Contains(cat))
                return false;

            return !string.IsNullOrEmpty(cat) && cat != "I";
        }

        public virtual void UpdateAccountAppearance(string? accountId)
        {
            bool hasDetail = IsDetailAccountForControl(accountId);
            if (ButtonOnlyMode)
            {
                this.Visibility = hasDetail ? Visibility.Visible : Visibility.Collapsed;
            }

            if (hasDetail)
            {
                this.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(21, 101, 192)); // Xanh đậm #1565C0
                this.FontWeight = FontWeights.SemiBold;
                if (_detailButton != null)
                {
                    _detailButton.Visibility = ShowDetailButton ? Visibility.Visible : Visibility.Collapsed;
                    _detailButton.IsEnabled = true;
                    _detailButton.ToolTip = ButtonOnlyMode ? "Mở form chi tiết công nợ" : "Tài khoản có chi tiết (Bấm để xem/sửa chi tiết con)";
                }
            }
            else
            {
                this.ClearValue(ForegroundProperty);
                this.ClearValue(FontWeightProperty);
                if (_detailButton != null)
                {
                    _detailButton.Visibility = Visibility.Collapsed;
                    _detailButton.IsEnabled = false;
                    _detailButton.ToolTip = "Tài khoản không mở chi tiết";
                }
            }

            if (!string.IsNullOrEmpty(accountId))
            {
                var acc = GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == accountId);
                if (acc != null)
                {
                    this.ToolTip = $"{acc.AccountId} - {acc.AccountName} ({acc.CategoryDescription})";
                }
                else
                {
                    this.ToolTip = accountId;
                }
            }
            else
            {
                this.ClearValue(ToolTipProperty);
            }
        }

        public AccountLookUpEditBase()
        {
            this.DisplayMember = "AccountId";
            this.ValueMember = "AccountId";
            this.AutoPopulateColumns = false;
            this.AutoComplete = false;
            this.ImmediatePopup = true;
            this.IsTextEditable = true;
            this.IncrementalFiltering = true;
            this.FilterCondition = DevExpress.Data.Filtering.FilterCondition.StartsWith;
            this.PopupWidth = 560;
            this.PopupMinWidth = 480;
            this.PopupMinHeight = 280;
            this.PopupMaxHeight = 450;

            if (this.PopupContentTemplate == null)
            {
                this.PopupContentTemplate = _defaultPopupTemplate.Value;
            }

            _detailButton = new ButtonInfo
            {
                Content = CreateDetailGlyph(),
                ToolTip = "Chi tiết tài khoản (Mở form phụ)",
                GlyphKind = GlyphKind.Custom,
                Visibility = ShowDetailButton ? Visibility.Visible : Visibility.Collapsed
            };
            _detailButton.Click += Btn_Click;
            this.Buttons.Add(_detailButton);

            this.PopupOpened += (s, e) => _wasPopupOpened = true;
            this.PopupClosed += (s, e) => { Dispatcher.BeginInvoke(new Action(() => _wasPopupOpened = false)); };
            this.SubstituteDisplayFilter += OnSubstituteDisplayFilter;
            this.EditValueChanged += BaseAccountLookUpEdit_EditValueChanged;
            this.Loaded += BaseAccountLookUpEdit_Loaded;
        }

        protected virtual void OnSubstituteDisplayFilter(object? sender, SubstituteDisplayFilterEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.DisplayText)) return;
            string raw = e.DisplayText.Trim();

            e.DisplayFilter = new DevExpress.Data.Filtering.GroupOperator(
                DevExpress.Data.Filtering.GroupOperatorType.Or,
                new DevExpress.Data.Filtering.FunctionOperator(DevExpress.Data.Filtering.FunctionOperatorType.StartsWith,
                    new DevExpress.Data.Filtering.OperandProperty("AccountId"),
                    new DevExpress.Data.Filtering.ConstantValue(raw)),
                new DevExpress.Data.Filtering.FunctionOperator(DevExpress.Data.Filtering.FunctionOperatorType.Contains,
                    new DevExpress.Data.Filtering.OperandProperty("AccountName"),
                    new DevExpress.Data.Filtering.ConstantValue(raw))
            );
            e.Handled = true;
        }

        private void BaseAccountLookUpEdit_Loaded(object sender, RoutedEventArgs e)
        {
            if (ButtonOnlyMode)
            {
                ApplyButtonOnlyMode(true);
            }
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
            if (d is AccountLookUpEditBase editor)
            {
                editor.RefreshItemsSource();
            }
        }

        public virtual void RefreshItemsSource()
        {
            var list = GetLeafAccounts();
            if (this.ExcludeMaterialAccounts)
            {
                this.ItemsSource = list.Where(a => a.CategoryId != "B" && !a.AccountId.StartsWith("15")).ToList();
            }
            else
            {
                this.ItemsSource = list;
            }
        }

        private static void OnJournalEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AccountLookUpEditBase editor)
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

        private void Entry_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_isInternalUpdating) return;
            if (e.PropertyName == nameof(JournalEntryEntity.AccountId))
            {
                if (sender is JournalEntryEntity entry && this.EditValue?.ToString() != entry.AccountId)
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

        protected JournalEntryEntity? GetCurrentEntry()
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

        public static string ResolveCategory(string? accountId, string? categoryId)
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

        public static string GetCategoryForAccount(string? accountId)
        {
            if (string.IsNullOrEmpty(accountId)) return string.Empty;

            var cached = GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == accountId);
            if (cached != null)
            {
                return ResolveCategory(accountId, cached.CategoryId);
            }

            using var adapter = AppDataAccessAdapter.Create();
            var acc = new ChartOfAccountEntity(accountId);
            if (adapter.FetchEntity(acc))
            {
                return ResolveCategory(accountId, acc.CategoryId);
            }
            return ResolveCategory(accountId, string.Empty);
        }

        private void BaseAccountLookUpEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            UpdateAccountAppearance(e.NewValue?.ToString());

            if (_isInternalUpdating) return;

            var currentEntry = GetCurrentEntry();
            string? oldAccountId = e.OldValue?.ToString() ?? currentEntry?.AccountId;
            string? newAccountId = e.NewValue?.ToString();

            if (!BeforeAccountSelectionChanged(oldAccountId, newAccountId))
            {
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

            if (currentEntry != null && currentEntry.AccountId != newAccountId)
            {
                currentEntry.AccountId = newAccountId;
                currentEntry.ChartOfAccount = GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == newAccountId);
            }
            if (JournalEntry != null && JournalEntry.AccountId != newAccountId)
            {
                JournalEntry.AccountId = newAccountId;
                JournalEntry.ChartOfAccount = GetLeafAccounts()?.FirstOrDefault(a => a.AccountId == newAccountId);
            }

            // Gọi logic mở rộng nếu có
            OnAccountSelectionChanged(newAccountId);
        }

        protected virtual bool BeforeAccountSelectionChanged(string? oldAccountId, string? newAccountId)
        {
            return true;
        }

        protected virtual void OnAccountSelectionChanged(string? newAccountId)
        {
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            TriggerDetailForm();
        }

        public virtual void TriggerDetailForm()
        {
            if (!ShowDetailButton) return;
            if (this.EditValue == null) return;
            string accountId = this.EditValue.ToString() ?? "";

            // Kiểm tra nếu tài khoản không được phép mở chi tiết (ví dụ vật tư B, hoặc AllowDetail = false)
            if (!IsDetailAccountForControl(accountId)) return;

            var currentEntry = GetCurrentEntry();
            if (currentEntry == null)
            {
                currentEntry = new JournalEntryEntity();
            }

            currentEntry.AccountId = accountId;
            var owner = Window.GetWindow(this);

            if (DetailWindowOpener != null)
            {
                bool? result = DetailWindowOpener(currentEntry, owner, false);
                OnDetailWindowClosed(result == true);
            }
        }

        protected virtual void OnDetailWindowClosed(bool success)
        {
        }
    }
}
