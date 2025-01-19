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
    [Route("api/[controller]")]
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model) //, RefreshToken refresh
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
            //var token = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.UserId == user.Id); //?
            
            bool isPasswordValid = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(); // Invalid password
            }

            var accessToken = _jwtService.GenerateAccessToken(new {Email = user.Email, Role = user.Role});
            var refreshToken = _jwtService.GenerateRefreshToken(new { Email = user.Email });
            _context.RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = refreshToken});
            
            await _context.SaveChangesAsync();

            Response.Cookies.Append("jwt", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                MaxAge = TimeSpan.FromDays(1)
            });

            return Ok(new {
                AccessToken = accessToken
            });
        }
    }
}
