using GraceProject.Data;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraceProject.Controllers.Student
{
    [Route("Student/Courses")]
    public class StudentCoursesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GraceDbContext _context;

        public StudentCoursesController(UserManager<ApplicationUser> userManager, GraceDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get student courses
            var studentSessions = await _context.StudentSessions
                .Where(ss => ss.StudentID == user.Id)
                .Include(ss => ss.Session)
                .ThenInclude(s => s.Course)
                .ToListAsync();

            var studentCourses = studentSessions
                .GroupBy(ss => ss.Session.Course)
                .Select(group => new StudentCourseViewModel
                {
                    Course = group.Key
                })
                .ToList();

            ViewData["UserID"] = user.Id;

            return View("~/Views/Student/Courses/Index.cshtml", studentCourses);
        }

        [Route("Details/{courseId}", Name = "CourseDetails")]
        public async Task<IActionResult> Details(string courseId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseID == courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            // ✅ Get sessions the student is registered for with Educator details using explicit joins
            var studentSessions = await _context.StudentSessions
                .Where(ss => ss.StudentID == user.Id && ss.Session.CourseID == courseId)
                .Include(ss => ss.Session)
                .ThenInclude(s => s.EducatorSessions)
                .ThenInclude(es => es.Educator) // Explicitly fetch Educator (ApplicationUser)
                .Select(ss => new
                {
                    ss.Session.SessionID,
                    EducatorName = _context.Users
                        .Where(u => ss.Session.EducatorSessions.Any(es => es.EducatorID == u.Id))
                        .Select(u => u.FirstName + " " + u.LastName)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // ✅ Extract session list and ensure educator name is properly retrieved
            var sessionList = studentSessions.Select(ss => new StudentSessionViewModel
            {
                SessionID = ss.SessionID,
                EducatorName = !string.IsNullOrEmpty(ss.EducatorName) ? ss.EducatorName : "Instructor Not Assigned"
            }).ToList();

            var model = new StudentCourseDetailsViewModel
            {
                Course = course,
                Sessions = sessionList
            };

            return View("~/Views/Student/Courses/Details.cshtml", model);
        }

    }
}
