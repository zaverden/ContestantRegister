using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace ContestantRegister.Framework.Filter
{
    public static class FastTypeInfo
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        private static readonly ConcurrentDictionary<Type, MethodInfo[]> MethodsCache =
            new ConcurrentDictionary<Type, MethodInfo[]>();


        public static PropertyInfo[] GetPublicProperties(Type type)
        {
            return PropertiesCache.GetOrAdd(type, type
                .GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .ToArray());
        }

        public static MethodInfo[] GetPublicMethods(Type type)
        {
            return MethodsCache.GetOrAdd(type, type
                .GetMethods()
                .Where(x => x.IsPublic && !x.IsAbstract)
                .ToArray());

        }
    }
}
