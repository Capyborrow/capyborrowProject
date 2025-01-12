using capyborrowProject.Data;
using capyborrowProject.Helpers;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetAllTeachers()
        {
            var teachers = await context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Notifications)
                .ToListAsync();

            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeacher(int id)
        {
            var teacher = await context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Notifications)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher is null)
            {
                return NotFound();
            }

            return Ok(teacher);
        }

        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacherToAdd)
        {
            var teacher = new Teacher
            {
                Id = teacherToAdd.Id,
                FirstName = teacherToAdd.FirstName,
                MiddleName = teacherToAdd.MiddleName,
                LastName = teacherToAdd.LastName,
                PasswordHash = PasswordHelper.HashPassword(teacherToAdd.PasswordHash),
                Email = teacherToAdd.Email,
                ProfilePicture = teacherToAdd.ProfilePicture,
                Role = teacherToAdd.Role,
                Subjects = teacherToAdd.Subjects,
                Notifications = teacherToAdd.Notifications
            };

            context.Teachers.Add(teacher);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Teachers.Any(e => e.Id == id))
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
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Notifications)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher is null)
            {
                return NotFound();
            }

            context.Teachers.Remove(teacher);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
