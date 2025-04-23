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
        public async Task<IActionResult> CreateAssignmentForLesson([FromForm] CreateAssignmentDto createAssignmentDto)
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

            if (createAssignmentDto.AssignmentFiles?.Count > 0)
            {
                var fileUploadTasks = createAssignmentDto.AssignmentFiles.Select(async file =>
                {
                    await using var stream = file.OpenReadStream();
                    var fileUrl = await blobStorageService.UploadFileAsync(stream, file.FileName, "assignment", file.ContentType ?? "application/octet-stream");

                    return new AssignmentFile
                    {
                        FileUrl = fileUrl,
                        FileName = file.FileName
                    };
                });

                var uploadedFiles = await Task.WhenAll(fileUploadTasks);
                assignment.AssignmentFiles = [.. uploadedFiles];
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

        [HttpPost("CreateAssignmentWithPreupload")]
        public async Task<IActionResult> CreateAssignmentForLessonWithPreload([FromForm] CreateAssignmentWithTempFilesDto createAssignmentDto)
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

            if (createAssignmentDto.TempFileIds?.Count > 0)
            {
                var tempFiles = await context.TempAssignmentFiles
                    .Where(f => createAssignmentDto.TempFileIds.Contains(f.Id))
                    .ToListAsync();

                assignment.AssignmentFiles = [.. tempFiles.Select(temp => new AssignmentFile
                {
                    FileName = temp.FileName,
                    FileUrl = temp.FileUrl
                })];

                context.TempAssignmentFiles.RemoveRange(tempFiles);
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

        [HttpDelete("DeleteAssignment/{assignmentId}")]
        public async Task<IActionResult> DeleteAssignment(int assignmentId)
        {
            var assignment = await context.Assignments
                    .Include(a => a.AssignmentFiles)
                    .Include(a => a.StudentAssignments)
                        .ThenInclude(s => s.SubmissionFiles)
                    .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment is null)
                return NotFound("Assignment not found");

            foreach (var file in assignment.AssignmentFiles)
            {
                await blobStorageService.DeleteFileAsync(file.FileName, "assignment");
            }

            foreach (var sa in assignment.StudentAssignments)
            {
                if (sa.SubmissionFiles is not null)
                {
                    foreach (var sf in sa.SubmissionFiles)
                    {
                        await blobStorageService.DeleteFileAsync(sf.FileName, "submission");
                    }
                }
            }

            context.Assignments.Remove(assignment);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Assignment and related files deleted successfully",
                AssignmentId = assignmentId
            });
        }

        [HttpPut("SubmitAssignment/{studentId}/{assignmentId}")]
        public async Task<ActionResult<AssignmentStatus>> SubmitStudentAssignment(string studentId, int assignmentId)
        {
            var studentAssignment = await context.StudentAssignments
                .Include(sa => sa.Assignment)
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.AssignmentId == assignmentId);

            if (studentAssignment is null)
                return NotFound("Student assignment not found");

            studentAssignment.SubmittedAt = DateTime.Now;
            await context.SaveChangesAsync();

            var currentStatus = studentAssignment.ComputedStatus;

            return Ok(new
            {
                Message = "Assignment submitted successfully",
                StudentAssignmentId = studentAssignment.AssignmentId,
                Status = currentStatus
            });
        }

        [HttpPut("CancelAssignmentSubmission/{studentId}/{assignmentId}")]
        public async Task<ActionResult<AssignmentStatus>> CancelAssignmentSubmission(string studentId, int assignmentId)
        {
            var studentAssignment = await context.StudentAssignments
                .Include(sa => sa.Assignment)
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.AssignmentId == assignmentId);

            if (studentAssignment is null)
                return NotFound("Student assignment not found");

            studentAssignment.SubmittedAt = null;
            await context.SaveChangesAsync();

            var currentStatus = studentAssignment.ComputedStatus;

            return Ok(new
            {
                Message = "Assignment submission cancelled successfully",
                StudentAssignmentId = studentAssignment.AssignmentId,
                Status = currentStatus
            });
        }
        [HttpPost("GradeStudentAssigment")]
        public async Task<IActionResult> GradeStudentAssigment(GradeStudentAssigmentDto gradeStudentAssigmentDto)
        {
            var studentAssignment = await context.StudentAssignments
                .Include(sa => sa.Assignment)
                .FirstOrDefaultAsync(sa =>
                    sa.StudentId == gradeStudentAssigmentDto.StudentId &&
                    sa.AssignmentId == gradeStudentAssigmentDto.AssignmentId);

            if (studentAssignment == null)
            {
                return NotFound("StudentAssignment not found");
            }
            if (gradeStudentAssigmentDto.Score < 0 || gradeStudentAssigmentDto.Score > studentAssignment.Assignment.MaxScore)
            {
                return BadRequest($"Score must be between 0 and {studentAssignment.Assignment.MaxScore}");
            }

            studentAssignment.Score = gradeStudentAssigmentDto.Score;
            await context.SaveChangesAsync();

            return Ok("Score updated");
        }

        [HttpPut("CancelAssignmentGrade/{studentId}/{assignmentId}")]
        public async Task<IActionResult> CancelAssignmentGrade(string studentId, int assignmentId)
        {
            var studentAssignment = await context.StudentAssignments
                .FirstOrDefaultAsync(sa =>
                    sa.StudentId == studentId &&
                    sa.AssignmentId == assignmentId);

            if (studentAssignment is null)
                return NotFound("StudentAssignment not found");

            studentAssignment.Score = null;

            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Grade cancelled successfully",
                StudentAssignmentId = assignmentId
            });
        }

    }
}
