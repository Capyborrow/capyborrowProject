﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using capyborrowProject.Models;
using capyborrowProject.Data;
using capyborrowProject.Models.DTOs;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController (ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("getAssignmentsForLesson/{lessonId}/{studentId}")]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAssignmentsByLessonAndStudent(int lessonId, string studentId)
        {
            var assignments = await context.Assignments
                .Where(a => a.LessonId == lessonId)
                .Include(a => a.AssignmentFiles)
                .Include(a => a.StudentAssignments.Where(sa => sa.StudentId == studentId))
                    .ThenInclude(sa => sa.SubmissionFiles)
                .ToListAsync();


            var filteredAssignments = assignments
                .Where(a =>
                    !a.IsSubmittable ||
                    (a.IsSubmittable && a.StudentAssignments.Count != 0))
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedDate = a.CreatedDate,
                    MaxScore = a.MaxScore,
                    IsSubmittable = a.IsSubmittable,
                    Attachments = [.. a.AssignmentFiles.Select(af => new AssignmentFileDto
                    {
                        Id = af.Id,
                        FileName = af.FileName,
                        FileUrl = af.FileUrl
                    })],
                    StudentAssignment = a.StudentAssignments.FirstOrDefault() != null ? new StudentAssignmentDto
                    {
                        Score = a.StudentAssignments.First().Score,
                        SubmittedAt = a.StudentAssignments.First().SubmittedAt,
                        Status = a.StudentAssignments.First().ComputedStatus,
                        Submissions = [.. a.StudentAssignments.First().SubmissionFiles.Select(sf => new SubmissionFileDto
                        {
                            Id = sf.Id,
                            FileName = sf.FileName,
                            FileUrl = sf.FileUrl
                        })]
                    } : null
                });

            return Ok(filteredAssignments);
        }
    }
}
