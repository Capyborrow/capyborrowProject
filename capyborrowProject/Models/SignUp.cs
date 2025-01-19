namespace capyborrowProject.Models
{
    public class SignUp
    {
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required int Role { get; set; }
    }
}
