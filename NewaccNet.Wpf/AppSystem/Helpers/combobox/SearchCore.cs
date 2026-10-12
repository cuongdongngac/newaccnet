using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

// File này KHÔNG phụ thuộc WPF/DevExpress -> unit test được.
namespace NewaccNet.Wpf.Controls.Search
{
    /// <summary>Tìm theo gì.</summary>
    public enum SearchMode
    {
        Code,   // mã bắt đầu bằng chuỗi gõ vào
        Name,   // tên chứa tất cả các từ gõ vào
        All     // mã hoặc tên
    }

    /// <summary>Ô nhập hiển thị trường nào sau khi chọn.</summary>
    public enum SearchDisplay
    {
        Name,   // VD: danh mục sản phẩm, khách hàng
        Code    // VD: tài khoản kế toán
    }

    /// <summary>
    /// Model tự khai báo mã/tên.
    /// LƯU Ý: phải implement PUBLIC (không tường minh), vì DevExpress lọc theo tên property
    /// "SearchCode"/"SearchName" qua CriteriaOperator.
    /// </summary>
    public interface ISearchItem
    {
        string SearchCode { get; }
        string SearchName { get; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class SearchCodeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public sealed class SearchNameAttribute : Attribute { }

    /// <summary>Cấu hình mặc định cho cả loại danh mục (đặt trên class model).</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class SearchProfileAttribute : Attribute
    {
        public SearchDisplay Display { get; set; } = SearchDisplay.Name;
        public SearchMode DefaultMode { get; set; } = SearchMode.All;
        public string CodeHeader { get; set; }
        public string NameHeader { get; set; }
    }

    /// <summary>Cấu hình tìm kiếm đã resolve cho một loại item.</summary>
    public sealed class SearchProfile
    {
        public string CodeField { get; }
        public string NameField { get; }
        public SearchDisplay Display { get; }
        public SearchMode DefaultMode { get; }
        public string CodeHeader { get; }
        public string NameHeader { get; }

        public string DisplayField => Display == SearchDisplay.Code ? CodeField : NameField;

        public SearchProfile(string codeField, string nameField,
                             SearchDisplay display = SearchDisplay.Name,
                             SearchMode defaultMode = SearchMode.All,
                             string codeHeader = null, string nameHeader = null)
        {
            if (string.IsNullOrEmpty(codeField)) throw new ArgumentNullException(nameof(codeField));
            if (string.IsNullOrEmpty(nameField)) throw new ArgumentNullException(nameof(nameField));
            CodeField = codeField;
            NameField = nameField;
            Display = display;
            DefaultMode = defaultMode;
            CodeHeader = codeHeader ?? SearchProfiles.DefaultCodeHeader;
            NameHeader = nameHeader ?? SearchProfiles.DefaultNameHeader;
        }
    }

    /// <summary>
    /// Nơi resolve cấu hình theo kiểu item. Thứ tự ưu tiên:
    ///   1. SearchProfiles.Register&lt;T&gt;(...)   (code, cho DTO không sửa được)
    ///   2. [SearchCode]/[SearchName] + [SearchProfile] trên model
    ///   3. ISearchItem (public)
    /// (Thuộc tính đặt trên Behavior trong XAML được ưu tiên cao nhất - xử lý ở Behavior.)
    /// </summary>
    public static class SearchProfiles
    {
        public static string DefaultCodeHeader { get; set; } = "Mã";
        public static string DefaultNameHeader { get; set; } = "Tên";

        private static readonly ConcurrentDictionary<Type, SearchProfile> Registered
            = new ConcurrentDictionary<Type, SearchProfile>();
        private static readonly ConcurrentDictionary<Type, SearchProfile> Cache
            = new ConcurrentDictionary<Type, SearchProfile>();

        /// <summary>Đăng ký bằng code. Gọi một lần lúc khởi động (App.OnStartup).</summary>
        public static void Register<T>(Expression<Func<T, object>> code,
                                       Expression<Func<T, object>> name,
                                       SearchDisplay display = SearchDisplay.Name,
                                       SearchMode defaultMode = SearchMode.All,
                                       string codeHeader = null, string nameHeader = null)
        {
            Registered[typeof(T)] = new SearchProfile(GetPath(code), GetPath(name),
                                                      display, defaultMode, codeHeader, nameHeader);
            Cache.Clear();
        }

        /// <summary>Trả null nếu kiểu không có cấu hình nào.</summary>
        public static SearchProfile Resolve(Type itemType)
        {
            if (itemType == null) return null;
            return Cache.GetOrAdd(itemType, Build);
        }

        private static SearchProfile Build(Type t)
        {
            for (Type x = t; x != null; x = x.BaseType)
            {
                SearchProfile p;
                if (Registered.TryGetValue(x, out p)) return p;
            }

            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string code = props.FirstOrDefault(p => p.IsDefined(typeof(SearchCodeAttribute), true))?.Name;
            string name = props.FirstOrDefault(p => p.IsDefined(typeof(SearchNameAttribute), true))?.Name;

            if (typeof(ISearchItem).IsAssignableFrom(t))
            {
                if (code == null) code = nameof(ISearchItem.SearchCode);
                if (name == null) name = nameof(ISearchItem.SearchName);
            }

            if (code == null || name == null) return null;

            var cls = t.GetCustomAttribute<SearchProfileAttribute>(true);
            return new SearchProfile(code, name,
                                     cls?.Display ?? SearchDisplay.Name,
                                     cls?.DefaultMode ?? SearchMode.All,
                                     cls?.CodeHeader, cls?.NameHeader);
        }

        /// <summary>x => x.A.B  ->  "A.B"</summary>
        internal static string GetPath(LambdaExpression expr)
        {
            Expression body = expr.Body;
            if (body is UnaryExpression u && u.NodeType == ExpressionType.Convert) body = u.Operand;

            var parts = new Stack<string>();
            while (body is MemberExpression m)
            {
                parts.Push(m.Member.Name);
                body = m.Expression;
            }
            if (parts.Count == 0)
                throw new ArgumentException("Biểu thức phải là truy cập property, VD: x => x.Code", nameof(expr));
            return string.Join(".", parts);
        }
    }

    /// <summary>Chữ thường + bỏ dấu tiếng Việt ("Đức" -> "duc").</summary>
    public static class TextNormalizer
    {
        public static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string decomposed = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (char c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString()
                     .Normalize(NormalizationForm.FormC)
                     .Replace('đ', 'd').Replace('Đ', 'D')
                     .ToLowerInvariant();
        }
    }

    /// <summary>Luật so khớp. Được gọi cho từng dòng từ custom function của DevExpress.</summary>
    public static class SearchMatcher
    {
        private sealed class Query
        {
            public string Raw;
            public string Normalized;
            public string[] Words;
        }

        // Cache chuỗi gõ vào gần nhất: cùng một lần lọc, query giống nhau cho mọi dòng.
        private static volatile Query _last;

        private static Query GetQuery(string raw)
        {
            raw = raw ?? "";
            Query q = _last;
            if (q != null && q.Raw == raw) return q;

            string norm = TextNormalizer.Normalize(raw).Trim();
            q = new Query
            {
                Raw = raw,
                Normalized = norm,
                Words = norm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            };
            _last = q;
            return q;
        }

        /// <summary>Mã bắt đầu bằng chuỗi gõ vào. "kh0" khớp "KH001".</summary>
        public static bool StartsWith(string value, string query)
        {
            Query q = GetQuery(query);
            if (q.Normalized.Length == 0) return true;
            return TextNormalizer.Normalize(value).StartsWith(q.Normalized, StringComparison.Ordinal);
        }

        /// <summary>Mọi từ gõ vào đều có trong giá trị. "ng an" khớp "Nguyễn Văn An".</summary>
        public static bool ContainsAllWords(string value, string query)
        {
            Query q = GetQuery(query);
            if (q.Words.Length == 0) return true;
            string v = TextNormalizer.Normalize(value);
            foreach (string w in q.Words)
            {
                if (v.IndexOf(w, StringComparison.Ordinal) < 0) return false;
            }
            return true;
        }
    }
}
