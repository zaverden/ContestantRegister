using System.Reflection;
using ContestantRegister.Framework.Filter.Attributes;

namespace ContestantRegister.Framework.Filter
{
    public class FilterProperty
    {
        public PropertyInfo PropertyInfo { get; set; }
        public RelatedObjectAttribute RelatedObjectAttribute { get; set; }
        public string ObjectPropertyName { get; set; }
        public StringFilterAttribute StringFilterAttribute { get; set; }
        public IFilverValueConverter Converter { get; set; }
        public FilterConditionAttribute FilterConditionAttribute { get; set; }
    }
}
