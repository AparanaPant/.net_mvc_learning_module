// Controllers/StudentController.cs
using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Controllers
{
    public class StudentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly StudentService _studentService;
        private readonly GraceDbContext _context;

        public StudentController(GraceDbContext context,UserManager<ApplicationUser> userManager, StudentService studentService)
        {
            _userManager = userManager;
            _studentService = studentService;
            _context = context;

        }

        [Authorize(Roles = "Student")]
        [HttpGet("/Student/RegisterCourse")]
        public async Task<IActionResult> RegisterCourse(int sessionId, [FromServices] StudentService studentService)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login", new { returnUrl = $"/Student/RegisterCourse?sessionId={sessionId}" });
            }

            var isRegistered = await studentService.RegisterStudentToSession(user.Id, sessionId);

            if (isRegistered)
            {
                TempData["SuccessMessage"] = "Successfully registered for the course.";
            }
            else
            {
                TempData["InfoMessage"] = "You are already registered for this course.";
            }

            return Redirect("/Student/Home/Dashboard");

        }

        [Authorize(Roles = "Student")]
        [HttpGet("/Student/Home/Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Get all courses the student is registered to
            var registeredCourses = await _context.StudentSessions
                .Where(ss => ss.StudentID == user.Id)
                .Include(ss => ss.Session)
                .ThenInclude(s => s.Course)
                .Select(ss => ss.Session.Course)
                .Distinct()
                .ToListAsync();

            ViewData["RegisteredCourses"] = registeredCourses;

            return View("~/Views/Student/Home/Dashboard.cshtml");
        }



    }
}
