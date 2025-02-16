using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Assignment
    {
        public enum AssignmentStatus
        {
            Done,
            Due,
            Overdue,
            Marked
        }

        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public AssignmentStatus Status { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
