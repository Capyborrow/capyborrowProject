namespace capyborrowProject.Models
{
    public class Assignment
    {
        public enum TaskStatus
        {
            InReview,
            Marked,
            PastDeadline
        }

        public int Id { get; set; }
        public required string title { get; set; }
        public TaskStatus status { get; set; }
        public string description { get; set; } = string.Empty;
        public required Teacher teacher { get; set; }
    }
}
