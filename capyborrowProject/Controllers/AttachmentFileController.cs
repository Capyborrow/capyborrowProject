using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentFileController(ApplicationDbContext context, BlobStorageService blobStorageService) : ControllerBase
    {
        [HttpPost("{assignmentId}/upload")]
        public async Task<IActionResult> UploadAssignmentFile(int assignmentId, IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("Invalid file.");

            if (!context.Assignments.Any(a => a.Id == assignmentId))
                return NotFound("Assignment not found.");

            using var stream = file.OpenReadStream();
            var fileUrl = await blobStorageService.UploadFileAsync(stream, file.FileName, "assignment", file.ContentType);

            var assignmentFile = new AssignmentFile
            {
                FileUrl = fileUrl,
                FileName = file.FileName,
                AssignmentId = assignmentId
            };

            context.AssignmentFiles.Add(assignmentFile);
            await context.SaveChangesAsync();

            return Ok(assignmentFile);
        }

        [HttpPost("{studentId}/{assignmentId}/upload")]
        public async Task<IActionResult> UploadSubmissionFile(string studentId, int assignmentId, IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest("Invalid file.");

            if (!context.StudentAssignments.Any(a => a.StudentId == studentId) || !context.StudentAssignments.Any(a => a.AssignmentId == assignmentId))
                return NotFound("Student assignment not found.");

            using var stream = file.OpenReadStream();
            var fileUrl = await blobStorageService.UploadFileAsync(stream, file.FileName, "submission", file.ContentType);

            var submissionFile = new SubmissionFile
            {
                FileUrl = fileUrl,
                FileName = file.FileName,
                StudentId = studentId,
                AssignmentId = assignmentId
            };

            context.SubmissionFiles.Add(submissionFile);
            await context.SaveChangesAsync();

            return Ok(submissionFile);
        }

        [HttpGet("download/{fileType}/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileType, int fileId)
        {
            try
            {
                var fileName = string.Empty;

                if (fileType.Equals("submission", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = await context.SubmissionFiles
                        .Where(sf => sf.Id == fileId)
                        .Select(sf => sf.FileName)
                        .FirstOrDefaultAsync();
                }
                else if (fileType.Equals("assignment", StringComparison.OrdinalIgnoreCase))
                {
                    fileName = await context.AssignmentFiles
                        .Where(af => af.Id == fileId)
                        .Select(af => af.FileName)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    return BadRequest(new { Message = "Invalid file type" });
                }

                if (string.IsNullOrEmpty(fileName))
                    return NotFound(new { Message = "File not found" });

                var fileStream = await blobStorageService.DownloadFileAsync(fileName, fileType);
                
                if (fileStream is null)
                    return NotFound(new { Message = "File not found in Blob Storage" });

                var contentType = "application/octet-stream";
                return File(fileStream, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { Message = "File not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [HttpDelete("delete/{fileType}/{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileType, int fileId)
        {
            try
            {
                if (fileType.Equals("submission", StringComparison.OrdinalIgnoreCase))
                {
                    var submissionFile = await context.SubmissionFiles.FirstOrDefaultAsync(sf => sf.Id == fileId);
                    if (submissionFile is null)
                        return NotFound(new { Message = "Submission file not found" });

                    bool isDeletedFromStorage = await blobStorageService.DeleteFileAsync(submissionFile.FileName, fileType);
                    if (!isDeletedFromStorage)
                        return NotFound(new { Message = "File not found in Blob Storage" });

                    context.SubmissionFiles.Remove(submissionFile);
                    await context.SaveChangesAsync();

                    return Ok(new { Message = "Submission file deleted successfully" });
                }
                else if (fileType.Equals("assignment", StringComparison.OrdinalIgnoreCase))
                {
                    var assignmentFile = await context.AssignmentFiles.FirstOrDefaultAsync(af => af.Id == fileId);
                    if (assignmentFile is null)
                        return NotFound(new { Message = "Assignment file not found" });

                    bool isDeletedFromStorage = await blobStorageService.DeleteFileAsync(assignmentFile.FileName, fileType);
                    if (!isDeletedFromStorage)
                        return NotFound(new { Message = "File not found in Blob Storage" });

                    context.AssignmentFiles.Remove(assignmentFile);
                    await context.SaveChangesAsync();

                    return Ok(new { Message = "Assignment file deleted successfully" });
                }

                return BadRequest(new { Message = "Invalid file type" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
    }
}
