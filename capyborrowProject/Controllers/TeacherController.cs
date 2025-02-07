using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController(ApplicationDbContext context) : ControllerBase
    {
        [Authorize(Roles = "teacher")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetAllTeachers()
        {
            var teachers = await context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Lessons)
                .ToListAsync();

            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetTeacher(string id)
        {
            var teacher = await context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(t => t.Id == id);

            return teacher is null ? NotFound() : Ok(teacher);
        }

        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher(Teacher teacher)
        {
            if (teacher is null)
            {
                return BadRequest(new { message = "Invalid teacher data." });
            }

            context.Teachers.Add(teacher);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(string id, Teacher updatedTeacher)
        {
            if (id != updatedTeacher.Id)
            {
                return BadRequest(new { message = "Teacher ID mismatch." });
            }

            var existingTeacher = await context.Teachers.FindAsync(id);
            if (existingTeacher is null)
            {
                return NotFound();
            }

            existingTeacher.FirstName = existingTeacher.FirstName;
            existingTeacher.MiddleName = existingTeacher.MiddleName;
            existingTeacher.LastName = existingTeacher.LastName;
            existingTeacher.ProfilePicture = existingTeacher.ProfilePicture;

            context.Entry(existingTeacher).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating teacher." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(string id)
        {
            var teacher = await context.Teachers.FindAsync(id);
            if (teacher is null)
            {
                return NotFound();
            }

            teacher.Subjects.Clear();
            teacher.Lessons.Clear();

            context.Teachers.Remove(teacher);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
