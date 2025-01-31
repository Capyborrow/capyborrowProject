using capyborrowProject.Models.PredefinedTables;
using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Lesson
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }
        public DateTime? Date { get; set; }

        public int? LessonTypeId { get; set; }
        public LessonType? LessonType { get; set; }

        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public int? NotificationId { get; set; }
        public Notification? Notification { get; set; }

        public string? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? GroupId { get; set; }
        public Group? Group { get; set; }

        //public int? AssignmentId { get; set; }
        //public Assignment? Assignment { get; set; }

    } 
}
