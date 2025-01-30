using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class ResendEmailRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

    }
}
