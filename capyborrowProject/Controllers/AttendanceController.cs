//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using capyborrowProject.Models;
//using capyborrowProject.Data;

//namespace capyborrowProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AttendanceController(ApplicationDbContext context) : ControllerBase
//    {
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Attendance>>> GetAllAttendances()
//        {
//            var attendances = await context.Attendances
//                .Include(a => a.Lesson)
//                .Include(a => a.Student)
//                .ToListAsync();

//            return Ok(attendances);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance(int id)
//        {
//            var attendance = await context.Attendances
//                .Include(a => a.Lesson)
//                .Include(a => a.Student)
//                .FirstOrDefaultAsync(a => a.Id == id);

//            if (attendance is null)
//            {
//                return NotFound();
//            }

//            return Ok(attendance);
//        }

//        [HttpPost]
//        public async Task<ActionResult<Attendance>> PostAttendance(Attendance attendanceToAdd)
//        {
//            context.Attendances.Add(attendanceToAdd);
//            await context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetAttendance), new { id = attendanceToAdd.Id }, attendanceToAdd);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutAttendance(int id, Attendance attendance)
//        {
//            if (id != attendance.Id)
//            {
//                return BadRequest();
//            }

//            context.Entry(attendance).State = EntityState.Modified;

//            try
//            {
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!context.Attendances.Any(a => a.Id == id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteAttendance(int id)
//        {
//            var attendance = await context.Attendances
//                .Include(a => a.Lesson)
//                .Include(a => a.Student)
//                .FirstOrDefaultAsync(a => a.Id == id);

//            if (attendance is null)
//            {
//                return NotFound();
//            }

//            context.Attendances.Remove(attendance);
//            await context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
