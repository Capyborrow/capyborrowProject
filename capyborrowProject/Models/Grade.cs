namespace capyborrowProject.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public float Score { get; set; }

        public string? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }
    }
}
