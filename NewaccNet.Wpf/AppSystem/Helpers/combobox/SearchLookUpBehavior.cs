using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.LookUp;

namespace NewaccNet.Wpf.Controls.Search
{
    /// <summary>
    /// Gắn vào LookUpEdit (2 cột Mã | Tên) hoặc ComboBoxEdit.
    /// - Tự lấy cấu hình (trường mã/tên, hiển thị, mode mặc định, tiêu đề cột) theo kiểu item.
    /// - Mọi thuộc tính bên dưới để null = dùng cấu hình của model; set = ghi đè tại chỗ.
    ///
    /// &lt;dxg:LookUpEdit ItemsSource="{Binding Accounts}"&gt;
    ///   &lt;dxmvvm:Interaction.Behaviors&gt;
    ///     &lt;dxs:SearchLookUpBehavior SearchMode="{Binding AccountSearchMode}"/&gt;
    ///   &lt;/dxmvvm:Interaction.Behaviors&gt;
    /// &lt;/dxg:LookUpEdit&gt;
    ///
    /// KHÔNG set DisplayMember / AutoPopulateColumns / IncrementalFiltering trên editor - behavior quản lý.
    /// </summary>
    public class SearchLookUpBehavior : Behavior<LookUpEditBase>
    {
        static SearchLookUpBehavior()
        {
            VnSearchFunctions.EnsureRegistered();
        }

        #region Dependency properties (null = theo cấu hình model)

        public static readonly DependencyProperty SearchModeProperty = DependencyProperty.Register(
            nameof(SearchMode), typeof(SearchMode?), typeof(SearchLookUpBehavior),
            new PropertyMetadata(null, OnConfigChanged));

        /// <summary>Đổi lúc runtime được (bind từ ViewModel).</summary>
        public SearchMode? SearchMode
        {
            get => (SearchMode?)GetValue(SearchModeProperty);
            set => SetValue(SearchModeProperty, value);
        }

        public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register(
            nameof(Display), typeof(SearchDisplay?), typeof(SearchLookUpBehavior),
            new PropertyMetadata(null, OnConfigChanged));

        public SearchDisplay? Display
        {
            get => (SearchDisplay?)GetValue(DisplayProperty);
            set => SetValue(DisplayProperty, value);
        }

        public static readonly DependencyProperty CodeFieldProperty = DependencyProperty.Register(
            nameof(CodeField), typeof(string), typeof(SearchLookUpBehavior),
            new PropertyMetadata(null, OnConfigChanged));

        public string CodeField
        {
            get => (string)GetValue(CodeFieldProperty);
            set => SetValue(CodeFieldProperty, value);
        }

        public static readonly DependencyProperty NameFieldProperty = DependencyProperty.Register(
            nameof(NameField), typeof(string), typeof(SearchLookUpBehavior),
            new PropertyMetadata(null, OnConfigChanged));

        public string NameField
        {
            get => (string)GetValue(NameFieldProperty);
            set => SetValue(NameFieldProperty, value);
        }

        public static readonly DependencyProperty CodeHeaderProperty = DependencyProperty.Register(
            nameof(CodeHeader), typeof(string), typeof(SearchLookUpBehavior),
            new PropertyMetadata(null, OnConfigChanged));

        public string CodeHeader
        {
            get => (string)GetValue(CodeHeaderProperty);
            set => SetValue(CodeHeaderProperty, value);
        }

        public static readonly DependencyProperty NameHeaderProperty = DependencyProperty.Register(
            nameof(NameHeader), typeof(string), typeof(SearchLookUpBehavior),
            new PropertyMetadata(null, OnConfigChanged));

        public string NameHeader
        {
            get => (string)GetValue(NameHeaderProperty);
            set => SetValue(NameHeaderProperty, value);
        }

        private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((SearchLookUpBehavior)d).Apply();

        #endregion

        private SearchProfile _profile;
        private DependencyPropertyDescriptor _itemsSourceDescriptor;
        private GridControl _columnsGrid;
        private SearchProfile _columnsProfile;

        /// <summary>Cấu hình đang áp dụng (null nếu chưa resolve được).</summary>
        public SearchProfile Profile => _profile;

        public SearchMode EffectiveMode
            => SearchMode ?? _profile?.DefaultMode ?? Controls.Search.SearchMode.All;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SubstituteDisplayFilter += OnSubstituteDisplayFilter;
            AssociatedObject.PopupOpened += OnPopupOpened;

            _itemsSourceDescriptor = DependencyPropertyDescriptor.FromProperty(
                LookUpEditBase.ItemsSourceProperty, typeof(LookUpEditBase));
            _itemsSourceDescriptor.AddValueChanged(AssociatedObject, OnItemsSourceChanged);

            Apply();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SubstituteDisplayFilter -= OnSubstituteDisplayFilter;
            AssociatedObject.PopupOpened -= OnPopupOpened;
            _itemsSourceDescriptor?.RemoveValueChanged(AssociatedObject, OnItemsSourceChanged);
            _itemsSourceDescriptor = null;
            _columnsGrid = null;
            base.OnDetaching();
        }

        private void OnItemsSourceChanged(object sender, EventArgs e) => Apply();

        private void Apply()
        {
            LookUpEditBase edit = AssociatedObject;
            if (edit == null) return;

            _profile = BuildProfile(GetItemType(edit.ItemsSource));
            if (_profile == null) return;   // chưa có dữ liệu / model chưa cấu hình

            edit.SetCurrentValue(LookUpEditBase.DisplayMemberProperty, _profile.DisplayField);
            edit.SetCurrentValue(LookUpEditBase.IncrementalFilteringProperty, true);
            edit.SetCurrentValue(LookUpEditBase.ImmediatePopupProperty, true);
            edit.SetCurrentValue(LookUpEditBase.AutoCompleteProperty, false);

            if (edit is LookUpEdit lookUp)
            {
                lookUp.SetCurrentValue(LookUpEdit.AutoPopulateColumnsProperty, false);
                if (double.IsNaN(lookUp.PopupMinWidth) || lookUp.PopupMinWidth < 360)
                    lookUp.SetCurrentValue(LookUpEditBase.PopupMinWidthProperty, 360.0);
                BuildColumns(lookUp.GetGridControl());   // null nếu popup chưa mở lần nào
            }
        }

        private SearchProfile BuildProfile(Type itemType)
        {
            SearchProfile p = SearchProfiles.Resolve(itemType);

            string code = CodeField ?? p?.CodeField;
            string name = NameField ?? p?.NameField;
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name)) return null;

            return new SearchProfile(
                code, name,
                Display ?? p?.Display ?? SearchDisplay.Name,
                p?.DefaultMode ?? Controls.Search.SearchMode.All,
                CodeHeader ?? p?.CodeHeader,
                NameHeader ?? p?.NameHeader);
        }

        private void OnSubstituteDisplayFilter(object sender, SubstituteDisplayFilterEventArgs e)
        {
            if (_profile == null || string.IsNullOrWhiteSpace(e.DisplayText)) return;

            string text = e.DisplayText;
            CriteriaOperator byCode = VnSearchFunctions.StartsWith(_profile.CodeField, text);
            CriteriaOperator byName = VnSearchFunctions.ContainsAll(_profile.NameField, text);

            switch (EffectiveMode)
            {
                case Controls.Search.SearchMode.Code:
                    e.DisplayFilter = byCode;
                    break;
                case Controls.Search.SearchMode.Name:
                    e.DisplayFilter = byName;
                    break;
                default:
                    e.DisplayFilter = new GroupOperator(GroupOperatorType.Or, byCode, byName);
                    break;
            }
            e.Handled = true;
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            if (AssociatedObject is LookUpEdit lookUp)
                BuildColumns(lookUp.GetGridControl());
        }

        /// <summary>Luôn 2 cột: Mã | Tên. Chỉ dựng lại khi grid hoặc cấu hình đổi.</summary>
        private void BuildColumns(GridControl grid)
        {
            if (grid == null || _profile == null) return;
            if (ReferenceEquals(grid, _columnsGrid) && ReferenceEquals(_profile, _columnsProfile)) return;

            grid.Columns.Clear();
            grid.Columns.Add(new GridColumn { FieldName = _profile.CodeField, Header = _profile.CodeHeader, Width = 110 });
            grid.Columns.Add(new GridColumn { FieldName = _profile.NameField, Header = _profile.NameHeader });

            if (grid.View is TableView view)
            {
                view.AutoWidth = true;
                view.ShowAutoFilterRow = false;
                view.ShowGroupPanel = false;
            }

            _columnsGrid = grid;
            _columnsProfile = _profile;
        }

        private static Type GetItemType(object source)
        {
            if (source == null) return null;

            Type t = source.GetType();
            Type enumerable = new[] { t }.Concat(t.GetInterfaces())
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumerable != null)
            {
                Type arg = enumerable.GetGenericArguments()[0];
                if (arg != typeof(object)) return arg;
            }

            // ICollectionView, IList không generic... -> lấy kiểu của item đầu tiên
            if (source is IEnumerable items)
            {
                foreach (object item in items)
                {
                    if (item != null) return item.GetType();
                }
            }
            return null;
        }
    }
}
