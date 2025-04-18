using System.Text.Json.Serialization;

namespace capyborrowProject.Models
{
    public enum AssignmentStatus
    {
        Due,          // Очікує виконання
        Overdue,      // Прострочено
        Submitted,    // Здано
        Graded,       // Оцінено
        Expired       // Закрито для здачі
    }
    public class StudentAssignment
    {
        public float? Score { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        public DateTime? SubmittedAt { get; set; }

        public AssignmentStatus? ComputedStatus
        {
            get
            {
                Console.WriteLine(DateTime.Now);

                if (Assignment.IsClosed)
                    return AssignmentStatus.Expired;
                if (Score.HasValue)
                    return AssignmentStatus.Graded;
                if (SubmittedAt.HasValue)
                    return AssignmentStatus.Submitted;
                if (Assignment.DueDate.HasValue && DateTime.Now > Assignment.DueDate)
                    return AssignmentStatus.Overdue;
                if (Assignment.DueDate.HasValue && DateTime.Now <= Assignment.DueDate)
                    return AssignmentStatus.Due;
                if(Assignment.DueDate is null && Assignment.IsSubmittable && !Assignment.IsClosed && SubmittedAt is null)
                    return AssignmentStatus.Due;
                return null;
            }
        }

        public ICollection<SubmissionFile> SubmissionFiles { get; set; } = new List<SubmissionFile>();
    }
}
