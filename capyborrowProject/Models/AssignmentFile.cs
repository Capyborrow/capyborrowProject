namespace capyborrowProject.Models
{
    public class AssignmentFile
    {
        public int Id { get; set; }
        public required string FileUrl { get; set; }
        public required string FileName { get; set; }
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
    }
}
