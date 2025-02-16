using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
