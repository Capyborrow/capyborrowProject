using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAllSubjects()
        {
            var subjects = await context.Subjects
                .Include(s => s.Teacher)
                .ToListAsync();

            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubject(int id)
        {
            var subject = await context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject is null)
            {
                return NotFound();
            }

            return Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> PostSubject(Subject subjectToAdd)
        {
            context.Subjects.Add(subjectToAdd);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubject), new { id = subjectToAdd.Id }, subjectToAdd);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(int id, Subject subject)
        {
            if (id != subject.Id)
            {
                return BadRequest();
            }

            context.Entry(subject).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Subjects.Any(s => s.Id == id))
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
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject is null)
            {
                return NotFound();
            }

            context.Subjects.Remove(subject);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
