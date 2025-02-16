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

        public enum AttendanceType
        {
            Present,
            Absent,
            Cancelled,
            Unknown
        }

        public int Id { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }
        public DateTime? Date { get; set; }

        public LessonType Type { get; set; }
        public AttendanceType Attendance { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? NotificationId { get; set; }
        public Notification? Notification { get; set; }

        public string? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    } 
}
