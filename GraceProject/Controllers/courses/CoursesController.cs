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

        var course = await _context.Course 
            .FirstOrDefaultAsync(c => c.CourseID == courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        
        ViewData["Course"] = course;

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
