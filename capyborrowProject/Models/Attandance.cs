namespace capyborrowProject.Models
{
    public class Attandance
    {
        public enum AttandanceStatus
        {
            Present,
            Absent,
            Excused,
            NoInfo
        }
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public required Lesson Lesson { get; set; }
        public required Student Student { get; set; }
        public required AttandanceStatus Status { get; set; }
    }
}
