using System.Threading.Tasks;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolesManager)
        {
            var roleName = "admin";
            if (await rolesManager.FindByNameAsync(roleName) == null)
            {
                await rolesManager.CreateAsync(new IdentityRole(roleName));
            }

            var adminEmail = "acm@sfu-kras.ru";
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                };

                await userManager.CreateAsync(admin, "123qweASD!");
                await userManager.AddToRoleAsync(admin, roleName);
            }
        }
    }
}
