using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly JwtService _jwtService;
        public RefreshTokenController(APIContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Refresh(HttpContext context)
        {
            var cookies = context.Request.Cookies;
            if (!cookies.ContainsKey("jwt"))
            {
                return Unauthorized();
            }

            var refreshToken = cookies["jwt"];

            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user is null)
            {
                return Forbid();
            }
            return Ok();
        }
    }
}
