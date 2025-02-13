using capyborrowProject.Models;
using capyborrowProject.Models.CsvFilesModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CsvHelper;
using System.Globalization;
using System.Security.Claims;


namespace capyborrowProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CsvImportController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    public CsvImportController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [Route("ImportUsersFromCSV")]
    [HttpPost]
    public async Task<IActionResult> ImportStudents(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Upload a valid CSV file.");

        var errors = new List<string>();
        var successUsers = new List<string>();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<BulkRegisterMap>();

        var records = csv.GetRecords<BulkRegister>().ToList();

        foreach (var userDto in records)
        {
            if (await _userManager.FindByEmailAsync(userDto.Email) != null)
            {
                errors.Add($"A user with this email '{userDto.Email}' already exists.");
                continue;
            }

            ApplicationUser user = null;

            if (userDto.Role.ToLower() == "student")
            {
                user = new Student
                {
                    FirstName = userDto.FirstName,
                    MiddleName = userDto.MiddleName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    EmailConfirmed = true
                };
            }
            else if (userDto.Role.ToLower() == "teacher")
            {
                user = new Teacher
                {
                    FirstName = userDto.FirstName,
                    MiddleName = userDto.MiddleName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    UserName = userDto.Email,
                    EmailConfirmed = true
                };
            }

            else
            {
                errors.Add($"Invalid role for {userDto.Email}: {userDto.Role}");
                continue;
            }

            var createUserResult = await _userManager.CreateAsync(user, userDto.Password);
            if (!createUserResult.Succeeded)
            {
                errors.Add($"Error creating {userDto.Email}: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                continue;
            }

            await _userManager.AddToRoleAsync(user, userDto.Role);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            await _userManager.AddClaimsAsync(user, claims);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
            {
                errors.Add($"Error confirming email for  {userDto.Email}: {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
                continue;
            }

            successUsers.Add(userDto.Email);
        }

        var response = new
        {
            message = "Import completed.",
            totalProcessed = records.Count,
            totalSuccessful = successUsers.Count,
            totalFailed = errors.Count,
            successfulUsers = successUsers,
            errors
        };

        return errors.Count > 0 ? BadRequest(response) : Ok(response);
    }
}

