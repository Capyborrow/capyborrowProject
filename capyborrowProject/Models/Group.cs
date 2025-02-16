using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Group
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public required string Name { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();  
    }
}
