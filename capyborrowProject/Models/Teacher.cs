using capyborrowProject.Models.JoinTables;

namespace capyborrowProject.Models
{
    public class Teacher : ApplicationUser
    {
        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
