using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using capyborrowProject.Data;
using capyborrowProject.Service;
using capyborrowProject.Models.DTOs;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController(ApplicationDbContext context, BlobStorageService blobStorageService) : ControllerBase
    {
        [HttpPost("CreateAssignmentForLesson")]
        public async Task<IActionResult> CreateAssignmentForLesson(CreateAssignmentDto createAssignmentDto)
        {
            var lesson = await context.Lessons
                .Include(l => l.Group)
                    .ThenInclude(g => g.Students)
                .FirstOrDefaultAsync(l => l.Id == createAssignmentDto.LessonId);

            if (lesson is null)
                return NotFound("Lesson not found");

            var assignment = new Assignment
            {
                Name = createAssignmentDto.Name,
                Description = createAssignmentDto.Description,
                CreatedDate = DateTime.Now,
                DueDate = createAssignmentDto.DueDate,
                IsAutomaticallyClosed = createAssignmentDto.IsAutomaticallyClosed,
                MaxScore = createAssignmentDto.MaxScore,
                IsSubmittable = createAssignmentDto.IsSubmittable,
                LessonId = createAssignmentDto.LessonId,
                Lesson = lesson
            };

            if (createAssignmentDto.AssignmentFiles is not null && createAssignmentDto.AssignmentFiles.Count != 0)
            {
                foreach (var file in createAssignmentDto.AssignmentFiles)
                {
                    using var stream = file.OpenReadStream();
                    var fileUrl = await blobStorageService.UploadFileAsync(stream, file.FileName, file.ContentType ?? "application/octet-stream");

                    assignment.AssignmentFiles.Add(new AssignmentFile
                    {
                        FileUrl = fileUrl,
                        FileName = file.FileName,
                    });
                }
            }

            await context.Assignments.AddAsync(assignment);
            await context.SaveChangesAsync();

            if (createAssignmentDto.IsSubmittable)
            {
                List<string> studentIdsToAssign;

                if (createAssignmentDto.StudentIds is not null && createAssignmentDto.StudentIds.Count != 0)
                {
                    studentIdsToAssign = createAssignmentDto.StudentIds;
                }
                else
                {
                    if (lesson.Group is null)
                        return BadRequest("Group not found");

                    if (lesson.Group.Students is null || lesson.Group.Students.Count == 0)
                        return BadRequest("No students in group");

                    studentIdsToAssign = [.. lesson.Group.Students.Select(s => s.Id)];
                }

                var studentAssignments = studentIdsToAssign.Select(studentId => new StudentAssignment
                {
                    StudentId = studentId,
                    AssignmentId = assignment.Id
                });

                await context.StudentAssignments.AddRangeAsync(studentAssignments);
                await context.SaveChangesAsync();
            }

            return Ok(new
            {
                Message = "Assignment created successfully",
                AssignmentId = assignment.Id,
            });
        }

        [HttpPut("SubmitAssignment/{studentId}/{assignmentId}")]
        public async Task<IActionResult> SubmitStudentAssignment(string studentId, int assignmentId)
        {
            var studentAssignment = await context.StudentAssignments.FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.AssignmentId == assignmentId);

            if (studentAssignment is null)
                return NotFound("Student assignment not found");

            studentAssignment.SubmittedAt = DateTime.Now;
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Assignment submitted successfully",
                StudentAssignmentId = studentAssignment.AssignmentId,
            });
        }

        [HttpPut("CancelAssignmentSubmission/{studentId}/{assignmentId}")]
        public async Task<IActionResult> CancelAssignmentSubmission(string studentId, int assignmentId)
        {
            var studentAssignment = await context.StudentAssignments.FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.AssignmentId == assignmentId);

            if (studentAssignment is null)
                return NotFound("Student assignment not found");

            studentAssignment.SubmittedAt = null;
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Assignment submission cancelled successfully",
                StudentAssignmentId = studentAssignment.AssignmentId,
            });
        }
    }
}
