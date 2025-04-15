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
                .Include(l => l.Attendances)
                .Include(l => l.Assignments)
                .ToListAsync();
            var assignmentIds = lessons.SelectMany(l => l.Assignments.Select(a => a.Id))
                                       .Distinct()
                                       .ToList();
            var studentAssignments = await context.StudentAssignments
                .Where(sa => sa.StudentId == student.Id && assignmentIds.Contains(sa.AssignmentId))
                .ToListAsync();
            var studentAssignmentLookup = studentAssignments.ToLookup(sa => sa.AssignmentId, sa => sa.ComputedStatus);
            var timetable = lessons.Select(l =>
            {
                AssignmentStatus? assignmentStatus = null;
                foreach (var assignment in l.Assignments)
                {
                    assignmentStatus = studentAssignmentLookup[assignment.Id]
                        .FirstOrDefault(cs => cs.HasValue);
                    if (assignmentStatus.HasValue)
                    {
                        break;
                    }
                }
                var attendance = l.Attendances.FirstOrDefault(a => a.StudentId == student.Id);
                var lessonStatus = attendance != null ? attendance.Type : AttendanceType.Unknown;

                return new TimetableDto
                {
                    Id = l.Id,
                    Date = l.Date ?? default(DateTime),
                    SubjectName = l.Subject?.Name ?? "Unknown",
                    TeacherName = l.Teacher != null ? $"{l.Teacher.FirstName} {l.Teacher.LastName}" : "Unknown",
                    TeacherAvatar = l.Teacher?.ProfilePicture ?? "",
                    Link = l.Link,
                    Room = l.Room,
                    Type = (LessonType)l.Type,
                    LessonStatus = lessonStatus,
                    AssignmentStatus = assignmentStatus,
                };
            }).ToList();

            return Ok(timetable);
        }
    }
}