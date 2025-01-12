using capyborrowProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await context.Notifications
                .Include(n => n.Lesson)
                .Include(n => n.Teacher)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotification(int id)
        {
            var notification = await context.Notifications
                .Include(n => n.Lesson)
                .Include(n => n.Teacher)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification is null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult<Notification>> PostNotification(Notification notificationToAdd)
        {
            context.Notifications.Add(notificationToAdd);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNotification), new { id = notificationToAdd.Id }, notificationToAdd);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, Notification notification)
        {
            if (id != notification.Id)
            {
                return BadRequest();
            }

            context.Entry(notification).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Notifications.Any(n => n.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await context.Notifications
                .Include(n => n.Lesson)
                .Include(n => n.Teacher)
                .FirstOrDefaultAsync(n => n.Id == id);

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
