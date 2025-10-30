using GraceProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace GraceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UploadController : ControllerBase
    {
        [HttpPost]
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Check if file size is greater than 1 MB (1 MB = 1 * 1024 * 1024 bytes)
            if (file.Length > 2 * 1024 * 1024)
                return BadRequest("File size exceeds 2 MB");

            // Validate image dimensions (1200x630)
            using (var image = Image.FromStream(file.OpenReadStream()))
            {
                if (image.Width > 1200 || image.Height > 630)
                {
                    return BadRequest("Image dimensions exceed 1200x630 pixels");
                }
            }

            // Generate a random 10-digit number for the file name
            var random = new Random();
            var randomFileName = random.Next(1000000000, 1999999999).ToString();

            // Get the file extension
            var fileExtension = Path.GetExtension(file.FileName);

            // Combine the random file name with the file extension
            var newFileName = randomFileName + fileExtension;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedFiles/Slides/Images", newFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { fileName = newFileName });
        }


        [HttpPost]
        [Route("SubmitInfo")]
        public IActionResult SubmitInfo([FromBody] string SlideInfo)
        {
            if (SlideInfo == null)
                return BadRequest("Invalid user information");

            // Perform any necessary processing or saving of the user information here
            // For demonstration, we simply return the received information

            return Ok(new { message = "Information submitted successfully", SlideInfo });
        }
    }
}
