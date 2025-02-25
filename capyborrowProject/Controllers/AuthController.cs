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
using Microsoft.AspNetCore.Http.HttpResults;
using capyborrowProject.Models.AuthModels;

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
                return BadRequest(new { message = "Wrong register credentials", errors = ModelState });

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new { message = "A user with this email already exists." });

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
                return BadRequest(new { message = "Invalid role." });
            }

            var createUserResult = await _userManager.CreateAsync(user, request.Password);

            if (user is null)
                return BadRequest(new { message = "User registration failed." });

            await _userManager.AddToRoleAsync(user, request.Role);


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            await _userManager.AddClaimsAsync(user, claims);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:5174/confirm_email_status/?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            var subject = "Confirm Your Email";
            var body = $"Click <a href='{confirmationLink}'>here</a> to confirm your email.";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Created(user.Email, new { Message = "User registered successfully." });

        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) 
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Wrong login credentials", errors = ModelState });

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return BadRequest(new { message = "User not found." });

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized(new {message = "Wrong login or password."});

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized(new { message = "Email not confirmed." });
            }


            var claims = _userManager.GetClaimsAsync(user).Result.ToList();

            var refreshToken = _jwtService.GenerateRefreshToken(claims);
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

        [Route("RefreshAccessToken")]
        [HttpPost]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var refreshToken = Request.Cookies["jwt"];

            var foundToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == refreshToken);

            if (foundToken is null)
                return Unauthorized(new { Message = "Refresh token is missing or invalid." });

            var user = foundToken.User;

            if (user is null)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Forbidden" });

            var claimsPrincipal = _jwtService.ValidateRefreshToken(refreshToken);

            if (claimsPrincipal == null || claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value != user.Email)
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Forbidden" });

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

            if (user is null) return Unauthorized(new { Message = "User not found." });

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
                return BadRequest(new { message = "Wrong forgot password credentials", errors = ModelState });

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return BadRequest(new { message = "User not found." });

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
                return BadRequest(new { message = "Wring reset password data.", errors = ModelState });

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return BadRequest(new { message = "User not found." });

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { Message = "Password reset successful." });
        }

        [Route("ResendConfirmationEmail")]
        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return BadRequest(new { message = "User not found." });

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new { message = "Email already confirmed" });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:5174/confirm_email_status/?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";
  
            var subject = "Confirm Your Email";
            var body = $"Click <a href='{confirmationLink}'>here</a> to confirm your email.";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return Ok(new { mesage = "Confirmation email resent." });
        }


        [Route("ConfirmEmail")]
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new { message = "Wrong data.", errors = ModelState }); 
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to confirm email.", errors = result.Errors });
            }

            return Ok(new { Message = "Email confirmed." });
        }

        [Route("CheckEmailConfirmation")]
        [HttpPost]
        public async Task<IActionResult> CheckEmailConfirmation([FromBody] CheckEmailConfirmationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Wrong data.", errors = ModelState });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var confirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Ok(new { Confirmed = confirmed });
        }
    }
}
