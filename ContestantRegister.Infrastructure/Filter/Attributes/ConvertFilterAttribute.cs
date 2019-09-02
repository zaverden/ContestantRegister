using System;

namespace ContestantRegister.Infrastructure.Filter.Attributes
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
}
