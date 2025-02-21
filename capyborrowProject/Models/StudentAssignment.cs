namespace capyborrowProject.Models
{
    public class StudentAssignment
    {
        public float? Score { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public bool IsReturned { get; set; } = false;

        public DateTime? SubmittedAt { get; set; }

        public enum AssignmentStatus
        {
            Pending,      // Очікує виконання
            Overdue,      // Прострочено
            Submitted,    // Здано
            Graded,       // Оцінено
            Returned,     // Повернено (викладач повернув роботу студенту без оцінки)
        }

        public AssignmentStatus ComputedStatus
        {
            get
            {
                if (IsReturned)
                    return AssignmentStatus.Returned;
                if (Score.HasValue)
                    return AssignmentStatus.Graded;
                if (SubmittedAt.HasValue)
                    return AssignmentStatus.Submitted;
                if (DateTime.UtcNow > Assignment.DueDate)
                    return AssignmentStatus.Overdue;
                return AssignmentStatus.Pending;
            }
        }

        //Attachments
    }
}
