using capyborrowProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetAllLessons()
        {
            var lessons = await context.Lessons
                .Include(l => l.Subject)
                .Include(l => l.Group)
                .ToListAsync();

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLesson(int id)
        {
            var lesson = await context.Lessons
                .Include(l => l.Subject)
                .Include(l => l.Group)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson is null)
            {
                return NotFound();
            }

            return Ok(lesson);
        }

        [HttpPost]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lessonToAdd)
        {
            context.Lessons.Add(lessonToAdd);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLesson), new { id = lessonToAdd.Id }, lessonToAdd);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Lesson>> PutLesson(int id, Lesson lessonToUpdate)
        {
            if (id != lessonToUpdate.Id)
            {
                return BadRequest();
            }

            context.Entry(lessonToUpdate).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Lessons.Any(e => e.Id == id))
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
        public async Task<ActionResult<Lesson>> DeleteLesson(int id)
        {
            var lesson = await context.Lessons
                .Include(l => l.Subject)
                .Include(l => l.Group)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson is null)
            {
                return NotFound();
            }

            context.Lessons.Remove(lesson);
            await context.SaveChangesAsync();

            return Ok(lesson);
        }
    }
}
