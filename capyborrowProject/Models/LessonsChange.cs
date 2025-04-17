using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace capyborrowProject.Models
{
    public class LessonsChange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LessonId { get; set; }

        public DateTime LastModified { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
    }
}
