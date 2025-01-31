using capyborrowProject.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public ICollection<GroupSubject> GroupSubjects { get; set; } = new List<GroupSubject>();
    }
}
