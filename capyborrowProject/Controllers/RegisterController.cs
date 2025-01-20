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
    public class RegisterController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly JwtService _jwtService;
        public RegisterController(APIContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("A user with this email already exists.");

            var passwordHash = PasswordHelper.HashPassword(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role,
                RefreshTokens = new List<RefreshToken>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created("", new { Message = "User registered successfully." });

        }


    }
}
