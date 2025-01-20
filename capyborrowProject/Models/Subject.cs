namespace capyborrowProject.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public required string Title { get; set; }

        public required Teacher Teacher { get; set; }

    }
}
