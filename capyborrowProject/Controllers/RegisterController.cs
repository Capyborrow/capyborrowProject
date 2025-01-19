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
    public class RegisterController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly JwtService _jwtService;
        public RegisterController(APIContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] SignUp model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("A user with this email already exists.");

            var passwordHash = PasswordHelper.HashPassword(model.Password);

            var user = new User
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = model.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Created("", new { Message = "User registered successfully." });

        }


    }
}
