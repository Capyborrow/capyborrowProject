namespace capyborrowProject.Models.CsvFilesModels
{
    public class LessonCsvDto
    {
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public int Attendance { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherEmail { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
    }
}
