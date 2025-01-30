using Microsoft.AspNetCore.Identity;

namespace capyborrowProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
