using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ContestantRegister.Models;
using ContestantRegister.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ContestantRegister.Services
{
    public static class Extensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static async Task<bool> IsUserAdminAsync(this UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated) return false;

            var user = await userManager.GetUserAsync(principal);
            return await userManager.IsInRoleAsync(user, Roles.Admin);
        }

        public static string GetRequredFieldErrorMessage(this Object obj, string propertyName)
        {
            var displayAttribute = (DisplayAttribute)obj.GetType().GetProperty(propertyName).GetCustomAttribute(typeof(DisplayAttribute));
            return string.Format(Resource.RequiredFieldErrorMessage, displayAttribute.Name);
        }

        public static void AddErrors(this ModelStateDictionary model, IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                model.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
