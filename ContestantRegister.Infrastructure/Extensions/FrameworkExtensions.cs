using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace ContestantRegister.Framework.Extensions
{
    public static class FrameworkExtensions
    {
        public static string GetEnumDisplayName<T>(T value)
        {
            return GetEnumDisplayName(typeof(T), value);
        }

        public static string GetEnumDisplayName(Type type, object value)
        {
            FieldInfo fieldInfo = type.GetField(value.ToString());
            DisplayAttribute attr = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }
    }
}
