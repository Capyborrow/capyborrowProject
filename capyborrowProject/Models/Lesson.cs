namespace capyborrowProject.Models
{
    public class Lesson
    {
        public enum LessonType
        {
            Lecture,
            Seminar,
            Lab
        }

        public enum LessonImportance
        {
            Test,
            Usual
        }

        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int GroupId { get; set; }
        public required string Location { get; set; }
        public required DateTime Date { get; set; }
        public required LessonType Type { get; set; }
        public required LessonImportance Importance { get; set; }
        public required ICollection<Attendance> Attendances { get; set; }

        public required Subject Subject { get; set; }
        public required Group Group { get; set; }
    } 
}
