using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace capyborrowProject.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [MaxLength(300)]
        public required string Text { get; set; }

        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }
    }
}
