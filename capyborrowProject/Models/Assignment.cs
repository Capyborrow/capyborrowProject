namespace capyborrowProject.Models
{
    public class Assignment
    {
        public enum TaskStatus
        {
            InReview,
            Evaluated,
            PastDeadline
        }

        public int ID { get; set; }
        public TaskStatus Status { get; set; }
        public string? Description { get; set; }
        public required Teacher Teacher { get; set; }

    }
}
