﻿namespace capyborrowProject.Models
{
    public class StudentAssignment
    {
        public float? Score { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public DateTime? SubmittedAt { get; set; }

        public enum AssignmentStatus
        {
            Due,          // Очікує виконання
            Overdue,      // Прострочено
            Submitted,    // Здано
            Graded,       // Оцінено
            Expired       // Закрито для здачі
        }

        public AssignmentStatus ComputedStatus
        {
            get
            {
                if (Assignment.IsClosed)
                    return AssignmentStatus.Expired;
                if (Score.HasValue)
                    return AssignmentStatus.Graded;
                if (SubmittedAt.HasValue)
                    return AssignmentStatus.Submitted;
                if (Assignment.DueDate.HasValue && DateTime.Now > Assignment.DueDate)
                    return AssignmentStatus.Overdue;
                return AssignmentStatus.Due;
            }
        }
    }
}
