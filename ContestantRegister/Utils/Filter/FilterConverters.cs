using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Utils.Filter
{
    public class NullableIntToNullableBooleanConverter : IFilverValueConverter
    {
        public object Convert(object v)
        {
            var value = (int?)v;
            if (!value.HasValue) return null;

            return value.Value == 1;
        }
    }

    public class EnumDisplayToValueConverter<TEnum> : IFilverValueConverter
    {
        public object Convert(object v)
        {
            var value = (string)v;

            var enumType = typeof(TEnum);
            object res = null;
            foreach (var enumVal in Enum.GetValues(enumType))
            {
                if (HtmlHelperExtensions.GetDisplayName(enumType, enumVal).ContainsIgnoreCase(value))
                {
                    if (res != null) return null; // two or more corresponding items found
                    res = enumVal;
                }
            }                
            
            return res;
        }
    }
}
