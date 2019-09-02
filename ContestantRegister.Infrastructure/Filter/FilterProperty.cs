using System.Reflection;
using ContestantRegister.Infrastructure.Filter.Attributes;

namespace ContestantRegister.Infrastructure.Filter
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
