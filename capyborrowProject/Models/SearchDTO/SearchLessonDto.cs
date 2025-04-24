namespace capyborrowProject.Models.SearchDTO
{
    public class SearchLessonDto
    {
        public string LessonId { get; set; } = default!;
        public DateTimeOffset? LessonDate { get; set; }

        public int? LessonType { get; set; }
        public int? LessonStatus { get; set; }

        public string? LessonTypeName { get; set; }
        public string? LessonStatusName { get; set; }

        public string? Room { get; set; }
        public string? Link { get; set; }

        public string? SubjectTitle { get; set; }
        public string? TeacherFullName { get; set; }
        public string? TeacherEmail { get; set; }
        public string? GroupTitle{ get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
