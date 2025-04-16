using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models.AuthModels
{
    public class ResendEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }

    }
}
