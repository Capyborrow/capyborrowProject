using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models.AuthModels
{
    public class CheckEmailConfirmationRequest
    {
        [Required]
        [EmailAddress (ErrorMessage = "Email is required")]
        public required string Email { get; set; }
    }
}
