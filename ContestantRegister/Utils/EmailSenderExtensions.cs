using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ContestantRegister.Properties;
using Microsoft.ApplicationInsights.AspNetCore;

namespace ContestantRegister.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }

    public static class ValidationExtensions
    {
        public static string GetRequredFieldErrorMessage(this Object obj, string propertyName)
        {
            var displayAttribute = (DisplayAttribute)obj.GetType().GetProperty(propertyName).GetCustomAttribute(typeof(DisplayAttribute));
            return string.Format(Resource.RequiredFieldErrorMessage, displayAttribute.Name);
        }
    }
}
