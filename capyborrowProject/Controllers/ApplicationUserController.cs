using capyborrowProject.Data;
using capyborrowProject.Models;
using capyborrowProject.Models.DTOs;
using capyborrowProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace capyborrowProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController(ApplicationDbContext context, BlobStorageService blobStorageService) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUserById(string id)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPut("EditProfile")]
        public async Task<ActionResult<EditProfileDto>> EditProfile([FromForm] EditProfileDto editProfileDto)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == editProfileDto.Id);
            
            if (user is null)
                return NotFound("User not found");

            bool isModified = false;

            if (!string.IsNullOrEmpty(editProfileDto.FirstName) && editProfileDto.FirstName != user.FirstName)
            {
                user.FirstName = editProfileDto.FirstName;
                isModified = true;
            }

            if (!string.IsNullOrEmpty(editProfileDto.MiddleName) && editProfileDto.MiddleName != user.MiddleName)
            {
                user.MiddleName = editProfileDto.MiddleName;
                isModified = true;
            }

            if (!string.IsNullOrEmpty(editProfileDto.LastName) && editProfileDto.LastName != user.LastName)
            {
                user.LastName = editProfileDto.LastName;
                isModified = true;
            }

            if (editProfileDto.ProfilePicture is not null)
            {
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    await blobStorageService.DeleteProfilePictureAsync(user.ProfilePicture);
                }

                var newProfilePictureUrl = await blobStorageService.UploadProfilePictureAsync(editProfileDto.ProfilePicture, editProfileDto.Id);
                user.ProfilePicture = newProfilePictureUrl;
                isModified = true;
            }

            if (isModified)
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }

            var updatedUser = new UserProfileDto
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                ProfilePicture = user.ProfilePicture
            };

            return Ok(updatedUser);
        }
    }
}
