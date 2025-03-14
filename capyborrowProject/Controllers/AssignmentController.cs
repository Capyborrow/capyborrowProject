using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using capyborrowProject.Data;
using capyborrowProject.Service;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController (ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<Assignment>> GetAssignment(int id)
        {
            var assignment = await context.Assignments
                .Include(a => a.Lesson)
                .FirstOrDefaultAsync(a => a.Id == id);

            return assignment is null ? NotFound() : Ok(assignment);
        }

        [HttpPost]
        public async Task<ActionResult<Assignment>> CreateAssignment(Assignment assignment)
        {
            if (assignment is null)
            {
                return BadRequest(new { message = "Invalid assignment data." });
            }

            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, Assignment updatedAssignment)
        {
            if (id != updatedAssignment.Id)
            {
                return BadRequest(new { message = "Assignment ID mismatch." });
            }

            var existingAssignment = await context.Assignments.FindAsync(id);

            if (existingAssignment is null)
            {
                return NotFound();
            }

            existingAssignment.Name = updatedAssignment.Name;
            existingAssignment.Description = updatedAssignment.Description;
            existingAssignment.DueDate = updatedAssignment.DueDate;
            existingAssignment.LessonId = updatedAssignment.LessonId;
            context.Entry(existingAssignment).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating assignment." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var existingAssignment = await context.Assignments.FindAsync(id);

            if (existingAssignment is null)
            {
                return NotFound();
            }

            context.Assignments.Remove(existingAssignment);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Error deleting the assignment." });
            }

            return NoContent();
        }
    }
}
