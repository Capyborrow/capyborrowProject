namespace capyborrowProject.Models.CsvFilesModels
{
    public class LessonCsvDto
    {
        public string Room { get; set; } = string.Empty;
        public string? Link { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherEmail { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
    }
}
