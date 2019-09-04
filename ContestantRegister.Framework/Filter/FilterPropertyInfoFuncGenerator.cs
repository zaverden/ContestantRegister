using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ContestantRegister.Framework.Filter
{
    public static class FilterPropertyInfoFuncGenerator
    {
        public static MethodInfo StartsWith = typeof(string)
            .GetMethod("StartsWith", new[] { typeof(string) });

        public static MethodInfo Contains = typeof(string)
            .GetMethod("Contains", new[] { typeof(string) });

        private static string StringStartWithIgnoreCase = "StringStartWithIgnoreCase";
        private static string StringStartWith = "StringStartWith";
        private static string StringContainsIgnoreCase = "StringContainsIgnoreCase";
        private static string StringContains = "StringContains";

        private static Dictionary<string, Func<MemberExpression, Expression, Expression>> _filters
            = new Dictionary<string, Func<MemberExpression, Expression, Expression>>();

        public static Func<MemberExpression, Expression, Expression> GetFilterBuilderFunc(FilterPropertyInfo filterPropertyInfo)
        {
            var propertyType = filterPropertyInfo.Property != null ?
                filterPropertyInfo.Property.PropertyType :
                filterPropertyInfo.FilterProperty.PropertyInfo.PropertyType;

            if (propertyType == typeof(string))
                return GetStringBuilderFunc(filterPropertyInfo);

            if (filterPropertyInfo.FilterProperty.FilterConditionAttribute != null)
                return GetConditionBuilderFunc(filterPropertyInfo);

            return Expression.Equal;
        }

        private static Func<MemberExpression, Expression, Expression> GetConditionBuilderFunc(FilterPropertyInfo filterPropertyInfo)
        {
            switch (filterPropertyInfo.FilterProperty.FilterConditionAttribute.FilterCondition)
            {
                case FilterCondition.Equal: return Expression.Equal;
                case FilterCondition.Greater: return Expression.GreaterThan;
                case FilterCondition.GreaterOrEqual: return Expression.GreaterThanOrEqual;
                case FilterCondition.Less: return Expression.LessThan;
                case FilterCondition.LessOrEqual: return Expression.LessThanOrEqual;
            }

            return Expression.Equal;
        }

        private static Func<MemberExpression, Expression, Expression> GetStringBuilderFunc(FilterPropertyInfo filterPropertyInfo)
        {
            if (filterPropertyInfo.FilterProperty.StringFilterAttribute == null)
                return (p, v) => Expression.Call(p, StartsWith, v);

            Func<MemberExpression, Expression, Expression> result = null;
            string key = null;

            switch (filterPropertyInfo.FilterProperty.StringFilterAttribute.StringFilter)
            {
                case StringFilter.StartsWith:
                    if (filterPropertyInfo.FilterProperty.StringFilterAttribute.IgnoreCase)
                    {
                        if (_filters.ContainsKey(StringStartWithIgnoreCase)) return _filters[StringStartWithIgnoreCase];

                        result = (p, v) =>
                        {
                            var mi = typeof(string).GetMethod("ToLower", new Type[] { });
                            var pl = Expression.Call(p, mi);
                            var vl = Expression.Call(v, mi);
                            return Expression.Call(pl, StartsWith, vl);
                        };
                        key = StringStartWithIgnoreCase;
                    }
                    else
                    {
                        if (_filters.ContainsKey(StringStartWith)) return _filters[StringStartWith];

                        result = (p, v) => Expression.Call(p, StartsWith, v);
                        key = StringStartWith;
                    }
                    break;
                case StringFilter.Contains:
                    if (filterPropertyInfo.FilterProperty.StringFilterAttribute.IgnoreCase)
                    {
                        if (_filters.ContainsKey(StringContainsIgnoreCase)) return _filters[StringContainsIgnoreCase];

                        result = (p, v) =>
                        {
                            var mi = typeof(string).GetMethod("ToLower", new Type[] { });
                            var pl = Expression.Call(p, mi);
                            var vl = Expression.Call(v, mi);
                            return Expression.Call(pl, Contains, vl);
                        };
                        key = StringContainsIgnoreCase;
                    }
                    else
                    {
                        if (_filters.ContainsKey(StringContains)) return _filters[StringContains];

                        result = (p, v) => Expression.Call(p, Contains, v);
                        key = StringContains;
                    }
                    break;
            }

            _filters.Add(key, result);
            return result;
        }
    }
}
