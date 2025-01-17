using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using capyborrowProject.Models;
using capyborrowProject.Data;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grade>>> GetAllGrades()
        {
            var grades = await context.Grades
                .Include(g => g.Student)
                .Include(g => g.Lesson)
                .ToListAsync();

            return Ok(grades);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Grade>>> GetGrade(int id)
        {
            var grade = await context.Grades
                .Include(g => g.Student)
                .Include(g => g.Lesson)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade is null)
            {
                return NotFound();
            }

            return Ok(grade);
        }

        [HttpPost]
        public async Task<ActionResult<Grade>> PostGrade(Grade gradeToAdd)
        {
            context.Grades.Add(gradeToAdd);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGrade), new { id = gradeToAdd.Id }, gradeToAdd);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGrade(int id, Grade grade)
        {
            if (id != grade.Id)
            {
                return BadRequest();
            }

            context.Entry(grade).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Grades.Any(g => g.Id == id))
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
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await context.Grades
                .Include(g => g.Student)
                .Include(g => g.Lesson)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grade is null)
            {
                return NotFound();
            }

            context.Grades.Remove(grade);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
