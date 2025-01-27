﻿namespace capyborrowProject.Models
{
    public class Assignment
    {
        public enum TaskStatus
        {
            ToDo,
            InReview,
            Marked,
            PastDeadline
        }

        public int Id { get; set; }
        public int LessonId { get; set; }
        public required string Title { get; set; }
        public required TaskStatus Status { get; set; }
        public string? Description { get; set; }
        public required DateTime DateAssigned { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Comment { get; set; }
        public required ICollection<Student> Students { get; set; } 

        public required Lesson Lesson { get; set; }
    }
}
