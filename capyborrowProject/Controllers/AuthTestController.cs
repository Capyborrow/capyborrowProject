using Azure.Core;
using capyborrowProject.Data;
using capyborrowProject.Helpers;
using capyborrowProject.Models;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace capyborrowProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthTestController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly JwtService _jwtService;

        public AuthTestController(APIContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] SignUp model)
        {
            if (await _context.User.AnyAsync(u => u.Email == model.Email))
                return BadRequest("A user with this email already exists.");

            var passwordHash = PasswordHelper.HashPassword(model.Password);

            var user = new User
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = model.Role,
                RefreshToken = null,
                RefreshTokenExpiry = DateTime.UtcNow
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == model.Email);


            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                return Unauthorized(new { Message = "Invalid email or password." });



            //var roles = foundUser.Roles ?? new List<string>();
            //string accessToken = GenerateJwtToken(foundUser.Username, roles, _configuration["JWT:AccessTokenSecret"], TimeSpan.FromSeconds(30));
            //string refreshToken = GenerateJwtToken(foundUser.Username, null, _configuration["JWT:RefreshTokenSecret"], TimeSpan.FromDays(1));

            //// Update refresh token in users.json
            //foundUser.RefreshToken = refreshToken;
            //await SaveUsersAsync(users);

            //Response.Cookies.Append("jwt", refreshToken, new CookieOptions
            //{
            //    HttpOnly = true,
            //    SameSite = SameSiteMode.None,
            //    Secure = true,
            //    MaxAge = TimeSpan.FromDays(1)
            //});

            //return Ok(new { accessToken });

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")] //Separate refresh and access tokens refreshing method..
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken); //Maybe change to Email search

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized(new { Message = "Invalid or expired refresh token." });

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(); //

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Login model, string refreshToken)
        {

            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == model.Email); //Maybe change to Email search

            if (user == null)
                return NotFound(new { Message = "User not found." });

            user.RefreshToken = null;
            user.RefreshTokenExpiry = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Logged out successfully." });
        }
    }


    //  [ApiController]
    //  [Route("api/[controller]")]
    //  public class AuthController : ControllerBase
    //  {
    //      private readonly APIContext _context;
    //      private readonly JwtService _jwtService;

    //      public AuthController(APIContext context, JwtService jwtService)
    //      {
    //          _context = context;
    //          _jwtService = jwtService;
    //      }

    //      [HttpPost("sign-up")]
    //      public async Task<IActionResult> SignUp([FromBody] SignUp model)
    //      {
    //          if (await _context.Users.AnyAsync(u => u.Email == model.Email))
    //              return BadRequest("User with this email already exists.");

    //          var user = new User
    //          {
    //              FirstName = model.FirstName,
    //              MiddleName = model.MiddleName,
    //              LastName = model.LastName,
    //              Email = model.Email,
    //              PasswordHash = PasswordHelper.HashPassword(model.Password),
    //              Role = model.Role
    //          };

    //          _context.Users.Add(user);
    //          await _context.SaveChangesAsync();

    //          return Ok("User registered successfully.");
    //      }

    //      [HttpPost("login")]
    //      public async Task<IActionResult> Login([FromBody] Login model)
    //      {
    //          //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
    //          var user = _context.Users.SingleOrDefault(u => u.Email == request.Email);
    //          if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
    //              return Unauthorized("Invalid credentials.");

    //          var accessToken = _jwtService.GenerateAccessToken(user);
    //          var refreshToken = _jwtService.GenerateRefreshToken();

    //          user.RefreshToken = newRefreshToken;
    //          user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
    //          await _context.SaveChangesAsync();

    //          return Ok(new TokenResponse { AccessToken = accessToken, RefreshToken = newRefreshToken });
    //      }

    //      [HttpPost("refresh-token")]
    //      public async Task<IActionResult> RefreshToken([FromBody] TokenResponse tokenResponse)
    //      {
    //          var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == tokenResponse.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
    //          if (user == null)
    //              return Unauthorized("Invalid or expired refresh token.");

    //          var accessToken = _jwtService.GenerateAccessToken(user);
    //          var newRefreshToken = _jwtService.GenerateRefreshToken();

    //          user.RefreshToken = newRefreshToken;
    //          user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
    //          await _context.SaveChangesAsync();

    //          return Ok(new TokenResponse { AccessToken = accessToken, RefreshToken = newRefreshToken });
    //      }

    //[HttpPost("sign-out")]
    //  public async Task<IActionResult> SignOut()
    //  {
    //      var userId = GetUserIdFromClaims();
    //      var user = await _context.Users.FindAsync(userId);

    //      if (user == null)
    //          return Unauthorized();

    //      user.RefreshToken = null;
    //      user.RefreshTokenExpiry = DateTime.MinValue;
    //      await _context.SaveChangesAsync();

    //      return Ok("Signed out successfully.");
    //  }

    //      private int GetUserIdFromClaims()
    //      {
    //          var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    //          return int.TryParse(userIdClaim, out var userId) ? userId : -1;
    //      }
    //  }

}
