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
    public async Task<IActionResult> ImportUsers(IFormFile file)
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

            ApplicationUser user;

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

            student.GroupId = groupId;
            successGroups.Add($"{record.StudentEmail} -> {record.GroupName}");
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

            var isLessonDuplicate = await _context.Lessons.AnyAsync(l => l.Date == lessonDto.Date
                && l.SubjectId == subjectId
                && l.TeacherId == teacherId
                && l.GroupId == groupId);

            if (isLessonDuplicate)
            {
                errors.Add($"Duplicate lesson found: Subject '{lessonDto.SubjectName}', Group '{lessonDto.GroupName}', Teacher '{lessonDto.TeacherEmail}' at {lessonDto.Date}.");
                continue;
            }

            if (!Enum.IsDefined(typeof(LessonType), lessonDto.Type))
            {
                errors.Add($"Invalid lesson type '{lessonDto.Type}' for Subject '{lessonDto.SubjectName}'.");
                continue;
            }
            if (!Enum.TryParse(lessonDto.Type.ToString(), out LessonType lessonType))
            {
                errors.Add($"Failed to parse lesson type '{lessonDto.Type}' for Subject '{lessonDto.SubjectName}'.");
                continue;
            }

            if (!Enum.IsDefined(typeof(LessonStatus), lessonDto.Status))
            {
                errors.Add($"Invalid lesson status '{lessonDto.Status}' for subject '{lessonDto.SubjectName}'.");
                continue;
            }
            if (!Enum.TryParse(lessonDto.Status.ToString(), out LessonStatus lessonStatus))
            {
                errors.Add($"Failed to parse lesson status '{lessonDto.Status}' for Subject '{lessonDto.SubjectName}'.");
                continue;
            }

            var lesson = new Lesson
            {
                Room = lessonDto.Room,
                Link = lessonDto.Link,
                Date = lessonDto.Date,
                Type = lessonType,
                Status = lessonStatus,
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

    [Route("ImportAssignmentsFromCSV")]
    [HttpPost]
    public async Task<IActionResult> ImportAssignments(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Upload a valid CSV file.");

        var errors = new List<string>();
        var successAssignments = new List<string>();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<AssignmentsCsvMap>();

        var records = csv.GetRecords<AssignmentsCsvDto>().ToList();

        foreach (var assignmentDto in records)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Group)
                .ThenInclude(g => g.Students)
                .FirstOrDefaultAsync(l => l.Teacher!.Email == assignmentDto.TeacherEmail && l.Date.HasValue && l.Date.Value == assignmentDto.LessonDate);

            if (lesson == null)
            {
                errors.Add($"Lesson not found for Teacher: {assignmentDto.TeacherEmail} on {assignmentDto.LessonDate}");
                continue;
            }

            if (lesson.Group == null)
            {
                errors.Add($"No group associated with lesson {lesson.Id}");
                continue;
            }

            var assignment = new Assignment
            {
                Name = assignmentDto.Name,
                Description = assignmentDto.Description,
                CreatedDate = DateTime.Now,
                DueDate = assignmentDto.DueDate,
                IsAutomaticallyClosed = assignmentDto.IsAutomaticallyClosed,
                MaxScore = assignmentDto.MaxScore,
                LessonId = lesson.Id
            };

            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            var groupStudents = lesson.Group.Students.ToList();
            var studentAssignments = new List<StudentAssignment>();

            foreach (var student in groupStudents)
            {
                var studentAssignment = new StudentAssignment
                {
                    Score = assignmentDto.Score,
                    StudentId = student.Id,
                    AssignmentId = assignment.Id,
                    SubmittedAt = assignmentDto.SubmittedAt
                };

                studentAssignments.Add(studentAssignment);
            }

            _context.StudentAssignments.AddRange(studentAssignments);
            _context.SaveChanges();

            successAssignments.Add($"Assignment '{assignment.Name}' created with {studentAssignments.Count} students.");
        }

        await _context.SaveChangesAsync();

        var response = new
        {
            message = "Import completed.",
            totalProcessed = records.Count,
            totalSuccessful = successAssignments.Count,
            totalFailed = errors.Count,
            successfulLessons = successAssignments,
            errors
        };

        return errors.Count > 0 ? BadRequest(response) : Ok(response);
    }
}   


