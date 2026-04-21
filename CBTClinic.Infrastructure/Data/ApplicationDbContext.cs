using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CBTClinic.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CBTClinic.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<PatientFavoriteDoctor> PatientFavoriteDoctors { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AdminReport> AdminReports { get; set; }
        public DbSet<Quizzes> Quizzes { get; set; }

        public DbSet<Questions> Questions { get; set; }

        public DbSet<Option> Options { get; set; }

        public DbSet<PatientAnswers> PatientAnswers { get; set; }

        public DbSet<PatientAssessment> PatientAssessments { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Podcast> Podcasts { get; set; }

        public DbSet<Video> Videos { get; set; }

        public DbSet<Challenge> Challenges { get; set; }






        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PatientFavoriteDoctor>()
                .HasKey(pfd => new { pfd.PatientId, pfd.DoctorId });

            // Relationships
            builder.Entity<PatientFavoriteDoctor>()
                .HasOne(pfd => pfd.Patient)
                .WithMany(p => p.FavoriteDoctors)
                .HasForeignKey(pfd => pfd.PatientId)
                .HasPrincipalKey(p => p.UserId);

            builder.Entity<PatientFavoriteDoctor>()
                .HasOne(pfd => pfd.Doctor)
                .WithMany(d => d.PatientsWhoFavorited)
                .HasForeignKey(pfd => pfd.DoctorId);

            // Fix Decimal for Quiz Price
            builder.Entity<Quizzes>()
                .Property(q => q.Price)
                .HasPrecision(18, 2);
        }
    }



}
