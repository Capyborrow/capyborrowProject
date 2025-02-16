namespace capyborrowProject.Models
{
    public class Grade
    {
        public int Id { get; set; }

        public float Score { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;

        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
    }
}
