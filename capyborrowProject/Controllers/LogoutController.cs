using capyborrowProject.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly APIContext _context;

        public LogoutController(APIContext context)
        {
            _context = context;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { Message = "No refresh token found." });
            }

            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null)
            {
                return NotFound(new { Message = "Refresh token not found or already invalidated." });
            }

            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();

            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });

            return Ok(new { Message = "Logged out successfully." });
        }
    }
}
