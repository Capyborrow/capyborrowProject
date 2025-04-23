namespace capyborrowProject.Models.DTOs
{
    public class StudentWithAttendanceDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public required AttendanceType AttendanceType { get; set; }
    }
}
