namespace capyborrowProject.Models.DTOs
{
    public class UserProfileDto
    {
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
