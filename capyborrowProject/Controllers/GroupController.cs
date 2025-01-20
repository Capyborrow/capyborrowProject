using capyborrowProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController(APIContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
        {
            var groups = await context.Groups
                .Include(g => g.Students)
                .ToListAsync();

            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroup(int id)
        {
            var group = await context.Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null)
            {
                return NotFound();
            }

            return Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group groupToAdd)
        {
            context.Groups.Add(groupToAdd);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroup), new { id = groupToAdd.Id }, groupToAdd);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group group)
        {
            if (id != group.Id)
            {
                return BadRequest();
            }

            context.Entry(group).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!context.Groups.Any(e => e.Id == id))
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
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await context.Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id);

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
