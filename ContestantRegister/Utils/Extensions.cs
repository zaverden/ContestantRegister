using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ContestantRegister.Models;
using ContestantRegister.Properties;
using ContestantRegister.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ContestantRegister.Utils
{
    public static class Extensions
    {
        public static DateTime SfuServerNow => DateTime.Now.AddHours(7);

        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Подтвердите email на сайте олимпиад ИКИТ СФУ",
                $"Подтвердите ваш email на сайте олимпиад ИКИТ СФУ olimp.ikit.sfu-kras.ru, кликнув по ссылке: <a href='{HtmlEncoder.Default.Encode(link)}'>ссылка</a>");
        }

        /// <summary>
        /// Синхронный метод нужен для использования во вьюхах
        /// </summary>
        public static ApplicationUser GetUser(this UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var task = userManager.GetUserAsync(principal);
            task.Wait();
            return task.Result;
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

        public static string ToJson<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static string[] SplitByNewLineEndAndRemoveWindowsLineEnds(this string value)
        {
            var res = value.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = res[i].TrimEnd('\r');
            }
            return res;
        }
    }
}
