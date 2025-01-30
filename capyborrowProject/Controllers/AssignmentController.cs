//using capyborrowProject.Data;
//using capyborrowProject.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace capyborrowProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AssignmentController(ApplicationDbContext context) : ControllerBase
//    {
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Assignment>>> GetAllAssignments()
//        {
//            var assignments = await context.Assignments
//                .Include(a => a.Students)
//                .Include(a => a.Lesson)
//                .ToListAsync();

//            return Ok(assignments);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignment(int id)
//        {
//            var assignment = await context.Assignments
//                .Include(a => a.Students)
//                .Include(a => a.Lesson)
//                .FirstOrDefaultAsync(a => a.Id == id);

//            if (assignment is null)
//            {
//                return NotFound();
//            }

//            return Ok(assignment);
//        }

//        [HttpPost]
//        public async Task<ActionResult<Assignment>> PostAssignment(Assignment assignmentToAdd)
//        {
//            context.Assignments.Add(assignmentToAdd);
//            await context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetAssignment), new { id = assignmentToAdd.Id }, assignmentToAdd);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutAssignment(int id, Assignment assignment)
//        {
//            if (id != assignment.Id)
//            {
//                return BadRequest();
//            }

//            context.Entry(assignment).State = EntityState.Modified;

//            try
//            {
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!context.Assignments.Any(a => a.Id == id))
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
//        public async Task<IActionResult> DeleteAssignment(int id)
//        {
//            var assignment = await context.Assignments
//                .Include(a => a.Students)
//                .Include(a => a.Lesson)
//                .FirstOrDefaultAsync(a => a.Id == id);

//            if (assignment is null)
//            {
//                return NotFound();
//            }

//            context.Assignments.Remove(assignment);
//            await context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
