using System;
using System.Threading.Tasks;
using ContestantRegister.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContestantRegister
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    RoleInitializer.InitializeAsync(userManager, rolesManager).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

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

                    //TODO Logging
                    var r1 = await userManager.CreateAsync(admin, "123qweASD!");
                    if (r1.Succeeded)
                    {
                        var r2 = await userManager.AddToRoleAsync(admin, roleName);
                    }
                }
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
