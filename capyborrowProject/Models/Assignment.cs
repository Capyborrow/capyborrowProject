using System;
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
        public required bool IsAutomaticallyClosed { get; set; }
        public bool IsClosed
        {
            get
            {
                if (IsAutomaticallyClosed == true && DueDate.HasValue && DateTime.Now > DueDate)
                    return true;
                return false;
            }
        }
        public required float MaxScore { get; set; }
        public bool IsSubmittable { get; set; } = true;

        public int? LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;

        public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
        public ICollection<AssignmentFile> AssignmentFiles { get; set; } = new List<AssignmentFile>();
    }
}
