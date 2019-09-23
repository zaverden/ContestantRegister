using System;
using System.Collections.Generic;
using System.Security.Claims;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ContestantRegister.Utils
{
    public static class Extensions
    {   
        /// <summary>
        /// Синхронный метод нужен для использования во вьюхах
        /// </summary>
        public static ApplicationUser GetUser(this UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var task = userManager.GetUserAsync(principal);
            task.Wait();
            return task.Result;
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
        
    }
}
