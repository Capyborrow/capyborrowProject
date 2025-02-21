using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Student : ApplicationUser
    {
        [Range(1, 6)]
        public int? Course { get; set; }

        public ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();

        public int? GroupId { get; set; }
        public Group? Group { get; set; } = null;

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
