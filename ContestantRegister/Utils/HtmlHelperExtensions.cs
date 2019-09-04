using System;
using System.Linq;
using System.Text;
using ContestantRegister.Framework.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContestantRegister.Utils
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent RawJson(this IHtmlHelper html, object value)
        {
            string json = value.ToJson();
            return html.Raw(json);
        }

        public static IHtmlContent EnumAsJsMap<TEnum>(this IHtmlHelper html, bool useDisplayName = false) where TEnum : struct
        {
            return EnumAsJsMap<TEnum>(html, v => useDisplayName ? FrameworkExtensions.GetEnumDisplayName(v) : v.ToString());
        }

        private static IHtmlContent EnumAsJsMap<TEnum>(this IHtmlHelper html, Func<TEnum, string> getDisplayName) where TEnum : struct
        {
            const string mapPrefix = "new Map([";
            const string mapSuffix = "]);";
            var sb = new StringBuilder();
            foreach (TEnum value in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                int intValue = (int)Convert.ChangeType(value, TypeCode.Int32);
                string displayName = getDisplayName(value);
                sb.Append($"[{intValue},'{displayName}'],");
            }

            return html.Raw(mapPrefix + sb.ToString() + mapSuffix);
        }


        
    }
}
