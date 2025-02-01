using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models.AuthModels
{
    public class ForgotPasswordRequest
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]

        public string Email { get; set; }
    }
}
