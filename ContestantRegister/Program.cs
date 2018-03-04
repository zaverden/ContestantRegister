using System;
using ContestantRegister.Models;
using ContestantRegister.Services.BackgroundJobs;
using FluentScheduler;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

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

            //JobManager.JobFactory = new JobFactory(host.Services);
            //JobManager.Initialize(new FluentSchedulerRegistry());
            //JobManager.JobException += info =>
            //{
            //    var logger = host.Services.GetRequiredService<ILogger<Program>>();
            //    logger.LogError(info.Exception, "Unhandled exception in email job");
            //};

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }

    

    

    
}
