using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("educator/courses")]
public class DashboardController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly GraceDbContext _context;

    public DashboardController(UserManager<ApplicationUser> userManager, GraceDbContext context)
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

        // Fetch all sessions assigned to the educator
        var educatorSessions = await _context.EducatorSession
            .Where(es => es.EducatorID == user.Id)
            .Include(es => es.Session)
            .ThenInclude(s => s.Course)
            .ToListAsync();

        // Extract assigned courses
        var assignedCourses = educatorSessions
            .Where(es => es.Session != null && es.Session.Course != null)
            .Select(es => es.Session.Course)
            .Distinct()
            .ToList();

        var educatorCourses = assignedCourses
            .GroupBy(course => course)
            .Select(group => new
            {
                Course = group.Key,
                Sessions = educatorSessions
                    .Where(es => es.Session.Course.CourseID == group.Key.CourseID)
                    .Select(es => es.Session)
                    .ToList()
            })
            .ToList();

        // Fetch all courses excluding the ones the educator is assigned to
        var allCourses = await _context.Course
            .Where(c => !assignedCourses.Select(ac => ac.CourseID).Contains(c.CourseID))
            .ToListAsync();

        ViewData["UserID"] = user.Id;
        ViewData["EducatorCourses"] = educatorCourses.Cast<dynamic>().ToList();
        ViewData["AllCourses"] = allCourses; 

        return View("~/Views/Educator/Courses/index.cshtml");
    }

    [HttpGet("GenerateRegistrationLink")]
    public IActionResult GenerateRegistrationLink(int sessionId)
    {
        if (sessionId <= 0)
        {
            return BadRequest("Invalid Session ID");
        }

        // Generate the registration link for the session
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        string registrationLink = $"{baseUrl}/Identity/Account/Login?returnUrl=/Student/Register?sessionId={sessionId}";

        return Json(new { link = registrationLink });
    }

    [Route("details/{courseId}")]
    public async Task<IActionResult> CourseDetails(string courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // ✅ Ensure CourseID is valid
        if (string.IsNullOrEmpty(courseId))
        {
            return NotFound("Invalid Course ID.");
        }

        // ✅ Fetch the course with its sessions
        var course = await _context.Course
            .Include(c => c.Sessions)  // Include sessions related to the course
            .FirstOrDefaultAsync(c => c.CourseID == courseId);

        // ✅ If no course is found, return NotFound
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // ✅ Fetch the educator's session(s) for this course
        var educatorSessions = await _context.EducatorSession
            .Where(es => es.EducatorID == user.Id && es.Session.CourseID == courseId)
            .Include(es => es.Session)
            .ToListAsync();

        // ✅ Get the first available session for the educator (for sharing)
        var firstSession = educatorSessions.FirstOrDefault()?.Session;

        ViewData["Course"] = course;
        ViewData["SessionID"] = firstSession?.SessionID; // Store session ID for the share button

        return View("~/Views/Educator/Courses/Details.cshtml", course);
    }



}
