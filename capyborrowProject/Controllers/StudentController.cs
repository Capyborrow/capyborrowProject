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
    public class StudentController : ControllerBase
    {
        private readonly APIContext _context; 

        public StudentController(APIContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

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
                firstName = studentToAdd.firstName,
                middleName = studentToAdd.middleName,
                lastName = studentToAdd.lastName,
                passwordHash = PasswordHelper.HashPassword(studentToAdd.passwordHash),
                email = studentToAdd.email,
                group = studentToAdd.group,
                course = studentToAdd.course
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Students.Any(e => e.Id == id))
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
            var student = await _context.Students.FindAsync(id);

            if (student is null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
