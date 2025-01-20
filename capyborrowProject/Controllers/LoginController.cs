using Azure.Core;
using capyborrowProject.Data;
using capyborrowProject.Helpers;
using capyborrowProject.Models;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/Auth/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly JwtService _jwtService;
        public LoginController(APIContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest model) //, RefreshToken refresh
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);

            if (user is null)
            {
                return BadRequest(ModelState);
            }
            
            bool isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized();
            }

            var accessToken = _jwtService.GenerateAccessToken(new {email = user.Email, role = user.Role});
            var refreshToken = _jwtService.GenerateRefreshToken(new { email = user.Email });
            _context.RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = refreshToken});
            
            await _context.SaveChangesAsync();

            Response.Cookies.Append("jwt", refreshToken, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                MaxAge = TimeSpan.FromDays(1)
            });

            return Ok(new {
                AccessToken = accessToken
            });
        }
    }
}
