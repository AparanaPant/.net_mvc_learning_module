using GraceProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraceProject.Controllers.Student
{
    [Route("Student")]
    public class GradeBookController : Controller

    {
        private readonly GraceDbContext _context;
        public GradeBookController(GraceDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        [Route("Gradebook/{courseId}/{sessionId}/{studentId}")]
        public IActionResult Gradebook(string courseId, int sessionId, string studentId)
        {
            if (_context.Course == null)
            {
                return NotFound("Courses table is not available.");
            }
            var course = _context.Course.FirstOrDefault(c => c.CourseID == courseId);

            var quizzes = _context.Quizzes
                .Where(q =>
                    (!string.IsNullOrWhiteSpace(courseId) && q.CourseID == courseId) ||
                    (sessionId > 0 && q.SessionID == sessionId)
                )
                .Select(q => new QuizResultViewModel
                {
                    QuizTitle = q.Title,
                    TotalScore = q.TotalScore ?? 0,
                    ObtainedScore = q.UserQuizzes
                        .Where(uq => uq.UserId == studentId)
                        .Sum(uq => uq.Score.GetValueOrDefault()),
                    // Set the flag based on whether the student has any quiz attempts
                    IsAttempted = q.UserQuizzes.Any(uq => uq.UserId == studentId)
                })
                .ToList();

            // Filter out unattempted quizzes for overall total calculations
            var attemptedQuizzes = quizzes.Where(q => q.IsAttempted).ToList();

            var model = new GradebookViewModel
            {
                CourseTitle = course?.Title ?? "Unknown Course",
                Quizzes = quizzes,
                // Only sum totals for quizzes that were attempted
                TotalScore = attemptedQuizzes.Sum(q => q.TotalScore),
                ObtainedScore = attemptedQuizzes.Sum(q => q.ObtainedScore)
            };

            return View("~/Views/Student/Courses/gradebook.cshtml", model);
        }


    }
}
