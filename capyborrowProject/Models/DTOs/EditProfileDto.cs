namespace capyborrowProject.Models.DTOs
{
    public class EditProfileDto
    {
        public required string Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
