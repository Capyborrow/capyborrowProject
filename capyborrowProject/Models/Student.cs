namespace capyborrowProject.Models
{
    public class Student : User
    {
        public int? GroupId { get; set; }
        public int? Course { get; set; }
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public Group? Group { get; set; } = null;
    }
}
