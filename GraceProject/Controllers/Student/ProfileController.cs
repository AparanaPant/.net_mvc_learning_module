using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GraceProject.Data;
using GraceProject.Models;

namespace GraceProject.Controllers.Student
{
    [Route("student/[controller]")]
    public class ProfileController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(
            GraceDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ProfileController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return View("~/Views/Student/Profile/profile.cshtml", user);
        }

        [HttpGet("edit")]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View("~/Views/Student/Profile/edit.cshtml", model);
        }

        [HttpPost("editpost")]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Student/Profile/edit.cshtml", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("~/Views/Student/Profile/edit.cshtml", model);
        }

        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || profileImage == null || profileImage.Length == 0)
                return BadRequest("Invalid upload.");

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "profile-pictures");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueName = Guid.NewGuid() + "_" + Path.GetFileName(profileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.ProfilePictureUrl = "/profile-pictures/" + uniqueName;
            await _userManager.UpdateAsync(user);

            return Ok(new { imageUrl = user.ProfilePictureUrl });
        }
    }
}
public class EditProfileViewModel
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; } // Just for display
}
