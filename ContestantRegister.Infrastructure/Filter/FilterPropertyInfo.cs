using System.Reflection;

namespace ContestantRegister.Infrastructure.Filter
{
    public class FilterPropertyInfo
    {
        public PropertyInfo Property { get; set; }
        public object Value { get; set; }
        public FilterProperty FilterProperty { get; set; }
    }
}
