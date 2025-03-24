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

            var now = DateTime.Now; // current date for comparison

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
                    IsAttempted = q.UserQuizzes.Any(uq => uq.UserId == studentId),
                    DueDate = q.DueDate,
                    IsPastDue = q.DueDate.HasValue && q.DueDate <= now
                })
                .ToList();

            // Include all quizzes that were attempted OR are past due date in the totals.
            var quizzesToCount = quizzes.Where(q => q.IsAttempted || q.IsPastDue).ToList();

            var model = new GradebookViewModel
            {
                CourseTitle = course?.Title ?? "Unknown Course",
                Quizzes = quizzes,
                TotalScore = quizzesToCount.Sum(q => q.TotalScore),
                ObtainedScore = quizzesToCount.Sum(q => q.IsAttempted ? q.ObtainedScore : 0)
            };

            return View("~/Views/Student/Courses/gradebook.cshtml", model);
        }

    }
}
