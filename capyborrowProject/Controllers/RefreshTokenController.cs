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

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { Message = "Refresh token is missing or invalid." });
            }

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user is null)
            {
                return Forbid("Invalid refresh token.");
            }

            var claimsPrincipal = _jwtService.ValidateRefreshToken(refreshToken);
            if (claimsPrincipal == null || claimsPrincipal.FindFirst("Email")?.Value != user.Email)
            {
                return Forbid("Invalid refresh token.");
            }

            var accessToken = _jwtService.GenerateAccessToken(new
            {
                Email = user.Email,
                Role = user.Role
            });

            // Optionally update or invalidate old refresh tokens (best practice)
            // e.g., you could rotate the refresh token here:
            // var newRefreshToken = _jwtService.GenerateRefreshToken(new { Email = user.Email });
            // var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            // if (existingToken != null) existingToken.Token = newRefreshToken;

            // Save changes (if you rotate tokens)
            // await _context.SaveChangesAsync();

            return Ok(new { AccessToken = accessToken });
        }
    }
    
}
