using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GraceProject.Data;
using GraceProject.Models;
using Microsoft.Extensions.Logging;

namespace GraceProject.Controllers.Student
{
    [Route("student/[controller]")]
    public class ProfileController : Controller
    {
        private readonly GraceDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(GraceDbContext context, UserManager<ApplicationUser> userManager, ILogger<ProfileController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Student/Profile/profile.cshtml", user);
        }

        [HttpGet("edit")]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View("~/Views/Student/Profile/edit.cshtml", user);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogError("Model state is not valid. Errors: {Errors}", string.Join(", ", errors));
                Console.WriteLine("Model state is not valid. Errors: {0}", string.Join(", ", errors));
                return View("~/Views/Student/Profile/edit.cshtml", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

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

            var updateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            _logger.LogError("Failed to update user. Errors: {Errors}", string.Join(", ", updateErrors));
            Console.WriteLine("Failed to update user. Errors: {0}", string.Join(", ", updateErrors));
            return View("~/Views/Student/Profile/edit.cshtml", model);
        }
    }
}
