using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ContestantRegister.Domain.Properties;

namespace ContestantRegister.Services.Extensions
{
    public static class ValidationExtensions
    {
        public static string GetRequredFieldErrorMessage(this Object obj, string propertyName)
        {
            var displayAttribute = obj.GetType().GetProperty(propertyName).GetCustomAttribute<DisplayAttribute>();
            return string.Format(Resource.RequiredFieldErrorMessage, displayAttribute.Name);
        }
    }
}
