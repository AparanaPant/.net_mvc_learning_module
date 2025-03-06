using GraceProject.Data;
using GraceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("educator/courses")]
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

        if (string.IsNullOrEmpty(courseId))
        {
            return NotFound("Invalid Course ID.");
        }

        var course = await _context.Course
            .Include(c => c.Sessions)  // Include sessions related to the course
            .FirstOrDefaultAsync(c => c.CourseID == courseId);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        var educatorSessions = await _context.EducatorSession
            .Where(es => es.EducatorID == user.Id && es.Session.CourseID == courseId)
            .Include(es => es.Session)
            .ToListAsync();

        var firstSession = educatorSessions.FirstOrDefault()?.Session;

        ViewData["Course"] = course;
        ViewData["SessionID"] = firstSession?.SessionID; // Store session ID for the share button

        return View("~/Views/Educator/Courses/Details.cshtml", course);
    }

    [HttpGet("Gradebook/{courseId}")]
    public async Task<IActionResult> GradeBook(string courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var students = await _context.StudentSessions
            .Where(ss => ss.Session.CourseID == courseId)
            .Select(ss => ss.Student)
            .Distinct()
            .ToListAsync();

        var quizzes = await _context.Quizzes
            .Where(q => q.CourseID == courseId)
            .ToListAsync();

        var gradeBook = new List<GraceProject.Models.ViewModels.GradeBookViewModel>();

        foreach (var student in students)
        {
            var studentScores = await _context.UserQuizzes
                .Where(uq => uq.UserId == student.Id && uq.Quiz.CourseID == courseId)
                .Select(uq => new GraceProject.Models.ViewModels.QuizScoreViewModel
                {
                    QuizId = uq.QuizId,
                    QuizTitle = uq.Quiz.Title,
                    Score = uq.Score,
                    CompletedAt = uq.CompletedAt
                })
                .ToListAsync();

            // Include quizzes with no scores yet (default to null)
            var allScores = quizzes.Select(quiz => studentScores.FirstOrDefault(s => s.QuizId == quiz.QuizId) ??
                new GraceProject.Models.ViewModels.QuizScoreViewModel
                {
                    QuizId = quiz.QuizId,
                    QuizTitle = quiz.Title,
                    Score = null
                }).ToList();

            gradeBook.Add(new GraceProject.Models.ViewModels.GradeBookViewModel
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                QuizScores = allScores
            });
        }

        ViewBag.CourseID = courseId;
        return View("~/Views/Educator/Courses/GradeBook.cshtml", gradeBook);
    }


}
