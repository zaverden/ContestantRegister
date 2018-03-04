using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Utils
{
    public class Roles
    {
        public const string Admin = "admin";
    }

    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolesManager, ApplicationDbContext context)
        {
            var krs = await context.Cities.SingleOrDefaultAsync(c => c.Name == "Красноярск");
            if (krs == null)
            {
                krs = new City {Name = "Красноярск"};
                context.Cities.Add(krs);
                await context.SaveChangesAsync();
            }

            var isit = await context.Institutions.SingleOrDefaultAsync(i => i.ShortName == "ИКИТ СФУ");
            if (isit == null)
            {
                isit = new Institution
                {
                    City = krs,
                    ShortName = "ИКИТ СФУ",
                    FullName = "Институт Космических и Информационных Технологий Сибирского Федерального Университета",
                    Site = "https://ikit.sfu-kras.ru",
                    ShortNameEnglish = "ISIT SFU",
                    FullNameEnglish = "Institute of Space and Information Technology of Siberian Federal University",
                };
                context.Institutions.Add(isit);
                await context.SaveChangesAsync();
            }

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
                    StudyPlace = isit,
                    Name = "Админ",
                    Surname = "Админ",
                    Patronymic = "Админ",
                    UserType = UserType.Trainer

                };

                await userManager.CreateAsync(admin, "123qweASD!");
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}
