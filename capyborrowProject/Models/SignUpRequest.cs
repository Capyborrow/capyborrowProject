using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "FirstName is required.")]
        public required string FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public required string Role { get; set; }
    }
}
