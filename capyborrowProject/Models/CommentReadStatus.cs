namespace capyborrowProject.Models
{
    public class CommentReadStatus
    {
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }
        public required bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
    }
}
