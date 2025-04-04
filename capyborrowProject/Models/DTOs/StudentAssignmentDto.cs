namespace capyborrowProject.Models.DTOs
{
    public class StudentAssignmentDto
    {
        public float? Score { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public AssignmentStatus? Status { get; set; }

        public List<SubmissionFileDto> Submissions { get; set; } = [];
    }
}
