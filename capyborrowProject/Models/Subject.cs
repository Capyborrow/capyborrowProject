namespace capyborrowProject.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string TeacherId { get; set; }
        public required string Title { get; set; }

        public required Teacher Teacher { get; set; }

    }
}
