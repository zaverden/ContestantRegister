using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace ContestantRegister.Utils
{
    public class ConvertFilterAttribute : Attribute
    {
        public ConvertFilterAttribute(Type destinationType)
        {
            if (destinationType.GetInterface(nameof(IFilverValueConverter)) == null)
                throw new ArgumentException($"Not implemented {nameof(IFilverValueConverter)}");
            
            DestinationType = destinationType;
        }

        public Type DestinationType { get; set; }
    }

    public class PropertyNameAttribute : Attribute
    {
        public PropertyNameAttribute(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public string PropertyName { get; }
    }

    public class RelatedObjectAttribute : Attribute
    {
        public RelatedObjectAttribute(string objectName)
        {
            ObjectName = objectName ?? throw new ArgumentNullException(nameof(objectName));
        }

        public string ObjectName { get; }

        public string PropertyName { get; set; }
    }

    public enum StringFilter
    {
        StartsWith,
        Contains
    }

    public class StringFilterAttribute : Attribute
    {
        public StringFilterAttribute(StringFilter stringFilter)
        {
            StringFilter = stringFilter;
        }

        public StringFilter StringFilter { get; }

        public bool IgnoreCase { get; set; }
    }

    public enum ComposeKind
    {
        And, Or
    }

    //Для IEnumerable тоже работает для скарярных свойств, но для навигационных, где нужно делать проверку на null, уже нет, наго строить другое Linq выражение. Можно добилить, если будет настроение :)
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
        public static ConventionalFilters Filters { get; } = new ConventionalFilters();
    }

    public class ConventionalFilters
    {
        public static MethodInfo StartsWith = typeof(string)
            .GetMethod("StartsWith", new[] { typeof(string) });
        public static MethodInfo StartsWithComparsion = typeof(string)
            .GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });

        public static MethodInfo Contains = typeof(string)
            .GetMethod("Contains", new[] { typeof(string) });
        public static MethodInfo IndexOfComparsion = typeof(string)
            .GetMethod("IndexOf", new[] { typeof(string), typeof(StringComparison) });

        private static Dictionary<Type, Func<MemberExpression, Expression, Expression>> _filters
            = new Dictionary<Type, Func<MemberExpression, Expression, Expression>>()
            {
                { typeof(string),  (p, v) => Expression.Call(p, StartsWith, v) }
            };

        internal ConventionalFilters()
        {
                
        }

        public Func<MemberExpression, Expression, Expression> this[Type key]
        {
            get => _filters.ContainsKey(key)
                ? _filters[key]
                : Expression.Equal;
            set => _filters[key] = value ?? throw new ArgumentException(nameof(value));
        }
    }

    public interface IFilverValueConverter
    {
        object Convert(object value);
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

            return query.Where(expr.Compile()); //можно сделать кеш компилированных Linq выражений

        }

        private static Expression<Func<TSubject, bool>>[] GetProps<TSubject, TPredicate>(TPredicate predicate, bool checkNullNavProperties)
        {
            var filterProps = FastTypeInfo<TPredicate>
                .PublicProperties
                .ToArray();

            var filterPropsData = filterProps
                .Select(p => new
                {
                    FilterProperty = p,
                    Value = p.GetValue(predicate),
                    RelatedObjectAttribute = p.GetCustomAttribute<RelatedObjectAttribute>(),
                    PropertyNameAttribute = p.GetCustomAttribute<PropertyNameAttribute>(),
                    ConverAttribute = p.GetCustomAttribute<ConvertFilterAttribute>(),
                })
                .Where(obj => obj.Value != null)
                .Select(obj => new
                {
                    obj.FilterProperty,
                    Value = obj.ConverAttribute != null ?
                                ((IFilverValueConverter)Activator.CreateInstance(obj.ConverAttribute.DestinationType)).Convert(obj.Value) : 
                                obj.Value,
                    obj.RelatedObjectAttribute,
                    Name = obj.PropertyNameAttribute != null ? obj.PropertyNameAttribute.PropertyName : obj.FilterProperty.Name,
                })
                .ToArray();

            var modelType = typeof(TSubject);
            var stringType = typeof(string);
            var dateTimeType = typeof(DateTime);
            var dateTimeNullableType = typeof(DateTime?);

            var parameter = Expression.Parameter(modelType);

            var subjectNotnavigationProps = FastTypeInfo<TSubject>
                .PublicProperties
                .Join(filterPropsData.Where(x => x.RelatedObjectAttribute == null), fp => fp.Name, sp => sp.Name, (propInfo, filter) => new FilterPropertyInfo
                {
                    Property = propInfo,
                    Value = filter.Value,
                    FilterProperty = filter.FilterProperty,                    
                    RelatedObjectAttribute = filter.RelatedObjectAttribute,
                })
                .ToArray();


            var props = subjectNotnavigationProps
                .Select(x =>
                {
                    return Calc<TSubject>(x, parameter, checkNullNavProperties);
                })
                .ToList();

            var navigationProperties = filterPropsData
                .Where(x => x.RelatedObjectAttribute != null)
                .Select(filter => new FilterPropertyInfo
                {
                    Value = filter.Value,
                    FilterProperty = filter.FilterProperty,
                    RelatedObjectAttribute = filter.RelatedObjectAttribute,
                });

            var props2 = navigationProperties
                .Select(x =>
                {
                    return Calc<TSubject>(x, parameter, checkNullNavProperties);
                })
                .ToArray();

            props.AddRange(props2);

            return props.ToArray();
        }

        private class FilterPropertyInfo
        {
            public PropertyInfo Property { get; set; }
            public object Value { get; set; }
            public PropertyInfo FilterProperty { get; set; }
            public RelatedObjectAttribute RelatedObjectAttribute { get; set; }
        }

        private static Expression<Func<TSubject, bool>> Calc<TSubject>(FilterPropertyInfo x, ParameterExpression parameter, bool checkNullNavProperties)
        {
            Func<MemberExpression, Expression, Expression> stringFunc = null;

            if (x.FilterProperty.PropertyType == typeof(string))
            {
                var stringFilter = x.FilterProperty.GetCustomAttribute<StringFilterAttribute>();
                if (stringFilter != null)
                {

                    switch (stringFilter.StringFilter)
                    {
                        case StringFilter.StartsWith:
                            if (stringFilter.IgnoreCase)
                            {
                                var enumVal = Expression.Constant(StringComparison.OrdinalIgnoreCase);
                                stringFunc = (p, v) => Expression.Call(p, ConventionalFilters.StartsWithComparsion, v, enumVal);
                            }
                            else
                            {
                                stringFunc = (p, v) => Expression.Call(p, ConventionalFilters.StartsWith, v);
                            }
                            break;
                        case StringFilter.Contains:
                            if (stringFilter.IgnoreCase)
                            {
                                var enumVal = Expression.Constant(StringComparison.OrdinalIgnoreCase);
                                var notFound = Expression.Constant(-1);
                                stringFunc = (p, v) =>
                                {
                                    var indexOf = Expression.Call(p, ConventionalFilters.IndexOfComparsion, v, enumVal);
                                    return Expression.NotEqual(indexOf, notFound);
                                };
                            }
                            else
                            {
                                stringFunc = (p, v) => Expression.Call(p, ConventionalFilters.Contains, v);
                            }
                            break;
                    }
                }
            }

            MemberExpression property;

            var nullchecks = new List<Expression>();
            if (x.RelatedObjectAttribute != null)
            {
                var propNames = x.RelatedObjectAttribute.ObjectName.Split('.');
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
                if (!string.IsNullOrEmpty(x.RelatedObjectAttribute.PropertyName))
                {
                    property = Expression.Property(property, x.RelatedObjectAttribute.PropertyName);
                }
                else
                {
                    property = Expression.Property(property, x.FilterProperty.Name);
                }
            }
            else
            {
                property = Expression.Property(parameter, x.Property);
            }

            Expression value = Expression.Constant(x.Value);

            value = Expression.Convert(value, property.Type); // нужна ли эта конвертация?
            var func = property.Type == typeof(string) ?
                stringFunc :
                Conventions.Filters[property.Type];

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

    }

    public delegate T ObjectActivator<out T>(params object[] args);

    public static class FastTypeInfo<T>
    {
        private static Attribute[] _attributes;

        private static PropertyInfo[] _properties;

        private static MethodInfo[] _methods;

        private static ConstructorInfo[] _constructors;

        private static ConcurrentDictionary<string, ObjectActivator<T>> _activators;

        static FastTypeInfo()
        {
            var type = typeof(T);
            _attributes = type.GetCustomAttributes().ToArray();

            _properties = type
                .GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .ToArray();

            _methods = type.GetMethods()
                .Where(x => x.IsPublic && !x.IsAbstract)
                .ToArray();

            _constructors = typeof(T).GetConstructors();
            _activators = new ConcurrentDictionary<string, ObjectActivator<T>>();
        }

        public static PropertyInfo[] PublicProperties => _properties;

        public static MethodInfo[] PublicMethods => _methods;

        public static Attribute[] Attributes => _attributes;

        public static bool HasAttribute<TAttr>()
            where TAttr : Attribute
            => Attributes.Any(x => x.GetType() == typeof(TAttr));

        public static TAttr GetCustomAttribute<TAttr>()
            where TAttr : Attribute
            => (TAttr)_attributes.FirstOrDefault(x => x.GetType() == typeof(TAttr));

        #region Create

        public static T Create(params object[] args)
            => _activators.GetOrAdd(
                GetSignature(args),
                GetActivator(GetConstructorInfo(args)))
                    .Invoke(args);

        private static string GetSignature(object[] args)
            => args
                .Select(x => x.GetType().ToString())
                .Join(",");

        private static ConstructorInfo GetConstructorInfo(object[] args)
        {
            for (var i = 0; i < _constructors.Length; i++)
            {
                var consturctor = _constructors[i];
                var ctrParams = consturctor.GetParameters();
                if (ctrParams.Length != args.Length)
                {
                    continue;
                }

                var flag = true;
                for (var j = 0; j < args.Length; i++)
                {
                    if (ctrParams[j].ParameterType != args[j].GetType())
                    {
                        flag = false;
                        break;
                    }
                }

                if (!flag)
                {
                    continue;
                }

                return consturctor;
            }

            var signature = GetSignature(args);

            throw new InvalidOperationException(
                $"Constructor ({signature}) is not found for {typeof(T)}");
        }

        private static ObjectActivator<T> GetActivator(ConstructorInfo ctor)
        {
            var type = ctor.DeclaringType;
            var paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                var index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            var newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            var compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        public static Delegate CreateMethod(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (!method.IsStatic)
            {
                throw new ArgumentException("The provided method must be static.", nameof(method));
            }

            if (method.IsGenericMethod)
            {
                throw new ArgumentException("The provided method must not be generic.", nameof(method));
            }

            var parameters = method.GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                .ToArray();

            var call = Expression.Call(null, method, parameters);
            return Expression.Lambda(call, parameters).Compile();
        }

        #endregion

        public static Func<TObject, TProperty> PropertyGetter<TObject, TProperty>(string propertyName)
        {
            var paramExpression = Expression.Parameter(typeof(TObject), "value");

            var propertyGetterExpression = Expression.Property(paramExpression, propertyName);

            var result = Expression.Lambda<Func<TObject, TProperty>>(propertyGetterExpression, paramExpression)
                .Compile();

            return result;
        }

        public static Action<TObject, TProperty> PropertySetter<TObject, TProperty>(string propertyName)
        {
            var paramExpression = Expression.Parameter(typeof(TObject));
            var paramExpression2 = Expression.Parameter(typeof(TProperty), propertyName);
            var propertyGetterExpression = Expression.Property(paramExpression, propertyName);
            var result = Expression.Lambda<Action<TObject, TProperty>>
            (
                Expression.Assign(propertyGetterExpression, paramExpression2), paramExpression, paramExpression2
            ).Compile();

            return result;
        }
    }

    public static class StringExtensions
    {
        /// <summary>
        /// Indicates whether the specified string not null or an empty string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static bool Contains(this string input, string value, StringComparison comparisonType)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return input.IndexOf(value, comparisonType) != -1;
            }

            return false;
        }

        public static bool LikewiseContains(this string input, string value)
        {
            return Contains(input, value, StringComparison.CurrentCulture);
        }

        public static string ToString(this int value, string oneForm, string twoForm, string fiveForm)
        {
            var significantValue = value % 100;

            if (significantValue >= 10 && significantValue <= 20)
                return $"{value} {fiveForm}";

            var lastDigit = value % 10;
            switch (lastDigit)
            {
                case 1:
                    return $"{oneForm}";
                case 2:
                case 3:
                case 4:
                    return $"{twoForm}";
            }

            return $"{fiveForm}";

        }

        public static string ToUnderscoreCase(this string str)
        {
            return string
                .Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
        }
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
