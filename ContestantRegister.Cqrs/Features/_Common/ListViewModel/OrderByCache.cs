using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ContestantRegister.Infrastructure.Filter;
using ContestantRegister.Models;

namespace ContestantRegister.Utils.ViewModelsSorting
{
    public static class OrderByCache
    {
        private static readonly ConcurrentDictionary<Type, string> Cache = new ConcurrentDictionary<Type, string>();

        public static string GetOrderBy(Type type)
        {
            return Cache.GetOrAdd(type, CalcOrderBy(type));
        }

        private static string CalcOrderBy(Type type)
        {
            var orderByProps = FastTypeInfo.GetPublicProperties(type)
                .Select(x => new
                {
                    Property = x,
                    OrderByAttribute = x.GetCustomAttribute<OrderByAttribute>()
                })
                .Where(x => x.OrderByAttribute != null)
                .ToArray();

            string orderBy = null;
            if (orderByProps.Length == 1)
            {
                orderBy = orderByProps[0].OrderByAttribute.IsDesc ?
                    $"{orderByProps[0].Property.Name}.DESC" :
                    orderByProps[0].Property.Name;
            }

            return orderBy;
        }
    }
}
