using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
        {
            var groups = await context.Groups
                .Include(g => g.Students)
                .Include(g => g.Subjects)
                .Include(g => g.Lessons)
                .Include(g => g.Assignments)
                .ToListAsync();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var group = await context.Groups
                .Include(g => g.Students)
                .Include(g => g.Subjects)
                .Include(g => g.Lessons)
                .Include(g => g.Assignments)
                .FirstOrDefaultAsync(g => g.Id == id);

            return group is null ? NotFound() : Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<Group>> CreateGroup(Group group)
        {
            if (group is null)
            {
                return BadRequest(new { message = "Invalid group data." });
            }

            context.Groups.Add(group);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, Group updatedGroup)
        {
            if (id != updatedGroup.Id)
            {
                return BadRequest(new { message = "Group ID mismatch." });
            }

            var existingGroup = await context.Groups.FindAsync(id);
            if (existingGroup is null)
            {
                return NotFound();
            }

            existingGroup.Name = updatedGroup.Name;

            context.Entry(existingGroup).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error updating group." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await context.Groups.FindAsync(id);
            if (group is null)
            {
                return NotFound();
            }

            context.Groups.Remove(group);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}