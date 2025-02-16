using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using capyborrowProject.Data;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetGroupTimetable(DateTime startDate, DateTime endDate, int groupId)
        {
            var timetable = await context.Lessons
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Group)
                .Include(l => l.Notification)
                .Where(l => l.Date >= startDate && l.Date <= endDate && l.GroupId == groupId)
                .ToListAsync();

            return timetable is null ? NotFound() : Ok(timetable);
        }
        [HttpGet("teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetTeacherTimetable(DateTime startDate, DateTime endDate, string teacherId)
        {
            var timetable = await context.Lessons
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Group)
                .Include(l => l.Notification)
                .Where(l => l.Date >= startDate && l.Date <= endDate && l.TeacherId == teacherId)
                .ToListAsync();

            return timetable is null ? NotFound() : Ok(timetable);
        }
    }   
}
