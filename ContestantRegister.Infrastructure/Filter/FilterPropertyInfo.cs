using System.Reflection;

namespace ContestantRegister.Framework.Filter
{
    public class FilterPropertyInfo
    {
        public PropertyInfo Property { get; set; }
        public object Value { get; set; }
        public FilterProperty FilterProperty { get; set; }
    }
}
