using Azure.Core;
using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;
        public AuthController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, JwtService jwtService, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _emailService = emailService;

        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest("A user with this email already exists.");

            ApplicationUser user = null;

            if (request.Role.ToLower() == "student")
            {
                user = new Student
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.Email
                };
            }
            else if (request.Role.ToLower() == "teacher")
            {
                user = new Teacher
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.Email
                };
            }
            else
            {
                return BadRequest("Invalid role.");
            }

            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (user is null)
                return BadRequest("User registration failed.");

            if (!await _roleManager.RoleExistsAsync(request.Role))
                await _roleManager.CreateAsync(new IdentityRole(request.Role));
            await _userManager.AddToRoleAsync(user, request.Role);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            await _userManager.AddClaimsAsync(user, claims);

            return Created("", new { Message = "User registered successfully." });

        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return BadRequest("User not authenticated");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized();


            var claims = _userManager.GetClaimsAsync(user).Result.ToList();
            Console.WriteLine(claims);
            var refreshToken = _jwtService.GenerateRefreshToken(claims/*new List<Claim> { new Claim(ClaimTypes.Email, user.Email) }*/);
            var accessToken = _jwtService.GenerateAccessToken(claims);

            _context.RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = refreshToken });

            await _context.SaveChangesAsync();

            Response.Cookies.Append("jwt", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                MaxAge = TimeSpan.FromDays(1)
            });

            return Ok(new
            {
                AccessToken = accessToken
            });
        }

        [Route("Refresh")]
        [HttpPost]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { Message = "Refresh token is missing or invalid." });

            var foundToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken);

            if (foundToken is null)
                return Unauthorized(new { Message = "Refresh token is missing or invalid." });

            var user = foundToken.User;

            if (user is null)
                return Forbid("Bearer");

            var claimsPrincipal = _jwtService.ValidateRefreshToken(refreshToken);

            if (claimsPrincipal == null || claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value != user.Email)
                return Forbid("Bearer");

            var claims = _userManager.GetClaimsAsync(user).Result.ToList();

            var accessToken = _jwtService.GenerateAccessToken(claims);

            var newRefreshToken = _jwtService.GenerateRefreshToken(claims/*new List<Claim> { new Claim(ClaimTypes.Email, user.Email) }*/);

            var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

            if (existingToken != null)
                existingToken.Token = newRefreshToken;

            await _context.SaveChangesAsync();

            Response.Cookies.Append("jwt", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                MaxAge = TimeSpan.FromDays(1)
            });

            return Ok(new { AccessToken = accessToken });
        }

        [Route("Logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["jwt"];

            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

            if (user is null)
                return Forbid("Bearer");

            var existingToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

            if (existingToken != null)
                user.RefreshTokens.Remove(existingToken);

            await _context.SaveChangesAsync();

            Response.Cookies.Delete("refresh");

            return Ok(new { Message = "User logged out successfully." });
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return BadRequest("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = $"http://localhost:5174/reset_password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            var subject = "Reset Your Password";
            var body = $"Click <a href='{resetLink}'>here</a> to reset your password.";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok(new { Message = "Password reset link sent to email." });
        }


        [Route("ResetPassword")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return BadRequest("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Password reset successful." });
        }


    }
}
