using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("Courses")]
public class CoursesController : Controller
{
    private readonly GraceDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CoursesController(GraceDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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

        // ✅ Check if the Guest is already enrolled
        var isEnrolled = await _context.StudentSessions
            .AnyAsync(ss => ss.StudentID == user.Id && ss.Session.CourseID == courseId);

        // ✅ If enrolled, redirect to Student Course Details
        if (isEnrolled)
        {
            return Redirect($"~/Student/Courses/Details/{courseId}");
        }

        // ✅ If the user is a guest, fetch the course and session details
        var course = await _context.Course
            .Include(c => c.Sessions)
            .ThenInclude(s => s.EducatorSessions)
            .ThenInclude(es => es.Educator)
            .FirstOrDefaultAsync(c => c.CourseID == courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // ✅ Fetch Sessions with Educator Details
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

        // ✅ If the user is a guest, return GuestCourseDetailsViewModel
        if (await _userManager.IsInRoleAsync(user, "Guest"))
        {
            var viewModel = new GuestCourseDetailsViewModel
            {
                Course = course,
                Sessions = courseSessions
            };

            return View("~/Views/Guest/Courses/Details.cshtml", viewModel);
        }

        // ✅ If not a guest, return the default Course view
        return View("~/Views/Courses/Details.cshtml", course);
    }



    [HttpPost("RegisterToCourse")]
    public async Task<IActionResult> RegisterToCourse(string courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrEmpty(courseId))
        {
            return BadRequest("Invalid Course ID.");
        }

        var educatorService = new EducatorService(_context);
        bool success = await educatorService.RegisterEducatorToCourse(user.Id, courseId);

        if (success)
        {
            TempData["SuccessMessage"] = "✅ You have been successfully registered for this course!";
        }
        else
        {
            TempData["ErrorMessage"] = "⚠ You are already registered for this course.";
        }

        return Redirect("~/Educator/Courses");

    }

}
