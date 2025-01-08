namespace capyborrowProject.Models
{
    public class Teacher : User
    {
        public required ICollection<Subject> Subjects { get; set; }
    }
}
