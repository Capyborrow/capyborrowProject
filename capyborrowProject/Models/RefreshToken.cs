﻿namespace capyborrowProject.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }

        //public DateTime ExpiryDate { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime? RevokedDate { get; set; }
        public User User { get; set; }
    }
}
