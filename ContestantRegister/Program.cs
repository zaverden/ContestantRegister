﻿using System;
using System.Collections.Generic;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Services.BackgroundJobs;
using ContestantRegister.Utils;
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
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    RoleInitializer.InitializeAsync(userManager, rolesManager, context).Wait();

                    //PopulateTestData(userManager, context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            JobManager.JobFactory = new JobFactory(host.Services);
            JobManager.Initialize(new FluentSchedulerRegistry());
            JobManager.JobException += info =>
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(info.Exception, "Unhandled exception in email job");
            };

            host.Run();
        }

        private static void PopulateTestData(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            var cities = new List<City>();
            int cityCount = 2;
            for (int i = 1; i <= cityCount; i++)
            {
                var city = new City { Name = $"Город{i}" };
                cities.Add(city);
            }
            context.Cities.AddRange(cities);
            context.SaveChanges();

            var schools = new List<School>();
            int schoolCount = 2;
            for (int i = 1; i <= cityCount; i++)
            {
                for (int j = 1; j <= schoolCount; j++)
                {
                    var sc = new School
                    {
                        ShortName = $"Школа{j} Город{i}",
                        FullName = $"Школа{j} Город{i} полное имя",
                        City = cities[i - 1],
                        Email = $"school{j}@city{i}.ru",
                        Site = $"http://school{j}@city{i}.ru"
                    };
                    schools.Add(sc);
                }
            }
            context.Schools.AddRange(schools);
            context.SaveChanges();
            
            var institutions = new List<Institution>();
            int instCount = 2;
            for (int i = 1; i <= cityCount; i++)
            {
                for (int j = 1; j <= instCount; j++)
                {
                    var inst = new Institution
                    {
                        ShortName = $"Вуз{j} Город{i}",
                        FullName = $"Вуз{j} Город{i} полное имя",
                        City = cities[i - 1],
                        Site = $"http://institution{j}@city{i}.ru",
                        ShortNameEnglish = $"Institution{j} City{i}",
                        FullNameEnglish = $"Institution{j} City{i} full name",
                    };
                    institutions.Add(inst);
                }
            }
            context.Institutions.AddRange(institutions);
            context.SaveChanges();
            
            int pupilCount = 2;
            for (int i = 1; i <= cityCount; i++)
            {
                for (int j = 1; j <= schoolCount; j++)
                {
                    for (int k = 1; k <= pupilCount; k++)
                    {
                        userManager.CreateAsync(new ApplicationUser
                        {
                            UserName = $"pupil{k}@school{j}.city{i}",
                            Email = $"pupil{k}@school{j}.city{i}",
                            EmailConfirmed = true,

                            UserType = UserType.Pupil,
                            Name = $"Учен{k}Школ{j}Гор{i}Имя",
                            Surname = $"Учен{k}Школ{j}Гор{i}Фам",
                            Patronymic = $"Учен{k}Школ{j}Гор{i}Отч",

                            StudyPlace = schools[(i-1)*2 +j-1]

                        }, "123456").Wait();
                    }
                }
            }

            int studentCount = 2;
            for (int i = 1; i <= instCount; i++)
            {
                for (int j = 1; j <= instCount; j++)
                {
                    for (int k = 1; k <= studentCount; k++)
                    {
                        userManager.CreateAsync(new ApplicationUser
                        {
                            UserName = $"student{k}@inst{j}.city{i}",
                            Email = $"student{k}@inst{j}.city{i}",
                            EmailConfirmed = true,

                            UserType = UserType.Student,
                            Name = $"Студ{k}Вуз{j}Гор{i}Имя",
                            Surname = $"Студ{k}Вуз{j}Гор{i}Фам",
                            Patronymic = $"Студ{k}Вуз{j}Гор{i}Отч",
                            FirstName = $"Stud{k}Inst{j}City{i}FName",
                            LastName = $"Stud{k}Inst{j}City{i}LName",

                            EducationStartDate = new DateTime(2017, 9, 1),
                            EducationEndDate = new DateTime(2022, 6, 1),
                            DateOfBirth = new DateTime(1995, 1, 1),

                            StudyPlace = institutions[(i - 1) * 2 + j - 1]

                        }, "123456").Wait();
                    }
                }
            }

            for (int i = 1; i <= cityCount; i++)
            {
                for (int j = 1; j <= schoolCount; j++)
                {
                    userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = $"trainer@school{j}.city{i}",
                        Email = $"trainer@school{j}.city{i}",
                        EmailConfirmed = true,

                        UserType = UserType.Trainer,
                        Name = $"ТренШкол{j}Гор{i}Имя",
                        Surname = $"ТренШкол{j}Гор{i}Фам",
                        Patronymic = $"ТренШкол{j}Гор{i}Отч",
                        FirstName = $"TrainSchool{j}City{i}FName",
                        LastName = $"TrainSchool{j}City{i}LName",

                        StudyPlace = schools[(i-1)*2 + j - 1],

                        PhoneNumber = "123456789",

                    }, "123456").Wait();
                }
            }

            for (int i = 1; i <= cityCount; i++)
            {
                for (int j = 1; j <= instCount; j++)
                {
                    userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = $"trainer@inst{j}.city{i}",
                        Email = $"trainer@inst{j}.city{i}",
                        EmailConfirmed = true,

                        UserType = UserType.Trainer,
                        Name = $"ТренИнст{j}Гор{i}Имя",
                        Surname = $"ТренИнст{j}Гор{i}Фам",
                        Patronymic = $"ТренИнст{j}Гор{i}Отч",
                        FirstName = $"TrainInst{j}City{i}FName",
                        LastName = $"TrainInst{j}City{i}LName",

                        StudyPlace = institutions[(i-1)*2 + j - 1],

                        PhoneNumber = "123456789",

                    }, "123456").Wait();
                }
            }

        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }






}
