using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                SubjectName = l.Subject?.Name ?? "Unknown",
                TeacherName = l.Teacher != null ? l.Teacher.FirstName + " " + l.Teacher.LastName : "Unknown",
                TeacherAvatar = l.Teacher?.ProfilePicture ?? "",
                Link = l.Link,
                Room = l.Room,
                Type = (LessonType)l.Type,
                LessonStatus = l.Attendances.FirstOrDefault(a => a.StudentId == student.Id)?.Type ?? AttendanceType.Unknown
,
                AssignmentStatus = l.Assignments
                    .Select(a => context.StudentAssignments
                        .FirstOrDefault(sa => sa.AssignmentId == a.Id && sa.StudentId == student.Id)?.ComputedStatus)
                    .FirstOrDefault(sa => sa.HasValue) ?? null, // Повертаємо null, якщо немає жодного статусу
                IsRead = l.Comments.Any(c => context.CommentReadStatuses.Any(cr => cr.CommentId == c.Id && cr.UserId == student.Id && cr.IsRead))
                    ? TimetableDto.CommentStatusEnum.Read
                    : TimetableDto.CommentStatusEnum.Unread

            }).ToList();

            return Ok(timetable);
        }
    }
}
