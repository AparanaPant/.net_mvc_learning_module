using GraceProject.Data;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraceProject.Controllers.Student
{
    [Route("Student/Quizzes")]
    public class StudentQuizzesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GraceDbContext _context;
        private readonly TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        public StudentQuizzesController(UserManager<ApplicationUser> userManager, GraceDbContext context)
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

            // Get the session ID for the student
            var studentSessionId = await _context.StudentSessions
                .Where(ss => ss.StudentID == user.Id && ss.Session.CourseID == courseId)
                .Select(ss => ss.SessionID)
                .FirstOrDefaultAsync();

            // Get Default Quizzes (Created by Admin) and include UserQuizzes to count attempts.
            var defaultQuizzes = await _context.Quizzes
                .Where(q => q.CourseID == courseId && q.IsActive)
                .Include(q => q.UserQuizzes)
                .ToListAsync();

            // Get Session Quizzes (Created by Educator) and include UserQuizzes.
            var sessionQuizzes = await _context.Quizzes
                .Where(q => q.SessionID == studentSessionId && q.IsActive)
                .Include(q => q.UserQuizzes)
                .ToListAsync();

            // Populate the AttemptsUsed property for each quiz.
            foreach (var quiz in defaultQuizzes)
            {
                quiz.AttemptsUsed = quiz.UserQuizzes.Count(uq => uq.UserId == user.Id);
            }
            foreach (var quiz in sessionQuizzes)
            {
                quiz.AttemptsUsed = quiz.UserQuizzes.Count(uq => uq.UserId == user.Id);
            }

            // Convert current UTC time to CST.
            var currentTimeCST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);

            var model = new QuizzesViewModel
            {
                Course = course,
                DefaultQuizzes = defaultQuizzes,
                SessionQuizzes = sessionQuizzes,
                CurrentTimeCST = currentTimeCST // Pass the current CST time to view.
            };

            return View("~/Views/Student/Quizzes/Index.cshtml", model);
        }

        [HttpGet("Take/{quizId}")]
        public async Task<IActionResult> Take(int quizId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            var currentTimeCST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, cstZone);

            // Ensure quiz is only accessible after the start time
            if (currentTimeCST < quiz.StartTimeCST)
            {
                return BadRequest("This quiz is not yet available.");
            }

            var model = new QuizAttemptViewModel
            {
                Quiz = quiz,
                Questions = quiz.Questions.ToList()
            };

            return View("~/Views/Student/Quizzes/Take.cshtml", model);
        }
    }
}
