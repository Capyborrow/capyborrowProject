using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace capyborrowProject.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<AssignmentFile> AssignmentFiles { get; set; }
        public DbSet<SubmissionFile> SubmissionFiles { get; set; }
        public DbSet<TempAssignmentFile> TempAssignmentFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .UseTphMappingStrategy();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite primary key for StudentAssignment
            modelBuilder.Entity<StudentAssignment>()
                .HasKey(sa => new { sa.StudentId, sa.AssignmentId });

            // Student-StudentAssignment (One-to-Many)
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany(s => s.StudentAssignments)
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment-StudentAssignment (One-to-Many)
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(sa => sa.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student-Group (Many-to-One)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Composite primary key for Attendance
            modelBuilder.Entity<Attendance>()
                .HasKey(a => new { a.LessonId, a.StudentId });

            // Lesson-Attendance (One-to-Many)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Lesson)
                .WithMany(l => l.Attendances)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student-Attendance (One-to-Many)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId);

            // Lesson-Group (Many-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Group)
                .WithMany(g => g.Lessons)
                .HasForeignKey(l => l.GroupId);

            // Lesson-Subject (Many-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Subject)
                .WithMany(s => s.Lessons)
                .HasForeignKey(l => l.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lesson-Teacher (Many-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Teacher)
                .WithMany(t => t.Lessons)
                .HasForeignKey(l => l.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Assignment-Lesson (Many-to-One)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Lesson)
                .WithMany(l => l.Assignments)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment-AssignmentFile (One-to-Many)
            modelBuilder.Entity<AssignmentFile>()
                .HasOne(af => af.Assignment)
                .WithMany(a => a.AssignmentFiles)
                .HasForeignKey(af => af.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentAssignment-SubmissionFile (One-to-Many)
            modelBuilder.Entity<SubmissionFile>()
                .HasOne(sf => sf.StudentAssignment)
                .WithMany(sa => sa.SubmissionFiles)
                .HasForeignKey(sf => new { sf.StudentId, sf.AssignmentId })
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
