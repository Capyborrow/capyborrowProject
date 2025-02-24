using capyborrowProject.Models;
using capyborrowProject.Models.CsvFilesModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using System.Security.Claims;
using capyborrowProject.Data;

namespace capyborrowProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CsvImportController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public CsvImportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
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

    [Route("ImportSubjectsFromCSV")]
    [HttpPost]
    public async Task<IActionResult> ImportSubjects(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Upload a valid CSV file.");

        var errors = new List<string>();
        var successSubjects = new List<string>();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<SubjectCsvMap>();

        var records = csv.GetRecords<SubjectCsvDto>().ToList();

        foreach (var subjectDto in records)
        {
            if (await _context.Subjects.AnyAsync(s => s.Name == subjectDto.Name))
            {
                errors.Add($"Subject '{subjectDto.Name}' already exists.");
                continue;
            }

            var subject = new Subject
            {
                Name = subjectDto.Name
            };

            _context.Subjects.Add(subject);
            successSubjects.Add(subjectDto.Name);
        }

        await _context.SaveChangesAsync();

        var response = new
        {
            message = "Import completed.",
            totalProcessed = records.Count,
            totalSuccessful = successSubjects.Count,
            totalFailed = errors.Count,
            successfulSubjects = successSubjects,
            errors
        };

        return errors.Count > 0 ? BadRequest(response) : Ok(response);
    }

    [Route("ImportLessonsFromCSV")]
    [HttpPost]
    public async Task<IActionResult> ImportLessons(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Upload a valid CSV file.");

        var errors = new List<string>();
        var successLessons = new List<string>();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<LessonCsvMap>();

        var records = csv.GetRecords<LessonCsvDto>().ToList();

        foreach (var lessonDto in records)
        {
            var subjectId = await _context.Subjects
                        .Where(s => s.Name == lessonDto.SubjectName)
                        .Select(s => s.Id)
                        .FirstOrDefaultAsync();

            if (subjectId == 0)
            {
                errors.Add($"Subject '{lessonDto.SubjectName}' not found.");
                continue;
            }

            var teacherId = await _context.Teachers
                .Where(t => t.Email == lessonDto.TeacherEmail)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (teacherId == null)
            {
                errors.Add($"Teacher with email '{lessonDto.TeacherEmail}' not found.");
                continue;
            }

            var groupId = await _context.Groups
                .Where(g => g.Name == lessonDto.GroupName)
                .Select(g => g.Id)
                .FirstOrDefaultAsync();

            if (groupId == 0)
            {
                errors.Add($"Group '{lessonDto.GroupName}' not found.");
                continue;
            }

            if (!Enum.IsDefined(typeof(Lesson.LessonType), lessonDto.Type))
            {
                errors.Add($"Invalid lesson type '{lessonDto.Type}' for Subject '{lessonDto.SubjectName}'.");
                continue;
            }

            var lesson = new Lesson
            {
                Room = lessonDto.Room,
                Link = lessonDto.Link,
                Date = lessonDto.Date,
                Type = (Lesson.LessonType)lessonDto.Type,
                SubjectId = subjectId,
                TeacherId = teacherId,
                GroupId = groupId
            };

            _context.Lessons.Add(lesson);
            successLessons.Add($"{lessonDto.SubjectName} at {lessonDto.Room} on {lessonDto.Date}");
        }


        await _context.SaveChangesAsync();

        var response = new
        {
            message = "Import completed.",
            totalProcessed = records.Count,
            totalSuccessful = successLessons.Count,
            totalFailed = errors.Count,
            successfulLessons = successLessons,
            errors
        };

        return errors.Count > 0 ? BadRequest(response) : Ok(response);
    }

    [Route("ImportGroupsFromCSV")]
    [HttpPost]
    public async Task<IActionResult> ImportGroups(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Upload a valid CSV file.");

        var errors = new List<string>();
        var successGroups = new List<string>();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<GroupCsvMap>();

        var records = csv.GetRecords<GroupCsvDto>().ToList();

        foreach (var record in records)
        {
            var groupId = await _context.Groups
                .Where(g => g.Name == record.GroupName)
                .Select(g => g.Id)
                .FirstOrDefaultAsync();

            if (groupId == 0)
            {
                var newGroup = new Group { Name = record.GroupName };
                _context.Groups.Add(newGroup);
                await _context.SaveChangesAsync();
                groupId = newGroup.Id;
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == record.StudentEmail);
            if (student == null)
            {
                errors.Add($"Student with email '{record.StudentEmail}' not found.");
                continue;
            }

            if (student.GroupId != groupId)
            {
                student.GroupId = groupId;
                successGroups.Add($"{record.StudentEmail} -> {record.GroupName}");
            }

            //var group = await _context.Groups
            //    .Include(g => g.Students)
            //    .FirstOrDefaultAsync(g => g.Name == record.GroupName);

            //if (group == null)
            //{
            //    group = new Group { Name = record.GroupName };
            //    _context.Groups.Add(group);
            //    await _context.SaveChangesAsync();
            //}

            //var student = await _context.Students
            //    .FirstOrDefaultAsync(s => s.Email == record.StudentEmail);

            //if (student == null)
            //{
            //    errors.Add($"Student with email '{record.StudentEmail}' not found.");
            //    continue;
            //}

            //if (!group.Students.Contains(student))
            //{
            //    group.Students.Add(student);
            //    successGroups.Add($"{student.Email} -> {group.Name}");
            //}
        }

        await _context.SaveChangesAsync();

        var response = new
        {
            message = "Import completed.",
            totalProcessed = records.Count,
            totalSuccessful = successGroups.Count,
            totalFailed = errors.Count,
            successfulEntries = successGroups,
            errors
        };

        return errors.Count > 0 ? BadRequest(response) : Ok(response);
    }

}


