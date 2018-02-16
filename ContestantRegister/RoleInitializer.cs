using System.Threading.Tasks;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister
{
    public class Roles
    {
        public const string Admin = "admin";
    }

    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolesManager)
        {
            if (await rolesManager.FindByNameAsync(Roles.Admin) == null)
            {
                await rolesManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            var adminEmail = "acm@sfu-kras.ru";
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    Email = adminEmail,
                    EmailConfirmed = true,
                    UserName = adminEmail,
                };

                await userManager.CreateAsync(admin, "123qweASD!");
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}
