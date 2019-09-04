using System;

namespace ContestantRegister.Framework.Filter.Attributes
{
    public class StringFilterAttribute : Attribute
    {
        public StringFilterAttribute(StringFilter stringFilter)
        {
            StringFilter = stringFilter;
        }

        public StringFilter StringFilter { get; }

        public bool IgnoreCase { get; set; }
    }
}
