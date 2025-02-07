using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using capyborrowProject.Data;


namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController (ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
            var subject = await context.Subjects
                .Include(s => s.Teachers)
                .Include(s => s.Groups)
                .FirstOrDefaultAsync(s => s.Id == id);

            return subject is null ? NotFound() : Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> CreateSubject(Subject subject)
        {
            if (subject is null)
            {
                return BadRequest(new { message = "Invalid student data." });
            }

            context.Subjects.Add(subject);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, subject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, Subject updatedSubject)
        {
            if (id != updatedSubject.Id)
            {
                return BadRequest(new { message = "Subject ID mismatch." });
            }

            var existingSubject = await context.Subjects.FindAsync(id);

            if (existingSubject is null)
            {
                return NotFound();
            }
            existingSubject.Name = updatedSubject.Name;
            context.Entry(existingSubject).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                return StatusCode(500, new { message = "Error updating subject." });
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var existingSubject = await context.Subjects.FindAsync(id);

            if (existingSubject is null)
            {
                return NotFound();
            }

            existingSubject.Teachers.Clear();
            existingSubject.Groups.Clear();

            context.Subjects.Remove(existingSubject);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Error deleting the subject." });
            }

            return NoContent();
        }

    }
}
