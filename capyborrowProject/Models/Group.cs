namespace capyborrowProject.Models
{
    public class Group
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required ICollection<Student> Students { get; set; }
    }
}
