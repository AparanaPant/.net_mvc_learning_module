using GraceProject.Data;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraceProject.Controllers.Courses
{
    [Route("Courses/Quizzes")]
    public class QuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GraceDbContext _context;

        public QuizzesController(UserManager<ApplicationUser> userManager, GraceDbContext context)
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

            // ✅ Fetch only default quizzes created for the course
            var defaultQuizzes = await _context.Quizzes
                .Where(q => q.CourseID == courseId)
                .ToListAsync();

            var model = new QuizzesViewModel
            {
                Course = course,
                DefaultQuizzes = defaultQuizzes,
                SessionQuizzes = new List<Quiz>() // Empty list since guests don't have session quizzes
            };

            return View("~/Views/Courses/Quizzes.cshtml", model);
        }
    }
}
