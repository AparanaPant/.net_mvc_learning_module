using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

[Route("guest/courses")]
public class GuestCoursesController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly GraceDbContext _context;

    public GuestCoursesController(UserManager<ApplicationUser> userManager, GraceDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // ✅ Fetch all courses the guest is enrolled in
        var guestCourses = await _context.StudentSessions
            .Where(ss => ss.StudentID == user.Id)
            .Include(ss => ss.Session)
            .ThenInclude(s => s.Course)
            .Select(ss => ss.Session.Course)
            .Distinct()
            .ToListAsync();

        // ✅ Fetch all courses WITHOUT removing already enrolled courses
        var allCourses = await _context.Course
            .ToListAsync(); // Don't filter out enrolled courses

        ViewData["UserID"] = user.Id;
        ViewData["GuestCourses"] = guestCourses;
        ViewData["AllCourses"] = allCourses; // ✅ Show all courses

        return View("~/Views/Guest/Courses/Index.cshtml");
    }

    [Authorize(Roles = "Guest")]
    [HttpGet("/Guest/RegisterCourse")]
    public async Task<IActionResult> RegisterCourse(int sessionId, [FromServices] StudentService studentService)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Identity/Account/Login", new { returnUrl = $"/Guest/RegisterCourse?sessionId={sessionId}" });
        }

        var isRegistered = await studentService.RegisterStudentToSession(user.Id, sessionId);

        if (isRegistered)
        {
            TempData["SuccessMessage"] = "✅ Successfully enrolled in the course session.";
        }
        else
        {
            TempData["InfoMessage"] = "⚠ You are already registered for this session.";
        }

        return Redirect("/Guest/Dashboard");
    }


    [Route("Details/{courseId}")]
    public async Task<IActionResult> CourseDetails(string courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrEmpty(courseId))
        {
            return NotFound("Invalid Course ID.");
        }

        var course = await _context.Course
            .Include(c => c.Sessions)
            .ThenInclude(s => s.EducatorSessions)
            .ThenInclude(es => es.Educator)
            .FirstOrDefaultAsync(c => c.CourseID == courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // ✅ Fetch Sessions & Educator Details Using Joins
        var courseSessions = await _context.Session
            .Where(s => s.CourseID == courseId)
            .Select(session => new GuestSessionViewModel
            {
                SessionID = session.SessionID,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                EducatorName = _context.Users
                    .Where(u => session.EducatorSessions.Any(es => es.EducatorID == u.Id))
                    .Select(u => u.FirstName + " " + u.LastName)
                    .FirstOrDefault() ?? "Unknown Educator"
            })
            .ToListAsync();

        var viewModel = new GuestCourseDetailsViewModel
        {
            Course = course,
            Sessions = courseSessions
        };

        return View("~/Views/Guest/Courses/Details.cshtml", viewModel);
    }
}

    public class GuestCourseDetailsViewModel
{
    public Course Course { get; set; }
    public List<GuestSessionViewModel> Sessions { get; set; }
}

public class GuestSessionViewModel
{
    public int SessionID { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string EducatorName { get; set; }
}

