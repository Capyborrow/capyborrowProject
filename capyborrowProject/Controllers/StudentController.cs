using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Helpers;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await context.Students
                .Include(s => s.Group)
                .Include(s => s.Grades)
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent(int id)
        {
            var student = await context.Students
                .Include(s => s.Group)
                .Include(s => s.Grades)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student studentToAdd)
        {
            var student = new Student
            {
                Id = studentToAdd.Id,
                FirstName = studentToAdd.FirstName,
                MiddleName = studentToAdd.MiddleName,
                LastName = studentToAdd.LastName,
                PasswordHash = PasswordHelper.HashPassword(studentToAdd.PasswordHash),
                Email = studentToAdd.Email,
                ProfilePicture = studentToAdd.ProfilePicture,
                Role = studentToAdd.Role,
                GroupId = studentToAdd.GroupId,
                Course = studentToAdd.Course,
                Grades = studentToAdd.Grades,
                Assignments = studentToAdd.Assignments,
                Attendances = studentToAdd.Attendances,
                Group = studentToAdd.Group,
                RefreshTokens = studentToAdd.RefreshTokens
            };

            context.Students.Add(student);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            context.Entry(student).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Students.Any(e => e.Id == id))
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
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await context.Students
                .Include(s => s.Group)
                .Include(s => s.Grades)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            context.Students.Remove(student);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
