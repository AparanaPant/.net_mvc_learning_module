using GraceProject.Data;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;


namespace GraceProject.Controllers.Educator
{
    [Route("Educator/Quizzes")]
    public class EducatorQuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GraceDbContext _context;

        public EducatorQuizzesController(UserManager<ApplicationUser> userManager, GraceDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Route("{courseId}")]
        public async Task<IActionResult> Index(string courseId)
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

            // Get the educator's session ID for this course
            var educatorSessionId = await _context.EducatorSession
                .Where(es => es.EducatorID == user.Id && es.Session.CourseID == courseId)
                .Select(es => es.SessionID)
                .FirstOrDefaultAsync();

            // Filter Default Quizzes: Active and not archived
            var defaultQuizzes = await _context.Quizzes
                .Where(q => q.CourseID == courseId && q.IsActive && !q.IsArchived)
                .ToListAsync();

            // Filter Session Quizzes: Active and not archived
            var sessionQuizzes = await _context.Quizzes
                .Where(q => q.SessionID == educatorSessionId && q.IsActive && !q.IsArchived)
                .ToListAsync();

            ViewData["SessionID"] = educatorSessionId;

            var model = new QuizzesViewModel
            {
                Course = course,
                DefaultQuizzes = defaultQuizzes,
                SessionQuizzes = sessionQuizzes
            };

            return View("~/Views/Educator/Quizzes/Index.cshtml", model);
        }

    }
}
