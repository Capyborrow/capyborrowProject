namespace capyborrowProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string firstName { get; set; }
        public string middleName { get; set; } = string.Empty;
        public required string lastName { get; set; }
        public required string passwordHash { get; set; }
        public required string email { get; set; }
    }
}
