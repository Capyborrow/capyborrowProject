namespace capyborrowProject.Models
{
    public class Student : User
    {
        public required string group { get; set; }
        public required int course { get; set; }
    }
}
