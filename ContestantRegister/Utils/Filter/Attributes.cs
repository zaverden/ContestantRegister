using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Utils.Filter
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

    public enum FilterCondition
    {
        Equal,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual
    }
    public class FilterConditionAttribute : Attribute
    {
        public FilterConditionAttribute(FilterCondition condition)
        {
            FilterCondition = condition;
        }

        public FilterCondition FilterCondition { get; }
    }

}
