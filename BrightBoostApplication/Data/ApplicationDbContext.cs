using BrightBoostApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BrightBoostApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }
        public virtual DbSet<Student> Student { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TermCourse> TermCourses { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public DbSet<TutorAllocation> TutorAllocations { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<StudentSignUp> StudentSignUps { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.firstName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.lastName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.isActive)
                .HasColumnName("isActive")
                .HasColumnType("bit")
                .IsRequired(false);


        }
    }
}