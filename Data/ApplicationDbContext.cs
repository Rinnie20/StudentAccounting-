using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Models;

namespace StudentAccounting.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectGrade> SubjectGrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SubjectGrade>()
                .HasIndex(sg => new { sg.StudentId, sg.SubjectId })
                .IsUnique();
            modelBuilder.Entity<SubjectGrade>()
                .HasOne(sg => sg.Student)
                .WithMany(s => s.SubjectGrades)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SubjectGrade>()
                .HasOne(sg => sg.Subject)
                .WithMany(s => s.SubjectGrades)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}