using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyApp.Controls
{
    /// <summary>
    /// Dropdown chọn nhiều bằng checkbox, tìm kiếm theo cả Key và Display
    /// (không phân biệt hoa thường, không dấu), trả về mảng key qua SelectedKeys.
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        public class CheckItem : INotifyPropertyChanged
        {
            private bool _isChecked;
            public object Key { get; internal set; }
            public string Display { get; internal set; }
            public object Source { get; internal set; }
            internal string SearchText;

            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    if (_isChecked == value) return;
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private List<CheckItem> _items = new List<CheckItem>();
        private ListCollectionView _view;
        private string[] _tokens = new string[0];
        private bool _suppress;   // đang check/uncheck hàng loạt
        private bool _pushing;    // đang ghi SelectedKeys từ bên trong control
        private INotifyCollectionChanged _observed;

        /// <summary>Phát ra mỗi khi danh sách key được chọn thay đổi.</summary>
        public event EventHandler SelectionChanged;

        public MultiSelectComboBox()
        {
            InitializeComponent();
            SetView();
            UpdateText();
        }

        #region Dependency properties

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(MultiSelectComboBox),
            new PropertyMetadata(null, (d, e) => ((MultiSelectComboBox)d).OnItemsSourceChanged(e.NewValue as IEnumerable)));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>Tên thuộc tính làm key (vd "Id"). Để trống = dùng chính item.</summary>
        public static readonly DependencyProperty KeyMemberPathProperty = DependencyProperty.Register(
            nameof(KeyMemberPath), typeof(string), typeof(MultiSelectComboBox),
            new PropertyMetadata("", (d, e) => ((MultiSelectComboBox)d).Rebuild()));

        public string KeyMemberPath
        {
            get => (string)GetValue(KeyMemberPathProperty);
            set => SetValue(KeyMemberPathProperty, value);
        }

        /// <summary>Tên thuộc tính để hiển thị (vd "Name"). Để trống = ToString() của key.</summary>
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register(
            nameof(DisplayMemberPath), typeof(string), typeof(MultiSelectComboBox),
            new PropertyMetadata("", (d, e) => ((MultiSelectComboBox)d).Rebuild()));

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        /// <summary>Mảng key đang chọn (object[]). Mặc định binding TwoWay.</summary>
        public static readonly DependencyProperty SelectedKeysProperty = DependencyProperty.Register(
            nameof(SelectedKeys), typeof(IEnumerable), typeof(MultiSelectComboBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d, e) => ((MultiSelectComboBox)d).OnSelectedKeysChanged()));

        public IEnumerable SelectedKeys
        {
            get => (IEnumerable)GetValue(SelectedKeysProperty);
            set => SetValue(SelectedKeysProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            nameof(Placeholder), typeof(string), typeof(MultiSelectComboBox),
            new PropertyMetadata("-- Chọn --", (d, e) => ((MultiSelectComboBox)d).UpdateText()));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>Số mục tối đa liệt kê tên trên ô; vượt quá sẽ hiện "Đã chọn N mục".</summary>
        public static readonly DependencyProperty MaxDisplayItemsProperty = DependencyProperty.Register(
            nameof(MaxDisplayItems), typeof(int), typeof(MultiSelectComboBox),
            new PropertyMetadata(3, (d, e) => ((MultiSelectComboBox)d).UpdateText()));

        public int MaxDisplayItems
        {
            get => (int)GetValue(MaxDisplayItemsProperty);
            set => SetValue(MaxDisplayItemsProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            nameof(IsDropDownOpen), typeof(bool), typeof(MultiSelectComboBox),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(
            nameof(MaxDropDownHeight), typeof(double), typeof(MultiSelectComboBox),
            new PropertyMetadata(260.0));

        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        #endregion

        #region Public helpers

        /// <summary>Lấy mảng key có kiểu, vd GetSelectedKeys&lt;int&gt;().</summary>
        public T[] GetSelectedKeys<T>() =>
            _items.Where(i => i.IsChecked).Select(i => (T)i.Key).ToArray();

        /// <summary>Lấy các object gốc đang được chọn.</summary>
        public object[] GetSelectedItems() =>
            _items.Where(i => i.IsChecked).Select(i => i.Source).ToArray();

        public void ClearSelection() => SetAll(_items, false);

        #endregion

        #region Build items

        private void OnItemsSourceChanged(IEnumerable newValue)
        {
            if (_observed != null) _observed.CollectionChanged -= Source_CollectionChanged;
            _observed = newValue as INotifyCollectionChanged;
            if (_observed != null) _observed.CollectionChanged += Source_CollectionChanged;
            Rebuild();
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Rebuild();

        private void Rebuild()
        {
            foreach (var it in _items) it.PropertyChanged -= Item_PropertyChanged;

            var selected = new HashSet<object>(ToObjects(SelectedKeys));
            var list = new List<CheckItem>();

            if (ItemsSource != null)
            {
                foreach (var src in ItemsSource)
                {
                    var key = Resolve(src, KeyMemberPath);
                    var display = Resolve(src, DisplayMemberPath)?.ToString() ?? key?.ToString() ?? "";
                    var item = new CheckItem
                    {
                        Key = key,
                        Display = display,
                        Source = src,
                        IsChecked = key != null && selected.Contains(key),
                        SearchText = Normalize(key?.ToString()) + "\u0001" + Normalize(display)
                    };
                    item.PropertyChanged += Item_PropertyChanged;
                    list.Add(item);
                }
            }

            _items = list;
            SetView();
            UpdateText();
            UpdateSelectAllState();
        }

        private void SetView()
        {
            _view = new ListCollectionView(_items) { Filter = FilterItem };
            PART_List.ItemsSource = _view;
        }

        private static object Resolve(object obj, string path)
        {
            if (obj == null || string.IsNullOrEmpty(path)) return obj;
            foreach (var part in path.Split('.'))
            {
                if (obj == null) return null;
                var prop = obj.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                    throw new InvalidOperationException($"Không tìm thấy thuộc tính '{part}' trên kiểu {obj.GetType().Name}.");
                obj = prop.GetValue(obj, null);
            }
            return obj;
        }

        private static IEnumerable<object> ToObjects(IEnumerable source) =>
            source == null ? Enumerable.Empty<object>() : source.Cast<object>().Where(o => o != null);

        #endregion

        #region Selection sync

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_suppress || e.PropertyName != nameof(CheckItem.IsChecked)) return;
            PushSelectedKeys();
        }

        private void PushSelectedKeys()
        {
            _pushing = true;
            try
            {
                SetCurrentValue(SelectedKeysProperty,
                    _items.Where(i => i.IsChecked).Select(i => i.Key).ToArray());
            }
            finally { _pushing = false; }

            UpdateText();
            UpdateSelectAllState();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        // SelectedKeys bị gán từ bên ngoài (ViewModel) -> cập nhật checkbox
        private void OnSelectedKeysChanged()
        {
            if (_pushing) return;
            var selected = new HashSet<object>(ToObjects(SelectedKeys));
            _suppress = true;
            try
            {
                foreach (var it in _items)
                    it.IsChecked = it.Key != null && selected.Contains(it.Key);
            }
            finally { _suppress = false; }
            UpdateText();
            UpdateSelectAllState();
        }

        private void SetAll(IEnumerable<CheckItem> items, bool value)
        {
            _suppress = true;
            try { foreach (var it in items) it.IsChecked = value; }
            finally { _suppress = false; }
            PushSelectedKeys();
        }

        #endregion

        #region Search / filter

        private bool FilterItem(object o)
        {
            if (_tokens.Length == 0) return true;
            var text = ((CheckItem)o).SearchText;
            // Mọi từ khóa đều phải xuất hiện (trong key hoặc display)
            return _tokens.All(t => text.Contains(t));
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            _tokens = Normalize(PART_Search.Text)
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            _view.Refresh();
            UpdateSelectAllState();
        }

        /// <summary>Chữ thường + bỏ dấu tiếng Việt, để "ha noi" khớp "Hà Nội".</summary>
        private static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var d = s.Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(d.Length);
            foreach (var c in d)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        #endregion

        #region UI

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            var visible = _view.Cast<CheckItem>().ToList();
            bool check = !(visible.Count > 0 && visible.All(i => i.IsChecked));
            SetAll(visible, check);
        }

        private void UpdateSelectAllState()
        {
            if (PART_SelectAll == null || _view == null) return;
            var visible = _view.Cast<CheckItem>().ToList();
            int n = visible.Count(i => i.IsChecked);
            PART_SelectAll.IsChecked = n == 0 ? false : n == visible.Count ? true : (bool?)null;
            PART_SelectAll.IsEnabled = visible.Count > 0;
        }

        private void UpdateText()
        {
            if (PART_Text == null) return;
            var sel = _items.Where(i => i.IsChecked).ToList();
            if (sel.Count == 0)
            {
                PART_Text.Text = Placeholder;
                PART_Text.Foreground = Brushes.Gray;
            }
            else
            {
                PART_Text.Text = sel.Count <= MaxDisplayItems
                    ? string.Join(", ", sel.Select(i => i.Display))
                    : $"Đã chọn {sel.Count} mục";
                PART_Text.Foreground = Brushes.Black;
            }
            PART_Toggle.ToolTip = sel.Count > 0 ? string.Join("\n", sel.Select(i => i.Display)) : null;
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            // Tránh việc bấm lại nút khi popup đang mở làm popup đóng rồi mở lại
            PART_Toggle.IsHitTestVisible = false;
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => PART_Search.Focus()));
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            PART_Toggle.IsHitTestVisible = true;
            PART_Search.Text = "";
        }

        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                IsDropDownOpen = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Down && PART_List.Items.Count > 0)
            {
                PART_List.SelectedIndex = 0;
                (PART_List.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem)?.Focus();
                e.Handled = true;
            }
        }

        private void List_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && PART_List.SelectedItem is CheckItem it)
            {
                it.IsChecked = !it.IsChecked;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        #endregion
    }
}
