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
        }
    }
}
