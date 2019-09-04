using System;

namespace ContestantRegister.Framework.Filter.Attributes
{
    public class RelatedObjectAttribute : Attribute
    {
        public RelatedObjectAttribute(string objectName)
        {
            ObjectName = objectName ?? throw new ArgumentNullException(nameof(objectName));
        }

        public string ObjectName { get; }

        public string PropertyName { get; set; }
    }
}
