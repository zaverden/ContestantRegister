using System;
using System.Collections.Generic;
using System.Text;

namespace ContestantRegister.Framework.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

    }
}
