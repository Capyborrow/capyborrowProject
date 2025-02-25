using capyborrowProject.Data;
using capyborrowProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUserById(string id)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            return user is null ? NotFound() : Ok(user);
        }
    }
}
