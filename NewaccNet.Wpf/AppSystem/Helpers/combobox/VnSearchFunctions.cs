using System;
using DevExpress.Data.Filtering;

namespace NewaccNet.Wpf.Controls.Search
{
    /// <summary>
    /// Custom function cho CriteriaOperator: so khớp bỏ dấu tiếng Việt.
    ///   VnStartsWith([Field], 'text')
    ///   VnContainsAll([Field], 'text')
    /// </summary>
    public static class VnSearchFunctions
    {
        public const string StartsWithName = "VnStartsWith";
        public const string ContainsAllName = "VnContainsAll";

        private static readonly object Sync = new object();
        private static bool _registered;

        /// <summary>Gọi được nhiều lần; chỉ đăng ký một lần.</summary>
        public static void EnsureRegistered()
        {
            lock (Sync)
            {
                if (_registered) return;
                Register(new StringPredicateFunction(StartsWithName, SearchMatcher.StartsWith));
                Register(new StringPredicateFunction(ContainsAllName, SearchMatcher.ContainsAllWords));
                _registered = true;
            }
        }

        public static CriteriaOperator StartsWith(string field, string text)
            => Call(StartsWithName, field, text);

        public static CriteriaOperator ContainsAll(string field, string text)
            => Call(ContainsAllName, field, text);

        private static CriteriaOperator Call(string function, string field, string text)
            => new FunctionOperator(FunctionOperatorType.Custom,
                                    new OperandValue(function),
                                    new OperandProperty(field),
                                    new OperandValue(text));

        private static void Register(ICustomFunctionOperator fn)
        {
            // RegisterCustomFunction ném exception nếu tên đã được đăng ký
            if (CriteriaOperator.GetCustomFunction(fn.Name) == null)
                CriteriaOperator.RegisterCustomFunction(fn);
        }

        private sealed class StringPredicateFunction : ICustomFunctionOperator
        {
            private readonly Func<string, string, bool> _predicate;

            public StringPredicateFunction(string name, Func<string, string, bool> predicate)
            {
                Name = name;
                _predicate = predicate;
            }

            public string Name { get; }

            public object Evaluate(params object[] operands)
            {
                if (operands == null || operands.Length < 2) return false;
                return _predicate(operands[0]?.ToString(), operands[1]?.ToString());
            }

            public Type ResultType(params Type[] operands) => typeof(bool);
        }
    }
}
