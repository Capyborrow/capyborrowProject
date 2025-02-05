using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController(ApplicationDbContext context) : ControllerBase
    {
        [Authorize(Policy = "StudentOrAdmin")]
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
        public async Task<ActionResult<Student>> GetStudent(string id)
        {
            var student = await context.Students
                .Include(s => s.Group)
                .Include(s => s.Grades)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            return student is null ? NotFound() : Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            if (student is null)
            {
                return BadRequest(new { message = "Invalid student data." });
            }

            context.Students.Add(student);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(string id, Student updatedStudent)
        {
            if (id != updatedStudent.Id)
            {
                return BadRequest(new { message = "Student ID mismatch." });
            }

            var existingStudent = await context.Students.FindAsync(id);
            if (existingStudent is null)
            {
                return NotFound();
            }

            existingStudent.FirstName = updatedStudent.FirstName;
            existingStudent.MiddleName = updatedStudent.MiddleName;
            existingStudent.LastName = updatedStudent.LastName;
            existingStudent.ProfilePicture = updatedStudent.ProfilePicture;
            existingStudent.Course = updatedStudent.Course;
            existingStudent.GroupId = updatedStudent.GroupId;

            context.Entry(existingStudent).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating student." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await context.Students.FindAsync(id);
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