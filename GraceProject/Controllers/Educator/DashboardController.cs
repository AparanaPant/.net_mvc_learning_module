using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("educator/dashboard")]
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

        var educatorSessions = await _context.EducatorSession
            .Where(es => es.EducatorID == user.Id)
            .Include(es => es.Session)
            .ThenInclude(s => s.Course) 
            .ToListAsync();

        // Group Sessions by Course
        var educatorCourses = educatorSessions
             .Where(es => es.Session != null && es.Session.Course != null) 
             .GroupBy(es => es.Session.Course)
             .Select(group => new
             {
                 Course = group.Key,
                 Sessions = group.Select(es => es.Session).ToList()
             })
             .ToList();

        ViewData["UserID"] = user.Id;
        ViewData["EducatorCourses"] = educatorCourses.Cast<dynamic>().ToList();

        return View("~/Views/Educator/Home/Dashboard.cshtml");
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
}



