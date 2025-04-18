namespace capyborrowProject.Models.DTOs
{
    public class AssignedStudentDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
