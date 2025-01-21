using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace capyborrowProject.Controllers
{
    [Route("api/Auth/[controller]")]
    [ApiController]
    public class RefreshController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly JwtService _jwtService;
        public RefreshController(APIContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost]
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
                return Forbid("Bearer");
            }

            var claimsPrincipal = _jwtService.ValidateRefreshToken(refreshToken);
            if (claimsPrincipal == null || claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value != user.Email)
            {
                return Forbid("Bearer");
            }

            var accessToken = _jwtService.GenerateAccessToken(new
            {
                email = user.Email,
                role = user.Role
            });

            var newRefreshToken = _jwtService.GenerateRefreshToken(new { email = user.Email });
            var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (existingToken != null) existingToken.Token = newRefreshToken;

            await _context.SaveChangesAsync();

            Response.Cookies.Append("jwt", newRefreshToken, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                MaxAge = TimeSpan.FromDays(1)
            });

            return Ok(new { AccessToken = accessToken });
        }
    }
    
}
