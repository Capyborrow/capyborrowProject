namespace capyborrowProject.Models
{
    public class JwtSettings
    {
        public string AccessTokenSecret { get; set; }
        public string RefreshTokenSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiryInSeconds { get; set; }
        public int RefreshTokenExpiryInSeconds { get; set; }

    }
}
