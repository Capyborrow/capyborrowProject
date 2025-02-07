namespace capyborrowProject.Models
{
    public class Teacher : ApplicationUser
    {
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
