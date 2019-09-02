using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using ContestantRegister.Domain.Properties;

namespace ContestantRegister.Services
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
