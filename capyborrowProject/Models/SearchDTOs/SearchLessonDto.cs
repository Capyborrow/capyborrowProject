namespace capyborrowProject.Models.SearchDTOs
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

        public string? SubjectTitle_en { get; set; }
        public string? SubjectTitle_uk { get; set; }

        public string? TeacherFullName_en { get; set; }
        public string? TeacherFullName_uk { get; set; }

        public string? TeacherEmail { get; set; }

        public string? GroupTitle_en { get; set; }
        public string? GroupTitle_uk { get; set; }

        public DateTimeOffset? LastModified { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
