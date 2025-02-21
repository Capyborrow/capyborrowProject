using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using capyborrowProject.Data;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController (ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = await context.Lessons
                .Include(l => l.Assignments)
                .Include(l => l.Subject)
                .Include(l => l.Comments)
                .Include(l => l.Teacher)
                .Include(l => l.Group)
                .FirstOrDefaultAsync(l => l.Id == id);

            return lesson is null ? NotFound() : Ok(lesson);
        }

        [HttpPost]
        public async Task<ActionResult<Lesson>> CreateLesson(Lesson lesson)
        {
            if (lesson is null)
            {
                return BadRequest(new { message = "Invalid lesson data." });
            }

            context.Lessons.Add(lesson);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, lesson);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLesson(int id, Lesson updatedLesson)
        {
            if (id != updatedLesson.Id)
            {
                return BadRequest(new { message = "Lesson ID mismatch." });
            }

            var existingLesson = await context.Lessons.FindAsync(id);

            if (existingLesson is null)
            {
                return NotFound();
            }

            existingLesson.Room = updatedLesson.Room;
            existingLesson.Link = updatedLesson.Link;
            existingLesson.Date = updatedLesson.Date;
            existingLesson.SubjectId = updatedLesson.SubjectId;
            existingLesson.TeacherId = updatedLesson.TeacherId;
            existingLesson.GroupId = updatedLesson.GroupId;
            context.Entry(existingLesson).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating lesson." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var existingLesson = await context.Lessons.FindAsync(id);

            if (existingLesson is null)
            {
                return NotFound();
            }

            context.Lessons.Remove(existingLesson);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Error deleting the lesson." });
            }

            return NoContent();
        }
    }
}
