using ContestantRegister.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ContestantRegister.Utils
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string action, string controller, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: action,//nameof(AccountController.ConfirmEmail),
                controller: controller, //"Account",
                values: new { userId, code },
                protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string action, string controller, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: action,//nameof(AccountController.ResetPassword),
                controller: controller,//"Account",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
