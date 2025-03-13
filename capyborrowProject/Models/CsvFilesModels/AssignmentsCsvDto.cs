using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models.CsvFilesModels
{
    public class AssignmentsCsvDto
    {
        public DateTime LessonDate { get; set; }
        public string TeacherEmail { get; set; } = string.Empty;

        [MaxLength(100)]
        public required string Name { get; set; }
        public required float MaxScore { get; set; }
        public DateTime? DueDate { get; set; }
        public required bool IsAutomaticallyClosed { get; set; }
        public string? Description { get; set; }

        public float? Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}