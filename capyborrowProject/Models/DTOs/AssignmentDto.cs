namespace capyborrowProject.Models.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public float MaxScore { get; set; }
        public List<AssignmentFileDto> Attachments { get; set; } = [];
        public StudentAssignmentDto? StudentAssignment { get; set; }
    }
}
