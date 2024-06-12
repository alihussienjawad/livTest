 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models;



namespace ClassLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);





            modelBuilder.Entity<UserSavedItem>()
         .HasKey(bc => new { bc.ApplicationUserId, bc.CourseId });

            modelBuilder.Entity<UserSavedItem>()
                 .HasOne(bc => bc.ApplicationUser)
                 .WithMany(c => c.UserSavedItems)
                 .HasForeignKey(bc => bc.ApplicationUserId);

            modelBuilder.Entity<UserSavedItem>()
                .HasOne(bc => bc.Course)
                .WithMany(b => b.UserSavedItems)
                .HasForeignKey(bc => bc.CourseId);
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<UserSavedItem> UserSavedItems { get; set; }
        public DbSet<Category> Catogoris { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseDetails> CourseDetails { get; set; }
        public virtual DbSet<CourseLearn> CourseLearns { get; set; }
        public virtual DbSet<TeacherCourse> TeacherCourses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}
