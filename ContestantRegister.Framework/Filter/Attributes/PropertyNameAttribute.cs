using System;

namespace ContestantRegister.Framework.Filter.Attributes
{
    public class PropertyNameAttribute : Attribute
    {
        public PropertyNameAttribute(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public string PropertyName { get; }
    }
}
