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
        private static int GetAssignmentStatusPriority(AssignmentStatus status)
        {
            return status switch
            {
                AssignmentStatus.Expired => 0,
                AssignmentStatus.Overdue => 1,
                AssignmentStatus.Due => 2,
                AssignmentStatus.Submitted => 3,
                AssignmentStatus.Graded => 4,
                _ => int.MaxValue
            };
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
                int bestPriority = int.MaxValue;

                foreach (var assignment in l.Assignments)
                {
                    if (!studentAssignmentLookup.Contains(assignment.Id)) continue;

                    foreach (var status in studentAssignmentLookup[assignment.Id].Where(s => s.HasValue).Select(s => s.Value))
                    {
                        int priority = GetAssignmentStatusPriority(status);
                        if (priority < bestPriority)
                        {
                            bestPriority = priority;
                            assignmentStatus = status;

                            if (priority == 0) break;
                        }
                    }

                    if (bestPriority == 0) break;
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
        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<TeacherTimetableDto>>> GetTeacherTimetable(DateTime startDate, DateTime endDate, string teacherId)
        {
            var teacher = await context.Teachers
                .FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher == null)
            {
                return NotFound("Teacher not found");
            }

            var lessons = await context.Lessons
                .Where(l => l.TeacherId == teacher.Id && l.Date >= startDate && l.Date <= endDate)
                .Include(l => l.Subject)
                .Include(l => l.Group)
                .ToListAsync();

            var timetable = lessons.Select(l => new TeacherTimetableDto
            {
                Id = l.Id,
                Date = l.Date ?? default(DateTime),
                SubjectName = l.Subject?.Name ?? "Unknown",
                GroupName = l.Group?.Name ?? "Unknown",
                Link = l.Link,
                Room = l.Room,
                Type = l.Type,
                LessonStatus = l.Status
            }).ToList();

            return Ok(timetable);
        }
    }
}