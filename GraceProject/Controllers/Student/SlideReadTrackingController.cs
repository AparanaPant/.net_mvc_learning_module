using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GraceProject.Data;
using GraceProject.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GraceProject.Controllers.Student
{
    [Route("Student/SlideReadTrackingController")]
    public class SlideReadTrackingController : Controller
    {
        private readonly GraceDbContext _context;

        public SlideReadTrackingController(GraceDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("StoreSlideReading")]
        public IActionResult StoreSlideReading([FromBody] string jsonData)
        {

            try
            {
                // Deserialize JSON data
                var slideData = JsonConvert.DeserializeObject<SlideReadTracking>(jsonData);
                if (slideData == null)
                {
                    return BadRequest("Invalid data");
                }

                // Get logged-in user ID
                var UserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                // Create a new SlideReadTracking entry
                var newEntry = new SlideReadTracking
                {
                    SlideId = slideData.SlideId,
                    UserId = UserId,
                    ReadDateTime = DateTime.UtcNow,
                    DurationSeconds = slideData.DurationSeconds
                };

                // Insert into the database
                _context.SlideReadTracking.Add(newEntry);
                _context.SaveChanges();

                return Ok("Slide read entry added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }

        }
    }
}
