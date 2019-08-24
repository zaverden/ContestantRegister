using ContestantRegister.Utils.Filter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace ContestantRegister.Utils.Filter
{    
    public enum ComposeKind
    {
        And, Or
    }

    public static class ConventionsExtensions
    {
        public static IQueryable<TSubject> AutoFilter<TSubject, TPredicate>(
            this IQueryable<TSubject> query, TPredicate predicate, ComposeKind composeKind = ComposeKind.And)
        {
            var filtered = Conventions<TSubject>.Filter(query, predicate, composeKind);
            return filtered;            
        }

        public static IEnumerable<TSubject> AutoFilter<TSubject, TPredicate>(
            this IEnumerable<TSubject> query, TPredicate predicate, ComposeKind composeKind = ComposeKind.And)
        {
            var filtered = Conventions<TSubject>.Filter(query, predicate, composeKind);
            return filtered;
        }
    }

    public static class Conventions
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
            if (filterPropertyInfo.Property.PropertyType == typeof(string))
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
    
    public class FilterPropertyInfo
    {
        public PropertyInfo Property { get; set; }
        public object Value { get; set; }
        public FilterProperty FilterProperty { get; set; }
    }

    public class FilterProperty
    {
        public PropertyInfo PropertyInfo { get; set; }
        public RelatedObjectAttribute RelatedObjectAttribute { get; set; }
        public string ObjectPropertyName { get; set; }
        public StringFilterAttribute StringFilterAttribute { get; set; }
        public IFilverValueConverter Converter { get; set; }
        public FilterConditionAttribute FilterConditionAttribute { get; set; }
    }

    public static class Conventions<TSubject>
    {
        public static IQueryable<TSubject> Filter<TPredicate>(IQueryable<TSubject> query,
            TPredicate predicate,
            ComposeKind composeKind = ComposeKind.And)
        {
            var props = GetProps<TSubject, TPredicate>(predicate, false);

            if (!props.Any())
            {
                return query;
            }

            var expr = composeKind == ComposeKind.And
                ? props.Aggregate((c, n) => c.And(n))
                : props.Aggregate((c, n) => c.Or(n));

            return query.Where(expr);

        }

        public static IEnumerable<TSubject> Filter<TPredicate>(IEnumerable<TSubject> query,
            TPredicate predicate,
            ComposeKind composeKind = ComposeKind.And)
        {
            var props = GetProps<TSubject, TPredicate>(predicate, true);

            if (!props.Any())
            {
                return query;
            }

            var expr = composeKind == ComposeKind.And
                ? props.Aggregate((c, n) => c.And(n))
                : props.Aggregate((c, n) => c.Or(n));

            return query.Where(expr.AsFunc()); 

        }

        private static Expression<Func<TSubject, bool>>[] GetProps<TSubject, TPredicate>(TPredicate predicate, bool checkNullNavProperties)
        {
            var filterProps = FastFilterPropertyInfo<TPredicate>
                .PublicProperties
                .ToArray();

            var filterPropsData = filterProps
                .Select(x => new
                {
                    FilterProperty = x,
                    Value = x.PropertyInfo.GetValue(predicate),
                })
                .Select(x => new
                {
                    x.FilterProperty,
                    Value = x.FilterProperty.Converter != null && x.Value != null ?
                                x.FilterProperty.Converter.Convert(x.Value) :
                                x.Value,
                })
                .Where(x => x.Value != null)
                .ToArray();

            var modelType = typeof(TSubject);
            var stringType = typeof(string);
            var dateTimeType = typeof(DateTime);
            var dateTimeNullableType = typeof(DateTime?);

            var parameter = Expression.Parameter(modelType);

            var subjectNotnavigationProps = FastPropertyInfo<TSubject>
                .PublicProperties
                .Join(filterPropsData.Where(x => x.FilterProperty.RelatedObjectAttribute == null), p => p.Name, f => f.FilterProperty.ObjectPropertyName, (propInfo, filter) => new FilterPropertyInfo
                {
                    Property = propInfo,
                    Value = filter.Value,
                    FilterProperty = filter.FilterProperty,                    
                })
                .ToArray();


            var props = subjectNotnavigationProps
                .Select(x => Calc<TSubject>(x, parameter, checkNullNavProperties))
                .ToList();

            var navigationProperties = filterPropsData
                .Where(x => x.FilterProperty.RelatedObjectAttribute != null)
                .Select(filter => new FilterPropertyInfo
                {
                    Value = filter.Value,
                    FilterProperty = filter.FilterProperty,                    
                });

            var props2 = navigationProperties
                .Select(x => Calc<TSubject>(x, parameter, checkNullNavProperties))
                .ToArray();

            props.AddRange(props2);

            return props.ToArray();
        }

        

        private static Expression<Func<TSubject, bool>> Calc<TSubject>(FilterPropertyInfo x, ParameterExpression parameter, bool checkNullNavProperties)
        {
            

            MemberExpression property;

            var nullchecks = new List<Expression>();
            if (x.FilterProperty.RelatedObjectAttribute != null)
            {
                var propNames = x.FilterProperty.RelatedObjectAttribute.ObjectName.Split('.');
                property = Expression.Property(parameter, propNames[0]);
                var nullExpression = Expression.Constant(null);
                
                if (checkNullNavProperties)
                {
                    var nullCheck = Expression.NotEqual(property, nullExpression);
                    nullchecks.Add(nullCheck);
                }
                for (int i = 1; i < propNames.Length; i++)
                {
                    property = Expression.Property(property, propNames[i]);
                    if (checkNullNavProperties)
                    {
                        var nullCheck = Expression.NotEqual(property, nullExpression);
                        nullchecks.Add(nullCheck);
                    }
                }
                if (!string.IsNullOrEmpty(x.FilterProperty.RelatedObjectAttribute.PropertyName))
                {
                    property = Expression.Property(property, x.FilterProperty.RelatedObjectAttribute.PropertyName);
                }
                else
                {
                    property = Expression.Property(property, x.FilterProperty.ObjectPropertyName);
                }
            }
            else
            {
                property = Expression.Property(parameter, x.Property);
            }

            Expression value = Expression.Constant(x.Value);

            value = Expression.Convert(value, property.Type); //например для конвертирования enum в object или int? в int

            var func = Conventions.GetFilterBuilderFunc(x);

            var body = func(property, value);

            if (checkNullNavProperties && nullchecks.Any())
            {
                var expr = nullchecks[0];
                for (int i = 1; i < nullchecks.Count; i++)
                {
                    expr = Expression.AndAlso(expr, nullchecks[i]);
                }
                body = Expression.AndAlso(expr, body);                
            }

            var res = Expression.Lambda<Func<TSubject, bool>>(body, parameter);

            return res;
        }

        

        private static class FastFilterPropertyInfo<T>
        {
            static FastFilterPropertyInfo()
            {
                PublicProperties = FastPropertyInfo<T>
                    .PublicProperties
                    .Select(x => new
                    {
                        PropertyInfo = x,
                        PropertyNameAttribute = x.GetCustomAttribute<PropertyNameAttribute>(),
                        ConvertFilterAttribute = x.GetCustomAttribute<ConvertFilterAttribute>(),                        
                    })
                    .Select(x => new FilterProperty
                    {
                        PropertyInfo = x.PropertyInfo,
                        ObjectPropertyName = x.PropertyNameAttribute != null ?
                                                x.PropertyNameAttribute.PropertyName :
                                                x.PropertyInfo.Name,
                        Converter = x.ConvertFilterAttribute != null ?
                                        ((IFilverValueConverter)Activator.CreateInstance(x.ConvertFilterAttribute.DestinationType)) :
                                        null,

                        RelatedObjectAttribute = x.PropertyInfo.GetCustomAttribute<RelatedObjectAttribute>(),
                        StringFilterAttribute = x.PropertyInfo.GetCustomAttribute<StringFilterAttribute>(),
                        FilterConditionAttribute = x.PropertyInfo.GetCustomAttribute<FilterConditionAttribute>()
                    })
                    .ToArray();
            }

            public static FilterProperty[] PublicProperties { get; }

        }

        private static class FastPropertyInfo<T>
        {
            static FastPropertyInfo()
            {
                var type = typeof(T);
                PublicProperties = type
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite)
                    .ToArray();
            }

            public static PropertyInfo[] PublicProperties { get; }

        }
    }

    internal class CompiledExpressions<TIn, TOut>
    {
        private static readonly ConcurrentDictionary<Expression<Func<TIn, TOut>>, Func<TIn, TOut>> Cache
            = new ConcurrentDictionary<Expression<Func<TIn, TOut>>, Func<TIn, TOut>>();

        internal static Func<TIn, TOut> AsFunc(Expression<Func<TIn, TOut>> expr)
            => Cache.GetOrAdd(expr, k => k.Compile());
    }

    public static class ExpressionExtensions
    {
        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this Expression<Func<TIn, TOut>> expr)
            => CompiledExpressions<TIn, TOut>.AsFunc(expr);

    }

    public static class PredicateBuilder
    {
        /// <summary>
        /// Creates a predicate that evaluates to true.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        /// <summary>
        /// Creates a predicate that evaluates to false.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>
        /// Creates a predicate expression from the specified lambda expression.
        /// </summary>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

        /// <summary>
        /// Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose<Func<T, bool>>(second, Expression.AndAlso);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose<Func<T, bool>>(second, Expression.OrElse);
        }

        /// <summary>
        /// Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        public static Expression<T> Compose<T>(this LambdaExpression first, LambdaExpression second,
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
            
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> _map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (_map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}
