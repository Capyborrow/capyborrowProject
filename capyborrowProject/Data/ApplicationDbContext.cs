﻿using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using capyborrowProject.Models.PredefinedTables;

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
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<LessonType> LessonTypes { get; set; } // Predefined table
        public DbSet<AssignmentStatus> AssignmentStatuses { get; set; } // Predefined table


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

            // Group-Subject (Many-to-Many)
            modelBuilder.Entity<Group>()
                .HasMany(g => g.Subjects)
                .WithMany(s => s.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "GroupSubject",
                    j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId"),
                    j => j.HasOne<Group>().WithMany().HasForeignKey("GroupId"),
                    j =>
                    {
                        j.HasKey("GroupId", "SubjectId");
                    }
                );


            // Teacher-Subject (Many-to-Many)
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Subjects)
                .WithMany(s => s.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "TeacherSubject",
                    j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId"),
                    j => j.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId"),
                    j =>
                    {
                        j.HasKey("TeacherId", "SubjectId");
                    }
                );

            // Student-Group (One-to-Many)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Grade-Student (One-to-Many)
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Grade-Assignment
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Assignment)
                .WithMany()
                .HasForeignKey(g => g.AssignmentId);

            // Lesson-Group (Many-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Group)
                .WithMany(g => g.Lessons)
                .HasForeignKey(l => l.GroupId);

            // Lesson-Subject
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Subject)
                .WithMany()
                .HasForeignKey(l => l.SubjectId);

            // Lesson-LessonType
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.LessonType)
                .WithMany()
                .HasForeignKey(l => l.LessonTypeId);

            // Lesson-Teacher (Many-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Teacher)
                .WithMany(t => t.Lessons)
                .HasForeignKey(l => l.TeacherId);

            // Lesson-Notification (One-to-One)
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Notification)
                .WithOne(n => n.Lesson)
                .HasForeignKey<Notification>(n => n.LessonId);

            // Assignment-Group (Many-to-One)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Group)
                .WithMany(g => g.Assignments)
                .HasForeignKey(a => a.GroupId);

            // Assignment-Lesson
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Lesson)
                .WithMany()
                .HasForeignKey(a => a.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment-AssignmentStatus
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.AssignmentStatus)
                .WithMany()
                .HasForeignKey(a => a.AssignmentStatusId);

            modelBuilder.Entity<LessonType>().HasData(
                new LessonType { Id = 1, Name = "Lecture" },
                new LessonType { Id = 2, Name = "Lab" },
                new LessonType { Id = 3, Name = "Seminar" }
            );

            modelBuilder.Entity<AssignmentStatus>().HasData(
                new AssignmentStatus { Id = 1, Name = "ToDo" },
                new AssignmentStatus { Id = 2, Name = "InReview" },
                new AssignmentStatus { Id = 3, Name = "PastDue" },
                new AssignmentStatus { Id = 4, Name = "Marked" }
            );
        }
    }
}
