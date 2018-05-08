﻿using ContestantRegister.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<Contest> Contests { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<StudyPlace> StudyPlaces { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Institution> Institutions { get; set; }

        public DbSet<ContestRegistration> ContestRegistrations { get; set; }
        public DbSet<TeamContestRegistration> TeamContestRegistrations { get; set; }
        public DbSet<IndividualContestRegistration> IndividualContestRegistrations { get; set; }

        public DbSet<Email> Emails { get; set; }

        public DbSet<CompClass> CompClasses { get; set; }
        public DbSet<ContestCompClass> ContestCompClasses { get; set; }
        public DbSet<ContestCompClassParticipant> ContestCompClassParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            builder.Entity<Institution>().HasBaseType<StudyPlace>();
            builder.Entity<School>().HasBaseType<StudyPlace>();

            builder.Entity<TeamContestRegistration>().HasBaseType<ContestRegistration>();
            builder.Entity<IndividualContestRegistration>().HasBaseType<ContestRegistration>();

            builder.Entity<ContestCompClass>()
                .HasAlternateKey(item => new {item.ContestId, item.CompClassId});

            builder.Entity<StudyPlace>(entity =>
            {
                entity.HasOne(e => e.City)
                    .WithMany(p => p.StudyPlaces)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<City>(entity =>
            {
                entity.HasOne(e => e.Region)
                    .WithMany(p => p.Cities)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(e => e.StudyPlace)
                    .WithMany(p => p.Users)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RegistredBy)
                    .WithMany(p => p.RegistredByThisUser)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ContestRegistration>(entity =>
            {
                entity.HasOne(e => e.Contest)
                    .WithMany(p => p.ContestRegistrations)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.StudyPlace)
                    .WithMany(p => p.ContestRegistrations)
                    .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(e => e.RegistredBy)
                      .WithMany(p => p.ContestRegistrationsRegistredBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Participant1)
                    .WithMany(p => p.ContestRegistrationsParticipant1)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Trainer)
                    .WithMany(p => p.ContestRegistrationsTrainer)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Manager)
                    .WithMany(p => p.ContestRegistrationsManager)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<TeamContestRegistration>(entity =>
            {
                entity.HasOne(e => e.Participant2)
                    .WithMany(p => p.ContestRegistrationsParticipant2)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Participant3)
                    .WithMany(p => p.ContestRegistrationsParticipant3)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}