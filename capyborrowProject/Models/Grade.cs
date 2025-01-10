namespace capyborrowProject.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int LessonId { get; set; }
        public required Lesson Lesson { get; set; }
        public required Student Student { get; set; }
        public required double Score { get; set; }
        public string? Comment { get; set; }
        public required DateTime DataAssigned { get; set; }

    }
}
