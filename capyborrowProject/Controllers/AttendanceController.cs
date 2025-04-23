using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Models.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("GetStudentsInGroupAndTheirAttendance/{lessonId}")]
        public async Task<ActionResult<IEnumerable<StudentWithAttendanceDto>>> GetStudentsInGroupAndTheirAttendance(int lessonId)
        {
            var lesson = await context.Lessons
                .Include(l => l.Group)
                    .ThenInclude(g => g.Students)
                .Include(l => l.Attendances)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson is null)
                return NotFound("Lesson not found");

            if (lesson.Group is null || lesson.Group.Students is null)
                return NotFound("Group or students not found");

            var students = lesson.Group.Students.Select(s => new StudentWithAttendanceDto
            {
                Id = s.Id,
                FullName = s.MiddleName is null ? $"{s.FirstName} {s.LastName}" : $"{s.FirstName} {s.MiddleName} {s.LastName}",
                ProfilePicture = s.ProfilePicture,
                AttendanceType = lesson.Attendances.FirstOrDefault(a => a.StudentId == s.Id)?.Type ?? AttendanceType.Unknown
            });

            return Ok(students);
        }

        [HttpPut("SetAttendance/{lessonId}/{studentId}")]
        public async Task<IActionResult> SetAttendance(int lessonId, string studentId)
        {
            var attendance = await context.Attendances
                .FirstOrDefaultAsync(a => a.LessonId == lessonId && a.StudentId == studentId);

            if (attendance is null)
                return NotFound("Attendance not found");

            attendance.Type = attendance.Type == AttendanceType.Attended ? AttendanceType.Skipped : AttendanceType.Attended;

            context.Attendances.Update(attendance);

            await context.SaveChangesAsync();

            return Ok(attendance);
        }
    }
}
