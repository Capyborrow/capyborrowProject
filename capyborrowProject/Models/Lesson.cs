using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Lesson
    {
        public enum LessonType
        {
            Lecture,
            Practice,
            Seminar,
            Consultation,
            Exam
        }
        public int Id { get; set; }

        [MaxLength(10)]
        public string? Room { get; set; }
        [MaxLength(2000)]
        public string? Link { get; set; }
        public DateTime? Date { get; set; }
        public LessonType Type { get; set; }
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public string? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
