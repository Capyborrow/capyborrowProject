using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;

namespace capyborrowProject.Data
{
    public class APIContext(DbContextOptions<APIContext> options) : DbContext(options)
    {

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships here
            modelBuilder.Entity<User>()
                .UseTpcMappingStrategy();

            // STUDENT RELATIONSHIPS
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Grades)
                .WithOne(g => g.Student)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Assignments)
                .WithMany(a => a.Students)
                .UsingEntity(j => j.ToTable("StudentAssignments"));

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // TEACHER RELATIONSHIPS
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Subjects)
                .WithOne(s => s.Teacher)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Notifications)
                .WithOne(s => s.Teacher)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            // GROUP RELATIONSHIPS
            modelBuilder.Entity<Group>()
                .HasMany(t => t.Students)
                .WithOne(s => s.Group)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // SUBJECT RELATIONSHIPS
            modelBuilder.Entity<Subject>()
                .HasOne(t => t.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            // LESSON RELATIONSHIPS
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Group)
                .WithMany()
                .HasForeignKey(l => l.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Subject)
                .WithMany()
                .HasForeignKey(l => l.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson>()
                .HasMany(l => l.Attendances)
                .WithOne(a => a.Lesson)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            // NOTIFICATION RELATIONSHIPS
            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Lesson)
               .WithMany()
               .HasForeignKey(n => n.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Teacher)
               .WithMany(n => n.Notifications)
               .HasForeignKey(n => n.TeacherId)
               .OnDelete(DeleteBehavior.Cascade);

            // ASSIGNMENT RELATIONSHIPS
            modelBuilder.Entity<Assignment>()
               .HasOne(t => t.Lesson)
               .WithMany()
               .HasForeignKey(t => t.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

            // ATTENDANCE RELATIONSHIPS
            modelBuilder.Entity<Attendance>()
               .HasOne(a => a.Lesson)
               .WithMany(b => b.Attendances)
               .HasForeignKey(a => a.LessonId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
               .HasOne(a => a.Student)
               .WithMany(b => b.Attendances)
               .HasForeignKey(a => a.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

            // GRADE RELATIONSHIPS
            modelBuilder.Entity<Grade>()
               .HasOne(t => t.Lesson)
               .WithMany()
               .HasForeignKey(s => s.LessonId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
               .HasOne(t => t.Student)
               .WithMany(s => s.Grades)
               .HasForeignKey(g => g.StudentId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
