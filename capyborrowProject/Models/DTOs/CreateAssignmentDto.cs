namespace capyborrowProject.Models.DTOs
{
    public class CreateAssignmentDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsAutomaticallyClosed { get; set; }
        public bool IsSubmittable { get; set; }
        public float MaxScore { get; set; }
        public int LessonId { get; set; }
        public List<IFormFile>? AssignmentFiles { get; set; }
        public List<string>? StudentIds { get; set; }
    }
}
