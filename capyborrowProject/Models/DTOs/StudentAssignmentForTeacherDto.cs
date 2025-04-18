namespace capyborrowProject.Models.DTOs
{
    public class StudentAssignmentForTeacherDto
    {
        public float? Score { get; set; }
        public string? StudentName { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public AssignmentStatus? Status { get; set; }

        public List<SubmissionFileDto> Submissions { get; set; } = [];
    }
}
