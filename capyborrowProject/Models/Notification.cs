namespace capyborrowProject.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int TeacherId { get; set; }
        public required Lesson Lesson { get; set; }
        public required Teacher Teacher { get; set; }
        public required string Text {  get; set; }
    }
}
