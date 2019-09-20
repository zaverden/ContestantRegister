﻿using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using ContestantRegister.BackgroundJobs;
using ContestantRegister.Data;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Features.Admin.Areas.Utils;
using ContestantRegister.Features.Admin.Cities.Utils;
using ContestantRegister.Features.Admin.Institutions.Utils;
using ContestantRegister.Features.Admin.Schools.Utils;
using ContestantRegister.Features.Frontend.Account.Utils;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Infrastructure;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;
using ContestantRegister.Services.DomainServices.ContestRegistration;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using ContestantRegister.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IMyUrlHelper = ContestantRegister.Services.InfrastructureServices.IUrlHelper;
using IUrlHelper = Microsoft.AspNetCore.Mvc.IUrlHelper;

namespace ContestantRegister
{
    public class TestCommandMiddleware : CommandHandlerMiddleware
    {
        private readonly IEmailSender _emailSender;

        public TestCommandMiddleware(object next, IEmailSender emailSender) : base(next)
        {
            _emailSender = emailSender;
        }

        public override Task HandleAsync(ICommand command)
        {
            return HandleNextAsync(command);
        }
    }

    public class TestQueryMiddleware : QueryHandlerMiddleware
    {
        private readonly IEmailSender _emailSender;

        public TestQueryMiddleware(IEmailSender emailSender, object next) : base(next)
        {
            _emailSender = emailSender;
        }

        public override async Task<object> HandleAsync(IQuery<object> query)
        {
            return await HandleNextAsync(query);
        }
    }

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

            var profiles = Assembly
                .GetEntryAssembly()
                .GetReferencedAssemblies()
                .Where(x => x.Name.StartsWith("ContestantRegister"))
                .Select(Assembly.Load)
                .SelectMany(x => x.DefinedTypes)
                .Where(type => typeof(Profile).IsAssignableFrom(type.AsType()));
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMissingTypeMaps = true;
                cfg.AddProfiles(profiles);
            });

            // Add application services.
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IContestRegistrationService, ContestRegistrationService>();

            //CQRS
            var metadata = new MiddlewareMetadata();
            metadata.AddCommandMiddleware<TestCommandMiddleware>();
            metadata.AddQueryMiddleware<TestQueryMiddleware>();
            services.AddScoped(provider => metadata);
            services.AddScoped<IHandlerDispatcher, HandlerDispatcher>();

            //Infrastructure
            services.AddScoped<IReadRepository, EfCoreReadRepository>();
            services.AddScoped<IRepository, EfCoreRepository>();
            services.AddScoped<IMyUrlHelper, MvcUrlHelper>();
            services.AddTransient<IEmailSender, EmailSender>();

            //TODO можно заменить на QuerableExecutor, который инжектируется во все хендлерыю но получаем еще одну зависимость и теряем привычный синтаксис экстеншн-методов
            QueryableExtensions.OrmExtensionsHider = new EfOrmExtensionsHider();

            //TODO можно изобрести какие-то модули или просто перейти на Autofac, там они уже есть
            //TODO еще можно регистрировать все каким-нибудь аццким рефлекшоном, но я не фанат таких решений :)
            //Admin services
            services.RegisterCitiesServices();
            services.RegisterAreasServices(); 
            services.RegisterCompClassesServices();
            services.RegisterInstitutionsServices();
            services.RegisterRegionsServices();
            services.RegisterSchoolsServices();
            services.RegisterEmailsServices();
            services.RegisterContestsServices();
            services.RegisterUsersServices();
            //Frontend services
            services.RegisterAccountServices();
            services.RegisterHomeServices();
            services.RegisterManageServices();

            services.AddMvc();

            //TODO перетащить вьюхи в папки с фичами
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new FeatureLocationExpander());
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(serviceProvider =>
            {
                var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            //Background jobs
            services.AddTransient<EmailJob>();
            services.AddTransient<ContestStatusJob>();
        }

        public void ConfigureOptions(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<MailOptions>(Configuration.GetSection("SendEmail"));
            services.Configure<SuggestStudyPlaceOptions>(Configuration.GetSection("SuggestStudyPlace"));
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