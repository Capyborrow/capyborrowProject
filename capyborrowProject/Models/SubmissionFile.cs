namespace capyborrowProject.Models
{
    public class SubmissionFile
    {
        public int Id { get; set; }
        public required string FileUrl { get; set; }
        public required string FileName { get; set; }
        public required string StudentId { get; set; }
        public int AssignmentId { get; set; }
        public StudentAssignment StudentAssignment { get; set; } = null!;
    }
}
