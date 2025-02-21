using System.ComponentModel.DataAnnotations;

namespace capyborrowProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required bool IsPrivate { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }
        public int? AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public ICollection<CommentReadStatus> CommentReadStatuses { get; set; } = new List<CommentReadStatus>();
    }
}
