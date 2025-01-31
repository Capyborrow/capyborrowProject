using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Student : ApplicationUser
    {
        [Range(1, 6)]
        public int? Course { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public int? GroupId { get; set; }
        public Group? Group { get; set; } = null;
    }
}
