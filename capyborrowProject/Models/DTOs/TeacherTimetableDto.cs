namespace capyborrowProject.Models.DTOs
{
    public class TeacherTimetableDto
    {
        public int Id { get; set; }
        public required DateTime Date { get; set; }
        public required string SubjectName { get; set; }
        public required string GroupName { get; set; }
        public string? Link { get; set; }
        public string? Room { get; set; }
        public required LessonType Type { get; set; }
        public LessonStatus? LessonStatus { get; set; }
    }
}
