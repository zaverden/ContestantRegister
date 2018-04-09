using System.Globalization;
using AutoMapper;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services;
using ContestantRegister.Services.BackgroundJobs;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContestantRegister
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo("data-protection-keys"))
                .SetApplicationName("olimp.ikit.sfu-kras.ru");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgreConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            ConfigureOptions(services);
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IContestRegistrationService, ContestRegistrationService>();

            services.AddMvc();

            //TODO если заставить AspNet Core DI сделать Build, эту регистрацию можно убрать
            services.AddTransient<EmailJob>();
            services.AddTransient<ContestStatusJob>();
        }

        public void ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<MailOptions>(Configuration.GetSection("SendEmail"));
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context)
        {
            var ruCultureInfo = new CultureInfo("ru-RU");

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(ruCultureInfo),
                SupportedCultures = new[] { ruCultureInfo },
                SupportedUICultures = new[] { ruCultureInfo }
            });

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            context.Database.Migrate();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
