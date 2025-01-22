namespace capyborrowProject.Models
{
    public class Teacher : User
    {
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
