using capyborrowProject.Models.PredefinedTables;
using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        public int? AssignmentStatusId { get; set; }
        public AssignmentStatus? AssignmentStatus { get; set; }

        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }
    }
}
