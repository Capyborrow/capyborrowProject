namespace capyborrowProject.Models.DTOs
{
    public class GradeStudentAssigmentDto
    {
        public required string StudentId { get; set; }
        public required int AssignmentId { get; set; }
        public required float Score { get; set; }
    }
}
