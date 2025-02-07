using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await context.Notifications
                .Include(n => n.Lesson)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            var notification = await context.Notifications
                .Include(n => n.Lesson)
                .FirstOrDefaultAsync(n => n.Id == id);

            return notification is null ? NotFound() : Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification(Notification notification)
        {
            if (notification is null)
            {
                return BadRequest(new { message = "Invalid notification data." });
            }

            if (notification.LessonId.HasValue)
            {
                var lessonExists = await context.Lessons.AnyAsync(l => l.Id == notification.LessonId);
                if (!lessonExists)
                {
                    return BadRequest(new { message = "Associated lesson does not exist." });
                }
            }

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, Notification updatedNotification)
        {
            if (id != updatedNotification.Id)
            {
                return BadRequest(new { message = "Notification ID mismatch." });
            }

            var existingNotification = await context.Notifications.FindAsync(id);
            if (existingNotification is null)
            {
                return NotFound();
            }

            existingNotification.Text = updatedNotification.Text;
            existingNotification.LessonId = updatedNotification.LessonId;

            if (updatedNotification.LessonId.HasValue)
            {
                var lessonExists = await context.Lessons.AnyAsync(l => l.Id == updatedNotification.LessonId);
                if (!lessonExists)
                {
                    return BadRequest(new { message = "Associated lesson does not exist." });
                }
            }

            context.Entry(existingNotification).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating notification." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await context.Notifications.FindAsync(id);
            if (notification is null)
            {
                return NotFound();
            }

            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}