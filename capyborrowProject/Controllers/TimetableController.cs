using capyborrowProject.Data;
using capyborrowProject.DTOs;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController(ApplicationDbContext context) : ControllerBase
    {
        private static readonly List<(TimeSpan Start, int Slot)> timeSlots = new()
        {
            (new TimeSpan(8, 40, 0), 1),
            (new TimeSpan(10, 35, 0), 2),
            (new TimeSpan(12, 20, 0),3),
            (new TimeSpan(14, 05, 0), 4)
        };
        private int GetTimeSlot(DateTime startDateTime)
        {
            TimeSpan startTime = startDateTime.TimeOfDay;
            foreach (var slot in timeSlots)
            {
                if (startTime >= slot.Start && startTime < slot.Start + TimeSpan.FromMinutes(90))
                    return slot.Slot;
            }

            return -1;
        }
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<TimetableDto>>> GetStudentTimetable(DateTime startDate, DateTime endDate, string studentId)
        {
            var student = await context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null || student.Group == null)
            {
                return NotFound("Student or group not found");
            }

            var lessons = await context.Lessons
                .Where(l => l.GroupId == student.Group.Id && l.Date >= startDate && l.Date <= endDate)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Assignments)
                .Include(l => l.Attendances)
                .Include(l => l.Comments)
                .ToListAsync();

            var timetable = lessons.Select(l => new TimetableDto
            {
                Date = l.Date ?? default(DateTime),
                Day = (l.Date ?? startDate).Subtract(startDate).Days + 1,
                TimeSlot = GetTimeSlot(l.Date ?? default(DateTime)),
                SubjectName = l.Subject?.Name ?? "Unknown",
                TeacherName = l.Teacher != null ? l.Teacher.FirstName + " " + l.Teacher.LastName : "Unknown",
                TeacherAvatar = l.Teacher?.ProfilePicture ?? "",
                Link = l.Link,
                Room = l.Room,
                Type = (LessonType)l.Type,
                AttendanceStatus = l.Attendances.FirstOrDefault(a => a.StudentId == student.Id)?.Type ?? AttendanceType.Unknown
,
                AssignmentStatusEnum = l.Assignments.Select(a => context.StudentAssignments
                    .FirstOrDefault(sa => sa.AssignmentId == a.Id && sa.StudentId == student.Id)?.ComputedStatus
                    ?? AssignmentStatus.Due).FirstOrDefault(),
                IsRead = l.Comments.Any(c => context.CommentReadStatuses.Any(cr => cr.CommentId == c.Id && cr.UserId == student.Id && cr.IsRead))
                    ? TimetableDto.CommentStatusEnum.Read
                    : TimetableDto.CommentStatusEnum.Unread

            }).ToList();

            return Ok(timetable);
        }
        /*[HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetTeacherTimetable(DateTime startDate, DateTime endDate, string teacherId)
        {
            var timetable = await context.Lessons
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Group)
                .Include(l => l.Comments)
                .Where(l => l.Date >= startDate && l.Date <= endDate && l.TeacherId == teacherId)
                .ToListAsync();

            return timetable is null ? NotFound() : Ok(timetable);
        }*/
    }
}
