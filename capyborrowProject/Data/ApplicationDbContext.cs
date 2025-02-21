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
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentReadStatus> CommentReadStatuses { get; set; }
        public DbSet<Attendance> Attendances { get; set; }


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

            // Comment-Lesson (Many-to-One)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Lesson)
                .WithMany(l => l.Comments)
                .HasForeignKey(c => c.LessonId)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment-User (Many-to-One)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Comment-Assignment (Many-to-One)
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Assignment)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Reply relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Composite primary key for CommentReadStatus
            modelBuilder.Entity<CommentReadStatus>()
                .HasKey(crs => new { crs.CommentId, crs.UserId });

            // Comment-CommentReadStatus (One-to-Many)
            modelBuilder.Entity<CommentReadStatus>()
                .HasOne(crs => crs.Comment)
                .WithMany(c => c.CommentReadStatuses)
                .HasForeignKey(crs => crs.CommentId);

            // User-CommentReadStatus (One-to-Many)
            modelBuilder.Entity<CommentReadStatus>()
                .HasOne(crs => crs.User)
                .WithMany(u => u.CommentReadStatuses)
                .HasForeignKey(crs => crs.UserId);

            // Assignment-Lesson (Many-to-One)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Lesson)
                .WithMany(l => l.Assignments)
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
