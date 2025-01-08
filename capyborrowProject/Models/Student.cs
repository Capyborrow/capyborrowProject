namespace capyborrowProject.Models
{
    public class Student : User
    {
        public required int GroupId { get; set; }
        //public ICollection<Grade> Grades { get; set; }
        public required ICollection<Assignment> Assignments { get; set; }
        public required int Course { get; set; }

        public required Group Group { get; set; }
    }
}
