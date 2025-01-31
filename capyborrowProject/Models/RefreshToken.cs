namespace capyborrowProject.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }

        //public DateTime ExpiryDate { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime? RevokedDate { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
