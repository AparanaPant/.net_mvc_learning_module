using GraceProject.Data;
using GraceProject.Models;
using GraceProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraceProject.Controllers.Educator
{
    [Route("Educator")]
    public class GradeBookController : Controller
    {
        private readonly GraceDbContext _context;
        public GradeBookController(GraceDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Route("Courses/Gradebook/{courseId}/{sessionId}")]
        public IActionResult Gradebook(string courseId, int sessionId)
        {
            if (_context.Course == null)
            {
                return NotFound("Courses table is not available.");
            }

            var course = _context.Course.FirstOrDefault(c => c.CourseID == courseId);
            var session = _context.Session.FirstOrDefault(s => s.SessionID == sessionId);

            if (course == null || session == null)
            {
                return NotFound("Course or Session not found.");
            }

            var now = DateTime.Now;

            var students = _context.StudentSessions
                .Where(ss => ss.SessionID == sessionId)
                .Select(ss => new StudentQuizResultViewModel
                {
                    StudentId = ss.StudentID,
                    StudentName = _context.Users
                        .Where(u => u.Id == ss.StudentID)
                        .Select(u => u.FirstName)
                        .FirstOrDefault(),
                    Quizzes = _context.Quizzes
                        .Where(q => q.CourseID == courseId || q.SessionID == sessionId)
                        .Select(q => new EducatorQuizResultViewModel
                        {
                            QuizTitle = q.Title,
                            TotalScore = q.TotalScore ?? 0,
                            ObtainedScore = q.UserQuizzes
                                .Where(uq => uq.UserId == ss.StudentID)
                                .Sum(uq => uq.Score) ?? 0,
                            IsAttempted = q.UserQuizzes.Any(uq => uq.UserId == ss.StudentID),
                            DueDate = q.DueDate,
                            IsPastDue = q.DueDate.HasValue && q.DueDate <= now
                        }).ToList()
                }).ToList();

            // Apply filtering logic
            foreach (var student in students)
            {
                var consideredQuizzes = student.Quizzes
                    .Where(q => q.IsAttempted || q.IsPastDue)
                    .ToList();

                student.TotalScore = consideredQuizzes.Sum(q => q.TotalScore);
                student.ObtainedScore = consideredQuizzes.Sum(q => q.IsAttempted ? q.ObtainedScore : 0);
                student.Percentage = student.TotalScore > 0 ? (student.ObtainedScore / student.TotalScore) * 100 : 0;
                student.Grade = GetGrade(student.Percentage);
            }

            var model = new TeacherGradebookViewModel
            {
                CourseTitle = course.Title ?? "Unknown Course",
                SessionTitle = session.IsActive ? "Active Session" : "Inactive Session",
                Students = students
            };

            return View("~/Views/Educator/Courses/Gradebook.cshtml", model);
        }

        // Helper method to determine grade
        private string GetGrade(double percentage)
        {
            if (percentage >= 90) return "A";
            else if (percentage >= 80) return "B";
            else if (percentage >= 70) return "C";
            else if (percentage >= 60) return "D";
            else return "F";
        }
    }
}
