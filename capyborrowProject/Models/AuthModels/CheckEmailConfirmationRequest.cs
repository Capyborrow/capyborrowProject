using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models.AuthModels
{
    public class CheckEmailConfirmationRequest
    {
        [Required]
        [EmailAddress (ErrorMessage = "Email is required")]
        public string Email { get; set; }
    }
}
