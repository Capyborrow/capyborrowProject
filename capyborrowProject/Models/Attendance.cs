using System.Text.Json.Serialization;

namespace capyborrowProject.Models
{
    public enum AttendanceType
    {
        Attended,
        Skipped,
        Excused,
        Unknown,
        Cancelled
    }
    public class Attendance
    {
        public AttendanceType Type { get; set; }
        public string? StudentId { get; set; }
        public Student? Student { get; set; }
        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }
    }
}
