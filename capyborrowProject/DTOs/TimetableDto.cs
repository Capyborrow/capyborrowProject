using capyborrowProject.Models;

namespace capyborrowProject.DTOs
{
    public class TimetableDto
    {
        /*public enum AttendanceType
        {
            Present,
            Absent,
            Excused,
            Unknown,
            Cancelled
        }

        public enum AssignmentStatusEnum
        {
            Due,
            Overdue,     
            Submitted,   
            Graded,      
            Expired       
        }*/

        public enum CommentStatusEnum
        {
            Read,
            Unread
        }
        /*public enum LessonType
        {
            Lecture,
            Practice,
            Seminar,
            Consultation,
            Exam
        }*/
        public DateTime Date { get; set; }
        public required int Day { get; set; }
        public required int TimeSlot { get; set; }
        public required string SubjectName { get; set; }
        public required string TeacherName { get; set; }
        public required string TeacherAvatar { get; set; }
        public string? Link { get; set; }
        public string? Room { get; set; }
        public required LessonType Type { get; set; }
        public AttendanceType AttendanceStatus { get; set; }
        public AssignmentStatus AssignmentStatusEnum { get; set; }
        public required CommentStatusEnum IsRead { get; set; }

    }
}
