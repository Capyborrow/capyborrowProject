using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;

namespace capyborrowProject.Data
{
    public class APIContext : DbContext
    {

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attandance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public APIContext(DbContextOptions<APIContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships here
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId);

            modelBuilder.Entity<Subject>()
                .HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Subject)
                .WithMany()
                .HasForeignKey(l => l.SubjectId);

            modelBuilder.Entity<Lesson>()
                .HasOne(s => s.Group)
                .WithMany()
                .HasForeignKey(s => s.GroupId);

            modelBuilder.Entity<Assignment>()
               .HasOne(a => a.Lesson)
               .WithMany()
               .HasForeignKey(a => a.LessonId);

            modelBuilder.Entity<Attandance>()
               .HasOne(a => a.Lesson)
               .WithMany()
               .HasForeignKey(a => a.LessonId);

            modelBuilder.Entity<Attandance>()
               .HasOne(a => a.Student)
               .WithMany()
               .HasForeignKey(a => a.StudentId);

            modelBuilder.Entity<Grade>()
               .HasOne(g => g.Lesson)
               .WithMany()
               .HasForeignKey(g => g.LessonId);

            modelBuilder.Entity<Grade>()
               .HasOne(g => g.Student)
               .WithMany()
               .HasForeignKey(g => g.StudentId);

            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Lesson)
               .WithMany()
               .HasForeignKey(n => n.LessonId);

            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Teacher)
               .WithMany()
               .HasForeignKey(n => n.TeacherId);
        }
    }
}
