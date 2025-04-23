namespace capyborrowProject.Models
{
    public class TempAssignmentFile
    {
        public int Id { get; set; }
        public required string FileUrl { get; set; }
        public required string FileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
