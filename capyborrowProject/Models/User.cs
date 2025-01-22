namespace capyborrowProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public required string Role { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
