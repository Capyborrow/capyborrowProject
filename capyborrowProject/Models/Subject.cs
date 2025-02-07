using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
