﻿namespace capyborrowProject.Models
{
    public class Teacher
    {
        public int ID { get; set; }
        public required string Name { get; set; }
        public string? MiddleName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
    }
}
