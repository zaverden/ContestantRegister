using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ContestantRegister.Infrastructure.Extensions;
using ContestantRegister.Infrastructure.Filter.Attributes;

namespace ContestantRegister.Infrastructure.Filter
{
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

        public static IOrderedQueryable<TSubject> Sort(IQueryable<TSubject> query, string propertyName)
        {
            (string, bool) GetSorting()
            {
                var arr = propertyName.Split('.');
                if (arr.Length == 1)
                    return (arr[0], false);
                var sort = arr[1];
                if (string.Equals(sort, "ASC", StringComparison.CurrentCultureIgnoreCase))
                    return (arr[0], false);
                if (string.Equals(sort, "DESC", StringComparison.CurrentCultureIgnoreCase))
                    return (arr[0], true);
                return (arr[0], false);
            }

            var (name, isDesc) = GetSorting();
            propertyName = name;

            var property = FastTypeInfo
                .GetPublicProperties(typeof(TSubject))
                .FirstOrDefault(x => string.Equals(x.Name, propertyName, StringComparison.CurrentCultureIgnoreCase));

            if (property == null)
                throw new InvalidOperationException($"There is no public property \"{propertyName}\" " +
                                                    $"in type \"{typeof(TSubject)}\"");

            var parameter = Expression.Parameter(typeof(TSubject));
            var body = Expression.Property(parameter, propertyName);

            var lambda = FastTypeInfo
                .GetPublicMethods(typeof(Expression))
                .First(x => x.Name == "Lambda");

            lambda = lambda.MakeGenericMethod(typeof(Func<,>)
                .MakeGenericType(typeof(TSubject), property.PropertyType));

            var expression = lambda.Invoke(null, new object[] { body, new[] { parameter } });

            var methodName = isDesc ? "OrderByDescending" : "OrderBy";

            var orderBy = typeof(Queryable)
                .GetMethods()
                .First(x => x.Name == methodName && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TSubject), property.PropertyType);

            return (IOrderedQueryable<TSubject>)orderBy.Invoke(query, new object[] { query, expression });
        }

        private static Expression<Func<TSubject, bool>>[] GetProps<TSubject, TPredicate>(TPredicate predicate, bool checkNullNavProperties)
        {
            var filterProps = FastFilterPropertyInfo.GetPublicProperties(predicate.GetType())
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

            var parameter = Expression.Parameter(modelType);

            var subjectNotnavigationProps = FastTypeInfo
                .GetPublicProperties(typeof(TSubject))
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

            var func = FilterPropertyInfoFuncGenerator.GetFilterBuilderFunc(x);

            var body = func(property, value);

            if (checkNullNavProperties && nullchecks.Any())
            {
                var expr = nullchecks.Aggregate((cur, next) => Expression.AndAlso(cur, next));
                body = Expression.AndAlso(expr, body);
            }

            var res = Expression.Lambda<Func<TSubject, bool>>(body, parameter);

            return res;
        }

        private static class FastFilterPropertyInfo
        {
            private static readonly ConcurrentDictionary<Type, FilterProperty[]> Cache = new ConcurrentDictionary<Type, FilterProperty[]>();

            public static FilterProperty[] GetPublicProperties(Type type)
            {
                return Cache.GetOrAdd(type, CalcFilterProperties(type));
            }

            private static FilterProperty[] CalcFilterProperties(Type type)
            {
                var props = FastTypeInfo
                    .GetPublicProperties(type)
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

                return props;
            }
        }
    }
}
