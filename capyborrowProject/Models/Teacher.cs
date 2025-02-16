namespace capyborrowProject.Models
{
    public class Teacher : ApplicationUser
    {
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
