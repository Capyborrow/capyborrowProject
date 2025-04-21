using capyborrowProject.Data;
using capyborrowProject.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("GetStudentsInGroupForLesson/{lessonId}")]
        public async Task<ActionResult<IEnumerable<AssignedStudentDto>>> GetStudentsInGroupForAssignment(int lessonId)
        {
            var lesson = await context.Lessons
                .Include(l => l.Group)
                    .ThenInclude(g => g.Students)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson is null)
                return NotFound("Lesson not found");

            if (lesson.Group is null || lesson.Group.Students is null)
                return NotFound("Group or students not found");

            var students = lesson.Group.Students.Select(s => new AssignedStudentDto
            {
                Id = s.Id,
                FullName = s.MiddleName is null ? $"{s.FirstName} {s.LastName}" : $"{s.FirstName} {s.MiddleName} {s.LastName}",
                ProfilePicture = s.ProfilePicture
            });

            return Ok(students);
        }
    }
}
