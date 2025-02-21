namespace capyborrowProject.Models
{
    public class Attendance
    {
        public enum AttendanceType
        {
            Present,
            Absent,
            Excused,
            Unknown
        }
        public AttendanceType Type { get; set; }
        public string? StudentId { get; set; }
        public Student? Student { get; set; }
        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }
    }
}
