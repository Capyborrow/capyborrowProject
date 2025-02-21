using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(30)]
        public required string FirstName { get; set; }

        [MaxLength(30)]
        public string? MiddleName { get; set; }

        [MaxLength(30)]
        public required string LastName { get; set; }

        public string? ProfilePicture { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<CommentReadStatus> CommentReadStatuses { get; set; } = new List<CommentReadStatus>();
    }
}
